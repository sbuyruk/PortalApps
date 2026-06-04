using Model.MTS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace MTS_WebParts.FaaliyetListesiROWP
{
    [ToolboxItemAttribute(false)]
    public partial class FaaliyetListesiROWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public FaaliyetListesiROWP()
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
            try
            {
                TabloOlustur();
            }
            catch (Exception ex)
            {
                MessageHelper.PublishMessage(ex.Message, ProjeConstants.MESAJ_HATA);
            }
        }
        private void TabloOlustur()
        {
            var jsonData = TabloJson(); //veri çekilip json a çeviriliyor
            UtilityHelper.ScriptCalistir("setDataSet(" + jsonData + ");");
        }
        private string TabloJson()
        {
            string jSon = string.Empty;

            try
            {
                List<FaaliyetListItem> list = GetDataList();
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
        private List<FaaliyetListItem> GetDataList()
        {
            List<FaaliyetListItem> faaliyetList = new List<FaaliyetListItem>();
            Faaliyet faaliyetDao = new Faaliyet();
            DataTable dataTable = faaliyetDao.SelectAllByKatilimciFaaliyetReturnDataTable(ProjeConstants.HEPSI_INT, -3, ProjeConstants.HEPSI, ProjeConstants.NULL_TARIH, ProjeConstants.NULL_TARIH, ProjeConstants.HEPSI);

            if (dataTable != null)
            {
                int tempFaaliyetId = 0;
                int katilimciAdedi = 0;

                FaaliyetListItem tempFaaliyetListItem = new FaaliyetListItem();
                foreach (DataRow row in dataTable.Rows)
                {
                    int faaliyetId = row["FaaliyetId"].ConvertToInt();
                    string faaliyetTipi = row["FaaliyetTipi"].ToString();
                    string faaliyetYeri = row["FaaliyetYeri"].ToString(); ;
                    string faaliyetKonusu = row["FaaliyetKonusu"].ToString();
                    string faaliyetAmaci = row["FaaliyetAmaciId"].ToString();
                    string faaliyetDurumu = row["FaaliyetDurumu"].ToString();

                    DateTime basTar = row["BaslangicTarihi"].ConvertToDatetime();
                    DateTime bitTar = row["BitisTarihi"].ConvertToDatetime();

                    string baslangicTarihi = basTar.ToString("dd.MM.yyyy HH:mm");
                    string bitisTarihi = bitTar.ToString("dd.MM.yyyy HH:mm");

                    string faaliyetAmaciStr = MTSOrtak.ParseFaaliyetAmaci(faaliyetAmaci);
                    string faaliyetDurumuStr = MTSOrtak.ParseFaaliyetDurumu(faaliyetDurumu.ConvertToInt());
                    string katilimci = row["Adi"].ToString() + " " + row["Soyadi"].ToString();

                    if (tempFaaliyetId == faaliyetId)
                    {
                        tempFaaliyetId = faaliyetId;
                        faaliyetList.Remove(tempFaaliyetListItem);
                        katilimciAdedi++;
                        tempFaaliyetListItem.Katilimci += katilimci + "</br> ";
                        faaliyetList.Add(tempFaaliyetListItem);
                    }
                    if (tempFaaliyetId != faaliyetId)
                    {
                        FaaliyetListItem faaliyetListItem = new FaaliyetListItem();
                        faaliyetListItem.FaaliyetId = faaliyetId;
                        faaliyetListItem.FaaliyetTipi = faaliyetTipi;
                        faaliyetListItem.FaaliyetYeri = faaliyetYeri;
                        faaliyetListItem.FaaliyetKonusu = faaliyetKonusu;
                        faaliyetListItem.BaslangicTarihi = baslangicTarihi;
                        faaliyetListItem.BitisTarihi = bitisTarihi;
                        faaliyetListItem.FaaliyetAmaci = faaliyetAmaciStr;
                        faaliyetListItem.FaaliyetDurumu = faaliyetDurumuStr;

                        faaliyetListItem.Katilimci += katilimci + "</br ";
                        tempFaaliyetListItem = faaliyetListItem;
                        faaliyetList.Add(tempFaaliyetListItem);
                    }
                    tempFaaliyetId = faaliyetId;

                }
            }
            //faaliyetList = faaliyetList.OrderBy(r => r.FaaliyetTarihi).ToList();
            return faaliyetList;
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

        protected void FaaliyetViewerROBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_FAALIYET_TAKVIM_RO);
        }
        private class FaaliyetListItem
        {
            public int FaaliyetId { get; set; }
            public string FaaliyetTipi { get; set; }
            public string FaaliyetYeri { get; set; }
            public string FaaliyetKonusu { get; set; }
            public string FaaliyetAmaci { get; set; }
            public string FaaliyetDurumu { get; set; }
            public string BaslangicTarihi { get; set; }
            public string BitisTarihi { get; set; }
            public string Katilimci { get; set; }
        }
    }
}