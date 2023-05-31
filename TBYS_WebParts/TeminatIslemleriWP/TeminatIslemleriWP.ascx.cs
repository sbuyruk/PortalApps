using Model.NBYS;
using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
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
                            AdiLbl.Text = kiraci == null ? "" : kiraci.Adi + " " + kiraci.Soyadi ;
                            SozlesmeTasinmazTablosunuDoldur(kiraSozlesme);
                            SozlesmeLbl.Text = " ( " +kiraSozlesme.SozBasTar.ConvertToDatetimeEmptyIfNull() + " - " + kiraSozlesme.SozBitTar.ConvertToDatetimeEmptyIfNull() + " Tarihli Sözleşme )";
                        }
                        TBYSOrtak.TeminatIslemleriniHesaplaVeKaydet(kiraSozlesme);
                        IslemTipiDDLDoldur();
                        TeminatCinsiDDLDoldur();
                        TeminatBilgileriniDoldur(kiraSozlesme);
                    }
                    else
                    {
                        MessageHelper.PublishMessage("Kira Sözlesmesi Bulunamadı.", ProjeConstants.MESAJ_HATA);
                    }
                }
                TabloOlustur(kiraSozlesme.KiraciId);
            }
            catch (Exception exception)
            {

                ExceptionHelper exhelper = new ExceptionHelper(exception);
                exhelper.PublishException();
            }
        }

        private void SozlesmeTasinmazTablosunuDoldur(KiraSozlesme kiraSozlesme)
        {
            TasinmazAdresTable.Rows.Clear();
            string[] headers = { "Sıra", "Taşınmaz Adresi" };
            UtilityHelper.SetTableHeaders(TasinmazAdresTable, headers);
            SozlesmeTasinmaz st = new SozlesmeTasinmaz();
            DataTable dataTable = st.SelectBySozlesmeIdReturnDataTable(kiraSozlesme.Id);
            if (dataTable != null)
            {
                int sira = 0;
                foreach (DataRow row in dataTable.Rows)
                {
                    TableRow tableRow = new TableRow();
                    TableCell siraCell = new TableCell();
                    siraCell.Text = (++sira).ToString();
                    TableCell adresCell = new TableCell();
                    adresCell.Text = row["AdresBolumNoIliIlcesi"].ReturnEmptyIfNull().ToString();
                    tableRow.Controls.Add(siraCell);
                    tableRow.Controls.Add(adresCell);
                    TasinmazAdresTable.Rows.Add(tableRow);
                    if (sira > 2 && dataTable.Rows.Count>3)
                    {
                        TableRow tableRow1 = new TableRow();
                        TableCell siraCell1 = new TableCell();
                        siraCell1.Text = "...";
                        TableCell adresCell1 = new TableCell();
                        adresCell1.Text = " +"+(dataTable.Rows.Count-3)+" Taşınmaz daha... (Toplam "+ dataTable.Rows.Count+" Taşınmaz.)";
                        tableRow1.Controls.Add(siraCell1);
                        tableRow1.Controls.Add(adresCell1);
                        TasinmazAdresTable.Rows.Add(tableRow1);
                        break;
                    }
                }
            }
        }

        private void TeminatBilgileriniDoldur(KiraSozlesme kiraSozlesme)
        {
            if (kiraSozlesme != null)
            {
                string teminatCinsi = string.IsNullOrEmpty(kiraSozlesme.TeminatCinsi) ? ProjeConstants.TEMINATCINSI_NAKIT_TL : kiraSozlesme.TeminatCinsi;
                UtilityHelper.SetDDLValue(TeminatCinsiDDL, teminatCinsi);
                TeminatTarihiTxt.Value = kiraSozlesme.TeminatOdemeTarihi.ConvertToDatetimeEmptyIfNull();
                TeminatAciklamaTxt.Text = kiraSozlesme.TeminatAciklama;
                TeminatTutariTxt.Value = kiraSozlesme.TeminatTutari.ToString("N", culturInfo);
                OdenenTeminatTxt.Value = kiraSozlesme.OdenenTeminatTutari.ToString("N", culturInfo);
                IadeTeminatTxt.Value = kiraSozlesme.IadeTeminatTutari.ToString("N", culturInfo);
                KalanTeminatTxt.Value = kiraSozlesme.KalanTeminatTutari.ToString("N", culturInfo);
            }

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
        private void TeminatCinsiDDLDoldur()
        {

            if (TeminatCinsiDDL.SelectedItem == null)
            {
                TeminatCinsiDDL.Items.Clear();

                TeminatCinsiDDL.Items.Add(new ListItem(ProjeConstants.TEMINATCINSI_NAKIT_TL, ProjeConstants.TEMINATCINSI_NAKIT_TL));
                TeminatCinsiDDL.Items.Add(new ListItem(ProjeConstants.TEMINATCINSI_BANKATEMINATMEKTUBU, ProjeConstants.TEMINATCINSI_BANKATEMINATMEKTUBU));
                TeminatCinsiDDL.Items.Add(new ListItem(ProjeConstants.TEMINATCINSI_IPOTEK, ProjeConstants.TEMINATCINSI_IPOTEK));
                TeminatCinsiDDL.Items.Add(new ListItem(ProjeConstants.TEMINATCINSI_NAKIT_USD, ProjeConstants.TEMINATCINSI_NAKIT_USD));
                TeminatCinsiDDL.Items.Add(new ListItem(ProjeConstants.TEMINATCINSI_NAKIT_EURO, ProjeConstants.TEMINATCINSI_NAKIT_EURO));
                
            }
            if (TeminatCinsiDDL.Items.FindByValue(ProjeConstants.TEMINATCINSI_NAKIT_TL) != null)
                TeminatCinsiDDL.SelectedValue = TeminatCinsiDDL.Items.FindByValue(ProjeConstants.TEMINATCINSI_NAKIT_TL).Value;
        }

        #region Tablo
        private void TabloOlustur(int kiraciId)
        {
            var jsonData = TabloJson(kiraciId); //veri çekilip json a çeviriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string TabloJson(int kiraciId)
        {
            string jSon = string.Empty;

            try
            {
                List<TeminatIslemListItem> list = GetDataList(kiraciId);
                var serializer = new JavaScriptSerializer();
                jSon = serializer.Serialize(list);
            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper();
                exceptionHelper.Exceptions.Add(exception);
                exceptionHelper.PublishException();
            }
            return jSon;
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
                item2.IslemTutari = item.IslemTutari.ToString("N", culturInfo)+" TL";
                item2.Duzenle = "<a href=# onclick=GuncelleModalDoldur('" + item.Id + "'); class=\'btn btn-outline-primary \'>Düzenle</a>";
                item2.Sil = "<a href=# onclick=DeleteModalDoldur('" + item.Id + "'); class=\'btn btn-outline-danger \'>Sil</a>";
                item2.KiraciId = item.KiraciId.ToString();
                list2.Add(item2);
            }
            return list2;
        }
        private string CreateDataTable(string jsonData)
        {
            string titleStr ="'" + AdiLbl.Text + " Teminat İşlemleri" +"'";
            string kiraciAdiStr = AdiLbl.Text.ReplaceTrChars().Replace(" ", "").Replace(".", "");
            kiraciAdiStr ="'" + kiraciAdiStr.Substring(0, kiraciAdiStr.Length > 20?20: kiraciAdiStr.Length-1) + 
                "TeminatIslemleri" + DateTime.Now.ConvertToDDMMYYYHHmmFormat().Replace(" ", "").Replace(".", "").Replace(" ", "-")+"'";
            string tableString = @"
                if ( jQuery.fn.DataTable.isDataTable('#CustomDataTable') ) {
                    jQuery('#CustomDataTable').DataTable().destroy();
                }
                jQuery('#CustomDataTable tbody').empty();

                jQuery.fn.dataTable.moment('DD.MM.YYYY HH:mm');//sort date
                jQuery(document).ready(function () {
                    jQuery('#CustomDataTable').DataTable({
                        'initComplete': function (settings, json) {//tablo yüklendiğinde
                            var api = this.api();
                            var row = api.row(function (idx, data, node) { //secilen toplantıya gider
                                return data['Secildi'] == true;
                            });
                            if (row.length > 0) {
                                row.select()
                                    .show()
                                    .draw(false);
                            }
                        },
                        data: " + jsonData + @",
                        columns: [
                            { data: 'IslemTarihi'},
                            { data: 'IslemTipi'},
                            { data: 'IslemTutari'},
                            { data: 'IslemAciklama'},
                            { data: 'Duzenle'},
                            { data: 'Sil'},
                        ],
                        columnDefs: [
                            {
                                targets: 2,
                                className: 'dt-body-right'
                            }
                          ],
                        'language': {
                            'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                            'decimal': ',',
                            'thousands': '.'
                        },
                        responsive: true,
                        destroy: true,
                        pageLength:10,
                        dom: 'Brti',
                        buttons: [
                            {
                                extend: 'print',
                                exportOptions: {
                                    columns: ':visible'
                                }
                            },
                            {
                                extend: 'excel',
                                title: " + titleStr+  @",
                                filename: function(){
                                    var d = new Date();
                                    var n = d.getTime();
                                    return " + kiraciAdiStr+ @";
                                },
                                exportOptions: {
                                    columns: ':visible'
                                }
                            },
                            {
                                extend: 'pdf',
                                title: "+ titleStr+  @",
                                filename: function(){
                                    var d = new Date();
                                    var n = d.getTime();
                                    return " + kiraciAdiStr+ @";
                                },
                                exportOptions: {
                                    columns: ':visible'
                                }
                            },
                            {
                                extend: 'copy',
                                exportOptions: {
                                    columns: ':visible'
                                }
                            },
                            , 'pageLength', 'colvis'
                        ],
                        });
                    });

            ";

            return tableString;
        }
            
        #endregion
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
                    string teminatCinsi = string.IsNullOrEmpty(TeminatCinsiDDL.SelectedItem.Value) ? ProjeConstants.TEMINATCINSI_NAKIT_TL : TeminatCinsiDDL.SelectedItem.Value;
                    kiraSozlesme.TeminatCinsi = teminatCinsi;
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
                            mahsupEdilecekKiraSozlesme = new KiraSozlesme();
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
                            TabloOlustur(kiraSozlesme.KiraciId);
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
                    TabloOlustur(kiraSozlesme.KiraciId);
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
            public string Duzenle { get; set; }
            public string Sil { get; set; }

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
