using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.KiraciEslestirWP
{
    [ToolboxItemAttribute(false)]
    public partial class KiraciEslestirWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public KiraciEslestirWP()
        {
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
                    if (!string.IsNullOrEmpty(KiraEkstreAktarmaIdQS))
                    {
                        KiraEkstreAktarma ekstreAktarma = new KiraEkstreAktarma();
                        ekstreAktarma = ekstreAktarma.Select<KiraEkstreAktarma>(KiraEkstreAktarmaIdQS.ConvertToInt());
                        KiraciAraTxt.Text = ekstreAktarma != null ? ekstreAktarma.Adi : "";

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
        private string GetKiraciData()
        {
            string json = string.Empty;
            if (!string.IsNullOrEmpty(KiraciAraTxt.Text) && KiraciAraTxt.Text.Length >= 3)
            {
                Kiraci kiraci = new Kiraci();
                json = kiraci.SelectByFilter(KiraciAraTxt.Text);
            }
            return json;
        }
        private void KayitGetir()
        {
            var jsonData = GetKiraciData(); //veri çekilip json a çeviriliyor
            if (!string.IsNullOrEmpty(jsonData))
            {
                string jsString = CreateJsString(jsonData); //javascript kodu hazırlanıyor.
                System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), jsString, true);
                KiraciSecTableDiv.Attributes["style"] = "display:block";
            }
        }
        private string CreateJsString(string jsonData)
        {
            string spaceStr = HttpUtility.UrlEncode(KiraciAraTxt.Text.ToString());
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
                                                        return $('<a href=# onclick=OpenModal('+rowData.KiraciId+'); class=\'btn btn-link \'>'+rowData.Adi+'</a>')
                                                    }
                                            },
                                            { field: 'TCKimlikNo', headerText: 'TCKimlikNo', sortable:true,filter: true },
                                            { field: 'Ili', headerText: 'İl', sortable:true,filter: true },
                                            { field: 'Ilcesi', headerText: 'İlçe', sortable:true,filter: true },
                                            { field: 'Telefon', headerText: 'Telefon',filter: true},
                                            { field: 'Adres', headerText: 'Adres',filter: true,headerClass:'genisSutun'},
                                            { field: 'KiraciId', content: function (rowData)
                                    	                { 
                                                            return $('<a href=# onclick=KiraciSec('+rowData.KiraciId+'); class=\'btn btn-outline-primary \'>Seç</a>')
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

        protected void KiraciAraTxt_TextChanged(object sender, EventArgs e)
        {
            KayitGetir();
        }

        protected void KiraciAraBtn_Click(object sender, EventArgs e)
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
                KiraEkstreAktarma ekstreAktarma = new KiraEkstreAktarma();
                ekstreAktarma = ekstreAktarma.Select<KiraEkstreAktarma>(KiraEkstreAktarmaIdQS.ConvertToInt());
                if ((ekstreAktarma != null) && (SenderAppQS.Equals("EkstreListesi")))//ekstrelistesinden'dan geldiyse
                {
                    newUrl += "/" + ProjeConstants.PAGE_KIRAEKSTRE_LIST + "?KiraEkstreAktarmaId=" + KiraEkstreAktarmaIdQS + "&Banka=" + BankaQS;
                }
                else if ((ekstreAktarma != null) && (SenderAppQS.Equals("EAE")))//ekstreaktarmaedit ten geldiyse
                {
                    newUrl += "/" + ProjeConstants.PAGE_EKSTRE_AKTARMAEDIT + "?KiraEkstreAktarmaId=" + KiraEkstreAktarmaIdQS +  "&Banka=" + BankaQS;
                }
                else
                {
                    newUrl += "/" + ProjeConstants.PAGE_KIRAEKSTRE_LIST;
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
            RedirectToPage(ProjeConstants.PAGE_KIRAEKSTRE_LIST + "?KiraEkstreAktarmaId =" + KiraEkstreAktarmaIdQS + "&Banka=" + BankaQS);
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

        protected void ModalDoldurBtn_Click(object sender, EventArgs e)
        {
            try
            {

                //tabloda modal açılırken seçili olan pagination degerini pageIndex degiskeninde saklar ve modal açıldıktan sonra pageload sırasında sayfayı pageIndex degerine getirir
                System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "SetPageIndex();", true);
            }
            catch (Exception exception)
            {
                ExceptionHelper ex = new ExceptionHelper(exception);
                ex.PublishException();
            }
        }
        protected void KiraciSecBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Kiraci kiraci = new Kiraci();
                kiraci = kiraci.Select(paramKiraciIdLbl.Value.ConvertToInt());
                if (kiraci != null)
                {
                    KiraEkstreAktarma kiraEkstreAktarma = new KiraEkstreAktarma();
                    kiraEkstreAktarma = kiraEkstreAktarma.Select<KiraEkstreAktarma>(KiraEkstreAktarmaIdQS.ConvertToInt());
                    if (kiraEkstreAktarma != null)
                    {
                        kiraEkstreAktarma.KiraciId = kiraci.Id;
                        kiraEkstreAktarma.Update();
                        RedirectToPage(ProjeConstants.PAGE_KIRAEKSTRE_LIST );
                    }
                }
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

