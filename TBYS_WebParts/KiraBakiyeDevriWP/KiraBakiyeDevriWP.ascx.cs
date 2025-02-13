using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.KiraBakiyeDevriWP
{
    [ToolboxItemAttribute(false)]
    public partial class KiraBakiyeDevriWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public KiraBakiyeDevriWP()
        {
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string SenderAppQS
        {
            get
            {

                if (ViewState["SenderApp"] == null)
                {
                    if (Page.Request.QueryString["SenderApp"] != null)
                    {
                        ViewState["SenderApp"] = Page.Request.QueryString["SenderApp"];
                    }
                    else
                    {
                        ViewState["SenderApp"] = string.Empty;
                    }
                }
                return ViewState["SenderApp"].ToString();
            }

            set
            {
                ViewState["SenderApp"] = value;
            }
        }
        private string DestinationAppQS
        {
            get
            {

                if (ViewState["DestinationApp"] == null)
                {
                    if (Page.Request.QueryString["DestinationApp"] != null)
                    {
                        ViewState["DestinationApp"] = Page.Request.QueryString["DestinationApp"];
                    }
                    else
                    {
                        ViewState["DestinationApp"] = string.Empty;
                    }
                }
                return ViewState["DestinationApp"].ToString();
            }

            set
            {
                ViewState["DestinationApp"] = value;
            }
        }
        private string KiraciIdQS
        {
            get
            {

                if (ViewState["KiraciId"] == null)
                {
                    if (Page.Request.QueryString["KiraciId"] != null)
                    {
                        ViewState["KiraciId"] = Page.Request.QueryString["KiraciId"];
                    }
                    else
                    {
                        ViewState["KiraciId"] = string.Empty;
                    }
                }
                return ViewState["KiraciId"].ToString();
            }

            set
            {
                ViewState["KiraciId"] = value;
            }
        }
        private IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Kiraci kiraci = new Kiraci();
                kiraci = kiraci.Select<Kiraci>(KiraciIdQS.ConvertToInt());
                if (kiraci==null)
                {
                    kiraci = new Kiraci();
                    kiraci = kiraci.SelectMin();    
                }
                
                if (kiraci != null)
                {
                    bool isKiraciLoaded = KiraciBilgileriniDoldur(kiraci);
                    EnableOdemeSozlesmeBtns(KiraciIdQS);
                    if (isKiraciLoaded)
                    {
                        OdemeleriHesaplaOdemePlaniniGuncelle(kiraci.Id);
                        SozlesmelerTablosunuDoldur(kiraci.Id);
                    }
                }
                else
                {
                    MessageHelper.PublishMessage("Kiracı bulunamadı!", ProjeConstants.MESAJ_HATA);
                }
            }
        }
        private bool KiraciBilgileriniDoldur(Kiraci kiraci)
        {            
            bool dataFilledToModal = true;
            try
            {
                IdLbl.Visible = true;
                IdLbl.Text = kiraci.Id.ToString();
                PrevBtn.Visible = true;
                NextBtn.Visible = true;

                IdLbl.Text = "" + kiraci.Id;
                AdiLbl.Text = kiraci.Adi;
                TCKimlikNoLbl.Text = kiraci.TCKimlikNo;

                IliLbl.Text = kiraci.Ili;
                IlcesiLbl.Text = kiraci.Ilcesi;
                SorumluBolgeLbl.Text = BolgeGetir(kiraci);
                KiralamaAmaciLbl.Text = kiraci.KiralamaAmaci;
                AdresLbl.Text = kiraci.Adres;
                TelefonLbl.Text = kiraci.Telefon;
            }
            catch (Exception)
            {
                MessageHelper.PublishMessage("Kiracı bulunamadı!", ProjeConstants.MESAJ_HATA);
                dataFilledToModal = false;
            }
            return dataFilledToModal;
        }
        private void EnableOdemeSozlesmeBtns(string kiraciId)
        {
            Kiraci kiraci = new Kiraci();
            kiraci = kiraci.Select<Kiraci>(KiraciIdQS.ConvertToInt());
            if (kiraci != null)//bu kiraci varsa
            {

                KiraSozlesme kiraSozlesme = new KiraSozlesme();
                kiraSozlesme = kiraSozlesme.SelectAktifSozlesmeByKiraciId(kiraci.Id);
                if (kiraSozlesme != null) //bu sozlesme varsa
                {
                    KiraKartiBtn.Visible = true;
                    OdemePlaniGoruntuleBtn.Visible = false;
                    OdemePlaniBtn.Visible = false;
                    OdemePlani odemePlani = new OdemePlani();
                    bool odemePlaniVarMi = odemePlani.OdemePlaniVarMi(kiraSozlesme.Id);
                    if (odemePlaniVarMi)
                    {
                        OdemePlaniGoruntuleBtn.Visible = true;
                        OdemePlaniBtn.Visible = true;
                    }
                }
                else
                {
                    KiraKartiBtn.Visible = false;
                }
            }
        }
        private void SozlesmelerTablosunuDoldur(int kiraciId)
        {
            List<KiraSozlesmeListItem> kiraSozlesmeItemListesi = new List<KiraSozlesmeListItem>();
          
            
            KiraSozlesme kiraSozlesmeDao = new KiraSozlesme();
            List<KiraSozlesme> sozlesmeListesi = kiraSozlesmeDao.SelectByKiraciIdReturnList(kiraciId);
            foreach (var kiraSozlesme in sozlesmeListesi)
            {
                if (kiraSozlesme != null)
                {
                    KiraSozlesmeListItem kiraSozlesmeListItem = new KiraSozlesmeListItem();
                    kiraSozlesmeListItem.DosyaNo = kiraSozlesme.DosyaNo.ToString();
                    kiraSozlesmeListItem.Aktif = kiraSozlesme.Aktif.ToString();
                    kiraSozlesmeListItem.TarihAraligi = kiraSozlesme.SozBasTar.ConvertToDatetimeEmptyIfNull() + "-" + kiraSozlesme.SozBitTar.ConvertToDatetimeEmptyIfNull();
                    kiraSozlesmeListItem.KiraBedeli = kiraSozlesme.KiraBedeli.ToString("N", culturInfo);
                    kiraSozlesmeListItem.SozlesmeId = kiraSozlesme.Id.ToString();
                    decimal sonAnaPara = 0;
                    decimal sonFaizliBakiye = 0;
                    decimal sonFaizTutari = 0;
                    kiraSozlesmeListItem.FarkVarMi = false;
                    KiraSozlesme oncekiKiraSozlesmesi = kiraSozlesme.SelectOncekiKiraSozlesme();
                    if (oncekiKiraSozlesmesi != null)
                    {
                        OdemePlani odemePlaniDao = new OdemePlani();
                        List<OdemePlani> odemePlaniList = odemePlaniDao.SelectBySozlesmeId(oncekiKiraSozlesmesi.Id);
                        if (odemePlaniList.Count>0)
                        {
                            OdemePlani odemePlani = odemePlaniList[odemePlaniList.Count - 1];
                            sonAnaPara = odemePlani.AnaPara;
                            sonFaizliBakiye = odemePlani.FaizliBakiye; 
                        }
                        sonFaizTutari = sonFaizliBakiye - sonAnaPara;
                        kiraSozlesmeListItem.FarkVarMi = (sonAnaPara != kiraSozlesme.DevirAnaPara) || (sonFaizliBakiye != kiraSozlesme.DevirFaizliBakiye);
                    }
                    kiraSozlesmeListItem.DevirAnapara = sonAnaPara.ToString("N", culturInfo) + " (" + kiraSozlesme.DevirAnaPara.ToString("N", culturInfo) + ")";
                    kiraSozlesmeListItem.DevirFaiz = sonFaizTutari.ToString("N", culturInfo) + " (" + kiraSozlesme.DevirFaizTutari.ToString("N", culturInfo) + ")";
                    kiraSozlesmeListItem.DevirFaizliBakiye = sonFaizliBakiye.ToString("N", culturInfo) + " (" + kiraSozlesme.DevirFaizliBakiye.ToString("N", culturInfo) + ")";

                   
                    kiraSozlesmeItemListesi.Add(kiraSozlesmeListItem);
                }
            }


            var serializer = new JavaScriptSerializer();
            var jsonData = serializer.Serialize(kiraSozlesmeItemListesi);

            var jsString = CreateJsString(jsonData); //javascript kodu hazırlanıyor.
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), jsString, true);
        }

        private void OdemeleriHesaplaOdemePlaniniGuncelle(int kiraciId)
        {
            KiraSozlesme kiraSozlesmeDao = new KiraSozlesme();
            List<KiraSozlesme> sozlesmeListesi = kiraSozlesmeDao.SelectByKiraciIdReturnList(kiraciId);
            sozlesmeListesi = sozlesmeListesi.OrderBy(x => x.SozBasTar).ToList();
            foreach (var kiraSozlesme in sozlesmeListesi)
            {
                TBYSOrtak.BakiyeBorcHesapla(kiraSozlesme);
            }
        }

        private string CreateJsString(string jsonData)
        {
            string tablestr = @"
                                var counter=1;   
                                $('#tblfilter').puidatatable({
                                caption: 'Kiracının Sözleşmeleri ',
                                editMode: 'cell',
                                columns: [
                                    { field: 'DosyaNo', headerText: 'D.No', headerStyle:'width: 5%'},
                                    { field: 'TarihAraligi', headerText: 'Sözleşme Tarihi',headerStyle:'width: 15%' },        
                                    { field: 'KiraBedeli', headerText: 'Kira Bedeli',bodyClass:'text-right',headerStyle:'width: 10%' },
                                    { field: 'DevirAnapara', headerText: 'Devir AnaPara',bodyClass:'text-right',headerStyle:'width: 10%' },
                                    { field: 'DevirFaiz', headerText: 'Devir Faiz',bodyClass:'text-right',headerStyle:'width: 10%' },
                                    { field: 'DevirFaizliBakiye', headerText: 'Devir Faizli Bakiye',bodyClass:'text-right',headerStyle:'width: 10%' },
                                    { field: 'SozlesmeId',bodyClass:'text-center',headerText: 'Devir Al', headerStyle:'width: 10%', content: function (rowData)
                                        { 
                                            if (rowData.FarkVarMi)
                                            {
                                                return $('<a href=# onclick=DevirAl('+rowData.SozlesmeId+'); class=\'btn btn-outline-danger \'>Devir Al</a>')
                                            }else
                                            {
                                                return('');
                                            }
                                        }
                                    },
                                    { field: 'SozlesmeId',headerText: 'Sözleşme', headerStyle:'width: 10%', content: function (rowData)
                                        { 
                                            contentFunc(rowData, 'Aktif',counter++);
                                            if (rowData.Aktif=='False'){
                                                return $('<a href=" + ProjeConstants.PAGE_BITENKIRASOZLESMESI + @"?DestinationApp=KS&SenderApp=KD&KiraSozlesmeId='+rowData.SozlesmeId +' class=\'btn btn-outline-secondary \'>Sözleşme</a>')
                                            }else{
                                                return $('<a href=" + ProjeConstants.PAGE_KIRASOZLESMESI + @"?DestinationApp=KS&SenderApp=KD&KiraSozlesmeId='+rowData.SozlesmeId +'&KiraciId='+rowData.KiraciId +' class=\'btn btn-outline-secondary \'>Sözleşme</a>')
                                            }
                                            
                                        }
                                    },
                                    { field: 'SozlesmeId',headerText: 'Ödeme Planı', headerStyle:'width: 10%', content: function (rowData)
                                        { 
                                            return $('<a href=" + ProjeConstants.PAGE_ODEMEPLANI + @"?KiraSozlesmeId='+rowData.SozlesmeId +' class=\'btn btn-outline-secondary \'>Ödm.Planı</a>')
                                        }
                                    },
                                ],
                                datasource:" + jsonData + @",
                                resizableColumns: true
                            });
                            $('#messages').puigrowl();
                            ";

            return tablestr;
        }

        private string BolgeGetir(Kiraci kiraci)
        {
            string ili = kiraci == null ? "" : kiraci.Ili;
            Il il = new Il();
            il = il.SelectByIlAdi(kiraci.Ili);
            string bolge = il == null ? "" : il.Bolge;
            return bolge;
        }
        protected void OdemePlaniGoruntuleBtn_Click(object sender, EventArgs e)
        {
            Kiraci kiraci = new Kiraci();
            kiraci = kiraci.Select<Kiraci>(KiraciIdQS.ConvertToInt());
            if (kiraci != null)//bu kiraci varsa
            {
                KiraSozlesme kiraSozlesme = new KiraSozlesme();
                kiraSozlesme = kiraSozlesme.SelectAktifSozlesmeByKiraciId(kiraci.Id);
                if (kiraSozlesme != null) //bu sozlesme varsa
                {
                    OdemePlani odemePlani = new OdemePlani();
                    bool odemePlaniVarMi = odemePlani.OdemePlaniVarMi(kiraSozlesme.Id);
                    if (odemePlani != null)
                    {
                        OdemePlaniGoruntule(kiraSozlesme);

                    }
                }
            }
        }
        private void OdemePlaniGoruntule(KiraSozlesme kiraSozlesme)
        {
            Kiraci kiraci = new Kiraci();
            kiraci = kiraci.Select<Kiraci>(kiraSozlesme.KiraciId);
            TitleLbl.Text = " Kiracı : " + kiraci.Adi + " " + kiraci.Soyadi;

            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);

            OdemePlani odemePlaniDao = new OdemePlani();
            List<OdemePlani> list = odemePlaniDao.SelectBySozlesmeId(kiraSozlesme.Id);
            if (list.Count > 0)
            {
                DevirLbl.Text = "Devir Anapara : " + kiraSozlesme.DevirAnaPara.ToString("N", culturInfo) + "      "
                    + "Devir Faiz : " + kiraSozlesme.DevirFaizTutari.ToString("N", culturInfo)
                    + "Devir FaizliBakiye : " + kiraSozlesme.DevirFaizliBakiye.ToString("N", culturInfo);
            }
            foreach (OdemePlani odemePlani in list)
            {
                if (odemePlani.Sira == 0)
                {
                    continue;
                }
                TableRow row = new TableRow();
                TableCell SiraNoCell = new TableCell();
                SiraNoCell.Text = odemePlani.Sira.ToString(); ;
                row.Controls.Add(SiraNoCell);

                TableCell YilCell = new TableCell();
                YilCell.CssClass = "text-right";
                YilCell.Text = odemePlani.Yil.ToString();
                row.Controls.Add(YilCell);

                TableCell AyCell = new TableCell();
                AyCell.Text = odemePlani.Ay.ToString();
                row.Controls.Add(AyCell);

                TableCell KiraBedeliCell = new TableCell();
                KiraBedeliCell.CssClass = "text-right";
                KiraBedeliCell.Text = odemePlani.KiraBedeli.ToString("N", culturInfo);
                row.Controls.Add(KiraBedeliCell);

                TableCell OdenenTutarCell = new TableCell();
                OdenenTutarCell.CssClass = "text-right";
                OdenenTutarCell.Text = odemePlani.OdenenTutar.ToString("N", culturInfo);
                row.Controls.Add(OdenenTutarCell);

                OdemePlaniTable.Controls.Add(row);
            }
            var jsString = @"         
                var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('OdemePlaniModal'));
                myModalInstance.show();";
            ScriptManager.RegisterStartupScript((Page)System.Web.HttpContext.Current.Handler, typeof(Page), System.Guid.NewGuid().ToString(), jsString, true);
        }
        protected void DevirAlBtn_Click(object sender, EventArgs e)
        {
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            kiraSozlesme = kiraSozlesme.Select(paramSozlesmeIdLbl.Value.ConvertToInt());
            if (kiraSozlesme != null)
            {
                KiraSozlesme oncekiKiraSozlesmesi = kiraSozlesme.SelectOncekiKiraSozlesme();
                if (oncekiKiraSozlesmesi != null)
                {
                    //önceki ödeme planının son satırındaki anapara, faiztutari, faizlibakiye alanlarını al, sözleşmenin devir alanlarına koy
                    OdemePlani odemePlaniDao = new OdemePlani();
                    List<OdemePlani> oncekiOdemePlaniList = odemePlaniDao.SelectBySozlesmeId(oncekiKiraSozlesmesi.Id);
                    OdemePlani oncekiOdemePlani = oncekiOdemePlaniList[oncekiOdemePlaniList.Count - 1];
                    kiraSozlesme.DevirAnaPara = oncekiOdemePlani.AnaPara;
                    kiraSozlesme.DevirFaizliBakiye = oncekiOdemePlani.FaizliBakiye;
                    kiraSozlesme.DevirFaizTutari = kiraSozlesme.DevirFaizliBakiye - kiraSozlesme.DevirAnaPara;
                    bool devirAlanlariGuncellendi= kiraSozlesme.Update();
                    if (devirAlanlariGuncellendi)
                    {
                        //Ödeme Planında devir satırını güncelle 0ncı satır
                        List<OdemePlani> odemePlaniList= odemePlaniDao.SelectBySozlesmeId(kiraSozlesme.Id);
                        OdemePlani odemePlani = odemePlaniList[0];//devir satiri
                        if (odemePlani != null)
                        {
                            if ((odemePlani.AnaPara != kiraSozlesme.DevirAnaPara)
                                || (odemePlani.FaizTutari != kiraSozlesme.DevirFaizTutari)
                                || (odemePlani.FaizliBakiye != kiraSozlesme.DevirFaizliBakiye))
                            {
                                odemePlani.AnaPara = kiraSozlesme.DevirAnaPara;
                                odemePlani.FaizTutari = kiraSozlesme.DevirFaizTutari;
                                odemePlani.FaizliBakiye = kiraSozlesme.DevirFaizliBakiye;
                                if (odemePlani.Update())
                                {
                                    OdemeleriHesaplaOdemePlaniniGuncelle(kiraSozlesme.KiraciId);
                                    SozlesmelerTablosunuDoldur(kiraSozlesme.KiraciId);
                                    MessageHelper.PublishMessage("Devir alma işlemi tamamlandı.", ProjeConstants.MESAJ_BASARILI,2000);
                                }
                                
                            }
                        }
                    }
                }
            }
                
        }
        protected void KiraKartiBtn_Click(object sender, EventArgs e)
        {
            Kiraci kiraci = new Kiraci();
            kiraci = kiraci.Select<Kiraci>(KiraciIdQS.ConvertToInt());
            if (kiraci != null)//bu kiraci varsa
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));
                newUrl += "/" + ProjeConstants.PAGE_KIRAKARTI + "?SenderApp=KD&KiraciId=" + kiraci.Id;
                Page.Response.Redirect(newUrl, true);

            }
        }
        protected void OdemePlaniBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Kiraci kiraci = new Kiraci();
                kiraci = kiraci.Select<Kiraci>(KiraciIdQS.ConvertToInt());
                if (kiraci != null)
                {
                    KiraSozlesme kiraSozlesme = new KiraSozlesme();
                    kiraSozlesme = kiraSozlesme.SelectAktifSozlesmeByKiraciId(kiraci.Id);
                    if (kiraSozlesme != null) //bu sozlesme varsa
                    {
                        OdemePlani odemePlani = new OdemePlani();
                        string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                        string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_ODEMEPLANI;
                        newUrl = newUrl + "?SenderApp=KD&KiraciId=" + kiraci.Id + "&KiraSozlesmeId=" + kiraSozlesme.Id;
                        Page.Response.Redirect(newUrl, true);
                    }
                    else
                        MessageHelper.PublishMessage("Bu kiracıya ait sözleşme kaydı bulunamadı", ProjeConstants.MESAJ_HATA);
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }


        }
        protected void PrevBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Kiraci kiraci = new Kiraci();
                kiraci = kiraci.Select<Kiraci>(KiraciIdQS.ConvertToInt());
                Kiraci oncekiKiraci = kiraci.SelectPrev();
                if (oncekiKiraci != null)
                {
                    RedirectToPage(ProjeConstants.PAGE_KIRA_BAKIYEDEVRI + "?KiraciId=" + oncekiKiraci.Id);
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void NextBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Kiraci kiraci = new Kiraci();
                kiraci = kiraci.Select<Kiraci>(KiraciIdQS.ConvertToInt());
                Kiraci sonrakiKiraci = kiraci.SelectNext();
                if (sonrakiKiraci != null)
                {
                    RedirectToPage(ProjeConstants.PAGE_KIRA_BAKIYEDEVRI + "?KiraciId=" + sonrakiKiraci.Id);
                }
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
        protected void KiraciListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_KIRACI_LIST + "?SecilenId=" + KiraciIdQS);
        }
        protected void KiraciAylikOdemeBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_KIRACI_AYLIKODEME + "?KiraciId=" + KiraciIdQS + "&SecilenAy=0&SecilenYil=0");
        }
        private class KiraSozlesmeListItem
        {
            public string DosyaNo { get; set; }
            public string Aktif { get; set; }
            public string SozlesmeId { get; set; }
            public string KiraciId { get; set; }
            public string SozlesmeTarihi { get; set; }
            public string SozlesmeBasTar { get; set; }
            public string SozlesmeBitTar { get; set; }
            public string TarihAraligi { get; set; }
            public string ArtisAyi { get; set; }
            public string OdemeSekli { get; set; }
            public string KiraBedeli { get; set; }
            public string SozlesmeDurumu { get; set; }
            public string DevirAnapara { get; set; }
            public string DevirFaiz { get; set; }
            public string DevirFaizliBakiye { get; set; }
            public bool FarkVarMi { get; set; }
        }
    }
}
