using Model.MTS;
using Model.NBYS;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace MTS_WebParts.BagisciListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class BagisciListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public BagisciListesiWP()
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

        private void TabloOlustur()
        {
            List<Kisi> list = new List<Kisi>();
            var jsonData = TabloJson(); //veri çekilip json a çeviriliyor
            //var jsString = CreateDataTable(jsonData); //javascript kodu hazırlanıyor.
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler,
                typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "setDataSet(" + jsonData + ");", true);
        }
        private string TabloJson()
        {
            string jSon = string.Empty;

            try
            {
                List<BagisciListItem> list = GetModalDataList();
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
        private List<BagisciListItem> GetModalDataList()
        {
            NakitBagisci nakitBagisci = new NakitBagisci();
            DataTable dataTableNakit = nakitBagisci.SelectSecilmemisKatilimcilarByFaaliyetIdReturnDT(ProjeConstants.HEPSI_INT);
            TasinmazBagisci tasinmazBagisci = new TasinmazBagisci();
            DataTable dataTableTasinmaz = tasinmazBagisci.SelectSecilmemisKatilimcilarByFaaliyetIdReturnDT(ProjeConstants.HEPSI_INT);

            dataTableTasinmaz.Merge(dataTableNakit);
            int SiraNo = 1;
            List<BagisciListItem> list = new List<BagisciListItem>();
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            if (dataTableTasinmaz != null)
            {
                foreach (DataRow row in dataTableTasinmaz.Rows)
                {
                    string katilimciId = row["KatilimciId"].ToString();
                    int katilimciTipi = row["KatilimciTipi"].ConvertToInt();
                    string adi = row["Adi"].ToString();
                    string soyadi = row["Soyadi"].ToString();
                    string telefon = row["Telefon"].ToString();
                    string adres = row["Adres"].ToString();
                    string il = row["Il"].ToString();
                    string ilce = row["Ilce"].ToString();

                    BagisciListItem katilimciItem = new BagisciListItem();
                    katilimciItem.Sirano = SiraNo++.ToString();
                    katilimciItem.KatilimciId = katilimciId;
                    katilimciItem.AdiSoyadi = (adi + " " + soyadi.Trim());
                    katilimciItem.Adi = adi;
                    katilimciItem.Soyadi = soyadi;
                    katilimciItem.Telefon = telefon;
                    katilimciItem.Adres = adres;
                    katilimciItem.Il = il;
                    katilimciItem.Ilce = ilce;
                    katilimciItem.KatilimciTipiStr = KatilimciTipiGetir(katilimciTipi);
                    if (katilimciTipi == ProjeConstants.FAALIYET_KATILIMCI_TASINMAZBAGISCI_INT)
                    {
                        katilimciItem.BagisciKarti= "<a  target='_blank' href=" + ProjeConstants.PAGE_TASINMAZBAGISCI_KARTI + "?BagisciId=" + katilimciId + " class='btn btn-outline-secondary'>Bağışçı Kartı</a>";
                    }
                    katilimciItem.KatilimciTipi = katilimciTipi.ToString();

                    katilimciItem.KisiKarti = "<a  target='_blank' href=" + ProjeConstants.PAGE_KISI_KARTI + "?KatilimciId=" + katilimciId + "&KatilimciTipi=" + katilimciTipi + " class='btn btn-outline-info'>Kişi Kartı</a>";

                    list.Add(katilimciItem);
                }
            }
            return list;
        }
        private string KatilimciTipiGetir(int katilimciTipi)
        {
            string katilimciTipStr = ProjeConstants.FAALIYET_KATILIMCI_DIS;
            switch (katilimciTipi)
            {
                case ProjeConstants.FAALIYET_KATILIMCI_IC_INT:
                    {
                        katilimciTipStr = ProjeConstants.FAALIYET_KATILIMCI_IC;
                        break;
                    }
                case ProjeConstants.FAALIYET_KATILIMCI_DIS_INT:
                    {
                        katilimciTipStr = ProjeConstants.FAALIYET_KATILIMCI_DIS;
                        break;
                    }
                case ProjeConstants.FAALIYET_KATILIMCI_NAKITBAGISCI_INT:
                    {
                        katilimciTipStr = ProjeConstants.FAALIYET_KATILIMCI_NAKITBAGISCI;
                        break;
                    }
                case ProjeConstants.FAALIYET_KATILIMCI_TASINMAZBAGISCI_INT:
                    {
                        katilimciTipStr = ProjeConstants.FAALIYET_KATILIMCI_TASINMAZBAGISCI;
                        break;
                    }
                default:
                    break;
            }
            return katilimciTipStr;
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
        protected void RandevuTakvimiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_FAALIYET_TAKVIM);
        }
        private class BagisciListItem
        {
            public string Sirano { get; set; }
            public string KatilimciId { get; set; }
            public string KatilimciTipi { get; set; }
            public string KatilimciTipiStr { get; set; }
            public string AdiSoyadi { get; set; }
            public string Adi { get; set; }
            public string Soyadi { get; set; }
            public string Telefon { get; set; }
            public string Adres { get; set; }
            public string Il { get; set; }
            public string Ilce { get; set; }
            public string BagisciKarti { get; set; }

            public string KisiKarti { get; set; }

        }
    }
}
