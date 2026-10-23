using Model.MTS;
using System;
using System.Globalization;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using Utility.ProjeGlobal;

namespace Model.Services.MTS
{
    public static class MtsActivityNotificationService
    {
        // SMTP is outside the business transaction. Failure must remain visible to the caller.
        public static void Send(Faaliyet item, string recipient, bool cancel)
        {
            if (item == null || item.UniqueId == Guid.Empty || string.IsNullOrWhiteSpace(recipient))
                throw new InvalidOperationException("Takvim daveti için faaliyet ve alıcı bilgisi gereklidir.");
            var zone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
            DateTime start = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(item.BaslangicTarihi, DateTimeKind.Unspecified), zone);
            DateTime end = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(item.BitisTarihi, DateTimeKind.Unspecified), zone);
            string method = cancel ? "CANCEL" : "REQUEST";
            var calendar = new StringBuilder("BEGIN:VCALENDAR\r\nVERSION:2.0\r\nPRODID:-//TSKGV//MTS//TR\r\nMETHOD:");
            calendar.Append(method).Append("\r\nBEGIN:VEVENT\r\nUID:").Append(item.UniqueId);
            calendar.Append("\r\nDTSTAMP:").Append(DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture));
            calendar.Append("\r\nDTSTART:").Append(start.ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture));
            calendar.Append("\r\nDTEND:").Append(end.ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture));
            calendar.Append("\r\nSEQUENCE:").Append(((long)(DateTime.UtcNow - new DateTime(2000, 1, 1)).TotalSeconds).ToString(CultureInfo.InvariantCulture));
            calendar.Append("\r\nORGANIZER:mailto:").Append(new MailAddress(ProjeConstants.PARAM_MTSMAILADRESI).Address);
            calendar.Append("\r\nATTENDEE:mailto:").Append(new MailAddress(recipient).Address);
            calendar.Append("\r\nSUMMARY:").Append(Escape(item.FaaliyetKonusu));
            calendar.Append("\r\nLOCATION:").Append(Escape(item.FaaliyetYeriStr));
            calendar.Append("\r\nDESCRIPTION:").Append(Escape(item.Aciklama));
            calendar.Append("\r\nSTATUS:").Append(cancel ? "CANCELLED" : "CONFIRMED").Append("\r\nEND:VEVENT\r\nEND:VCALENDAR\r\n");
            using (var client = new SmtpClient(ProjeConstants.PARAM_INTERNET_SMTP_IP_ADRESI))
            using (var message = new MailMessage())
            {
                message.From = new MailAddress(ProjeConstants.PARAM_MTSMAILADRESI);
                message.To.Add(new MailAddress(recipient));
                message.Subject = (cancel ? "Faaliyet iptali: " : "Faaliyet: ") + (item.FaaliyetKonusu ?? string.Empty).Replace("\r", " ").Replace("\n", " ");
                message.SubjectEncoding = message.BodyEncoding = Encoding.UTF8;
                message.IsBodyHtml = false;
                message.Body = (cancel ? "Faaliyet takvimden kaldırılacaktır.\r\n" : string.Empty) + item.FaaliyetKonusu + "\r\n" + item.Aciklama;
                var type = new ContentType("text/calendar"); type.Parameters.Add("method", method); type.CharSet = "utf-8";
                message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(calendar.ToString(), type));
                client.Send(message);
            }
        }
        private static string Escape(string value)
        {
            return (value ?? string.Empty).Replace("\\", "\\\\").Replace("\r\n", "\n").Replace("\r", "\n")
                .Replace("\n", "\\n").Replace(";", "\\;").Replace(",", "\\,");
        }
    }
}
