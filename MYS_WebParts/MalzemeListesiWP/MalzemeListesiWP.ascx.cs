using Model.MYS;
using Model.Ortak;
using System;
using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;
using TSKGV_Utility.HelperClasses;
using TSKGV_Utility.ProjeGlobal;

namespace MYS_WebParts.MalzemeListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class MalzemeListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public MalzemeListesiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string MalzemeIdQS
        {
            get
            {

                if (ViewState["MalzemeId"] == null)
                {
                    if (Page.Request.QueryString["MalzemeId"] != null)
                    {
                        ViewState["MalzemeId"] = Page.Request.QueryString["MalzemeId"];
                    }
                    else
                    {
                        ViewState["MalzemeId"] = string.Empty;
                    }
                }
                return ViewState["MalzemeId"].ToString();
            }

            set
            {
                ViewState["MalzemeId"] = value;
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
            Malzeme malzeme = new Malzeme();
            var json = malzeme.SelectAllReturnJson();
            return json;
        }
        private string CreateJsString(string jsonData)
        {
            //{ field: 'Gorev', headerText: 'Görev', sortable:true,filter: true },
            //string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            //string rawUrl = System.Web.HttpContext.Current.Request.RawUrl.ToString();
            //int index = currentUrl.IndexOf(rawUrl);
            //string rootUrl = (index < 0) ? currentUrl : currentUrl.Remove(index, rawUrl.Length);
            //string imgUrl = rootUrl + "/" + ProjeConstants.RESIMLER_MALZEME + "/_t/";// + malzeme.Id + "_jpg.jpg";
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));
            string imgUrl = newUrl + "/../" + ProjeConstants.RESIMLER_MALZEME + "/_t/";

            string tablestr = @"   
                                $('#tblfilter').puidatatable({
                                caption: '',
                                editMode: 'cell',
                                paginator: {
                                            rows:8
                                            },
                                columns: [

                                    { field: 'Sirano', headerText: 'Sıra', headerClass:'darSutun' },
                                    { field: 'Malzeme', headerText: 'Malzeme', sortable:true,filter: true},
                                    { field: 'Adet', headerText: 'Adet', sortable:true,filter: true},
                                    { field: 'EnvantereGirisTar', headerText: 'Envantere Gir.Tar.', sortable:true, 
                                        content: function (rowData){ 
                                                                if(rowData.EnvantereGirisTar!=null)
                                                                {
                                                                    var date = new Date(parseInt(rowData.EnvantereGirisTar.substr(6)));
                                                                    return date.format('dd.MM.yyyy');  
                                                                }
                                                                else
                                                                {
                                                                    return '';
                                                                }
                                                            }
                                        },
                                    { field: 'MalzemeCinsi', headerText: 'Malzeme Cinsi',sortable:true,filter: true },
                                    { field: 'Kategori', headerText: 'Kategori',sortable:true,filter: true },
                                    { field: 'MalzemeId',headerClass:'darSutun', content: function (rowData)
                                    	{
                                            return $('<img src=" + imgUrl + @"'+rowData.MalzemeId +  '_jpg.jpg height=50px />')
                                    	}
                                    },

                                    { field: 'MalzemeId',headerClass:'darSutun', content: function (rowData)
                                    	{
                                            return $('<a href='+'MalzemeGirisi.aspx?DestinationApp=MKD&MalzemeId='+rowData.MalzemeId +  ' class=\'btn btn-outline-primary \'>Düzenle</a>')
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
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_MALZEME_LIST;
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
