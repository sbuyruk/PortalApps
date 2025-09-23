using Microsoft.SharePoint.JsonUtilities;
using Model.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace IKYS_WebParts.GorevOnayListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class GorevOnayListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public GorevOnayListesiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string SecilenIdQS
        {
            get
            {

                if (ViewState["SecilenId"] == null)
                {
                    if (Page.Request.QueryString["SecilenId"] != null)
                    {
                        ViewState["SecilenId"] = Page.Request.QueryString["SecilenId"];
                    }
                    else
                    {
                        ViewState["SecilenId"] = string.Empty;
                    }
                }
                return ViewState["SecilenId"].ToString();
            }

            set
            {
                ViewState["SecilenId"] = value;
            }
        }
        private string AuthQS
        {
            get
            {

                if (ViewState["Auth"] == null || string.IsNullOrEmpty(ViewState["Auth"].ToString()))
                {
                    if (Page.Request.QueryString["Auth"] != null)
                    {
                        ViewState["Auth"] = Page.Request.QueryString["Auth"];
                    }
                    else
                    {
                        ViewState["Auth"] = string.Empty;
                    }
                }
                return ViewState["Auth"].ToString();
            }

            set
            {
                ViewState["Auth"] = value;
            }
        }
        private string CurrentUserName
        {
            get
            {

                if (ViewState["CurrentUserName"] == null)
                {
                    ViewState["CurrentUserName"] = UtilityHelper.GetCurrentUserLoginName();
                }
                return ViewState["CurrentUserName"].ToString();
            }

            set
            {
                ViewState["CurrentUserName"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    SecildiFalseYap();
                    TabloOlustur();
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }

        /// <summary>
        /// GorevOnay.Secildi kolonu sadece toplu Rapor Alımı için kullanılır
        /// SSRS Select ... IN (parametre) şeklindeki sorgularda "," ile ayrılmış parametreleri desteklemediği için tabloya Secildi alanı eklendi
        /// Bu nedenle her açılışta tüm secildi kolonları false yapılır
        /// </summary>
        private void SecildiFalseYap()
        {
            try
            {
                GorevOnay gorevOnay = new GorevOnay();
                gorevOnay.UpdateAllSecildiToFalse();
            }
            catch (Exception ex)
            {

                ExceptionHelper exceptionHelper = new ExceptionHelper();
                exceptionHelper.Exceptions.Add(ex);
            }
        }

        private void TabloOlustur()
        {
            var jsonData = TabloJson(); //veri çekilip json a çeviriliyor
            //var jsString = CreateDataTable(jsonData); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir("setDataSet(" + jsonData + ");");
        }
        private string TabloJson()
        {
            string jSon = string.Empty;

            try
            {
                List<GorevOnayListItem> list = GetDataList();
                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;
                jSon = serializer.Serialize(list);
            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper();
                exceptionHelper.Exceptions.Add(exception);
                exceptionHelper.PublishException();
            }
            return jSon;
        }
        private List<GorevOnayListItem> GetDataList()
        {
            string gorevOnayIdStr = string.Empty;
            GorevOnay gorevOnay = new GorevOnay();
            DataTable dataTable = gorevOnay.SelectAllReturnDT(0);

            List<GorevOnayListItem> list = new List<GorevOnayListItem>();
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            Personel personel = IKYSOrtak.PersonelGetir(CurrentUserName);
            DataView dataView = new DataView(dataTable);
            foreach (DataRowView row in dataView)
            {
                string gorevOnayId = row["GorevOnayId"].ToString();
                string secildi = row["Secildi"].ReturnEmptyIfNull().ToString().ToUpper().Equals("TRUE")?"checked":string.Empty;
                string adiSoyadi = row["AdiSoyadi"].ToString();
                string gorevinSebebi = row["GorevinSebebi"].ToString();
                string baslangicTarihi = row["BaslangicTarihi"].ReturnEmptyIfNull().ConvertToDatetime().ToString("dd.MM.yyyy HH:mm");
                string bitisTarihi = row["BitisTarihi"].ReturnEmptyIfNull().ConvertToDatetime().ToString("dd.MM.yyyy HH:mm");
                string gorevinYeri = row["GorevinYeri"].ToString();
                int personelId = row["PersonelId"].ConvertToInt();

                GorevOnayListItem gorevOnayListItem = new GorevOnayListItem();
                gorevOnayListItem.GorevOnayId = gorevOnayId;
                gorevOnayListItem.SecChk = "<input type=checkbox id=chkBox" + gorevOnayId + " name=chkBox" + gorevOnayId + " "+ secildi + " onclick='AddRemoveSecimListesi(" + gorevOnayId + ",this);' />";
                gorevOnayListItem.AdiSoyadi = adiSoyadi;
                gorevOnayListItem.GorevinSebebi = gorevinSebebi ;
                gorevOnayListItem.BaslangicTarihi = baslangicTarihi;
                gorevOnayListItem.BitisTarihi = bitisTarihi;
                gorevOnayListItem.GorevinYeri = gorevinYeri;
                gorevOnayListItem.Secildi = SecilenIdQS.Equals(gorevOnayId);
                gorevOnayListItem.BaslangicTarihiHidden = row["BaslangicTarihi"].ConvertToDatetime();

                if (AuthQS.Equals(ProjeConstants.IKYS_YETKILI_BIRIM))
                {
                    gorevOnayListItem.RaporAl = "<a href=" + ProjeConstants.RAPOR_GOREVONAYBELGESI_URL + "?Auth="+AuthQS+"&GorevOnayId=" + gorevOnayId + " class='btn btn-outline-primary'>Rapor Al</a>";
                    gorevOnayListItem.Duzenle = "<a href=" + ProjeConstants.PAGE_GOREVONAY_GIRIS + "?Auth="+AuthQS+"&GorevOnayId=" + gorevOnayId + " class='btn btn-outline-primary'>Düzenle</a>";
                    list.Add(gorevOnayListItem);
                }else if (personel.Id == personelId)
                {
                    DateTime today = DateTime.Today;
                    int fark = (today - baslangicTarihi.ConvertToDatetime()).Days;
                    if (!string.IsNullOrEmpty(baslangicTarihi) && fark<4)
                    {
                        gorevOnayListItem.RaporAl = "<a href=" + ProjeConstants.RAPOR_GOREVONAYBELGESI_URL + "?GorevOnayId=" + gorevOnayId + " class='btn btn-outline-primary'>Rapor Al</a>";
                        gorevOnayListItem.Duzenle = "<a href=" + ProjeConstants.PAGE_GOREVONAY_GIRIS + "?GorevOnayId=" + gorevOnayId + " class='btn btn-outline-primary'>Düzenle</a>"; 
                    }
                    list.Add(gorevOnayListItem);
                }


            }
            return list;
        }

        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        protected void RaporAlBtn_Click(object sender, EventArgs e)
        {
            SecilenleriKaydet();
            if (SecilenGorevVarMi())
            {
                RedirectToPage(ProjeConstants.RAPOR_GOREVONAYTOPLUBELGESI_URL);
            }
            else
            {
                MessageHelper.PublishMessage("Toplu rapor almak için görevleri seçmeniz gerekli, henüz hiç görev seçmediniz",ProjeConstants.MESAJ_HATA);
            }
            
        }

        private void SecilenleriKaydet()
        {
            SecildiFalseYap();
            string idler = paramidArray.Value.Equals(",")?string.Empty: paramidArray.Value;
            if (idler.Length > 1)
            {
                idler = idler.IndexOf(",") == 0 ? idler.Substring(1, idler.Length - 1) : idler;
            }
            
            
            GorevOnay gorevOnay = new GorevOnay();
            bool secilenlerKaydedildi = gorevOnay.UpdateAllSecildiToTrue(idler);
        }

        private bool SecilenGorevVarMi()
        {
            GorevOnay gorevOnay = new GorevOnay();
            List<GorevOnay> liste = gorevOnay.SelectAllBySecildi(true);
            return liste.Count > 0;
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
        private class GorevOnayListItem
        {
            public DateTime BaslangicTarihiHidden { get; set; }
            public string GorevOnayId { get; set; }
            public string SecChk { get; set; }
            public string AdiSoyadi { get; set; }
            public string GorevinYeri { get; set; }
            public string BaslangicTarihi { get; set; }
            public string BitisTarihi { get; set; }
            public string GorevinSebebi { get; set; }
            public string RaporAl { get; set; }
            public string Duzenle { get; set; }
            public bool Secildi { get; set; }

        }
        protected void SecilenleriKaydetBtn_Click(object sender, EventArgs e)
        {
            string aa = paramidArray.Value;
        }

        protected void YeniGorevOnayiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_GOREVONAY_GIRIS);
        }
    }
}
