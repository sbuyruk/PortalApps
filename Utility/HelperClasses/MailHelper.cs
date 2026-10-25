using System;
using System.Net.Mail;
using Utility.ProjeGlobal;

namespace Utility.HelperClasses
{
    public static class MailHelper
    {
        // The caller owns error logging and the user-facing notification.
        public static void EPostaGonderVeyaHataVer(string from, string to, string subject, string body, string smtpAdres)
        {
            if (string.IsNullOrWhiteSpace(to))
                throw new InvalidOperationException("E-posta alıcısı belirlenemedi.");
            using (SmtpClient smtp = new SmtpClient(smtpAdres ?? ProjeConstants.PARAM_ALTERNATIVE_SMTP_IP_ADRESI))
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(from);
                mail.Subject = subject;
                mail.IsBodyHtml = true;
                foreach (string adres in to.Split(new char[] { ';' }))
                {
                    if (!string.IsNullOrWhiteSpace(adres)) mail.To.Add(new MailAddress(adres));
                }
                if (mail.To.Count == 0)
                    throw new InvalidOperationException("E-posta alıcısı belirlenemedi.");
                mail.Body = body;
                smtp.Send(mail);
            }
        }
    }
}
