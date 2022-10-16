using Model.NBYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.NakitBagisciBagislariWP
{
    [ToolboxItemAttribute(false)]
    public partial class NakitBagisciBagislariWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public NakitBagisciBagislariWP()
        {
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
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
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    if (!string.IsNullOrEmpty(ParamQS))
                    {
                        BagisciAraTxt.Text = ParamQS;

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
            ExcelBtn.Visible = false;
            AdiLbl.Text = string.Empty;
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
                                            { field: 'Adi', headerText: 'Adi', sortable:true,filter: true, headerStyle: 'width: 25%'},
                                            { field: 'TCKimlikNo', headerText: 'TCKimlikNo', sortable:true,filter: true, headerStyle: 'width: 10%' },
                                            { field: 'Ili', headerText: 'İl', sortable:true,filter: true, headerStyle: 'width: 10%'},
                                            { field: 'Ilcesi', headerText: 'İlçe', sortable:true,filter: true, headerStyle: 'width: 10%' },
                                            { field: 'Telefon1', headerText: 'Telefon',filter: true, headerStyle: 'width: 10%'},
                                            { field: 'Adres', headerText: 'Adres',filter: true, headerStyle: 'width: 25%'},
                                            { field: 'NakitBagisciId', headerStyle: 'width: 10%', content: function (rowData)
                                    	            { 
                                                        return $('<a href=# onclick=BagisListesiGoster('+rowData.NakitBagisciId + '); class=\'btn btn-outline-primary \'>Seç</a>')
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
            ExcelBtn.Visible = rowCount > 0;
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
                                        
                                        { field: 'BagisTarihiDDMMYY', headerText: 'BagisTarihi',filter: true,sortable:true, headerStyle: 'width: 12%'},
                                        { field: 'BagisMiktari', headerText: 'Tutar',filter: true, sortable:true,bodyClass:'text-right', headerStyle: 'width: 12%'}, 
                                        { field: 'DovizCinsi', headerText: 'Döviz', sortable:true, headerStyle: 'width: 6%'},        
                                        { field: 'Banka', headerText: 'Banka',sortable:true ,bodyClass:'text-center', headerStyle: 'width: 10%' },
                                        { field: 'Armagan', headerText: 'Armağan',sortable:true ,bodyClass:'text-center', headerStyle: 'width: 20%' },
                                        { field: 'Aciklama', headerText: 'Açıklama', headerStyle: 'width: 40%' },
                                                ],
                                                datasource:" + jsonData + @",
                                                resizableColumns: true,
                                                globalFilter:'#globalFilter',
                                            });

                                            $('#messages').puigrowl();
                                ";


            return ekstretablestr;
        }
        private DataTable GetBagisListesiData()
        {
            DataTable dataTable = null;
           int nakitBagisciId = paramNakitBagisciIdLbl.Value.ConvertToInt();
            if (nakitBagisciId >0 )
            {
                NakitBagisHareket nbh = new NakitBagisHareket();
                dataTable = nbh.SelectByBagisciIdReturnDataTable(nakitBagisciId);
            }

            return dataTable;
        }
        protected void BagisListesiGosterBtn_Click(object sender, EventArgs e)
        {
            try
            {
                int nakitBagisciId = paramNakitBagisciIdLbl.Value.ConvertToInt();
                if (nakitBagisciId > 0)
                {
                    NakitBagisci nakitBagisci = new NakitBagisci();
                    nakitBagisci = nakitBagisci.Select<NakitBagisci>(nakitBagisciId);
                    AdiLbl.Text = nakitBagisci.Adi + " " + nakitBagisci.Soyadi;
                    NakitBagisListesiniDoldur(paramNakitBagisciIdLbl.Value);
                }
                
            }
            catch (Exception exception)
            {
                ExceptionHelper ex = new ExceptionHelper(exception);
                ex.PublishException();

            }
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
        protected void ExportToExcel()
        {
            DataTable dataTable = GetBagisListesiData();
            if (dataTable!=null)
            {
                GridView GridView1 = new GridView();
                GridView1.AllowPaging = false;


                GridView1.DataSource = dataTable;
                GridView1.DataBind();

                Page.Response.Clear();
                Page.Response.Buffer = true;
                Page.Response.AddHeader("content-disposition",
                 "attachment;filename=BagisListesi(" + DateTime.Now + ").xls");
                Page.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1254");
                Page.Response.Charset = "windows-1254";//ISO-8859-9
                Page.Response.ContentType = "application/vnd.ms-excel";
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
            else
            {
                MessageHelper.PublishMessage("Bağış Bulunamadı", ProjeConstants.MESAJ_BILGI, 2000);
            }

        }

        
    }
}
