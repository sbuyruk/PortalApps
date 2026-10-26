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
        public string Method { get; set; }
    }

    // Sends only durable queue entries, outside the meeting transaction and HTTP request.
    public sealed class PortalToplantiNotificationService
    {
        private readonly PortalLogContext context;
        private readonly PortalToplantiOutboxRepository repository;
        private readonly PortalToplantiOutboxItem outboxItem;
        private readonly List<PortalToplantiMail> plan = new List<PortalToplantiMail>();
        internal PortalToplantiNotificationService(PortalToplantiOutboxRepository repository, PortalToplantiOutboxItem item)
        {
            this.repository = repository; outboxItem = item; context = item.Context;
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
            string location = change.Location;
            string body = "<p>Toplantı " + action + ".</p><p>" + H(item.ToplantiKonusu) + "</p><p>"
                + item.BaslangicTarihi.ToString("dd.MM.yyyy HH:mm") + " – " + item.BitisTarihi.ToString("dd.MM.yyyy HH:mm")
                + "</p><p>Yer: " + H(location) + "</p><p>" + H(item.Aciklama) + "</p><p>Dış katılımcılar: "
                + H(item.DisKatilimcilar) + "</p><p>Çevrim içi: " + (item.CevrimIci ? "Evet" : "Hayır")
                + "</p><p>İkram: " + (item.IkramOnayi ? H(item.IkramMalzemesi) : "Yok") + "</p>";
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
                    AddDelivery(sent, address, "Toplantı " + action + ": " + item.ToplantiKonusu, personBody,
                        cancel ? "CANCEL" : invite ? "REQUEST" : null);
                }, ref complete);
            }
            Attempt(() =>
            {
                repository.Renew(outboxItem);
                string address = Address(item.ToplantiYetkilisi);
                if (!sent.Contains(address))
                {
                    AddDelivery(sent, address, "Toplantı " + action + ": " + item.ToplantiKonusu, body, null);
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
                                        AddDelivery(sent, recipient, "Toplantı " + action + ": " + item.ToplantiKonusu, body, null);
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
                PortalTechnicalLog.WriteError(context, "Portal.Toplanti.Notification", error);
            }
        }
        private static JavaScriptSerializer PlanSerializer()
        {
            return new JavaScriptSerializer { MaxJsonLength = 16777216 };
        }
        private void AddDelivery(HashSet<string> sent, string address, string subject, string body, string method)
        {
            string recipient = new MailAddress(address).Address.Trim();
            if (recipient.Length > 320) throw new InvalidOperationException("Bildirim alıcısının e-posta adresi çok uzun.");
            if (sent.Contains(recipient)) return;
            plan.Add(new PortalToplantiMail { Recipient = recipient, Subject = subject, Body = body, Method = method });
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
                    SendMail(smtp, delivery.Recipient, delivery.Subject, delivery.Body, change.After ?? change.Before,
                        change.Location, delivery.Method, outboxItem.Id, outboxItem.CreatedUtc);
                    repository.MarkDelivered(outboxItem, delivery.Recipient);
                    sent.Add(delivery.Recipient);
                }, ref complete);
            }
            return complete;
        }

        private static void SendMail(string smtp, string address, string subject, string body, Toplanti item, string location,
            string method, int sequence, DateTime createdUtc)
        {
            using (var client = new SmtpClient(smtp))
            using (var message = new MailMessage())
            {
                client.Timeout = 30000;
                message.From = new MailAddress("tys@tskgv.local", "Toplantı Yönetim Sistemi");
                message.To.Add(new MailAddress(address));
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;
                message.BodyEncoding = Encoding.UTF8;
                message.SubjectEncoding = Encoding.UTF8;
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
                    calendar.Append("\r\nDESCRIPTION:").Append(Ics(item.Aciklama));
                    calendar.Append("\r\nORGANIZER:mailto:tys@tskgv.local\r\nATTENDEE;RSVP=TRUE:mailto:").Append(message.To[0].Address);
                    calendar.Append("\r\nSTATUS:").Append(method == "CANCEL" ? "CANCELLED" : "CONFIRMED");
                    calendar.Append("\r\nEND:VEVENT\r\nEND:VCALENDAR\r\n");
                    var type = new ContentType("text/calendar");
                    type.Parameters.Add("method", method); type.Parameters.Add("charset", "UTF-8");
                    message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(calendar.ToString(), type));
                }
                client.Send(message);
            }
        }
        private static string H(string value) { return HttpUtility.HtmlEncode(value ?? string.Empty); }
        private static string Ics(string value)
        {
            return (value ?? string.Empty).Replace("\\", "\\\\").Replace("\r\n", "\n").Replace("\r", "\n")
                .Replace("\n", "\\n").Replace(";", "\\;").Replace(",", "\\,");
        }
    }
}
