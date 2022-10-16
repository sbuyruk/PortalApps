using Microsoft.SharePoint;
using System;
using System.ComponentModel;
using System.Data;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;

namespace Portal_WebParts.DuyuruWP
{
    [ToolboxItemAttribute(false)]
    public partial class DuyuruWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public DuyuruWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        #region Private Variables

        protected SPWeb _rootWeb;
        protected SPList _duyuruListesi;
        protected string _listeAdi = "Duyurular";
        protected string _duyuruBasligi = "Duyurular";
        protected int _yeniSuresi = 10;

        protected string _sorguCumlesiGunlukDuyurular =
            "<Where>" +
               "<And>" +
                  "<Or>" +
                      "<Geq>" +
                          "<FieldRef Name=\"Expires\" />" +
                          "<Value Type=\"DateTime\">{0}</Value>" +
                      "</Geq>" +
                      "<IsNull>" +
                          "<FieldRef Name=\"Expires\" />" +
                      "</IsNull>" +
                  "</Or>" +
                  "<Eq><FieldRef Name=\"TekrarliMi\" />" +
                      "<Value Type=\"Boolean\">0</Value>" +
                  "</Eq>" +
               "</And>" +
            "</Where>" +
            "<OrderBy>" +
               "<FieldRef Name=\"Created\" Ascending=\"False\"/>" +
            "</OrderBy>";

        protected string _sorguCumlesiGenelDuyurular =
            "<Where>" +
              "<And>" +
                "<Eq>" +
                   "<FieldRef Name=\"TekrarliMi\" />" +
                   "<Value Type=\"Boolean\">1</Value>" +
                "</Eq>" +
                "<IsNotNull>" +
                   "<FieldRef Name=\"Expires\" />" +
                "</IsNotNull>" +
              "</And>" +
            "</Where>" +
            "<OrderBy>" +
            "<FieldRef Name=\"Created\" Ascending=\"False\"/>" +
            "</OrderBy>";



        #endregion

        #region Public Variables

        public string DuyuruBasligi
        {
            get
            {
                return "Duyuru";
            }
        }

        public string ListeAdi
        {
            get
            {
                return "Duyurular";
            }
        }

        public int YeniSuresi
        {
            get
            {
                return 2;
            }

        }

        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                DuyuruListesiniDoldur();
            }
            catch (Exception ex)
            {
                ExceptionHelper eh = new ExceptionHelper(ex);
                eh.PublishException();
            }
            if (true)
            {
                //var openPopup = "OpenDuyuruModal(false);";
                //System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), openPopup, true);
            }
        }

        private void DuyuruListesiniDoldur()
        {
            try
            {
                _listeAdi = ListeAdi;
                _duyuruBasligi = DuyuruBasligi;
                _yeniSuresi = Convert.ToInt32(YeniSuresi);
                _rootWeb = Microsoft.SharePoint.SPContext.Current.Web;
            }
            catch
            {
                this.EklePanel.Visible = false;
                this.DuyuruRepeater.Visible = false;
                this.lblHata.Text = YeniSuresi + " " + ListeAdi + " " + DuyuruBasligi;
                this.lblHata.Visible = true;
            }

            try
            {
                _duyuruListesi = _rootWeb.Lists[_listeAdi];
                if (_duyuruListesi != null)
                {
                    this.DuyuruRepeater.DataSource = getData();
                    this.DuyuruRepeater.DataBind();

                    _rootWeb.Site.CatchAccessDeniedException = false;
                    lblTitle.Text = _duyuruBasligi;

                    try
                    {
                        // eger kullaninin duyuru listesine duyuru ekleme hakki varsa hyperlink gorunur oluyor
                        if (_duyuruListesi.DoesUserHavePermissions(SPBasePermissions.AddListItems))
                        {
                            this.YeniHyperLink.NavigateUrl = _duyuruListesi.RootFolder.ServerRelativeUrl + "/NewForm.aspx";
                            this.EklePanel.Visible = true;
                        }
                        else
                        {
                            this.EklePanel.Visible = false;
                        }
                    }
                    catch (System.UnauthorizedAccessException)
                    {
                        this.EklePanel.Visible = false;
                    }

                    _rootWeb.Site.CatchAccessDeniedException = true;
                }
                else
                {
                    this.EklePanel.Visible = false;
                    this.DuyuruRepeater.Visible = false;
                    this.lblHata.Text = ListeAdi + " adındaki liste bulunamamıştır.Lütfen Liste adını kontrol ediniz.";
                    this.lblHata.Visible = true;
                }
            }
            catch (Exception )
            {
                this.EklePanel.Visible = false;
                this.DuyuruRepeater.Visible = false;
                this.lblHata.Text = ListeAdi + " adındaki liste bulunamamıştır.Lütfen Liste adını kontrol ediniz2.";
                this.lblHata.Visible = true;
            }

        }


        protected DataTable getData()
        {
            DataTable tablo = new DataTable();
            tablo.Columns.Add(new DataColumn("Subject"));
            tablo.Columns.Add(new DataColumn("Link"));
            tablo.Columns.Add(new DataColumn("Image"));

            DateTime simdi = DateTime.Now;
            DateTime bugun = DateTime.Today;
            string gunDegeri = bugun.ToString("yyyy-MM-dd");

            SPListItemCollection duyurular = null;
            SPQuery sorgu = null;
            long beklenenFark = TimeSpan.FromDays(_yeniSuresi).Ticks;
            DateTime olusturulmaTarihi;
            string itemURL = String.Empty;
            string itemTitle = String.Empty;
            string imageURL = String.Empty;

            #region Genel Duyurular

            // Genel Duyurulari Tabloya Ekle
            sorgu = new SPQuery();
            sorgu.Query = _sorguCumlesiGenelDuyurular;

            try
            {
                duyurular = _duyuruListesi.GetItems(sorgu);
            }
            catch
            {
                duyurular = null;
            }

            if (duyurular != null)
            {
                int duyuruGunu = 0;
                int duyuruAyi = 0;
                int gununGunu = bugun.Day;
                int gununAyi = bugun.Month;
                int gununYili = bugun.Year;
                DateTime duyuruTarihi;

                for (int i = 0; i < duyurular.Count; i++)
                {
                    duyuruTarihi = (DateTime)duyurular[i]["Expires"];
                    duyuruGunu = duyuruTarihi.Day;
                    duyuruAyi = duyuruTarihi.Month;

                    if (duyuruGunu == gununGunu && duyuruAyi == gununAyi)
                    {
                        itemURL = _duyuruListesi.RootFolder.ServerRelativeUrl + "/DispForm.aspx?ID=" + duyurular[i].ID;
                        olusturulmaTarihi = new DateTime(gununYili, gununAyi, gununGunu);
                        itemTitle = "(" + olusturulmaTarihi.ToString("dd.MM.yyyy") + ") " + duyurular[i]["Title"].ToString();
                        imageURL = "/_layouts/1033/images/new.gif";
                        tablo.Rows.Add(itemTitle, itemURL, imageURL);
                    }
                }
            }
            #endregion

            #region Gunluk Duyurular

            // Gunluk Duyurulari Tabloya Ekle
            sorgu = new SPQuery();
            sorgu.Query = String.Format(_sorguCumlesiGunlukDuyurular, gunDegeri);

            try
            {
                duyurular = _duyuruListesi.GetItems(sorgu);
            }
            catch
            {
                duyurular = null;
            }

            if (duyurular != null)
            {
                for (int i = 0; i < duyurular.Count; i++)
                {
                    itemURL = _duyuruListesi.RootFolder.ServerRelativeUrl + "/DispForm.aspx?ID=" + duyurular[i].ID;
                    olusturulmaTarihi = (DateTime)duyurular[i]["Created"];
                    itemTitle = "(" + olusturulmaTarihi.ToString("dd.MM.yyyy") + ") " + duyurular[i]["Title"].ToString();
                    if ((simdi.Ticks - olusturulmaTarihi.Ticks) < beklenenFark)
                    {

                        imageURL = "/_layouts/1033/images/new.gif";
                    }
                    else
                    {
                        imageURL = "/_layouts/images/blank.gif";
                    }

                    tablo.Rows.Add(itemTitle, itemURL, imageURL);
                }
            }
            #endregion

            return tablo;
        }



        public class Duyuru
        {
            public string Baslik { get; set; }
            public string Metin { get; set; }
            public string SureSonu { get; set; }
            public string TekrarliMi { get; set; }
            public string PopupGostesinMi { get; set; }
            public string ResimPath { get; set; }
        }
    }
}
