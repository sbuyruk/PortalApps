using Model.NBYS;
using Model.Ortak;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.BolgelereGoreFTKRaporuWP
{
    [ToolboxItemAttribute(false)]
    public partial class BolgelereGoreFTKRaporuWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public BolgelereGoreFTKRaporuWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                KurulusTarihiTxt.Text = (new DateTime(2017, 01, 01)).ConvertToDatetimeEmptyIfNull();
                GuncellemeTarihiTxt.Text = (new DateTime(2017, 01, 01)).ConvertToDatetimeEmptyIfNull();
                
            }
            TabloyuDoldur();
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
                Page.Response.Redirect(newUrl);
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }

        protected void FTKGuncellemeTarihiTxt_TextChanged(object sender, EventArgs e)
        {
            TabloyuDoldur();
        }

        private int gmIl, gmIlce, istIl, istIlce, izmIl, izmIlce, merIl, merIlce, toplamIl, toplamIlce;
        private int gmKOIl, gmKOIlce, istKOIl, istKOIlce, izmKOIl, izmKOIlce, merKOIl, merKOIlce, toplamKOIl, toplamKOIlce;
        private int gmKOlmayanIl, gmKOlmayanIlce, istKOlmayanIl, istKOlmayanIlce, izmKOlmayanIl, izmKOlmayanIlce, merKOlmayanIl, merKOlmayanIlce, toplamKOlmayanIl, toplamKOlmayanIlce;
        private int gmGuncellenen, istGuncellenen, izmGuncellenen, merGuncellenen, toplamGuncellenen;
        private int gmYeniKurulan, istYeniKurulan, izmYeniKurulan, merYeniKurulan, toplamYeniKurulan;

        private const string KURULU_ILLER = "KURULU_ILLER";
        private const string KURULU_ILCELER = "KURULU_ILCELER";
        private const string KURULU_IL_VE_ILCELER = "KURULU_IL_VE_ILCELER";
        private const string YENI_KURULAN = "YENI_KURULAN";
        private const string GUNCELLENEN = "GUNCELLENEN";
        private const string KURULUOLMAYAN_ILLER = "KURULUOLMAYAN_ILLER";
        private const string KURULUOLMAYAN_ILCELER = "KURULUOLMAYAN_ILCELER";
        private const string KURULUM_ORANI = "KURULUM_ORANI";
        private const string GUNCELLEME_DURUMU = "GUNCELLEME_DURUMU";
        private IFormatProvider cultureInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);

        private void TabloyuDoldur()
        {

            IlSayilariniDoldur();
            IlceSayilariniDoldur();
            KuruluIlSayilariniDoldur();
            KuruluIlceSayilariniDoldur();
            KOlmayanIlSayisiniDoldur();
            KOlmayanIlceSayisiniDoldur();
            GuncellenenIlIlceSayisiniDoldur();
            YeniKurulanIlIlceSayisiniDoldur();
            KOlmayanIlSayisiniDoldur();
            KOlmayanIlceSayisiniDoldur();
            KurulumOraniniDoldur();
            GuncellemeDurumuDoldur();
        }
        
        private int IlSayisiGetir(string bolge)
        {
            Il il = new Il();
            int ilSayisi = il.SelectCountIlByBolge(bolge);
            return ilSayisi;
        }
        private int IlceSayisiGetir(string bolge)
        {
            Ilce ilce = new Ilce();
            int ilceSayisi = ilce.SelectCountIlceByBolge(bolge);
            return ilceSayisi;
        }
        private void KuruluIlSayilariniDoldur()
        {
            gmKOIl = KOIlSayisiGetir(ProjeConstants.BOLGE_GENELMUDURLUK);
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, GMKOIlCell, gmKOIl, ProjeConstants.BOLGE_GENELMUDURLUK, KURULU_ILLER);
            istKOIl = KOIlSayisiGetir(ProjeConstants.BOLGE_ISTANBUL);
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, IstKOIlCell, istKOIl, ProjeConstants.BOLGE_ISTANBUL, KURULU_ILLER);
            izmKOIl = KOIlSayisiGetir(ProjeConstants.BOLGE_IZMIR);
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, IzmKOIlCell, izmKOIl, ProjeConstants.BOLGE_IZMIR, KURULU_ILLER);
            merKOIl = KOIlSayisiGetir(ProjeConstants.BOLGE_MERSIN);
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, MerKOIlCell, merKOIl, ProjeConstants.BOLGE_MERSIN, KURULU_ILLER);
            toplamKOIl = gmKOIl + istKOIl + izmKOIl + merKOIl;
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, TopKOIlCell, toplamKOIl, ProjeConstants.BOLGE_HEPSI, KURULU_ILLER);
        }
        private void KuruluIlceSayilariniDoldur()
        {
            gmKOIlce = KOIlceSayisiGetir(ProjeConstants.BOLGE_GENELMUDURLUK);
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, GMKOIlceCell, gmKOIlce, ProjeConstants.BOLGE_GENELMUDURLUK, KURULU_ILCELER);
            istKOIlce = KOIlceSayisiGetir(ProjeConstants.BOLGE_ISTANBUL);
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, IstKOIlceCell, istKOIlce, ProjeConstants.BOLGE_ISTANBUL, KURULU_ILCELER);
            izmKOIlce = KOIlceSayisiGetir(ProjeConstants.BOLGE_IZMIR);
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, IzmKOIlceCell, izmKOIlce, ProjeConstants.BOLGE_IZMIR, KURULU_ILCELER);
            merKOIlce = KOIlceSayisiGetir(ProjeConstants.BOLGE_MERSIN);
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, MerKOIlceCell, merKOIlce, ProjeConstants.BOLGE_MERSIN, KURULU_ILCELER);
            toplamKOIlce = gmKOIlce + istKOIlce + izmKOIlce + merKOIlce;
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, TopKOIlceCell, toplamKOIlce, ProjeConstants.BOLGE_HEPSI, KURULU_ILCELER);
        }
        private void IlSayilariniDoldur()
        {
            GMSBIlCell.Text = (gmIl = IlSayisiGetir(ProjeConstants.BOLGE_GENELMUDURLUK)).ToString();
            IstSBIlCell.Text = (istIl = IlSayisiGetir(ProjeConstants.BOLGE_ISTANBUL)).ToString();
            IzmSBIlCell.Text = (izmIl = IlSayisiGetir(ProjeConstants.BOLGE_IZMIR)).ToString();
            MerSBIlCell.Text = (merIl = IlSayisiGetir(ProjeConstants.BOLGE_MERSIN)).ToString();
            TopSBIlCell.Text = (toplamIl=gmIl + istIl + izmIl + merIl).ToString();
        }
        private void IlceSayilariniDoldur()
        {
            GMSBIlceCell.Text = (gmIlce = IlceSayisiGetir(ProjeConstants.BOLGE_GENELMUDURLUK)).ToString();
            IstSBIlceCell.Text = (istIlce = IlceSayisiGetir(ProjeConstants.BOLGE_ISTANBUL)).ToString();
            IzmSBIlceCell.Text = (izmIlce = IlceSayisiGetir(ProjeConstants.BOLGE_IZMIR)).ToString();
            MerSBIlceCell.Text = (merIlce = IlceSayisiGetir(ProjeConstants.BOLGE_MERSIN)).ToString();
            TopSBIlceCell.Text = (toplamIlce=gmIlce + istIlce + izmIlce + merIlce).ToString();
        }
        private int KOIlSayisiGetir(string bolge)
        {
            FTK ftk = new FTK();
            int ilSayisi = ftk.SelectKuruluOlanIlSayisiByBolge(bolge);
            return ilSayisi;
        }
        private int KOIlceSayisiGetir(string bolge)
        {
            FTK ftk = new FTK();
            int ilSayisi = ftk.SelectKuruluOlanIlceSayisiByBolge(bolge);
            return ilSayisi;
        }
        private void GuncellenenIlIlceSayisiniDoldur()
        {
            FTK ftk = new FTK();
            gmGuncellenen = ftk.SelectGuncellenenIlIlceSayisiByBolge(ProjeConstants.BOLGE_GENELMUDURLUK, GuncellemeTarihiTxt.Text.ConvertToDatetime());
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, GMGuncellenenCell, gmGuncellenen, ProjeConstants.BOLGE_GENELMUDURLUK, GUNCELLENEN);
            istGuncellenen = ftk.SelectGuncellenenIlIlceSayisiByBolge(ProjeConstants.BOLGE_ISTANBUL, GuncellemeTarihiTxt.Text.ConvertToDatetime());
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, IstGuncellenenCell, istGuncellenen, ProjeConstants.BOLGE_ISTANBUL, GUNCELLENEN);
            izmGuncellenen = ftk.SelectGuncellenenIlIlceSayisiByBolge(ProjeConstants.BOLGE_IZMIR, GuncellemeTarihiTxt.Text.ConvertToDatetime());
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, IzmGuncellenenCell, izmGuncellenen, ProjeConstants.BOLGE_IZMIR, GUNCELLENEN);
            merGuncellenen = ftk.SelectGuncellenenIlIlceSayisiByBolge(ProjeConstants.BOLGE_MERSIN, GuncellemeTarihiTxt.Text.ConvertToDatetime());
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, MerGuncellenenCell, merGuncellenen, ProjeConstants.BOLGE_MERSIN, GUNCELLENEN);
            toplamGuncellenen = gmGuncellenen + istGuncellenen + izmGuncellenen + merGuncellenen;
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, TopGuncellenenCell, toplamGuncellenen, ProjeConstants.BOLGE_HEPSI, GUNCELLENEN);
        }
        private void YeniKurulanIlIlceSayisiniDoldur()
        {
            FTK ftk = new FTK();
            gmYeniKurulan = ftk.SelectKuruluIlIlceSayisiByBolge(ProjeConstants.BOLGE_GENELMUDURLUK, KurulusTarihiTxt.Text.ConvertToDatetime());
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, GMYeniKurulanCell, gmYeniKurulan, ProjeConstants.BOLGE_GENELMUDURLUK, YENI_KURULAN);
            istYeniKurulan = ftk.SelectKuruluIlIlceSayisiByBolge(ProjeConstants.BOLGE_ISTANBUL, KurulusTarihiTxt.Text.ConvertToDatetime());
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, IstYeniKurulanCell, istYeniKurulan, ProjeConstants.BOLGE_ISTANBUL, YENI_KURULAN);
            izmYeniKurulan = ftk.SelectKuruluIlIlceSayisiByBolge(ProjeConstants.BOLGE_IZMIR, KurulusTarihiTxt.Text.ConvertToDatetime());
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, IzmYeniKurulanCell, izmYeniKurulan, ProjeConstants.BOLGE_IZMIR, YENI_KURULAN);
            merYeniKurulan = ftk.SelectKuruluIlIlceSayisiByBolge(ProjeConstants.BOLGE_MERSIN, KurulusTarihiTxt.Text.ConvertToDatetime());
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, MerYeniKurulanCell, merYeniKurulan, ProjeConstants.BOLGE_MERSIN, YENI_KURULAN);
            toplamYeniKurulan = gmYeniKurulan + istYeniKurulan + izmYeniKurulan + merYeniKurulan;
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, TopYeniKurulanCell, toplamYeniKurulan, ProjeConstants.BOLGE_HEPSI, YENI_KURULAN);
        }
        private void KOlmayanIlSayisiniDoldur()
        {
            gmKOlmayanIl = gmIl - gmKOIl;
            HyperLinkEkle(ProjeConstants.PAGE_FTKKURULU_OLMAYAN_ILILCE_LIST, GMKOlmayanIlCell, gmKOlmayanIl, ProjeConstants.BOLGE_GENELMUDURLUK, KURULUOLMAYAN_ILLER);
            istKOlmayanIl = istIl - istKOIl;
            HyperLinkEkle(ProjeConstants.PAGE_FTKKURULU_OLMAYAN_ILILCE_LIST, IstKOlmayanIlCell, istKOlmayanIl, ProjeConstants.BOLGE_ISTANBUL, KURULUOLMAYAN_ILLER);
            izmKOlmayanIl = izmIl - izmKOIl;
            HyperLinkEkle(ProjeConstants.PAGE_FTKKURULU_OLMAYAN_ILILCE_LIST, IzmKOlmayanIlCell, izmKOlmayanIl, ProjeConstants.BOLGE_IZMIR, KURULUOLMAYAN_ILLER);
            merKOlmayanIl = merIl - merKOIl;
            HyperLinkEkle(ProjeConstants.PAGE_FTKKURULU_OLMAYAN_ILILCE_LIST, MerKOlmayanIlCell, merKOlmayanIl, ProjeConstants.BOLGE_MERSIN, KURULUOLMAYAN_ILLER);
            toplamKOlmayanIl = gmKOlmayanIl + istKOlmayanIl + izmKOlmayanIl + merKOlmayanIl;
            HyperLinkEkle(ProjeConstants.PAGE_FTKKURULU_OLMAYAN_ILILCE_LIST, TopKOlmayanIlCell, toplamKOlmayanIl, ProjeConstants.BOLGE_HEPSI, KURULUOLMAYAN_ILLER);

        }
        private void KOlmayanIlceSayisiniDoldur()
        {
            gmKOlmayanIlce = gmIlce - gmKOIlce;
            HyperLinkEkle(ProjeConstants.PAGE_FTKKURULU_OLMAYAN_ILILCE_LIST, GMKOlmayanIlceCell, gmKOlmayanIlce, ProjeConstants.BOLGE_GENELMUDURLUK, KURULUOLMAYAN_ILCELER);
            istKOlmayanIlce = istIlce - istKOIlce;
            HyperLinkEkle(ProjeConstants.PAGE_FTKKURULU_OLMAYAN_ILILCE_LIST, IstKOlmayanIlceCell, istKOlmayanIlce, ProjeConstants.BOLGE_ISTANBUL, KURULUOLMAYAN_ILCELER);
            izmKOlmayanIlce = izmIlce - izmKOIlce;
            HyperLinkEkle(ProjeConstants.PAGE_FTKKURULU_OLMAYAN_ILILCE_LIST, IzmKOlmayanIlceCell, izmKOlmayanIlce, ProjeConstants.BOLGE_IZMIR, KURULUOLMAYAN_ILCELER);
            merKOlmayanIlce = merIlce - merKOIlce;
            HyperLinkEkle(ProjeConstants.PAGE_FTKKURULU_OLMAYAN_ILILCE_LIST, MerKOlmayanIlceCell, merKOlmayanIlce, ProjeConstants.BOLGE_MERSIN, KURULUOLMAYAN_ILCELER);
            toplamKOlmayanIlce = gmKOlmayanIlce + istKOlmayanIlce + izmKOlmayanIlce + merKOlmayanIlce;
            HyperLinkEkle(ProjeConstants.PAGE_FTKKURULU_OLMAYAN_ILILCE_LIST, TopKOlmayanIlceCell, toplamKOlmayanIlce, ProjeConstants.BOLGE_HEPSI, KURULUOLMAYAN_ILCELER);
        }

        private void KurulumOraniniDoldur()
        {
            decimal gmKuruluIlIlceOrani = (gmIl + gmIlce) == 0 ? 0 : ((decimal)(gmKOIl + gmKOIlce) / (gmIl + gmIlce)) * 100;
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, GMKurulumOraniCell, gmKuruluIlIlceOrani, ProjeConstants.BOLGE_GENELMUDURLUK, KURULUM_ORANI);
            decimal istKuruluIlIlceOrani = (istIl + istIlce) == 0 ? 0 : ((decimal)(istKOIl + istKOIlce) / (istIl + istIlce)) * 100;
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, IstKurulumOraniCell, istKuruluIlIlceOrani, ProjeConstants.BOLGE_ISTANBUL, KURULUM_ORANI);

            decimal izmKuruluIlIlceOrani = (izmIl + izmIlce) == 0 ? 0 : ((decimal)(izmKOIl + izmKOIlce) / (izmIl + izmIlce)) * 100;
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, IzmKurulumOraniCell, izmKuruluIlIlceOrani, ProjeConstants.BOLGE_IZMIR, KURULUM_ORANI);
            decimal merKuruluIlIlceOrani = (decimal)(merIl + merIlce) == 0 ? 0 : ((decimal)(merKOIl + merKOIlce) / (merIl + merIlce)) * 100;
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, MerKurulumOraniCell, merKuruluIlIlceOrani, ProjeConstants.BOLGE_MERSIN, KURULUM_ORANI);
            decimal topKuruluIlIlceOrani = (toplamIl + toplamIlce) == 0 ? 0 : ((decimal)(toplamKOIl + toplamKOIlce) / (toplamIl + toplamIlce)) * 100;
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, TopKurulumOraniCell, topKuruluIlIlceOrani, ProjeConstants.BOLGE_HEPSI, KURULUM_ORANI);
        }
        private void GuncellemeDurumuDoldur()
        {
            decimal gmGuncellenenIlIlceOrani = (gmIl + gmIlce) == 0 ? 0 : ((decimal)(gmGuncellenen) / (gmIl + gmIlce)) * 100;
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, GMGuncellemeDurumuCell, gmGuncellenenIlIlceOrani, ProjeConstants.BOLGE_GENELMUDURLUK, GUNCELLEME_DURUMU);
            decimal istGuncellenenIlIlceOrani = (istIl + istIlce) == 0 ? 0 : ((decimal)(istGuncellenen) / (istIl + istIlce)) * 100;
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, IstGuncellemeDurumuCell, istGuncellenenIlIlceOrani, ProjeConstants.BOLGE_ISTANBUL, GUNCELLEME_DURUMU);

            decimal izmGuncellenenIlIlceOrani = (izmIl + izmIlce) == 0 ? 0 : ((decimal)(izmGuncellenen) / (izmIl + izmIlce)) * 100;
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, IzmGuncellemeDurumuCell, izmGuncellenenIlIlceOrani, ProjeConstants.BOLGE_IZMIR, GUNCELLEME_DURUMU);
            decimal merGuncellenenIlIlceOrani = (decimal)(merIl + merIlce) == 0 ? 0 : ((decimal)(merGuncellenen) / (merIl + merIlce)) * 100;
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, MerGuncellemeDurumuCell, merGuncellenenIlIlceOrani, ProjeConstants.BOLGE_MERSIN, GUNCELLEME_DURUMU);
            decimal topGuncellenenIlIlceOrani = (toplamIl + toplamIlce) == 0 ? 0 : ((decimal)(toplamGuncellenen) / (toplamIl + toplamIlce)) * 100;
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, TopGuncellemeDurumuCell, topGuncellenenIlIlceOrani, ProjeConstants.BOLGE_HEPSI, GUNCELLEME_DURUMU);
        }
        private void HyperLinkEkle(string page, TableCell cell,decimal value, string bolge,string ozellik)
        {
            HyperLink cellLnk = new HyperLink();
            cellLnk.Text = ((int)value).ToString();
            string queryString = string.Empty;
            if (ozellik.Equals(KURULU_ILLER))
            {
                queryString = "&Grup="+ProjeConstants.FTK_GRUPLAMA_IL_ILCEYE_GORE_INT+"&IlcesiId=" + ProjeConstants.VALILIK_INT;
            }
            else if (ozellik.Equals(KURULU_ILCELER))
            {
                queryString = "&Grup=" + ProjeConstants.FTK_GRUPLAMA_IL_ILCEYE_GORE_INT + "&IlcesiId=" + ProjeConstants.SADECE_ILCELER_INT;
            }
            else if (ozellik.Equals(KURULU_IL_VE_ILCELER))
            {
                queryString = "&Grup=" + ProjeConstants.FTK_GRUPLAMA_IL_ILCEYE_GORE_INT + "&IlcesiId=" + ProjeConstants.HEPSI_INT;
            }
            else if (ozellik.Equals(YENI_KURULAN))
            {
                queryString = "&Grup=" + ProjeConstants.FTK_GRUPLAMA_IL_ILCEYE_GORE_INT + "&IlcesiId=" + ProjeConstants.HEPSI_INT +"&KurulusTarihi="+KurulusTarihiTxt.Text;
            }
            else if (ozellik.Equals(GUNCELLENEN))
            {
                queryString = "&Grup=" + ProjeConstants.FTK_GRUPLAMA_IL_ILCEYE_GORE_INT + "&IlcesiId=" + ProjeConstants.HEPSI_INT + "&GuncellemeTarihi=" + GuncellemeTarihiTxt.Text;
            }
            else if (ozellik.Equals(KURULUOLMAYAN_ILLER))
            {
                queryString = "&IlcesiId=" + ProjeConstants.VALILIK_INT;
            }
            else if (ozellik.Equals(KURULUOLMAYAN_ILCELER))
            {
                queryString = "&IlcesiId=" + ProjeConstants.SADECE_ILCELER_INT;
            }
            else if (ozellik.Equals(KURULUM_ORANI))
            {
                cellLnk.Text = value.ToString("N", cultureInfo);
                queryString = "&Grup=" + ProjeConstants.FTK_GRUPLAMA_IL_ILCEYE_GORE_INT + "&IlcesiId=" + ProjeConstants.HEPSI_INT ;
            }
            else if (ozellik.Equals(GUNCELLEME_DURUMU))
            {
                cellLnk.Text = value.ToString("N", cultureInfo);
                queryString = "&Grup=" + ProjeConstants.FTK_GRUPLAMA_IL_ILCEYE_GORE_INT + "&IlcesiId=" + ProjeConstants.HEPSI_INT + "&GuncellemeTarihi=" + GuncellemeTarihiTxt.Text;
            }
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            
            
            if (value > 0)
            {
                string linkUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + page + "?Bolge="+ bolge +queryString;
                cellLnk.NavigateUrl = linkUrl;
            }
            cell.Controls.Clear();
            cell.Controls.Add(cellLnk);
        }
        private void RedirectToPage(string pageUrl)
        {
            try
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + pageUrl;
                Page.Response.Redirect(newUrl);
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void FTKListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_FTK_LIST );
        }
        protected void FTKIslemleriBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_FTKISLEMLERI);
        }
    }
}
