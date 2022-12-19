using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.OdemePlaniWP
{
    [ToolboxItemAttribute(false)]
    public partial class OdemePlaniWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public OdemePlaniWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string SenderAppQS
        {
            get
            {

                if (ViewState["SenderApp"] == null)
                {
                    if (Page.Request.QueryString["SenderApp"] != null)
                    {
                        ViewState["SenderApp"] = Page.Request.QueryString["SenderApp"];
                    }
                    else
                    {
                        ViewState["SenderApp"] = string.Empty;
                    }
                }
                return ViewState["SenderApp"].ToString();
            }

            set
            {
                ViewState["SenderApp"] = value;
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
        private string KiraciIdQS
        {
            get
            {

                if (ViewState["KiraciId"] == null)
                {
                    if (Page.Request.QueryString["KiraciId"] != null)
                    {
                        ViewState["KiraciId"] = Page.Request.QueryString["KiraciId"];
                    }
                    else
                    {
                        ViewState["KiraciId"] = string.Empty;
                    }
                }
                return ViewState["KiraciId"].ToString();
            }

            set
            {
                ViewState["KiraciId"] = value;
            }
        }
        private string KiraSozlesmeIdQS
        {
            get
            {

                if (ViewState["KiraSozlesmeId"] == null)
                {
                    if (Page.Request.QueryString["KiraSozlesmeId"] != null)
                    {
                        ViewState["KiraSozlesmeId"] = Page.Request.QueryString["KiraSozlesmeId"];
                    }
                    else
                    {
                        ViewState["KiraSozlesmeId"] = string.Empty;
                    }
                }
                return ViewState["KiraSozlesmeId"].ToString();
            }

            set
            {
                ViewState["KiraSozlesmeId"] = value;
            }
        }
        private IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(KiraSozlesmeIdQS))
            {
                IdLbl.Text = KiraSozlesmeIdQS;
                KiraSozlesme kiraSozlesme = new KiraSozlesme();
                kiraSozlesme = kiraSozlesme.Select(KiraSozlesmeIdQS.ConvertToInt());
                if (kiraSozlesme != null)
                {
                    AdiLbl.Text = KiraciGetir(kiraSozlesme.KiraciId) + "( Sözleşme : " + kiraSozlesme.SozBasTar.ConvertToDatetimeEmptyIfNull() + " - " + kiraSozlesme.SozBitTar.ConvertToDatetimeEmptyIfNull() + ")";
                    if (string.IsNullOrEmpty(KiraciIdQS))
                        KiraciIdQS = kiraSozlesme.KiraciId.ToString();
                    if (!Page.IsPostBack)
                    {
                        TBYSOrtak.BakiyeBorcHesapla(kiraSozlesme);
                        if (kiraSozlesme.GecikmeZammiTipi.Equals(ProjeConstants.KIRASOZLESME_GECIKMEZAMMI_GUNLUK))
                        {
                            GecikmeZammiTipiLbl.Text = "Gecikme Zammı : " + ProjeConstants.KIRASOZLESME_GECIKMEZAMMI_GUNLUK;
                        }
                        else
                        {
                            GecikmeZammiTipiLbl.Text = "Gecikme Zammı : " + ProjeConstants.KIRASOZLESME_GECIKMEZAMMI_AYLIK;

                        }
                        GecikmeZammmiGunlukBtn.Visible = !kiraSozlesme.GecikmeZammiTipi.Equals(ProjeConstants.KIRASOZLESME_GECIKMEZAMMI_GUNLUK);
                        GecikmeZammmiAylikBtn.Visible = !kiraSozlesme.GecikmeZammiTipi.Equals(ProjeConstants.KIRASOZLESME_GECIKMEZAMMI_AYLIK); ;
                    }

                    OdemePlaniTablosunuDoldur(kiraSozlesme);
                    EnableDisableOdemePlaniEkleBtn();
                }
                else
                {
                    MessageHelper.PublishMessage("Kira Sözleşmesi Bulunamadı", ProjeConstants.MESAJ_HATA);
                }
            }

        }

        private string KiraciGetir(int kiraciId)
        {
            Kiraci kiraci = new Kiraci();
            kiraci = kiraci.Select(kiraciId);
            if (kiraci != null)
            {
                return kiraci.Adi + " " + kiraci.Soyadi;
            }
            else
            {
                return string.Empty;
            }
        }

        private void OdemePlaniTablosunuDoldur(KiraSozlesme kiraSozlesme)
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            OdemePlani opl = new OdemePlani();
            List<OdemePlani> odemePlaniListesi = opl.SelectBySozlesmeId(kiraSozlesme.Id);
            //if (odemePlaniListesi.Count > 0)
            //{
            //    OdemeYapDiv.Attributes["style"] = "display:block";

            //}
            //else
            //{
            //    OdemeYapDiv.Attributes["style"] = "display:none";
            //}
            bool isaretlendiMi = false;
            foreach (OdemePlani op in odemePlaniListesi)
            {
                TableRow row = new TableRow();
                TableCell SiraNoCell = new TableCell();
                SiraNoCell.Text = op.Sira.ToString();

                TableCell VadeBasTarCell = new TableCell();
                string vadeBastar = op.VadeBasTar == DateTime.MinValue ? "" : op.VadeBasTar.ConvertToDatetimeEmptyIfNull();
                string vadeBittar = op.VadeBasTar == DateTime.MinValue ? "" : op.VadeBitTar.ConvertToDatetimeEmptyIfNull();

                LinkButton VadeBasTarBtn = new LinkButton();
                VadeBasTarBtn.Text = vadeBastar + " - " + vadeBittar;
                VadeBasTarBtn.CssClass = "btn btn-link";
                VadeBasTarBtn.Click += delegate
                {
                    HiddenOdemePlaniId.Value = op.Id.ToString();
                    PopupMesajLbl.Text = "Vade Tarihi Değiştirme";
                    VadeGosterDiv.Attributes["style"] = "display: block";
                    KiraBedeliDegistirDiv.Attributes["style"] = "display: none";
                    GecikmeZammiOraniGosterDiv.Attributes["style"] = "display: none";

                    SiraLbl.Text = "Sıra : " + op.Sira;
                    VadeBasTarTxt.Value = op.VadeBasTar.ConvertToDatetimeEmptyIfNull();
                    VadeBitTarTxt.Value = op.VadeBitTar.ConvertToDatetimeEmptyIfNull();
                    var openPopup = "OpenModalOnay();";
                    UtilityHelper.ScriptCalistir(openPopup);
                };
                VadeBasTarCell.Controls.Add(VadeBasTarBtn);

                TableCell KiraBedeliCell = new TableCell();
                KiraBedeliCell.Text = op.KiraBedeli.ToString("N", culturInfo);
                KiraBedeliCell.CssClass = "text-right";

                LinkButton kiraBedeliBtn = new LinkButton();
                kiraBedeliBtn.Text = op.KiraBedeli.ToString("N", culturInfo);
                kiraBedeliBtn.CssClass = "btn btn-link";
                kiraBedeliBtn.Click += delegate
                {
                    HiddenOdemePlaniId.Value = op.Id.ToString();
                    PopupMesajLbl.Text = "Kira Bedeli Değiştirme";
                    KiraBedeliDegistirDiv.Attributes["style"] = "display: block";
                    GecikmeZammiOraniGosterDiv.Attributes["style"] = "display: none";
                    VadeGosterDiv.Attributes["style"] = "display: none";
                    VadeBasTarLbl.Text = "Vade tarihi : " + vadeBastar + " - " + vadeBittar;
                    SiraLbl.Text = "Sıra : " + op.Sira;
                    YeniKiraBedeliTxt.Text = op.KiraBedeli.ToString("N", culturInfo);
                    var openPopup = "OpenModalOnay();";
                    System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), openPopup, true);
                };
                KiraBedeliCell.Controls.Add(kiraBedeliBtn);

                TableCell OdenenCell = new TableCell();
                OdenenCell.CssClass = "text-right";
                OdenenCell.Text = op.OdenenTutar.ToString("N", culturInfo);

                TableCell AnaParaCell = new TableCell();
                AnaParaCell.Text = op.AnaPara.ToString("N", culturInfo);
                AnaParaCell.CssClass = "text-right";

                TableCell GecikmeZammiOraniCell = new TableCell();
                GecikmeZammiOraniCell.CssClass = "text-right";
                if (kiraSozlesme.GecikmeZammiTipi.Equals(ProjeConstants.KIRASOZLESME_GECIKMEZAMMI_AYLIK) || op.FaizOrani == 0)
                {
                    GecikmeZammiOraniCell.Text = op.FaizOrani.ToString("N", culturInfo);
                }
                else
                {
                    LinkButton GecikmeZammiOraniBtn = new LinkButton();
                    GecikmeZammiOraniBtn.Text = op.FaizOrani.ToString("N", culturInfo);
                    GecikmeZammiOraniBtn.CssClass = "btn btn-link text-right";
                    GecikmeZammiOraniBtn.Click += delegate
                    {
                        PopupMesajLbl.Text = "Gecikme Zammı Oranı";
                        GecikmeZammiBilgileriniDoldur(op.Id);
                        KiraBedeliDegistirDiv.Attributes["style"] = "display: none";
                        VadeGosterDiv.Attributes["style"] = "display: none";
                        GecikmeZammiOraniGosterDiv.Attributes["style"] = "display: block";
                        VadeBasTarLbl.Text = vadeBastar + " - " + vadeBittar;

                        var openPopup = "OpenModalOnay();";
                        UtilityHelper.ScriptCalistir(openPopup);
                    };
                    GecikmeZammiOraniCell.Controls.Add(GecikmeZammiOraniBtn);
                }

                TableCell FaizTutariCell = new TableCell();
                FaizTutariCell.CssClass = "text-right";

                if (kiraSozlesme.GecikmeZammiTipi.Equals(ProjeConstants.KIRASOZLESME_GECIKMEZAMMI_AYLIK) || op.FaizTutari == 0)
                {
                    FaizTutariCell.Text = op.FaizTutari.ToString("N", culturInfo);
                }
                else
                {
                    LinkButton GecikmeZammiTutariBtn = new LinkButton();
                    GecikmeZammiTutariBtn.Text = op.FaizTutari.ToString("N", culturInfo);
                    GecikmeZammiTutariBtn.CssClass = "btn btn-link text-right";
                    GecikmeZammiTutariBtn.Click += delegate
                    {
                        PopupMesajLbl.Text = "Gecikme Zammı Tutari";
                        GecikmeZammiBilgileriniDoldur(op.Id);
                        KiraBedeliDegistirDiv.Attributes["style"] = "display: none";
                        VadeGosterDiv.Attributes["style"] = "display: none";
                        GecikmeZammiOraniGosterDiv.Attributes["style"] = "display: block";
                        VadeBasTarLbl.Text = vadeBastar + " - " + vadeBittar;

                        var openPopup = "OpenModalOnay();";
                        UtilityHelper.ScriptCalistir(openPopup);
                    };
                    FaizTutariCell.Controls.Add(GecikmeZammiTutariBtn);
                }


                TableCell FaizliBakiyeCell = new TableCell();
                FaizliBakiyeCell.Text = op.FaizliBakiye.ToString("N", culturInfo);
                FaizliBakiyeCell.CssClass = "text-right";

                if (op.Sira == 0)
                {
                    SiraNoCell.Text = string.Empty;
                    VadeBasTarCell.Text = "Devir";
                    KiraBedeliCell.Text = string.Empty;
                    OdenenCell.Text = string.Empty;
                    GecikmeZammiOraniCell.Text = string.Empty;
                }

                TableCell OdemeListesiCell = new TableCell();
                if (op.OdenenTutar > 0)
                {
                    LinkButton OdemeListesiBtn = new LinkButton();
                    OdemeListesiBtn.Text = "Ödemeler";
                    OdemeListesiBtn.CssClass = "btn btn-outline-primary";
                    OdemeListesiBtn.Click += delegate
                    {
                        OdemeGoruntule(kiraSozlesme, op.VadeBasTar.Year, op.VadeBasTar.Month, op.Id);
                    };
                    OdemeListesiCell.Controls.Add(OdemeListesiBtn);
                }
                DateTime today = DateTime.Today;

                DateTime vade = op.VadeBasTar.AddMonths(1);

                if ((vade > today) && op.Sira != 0 && !isaretlendiMi)
                {
                    row.CssClass = "alert-primary border border-dark border-2";
                    isaretlendiMi = true;
                }

                row.Controls.Add(SiraNoCell);
                row.Controls.Add(VadeBasTarCell);
                row.Controls.Add(KiraBedeliCell);
                row.Controls.Add(OdenenCell);
                row.Controls.Add(AnaParaCell);
                row.Controls.Add(GecikmeZammiOraniCell);
                row.Controls.Add(FaizTutariCell);
                row.Controls.Add(FaizliBakiyeCell);
                row.Controls.Add(OdemeListesiCell);

                AyrintiTable.Controls.Add(row);
            }
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        protected void SozlesmeBtn_Click(object sender, EventArgs e)
        {
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            kiraSozlesme = kiraSozlesme.Select<KiraSozlesme>(KiraSozlesmeIdQS.ConvertToInt());
            if (kiraSozlesme != null)
            {
                if (kiraSozlesme.Aktif)
                    RedirectToPage(ProjeConstants.PAGE_KIRASOZLESMESI + "?SenderApp=" + SenderAppQS + "&KiraSozlesmeId=" + kiraSozlesme.Id + "&KiraciId = " + KiraciIdQS);
                else
                    RedirectToPage(ProjeConstants.PAGE_BITENKIRASOZLESMESI + "?SenderApp=" + SenderAppQS + "&KiraSozlesmeId=" + kiraSozlesme.Id + "&KiraciId=" + KiraciIdQS);
            }
            else
            {
                MessageHelper.PublishMessage("Kira Sözleşmesi Bulunamadı", ProjeConstants.MESAJ_HATA);
            }
        }
        protected void KiraciKartiBtn_Click(object sender, EventArgs e)
        {
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            kiraSozlesme = kiraSozlesme.Select<KiraSozlesme>(KiraSozlesmeIdQS.ConvertToInt());
            if (kiraSozlesme != null)
            {
                Kiraci kiraci = new Kiraci();
                kiraci = kiraci.Select<Kiraci>(kiraSozlesme.KiraciId);
                if (kiraci != null)
                {
                    RedirectToPage(ProjeConstants.PAGE_KIRAKARTI + "?SenderApp=" + SenderAppQS + "&KiraSozlesmeId=" + kiraSozlesme.Id + "&KiraciId=" + kiraci.Id);
                }
                else
                {
                    MessageHelper.PublishMessage("Kiracı Bulunamadı", ProjeConstants.MESAJ_HATA);
                }
            }
            else
            {
                MessageHelper.PublishMessage("Kiracı Bulunamadı", ProjeConstants.MESAJ_HATA);
            }
        }
        protected void KiraciBtn_Click(object sender, EventArgs e)
        {
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            kiraSozlesme = kiraSozlesme.Select<KiraSozlesme>(KiraSozlesmeIdQS.ConvertToInt());
            if (kiraSozlesme != null)
            {
                Kiraci kiraci = new Kiraci();
                kiraci = kiraci.Select<Kiraci>(kiraSozlesme.KiraciId);
                if (kiraci != null)
                {
                    RedirectToPage(ProjeConstants.PAGE_KIRACI_GIRIS + "?DestinationApp=KD&SenderApp=KL&KiraciId=" + KiraciIdQS);
                }
                else
                {
                    MessageHelper.PublishMessage("Kiracı Bulunamadı", ProjeConstants.MESAJ_HATA);
                }
            }
            else
            {
                MessageHelper.PublishMessage("Kiracı Bulunamadı", ProjeConstants.MESAJ_HATA);
            }

        }
        protected void OdemePlaniListBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_ODEMEPLANI_LIST);
        }
        protected void KiraciAylikOdemeBtn_Click(object sender, EventArgs e)
        {
            string ay = "0";
            string yil = "0";
            if (!string.IsNullOrEmpty(KiraSozlesmeIdQS))
            {
                KiraSozlesme kiraSozlesme = new KiraSozlesme();
                kiraSozlesme = kiraSozlesme.Select(KiraSozlesmeIdQS.ConvertToInt());
                if (kiraSozlesme != null)
                {
                    yil = kiraSozlesme.SozBasTar.Year.ToString();
                }

            }

            RedirectToPage(ProjeConstants.PAGE_KIRACI_AYLIKODEME + "?KiraciId=" + KiraciIdQS + "&SecilenAy=" + ay + "&SecilenYil=" + yil);
        }
        protected void BakiyeDevirBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_KIRA_BAKIYEDEVRI + "?KiraciId=" + KiraciIdQS);
        }
        protected void OdemePlaniEkleBtn_Click(object sender, EventArgs e)
        {
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            kiraSozlesme = kiraSozlesme.Select<KiraSozlesme>(KiraSozlesmeIdQS.ConvertToInt());
            // kira sozlesmesinin sozbastar ve sozbittar alanları dolu değilse 
            if (string.IsNullOrEmpty(kiraSozlesme.SozBasTar.ConvertToDatetimeEmptyIfNull()) || string.IsNullOrEmpty(kiraSozlesme.SozBitTar.ConvertToDatetimeEmptyIfNull()))
            {
                MessageHelper.PublishMessage("Bu sözleşme için ödeme Planı oluşturulamaz. Ödeme Planı oluşturabilmek için, Lütfen Kira Sözleşmesinde Sözleşme Başlangıç ve Bitiş tarihini giriniz", ProjeConstants.MESAJ_HATA);
            }
            else
            {
                bool planOlustu = OdemePlaniniKontrolEtVeOlustur();
                if (planOlustu)
                {
                    //Page.Response.Redirect("/pages/OdemePlani.aspx?KiraSozlesmeId=" + kiraSozlesme.Id);
                    string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                    string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));
                    newUrl += "/" + ProjeConstants.PAGE_ODEMEPLANI + "?SenderApp=" + SenderAppQS + "&KiraciId=" + KiraciIdQS + "&KiraSozlesmeId=" + kiraSozlesme.Id;
                    Page.Response.Redirect(newUrl, true);
                }
            }
        }
        private void EnableDisableOdemePlaniEkleBtn()
        {
            OdemePlaniEkleBtn.Visible = true;
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            kiraSozlesme = kiraSozlesme.Select<KiraSozlesme>(KiraSozlesmeIdQS.ConvertToInt());
            // kira sozlesmesinin sozbastar ve sozbittar alanları dolu değilse 
            if (string.IsNullOrEmpty(kiraSozlesme.SozBasTar.ConvertToDatetimeEmptyIfNull()) || string.IsNullOrEmpty(kiraSozlesme.SozBitTar.ConvertToDatetimeEmptyIfNull()))
            {
                OdemePlaniEkleBtn.Visible = false;
                OdemePlaniSilBtn.Visible = false;
                //TumunuOdeBtn.Visible = false;
                MessageHelper.PublishMessage("Ödeme planı oluşturulamiyor. Lütfen sözleşmenin başlama ve bitiş tarihlerini girerek tekrar deneyin.", ProjeConstants.MESAJ_HATA);
            }
            else //if (SozlesmeGecerli(kiraSozlesme.SozBitTar))
            {
                if (!OdemePlaniVarMi(kiraSozlesme))
                {
                    OdemePlaniEkleBtn.Visible = true;
                    OdemePlaniSilBtn.Visible = false;
                    GecikmeZammmiGunlukBtn.Visible = false;
                    GecikmeZammmiAylikBtn.Visible = false;
                }
                else// odeme planı var
                {
                    OdemePlaniEkleBtn.Visible = false;
                    OdemePlaniSilBtn.Visible = true;
                    GecikmeZammmiGunlukBtn.Visible = !kiraSozlesme.GecikmeZammiTipi.Equals(ProjeConstants.KIRASOZLESME_GECIKMEZAMMI_GUNLUK); 
                    GecikmeZammmiAylikBtn.Visible = !kiraSozlesme.GecikmeZammiTipi.Equals(ProjeConstants.KIRASOZLESME_GECIKMEZAMMI_AYLIK);
                }
            }
            if (!kiraSozlesme.Aktif)//bitmiş bir kira sözleşmesi
            {
                TitleLbl.CssClass = "col-form-label text-secondary font-weight-bold mb-1";
                AdiLbl.CssClass = "col-form-label text-secondary";
                DosyaNoTxt.CssClass = "col-form-label text-secondary float-right";
                OdemePlaniListBtn.Visible = false;
            }
        }
        private bool OdemePlaniniKontrolEtVeOlustur()
        {
            bool isSaved = false;
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            kiraSozlesme = kiraSozlesme.Select(KiraSozlesmeIdQS.ConvertToInt());
            // kira sozlesmesinin sozbastar ve sozbittar alanları dolu değilse 
            if (string.IsNullOrEmpty(kiraSozlesme.SozBasTar.ConvertToDatetimeEmptyIfNull()) || string.IsNullOrEmpty(kiraSozlesme.SozBitTar.ConvertToDatetimeEmptyIfNull()))
            {
                OdemePlaniEkleBtn.Visible = false;
            }
            else
            {
                if (!OdemePlaniVarMi(kiraSozlesme))
                {
                    OdemePlani op = new OdemePlani();
                    isSaved = op.OdemePlaniOlustur(kiraSozlesme);
                }
                else// odeme planı var
                {
                    MessageHelper.PublishMessage("Ödeme Planı Oluşturulamaz. Zaten geçerli bir ödeme planı mevcut.", ProjeConstants.MESAJ_HATA);
                }
            }

            if (isSaved)
            {
                MessageHelper.PublishMessage("Ödeme Planı Oluşturuldu. Ödeme planı geçerlilik Tarihi" + kiraSozlesme.SozBitTar + " -" + kiraSozlesme.SozBitTar, ProjeConstants.MESAJ_BASARILI, 2000);
            }
            return isSaved;
        }
        private bool OdemePlaniVarMi(KiraSozlesme kiraSozlesme)
        {
            bool retval = false;

            OdemePlani odemePlaniDao = new OdemePlani();
            List<OdemePlani> list = odemePlaniDao.SelectBySozlesmeId(kiraSozlesme.Id);
            if (list.Count > 0)
            {
                retval = true;
            }

            return retval;
        }
        private void GecikmeZammiBilgileriniDoldur(int odemePlaniId)
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);

            OdemePlani odemePlani = new OdemePlani();
            odemePlani = odemePlani.Select<OdemePlani>(odemePlaniId);

            if (odemePlani != null)
            {
                OdemeAyrinti odemeAyrinti = new OdemeAyrinti();
                List<OdemeAyrinti> odemeAyrintiList = odemeAyrinti.Select(odemePlani);
                foreach (OdemeAyrinti item in odemeAyrintiList)
                {
                    TableRow row = new TableRow();

                    TableCell TarihAraligiCell = new TableCell();
                    TarihAraligiCell.Text = item.IlkTarih.ConvertToDatetimeEmptyIfNull() + " - " + item.SonTarih.ConvertToDatetimeEmptyIfNull();

                    TableCell ZamOraniCell = new TableCell();
                    ZamOraniCell.Text = item.GecikmeZammiOrani.ToString("N", culturInfo);
                    ZamOraniCell.CssClass = "text-right";

                    TableCell ZamTutariCell = new TableCell();
                    ZamTutariCell.Text = item.GecikmeZammiTutari.ToString("N", culturInfo);
                    ZamTutariCell.CssClass = "text-right";

                    TableCell AnaParaCell = new TableCell();
                    AnaParaCell.Text = item.AnaPara.ToString("N", culturInfo);

                    TableCell OdenenTutarCell = new TableCell();
                    OdenenTutarCell.CssClass = "text-right";
                    OdenenTutarCell.Text = (item.OdenenTutar).ToString("N", culturInfo);

                    TableCell KalanAnaParaCell = new TableCell();
                    KalanAnaParaCell.Text = item.KalanAnaPara.ToString("N", culturInfo);

                    TableCell AciklamaCell = new TableCell();
                    AciklamaCell.Text = item.Aciklama.ToString();

                    row.Controls.Add(TarihAraligiCell);
                    row.Controls.Add(AnaParaCell);
                    row.Controls.Add(OdenenTutarCell);
                    row.Controls.Add(KalanAnaParaCell);
                    row.Controls.Add(ZamOraniCell);
                    row.Controls.Add(ZamTutariCell);
                    row.Controls.Add(AciklamaCell);
                    GecikmeZammiTable.Controls.Add(row);
                }
            }
        }
        private void RedirectToPage(string pageUrl)
        {
            try
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                int index = currentUrl.LastIndexOf("?") < 0 ? currentUrl.Length : currentUrl.LastIndexOf("?");

                string rawUrl = currentUrl.Substring(0, index);
                string newUrl = rawUrl.Substring(0, rawUrl.LastIndexOf("/")) + "/" + pageUrl;
                Page.Response.Redirect(newUrl);
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void OdeBtn_Click(object sender, EventArgs e)
        {
            try
            {
                KiraSozlesme kiraSozlesme = new KiraSozlesme();
                kiraSozlesme = kiraSozlesme.SelectByKiraciIdTarih(KiraciIdQS.ConvertToInt(), OdemeTarihiTxt.Value.ConvertToDatetime());
                if (kiraSozlesme != null)
                {
                    DateTime odemeTarihi = OdemeTarihiTxt.Value.ConvertToDatetime();
                    decimal odemeTutari = OdenenTutarTxt.Value.ConvertToDecimal();
                    int odemeId = 0;
                    bool odemeYapildiMi = TBYSOrtak.OdemeYap(kiraSozlesme, odemeTarihi, odemeTutari, AciklamaTxt.Text, ref odemeId);
                    RedirectToPage(ProjeConstants.PAGE_ODEMEPLANI + "?SenderApp=" + SenderAppQS + "&KiraSozlesmeId=" + kiraSozlesme.Id);
                }
                else
                {
                    throw (new Exception("Kira Sözleşmesi mevcut değil."));
                }
            }
            catch (Exception exception)
            {
                ExceptionHelper exHelper = new ExceptionHelper(exception);
                exHelper.Exceptions.Add(new Exception("Ödeme Yapılamadı."));
                exHelper.PublishException();
            }

            //try
            //{
            //    KiraSozlesme kiraSozlesme = new KiraSozlesme();
            //    kiraSozlesme = kiraSozlesme.Select<KiraSozlesme>(KiraSozlesmeIdQS.ConvertToInt());
            //    if (kiraSozlesme != null)
            //    {
            //        DateTime odemeTarihi = OdemeTarihiTxt.Value.ConvertToDatetime();
            //        int yil = odemeTarihi.Year;
            //        int ay = odemeTarihi.Month;

            //        OdemePlani odemePlaniDao = new OdemePlani();
            //        List<OdemePlani> list = odemePlaniDao.SelectBySozlesmeId(kiraSozlesme.Id);
            //        if (list.Count > 0)
            //        {
            //            OdemePlani odemePlani = list[1];//ilk taksit, lits[0] da devir kaydı var

            //            if (odemeTarihi < odemePlani.OdemeBasTar)// ödeme başlama tarihinden önce ödeme yapılmış
            //            {
            //                odemePlani = odemePlani.SelectIlkOdemePlaniBySozlesmeId(kiraSozlesme.Id);//odemeyi ilk OdemePlanina kaydet 
            //                Odeme odeme = new Odeme();
            //                odeme = odeme.OdemeyiKaydetOdemePlaniniGuncelle(kiraSozlesme, odemePlani, OdemeTarihiTxt.Value.ConvertToDatetime(), OdenenTutarTxt.Value.ConvertToDecimal(), AciklamaTxt.Text, CurrentUserName);
            //                RedirectToPage(ProjeConstants.PAGE_ODEMEPLANI + "?SenderApp=" + SenderAppQS + "&KiraSozlesmeId=" + kiraSozlesme.Id);
            //            }
            //            else if (odemeTarihi > odemePlani.OdemeBitTar)//ödeme bitiş tarihinden sonra ödeme yapılmış
            //            {
            //                //
            //                // Taksit SB
            //                // sözleşme yıllıksa ve ödeme sözleşme bitmeden yapılmışsa
            //                if ((kiraSozlesme.OdemeSekli == ProjeConstants.KIRA_ODMSEKLI_YILLIK) &&
            //                    odemeTarihi < kiraSozlesme.SozBitTar)
            //                {
            //                    if (kiraSozlesme.SozBitTar >= odemeTarihi)
            //                    {
            //                        odemePlani = odemePlani.SelectBySozlesmeIdOdemeTarihi(kiraSozlesme.Id, OdemeTarihiTxt.Value.ConvertToDatetime());
            //                    }
            //                    else //1 taksitte ödenenlerde odemem plani tarihe göre bulunanamayabiliyor o yüzden son takside eklesin
            //                    {
            //                        odemePlani = odemePlani.SelectSonOdemePlaniBySozlesmeId(kiraSozlesme.Id);  //odemeyi son OdemePlanina kaydet   
            //                    }
            //                    if (odemePlani == null)
            //                        throw (new Exception("Ödeme Planı mevcut değil."));
            //                    else
            //                    {
            //                        Odeme odeme = new Odeme();
            //                        odeme = odeme.OdemeyiKaydetOdemePlaniniGuncelle(kiraSozlesme, odemePlani, OdemeTarihiTxt.Value.ConvertToDatetime(), OdenenTutarTxt.Value.ConvertToDecimal(), AciklamaTxt.Text, CurrentUserName);
            //                        RedirectToPage(ProjeConstants.PAGE_ODEMEPLANI + "?SenderApp=" + SenderAppQS + "&KiraSozlesmeId=" + kiraSozlesme.Id);

            //                    }
            //                }
            //                else

            //                {
            //                    //throw (new Exception("Ödeme Tarihi Sözleşme bitişinden sonra olamaz."));
            //                    odemePlani = odemePlani.SelectSonOdemePlaniBySozlesmeId(kiraSozlesme.Id);  //odemeyi son OdemePlanina kaydet   
            //                    if (odemePlani != null)
            //                    {
            //                        Odeme odeme = new Odeme();
            //                        odeme = odeme.OdemeyiKaydetOdemePlaniniGuncelle(kiraSozlesme, odemePlani, OdemeTarihiTxt.Value.ConvertToDatetime(), OdenenTutarTxt.Value.ConvertToDecimal(), AciklamaTxt.Text, CurrentUserName);
            //                        RedirectToPage(ProjeConstants.PAGE_ODEMEPLANI + "?SenderApp=" + SenderAppQS + "&KiraSozlesmeId=" + kiraSozlesme.Id);
            //                    }
            //                    else
            //                    {
            //                        throw (new Exception("Ödeme Planı Bulunamadı."));
            //                    }

            //                }

            //            }
            //            else //odeme baş- bit arasında yapılmış
            //            {
            //                odemePlani = odemePlani.SelectBySozlesmeIdOdemeTarihi(kiraSozlesme.Id, OdemeTarihiTxt.Value.ConvertToDatetime()); 
            //                if (odemePlani == null)
            //                {
            //                    odemePlani = odemePlani.SelectSonOdemePlaniBySozlesmeId(kiraSozlesme.Id);  //odemeyi son OdemePlanina kaydet   
            //                }
            //                if (odemePlani == null)
            //                    throw (new Exception("Ödeme Planı mevcut değil."));
            //                else
            //                {
            //                    Odeme odeme = new Odeme();
            //                    odeme = odeme.OdemeyiKaydetOdemePlaniniGuncelle(kiraSozlesme, odemePlani, OdemeTarihiTxt.Value.ConvertToDatetime(), OdenenTutarTxt.Value.ConvertToDecimal(), AciklamaTxt.Text, CurrentUserName);
            //                    RedirectToPage(ProjeConstants.PAGE_ODEMEPLANI + "?SenderApp=" + SenderAppQS + "&KiraSozlesmeId=" + kiraSozlesme.Id);

            //                }
            //            }

            //        }
            //        throw (new Exception("Ödeme Planı mevcut değil."));
            //    }
            //    throw (new Exception("Kira Sözleşmesi mevcut değil."));
            //}
            //catch (Exception ex)
            //{
            //    ExceptionHelper exHelper = new ExceptionHelper(ex);
            //    exHelper.Exceptions.Add(new Exception("Ödeme Yapılamadı."));
            //    exHelper.PublishException();
            //}
        }
        private void OdemeGoruntule(KiraSozlesme kiraSozlesme, int yil, int ay, int odemePlaniId)
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            Kiraci kiraci = new Kiraci();
            kiraci = kiraci.Select<Kiraci>(kiraSozlesme.KiraciId);
            BaslikLbl.Text = "Kiracı: " + kiraci.Adi + " " + kiraci.Soyadi;
            DateTime tarih = new DateTime(yil, ay, 1);
            TarihLbl.Text = "(" + tarih.ToString("MMMM") + " " + yil + ")";
            int SiraNo = 1;

            Odeme odemeDao = new Odeme();
            List<Odeme> list = odemeDao.SelectBySozlesmeIdOdemePlaniId(kiraSozlesme.Id, odemePlaniId);
            foreach (Odeme odeme in list)
            {
                TableRow row = new TableRow();
                TableCell SiraNoCell = new TableCell();
                SiraNoCell.Text = SiraNo++ + "";
                row.Controls.Add(SiraNoCell);

                TableCell OdemeTarCell = new TableCell();
                OdemeTarCell.Text = odeme.OdemeTarihi.ConvertToDatetimeEmptyIfNull();
                row.Controls.Add(OdemeTarCell);

                TableCell OdenenTutarCell = new TableCell();
                OdenenTutarCell.Text = odeme.OdenenTutar.ToString("N", culturInfo);
                OdenenTutarCell.CssClass = "text-right";
                row.Controls.Add(OdenenTutarCell);

                TableCell AciklamaCell = new TableCell();
                AciklamaCell.Text = odeme.Aciklama;
                row.Controls.Add(AciklamaCell);

                OdemePlaniTable.Controls.Add(row);
            }

            var jsString = " $('#OdemePlaniModal').modal({ backdrop: false });";
            ScriptManager.RegisterStartupScript((Page)System.Web.HttpContext.Current.Handler, typeof(Page), System.Guid.NewGuid().ToString(), jsString, true);
        }
        private bool OdemePlaniniKontrolEtVeSil(KiraSozlesme kiraSozlesme)
        {
            bool silindi = false;
            OdemePlani odemePlaniDao = new OdemePlani();
            List<OdemePlani> list = odemePlaniDao.SelectBySozlesmeId(kiraSozlesme.Id);
            if (list.Count > 0)
            {
                OdemePlani op = list[0];
                if (op.SozlesmeId == kiraSozlesme.Id)
                {
                    silindi = op.DeleteBySozlesmeId(kiraSozlesme.Id);
                    if (silindi)
                    {
                        //Odemeler de silinsin
                        //Odeme odemeDao = new Odeme();
                        //List<Odeme> odemeListesi = odemeDao.SelectBySozlesmeId(kiraSozlesme.Id);
                        //if (odemeListesi.Count > 0)
                        //{
                        //    odemeDao.DeleteBySozlesmeId(kiraSozlesme.Id);
                        //}
                        OdemeAyrinti odemeAyrintiDao = new OdemeAyrinti();
                        List<OdemeAyrinti> odemeAyrintiListesi = odemeAyrintiDao.SelectBySozlesmeId(kiraSozlesme.Id);
                        if (odemeAyrintiListesi.Count > 0)
                        {
                            odemeAyrintiDao.DeleteBySozlesmeId(kiraSozlesme.Id);
                        }
                    }
                }
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));
                newUrl += "/" + ProjeConstants.PAGE_ODEMEPLANI + "?KiraSozlesmeId=" + kiraSozlesme.Id;
                Page.Response.Redirect(newUrl, true);
            }
            return silindi;
        }
        protected void OdemePlaniSilBtn_Click(object sender, EventArgs e)
        {
            try
            {
                bool odemePlaniSilindi = false;
                KiraSozlesme kiraSozlesme = new KiraSozlesme();
                kiraSozlesme = kiraSozlesme.Select<KiraSozlesme>(KiraSozlesmeIdQS.ConvertToInt());
                if (kiraSozlesme != null)
                {
                    int kiraSozlesmeId = kiraSozlesme.Id;
                    odemePlaniSilindi = OdemePlaniniKontrolEtVeSil(kiraSozlesme);
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exhelper = new ExceptionHelper(ex);
                exhelper.PublishException();
            }
        }
        protected void PrevBtn_Click(object sender, EventArgs e)
        {
            try
            {
                KiraSozlesme kiraSozlesme = new KiraSozlesme();
                kiraSozlesme = kiraSozlesme.Select<KiraSozlesme>(KiraSozlesmeIdQS.ConvertToInt());
                KiraSozlesme oncekiKiraSozlesme = kiraSozlesme.SelectPrev();
                if (!kiraSozlesme.Aktif)
                {
                    oncekiKiraSozlesme = kiraSozlesme.SelectPrevBiten();
                }


                RedirectToPage(ProjeConstants.PAGE_ODEMEPLANI + "?SenderApp=" + SenderAppQS + "&KiraSozlesmeId=" + oncekiKiraSozlesme.Id);
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }


        }
        protected void NextBtn_Click(object sender, EventArgs e)
        {
            try
            {
                KiraSozlesme kiraSozlesme = new KiraSozlesme();
                kiraSozlesme = kiraSozlesme.Select<KiraSozlesme>(KiraSozlesmeIdQS.ConvertToInt());
                KiraSozlesme sonrakiKiraSozlesme = kiraSozlesme.SelectNext();
                if (!kiraSozlesme.Aktif)
                {
                    sonrakiKiraSozlesme = kiraSozlesme.SelectNextBiten();
                }
                RedirectToPage(ProjeConstants.PAGE_ODEMEPLANI + "?SenderApp=" + SenderAppQS + "&KiraSozlesmeId=" + sonrakiKiraSozlesme.Id);
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void KiraBedeliniDegistirBtn_Click(object sender, EventArgs e)
        {
            OdemePlani odemePlani = new OdemePlani();
            odemePlani = odemePlani.Select<OdemePlani>(HiddenOdemePlaniId.Value.ConvertToInt());
            if (odemePlani != null)
            {
                odemePlani.KiraBedeli = YeniKiraBedeliTxt.Text.ConvertToDecimal();
                odemePlani.Update();
                RedirectToPage(ProjeConstants.PAGE_ODEMEPLANI + "?SenderApp=" + SenderAppQS + "&KiraSozlesmeId=" + KiraSozlesmeIdQS);
            }
        }

        protected void GecikmeZammmiGunlukBtn_Click(object sender, EventArgs e)
        {
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            kiraSozlesme = kiraSozlesme.Select<KiraSozlesme>(KiraSozlesmeIdQS.ConvertToInt());
            if (kiraSozlesme != null)
            {
                kiraSozlesme.GecikmeZammiTipi = ProjeConstants.KIRASOZLESME_GECIKMEZAMMI_GUNLUK;
                kiraSozlesme.Update();
            }

            RedirectToPage(ProjeConstants.PAGE_ODEMEPLANI + "?SenderApp=" + SenderAppQS + "&KiraSozlesmeId=" + KiraSozlesmeIdQS);
        }
        protected void GecikmeZammmiAylikBtn_Click(object sender, EventArgs e)
        {
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            kiraSozlesme = kiraSozlesme.Select<KiraSozlesme>(KiraSozlesmeIdQS.ConvertToInt());
            if (kiraSozlesme != null)
            {
                kiraSozlesme.GecikmeZammiTipi = ProjeConstants.KIRASOZLESME_GECIKMEZAMMI_AYLIK;
                kiraSozlesme.Update();
            }

            RedirectToPage(ProjeConstants.PAGE_ODEMEPLANI + "?SenderApp=" + SenderAppQS + "&KiraSozlesmeId=" + KiraSozlesmeIdQS);
        }
        protected void VadeDegistirBtn_Click(object sender, EventArgs e)
        {
            OdemePlani odemePlani = new OdemePlani();
            odemePlani = odemePlani.Select<OdemePlani>(HiddenOdemePlaniId.Value.ConvertToInt());
            if (odemePlani != null)
            {
                odemePlani.VadeBasTar = VadeBasTarTxt.Value.ConvertToDatetime();
                odemePlani.VadeBitTar = VadeBitTarTxt.Value.ConvertToDatetime();
                odemePlani.Update();
                RedirectToPage(ProjeConstants.PAGE_ODEMEPLANI + "?SenderApp=" + SenderAppQS + "&KiraSozlesmeId=" + KiraSozlesmeIdQS);
            }
        }
        protected void YeniOdemeGirisiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_ODEME_GIRIS + "?KiraciId=" + KiraciIdQS + "&KiraSozlesmeId="+KiraSozlesmeIdQS);
        }

    }
}
