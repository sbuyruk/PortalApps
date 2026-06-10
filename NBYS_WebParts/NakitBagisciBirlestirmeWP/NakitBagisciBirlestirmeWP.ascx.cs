using DAO.Ortak;
using Model.NBYS;
using Model.Ortak;
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
            try
            {
                if (!Page.IsPostBack)
                {
                    FooterDiv.Visible = false;
                    if (!string.IsNullOrEmpty(ParamQS))
                    {
                        AsilBagisciAraTxt.Text = ParamQS;
                        AsilBagisciTabloOlustur(true);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
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

            try
            {
                List<NakitBagisciListItem> list = AsilBagisciGetDataList(isSelectable);
                var serializer = new JavaScriptSerializer();
                json = serializer.Serialize(list);
            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper();
                exceptionHelper.Exceptions.Add(exception);
                exceptionHelper.PublishException();
            }
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
                NakitBagisci nakitBagisci = new NakitBagisci();
                dataTable = nakitBagisci.SelectByFilterReturnDataTable(AsilBagisciAraTxt.Text, 0);
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

            try
            {
                List<NakitBagisciListItem> list = BirlesecekBagisciGetDataList();
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
                MessageHelper.PublishMessage("Kayıt bulunamadı", ProjeConstants.MESAJ_BILGI,2000);
            }
            return list;
        }
        private DataTable BirlesecekBagisciGetData()
        {
            DataTable dataTable = null;
            if (!string.IsNullOrEmpty(BirlesecekBagisciAraTxt.Text) && BirlesecekBagisciAraTxt.Text.Length > 3)
            {
                NakitBagisci nakitBagisci = new NakitBagisci();
                dataTable = nakitBagisci.SelectByFilterReturnDataTable(BirlesecekBagisciAraTxt.Text, SecilenAsilBagisciId.ConvertToInt());
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
            NakitBagisci secilenNb = new NakitBagisci();
            int id = paramLbl.Value.ConvertToInt();
            secilenNb = secilenNb.Select<NakitBagisci>(id);
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
        }
        protected void BirlesecekBagisciHiddenBtn_Click(object sender, EventArgs e)
        {
            NakitBagisci secilenNb = new NakitBagisci();
            int id = paramLbl.Value.ConvertToInt();
            secilenNb = secilenNb.Select<NakitBagisci>(id);
            if (secilenNb != null)
            {
                //BirlesecekBagisciId = secilenNb.Id.ReturnZeroIfNull().ToString();
                FillSecilenBirlesecekBagisciTable();
                BagisciSecTableDiv.Attributes["style"] = "display:none";
                BirlesecekBagisciAraDiv.Attributes["style"] = "display:none";
                BirlesecekBagisciDiv.Attributes["style"] = "display:block";

            }
            FillSecilenAsilBagisciTable();
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
            NakitBagisci secilenBagisci = new NakitBagisci();
            secilenBagisci = secilenBagisci.Select<NakitBagisci>(SecilenAsilBagisciId.ConvertToInt());
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

                Il il = new Il();
                il = il.Select<Il>(secilenBagisci.Ili.ConvertToInt());
                if (il != null)
                {
                    IlIlceCell.Text = il.IlAdi + " ";
                }
                Ilce ilce = new Ilce();
                ilce = ilce.Select<Ilce>(secilenBagisci.Ilcesi.ConvertToInt());
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
                    NakitBagisci secilenBagisci = new NakitBagisci();
                    secilenBagisci = secilenBagisci.Select<NakitBagisci>(bagisciId);
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
            BagisciyiBirlestirAtomic();

        }
        private void BagisciyiBirlestirAtomic()
        {
            string currentUser = UtilityHelper.GetCurrentUserLoginName();
            string mesaj1 = string.Empty;
            NakitBagisci asilBagisci = new NakitBagisci();
            asilBagisci = asilBagisci.Select<NakitBagisci>(SecilenAsilBagisciId.ConvertToInt());
            if (asilBagisci == null || SecilenAsilBagisciId.ConvertToInt() == 0)
            {
                MessageHelper.PublishMessage("Seçtiğiniz Asıl Bağışçı Bulunamadı.", ProjeConstants.MESAJ_HATA);
                return;
            }

            string value = paramArray.Value;
            string[] idList = value.Split(',');
            foreach (string birlesecekBagisciId in idList)
            {
                int bagisciId = birlesecekBagisciId.ConvertToInt();
                if (bagisciId > 0)
                {
                    NakitBagisci birlesecekBagisci = new NakitBagisci();
                    birlesecekBagisci = birlesecekBagisci.Select<NakitBagisci>(bagisciId);
                    if (birlesecekBagisci != null)
                    {
                        if (birlesecekBagisci == null || birlesecekBagisciId.ConvertToInt() == 0)
                        {
                            MessageHelper.PublishMessage("Birleşecek Bağışçı Bulunamadı. Bağışçı=" + birlesecekBagisci.Adi + " #Id: " + birlesecekBagisciId, ProjeConstants.MESAJ_BILGI);
                            continue;
                        }
                        else
                        {
                            try
                            {
                                DbClass db = new DbClass();
                                #region  birlesecek bagisçiya ait Hareket listesini al
                                //NakitBagisHareketiDuzenle();

                                NakitBagisHareket nbhList = new NakitBagisHareket();
                                List<NakitBagisHareket> listofBagisHareket = nbhList.SelectByBagisciId(birlesecekBagisci.Id);

                                string bagiscisiDegisenNbhs = string.Empty;
                                try
                                {
                                    foreach (NakitBagisHareket item in listofBagisHareket)
                                    {
                                        string bagisciIdOnceki = item.BagisciId.ToString();
                                        item.Aciklama = item.Aciklama + "(" + item.BagisciId + " tarafından yapılan bağış birleştirildi) ";
                                        item.BagisciId = SecilenAsilBagisciId.ConvertToInt();
                                        bagiscisiDegisenNbhs += "," + item.Id;
                                        item.Degistiren = UtilityHelper.GetCurrentUserLoginName();

                                        DBObject nbhDbo = new DBObject();
                                        nbhDbo.SQLString = item.GetUpdateSQL("");
                                        nbhDbo.SQLType = ProjeConstants.SQL_UPDATE;
                                        nbhDbo.IsFilled = true;
                                        db.DBObjectList.Add(nbhDbo);

                                        #region SilinenKayit_Table'a yaz
                                        SilinenKayit skNBH = new SilinenKayit();
                                        skNBH.Silen = currentUser;
                                        skNBH.SilinmeSebebi = "Bağış Birleştirme";
                                        skNBH.TabloAdi = "NakitBagisHareket_Table";
                                        skNBH.SilinmeTarihi = DateTime.Now.ReturnTRDateFormat();
                                        skNBH.SilinenKayitBilgisi = " #BağışçıId=" + bagisciIdOnceki + " numaralı bağışçıya ait " +
                                            item.Id + " numaralı nakit bağış  ( Bağış Tarihi=" + item.BagisTarihi + " #Bağış Miktarı=" + item.BagisMiktari + ") " +
                                            item.BagisciId + " numaralı bağışçıya birleştirilmiştir.";

                                        DBObject skNBHDbo = new DBObject();
                                        skNBHDbo.SQLString = skNBH.GetInsertSQL("");
                                        skNBHDbo.SQLType = ProjeConstants.SQL_INSERT;
                                        skNBHDbo.IsFilled = true;
                                        db.DBObjectList.Add(skNBHDbo);
                                        #endregion
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ExceptionHelper exHelper = new ExceptionHelper(ex);
                                    exHelper.PublishException();
                                }


                                #endregion
                                #region birlesecek bagisçiya ait Armagan listesini al
                                string mesaj = string.Empty;
                                try
                                {
                                    Armagan armaganList = new Armagan();
                                    List<Armagan> listofArmagan = armaganList.SelectByBagisciId(birlesecekBagisci.Id.ConvertToInt());
                                    string bagiscisiDegisenArmagans = string.Empty;
                                    foreach (Armagan item in listofArmagan)
                                    {
                                        string bagisciIdOnceki = item.BagisciId.ToString();

                                        item.Aciklama = item.Aciklama + "(" + item.BagisciId + " tarafindan yapilan bagis birlestirildi) ";
                                        item.BagisciId = SecilenAsilBagisciId.ConvertToInt();
                                        bagiscisiDegisenArmagans += "," + item.Id;
                                        item.Degistiren = UtilityHelper.GetCurrentUserLoginName();
                                        DBObject armaganDbo = new DBObject();
                                        armaganDbo.SQLString = item.GetUpdateSQL("");
                                        armaganDbo.SQLType = ProjeConstants.SQL_UPDATE;
                                        armaganDbo.IsFilled = true;
                                        db.DBObjectList.Add(armaganDbo);

                                        #region SilinenKayit_Table'a yaz
                                        SilinenKayit skNBH = new SilinenKayit();
                                        skNBH.Silen = currentUser;
                                        skNBH.SilinmeSebebi = "Bağış Birleştirme";
                                        skNBH.TabloAdi = "Armagan_Table";
                                        skNBH.SilinmeTarihi = DateTime.Now.ReturnTRDateFormat();
                                        skNBH.SilinenKayitBilgisi = " #BağışçıId=" + bagisciIdOnceki + " numaralı bağışçıya ait " +
                                            item.Id + " numaralı armağan " +
                                            item.BagisciId + " numaralı bağışçıya birleştirilmiştir.";

                                        DBObject skArmaganDbo = new DBObject();
                                        skArmaganDbo.SQLString = skNBH.GetInsertSQL("");
                                        skArmaganDbo.SQLType = ProjeConstants.SQL_INSERT;
                                        skArmaganDbo.IsFilled = true;
                                        db.DBObjectList.Add(skArmaganDbo);
                                        #endregion

                                    }
                                    //mesaj = string.Format(@"Seçilen Asıl Bağışçı={0} ; Birleşecek Bağışçı={1} ; Armağanlar:{2} ",
                                    //    SecilenAsilBagisciId, birlesecekBagisciId, bagiscisiDegisenArmagans);
                                    //silinenBilgileriKaydet(mesaj, "Armagan_Table");
                                }
                                catch (Exception ex)
                                {
                                    ExceptionHelper exhelper = new ExceptionHelper();
                                    Exception exception = new Exception(mesaj, ex);
                                    exhelper.Exceptions.Add(exception);
                                    exhelper.PublishException();
                                }
                                #endregion

                                //transaction lari yap
                                List<DBObject> savedDBOList = db.ExecuteTransaction();
                                if (savedDBOList.Count > 0)
                                {
                                    #region Birlestirilen NakitBagisciyi sil
                                    db = new DbClass();
                                    //NakitBagisciDanSil(birlesecekBagisci);
                                    NakitBagisci silinecekBagisci = new NakitBagisci();
                                    silinecekBagisci = silinecekBagisci.SelectBagisiOlmayanBagisciById(birlesecekBagisci.Id);
                                    if (silinecekBagisci != null)
                                    {

                                        DBObject nbDbo = new DBObject();
                                        nbDbo.SQLString = silinecekBagisci.GetDeleteSQL("");
                                        nbDbo.SQLType = ProjeConstants.SQL_DELETE;
                                        nbDbo.IsFilled = true;
                                        db.DBObjectList.Add(nbDbo);

                                        //silinenKayit_Table'a yaz
                                        SilinenKayit skBagisci = new SilinenKayit();
                                        skBagisci.Silen = currentUser;
                                        skBagisci.SilinmeSebebi = "Bagis Birlestirme";
                                        skBagisci.TabloAdi = "NakitBagisci_Table";
                                        skBagisci.SilinmeTarihi = DateTime.Now.ReturnTRDateFormat();
                                        skBagisci.SilinenKayitBilgisi = " #Bağışçı=" + silinecekBagisci.Adi + " " + silinecekBagisci.Soyadi + " #BağışçıId=" + silinecekBagisci.Id + " numaralı bağışçı silindi.";
                                    }
                                    List<DBObject> bagisciDBOList = db.ExecuteTransaction();
                                    #endregion

                                    MessageHelper.PublishMessage("Birleştirme Tamamlandı", ProjeConstants.MESAJ_BASARILI, 2000);
                                    ParamQS = AsilBagisciAraTxt.Text;
                                    AsilBagisciTabloOlustur(false);
                                    BirlestirSubDiv.Visible = false;
                                    BirlestirBtn.Visible = false;
                                    BirlesecekBagisciDiv.Attributes["style"] = "display:none";
                                    FooterDiv.Attributes["style"] = "display:none";
                                }
                                else
                                {
                                    MessageHelper.PublishMessage("Birlestirme Yapilmadi", ProjeConstants.MESAJ_BILGI);
                                }

                            }
                            catch (Exception ex)
                            {
                                ExceptionHelper exhelper = new ExceptionHelper();
                                Exception exception = new Exception(mesaj1, ex);
                                exhelper.Exceptions.Add(exception);
                                exhelper.PublishException();
                            }
                        }

                    }
                }
            }

        }
        protected void BirlestirRBL_SelectedIndexChanged(object sender, EventArgs e)
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

        }
        protected void BasadonBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_BAGISCI_BIRLESTIRME;
            Page.Response.Redirect(newUrl, true);

        }
        protected void AsilBagisciAraBtn_Click(object sender, EventArgs e)
        {
            AsilBagisciTabloOlustur(true);
        }
        protected void BirlesecekBagisciAraBtn_Click(object sender, EventArgs e)
        {

            BirlesecekBagisciTabloOlustur();
            FillSecilenAsilBagisciTable();

        }
        protected void TamamBtn_Click(object sender, EventArgs e)
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
        }
        protected void AsilBagisciAraTxt_TextChanged(object sender, EventArgs e)
        {
            AsilBagisciTabloOlustur(true);
        }
        protected void BirlesecekBagisciAraTxt_TextChanged(object sender, EventArgs e)
        {
            BirlesecekBagisciTabloOlustur();
            FillSecilenAsilBagisciTable();
        }
        protected void BagisciDuzenleHiddenBtn_Click(object sender, EventArgs e)
        {
            string pageUrl = ProjeConstants.PAGE_NAKITBAGISCI_EDIT + "?NakitBagisciId=" + paramLbl.Value + "&SenderApp=BB";
            RedirectToPage(pageUrl);
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {

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
            NakitBagisci nb = new NakitBagisci();
            nb = nb.Select<NakitBagisci>(nakitBagisciId);
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
            NakitBagisHareket nbh = new NakitBagisHareket();
            int rowCount = 0;
            var json = nbh.SelectByBagisciIdReturnJSon(nakitBagisciId, ref rowCount);
            decimal toplamTutar = nbh.GetSumBagisMiktariByNakitBagisciIdBetweenBasTarBitTar(
                ProjeConstants.BAGIS_SORGU_BASTAR.ConvertToDatetime(), DateTime.Today, nakitBagisciId.ConvertToInt());
            BagisBilgileriLbl.Text = rowCount < 1 ? "Bagis bulunmamaktadir" :
                "Bagisçinin " + rowCount + " defada yaptigi toplam " + toplamTutar.ToString("N", culturInfo) + "TL bagisi bulunmaktadir";
            return json;
        }
        protected void ModalDoldurBtn_Click(object sender, EventArgs e)
        {
            try
            {

                TabloModalOlustur(paramNakitBagisciIdLbl.Value);
                NakitBagisciFormunuDoldur(paramNakitBagisciIdLbl.Value);
            }
            catch (Exception exception)
            {
                ExceptionHelper ex = new ExceptionHelper(exception);
                ex.PublishException();

            }
        }
        private void NakitBagisciFormunuDoldur(string nakitBagisciIdStr)
        {
            if (!string.IsNullOrEmpty(nakitBagisciIdStr))
            {
                int nakitBagisciId = nakitBagisciIdStr.ConvertToInt();

                NakitBagisci nakitBagisci = new NakitBagisci();
                nakitBagisci = nakitBagisci.Select<NakitBagisci>(nakitBagisciId);
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
                    Il il = new Il();
                    il = il.Select<Il>(ilId);
                    if (il != null)
                    {

                        IlIlceCell.Text = il.IlAdi.ReturnEmptyIfNull().ToString();
                    }
                    int ilceId = nakitBagisci.Ilcesi.ConvertToInt();
                    Ilce ilce = new Ilce();
                    ilce = ilce.Select<Ilce>(ilceId);
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
