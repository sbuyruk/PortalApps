using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.TeminatIslemleriWP
{
    [ToolboxItemAttribute(false)]
    public partial class TeminatIslemleriWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public TeminatIslemleriWP()
        {
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

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
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

        private readonly IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                KiraSozlesme kiraSozlesme = new KiraSozlesme();
                kiraSozlesme = kiraSozlesme.Select(KiraSozlesmeIdQS.ConvertToInt());
                if (!Page.IsPostBack)
                {
                    if (KiraSozlesmeIdQS.ConvertToInt() > 0)
                    {
                        Kiraci kiraci = new Kiraci();
                        kiraci = kiraci.Select(kiraSozlesme.KiraciId);
                        if (kiraci != null)
                        {
                            KiraciIdQS = kiraci.Id.ToString();
                            AdiLbl.Text = kiraci == null ? "" : kiraci.Adi + " " + kiraci.Soyadi + ": " + kiraSozlesme.SozBasTar.ConvertToDatetimeEmptyIfNull() + " - " + kiraSozlesme.SozBitTar.ConvertToDatetimeEmptyIfNull() + " Tarihli Sözleşme";
                        }
                        TBYSOrtak.TeminatIslemleriniHesaplaVeKaydet(kiraSozlesme);
                        TeminatBilgileriniDoldur(kiraSozlesme);
                        IslemTipiDDLDoldur();
                    }
                    else
                    {
                        MessageHelper.PublishMessage("Kira Sözlesmesi Bulunamadı.", ProjeConstants.MESAJ_HATA);
                    }
                }
                TeminatIslemleriTablosunuDoldur(kiraSozlesme.KiraciId);
            }
            catch (Exception exception)
            {

                ExceptionHelper exhelper = new ExceptionHelper(exception);
                exhelper.PublishException();
            }
        }
        private void TeminatBilgileriniDoldur(KiraSozlesme kiraSozlesme)
        {
            if (kiraSozlesme != null)
            {
                TeminatCinsiTxt.Value = string.IsNullOrEmpty(kiraSozlesme.TeminatCinsi) ? ProjeConstants.DOVIZ_TL : kiraSozlesme.TeminatCinsi;
                TeminatTarihiTxt.Value = kiraSozlesme.TeminatOdemeTarihi.ConvertToDatetimeEmptyIfNull();
                TeminatAciklamaTxt.Text = kiraSozlesme.TeminatAciklama;
                TeminatTutariTxt.Value = kiraSozlesme.TeminatTutari.ToString("N", culturInfo);
                OdenenTeminatTxt.Value = kiraSozlesme.OdenenTeminatTutari.ToString("N", culturInfo);
                IadeTeminatTxt.Value = kiraSozlesme.IadeTeminatTutari.ToString("N", culturInfo);
                KalanTeminatTxt.Value = kiraSozlesme.KalanTeminatTutari.ToString("N", culturInfo);
            }

        }
        private void TeminatIslemleriTablosunuDoldur(int kiraciId)
        {
            var jsonData = GetJson(kiraciId);
            var jsString = CreateJsString(jsonData); //javascript kodu hazırlanıyor.
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), jsString, true);
        }
        //private void TeminatIslemleriniHesaplaVeKaydet(KiraSozlesme kiraSozlesme)
        //{
        //    decimal toplamOdeme = 0m;
        //    decimal toplamIade = 0m;
        //    TeminatIslem teminatIslemDao = new TeminatIslem();
        //    DataTable dataTable = teminatIslemDao.SelectSumIslemTutariByKiraciIdGroupByIslemTipi(kiraSozlesme.KiraciId);
        //    if (dataTable != null)
        //    {
        //        foreach (DataRow row in dataTable.Rows)
        //        {
        //            string islemTipi = row["IslemTipi"].ToString();
        //            decimal islemToplami = row["IslemToplami"].ReturnZeroIfNull().ConvertToDecimal();


        //            switch (islemTipi)
        //            {
        //                case ProjeConstants.TEMINAT_ODEMESI:
        //                    {
        //                        toplamOdeme += islemToplami;
        //                        break;
        //                    }
        //                case ProjeConstants.TEMINAT_GECICITEMINATODEMESI:
        //                    {
        //                        toplamOdeme += islemToplami;
        //                        break;
        //                    }
        //                case ProjeConstants.TEMINAT_KIRACIYAIADE:
        //                    {
        //                        toplamIade += islemToplami;
        //                        break;
        //                    }
        //                case ProjeConstants.TEMINAT_KIRAYAMAHSUP:
        //                    {
        //                        toplamIade += islemToplami;
        //                        break;
        //                    }
        //                case ProjeConstants.TEMINAT_HASARAMAHSUP:
        //                    {
        //                        toplamIade += islemToplami;
        //                        break;
        //                    }
        //                case ProjeConstants.TEMINAT_AIDATAMAHSUP:
        //                    {
        //                        toplamIade += islemToplami;
        //                        break;
        //                    }
        //                case ProjeConstants.TEMINAT_VAKFABAGIS:
        //                    {
        //                        toplamIade += islemToplami;
        //                        break;
        //                    }
        //                default:
        //                    break;
        //            }


        //        }

        //    }
        //    kiraSozlesme.OdenenTeminatTutari = toplamOdeme;
        //    kiraSozlesme.IadeTeminatTutari = toplamIade;
        //    kiraSozlesme.KalanTeminatTutari = toplamOdeme - toplamIade;
        //    kiraSozlesme.Update();
        //}
        private string CreateJsString(string jsonData)
        {
            string tablestr = @"
                                var counter=1;   
                                $('#tblfilter').puidatatable({
                                caption: 'Teminat Ödeme-İade İşlemleri',
                                editMode: 'cell',
                                columns: [
                                    { field: 'IslemTarihi', headerText: 'İşlem tarihi', headerStyle:'width: 15%'},
                                    { field: 'IslemTipi', headerText: 'İşlem Tipi',headerStyle:'width: 20%' },        
                                    { field: 'IslemTutari', headerText: 'İşlem Tutarı',bodyClass:'text-right',headerStyle:'width: 10%' },
                                    { field: 'IslemAciklama', headerText: 'İşlem Açıklaması',headerStyle:'width: 30%' },
                                    { field: 'TeminatIslemId',headerText: 'Düzenle',bodyClass:'text-center', headerStyle:'width: 10%', content: function (rowData)
                                        { 
                                            return $('<a href=# onclick=GuncelleModalDoldur(' + rowData.TeminatIslemId + '); class=\'btn btn-outline-primary \'>Düzenle</a>')
                                        }
                                    },
                                    { field: 'Id',headerText: 'Sil',bodyClass:'text-center', headerStyle:'width: 10%', content: function (rowData)
                                        { 
                                            return $('<a href=# onclick=DeleteModalDoldur(' + rowData.TeminatIslemId + '); class=\'btn btn-outline-danger \'>Sil</a>')
                                        }
                                    }
                                ],
                                datasource:" + jsonData + @",
                                resizableColumns: true
                            });
                            $('#messages').puigrowl();
                            ";

            return tablestr;
        }
        private string GetJson(int kiraciId)
        {
            string jSon = string.Empty;

            List<TeminatIslemListItem> list = GetDataList(kiraciId);

            var serializer = new JavaScriptSerializer();
            jSon = serializer.Serialize(list);
            return jSon;
        }
        private void IslemTipiDDLDoldur()
        {
            IslemTipiDDL.Items.Clear();
            IslemTipiDDL.Items.Add(ProjeConstants.TEMINAT_ODEMESI);
            IslemTipiDDL.Items.Add(ProjeConstants.TEMINAT_GECICITEMINATODEMESI);
            IslemTipiDDL.Items.Add(ProjeConstants.TEMINAT_KIRACIYAIADE);
            IslemTipiDDL.Items.Add(ProjeConstants.TEMINAT_KIRAYAMAHSUP);
            IslemTipiDDL.Items.Add(ProjeConstants.TEMINAT_HASARAMAHSUP);
            IslemTipiDDL.Items.Add(ProjeConstants.TEMINAT_AIDATAMAHSUP);
            IslemTipiDDL.Items.Add(ProjeConstants.TEMINAT_VAKFABAGIS);
        }
        private List<TeminatIslemListItem> GetDataList(int kiraciId)
        {
            TeminatIslem teminatIslemDao = new TeminatIslem();
            List<TeminatIslem> list = teminatIslemDao.SelectByKiraciId(kiraciId);
            List<TeminatIslemListItem> list2 = new List<TeminatIslemListItem>();
            foreach (var item in list)
            {
                TeminatIslemListItem item2 = new TeminatIslemListItem();
                item2.TeminatIslemId = item.Id.ToString();
                item2.IslemAciklama = item.Aciklama;
                item2.DovizCinsi = item.DovizCinsi;
                item2.IslemTarihi = item.IslemTarihi.ToString("dd.MM.yyyy HH:mm");
                item2.IslemTipi = item.IslemTipi;
                item2.IslemTutari = item.IslemTutari.ToString("N", culturInfo);
                item2.KiraciId = item.KiraciId.ToString();
                list2.Add(item2);

            }
            return list2;
        }
        protected void TeminatGuncelleBtn_Click(object sender, EventArgs e)
        {
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            kiraSozlesme = kiraSozlesme.Select(KiraSozlesmeIdQS.ConvertToInt());
            if (kiraSozlesme != null)
            {
                try
                {
                    kiraSozlesme.TeminatAciklama = TeminatAciklamaTxt.Text;
                    kiraSozlesme.TeminatTutari = TeminatTutariTxt.Value.ConvertToDecimal();
                    kiraSozlesme.OdenenTeminatTutari = OdenenTeminatTxt.Value.ConvertToDecimal();
                    kiraSozlesme.KalanTeminatTutari = KalanTeminatTxt.Value.ConvertToDecimal();
                    kiraSozlesme.Degistiren = UtilityHelper.GetCurrentUserLoginName();
                    kiraSozlesme.IadeTeminatTutari = IadeTeminatTxt.Value.ConvertToDecimal();
                    kiraSozlesme.TeminatCinsi = string.IsNullOrEmpty(TeminatCinsiTxt.Value) ? ProjeConstants.DOVIZ_TL : TeminatCinsiTxt.Value;
                    kiraSozlesme.TeminatOdemeTarihi = TeminatTarihiTxt.Value.ConvertToDatetime();
                    bool guncellendiMi = kiraSozlesme.Update();
                    if (guncellendiMi)
                    {
                        MessageHelper.PublishMessage("Teminat bilgileri güncellendi.", ProjeConstants.MESAJ_BASARILI);
                    }
                    else
                    {
                        MessageHelper.PublishMessage("Teminat bilgileri güncellenemedi.", ProjeConstants.MESAJ_HATA);
                    }
                }
                catch (Exception exception)
                {

                    ExceptionHelper exhelper = new ExceptionHelper(exception);
                    exhelper.PublishException();
                }
            }
            else
            {
                MessageHelper.PublishMessage("Kira Sözlesmesi Bulunamadı ", ProjeConstants.MESAJ_HATA);
            }

        }
        private void OdemeyiGuncelle(KiraSozlesme kiraSozlesme, DateTime odemeTarihi, decimal odenenTutar, int odemeId, string aciklama)
        {
            bool guncellendiMi = false;
            if (kiraSozlesme != null)
            {
                Odeme odeme = new Odeme();
                odeme = odeme.Select(odemeId);
                if (odeme != null)
                {
                    OdemePlani opl = new OdemePlani();
                    opl = opl.SelectBySozlesmeIdOdemeTarihi(kiraSozlesme.Id, odemeTarihi);
                    int yeniOdemePlaniId = opl != null ? opl.Id : odeme.OdemePlaniId;
                    guncellendiMi = odeme.OdemeyiVeOdemePlaniniGuncelle(kiraSozlesme.Id, odeme.OdemePlaniId, yeniOdemePlaniId,
                    odemeId, odemeTarihi, odenenTutar, aciklama, UtilityHelper.GetCurrentUserLoginName());
                }

            }
            else
            {
                MessageHelper.PublishMessage("Sözleşme Bulunamadı", ProjeConstants.MESAJ_HATA);
            }
        }
        protected void ModalEkleBtn_Click(object sender, EventArgs e)
        {
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            kiraSozlesme = kiraSozlesme.Select(KiraSozlesmeIdQS.ConvertToInt());
            if (kiraSozlesme != null)
            {
                IslemTipiDDL.Visible = true;
                IslemTipiTxt.Visible = false;
                ModalTitleLbl.Text = "Teminat İşlemi Eklenecek";

                IslemTarihiTxt.Value = kiraSozlesme.TeminatOdemeTarihi.ConvertToDatetimeEmptyIfNull();
                IslemSaatiTxt.Value = kiraSozlesme.TeminatOdemeTarihi.ToString("HH:mm");
                //decimal tutar = kiraSozlesme.TeminatTutari - kiraSozlesme.OdenenTeminatTutari + kiraSozlesme.IadeTeminatTutari;
                ///IslemTutariTxt.Value = tutar.ToString("N", culturInfo);
                IslemAciklamaTxt.Text = "";
                //IslemTipiDDLDoldur();
                ModalEkleNowBtn.Visible = true;
                ModalGuncelleNowBtn.Visible = false;
                UtilityHelper.ScriptCalistir("OpenTeminatIslemiModal();");
            }

        }
        protected void ModalGuncelleBtn_Click(object sender, EventArgs e)
        {
            TeminatIslem teminatIslem = new TeminatIslem();
            teminatIslem = teminatIslem.Select(TeminatIslemIdGuncelleHdn.Value.ConvertToInt());
            if (teminatIslem != null)
            {
                //IslemTipiDDLDoldur();
                IslemTipiDDL.Visible = false;
                IslemTipiTxt.Visible = true;
                ModalTitleLbl.Text = "Teminat İşlemi Değiştirilecek";
                ModalEkleNowBtn.Visible = false;
                ModalGuncelleNowBtn.Visible = true;
                IslemTipiTxt.Text = teminatIslem.IslemTipi;
                IslemTarihiTxt.Value = teminatIslem.IslemTarihi.ConvertToDatetimeEmptyIfNull();
                IslemSaatiTxt.Value = teminatIslem.IslemTarihi.ToString("HH:mm");
                IslemTutariTxt.Value = teminatIslem.IslemTutari.ToString("N", culturInfo);
                IslemTipiDDL.SelectedItem.Value = teminatIslem.IslemTipi;
                IslemAciklamaTxt.Text = teminatIslem.Aciklama;

                ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "OpenTeminatIslemiModal();", true);
            }
        }

        protected void ModalSilBtn_Click(object sender, EventArgs e)
        {
            TeminatIslem teminatIslem = new TeminatIslem();
            teminatIslem = teminatIslem.Select(TeminatIslemIdSilLbl.Value.ConvertToInt());
            if (teminatIslem != null)
            {
                SilmeMesajiLbl.Text = teminatIslem.IslemTarihi.ConvertToDatetimeEmptyIfNull() + " tarihli ve " + teminatIslem.IslemTutari.ToString("N", culturInfo) + " tutarlı '" + teminatIslem.IslemTipi + "' işlemi silinecek.";
                UtilityHelper.ScriptCalistir("OpenDeleteModalOnay();");
            }
        }
        protected void ModalEkleNowBtn_Click(object sender, EventArgs e)
        {
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            kiraSozlesme = kiraSozlesme.Select(KiraSozlesmeIdQS.ConvertToInt());
            if (kiraSozlesme != null)
            {

                TeminatIslem teminatIslem = new TeminatIslem();
                teminatIslem.KiraciId = kiraSozlesme.KiraciId;
                teminatIslem.Aciklama = IslemAciklamaTxt.Text;
                teminatIslem.DovizCinsi = ProjeConstants.DOVIZ_TL;
                teminatIslem.IslemTarihi = UtilityHelper.TariheSaatEkle(IslemTarihiTxt.Value.ConvertToDatetime(), IslemSaatiTxt.Value);
                teminatIslem.IslemTipi = IslemTipiDDL.SelectedValue;
                teminatIslem.IslemTutari = IslemTutariTxt.Value.ConvertToDecimal();
                teminatIslem.Olusturan = UtilityHelper.GetCurrentUserLoginName();
                int teminatIslemId = teminatIslem.Save();
                if (teminatIslemId > 0)
                {
                    if (teminatIslem.IslemTipi.Equals(ProjeConstants.TEMINAT_KIRAYAMAHSUP))
                    {
                        KiraSozlesme mahsupEdilecekKiraSozlesme = new KiraSozlesme();
                        mahsupEdilecekKiraSozlesme = mahsupEdilecekKiraSozlesme.SelectByKiraciIdTarih (kiraSozlesme.KiraciId, IslemTarihiTxt.Value.ConvertToDatetime());
                        if (mahsupEdilecekKiraSozlesme == null)
                        {
                            mahsupEdilecekKiraSozlesme = mahsupEdilecekKiraSozlesme.SelectEnYakinTarihliSozlesmeByKiraciIdTarih(kiraSozlesme.KiraciId, IslemTarihiTxt.Value.ConvertToDatetime());
                        }
                        if (mahsupEdilecekKiraSozlesme != null)
                        {
                            int odemeId = 0;
                            TBYSOrtak.OdemeYap(mahsupEdilecekKiraSozlesme, teminatIslem.IslemTarihi, teminatIslem.IslemTutari, teminatIslem.Aciklama, ref odemeId);
                            teminatIslem.OdemeId = odemeId;
                            teminatIslem.Update();
                        }
                    }
                    TBYSOrtak.TeminatIslemleriniHesaplaVeKaydet(kiraSozlesme);
                    RedirectToPage(ProjeConstants.PAGE_TEMINAT_ISLEMLERI + "?KiraSozlesmeId=" + KiraSozlesmeIdQS);
                }
                else
                {
                    MessageHelper.PublishMessage("Mahsup edilecek bir sözleşme bulunamadı", ProjeConstants.MESAJ_HATA);
                }

            }

        }
        protected void ModalGuncelleNowBtn_Click(object sender, EventArgs e)
        {
            TeminatIslem teminatIslem = new TeminatIslem();
            teminatIslem = teminatIslem.Select(TeminatIslemIdGuncelleHdn.Value.ConvertToInt());
            if (teminatIslem != null)
            {

                teminatIslem.Aciklama = IslemAciklamaTxt.Text;
                teminatIslem.DovizCinsi = ProjeConstants.DOVIZ_TL;
                teminatIslem.IslemTarihi = UtilityHelper.TariheSaatEkle(IslemTarihiTxt.Value.ConvertToDatetime(), IslemSaatiTxt.Value);
                teminatIslem.IslemTipi = IslemTipiDDL.SelectedValue;
                teminatIslem.IslemTutari = IslemTutariTxt.Value.ConvertToDecimal();
                teminatIslem.Degistiren = UtilityHelper.GetCurrentUserLoginName();
                if (teminatIslem.Update())
                {
                    KiraSozlesme kiraSozlesme = new KiraSozlesme();
                    kiraSozlesme = kiraSozlesme.Select(KiraSozlesmeIdQS.ConvertToInt());
                    if (kiraSozlesme != null)
                    {
                        KiraSozlesme mahsupEdilecekKiraSozlesme = new KiraSozlesme();
                        mahsupEdilecekKiraSozlesme = mahsupEdilecekKiraSozlesme.SelectEnYakinTarihliSozlesmeByKiraciIdTarih(kiraSozlesme.KiraciId, IslemTarihiTxt.Value.ConvertToDatetime());

                        if (mahsupEdilecekKiraSozlesme != null)
                        {
                            TBYSOrtak.TeminatIslemleriniHesaplaVeKaydet(kiraSozlesme);
                            TeminatBilgileriniDoldur(kiraSozlesme);
                            TeminatIslemleriTablosunuDoldur(kiraSozlesme.KiraciId);
                            MessageHelper.PublishMessage("Teminat İşlemi Güncellendi", ProjeConstants.MESAJ_BASARILI, 2000);
                            if (teminatIslem.IslemTipi.Equals(ProjeConstants.TEMINAT_KIRAYAMAHSUP))
                            {
                                OdemeyiGuncelle(mahsupEdilecekKiraSozlesme, teminatIslem.IslemTarihi, teminatIslem.IslemTutari, teminatIslem.OdemeId, teminatIslem.Aciklama);
                            }
                        }

                    }
                    else
                    {
                        MessageHelper.PublishMessage("Teminata ait sözlesme bulunamadı!", ProjeConstants.MESAJ_HATA);
                    }
                }
            }
            else
            {
                MessageHelper.PublishMessage("Teminat İşlemi bulunamadı!", ProjeConstants.MESAJ_HATA);
            }

        }
        protected void ModalSilNowBtn_Click(object sender, EventArgs e)
        {
            int teminatId = TeminatIslemIdSilLbl.Value.ConvertToInt();

            TeminatIslem teminatIslem = new TeminatIslem();
            teminatIslem = teminatIslem.Select(teminatId);
            if (teminatIslem != null)
            {
                bool silindiMi = teminatIslem.Delete();
                KiraSozlesme kiraSozlesme = new KiraSozlesme();
                kiraSozlesme = kiraSozlesme.Select(KiraSozlesmeIdQS.ConvertToInt());
                if (kiraSozlesme != null)
                {
                    if (teminatIslem.IslemTipi.Equals(ProjeConstants.TEMINAT_KIRAYAMAHSUP) && silindiMi)
                    {
                        Odeme odeme = new Odeme();
                        odeme = odeme.Select(teminatIslem.OdemeId);
                        if (odeme != null)
                        {
                            bool odemeSilindiMi = odeme.Delete();
                        }

                    }
                    TBYSOrtak.TeminatIslemleriniHesaplaVeKaydet(kiraSozlesme);
                    TeminatBilgileriniDoldur(kiraSozlesme);
                    TeminatIslemleriTablosunuDoldur(kiraSozlesme.KiraciId);
                    MessageHelper.PublishMessage("Teminat İşlemi Silindi", ProjeConstants.MESAJ_BASARILI, 2000);
                }
                else
                {
                    MessageHelper.PublishMessage("Teminat İşlemi Silindi. Teminata ait sözlesme bulunamadı!", ProjeConstants.MESAJ_BILGI, 4000);
                }
            }
            else
            {
                MessageHelper.PublishMessage("Teminat İşlemi bulunamadı!", ProjeConstants.MESAJ_HATA);
            }
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

        private class TeminatIslemListItem
        {
            public string TeminatIslemId { get; set; }
            public string DosyaNo { get; set; }
            public string KiraciId { get; set; }
            public string IslemTarihi { get; set; }
            public string IslemSaati { get; set; }
            public string IslemTipi { get; set; }
            public string IslemTutari { get; set; }
            public string IslemAciklama { get; set; }
            public string DovizCinsi { get; set; }
            public string OdemeId { get; set; }

        }
        protected void KiraKartiBtn_Click(object sender, EventArgs e)
        {
            Kiraci kiraci = new Kiraci();
            kiraci = kiraci.Select<Kiraci>(KiraciIdQS.ConvertToInt());
            if (kiraci != null)//bu kiraci varsa
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));
                newUrl += "/" + ProjeConstants.PAGE_KIRAKARTI + "?SenderApp=KD&KiraciId=" + kiraci.Id;
                Page.Response.Redirect(newUrl, true);

            }
        }
        protected void SozlesmeyeGitBtn_Click(object sender, EventArgs e)
        {

            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            kiraSozlesme = kiraSozlesme.Select(KiraSozlesmeIdQS.ConvertToInt());
            if (kiraSozlesme != null) //bu sozlesme varsa
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_KIRASOZLESMESI;
                newUrl = newUrl + "?SenderApp=KD&KiraSozlesmeId=" + kiraSozlesme.Id + "&KiraciId=" + KiraciIdQS;
                Page.Response.Redirect(newUrl, true);
            }
            else
            {
                MessageHelper.PublishMessage("Sözleşme kaydı bulunamadı", ProjeConstants.MESAJ_HATA);
            }
        }
        protected void OdemePlaninaGitBtn_Click(object sender, EventArgs e)
        {
            try
            {
                KiraSozlesme kiraSozlesme = new KiraSozlesme();
                kiraSozlesme = kiraSozlesme.Select(KiraSozlesmeIdQS.ConvertToInt());
                if (kiraSozlesme != null) //bu sozlesme varsa
                {
                    OdemePlani odemePlani = new OdemePlani();
                    string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                    string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_ODEMEPLANI;
                    newUrl = newUrl + "?SenderApp=KD&KiraciId=" + kiraSozlesme.KiraciId + "&KiraSozlesmeId=" + kiraSozlesme.Id;
                    Page.Response.Redirect(newUrl, true);
                }
                else
                    MessageHelper.PublishMessage("Bu kiracıya ait sözleşme kaydı bulunamadı", ProjeConstants.MESAJ_HATA);

            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void KiraciListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_KIRACI_LIST + "?SecilenId=" + KiraciIdQS);
        }
        protected void PrevBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Kiraci kiraci = new Kiraci();
                kiraci = kiraci.Select<Kiraci>(KiraciIdQS.ConvertToInt());
                Kiraci oncekiKiraci = kiraci.SelectNext();
                if (oncekiKiraci != null)
                {
                    RedirectToPage(ProjeConstants.PAGE_KIRACI_GIRIS + "?DestinationApp=KD&KiraciId=" + oncekiKiraci.Id);
                }
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
                Kiraci kiraci = new Kiraci();
                kiraci = kiraci.Select<Kiraci>(KiraciIdQS.ConvertToInt());
                Kiraci sonrakiKiraci = kiraci.SelectNext();
                if (sonrakiKiraci != null)
                {
                    RedirectToPage(ProjeConstants.PAGE_KIRACI_GIRIS + "?DestinationApp=KD&KiraciId=" + sonrakiKiraci.Id);
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
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
        protected void TeminatListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_TEMINAT_LIST);
        }

        protected void AylikOdemelerBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Kiraci kiraci = new Kiraci();
                kiraci = kiraci.Select<Kiraci>(KiraciIdQS.ConvertToInt());
                if (kiraci != null)
                {
                    RedirectToPage(ProjeConstants.PAGE_KIRACI_AYLIKODEME + "?KiraciId=" + KiraciIdQS + "&SecilenAy=0&SecilenYil=0");
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }

        }
    }
}
