using Model.Ortak;
using Model.Portal;
using Model.Services.IKYS;
using Model.Services.Ortak;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Web;
using Utility.Logging;
using Utility.ProjeGlobal;

namespace Model.Services.Portal
{
    // Called only after the database transaction has committed. Never emits UI messages.
    public sealed class PortalToplantiNotificationService
    {
        private readonly PortalLogContext context;
        public PortalToplantiNotificationService(PortalLogContext context) { this.context = context; }

        public bool Send(PortalToplantiChange change)
        {
            bool complete = true;
            var sent = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            Toplanti item = change.After ?? change.Before;
            bool deleted = change.After == null;
            string action = deleted ? "iptal edildi" : change.Before == null ? "oluşturuldu" : "güncellendi";
            string smtp = Parameter(ProjeConstants.PARAM_SMTP_ADRESI_LBL);
            if (string.IsNullOrWhiteSpace(smtp)) smtp = ProjeConstants.PARAM_ALTERNATIVE_SMTP_IP_ADRESI;
            string location = item.ToplantiYeri == ProjeConstants.PARAM_DIGER_INT ? item.ToplantiYeriDiger
                : (new ToplantiParametreService(context).GetById(item.ToplantiYeri) ?? new ToplantiParametre()).Deger;
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
                    var old = change.PreviousParticipants.FirstOrDefault(p => p.KatilimciId == id);
                    var current = change.Participants.FirstOrDefault(p => p.KatilimciId == id);
                    bool removed = current == null;
                    bool cancel = old != null && !old.Bilgi && (removed || current.Bilgi);
                    bool invite = current != null && !current.Bilgi;
                    string address = Address(id);
                    string personBody = removed && !deleted ? "<p>Toplantının katılımcı listesinden çıkarıldınız.</p>" + body
                        : current != null && current.Bilgi ? "<p>Bilgi amacıyla gönderilmiştir.</p>" + body : body;
                    SendMail(smtp, address, "Toplantı " + action + ": " + item.ToplantiKonusu, personBody,
                        item, location, cancel ? "CANCEL" : invite ? "REQUEST" : null);
                    sent.Add(address);
                }, ref complete);
            }
            Attempt(() =>
            {
                string address = Address(item.ToplantiYetkilisi);
                if (!sent.Contains(address))
                {
                    SendMail(smtp, address, "Toplantı " + action + ": " + item.ToplantiKonusu, body, item, location, null);
                    sent.Add(address);
                }
            }, ref complete);
            var groups = new List<string> { ProjeConstants.PARAM_TOPLANTI_MAILGRUBU };
            if (item.CevrimIci || (change.Before != null && change.Before.CevrimIci)) groups.Add(ProjeConstants.PARAM_BILGISISTEM_MAILGRUBU);
            if (item.IkramOnayi || (change.Before != null && change.Before.IkramOnayi)) groups.Add(ProjeConstants.PARAM_IKRAM_MAILGRUBU);
            foreach (string key in groups)
            {
                Attempt(() =>
                {
                    string address = Parameter(key);
                    if (string.IsNullOrWhiteSpace(address)) throw new InvalidOperationException("Toplantı bildirim grubu tanımlı değil: " + key);
                    string groupName = address.Split('@')[0];
                    using (System.Web.Hosting.HostingEnvironment.Impersonate())
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
                                        if (string.IsNullOrWhiteSpace(recipient)) throw new InvalidOperationException("Bildirim alıcısının e-posta adresi eksik.");
                                        if (sent.Contains(recipient)) return;
                                        SendMail(smtp, recipient, "Toplantı " + action + ": " + item.ToplantiKonusu, body, item, location, null);
                                        sent.Add(recipient);
                                    }, ref complete);
                                }
                            }
                        }
                    }
                }, ref complete);
            }
            return complete;
        }
        private string Parameter(string key)
        {
            var parameter = new OrtakParametreService().GetByKey(key, context);
            return parameter == null ? null : parameter.Deger;
        }
        private string Address(int personId)
        {
            var contact = new IletisimBilgileriService().GetByPersonelId(personId, context);
            if (contact == null || string.IsNullOrWhiteSpace(contact.IntranetEPosta))
                throw new InvalidOperationException("Toplantı alıcısının e-posta adresi eksik.");
            return contact.IntranetEPosta.Trim();
        }
        private void Attempt(Action send, ref bool complete)
        {
            try { send(); }
            catch (Exception error)
            {
                complete = false;
                PortalTechnicalLog.WriteError(context, "Portal.Toplanti.Notification", error);
            }
        }
        private static void SendMail(string smtp, string address, string subject, string body, Toplanti item, string location, string method)
        {
            using (var client = new SmtpClient(smtp))
            using (var message = new MailMessage())
            {
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
                    calendar.Append("\r\nDTSTAMP:").Append(DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture));
                    calendar.Append("\r\nDTSTART:").Append(start.ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture));
                    calendar.Append("\r\nDTEND:").Append(end.ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture));
                    calendar.Append("\r\nSEQUENCE:").Append(((long)(DateTime.UtcNow - new DateTime(2000, 1, 1)).TotalSeconds).ToString(CultureInfo.InvariantCulture));
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
