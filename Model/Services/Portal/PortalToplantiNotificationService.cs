using Model.Ortak;
using Model.Portal;
using DAO.Repositories.Portal;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using Utility.Logging;
using Utility.ProjeGlobal;

namespace Model.Services.Portal
{
    public sealed class PortalToplantiMail
    {
        public string Recipient { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string TextBody { get; set; }
        public string Method { get; set; }
    }

    // Sends only durable queue entries, outside the meeting transaction and HTTP request.
    public sealed class PortalToplantiNotificationService
    {
        private const string Footer = "TYS™ Bilgi Sistemleri Kısmı tarafından geliştirilmiştir.";
        private readonly PortalLogContext context;
        private readonly PortalToplantiOutboxRepository repository;
        private readonly PortalToplantiOutboxItem outboxItem;
        private readonly Action<int, Guid, Exception> reportError;
        private readonly List<PortalToplantiMail> plan = new List<PortalToplantiMail>();
        internal PortalToplantiNotificationService(PortalToplantiOutboxRepository repository,
            PortalToplantiOutboxItem item, Action<int, Guid, Exception> reportError)
        {
            this.repository = repository; outboxItem = item; context = item.Context;
            this.reportError = reportError;
        }

        internal bool Send(PortalToplantiChange change)
        {
            if (outboxItem.DeliveryPlan != null)
                return SendPlan(change, PlanSerializer().Deserialize<List<PortalToplantiMail>>(outboxItem.DeliveryPlan));
            bool complete = true;
            var sent = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            Toplanti item = change.After ?? change.Before;
            bool deleted = change.After == null;
            string action = deleted ? "iptal edildi" : change.Before == null ? "oluşturuldu" : "güncellendi";
            var details = MeetingDetails(change);
            string body = HtmlBody(action, details);
            string textBody = TextBody(action, details);
            var ids = change.PreviousParticipants.Select(p => p.KatilimciId)
                .Union(change.Participants.Select(p => p.KatilimciId)).ToArray();
            foreach (int id in ids)
            {
                Attempt(() =>
                {
                    repository.Renew(outboxItem);
                    var old = change.PreviousParticipants.FirstOrDefault(p => p.KatilimciId == id);
                    var current = change.Participants.FirstOrDefault(p => p.KatilimciId == id);
                    bool removed = current == null;
                    bool cancel = old != null && !old.Bilgi && (removed || current.Bilgi);
                    bool invite = current != null && !current.Bilgi;
                    string address = Address(id);
                    string personBody = removed && !deleted ? "<p>Toplantının katılımcı listesinden çıkarıldınız.</p>" + body
                        : current != null && current.Bilgi ? "<p>Bilgi amacıyla gönderilmiştir.</p>" + body : body;
                    string personText = removed && !deleted ? "Toplantının katılımcı listesinden çıkarıldınız.\r\n\r\n" + textBody
                        : current != null && current.Bilgi ? "Bilgi amacıyla gönderilmiştir.\r\n\r\n" + textBody : textBody;
                    AddDelivery(sent, address, "Toplantı " + action + ": " + item.ToplantiKonusu, personBody,
                        personText, cancel ? "CANCEL" : invite ? "REQUEST" : null);
                }, ref complete);
            }
            Attempt(() =>
            {
                repository.Renew(outboxItem);
                string address = Address(item.ToplantiYetkilisi);
                if (!sent.Contains(address))
                {
                    AddDelivery(sent, address, "Toplantı " + action + ": " + item.ToplantiKonusu, body, textBody, null);
                }
            }, ref complete);
            var groups = new List<string> { ProjeConstants.PARAM_TOPLANTI_MAILGRUBU };
            if (item.CevrimIci || (change.Before != null && change.Before.CevrimIci)) groups.Add(ProjeConstants.PARAM_BILGISISTEM_MAILGRUBU);
            if (item.IkramOnayi || (change.Before != null && change.Before.IkramOnayi)) groups.Add(ProjeConstants.PARAM_IKRAM_MAILGRUBU);
            foreach (string key in groups)
            {
                Attempt(() =>
                {
                    repository.Renew(outboxItem);
                    string address = Parameter(key);
                    if (string.IsNullOrWhiteSpace(address)) throw new InvalidOperationException("Toplantı bildirim grubu tanımlı değil: " + key);
                    string groupName = address.Split('@')[0];
                    using (var domain = new PrincipalContext(ContextType.Domain, "TSKGV"))
                    using (var group = GroupPrincipal.FindByIdentity(domain, groupName))
                    {
                        if (group == null) throw new InvalidOperationException("Toplantı bildirim grubu bulunamadı.");
                        using (var members = group.GetMembers())
                        {
                            foreach (Principal member in members)
                            {
                                using (member)
                                {
                                    var user = member as UserPrincipal;
                                    if (user == null) continue;
                                    string recipient = user.EmailAddress;
                                    Attempt(() =>
                                    {
                                        repository.Renew(outboxItem);
                                        if (string.IsNullOrWhiteSpace(recipient)) throw new InvalidOperationException("Bildirim alıcısının e-posta adresi eksik.");
                                        if (sent.Contains(recipient)) return;
                                        AddDelivery(sent, recipient, "Toplantı " + action + ": " + item.ToplantiKonusu, body, textBody, null);
                                    }, ref complete);
                                }
                            }
                        }
                    }
                }, ref complete);
            }
            // Freeze the complete recipient list before any SMTP call. Later AD membership changes cannot
            // remove a failed recipient or add a new recipient halfway through retries of this event.
            if (!complete) return false;
            repository.SavePlan(outboxItem, PlanSerializer().Serialize(plan));
            return SendPlan(change, plan);
        }

        private List<KeyValuePair<string, string>> MeetingDetails(PortalToplantiChange change)
        {
            Toplanti item = change.After ?? change.Before;
            // A deleted meeting no longer has participant rows. Use the queued event snapshot.
            var participants = change.After == null ? change.PreviousParticipants : change.Participants;
            var names = new Dictionary<int, string>();
            string owner = PersonName(item.ToplantiYetkilisi, names);
            var attendees = new List<string>();
            var informed = new List<string>();
            foreach (var participant in participants.OrderBy(p => p.Id))
            {
                string name = PersonName(participant.KatilimciId, names);
                (participant.Bilgi ? informed : attendees).Add(name);
            }
            repository.Renew(outboxItem);
            string coordinator = item.Koordinator > 0 ? repository.CoordinatorName(item.Koordinator) : null;
            if (string.IsNullOrWhiteSpace(coordinator) && item.Koordinator > 0)
                coordinator = "Birim No: " + item.Koordinator.ToString(CultureInfo.InvariantCulture);

            return new List<KeyValuePair<string, string>>
            {
                Detail("Toplantı numarası", item.Id.ToString(CultureInfo.InvariantCulture)),
                Detail("Toplantı konusu", item.ToplantiKonusu),
                Detail("Başlangıç zamanı", item.BaslangicTarihi.ToString("dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture)),
                Detail("Bitiş zamanı", item.BitisTarihi.ToString("dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture)),
                Detail("Toplantı yeri", change.Location),
                Detail("Toplantı yetkilisi", owner),
                Detail("Koordinatör", coordinator),
                Detail("İç katılımcılar", string.Join("\r\n", attendees)),
                Detail("Dış katılımcılar", item.DisKatilimcilar),
                Detail("Bilgi verilecek kişiler", string.Join("\r\n", informed)),
                Detail("Çevrim içi toplantı", item.CevrimIci ? "Evet" : "Hayır"),
                Detail("İkram onayı", item.IkramOnayi ? "Evet" : "Hayır"),
                Detail("Açıklama / toplantı gündemi", item.Aciklama),
                Detail("İkram malzemesi", item.IkramOnayi ? item.IkramMalzemesi : "Yok")
            };
        }

        private string PersonName(int personId, IDictionary<int, string> names)
        {
            if (personId <= 0) return "Belirtilmemiş";
            string name;
            if (!names.TryGetValue(personId, out name))
            {
                repository.Renew(outboxItem);
                name = repository.PersonName(personId);
                if (string.IsNullOrWhiteSpace(name))
                    name = "Personel No: " + personId.ToString(CultureInfo.InvariantCulture);
                names.Add(personId, name);
            }
            return name;
        }

        private static KeyValuePair<string, string> Detail(string label, string value)
        {
            return new KeyValuePair<string, string>(label, string.IsNullOrWhiteSpace(value) ? "Belirtilmemiş" : value);
        }

        private static string HtmlBody(string action, IEnumerable<KeyValuePair<string, string>> details)
        {
            var html = new StringBuilder("<p>Toplantı " + H(action) + ".</p>");
            html.Append("<table style='border-collapse:collapse;width:100%;font-family:Arial,sans-serif;font-size:13px'>")
                .Append("<tr><th colspan='2' style='border:1px solid #999;padding:8px;background-color:#eee;text-align:center'>Toplantı Ayrıntıları</th></tr>");
            foreach (var detail in details)
            {
                html.Append("<tr><th scope='row' style='border:1px solid #999;padding:8px;text-align:left;vertical-align:top;width:30%'>")
                    .Append(H(detail.Key)).Append("</th><td style='border:1px solid #999;padding:8px;vertical-align:top'>")
                    .Append(H(detail.Value).Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "<br />"))
                    .Append("</td></tr>");
            }
            return html.Append("</table><p style='font-family:Arial,sans-serif;font-size:11px;color:#888;margin-top:12px'>")
                .Append(H(Footer)).Append("</p>").ToString();
        }

        private static string TextBody(string action, IEnumerable<KeyValuePair<string, string>> details)
        {
            var text = new StringBuilder("Toplantı " + action + ".\r\n\r\nToplantı Ayrıntıları\r\n");
            foreach (var detail in details)
                text.Append(detail.Key).Append(": ").Append(detail.Value).Append("\r\n\r\n");
            return text.Append(Footer).ToString();
        }
        private string Parameter(string key)
        {
            return repository.Parameter(key);
        }
        private string Address(int personId)
        {
            string address = repository.Address(personId);
            if (string.IsNullOrWhiteSpace(address))
                throw new InvalidOperationException("Toplantı alıcısının e-posta adresi eksik.");
            return address.Trim();
        }
        private void Attempt(Action send, ref bool complete)
        {
            try { send(); }
            catch (System.Threading.ThreadAbortException) { throw; }
            catch (Exception error)
            {
                complete = false;
                if (reportError == null)
                    PortalTechnicalLog.WriteError(context, "Portal.Toplanti.Notification", error);
                else
                    PortalToplantiOutboxDispatcher.ReportFailure(reportError, outboxItem, error);
            }
        }
        private static JavaScriptSerializer PlanSerializer()
        {
            return new JavaScriptSerializer { MaxJsonLength = 16777216 };
        }
        private void AddDelivery(HashSet<string> sent, string address, string subject, string body, string textBody, string method)
        {
            string recipient = new MailAddress(address).Address.Trim();
            if (recipient.Length > 320) throw new InvalidOperationException("Bildirim alıcısının e-posta adresi çok uzun.");
            if (sent.Contains(recipient)) return;
            plan.Add(new PortalToplantiMail { Recipient = recipient, Subject = subject, Body = body,
                TextBody = textBody, Method = method });
            sent.Add(recipient);
        }

        private bool SendPlan(PortalToplantiChange change, List<PortalToplantiMail> deliveries)
        {
            if (deliveries == null || deliveries.Count == 0) throw new InvalidOperationException("Bildirim alıcı planı boş.");
            var sent = repository.Delivered(outboxItem.Id);
            string smtp = Parameter(ProjeConstants.PARAM_SMTP_ADRESI_LBL);
            if (string.IsNullOrWhiteSpace(smtp)) smtp = ProjeConstants.PARAM_ALTERNATIVE_SMTP_IP_ADRESI;
            bool complete = true;
            foreach (PortalToplantiMail delivery in deliveries)
            {
                if (sent.Contains(delivery.Recipient)) continue;
                Attempt(() =>
                {
                    repository.Renew(outboxItem);
                    // Older saved plans contain only the previous paragraph-based HTML body.
                    // Preserve that frozen content instead of rebuilding a partially delivered event.
                    string textBody = delivery.TextBody ?? HttpUtility.HtmlDecode((delivery.Body ?? string.Empty)
                        .Replace("</p>", "\r\n").Replace("<p>", string.Empty));
                    SendMail(smtp, delivery.Recipient, delivery.Subject, delivery.Body, textBody, change.After ?? change.Before,
                        change.Location, delivery.Method, outboxItem.Id, outboxItem.CreatedUtc);
                    repository.MarkDelivered(outboxItem, delivery.Recipient);
                    sent.Add(delivery.Recipient);
                }, ref complete);
            }
            return complete;
        }

        private static void SendMail(string smtp, string address, string subject, string body, string textBody, Toplanti item, string location,
            string method, int sequence, DateTime createdUtc)
        {
            using (var client = new SmtpClient(smtp))
            using (var message = new MailMessage())
            {
                client.Timeout = 30000;
                message.From = new MailAddress("tys@tskgv.org.tr", "Toplantı Yönetim Sistemi");
                message.To.Add(new MailAddress(address));
                message.Subject = subject;
                message.Body = textBody;
                message.BodyEncoding = Encoding.UTF8;
                message.SubjectEncoding = Encoding.UTF8;
                message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(body, Encoding.UTF8, MediaTypeNames.Text.Html));
                if (method != null)
                {
                    var zone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
                    DateTime start = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(item.BaslangicTarihi, DateTimeKind.Unspecified), zone);
                    DateTime end = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(item.BitisTarihi, DateTimeKind.Unspecified), zone);
                    var calendar = new StringBuilder();
                    calendar.Append("BEGIN:VCALENDAR\r\nVERSION:2.0\r\nPRODID:-//TSKGV//PortalApps//TR\r\nMETHOD:").Append(method);
                    calendar.Append("\r\nBEGIN:VEVENT\r\nUID:").Append(item.UniqueId);
                    calendar.Append("\r\nDTSTAMP:").Append(createdUtc.ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture));
                    calendar.Append("\r\nDTSTART:").Append(start.ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture));
                    calendar.Append("\r\nDTEND:").Append(end.ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture));
                    // Reserve the old time-based sequence range so existing invitations accept the new revision.
                    calendar.Append("\r\nSEQUENCE:").Append((1000000000L + sequence).ToString(CultureInfo.InvariantCulture));
                    calendar.Append("\r\nSUMMARY:").Append(Ics(item.ToplantiKonusu));
                    calendar.Append("\r\nLOCATION:").Append(Ics(location));
                    calendar.Append("\r\nDESCRIPTION:").Append(Ics(textBody));
                    // Outlook's HTML calendar description keeps the same table as the email body.
                    calendar.Append("\r\nX-ALT-DESC;FMTTYPE=text/html:").Append(Ics(body));
                    calendar.Append("\r\nORGANIZER:mailto:tys@tskgv.org.tr\r\nATTENDEE;RSVP=TRUE:mailto:").Append(message.To[0].Address);
                    calendar.Append("\r\nSTATUS:").Append(method == "CANCEL" ? "CANCELLED" : "CONFIRMED");
                    calendar.Append("\r\nEND:VEVENT\r\nEND:VCALENDAR\r\n");
                    var type = new ContentType("text/calendar");
                    type.Parameters.Add("method", method); type.Parameters.Add("charset", "UTF-8");
                    message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(FoldCalendar(calendar.ToString()), type));
                }
                client.Send(message);
            }
        }
        private static string H(string value) { return HttpUtility.HtmlEncode(value ?? string.Empty); }

        private static string FoldCalendar(string value)
        {
            // RFC 5545 limits content lines to 75 octets; never split a UTF-8 character.
            var folded = new StringBuilder();
            int lineBytes = 0;
            for (int index = 0; index < value.Length; index++)
            {
                if (value[index] == '\r' && index + 1 < value.Length && value[index + 1] == '\n')
                {
                    folded.Append("\r\n");
                    lineBytes = 0;
                    index++;
                    continue;
                }
                int length = char.IsHighSurrogate(value[index]) && index + 1 < value.Length
                    && char.IsLowSurrogate(value[index + 1]) ? 2 : 1;
                int bytes = Encoding.UTF8.GetByteCount(value.Substring(index, length));
                if (lineBytes + bytes > 75)
                {
                    folded.Append("\r\n ");
                    lineBytes = 1;
                }
                folded.Append(value, index, length);
                lineBytes += bytes;
                index += length - 1;
            }
            return folded.ToString();
        }

        private static string Ics(string value)
        {
            return (value ?? string.Empty).Replace("\\", "\\\\").Replace("\r\n", "\n").Replace("\r", "\n")
                .Replace("\n", "\\n").Replace(";", "\\;").Replace(",", "\\,");
        }
    }
}
