using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.SharePointLibraryFileListWP
{
    [ToolboxItemAttribute(false)]
    public partial class SharePointLibraryFileListWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public SharePointLibraryFileListWP()
        {
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string YaziQS
        {
            get
            {

                if (ViewState["Yazi"] == null)
                {
                    if (Page.Request.QueryString["Yazi"] != null)
                    {
                        ViewState["Yazi"] = Page.Request.QueryString["Yazi"];
                    }
                    else
                    {
                        ViewState["Yazi"] = string.Empty;
                    }
                }
                return ViewState["Yazi"].ToString();
            }

            set
            {
                ViewState["yazi"] = value;
            }
        }
        private string LibraryNameQS
        {
            get
            {

                if (ViewState["LibraryName"] == null)
                {
                    if (Page.Request.QueryString["LibraryName"] != null)
                    {
                        ViewState["LibraryName"] = Page.Request.QueryString["LibraryName"];
                    }
                    else
                    {
                        ViewState["LibraryName"] = ProjeConstants.TBYSBELGELERI_LIB;
                    }
                }
                return ViewState["LibraryName"].ToString();
            }

            set
            {
                ViewState["LibraryName"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            string fileName = ProjeConstants.KIRABORCU_DOSYA;
            if (string.IsNullOrEmpty(YaziQS))
            {
                TitleLbl.Text = "Kira Borcu Bildirim Yazıları";
                LibraryNameQS = ProjeConstants.TBYSBELGELERI_LIB;
                fileName = ProjeConstants.KIRABORCU_DOSYA;
            }
            else
            {
                if (YaziQS.Equals(ProjeConstants.KIRAARTIS_DOSYA))
                {
                    TitleLbl.Text = "Kira Artış Bildirim Yazıları";
                    LibraryNameQS = ProjeConstants.TBYSBELGELERI_LIB;
                    fileName = ProjeConstants.KIRAARTIS_DOSYA;

                }
            }
            List<SPFile> fileList = DosyaListesiniGetir(LibraryNameQS, fileName);
            var jsonData = ToJSON(fileList, fileName); //veri çekilip json a çeviriliyor
            //var jsonData = GetBagisciData();
            var jsString = CreateJsString(jsonData); //javascript kodu hazırlanıyor.
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), jsString, true);
        }
        public string ToJSON(List<SPFile> fileList, string pDosyaAdi)
        {
            string json = "[]";
            try
            {
                if (fileList != null)
                {

                    JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
                    List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
                    Dictionary<string, object> childRow;
                    foreach (var file in fileList)
                    {
                        childRow = new Dictionary<string, object>();
                        childRow.Add("FileName", file.Name);
                        childRow.Add("Author", file.Author.Name.ToString());
                        childRow.Add("ModifiedBy", file.ModifiedBy.Name.ToString());
                        childRow.Add("TimeLastModified", file.TimeLastModified.ToString());

                        string sourceString = file.Name;
                        string removeString = pDosyaAdi;
                        int index = sourceString.IndexOf(removeString);
                        string zaman = (index < 0)
                            ? sourceString
                            : sourceString.Remove(index, removeString.Length);
                        string etiketEki = YaziQS.Equals(ProjeConstants.KIRABORCU_DOSYA) ? "KB" : "KA";
                        string etiketDosyaAdi = ProjeConstants.ADRESETIKETI_DOSYA + etiketEki + zaman;

                        bool isDosyaVarMi = DosyaVarMi(LibraryNameQS, etiketDosyaAdi);

                        etiketDosyaAdi = isDosyaVarMi ? etiketDosyaAdi : string.Empty;
                        childRow.Add("LabelFileName", etiketDosyaAdi);

                        parentRow.Add(childRow);
                    }
                    jsSerializer.MaxJsonLength = Int32.MaxValue;
                    json = jsSerializer.Serialize(parentRow);
                }
            }
            catch (Exception e)
            {
                ExceptionHelper eh = new ExceptionHelper(e);
                eh.PublishException();
                throw;
            }
            return json;
        }
        private bool DosyaVarMi(string libName, string fileName)
        {
            bool isDosyaBulundu = false;
            SPList list = SPContext.Current.Web.Lists[libName];
            SPQuery query = new SPQuery();
            query.ViewFields = @"<FieldRef Name='FileLeafRef' />";
            query.Query = @"<Where>
                          <Eq>
                            <FieldRef Name='FileLeafRef' />
                            <Value Type='File'>" + fileName + @"</Value>
                          </Eq>
                        </Where>";
            SPListItemCollection collection = list.GetItems(query);

            if (collection.Count > 0)
            {
                isDosyaBulundu = true;
            }
            return isDosyaBulundu;
        }
        private string CreateJsString(string jsonData)
        {
            string dosyaUrl = SPContext.Current.Web.Url + @"/" + LibraryNameQS + @"/";

            string sourceString = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
            string removeString = System.Web.HttpContext.Current.Request.Url.AbsolutePath;

            int index = sourceString.IndexOf(removeString);
            string rootUrl = (index < 0)
                ? sourceString
                : sourceString.Substring(0, index);
            string tablestr = @"   
                $('#tblfilter').puidatatable({
                    caption: '',
                    editMode: 'cell',
                    paginator: {
                                rows: 8
                                },
                columns: [
                    { field: 'FileName', headerText: 'Dosya Adı', sortable:true,filter: true,headerStyle:'width: 35%' , content: function (rowData)
                        { 
                            return $('<a href=" + rootUrl + "/_layouts/15/download.aspx?SourceUrl=" + dosyaUrl +
                                @"'+rowData.FileName+' class=\'btn-link \'>'+rowData.FileName+'</a>');
                        }
                    },
                    { field: 'LabelFileName', headerText: 'Etiket Dosyası', sortable:true,headerStyle:'width: 35%' , content: function (rowData)
                        { 
                            return $('<a href=" + rootUrl + "/_layouts/15/download.aspx?SourceUrl=" + dosyaUrl +
                                @"'+rowData.LabelFileName+' class=\'btn-link \'>'+rowData.LabelFileName+'</a>');
                        }
                    },
                    { field: 'Author', headerText: 'Yazan', sortable:true,headerStyle:'width: 15%' },
                    { field: 'TimeLastModified', headerText: 'Tarih',headerStyle:'width: 15%'},
                    { field: '', headerText: 'Dosyayı Sil',headerStyle:'width: 15%', content: function (rowData)
                        { 
                            return $('<a href=# onclick=CallButtonClick(\''+rowData.FileName + '\'); class=\'btn btn-outline-danger \'>Dosyayı Sil</a>')  
                        }
                    }
                ],
                datasource:" + jsonData + @",
                resizableColumns: true,
                globalFilter:'#globalFilter'
                });
                  ";


            return tablestr;
        }
        private bool DosyayiSPListesindenSil(string libName, string fileName)
        {
            bool isDeleted = false;
            string newFileUrl = string.Empty;
            List<SPFile> lstFile = new List<SPFile>();
            string siteUrl = SPContext.Current.Web.Url;
            using (SPSite spSite = new SPSite(siteUrl))
            {
                SPList list = SPContext.Current.Web.Lists[libName];
                SPQuery query = new SPQuery();
                query.Query = @"<Where>
                          <Eq>
                            <FieldRef Name='FileLeafRef' />
                            <Value Type='File'>" + fileName + @"</Value>
                          </Eq>
                        </Where>";
                SPListItemCollection collection = list.GetItems(query);
                foreach (SPListItem item in collection)
                {
                    if (item.Name.Equals(fileName))
                    {
                        item.Delete();
                        isDeleted = true;
                        break;
                    }
                }
            }
            return isDeleted;
        }
        private List<SPFile> DosyaListesiniGetir(string libName, string fileName)
        {
            string newFileUrl = string.Empty;
            List<SPFile> lstFile = new List<SPFile>();
            string siteUrl = SPContext.Current.Web.Url;
            using (SPSite spSite = new SPSite(siteUrl))
            {
                SPList list = SPContext.Current.Web.Lists[libName];
                SPQuery query = new SPQuery();
                query.ViewFields = @"<FieldRef Name='FileLeafRef' />";
                query.Query =
                  @"< Where >
                        < Contains >
                            < FieldRef Name = 'Title' />
                            < Value Type = 'File' > " + fileName + @" </ Value >
                        </ Contains >
                    </ Where >
                    <OrderBy>
                        <FieldRef Name='Modified' Ascending='False'/>
                    </OrderBy>";
                SPListItemCollection collection = list.GetItems(query);
                foreach (SPListItem item in collection)
                {
                    if (item.Name.Contains(fileName))
                    {
                        lstFile.Add(item.File);
                    }
                }

                return lstFile;
            }
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        protected void DosyayiSilBtn_Click(object sender, EventArgs e)
        {
            DosyaAdiLbl.Text = paramDosyaAdiLbl.Value;
            DosyayiSilNowBtn.Visible = true;
            var openPopup = "OpenModalOnay();";
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), openPopup, true);
        }
        protected void DosyayiSilNowBtn_Click(object sender, EventArgs e)
        {
            DosyayiSPListesindenSil(LibraryNameQS, paramDosyaAdiLbl.Value);
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            Page.Response.Redirect(currentUrl);
        }

    }
}
