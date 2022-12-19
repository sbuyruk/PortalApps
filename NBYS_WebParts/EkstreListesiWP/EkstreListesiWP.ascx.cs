using Model.NBYS;
using Model.Ortak;
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

namespace NBYS_WebParts.EkstreListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class EkstreListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public EkstreListesiWP()
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
        private string BankaQS
        {
            get
            {
                if (ViewState["Banka"] == null)
                {
                    if (Page.Request.QueryString["Banka"] != null)
                    {
                        ViewState["Banka"] = Page.Request.QueryString["Banka"];
                    }
                    else
                    {
                        ViewState["Banka"] = BankaDDL.SelectedItem.Value;
                    }
                }
                return ViewState["Banka"].ToString();
            }

            set
            {
                ViewState["Banka"] = value;
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
                        if (!string.IsNullOrEmpty(IslemTarihiQS))
                        {
                            IslemTarihiTxt.Text = IslemTarihiQS;
                        }
                        else
                        {
                            DateTime today = DateTime.Now;
                            IslemTarihiTxt.Text = today.ToString(ProjeConstants.DATE_TR);
                        }

                        BankaDDLDoldur();
                        if (!string.IsNullOrEmpty(BankaQS))
                        {
                            if (BankaDDL.Items.FindByValue(BankaQS) != null)
                                BankaDDL.SelectedValue = BankaDDL.Items.FindByValue(BankaQS).Value;
                        }
                        BankaEtiketleriniBaşlat();
                        AktarilanBankalariOkLe(IslemTarihiTxt.Text.ConvertToDatetime());
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
            var jsonData = EkstreAktarmaJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateJsString(jsonData); //javascript kodu hazırlanıyor.
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), jsString, true);
        }
        private string EkstreAktarmaJson()
        {
            //DateTime zaman1 = DateTime.Now;
            string jSon = string.Empty;
            List<EkstreAktarmaListItem> list = GetDataList();
            //DateTime zaman2 = DateTime.Now;
            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
            jSon = serializer.Serialize(list);
            //DateTime zaman3 = DateTime.Now;
            //MessageHelper.PublishMessage(zaman1+" ---- "+zaman2+" ---- " + zaman3 + " ---- ", ProjeConstants.MESAJ_BILGI);
            return jSon;
        }
        private string CreateJsString(string jsonData)
        {

            //return '<span class=bagis-iade-edildi>'+rowData.IadeMiktari+ ' '+rowData.DovizCinsi+ ' Parası İade edildi</span>';
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
                field: 'EkstreAktarmaId', headerText: 'Seç', headerStyle: 'width: 5%',content: function(rowData)
                        {
                            if (rowData.AktarildiMi == 'True')
                            {
                               return '<input id=chk type=checkbox checked disabled class=ekstre-aktarildi />';
                            }
                            else
                            {
                                var isChecked='';
                                if ('" + SelectAllQS.ConvertToBool() + @"'=='True')
                                {
                                    
                                    isChecked='checked';
                                    EkleCikar(rowData.EkstreAktarmaId,isChecked);
                                }else
                                {
                                    isChecked='';
                                    EkleCikar(rowData.EkstreAktarmaId,isChecked);
                                }
                                    
                               return $('<input id=chk class=ekstre-aktarilmadi '+isChecked+' onchange=addRemoveEkstreIdToList('+rowData.EkstreAktarmaId+',this); type=checkbox />');
                            }
                        }
                    },
                    { field: 'EkstreAktarmaId', headerText: 'S.No', sortable:true, filter: true, headerStyle:'width: 6%',content: function (rowData)
                        { 
                            if (rowData.AktarildiMi=='True')
                            {
                                return '<span class=ekstre-aktarildi>'+rowData.EkstreAktarmaId+ '</span>';
                            }else{
                                if (rowData.CakismaVarMi == 'True'){
                                    return '<span class=cakisma-var>'+rowData.EkstreAktarmaId+ '</span>';
                                }else{
                                    return '<span class=ekstre-aktarilmadi>'+rowData.EkstreAktarmaId+ '</span>';
                                }                                
                            }
                        }
                    },
                    { field: 'BankaAdi', headerText: 'Banka Adı', sortable:true, filter: true,headerStyle:'width: 13%',content: function (rowData)
                        {
                            if (rowData.AktarildiMi=='True')
                            {
                                return '<span class=ekstre-aktarildi>'+rowData.BankaAdi+ '</span>';
                            }else{
                                if (rowData.CakismaVarMi == 'True'){                                    
                                    return '<span class=cakisma-var>'+rowData.BankaAdi+ '</span>';
                                }else{                                    
                                     return '<span class=ekstre-aktarilmadi>'+rowData.BankaAdi+ '</span>';
                                }  
                            }
                        }
                    },
                    { field: 'TCKimlikNo', headerText: 'TCKimlikNo', sortable:true, filter: true, headerStyle:'width: 11%',content: function (rowData)
                        { 
                            if (rowData.AktarildiMi=='True')
                            {
                                return '<span class=ekstre-aktarildi>'+rowData.TCKimlikNo+ '</span>';
                            }else{
                                if (rowData.CakismaVarMi == 'True'){
                                    return '<span class=cakisma-var>'+rowData.TCKimlikNo+ '</span>';
                                }else{                                    
                                    return '<span class=ekstre-aktarilmadi>'+rowData.TCKimlikNo+ '</span>';
                                }          
                            }
                        }
                    },
                    { field: 'AdiSoyadi', headerText: 'Adı Soyadı', sortable:true,filter: true,headerStyle:'width: 20%',content: function (rowData)
                        { 
                            if (rowData.AktarildiMi=='True')
                            {
                                return '<span class=ekstre-aktarildi>'+rowData.AdiSoyadi+ '</span>';
                            }else{
                                if (rowData.CakismaVarMi == 'True'){
                                    return '<span class=cakisma-var>'+rowData.AdiSoyadi+ '</span>';
                                    //return $('<a href=# onclick=OpenModal('+rowData.NakitBagisciId+'); class=cakisma-var>'+rowData.Adi+'</a>')
                                }else{                                    
                                    return '<span class=ekstre-aktarilmadi>'+rowData.AdiSoyadi+ '</span>';
                                    //return $('<a href=# onclick=OpenModal('+rowData.NakitBagisciId+'); class=ekstre-aktarilmadi>'+rowData.Adi+'</a>')
                                }          
                            }
                        }
                    },
                    { field: 'Telefon', headerText: 'Telefon', sortable:true,filter: true,headerStyle:'width: 10%',content: function (rowData)
                        { 
                            if (rowData.AktarildiMi=='True')
                            {
                                return '<span class=ekstre-aktarildi>'+rowData.Telefon+ '</span>';
                            }else{
                                if (rowData.CakismaVarMi == 'True'){
                                    return '<span class=cakisma-var>'+rowData.Telefon+ '</span>';
                                }else{                                    
                                    return '<span class=ekstre-aktarilmadi>'+rowData.Telefon+ '</span>';
                                }          
                            }
                        }
                    },
                    { field: 'BagisTarihi', headerText: 'Bağış Tarihi', sortable:true,headerStyle:'width: 9%',content: function (rowData)
                        { 
                            if (rowData.AktarildiMi=='True')
                            {
                                return '<span class=ekstre-aktarildi>'+rowData.BagisTarihi+ '</span>';
                            }else{
                                if (rowData.CakismaVarMi == 'True'){                                    
                                    return '<span class=cakisma-var>'+rowData.BagisTarihi+ '</span>';
                                }else{                                    
                                        return '<span class=ekstre-aktarilmadi>'+rowData.BagisTarihi+ '</span>';
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
                                        return '<span class=ekstre-aktarilmadi>'+rowData.Tutar+ '</span>';
                                }          
                            }
                        }
                    },
                    { field: 'Id',headerText: 'Düzenle',bodyClass:'text-center',headerStyle:'width: 8%',content: function (rowData)
                        { 
                            if (rowData.AktarildiMi=='True')
                            {
                                return '';
                            }else{
                                return $('<a target=\'\' href=EkstreAktarmaEdit.aspx?SenderApp=EkstreListesi&EkstreAktarmaId='+rowData.EkstreAktarmaId + ' class=\'btn btn-outline-primary \'>Düzenle</a>')  
                            }
                        }
                    },
                    { field: 'EkstreAktarmaId',headerText: 'Eşleştir',bodyClass:'text-center',headerStyle:'width: 8%',content: function (rowData)
                        { 
                            if (rowData.AktarildiMi=='True')
                            {
                                return '';
                            }else{
                                return $('<a target=\'\' href=NakitBagisciEslestir.aspx?EkstreAktarmaId='+rowData.EkstreAktarmaId + ' class=\'btn btn-outline-primary \'>Eşleştir</a>')  
                            }
                        }
                    },
                    //{ field: 'EkstreAktarmaId',headerText: 'Sil',bodyClass:'text-center',headerStyle:'width: 6%',content: function (rowData)
                    //    { 
                    //        if (rowData.AktarildiMi=='True')
                    //        {
                    //            return '';
                    //        }else{
                    //            return $('<a href=# onclick=CallButtonClick('+rowData.EkstreAktarmaId + '); class=\'btn btn-outline-danger \'>Sil</a>')                                           
                    //        }
                    //    }
                    //},
                    ],
                    datasource:" + jsonData + @",
                    resizableColumns: true,
                    globalFilter:'#globalFilter'
                });
                $('#messages').puigrowl();
            ";
            return ekstretablestr;
        }
        private void BankaEtiketleriniBaşlat()
        {
            AkbankLbl.Text = ProjeConstants.BANKA_AKBANK;
            GarantiLbl.Text = ProjeConstants.BANKA_GARANTI;
            HalkbankLbl.Text = ProjeConstants.BANKA_HALKBANK;
            Halkbank2Lbl.Text = ProjeConstants.BANKA_HALKBANK2;
            IsbankLbl.Text = ProjeConstants.BANKA_ISBANK;
            ZiraatBankLbl.Text = ProjeConstants.BANKA_ZIRAAT;
            ZiraatBankEkstreLbl.Text = ProjeConstants.BANKA_ZIRAATEKSTRE;
            ZiraatKatilimLbl.Text = ProjeConstants.BANKA_ZIRAAT_KATILIM;
            VakifbankLbl.Text = ProjeConstants.BANKA_VAKIF;
            Vakifbank2Lbl.Text = ProjeConstants.BANKA_VAKIF2;
            KartIleOkLbl.Text = string.Empty;

            AkbankOkLbl.Text = string.Empty;
            GarantiOkLbl.Text = string.Empty;
            HalkbankOkLbl.Text = string.Empty;
            Halkbank2OkLbl.Text = string.Empty;
            IsbankOkLbl.Text = string.Empty;
            ZiraatBankOkLbl.Text = string.Empty;
            ZiraatBankEkstreOkLbl.Text = string.Empty;
            ZiraatKatilimOkLbl.Text = string.Empty;
            KartIleOkLbl.Text = string.Empty;

        }
        private void AktarilanBankalariOkLe(DateTime islemTarihi)
        {

            EkstreAktarma ekstreAktarma = new EkstreAktarma();
            bool isAkbankAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_AKBANK, islemTarihi);
            if (isAkbankAktarildi)
            {
                AkbankOkLbl.ForeColor = System.Drawing.Color.Green;
                AkbankOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            else
            {
                AkbankOkLbl.ForeColor = System.Drawing.Color.Red;
                AkbankOkLbl.Text = "X";
            }


            bool isGarantiAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_GARANTI, islemTarihi);
            if (isGarantiAktarildi)
            {
                GarantiOkLbl.ForeColor = System.Drawing.Color.Green;
                GarantiOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            else
            {
                GarantiOkLbl.ForeColor = System.Drawing.Color.Red;
                GarantiOkLbl.Text = "X";
            }


            bool isHalkBankAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_HALKBANK, islemTarihi);
            if (isHalkBankAktarildi)
            {
                HalkbankOkLbl.ForeColor = System.Drawing.Color.Green;
                HalkbankOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            else
            {
                HalkbankOkLbl.ForeColor = System.Drawing.Color.Red;
                HalkbankOkLbl.Text = "X";
            }
            bool isHalkbank2Aktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_HALKBANK2, islemTarihi);
            if (isHalkbank2Aktarildi)
            {
                Halkbank2OkLbl.ForeColor = System.Drawing.Color.Green;
                Halkbank2OkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            else
            {
                Halkbank2OkLbl.ForeColor = System.Drawing.Color.Red;
                Halkbank2OkLbl.Text = "X";
            }
            bool isIsbankAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_ISBANK, islemTarihi);
            if (isIsbankAktarildi)
            {
                IsbankOkLbl.ForeColor = System.Drawing.Color.Green;
                IsbankOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            else
            {
                IsbankOkLbl.ForeColor = System.Drawing.Color.Red;
                IsbankOkLbl.Text = "X";
            }

            bool isVakifBankAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_VAKIF, islemTarihi);
            if (isVakifBankAktarildi)
            {
                VakifbankOkLbl.ForeColor = System.Drawing.Color.Green;
                VakifbankOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            else
            {
                VakifbankOkLbl.ForeColor = System.Drawing.Color.Red;
                VakifbankOkLbl.Text = "X";
            }
            bool isVakifBank2Aktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_VAKIF2, islemTarihi);
            if (isVakifBank2Aktarildi)
            {
                Vakifbank2OkLbl.ForeColor = System.Drawing.Color.Green;
                Vakifbank2OkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            else
            {
                Vakifbank2OkLbl.ForeColor = System.Drawing.Color.Red;
                Vakifbank2OkLbl.Text = "X";
            }
            bool isZiraatAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_ZIRAAT, islemTarihi);
            if (isZiraatAktarildi)
            {
                ZiraatBankOkLbl.ForeColor = System.Drawing.Color.Green;
                ZiraatBankOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            else
            {
                ZiraatBankOkLbl.ForeColor = System.Drawing.Color.Red;
                ZiraatBankOkLbl.Text = "X";
            }
            bool isZiraatEkstreAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_ZIRAATEKSTRE, islemTarihi);
            if (isZiraatEkstreAktarildi)
            {
                ZiraatBankEkstreOkLbl.ForeColor = System.Drawing.Color.Green;
                ZiraatBankEkstreOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            else
            {
                ZiraatBankEkstreOkLbl.ForeColor = System.Drawing.Color.Red;
                ZiraatBankEkstreOkLbl.Text = "X";
            }
            bool isZiraatKatilimAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_ZIRAAT_KATILIM, islemTarihi);
            if (isZiraatKatilimAktarildi)
            {
                ZiraatKatilimOkLbl.ForeColor = System.Drawing.Color.Green;
                ZiraatKatilimOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            else
            {
                ZiraatKatilimOkLbl.ForeColor = System.Drawing.Color.Red;
                ZiraatKatilimOkLbl.Text = "X";
            }

            bool isTebAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_TEB, islemTarihi);
            if (isTebAktarildi)
            {
                TebOkLbl.ForeColor = System.Drawing.Color.Green;
                TebOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            else
            {
                TebOkLbl.ForeColor = System.Drawing.Color.Red;
                TebOkLbl.Text = "X";
            }

            bool isKartIleAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_KARTILEBAGIS, islemTarihi);
            if (isKartIleAktarildi)
            {
                KartIleOkLbl.ForeColor = System.Drawing.Color.Green;
                KartIleOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            else
            {
                KartIleOkLbl.ForeColor = System.Drawing.Color.Red;
                KartIleOkLbl.Text = "X";
            }
        }
        protected void SecilenListeyiKaydet()
        {
            string value = paramArray.Value;
            //string[] idList = value.Split(',');
            //foreach (string item in idList)
            //{

            //}

            string currentUser = UtilityHelper.GetCurrentUserLoginName();
            if (!string.IsNullOrEmpty(value))
            {
                EkstreAktarma eaDao = new EkstreAktarma();
                int rowCount = 0;
                List<EkstreAktarma> aktarilmayanlar = eaDao.selectByEkstreIdList(value, ref rowCount);
                if (aktarilmayanlar.Count > 0)
                {
                    var exceptionHelper = EkstreAktarma.SaveAll(aktarilmayanlar, currentUser); //seçilenler diğer tablolara dağıtılıyor
                    if (exceptionHelper.Exceptions.Count > 0)
                    {
                        ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "CloseModalOnay();", true);
                        exceptionHelper.PublishException();
                    }
                    else
                    {
                        RedirectToPage(ProjeConstants.PAGE_EKSTRE_LIST + "?Mesaj=true&Islem=kaydet&basarili=true&IslemTarihi=" + IslemTarihiQS + "&AktarilanlarHaric=" + AktarilanlarHaricQS + "&Banka=" + BankaQS);
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
            string value = paramArray.Value;
            //string[] idList = value.Split(',');
            //foreach (string item in idList)
            //{

            //}

            string currentUser = UtilityHelper.GetCurrentUserLoginName();
            if (!string.IsNullOrEmpty(value))
            {
                try
                {
                    EkstreAktarma eaDao = new EkstreAktarma();
                    List<EkstreAktarma> silinecekler = eaDao.selectByIdList(value);
                    string mesaj = string.Empty;
                    int counter = 0;
                    foreach (EkstreAktarma item in silinecekler)
                    {
                        mesaj += " #" + counter + ":" + item.Adi;// + " BagisTarihi:" + item.BagisTarihi + " BagisMiktari:" + item.Tutar + " IslemTarihi:" + item.IslemTarihi.ConvertToDatetimeEmptyIfNull();
                        item.Delete();

                    }
                    SilinenKayit sk = new SilinenKayit();
                    sk.Silen = currentUser;
                    sk.SilinmeSebebi = counter + " adet kayıt silindi";
                    sk.TabloAdi = "EkstreAktarma_Table";
                    sk.SilinmeTarihi = DateTime.Now.ReturnTRDateFormat();
                    sk.SilinenKayitBilgisi = mesaj;
                    sk.Save();

                    RedirectToPage(ProjeConstants.PAGE_EKSTRE_LIST + "?IslemTarihi=" + IslemTarihiQS + "&AktarilanlarHaric=" + AktarilanlarHaricQS + "&Banka=" + BankaQS + "&Mesaj=true&Islem=Silme&Basarili=true");
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
        private void BankaDDLDoldur()
        {
            List<EkstreAktarmaListItem> returnlist = new List<EkstreAktarmaListItem>();

            if (BankaDDL.SelectedItem == null)
            {
                BankaDDL.Items.Clear();
                ListItem li = new ListItem(ProjeConstants.HEPSI, ProjeConstants.HEPSI_INT.ToString());
                BankaDDL.Items.Add(li);
                BankaTanim pBanka = new BankaTanim();
                List<BankaTanim> list = pBanka.SelectAll<BankaTanim>();
                //TextInfo culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true).TextInfo;
                foreach (BankaTanim banka in list)
                {
                    //string UpperCaseBanka = culturInfo.ToUpper(banka.Banka);
                    BankaDDL.Items.Add(new ListItem(banka.Banka, banka.Id.ToString()));
                }
            }
            if (BankaDDL.Items.FindByValue(ProjeConstants.HEPSI_INT.ToString()) != null)
                BankaDDL.SelectedValue = BankaDDL.Items.FindByValue(ProjeConstants.HEPSI_INT.ToString()).Value;
        }
        private List<EkstreAktarmaListItem> GetDataList()
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            DateTime islemTarihiDateTime = IslemTarihiQS.ConvertToDatetime();
            if (string.IsNullOrEmpty(IslemTarihiQS))
            {
                islemTarihiDateTime = DateTime.Today;
            }
            EkstreAktarma ea = new EkstreAktarma();
            int rowCount = 0;
            string banka = BankaDDL.SelectedItem.Text;
            DataTable dataTable = ea.SelectByIslemTarihi(islemTarihiDateTime, ref rowCount, AktarilanlarHaricQS.ConvertToBool(), banka);

            RowCountLbl.Text = "Kayıt Sayısı : " + rowCount.ToString();
            List<EkstreAktarmaListItem> returnlist = new List<EkstreAktarmaListItem>();
            if (dataTable != null)
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    EkstreAktarmaListItem ekstreAktarmaListItem = new EkstreAktarmaListItem();

                    ekstreAktarmaListItem.EkstreAktarmaId = dataRow["EkstreAktarmaId"].ToString();
                    ekstreAktarmaListItem.BankaAdi = dataRow["BankaAdi"].ToString();
                    ekstreAktarmaListItem.TCKimlikNo = dataRow["TCKimlikNo"].ToString();

                    ekstreAktarmaListItem.AdiSoyadi = dataRow["AdiSoyadi"].ToString();
                    ekstreAktarmaListItem.Telefon = dataRow["Telefon"].ToString();
                    ekstreAktarmaListItem.Telefon1 = dataRow["Telefon1"].ToString();
                    ekstreAktarmaListItem.Telefon2 = dataRow["Telefon2"].ToString();
                    ekstreAktarmaListItem.BagisTarihi = dataRow["BagisTarihi"].ToString().ConvertToDatetimeEmptyIfNull();
                    decimal tutar = dataRow["Tutar"].ConvertToDecimal();
                    string dovizCinsi = dataRow["DovizCinsi"].ToString();
                    ekstreAktarmaListItem.Tutar = tutar > 0 ? tutar.ToString("N", culturInfo) + " " + dovizCinsi : "";
                    bool aktarildiMi = dataRow["AktarildiMi"].ConvertToBool();
                    ekstreAktarmaListItem.AktarildiMi = aktarildiMi.ToString();
                    //ekstreAktarmaListItem.CakismaVarMi = aktarildiMi ? "False" : CakismaKontrolu(ekstreAktarmaListItem.EkstreAktarmaId.ConvertToInt(),ekstreAktarmaListItem.AdiSoyadi, ekstreAktarmaListItem.Telefon1, ekstreAktarmaListItem.Telefon2).ToString();
                    returnlist.Add(ekstreAktarmaListItem);
                }
            }
            return returnlist;
        }
        private bool CakismaKontrolu(int ekstreAktarmaId, string adiSoyadi, string telefon1, string telefon2)
        {
            bool isConflict = false;
            EkstreAktarma ekstreAktarma = new EkstreAktarma();
            ekstreAktarma = ekstreAktarma.Select<EkstreAktarma>(ekstreAktarmaId);
            if (ekstreAktarma == null)
            {
                isConflict = true;
            }
            else
            {


                if (ekstreAktarma.TCKimlikNo != 0) // TC Kimlik numarası var konflict yok.. // TCKİMLİKNO geçerli mi diye kontrol etmek gerekir mi?
                {
                    isConflict = false;
                    return isConflict;
                }
                else if (string.IsNullOrEmpty(telefon1.Trim()) &&
                    string.IsNullOrEmpty(telefon2.Trim()) &&
                    string.IsNullOrEmpty(adiSoyadi.Trim()))// Telefonu ve adı da boşsa konflict vardır.. 
                {
                    isConflict = true;
                    return isConflict;
                }
                else // tc kimlikno yok conflict ihtimali var
                {
                    //NakitBagisci nakitBagisci = new NakitBagisci();
                    //List<NakitBagisci> nakitBagisciList = nakitBagisci.SelectByAd(ekstreAktarma.Adi);
                    //if (nakitBagisciList.Count > 0)//bu kişi bağışçı tablosunda var mı?
                    //{
                    //    //yok ise,  yeni bir bağışçıdır, conflict yok
                    //    //var ise telefon numaraları tutuyor mu
                    //    foreach (NakitBagisci eskiBagisci in nakitBagisciList)
                    //    {
                    //        //numaralardan biri boş sa conflict var değilse farklı kişi olma ihtimali yüksek
                    //        if (string.IsNullOrEmpty(eskiBagisci.Telefon1) || string.IsNullOrEmpty(ekstreAktarma.Telefon1))
                    //        {
                    //            isConflict = true;//numaralardan biri boş
                    //            break;  // yasin gökhan yüksel 03.02.2020 (isConflict true olunca döngü dursun)
                    //        }
                    //        else
                    //        {
                    //            //numaralar aynı ise conflict yok, bu aynı kişi
                    //            //değilse
                    //            if (!eskiBagisci.Telefon1.ReturnEmptyIfNull().Equals(ekstreAktarma.Telefon1.ReturnEmptyIfNull()))
                    //            {
                    //                isConflict = true;//numaralar farklı conflict var
                    //                break;  // yasin gökhan yüksel 03.02.2020 (isConflict true olunca döngü dursun)
                    //            }
                    //        }
                    //    }
                    //}
                }
            }

            return isConflict;
        }
        //private bool CakismaKontrolu1(int kayitAdedi, int nbId, int ekstreAktarmaId, long extreTCKimlikNo, string telefon1, string telefon2, long bTCKimlikNo, string adiSoyadi, string bTelefon1, string bTelefon2)
        //{
        //    bool isConflict = false;
        //    if (nbId == 0)
        //    {
        //        isConflict = false;
        //        return isConflict;
        //    }
        //    else if (extreTCKimlikNo != 0) // TC Kimlik numarası var konflict yok.. // TCKİMLİKNO geçerli mi diye kontrol etmek gerekir mi?
        //    {
        //        isConflict = false;
        //        return isConflict;
        //    }
        //    else if (
        //        string.IsNullOrEmpty(adiSoyadi.Trim())&&
        //        string.IsNullOrEmpty(telefon1.Trim()) &&
        //        string.IsNullOrEmpty(telefon2.Trim()) 
        //        )// Telefonu ve adı da boşsa konflict vardır.. 
        //    {
        //        isConflict = true;
        //        return isConflict;
        //    }
        //    else if (
        //        adiSoyadi.Equals(ProjeConstants.NAKITBAGISCI_BILINMEYEN) &&
        //        string.IsNullOrEmpty(telefon1.Trim()) &&
        //        string.IsNullOrEmpty(telefon2.Trim())
        //        )// Telefonu ve adı da boşsa konflict vardır.. 
        //    {
        //        isConflict = true;
        //        return isConflict;
        //    }
        //    else
        //    {
        //        if (kayitAdedi > 1)
        //        {
        //            isConflict = true;
        //        }
        //        else
        //        {
        //            if ((bTCKimlikNo > 0 && extreTCKimlikNo != bTCKimlikNo))
        //                isConflict = true;
        //            else if (!string.IsNullOrEmpty(telefon1) || !string.IsNullOrEmpty(telefon2) || !string.IsNullOrEmpty(bTelefon1) || !string.IsNullOrEmpty(bTelefon2))
        //            {
        //                if (!
        //                        (string.IsNullOrEmpty(telefon1) ? "#$½%&" : telefon1).Equals(bTelefon1.ReturnEmptyIfNull().ToString()) ||
        //                        (string.IsNullOrEmpty(telefon1) ? "#$½%&" : telefon1).Equals(bTelefon2.ReturnEmptyIfNull().ToString()) ||
        //                        (string.IsNullOrEmpty(telefon2) ? "#$½%&" : telefon2).Equals(bTelefon1.ReturnEmptyIfNull().ToString()) ||
        //                        (string.IsNullOrEmpty(telefon2) ? "#$½%&" : telefon2).Equals(bTelefon2.ReturnEmptyIfNull().ToString()))
        //                {
        //                    isConflict = true;
        //                }
        //            }
        //            else if ((bTCKimlikNo != 0)
        //                    && (string.IsNullOrEmpty(telefon1) && string.IsNullOrEmpty(telefon2) && string.IsNullOrEmpty(bTelefon1) && string.IsNullOrEmpty(bTelefon2))
        //                    )
        //            {
        //                isConflict = true;
        //            }
        //            else if ((nbId != 0)
        //                    && (string.IsNullOrEmpty(telefon1) && string.IsNullOrEmpty(telefon2) && string.IsNullOrEmpty(bTelefon1) && string.IsNullOrEmpty(bTelefon2))
        //                    )
        //            {
        //                isConflict = true;
        //            }
        //        }

        //    }
        //    return isConflict;
        //}
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
            List<NakitBagisci> list = new List<NakitBagisci>();
            GridView1.DataSource = GetDataList(); ;
            GridView1.DataBind();

            Page.Response.Clear();
            Page.Response.Buffer = true;
            Page.Response.AddHeader("content-disposition",
             "attachment;filename=EkstreAktarmaListesi_" + IslemTarihiQS.ConvertToDatetimeEmptyIfNull() + ".xls");
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
        private class EkstreAktarmaListItem
        {
            public string EkstreAktarmaId { get; set; }
            public string TCKimlikNo { get; set; }
            public string Telefon { get; set; }
            public string Telefon1 { get; set; }
            public string Telefon2 { get; set; }
            public string AdiSoyadi { get; set; }
            public string Adres { get; set; }
            public string Aciklama { get; set; }
            public string BagisTarihi { get; set; }
            public string BankaAdi { get; set; }
            public string Tutar { get; set; }
            public string AktarildiMi { get; set; }
            public string CakismaVarMi { get; set; }

        }
        protected void IslemTarihiTxt_TextChanged(object sender, EventArgs e)
        {
            IslemTarihiQS = IslemTarihiTxt.Text.ConvertToDatetimeEmptyIfNull();
            RedirectToPage(ProjeConstants.PAGE_EKSTRE_LIST + "?IslemTarihi=" + IslemTarihiQS + "&AktarilanlarHaric=" + AktarilanlarHaricQS + "&Banka=" + BankaQS);
        }
        protected void AktarilanlarHaricChk_CheckedChanged(object sender, EventArgs e)
        {
            AktarilanlarHaricQS = AktarilanlarHaricChk.Checked.ToString();
            RedirectToPage(ProjeConstants.PAGE_EKSTRE_LIST + "?IslemTarihi=" + IslemTarihiQS + "&AktarilanlarHaric=" + AktarilanlarHaricQS + "&Banka=" + BankaQS);
        }
        protected void BankaDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            BankaQS = BankaDDL.SelectedItem.Value;
            RedirectToPage(ProjeConstants.PAGE_EKSTRE_LIST + "?IslemTarihi=" + IslemTarihiQS + "&AktarilanlarHaric=" + AktarilanlarHaricQS + "&Banka=" + BankaQS);
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

            string value = paramArray.Value;
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
        protected void TumunuSecChk_CheckedChanged(object sender, EventArgs e)
        {
            SelectAllQS = TumunuSecChk.Checked.ToString();
            KayitGetir();
        }
    }
}