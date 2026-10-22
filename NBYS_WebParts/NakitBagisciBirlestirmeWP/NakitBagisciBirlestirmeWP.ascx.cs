using NBYS_WebParts.Logging;
using Utility.Logging;
using Model.NBYS;
using Model.Ortak;
using Model.Services.Ortak;
using Model.Services.NBYS;
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

namespace NBYS_WebParts.NakitBagisciBirlestirmeWP
{
    [ToolboxItemAttribute(false)]
    public partial class NakitBagisciBirlestirmeWP : WebPart
    {
        private PortalLogContext LoggingContext
        {
            get { return PortalLogContext.FromCurrentRequest("NBYS", "NakitBagisciBirlestirmeWP"); }
        }

        private void ExecuteAction(string operation, Action action)
        {
            NbysWebPartLog.ExecuteAction(LoggingContext, operation, action);
        }
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public NakitBagisciBirlestirmeWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string SecilenIdQS
        {
            get
            {

                if (ViewState["SecilenId"] == null)
                {
                    if (Page.Request.QueryString["SecilenId"] != null)
                    {
                        ViewState["SecilenId"] = Page.Request.QueryString["SecilenId"];
                    }
                    else
                    {
                        ViewState["SecilenId"] = string.Empty;
                    }
                }
                return ViewState["SecilenId"].ToString();
            }

            set
            {
                ViewState["SecilenId"] = value;
            }
        }
        private string ParamQS//nakit bagisci düzenlemeden dönüyorsa aranan texti tekrar arasin
        {
            get
            {

                if (ViewState["Param"] == null)
                {
                    if (Page.Request.QueryString["Param"] != null)
                    {
                        ViewState["Param"] = Page.Request.QueryString["Param"];
                    }
                    else
                    {
                        ViewState["Param"] = string.Empty;
                    }
                }
                return ViewState["Param"].ToString();
            }

            set
            {
                ViewState["Param"] = value;
            }
        }
        private string SecilenAsilBagisciId
        {
            get
            {

                if (ViewState["SecilenAsilBagisci"] == null)
                {
                    if (Page.Request.QueryString["SecilenAsilBagisci"] != null)
                    {
                        ViewState["SecilenAsilBagisci"] = Page.Request.QueryString["SecilenAsilBagisci"];
                    }
                    else
                    {
                        ViewState["SecilenAsilBagisci"] = string.Empty;
                    }
                }
                return ViewState["SecilenAsilBagisci"].ToString();
            }

            set
            {
                ViewState["SecilenAsilBagisci"] = value;
            }
        }
        /***
         * Önce Master yani üzerinde birlesme olacak kisi(bagisçi seçilir)
         *  Master (asil) bagisçi sabitlenir
         * Listeden birlesecek kisi seçilir
         * TAMAM'a basildiginda Birlestirmek istiyorum radio butonu görünür
         * Birlestirmek istiyorum radio butonu seçildiginde Birlestir Butonu görünür.
         * Birlestir butonuna basildiginda
         *      *
         *      ***/
        protected void Page_Load(object sender, EventArgs e)
        {
            ExecuteAction("Page_Load", () =>
            {
                if (!Page.IsPostBack) NbysWebPartLog.RecordOpened(LoggingContext);
                if (!Page.IsPostBack)
                {
                    FooterDiv.Visible = false;
                    if (!string.IsNullOrEmpty(ParamQS))
                    {
                        AsilBagisciAraTxt.Text = ParamQS;
                        AsilBagisciTabloOlustur(true);
                    }
                }
            });
        }

        #region AsilBagisci CustomDataTable
        private void AsilBagisciTabloOlustur(bool isSelectable)
        {
            var jsonData = AsilBagisciTabloJson(isSelectable); //veri çekilip json a çeviriliyor
            var jsString = AsilBagisciCreateDataTable(jsonData); //javascript kodu hazirlaniyor.
            UtilityHelper.ScriptCalistir(jsString);
            BagisciSecTableDiv.Attributes["style"] = "display:block";
        }
        private string AsilBagisciTabloJson(bool isSelectable)
        {
            string json = string.Empty;

            List<NakitBagisciListItem> list = AsilBagisciGetDataList(isSelectable);
            var serializer = new JavaScriptSerializer();
            json = serializer.Serialize(list);
            return string.IsNullOrEmpty(json) ? "[{}]" : json;
        }
        private List<NakitBagisciListItem> AsilBagisciGetDataList(bool isSelectable)
        {

            DataTable dataTable = AsilBagisciGetData();
            List<NakitBagisciListItem> list = new List<NakitBagisciListItem>();
            if (dataTable!=null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    int nakitBagisciId = row["NakitBagisciId"].ReturnZeroIfNull().ConvertToInt();
                    string adi = "<a href=# onclick=OpenModal(" + nakitBagisciId + "); class='text-link'>" + row["Adi"].ToString() + "</a>";
                    string TcKimlik = row["TcKimlikNo"].ToString();
                    string ili = row["Ili"].ToString();
                    string ilcesi = row["Ilcesi"].ToString();
                    string telefon = row["Telefon1"].ToString();
                    string adres = row["Adres"].ToString();

                    string duzenleUrl = ProjeConstants.PAGE_NAKITBAGISCI_EDIT + "?SenderApp=BB&NakitBagisciId=" + nakitBagisciId + "&Param=" + AsilBagisciAraTxt.Text;

                    string secUrl = "<a class='btn btn-outline-info' onclick=FillAsilBagisciTable(" + nakitBagisciId + ");>Seç</>";

                    NakitBagisciListItem nakitBagisciListItem = new NakitBagisciListItem();
                    nakitBagisciListItem.NakitBagisciId = nakitBagisciId;
                    nakitBagisciListItem.Adi = adi.Trim();
                    nakitBagisciListItem.TCKimlikNo = TcKimlik;
                    nakitBagisciListItem.Ili = ili;
                    nakitBagisciListItem.Ilcesi = ilcesi;
                    nakitBagisciListItem.Telefon = telefon;
                    nakitBagisciListItem.Adres = adres;

                    nakitBagisciListItem.Duzenle = "<a href=" + duzenleUrl + @"?DestinationApp=TBD&BagisciId=" + nakitBagisciId + "  class='btn btn-outline-primary'>Düzenle</a>";
                    if (isSelectable)
                        nakitBagisciListItem.Sec = adi.IndexOf("BILINMEYEN") >= 0 ? string.Empty : secUrl;

                    nakitBagisciListItem.Secildi = SecilenIdQS.Equals(nakitBagisciListItem.NakitBagisciId);
                    list.Add(nakitBagisciListItem);
                }
            }
            return list;
        }
        private DataTable AsilBagisciGetData()
        {
            DataTable dataTable = null;
            if (!string.IsNullOrEmpty(AsilBagisciAraTxt.Text) && AsilBagisciAraTxt.Text.Length > 3)
            {
                NakitBagisciService service = new NakitBagisciService(LoggingContext);
                dataTable = service.Search(AsilBagisciAraTxt.Text, 0);
            }
            return dataTable;
        }
        private string AsilBagisciCreateDataTable(string jsonData)
        {
            string tableString = @"
            jQuery(document).ready(function() {

                jQuery('#CustomDataTable').DataTable({
                    'initComplete': function(settings, json) {//tablo yüklendiginde
                        var api = this.api();
                        var row = api.row(function(idx, data, node) { //secilen Id'ye gider
                            return data['Secildi'] == true;
                        });
                        if (row.length > 0)
                        {
                            row.select()
                                .show()
                                .draw(false);
                        }
                    },
            data: " + jsonData + @",
            columns:
                    [
                { data: 'Adi' },
                { data: 'TCKimlikNo' },
                { data: 'Ili' },
                { data: 'Ilcesi' },
                { data: 'Telefon', 'width': '14%' },
                { data: 'Adres' },
                { data: 'Duzenle' },
                { data: 'Sec' },

            ],
            'order': [[0, 'asc']],//AdiSoyadi Sirali
            columnDefs:
                [
                ],
            'language': {
                'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                'decimal': ',',
                'thousands': '.'
            },
            responsive: true,
            destroy: true,
            autoWidth: false,
            dom: 'frtip',

                });
            });";
            return tableString;
        }
        #endregion
        #region BirlesecekBagisci CustomDataTable
        private void BirlesecekBagisciTabloOlustur()
        {
            var jsonData = BirlesecekBagisciTabloJson(); //veri çekilip json a çeviriliyor
            var jsString = BirlesecekBagisciCreateDataTable(jsonData); //javascript kodu hazirlaniyor.
            UtilityHelper.ScriptCalistir(jsString);
            BagisciSecTableDiv.Attributes["style"] = "display:block";
        }
        private string BirlesecekBagisciTabloJson()
        {
            string jSon = string.Empty;

            List<NakitBagisciListItem> list = BirlesecekBagisciGetDataList();
            var serializer = new JavaScriptSerializer();
            jSon = serializer.Serialize(list);
            return jSon;
        }
        private List<NakitBagisciListItem> BirlesecekBagisciGetDataList()
        {
            DataTable dataTable = BirlesecekBagisciGetData();
            List<NakitBagisciListItem> list = new List<NakitBagisciListItem>();
            if (dataTable!=null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    int nakitBagisciId = row["NakitBagisciId"].ReturnZeroIfNull().ConvertToInt();
                    string adi = "<a href=# onclick=OpenModal(" + nakitBagisciId + "); class='text-link'>" + row["Adi"].ToString() + "</a>";
                    string TcKimlik = row["TcKimlikNo"].ToString();
                    string ili = row["Ili"].ToString();
                    string ilcesi = row["Ilcesi"].ToString();
                    string telefon = row["Telefon1"].ToString();
                    string adres = row["Adres"].ToString();

                    string secUrl = "<input id=chk onchange=addRemoveBagisciToList(" + nakitBagisciId + ",this); type=checkbox />";

                    NakitBagisciListItem nakitBagisciListItem = new NakitBagisciListItem();
                    nakitBagisciListItem.NakitBagisciId = nakitBagisciId;
                    nakitBagisciListItem.Adi = adi.Trim();
                    nakitBagisciListItem.TCKimlikNo = TcKimlik;
                    nakitBagisciListItem.Ili = ili;
                    nakitBagisciListItem.Ilcesi = ilcesi;
                    nakitBagisciListItem.Telefon = telefon;
                    nakitBagisciListItem.Adres = adres;

                    nakitBagisciListItem.Sec = adi.IndexOf("BILINMEYEN") >= 0 ? string.Empty : secUrl;

                    nakitBagisciListItem.Secildi = SecilenIdQS.Equals(nakitBagisciListItem.NakitBagisciId);
                    list.Add(nakitBagisciListItem);
                }
            }
            else
            {
                Messages.Info("Aramanıza uygun bağışçı bulunamadı.");
            }
            return list;
        }
        private DataTable BirlesecekBagisciGetData()
        {
            DataTable dataTable = null;
            if (!string.IsNullOrEmpty(BirlesecekBagisciAraTxt.Text) && BirlesecekBagisciAraTxt.Text.Length > 3)
            {
                NakitBagisciService service = new NakitBagisciService(LoggingContext);
                dataTable = service.Search(BirlesecekBagisciAraTxt.Text, SecilenAsilBagisciId.ConvertToInt());
            }
            return dataTable;
        }
        private string BirlesecekBagisciCreateDataTable(string jsonData)
        {
            string tableString = @"
            jQuery(document).ready(function() {

                jQuery('#CustomDataTable').DataTable({
                    'initComplete': function(settings, json) {//tablo yüklendiginde
                        var api = this.api();
                        var row = api.row(function(idx, data, node) { //secilen Id'ye gider
                            return data['Secildi'] == true;
                        });
                        if (row.length > 0)
                        {
                            row.select()
                                .show()
                                .draw(false);
                        }
                    },
            data: " + jsonData + @",
            columns:
                    [
                { data: 'Adi' },
                { data: 'TCKimlikNo' },
                { data: 'Ili' },
                { data: 'Ilcesi' },
                { data: 'Telefon', 'width': '14%' },
                { data: 'Adres' },
                { data: 'Duzenle' },
                { data: 'Sec' },

            ],
            'order': [[0, 'asc']],//AdiSoyadi Sirali
            columnDefs:
                [
                ],
            'language': {
                'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                'decimal': ',',
                'thousands': '.'
            },
            responsive: true,
            destroy: true,
            autoWidth: false,
            dom: 'frtip',

                });
            });";
            return tableString;
        }
        #endregion

        protected void AsilBagisciHiddenBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("AsilBagisciHiddenBtn_Click", () =>
            {
                int id = paramLbl.Value.ConvertToInt();
                NakitBagisci secilenNb = new NakitBagisciService(LoggingContext).GetById(id);
                if (secilenNb != null)
                {
                    SecilenAsilBagisciId = secilenNb.Id.ReturnZeroIfNull().ToString();

                    FillSecilenAsilBagisciTable();
                    BirlesecekBagisciAraTxt.Text = AsilBagisciAraTxt.Text;
                    BagisciSecTableDiv.Attributes["style"] = "display:none";
                    AsilBagisciAraDiv.Attributes["style"] = "display:none";
                    AsilBagisciDiv.Attributes["style"] = "display:block";
                    BirlesecekBagisciAraDiv.Attributes["style"] = "display:block";
                    BirlesecekBagisciDiv.Attributes["style"] = "display:none";
                }
            });
        }
        protected void BirlesecekBagisciHiddenBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("BirlesecekBagisciHiddenBtn_Click", () =>
            {
                int id = paramLbl.Value.ConvertToInt();
                NakitBagisci secilenNb = new NakitBagisciService(LoggingContext).GetById(id);
                if (secilenNb != null)
                {
                    //BirlesecekBagisciId = secilenNb.Id.ReturnZeroIfNull().ToString();
                    FillSecilenBirlesecekBagisciTable();
                    BagisciSecTableDiv.Attributes["style"] = "display:none";
                    BirlesecekBagisciAraDiv.Attributes["style"] = "display:none";
                    BirlesecekBagisciDiv.Attributes["style"] = "display:block";

                }
                FillSecilenAsilBagisciTable();
            });
        }
        private void SecilenAsilBagisciTableHeaders()
        {
            AsilBagisciTable.Rows.Clear();
            TableHeaderRow th = new TableHeaderRow();

            TableHeaderCell AdiCell = new TableHeaderCell();
            AdiCell.Text = "Adi";
            TableHeaderCell TCKimlikNoCell = new TableHeaderCell();
            TCKimlikNoCell.Text = "TC Kimlik No";
            TableHeaderCell TelefonCell = new TableHeaderCell();
            TelefonCell.Text = "Telefon";
            TableHeaderCell AdresCell = new TableHeaderCell();
            AdresCell.Text = "Adres";
            TableHeaderCell IlIlceCell = new TableHeaderCell();
            IlIlceCell.Text = "Il-Ilçe";

            th.Controls.Add(AdiCell);
            th.Controls.Add(TCKimlikNoCell);
            th.Controls.Add(TelefonCell);
            th.Controls.Add(AdresCell);
            th.Controls.Add(IlIlceCell);

            AsilBagisciTable.Controls.Add(th);
        }
        private void FillSecilenAsilBagisciTable()
        {
            NakitBagisci secilenBagisci = new NakitBagisciService(LoggingContext).GetById(SecilenAsilBagisciId.ConvertToInt());
            if (secilenBagisci != null)
            {

                SecilenAsilBagisciTableHeaders();


                TableRow row = new TableRow();
                TableCell AdiCell = new TableCell();
                TableCell TCKimlikNoCell = new TableCell();
                TableCell TelefonCell = new TableCell();
                TableCell IlIlceCell = new TableCell();
                TableCell AdresCell = new TableCell();

                AdiCell.Text = secilenBagisci.Adi;
                TCKimlikNoCell.Text = secilenBagisci.TCKimlikNo.ReturnEmptyIfNull().ToString();
                TelefonCell.Text = secilenBagisci.Telefon1 + " " + secilenBagisci.Telefon2;

                Il il = new IlService().GetById(secilenBagisci.Ili.ConvertToInt(), LoggingContext);
                if (il != null)
                {
                    IlIlceCell.Text = il.IlAdi + " ";
                }
                Ilce ilce = new IlceService().GetById(secilenBagisci.Ilcesi.ConvertToInt(), LoggingContext);
                if (ilce != null)
                {
                    IlIlceCell.Text += ilce.IlceAdi;
                }

                AdresCell.Text = secilenBagisci.Adres;

                row.Controls.Add(AdiCell);
                row.Controls.Add(TCKimlikNoCell);
                row.Controls.Add(TelefonCell);
                row.Controls.Add(AdresCell);
                row.Controls.Add(IlIlceCell);

                AsilBagisciTable.Controls.Add(row);

            }
        }
        private void SecilenBirlesecekBagisciTableHeaders()
        {
            BirlesecekBagisciTable.Rows.Clear();
            TableHeaderRow th = new TableHeaderRow();

            TableHeaderCell AdiCell = new TableHeaderCell();
            AdiCell.Text = "Adı";
            TableHeaderCell TCKimlikNoCell = new TableHeaderCell();
            TCKimlikNoCell.Text = "TC Kimlik No";
            TableHeaderCell TelefonCell = new TableHeaderCell();
            TelefonCell.Text = "Telefon";
            TableHeaderCell AdresCell = new TableHeaderCell();
            AdresCell.Text = "Adres";
            TableHeaderCell IlIlceCell = new TableHeaderCell();
            IlIlceCell.Text = "İl-İlçe";

            th.Controls.Add(AdiCell);
            th.Controls.Add(TCKimlikNoCell);
            th.Controls.Add(TelefonCell);
            th.Controls.Add(AdresCell);
            th.Controls.Add(IlIlceCell);

            BirlesecekBagisciTable.Controls.Add(th);
        }
        private void FillSecilenBirlesecekBagisciTable()
        {
            SecilenBirlesecekBagisciTableHeaders();
            string value = paramArray.Value;
            string[] idList = value.Split(',');
            foreach (string item in idList)
            {
                int bagisciId = item.ConvertToInt();
                if (bagisciId > 0)
                {
                    NakitBagisci secilenBagisci = new NakitBagisciService(LoggingContext).GetById(bagisciId);
                    if (secilenBagisci != null)
                    {

                        TableRow row = new TableRow();
                        TableCell AdiCell = new TableCell();
                        TableCell TCKimlikNoCell = new TableCell();
                        TableCell TelefonCell = new TableCell();
                        TableCell IlIlceCell = new TableCell();
                        TableCell AdresCell = new TableCell();

                        AdiCell.Text = secilenBagisci.Adi;
                        TCKimlikNoCell.Text = secilenBagisci.TCKimlikNo.ReturnEmptyIfNull().ToString();
                        TelefonCell.Text = secilenBagisci.Telefon1 + " " + secilenBagisci.Telefon2;
                        IlIlceCell.Text = secilenBagisci.Ili + " " + secilenBagisci.Ilcesi;
                        AdresCell.Text = secilenBagisci.Adres;

                        row.Controls.Add(AdiCell);
                        row.Controls.Add(TCKimlikNoCell);
                        row.Controls.Add(TelefonCell);
                        row.Controls.Add(AdresCell);
                        row.Controls.Add(IlIlceCell);

                        BirlesecekBagisciTable.Controls.Add(row);
                    }
                }
            }


        }
        protected void BirlestirBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("BirlestirBtn_Click", () =>
            {
                BagisciyiBirlestirAtomic();
            });
        }
        private void BagisciyiBirlestirAtomic()
        {
            int targetId;
            if (!int.TryParse(SecilenAsilBagisciId, out targetId) || targetId <= 0)
            {
                Messages.Warning("Asıl bağışçıyı seçin.");
                return;
            }
            List<int> sourceIds = new List<int>();
            foreach (string raw in (paramArray.Value ?? string.Empty).Split(','))
            {
                if (string.IsNullOrWhiteSpace(raw)) continue;
                int id;
                if (!int.TryParse(raw.Trim(), out id) || id <= 0)
                {
                    Messages.Warning("Birleştirilecek bağışçı seçimi geçersiz. Listeyi yenileyip tekrar seçin.");
                    return;
                }
                sourceIds.Add(id);
            }
            NbysDonationWriteResult result = new NbysDonationWriteService(LoggingContext).Merge(targetId, sourceIds);
            NbysDonationFeedback.Show(LoggingContext, result, "Seçilen bağışçılar birleştirildi.", () =>
            {
                ParamQS = AsilBagisciAraTxt.Text;
                AsilBagisciTabloOlustur(false);
                BirlestirSubDiv.Visible = false;
                BirlestirBtn.Visible = false;
                BirlesecekBagisciDiv.Attributes["style"] = "display:none";
                FooterDiv.Attributes["style"] = "display:none";
                paramArray.Value = string.Empty;
            });
        }
        protected void BirlestirRBL_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecuteAction("BirlestirRBL_SelectedIndexChanged", () =>
            {
                FillSecilenAsilBagisciTable();
                FillSecilenBirlesecekBagisciTable();
                if (BirlestirRBL.SelectedIndex == 0)
                {
                    BirlestirSubDiv.Visible = false;
                    BirlestirBtn.Visible = false;

                }
                else
                {
                    BirlestirSubDiv.Visible = true;
                    BirlestirBtn.Visible = true;
                }
            });
        }
        protected void BasadonBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("BasadonBtn_Click", () =>
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_BAGISCI_BIRLESTIRME;
                Page.Response.Redirect(newUrl, true);
            });
        }
        protected void AsilBagisciAraBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("AsilBagisciAraBtn_Click", () =>
            {
                AsilBagisciTabloOlustur(true);
            });
        }
        protected void BirlesecekBagisciAraBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("BirlesecekBagisciAraBtn_Click", () =>
            {
                BirlesecekBagisciTabloOlustur();
                FillSecilenAsilBagisciTable();
            });
        }
        protected void TamamBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("TamamBtn_Click", () =>
            {
                BirlesecekBagisciTable.CssClass = "table table-danger m-0";
                //PUTableDiv.Visible = false;
                FooterDiv.Visible = true;
                AsilBagisciBagislariDiv.Visible = true;
                FillSecilenAsilBagisciTable();
                FillSecilenBirlesecekBagisciTable();
                BirlesecekBagisciAraDiv.Attributes["style"] = "display:none";
                BagisciSecTableDiv.Attributes["style"] = "display:none";
                BirlesecekBagisciDiv.Attributes["style"] = "display:block";
                TamamDiv.Attributes["style"] = "display:none";
            });
        }
        protected void AsilBagisciAraTxt_TextChanged(object sender, EventArgs e)
        {
            ExecuteAction("AsilBagisciAraTxt_TextChanged", () =>
            {
                AsilBagisciTabloOlustur(true);
            });
        }
        protected void BirlesecekBagisciAraTxt_TextChanged(object sender, EventArgs e)
        {
            ExecuteAction("BirlesecekBagisciAraTxt_TextChanged", () =>
            {
                BirlesecekBagisciTabloOlustur();
                FillSecilenAsilBagisciTable();
            });
        }
        protected void BagisciDuzenleHiddenBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("BagisciDuzenleHiddenBtn_Click", () =>
            {
                string pageUrl = ProjeConstants.PAGE_NAKITBAGISCI_EDIT + "?NakitBagisciId=" + paramLbl.Value + "&SenderApp=BB";
                RedirectToPage(pageUrl);
            });
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("CloseBtn_Click", () =>
            {

            });
        }
        #region Modal popup islemleri


        private void TabloModalOlustur(string nakitBagisciId)
        {
            var jsonData = GetModalDataJson(nakitBagisciId); //veri çekilip json a çeviriliyor
            var jsString = CreateModalDataTable(jsonData, nakitBagisciId.ConvertToInt()); //javascript kodu hazirlaniyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string CreateModalDataTable(string jsonData, int nakitBagisciId)
        {
            NakitBagisci nb = new NakitBagisciService(LoggingContext).GetById(nakitBagisciId);
            string bagisciAdi = nb != null ? (nb.Adi + nb.Soyadi).ReplaceTrChars() : "Bagisci";
            string filename = bagisciAdi + "-" + DateTime.Today.Day + "-" + DateTime.Today.Month + "-" + DateTime.Today.Year;
            string tableString = @"
        jQuery(document).ready(function () {
            if ( jQuery.fn.DataTable.isDataTable('#CustomModalDataTable') ) {
                jQuery('#CustomModalDataTable').DataTable().destroy();
            }
            jQuery('#CustomModalDataTable tbody').empty();

            jQuery.fn.dataTable.moment('DD.MM.YYYY');//sort date
            jQuery('#CustomModalDataTable').DataTable({
                data: " + jsonData + @",
                columns: [
                    { data: 'BagisTarihi' },
                    { data: 'BagisMiktari', 'width': '10%', 'className': 'text-end' },
                    { data: 'Banka' },
                    { data: 'Armagan' },
                    { data: 'ArmaganTutari' },
                    { data: 'Durum' },
                ],
                'order': [[0, 'desc']],

                'language': {
                    'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                    'decimal': ',',
                    'thousands': '.'
                },
                columnDefs:[
                    {targets:0, render:function(data){
                        return moment(data).format('DD.MM.YYYY');
                    }},
                ],
                responsive: true,
                dom: 'Bfrtip',
                buttons:
                [
                    {
                extend: 'print',
                        exportOptions:
                    {
                    columns: ':visible'
                        }
                },
                    {
                      extend: 'excel',
                      title:'" + filename + @"',
                      exportOptions: {
                          columns: ':visible',
                          format: {
                              body: function(data, row, column, node) {
                                  data = $('<p>' + data + '</p>').text();

                                  return $.isNumeric(data.replace(',', '.')) ? data.replace(',', '.') : data;
                              }
                          }
                      },
                },
                    {
                extend: 'pdf',
                        exportOptions:
                    {
                    columns: ':visible'
                        }
                },
                    {
                extend: 'copy',
                        exportOptions:
                    {
                    columns: ':visible'
                        }
                },
                    , 'pageLength', 'colvis'
                ]

            });
        });
        ";
            return tableString;
        }
        private string GetModalDataJson(string nakitBagisciId)
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            int rowCount = 0;
            var json = new NakitBagisHareketService(LoggingContext).GetDonorDetailJson(nakitBagisciId.ConvertToInt(), ref rowCount);
            decimal toplamTutar = new NakitBagisHareketService(LoggingContext).GetTotalByBagisciIdDateRange(
                ProjeConstants.BAGIS_SORGU_BASTAR.ConvertToDatetime(), DateTime.Today, nakitBagisciId.ConvertToInt());
            BagisBilgileriLbl.Text = rowCount < 1 ? "Bağış bulunmamaktadır" :
                "Bağışçının " + rowCount + " defada yaptığı toplam " + toplamTutar.ToString("N", culturInfo) + "TL bağışı bulunmaktadır";
            return json;
        }
        protected void ModalDoldurBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("ModalDoldurBtn_Click", () =>
            {
                TabloModalOlustur(paramNakitBagisciIdLbl.Value);
                NakitBagisciFormunuDoldur(paramNakitBagisciIdLbl.Value);
            });
        }
        private void NakitBagisciFormunuDoldur(string nakitBagisciIdStr)
        {
            if (!string.IsNullOrEmpty(nakitBagisciIdStr))
            {
                int nakitBagisciId = nakitBagisciIdStr.ConvertToInt();

                NakitBagisci nakitBagisci = new NakitBagisciService(LoggingContext).GetById(nakitBagisciId);
                if (nakitBagisci != null)
                {
                    //NakitBagisciIdLbl.Text = nakitBagisciId.ToString();
                    TableRow row = new TableRow();
                    TableCell AdiCell = new TableCell();
                    TableCell TCKimlikNoCell = new TableCell();
                    TableCell AdresCell = new TableCell();
                    TableCell IlIlceCell = new TableCell();
                    TableCell TelefonCell = new TableCell();
                    TableCell TuzelKisiCell = new TableCell();

                    AdiCell.Text = nakitBagisci.Adi.ReturnEmptyIfNull().ToString();
                    TCKimlikNoCell.Text = nakitBagisci.TCKimlikNo.ReturnEmptyIfNull().ToString();
                    AdresCell.Text = nakitBagisci.Adres.ReturnEmptyIfNull().ToString();

                    int ilId = nakitBagisci.Ili.ConvertToInt();
                    Il il = new IlService().GetById(ilId, LoggingContext);
                    if (il != null)
                    {

                        IlIlceCell.Text = il.IlAdi.ReturnEmptyIfNull().ToString();
                    }
                    int ilceId = nakitBagisci.Ilcesi.ConvertToInt();
                    Ilce ilce = new IlceService().GetById(ilceId, LoggingContext);
                    if (ilce != null)
                    {

                        IlIlceCell.Text += " " + ilce.IlceAdi.ReturnEmptyIfNull().ToString();
                    }
                    TelefonCell.Text = nakitBagisci.Telefon1.ReturnEmptyIfNull().ToString();
                    TuzelKisiCell.Text = nakitBagisci.TuzelKisi.ConvertToBool() ? "Evet" : "Hayir";
                    row.Controls.Add(AdiCell);
                    row.Controls.Add(TCKimlikNoCell);
                    row.Controls.Add(AdresCell);
                    row.Controls.Add(IlIlceCell);
                    row.Controls.Add(TelefonCell);
                    row.Controls.Add(TuzelKisiCell);
                    BagisciTable.Controls.Add(row);

                }
            }
        }
        #endregion
        private void RedirectToPage(string pageUrl)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + pageUrl;
            Page.Response.Redirect(newUrl);
        }
        private class NakitBagisciListItem
        {
            public int NakitBagisciId { get; set; }
            public string Adi { get; set; }
            public string TCKimlikNo { get; set; }
            public string Ili { get; set; }
            public string Ilcesi { get; set; }
            public string Telefon { get; set; }
            public string Adres { get; set; }
            public string Duzenle { get; set; }
            public string Sec { get; set; }
            public bool Secildi { get; set; }
        }
    }
}
