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
        private int BolgeIdQS
        {
            get
            {

                if (ViewState["BolgeId"] == null)
                {
                    if (Page.Request.QueryString["BolgeId"] != null)
                    {
                        ViewState["BolgeId"] = Page.Request.QueryString["BolgeId"];
                    }
                    else
                    {
                        ViewState["BolgeId"] = string.Empty;
                    }
                }
                return ViewState["BolgeId"].ReturnZeroIfNull().ConvertToInt();
            }

            set
            {
                ViewState["BolgeId"] = value;
            }
        }
        private string CurrentUserName
        {
            get
            {

                if (ViewState["CurrentUserName"] == null)
                {
                    ViewState["CurrentUserName"] = UtilityHelper.GetCurrentUserLoginName();
                }
                return ViewState["CurrentUserName"].ToString();
            }

            set
            {
                ViewState["CurrentUserName"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                Bolge bolge = IKYSOrtak.BolgeGetirByUserName(CurrentUserName);
                BolgeIdQS = bolge == null ? 0 : bolge.Id;


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

        private int ankIl, ankIlce, istIl, istIlce, izmIl, izmIlce, merIl, merIlce, erzIl, erzIlce, toplamIl, toplamIlce;
        private int ankKOIl, ankKOIlce, istKOIl, istKOIlce, izmKOIl, izmKOIlce, merKOIl, merKOIlce, erzKOIl, erzKOIlce, toplamKOIl, toplamKOIlce;
        private int ankKOlmayanIl, ankKOlmayanIlce, istKOlmayanIl, istKOlmayanIlce, izmKOlmayanIl, izmKOlmayanIlce, merKOlmayanIl, merKOlmayanIlce, erzKOlmayanIl, erzKOlmayanIlce, toplamKOlmayanIl, toplamKOlmayanIlce;
        private int ankGuncellenen, istGuncellenen, izmGuncellenen, merGuncellenen, erzGuncellenen, toplamGuncellenen;
        private int ankYeniKurulan, istYeniKurulan, izmYeniKurulan, merYeniKurulan, erzYeniKurulan , toplamYeniKurulan;

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

        private int IlSayisiGetir(int bolgeId)
        {
            Il il = new Il();
            int ilSayisi = il.SelectCountIlByBolgeId(bolgeId);
            return ilSayisi;
        }
        private int IlceSayisiGetir(int bolgeId)
        {
            Ilce ilce = new Ilce();
            int ilceSayisi = ilce.SelectCountIlceByBolgeId(bolgeId);
            return ilceSayisi;
        }
        private void KuruluIlSayilariniDoldur()
        {
            ankKOIl = KOIlSayisiGetir(ProjeConstants.BOLGE_ANKARA_INT);
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, AnkKOIlCell, ankKOIl, ProjeConstants.BOLGE_ANKARA_INT, KURULU_ILLER);
            istKOIl = KOIlSayisiGetir(ProjeConstants.BOLGE_ISTANBUL_INT);
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, IstKOIlCell, istKOIl, ProjeConstants.BOLGE_ISTANBUL_INT, KURULU_ILLER);
            izmKOIl = KOIlSayisiGetir(ProjeConstants.BOLGE_IZMIR_INT);
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, IzmKOIlCell, izmKOIl, ProjeConstants.BOLGE_IZMIR_INT, KURULU_ILLER);
            merKOIl = KOIlSayisiGetir(ProjeConstants.BOLGE_MERSIN_INT);
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, MerKOIlCell, merKOIl, ProjeConstants.BOLGE_MERSIN_INT, KURULU_ILLER);
            erzKOIl = KOIlSayisiGetir(ProjeConstants.BOLGE_ERZURUM_INT);
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, ErzKOIlCell, erzKOIl, ProjeConstants.BOLGE_ERZURUM_INT, KURULU_ILLER);
            toplamKOIl = ankKOIl + istKOIl + izmKOIl + merKOIl + erzKOIl;
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, TopKOIlCell, toplamKOIl, ProjeConstants.BOLGE_HEPSI_INT, KURULU_ILLER);
        }
        private void KuruluIlceSayilariniDoldur()
        {
            ankKOIlce = KOIlceSayisiGetir(ProjeConstants.BOLGE_ANKARA_INT);
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, AnkKOIlceCell, ankKOIlce, ProjeConstants.BOLGE_ANKARA_INT, KURULU_ILCELER);
            istKOIlce = KOIlceSayisiGetir(ProjeConstants.BOLGE_ISTANBUL_INT);
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, IstKOIlceCell, istKOIlce, ProjeConstants.BOLGE_ISTANBUL_INT, KURULU_ILCELER);
            izmKOIlce = KOIlceSayisiGetir(ProjeConstants.BOLGE_IZMIR_INT);
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, IzmKOIlceCell, izmKOIlce, ProjeConstants.BOLGE_IZMIR_INT, KURULU_ILCELER);
            merKOIlce = KOIlceSayisiGetir(ProjeConstants.BOLGE_MERSIN_INT);
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, MerKOIlceCell, merKOIlce, ProjeConstants.BOLGE_MERSIN_INT, KURULU_ILCELER);
            erzKOIlce = KOIlceSayisiGetir(ProjeConstants.BOLGE_ERZURUM_INT);
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, ErzKOIlceCell, erzKOIlce, ProjeConstants.BOLGE_ERZURUM_INT, KURULU_ILCELER);
            toplamKOIlce = ankKOIlce + istKOIlce + izmKOIlce + merKOIlce + erzKOIlce;
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, TopKOIlceCell, toplamKOIlce, ProjeConstants.BOLGE_HEPSI_INT, KURULU_ILCELER);
        }
        private void IlSayilariniDoldur()
        {
            AnkSBIlCell.Text = (ankIl = IlSayisiGetir(ProjeConstants.BOLGE_ANKARA_INT)).ToString();
            IstSBIlCell.Text = (istIl = IlSayisiGetir(ProjeConstants.BOLGE_ISTANBUL_INT)).ToString();
            IzmSBIlCell.Text = (izmIl = IlSayisiGetir(ProjeConstants.BOLGE_IZMIR_INT)).ToString();
            MerSBIlCell.Text = (merIl = IlSayisiGetir(ProjeConstants.BOLGE_MERSIN_INT)).ToString();
            ErzSBIlCell.Text = (erzIl = IlSayisiGetir(ProjeConstants.BOLGE_ERZURUM_INT)).ToString();
            TopSBIlCell.Text = (toplamIl = ankIl + istIl + izmIl + merIl + erzIl).ToString();
        }
        private void IlceSayilariniDoldur()
        {
            AnkSBIlceCell.Text = (ankIlce = IlceSayisiGetir(ProjeConstants.BOLGE_ANKARA_INT)).ToString();
            IstSBIlceCell.Text = (istIlce = IlceSayisiGetir(ProjeConstants.BOLGE_ISTANBUL_INT)).ToString();
            IzmSBIlceCell.Text = (izmIlce = IlceSayisiGetir(ProjeConstants.BOLGE_IZMIR_INT)).ToString();
            MerSBIlceCell.Text = (merIlce = IlceSayisiGetir(ProjeConstants.BOLGE_MERSIN_INT)).ToString();
            ErzSBIlceCell.Text = (erzIlce = IlceSayisiGetir(ProjeConstants.BOLGE_ERZURUM_INT)).ToString();
            TopSBIlceCell.Text = (toplamIlce = ankIlce + istIlce + izmIlce + merIlce + erzIlce).ToString();
        }
        private int KOIlSayisiGetir(int bolgeId)
        {
            FTK ftk = new FTK();
            int ilSayisi = ftk.SelectKuruluOlanIlSayisiByBolgeId(bolgeId);
            return ilSayisi;
        }
        private int KOIlceSayisiGetir(int bolgeId)
        {
            FTK ftk = new FTK();
            int ilSayisi = ftk.SelectKuruluOlanIlceSayisiByBolgeId(bolgeId);
            return ilSayisi;
        }
        private void GuncellenenIlIlceSayisiniDoldur()
        {
            FTK ftk = new FTK();
            ankGuncellenen = ftk.SelectGuncellenenIlIlceSayisiByBolgeId(ProjeConstants.BOLGE_ANKARA_INT, GuncellemeTarihiTxt.Text.ConvertToDatetime());
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, AnkGuncellenenCell, ankGuncellenen, ProjeConstants.BOLGE_ANKARA_INT, GUNCELLENEN);
            istGuncellenen = ftk.SelectGuncellenenIlIlceSayisiByBolgeId(ProjeConstants.BOLGE_ISTANBUL_INT, GuncellemeTarihiTxt.Text.ConvertToDatetime());
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, IstGuncellenenCell, istGuncellenen, ProjeConstants.BOLGE_ISTANBUL_INT, GUNCELLENEN);
            izmGuncellenen = ftk.SelectGuncellenenIlIlceSayisiByBolgeId(ProjeConstants.BOLGE_IZMIR_INT, GuncellemeTarihiTxt.Text.ConvertToDatetime());
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, IzmGuncellenenCell, izmGuncellenen, ProjeConstants.BOLGE_IZMIR_INT, GUNCELLENEN);
            merGuncellenen = ftk.SelectGuncellenenIlIlceSayisiByBolgeId(ProjeConstants.BOLGE_MERSIN_INT, GuncellemeTarihiTxt.Text.ConvertToDatetime());
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, MerGuncellenenCell, merGuncellenen, ProjeConstants.BOLGE_MERSIN_INT, GUNCELLENEN);
            erzGuncellenen = ftk.SelectGuncellenenIlIlceSayisiByBolgeId(ProjeConstants.BOLGE_ERZURUM_INT, GuncellemeTarihiTxt.Text.ConvertToDatetime());
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, ErzGuncellenenCell, erzGuncellenen, ProjeConstants.BOLGE_ERZURUM_INT, GUNCELLENEN);
            toplamGuncellenen = ankGuncellenen + istGuncellenen + izmGuncellenen + merGuncellenen + erzGuncellenen ;
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, TopGuncellenenCell, toplamGuncellenen, ProjeConstants.BOLGE_HEPSI_INT, GUNCELLENEN);
        }
        private void YeniKurulanIlIlceSayisiniDoldur()
        {
            FTK ftk = new FTK();
            ankYeniKurulan = ftk.SelectKuruluIlIlceSayisiByBolgeId(ProjeConstants.BOLGE_ANKARA_INT, KurulusTarihiTxt.Text.ConvertToDatetime());
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, AnkYeniKurulanCell, ankYeniKurulan, ProjeConstants.BOLGE_ANKARA_INT, YENI_KURULAN);
            istYeniKurulan = ftk.SelectKuruluIlIlceSayisiByBolgeId(ProjeConstants.BOLGE_ISTANBUL_INT, KurulusTarihiTxt.Text.ConvertToDatetime());
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, IstYeniKurulanCell, istYeniKurulan, ProjeConstants.BOLGE_ISTANBUL_INT, YENI_KURULAN);
            izmYeniKurulan = ftk.SelectKuruluIlIlceSayisiByBolgeId(ProjeConstants.BOLGE_IZMIR_INT, KurulusTarihiTxt.Text.ConvertToDatetime());
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, IzmYeniKurulanCell, izmYeniKurulan, ProjeConstants.BOLGE_IZMIR_INT, YENI_KURULAN);
            merYeniKurulan = ftk.SelectKuruluIlIlceSayisiByBolgeId(ProjeConstants.BOLGE_MERSIN_INT, KurulusTarihiTxt.Text.ConvertToDatetime());
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, MerYeniKurulanCell, merYeniKurulan, ProjeConstants.BOLGE_MERSIN_INT, YENI_KURULAN);
            erzYeniKurulan = ftk.SelectKuruluIlIlceSayisiByBolgeId(ProjeConstants.BOLGE_ERZURUM_INT, KurulusTarihiTxt.Text.ConvertToDatetime());
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, ErzYeniKurulanCell, erzYeniKurulan, ProjeConstants.BOLGE_ERZURUM_INT, YENI_KURULAN);
            toplamYeniKurulan = ankYeniKurulan + istYeniKurulan + izmYeniKurulan + merYeniKurulan + erzYeniKurulan;
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, TopYeniKurulanCell, toplamYeniKurulan, ProjeConstants.BOLGE_HEPSI_INT, YENI_KURULAN);
        }
        private void KOlmayanIlSayisiniDoldur()
        {
            ankKOlmayanIl = ankIl - ankKOIl;
            HyperLinkEkle(ProjeConstants.PAGE_FTKKURULU_OLMAYAN_ILILCE_LIST, AnkKOlmayanIlCell, ankKOlmayanIl, ProjeConstants.BOLGE_ANKARA_INT, KURULUOLMAYAN_ILLER);
            istKOlmayanIl = istIl - istKOIl;
            HyperLinkEkle(ProjeConstants.PAGE_FTKKURULU_OLMAYAN_ILILCE_LIST, IstKOlmayanIlCell, istKOlmayanIl, ProjeConstants.BOLGE_ISTANBUL_INT, KURULUOLMAYAN_ILLER);
            izmKOlmayanIl = izmIl - izmKOIl;
            HyperLinkEkle(ProjeConstants.PAGE_FTKKURULU_OLMAYAN_ILILCE_LIST, IzmKOlmayanIlCell, izmKOlmayanIl, ProjeConstants.BOLGE_IZMIR_INT, KURULUOLMAYAN_ILLER);
            merKOlmayanIl = merIl - merKOIl;
            HyperLinkEkle(ProjeConstants.PAGE_FTKKURULU_OLMAYAN_ILILCE_LIST, MerKOlmayanIlCell, merKOlmayanIl, ProjeConstants.BOLGE_MERSIN_INT, KURULUOLMAYAN_ILLER);
            erzKOlmayanIl = erzIl - erzKOIl;
            HyperLinkEkle(ProjeConstants.PAGE_FTKKURULU_OLMAYAN_ILILCE_LIST, ErzKOlmayanIlCell, erzKOlmayanIl, ProjeConstants.BOLGE_ERZURUM_INT, KURULUOLMAYAN_ILLER);
            toplamKOlmayanIl = ankKOlmayanIl + istKOlmayanIl + izmKOlmayanIl + merKOlmayanIl + erzKOlmayanIl;
            HyperLinkEkle(ProjeConstants.PAGE_FTKKURULU_OLMAYAN_ILILCE_LIST, TopKOlmayanIlCell, toplamKOlmayanIl, ProjeConstants.BOLGE_HEPSI_INT, KURULUOLMAYAN_ILLER);

        }
        private void KOlmayanIlceSayisiniDoldur()
        {
            ankKOlmayanIlce = ankIlce - ankKOIlce;
            HyperLinkEkle(ProjeConstants.PAGE_FTKKURULU_OLMAYAN_ILILCE_LIST, AnkKOlmayanIlceCell, ankKOlmayanIlce, ProjeConstants.BOLGE_ANKARA_INT, KURULUOLMAYAN_ILCELER);
            istKOlmayanIlce = istIlce - istKOIlce;
            HyperLinkEkle(ProjeConstants.PAGE_FTKKURULU_OLMAYAN_ILILCE_LIST, IstKOlmayanIlceCell, istKOlmayanIlce, ProjeConstants.BOLGE_ISTANBUL_INT, KURULUOLMAYAN_ILCELER);
            izmKOlmayanIlce = izmIlce - izmKOIlce;
            HyperLinkEkle(ProjeConstants.PAGE_FTKKURULU_OLMAYAN_ILILCE_LIST, IzmKOlmayanIlceCell, izmKOlmayanIlce, ProjeConstants.BOLGE_IZMIR_INT, KURULUOLMAYAN_ILCELER);
            merKOlmayanIlce = merIlce - merKOIlce;
            HyperLinkEkle(ProjeConstants.PAGE_FTKKURULU_OLMAYAN_ILILCE_LIST, MerKOlmayanIlceCell, merKOlmayanIlce, ProjeConstants.BOLGE_MERSIN_INT, KURULUOLMAYAN_ILCELER);
            erzKOlmayanIlce = erzIlce - erzKOIlce;
            HyperLinkEkle(ProjeConstants.PAGE_FTKKURULU_OLMAYAN_ILILCE_LIST, ErzKOlmayanIlceCell, erzKOlmayanIlce, ProjeConstants.BOLGE_ERZURUM_INT, KURULUOLMAYAN_ILCELER);
            toplamKOlmayanIlce = ankKOlmayanIlce + istKOlmayanIlce + izmKOlmayanIlce + merKOlmayanIlce+erzKOlmayanIlce;
            HyperLinkEkle(ProjeConstants.PAGE_FTKKURULU_OLMAYAN_ILILCE_LIST, TopKOlmayanIlceCell, toplamKOlmayanIlce, ProjeConstants.BOLGE_HEPSI_INT, KURULUOLMAYAN_ILCELER);
        }

        private void KurulumOraniniDoldur()
        {
            decimal gmKuruluIlIlceOrani = (ankIl + ankIlce) == 0 ? 0 : ((decimal)(ankKOIl + ankKOIlce) / (ankIl + ankIlce)) * 100;
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, AnkKurulumOraniCell, gmKuruluIlIlceOrani, ProjeConstants.BOLGE_ANKARA_INT, KURULUM_ORANI);
            decimal istKuruluIlIlceOrani = (istIl + istIlce) == 0 ? 0 : ((decimal)(istKOIl + istKOIlce) / (istIl + istIlce)) * 100;
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, IstKurulumOraniCell, istKuruluIlIlceOrani, ProjeConstants.BOLGE_ISTANBUL_INT, KURULUM_ORANI);

            decimal izmKuruluIlIlceOrani = (izmIl + izmIlce) == 0 ? 0 : ((decimal)(izmKOIl + izmKOIlce) / (izmIl + izmIlce)) * 100;
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, IzmKurulumOraniCell, izmKuruluIlIlceOrani, ProjeConstants.BOLGE_IZMIR_INT, KURULUM_ORANI);
            decimal merKuruluIlIlceOrani = (decimal)(merIl + merIlce) == 0 ? 0 : ((decimal)(merKOIl + merKOIlce) / (merIl + merIlce)) * 100;
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, MerKurulumOraniCell, merKuruluIlIlceOrani, ProjeConstants.BOLGE_MERSIN_INT, KURULUM_ORANI);
            decimal erzKuruluIlIlceOrani = (decimal)(erzIl + erzIlce) == 0 ? 0 : ((decimal)(erzKOIl + erzKOIlce) / (erzIl + erzIlce)) * 100;
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, ErzKurulumOraniCell, erzKuruluIlIlceOrani, ProjeConstants.BOLGE_ERZURUM_INT, KURULUM_ORANI);
            decimal topKuruluIlIlceOrani = (toplamIl + toplamIlce) == 0 ? 0 : ((decimal)(toplamKOIl + toplamKOIlce) / (toplamIl + toplamIlce)) * 100;
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, TopKurulumOraniCell, topKuruluIlIlceOrani, ProjeConstants.BOLGE_HEPSI_INT, KURULUM_ORANI);
        }
        private void GuncellemeDurumuDoldur()
        {
            decimal gmGuncellenenIlIlceOrani = (ankIl + ankIlce) == 0 ? 0 : ((decimal)(ankGuncellenen) / (ankIl + ankIlce)) * 100;
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, AnkGuncellemeDurumuCell, gmGuncellenenIlIlceOrani, ProjeConstants.BOLGE_ANKARA_INT, GUNCELLEME_DURUMU);
            decimal istGuncellenenIlIlceOrani = (istIl + istIlce) == 0 ? 0 : ((decimal)(istGuncellenen) / (istIl + istIlce)) * 100;
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, IstGuncellemeDurumuCell, istGuncellenenIlIlceOrani, ProjeConstants.BOLGE_ISTANBUL_INT, GUNCELLEME_DURUMU);

            decimal izmGuncellenenIlIlceOrani = (izmIl + izmIlce) == 0 ? 0 : ((decimal)(izmGuncellenen) / (izmIl + izmIlce)) * 100;
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, IzmGuncellemeDurumuCell, izmGuncellenenIlIlceOrani, ProjeConstants.BOLGE_IZMIR_INT, GUNCELLEME_DURUMU);
            decimal merGuncellenenIlIlceOrani = (decimal)(merIl + merIlce) == 0 ? 0 : ((decimal)(merGuncellenen) / (merIl + merIlce)) * 100;
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, MerGuncellemeDurumuCell, merGuncellenenIlIlceOrani, ProjeConstants.BOLGE_MERSIN_INT, GUNCELLEME_DURUMU);
            decimal erzGuncellenenIlIlceOrani = (decimal)(erzIl + erzIlce) == 0 ? 0 : ((decimal)(erzGuncellenen) / (erzIl + erzIlce)) * 100;
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, ErzGuncellemeDurumuCell, erzGuncellenenIlIlceOrani, ProjeConstants.BOLGE_ERZURUM_INT, GUNCELLEME_DURUMU);
            decimal topGuncellenenIlIlceOrani = (toplamIl + toplamIlce) == 0 ? 0 : ((decimal)(toplamGuncellenen) / (toplamIl + toplamIlce)) * 100;
            HyperLinkEkle(ProjeConstants.PAGE_FTK_LIST, TopGuncellemeDurumuCell, topGuncellenenIlIlceOrani, ProjeConstants.BOLGE_HEPSI_INT, GUNCELLEME_DURUMU);
        }
        private void HyperLinkEkle(string page, TableCell cell, decimal value, int bolgeId, string ozellik)
        {
            HyperLink cellLnk = new HyperLink();
            cellLnk.Text = ((int)value).ToString();
            string queryString = string.Empty;
            
            if (ozellik.Equals(KURULU_ILLER))
            {
                queryString = "&Grup=" + ProjeConstants.FTK_GRUPLAMA_IL_ILCEYE_GORE_INT + "&IlcesiId=" + ProjeConstants.VALILIK_INT;
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
                queryString = "&Grup=" + ProjeConstants.FTK_GRUPLAMA_IL_ILCEYE_GORE_INT + "&IlcesiId=" + ProjeConstants.HEPSI_INT + "&KurulusTarihi=" + KurulusTarihiTxt.Text;
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
                queryString = "&Grup=" + ProjeConstants.FTK_GRUPLAMA_IL_ILCEYE_GORE_INT + "&IlcesiId=" + ProjeConstants.HEPSI_INT;
            }
            else if (ozellik.Equals(GUNCELLEME_DURUMU))
            {
                cellLnk.Text = value.ToString("N", cultureInfo);
                queryString = "&Grup=" + ProjeConstants.FTK_GRUPLAMA_IL_ILCEYE_GORE_INT + "&IlcesiId=" + ProjeConstants.HEPSI_INT + "&GuncellemeTarihi=" + GuncellemeTarihiTxt.Text;
            }
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();

            bool bolgeBool = (bolgeId == BolgeIdQS) || (BolgeIdQS == ProjeConstants.BOLGE_GENELMUDURLUK_INT);
            if (value > 0 && bolgeBool)
            {
                string linkUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + page + "?Bolge=" + bolgeId + queryString;
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
            RedirectToPage(ProjeConstants.PAGE_FTK_LIST);
        }
        protected void FTKIslemleriBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_FTKISLEMLERI);
        }
    }
}
