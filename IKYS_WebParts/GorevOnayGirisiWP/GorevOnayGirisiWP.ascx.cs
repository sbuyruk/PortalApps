using Model.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace IKYS_WebParts.GorevOnayGirisiWP
{
    [ToolboxItemAttribute(false)]
    public partial class GorevOnayGirisiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public GorevOnayGirisiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string GorevOnayIdQS
        {
            get
            {

                if (ViewState["GorevOnayId"] == null)
                {
                    if (Page.Request.QueryString["GorevOnayId"] != null)
                    {
                        ViewState["GorevOnayId"] = Page.Request.QueryString["GorevOnayId"];
                    }
                    else
                    {
                        ViewState["GorevOnayId"] = string.Empty;
                    }
                }
                return ViewState["GorevOnayId"].ToString();
            }

            set
            {
                ViewState["GorevOnayId"] = value;
            }
        }
        private string PersonelIdQS
        {
            get
            {

                if (ViewState["PersonelId"] == null)
                {
                    if (Page.Request.QueryString["PersonelId"] != null)
                    {
                        ViewState["PersonelId"] = Page.Request.QueryString["PersonelId"];
                    }
                    else
                    {
                        ViewState["PersonelId"] = string.Empty;
                    }
                }
                return ViewState["PersonelId"].ToString();
            }

            set
            {
                ViewState["PersonelId"] = value;
            }
        }
        private string BirimTanimIdQS
        {
            get
            {

                if (ViewState["BirimTanimId"] == null)
                {
                    if (Page.Request.QueryString["BirimTanimId"] != null)
                    {
                        ViewState["BirimTanimId"] = Page.Request.QueryString["BirimTanimId"];
                    }
                    else
                    {
                        ViewState["BirimTanimId"] = string.Empty;
                    }
                }
                return ViewState["BirimTanimId"].ToString();
            }

            set
            {
                ViewState["BirimTanimId"] = value;
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
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (AuthQS.Equals("IKYS"))
                {
                    PersonelDiv.Attributes["style"] = "display:block";
                    GorevOnayListesiBtn.Visible = true;
                }
                else if (AuthQS.Equals("BIRIM"))
                {
                    PersonelDiv.Attributes["style"] = "display:block";
                    GorevOnayListesiBtn.Visible = true;
                }
                else
                {
                    //personelDiv display:none yap
                    PersonelDiv.Attributes["style"] = "display:none";
                    GorevOnayListesiBtn.Visible=false;
                }
                if (string.IsNullOrEmpty(GorevOnayIdQS))
                {
                    OpenGiris();
                }
                else
                {
                    GorevOnay gorevOnay = new GorevOnay();
                    gorevOnay = gorevOnay.Select<GorevOnay>(GorevOnayIdQS.ConvertToInt());
                    if (gorevOnay == null)
                    {
                        MessageHelper.PublishMessage("Görev kaydı bulunamadı. Yeni görev girişi yapabilirsiniz.",ProjeConstants.MESAJ_HATA);
                        OpenGiris();
                    }
                    else
                    {
                        OpenDuzenle();
                    }
                }
            }
            catch (Exception exception)
            {
                ExceptionHelper exHelper = new ExceptionHelper(exception);
                exHelper.PublishException();
            }
        }
        private void OpenDuzenle()
        {
            SaveBtn.Visible = false;
            UpdateBtn.Visible = true;
            DeleteBtn.Visible = true;
            RaporAlBtn.Visible = true;
            TitleLbl.CssClass = "col-form-primary  btn-outline-primary mb-1";
            TitleLbl.Text = "Görev Onayı Düzenleme";
            Personel personel = PersonelGetir();
            if (!Page.IsPostBack)
            {
                if (personel != null)
                {
                    PersonelAdiLbl.Text = personel.Adi + " " + personel.Soyadi;
                    FillBasSaat();
                    FillBitSaat();
                    FillParaBirimiDDL();
                    FillUlasimAraciDDL();
                    FillPersonelDDL(personel);
                    FillPerSubeImzaDDL();
                    FillOnayImzaDDL();
                    FillGMImzaDDL();
                    FillOnayMakamDDL();
                    FillGorevOnayForm();

                }
            }
        }
        private void OpenGiris()
        {
            SaveBtn.Visible = true;
            UpdateBtn.Visible = false;
            DeleteBtn.Visible = false;
            RaporAlBtn.Visible = false;
            if (!Page.IsPostBack)
            {
                FillBasSaat();
                FillBitSaat();
                FillParaBirimiDDL();
                FillUlasimAraciDDL();
                FillPerSubeImzaDDL();
                FillOnayImzaDDL();
                FillGMImzaDDL();
                FillOnayMakamDDL();
                Personel personel = PersonelGetir();
                FillPersonelDDL(personel);

                if (personel != null)
                {
                    PersonelAdiLbl.Text = personel.Adi + " " + personel.Soyadi;
                    DateTime today = DateTime.Today;
                    TimeSpan mesaiBasSaati = ProjeConstants.MESAI_BASLAMA_SAATI;
                    TimeSpan mesaiBitSaati = ProjeConstants.MESAI_BITIS_SAATI;
                    DateTime bastar = today + mesaiBasSaati;
                    DateTime bittar = today + mesaiBitSaati;
                    BasTarTxt.Value = bastar.ConvertToDatetimeEmptyIfNull();
                    BitTarTxt.Value = bittar.ConvertToDatetimeEmptyIfNull();
                    string bassaat = bastar.ToString("HH:mm");
                    string bitsaat = bittar.ToString("HH:mm");
                    SelectDDLByText(BasSaatDDL, bassaat);
                    SelectDDLByText(BitSaatDDL, bitsaat);
                }


            }
        }
        private void FillPersonelDDL(Personel personel)
        {
            PersonelDDL.Items.Clear();
            
            List<Personel> list = new List<Personel>();

            if (AuthQS.Equals("IKYS"))
            {
                Personel perdao = new Personel();
                list = perdao.SelectCalisanPersonel();
            }
            else if (AuthQS.Equals("BIRIM"))
            {
                
                list = BirimdekiPersoneliGetir(personel);
            }
            else
            {
                personel = PersonelGetir(PersonelIdQS.ConvertToInt());
                if (personel != null)
                {
                    list.Add(personel);
                }
            }

            foreach (Personel item in list)
            {
                ListItem li = new ListItem(item.Adi.ReturnEmptyIfNull().ToString() + " " + item.Soyadi.ReturnEmptyIfNull().ToString(), item.Id.ReturnZeroIfNull().ToString());
                PersonelDDL.Items.Add(li);
            }

            string secilenPer = !string.IsNullOrEmpty(PersonelIdQS) ? PersonelIdQS : "0";
            ListItem perItem = new ListItem();
            if (!string.IsNullOrEmpty(secilenPer))
                perItem = PersonelDDL.Items.FindByValue(secilenPer);

            if (perItem != null)
            {
                PersonelDDL.SelectedValue = perItem.Value;
                PersonelIdQS = perItem.Value;
            }
        }
        private string ChildBirimGetir(int parentId)
        {
            string retVal = parentId + ",";
            BirimTanim bt = new BirimTanim();
            List<BirimTanim> list = bt.SelectByParentId(parentId);
            foreach (BirimTanim item in list)
            {
                //retVal += item.Id + ",";
                string val = ChildBirimGetir(item.Id);
                if (string.IsNullOrEmpty(val))
                {
                    return retVal;
                }
                retVal += val;
            }
            return retVal;
        }
        private string BirimListesiGetir(BirimTanim birimTanim)
        {
            string birimIdStr = string.Empty;
            if (birimTanim != null)
            {
                string birim = ChildBirimGetir(birimTanim.Id);
                birimIdStr = string.IsNullOrEmpty(birim) ? "" : birim.Substring(0, birim.Length - 1);
            }

            return birimIdStr;
        }
        private List<Personel> BirimdekiPersoneliGetir(Personel personel)
        {
            
            List<Personel> list = new List<Personel>();
            if (personel != null)
            {
                string birimListesiStr = string.Empty;
                if (string.IsNullOrEmpty(BirimTanimIdQS))
                {
                    IsBilgileri ib = new IsBilgileri();
                    ib = ib.SelectByPersonelId(personel.Id);
                    if (ib != null)
                    {
                        int birimId = ib.BirimId;
                        BirimTanimIdQS = ib.BirimId.ToString();
                        //Personel perdao = new Personel();
                        //list = perdao.SelectCalisanPersonelByBirimId(birimId);
                    } 
                }
                
                BirimTanim bt = new BirimTanim();
                bt = bt.Select(BirimTanimIdQS.ConvertToInt());
                if (bt != null)
                {
                    birimListesiStr = BirimListesiGetir(bt);
                }
                Personel perdao = new Personel();
                list = perdao.SelectCalisanPersonelByBirimReturnList(birimListesiStr);
            }
            return list;
        }
        //private List<Personel> BirimdekiPersoneliGetir(Personel personel)
        //{
        //    List<Personel> list = new List<Personel>();
        //    if (personel != null)
        //    {
        //        IsBilgileri ib = new IsBilgileri();
        //        ib = ib.SelectByPersonelId(personel.Id);
        //        if (ib != null)
        //        {
        //            int birimId = ib.BirimId;
        //            Personel perdao = new Personel();
        //            list = perdao.SelectCalisanPersonelByBirimId(birimId);
        //        }
        //    }
        //    return list;
        //}
        private void FillParaBirimiDDL()
        {
            ParaBirimiDDL.Items.Clear();

            ListItem li = new ListItem("TL", "TL");
            ParaBirimiDDL.Items.Add(li);
            ListItem li1 = new ListItem("Avro", "Avro");
            ParaBirimiDDL.Items.Add(li1);
            ListItem li2 = new ListItem("Dolar", "Dolar");
            ParaBirimiDDL.Items.Add(li2);
            ListItem li3 = new ListItem("Sterlin", "Sterlin");
            ParaBirimiDDL.Items.Add(li3);
        }
        private void FillUlasimAraciDDL()
        {
            UlasimAraciDDL.Items.Clear();
            ListItem li = new ListItem(ProjeConstants.ULASIMARACI_BOS);
            UlasimAraciDDL.Items.Add(li);
            ListItem li0 = new ListItem(ProjeConstants.ULASIMARACI_UCAK);
            UlasimAraciDDL.Items.Add(li0);
            ListItem li1 = new ListItem(ProjeConstants.ULASIMARACI_OTOBUS);
            UlasimAraciDDL.Items.Add(li1);
            ListItem li2 = new ListItem(ProjeConstants.ULASIMARACI_TREN);
            UlasimAraciDDL.Items.Add(li2);
            ListItem li3 = new ListItem(ProjeConstants.ULASIMARACI_VAKIFARACI);
            UlasimAraciDDL.Items.Add(li3);

        }
        private void FillBasSaat()
        {
            BasSaatDDL.Items.Clear();
            TimeSpan bastarTS = new TimeSpan(0, 0, 0);
            TimeSpan aralikTS = new TimeSpan(0, 5, 0);
            TimeSpan bittarTS = new TimeSpan(23, 59, 0);

            TimeSpan nextTS = bastarTS;

            while (nextTS < bittarTS)
            {
                string bastarStr = string.Format("{0:00}:{1:00}", nextTS.Hours, nextTS.Minutes);
                ListItem li = new ListItem(bastarStr);
                BasSaatDDL.Items.Add(li);
                nextTS = nextTS + aralikTS;
            }
        }
        private void FillBitSaat()
        {
            string bitsaatStr = BitSaatDDL.SelectedItem == null ? "17:00" : BitSaatDDL.SelectedItem.Text;
            BitSaatDDL.Items.Clear();
            string bassaatStr = BasSaatDDL.SelectedItem == null ? "07:05" : BasSaatDDL.SelectedItem.Text;
            TimeSpan bastarTS = bassaatStr.ConvertToTimeSpan();
            TimeSpan aralikTS = new TimeSpan(0, 5, 0);
            TimeSpan bittarTS = new TimeSpan(23, 59, 0);

            TimeSpan nextTS = bastarTS;

            do
            {
                nextTS = nextTS + aralikTS;
                string bastarStr = string.Format("{0:00}:{1:00}", nextTS.Hours, nextTS.Minutes);
                ListItem li = new ListItem(bastarStr);
                BitSaatDDL.Items.Add(li);


            } while (nextTS < bittarTS);
            if (BitSaatDDL.Items.FindByText(bitsaatStr) != null)
            {
                SelectDDLValue(BitSaatDDL, bitsaatStr);
            }
            else
            {
                SelectDDLValue(BitSaatDDL, "17:00");
            }

        }
        private void FillPerSubeImzaDDL()
        {
            PerSubeImzaDDL.Items.Clear();
            Personel personel = new Personel();
            DataTable dataTable = personel.SelectCalisanPersonelReturnDataTable();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                int personelId = dataRow["PersonelId"].ConvertToInt();
                string adiSoyadi = dataRow["Adi"].ToString() + " " + dataRow["Soyadi"].ToString();
                string gorev = dataRow["Gorev"].ToString();
                string birim = dataRow["BirimSube"].ToString();
                if (birim.Contains(ProjeConstants.PER_SUBE))
                {
                    ListItem li = new ListItem(adiSoyadi, personelId.ToString());
                    PerSubeImzaDDL.Items.Add(li);
                }

            }
        }
        private void FillOnayImzaDDL()
        {
            OnayImzaDDL.Items.Clear();
            Personel personel = new Personel();
            DataTable dataTable = personel.SelectAmirReturnDataTable();
            foreach (DataRow dataRow in dataTable.Rows)
            {

                int personelId = dataRow["PersonelId"].ConvertToInt();
                string adiSoyadi = dataRow["Adi"].ToString() + " " + dataRow["Soyadi"].ToString();
                if (OnayImzaDDL.Items.FindByText(adiSoyadi) != null)
                {
                    continue;
                }
                else
                {
                    ListItem li = new ListItem(adiSoyadi, personelId.ToString());
                    OnayImzaDDL.Items.Add(li);
                }

            }
        }
        private void FillGMImzaDDL()
        {
            GMImzaDDL.Items.Clear();
            Personel personel = new Personel();
            DataTable dataTable = personel.SelectAmirReturnDataTable();

            ListItem li0 = new ListItem("", "");
            GMImzaDDL.Items.Add(li0);
            foreach (DataRow dataRow in dataTable.Rows)
            {

                int personelId = dataRow["PersonelId"].ConvertToInt();
                string adiSoyadi = dataRow["Adi"].ToString() + " " + dataRow["Soyadi"].ToString();
                if (GMImzaDDL.Items.FindByText(adiSoyadi) != null)
                {
                    continue;
                }
                else
                {
                    ListItem li = new ListItem(adiSoyadi, personelId.ToString());
                    GMImzaDDL.Items.Add(li);
                }

            }
        }
        private void FillOnayMakamDDL()
        {
            OnayMakamDDL.Items.Clear();
            GorevTanim gorevDao = new GorevTanim();
            List<GorevTanim> list = gorevDao.SelectAll<GorevTanim>();
            foreach (GorevTanim gr in list)
            {
                ListItem li = new ListItem(gr.Adi.ReturnEmptyIfNull().ToString(), gr.Id.ReturnZeroIfNull().ToString());
                OnayMakamDDL.Items.Add(li);
            }
        }
        /// <summary>
        /// QueryString'den gelen GorevOnayIdQS ile GorevOnay_Table'dan kayıt getir
        /// Bu bilgileri forma doldur
        /// Güncelle butonunu aç
        /// Kaydet butonunu sakla
        /// </summary>
        private void FillGorevOnayForm()
        {
            GorevOnay gorevOnay = new GorevOnay();
            gorevOnay = gorevOnay.Select<GorevOnay>(GorevOnayIdQS.ConvertToInt());
            if (gorevOnay != null)
            {
                GorevOnayIdLbl.Text = gorevOnay.Id.ReturnEmptyIfNull().ToString();
                Personel personel = PersonelGetir(gorevOnay.PersonelId);
                if (personel != null)
                {
                    PersonelAdiLbl.Text = personel.Adi + " " + personel.Soyadi;

                }
                else
                {
                    PersonelAdiLbl.Text = "Personel bulunamadı";
                }
                
                SelectDDLValue(ParaBirimiDDL, gorevOnay.ParaBirimi.ToString());
                SelectDDLValue(PersonelDDL, gorevOnay.PersonelId.ToString());
                SelectDDLValue(OnayMakamDDL, gorevOnay.OnayMakam.ToString());
                SelectDDLValue(UlasimAraciDDL, gorevOnay.UlasimAraci.ToString());
                string bassaat = gorevOnay.BaslangicTarihi.ToString("HH:mm");
                SelectDDLByText(BasSaatDDL, bassaat);
                string bitsaat = gorevOnay.BitisTarihi.ToString("HH:mm");
                SelectDDLByText(BitSaatDDL, bitsaat);
                BasTarTxt.Value = gorevOnay.BaslangicTarihi.ConvertToDatetimeEmptyIfNull();
                BitTarTxt.Value = gorevOnay.BitisTarihi.ConvertToDatetimeEmptyIfNull();

                GorevinYeriTxt.Text = gorevOnay.GorevinYeri.ReturnEmptyIfNull().ToString();
                GorevinSebebiTxt.Text = gorevOnay.GorevinSebebi.ReturnEmptyIfNull().ToString();
                AciklamaTxt.Text = gorevOnay.Aciklama.ReturnEmptyIfNull().ToString();
                SelectDDLValue(PerSubeImzaDDL, gorevOnay.PerSubeImza.ToString());
                SelectDDLValue(OnayImzaDDL, gorevOnay.OnayImza.ToString());
                SelectDDLValue(OnayMakamDDL, gorevOnay.OnayMakam.ToString());
                SelectDDLValue(GMImzaDDL, gorevOnay.GMImza.ToString());
                PersubeVekilChk.Checked = gorevOnay.PerSubeVekil.ConvertToBool();
                OnayVekilChk.Checked = gorevOnay.OnayMakamVekil.ConvertToBool();
                SureTxt.Text = gorevOnay.Sure.ReturnEmptyIfNull().ToString();
                AvansTxt.Text = gorevOnay.Avans.ReturnEmptyIfNull().ToString();
                YevmiyeTxt.Text = gorevOnay.Yevmiye.ReturnEmptyIfNull().ToString();
                GMVekilChk.Checked = gorevOnay.GMVekil.ConvertToBool();
                //AracTahsisiChk.Checked = gorevOnay.AracTahsisi.ConvertToBool();
                UlasimAraciDDL.SelectedItem.Text = gorevOnay.UlasimAraci;
                AracPlakasiTxt.Text = gorevOnay.AracPlakasi.ReturnEmptyIfNull().ToString();
            }
            else
            {
                MessageHelper.PublishMessage("Kayıt bulunamadı", ProjeConstants.MESAJ_BILGI);
            }
        }

        private Personel PersonelGetir(int personelId)
        {
            Personel personel = new Personel();
            personel = personel.Select<Personel>(personelId);
            string personelAdi = string.Empty;
            
            return personel;
        }

        private string SelectDDLValue(DropDownList ddlList, string value)
        {
            if (ddlList.Items.FindByValue(value) != null)
            {
                ListItem li = ddlList.Items.FindByValue(value);
                ddlList.SelectedValue = li.Value;
                return li.Value;
            }
            return string.Empty;
        }
        private string SelectDDLByText(DropDownList ddlList, string value)
        {
            if (ddlList.Items.FindByText(value) != null)
            {
                ListItem li = ddlList.Items.FindByText(value);
                ddlList.SelectedValue = li.Value;
                return li.Value;
            }
            return string.Empty;
        }
        private Personel PersonelGetir()
        {
            Personel personel = new Personel();

            if (!string.IsNullOrEmpty(PersonelIdQS))
            {
                personel = personel.Select<Personel>(PersonelIdQS.ConvertToInt());

            }
            else
            {
                string userName = CurrentUserName.Substring(CurrentUserName.LastIndexOf("\\") + 1, CurrentUserName.Length - CurrentUserName.LastIndexOf("\\") - 1);
                personel = personel.SelectByUserName(userName);
                PersonelIdQS = personel.Id.ToString();
            }

            return personel;
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        protected void GorevOnayListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_GOREVONAY_LIST + "?SecilenId=" + GorevOnayIdQS);
        }
        protected void SaveBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Personel personel = new Personel();
                personel = PersonelGetir();
                if (personel != null)
                {
                    bool isValid = validateInputValues();
                    if (isValid)
                    {
                        GorevOnay gorevOnay = Kaydet();
                        if (gorevOnay!=null)
                        {
                            if (AuthQS.Equals("IKYS"))
                            {

                            }
                            else
                            {
                                IKYSOrtak.GorevOnayEPostasiGonder(personel, gorevOnay.Id, "YurtIci/YurtDisi");
                            }
                            MessageHelper.PublishMessage("Kaydedildi", ProjeConstants.MESAJ_BASARILI, 2000);
                        }
                    }
                    else
                    {
                        MessageHelper.PublishMessage("Lütfen bilgileri tamamlayınız.", ProjeConstants.MESAJ_HATA);
                    }
                }
                else
                {
                    MessageHelper.PublishMessage("Personel Bulunamadı!", ProjeConstants.MESAJ_HATA);
                }

            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper(exception);
                Exception exceptionInfo = new Exception("Görev kaydedilemedi");
                exceptionHelper.Exceptions.Add(exceptionInfo);
                exceptionHelper.PublishException();
            }
        }
        private GorevOnay Kaydet()
        {
            GorevOnay gorevOnay = new GorevOnay();
            Personel personel = PersonelGetir();
            if (personel != null)
            {
                DateTime bastar = BasTarTxt.Value.ConvertToDatetime();
                DateTime bittar = BitTarTxt.Value.ConvertToDatetime();
                string basSaat = BasSaatDDL.SelectedItem.Text;
                bastar = UtilityHelper.TariheSaatEkle(bastar, basSaat);
                string bitSaat = BitSaatDDL.SelectedItem.Text;
                bittar = UtilityHelper.TariheSaatEkle(bittar, bitSaat);

                
                gorevOnay.Aciklama = AciklamaTxt.Text;
                gorevOnay.AracPlakasi = AracPlakasiTxt.Text;
                //gorevOnay.AracTahsisi = AracTahsisiChk.Checked;
                gorevOnay.UlasimAraci = UlasimAraciDDL.SelectedItem.Text;
                gorevOnay.Avans = AvansTxt.Text;
                gorevOnay.BaslangicTarihi = bastar;
                gorevOnay.BitisTarihi = bittar;
                gorevOnay.GorevinYeri = GorevinYeriTxt.Text;
                gorevOnay.Olusturan = CurrentUserName;
                gorevOnay.OnayImza = OnayImzaDDL.SelectedItem == null ? 0 : OnayImzaDDL.SelectedItem.Value.ConvertToInt();
                gorevOnay.OnayMakam = OnayMakamDDL.SelectedItem == null ? 0 : OnayMakamDDL.SelectedItem.Value.ConvertToInt();
                gorevOnay.OnayMakamVekil = OnayVekilChk.Checked;
                gorevOnay.ParaBirimi = ParaBirimiDDL.SelectedItem == null ? "" : ParaBirimiDDL.SelectedItem.Value;
                gorevOnay.UlasimAraci = UlasimAraciDDL.SelectedItem == null ? "" : UlasimAraciDDL.SelectedItem.Value;
                gorevOnay.PersonelId = personel.Id;
                gorevOnay.PerSubeImza = PerSubeImzaDDL.SelectedItem == null ? 0 : PerSubeImzaDDL.SelectedItem.Value.ConvertToInt();
                gorevOnay.PerSubeVekil = PersubeVekilChk.Checked;
                gorevOnay.GorevinSebebi = GorevinSebebiTxt.Text;
                gorevOnay.GMImza = GMImzaDDL.SelectedItem == null ? 0 : GMImzaDDL.SelectedItem.Value.ConvertToInt();
                gorevOnay.GMVekil = GMVekilChk.Checked;
                gorevOnay.Sure = SureTxt.Text;
                gorevOnay.Yevmiye = YevmiyeTxt.Text;
                gorevOnay.Id = gorevOnay.Save();
            }
            return gorevOnay;
        }
        
        private bool validateInputValues()
        {
            bool isEmpty = string.IsNullOrEmpty(BasTarTxt.Value.ConvertToDatetimeEmptyIfNull()) ||
                string.IsNullOrEmpty(BitTarTxt.Value.ConvertToDatetimeEmptyIfNull()) ||
                (BasSaatDDL.SelectedItem == null) ||
                (BitSaatDDL.SelectedItem == null) ||
                string.IsNullOrEmpty(BasSaatDDL.SelectedItem.Value) ||
                string.IsNullOrEmpty(BitSaatDDL.SelectedItem.Value);
            return !isEmpty;
        }
        protected void UpdateBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Personel personel = new Personel();
                personel = PersonelGetir();
                if (personel != null)
                {
                    bool isValid = validateInputValues();
                    if (isValid)
                    {
                        bool isUpdated = Guncelle();
                        if (isUpdated)
                        {
                            MessageHelper.PublishMessage("Güncellendi", ProjeConstants.MESAJ_BASARILI, 2000);
                        }
                    }
                    else
                    {
                        MessageHelper.PublishMessage("Lütfen bilgileri tamamlayınız.", ProjeConstants.MESAJ_HATA);
                    }
                }
                else
                {
                    MessageHelper.PublishMessage("Personel Bulunamadı!", ProjeConstants.MESAJ_HATA);
                }

            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper(exception);
                Exception exceptionInfo = new Exception("Güncellenemedi");
                exceptionHelper.Exceptions.Add(exceptionInfo);
                exceptionHelper.PublishException();
            }

        }
        private bool Guncelle()
        {
            bool isUpdated = false;

            GorevOnay gorevOnay = new GorevOnay();
            gorevOnay = gorevOnay.Select<GorevOnay>(GorevOnayIdQS.ConvertToInt());
            if (gorevOnay != null)
            {
                DateTime bastar = BasTarTxt.Value.ConvertToDatetime();
                DateTime bittar = BitTarTxt.Value.ConvertToDatetime();
                string basSaat = BasSaatDDL.SelectedItem.Text;
                bastar = UtilityHelper.TariheSaatEkle(bastar, basSaat);
                string bitSaat = BitSaatDDL.SelectedItem.Text;
                bittar = UtilityHelper.TariheSaatEkle(bittar, bitSaat);

                gorevOnay.Aciklama = AciklamaTxt.Text;
                gorevOnay.AracPlakasi = AracPlakasiTxt.Text;
                gorevOnay.AracTahsisi = UlasimAraciDDL.SelectedItem.Text==ProjeConstants.ULASIMARACI_UCAK;
                gorevOnay.UlasimAraci = UlasimAraciDDL.SelectedItem.Text;
                gorevOnay.Avans = AvansTxt.Text;
                gorevOnay.BaslangicTarihi = bastar;
                gorevOnay.BitisTarihi = bittar;
                gorevOnay.Degistiren = CurrentUserName;
                gorevOnay.GorevinYeri = GorevinYeriTxt.Text;

                gorevOnay.OnayImza = OnayImzaDDL.SelectedItem == null ? 0 : OnayImzaDDL.SelectedItem.Value.ConvertToInt();
                gorevOnay.OnayMakam = OnayMakamDDL.SelectedItem == null ? 0 : OnayMakamDDL.SelectedItem.Value.ConvertToInt();
                gorevOnay.OnayMakamVekil = OnayVekilChk.Checked;
                gorevOnay.ParaBirimi = ParaBirimiDDL.SelectedItem == null ? "" : ParaBirimiDDL.SelectedItem.Value;
                gorevOnay.UlasimAraci = UlasimAraciDDL.SelectedItem == null ? "" : UlasimAraciDDL.SelectedItem.Value;
                gorevOnay.PerSubeImza = PerSubeImzaDDL.SelectedItem == null ? 0 : PerSubeImzaDDL.SelectedItem.Value.ConvertToInt();
                gorevOnay.PerSubeVekil = PersubeVekilChk.Checked;
                gorevOnay.GMImza = GMImzaDDL.SelectedItem == null ? 0 : GMImzaDDL.SelectedItem.Value.ConvertToInt();
                gorevOnay.GMVekil = GMVekilChk.Checked;
                gorevOnay.GorevinSebebi = GorevinSebebiTxt.Text;
                gorevOnay.Sure = SureTxt.Text;
                gorevOnay.Yevmiye = YevmiyeTxt.Text;
                isUpdated = gorevOnay.Update();
            }
            return isUpdated;
        }
        protected void PersonelDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            PersonelIdQS = PersonelDDL.SelectedItem.Value;
            Personel personel = PersonelGetir();
            if (personel != null)
            {
                PersonelAdiLbl.Text = personel.Adi + " " + personel.Soyadi;

            }

        }
        protected void RaporAlBtn_Click(object sender, EventArgs e)
        {

            string fullUrl = string.Format("{0}?GorevOnayId={1}", ProjeConstants.RAPOR_GOREVONAYBELGESI_URL, GorevOnayIdQS);
            RedirectToPage(fullUrl);
        }
        private string SureHesapla()
        {
            DateTime bastar = BasTarTxt.Value.ConvertToDatetime();
            DateTime bittar = BitTarTxt.Value.ConvertToDatetime();
            string basSaat = BasSaatDDL.SelectedItem.Text;
            bastar = UtilityHelper.TariheSaatEkle(bastar, basSaat);
            string bitSaat = BitSaatDDL.SelectedItem.Text;
            bittar = UtilityHelper.TariheSaatEkle(bittar, bitSaat);

            TimeSpan toplamSure = bittar - bastar;
            string sureStr = string.Empty;
            int gun = toplamSure.Days;
            int saat = toplamSure.Hours;
            if (gun < 1)
            {
                sureStr = toplamSure.Hours + " Saat";
            }
            else
            {
                if (saat >= 12)
                {
                    sureStr = (gun + 1) + " Gün";
                }
                else
                {
                    sureStr = gun + " Gün";
                }
            }

            return sureStr;
        }
        private bool GorevOnayiSil(GorevOnay gorevOnay)
        {
            bool isSaved = false;
            try
            {
                if (gorevOnay != null)
                {
                    isSaved = gorevOnay.Delete();

                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }

            return isSaved;
        }
        protected void SureHesaplaBtn_Click(object sender, EventArgs e)
        {
            SureTxt.Text = SureHesapla();
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
        protected void DeleteBtn_Click(object sender, EventArgs e)
        {
            SilLbl.Text = "Lütfen Dikkat: Görev Onayı Silinecek";
            SilmeMesajiLbl.Text = "Görev Onayını Silmek İstediğinizden Emin misiniz?";
            DeleteNowBtn.Visible = true;
            var openPopup = "OpenModal();";
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), openPopup, true);
        }
        protected void DeleteNowBtn_Click(object sender, EventArgs e)
        {
            try
            {
                GorevOnay gorevOnay = new GorevOnay();
                gorevOnay = gorevOnay.Select<GorevOnay>(GorevOnayIdQS.ConvertToInt());
                if (gorevOnay != null)
                {
                    bool isDeleted = GorevOnayiSil(gorevOnay);
                    if (isDeleted)
                    {
                        MessageHelper.PublishMessage("Görev Onayı Silindi", ProjeConstants.MESAJ_BASARILI, 2000);
                        RedirectToPage(ProjeConstants.PAGE_GOREVONAY_LIST + "?Mesaj=true");
                    }
                }
            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper(exception);
                Exception exceptionInfo = new Exception("Görev Onayı Silinemedi");
                exceptionHelper.Exceptions.Add(exceptionInfo);
                exceptionHelper.PublishException();
            }
        }
    }
}
