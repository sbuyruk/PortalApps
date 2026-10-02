using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Model.IKYS;
using Model.Ortak;
using Model.Services.NBYS;
using Model.Services.Ortak;
using System;
using System.Xml.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.NBYS
{
    public class NBYSOrtak
    {
        public NBYSOrtak()
        {

        }
        
        public static string ParametreGetir(string grup,string anahtar)
        {

            NBYSParametre param = new NBYSParametreService().GetByGroupAndKey(grup, anahtar);
            if (param != null)
            {
                return param.Deger;
            }
            else
            {
                return string.Empty;
            }
        }
        public static bool FTKOlusturmaEPostasiGonder(int ilId, int ilceId)
        {
            bool epostaGonderildi = false;
            try
            {
                if (ilId > 0)
                {
                    Il il = new IlService().GetById(ilId);
                    if (il != null)
                    {
                        Bolge bolge = new BolgeService().GetById(il.BolgeId);
                        string userto = "asbuyruk@tskgv.local";
                        string ilstr = il.IlAdi + " ili ";
                        Ilce ilce = new IlceService().GetById(ilceId);
                        string ilcestr = ilce==null?string.Empty:ilce.IlceAdi + " ilçesi ";

                        if (bolge != null && (bolge.Id != ProjeConstants.BOLGE_HEPSI_INT || bolge.Id != ProjeConstants.BOLGE_GENELMUDURLUK_INT))
                        {
                            userto = UserToGetir(bolge);
                            string from = "dgundur@tskgv.local";
                            string subject = bolge.KisaAdi + " bölgesi sorumlulugunda bulunan " + ilstr + ilcestr + " FTK listesi olusturulmustur.";
                            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                            var ftkListesiUrl = "";
                            ftkListesiUrl = string.Format("{0}?BolgeId={1}&IlId={2}&IlceId={3}", currentUrl + "/" + ProjeConstants.PAGE_FTK_LIST, bolge.Id, ilId, ilceId);

                            string userbody = subject + " <br>ilgili FTK bilgilerine ulasmak için "
                                + " Ayrintili bilgi için <a href ='" + ftkListesiUrl + "'>FTK Listesi</a> sayfasina gidebilirsiniz.";

                            string smtpAdresi = UtilityHelper.ParametreDegeriSorgula(ProjeConstants.PARAM_SMTP_ADRESI_LBL);
                            MailHelper.EPostaGonder(from, userto, subject, userbody, smtpAdresi ?? ProjeConstants.PARAM_ALTERNATIVE_SMTP_IP_ADRESI);
                            epostaGonderildi = true;
                        }
                    }
                }
                else
                {
                    epostaGonderildi = false;
                }
            }
            catch (Exception)
            {

                epostaGonderildi = false;
            }
            return epostaGonderildi;
        }

        private static string UserToGetir(Bolge bolge)
        {
            string userTo="##";
            if (bolge != null)
            {
                
                Personel personelDao= new Personel();
                var bolgePersonelList= personelDao.SelectByBolgeId(bolge.Id);
                foreach (var item in bolgePersonelList)
                {
                    IletisimBilgileri iletisimBilgileri = new IletisimBilgileri();
                    iletisimBilgileri=iletisimBilgileri.SelectByPersonelId(item.Id);
                    string eposta = iletisimBilgileri == null ? string.Empty : iletisimBilgileri.IntranetEPosta;
                    userTo += ";"+ eposta ;
                }
            }
            userTo = userTo.Replace("##;","");
            return userTo;
        }
        public static bool YetkiKontrolu(Bolge kullanicininBolgesi, int nakitbagisciId)
        {
            Bolge bagiscininBolgesi = new BolgeService().GetByDonorId(nakitbagisciId);
            if (bagiscininBolgesi != null)
            {

                bool bolgeOk = kullanicininBolgesi.Id == ProjeConstants.HEPSI_INT
                    || kullanicininBolgesi.Id == ProjeConstants.BOLGE_GENELMUDURLUK_INT
                    || bagiscininBolgesi.Id == kullanicininBolgesi.Id ? true : false;
                return bolgeOk;
            }
            return false;
        }
    }
}
