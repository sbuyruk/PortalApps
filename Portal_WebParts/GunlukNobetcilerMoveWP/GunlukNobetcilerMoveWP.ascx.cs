using Portal_WebParts.Logging;
using Utility.Logging;
using Microsoft.SharePoint;
using System;
using System.ComponentModel;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.ProjeGlobal;

namespace Portal_WebParts.GunlukNobetcilerMoveWP
{
    [ToolboxItemAttribute(false)]
    public partial class GunlukNobetcilerMoveWP : WebPart
    {
        private PortalLogContext logContext;
        private PortalLogContext LogContext { get { return logContext ?? (logContext = PortalLogContext.FromCurrentRequest("Portal", "GunlukNobetcilerMoveWP")); } }
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public GunlukNobetcilerMoveWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            PortalWebPartLog.ExecuteAction(LogContext, "Page_Load", () =>
            {
                if (!Page.IsPostBack) PortalWebPartLog.RecordOpened(LogContext);
                TitelLbl.Text = "Bugün Görevli Personel";
                ulUsers.Visible = false;
                divEmptyUsers.Visible = false;
                var rows = GunlukNobetcileriGetir();
                NobetciRepeater.DataSource = rows;
                NobetciRepeater.DataBind();
                PopupRepeater.DataSource = rows;
                PopupRepeater.DataBind();
                ulUsers.Visible = rows.Rows.Count > 0;
                divEmptyUsers.Visible = rows.Rows.Count == 0;
            });
        }
        private DataTable GunlukNobetcileriGetir()
        {
            var table = new DataTable();
            foreach (string column in new[] { "Bolum", "Isim", "KullaniciAdi", "PersonelResimleri" }) table.Columns.Add(column, typeof(string));
            using (var site = new SPSite(SPContext.Current.Site.Url))
            using (var web = site.OpenWeb())
            {
                var list = web.Lists["Günlük Görevli Personel"];
                var query = new SPQuery { RowLimit = 1, ExpandUserField = true,
                    Query = "<Where><Eq><FieldRef Name='G_x00f6_rev_x0020_Tarihi' /><Value Type='DateTime'><Today /></Value></Eq></Where><OrderBy><FieldRef Name='Created' /></OrderBy>" };
                var items = list.GetItems(query);
                if (items.Count == 0) return table;
                var fields = new[] {
                    "Görevli Personel Mali Isl. ve Ist. Grp. Bsk. ligi (18:00-19:00)",
                    "Görevli Personel Vakif Hiz. Grp. Bsk. ligi (18:00-19:00)",
                    "Görevli Personel Per. ve Id. Isl. S. Md.lügü (18:00-19:00)",
                    "Hizmetli Personel (18:00-19:00)" };
                var labels = new[] { "Mali İşl. ve İst. Grp. Bşk. lığı", "Vakıf Hiz. Grp. Bşk. lığı", "İns. Kay. ve İd. İşl. Ş. Md.lüğü", "Hizmetli Personel" };
                for (int i = 0; i < fields.Length; i++)
                {
                    object value = items[0][fields[i]];
                    if (value == null) continue;
                    var user = new SPFieldUserValue(web, Convert.ToString(value));
                    string login = user.LoginName ?? string.Empty;
                    string account = login.Substring(login.LastIndexOf('\\') + 1);
                    table.Rows.Add(labels[i], user.LookupValue, Uri.EscapeDataString(account), ProjeConstants.PATH_RESIMLER_PERSONEL);
                }
            }
            return table;
        }
    }
}
