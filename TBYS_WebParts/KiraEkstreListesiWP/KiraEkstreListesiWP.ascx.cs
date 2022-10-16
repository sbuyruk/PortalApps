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
using TSKGV_Utility.HelperClasses;
using TSKGV_Utility.ProjeGlobal;

namespace TBYS_WebParts.KiraEkstreListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class KiraEkstreListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public KiraEkstreListesiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string IslemTarihiQS
        {
            get
            {

                if (ViewState["IslemTarihi"] == null)
                {
                    if (Page.Request.QueryString["IslemTarihi"] != null)
                    {
                        ViewState["IslemTarihi"] = Page.Request.QueryString["IslemTarihi"];
                    }
                    else
                    {
                        ViewState["IslemTarihi"] = string.Empty;
                    }
                }
                return ViewState["IslemTarihi"].ToString();
            }

            set
            {
                ViewState["IslemTarihi"] = value;
            }
        }
        private string IslemQS
        {
            get
            {
                if (ViewState["Islem"] == null)
                {
                    if (Page.Request.QueryString["Islem"] != null)
                    {
                        ViewState["Islem"] = Page.Request.QueryString["Islem"];
                    }
                    else
                    {
                        ViewState["Islem"] = string.Empty;
                    }
                }
                return ViewState["Islem"].ToString();
            }

            set
            {
                ViewState["Islem"] = value;
            }
        }
        private string AktarilanlarHaricQS
        {
            get
            {
                if (ViewState["AktarilanlarHaric"] == null)
                {
                    if (Page.Request.QueryString["AktarilanlarHaric"] != null)
                    {
                        ViewState["AktarilanlarHaric"] = Page.Request.QueryString["AktarilanlarHaric"];
                    }
                    else
                    {
                        ViewState["AktarilanlarHaric"] = AktarilanlarHaricChk.Checked;
                    }
                }
                return ViewState["AktarilanlarHaric"].ToString();
            }

            set
            {
                ViewState["AktarilanlarHaric"] = value;
            }
        }
        private string MesajQS
        {
            get
            {

                if (ViewState["Mesaj"] == null)
                {
                    if (Page.Request.QueryString["Mesaj"] != null)
                    {
                        ViewState["Mesaj"] = Page.Request.QueryString["Mesaj"];
                    }
                    else
                    {
                        ViewState["Mesaj"] = string.Empty;
                    }
                }
                return ViewState["Mesaj"].ToString();
            }

            set
            {
                ViewState["Mesaj"] = value;
            }
        }
        private string SelectAllQS
        {
            get
            {
                if (ViewState["SelectAll"] == null)
                {
                    if (Page.Request.QueryString["SelectAll"] != null)
                    {
                        ViewState["SelectAll"] = Page.Request.QueryString["SelectAll"];
                    }
                    else
                    {
                        ViewState["SelectAll"] = false;
                    }
                }
                return ViewState["SelectAll"].ToString();
            }

            set
            {
                ViewState["SelectAll"] = value;
            }
        }
        private readonly IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SayfaGirisKontrolu())
            {
                RedirectToPage(ProjeConstants.PAGE_HOME + "?Mesaj=true&Text=Sayfada düzenleme yapılmaktadır.Lütfen daha sonra tekrar deneyiniz.");
            }
            else
            {
                try
                {
                    if (!Page.IsPostBack)
                    {
                        if (!string.IsNullOrEmpty(MesajQS))
                        {
                            if (IslemQS.ToLower().Equals("silme"))
                            {
                                if (MesajQS.ToLower().Equals("true"))
                                    MessageHelper.PublishMessage("Kayıt Silindi", ProjeConstants.MESAJ_BASARILI, 2000);
                                else
                                    MessageHelper.PublishMessage("Kayıt Silinemedi", ProjeConstants.MESAJ_HATA);
                            }
                            else if (IslemQS.ToLower().Equals("kaydet"))
                            {
                                if (MesajQS.ToLower().Equals("true"))
                                    MessageHelper.PublishMessage("Aktarma tamamlandı.", ProjeConstants.MESAJ_BASARILI, 2000);
                                else
                                    MessageHelper.PublishMessage("Aktarma yapılamadı.", ProjeConstants.MESAJ_HATA);
                            }
                            MesajQS = string.Empty;
                        }
                        TumunuSecChk.Checked = SelectAllQS.ConvertToBool();
                        KayitGetir();
                        AktarilanlarHaricChk.Checked = AktarilanlarHaricQS.ConvertToBool();

                    }

                }
                catch (Exception ex)
                {
                    ExceptionHelper exHelper = new ExceptionHelper(ex);
                    exHelper.PublishException();
                }
            }

        }

        private bool SayfaGirisKontrolu()
        {
            return true;
        }

        private void KayitGetir()
        {
            List<TasinmazBagisci> list = new List<TasinmazBagisci>();
            var jsonData = KiraEkstreAktarmaJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateJsString(jsonData); //javascript kodu hazırlanıyor.
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), jsString, true);
        }
        private string KiraEkstreAktarmaJson()
        {

            string jSon = string.Empty;
            List<KiraEkstreAktarmaListItem> list = GetDataList();

            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
            jSon = serializer.Serialize(list);

            return jSon;
        }
        private string CreateJsString(string jsonData)
        {

            string ekstretablestr = @" 
                var counter=1;
                //tabloda modal açılırken seçili olan pagination degerini pageIndex degiskeninde saklar ve modal açıldıktan sonra pageload sırasında sayfayı pageIndex degerine getirir
                $(document).on('click','.ui-paginator-element',function(){
                    pageIndex= currentPage;
                });
                          
                $('#tblfilter').puidatatable({
                caption: '',
                editMode: 'cell',
                paginator: {
                            rows: 10
                            },
                columns: [
                                {
                field: 'KiraEkstreAktarmaId', headerText: 'Seç Kaydet', headerStyle: 'width: 5%',content: function(rowData)
                        {
                            if (rowData.AktarildiMi == 'True')
                            {
                               return '<input id=chk type=checkbox checked disabled class=ekstre-aktarildi />';
                            }
                            else if (rowData.KiraciId < 1)
                            {
                               return '';
                            }
                            else
                            {
                                var isChecked='';
                                if ('" + SelectAllQS.ConvertToBool() + @"'=='True')
                                {
                                    isChecked='checked';
                                    EkleCikar(rowData.KiraEkstreAktarmaId,isChecked);
                                }else
                                {
                                    isChecked='';
                                    EkleCikar(rowData.KiraEkstreAktarmaId,isChecked);
                                }
                                    
                               return $('<input id=chk class=ekstre-aktarilmadi '+isChecked+' onchange=addRemoveEkstreIdToList('+rowData.KiraEkstreAktarmaId+',this); type=checkbox />');
                            }
                        }
                    },
                    { field: 'AdiSoyadi', headerText: 'Parayı Yatıran', sortable:true,filter: true,headerStyle:'width: 15%',content: function (rowData)
                        { 
                            if (rowData.AktarildiMi=='True')
                            {
                                return '<span class=ekstre-aktarildi>'+rowData.AdiSoyadi+ '</span>';
                            }else{
                                if (rowData.CakismaVarMi == 'True'){
                                    return '<span class=cakisma-var>'+rowData.AdiSoyadi+ '</span>';
                                }else  if (rowData.Uyari == 'True'){
                                    return '<span class=uyari>'+rowData.AdiSoyadi+ '</span>';
                                }else
                                {   
                                    if (rowData.KiraciId > 0)
                                    {
                                        return '<span class=ekstre-aktarilabilir>'+rowData.AdiSoyadi+ '</span>';
                                    }else
                                    {
                                        return '<span class=ekstre-aktarilmadi>'+rowData.AdiSoyadi+ '</span>';
                                    }
                                }          
                            }
                        }
                    },

                    { field: 'OdemeTarihi', headerText: 'Ödeme Tarihi', sortable:true,headerStyle:'width: 10%',content: function (rowData)
                        { 
                            if (rowData.AktarildiMi=='True')
                            {
                                return '<span class=ekstre-aktarildi>'+rowData.OdemeTarihi+ '</span>';
                            }else{
                                if (rowData.CakismaVarMi == 'True'){                                    
                                    return '<span class=cakisma-var>'+rowData.OdemeTarihi+ '</span>';
                                }else{                                    
                                        
                                    if (rowData.KiraciId > 0)
                                    {
                                        return '<span class=ekstre-aktarilabilir>'+rowData.OdemeTarihi+ '</span>';
                                    }else
                                    {
                                        return '<span class=ekstre-aktarilmadi>'+rowData.OdemeTarihi+ '</span>';
                                    }
                                }  
                            }
                        }
                    },
                    { field: 'Tutar', headerText: 'Tutar', sortable:true,bodyClass:'text-right',headerStyle:'width: 10%',content: function (rowData)
                        { 
                            if (rowData.AktarildiMi=='True')
                            {
                                return '<span class=ekstre-aktarildi>'+rowData.Tutar+ '</span>';
                            }else{
                                if (rowData.CakismaVarMi == 'True'){                                    
                                    return '<span class=cakisma-var>'+rowData.Tutar+ '</span>';
                                }else{                                    
                                        
                                    if (rowData.KiraciId > 0)
                                    {
                                        return '<span class=ekstre-aktarilabilir>'+rowData.Tutar+ '</span>';
                                    }else
                                    {
                                        return '<span class=ekstre-aktarilmadi>'+rowData.Tutar+ '</span>';
                                    }
                                }          
                            }
                        }
                    },
                    { field: 'KiraciAdi', headerText: 'Kiracı Adı', sortable:true,filter: true,headerStyle:'width: 15%',bodyClass:'small-font',content: function (rowData)
                        { 
                            if (rowData.AktarildiMi=='True')
                            {
                                return '<span class=ekstre-aktarildi>'+rowData.KiraciAdi+ '</span>';
                            }else{
                                if (rowData.CakismaVarMi == 'True'){
                                    return '<span class=cakisma-var>'+rowData.KiraciAdi+ '</span>';
                                }else{                                    
                                   
                                    if (rowData.KiraciId > 0)
                                    {
                                        //return '<span class=ekstre-aktarilabilir>'+rowData.KiraciAdi+ '</span>';
                                        var odemeTarihi=rowData.OdemeTarihi.substr(0,rowData.OdemeTarihi.indexOf(' '));
                                    
                                        return $('<p class=\'ekstre-aktarilabilir btn btn-link\'  onclick=\'OdemePlaniModalAc('+rowData.KiraciId+',\''+odemeTarihi+'\');\' >'+rowData.KiraciAdi+'</p>');
                                    }else
                                    {
                                         return '<span class=ekstre-aktarilmadi>'+rowData.KiraciAdi+ '</span>';
                                    }

                                }          
                            }
                        }
                    },
                    { field: 'Aciklama', headerText: 'Açıklama', sortable:true,filter: true,headerStyle:'width: 32%',bodyClass:'small-font',content: function (rowData)
                        { 
                            if (rowData.AktarildiMi=='True')
                            {
                                return '<span class=ekstre-aktarildi>'+rowData.Aciklama+ '</span>';
                            }else{
                                if (rowData.CakismaVarMi == 'True'){
                                    return '<span class=cakisma-var>'+rowData.Aciklama+ '</span>';
                                }else{                                    
                                    if (rowData.KiraciId > 0)
                                    {
                                         return '<span class=ekstre-aktarilabilir>'+rowData.Aciklama+ '</span>';
                                    }else
                                    {
                                        return '<span class=ekstre-aktarilmadi>'+rowData.Aciklama+ '</span>';
                                    }
                                }          
                            }
                        }
                    },
                    { field: 'KiraEkstreAktarmaId',headerText: 'Eşleştir',bodyClass:'text-center',headerStyle:'width: 8%',content: function (rowData)
                        { 
                            if (rowData.AktarildiMi=='True')
                            {
                                return '';
                            }else{
                                return $('<a target=\'\' href=KiraciEslestir.aspx?KiraEkstreAktarmaId='+rowData.KiraEkstreAktarmaId + '&IslemTarihi='+rowData.IslemTarihi + ' class=\'btn btn-outline-primary \'>Eşleştir</a>')  
                            }
                        }
                    },
                    {field: 'KiraEkstreAktarmaId', headerText: 'Seç Sil', headerStyle: 'width: 5%',content: function(rowData)
                        {
                            if (rowData.AktarildiMi == 'True')
                            {
                                return '';
                            }
                            else
                            {                                   
                                return $('<input id=silChk class=sil-checkbox onchange=addRemoveEkstreIdToDeleteList('+rowData.KiraEkstreAktarmaId+',this); type=checkbox />');
                            }
                        }
                    },
                    ],
                    datasource:" + jsonData + @",
                    resizableColumns: true,
                    globalFilter:'#globalFilter'
                });
                $('#messages').puigrowl();
            ";
            return ekstretablestr;
        }
        protected void SecilenListeyiKaydet()
        {
            string value = paramArray.Value;
            //string[] idList = value.Split(',');
            //foreach (string item in idList)
            //{

            //}

            string currentUser = UtilityHelper.GetCurrentUser();
            if (!string.IsNullOrEmpty(value))
            {
                KiraEkstreAktarma eaDao = new KiraEkstreAktarma();
                int rowCount = 0;
                List<KiraEkstreAktarma> aktarilmayanlar = eaDao.SelectByEkstreIdList(value, ref rowCount);
                if (aktarilmayanlar.Count > 0)
                {
                    var exceptionHelper = OdemeIslemleriniYap(aktarilmayanlar, currentUser); //seçilenler diğer tablolara dağıtılıyor
                    if (exceptionHelper.Exceptions.Count > 0)
                    {
                        ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "CloseModalOnay();", true);
                        exceptionHelper.PublishException();
                    }
                    else
                    {
                        RedirectToPage(ProjeConstants.PAGE_KIRAEKSTRE_LIST + "?Mesaj=true&Islem=kaydet&basarili=true&IslemTarihi=" + IslemTarihiQS + "&AktarilanlarHaric=" + AktarilanlarHaricQS );
                    }
                }
            }
            else
            {
                MessageHelper.PublishMessage("Hiç kayıt seçilmedi. Devam etmek için en az bir kayıt seçiniz.", ProjeConstants.MESAJ_BILGI);
            }
        }
        protected void SecilenListeyiSil()
        {
            string value = paramSilinecekArray.Value;
            //string[] idList = value.Split(',');
            //foreach (string item in idList)
            //{

            //}

            string currentUser = UtilityHelper.GetCurrentUser();
            if (!string.IsNullOrEmpty(value))
            {
                try
                {
                    KiraEkstreAktarma eaDao = new KiraEkstreAktarma();
                    List<KiraEkstreAktarma> silinecekler = eaDao.SelectByIdList(value);
                    string mesaj = string.Empty;
                    int counter = 0;
                    foreach (KiraEkstreAktarma item in silinecekler)
                    {
                        mesaj += " #" + counter + ":" + item.Adi;// + " OdemeTarihi:" + item.OdemeTarihi + " BagisMiktari:" + item.Tutar + " IslemTarihi:" + item.IslemTarihi.ConvertToDatetimeEmptyIfNull();
                        item.Delete();

                    }
                    SilinenKayit sk = new SilinenKayit();
                    sk.Silen = currentUser;
                    sk.SilinmeSebebi = counter + " adet kayıt silindi";
                    sk.TabloAdi = "KiraEkstreAktarma_Table";
                    sk.SilinmeTarihi = DateTime.Now.ReturnTRDateFormat();
                    sk.SilinenKayitBilgisi = mesaj;
                    sk.Save();

                    RedirectToPage(ProjeConstants.PAGE_KIRAEKSTRE_LIST + "?IslemTarihi=" + IslemTarihiQS + "&AktarilanlarHaric=" + AktarilanlarHaricQS + "&Mesaj=true&Islem=Silme&Basarili=true");
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "CloseModalOnay();", true);
                    ExceptionHelper exHelper = new ExceptionHelper(ex);
                    exHelper.PublishException();
                }
            }
            else
            {
                MessageHelper.PublishMessage("Hiç kayıt seçilmedi. Devam etmek için en az bir kayıt seçiniz.", ProjeConstants.MESAJ_BILGI);
            }
        }
        public ExceptionHelper OdemeIslemleriniYap(List<KiraEkstreAktarma> listEkstreAktarma, string currentUser)
        {
            ExceptionHelper exceptionHelper = new ExceptionHelper();


            foreach (KiraEkstreAktarma ekstreAktarma in listEkstreAktarma)
            {

                try
                {
                    DateTime odemeTarihi = ekstreAktarma.OdemeTarihi;
                    decimal odemeTutari = ekstreAktarma.Tutar;

                    KiraSozlesme kiraSozlesme = new KiraSozlesme();
                    kiraSozlesme = kiraSozlesme.SelectByKiraciIdTarih(ekstreAktarma.KiraciId, odemeTarihi);
                    if (kiraSozlesme == null)
                    {
                        kiraSozlesme = new KiraSozlesme();
                        kiraSozlesme = kiraSozlesme.SelectEnYakinTarihliSozlesmeByKiraciIdTarih(ekstreAktarma.KiraciId, odemeTarihi);
                        
                    }
                    if (kiraSozlesme != null)
                    {
                        int odemeId = 0;
                        bool odemeYapildiMi = TBYSOrtak.OdemeYap(kiraSozlesme, odemeTarihi, odemeTutari, " Banka Ekstresinden Aktarma ", ref odemeId);
                        if (odemeYapildiMi)
                        {
                            ekstreAktarma.AktarildiMi = true;
                            ekstreAktarma.Update();

                        }
                    }
                    else
                    {
                        throw new Exception("Sözleşme Bulunamadı");
                    }
                       
                }
                catch (Exception e)
                {
                    exceptionHelper.Exceptions.Add(e);
                }

            }

            return exceptionHelper;
        }

        private List<KiraEkstreAktarmaListItem> GetDataList()
        {
            
            KiraEkstreAktarma ea = new KiraEkstreAktarma();
            int rowCount = 0;
            DataTable dataTable = ea.SelectYuklenenKayit(ref rowCount, AktarilanlarHaricQS.ConvertToBool());

            RowCountLbl.Text = "Kayıt Sayısı : " + rowCount.ToString();
            List<KiraEkstreAktarmaListItem> returnlist = new List<KiraEkstreAktarmaListItem>();
            if (dataTable != null)
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    KiraEkstreAktarmaListItem ekstreAktarmaListItem = new KiraEkstreAktarmaListItem();

                    ekstreAktarmaListItem.KiraEkstreAktarmaId = dataRow["KiraEkstreAktarmaId"].ToString();
                    ekstreAktarmaListItem.IslemTarihi = dataRow["IslemTarihi"].ToString();

                    ekstreAktarmaListItem.KiraciId = dataRow["KiraciId"].ToString();
                    ekstreAktarmaListItem.KiraciAdi = dataRow["KiraciAdi"].ToString();
                    ekstreAktarmaListItem.IslemNo = dataRow["IslemNo"].ToString();

                    ekstreAktarmaListItem.BankaAdi = dataRow["BankaAdi"].ToString();
                    ekstreAktarmaListItem.TCKimlikNo = dataRow["TCKimlikNo"].ToString();

                    ekstreAktarmaListItem.Adi = dataRow["Adi"].ToString();
                    ekstreAktarmaListItem.Soyadi = dataRow["Soyadi"].ToString();
                    ekstreAktarmaListItem.AdiSoyadi = dataRow["AdiSoyadi"].ToString();
                    ekstreAktarmaListItem.Telefon = dataRow["Telefon"].ToString();
                    ekstreAktarmaListItem.Telefon1 = dataRow["Telefon1"].ToString();
                    ekstreAktarmaListItem.Telefon2 = dataRow["Telefon2"].ToString();
                    ekstreAktarmaListItem.Aciklama = dataRow["Aciklama"].ToString();
                    DateTime odemeTarihi = dataRow["OdemeTarihi"].ConvertToDatetime();
                    ekstreAktarmaListItem.OdemeTarihi = odemeTarihi.ToString("dd.MM.yyyy HH:mm");
                    decimal tutar = dataRow["Tutar"].ConvertToDecimal();
                    string dovizCinsi = dataRow["DovizCinsi"].ToString();
                    ekstreAktarmaListItem.Tutar = tutar > 0 ? tutar.ToString("N", culturInfo) + " " + dovizCinsi : "";
                    bool aktarildiMi = dataRow["AktarildiMi"].ConvertToBool();
                    bool uyari = dataRow["Uyari"].ConvertToBool();
                    ekstreAktarmaListItem.AktarildiMi = aktarildiMi.ToString();
                    ekstreAktarmaListItem.Uyari = uyari.ToString();

                    ekstreAktarmaListItem.CakismaVarMi = aktarildiMi ? "False" : CakismaKontrolu(ekstreAktarmaListItem, tutar, odemeTarihi).ToString();

                    returnlist.Add(ekstreAktarmaListItem);
                    if (uyari)
                    {
                        //birden fazla kişi ile eşleşmiş
                        KayitSayisinaBol(returnlist,ekstreAktarmaListItem, tutar);
                    }
                   
                }
            }
            return returnlist;
        }

        private void KayitSayisinaBol(List<KiraEkstreAktarmaListItem> returnlist,KiraEkstreAktarmaListItem ekstreAktarmaListItem, decimal odenenTutar)
        {
            Kiraci kiraciDao = new Kiraci();

            if (!string.IsNullOrEmpty(ekstreAktarmaListItem.AdiSoyadi))
            {
                List<Kiraci> kiraciList = kiraciDao.SelectByAdi(
                    string.IsNullOrEmpty(ekstreAktarmaListItem.KiraciAdi) ? ekstreAktarmaListItem.AdiSoyadi:
                    ekstreAktarmaListItem.KiraciAdi);
                if (kiraciList.Count>1)
                {
                    returnlist.Remove(ekstreAktarmaListItem);

                    KiraEkstreAktarmaListItem son = new KiraEkstreAktarmaListItem();
                    foreach (var item in kiraciList)
                    {
                        KiraEkstreAktarmaListItem yeni = new KiraEkstreAktarmaListItem();
                        yeni.Aciklama = ekstreAktarmaListItem.Aciklama;
                        UtilityHelper.CopyProperties(ekstreAktarmaListItem, yeni);
                        KiraSozlesme kiraSozlesme = new KiraSozlesme();
                        kiraSozlesme = kiraSozlesme.SelectEnYakinTarihliSozlesmeByKiraciIdTarih(item.Id,ekstreAktarmaListItem.OdemeTarihi.ConvertToDatetime());
                        if (kiraSozlesme != null)
                        {
                            yeni.KiraciId = kiraSozlesme.KiraciId.ToString();
                            yeni.KiraciAdi = (item.Adi + item.Soyadi).Trim();
                            decimal buOdeme = (odenenTutar - kiraSozlesme.KiraBedeli) >= 0 ? kiraSozlesme.KiraBedeli : odenenTutar ;
                            yeni.Tutar = buOdeme.ToString("N", culturInfo) + " TL";
                            odenenTutar -= buOdeme;
                            son = yeni;
                        }
                        returnlist.Add(yeni);
                    }
                    if (odenenTutar > 0)
                        son.Tutar = (son.Tutar.Replace(" TL","").ConvertToDecimal()+ odenenTutar).ToString("N", culturInfo) + " TL"; ;
                }
            }

            


        }

        private bool CakismaKontrolu(KiraEkstreAktarmaListItem ekstreAktarmaListItem, decimal tutar, DateTime odemeTarihi)
        {
            bool isConflict = false;

            KiraEkstreAktarma keDao = new KiraEkstreAktarma();
            List<KiraEkstreAktarma> list = keDao.SelectByIslemNo(ekstreAktarmaListItem.IslemNo);
            if (list.Count > 1) 
            {
                isConflict = true;

            }
            else
            {
                list = keDao.SelectByColumns(ekstreAktarmaListItem.Adi, ekstreAktarmaListItem.Soyadi, tutar, odemeTarihi);
                if (list.Count > 1) // TC Kimlik numarası var konflict yok.. // TCKİMLİKNO geçerli mi diye kontrol etmek gerekir mi?
                {
                    isConflict = true;
                }
            }
            
            return isConflict;
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
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        protected void ExcelBtn_Click(object sender, EventArgs e)
        {
            try
            {
                ExportToExcel();
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void ExportToExcel()
        {
            GridView GridView1 = new GridView();
            GridView1.AllowPaging = false;
            List<TasinmazBagisci> list = new List<TasinmazBagisci>();
            GridView1.DataSource = GetDataList(); ;
            GridView1.DataBind();

            Page.Response.Clear();
            Page.Response.Buffer = true;
            Page.Response.AddHeader("content-disposition",
             "attachment;filename=KiraEkstreAktarmaListesi_" + IslemTarihiQS.ConvertToDatetimeEmptyIfNull() + ".xls");
            Page.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1254");
            Page.Response.Charset = "windows-1254";//ISO-8859-9
            Page.Response.ContentType = "application/vnd.ms-excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);

            for (int i = 0; i < GridView1.Rows.Count; i++)
            {
                //Apply text style to each Row
                GridView1.Rows[i].Attributes.Add("class", "textmode");
            }
            GridView1.RenderControl(hw);

            //style to format numbers to string
            string style = @"<style> .textmode { mso-number-format:\@; } </style>";
            Page.Response.Write(style);
            Page.Response.Output.Write(sw.ToString());
            Page.Response.Flush();
            Page.Response.End();

        }
        private class KiraEkstreAktarmaListItem
        {
            public string KiraEkstreAktarmaId { get; set; }
            public string IslemTarihi { get; set; }
            public string KiraciId { get; set; }
            public string KiraciAdi { get; set; }
            public string TCKimlikNo { get; set; }
            public string Telefon { get; set; }
            public string Telefon1 { get; set; }
            public string Telefon2 { get; set; }
            public string Adi { get; set; }
            public string Soyadi { get; set; }
            public string AdiSoyadi { get; set; }
            public string Adres { get; set; }
            public string Aciklama { get; set; }
            public string OdemeTarihi { get; set; }
            public string BankaAdi { get; set; }
            public string Tutar { get; set; }
            public string AktarildiMi { get; set; }
            public string IslemNo { get; set; }
            public string CakismaVarMi { get; set; }
            public string Uyari { get; set; }

        }
        protected void AktarilanlarHaricChk_CheckedChanged(object sender, EventArgs e)
        {
            AktarilanlarHaricQS = AktarilanlarHaricChk.Checked.ToString();
            RedirectToPage(ProjeConstants.PAGE_KIRAEKSTRE_LIST + "?IslemTarihi=" + IslemTarihiQS + "&AktarilanlarHaric=" + AktarilanlarHaricQS );
        }
        protected void SecilenleriKaydetBtn_Click(object sender, EventArgs e)
        {

            string value = paramArray.Value;
            string[] idList = value.Split(',');

            if (idList.Length < 1)
            {
                MessageHelper.PublishMessage("Kaydetmek için kayıt seçiniz.", ProjeConstants.MESAJ_BILGI, 2000);
            }
            else
            {
                ModalTitleLbl.Text = "Seçilen Kayıtlar Aktarılacak";
                ModalSubTitleLbl.Text = idList.Length + " Adet satırı kaydetmek için seçtiniz.";
                UyariMesajiLbl.Text = "Lütfen kaydetmeden önce dikkatle inceleyiniz.";
                SilNowBtn.Visible = false;
                KaydetNowBtn.Visible = true;
                var openPopup = "OpenModalOnay();";
                System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), openPopup, true);
            }
        }
        protected void SecilenleriSilBtn_Click(object sender, EventArgs e)
        {

            string value = paramSilinecekArray.Value;
            string[] idList = value.Split(',');
            if (idList.Length < 1)
            {
                MessageHelper.PublishMessage("Silmek için kayıt seçiniz.", ProjeConstants.MESAJ_BILGI, 2000);
            }
            else
            {
                ModalTitleLbl.Text = "Seçilen Kayıtlar Silinecek";
                ModalSubTitleLbl.Text = idList.Length + " Adet kaydı silmek için seçtiniz.";
                UyariMesajiLbl.Text = "Lütfen silmeden önce dikkatle inceleyiniz.";
                SilNowBtn.Visible = true;
                KaydetNowBtn.Visible = false;
                var openPopup = "OpenModalOnay();";
                System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), openPopup, true);
            }

        }
        protected void KaydetNowBtn_Click(object sender, EventArgs e)
        {
            try
            {
                SecilenListeyiKaydet();
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void SilNowBtn_Click(object sender, EventArgs e)
        {
            SecilenListeyiSil();
        }

        protected void OdemePlaniModalAcBtn_Click(object sender, EventArgs e)
        {
            int kiraciId = ParamKiraciIdLbl.Value.ConvertToInt();
            DateTime odemeTarihi = ParamOdemeTarihiLbl.Value.ConvertToDatetime();
            Kiraci kiraci = new Kiraci();
            kiraci = kiraci.Select<Kiraci>(kiraciId);
            if (kiraci != null)//bu kiraci varsa
            {
                KiraSozlesme kiraSozlesme = new KiraSozlesme();
                kiraSozlesme = kiraSozlesme.SelectEnYakinTarihliSozlesmeByKiraciIdTarih(kiraci.Id, odemeTarihi);
                if (kiraSozlesme != null) //bu sozlesme varsa
                {
                    OdemePlani odemePlani = new OdemePlani();
                    bool odemePlaniVarMi = odemePlani.OdemePlaniVarMi(kiraSozlesme.Id);
                    if (odemePlaniVarMi )
                    {
                        OdemePlaniGoruntule(kiraSozlesme, kiraci);
                    }
                }
            }
        }
        private void OdemePlaniGoruntule(KiraSozlesme kiraSozlesme, Kiraci kiraci)
        {
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
            var jsString = " $('#OdemePlaniModal').modal({ backdrop: false });";
            ScriptManager.RegisterStartupScript((Page)System.Web.HttpContext.Current.Handler, typeof(Page), System.Guid.NewGuid().ToString(), jsString, true);
        }
        protected void TumunuSecChk_CheckedChanged(object sender, EventArgs e)
        {
            SelectAllQS = TumunuSecChk.Checked.ToString();
            KayitGetir();
        }
    }
}