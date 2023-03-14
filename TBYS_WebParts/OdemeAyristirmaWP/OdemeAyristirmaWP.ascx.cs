using DAO.Ortak;
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

namespace TBYS_WebParts.OdemeAyristirmaWP
{
    [ToolboxItemAttribute(false)]
    public partial class OdemeAyristirmaWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public OdemeAyristirmaWP()
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
        private string KiraEkstreAktarmaIdQS
        {
            get
            {
                if (ViewState["KiraEkstreAktarmaId"] == null)
                {
                    if (Page.Request.QueryString["KiraEkstreAktarmaId"] != null)
                    {
                        ViewState["KiraEkstreAktarmaId"] = Page.Request.QueryString["KiraEkstreAktarmaId"];
                    }
                    else
                    {
                        ViewState["KiraEkstreAktarmaId"] = string.Empty;
                    }
                }
                return ViewState["KiraEkstreAktarmaId"].ToString();
            }

            set
            {
                ViewState["KiraEkstreAktarmaId"] = value;
            }
        }
        private List<OdemeListItem> OdemeAyristirmaListQS
        {
            get
            {
                if (ViewState["OdemeAyristirmaList"] == null)
                {
                    if (Page.Request.QueryString["OdemeAyristirmaList"] != null)
                    {
                        ViewState["OdemeAyristirmaList"] = Page.Request.QueryString["OdemeAyristirmaList"];
                    }
                    else
                    {
                        ViewState["OdemeAyristirmaList"] = new List<OdemeListItem>();
                    }
                }
                return (List<OdemeListItem>)ViewState["OdemeAyristirmaList"];
            }

            set
            {
                ViewState["OdemeAyristirmaList"] = value;
            }
        }
        private readonly IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                if (!Page.IsPostBack)
                {
                    KiraEkstreAktarma kiraEkstreAktarma = new KiraEkstreAktarma();
                    kiraEkstreAktarma = kiraEkstreAktarma.Select<KiraEkstreAktarma>(KiraEkstreAktarmaIdQS.ConvertToInt());
                    if (kiraEkstreAktarma!=null)
                    {
                        Kiraci kiraci = new Kiraci();
                        kiraci = kiraci.Select(kiraEkstreAktarma.KiraciId);
                        if (kiraci != null)
                        {
                            AdiLbl.Text = kiraci == null ? "" : kiraci.Adi + " " + kiraci.Soyadi;
                        }
                        OdemeBilgileriniDoldur(kiraEkstreAktarma);
                        TabloOlustur(kiraEkstreAktarma);//aktarıldı ise buttonları göstermesin
                        if (kiraEkstreAktarma.AktarildiMi)
                        {
                            DisableAllButtons();
                        }
                    }
                    else
                    {
                        MessageHelper.PublishMessage("Ödeme Bulunamadı.", ProjeConstants.MESAJ_HATA);
                    }
                }
            }
            catch (Exception exception)
            {

                ExceptionHelper exhelper = new ExceptionHelper(exception);
                exhelper.PublishException();
            }
        }
        private void OdemeBilgileriniDoldur(KiraEkstreAktarma kiraEkstreAktarma)
        {
            if (kiraEkstreAktarma != null)
            {

                OdemeTarihiLbl.Text = kiraEkstreAktarma.OdemeTarihi.ConvertToDDMMYYYHHmmFormat();
                OdemeAciklamaTxt.Text = kiraEkstreAktarma.Aciklama;
                OdenenTutarLbl.Text = kiraEkstreAktarma.Tutar.ToString("N", culturInfo);
                KiraTutariLbl.Text = string.Empty.ReturnZeroIfNull().ConvertToDecimal().ToString("N", culturInfo);
                KesinTeminatLbl.Text = string.Empty.ReturnZeroIfNull().ConvertToDecimal().ToString("N", culturInfo);
                GeciciTeminatLbl.Text = string.Empty.ReturnZeroIfNull().ConvertToDecimal().ToString("N", culturInfo);
                KalanTutarLbl.Text = OdenenTutarLbl.Text;
                if (kiraEkstreAktarma.Uyari)
                {
                    if (!kiraEkstreAktarma.AktarildiMi)
                        OdemeyiSozlesmelereBolBtn.Visible = true;
                    //ekstreAktarmaListItem.KiraciAdi = "<a herf=# class='btn btn-outline-danger text-left' style='white-space:normal'  onclick=OpenModalOdemeBolustur(" + ekstreAktarmaListItem.KiraEkstreAktarmaId + ");> Böl ve Öde</a>";
                }
                else
                {
                    OdemeyiSozlesmelereBolBtn.Visible = false;
                }

            }

        }

        #region CustomDataTable 
        private void TabloOlustur(KiraEkstreAktarma kiraEkstreAktarma)
        {
            var jsonData = TabloJson(kiraEkstreAktarma); //veri çekilip json a çeviriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string TabloJson(KiraEkstreAktarma kiraEkstreAktarma)
        {
            string jSon = string.Empty;
            if (kiraEkstreAktarma!=null)
            {
                if (kiraEkstreAktarma.AktarildiMi)
                {
                    AktarildiMiLbl.Text = " ( A K T A R I L D I )";
                    OdemeAyristirmaTabledanDoldur(kiraEkstreAktarma);
                }
                try
                {
                    var serializer = new JavaScriptSerializer();
                    jSon = serializer.Serialize(OdemeAyristirmaListQS);
                }
                catch (Exception exception)
                {
                    ExceptionHelper exceptionHelper = new ExceptionHelper();
                    exceptionHelper.Exceptions.Add(exception);
                    exceptionHelper.PublishException();
                } 
            }
            return jSon;
        }

        private void OdemeAyristirmaTabledanDoldur(KiraEkstreAktarma kiraEkstreAktarma)
        {
            OdemeAyristirma odemeAyristirma = new OdemeAyristirma();
            DataTable dataTable = odemeAyristirma.SelectByKiraEkstreAktarmaId(kiraEkstreAktarma.Id);
            if (dataTable != null)
            {
                OdemeAyristirmaListQS.Clear();
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    int kiraciId = dataRow["KiraciId"].ReturnZeroIfNull().ConvertToInt();
                    string odemeTarihi = dataRow["OdemeTarihi"].ReturnEmptyIfNull().ConvertToDDMMYYYHHmmFormat();
                    string odemeSebebi = dataRow["OdemeSebebi"].ToString();
                    decimal tutar = dataRow["Tutar"].ConvertToDecimal();
                    string kiraci = dataRow["AdiSoyadi"].ToString();


                    OdemeListItem odemeListItem = new OdemeListItem();


                    odemeListItem.OdemeTarihi = odemeTarihi;
                    odemeListItem.Tutar = tutar;
                    odemeListItem.OdemeSebebi = odemeSebebi;
                    odemeListItem.KiraEkstreAktarmaId = kiraEkstreAktarma.Id;
                    odemeListItem.KiraSozlesmeId = SozlesmeGetir(kiraciId, OdemeTarihiLbl.Text.ConvertToDatetime());
                    odemeListItem.OdemeId = kiraEkstreAktarma.OdemeId;
                    odemeListItem.Kiraci = kiraci +"(KiraciNo:"+kiraciId+" SözleşmeNo:"+ odemeListItem.KiraSozlesmeId+")";

                    OdemeAyristirmaListQS.Add(odemeListItem);
                    if (ToplamlariDuzenle(kiraEkstreAktarma.AktarildiMi))
                    {
                        OdemeAyristirmaListQS.Remove(odemeListItem);//ödenen tutar aşıldı ise listeden çıkar
                    }
                }
            }
        }

        private string CreateDataTable(string jsonData)
        {
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
                            { data: 'OdemeTarihi'},
                            { data: 'OdemeSebebi'},
                            { data: 'Tutar'},
                            { data: 'Kiraci'},
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
                        pageLength:100,
                        dom: 'rti',
                        
                        });
                    });

            ";

            return tableString;
        }
        #endregion
        protected void KaydetBtn_Click(object sender, EventArgs e)
        {
            KiraEkstreAktarma kiraEkstreAktarma = new KiraEkstreAktarma();
            kiraEkstreAktarma = kiraEkstreAktarma.Select<KiraEkstreAktarma>(KiraEkstreAktarmaIdQS.ConvertToInt());
            OdemeAyristirmaAtomicKaydet(kiraEkstreAktarma);
            DisableAllButtons();
            TabloOlustur(kiraEkstreAktarma);
        }

        private void OdemeAyristirmaAtomicKaydet(KiraEkstreAktarma kiraEkstreAktarma)
        {
            try
            {
                DbClass db = new DbClass();
                List<OdemeAyristirma> odemeAyristirmaList = new List<OdemeAyristirma>();
                foreach (OdemeListItem item in OdemeAyristirmaListQS)
                {
                    DBObject odemeDbo = new DBObject();
                    DBObject teminatDbo = new DBObject();
                    DBObject odemeAyristirmaInsertDbo = new DBObject();
                    OdemeAyristirma odemeAyristirma = new OdemeAyristirma();
                    if (item.OdemeSebebiId == ProjeConstants.ODEMESEBEBI_KIRA_INT)
                    {
                        #region Kira Ödemesi
                        Odeme kiraOdemesi = new Odeme();
                        kiraOdemesi.Aciklama = item.Aciklama;
                        kiraOdemesi.KiraciId = item.KiraciId;
                        kiraOdemesi.SozlesmeId = item.KiraSozlesmeId;
                        kiraOdemesi.OdemePlaniId = OdemePlaniGetir(item.KiraSozlesmeId, item.OdemeTarihi.ConvertToDatetime());
                        kiraOdemesi.OdemeTarihi = item.OdemeTarihi.ConvertToDatetime();
                        kiraOdemesi.OdenenTutar = item.Tutar;
                        kiraOdemesi.Olusturan = UtilityHelper.GetCurrentUserName();
                       
                        odemeDbo.SQLString = kiraOdemesi.GetInsertSQL("{0}");
                        odemeDbo.SQLType = ProjeConstants.SQL_INSERT;
                        odemeDbo.UseReturnIdAsParam = false;
                        odemeDbo.IsFilled = true;
                        db.DBObjectList.Add(odemeDbo);
                        odemeAyristirma.OdemeId = ProjeConstants.SQL_GENERIC_INT_VALUE;
                        odemeAyristirmaInsertDbo.DbObjectParamIndex = db.DBObjectList.IndexOf(odemeDbo);
                        #endregion

                    }
                    else if ((item.OdemeSebebiId == ProjeConstants.ODEMESEBEBI_KESINTEMINAT_INT)||
                        (item.OdemeSebebiId == ProjeConstants.ODEMESEBEBI_GECICITEMINAT_INT))
                    {
                        KiraSozlesme kiraSozlesme = new KiraSozlesme();
                        kiraSozlesme = kiraSozlesme.Select(item.KiraSozlesmeId);
                        if (kiraSozlesme != null)
                        {

                            TeminatIslem teminatIslem = new TeminatIslem();
                            teminatIslem.KiraciId = kiraSozlesme.KiraciId;
                            teminatIslem.Aciklama = item.Aciklama;
                            teminatIslem.DovizCinsi = ProjeConstants.DOVIZ_TL;
                            teminatIslem.IslemTarihi = item.OdemeTarihi.ConvertToDatetime();
                            teminatIslem.IslemTipi = item.OdemeSebebiId == ProjeConstants.ODEMESEBEBI_KESINTEMINAT_INT ? ProjeConstants.TEMINAT_ODEMESI : ProjeConstants.TEMINAT_GECICITEMINATODEMESI;
                            teminatIslem.IslemTutari = item.Tutar;
                            teminatIslem.Olusturan = UtilityHelper.GetCurrentUserName();

                            
                            teminatDbo.SQLString = teminatIslem.GetInsertSQL("{0}");
                            teminatDbo.SQLType = ProjeConstants.SQL_INSERT;
                            teminatDbo.UseReturnIdAsParam = false;
                            teminatDbo.IsFilled = true;
                            db.DBObjectList.Add(teminatDbo);
                            odemeAyristirma.TeminatIslemId = ProjeConstants.SQL_GENERIC_INT_VALUE;
                            odemeAyristirmaInsertDbo.DbObjectParamIndex = db.DBObjectList.IndexOf(teminatDbo);
                        }
                    }
                    #region odemeAyristirma
                    
                    odemeAyristirma.KiraEkstreAktarmaId = item.KiraEkstreAktarmaId;
                    odemeAyristirma.KiraciId = item.KiraciId;
                    odemeAyristirma.SozlesmeId = item.KiraSozlesmeId;
                    odemeAyristirma.DovizCinsi = item.DovizCinsi;
                    odemeAyristirma.OdemeSebebiId = item.OdemeSebebiId;
                    
                    
                    
                    odemeAyristirma.OdemeTarihi = item.OdemeTarihi.ConvertToDatetime();
                    odemeAyristirma.OdemeSaati = item.OdemeSaati;
                    odemeAyristirma.Tutar = item.Tutar;
                    odemeAyristirma.Aciklama = item.Aciklama;
                    odemeAyristirma.Olusturan = UtilityHelper.GetCurrentUserName();

                    
                    odemeAyristirmaInsertDbo.SQLString = odemeAyristirma.GetInsertSQL("{0}");
                    odemeAyristirmaInsertDbo.SQLType = ProjeConstants.SQL_INSERT;
                    odemeAyristirmaInsertDbo.UseReturnIdAsParam = true;
                    

                    odemeAyristirmaInsertDbo.IsFilled = true;
                    db.DBObjectList.Add(odemeAyristirmaInsertDbo);

                    #endregion




                }
                #region update KiraEkstreAktarildiMi=true 
                
                if (kiraEkstreAktarma != null)
                {
                    kiraEkstreAktarma.Aciklama += "--Ödeme Ayrıştırıldı--";
                    kiraEkstreAktarma.AktarildiMi = true;
                    kiraEkstreAktarma.Degistiren = UtilityHelper.GetCurrentUserName();

                    DBObject kiraEkstreAktarmaDbo = new DBObject();
                    kiraEkstreAktarmaDbo.SQLString = kiraEkstreAktarma.GetUpdateSQL(kiraEkstreAktarma.Id.ToString());
                    kiraEkstreAktarmaDbo.SQLType = ProjeConstants.SQL_UPDATE;
                    kiraEkstreAktarmaDbo.IsFilled = true;
                    db.DBObjectList.Add(kiraEkstreAktarmaDbo);

                }

                List<DBObject> savedDBOList = db.ExecuteTransaction();
                if (savedDBOList.Count > 0)
                {
                    MessageHelper.PublishMessage("Ödeme Bilgileri Kaydedildi", ProjeConstants.MESAJ_BASARILI, 2000);
                }
                else
                {
                    MessageHelper.PublishMessage("Ödeme Bilgileri Kaydedilemedi", ProjeConstants.MESAJ_HATA);
                }
                #endregion
                
            }
            catch (Exception ex)
            {
                MessageHelper.PublishMessage(ex.Message, ProjeConstants.MESAJ_HATA);

            }
        }

        private void DisableAllButtons()
        {
            KiraOdemesiEkleBtn.Enabled = false;
            KiraOdemesiEkleBtn.Visible = false;
            KesinTeminatEkleBtn.Enabled = false;
            KesinTeminatEkleBtn.Visible = false;
            GeciciTeminatEkleBtn.Enabled = false;
            GeciciTeminatEkleBtn.Visible = false;
            OdemeyiSozlesmelereBolBtn.Enabled = false;
            OdemeyiSozlesmelereBolBtn.Visible = false;
            KaydetBtn.Enabled = false;  
            KaydetBtn.Visible = false;  
        }

        private int OdemePlaniGetir(int kiraSozlesmeId,DateTime odemeTarihi)
        {
            OdemePlani odemePlani = new OdemePlani();
            odemePlani = odemePlani.SelectBySozlesmeIdOdemeTarihi(kiraSozlesmeId, odemeTarihi);
            return odemePlani == null ? 0 : odemePlani.Id;
        }

        protected void KiraOdemesiEkleBtn_Click(object sender, EventArgs e)
        {
            OdemeEkleModalAc(ProjeConstants.ODEMESEBEBI_KIRA, ProjeConstants.ODEMESEBEBI_KIRA_INT, "Kira Ödemesi Eklenecek");
        }
        protected void OdemeyiSozlesmelereBolBtn_Click(object sender, EventArgs e)
        {
            OdemeEkleModalAc(ProjeConstants.ODEMESEBEBI_KIRA, ProjeConstants.ODEMESEBEBI_KIRA_INT, "Kira Ödemesi Eklenecek");
        }

        private void OdemeEkleModalAc(string odemeSebebi, int odemeSebebiId, string title)
        {
            KiraEkstreAktarma kiraEkstreAktarma = new KiraEkstreAktarma();
            kiraEkstreAktarma = kiraEkstreAktarma.Select<KiraEkstreAktarma>(KiraEkstreAktarmaIdQS.ConvertToInt());
            if (kiraEkstreAktarma != null)
            {


                Kiraci kiraci = new Kiraci();
                string kiraciAdi = string.Empty;
                string kiraciSoyadi = string.Empty;
                if (kiraEkstreAktarma.KiraciId > 0)
                {
                    kiraci = kiraci.Select(kiraEkstreAktarma.KiraciId);
                    if (kiraci != null)
                    {
                        kiraciAdi = kiraci.Adi.Trim();// (kiraci.Adi + " " + kiraci.Soyadi).Trim();
                        kiraciSoyadi = kiraci.Soyadi.Trim();// (kiraci.Adi + " " + kiraci.Soyadi).Trim();
                    }
                }

                Kiraci kiraciDao = new Kiraci();

                KiraciDDL.Items.Clear();

                string ekstredekiKiraciAdi = kiraEkstreAktarma.Adi;// + " " + kiraEkstreAktarma.Soyadi;
                string ekstredekiKiraciSoyadi = kiraEkstreAktarma.Soyadi;
                if (!string.IsNullOrEmpty(ekstredekiKiraciAdi) || !string.IsNullOrEmpty(kiraciAdi))
                {
                    List<Kiraci> kiraciList = kiraciDao.SelectByAdi(string.IsNullOrEmpty(kiraciAdi) ? ekstredekiKiraciAdi : kiraciAdi, string.IsNullOrEmpty(kiraciSoyadi) ? ekstredekiKiraciSoyadi : kiraciSoyadi);
                    if (kiraciList.Count > 0)
                    {

                        foreach (var item in kiraciList)
                        {
                            KiraSozlesme kiraSozlesme = new KiraSozlesme();
                            kiraSozlesme = kiraSozlesme.SelectEnYakinTarihliSozlesmeByKiraciIdTarih(item.Id, OdemeTarihiLbl.Text.ConvertToDatetime());
                            if (kiraSozlesme != null)
                            {
                                KiraciDDL.Enabled = true;
                                string sozlesme = kiraSozlesme.SozBasTar.ConvertToDatetimeEmptyIfNull() + "-" + kiraSozlesme.SozBitTar.ConvertToDatetimeEmptyIfNull();
                                string kiraciBilgisi = "* " +(item.Adi + " " + item.Soyadi).Trim() + " (" + item.Adres + ")" + " (Sözlesme:" + sozlesme + ", Kira Bedeli:"+kiraSozlesme.KiraBedeli.ToString("N", culturInfo)+")";
                                //KiraciDDL.Items.Add(new ListItem(kiraciBilgisi, item.Id.ToString()));
                                ListItem li1= new ListItem (kiraciBilgisi, item.Id.ToString());
                                li1.Attributes.Add("title", kiraciBilgisi);
                                li1.Attributes.Add("style", "white-space:normal !important");
                                KiraciDDL.Items.Add(li1);
                            }

                        }
                    }
                }
                KiraciIdLbl.Text = kiraEkstreAktarma.KiraciId.ToString();
                if (KiraciDDL.Items.Count > 0)
                {
                    KiraciIdLbl.Text = KiraciDDL.SelectedItem.Value;
                }
                OdemeTarihiLbl.Text = kiraEkstreAktarma.OdemeTarihi.ConvertToDDMMYYYHHmmFormat();
                OdemeSaatiLbl.Text = kiraEkstreAktarma.OdemeTarihi.ToString("HH:mm");

                DovizCinsiLbl.Text = kiraEkstreAktarma.DovizCinsi;
                IslemTutariTxt.Value = 0.ToString();
                IslemAciklamaTxt.Text = string.Empty;
                OdemeSebebiLbl.Text = odemeSebebi;
                OdemeSebebiIdLbl.Text = odemeSebebiId.ToString();
                KiraEkstreAktarmaIdLbl.Text = kiraEkstreAktarma.Id.ToString();
                OdemeIdLbl.Text = kiraEkstreAktarma.OdemeId.ToString();
                KaydetVeyaGuncelleHdn.Value = ProjeConstants.KAYDET;
                ModalTitleLbl.Text = title;
                UtilityHelper.ScriptCalistir("OpenOdemeEkleModal();");
            }
        }

        protected void KesinTeminatEkleBtn_Click(object sender, EventArgs e)
        {
            OdemeEkleModalAc(ProjeConstants.ODEMESEBEBI_KESINTEMINAT, ProjeConstants.ODEMESEBEBI_KESINTEMINAT_INT, "Kesin Teminat Ödemesi Eklenecek");
        } 
        protected void GeciciTeminatEkleBtn_Click(object sender, EventArgs e)
        {
            OdemeEkleModalAc(ProjeConstants.ODEMESEBEBI_GECICITEMINAT, ProjeConstants.ODEMESEBEBI_GECICITEMINAT_INT, "Geçici Teminat Ödemesi Eklenecek");
        }
        protected void ListeyeEkleNowBtn_Click(object sender, EventArgs e)
        {
            KiraEkstreAktarma kiraEkstreAktarma = new KiraEkstreAktarma();
            kiraEkstreAktarma = kiraEkstreAktarma.Select<KiraEkstreAktarma>(KiraEkstreAktarmaIdQS.ConvertToInt());
            if (kiraEkstreAktarma!=null)
            {
                decimal islemTutari = IslemTutariTxt.Value.ConvertToDecimal();
                if (islemTutari >= 0)
                {
                    OdemeListItem odemeListItem = new OdemeListItem();
                    int kiraciId = KiraciDDL.Items.Count > 0 ? KiraciDDL.SelectedItem.Value.ConvertToInt() : KiraciIdLbl.Text.ConvertToInt();
                    Kiraci kiraci = new Kiraci();
                    kiraci = kiraci.Select(kiraciId);
                    if (kiraci != null)
                    {
                        odemeListItem.KiraciId = kiraci.Id;
                        odemeListItem.Kiraci = (kiraci.Adi + " " + kiraci.Soyadi).Trim() + " (" + kiraci.Adres + ")" + " (Kiracı No:" + kiraci.Id + ")";
                    }


                    odemeListItem.OdemeTarihi = kiraEkstreAktarma.OdemeTarihi.ConvertToDDMMYYYHHmmFormat();
                    odemeListItem.OdemeSaati = kiraEkstreAktarma.OdemeTarihi.ToString("HH:mm");
                    odemeListItem.DovizCinsi = kiraEkstreAktarma.DovizCinsi;
                    odemeListItem.Tutar = islemTutari;
                    odemeListItem.Aciklama = IslemAciklamaTxt.Text;
                    odemeListItem.OdemeSebebiId = OdemeSebebiIdLbl.Text.ConvertToInt();
                    odemeListItem.OdemeSebebi = OdemeSebebiLbl.Text;
                    odemeListItem.KiraEkstreAktarmaId = kiraEkstreAktarma.Id;
                    odemeListItem.KiraciId = KiraciDDL.SelectedItem.Value.ConvertToInt();
                    odemeListItem.KiraSozlesmeId = SozlesmeGetir(kiraci.Id, OdemeTarihiLbl.Text.ConvertToDatetime());
                    odemeListItem.OdemeId = kiraEkstreAktarma.OdemeId;
                    
                    if (!kiraEkstreAktarma.AktarildiMi)
                    {
                        odemeListItem.Duzenle = "<a href='#' class='btn btn-outline-primary' onclick=GuncelleModalDoldur('" + odemeListItem.GuId + "');>Düzenle</a>";
                        odemeListItem.Sil = "<a href='#' class='btn btn-outline-danger' onclick=SatirSil('" + odemeListItem.GuId + "')>Sil</a>";
                    }
                    

                    OdemeAyristirmaListQS.Add(odemeListItem);
                    if (ToplamlariDuzenle(kiraEkstreAktarma.AktarildiMi)) 
                    {
                        OdemeAyristirmaListQS.Remove(odemeListItem);//ödenen tutar aşıldı ise listeden çıkar
                    }
                }
                else
                {
                    MessageHelper.PublishMessage("Ödenen tutarı aştınız.", ProjeConstants.MESAJ_HATA, 2000);
                } 
            }
            TabloOlustur(kiraEkstreAktarma);
            UtilityHelper.ScriptCalistir("CloseModal()");
        }

        private int SozlesmeGetir(int kiraciId, DateTime odemeTarihi)
        {
            int sozlesmeId = 0;
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            kiraSozlesme = kiraSozlesme.SelectEnYakinTarihliSozlesmeByKiraciIdTarih(kiraciId, odemeTarihi);
            if (kiraSozlesme != null)
            {
                sozlesmeId=kiraSozlesme.Id;
            }
            return sozlesmeId;
        }

        protected void ModalGuncelleBtn_Click(object sender, EventArgs e)
        {
            OdemeGuncelleModalAc();
        }

        private void OdemeGuncelleModalAc()
        {
            string guid = GuncelleGuidHdn.Value;
            var odemeListItem = OdemeAyristirmaListQS.Find(x => x.GuId == guid);
            if (odemeListItem != null)
            {

                IslemTutariTxt.Value = odemeListItem.Tutar.ToString("N", culturInfo);
                IslemAciklamaTxt.Text = odemeListItem.Aciklama;
                OdemeSebebiIdLbl.Text = odemeListItem.OdemeSebebiId.ToString();
                OdemeSebebiLbl.Text = odemeListItem.OdemeSebebi;
                UtilityHelper.SetDDLValue(KiraciDDL, odemeListItem.KiraciId.ToString());
                KiraciDDL.Enabled = false;
                KaydetVeyaGuncelleHdn.Value = ProjeConstants.GUNCELLE;
                ModalTitleLbl.Text = "Ödeme Güncellenecek";
                UtilityHelper.ScriptCalistir("OpenOdemeEkleModal();");
            }
            else
            {
                MessageHelper.PublishMessage("Kayıt bulunamadı", ProjeConstants.MESAJ_HATA, 2000);
            }

        }

        protected void ListeyiGuncelleNowBtn_Click(object sender, EventArgs e)
        {
            KiraEkstreAktarma kiraEkstreAktarma = new KiraEkstreAktarma();
            kiraEkstreAktarma = kiraEkstreAktarma.Select<KiraEkstreAktarma>(KiraEkstreAktarmaIdQS.ConvertToInt());
            decimal islemTutari = IslemTutariTxt.Value.ConvertToDecimal();
            if (islemTutari >= 0)
            {
                
                string guid = GuncelleGuidHdn.Value;
                var odemeListItem = OdemeAyristirmaListQS.Find(x => x.GuId == guid);
                decimal oncekiTutar = odemeListItem.Tutar;

                odemeListItem.Tutar = IslemTutariTxt.Value.ConvertToDecimal();
                odemeListItem.Aciklama = IslemAciklamaTxt.Text;
                odemeListItem.KiraciId = KiraciDDL.SelectedItem.Value.ConvertToInt();
                odemeListItem.KiraSozlesmeId = SozlesmeGetir(odemeListItem.KiraciId, OdemeTarihiLbl.Text.ConvertToDatetime());
               

                if (ToplamlariDuzenle(kiraEkstreAktarma==null || kiraEkstreAktarma.AktarildiMi))
                {
                    odemeListItem.Tutar = oncekiTutar;//ödenen tutar aşıldı ise eski tutara dön
                }
            }
            else
            {
                MessageHelper.PublishMessage("Ödenen tutarı aştınız.", ProjeConstants.MESAJ_HATA, 2000);
            }
            TabloOlustur(kiraEkstreAktarma);
            UtilityHelper.ScriptCalistir("CloseModal()");
        }
        private bool ToplamlariDuzenle(bool aktarildiMi)
        {
            bool toplamAsildiMi = false;
            decimal odenenTutar = OdenenTutarLbl.Text.ConvertToDecimal();
            decimal kesinTeminat = 0;
            decimal geciciTeminat =0;
            decimal kira =0;

            foreach (var item in OdemeAyristirmaListQS)
            {
                decimal islemTutari = item.Tutar;
                switch (item.OdemeSebebiId)
                {
                    case ProjeConstants.ODEMESEBEBI_KESINTEMINAT_INT:
                        {
                            kesinTeminat += islemTutari;
                            break;
                        }
                    case ProjeConstants.ODEMESEBEBI_GECICITEMINAT_INT:
                        {
                            geciciTeminat += islemTutari;
                            break;
                        }
                    case ProjeConstants.ODEMESEBEBI_KIRA_INT:
                        {
                            kira += islemTutari;
                            break;
                        }
                }
            }

            decimal toplam = kira + kesinTeminat + geciciTeminat ;
            decimal fark = odenenTutar - toplam;
            if (fark < 0)
            {
                toplamAsildiMi = true;
                MessageHelper.PublishMessage("Toplam ödenen tutarı aştınız, girdiğiniz tutar listeye eklenmedi.", ProjeConstants.MESAJ_HATA, 2000);
            }
            else
            {
                KesinTeminatLbl.Text = kesinTeminat.ToString("N", culturInfo);
                GeciciTeminatLbl.Text = geciciTeminat.ToString("N", culturInfo);
                KiraTutariLbl.Text = kira.ToString("N", culturInfo);
                decimal kalanTutar = odenenTutar - kesinTeminat - geciciTeminat - kira;
                KalanTutarLbl.Text = kalanTutar.ToString("N", culturInfo); 
            }
            if ((fark == 0) && !aktarildiMi)
            {
                KaydetBtn.Visible = true;
            }
            else
            {
                KaydetBtn.Visible = false;
            }
            return toplamAsildiMi;
        }

        protected void SatirSilBtn_Click(object sender, EventArgs e)
        {
            string guid = SilGuidHdn.Value;
            var odemeListItem = OdemeAyristirmaListQS.Find(x => x.GuId == guid);
            if (odemeListItem != null)
            {
                OdemeAyristirmaListQS.Remove(odemeListItem);
                KiraEkstreAktarma kiraEkstreAktarma = new KiraEkstreAktarma();
                kiraEkstreAktarma = kiraEkstreAktarma.Select<KiraEkstreAktarma>(KiraEkstreAktarmaIdQS.ConvertToInt());

                TabloOlustur(kiraEkstreAktarma);
                ToplamlariDuzenle(kiraEkstreAktarma == null || kiraEkstreAktarma.AktarildiMi);
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
        [Serializable]
        private class OdemeListItem
        {
            private string _guid=Guid.NewGuid().ToString();
            public string GuId {
                get
                {
                    return _guid;
                }

                set
                {
                    _guid = value;
                }
            }
            public int KiraEkstreAktarmaId { get; set; }
            public int OdemeSebebiId { get; set; }
            public string OdemeSebebi { get; set; }
            public int KiraciId { get; set; }
            public int KiraSozlesmeId { get; set; }
            public string Kiraci { get; set; }
            public string OdemeTarihi { get; set; }
            public string OdemeSaati { get; set; }
            public decimal Tutar { get; set; }
            public string DovizCinsi { get; set; }
            public int OdemeId { get; set; }
            public string Aciklama { get; set; }
            public string Duzenle { get; set; }
            public string Sil { get; set; }

        }
        protected void KiraEkstreAktarmaBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_KIRAEKSTRE_LIST+ "?SecilenId=" + KiraEkstreAktarmaIdQS);
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
    }
}
