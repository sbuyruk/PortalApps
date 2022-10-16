using Model.MYS;
using Model.Ortak;
using System;
using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;
using TSKGV_Utility.HelperClasses;
using TSKGV_Utility.ProjeGlobal;

namespace MYS_WebParts.MalzemeCinsiListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class MalzemeCinsiListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public MalzemeCinsiListesiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string MalzemeCinsiIdQS
        {
            get
            {

                if (ViewState["MalzemeCinsiId"] == null)
                {
                    if (Page.Request.QueryString["MalzemeCinsiId"] != null)
                    {
                        ViewState["MalzemeCinsiId"] = Page.Request.QueryString["MalzemeCinsiId"];
                    }
                    else
                    {
                        ViewState["MalzemeCinsiId"] = string.Empty;
                    }
                }
                return ViewState["MalzemeCinsiId"].ToString();
            }

            set
            {
                ViewState["MalzemeCinsiId"] = value;
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
        private string CurrentUserName
        {
            get
            {

                if (ViewState["CurrentUserName"] == null)
                {
                    ViewState["CurrentUserName"] = UtilityHelper.GetCurrentUser();
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
                if (!Page.IsPostBack)
                {
                    KayitGetir();
                    if (!string.IsNullOrEmpty(MesajQS))
                    {
                        MessageHelper.PublishMessage("İşlem Tamamlandı", ProjeConstants.MESAJ_BASARILI, 2000);
                        MesajQS = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private void KayitGetir()
        {
            var jsonData = GetDataJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateJsString(jsonData); //javascript kodu hazırlanıyor.
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), jsString, true);
        }
        private string GetDataJson()
        {
            MalzemeCinsi mk = new MalzemeCinsi();
            var json = mk.SelectAllReturnJson();
            return json;
        }
        private string CreateJsString(string jsonData)
        {
            //{ field: 'Gorev', headerText: 'Görev', sortable:true,filter: true },
            string tablestr = @"   
                                $('#tblfilter').puidatatable({
                                caption: '',
                                editMode: 'cell',
                                paginator: {
                                            rows:8
                                            },
                                columns: [

                                    { field: 'Sirano', headerText: 'Sıra', sortable:true,filter: true },
                                    { field: 'MalzemeCinsi', headerText: 'Malzeme Cinsi', sortable:true,filter: true},

                                    { field: 'Kategori', headerText: 'Malzeme Kategorisi',sortable:true,filter: true },
                                    { field: 'MalzemeCinsiId',headerClass:'darSutun', content: function (rowData)
                                    	{
                                            return $('<a href='+'MalzemeCinsiGirisi.aspx?DestinationApp=MKD&MalzemeCinsiId='+rowData.MalzemeCinsiId +  ' class=\'btn btn-outline-primary \'>Düzenle</a>')
                                    	}
                                    }

                                ],
                                datasource:" + jsonData + @",
                                resizableColumns: true,
                                globalFilter:'#globalFilter'
                            });
                            $('#messages').puigrowl();
                            ";

            return tablestr;
        }
        protected void BackBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_MALZEMECINSI_LIST;
            Page.Response.Redirect(newUrl);
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
    }
}
