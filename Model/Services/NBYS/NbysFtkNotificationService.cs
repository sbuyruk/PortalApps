using DAO.Repositories.NBYS;
using Model.Ortak;
using Model.Services.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Mail;
using System.Web;
using Utility.Logging;
using Utility.ProjeGlobal;

namespace Model.Services.NBYS
{
    public enum NbysFtkNotificationResult { AcceptedByServer, NoRecipients }

    public sealed class NbysFtkNotificationService
    {
        private readonly PortalLogContext context;
        private readonly NbysSupportReadRepository repository;

        public NbysFtkNotificationService(PortalLogContext context)
        {
            this.context = context;
            repository = new NbysSupportReadRepository(context);
        }

        public NbysFtkNotificationResult Send(int provinceId, int districtId, bool update, string listUrl)
        {
            Il province = new IlService().GetById(provinceId, context);
            if (province == null) throw new InvalidOperationException("Bildirim ili bulunamadı.");
            Bolge region = new BolgeService().GetById(province.BolgeId, context);
            if (region == null) throw new InvalidOperationException("Bildirim bölgesi bulunamadı.");
            Ilce district = districtId == ProjeConstants.VALILIK_INT ? null : new IlceService().GetById(districtId, context);
            if (districtId != ProjeConstants.VALILIK_INT && (district == null || district.IlId != provinceId))
                throw new InvalidOperationException("Bildirim ilçesi bulunamadı.");
            HashSet<string> recipients = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            DataTable rows = repository.FtkMailRecipients(region.Id);
            if (rows != null)
                foreach (DataRow row in rows.Rows)
                {
                    foreach (string value in Convert.ToString(row["IntranetEPosta"]).Split(';'))
                    {
                        string address = value.Trim();
                        if (address.Length > 0) recipients.Add(address);
                    }
                }
            if (recipients.Count == 0) return NbysFtkNotificationResult.NoRecipients;
            DataTable server = repository.MailServer(ProjeConstants.PARAM_SMTP_ADRESI_LBL);
            string host = server == null ? null : Convert.ToString(server.Rows[0]["Deger"]).Trim();
            if (string.IsNullOrEmpty(host)) host = ProjeConstants.PARAM_ALTERNATIVE_SMTP_IP_ADRESI;
            string subject = province.IlAdi + (district == null ? "" : " / " + district.IlceAdi)
                + (update ? " FTK güncelleme yazısı hazırlandı." : " FTK kuruluş yazısı hazırlandı.");
            using (SmtpClient smtp = new SmtpClient(host))
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress("dgundur@tskgv.local");
                foreach (string address in recipients) mail.To.Add(new MailAddress(address));
                mail.Subject = subject;
                mail.IsBodyHtml = true;
                mail.Body = HttpUtility.HtmlEncode(subject) + "<br/>FTK bilgilerini <a href=\""
                    + HttpUtility.HtmlAttributeEncode(listUrl) + "\">FTK listesinden</a> kontrol edebilirsiniz.";
                // SMTP acceptance is not proof of delivery. Failures propagate to the single UI boundary.
                smtp.Send(mail);
            }
            return NbysFtkNotificationResult.AcceptedByServer;
        }
    }
}
