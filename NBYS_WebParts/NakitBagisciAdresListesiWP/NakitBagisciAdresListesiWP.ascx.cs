using Model.NBYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.NakitBagisciAdresListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class NakitBagisciAdresListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public NakitBagisciAdresListesiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string BagisciSayisiQS
        {
            get
            {

                if (ViewState["BagisciSayisi"] == null)
                {
                    if (Page.Request.QueryString["BagisciSayisi"] != null)
                    {
                        ViewState["BagisciSayisi"] = Page.Request.QueryString["BagisciSayisi"];
                    }
                    else
                    {
                        ViewState["BagisciSayisi"] = string.Empty;
                    }
                }
                return ViewState["BagisciSayisi"].ToString();
            }

            set
            {
                ViewState["BagisciSayisi"] = value;
            }
        }
        private string BasTarQS
        {
            get
            {

                if (ViewState["BasTar"] == null)
                {
                    if (Page.Request.QueryString["BasTar"] != null)
                    {
                        ViewState["BasTar"] = Page.Request.QueryString["BasTar"];
                    }
                    else
                    {
                        ViewState["BasTar"] = string.Empty;
                    }
                }
                return ViewState["BasTar"].ToString();
            }

            set
            {
                ViewState["BasTar"] = value;
            }
        }
        private string BitTarQS
        {
            get
            {

                if (ViewState["BitTar"] == null)
                {
                    if (Page.Request.QueryString["BitTar"] != null)
                    {
                        ViewState["BitTar"] = Page.Request.QueryString["BitTar"];
                    }
                    else
                    {
                        ViewState["BitTar"] = string.Empty;
                    }
                }
                return ViewState["BitTar"].ToString();
            }

            set
            {
                ViewState["BitTar"] = value;
            }
        }
        private string UlasilamayanlarHaricQS
        {
            get
            {

                if (ViewState["UlasilamayanlarHaric"] == null)
                {
                    if (Page.Request.QueryString["UlasilamayanlarHaric"] != null)
                    {
                        ViewState["UlasilamayanlarHaric"] = Page.Request.QueryString["UlasilamayanlarHaric"];
                    }
                    else
                    {
                        ViewState["UlasilamayanlarHaric"] = string.Empty;
                    }
                }
                return ViewState["UlasilamayanlarHaric"].ToString();
            }

            set
            {
                ViewState["UlasilamayanlarHaric"] = value;
            }
        }
        private string BelgeIstemeyenlerHaricQS
        {
            get
            {

                if (ViewState["BelgeIstemeyenlerHaric"] == null)
                {
                    if (Page.Request.QueryString["BelgeIstemeyenlerHaric"] != null)
                    {
                        ViewState["BelgeIstemeyenlerHaric"] = Page.Request.QueryString["BelgeIstemeyenlerHaric"];
                    }
                    else
                    {
                        ViewState["BelgeIstemeyenlerHaric"] = string.Empty;
                    }
                }
                return ViewState["BelgeIstemeyenlerHaric"].ToString();
            }

            set
            {
                ViewState["BelgeIstemeyenlerHaric"] = value;
            }
        }
        private string AdresiBosOlanlarHaricQS
        {
            get
            {

                if (ViewState["AdresiBosOlanlarHaric"] == null)
                {
                    if (Page.Request.QueryString["AdresiBosOlanlarHaric"] != null)
                    {
                        ViewState["AdresiBosOlanlarHaric"] = Page.Request.QueryString["AdresiBosOlanlarHaric"];
                    }
                    else
                    {
                        ViewState["AdresiBosOlanlarHaric"] = string.Empty;
                    }
                }
                return ViewState["AdresiBosOlanlarHaric"].ToString();
            }

            set
            {
                ViewState["AdresiBosOlanlarHaric"] = value;
            }
        }
        private string PostadanIadeEdilenlerHaricQS
        {
            get
            {

                if (ViewState["PostadanIadeEdilenlerHaric"] == null)
                {
                    if (Page.Request.QueryString["PostadanIadeEdilenlerHaric"] != null)
                    {
                        ViewState["PostadanIadeEdilenlerHaric"] = Page.Request.QueryString["PostadanIadeEdilenlerHaric"];
                    }
                    else
                    {
                        ViewState["PostadanIadeEdilenlerHaric"] = string.Empty;
                    }
                }
                return ViewState["PostadanIadeEdilenlerHaric"].ToString();
            }

            set
            {
                ViewState["PostadanIadeEdilenlerHaric"] = value;
            }
        }
        private string DergiGonderilmeyeceklerHaricQS
        {
            get
            {

                if (ViewState["DergiGonderilmeyeceklerHaric"] == null)
                {
                    if (Page.Request.QueryString["DergiGonderilmeyeceklerHaric"] != null)
                    {
                        ViewState["DergiGonderilmeyeceklerHaric"] = Page.Request.QueryString["DergiGonderilmeyeceklerHaric"];
                    }
                    else
                    {
                        ViewState["DergiGonderilmeyeceklerHaric"] = string.Empty;
                    }
                }
                return ViewState["DergiGonderilmeyeceklerHaric"].ToString();
            }

            set
            {
                ViewState["DergiGonderilmeyeceklerHaric"] = value;
            }
        }
        private string SadeceYeniBagiscilarQS
        {
            get
            {

                if (ViewState["SadeceYeniBagiscilar"] == null)
                {
                    if (Page.Request.QueryString["SadeceYeniBagiscilar"] != null)
                    {
                        ViewState["SadeceYeniBagiscilar"] = Page.Request.QueryString["SadeceYeniBagiscilar"];
                    }
                    else
                    {
                        ViewState["SadeceYeniBagiscilar"] = string.Empty;
                    }
                }
                return ViewState["SadeceYeniBagiscilar"].ToString();
            }

            set
            {
                ViewState["SadeceYeniBagiscilar"] = value;
            }
        }
        private string EkstreAktarmaIdQS //ekstreaktarmaedit ten secimyapmak için bağışçı listesi açıldığında query string ile gelen ekstreaktarmaid
        {
            get
            {
                if (ViewState["EkstreAktarmaId"] == null)
                {
                    if (Page.Request.QueryString["EkstreAktarmaId"] != null)
                    {
                        ViewState["EkstreAktarmaId"] = Page.Request.QueryString["EkstreAktarmaId"];
                    }
                    else
                    {
                        ViewState["EkstreAktarmaId"] = string.Empty;
                    }
                }
                return ViewState["EkstreAktarmaId"].ToString();
            }

            set
            {
                ViewState["EkstreAktarmaId"] = value;
            }
        }
        private string PageIndexQS
        {
            get
            {

                if (ViewState["PageIndex"] == null)
                {
                    if (Page.Request.QueryString["PageIndex"] != null)
                    {
                        ViewState["PageIndex"] = Page.Request.QueryString["PageIndex"];
                    }
                    else
                    {
                        ViewState["PageIndex"] = string.Empty;
                    }
                }
                return ViewState["PageIndex"].ToString();
            }

            set
            {
                ViewState["PageIndex"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    BagisciSayisiTxt.Text = "1000";
                    DateTime today = DateTime.Today;
                    DateTime basyil = today.AddYears(-1);
                    BasTarTxt.Text = basyil.ConvertToDatetimeEmptyIfNull();
                    BitTarTxt.Text = today.ConvertToDatetimeEmptyIfNull();
                    BagisciSayisiQS = string.IsNullOrEmpty(BagisciSayisiQS) ? BagisciSayisiTxt.Text : BagisciSayisiQS;
                    BasTarQS = string.IsNullOrEmpty(BasTarQS) ? BasTarTxt.Text : BasTarQS;
                    BitTarQS = string.IsNullOrEmpty(BitTarQS) ? BitTarTxt.Text : BitTarQS;
                    UlasilamayanlarHaricQS = string.IsNullOrEmpty(UlasilamayanlarHaricQS) ? true.ToString() : UlasilamayanlarHaricQS;
                    BelgeIstemeyenlerHaricQS = string.IsNullOrEmpty(BelgeIstemeyenlerHaricQS) ? true.ToString() : BelgeIstemeyenlerHaricQS;
                    AdresiBosOlanlarHaricQS = string.IsNullOrEmpty(AdresiBosOlanlarHaricQS) ? true.ToString() : AdresiBosOlanlarHaricQS;
                    PostadanIadeEdilenlerHaricQS = string.IsNullOrEmpty(PostadanIadeEdilenlerHaricQS) ? true.ToString() : PostadanIadeEdilenlerHaricQS;
                    DergiGonderilmeyeceklerHaricQS = string.IsNullOrEmpty(DergiGonderilmeyeceklerHaricQS) ? true.ToString() : DergiGonderilmeyeceklerHaricQS;
                    SadeceYeniBagiscilarQS = string.IsNullOrEmpty(SadeceYeniBagiscilarQS) ? false.ToString() : SadeceYeniBagiscilarQS;
                    BagisciSayisiTxt.Text = BagisciSayisiQS;
                    BasTarTxt.Text = BasTarQS;
                    BitTarTxt.Text = BitTarQS;
                    UlasilamayanlarHaricChk.Checked = UlasilamayanlarHaricQS.ConvertToBool();
                    BelgeIstemeyenlerHaricChk.Checked = BelgeIstemeyenlerHaricQS.ConvertToBool();
                    AdresiBosOlanlarHaricChk.Checked = AdresiBosOlanlarHaricQS.ConvertToBool();
                    BelgesiPostadanIadeEdilenlerHaricChk.Checked = PostadanIadeEdilenlerHaricQS.ConvertToBool();
                    DergiGonderilmesinlerHaricChk.Checked = DergiGonderilmeyeceklerHaricQS.ConvertToBool();
                    SadeceYeniBagiscilarChk.Checked = SadeceYeniBagiscilarQS.ConvertToBool();
                }
                KayitGetir();
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private string CreateJsString(string jsonData)
        {

            string queryStr = "&BagisciSayisi=" + BagisciSayisiQS + "&BasTar=" + BasTarQS + "&BitTar=" + BitTarQS + "&UlasilamayanlarHaric=" + UlasilamayanlarHaricQS + "&BelgeIstemeyenlerHaric=" + BelgeIstemeyenlerHaricQS +
                "&AdresiBosOlanlarHaric=" + AdresiBosOlanlarHaricQS + "&PostadanIadeEdilenlerHaric=" + PostadanIadeEdilenlerHaricQS +
                "&DergiGonderilmeyeceklerHaric=" + DergiGonderilmeyeceklerHaricQS + "&SadeceYeniBagiscilar=" + SadeceYeniBagiscilarQS + "&PageIndex='+currentPage+'";
            string ekstretablestr = @" 
                                        //tabloda modal açılırken seçili olan pagination degerini pageIndex degiskeninde saklar ve modal açıldıktan sonra pageload sırasında sayfayı pageIndex degerine getirir
                                        $(document).on('click','.ui-paginator-page',function(){
                                            pageIndex= parseInt($(this).text());
                                        });
  
                                        $('#tblfilter').puidatatable({
                                        caption: '',
                                        editMode: 'cell',
                                        paginator: {
                                                    rows: 8
                                                    },
                                        columns: [
                                            { field: 'Adi', headerText: 'Adi', sortable:true,filter: true,headerStyle:'width: 15%',
                                                content: function (rowData)
                                                    {
                                                        return $('<a href=# onclick=OpenModal('+rowData.NakitBagisciId+'); class=\'btn-link  text-primary\'>'+rowData.Adi+'</a>')
                                                    }
                                            },
                                            { field: 'TuzelKisi', headerText: 'T/Ö',bodyClass:'text-center', sortable:true,filter: true,headerStyle:'width: 4%',
                                                content: function (rowData)
                                                    {
                                                        if (rowData.TuzelKisi=='True'){
                                                            return 'T';
                                                        }else{
                                                            return 'Ö';
                                                        }
 
                                                        
                                                    }
                                            },
                                            { field: 'BagisMiktari', headerText: 'Bağış Tutarı',bodyClass:'text-right',headerStyle:'width: 10%' },
                                            { field: 'Adres', headerText: 'Adres',filter: true,headerStyle:'width: 20%'},
                                            { field: 'Ili', headerText: 'Ili', sortable:true, sortable:true,filter: true,headerStyle:'width: 10%' },
                                            { field: 'Ilcesi', headerText: 'İlçesi',filter: true,headerStyle:'width: 10%'  },
                                            { field: 'Telefon1', headerText: 'Telefon',filter: true,headerStyle:'width: 10%'},
                                            { field: 'NakitBagisciId',headerStyle:'width: 9%', content: function (rowData)
                                    	        { 
                                                    var sirano=parseInt(rowData.Sirano);
                                                    var currentPage=Math.ceil(sirano/8);
                                                    return $('<a class=\'btn btn-outline-primary\' href=' + 'NakitBagisciEdit.aspx?SenderApp=NBAL&NakitBagisciId=' + rowData.NakitBagisciId + '" + queryStr + @" >Düzenle</a>')
                                    	        }
                                            },
                                            { field: 'NakitBagisciId',bodyClass:'text-center',headerStyle:'width: 12%', content: function (rowData)
                                    	        { 
                                                    if (rowData.DergiGonderilmesin=='False'){
                                                        return $('<a href=# onclick=CallButtonClick('+rowData.NakitBagisciId + ',\'gonderme\'); class=\'btn btn-outline-danger \'>Dergi Gönderme</a>')                                                        
                                                    } else{
                                                        return $('<a href=# onclick=CallButtonClick('+rowData.NakitBagisciId + ',\'gonder\'); class=\'btn btn-outline-success \'>Dergi Gönder</a>')          
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
        private void KayitGetir()
        {
            List<NakitBagisci> list = new List<NakitBagisci>();
            var jsonData = NakitBagisciJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateJsString(jsonData); //javascript kodu hazırlanıyor.
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), jsString, true);
        }
        private string NakitBagisciJson()
        {
            string jSon = string.Empty;

            List<NakitBagisciListItem> list = GetDataList();
            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
            jSon = serializer.Serialize(list);
            return jSon;
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
            var json = nbh.SelectByBagisciId(nakitBagisciId, ref rowCount);
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
                                        { field: 'TCKimlikNo', headerText: 'TCKimlikNo',filter: true,sortable:true,bodyClass:'text-center'  },
                                        { field: 'BagisMiktari', headerText: 'Tutar',filter: true, sortable:true,bodyClass:'text-right'}, 
                                        { field: 'DovizCinsi', headerText: 'Döviz', sortable:true,headerClass:'darSutun'},                                    
                                        { field: 'Armagan', headerText: 'Armağan',sortable:true ,bodyClass:'text-center',headerClass:'genisSutun'  },
                                        { field: 'Durum', headerText: 'Durum',filter: true,sortable:true ,bodyClass:'text-center' },
                                        { field: 'Aciklama', headerText: 'Açıklama',bodyClass:'text-center' },
                                                ],
                                                datasource:" + jsonData + @",
                                                resizableColumns: true,
                                                globalFilter:'#globalFilter',
                                            });

                                            $('#messages').puigrowl();
                                ";


            return ekstretablestr;
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
        protected void BagisTarihiTxt_TextChanged(object sender, EventArgs e)
        {
            BagisciSayisiQS = BagisciSayisiTxt.Text;
            BasTarQS = BasTarTxt.Text.ConvertToDatetimeEmptyIfNull();
            BitTarQS = BitTarTxt.Text.ConvertToDatetimeEmptyIfNull();
            UlasilamayanlarHaricQS = UlasilamayanlarHaricChk.Checked.ToString();
            BelgeIstemeyenlerHaricQS = BelgeIstemeyenlerHaricChk.Checked.ToString();
            AdresiBosOlanlarHaricQS = AdresiBosOlanlarHaricChk.Checked.ToString();
            PostadanIadeEdilenlerHaricQS = BelgesiPostadanIadeEdilenlerHaricChk.Checked.ToString();
            DergiGonderilmeyeceklerHaricQS = DergiGonderilmesinlerHaricChk.Checked.ToString();
            SadeceYeniBagiscilarQS = SadeceYeniBagiscilarChk.Checked.ToString();

            KayitGetir();
        }
        protected void ExcelBtn_Click(object sender, EventArgs e)
        {
            try
            {
                ExportToExcel();
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private List<NakitBagisciListItem> GetDataList()
        {

            NakitBagisci bagisci = new NakitBagisci();
            DateTime today = DateTime.Today;
            DateTime basTar = BasTarTxt.Text.ConvertToDatetime();
            DateTime bitTar = BitTarTxt.Text.ConvertToDatetime();
            int bagisciSayisi = BagisciSayisiTxt.Text.ConvertToInt();
            int rowCount = 0;

            DataTable dataTable = bagisci.SelectByBagisTarihiBagisSayisi(basTar, bitTar, bagisciSayisi, ref rowCount,
                BelgeIstemeyenlerHaricChk.Checked, AdresiBosOlanlarHaricChk.Checked, BelgesiPostadanIadeEdilenlerHaricChk.Checked,
                DergiGonderilmesinlerHaricChk.Checked, UlasilamayanlarHaricChk.Checked, SadeceYeniBagiscilarChk.Checked);
            int SiraNo = 0;
            RowCountLbl.Text = "Kayıt Sayısı : " + rowCount.ToString();
            List<NakitBagisciListItem> list = new List<NakitBagisciListItem>();
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            DataView dataView = new DataView(dataTable);
            foreach (DataRowView row in dataView)
            {

                decimal bagisMiktari = row["BagisMiktari"].ConvertToDecimal();
                string nakitBagisciId = row["NakitBagisciId"].ToString();
                string adi = row["Adi"].ToString();
                string telefon1 = row["Telefon1"].ToString();
                string telefon2 = row["Telefon2"].ToString();

                string adres = row["Adres"].ToString();
                string ili = row["Ili"].ToString();
                string ilcesi = row["Ilcesi"].ToString();
                string tuzelKisi = row["TuzelKisi"].ToString();
                string dergiGonderilmesin = row["DergiGonderilmesin"].ToString();

                NakitBagisciListItem nakitBagisciItem = new NakitBagisciListItem();
                nakitBagisciItem.Sirano = SiraNo++.ToString();
                nakitBagisciItem.BagisMiktari = bagisMiktari.ToString("N", culturInfo);
                nakitBagisciItem.NakitBagisciId = nakitBagisciId.ToString();
                nakitBagisciItem.Adi = adi;
                nakitBagisciItem.Telefon1 = telefon1;
                nakitBagisciItem.Telefon2 = telefon2.ConvertToDatetimeEmptyIfNull();
                nakitBagisciItem.Adres = "- " + adres;
                nakitBagisciItem.Ilcesi = ilcesi;
                nakitBagisciItem.Ili = ili;
                nakitBagisciItem.DergiGonderilmesin = dergiGonderilmesin.ConvertToBool().ToString();
                nakitBagisciItem.TuzelKisi = tuzelKisi.ConvertToBool().ToString();
                list.Add(nakitBagisciItem);
            }
            return list;
        }
        protected void ExportToExcel()
        {

            //
            GridView GridView1 = new GridView();
            GridView1.AllowPaging = false;
            GridView1.DataSource = GetDataList();
            GridView1.DataBind();

            Page.Response.Clear();
            Page.Response.Buffer = true;
            Page.Response.AddHeader("content-disposition",
             "attachment;filename=BagisciListesi_" + BasTarQS.ConvertToDatetimeEmptyIfNull() + "_" + BitTarQS.ConvertToDatetimeEmptyIfNull() + ".xls");

            //Türkçe karakter sorunu düzeltmek için
            //buradan başladı
            //Page.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1254");
            //Page.Response.Charset = "ISO-8859-9";//"windows-1254"
            //Page.Response.ContentType = "application/vnd.ms-excel";

            Page.Response.ContentType = "application/ms-excel";
            Page.Response.ContentEncoding = System.Text.Encoding.Unicode;
            Page.Response.BinaryWrite(System.Text.Encoding.Unicode.GetPreamble());
            ///buraya kadar değişti 
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);

            for (int i = 0; i < GridView1.Rows.Count; i++)
            {
                //Apply text style to each Row
                GridView1.Rows[i].Attributes.Add("class", "textmode");
            }
            GridView1.RenderControl(hw);

            //style to format numbers to string
            string style = @"<style> .textmode { mso-number-format:\@; } </style>";
            Page.Response.Write(style);
            Page.Response.Output.Write(sw.ToString());
            Page.Response.Flush();
            Page.Response.End();
        }
        protected void ModalDoldurBtn_Click(object sender, EventArgs e)
        {
            try
            {
                NakitBagisListesiniDoldur(paramNakitBagisciIdLbl.Value);

                NakitBagisciFormunuDoldur(paramNakitBagisciIdLbl.Value);
                //tabloda modal açılırken seçili olan pagination degerini pageIndex degiskeninde saklar ve modal açıldıktan sonra pageload sırasında sayfayı pageIndex degerine getirir
                System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "SetPageIndex();", true);
            }
            catch (Exception exception)
            {
                ExceptionHelper ex = new ExceptionHelper(exception);
                ex.PublishException();

            }
        }
        protected void DergiGondermeBtn_Click(object sender, EventArgs e)
        {
            try
            {
                NakitBagisci nb = new NakitBagisci();
                nb = nb.Select<NakitBagisci>(paramNakitBagisciIdLbl.Value.ConvertToInt());
                if (nb != null)
                {
                    nb.Degistiren = UtilityHelper.GetCurrentUser();
                    if (isDergiGonderLbl.Value.Equals("gonder"))
                    {
                        nb.DergiGonderilmesin = ProjeConstants.DERGI_GONDERILSIN;
                    }
                    else
                    {
                        nb.DergiGonderilmesin = ProjeConstants.DERGI_GONDERILMESIN;
                    }

                    if (nb.Update())
                    {
                        MessageHelper.PublishMessage(nb.Adi + " Adlı bağışçıya dergi gönderilmeyecek.", ProjeConstants.MESAJ_BASARILI, 2000);

                        string queryStr = "&BagisciSayisi=" + BagisciSayisiQS + "&BasTar=" + BasTarQS + "&BitTar=" + BitTarQS + "&UlasilamayanlarHaric=" + UlasilamayanlarHaricQS +
                            "&BelgeIstemeyenlerHaric=" + BelgeIstemeyenlerHaricQS + "&AdresiBosOlanlarHaric=" + AdresiBosOlanlarHaricQS +
                            "&PostadanIadeEdilenlerHaric=" + PostadanIadeEdilenlerHaricQS + "&DergiGonderilmeyeceklerHaric=" + DergiGonderilmeyeceklerHaricQS + "&SadeceYeniBagiscilar=" + SadeceYeniBagiscilarQS;
                        RedirectToPage(ProjeConstants.PAGE_NAKITBAGISCI_ADRESLIST + "?Mesaj=true&PageIndex=" + PageIndexLbl.Value + queryStr);
                    }
                    else
                    {
                        MessageHelper.PublishMessage(" Bağışçı bilgisi değiştirilemedi.", ProjeConstants.MESAJ_HATA);
                    }
                }
                else
                {
                    MessageHelper.PublishMessage(" Bağışçı bulunamadı.", ProjeConstants.MESAJ_HATA);
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exh = new ExceptionHelper();
                exh.Exceptions.Add(new Exception("Dergi Gönderme durumu değiştirilemedi. "));
                exh.Exceptions.Add(ex);
                exh.PublishException();
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
        private class NakitBagisciListItem
        {
            public string Sirano { get; set; }
            public string BagisMiktari { get; set; }
            public string NakitBagisciId { get; set; }
            public string Adi { get; set; }
            public string Telefon1 { get; set; }
            public string Telefon2 { get; set; }
            public string Ili { get; set; }
            public string Ilcesi { get; set; }
            public string Adres { get; set; }
            public string TuzelKisi { get; set; }
            public string DergiGonderilmesin { get; set; }
        }
    }
}
