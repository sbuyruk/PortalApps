using Portal_WebParts.Logging;
using Utility.Logging;
using Microsoft.SharePoint;
using System;
using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;

namespace Portal_WebParts.GununSozuWP
{
    [ToolboxItemAttribute(false)]
    public partial class GununSozuWP : WebPart
    {
        private PortalLogContext logContext;
        private PortalLogContext LogContext { get { return logContext ?? (logContext = PortalLogContext.FromCurrentRequest("Portal", "GununSozuWP")); } }
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public GununSozuWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            PortalWebPartLog.ExecuteAction(LogContext, "Page_Load", () =>
            {
                if (!Page.IsPostBack) PortalWebPartLog.RecordOpened(LogContext);
                lblSubject.Text = string.Empty;
                pnlAdmin.Visible = false;

                using (SPSite site = new SPSite(SPContext.Current.Site.ID))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        string listname = "Günün Sözü";
                        SPList list = web.Lists.TryGetList(listname);
                        if (list == null) throw new InvalidOperationException("Günün Sözü listesi bulunamadı.");
                        SPQuery q = new SPQuery();
                        q.Query = "<OrderBy><FieldRef Name=\"Created\" Ascending=\"False\"/></OrderBy>";


                        SPListItemCollection items = list.GetItems(q);

                        if (items.Count > 0)
                        {
                            foreach (SPListItem item in items)
                            {
                                DateTime date = Convert.ToDateTime(item["Tarih"]);
                                if (date.Month == DateTime.Now.Month && date.Day == DateTime.Now.Day)
                                {
                                    lblSubject.Text = System.Web.HttpUtility.HtmlEncode(item.Title);
                                    break;
                                }
                            }

                        }
                        if (string.IsNullOrEmpty(lblSubject.Text)) lblSubject.Text = "Bugün için söz tanımlanmamış.";
                        try
                        {
                            // eger kullaninin duyuru listesine duyuru ekleme hakki varsa hyperlink gorunur oluyor
                            if (list.DoesUserHavePermissions(SPBasePermissions.AddListItems))
                            {
                                lnkDuyuruEkle.NavigateUrl = list.RootFolder.ServerRelativeUrl + "/NewForm.aspx";
                                pnlAdmin.Visible = true;
                            }
                            else
                            {
                                pnlAdmin.Visible = false;
                            }
                        }
                        catch (System.UnauthorizedAccessException)
                        {
                            pnlAdmin.Visible = false;
                        }

                        site.CatchAccessDeniedException = true;
                    }
                }
            });
        }
    }
}
