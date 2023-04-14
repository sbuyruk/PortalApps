using Model.NBYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;
using ListItem = System.Web.UI.WebControls.ListItem;

namespace NBYS_WebParts.FTKIslemleriWP
{
    [ToolboxItemAttribute(false)]
    public partial class FTKIslemleriWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public FTKIslemleriWP()
        {
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string FTKIslemIdQS
        {
            get
            {
                if (ViewState["FTKIslemId"] == null)
                {
                    if (Page.Request.QueryString["FTKIslemId"] != null)
                    {
                        ViewState["FTKIslemId"] = Page.Request.QueryString["FTKIslemId"];
                    }
                    else
                    {
                        ViewState["FTKIslemId"] = string.Empty;
                    }
                }
                return ViewState["FTKIslemId"].ToString();
            }
            set
            {
                ViewState["FTKIslemId"] = value;
            }
        }
        private string ValiIdQS
        {
            get
            {
                if (ViewState["ValiId"] == null)
                {
                    if (Page.Request.QueryString["ValiId"] != null)
                    {
                        ViewState["ValiId"] = Page.Request.QueryString["ValiId"];
                    }
                    else
                    {
                        ViewState["ValiId"] = string.Empty;
                    }
                }
                return ViewState["ValiId"].ToString();
            }
            set
            {
                ViewState["ValiId"] = value;
            }
        }
        private string IliIdQS
        {
            get
            {

                if (ViewState["IliId"] == null)
                {
                    if (Page.Request.QueryString["IliId"] != null)
                    {
                        ViewState["IliId"] = Page.Request.QueryString["IliId"];
                    }
                    else
                    {
                        ViewState["IliId"] = string.Empty;
                    }
                }
                return ViewState["IliId"].ToString();
            }

            set
            {
                ViewState["IliId"] = value;
            }
        }
        private string IlcesiIdQS
        {
            get
            {

                if (ViewState["IlcesiId"] == null)
                {
                    if (Page.Request.QueryString["IlcesiId"] != null)
                    {
                        ViewState["IlcesiId"] = Page.Request.QueryString["IlcesiId"];
                    }
                    else
                    {
                        ViewState["IlcesiId"] = string.Empty;
                    }
                }
                return ViewState["IlcesiId"].ToString();
            }

            set
            {
                ViewState["IlcesiId"] = value;
            }
        }
        private List<FTKListItem> FTKListQS
        {
            get
            {

                if (ViewState["FTKList"] == null)
                {
                    if (Page.Request.QueryString["FTKList"] != null)
                    {
                        ViewState["FTKList"] = Page.Request.QueryString["FTKList"];
                    }
                    else
                    {
                        ViewState["FTKList"] = new List<FTKListItem>();
                    }
                }
                return (List<FTKListItem>)ViewState["FTKList"];
            }

            set
            {
                ViewState["FTKList"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack) // sayfa ilk kez açılıyorsa (bu sayfanın içindeki butona basılma anı hariç)
                {

                    YonergeLnk.HRef = NBYSOrtak.YonergeURLGetir(ProjeConstants.PARAM_FTKYONERGE, ProjeConstants.PAGE_FTKISLEMLERI);
                    IlilceBolgeDDLDoldur();
                    FTKListQS.Clear();
                    if (string.IsNullOrEmpty(FTKIslemIdQS))
                    {
                        GirisiAc();
                    }
                    else
                    {
                        DuzenleAc();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exhelper = new ExceptionHelper(ex);
                exhelper.PublishException();
            }
        }
        private bool KuruluFTKVarMi(int ili, int ilcesi)
        {
            FTK ftk = new FTK();
            List<FTK> list = ftk.SelectSonFTKListesiByIliIlcesiReturnList(ili, ilcesi);
            return list.Count > 0;
        }
        private bool UyeKaydiVarMi(int ili, int ilcesi)
        {
            FTKKisi ftkKisi = new FTKKisi();
            List<FTKKisi> list = ftkKisi.SelectFTKUyeleriByIliIlcesiReturnList(ili, ilcesi, 0, true);
            return list.Count > 0;
        }
        private void IlilceBolgeDDLDoldur()
        {
            IliDDL.Items.Clear();
            Il newil = new Il();
            List<Il> list = newil.SelectAll<Il>();
            IliDDL.Items.Add(new System.Web.UI.WebControls.ListItem(string.Empty));
            foreach (Il il in list)
            {
                if (string.IsNullOrEmpty(il.IlAdi.Trim()))
                    continue;
                IliDDL.Items.Add(new System.Web.UI.WebControls.ListItem(il.IlAdi, il.Id.ToString()));
            }
            UtilityHelper.SetDDLValue(IliDDL, IliIdQS);
            IlceDDLDoldur();
            BolgeTxtDoldur();
            UtilityHelper.SetDDLValue(IlcesiDDL, IlcesiIdQS);

        }
        private void IlceDDLDoldur()
        {
            IlcesiDDL.Items.Clear();
            Ilce pilce = new Ilce();

            List<Ilce> list = pilce.SelectByIlId(IliDDL.SelectedValue.ConvertToInt());
            ListItem li0 = new ListItem(ProjeConstants.VALILIK, ProjeConstants.VALILIK_INT.ToString());
            IlcesiDDL.Items.Add(li0);
            foreach (Ilce ilce in list)
            {
                if (ilce.IlceAdi.ToUpper().Equals(ProjeConstants.ILCE_MERKEZ.ToUpper()))
                    continue;
                IlcesiDDL.Items.Add(new ListItem(ilce.IlceAdi, ilce.Id.ToString()));
            }
        }
        private void BolgeTxtDoldur()
        {
            Il il = new Il();
            il = il.Select<Il>(IliDDL.SelectedItem.Value.ConvertToInt());
            if (il != null)
            {
                SorumluBolgeTxt.Text = il.Bolge;
            }
        }
        private void GirisiAc()
        {
            FTKIslemleriFormunuDoldur();
            FTKListesiniVeriTabanindanDoldur();
            if (string.IsNullOrEmpty(FTKIslemIdQS))
            {
                MessageHelper.PublishMessage("Yeni FTK bilgilerini girerek kayıt yapabilirsiniz.", ProjeConstants.MESAJ_BILGI, 2000);
            }
        }
        private void FTKListesiniVeriTabanindanDoldur()
        {
            TabloOlustur();
        }
        private void TabloOlustur()
        {
            var jsonData = TabloJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string CreateDataTable(string jsonData)
        {
            string tableString = @"
            if ( $.fn.DataTable.isDataTable('#CustomDataTable') ) {
              $('#CustomDataTable').DataTable().destroy();
            }

            $('#CustomDataTable tbody').empty();

            jQuery('#CustomDataTable').DataTable({
            data: " + jsonData + @",
            columns: [
                { data: 'FTKGoreviId' },
                { data: 'FTKKisiId' },
                { data: 'AdiSoyadi'},
                { data: 'Gorevi'},                
                { data: 'Unvani'},                
                { data: 'KartNo'},
                { data: 'UyelikDurumu'},
                { data: 'Duzenle'},
            ],
            'createdRow': function(row, data, dataIndex) {
                $(row).addClass(data.Class);
            },//set row color
            'columnDefs': [
                { type: 'turkish', targets: [2,3,4] },
                { 'width': '20%', 'targets': [2,3] },
                {
                    'targets': [0],
                    'visible': false,
                    'searchable': false
                },
            ],
            'order': [[6, 'asc'],[0, 'asc'],[1, 'asc'],[2, 'asc']],
            'scrollY': '300px',
            'scrollCollapse': true,
            'paging': false,
            'language': {
                'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                'decimal': ',',
                'thousands': '.'
            },
            responsive: true,
            dom: 'srt',
            });
            ";

            return tableString;
        }
        private string TabloJson()
        {
            string jSon = string.Empty;

            try
            {
                FTKListQS = GetDataByIlIlce(AktifOlmayanlariGostermeChk.Checked);
                var serializer = new JavaScriptSerializer();
                jSon = serializer.Serialize(FTKListQS);
            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper();
                exceptionHelper.Exceptions.Add(exception);
                exceptionHelper.PublishException();
            }
            return jSon;
        }
        private List<FTKListItem> GetDataByIlIlce(bool aktif)
        {
            int ili = IliDDL.SelectedItem.Value.ConvertToInt();
            int ilcesi = IlcesiDDL.SelectedItem.Value.ConvertToInt();
            FTKKisi ftkKisiDao = new FTKKisi();
            DataTable dataTable = ftkKisiDao.SelectFTKUyeleriByIliIlcesiReturnDataTable(ili, ilcesi, FTKIslemIdQS.ConvertToInt(), aktif);
            List<FTKListItem> list = new List<FTKListItem>();
            if (dataTable != null)
            {
                int siraNo = 1;
                foreach (DataRow row in dataTable.Rows)
                {
                    int ftkkisiId = row["FTKKisiId"].ConvertToInt();
                    string adi = row["Adi"].ToString();
                    string soyadi = row["Soyadi"].ToString();
                    int ftkGoreviId = row["FTKGorevi"].ReturnZeroIfNull().ConvertToInt();
                    string uyelikDurumu = row["UyelikDurumu"].ToString();
                    string unvani = row["Unvani"].ToString();
                    string kartNo = row["KartNo"].ReturnEmptyIfNull().ToString();
                    string aciklama = row["Aciklama"].ReturnEmptyIfNull().ToString();

                    FTKListItem katilimciItem = new FTKListItem();
                    katilimciItem.FTKKisiId = ftkkisiId;
                    katilimciItem.FTKGoreviId = ftkGoreviId;
                    katilimciItem.AdiSoyadi = adi + " " + soyadi;
                    katilimciItem.Gorevi = ParseGorevi(ftkGoreviId);
                    katilimciItem.Unvani = unvani;
                    katilimciItem.KartNo = kartNo;
                    katilimciItem.Aciklama = aciklama;
                    katilimciItem.SiraNo = siraNo++; ;

                    katilimciItem.UyelikDurumu = uyelikDurumu;

                    katilimciItem.Duzenle = "<a href='#' class='btn btn-outline-primary' onclick=FTKKisiDuzenleBtnClick(" + ftkkisiId + ")>Düzenle</a>";

                    //FTK ftk = new FTK();
                    //List<FTK> ftkList = ftk.SelectSonFTKListesiByIliIlcesiReturnList(ili, ilcesi);
                    //if (ftkList.Count > 0)
                    //{
                    //    ftk = ftkList[0];
                    //    bool ayniMi = ftk.Adi.Equals(adi) || ftk.Soyadi.Equals(soyadi) || ftk.FTKGorevi == ftkGoreviId;
                    //    if (ayniMi)
                    //    {
                    //        katilimciItem.Class = "katilimci-degisti";
                    //    }


                    //}
                    katilimciItem.Class = !uyelikDurumu.Equals(ProjeConstants.FTK_UYELIK_DURUMU_AKTIF) ? "aktif-degil" : string.Empty;

                    list.Add(katilimciItem);
                    FTKListQS.Add(katilimciItem);

                }
            }
            return list;
        }
        private string ParseGorevi(int gorevi)
        {
            if (gorevi == ProjeConstants.FTK_GOREVI_FAHRIBASKAN_INT)
            {
                return ProjeConstants.FTK_GOREVI_FAHRIBASKAN;
            }
            else if (gorevi == ProjeConstants.FTK_GOREVI_BASKAN_INT)
            {
                return ProjeConstants.FTK_GOREVI_BASKAN;
            }
            else if (gorevi == ProjeConstants.FTK_GOREVI_GENELSEKRETER_INT)
            {
                return ProjeConstants.FTK_GOREVI_GENELSEKRETER;
            }
            else
            {
                return ProjeConstants.FTK_GOREVI_UYE;
            }

        }
        private void DuzenleAc()
        {
            if (FTKIslemIdQS.ConvertToInt() > 0)
            {
                FTKIslemleriFormunuDoldur();
                FTKListesiniVeriTabanindanDoldur();
            }
            else
            {
                GirisiAc();
                MessageHelper.PublishMessage("Toplantı bulunamadı. Yeni toplantı girebilirsiniz.", ProjeConstants.MESAJ_BILGI, 2000);
            }
        }
        private void FTKIslemleriFormunuDoldur()
        {
            FTKKurulusTarihiTxt.Text = string.Empty;
            FTKGuncellemeTarihiTxt.Text = string.Empty;
            AciklamaTxt.Text = string.Empty;

            int ili = IliDDL.SelectedItem.Value.ConvertToInt();
            int ilcesi = IlcesiDDL.SelectedItem.Value.ConvertToInt();

            FTKIslem ftkislemleri = new FTKIslem();
            ftkislemleri = ftkislemleri.SelectByIliIlcesi(ili, ilcesi);

            if (ftkislemleri == null)
            {
                FTKIslemIdQS = string.Empty;

                IdLbl.Text = string.Empty;

                SorumluBolgeTxt.Enabled = false;
                FTKKurulusTarihiTxt.Text = DateTime.Today.ConvertToDatetimeEmptyIfNull();
                FTKGuncellemeTarihiTxt.Text = DateTime.Today.ConvertToDatetimeEmptyIfNull();
                AciklamaTxt.Text = string.Empty;

            }
            else
            {
                FTKIslemIdQS = ftkislemleri.Id.ToString();
                TitleLbl.Text = "FTK İşlemleri Düzenle";
                IdLbl.Text = " ( FTK No: " + ftkislemleri.Id.ToString() + " )";
                SorumluBolgeTxt.Enabled = false;
                FTKKurulusTarihiTxt.Text = ftkislemleri.KurulusTarihi.ConvertToDatetimeEmptyIfNull();
                FTKGuncellemeTarihiTxt.Text = ftkislemleri.GuncellemeTarihi.ConvertToDatetimeEmptyIfNull();

                AciklamaTxt.Text = ftkislemleri.Aciklama;

                FTK ftk = new FTK();
                List<FTK> ftkList = ftk.SelectSonFTKListesiByIliIlcesiReturnList(ili, ilcesi);
                if (ftkList.Count > 0)
                {
                    ftk = ftkList[0];
                    FTKKurulusTarihiTxt.Font.Bold = ftk.KurulusTarihi == ftkislemleri.KurulusTarihi;
                    FTKGuncellemeTarihiTxt.Font.Bold = ftk.GuncellemeTarihi != ftkislemleri.GuncellemeTarihi;
                }
            }
            ValiTxt.Text = ValiDoldur(ili);
            KaymakamTxt.Text = KaymakamDoldur(ili, ilcesi);
            BaslikGuncelle();
        }
        private string ValiDoldur(int ili)
        {
            string adiSoyadi = string.Empty;
            if (ili > 0)
            {
                FTKKisi kisi = new FTKKisi();
                kisi = kisi.SelectVali(ili);

                if (kisi == null)
                {
                    ValiIdQS = string.Empty;
                }
                else
                {
                    ValiIdQS = kisi.Id.ToString();
                    adiSoyadi = kisi.Adi + " " + kisi.Soyadi;
                }
            }
            else
            {
                MessageHelper.PublishMessage("Önce İl seçiniz", ProjeConstants.MESAJ_BILGI, 2000);
            }

            return adiSoyadi;
        }
        private string KaymakamDoldur(int ili, int ilcesi)
        {
            string adiSoyadi = string.Empty;

            if (ili > 0 && ilcesi > 0)
            {
                FTKKisi kisi = new FTKKisi();
                kisi = kisi.SelectKaymakam(ili, ilcesi);

                if (kisi != null)
                {
                    adiSoyadi = kisi.Adi + " " + kisi.Soyadi;
                }

            }

            return adiSoyadi;
        }
        protected void KaydetBtn_Click(object sender, EventArgs e)
        {
            ModalTarihDiv.Attributes["style"] = "display:block";
            KaydetNowBtn.Visible = true;
            TumununGoreviniSonlandirNowBtn.Visible = false;
            ModalTabloOlustur(true);
            UtilityHelper.ScriptCalistir("ModalFTKListesiAc()");
        }
        #region Modal FTK Listesi
        private void ModalTabloOlustur(bool aktifMi)
        {
            ModalKurulusTarihiLbl.Text = FTKKurulusTarihiTxt.Text;
            ModalGuncellemeTarihiLbl.Text = FTKGuncellemeTarihiTxt.Text;
            var jsonData = ModalTabloJson(aktifMi); //veri çekilip json a çeviriliyor
            var jsString = CreateModalDataTable(jsonData); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string CreateModalDataTable(string jsonData)
        {
            string tableString = @"
            if ( $.fn.DataTable.isDataTable('#CustomModalDataTable') ) {
              $('#CustomModalDataTable').DataTable().destroy();
            }

            $('#CustomModalDataTable tbody').empty();

            jQuery('#CustomModalDataTable').DataTable({
            data: " + jsonData + @",
            columns: [
                { data: 'FTKKisiId' },
                { data: 'AdiSoyadi'},
                { data: 'Gorevi'},                
            ],
            'createdRow': function(row, data, dataIndex) {
                $(row).addClass(data.Class);
            },//set row color
            'order': [[2, 'asc'],[0, 'asc'],[1, 'asc']],
            'scrollY': '500px',
            'scrollCollapse': true,
            'paging': false,
            'language': {
                'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                'decimal': ',',
                'thousands': '.'
            },
            responsive: true,
            dom: 'srt',
            });
            ";

            return tableString;
        }
        private string ModalTabloJson(bool aktif)
        {
            string jSon = string.Empty;

            try
            {
                FTKListQS = GetDataByIlIlce(aktif);
                var serializer = new JavaScriptSerializer();
                jSon = serializer.Serialize(FTKListQS);
            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper();
                exceptionHelper.Exceptions.Add(exception);
                exceptionHelper.PublishException();
            }
            return jSon;
        }
        protected void KaydetNowBtn_Click(object sender, EventArgs e)
        {
            KaydetVeyaGuncelle();
        }
        private void KaydetVeyaGuncelle()
        {
            /***
             * 1 Seçilen il ve seçilen ilçe için FTKIslem_Table'da kayıt var mı
             *  Varsa
             *      2 Güncelleme yap
             *  Yoksa 
             *      3 kaydet 
             ***/
            int ili = IliDDL.SelectedItem.Value.ConvertToInt();
            int ilcesi = IlcesiDDL.SelectedItem.Value.ConvertToInt();
            FTK ftkDao = new FTK();
            ftkDao = ftkDao.SelectByIliIlcesi(ili, ilcesi, FTKGuncellemeTarihiTxt.Text.ConvertToDatetime());
            if ((ftkDao != null)
                && (!KayitDuzeltmesiChk.Checked))
            {
                MessageHelper.PublishMessage("Bu FTK " + FTKGuncellemeTarihiTxt.Text + " tarihinde zaten güncellenmiştir. FTK listesinde kayıt düzeltmek istiyorsanız, 'Bu Bir Kayıt Düzeltmesidir' hanesini seçili hale getiriniz.", ProjeConstants.MESAJ_HATA);
            }
            else
            {
                FTKIslem ftkIslem = new FTKIslem();
                ftkIslem = ftkIslem.SelectByIliIlcesi(ili, ilcesi);//1
                if (ftkIslem != null)
                {
                    //2 ve //4
                    FTKIstemleriniGuncelle(ftkIslem);
                }
                else
                {
                    //3 ve //4
                    FTKIstemleriniKaydet();
                }
                //4
                BaslikGuncelle();
            }
        }
        private void FTKIstemleriniGuncelle(FTKIslem ftkIslemleri)
        {
            if (ftkIslemleri != null)
            {
                try
                {
                    ftkIslemleri.Ili = IliDDL.SelectedItem.Value.ConvertToInt();
                    ftkIslemleri.Ilcesi = IlcesiDDL.SelectedItem.Value.ConvertToInt();

                    ftkIslemleri.SorumluBolge = SorumluBolgeTxt.Text;
                    ftkIslemleri.KurulusTarihi = FTKKurulusTarihiTxt.Text.ConvertToDatetime();
                    ftkIslemleri.GuncellemeTarihi = FTKGuncellemeTarihiTxt.Text.ConvertToDatetime();
                    ftkIslemleri.Aciklama = AciklamaTxt.Text;
                    ftkIslemleri.Degistiren = UtilityHelper.GetCurrentUserLoginName();

                    if (string.IsNullOrEmpty(IliDDL.SelectedItem.ToString()))
                    {
                        MessageHelper.PublishMessage("İl Boş Olamaz.", ProjeConstants.MESAJ_HATA, 2000);
                    }
                    else
                    {
                        bool guncellendiMi = ftkIslemleri.Update();
                        if (guncellendiMi)
                        {
                            FTKIslemIdQS = ftkIslemleri.Id.ToString();
                            if (FTKTablosunaKaydetveyaGuncelle(ftkIslemleri))
                            {
                                DuzenleAc();
                                MessageHelper.PublishMessage("FTK İşlemi güncellendi.", ProjeConstants.MESAJ_BASARILI, 2000);
                            }
                        }
                    }
                    UtilityHelper.ScriptCalistir("ModalFTKListesiKapat();");
                }
                catch (Exception exception)
                {

                    ExceptionHelper eh = new ExceptionHelper(exception);
                    eh.PublishException();

                }
            }
        }
        private bool FTKTablosunaKaydetveyaGuncelle(FTKIslem ftkIslem)
        {
            bool kaydedildiMi = false;
            if (KayitDuzeltmesiChk.Checked)
            {
                FTK ftkDao = new FTK();
                List<FTK> list = ftkDao.SelectSonFTKListesiByIliIlcesiReturnList(IliDDL.SelectedItem.Value.ConvertToInt(), IlcesiDDL.SelectedItem.Value.ConvertToInt());

                if (list.Count < 1)
                {
                    MessageHelper.PublishMessage("Düzeltmesi yapılacak bir FTK kaydı bulunamadı.", ProjeConstants.MESAJ_HATA);
                }
                else
                {
                    FTK ftk = list[0];
                    int ftkIslemId = ftk.FTKIslemId;
                    int sayac = ftk.Sayac;
                    if (ftkDao.DeleteByIslemIdSayac(ftkIslemId, sayac))
                    {
                        kaydedildiMi = FTKTablosunaKaydet(ftkIslem, sayac);
                    }
                }
            }
            else
            {
                kaydedildiMi = FTKTablosunaKaydet(ftkIslem, 0);
            }
            return kaydedildiMi;
        }
        private bool FTKTablosunaKaydet(FTKIslem ftkIslem, int sayac)
        {
            bool kaydedildiMi = false;
            FTKKisi fTKKisi = new FTKKisi();
            List<FTKKisi> UyeListesi = fTKKisi.SelectFTKUyeleriByIliIlcesiReturnList(ftkIslem.Ili, ftkIslem.Ilcesi, ftkIslem.Id, true);
            string aciklama = string.Empty;
            if (sayac > 0)
            {
                aciklama = DateTime.Now.ConvertToTimeSpanReturnInHHmm() + " tarihinde " + UtilityHelper.GetCurrentUserLoginName() + " tarafından kayıt düzeltmesi yapıldı.";
            }
            sayac = sayac > 0 ? sayac : SayacHesapla(ftkIslem.Ili, ftkIslem.Ilcesi) + 1;

            //FTK_Table a kaydet
            foreach (var item in UyeListesi)
            {
                FTK ftk = new FTK();
                ftk.FTKIslemId = ftkIslem.Id;
                ftk.Ili = IliDDL.SelectedItem.Value.ConvertToInt();
                ftk.Ilcesi = IlcesiDDL.SelectedItem.Value.ConvertToInt();
                ftk.Bolge = ftkIslem.SorumluBolge;
                ftk.KurulusTarihi = ftkIslem.KurulusTarihi;
                ftk.GuncellemeTarihi = ftkIslem.GuncellemeTarihi;
                ftk.FTKGorevi = ParseGorevi(item.FTKGorevi);
                ftk.Adi = item.Adi;
                ftk.Soyadi = item.Soyadi;
                ftk.Unvani = item.Unvani;
                ftk.Telefon = item.Telefon1 +
                    (!string.IsNullOrEmpty(item.Telefon1) && !string.IsNullOrEmpty(item.Telefon2) ? " / " : string.Empty) +
                    item.Telefon2;
                ftk.KartNo = item.KartNo;
                ftk.Sayac = sayac;
                ftk.KisiId = item.Id;
                ftk.Olusturan = UtilityHelper.GetCurrentUserLoginName();
                ftk.Aciklama = ftk.Aciklama + "</br>" + aciklama;
                kaydedildiMi = ftk.Save() > 0;
            }
            return kaydedildiMi;
        }
        private int SayacHesapla(int ili, int ilcesi)
        {
            FTK ftk = new FTK();
            int sayac = ftk.SelectMaxSayac(ili, ilcesi);
            return sayac;
        }
        private void FTKIstemleriniKaydet()
        {
            FTKIslem ftkIslemleri = new FTKIslem();
            try
            {
                ftkIslemleri.Ili = IliDDL.SelectedItem.Value.ConvertToInt();
                ftkIslemleri.Ilcesi = IlcesiDDL.SelectedItem.Value.ConvertToInt();

                ftkIslemleri.SorumluBolge = SorumluBolgeTxt.Text;
                ftkIslemleri.KurulusTarihi = FTKKurulusTarihiTxt.Text.ConvertToDatetime();
                ftkIslemleri.GuncellemeTarihi = FTKGuncellemeTarihiTxt.Text.ConvertToDatetime();
                ftkIslemleri.Aciklama = AciklamaTxt.Text;
                ftkIslemleri.Olusturan = UtilityHelper.GetCurrentUserLoginName();

                if (string.IsNullOrEmpty(IliDDL.SelectedItem.ToString()))
                {
                    MessageHelper.PublishMessage("İl Boş Olamaz.", ProjeConstants.MESAJ_HATA, 2000);
                }
                else
                {
                    int ftkIslemId = ftkIslemleri.Save();
                    FTKIslemIdQS = ftkIslemId.ToString();
                    if (ftkIslemId > 0)
                    {
                        FTKIslemIdQS = ftkIslemleri.Id.ToString();
                        if (FTKTablosunaKaydetveyaGuncelle(ftkIslemleri))
                        {
                            DuzenleAc();
                            MessageHelper.PublishMessage("FTK İşlemi kaydedildi.", ProjeConstants.MESAJ_BASARILI, 2000);
                        }

                    }
                }
            }
            catch (Exception exception)
            {
                ExceptionHelper eh = new ExceptionHelper(exception);
                eh.PublishException();
            }
        }
        #endregion 
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
        protected void IliDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            IliIdQS = IliDDL.SelectedItem.Value;
            IlceDDLDoldur();
            BolgeTxtDoldur();
            FTKIslemleriFormunuDoldur();
            FTKListesiniVeriTabanindanDoldur();

        }
        protected void IlcesiDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            IlcesiIdQS = IlcesiDDL.SelectedItem.Value;
            FTKIslemleriFormunuDoldur();
            FTKListesiniVeriTabanindanDoldur();
        }
        private void BaslikGuncelle()
        {
            bool ftkVarmi = KuruluFTKVarMi(IliDDL.SelectedItem.Value.ConvertToInt(), IlcesiDDL.SelectedItem.Value.ConvertToInt());
            bool uyeVarMi = UyeKaydiVarMi(IliDDL.SelectedItem.Value.ConvertToInt(), IlcesiDDL.SelectedItem.Value.ConvertToInt());

            if (ftkVarmi)
            {
                TitleLbl.Text = IliDDL.SelectedItem.Text + " " + IlcesiDDL.SelectedItem.Text + " FTK ";
                TitleLbl.CssClass = "col-form-label text-primary font-weight-bold mb-1";
                KaydetBtn.Text = IliDDL.SelectedItem.Text + " " + IlcesiDDL.SelectedItem.Text + " FTK Güncelle";
                KaydetBtn.CssClass = "btn btn-outline-primary font-weight-bold ";
                FTKYazilariBtn.Visible = true;
                KaydetBtn.Visible = true;
                UyelerinGoreviniSonlandirBtn.Visible = true;

                ModalFTKListesiBaslikLbl.Text = IliDDL.SelectedItem.Text + " " + IlcesiDDL.SelectedItem.Text + " FTK Listesi Güncellenecek";
                ModalFTKListesiBaslikLbl.CssClass = "col-form-label text-primary font-weight-bold mb-1";
                KaydetNowBtn.Text = IliDDL.SelectedItem.Text + " " + IlcesiDDL.SelectedItem.Text + " FTK Güncelle";
                KaydetNowBtn.CssClass = "btn btn-outline-primary font-weight-bold ";
                KayitDuzeltmeDiv.Attributes["style"] = "display:block";
            }
            else if (uyeVarMi)
            {
                TitleLbl.Text = IliDDL.SelectedItem.Text + " " + IlcesiDDL.SelectedItem.Text + " FTK ";
                TitleLbl.CssClass = "col-form-label text-success font-weight-bold mb-1";
                KaydetBtn.Text = IliDDL.SelectedItem.Text + " " + IlcesiDDL.SelectedItem.Text + " FTK Oluştur";
                KaydetBtn.CssClass = "btn btn-outline-success font-weight-bold ";
                FTKYazilariBtn.Visible = false;
                KaydetBtn.Visible = true;
                UyelerinGoreviniSonlandirBtn.Visible = true;

                ModalFTKListesiBaslikLbl.Text = IliDDL.SelectedItem.Text + " " + IlcesiDDL.SelectedItem.Text + " FTK Listesi Oluşturulacak";
                ModalFTKListesiBaslikLbl.CssClass = "col-form-label text-success font-weight-bold mb-1";
                KaydetNowBtn.Text = IliDDL.SelectedItem.Text + " " + IlcesiDDL.SelectedItem.Text + " FTK Oluştur";
                KaydetNowBtn.CssClass = "btn btn-outline-success font-weight-bold ";

                KayitDuzeltmeDiv.Attributes["style"] = "display:none";
            }
            else
            {
                TitleLbl.Text = IliDDL.SelectedItem.Text + " " + IlcesiDDL.SelectedItem.Text + " FTK ";
                TitleLbl.CssClass = "col-form-label text-danger font-weight-bold mb-1";
                KaydetBtn.Text = IliDDL.SelectedItem.Text + " " + IlcesiDDL.SelectedItem.Text + " FTK Oluştur";
                KaydetBtn.CssClass = "btn btn-outline-danger font-weight-bold ";
                FTKYazilariBtn.Visible = false;
                KaydetBtn.Visible = false;
                UyelerinGoreviniSonlandirBtn.Visible = false;

                ModalFTKListesiBaslikLbl.Text = IliDDL.SelectedItem.Text + " " + IlcesiDDL.SelectedItem.Text + " FTK Listesi Oluşturulacak";
                ModalFTKListesiBaslikLbl.CssClass = "col-form-label text-danger font-weight-bold mb-1";
                KaydetNowBtn.Text = IliDDL.SelectedItem.Text + " " + IlcesiDDL.SelectedItem.Text + " FTK Oluştur";
                KaydetNowBtn.CssClass = "btn btn-outline-danger font-weight-bold ";

                KayitDuzeltmeDiv.Attributes["style"] = "display:none";
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
        protected void FTKKisiEkleBtn_Click(object sender, EventArgs e)
        {

            RedirectToPage(ProjeConstants.PAGE_FTKKISI_GIRISI + "?IliId=" + IliDDL.SelectedItem.Value + "&IlcesiId=" + IlcesiDDL.SelectedItem.Value);
        }
        protected void FTKKisiDuzenleBtn_Click(object sender, EventArgs e)
        {
            int ftkUyeId = paramFTKIslemleriUyeIdLbl.Value.ConvertToInt();
            RedirectToPage(ProjeConstants.PAGE_FTKKISI_GIRISI + "?IliId=" + IliIdQS + "&IlcesiId=" + IlcesiIdQS + "&FTKKisiId=" + ftkUyeId);
        }
        [Serializable]
        private class FTKListItem
        {
            public int FTKKisiId { get; set; }
            public int FTKGoreviId { get; set; }
            public string AdiSoyadi { get; set; }
            public string Gorevi { get; set; }
            public string Unvani { get; set; }
            public string UyelikDurumu { get; set; }
            public string Aciklama { get; set; }
            public string Duzenle { get; set; }
            public int SiraNo { get; set; }
            public string KartNo { get; set; }
            public string BaskanSec { get; set; }
            public string GenSekSec { get; set; }
            public string UyeSec { get; set; }
            public string Cikar { get; set; }
            public string Class { get; set; }
        }
        protected void AktifOlmayanlariGostermeChk_CheckedChanged(object sender, EventArgs e)
        {
            FTKListesiniVeriTabanindanDoldur();
        }
        protected void FTKListesiBtn_Click(object sender, EventArgs e)
        {

            RedirectToPage(ProjeConstants.PAGE_FTK_LIST + "?IliId=" + IliDDL.SelectedItem.Value + "&IlcesiId=" + IlcesiDDL.SelectedItem.Value);
        }
        protected void BolgelereGoreFTKRaporuBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_BOLGELEREGORE_FTK_DAGILIMI);
        }

        protected void FTKYazilariBtnBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_FTK_YAZILARI + "?IliId=" + IliDDL.SelectedItem.Value + "&IlcesiId=" + IlcesiDDL.SelectedItem.Value);
        }



        protected void TumununGoreviniSonlandirNowBtn_Click(object sender, EventArgs e)
        {
            string idler = "";
            foreach (var item in FTKListQS)
            {
                idler += item.FTKKisiId + ",";
            }
            idler = idler.Length > 0 ? "(" + idler.Substring(0, idler.Length - 1) + ")" : string.Empty;
            if (!string.IsNullOrEmpty(idler))
            {
                FTKKisi ftkKisi = new FTKKisi();
                bool guncellendi = ftkKisi.UpdateAktifByIdList(idler);
                if (guncellendi)
                {
                    FTKListesiniVeriTabanindanDoldur();
                    MessageHelper.PublishMessage("Üyelerin görevleri sonlandırıldı.", ProjeConstants.MESAJ_BASARILI, 2000);
                }
                else
                {
                    MessageHelper.PublishMessage("Üyelerin görevleri sonlandırılamadı.", ProjeConstants.MESAJ_HATA);
                }
            }
        }

        protected void UyelerinGoreviniSonlandirBtn_Click(object sender, EventArgs e)
        {
            //ModalTarihDiv.Attributes["style"] = "display:block";
            //KaydetNowBtn.Visible = true;
            //TumununGoreviniSonlandirNowBtn.Visible = false;
            //ModalTabloOlustur(true);
            //UtilityHelper.ScriptCalistir("ModalFTKListesiAc()"); ;


            ModalTarihDiv.Attributes["style"] = "display:none";
            KaydetNowBtn.Visible = false;
            TumununGoreviniSonlandirNowBtn.Visible = true;
            ModalFTKListesiBaslikLbl.Text = "Üyelerin Görevi Sonlandırılacak";
            ModalTabloOlustur(true);
            UtilityHelper.ScriptCalistir("ModalFTKListesiAc()");
        }
    }
}
