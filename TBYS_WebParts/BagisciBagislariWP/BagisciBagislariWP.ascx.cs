using DocumentFormat.OpenXml.Spreadsheet;
using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Transactions;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.BagisciBagislariWP
{
    [ToolboxItemAttribute(false)]
    public partial class BagisciBagislariWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public BagisciBagislariWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
        private string BagisciIdQS
        {
            get
            {

                if (ViewState["BagisciId"] == null)
                {
                    if (Page.Request.QueryString["BagisciId"] != null)
                    {
                        ViewState["BagisciId"] = Page.Request.QueryString["BagisciId"];
                    }
                    else
                    {
                        ViewState["BagisciId"] = string.Empty;
                    }
                }
                return ViewState["BagisciId"].ToString();
            }

            set
            {
                ViewState["BagisciId"] = value;
            }
        }
        private string BagisIdQS
        {
            get
            {

                if (ViewState["BagisId"] == null)
                {
                    if (Page.Request.QueryString["BagisId"] != null)
                    {
                        ViewState["BagisId"] = Page.Request.QueryString["BagisId"];
                    }
                    else
                    {
                        ViewState["BagisId"] = string.Empty;
                    }
                }
                return ViewState["BagisId"].ToString();
            }

            set
            {
                ViewState["BagisId"] = value;
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
        private string MesajQS
        {
            get
            {

                if (ViewState["Mesaj"] == null)
                {
                    if (Page.Request.QueryString["Mesaj"] != null)
                    {
                        ViewState["Mesaj"] = Page.Request.QueryString["Mesaj"];
                    }
                    else
                    {
                        ViewState["Mesaj"] = string.Empty;
                    }
                }
                return ViewState["Mesaj"].ToString();
            }

            set
            {
                ViewState["Mesaj"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(MesajQS))
                {
                    MessageHelper.PublishMessage("Taşınmaz Eklendi", ProjeConstants.MESAJ_BASARILI, 2000);
                    MesajQS = string.Empty;
                }
                TasinmazBagisci bagisci = new TasinmazBagisci();
                bagisci = bagisci.Select<TasinmazBagisci>(BagisciIdQS.ConvertToInt());
                if (bagisci != null)
                {
                    BagisciTasinmazlarTablosunuDoldur(bagisci);
                }
                else
                {
                    MessageHelper.PublishMessage("Bağışçı bulunamadı", ProjeConstants.MESAJ_HATA);
                }

            }
            catch (Exception exception)
            {
                ExceptionHelper exHelper = new ExceptionHelper(exception);
                exHelper.PublishException();
            }
        }
        private void BagisTasinmazLarTableHeaders()
        {
            BagisTasinmazLarTable.Rows.Clear();
            TableHeaderRow th = new TableHeaderRow();

            TableCell siranoCell = new TableCell();
            siranoCell.Text = "Sıra no";

            TableCell cinsiCell = new TableCell();
            cinsiCell.Text = "Cinsi";

            TableCell kullanimSekliCell = new TableCell();
            kullanimSekliCell.Text = "Kullanım Şekli";

            TableCell iliCell = new TableCell();
            iliCell.Text = "İl-İlçe";

            TableCell adresCell = new TableCell();
            adresCell.Text = "Adres";

            TableCell mulkiyetCell = new TableCell();
            mulkiyetCell.Text = "Mülkiyet Şekli";

            TableCell kullanimCell = new TableCell();
            kullanimCell.Text = "Kira Durumu";

            TableCell emlakBeyanDegeriCell = new TableCell();
            emlakBeyanDegeriCell.Text = "Emlak Beyan Değeri";

            TableCell tahminiRayicDegeriCell = new TableCell();
            tahminiRayicDegeriCell.Text = "Tahmini Rayiç Değeri";

            TableCell silCell = new TableCell();
            silCell.Text = "Sil";

            th.Controls.Add(siranoCell);
            th.Controls.Add(kullanimSekliCell);
            th.Controls.Add(cinsiCell);
            th.Controls.Add(iliCell);
            th.Controls.Add(adresCell);
            th.Controls.Add(mulkiyetCell);
            th.Controls.Add(kullanimCell);
            th.Controls.Add(emlakBeyanDegeriCell);
            th.Controls.Add(tahminiRayicDegeriCell);
            th.Controls.Add(silCell);
            BagisTasinmazLarTable.Controls.Add(th);

        }
        private void BagisciTasinmazlarTablosunuDoldur(TasinmazBagisci bagisci)
        {
            TitleLbl.Text = bagisci.Adi + " " + bagisci.Soyadi + " Tarafından Yapılan Bağışlar";

            Bagis bagis = new Bagis();
            DataTable dataTable = bagis.SelectSatisVsDahilTasinmazByBagisciIdReturnDT(bagisci.Id);
            if (dataTable != null)
            {
                BagisTasinmazLarTable.Rows.Clear();
                BagisTasinmazLarTableHeaders();
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    string bagisIdStr = dataRow["BagisId"].ToString();
                    string tasinmazIdStr = dataRow["TasinmazId"].ToString();
                    string sira = dataRow["Sirano"].ToString();
                    string cinsi = dataRow["Cinsi"].ToString();
                    string ilIlce = dataRow["IlIlce"].ToString();
                    string adres = dataRow["Adres"].ToString();
                    string mulkiyetSekli = dataRow["MulkiyetSekli"].ToString();
                    string kiraDurumu = dataRow["KiraDurumu"].ToString();
                    string kullanimSekli = dataRow["KullanimSekli"].ToString();
                    string emlakBeyanDegeri = dataRow["EmlakBeyanDegeri"].ReturnZeroIfNull().ConvertToDecimal().ToString("N", culturInfo);
                    string tahminiRayicDegeri = dataRow["TahminiRayicDegeri"].ReturnZeroIfNull().ConvertToDecimal().ToString("N", culturInfo);

                    Tasinmaz satirTasinmazi = new Tasinmaz();
                    satirTasinmazi = satirTasinmazi.Select(tasinmazIdStr.ConvertToInt());
                    if (satirTasinmazi != null && satirTasinmazi.EnvanterdeMi == 0)
                    {
                        kiraDurumu = satirTasinmazi.EnvanterdenCikmaSebebi;
                    }

                    bool envanterdenCikmis = satirTasinmazi != null && satirTasinmazi.EnvanterdeMi == 0;
                    string satirCssClass = envanterdenCikmis ? "text-body-secondary" : string.Empty;

                    TableRow row = new TableRow();
                    if (envanterdenCikmis)
                    {
                        row.CssClass = satirCssClass;
                    }

                    TableCell SiraNoCell = new TableCell();
                    SiraNoCell.Text = sira;
                    SiraNoCell.CssClass = satirCssClass;
                    row.Controls.Add(SiraNoCell);

                    TableCell KullanimSekliCell = new TableCell();
                    KullanimSekliCell.Text = kullanimSekli;
                    KullanimSekliCell.CssClass = satirCssClass;
                    row.Controls.Add(KullanimSekliCell);

                    TableCell CinsiCell = new TableCell();
                    CinsiCell.Text = cinsi;
                    CinsiCell.CssClass = satirCssClass;
                    row.Controls.Add(CinsiCell);

                    TableCell IlICell = new TableCell();
                    IlICell.Text = ilIlce;
                    IlICell.CssClass = satirCssClass;
                    row.Controls.Add(IlICell);

                    TableCell AdresCell = new TableCell();
                    AdresCell.Text = adres;
                    AdresCell.CssClass = satirCssClass;
                    row.Controls.Add(AdresCell);

                    TableCell MulkiyetCell = new TableCell();
                    MulkiyetCell.Text = mulkiyetSekli;
                    MulkiyetCell.CssClass = satirCssClass;
                    row.Controls.Add(MulkiyetCell);

                    TableCell KullanimCell = new TableCell();
                    KullanimCell.Text = kiraDurumu;
                    KullanimCell.CssClass = satirCssClass;
                    row.Controls.Add(KullanimCell);

                    TableCell EmlakBeyanDegeriCell = new TableCell();
                    EmlakBeyanDegeriCell.Text = emlakBeyanDegeri;
                    EmlakBeyanDegeriCell.CssClass = satirCssClass;
                    row.Controls.Add(EmlakBeyanDegeriCell);

                    TableCell TahminiRayicDegeriCell = new TableCell();
                    TahminiRayicDegeriCell.Text = tahminiRayicDegeri;
                    TahminiRayicDegeriCell.CssClass = satirCssClass;
                    row.Controls.Add(TahminiRayicDegeriCell);

                    TableCell CikarCell = new TableCell();
                    LinkButton CikarBtn = new LinkButton();
                    CikarBtn.Text = "Çikar";
                    CikarBtn.CssClass = "btn btn-outline-danger btn-sm";

                    CikarBtn.ID = "CikarBtn" + sira;
                    TableUpdatePanel.ContentTemplateContainer.Controls.Add(CikarBtn);
                    CikarBtn.Click += delegate
                    {
                        CikarLbl.Text = "Lütfen Dikkat: Taşınmaz Bağışlardan Çıkarılacak";
                        CikarMesajiLbl.Text = "Seçilen Taşınmazı Bağışlardan Çıkarmak İstediğinizden Emin misiniz?";
                        //TasinmazIdLbl.Text = tasinmazIdStr;
                        Tasinmaz tasinmaz = new Tasinmaz();
                        tasinmaz = tasinmaz.Select<Tasinmaz>(tasinmazIdStr.ConvertToInt());
                        TasinmazAdresLbl.Text = tasinmaz.Adres + " " + tasinmaz.Ilcesi + "/" + tasinmaz.Ili +"</br>";

                        BagisIdQS = bagisIdStr;
                        CikarNowBtn.Visible = true;
                        var openPopup = "OpenModalOnay();";
                        System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), openPopup, true);

                    };
                    CikarCell.Controls.Add(CikarBtn);
                    row.Controls.Add(CikarCell);

                    BagisTasinmazLarTable.Controls.Add(row);

                }
            }
        }
        private bool BagisSil(Bagis bagis)
        {
            bool isSaved = false;
            try
            {
                if (bagis != null)
                {
                    isSaved = bagis.Delete();

                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }

            return isSaved;
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
        protected void CikarNowBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Bagis bagis = new Bagis();
                bagis = bagis.Select<Bagis>(BagisIdQS.ConvertToInt());
                if (bagis != null)
                {
                    int bagisciId = bagis.BagisciId;
                    bool isDeleted = BagisSil(bagis);
                    if (isDeleted)
                    {
                        TasinmazBagisci bagisci = new TasinmazBagisci();
                        bagisci = bagisci.Select<TasinmazBagisci>(BagisciIdQS.ConvertToInt());
                        if (bagisci != null)
                        {
                            BagisciTasinmazlarTablosunuDoldur(bagisci);
                        }
                        else
                        {
                            MessageHelper.PublishMessage("Bağışçı bulunamadı", ProjeConstants.MESAJ_HATA);
                        }

                        MessageHelper.PublishMessage("Bağışlardan Çıkarıldı", ProjeConstants.MESAJ_BASARILI, 2000);
                    }
                }
            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper(exception);
                Exception exceptionInfo = new Exception("Taşınmaz Bağışlardan Çıkarılamadı");
                exceptionHelper.Exceptions.Add(exceptionInfo);
                exceptionHelper.PublishException();
            }
        }
        protected void BackBtn_Click(object sender, EventArgs e)
        {

            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));
            newUrl += "/" + ProjeConstants.PAGE_TASINMAZBAGISCI_GIRIS + "?DestinationApp=TBD&BagisciId=" + BagisciIdQS;
            Page.Response.Redirect(newUrl, true);
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        protected void TasinmazEkleBtn_Click(object sender, EventArgs e)
        {
            //popup olarak Tasinmaz Listesini Aç
            TabloModalOlustur();
            UtilityHelper.ScriptCalistir("OpenModal();");
        }
        private void TabloModalOlustur()
        {
            var jsonData = TabloModalJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateModalDataTable(jsonData); //javascript kodu hazirlaniyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string TabloModalJson()
        {
            string jSon = string.Empty;

            try
            {
                List<TasinmazListItem> list = GetModalDataList();
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
        private string CreateModalDataTable(string jsonData)
        {
            string tableString = @"
            if ( $.fn.DataTable.isDataTable('#CustomModalDataTable') ) {
              $('#CustomModalDataTable').DataTable().destroy();
            }
            $('#CustomModalDataTable tbody').empty();

            jQuery('#CustomModalDataTable').DataTable({
            data: " + jsonData + @",
            columns: [
                { data: 'SiraNo', 'width': '10%' },
                { data: 'MulkiyetSekli'},
                { data: 'KullanimSekli' },
                { data: 'Ili' },
                { data: 'Ilcesi' },
                { data: 'Adres' },
                { data: 'Sec' },
            ],
            'order': [[3, 'asc']],
            'language': {
                'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
            },
            responsive: true,
            dom: 'fpirt',
            createdRow: function (row, data, dataIndex) {
                if (data.EnvanterdeMi === 0) {
                    $(row).addClass('text-secondary');
                    $('td', row).addClass('text-secondary');
                }
            },

            });
            ";

            return tableString;
        }
        protected void TasinmazEkleNowBtnBtn_Click(object sender, EventArgs e)
        {
            //paramTasinmazIdLbl daki TasinmazId'sini al
            //Bagislarda tasinmazId var mi bak
            //Seçileni ekle
            int tasinmazId = paramTasinmazIdLbl.Value.ConvertToInt();

            Tasinmaz tasinmaz = new Tasinmaz();
            tasinmaz = tasinmaz.Select(tasinmazId);
            bool kaydedildiMi = false;
            Bagis bagis = null;
            if (tasinmaz != null)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    bagis = new Bagis();
                    bagis = bagis.SelectByTasinmazId(tasinmazId);
                    if (bagis == null)
                    {
                        bagis = new Bagis();
                        bagis.BagisciId = BagisciIdQS.ConvertToInt();
                        bagis.BagisTarihi = tasinmaz.EnvantereGirisTarihi.ConvertToDatetime();
                        bagis.BagisYili = bagis.BagisTarihi.Year;
                        bagis.Olusturan = CurrentUserName;
                        bagis.TasinmazId = paramTasinmazIdLbl.Value.ConvertToInt();
                        bagis.Envanterde = true;
                        int bagisId = bagis.Save();
                        kaydedildiMi = bagisId > 0;
                    }
                    else
                    {
                        bagis.BagisciId = BagisciIdQS.ConvertToInt();
                        bagis.TasinmazId = tasinmazId;
                        bagis.Degistiren = CurrentUserName;
                        kaydedildiMi = bagis.Update();
                    }
                    if (kaydedildiMi)
                        scope.Complete();
                }
                if (kaydedildiMi && bagis != null)
                {
                    MessageHelper.PublishMessage("Bağış Kaydedildi", ProjeConstants.MESAJ_BASARILI, 2000);
                    RedirectToPage(ProjeConstants.PAGE_TASINMAZBAGISCI_BAGISLARI + "?Mesaj=true" + "&DestinationApp=TBD&BagisciId=" + bagis.BagisciId);
                }
            }
        }
        private List<TasinmazListItem> GetModalDataList()
        {
            Tasinmaz tasinmazDao = new Tasinmaz();
            DataTable dataTableModal = tasinmazDao.SelectBagiscisiOlmayanTasinmazlarByBagsciIdReturnDT();

            int SiraNo = 1;
            List<TasinmazListItem> list = new List<TasinmazListItem>();
            if (dataTableModal != null)
            {
                foreach (DataRow row in dataTableModal.Rows)
                {

                    int tasinmazId = row["TasinmazId"].ConvertToInt();
                    string mulkiyetSekli = row["MulkiyetSekli"].ReturnEmptyIfNull().ToString();
                    string kullanimSekli= row["KullanimSekli"].ReturnEmptyIfNull().ToString();
                    string ili = row["Ili"].ReturnEmptyIfNull().ToString();
                    string ilcesi = row["Ilcesi"].ReturnEmptyIfNull().ToString();
                    string adres = row["Adres"].ReturnEmptyIfNull().ToString();
                    int envanterdeMi = row["EnvanterdeMi"].ReturnZeroIfNull().ConvertToInt();
                    TasinmazListItem tasinmazItem = new TasinmazListItem();
                    tasinmazItem.SiraNo = SiraNo++.ToString();
                    tasinmazItem.TasinmazId = tasinmazId.ToString();
                    tasinmazItem.MulkiyetSekli = mulkiyetSekli;
                    tasinmazItem.KullanimSekli = kullanimSekli;
                    tasinmazItem.Ili = ili;
                    tasinmazItem.Ilcesi = ilcesi;
                    tasinmazItem.Adres = adres;
                    tasinmazItem.EnvanterdeMi = envanterdeMi;
                    tasinmazItem.Sec = "<a href='#' class='btn btn-outline-primary' onclick=CallButtonClick("+ BagisciIdQS + ","+ tasinmazId + ");>Ekle</a>";
                    list.Add(tasinmazItem);

                }
            }
            return list;
        }
        private class TasinmazListItem
        {
            public string SiraNo { get; set; }
            public string TasinmazId { get; set; }
            public string MulkiyetSekli { get; set; }
            public string KullanimSekli { get; set; }
            public string Ili { get; set; }
            public string Ilcesi { get; set; }
            public string Adres { get; set; }
            public string Sec { get; set; }
            public int EnvanterdeMi { get; set; }
        }
    }
}
