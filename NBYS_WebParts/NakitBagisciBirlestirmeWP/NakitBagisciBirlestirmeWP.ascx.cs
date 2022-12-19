using DAO.Ortak;
using Model.NBYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private string jsString
        {
            get
            {

                if (ViewState["jsString"] == null)
                {
                    if (Page.Request.QueryString["jsString"] != null)
                    {
                        ViewState["jsString"] = Page.Request.QueryString["jsString"];
                    }
                    else
                    {
                        ViewState["jsString"] = string.Empty;
                    }
                }
                return ViewState["jsString"].ToString();
            }

            set
            {
                ViewState["jsString"] = value;
            }
        }
        private string ParamQS//nakit bagisci düzenlemeden dönüyorsa aranan texti tekrar arasın
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
         * Önce Master yani üzerinde birleşme olacak kişi(bağışçı seçilir)
         *  Master (asil) bağışçı sabitlenir
         * Listeden birleşecek kişi seçilir
         * TAMAM'a basıldığında Birleştirmek istiyorum radio butonu görünür
         * Birleştirmek istiyorum radio butonu seçildiğinde Birleştir Butonu görünür.
         * Birleştir butonuna basıldığında
         *      * 
         *      ***/
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //ParamQS=BirlesecekBagisciAraTxt.Text = "Sinan";
                if (!Page.IsPostBack)
                {
                    FooterDiv.Visible = false;
                    if (!string.IsNullOrEmpty(ParamQS))
                    {
                        AsilBagisciAraTxt.Text = ParamQS;
                        AsilBagisciKayitGetir();

                        //BirlesecekBagisciKayitGetir();
                        //FillSecilenAsilBagisciTable();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private string GetBagisciData()
        {
            string json = string.Empty;
            if (!string.IsNullOrEmpty(AsilBagisciAraTxt.Text) && AsilBagisciAraTxt.Text.Length > 3)
            {
                NakitBagisci nakitBagisci = new NakitBagisci();
                json = nakitBagisci.SelectByFilter(AsilBagisciAraTxt.Text, 0);
            }
            return json;
        }
        private string GetBirlesecekBagisciData()
        {
            string json = string.Empty;
            if (!string.IsNullOrEmpty(BirlesecekBagisciAraTxt.Text) && BirlesecekBagisciAraTxt.Text.Length > 3)
            {
                NakitBagisci nakitBagisci = new NakitBagisci();
                json = nakitBagisci.SelectByFilter(BirlesecekBagisciAraTxt.Text, SecilenAsilBagisciId.ConvertToInt());
            }
            return json;
        }
        private string CreateJsString(string jsonData)
        {
            string linkStr = string.Format("return $('<a href=' + 'NakitBagisciEdit.aspx?NakitBagisciId=' + rowData.NakitBagisciId + '&SenderApp=BB class=btn-outline-primary >Düzenle</a>')");
            // return $('<a href='+'BagisciAyrinti.aspx?NakitBagisciId='+rowData.NakitBagisciId+'" + queryStr + @" class=btn btn-link >'+rowData.Adi+'</a>')
            string ekstretablestr = @"   
                                        $('#tblfilter').puidatatable({
                                        caption: '',
                                        editMode: 'cell',
                                        paginator: {
                                                    rows: 8
                                                    },
                                        columns: [
                                            { field: 'Adi', headerText: 'Adi', sortable:true,filter: true,headerClass:'genisSutun',
                                                content: function (rowData)
                                                {
                                                    return $('<a href=# onclick=OpenModal('+rowData.NakitBagisciId+'); class=\'text-link \'>'+rowData.Adi+'</a>')
                                                }
                                                    //{
                                                    //    var pageUrl='BagisciAyrinti.aspx?NakitBagisciId='+rowData.NakitBagisciId;
                                                    //    pageUrl='&apos;'+pageUrl+'&apos;';
                                                    //    var pageTitle='&apos;&apos;';
                                                    //    var pageWidth='&apos;1000&apos;';
                                                    //    var pageHeight='&apos;600&apos;';
                                                    //     var clickFunction='SharepointPopupNoReload('+pageUrl+','+pageTitle+','+pageWidth+','+pageHeight+');';
                                                    //    return $('<a class=\'btn btn-link\' href=# onclick='+clickFunction+'>'+rowData.Adi+'</a>')
                                                    //}
                                            },
                                            { field: 'TCKimlikNo', headerText: 'TCKimlikNo', sortable:true,filter: true },
                                            { field: 'Ili', headerText: 'İl', sortable:true,filter: true },
                                            { field: 'Ilcesi', headerText: 'İlçe', sortable:true,filter: true },
                                            { field: 'Telefon1', headerText: 'Telefon',filter: true},
                                            { field: 'Adres', headerText: 'Adres',filter: true,headerClass:'genisSutun'},
                                            { field: 'NakitBagisciId',headerClass:'darSutun', content: function (rowData)
                                    	                { 
                                                            return $('<a href='+'NakitBagisciEdit.aspx?SenderApp=BB&NakitBagisciId='+rowData.NakitBagisciId + '&Param=" + AsilBagisciAraTxt.Text +
                                                            @" class=\'btn btn-outline-primary \'>Düzenle</a>')
                                    	                }
                                                    },
                                            { field: 'NakitBagisciId', content: function (rowData)
                                    	                { 
                                                            if(rowData.Adi.indexOf('BİLİNMEYEN')>=0){
                                                                return $('');
                                                            }else{
                                                                return $('<a class=\'btn btn-outline-info \' onclick=FillAsilBagisciTable('+rowData.NakitBagisciId+')>Seç</a>')
                                                            }
                                                        }
                                                    }
                                                ],

                                       datasource:" + jsonData + @",
                                       resizableColumns: true,
                                       globalFilter:'#globalFilter'
                                       });
                                    ";


            return ekstretablestr;
        }
        private string BirlesecekBagisciCreateJsString(string jsonData)
        {
            string ekstretablestr = @"   
                                        $('#tblfilter').puidatatable({
                                        caption: '',
                                        editMode: 'cell',
                                        paginator: {
                                                    rows: 8
                                                    },
                                        columns: [
                                            { field: 'Adi', headerText: 'Adi', sortable:true,filter: true,headerClass:'genisSutun',
                                                content: function (rowData)
                                                {
                                                    return $('<a href=# onclick=OpenModal('+rowData.NakitBagisciId+'); class=\'text-link \'>'+rowData.Adi+'</a>')
                                                }
                                            },
                                            { field: 'TCKimlikNo', headerText: 'TCKimlikNo', sortable:true,filter: true },
                                            { field: 'Ili', headerText: 'Ili', sortable:true, sortable:true,filter: true },
                                            { field: 'Ilcesi', headerText: 'İlçesi',filter: true  },
                                            { field: 'Telefon1', headerText: 'Telefon',filter: true},
                                            { field: 'Adres', headerText: 'Adres',filter: true,headerClass:'genisSutun'},
                                            { field: 'NakitBagisciId',content: function (rowData)
                                    	                { 
                                                            if(rowData.Adi.indexOf('BİLİNMEYEN')>=0)
                                                            {
                                                                return $('');
                                                            }else
                                                            {
                                                                //return $('<input type=button class=btn-primary value=Seç onclick=addBirlesecekBagisciTable('+rowData.NakitBagisciId+') />')
                                                                //return $('<input type=button class=btn-primary value=Seç onclick=addBirlesecekBagisciTable('+rowData.NakitBagisciId+') />')
                                                                return $('<input id=chk onchange=addRemoveBagisciToList('+rowData.NakitBagisciId+',this); type=checkbox />')
                                                            }
                                                        }
                                                    }
                                                ],
                                       datasource:" + jsonData + @",
                                       resizableColumns: true,
                                       globalFilter:'#globalFilter'
                                       });
                                    ";


            return ekstretablestr;
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {

        }
        private void AsilBagisciKayitGetir()
        {
            List<NakitBagisci> list = new List<NakitBagisci>();
            var jsonData = GetBagisciData(); //veri çekilip json a çeviriliyor
            if (!string.IsNullOrEmpty(jsonData))
            {
                jsString = CreateJsString(jsonData); //javascript kodu hazırlanıyor.
                System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), jsString, true);
                BagisciSecTableDiv.Attributes["style"] = "display:block";
            }
        }
        private void BirlesecekBagisciKayitGetir()
        {
            List<NakitBagisci> list = new List<NakitBagisci>();
            var jsonData = GetBirlesecekBagisciData(); //veri çekilip json a çeviriliyor
            if (!string.IsNullOrEmpty(jsonData))
            {
                jsString = BirlesecekBagisciCreateJsString(jsonData); //javascript kodu hazırlanıyor.
                System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), jsString, true);
                BagisciSecTableDiv.Attributes["style"] = "display:block";
            }
        }
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
                    IlIlceCell.Text = il.IlAdi;
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
            //BağısciyiBirlestir();
            BagisciyiBirlestirAtomic();

        }
        //private void BagisciyiBirlestir()
        //{
        //    string mesaj1 = string.Empty;
        //    NakitBagisci asilBagisci = new NakitBagisci();
        //    asilBagisci = asilBagisci.Select<NakitBagisci>(SecilenAsilBagisciId.ConvertToInt());
        //    NakitBagisci birlesecekBagisci = new NakitBagisci();
        //    birlesecekBagisci = birlesecekBagisci.Select<NakitBagisci>(BirlesecekBagisciId.ConvertToInt());

        //    if (asilBagisci == null || SecilenAsilBagisciId.ConvertToInt() == 0)
        //    {
        //        MessageHelper.PublishMessage("Seçtiğiniz Asil Bağışçı Bulunamadı.", ProjeConstants.MESAJ_HATA);
        //        return;
        //    }
        //    else if (birlesecekBagisci == null || BirlesecekBagisciId.ConvertToInt() == 0)
        //    {
        //        MessageHelper.PublishMessage("Birleşecek Bağışçı Bulunamadı.", ProjeConstants.MESAJ_HATA);
        //        return;
        //    }
        //    else
        //    {
        //        try
        //        {
        //            //birlesecek bağışçıya ait Hareket listesini al
        //            NakitBagisHareketiDuzenle();

        //            //birlesecek bağışçıya ait Armağan listesini al
        //            ArmaganDuzenle();

        //            // Birleştirilen NakitBagisciyi sil
        //            NakitBagisciDanSil(birlesecekBagisci);
        //            MessageHelper.PublishMessage("Birleştirme Tamamlandı", ProjeConstants.MESAJ_BASARILI);

        //        }
        //        catch (Exception ex)
        //        {
        //            ExceptionHelper exhelper = new ExceptionHelper();
        //            Exception exception = new Exception(mesaj1, ex);
        //            exhelper.Exceptions.Add(exception);
        //            exhelper.PublishException();
        //        }
        //    }
        //}
        private void BagisciyiBirlestirAtomic()
        {
            string currentUser = UtilityHelper.GetCurrentUserLoginName();
            string mesaj1 = string.Empty;
            NakitBagisci asilBagisci = new NakitBagisci();
            asilBagisci = asilBagisci.Select<NakitBagisci>(SecilenAsilBagisciId.ConvertToInt());
            if (asilBagisci == null || SecilenAsilBagisciId.ConvertToInt() == 0)
            {
                MessageHelper.PublishMessage("Seçtiğiniz Asil Bağışçı Bulunamadı.", ProjeConstants.MESAJ_HATA);
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
                                #region  birlesecek bağışçıya ait Hareket listesini al
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
                                        skNBH.SilinenKayitBilgisi = " #BagisciId=" + bagisciIdOnceki + " numaralı bagisciya ait " +
                                            item.Id + " numaralı nakit bağış  ( Bağış Tarihi=" + item.BagisTarihi + " #BagisMiktari=" + item.BagisMiktari + ") " +
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
                                #region birlesecek bağışçıya ait Armağan listesini al
                                string mesaj = string.Empty;
                                try
                                {
                                    Armagan armaganList = new Armagan();
                                    List<Armagan> listofArmagan = armaganList.SelectByBagisciId(birlesecekBagisci.Id.ConvertToInt());
                                    string bagiscisiDegisenArmagans = string.Empty;
                                    foreach (Armagan item in listofArmagan)
                                    {
                                        string bagisciIdOnceki = item.BagisciId.ToString();

                                        item.Aciklama = item.Aciklama + "(" + item.BagisciId + " tarafından yapılan bağış birleştirildi) ";
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
                                        skNBH.SilinenKayitBilgisi = " #BagisciId=" + bagisciIdOnceki + " numaralı bagisciya ait " +
                                            item.Id + " numaralı armagan " +
                                            item.BagisciId + " numaralı bağışçıya birleştirilmiştir.";

                                        DBObject skArmaganDbo = new DBObject();
                                        skArmaganDbo.SQLString = skNBH.GetInsertSQL("");
                                        skArmaganDbo.SQLType = ProjeConstants.SQL_INSERT;
                                        skArmaganDbo.IsFilled = true;
                                        db.DBObjectList.Add(skArmaganDbo);
                                        #endregion

                                    }
                                    //mesaj = string.Format(@"Seçilen Asil Bağışçı={0} ; Birlesecek Bağışçı={1} ; Armağanlar:{2} ",
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

                                //transaction ları yap
                                List<DBObject> savedDBOList = db.ExecuteTransaction();
                                if (savedDBOList.Count > 0)
                                {
                                    #region Birleştirilen NakitBagisciyi sil
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
                                        skBagisci.SilinmeSebebi = "Bağış Birleştirme";
                                        skBagisci.TabloAdi = "NakitBagisci_Table";
                                        skBagisci.SilinmeTarihi = DateTime.Now.ReturnTRDateFormat();
                                        skBagisci.SilinenKayitBilgisi = " #Bağışçı=" + silinecekBagisci.Adi + " " + silinecekBagisci.Soyadi + " #BagisciId=" + silinecekBagisci.Id + " numaralı bağışçı silindi.";
                                    }
                                    List<DBObject> bagisciDBOList = db.ExecuteTransaction();
                                    #endregion

                                    MessageHelper.PublishMessage("Birleştirme Tamamlandı", ProjeConstants.MESAJ_BASARILI);
                                }
                                else
                                {
                                    MessageHelper.PublishMessage("Birleştirme Yapılmadı", ProjeConstants.MESAJ_BILGI);
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
            //------------------------------------


        }

        private void NakitBagisciDanSil(NakitBagisci silinecekBagisci)
        {
            string mesaj = string.Empty;
            try
            {
                NakitBagisci silinenBagisci = silinecekBagisci;
                silinecekBagisci.Delete();
                mesaj = string.Format(@"Silinen Bağışçı Id:{0}, Adı:{1}, TCKimlik:{2}, Adres:{3}, Telefon:{4}",
                    silinenBagisci.Id, silinenBagisci.Adi, silinenBagisci.TCKimlikNo, silinenBagisci.Adres, silinenBagisci.Telefon1);

                //silinen bilgileri sakla
                silinenBilgileriKaydet(mesaj, "NakitBagisci_Table");

            }
            catch (Exception ex)
            {
                ExceptionHelper exhelper = new ExceptionHelper();
                Exception exception = new Exception(mesaj, ex);
                exhelper.Exceptions.Add(exception);
                exhelper.PublishException();
            }
        }
        //private void NakitBagisHareketiDuzenle()
        //{
        //    string mesaj = string.Empty;
        //    try
        //    {
        //        NakitBagisHareket nbh = new NakitBagisHareket();
        //        List<NakitBagisHareket> listofBagisHareket = nbh.SelectByBagisciId(BirlesecekBagisciId.ConvertToInt());

        //        string bagiscisiDegisenNbhs = string.Empty;
        //        foreach (NakitBagisHareket item in listofBagisHareket)
        //        {
        //            item.BagisciId = SecilenAsilBagisciId.ConvertToInt();
        //            item.Update();
        //            bagiscisiDegisenNbhs += "," + item.Id;

        //        }
        //        mesaj = string.Format(@"Seçilen Asil Bağışçı={0} ; Birlesecek Bağışçı={1} ; NakitBagisHareketIdler:{2} ",
        //            SecilenAsilBagisciId, BirlesecekBagisciId, bagiscisiDegisenNbhs);
        //        silinenBilgileriKaydet(mesaj, "NakitBagisHareket_Table");
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionHelper exhelper = new ExceptionHelper();

        //        Exception exception = new Exception(mesaj, ex);
        //        exhelper.Exceptions.Add(exception);
        //        exhelper.PublishException();
        //    }
        //}
        //private void ArmaganDuzenle()
        //{
        //    string mesaj = string.Empty;
        //    try
        //    {
        //        Armagan armagan = new Armagan();
        //        List<Armagan> listofArmagan = armagan.SelectByBagisciId(BirlesecekBagisciId.ConvertToInt());
        //        string bagiscisiDegisenArmagans = string.Empty;
        //        foreach (Armagan item in listofArmagan)
        //        {
        //            item.BagisciId = SecilenAsilBagisciId.ConvertToInt();
        //            item.Update();
        //            bagiscisiDegisenArmagans += "," + item.Id;
        //        }
        //        mesaj = string.Format(@"Seçilen Asil Bağışçı={0} ; Birlesecek Bağışçı={1} ; Armağanlar:{2} ",
        //            SecilenAsilBagisciId, BirlesecekBagisciId, bagiscisiDegisenArmagans);
        //        silinenBilgileriKaydet(mesaj, "Armagan_Table");
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionHelper exhelper = new ExceptionHelper();
        //        Exception exception = new Exception(mesaj, ex);
        //        exhelper.Exceptions.Add(exception);
        //        exhelper.PublishException();
        //    }
        //}
        private void silinenBilgileriKaydet(string mesaj, string tablo)
        {
            try
            {
                string currentUser = UtilityHelper.GetCurrentUserLoginName();
                SilinenKayit sk = new SilinenKayit();
                sk.Silen = currentUser;
                sk.SilinmeSebebi = BirlestirmeSebebiTxt.Text;
                sk.TabloAdi = tablo;
                sk.SilinmeTarihi = DateTime.Now.ReturnTRDateFormat();
                sk.SilinenKayitBilgisi = mesaj;
                sk.Save();
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
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
            AsilBagisciKayitGetir();
        }
        protected void BirlesecekBagisciAraBtn_Click(object sender, EventArgs e)
        {

            BirlesecekBagisciKayitGetir();
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
            AsilBagisciKayitGetir();
        }
        protected void BirlesecekBagisciAraTxt_TextChanged(object sender, EventArgs e)
        {
            BirlesecekBagisciKayitGetir();
            FillSecilenAsilBagisciTable();
        }
        protected void BagisciDuzenleHiddenBtn_Click(object sender, EventArgs e)
        {
            string pageUrl = ProjeConstants.PAGE_NAKITBAGISCI_EDIT + "?NakitBagisciId=" + paramLbl.Value + "&SenderApp=BB";
            RedirectToPage(pageUrl);
        }
        #region Modal popup işlemleri
        protected void ModalDoldurBtn_Click(object sender, EventArgs e)
        {
            try
            {
                NakitBagisListesiniDoldur(paramLbl.Value);

                NakitBagisciFormunuDoldur(paramLbl.Value);
                //tabloda modal açılırken seçili olan pagination degerini pageIndex degiskeninde saklar ve modal açıldıktan sonra pageload sırasında sayfayı pageIndex degerine getirir
                System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "SetPageIndex();", true);
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
                    TuzelKisiCell.Text = nakitBagisci.TuzelKisi.ConvertToBool() ? "Evet" : "Hayır";
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
        private void NakitBagisListesiniDoldur(string nakitBagisciId)
        {
            var jsonData = GetBagisHareketDataJson(nakitBagisciId); //veri çekilip json a çeviriliyor
            var jsString = CreateModalJsString(jsonData); //javascript kodu hazırlanıyor.
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), jsString, true);
        }
        private string GetBagisHareketDataJson(string nakitBagisciId)
        {
            NakitBagisHareket nbh = new NakitBagisHareket();
            int rowCount = 0;
            var json = nbh.SelectByBagisciIdReturnJSon(nakitBagisciId, ref rowCount);
            //RowCountLbl.Text = rowCount.ToString();
            return json;
        }
        private string CreateModalJsString(string jsonData)
        {
            string ekstretablestr = @"$('#modaltblfilter').puidatatable({
                            caption: '',
                            editMode: 'cell',
                            paginator: {
                                        rows: 8
                                        },
                            columns: [
                            { field: 'BagisTarihi', headerText: 'BagisTarihi',sortable:true, 
                                content: function (rowData){ 
                                        if(rowData.BagisTarihi!=null)
                                        {
                                            var date = new Date(parseInt(rowData.BagisTarihi.substr(6)));
                                            return date.getDate()+'/'+(date.getMonth()+1)+'/'+date.getFullYear(); 
                                        }
                                        else
                                        {
                                            return '';
                                        }
                                    }
                                },
                            { field: 'BagisTutari', headerText: 'Bağış Miktarı',filter: true, sortable:true,bodyClass:'text-right',headerStyle:'width: 10%'}, 
                            { field: 'Armagan', headerText: 'Armağan',sortable:true ,bodyClass:'text-center',headerStyle:'width: 20%'  },
                            { field: 'ArmaganTutari', headerText: 'Armağan Tutarı',sortable:true ,bodyClass:'text-center',headerStyle:'width: 10%'  },
                            { field: 'Durum', headerText: 'Durum',filter: true,sortable:true ,bodyClass:'text-center',headerStyle:'width: 15%' },
                            { field: 'Aciklama', headerText: 'Açıklama',bodyClass:'text-center',headerStyle:'width: 20%' },
                                    ],
                                    datasource:" + jsonData + @",
                                    resizableColumns: true,
                                    globalFilter:'#globalFilter',
                                });

                                $('#messages').puigrowl();
                        ";


            return ekstretablestr;
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
    }
}
