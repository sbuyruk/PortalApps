using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.BagimsizBolumWP
{
    [ToolboxItemAttribute(false)]
    public partial class BagimsizBolumWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public BagimsizBolumWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string TasinmazIdQS
        {
            get
            {

                if (ViewState["TasinmazId"] == null)
                {
                    if (Page.Request.QueryString["TasinmazId"] != null)
                    {
                        ViewState["TasinmazId"] = Page.Request.QueryString["TasinmazId"];
                    }
                    else
                    {
                        ViewState["TasinmazId"] = string.Empty;
                    }
                }
                return ViewState["TasinmazId"].ToString();
            }

            set
            {
                ViewState["TasinmazId"] = value;
            }
        }
        private string EnvanterdeMiQS
        {
            get
            {

                if (ViewState["EnvanterdeMi"] == null)
                {
                    if (Page.Request.QueryString["EnvanterdeMi"] != null)
                    {
                        ViewState["EnvanterdeMi"] = Page.Request.QueryString["EnvanterdeMi"];
                    }
                    else
                    {
                        ViewState["EnvanterdeMi"] = string.Empty;
                    }
                }
                return ViewState["EnvanterdeMi"].ToString();
            }

            set
            {
                ViewState["EnvanterdeMi"] = value;
            }
        }
        private IFormatProvider TrCulture = new CultureInfo(ProjeConstants.CULTUREINFO, true);
        protected void Page_Load(object sender, EventArgs e)
        {
            Tasinmaz tasinmaz = new Tasinmaz();
            tasinmaz = tasinmaz.Select<Tasinmaz>(TasinmazIdQS.ConvertToInt());
            if (tasinmaz != null)
            {
                BagimsizBolumTablosunuDoldur(tasinmaz);
                AdresTxt.Text = tasinmaz.Adres;
            }

        }
        private void BagimsizBolumTableHeaders()
        {
            BagimsizBolumTable.Rows.Clear();
            TableHeaderRow th = new TableHeaderRow();
            TableHeaderCell siraCell = new TableHeaderCell();
            siraCell.Text = "Sira";
            TableHeaderCell AdresCell = new TableHeaderCell();
            AdresCell.Text = "Adres";
            TableHeaderCell BolumCell = new TableHeaderCell();
            BolumCell.Text = "Bölüm No";
            TableHeaderCell NitelikCell = new TableHeaderCell();
            NitelikCell.Text = "Nitelik";
            TableHeaderCell MetrekareCell = new TableHeaderCell();
            MetrekareCell.Text = "Metrekare";
            TableHeaderCell KullanimAmaciCell = new TableHeaderCell();
            KullanimAmaciCell.Text = "Kullanim Amaci";
            TableHeaderCell AciklamaCell = new TableHeaderCell();
            AciklamaCell.Text = "Açiklama";
            TableHeaderCell MuhasebeyeKayitliDegerCell = new TableHeaderCell();
            MuhasebeyeKayitliDegerCell.Text = "Muhasebeye Kayitli Deger";
            TableHeaderCell TahminiRayicDegeriCell = new TableHeaderCell();
            TahminiRayicDegeriCell.Text = "Tahmini Rayic Degeri";
            TableHeaderCell EmlakBeyanDegeriCell = new TableHeaderCell();
            EmlakBeyanDegeriCell.Text = "Emlak Beyan Degeri";
            TableHeaderCell YaklasikPiyasaDegeriCell = new TableHeaderCell();
            YaklasikPiyasaDegeriCell.Text = "Yaklasik Piyasa Degeri";
            TableHeaderCell DuzenleCell = new TableHeaderCell();
            DuzenleCell.Text = "Düzenle";
            TableHeaderCell SilCell = new TableHeaderCell();
            SilCell.Text = "Sil";


            th.Controls.Add(siraCell);
            th.Controls.Add(AdresCell);
            th.Controls.Add(BolumCell);
            th.Controls.Add(NitelikCell);
            th.Controls.Add(MetrekareCell);
            th.Controls.Add(KullanimAmaciCell);
            th.Controls.Add(AciklamaCell);
            th.Controls.Add(MuhasebeyeKayitliDegerCell);
            th.Controls.Add(TahminiRayicDegeriCell);
            th.Controls.Add(EmlakBeyanDegeriCell);
            th.Controls.Add(YaklasikPiyasaDegeriCell);
            th.Controls.Add(DuzenleCell);
            th.Controls.Add(SilCell);
            BagimsizBolumTable.Controls.Add(th);
        }
        private void BagimsizBolumTablosunuDoldur(Tasinmaz tasinmaz)
        {
            AdiLbl.Text = " Tasinmaz : " + tasinmaz.KullanimSekli + " " + tasinmaz.Ili + " " + tasinmaz.Ilcesi;
            IdLbl.Text = tasinmaz.Id + "";
            //Column headers
            BagimsizBolumTableHeaders();
            BagimsizBolum bt = new BagimsizBolum();
            List<BagimsizBolum> list = bt.SelectByTasinmazId(tasinmaz.Id);
            int SiraNo = 1;
            foreach (BagimsizBolum bagimsizBolum in list)
            {
                TableRow row = new TableRow();

                TableCell SiranoCell = new TableCell();

                SiranoCell.Text = SiraNo++ + "";
                row.Controls.Add(SiranoCell);

                TableCell AdresCell = new TableCell();
                AdresCell.Text = tasinmaz.Adres;
                row.Controls.Add(AdresCell);

                TableCell BolumNoCell = new TableCell();
                BolumNoCell.Text = bagimsizBolum.BolumNo;
                row.Controls.Add(BolumNoCell);
                 
                TableCell NitelikCell = new TableCell();
                NitelikCell.Text = bagimsizBolum.Nitelik;
                row.Controls.Add(NitelikCell);
                
                TableCell MetrekareCell = new TableCell();
                MetrekareCell.Text = bagimsizBolum.Metrekare.ToString("N", TrCulture);
                row.Controls.Add(MetrekareCell);

                TableCell KullanimAmaciCell = new TableCell();
                KullanimAmaciCell.Text = bagimsizBolum.KullanimAmaci;
                row.Controls.Add(KullanimAmaciCell);

                TableCell AciklamaCell = new TableCell();
                AciklamaCell.Text = bagimsizBolum.Aciklama;
                row.Controls.Add(AciklamaCell);

                TableCell MuhasebeyeKayitliDegerCell = new TableCell();
                MuhasebeyeKayitliDegerCell.Text = bagimsizBolum.MuhasebeyeKayitliDeger.ToString("N", TrCulture);
                MuhasebeyeKayitliDegerCell.HorizontalAlign = HorizontalAlign.Right;
                row.Controls.Add(MuhasebeyeKayitliDegerCell);

                TableCell TahminiRayicDegeriCell = new TableCell();
                TahminiRayicDegeriCell.Text = bagimsizBolum.TahminiRayicDegeri.ToString("N", TrCulture);
                TahminiRayicDegeriCell.HorizontalAlign = HorizontalAlign.Right;
                row.Controls.Add(TahminiRayicDegeriCell);

                TableCell EmlakBeyanDegeriCell = new TableCell();
                EmlakBeyanDegeriCell.Text = bagimsizBolum.EmlakBeyanDegeri.ToString("N", TrCulture);
                EmlakBeyanDegeriCell.HorizontalAlign = HorizontalAlign.Right;
                row.Controls.Add(EmlakBeyanDegeriCell);

                TableCell YaklasikPiyasaDegeriCell = new TableCell();
                YaklasikPiyasaDegeriCell.Text = bagimsizBolum.YaklasikPiyasaDegeri.ToString("N", TrCulture);
                YaklasikPiyasaDegeriCell.HorizontalAlign = HorizontalAlign.Right;
                row.Controls.Add(YaklasikPiyasaDegeriCell);

                //DuzenleButonu ve Cell ekle
                TableCell DuzenleCell = new TableCell();
                LinkButton DuzenleBtn = new LinkButton();
                DuzenleBtn.Text = "Düzenle";
                DuzenleBtn.CssClass = "btn btn-outline-primary";
                DuzenleBtn.Click += delegate
                {
                    BagimsizBolumDuzenleModalAc(tasinmaz, bagimsizBolum);
                };
                DuzenleCell.Controls.Add(DuzenleBtn);
                row.Controls.Add(DuzenleCell);

                BagimsizBolumTable.Controls.Add(row);



                TableCell SilCell = new TableCell();
                LinkButton SilBtn = new LinkButton();
                SilBtn.Text = "Sil";
                SilBtn.CssClass = "btn btn-outline-danger";
                SilBtn.Click += delegate
                {
                    BagimsizBolumSilModalAc(tasinmaz, bagimsizBolum);
                };
                SilCell.Controls.Add(SilBtn);
                row.Controls.Add(SilCell);

                BagimsizBolumTable.Controls.Add(row);
            }

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
        protected void TasinmazaGitBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_TASINMAZ_GIRIS + "?DestinationApp=TD&TasinmazId=" + TasinmazIdQS + "&EnvanterdeMi=" + EnvanterdeMiQS;
                Page.Response.Redirect(newUrl);
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void BagimsizBolumBtn_Click(object sender, EventArgs e)
        {
            Tasinmaz tasinmaz = new Tasinmaz();
            tasinmaz = tasinmaz.Select(TasinmazIdQS.ConvertToInt());
            if (tasinmaz != null)
            {
                BagimsizBolumEkleModalAc(tasinmaz);
            }
        }
        private void BagimsizBolumEkleModalAc(Tasinmaz tasinmaz)
        {
            KullanimAmaciDDLDoldur();
            BagimsizBolumHeaderLbl.InnerText = "Bagimsiz Bölüm Ekleme";
            AdresTxt.Text = tasinmaz.Adres;
            BolumNoTxt.Text = string.Empty;
            MetrekareTxt.Text = string.Empty;
            UtilityHelper.SetDDLValue(KullanimAmaciDDL, ProjeConstants.KIRALAMAAMACI_MESKEN);
            AciklamaTxt.Text = string.Empty;

            AciklamaTxt.Enabled = true;
            BolumNoTxt.Enabled = true;


            KaydetBtn.Visible = true;
            GuncelleBtn.Visible = false;
            SilBtn.Visible = false;
            MessageLbl.Visible = false;
            UtilityHelper.ScriptCalistir("BagimsizBolumModal();");
        }
        private void BagimsizBolumDuzenleModalAc(Tasinmaz tasinmaz, BagimsizBolum bagimsizBolum)
        {
            BagimsizBolumHeaderLbl.InnerText = "Bagimsiz Bölüm Düzenleme";
            ParamBagimsizBolumIdLbl.Text = bagimsizBolum.Id.ToString();
            AdresTxt.Text = tasinmaz.Adres;
            BolumNoTxt.Text = bagimsizBolum.BolumNo;
            NitelikTxt.Text = bagimsizBolum.Nitelik;
            MetrekareTxt.Text = bagimsizBolum.Metrekare.ToString("N", TrCulture);
            KullanimAmaciDDL.SelectedValue = bagimsizBolum.KullanimAmaci;
            AciklamaTxt.Text = bagimsizBolum.Aciklama;
            MuhasebeyeKayitliDegerTxt.Text = bagimsizBolum.MuhasebeyeKayitliDeger.ToString("N", TrCulture);
            TahminiRayicDegeriTxt.Text = bagimsizBolum.TahminiRayicDegeri.ToString("N", TrCulture);
            EmlakBeyanDegeriTxt.Text = bagimsizBolum.EmlakBeyanDegeri.ToString("N", TrCulture);
            YaklasikPiyasaDegeriTxt.Text = bagimsizBolum.YaklasikPiyasaDegeri.ToString("N", TrCulture);

            AciklamaTxt.Enabled = true;
            BolumNoTxt.Enabled = true;

            KaydetBtn.Visible = false;
            GuncelleBtn.Visible = true;
            SilBtn.Visible = false;
            MessageLbl.Visible = false;
            KullanimAmaciDDLDoldur();
            UtilityHelper.ScriptCalistir("BagimsizBolumModal();");
        }

        private void KullanimAmaciDDLDoldur()
        {
            KullanimAmaciDDL.Items.Clear();
            KullanimAmaciDDL.Items.Add(ProjeConstants.KIRALAMAAMACI_MESKEN);
            KullanimAmaciDDL.Items.Add(ProjeConstants.KIRALAMAAMACI_ISYERI);
            KullanimAmaciDDL.Items.Add(ProjeConstants.KIRALAMAAMACI_ARSA);
            KullanimAmaciDDL.Items.Add(ProjeConstants.KIRALAMAAMACI_TARLA);
            KullanimAmaciDDL.Items.Add(ProjeConstants.KIRALAMAAMACI_BAZISTASYONU);
            KullanimAmaciDDL.Items.Add(ProjeConstants.KIRALAMAAMACI_TESIS);
        }

        private void BagimsizBolumSilModalAc(Tasinmaz tasinmaz, BagimsizBolum bagimsizBolum)
        {
            BagimsizBolumHeaderLbl.InnerText = "Bagimsiz Bölüm Silme";
            ParamBagimsizBolumIdLbl.Text = bagimsizBolum.Id.ToString();
            AdresTxt.Text = tasinmaz.Adres;
            BolumNoTxt.Text = bagimsizBolum.BolumNo;
            NitelikTxt.Text = bagimsizBolum.Nitelik;
            MetrekareTxt.Text = bagimsizBolum.Metrekare.ToString("N", TrCulture) ;
            KullanimAmaciDDL.SelectedValue = bagimsizBolum.KullanimAmaci;
            AciklamaTxt.Text = bagimsizBolum.Aciklama;
            MuhasebeyeKayitliDegerTxt.Text = bagimsizBolum.MuhasebeyeKayitliDeger.ToString("N", TrCulture);
            TahminiRayicDegeriTxt.Text = bagimsizBolum.TahminiRayicDegeri.ToString("N", TrCulture);
            EmlakBeyanDegeriTxt.Text = bagimsizBolum.EmlakBeyanDegeri.ToString("N", TrCulture);
            YaklasikPiyasaDegeriTxt.Text = bagimsizBolum.YaklasikPiyasaDegeri.ToString("N", TrCulture);

            AciklamaTxt.Enabled = false;
            BolumNoTxt.Enabled = false;

            KaydetBtn.Visible = false;
            GuncelleBtn.Visible = false;

            MessageLbl.Text = "Bagimsiz Bölüm silinecek. Onayliyor musunuz?";
            MessageLbl.CssClass = "col-form-label text-danger fw-bold";
            MessageLbl.Visible = true;

            if (BagimsizBolumSilinebilirMi(bagimsizBolum))
            {
                SilBtn.Visible = true;
                SilBtn.Text = "Bagimsiz Bölümü Sil";
                SilBtn.CssClass = "btn btn-outline-danger";
            }
            else
            {
                MessageLbl.Text = "Bu Bagimsiz Bölüm ile iliskilendirilmis bir Sözlesme bulunmaktadir. Bagimsiz Bölüm kaydi silinemez.";
                MessageLbl.CssClass = "col-form-label text-danger fw-bold";
                MessageLbl.Visible = true;
                SilBtn.Visible = false;
            }
            UtilityHelper.ScriptCalistir("BagimsizBolumModal();");
        }
        private bool BagimsizBolumSilinebilirMi(BagimsizBolum bagimsizBolum)
        {
            bool silinebilirMi = false;
            SozlesmeTasinmaz sozlesmeTasinmaz = new SozlesmeTasinmaz();
            List<SozlesmeTasinmaz> list = sozlesmeTasinmaz.SelectByBolumId(bagimsizBolum.Id);
            if (list.Count < 1)
            {
                silinebilirMi = true;
            }
            return silinebilirMi;
        }
        protected void KaydetBtn_Click(object sender, EventArgs e)
        {
            BagimsizBolum bagimsizBolum = new BagimsizBolum();
            bagimsizBolum.BolumNo = BolumNoTxt.Text;
            bagimsizBolum.Nitelik = NitelikTxt.Text;
            bagimsizBolum.Metrekare= MetrekareTxt.Text.ConvertToDecimal();
            bagimsizBolum.KullanimAmaci=KullanimAmaciDDL.SelectedItem.Text ;
            bagimsizBolum.Aciklama = AciklamaTxt.Text;
            bagimsizBolum.TasinmazId = TasinmazIdQS.ConvertToInt();
            bagimsizBolum.MuhasebeyeKayitliDeger = MuhasebeyeKayitliDegerTxt.Text.ConvertToDecimal();
            bagimsizBolum.TahminiRayicDegeri = TahminiRayicDegeriTxt.Text.ConvertToDecimal();
            bagimsizBolum.EmlakBeyanDegeri = EmlakBeyanDegeriTxt.Text.ConvertToDecimal();
            bagimsizBolum.YaklasikPiyasaDegeri = YaklasikPiyasaDegeriTxt.Text.ConvertToDecimal();

            int bagimsizBolumId = bagimsizBolum.Save();
            bagimsizBolum.Id = bagimsizBolumId;
            string newUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            int queryIndex = newUrl.IndexOf("?");
            if (queryIndex > 0)
                newUrl = newUrl.Substring(0, queryIndex);

            Page.Response.Redirect(newUrl + "?TasinmazId=" + TasinmazIdQS, true);
        }
        protected void GuncelleBtn_Click(object sender, EventArgs e)
        {

            BagimsizBolum bagimsizBolum = new BagimsizBolum();
            bagimsizBolum = bagimsizBolum.Select<BagimsizBolum>(ParamBagimsizBolumIdLbl.Text.ConvertToInt());
            if (bagimsizBolum != null)
            {
                bagimsizBolum.BolumNo = BolumNoTxt.Text;
                bagimsizBolum.Nitelik = NitelikTxt.Text;
                bagimsizBolum.Metrekare = MetrekareTxt.Text.ConvertToDecimal();
                bagimsizBolum.KullanimAmaci = KullanimAmaciDDL.SelectedItem.Text;
                bagimsizBolum.Aciklama = AciklamaTxt.Text;
                bagimsizBolum.TasinmazId = TasinmazIdQS.ConvertToInt();
                bagimsizBolum.MuhasebeyeKayitliDeger = MuhasebeyeKayitliDegerTxt.Text.ConvertToDecimal();
                bagimsizBolum.TahminiRayicDegeri = TahminiRayicDegeriTxt.Text.ConvertToDecimal();
                bagimsizBolum.EmlakBeyanDegeri = EmlakBeyanDegeriTxt.Text.ConvertToDecimal();
                bagimsizBolum.YaklasikPiyasaDegeri = YaklasikPiyasaDegeriTxt.Text.ConvertToDecimal();
                if (bagimsizBolum.Update())
                {
                    Tasinmaz tasinmaz = new Tasinmaz();
                    tasinmaz = tasinmaz.Select(TasinmazIdQS.ConvertToInt());
                    if (tasinmaz != null)
                    {
                        BagimsizBolumTablosunuDoldur(tasinmaz);
                    }
                    MessageHelper.PublishMessage("Bagimsiz Bölüm güncellendi", ProjeConstants.MESAJ_BASARILI, 2000);
                    string newUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                    int queryIndex = newUrl.IndexOf("?");
                    if (queryIndex > 0)
                        newUrl = newUrl.Substring(0, queryIndex);

                    Page.Response.Redirect(newUrl + "?TasinmazId=" + TasinmazIdQS, true);
                }
                else
                {
                    MessageHelper.PublishMessage("Bagimsiz Bölüm güncellenemedi", ProjeConstants.MESAJ_HATA);
                }
            }
            else
            {
                MessageHelper.PublishMessage("Bagimsiz Bölüm bulunamadi", ProjeConstants.MESAJ_HATA);
            }

        }
        protected void SilBtn_Click(object sender, EventArgs e)
        {

            BagimsizBolum bagimsizBolum = new BagimsizBolum();
            bagimsizBolum = bagimsizBolum.Select<BagimsizBolum>(ParamBagimsizBolumIdLbl.Text.ConvertToInt());
            if (bagimsizBolum != null)
            {
                if (bagimsizBolum.Delete())
                {
                    Tasinmaz tasinmaz = new Tasinmaz();
                    tasinmaz = tasinmaz.Select(TasinmazIdQS.ConvertToInt());
                    if (tasinmaz != null)
                    {
                        BagimsizBolumTablosunuDoldur(tasinmaz);
                    }
                    MessageHelper.PublishMessage("Bagimsiz Bölüm silindi", ProjeConstants.MESAJ_BASARILI, 2000);
                    string newUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                    int queryIndex = newUrl.IndexOf("?");
                    if (queryIndex > 0)
                        newUrl = newUrl.Substring(0, queryIndex);

                    Page.Response.Redirect(newUrl + "?TasinmazId=" + TasinmazIdQS, true);
                }

            }
            else
            {
                MessageHelper.PublishMessage("Bagimsiz Bölüm silinemedi", ProjeConstants.MESAJ_HATA);
            }
        }
    }
}
