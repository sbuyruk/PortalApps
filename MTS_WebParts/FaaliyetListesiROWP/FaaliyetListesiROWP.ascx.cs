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
                List<RandevuListItem> list = GetDataList();
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
        private List<RandevuListItem> GetDataList()
        {
            List<RandevuListItem> randevuList = new List<RandevuListItem>();
            Randevu randevuDao = new Randevu();
            DataTable dataTable = randevuDao.SelectAllByKatilimciRandevuReturnDataTable(ProjeConstants.HEPSI_INT, -3);

            if (dataTable != null)
            {
                int tempRandevuId = 0;
                int katilimciAdedi = 0;

                RandevuListItem tempRandevuListItem = new RandevuListItem();
                foreach (DataRow row in dataTable.Rows)
                {
                    int randevuId = row["RandevuId"].ConvertToInt();
                    string randevuTipi = row["RandevuTipi"].ToString();
                    string randevuYeri = row["RandevuYeri"].ToString(); ;
                    string randevuKonusu = row["RandevuKonusu"].ToString();
                    string randevuAmaci = row["RandevuAmaci"].ToString();
                    string randevuDurumu = row["RandevuDurumu"].ToString();

                    DateTime basTar = row["BaslangicTarihi"].ConvertToDatetime();
                    DateTime bitTar = row["BitisTarihi"].ConvertToDatetime();

                    string baslangicTarihi = basTar.ToString("dd.MM.yyyy HH:mm");
                    string bitisTarihi = bitTar.ToString("dd.MM.yyyy HH:mm");

                    string randevuAmaciStr = ParseRandevuAmaci(randevuAmaci);
                    string randevuDurumuStr = ParseRandevuDurumu(randevuDurumu.ConvertToInt());
                    string katilimci = row["Adi"].ToString() + " " + row["Soyadi"].ToString();

                    if (tempRandevuId == randevuId)
                    {
                        tempRandevuId = randevuId;
                        randevuList.Remove(tempRandevuListItem);
                        katilimciAdedi++;
                        tempRandevuListItem.Katilimci += katilimci + "</br> ";
                        randevuList.Add(tempRandevuListItem);
                    }
                    if (tempRandevuId != randevuId)
                    {
                        RandevuListItem randevuListItem = new RandevuListItem();
                        randevuListItem.RandevuId = randevuId;
                        randevuListItem.RandevuTipi = randevuTipi;
                        randevuListItem.RandevuYeri = randevuYeri;
                        randevuListItem.RandevuKonusu = randevuKonusu;
                        randevuListItem.BaslangicTarihi = baslangicTarihi;
                        randevuListItem.BitisTarihi = bitisTarihi;
                        randevuListItem.RandevuAmaci = randevuAmaciStr;
                        randevuListItem.RandevuDurumu = randevuDurumuStr;

                        randevuListItem.Katilimci += katilimci + "</br ";
                        tempRandevuListItem = randevuListItem;
                        randevuList.Add(tempRandevuListItem);
                    }
                    tempRandevuId = randevuId;

                }
            }
            //randevuList = randevuList.OrderBy(r => r.RandevuTarihi).ToList();
            return randevuList;
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
        private string ParseRandevuAmaci(string amac)
        {
            string amacStr = string.Empty;
            switch (amac)
            {
                case ProjeConstants.RANDEVU_AMACI_DAVET_INT:
                    {
                        amacStr = ProjeConstants.RANDEVU_AMACI_DAVET;
                        break;
                    }
                case ProjeConstants.RANDEVU_AMACI_IZIN_INT:
                    {
                        amacStr = ProjeConstants.RANDEVU_AMACI_IZIN;
                        break;
                    }
                case ProjeConstants.RANDEVU_AMACI_OZELCALISMA_INT:
                    {
                        amacStr = ProjeConstants.RANDEVU_AMACI_OZELCALISMA;
                        break;
                    }
                case ProjeConstants.RANDEVU_AMACI_RESMITATIL_INT:
                    {
                        amacStr = ProjeConstants.RANDEVU_AMACI_RESMITATIL;
                        break;
                    }
                case ProjeConstants.RANDEVU_AMACI_TOPLANTI_INT:
                    {
                        amacStr = ProjeConstants.RANDEVU_AMACI_TOPLANTI;
                        break;
                    }
                case ProjeConstants.RANDEVU_AMACI_YILDONUMU_INT:
                    {
                        amacStr = ProjeConstants.RANDEVU_AMACI_YILDONUMU;
                        break;
                    }
                case ProjeConstants.RANDEVU_AMACI_ZIYARET_INT:
                    {
                        amacStr = ProjeConstants.RANDEVU_AMACI_ZIYARET;
                        break;
                    }
                default:
                    break;
            }
            return amacStr;
        }
        private string ParseRandevuDurumu(int durum)
        {
            string durumStr = string.Empty;
            switch (durum)
            {
                case ProjeConstants.RANDEVU_DURUMU_PLANLANDI_INT:
                    {
                        durumStr = ProjeConstants.RANDEVU_DURUMU_PLANLANDI;
                        break;
                    }
                case ProjeConstants.RANDEVU_DURUMU_ONAYLANDI_INT:
                    {
                        durumStr = ProjeConstants.RANDEVU_DURUMU_ONAYLANDI;
                        break;
                    }
                case ProjeConstants.RANDEVU_DURUMU_IPTALEDILDI_INT:
                    {
                        durumStr = ProjeConstants.RANDEVU_DURUMU_IPTALEDILDI;
                        break;
                    }
                default:
                    break;
            }
            return durumStr;
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
        private class RandevuListItem
        {
            public int RandevuId { get; set; }
            public string RandevuTipi { get; set; }
            public string RandevuYeri { get; set; }
            public string RandevuKonusu { get; set; }
            public string RandevuAmaci { get; set; }
            public string RandevuDurumu { get; set; }
            public string BaslangicTarihi { get; set; }
            public string BitisTarihi { get; set; }
            public string Katilimci { get; set; }
        }
    }
}