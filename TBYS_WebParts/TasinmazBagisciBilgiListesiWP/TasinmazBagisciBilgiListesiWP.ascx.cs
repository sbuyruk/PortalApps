using Model.Ortak;
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

namespace TBYS_WebParts.TasinmazBagisciBilgiListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class TasinmazBagisciBilgiListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public TasinmazBagisciBilgiListesiWP()
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
                if (!Page.IsPostBack)
                {
                    SagVefatDDLDoldur();
                    TabloOlustur();
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private void SagVefatDDLDoldur()
        {
            SagVefatDDL.Items.Clear();
            SagVefatDDL.Items.Add(new ListItem(ProjeConstants.BAGISCI_SAG, ProjeConstants.BAGISCI_SAG_INT.ToString()));
            SagVefatDDL.Items.Add(new ListItem(ProjeConstants.BAGISCI_VEFAT, ProjeConstants.BAGISCI_VEFAT_INT.ToString()));
            SagVefatDDL.Items.Add(new ListItem(ProjeConstants.HEPSI, ProjeConstants.HEPSI_INT.ToString()));
        }
        private void TabloOlustur()
        {
            var jsonData = TabloJson(); //veri çekilip json a çeviriliyor
            //var jsString = CreateDataTable(jsonData); //javascript kodu hazirlaniyor.
            UtilityHelper.ScriptCalistir("setDataSet(" + jsonData + ");");
        }
        private string TabloJson()
        {
            string jSon = string.Empty;

            try
            {
                List<BagisciListItem> list = GetDataList();
                var serializer = new JavaScriptSerializer();
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
        private List<BagisciListItem> GetDataList()
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            TasinmazBagisci tasinmazBagisciDao = new TasinmazBagisci();
            List<BagisciListItem> returnList = new List<BagisciListItem>();
            List<TasinmazBagisci> bagisciListesi = tasinmazBagisciDao.SelectAllSagBagiscilar(SagVefatDDL.SelectedItem.Text);
            int SiraNo = 1;


            foreach (TasinmazBagisci tasinmazBagisci in bagisciListesi)
            {
                Bagis bagisdao = new Bagis();
                
                string adres = string.Empty;
                
                BagisciListItem bagisciListItem = new BagisciListItem();
                bagisciListItem.Sirano = SiraNo++.ToString();
                bagisciListItem.BagisciId = tasinmazBagisci.Id.ToString();
                bagisciListItem.Bolge = BolgeGetir(tasinmazBagisci.Ili);
                bagisciListItem.AdiSoyadi = tasinmazBagisci.Adi + " " + tasinmazBagisci.Soyadi;
                bagisciListItem.Meslegi = tasinmazBagisci.Meslegi;
                bagisciListItem.Tahsil = tasinmazBagisci.Tahsil;
                bagisciListItem.SagVefat = tasinmazBagisci.Sag_vefat;
                bagisciListItem.Adres = tasinmazBagisci.Adres;
                bagisciListItem.IlceIl = tasinmazBagisci.Ilcesi + "/" + tasinmazBagisci.Ili;
                bagisciListItem.Telefon = tasinmazBagisci.Telefon1 +" - " + tasinmazBagisci.Telefon2;

                string talepler = string.Empty;
                BagisciTalepleri bagisciTalepleridao = new BagisciTalepleri();
                List<BagisciTalepleri> talepList = bagisciTalepleridao.SelectByBagisciId(tasinmazBagisci.Id);
           
                foreach (BagisciTalepleri talep in talepList)
                {

                    talepler += talep.Aciklama + "</br>" + System.Environment.NewLine ;

                }
                
                bagisciListItem.Talepleri = talepler;
                string bagislari = string.Empty;
                decimal tahminiRayicToplami = 0m;
                DataTable dataTable = bagisdao.SelectByBagisciIdGroupByKullanimSekli(tasinmazBagisci.Id);
                if (dataTable!=null)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        string kullanimSekli = row["KullanimSekli"].ToString();
                        string adet = row["Adet"].ToString();
                        string ili = row["Ili"].ToString();
                        decimal tahminiRayic = row["TahminiRayic"].ConvertToDecimal();

                        {
                            bagislari += adet + " Adet " + kullanimSekli + " Ili:" +ili +"</br>" + System.Environment.NewLine;
                            tahminiRayicToplami += tahminiRayic;
                        }
                    } 
                }
                bagisciListItem.Bagislari = bagislari;
                bagisciListItem.TahminiRayic = tahminiRayicToplami.ToString("N", culturInfo);


                returnList.Add(bagisciListItem);
            }
            return returnList;
        }

        private string BolgeGetir(string ilAdi)
        {
            string bolgeAdi = string.Empty;
            Il il = new Il();
            il = il.SelectByIlAdi(ilAdi);
            if (il != null)
            {
                Bolge bolge = new Bolge();
                bolge = bolge.Select(il.BolgeId);
                bolgeAdi = bolge.KisaAdi;
            }
            return (bolgeAdi);
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
        private class BagisciListItem
        {
            public string Sirano { get; set; }
            public string BagisciId { get; set; }
            public string Bolge { get; set; }
            public string AdiSoyadi { get; set; }
            public string Meslegi { get; set; }
            public string Tahsil { get; set; }
            public string SagVefat { get; set; }
            public string Adres { get; set; }
            public string IlceIl { get; set; }
            public string Telefon { get; set; }
            public string Bagislari { get; set; }
            public string Talepleri { get; set; }
            public string TahminiRayic { get; set; }
        }

        protected void SagVefatDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabloOlustur();
        }
    }
}
