using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.OdemeGirisWP
{
    [ToolboxItemAttribute(false)]
    public partial class OdemeGirisWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public OdemeGirisWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private IFormatProvider cultureInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
        private string SecilenAyQS
        {
            get
            {

                if (ViewState["SecilenAy"] == null)
                {
                    if (Page.Request.QueryString["SecilenAy"] != null)
                    {
                        ViewState["SecilenAy"] = Page.Request.QueryString["SecilenAy"];
                    }
                    else
                    {
                        ViewState["SecilenAy"] = DateTime.Today.Month.ToString();
                    }
                }
                return ViewState["SecilenAy"].ToString();
            }

            set
            {
                ViewState["SecilenAy"] = value;
            }
        }
        private string SecilenYilQS
        {
            get
            {

                if (ViewState["SecilenYil"] == null)
                {
                    if (Page.Request.QueryString["SecilenYil"] != null)
                    {
                        ViewState["SecilenYil"] = Page.Request.QueryString["SecilenYil"];
                    }
                    else
                    {
                        ViewState["SecilenYil"] = DateTime.Today.Year.ToString();
                    }
                }
                return ViewState["SecilenYil"].ToString();
            }

            set
            {
                ViewState["SecilenYil"] = value;
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
        private string OdemePlaniIdQS
        {
            get
            {

                if (ViewState["OdemePlaniId"] == null)
                {
                    if (Page.Request.QueryString["OdemePlaniId"] != null)
                    {
                        ViewState["OdemePlaniId"] = Page.Request.QueryString["OdemePlaniId"];
                    }
                    else
                    {
                        ViewState["OdemePlaniId"] = string.Empty;
                    }
                }
                return ViewState["OdemePlaniId"].ToString();
            }

            set
            {
                ViewState["OdemePlaniId"] = value;
            }
        }
        private string OdemeIdQS
        {
            get
            {

                if (ViewState["OdemeId"] == null)
                {
                    if (Page.Request.QueryString["OdemeId"] != null)
                    {
                        ViewState["OdemeId"] = Page.Request.QueryString["OdemeId"];
                    }
                    else
                    {
                        ViewState["OdemeId"] = string.Empty;
                    }
                }
                return ViewState["OdemeId"].ToString();
            }

            set
            {
                ViewState["OdemeId"] = value;
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
                if (OdemeIdQS.ConvertToInt() > 0)
                {
                    //odeme düzenleme
                    OdemeDuzenleAc();
                }
                else
                {
                    //odeme Girişi
                    OdemeGirisiAc();
                } 
            }
        }

        private void OdemeGirisiAc()
        {
            TitleLbl.Text = "Yeni Ödeme Girişi";
            TitleLbl.CssClass = "col-form-label text-success fw-bold mb-1";
            KaydetBtn.Visible = false;
            GuncelleBtn.Visible = false;
            SilBtn.Visible = false;

            try
            {
                Kiraci kiraci = new Kiraci();
                kiraci = kiraci.Select(KiraciIdQS.ConvertToInt());
                if (kiraci != null)
                {
                    KiraciAdiLbl.Text = kiraci.Adi + kiraci.Soyadi;
                    KiraciAdresiLbl.Text = kiraci.Adres;
                    OdemeTutariTxt.Text = string.Empty;

                    int ay = DateTime.Today.Month;
                    int yil = DateTime.Today.Year;
                    if (SecilenAyQS.ConvertToInt() > 0)
                    {
                        ay = SecilenAyQS.ConvertToInt();
                    }
                    if (SecilenYilQS.ConvertToInt() > 0)
                    {
                        yil = SecilenYilQS.ConvertToInt();
                    }
                    DateTime tarih = DateTime.Today; new DateTime(yil, ay, 1);
                    OdemeTarihiTxt.Text = tarih.ConvertToDatetimeEmptyIfNull();
                    OdemeSaatiTxt.Text = "00:00";

                    SozlesmeDDLDoldur(kiraci.Id, KiraSozlesmeIdQS.ConvertToInt(), SozlesmeDDL, OdemeTarihiTxt.Text);
                    OdemePlaniDDLDoldur(string.IsNullOrEmpty(KiraSozlesmeIdQS) ? SozlesmeDDL.SelectedItem.Value.ConvertToInt() : KiraSozlesmeIdQS.ConvertToInt()
                        , OdemePlaniIdQS.ConvertToInt(), OdemePlaniDDL);
                    KaydetBtn.Visible = true;
                    OdemePlani odemePlani = new OdemePlani();
                    odemePlani = odemePlani.Select<OdemePlani>(OdemePlaniDDL.SelectedItem.Value.ConvertToInt());
                    OdemeTutariTxt.Text = odemePlani == null ? "" : odemePlani.KiraBedeli.ToString("N", cultureInfo);
                }
                else
                {
                    KaydetBtn.Visible = false;
                    GuncelleBtn.Visible = false;
                    SilBtn.Visible = false;
                    MessageHelper.PublishMessage("Ödeme yapmak için bir kiracı seçmelisiniz.", ProjeConstants.MESAJ_HATA);
                }
            }
            catch (Exception exception)
            {

                ExceptionHelper exceptionHelper = new ExceptionHelper(exception);
                exceptionHelper.PublishException();
            }
        }

        private void OdemeDuzenleAc()
        {
            TitleLbl.Text = "Ödeme Düzenleme";
            TitleLbl.CssClass = "col-form-label text-primary fw-bold mb-1";
            KaydetBtn.Visible = false;
            GuncelleBtn.Visible = false;
            SilBtn.Visible = false;
            try
            {
                Odeme odeme = new Odeme();
                odeme = odeme.Select(OdemeIdQS.ConvertToInt());
                if (odeme != null)
                {
                    KiraciIdQS = odeme.KiraciId.ToString();
                    Kiraci kiraci = new Kiraci();
                    kiraci = kiraci.Select(odeme.KiraciId);
                    if (kiraci != null)
                    {
                        KiraciAdiLbl.Text = kiraci.Adi + kiraci.Soyadi;
                        KiraciAdresiLbl.Text = kiraci.Adres;
                        OdemeTutariTxt.Text = odeme.OdenenTutar.ToString("N", cultureInfo);
                        OdemeTarihiTxt.Text = odeme.OdemeTarihi.ConvertToDatetimeEmptyIfNull();
                        OdemeSaatiTxt.Text = odeme.OdemeTarihi.ToString("HH:mm");
                        AciklamaTxt.Text = odeme.Aciklama;
                        SozlesmeDDLDoldur(odeme.KiraciId, odeme.SozlesmeId, SozlesmeDDL, OdemeTarihiTxt.Text);
                        OdemePlaniDDLDoldur(odeme.SozlesmeId, odeme.OdemePlaniId, OdemePlaniDDL);
                        GuncelleBtn.Visible = true;
                        SilBtn.Visible = true;
                    }
                    else
                    {
                        KaydetBtn.Visible = false;
                        GuncelleBtn.Visible = false;
                        SilBtn.Visible = false;
                        MessageHelper.PublishMessage("Ödeme yapmak için bir kiracı seçmelisiniz.", ProjeConstants.MESAJ_HATA);
                    }
                }
            }
            catch (Exception exception)
            {

                ExceptionHelper exceptionHelper = new ExceptionHelper(exception);
                exceptionHelper.PublishException();
            }

        }

        private void OdemePlaniDDLDoldur(int sozlesmeId, int odemePlaniId, DropDownList odemePlaniIdDDL)
        {
            odemePlaniIdDDL.Items.Clear();

            if (sozlesmeId > 0)
            {
                OdemePlani odemePlaniDao = new OdemePlani();
                List<OdemePlani> odemePlaniList = odemePlaniDao.SelectBySozlesmeId(sozlesmeId);
                foreach (var item in odemePlaniList)
                {
                    if (item.Sira == 0)
                    {
                        continue;
                    }
                    else
                    {
                        string text = item.VadeBitTar.ToString("dd MMMM yyyy", cultureInfo);
                        string value = item.Id.ToString();
                        ListItem li = new ListItem(text, value);
                        odemePlaniIdDDL.Items.Add(li);
                    }
                }
            }
            ListItem odemeItem = new ListItem();
            odemeItem = odemePlaniIdDDL.Items.FindByValue(odemePlaniId.ToString());
            if (odemeItem != null)
            {
                odemePlaniIdDDL.SelectedValue = odemeItem.Value;
            }

        }
        private void SozlesmeDDLDoldur(int kiraciId, int sozlesmeId, DropDownList sozlesmeIdDDL, string odemetarihi)
        {
            int tempSozlesmeId = 0;
            int tempSozlesmeId1 = 0;
            sozlesmeIdDDL.Items.Clear();
            KiraSozlesme kiraSozlesmeDao = new KiraSozlesme();
            List<KiraSozlesme> kiraSozlesmeList = kiraSozlesmeDao.SelectByKiraciIdReturnList(kiraciId);
            foreach (var item in kiraSozlesmeList)
            {
                string text = item.SozBasTar.ConvertToDatetimeEmptyIfNull() + " - " + item.SozBitTar.ConvertToDatetimeEmptyIfNull();
                string value = item.Id.ToString();
                ListItem li = new ListItem(text, value);
                sozlesmeIdDDL.Items.Add(li);
                tempSozlesmeId1 = item.Id;
                if (odemetarihi.ConvertToDatetime() < item.SozBitTar)
                {
                    tempSozlesmeId = item.Id;
                }
            }
            ListItem sozlesmeItem = new ListItem();
            KiraSozlesmeIdQS = sozlesmeId < 1 ? (tempSozlesmeId<1? tempSozlesmeId1.ToString():tempSozlesmeId.ToString()) : sozlesmeId.ToString();
            sozlesmeItem = sozlesmeIdDDL.Items.FindByValue(KiraSozlesmeIdQS);
            if (sozlesmeItem != null)
            {
                sozlesmeIdDDL.SelectedValue = sozlesmeItem.Value;
            }
            else
            {
                throw new Exception("Geçerli bir sozlesme bulunamadı");
                
            }
        }
        protected void SozlesmeDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            OdemePlani odemePlani = new OdemePlani();
            int kiraSozlesmeId = SozlesmeDDL.SelectedItem.Value.ConvertToInt();

            odemePlani = odemePlani.SelectBySozlesmeIdOdemeTarihi(kiraSozlesmeId, OdemeTarihiTxt.Text.ConvertToDatetime());
            int odemePlaniId = 0;
            if (odemePlani != null)
            {
                odemePlaniId = odemePlani.Id;
            }
            OdemePlaniDDLDoldur(SozlesmeDDL.SelectedItem.Value.ConvertToInt(), odemePlaniId, OdemePlaniDDL);
        }
        protected void KaydetBtn_Click(object sender, EventArgs e)
        {
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            int sozlesmeId = SozlesmeDDL.SelectedItem.Value.ConvertToInt();
            kiraSozlesme = kiraSozlesme.Select(sozlesmeId);
            if (kiraSozlesme != null)
            {
                string sozlesmeStr = SozlesmeDDL.SelectedItem.Text;
                string odemePlaniStr = OdemePlaniDDL.SelectedItem.Text;
                DateTime odemeTarihi= UtilityHelper.TariheSaatEkle(OdemeTarihiTxt.Text.ConvertToDatetime(), OdemeSaatiTxt.Text);
                string odemeTarihiStr = odemeTarihi.ToString("dd.MM.yyyy HH:mm");
                string odenenTutarStr = OdemeTutariTxt.Text.ConvertToDecimal().ToString("N", cultureInfo);

                kaydetGuncelleSilHdn.Value = ProjeConstants.KAYDET;
                MessageLbl.Text = @"Ödeme tarihi=" + odemeTarihiStr + ", Ödeme Tutarı=" + odenenTutarStr + " şeklinde kaydedilecek ve " +
           @"
                        " + sozlesmeStr + " tarihli Kira Sözleşmesinin " + odemePlaniStr + " Son Ödeme Tarihli Ödeme Planına işlenecektir.";
                ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "OpenModalOnay();", true);

            }
            else
            {
                MessageHelper.PublishMessage("Sözleşme Bulunamadı", ProjeConstants.MESAJ_HATA);
            }
        }
        protected void GuncelleBtn_Click(object sender, EventArgs e)
        {
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            int sozlesmeId = SozlesmeDDL.SelectedItem.Value.ConvertToInt();
            kiraSozlesme = kiraSozlesme.Select(sozlesmeId);
            if (kiraSozlesme != null)
            {
                string sozlesmeStr = SozlesmeDDL.SelectedItem.Text;
                string odemePlaniStr = OdemePlaniDDL.SelectedItem.Text;
                DateTime odemeTarihi = UtilityHelper.TariheSaatEkle(OdemeTarihiTxt.Text.ConvertToDatetime(), OdemeSaatiTxt.Text);
                string odemeTarihiStr = odemeTarihi.ToString("dd.MM.yyyy HH:mm");
                string odenenTutarStr = OdemeTutariTxt.Text.ConvertToDecimal().ToString("N", cultureInfo);

                kaydetGuncelleSilHdn.Value = ProjeConstants.GUNCELLE;
                MessageLbl.Text = @"Ödeme tarihi=" + odemeTarihiStr + ", Ödeme Tutarı=" + odenenTutarStr + " şeklinde güncellenecek ve " +
           @"
                        " + sozlesmeStr + " tarihli Kira Sözleşmesinin " + odemePlaniStr + " Son Ödeme Tarihli Ödeme Planına kaydedilecektir";
                ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "OpenModalOnay();", true);

            }
            else
            {
                MessageHelper.PublishMessage("Sözleşme Bulunamadı", ProjeConstants.MESAJ_HATA);
            }
        }
        protected void SilBtn_Click(object sender, EventArgs e)
        {
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            int sozlesmeId = SozlesmeDDL.SelectedItem.Value.ConvertToInt();
            kiraSozlesme = kiraSozlesme.Select(sozlesmeId);
            if (kiraSozlesme != null)
            {
                ModalLbl.Text = "Ödeme Silinecek";
                ModalLbl.CssClass = "col-form-label text-primary fw-bold";
                string sozlesmeStr = SozlesmeDDL.SelectedItem.Text;
                string odemePlaniStr = OdemePlaniDDL.SelectedItem.Text;
                string odemeTarihiStr = OdemeTarihiTxt.Text.ConvertToDatetime().ConvertToDatetimeEmptyIfNull();
                string odemeTutariStr = OdemeTutariTxt.Text.ConvertToDecimal().ToString("N", cultureInfo);
                kaydetGuncelleSilHdn.Value = ProjeConstants.SIL;
                MessageLbl.Text = @"Ödeme tarihi=" + odemeTarihiStr + ", Ödeme Tutarı=" + odemeTutariStr + " şeklinde silinecek ve " +
           @"
                        " + sozlesmeStr + " tarihli Kira Sözleşmesinin " + odemePlaniStr + " Son Ödeme Tarihli Ödeme Planına işlenecektir.";
                ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "OpenModalOnay();", true);

            }
            else
            {
                MessageHelper.PublishMessage("Sözleşme Bulunamadı", ProjeConstants.MESAJ_HATA);
            }
        }
        protected void OnaylaBtn_Click(object sender, EventArgs e)
        {
            string ay = OdemeTarihiTxt.Text.ConvertToDatetime().Month.ToString();
            string yil = OdemeTarihiTxt.Text.ConvertToDatetime().Year.ToString();
            SecilenAyQS = ay;
            SecilenYilQS = yil;
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            kiraSozlesme = kiraSozlesme.Select(SozlesmeDDL.SelectedItem.Value.ConvertToInt());
            if (kiraSozlesme != null)
            {
                DateTime odemeTarihi = UtilityHelper.TariheSaatEkle(OdemeTarihiTxt.Text.ConvertToDatetime(),
                        string.IsNullOrEmpty(OdemeSaatiTxt.Text) ? "00:00" : OdemeSaatiTxt.Text);
                if (kaydetGuncelleSilHdn.Value.Equals(ProjeConstants.KAYDET))
                {
                    
                    int odemeId = 0;
                    bool odemeYapildiMi = TBYSOrtak.OdemeYap(kiraSozlesme, odemeTarihi, OdemeTutariTxt.Text.ConvertToDecimal(), AciklamaTxt.Text, ref odemeId);
                    if (odemeYapildiMi)
                    {
                        

                        RedirectToPage(ProjeConstants.PAGE_KIRACI_AYLIKODEME + "?Mesaj=true&KiraciId=" + KiraciIdQS + "&SecilenAy="+ay+"&SecilenYil="+yil);

                    }
                }
                else if (kaydetGuncelleSilHdn.Value.Equals(ProjeConstants.GUNCELLE))
                {
                    ModalLbl.Text = "Ödeme Güncellenecek";
                    ModalLbl.CssClass= "col-form-label text-primary fw-bold";
                    Odeme oncekiOdeme = new Odeme();
                    oncekiOdeme = oncekiOdeme.Select(OdemeIdQS.ConvertToInt());
                    if (oncekiOdeme != null)
                    {
                        int yeniOdemePlaniId = OdemePlaniDDL.SelectedItem.Value.ConvertToInt();
                        decimal yeniOdemeTutari = OdemeTutariTxt.Text.ConvertToDecimal();

                        Odeme odemeDao = new Odeme();
                        bool guncellendiMi = odemeDao.OdemeyiVeOdemePlaniniGuncelle(kiraSozlesme.Id, oncekiOdeme.OdemePlaniId, yeniOdemePlaniId,
                            oncekiOdeme.Id, odemeTarihi, yeniOdemeTutari, AciklamaTxt.Text, CurrentUserName);
                        if (guncellendiMi)
                        {
                            RedirectToPage(ProjeConstants.PAGE_KIRACI_AYLIKODEME + "?Mesaj=true&KiraciId=" + KiraciIdQS + "&SecilenAy=" + SecilenAyQS + "&SecilenYil=" + SecilenYilQS);
                        }
                            
                    }
                   
                }
                else if (kaydetGuncelleSilHdn.Value.Equals(ProjeConstants.SIL))
                {
                    if (kiraSozlesme != null)
                    {
                        Odeme silinecekOdeme = new Odeme();
                        silinecekOdeme = silinecekOdeme.Select(OdemeIdQS.ConvertToInt());

                        Odeme odemeDao = new Odeme();
                        bool silindiMi = odemeDao.OdemeyiSilOdemePlaniniGuncelle(silinecekOdeme.Id, string.Empty, silinecekOdeme.OdemePlaniId, CurrentUserName);
                        if (silindiMi)
                        {
                            RedirectToPage(ProjeConstants.PAGE_KIRACI_AYLIKODEME + "?KiraciId=" + KiraciIdQS + "&SecilenYil=" + SecilenYilQS + "&SecilenAy=" + SecilenAyQS);
                        }

                    }
                    else
                    {
                        MessageHelper.PublishMessage("Sözleşme Bulunamadı", ProjeConstants.MESAJ_HATA);
                    }
                }


            }
            else
            {
                MessageHelper.PublishMessage("Sözleşme Bulunamadı", ProjeConstants.MESAJ_HATA);
            }

            ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "CloseModalOnay();", true);
        }
        private void KiraciModalAc()
        {
            TabloModalOlustur();
            UtilityHelper.ScriptCalistir("OpenKiraciSecModal();");
        }
        private void TabloModalOlustur()
        {
            var jsonData = GetKiraciData(); //veri çekilip json a çeviriliyor
            var jsString = CreateModalDataTable(jsonData); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);
        }

        private string CreateModalDataTable(string jsonData)
        {
            string tableString = @"
            if ( $.fn.DataTable.isDataTable('#CustomModalDataTable') ) {
              $('#CustomModalDataTable').DataTable().destroy();
            }
            $('#CustomModalDataTable tbody').empty();

            jQuery('#CustomModalDataTable').DataTable({
            data: " + jsonData + @",
            'rowCallback': function(row, data, index) {
                if (data.Aktif != 1) {
                        $(row).addClass('table-danger');  // Bootstrap kırmızı tonu
                    } else {
                        $(row).addClass('table-success'); // Yeşil tonu
                    }
                },
            columnDefs:[
                {targets:5, render:function(data, type, row, meta){
                    var link='<a CausesValidation=\'false\' href=# onclick=CallButtonClick('+row.KiraciId + '); class=\'btn btn-outline-primary \'>Seç</a>';
    
                    return link;
                }}],   
            columns: [
                { data: 'KiraciId' },
                { data: 'Adi' },
                { data: 'TCKimlikNo' },
                { data: 'IlIlce' },
                { data: 'Adres' },
                { data: 'KiraciId' }
            ],
            'order': [[1, 'asc']],//AdiSoyadi Sıralı
            'language': {
                'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
            },
            responsive: true,
            dom: 'fpirt',

        });
            ";

            return tableString;
        }
        private string GetKiraciData()
        {
            Kiraci kiraci = new Kiraci();
            string json = kiraci.SelectAllReturnJson();

            return json;
        }
        protected void KiraciSecBtn_Click(object sender, EventArgs e)
        {
            KiraciModalAc();
        }
        protected void KiraciSecNowBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_ODEME_GIRIS + "?KiraciId=" + paramKiraciIdLbl.Value.ConvertToInt());
        }
        protected void KiraciAylikOdemeBtn_Click(object sender, EventArgs e)
        {
            string ay = OdemeTarihiTxt.Text.ConvertToDatetime().Month.ToString();
            string yil = OdemeTarihiTxt.Text.ConvertToDatetime().Year.ToString();
            RedirectToPage(ProjeConstants.PAGE_KIRACI_AYLIKODEME + "?KiraciId=" + KiraciIdQS + "&SecilenAy=" + ay + "&SecilenYil=" + yil);
        }
        protected void OdemePlaniDDL_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        protected void KiraciBtn_Click(object sender, EventArgs e)
        {

            Kiraci kiraci = new Kiraci();
            kiraci = kiraci.Select(KiraciIdQS.ConvertToInt());
            if (kiraci != null)
            {
                RedirectToPage(ProjeConstants.PAGE_KIRACI_GIRIS + "?DestinationApp=KD&SenderApp=KL&KiraciId=" + KiraciIdQS);
            }
            else
            {
                MessageHelper.PublishMessage("Kiracı Bulunamadı", ProjeConstants.MESAJ_HATA);
            }


        }
        protected void SozlesmeBtn_Click(object sender, EventArgs e)
        {
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            kiraSozlesme = kiraSozlesme.SelectAktifSozlesmeByKiraciId(KiraciIdQS.ConvertToInt());
            if (kiraSozlesme != null)
            {
                RedirectToPage(ProjeConstants.PAGE_KIRASOZLESMESI + "?KiraSozlesmeId=" + kiraSozlesme.Id);
            }
            else
            {
                kiraSozlesme = kiraSozlesme.SelectBitenSozlesmeByKiraciId(KiraciIdQS.ConvertToInt());

                if (kiraSozlesme == null)
                    MessageHelper.PublishMessage("Kiracıya ait bir Kira Sözleşmesi bulunamadı.", ProjeConstants.MESAJ_HATA);
            }
        }
        protected void OdemePlaniBtn_Click(object sender, EventArgs e)
        {
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            kiraSozlesme = kiraSozlesme.SelectAktifSozlesmeByKiraciId(KiraciIdQS.ConvertToInt());
            if (kiraSozlesme != null)
            {
                if (kiraSozlesme != null)
                {
                    RedirectToPage(ProjeConstants.PAGE_ODEMEPLANI + "?KiraSozlesmeId=" + kiraSozlesme.Id);
                }
                else
                {
                    MessageHelper.PublishMessage("OdemePlanı Bulunamadı", ProjeConstants.MESAJ_HATA);
                }
            }
            else
            {
                MessageHelper.PublishMessage("Kira Sözleşmesi Bulunamadı", ProjeConstants.MESAJ_HATA);
            }

        }
        protected void KiraciListBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_KIRACI_LIST);

        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_HOME);
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

        protected void OdemeTarihiTxt_TextChanged(object sender, EventArgs e)
        {
            DateTime odemeTarihi = OdemeTarihiTxt.Text.ConvertToDatetime();
            if (odemeTarihi > ProjeConstants.REFERANS_TARIHI)
            {
                KiraSozlesme kiraSozlesme = new KiraSozlesme();
                kiraSozlesme = kiraSozlesme.SelectByKiraciIdTarih(KiraciIdQS.ConvertToInt(), odemeTarihi);
                if (kiraSozlesme != null)
                {
                    UtilityHelper.SetDDLValue(SozlesmeDDL, kiraSozlesme.Id.ToString());
                    SozlesmeDDL_SelectedIndexChanged(this, EventArgs.Empty);

                }

            }
        }
    }
}
