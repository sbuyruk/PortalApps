using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Model.IKYS;
using Model.Ortak;
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

            NBYSParametre param = new NBYSParametre();
            param = param.SelectByGrupAnahtar(grup, anahtar);
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
                    Il il = new Il();
                    il = il.Select<Il>(ilId);
                    if (il != null)
                    {
                        Bolge bolge = new Bolge();
                        bolge = bolge.Select(il.BolgeId);
                        string userto = "asbuyruk@tskgv.local";
                        string ilstr = il.IlAdi + " ili ";
                        Ilce ilce= new Ilce();
                        ilce=ilce.Select<Ilce>(ilceId);
                        string ilcestr = ilce==null?string.Empty:ilce.IlceAdi + " ilçesi ";

                        if (bolge != null && (bolge.Id != ProjeConstants.BOLGE_HEPSI_INT || bolge.Id != ProjeConstants.BOLGE_GENELMUDURLUK_INT))
                        {
                            userto = UserToGetir(bolge);
                            string from = "dgundur@tskgv.local";
                            string subject = bolge.KisaAdi + " bölgesi sorumluluğunda bulunan " + ilstr + ilcestr + " FTK listesi oluşturulmuştur.";
                            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                            var ftkListesiUrl = "";
                            ftkListesiUrl = string.Format("{0}?BolgeId={1}&IlId={2}&IlceId={3}", currentUrl + "/" + ProjeConstants.PAGE_FTK_LIST, bolge.Id, ilId, ilceId);

                            string userbody = subject + " <br>ilgili FTK bilgilerine ulaşmak için "
                                + " Ayrıntılı bilgi için <a href ='" + ftkListesiUrl + "'>FTK Listesi</a> sayfasına gidebilirsiniz.";

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
    }
}
