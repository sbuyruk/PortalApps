using Microsoft.SharePoint;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.SPDosyaListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class SPDosyaListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public SPDosyaListesiWP()
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
                        ViewState["LibraryName"] = ProjeConstants.NBYSBELGELERI_LIB;
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
            string fileName = ProjeConstants.TESEKKUR_DOSYA;
            if (string.IsNullOrEmpty(YaziQS))
            {
                TitleLbl.Text = "Tesekkür Belgesi Dosyalari";
                LibraryNameQS = ProjeConstants.NBYSBELGELERI_LIB;
                fileName = ProjeConstants.TESEKKUR_DOSYA;
            }
            
            TabloOlustur(fileName);
            
        }
        public string ToJSON(List<SPFile> fileList, string pDosyaAdi)
        {
            string json = "[]";
            try
            {
                if (fileList != null)
                {

                    string dosyaUrl = SPContext.Current.Web.Url + @"/" + LibraryNameQS + @"/";

                    string sourceString = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                    string removeString = System.Web.HttpContext.Current.Request.Url.AbsolutePath;

                    int index = sourceString.IndexOf(removeString);
                    string rootUrl = (index < 0)
                        ? sourceString
                        : sourceString.Substring(0, index);

                    JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
                    List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
                    Dictionary<string, object> childRow;
                    foreach (var file in fileList)
                    {
                        if (file.Name.Equals("TesekkurBelgesiTemplate.docx"))
                            continue;//template dosyasini listeye koyma

                        childRow = new Dictionary<string, object>();
                        string fileUrl = "<a href=" + rootUrl + "/_layouts/15/download.aspx?SourceUrl=" + dosyaUrl + file.Name + @" class='btn-link fw-bold'>" + file.Name + "</a>";
                        childRow.Add("FileName", fileUrl);
                        childRow.Add("Author", file.Author.Name.ToString());
                        childRow.Add("ModifiedBy", file.ModifiedBy.Name.ToString());
                        DateTime lastModified = TimeZone.CurrentTimeZone.ToLocalTime(file.TimeLastModified);
                        childRow.Add("TimeLastModified", lastModified.ConvertToDDMMYYYHHmmFormat());

                        string sourceFileString = file.Name;
                        string removeFileString = pDosyaAdi;
                        int indexOfRemove = sourceFileString.IndexOf(removeFileString);
                        string zaman = (indexOfRemove < 0)
                            ? sourceFileString
                            : sourceFileString.Remove(indexOfRemove, removeFileString.Length);
                        string etiketEki = "TES";
                        string etiketDosyaAdi = ProjeConstants.ADRESETIKETI_DOSYA + etiketEki + zaman;

                        bool isDosyaVarMi = DosyaVarMi(LibraryNameQS, etiketDosyaAdi);

                        etiketDosyaAdi = isDosyaVarMi ? etiketDosyaAdi : string.Empty;

                        string labelUrl = "<a href=" + rootUrl + "/_layouts/15/download.aspx?SourceUrl=" + dosyaUrl + etiketDosyaAdi + @" class='btn-link fw-bold'>" + etiketDosyaAdi + "</a>";
                        childRow.Add("LabelFileName", labelUrl);
                        
                        string deleteFile = "<a href=# onclick=CallButtonClick('"+file.Name + "','"+ etiketDosyaAdi+"'); class='btn btn-outline-danger'>Dosyayi Sil</a>";
                        childRow.Add("DeleteFile", deleteFile);

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
        private void TabloOlustur(string fileName)
        {
            List<SPFile> fileList = DosyaListesiniGetir(LibraryNameQS, fileName);
            var jsonData = ToJSON(fileList, fileName); //veri çekilip json a çeviriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazirlaniyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string CreateDataTable(string jsonData)
        {
            string dosyaUrl = SPContext.Current.Web.Url + @"/" + LibraryNameQS + @"/";

            string sourceString = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
            string removeString = System.Web.HttpContext.Current.Request.Url.AbsolutePath;

            int index = sourceString.IndexOf(removeString);
            string rootUrl = (index < 0)
                ? sourceString
                : sourceString.Substring(0, index);

            string tableString = @"
                if ( jQuery.fn.DataTable.isDataTable('#CustomDataTable') ) {
                    jQuery('#CustomDataTable').DataTable().destroy();
                }
                jQuery('#CustomDataTable tbody').empty();

                jQuery.fn.dataTable.moment('DD.MM.YYYY HH:mm');//sort date
                jQuery(document).ready(function () {
                    jQuery('#CustomDataTable').DataTable({
                        data: " + jsonData + @",
                        columns: [
                            { data: 'FileName' },
                            { data: 'LabelFileName'},
                            { data: 'Author'},
                            { data: 'TimeLastModified'},
                            { data: 'DeleteFile'},
                        ],
                        columnDefs: [                           
                            
                        ],
                        'order': [[3, 'desc']],//sort 
                        'language': {
                            'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                            'decimal': ',',
                            'thousands': '.'
                        },
                        responsive: true,
                        destroy: true,
                        pageLength:10,
                        dom: 'ftipr',
                        
                        });
                    });

            ";

            return tableString;
        }
        private bool DosyaVarMi(string libName, string fileName)
        {
            SPWeb web = SPContext.Current.Web;
            string fileUrl = web.Url + "/" + libName + "/" + fileName;
            SPFile file = web.GetFile(fileUrl);
            return file != null && file.Exists;
        }
        private bool DosyalariSPListesindenSil(string libName, string tesekkurDosyaAdi, string etiketDosyaAdi)
        {
            bool isDeleted = false;
            string newFileUrl = string.Empty;
            List<SPFile> lstFile = new List<SPFile>();
            string siteUrl = SPContext.Current.Web.Url;
            using (SPSite spSite = new SPSite(siteUrl))
            {
                isDeleted = DosyaSil(spSite, libName, tesekkurDosyaAdi);
                isDeleted = DosyaSil(spSite, libName, etiketDosyaAdi);
            }
            return isDeleted;
        }
        private bool DosyaSil(SPSite spSite, string libName, string fileName)
        {
            bool isDeleted = false;
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
            return isDeleted;
        }
        private List<SPFile> DosyaListesiniGetir(string libName, string fileName)
        {
            List<SPFile> lstFile = new List<SPFile>();
            SPWeb web = SPContext.Current.Web;
            SPList list = web.Lists[libName];
            SPFolder folder = list.RootFolder;

            var matchedFiles = folder.Files.Cast<SPFile>()
                .Where(f => f.Name.Contains(fileName))
                .OrderByDescending(f => f.TimeLastModified);

            lstFile.AddRange(matchedFiles);
            return lstFile;
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
            EtiketDosyaAdiLbl.Text = paramEtiketDosyaAdiLbl.Value;
            DosyayiSilNowBtn.Visible = true;
            var openPopup = "OpenModalOnay();";
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), openPopup, true);
        }
        protected void DosyayiSilNowBtn_Click(object sender, EventArgs e)
        {
            DosyalariSPListesindenSil(LibraryNameQS, paramDosyaAdiLbl.Value, paramEtiketDosyaAdiLbl.Value);
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            Page.Response.Redirect(currentUrl);
        }
    }
}
