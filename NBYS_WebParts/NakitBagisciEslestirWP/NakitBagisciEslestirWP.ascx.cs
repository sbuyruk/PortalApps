using Model.NBYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.NakitBagisciEslestirWP
{
    [ToolboxItemAttribute(false)]
    public partial class NakitBagisciEslestirWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public NakitBagisciEslestirWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string EkstreAktarmaIdQS
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
        private string IslemTarihiQS
        {
            get
            {

                if (ViewState["IslemTarihi"] == null)
                {
                    if (Page.Request.QueryString["IslemTarihi"] != null)
                    {
                        ViewState["IslemTarihi"] = Page.Request.QueryString["IslemTarihi"];
                    }
                    else
                    {
                        ViewState["IslemTarihi"] = string.Empty;
                    }
                }
                return ViewState["IslemTarihi"].ToString();
            }

            set
            {
                ViewState["IslemTarihi"] = value;
            }
        }
        private string BankaQS
        {
            get
            {
                if (ViewState["Banka"] == null)
                {
                    if (Page.Request.QueryString["Banka"] != null)
                    {
                        ViewState["Banka"] = Page.Request.QueryString["Banka"];
                    }
                    else
                    {
                        ViewState["Banka"] = string.Empty;
                    }
                }
                return ViewState["Banka"].ToString();
            }

            set
            {
                ViewState["Banka"] = value;
            }
        }
        private string SenderAppQS
        {
            get
            {

                if (ViewState["SenderApp"] == null)
                {
                    if (Page.Request.QueryString["SenderApp"] != null)
                    {
                        ViewState["SenderApp"] = Page.Request.QueryString["SenderApp"];
                    }
                    else
                    {
                        ViewState["SenderApp"] = string.Empty;
                    }
                }
                return ViewState["SenderApp"].ToString();
            }

            set
            {
                ViewState["SenderApp"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    if (!string.IsNullOrEmpty(EkstreAktarmaIdQS))
                    {
                        EkstreAktarma ekstreAktarma = new EkstreAktarma();
                        ekstreAktarma = ekstreAktarma.Select<EkstreAktarma>(EkstreAktarmaIdQS.ConvertToInt());
                        BagisciAraTxt.Text = ekstreAktarma != null ? ekstreAktarma.Adi : "";

                    }
                }
                KayitGetir();
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
            if (!string.IsNullOrEmpty(BagisciAraTxt.Text) && BagisciAraTxt.Text.Length > 3)
            {
                NakitBagisci nakitBagisci = new NakitBagisci();
                json = nakitBagisci.SelectByFilter(BagisciAraTxt.Text, 0);
            }
            return json;
        }
        private void KayitGetir()
        {
            List<NakitBagisci> list = new List<NakitBagisci>();
            var jsonData = GetBagisciData(); //veri çekilip json a çeviriliyor
            if (!string.IsNullOrEmpty(jsonData))
            {
                string jsString = CreateJsString(jsonData); //javascript kodu hazırlanıyor.
                System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), jsString, true);
                BagisciSecTableDiv.Attributes["style"] = "display:block";
            }
        }
        private string CreateJsString(string jsonData)
        {
            string spaceStr = HttpUtility.UrlEncode(BagisciAraTxt.Text.ToString());
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
                                                        return $('<a href=# onclick=OpenModal('+rowData.NakitBagisciId+'); class=\'btn btn-link \'>'+rowData.Adi+'</a>')
                                                    }
                                            },
                                            { field: 'TCKimlikNo', headerText: 'TCKimlikNo', sortable:true,filter: true },
                                            { field: 'Ili', headerText: 'İl', sortable:true,filter: true },
                                            { field: 'Ilcesi', headerText: 'İlçe', sortable:true,filter: true },
                                            { field: 'Telefon1', headerText: 'Telefon',filter: true},
                                            { field: 'Adres', headerText: 'Adres',filter: true,headerClass:'genisSutun'},
                                            { field: 'NakitBagisciId', content: function (rowData)
                                    	                { 
                                                            return $('<a href='+'EkstreAktarmaEdit.aspx?SenderApp=NBE&EkstreAktarmaId='+" + EkstreAktarmaIdQS + "+'&NakitBagisciId='+rowData.NakitBagisciId + '&Param=" + spaceStr +
                                                            @" class=\'btn btn-outline-success \'>Seç</a>')
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

        protected void BagisciAraTxt_TextChanged(object sender, EventArgs e)
        {
            KayitGetir();
        }

        protected void BagisciAraBtn_Click(object sender, EventArgs e)
        {
            KayitGetir();
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
        protected void BackBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));
                EkstreAktarma ekstreAktarma = new EkstreAktarma();
                ekstreAktarma = ekstreAktarma.Select<EkstreAktarma>(EkstreAktarmaIdQS.ConvertToInt());
                if ((ekstreAktarma != null) && (SenderAppQS.Equals("EkstreListesi")))//ekstrelistesinden'dan geldiyse
                {
                    newUrl += "/" + ProjeConstants.PAGE_EKSTRE_LIST + "?EkstreAktarmaId=" + EkstreAktarmaIdQS + "&IslemTarihi=" + ekstreAktarma.IslemTarihi.ConvertToDatetimeEmptyIfNull() + "&Banka=" + BankaQS;
                }
                else if ((ekstreAktarma != null) && (SenderAppQS.Equals("EAE")))//ekstreaktarmaedit ten geldiyse
                {
                    newUrl += "/" + ProjeConstants.PAGE_EKSTRE_AKTARMAEDIT + "?EkstreAktarmaId=" + EkstreAktarmaIdQS + "&IslemTarihi=" + ekstreAktarma.IslemTarihi.ConvertToDatetimeEmptyIfNull() + "&Banka=" + BankaQS;
                }
                else
                {
                    newUrl += "/" + ProjeConstants.PAGE_EKSTRE_LIST;
                }
                Page.Response.Redirect(newUrl, true);
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void EkstreListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_EKSTRE_LIST + "?IslemTarihi=" + IslemTarihiQS + "&EkstreAktarmaId =" + EkstreAktarmaIdQS + "&Banka=" + BankaQS);
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
                    AdiLbl.Text = string.IsNullOrEmpty(nakitBagisci.Adi.ReturnEmptyIfNull().ToString()) ? "Boş" : nakitBagisci.Adi.ReturnEmptyIfNull().ToString();
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
    }
}

