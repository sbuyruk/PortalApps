using System;
using System.Net.Mail;
using System.Text;
using Utility.ProjeGlobal;

namespace Utility.HelperClasses
{
    public static class MailHelper
    {
        
        /// <summary>
        /// Eposta gönderir
        /// </summary>
        public static void EPostaGonder(string from, string to, string subject, string body,string smptpAdres)
        {
            if (string.IsNullOrEmpty(to))
            {
                MessageHelper.PublishMessage("Gidecegi adres yok", ProjeConstants.MESAJ_HATA);
            }
            else
            {
                try
                {

                    SmtpClient smtp = new SmtpClient(smptpAdres ?? ProjeConstants.PARAM_ALTERNATIVE_SMTP_IP_ADRESI);
                    MailMessage mail = new MailMessage();

                    mail.From = new MailAddress(from);
                    mail.Subject = subject;
                    mail.IsBodyHtml = true;
                    string[] toList= to.Split(new char[] { ';' });
                    foreach (var item in toList)
                    {
                        mail.To.Add(new MailAddress(item)); 
                    }

                    mail.Body = body;
                    smtp.Send(mail);

                    //MessageHelper.PublishMessage(" E-Posta gönderildi ;) : to= " + to + " konu=" + subject, ProjeConstants.MESAJ_BILGI);
                    //Console.WriteLine(" E-Posta gönderildi ;) : to= " + to + " konu=" + subject);
                }
                catch (System.Exception exception)
                {

                    ExceptionHelper eh = new ExceptionHelper(exception);
                    eh.PublishException();
                } 
            }
        }
        public static void TakvimeEkle(Guid uniqueId, string from, string to, string title, DateTime startTime, DateTime endTime, string location, string desc, string smptpAdres)
        {
            SmtpClient smtp = new SmtpClient(smptpAdres ?? ProjeConstants.PARAM_ALTERNATIVE_SMTP_IP_ADRESI);
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(from);
            mail.Subject = title;
            mail.IsBodyHtml = true;
            mail.To.Add(new MailAddress(to));

            AlternateView m_calV = CreateCalenderObject(uniqueId, title, startTime, endTime, location, desc, mail, "REQUEST", "CONFIRMED", 0);
            mail.AlternateViews.Add(m_calV);
            smtp.Send(mail);

            //MessageHelper.PublishMessage(" Takvime ekle gönderildi ;) : to= " + to + " konu=" + title, ProjeConstants.MESAJ_BILGI);
            //Console.WriteLine(" Takvime ekle gönderildi ;) : to= " + to + " konu=" + title);
        }
        public static void TakvimdenSil(Guid uniqueId, string from, string to, string title, DateTime startTime, DateTime endTime, string location, string desc,string smtpAdres)
        {
            try
            {
                title = "Iptal Edildi : " + title;
                string desc2 = desc + "\n\n Toplanti iptal edilmistir. Lütfen takviminizden kaldirmak için 'Takvimden Kaldir' butonuna tiklayiniz.";// UtilityHelper.parametreDegeriSorgula("Toplanti Takvimden Kaldirma Mesaji");

                SmtpClient smtp = new SmtpClient(string.IsNullOrEmpty(smtpAdres) ? ProjeConstants.PARAM_ALTERNATIVE_SMTP_IP_ADRESI : smtpAdres);
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(from);//new MailAddress(parametreDegeriSorgula("Sistem Hesabi") + parametreDegeriSorgula("EPosta Domain Uzantisi"));
                mail.Subject = title;
                mail.IsBodyHtml = true;
                mail.To.Add(new MailAddress(to));
                mail.Body = desc2;

                AlternateView m_calV = CreateCalenderObject(uniqueId, title, startTime, endTime, location, desc, mail, "CANCEL", "CANCELLED", 1);

                mail.AlternateViews.Add(m_calV);
                smtp.Send(mail);
                //MessageHelper.PublishMessage(" Takvimden sil gönderildi ;) : to= " + to + " konu=" + title, ProjeConstants.MESAJ_BILGI);
            }
            catch (Exception ex)
            {
                LogHelper.WriteTrace(ex);
            }
        }
        public static AlternateView CreateCalenderObject(Guid uniqueId,string title, DateTime startTime, DateTime endTime, string location, string desc, MailMessage mail, string method, string status, int sequenceNumber)
        {
            AlternateView m_calV = null;
            try
            {
                System.Net.Mime.ContentType typeC = new System.Net.Mime.ContentType("text/calendar");
                typeC.Parameters.Add("method", method);
                typeC.Parameters.Add("name", "meeting.ics");
                StringBuilder str = new StringBuilder();
                str.AppendLine("BEGIN:VCALENDAR");

                str.AppendLine("PRODID:-//TSKGV//Outlook MIMEDIR//EN");
                str.AppendLine("VERSION:2.0");
                str.AppendLine(string.Format("METHOD:{0}", method));

                str.AppendLine("BEGIN:VEVENT");

                str.AppendLine(string.Format("DTSTART:{0:yyyyMMddTHHmmssZ}", startTime.AddHours(-3)));
                str.AppendLine(string.Format("DTSTAMP:{0:yyyyMMddTHHmmssZ}", DateTime.UtcNow));
                str.AppendLine(string.Format("DTEND:{0:yyyyMMddTHHmmssZ}", endTime.AddHours(-3)));
                str.AppendLine(string.Format("LOCATION: {0}", location));
                str.AppendLine(string.Format("UID:{0}", uniqueId));
                str.AppendLine(string.Format("DESCRIPTION:{0}", desc));
                str.AppendLine(string.Format("X-ALT-DESC;FMTTYPE=text/html:{0}", desc));
                str.AppendLine(string.Format("SUMMARY:{0}", title));
                if (sequenceNumber == 1)
                {
                    str.AppendLine(string.Format("SEQUENCE:{0}", sequenceNumber));
                }
                str.AppendLine(string.Format("STATUS:{0}", status));
                str.AppendLine("BEGIN:VALARM");
                str.AppendLine(string.Format("TRIGGER:-PT{0}M", "15"));
                str.AppendLine("ACTION:DISPLAY");
                str.AppendLine("DESCRIPTION:Reminder");
                str.AppendLine("END:VALARM");
                str.AppendLine("END:VEVENT");


                str.AppendLine(string.Format("ORGANIZER:MAILTO:{0}", mail.From.Address));
                str.AppendLine(string.Format("ATTENDEE;CN=\"{0}\";RSVP=TRUE:mailto:{1}", mail.To[0].DisplayName, mail.To[0].Address));

                str.AppendLine("END:VCALENDAR");

                m_calV = AlternateView.CreateAlternateViewFromString(str.ToString(), typeC);
            }
            catch (Exception ex)
            {
                //Logging.WriteTrace(ex);
                throw;
            }

            return m_calV;
        }
        public static string BodyOlustur(string title, string body, string contentId)
        {
            StringBuilder govde = new StringBuilder();
            govde.Append("<html xmlns=\"http://www.w3.org/1999/xhtml\">");
            govde.Append("<head>");
            govde.Append("<meta http-equiv=\"Content-Language\" content=\"tr\" />");
            govde.Append("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />");
            govde.Append("<title>" + title + "</title>");
            govde.Append("<style type=\"text/css\">");
            govde.Append(".style1 {");
            govde.Append("	font-family: Calibri;");
            govde.Append("	font-weight: Bold;");
            govde.Append("	font-size: 12px;");
            govde.Append("	border: 1px solid #808080;");
            govde.Append("	background-color: #EEEEEE;");
            govde.Append("}");
            govde.Append(".altborder {");
            govde.Append("	border-bottom-style: solid;");
            govde.Append("	border-bottom-width: 1px;");
            govde.Append("	border-bottom-color: #808080;");
            govde.Append("}");
            govde.Append(".style2 {");
            govde.Append("	font-family: Calibri;");
            govde.Append("	font-size: 12px;");
            govde.Append("	border: 1px solid #808080;");
            govde.Append("}");
            govde.Append(".style3 {");
            govde.Append("	font-family: Calibri;");
            govde.Append("	font-size: 12px;");
            govde.Append("	border: 1px solid #808080;");
            govde.Append("	background-color: #F4F4F4;");
            govde.Append("}");
            govde.Append(".style4 {");
            govde.Append("	border-bottom: 1px solid #808080;");
            govde.Append("	font-family: Calibri;");
            govde.Append("	font-size: 12px;");
            govde.Append("}");
            govde.Append("</style>");
            govde.Append("</head>");
            govde.Append("<body>");
            govde.Append("<table style=\"width: 100%\" cellspacing=\"0\" cellpadding=\"4\">");
            govde.Append("	<tr>");
            govde.Append("	<td class=\"style2\">");
            // govde.Append("<img id=\"imgLogo\" src=\"" + rootUrl + "/Style Library/_i/" + parametreDegeriSorgula("Eposta Logo Dosya Adi") + "\"/>");
            govde.Append("<img id=\"imgLogo\" src=\"cid:" + contentId + "\" style=\"border-color: Silver;\"\n");
            govde.Append(" border-width: 2px\";/>");
            govde.Append("</td>");
            govde.Append("	</tr>");
            govde.Append("	<tr>");
            govde.Append("	<td class=\"style1\">" + title + "</td>");
            govde.Append("	</tr>");
            govde.Append("	<tr>");
            govde.Append("	<td class=\"style4\">");
            govde.Append(body);
            govde.Append("	</td>");
            govde.Append("	</tr>");
            govde.Append("</table>");
            govde.Append("</body>");
            govde.Append("</html>");
            return govde.ToString();
        }
        
    }
}
