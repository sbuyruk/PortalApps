using Microsoft.SharePoint;
using Model.IKYS;
using Model.Ortak;
using Model.Portal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Portal_WebParts.DuyuruGirisiWP
{
    [ToolboxItemAttribute(false)]
    public partial class DuyuruGirisiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public DuyuruGirisiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private List<PersonelItem> SecilenPersonelList
        {
            get
            {
                if (ViewState["SecilenPersonelList"] == null)
                {
                    ViewState["SecilenPersonelList"] = new List<PersonelItem>().ToList();//personel.SelectCalisanPersonel().ToList();

                }
                return (List<PersonelItem>)ViewState["SecilenPersonelList"];
            }
            set
            {
                ViewState["SecilenPersonelList"] = value;
            }
        }
        private List<PersonelItem> SecilmeyenPersonelList
        {
            get
            {
                if (ViewState["SecilmeyenPersonelList"] == null)
                {
                    ViewState["SecilmeyenPersonelList"] = new List<PersonelItem>().ToList();//personel.SelectCalisanPersonel().ToList();

                }
                return (List<PersonelItem>)ViewState["SecilmeyenPersonelList"];
            }
            set
            {
                ViewState["SecilmeyenPersonelList"] = value;
            }
        }
        private string DuyuruIdQS
        {
            get
            {

                if (ViewState["DuyuruId"] == null)
                {
                    if (Page.Request.QueryString["DuyuruId"] != null)
                    {
                        ViewState["DuyuruId"] = Page.Request.QueryString["DuyuruId"];
                    }
                    else
                    {
                        ViewState["DuyuruId"] = string.Empty;
                    }
                }
                return ViewState["DuyuruId"].ToString();
            }

            set
            {
                ViewState["DuyuruId"] = value;
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
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    if (string.IsNullOrEmpty(DuyuruIdQS) && !DestinationAppQS.Equals("DD"))
                    {
                        KaydetBtn.Visible = true;
                        GuncelleBtn.Visible = false;
                        SilBtn.Visible = false;
                        DuyuruGirisiAc();
                    }
                    else if (DestinationAppQS.Equals("DD"))
                    {
                        KaydetBtn.Visible = false;
                        GuncelleBtn.Visible = true;
                        SilBtn.Visible = true;
                        DuyuruDuzenlemeyiAc();
                        if (!string.IsNullOrEmpty(MesajQS))
                        {
                            MessageHelper.PublishMessage("Duyuru Kaydedildi", ProjeConstants.MESAJ_BASARILI, 2000);
                            MesajQS = string.Empty;
                        }
                    }
                }
                SecilenPersonelTablosunuDoldur();

            }
            catch (Exception ex)
            {

                ExceptionHelper exhelper = new ExceptionHelper(ex);
                exhelper.PublishException();
            }
        }

        private void DuyuruDuzenlemeyiAc()
        {
            TitleLbl.Text = "Duyuru Düzenleme";
            TitleLbl.CssClass = "col-form-label text-primary fw-bold mb-1";
            BaslangicSaatiDDLDoldur();
            BitisSaatiDDLDoldur();
            TekrarlaDDLDoldur();
            SecilenSecilmeyenPersonelListesiniDoldur(DuyuruIdQS.ConvertToInt());

            DuyuruFormunuDoldur();
            PersonelDDLDoldur();
        }
        private void SecilenSecilmeyenPersonelListesiniDoldur(int duyuruId)
        {
            SecilmeyenPersonelList.Clear();
            SecilenPersonelList.Clear();
            Personel personelDao = new Personel();
            List<Personel> list = personelDao.SelectCalisanPersonel();

            Duyuru duyuru = new Duyuru();
            duyuru = duyuru.Select<Duyuru>(DuyuruIdQS.ConvertToInt());
            if (duyuru != null)
            {
                int sirano = 1;
                foreach (Personel personel in list)
                {

                    PersonelItem pI = new PersonelItem();
                    pI.Id = personel.Id;
                    pI.Adi = personel.Adi;
                    pI.Soyadi = personel.Soyadi;
                    pI.SiraNo = sirano++;
                    if (duyuru.DuyuruAlicilari.Contains("," + personel.Id + ","))
                    {
                        if (!SecilenPersonelList.Contains(pI))
                        {
                            SecilenPersonelList.Add(pI);
                        }
                    }
                    else
                    {
                        if (!SecilmeyenPersonelList.Contains(pI))
                        {
                            SecilmeyenPersonelList.Add(pI);
                        }
                    }

                }
            }
        }
        private void DuyuruFormunuDoldur()
        {
            Duyuru duyuru = new Duyuru();
            duyuru = duyuru.Select<Duyuru>(DuyuruIdQS.ConvertToInt());
            BaslikTxt.Text = duyuru.Baslik;
            MetinTxt.Text= duyuru.Metin;
            DateTime basTar = duyuru.YayinBasTar;
            DateTime bitTar = duyuru.YayinBitTar;
            string basSaat = basTar.ToString("HH") + ":" + basTar.ToString("mm");
            string bitSaat = bitTar.ToString("HH") + ":" + bitTar.ToString("mm");
            YayinBasTarTxt.Value = basTar.ConvertToDatetimeEmptyIfNull();
            YayinBitTarTxt.Value = bitTar.ConvertToDatetimeEmptyIfNull();
            AktifChk.Checked = duyuru.Aktif;
            PopupChk.Checked = duyuru.Popup;

            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string hostUrl = currentUrl.Substring(0, currentUrl.LastIndexOf(System.Web.HttpContext.Current.Request.Url.AbsolutePath));
            string imgUrl = string.IsNullOrEmpty(duyuru.Resim)
                ? hostUrl + ProjeConstants.PATH_RESIMLER_DUYURU +"duyuru-resmi-yok.jpg"
                : hostUrl + ProjeConstants.PATH_RESIMLER_DUYURU + duyuru.Resim.ReplaceTrChars() + ".jpg";

            DisplayImage.ImageUrl = imgUrl;

            UtilityHelper.SetDDLValue(BasSaatDDL, basSaat);
            UtilityHelper.SetDDLValue(BitSaatDDL, bitSaat);
            UtilityHelper.SetDDLValue(TekrarlaDDL, duyuru.Tekrar);
        }
        private void DuyuruGirisiAc()
        {
            if (!Page.IsPostBack)
            {
                TitleLbl.Text = "Yeni Duyuru";
                TitleLbl.CssClass = "col-form-label text-success fw-bold mb-1";
                YayinBasTarTxt.Value = DateTime.Today.ConvertToDatetimeEmptyIfNull();
                YayinBitTarTxt.Value = DateTime.Today.ConvertToDatetimeEmptyIfNull();
                BaslangicSaatiDDLDoldur();
                BitisSaatiDDLDoldur();

                TekrarlaDDLDoldur();
                TümPersoneliSecilenmeyenPersonelListesieEkle();
                PersonelDDLDoldur();
            }

            SecilenPersonelTablosunuDoldur();
        }
        private void BaslangicSaatiDDLDoldur()
        {
            BasSaatDDL.Items.Clear();
            TimeSpan bastarTS = new TimeSpan(6, 0, 0);
            TimeSpan aralikTS = new TimeSpan(0, 30, 0);
            TimeSpan bittarTS = new TimeSpan(20, 0, 0);

            TimeSpan nextTS = bastarTS;

            while (nextTS < bittarTS)
            {
                string bastarStr = string.Format("{0:00}:{1:00}", nextTS.Hours, nextTS.Minutes);
                ListItem li = new ListItem(bastarStr);
                BasSaatDDL.Items.Add(li);
                nextTS = nextTS + aralikTS;
            }
        }
        private void BitisSaatiDDLDoldur()
        {
            BitSaatDDL.Items.Clear();
            string basSaatStr = BasSaatDDL.SelectedItem == null ? "20:00" : BasSaatDDL.SelectedItem.Text;
            TimeSpan bastarTS = basSaatStr.ConvertToTimeSpan();
            TimeSpan aralikTS = new TimeSpan(0, 30, 0);
            TimeSpan bittarTS = new TimeSpan(20, 0, 0);

            TimeSpan nextTS = bastarTS;

            do
            {
                nextTS = nextTS + aralikTS;
                string bittarStr = string.Format("{0:00}:{1:00}", nextTS.Hours, nextTS.Minutes);
                ListItem li = new ListItem(bittarStr);
                BitSaatDDL.Items.Add(li);

            } while (nextTS < bittarTS);

            if (BitSaatDDL.Items.FindByText("20:00") != null)
                BitSaatDDL.SelectedValue = BitSaatDDL.Items.FindByText("20:00").Value;
        }
        private void TekrarlaDDLDoldur()
        {
            TekrarlaDDL.Items.Clear();

            ListItem li = new ListItem(ProjeConstants.DUYURU_TEKRAR_YOK);
            //ListItem li1 = new ListItem(ProjeConstants.DUYURU_TEKRAR_SAAT);
            //ListItem li2 = new ListItem(ProjeConstants.DUYURU_TEKRAR_GUN);
            ListItem li3 = new ListItem(ProjeConstants.DUYURU_TEKRARLA_HAFTA);
            ListItem li4 = new ListItem(ProjeConstants.DUYURU_TEKRARLA_AY);
            ListItem li5 = new ListItem(ProjeConstants.DUYURU_TEKRARLA_YIL);
            TekrarlaDDL.Items.Add(li);
            //TekrarlaDDL.Items.Add(li1);
            //TekrarlaDDL.Items.Add(li2);
            TekrarlaDDL.Items.Add(li3);
            TekrarlaDDL.Items.Add(li4);
            TekrarlaDDL.Items.Add(li5);

        }
        private void PersonelDDLDoldur()
        {
            PersonelDDL.Items.Clear();
            ListItem bosLi = new ListItem(ProjeConstants.BOS, ProjeConstants.BOS_INT.ToString());
            PersonelDDL.Items.Add(bosLi);
            SecilmeyenPersonelList = SecilmeyenPersonelList.OrderBy(t => t.SiraNo).ToList();
            foreach (PersonelItem item in SecilmeyenPersonelList)
            {
                ListItem li = new ListItem(item.Adi.ReturnEmptyIfNull().ToString() + " " + item.Soyadi.ReturnEmptyIfNull().ToString(), item.Id.ReturnZeroIfNull().ToString());
                PersonelDDL.Items.Add(li);
            }
        }
        private void TümPersoneliSecilenPersonelListesineEkle()
        {
            SecilmeyenPersonelList.Clear();
            SecilenPersonelList.Clear();
            Personel personelDao = new Personel();
            List<Personel> list = personelDao.SelectCalisanPersonel();
            int sirano = 1;
            foreach (Personel personel in list)
            {
                PersonelItem pI = new PersonelItem();
                pI.Id = personel.Id;
                pI.Adi = personel.Adi;
                pI.Soyadi = personel.Soyadi;
                pI.SiraNo = sirano++;
                SecilenPersonelList.Add(pI);
            }
        }
        private void TümPersoneliSecilenmeyenPersonelListesieEkle()
        {
            SecilenPersonelList.Clear();

            SecilmeyenPersonelList.Clear();
            Personel personelDao = new Personel();
            List<Personel> list = personelDao.SelectCalisanPersonel();
            int sirano = 1;
            foreach (Personel personel in list)
            {
                PersonelItem pI = new PersonelItem();
                pI.Id = personel.Id;
                pI.Adi = personel.Adi;
                pI.Soyadi = personel.Soyadi;
                pI.SiraNo = sirano++;
                SecilmeyenPersonelList.Add(pI);
            }
        }
        private void SecilenPersonelTablosunuDoldur()
        {
            SecilenPersonelTable.Rows.Clear();
            int SiraNo = 0;
            SecilenPersonelList = SecilenPersonelList.OrderBy(t => t.SiraNo).ToList();
            foreach (PersonelItem pI in SecilenPersonelList)
            {
                if (pI != null)
                {
                    TableRow row = new TableRow();
                    TableCell adSoyadCell = new TableCell();
                    adSoyadCell.Text = pI.Adi + " " + pI.Soyadi;
                    row.Controls.Add(adSoyadCell);

                    TableCell SilCell = new TableCell();
                    LinkButton SilBtn = new LinkButton();
                    SilBtn.Text = "Çikar";

                    SilBtn.ID = "SilBtn" + SiraNo++;
                    TableUpdatePanel.ContentTemplateContainer.Controls.Add(SilBtn);
                    SilBtn.CssClass = "btn btn-outline-danger";
                    SilBtn.CausesValidation = false;
                    SilBtn.Click += delegate
                    {
                        SecilenPersonelList.Remove(pI);
                        if (!SecilmeyenPersonelList.Contains(pI))
                        {
                            SecilmeyenPersonelList.Add(pI);
                        }
                        PersonelDDLDoldur();
                        SecilenPersonelTablosunuDoldur();
                    };
                    SilCell.Controls.Add(SilBtn);
                    row.Controls.Add(SilCell);

                    SecilenPersonelTable.Controls.Add(row);

                }

            }


        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {

        }
        protected void BasSaatDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            BitisSaatiDDLDoldur();
        }
        protected void BitSaatDDL_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        protected void TekrarlaDDL_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        protected void PersonelDDL_SelectedIndexChanged(object sender, EventArgs e)
        {

            Personel personel = new Personel();
            personel = personel.Select<Personel>(PersonelDDL.SelectedItem.Value.ConvertToInt());
            if (personel != null)
            {
                IsBilgileri ib = new IsBilgileri();
                ib = ib.SelectByPersonelId(personel.Id);
                if (ib != null)
                {
                    PersonelItem pI = SecilmeyenPersonelList.Where(t => t.Id == personel.Id).FirstOrDefault();
                    if (!SecilenPersonelList.Contains(pI))
                    {
                        SecilenPersonelList.Add(pI);
                        SecilmeyenPersonelList.Remove(pI);
                        SecilenPersonelTablosunuDoldur();
                    }
                    PersonelDDL.Items.RemoveAt(PersonelDDL.SelectedIndex);

                }

            }
            else
            {
                MessageHelper.PublishMessage("Personel Bulunamadi!", ProjeConstants.MESAJ_HATA);
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_PERSONEL_LIST;
                Page.Response.Redirect(newUrl);
            }
        }
        protected void KaydetBtn_Click(object sender, EventArgs e)
        {
            try
            {
                bool kayitGeceliMi = false;
                kayitGeceliMi = AlanlariGecerle();
                if (kayitGeceliMi)
                {
                    Duyuru duyuru = DuyuruyuKaydet();
                    if (duyuru != null)
                    {
                        ResmiKaydet(duyuru);
                        RedirectToPage(ProjeConstants.PAGE_DUYURU_LIST + "?Mesaj=true&SecilenId=" + duyuru.Id );
                    }

                }
                else
                {
                    MessageHelper.PublishMessage("Lütfen Duyuru bilgilerini tamamladiktan sonra kaydedin.", ProjeConstants.MESAJ_HATA, 2000);
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exhelper = new ExceptionHelper(ex);
                exhelper.Exceptions.Add(new Exception("Duyuru kayit edilemedi."));
                exhelper.PublishException();
            }
        }
        protected void GuncelleBtn_Click(object sender, EventArgs e)
        {
            try
            {
                bool kayitGecerliMi = false;
                kayitGecerliMi = AlanlariGecerle();
                if (kayitGecerliMi)
                {
                    if (DuyuruyuGuncelle())
                    {
                        
                    }
                }
                else
                {
                    MessageHelper.PublishMessage("Lütfen Duyuru bilgilerini tamamladiktan sonra kaydedin.", ProjeConstants.MESAJ_HATA, 2000);
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exhelper = new ExceptionHelper(ex);
                exhelper.Exceptions.Add(new Exception("Duyuru kayit edilemedi."));
                exhelper.PublishException();
            }
        }
        protected void SilBtn_Click(object sender, EventArgs e)
        {
            try
            {

                SilLbl.Text = "Lütfen Dikkat: Duyuru Silinecek";
                SilmeMesajiLbl.Text = "Duyuruyu Silmek Istediginizden Emin misiniz?";
                DeleteNowBtn.Visible = true;
                var openPopup = "OpenModal();";
                System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), openPopup, true);
            }
            catch (Exception ex)
            {
                ExceptionHelper exhelper = new ExceptionHelper(ex);
                exhelper.Exceptions.Add(new Exception("Duyuru silinemedi."));
                exhelper.PublishException();
            }
        }
        protected void DuyuruListBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_DUYURU_LIST+"?DuyuruId="+DuyuruIdQS);
        }
        private void ResmiKaydet(Duyuru duyuru)
        {
            if (xFileUpload.HasFile)
            {
                string resim = "Duyuru" + duyuru.Id;
                
                duyuru.Resim= SaveImageFiles2SP(resim);
                
            }
            duyuru.Update();
        }
        private string SaveImageFiles2SP(string fotoFile)
        {
            string SPImageListName = ProjeConstants.RESIMLER_DUYURU;

            SPWeb web = Microsoft.SharePoint.SPContext.Current.Web;
            SPList listExists = web.Lists.TryGetList(SPImageListName);
            ExceptionHelper exhelper = new ExceptionHelper();
            exhelper = UtilityHelper.uploadFile2SP(xFileUpload, "", fotoFile + "", SPImageListName, exhelper, ProjeConstants.RESIM_DIGER_EN, ProjeConstants.RESIM_DIGER_BOY);
            if (exhelper.HasException())
            {
                Exception ex = new Exception("Resim Kaydedilemedi.");
                exhelper.Exceptions.Add(ex);
                exhelper.PublishException();
                return string.Empty;   
            }
            else
            {                
                MessageHelper.PublishMessage("Islem Tamamlandi.Resim yüklendi.", ProjeConstants.MESAJ_BASARILI);
                return fotoFile;
            }
        }
        private Duyuru DuyuruyuKaydet()
        {
            int duyuruId = 0;
            try
            {

                Duyuru duyuru = new Duyuru();
                duyuru.Baslik = BaslikTxt.Text;
                duyuru.Metin = MetinTxt.Text;
                string bassaat = BasSaatDDL.SelectedItem.Value;
                string bitsaat = BitSaatDDL.SelectedItem.Value;
                DateTime bastar = YayinBasTarTxt.Value.ConvertToDatetime();
                DateTime bittar = YayinBitTarTxt.Value.ConvertToDatetime();
                duyuru.YayinBasTar = UtilityHelper.TariheSaatEkle(bastar, bassaat);
                duyuru.YayinBitTar = UtilityHelper.TariheSaatEkle(bittar, bitsaat);
                duyuru.Tekrar = TekrarlaDDL.SelectedItem.Value;
                duyuru.DuyuruAlicilari = DuyuruAlicilariniAl();
                duyuru.Aktif = AktifChk.Checked;
                duyuru.Popup = PopupChk.Checked;
                duyuru.Olusturan = UtilityHelper.GetCurrentUserLoginName();
                duyuruId = duyuru.Id = duyuru.Save();

                if (duyuruId > 0)
                {
                    duyuru.Degistiren = UtilityHelper.GetCurrentUserLoginName();
                    duyuru.Update();
                    if (duyuru != null)
                    {
                        DuyuruGosterim dg = new DuyuruGosterim();
                        dg.SaveDuyuru(duyuru);
                    }
                }
                return duyuru;
            }
            catch (Exception ex)
            {
                ExceptionHelper exhelper = new ExceptionHelper(ex);
                exhelper.Exceptions.Add(new Exception("Duyuru kayit edilemedi."));
                exhelper.PublishException();
                return null;
            }
           
        }
        private bool DuyuruyuGuncelle()
        {
            bool guncellendiMi = false;
            try
            {

                Duyuru duyuru = new Duyuru();
                duyuru = duyuru.Select<Duyuru>(DuyuruIdQS.ConvertToInt());
                if (duyuru == null)
                {
                    MessageHelper.PublishMessage("Duyuru bulunamadi", ProjeConstants.MESAJ_HATA, 5000);
                }
                else
                {

                    duyuru.Baslik = BaslikTxt.Text;
                    duyuru.Metin = MetinTxt.Text;
                    string bassaat = BasSaatDDL.SelectedItem.Value;
                    string bitsaat = BitSaatDDL.SelectedItem.Value;
                    DateTime bastar = YayinBasTarTxt.Value.ConvertToDatetime();
                    DateTime bittar = YayinBitTarTxt.Value.ConvertToDatetime();
                    duyuru.YayinBasTar = UtilityHelper.TariheSaatEkle(bastar, bassaat);
                    duyuru.YayinBitTar = UtilityHelper.TariheSaatEkle(bittar, bitsaat);
                    duyuru.Tekrar = TekrarlaDDL.SelectedItem.Value;
                    duyuru.DuyuruAlicilari = DuyuruAlicilariniAl();
                    duyuru.Aktif = AktifChk.Checked;
                    duyuru.Popup = PopupChk.Checked;
                    duyuru.Degistiren = UtilityHelper.GetCurrentUserLoginName();
                    guncellendiMi = duyuru.Update();

                    if (guncellendiMi)
                    {
                        if (duyuru != null)
                        {
                            ResmiKaydet(duyuru);
                            DuyuruGosterim duyuruGosterim = new DuyuruGosterim();
                            duyuruGosterim.SaveDuyuru(duyuru);
                            
                        }
                        MessageHelper.PublishMessage("Duyuru güncellendi", ProjeConstants.MESAJ_BASARILI, 2000);
                    }
                    else
                    {
                        MessageHelper.PublishMessage("Duyuru güncellenemedi", ProjeConstants.MESAJ_HATA, 5000);
                    }
                }

            }
            catch (Exception ex)
            {
                ExceptionHelper exhelper = new ExceptionHelper(ex);
                exhelper.Exceptions.Add(new Exception("Duyuru güncellenemedi."));
                exhelper.PublishException();
            }
            return guncellendiMi;
        }
        private string DuyuruAlicilariniAl()
        {
            StringBuilder idstr = new StringBuilder();
            foreach (PersonelItem item in SecilenPersonelList)
            {
                string value = item.Id.ToString();
                if (idstr.ToString() == string.Empty)
                    idstr.AppendFormat(",{0},", value);
                else
                {
                    idstr.AppendFormat("{0},", value);
                }
            }
            return idstr.ToString();
        }
        private bool AlanlariGecerle()
        {
            bool isGecerli = false;
            try
            {
                isGecerli = !string.IsNullOrEmpty(BaslikTxt.Text) ||
                    !string.IsNullOrEmpty(MetinTxt.Text) ||
                    !string.IsNullOrEmpty(YayinBasTarTxt.Value) ||
                    !string.IsNullOrEmpty(YayinBitTarTxt.Value);
            }
            catch (Exception ex)
            {

                throw;
            }
            return isGecerli;
        }
        protected void DeleteNowBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(DuyuruIdQS))
                {
                    Duyuru duyuru = new Duyuru();
                    duyuru = duyuru.Select(DuyuruIdQS.ConvertToInt());
                    if (duyuru != null)
                    {
                        duyuru.Delete();
                        RedirectToPage(ProjeConstants.PAGE_DUYURU_LIST);
                    }
                    //duyuruyu sil,
                    //resmi sil
                    //duyru listesine git
                }
                else
                {
                    MessageHelper.PublishMessage("Duyuru bulunamadi.", ProjeConstants.MESAJ_HATA, 2000);
                }
            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper(exception);
                Exception exceptionInfo = new Exception("Duyuru Silinemedi");
                exceptionHelper.Exceptions.Add(exceptionInfo);
                exceptionHelper.PublishException();
            }
        }
        protected void SecilenResmiSilBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(DuyuruIdQS))
                {
                    Duyuru duyuru = new Duyuru();
                    duyuru = duyuru.Select(DuyuruIdQS.ConvertToInt());
                    if (duyuru != null)
                    {
                        duyuru.Resim=null;
                        duyuru.Update();
                    }
                    //duyuruyu sil,
                    //resmi sil
                    //duyru listesine git
                }
                else
                {
                    MessageHelper.PublishMessage("Duyuru bulunamadi.", ProjeConstants.MESAJ_HATA, 2000);
                }
            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper(exception);
                Exception exceptionInfo = new Exception("Duyuru Silinemedi");
                exceptionHelper.Exceptions.Add(exceptionInfo);
                exceptionHelper.PublishException();
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
        protected void HepsiniEkleBtn_Click(object sender, EventArgs e)
        {
            TümPersoneliSecilenPersonelListesineEkle();
            SecilenPersonelTablosunuDoldur();
            PersonelDDLDoldur();
        }

        protected void HepsiniCikarBtn_Click(object sender, EventArgs e)
        {
            TümPersoneliSecilenmeyenPersonelListesieEkle();
            SecilenPersonelTablosunuDoldur();
            PersonelDDLDoldur();
        }
        [Serializable]
        class PersonelItem
        {
            public int SiraNo { get; set; }
            public string Adi { get; set; }
            public string Soyadi { get; set; }
            public int Id { get; set; }
            public PersonelItem()
            {

            }
        }


    }
}
