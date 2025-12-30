using Model.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;
using static Model.IKYS.Personel;

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
        private IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    //if (AuthQS.Equals("IKYS"))
                    //{
                    //    PersonelDiv.Attributes["style"] = "display:block";
                    //    //GorevOnayListesiBtn.Visible = true;
                    //}
                    //else if (AuthQS.Equals("BIRIM"))
                    //{
                    //    PersonelDiv.Attributes["style"] = "display:block";
                    //    //GorevOnayListesiBtn.Visible = false;
                    //}
                    //else
                    //{
                    //    //personelDiv display:none yap
                    //    PersonelDiv.Attributes["style"] = "display:none";
                    //    //GorevOnayListesiBtn.Visible = false;
                    //}
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
                            MessageHelper.PublishMessage("Görev kaydı bulunamadı. Yeni görev girişi yapabilirsiniz.", ProjeConstants.MESAJ_HATA);
                            OpenGiris();
                        }
                        else
                        {
                            OpenDuzenle();
                        }
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

                if (personel != null)
                {
                    PersonelAdiLbl.Text = personel.Adi + " " + personel.Soyadi;
                    FillBasSaat();
                    FillBitSaat();
                    UlkeDDLDoldur(personel);
                    YevmiyeGetir();
                    HarcirahHesapla();
                    FillUlasimAraciDDL();
                    PersonelDDLDoldur(personel);
                    PerSubeImzaDDLDoldur();
                    OnayImzaDDLDoldur();
                    FillGorevOnayForm(gorevOnay);
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
                FillUlasimAraciDDL();
                PerSubeImzaDDLDoldur();
                OnayImzaDDLDoldur();
                Personel personel = PersonelGetir();
                PersonelDDLDoldur(personel);
                UlkeDDLDoldur(personel);
                YevmiyeGetir();
                HarcirahHesapla();
                if (personel != null)
                {
                    PersonelAdiLbl.Text = personel.Adi + " " + personel.Soyadi;
                    DateTime today = DateTime.Today;
                    TimeSpan mesaiBasSaati = ProjeConstants.MESAI_BASLAMA_SAATI;
                    TimeSpan mesaiBitSaati = ProjeConstants.MESAI_BITIS_SAATI;
                    DateTime bastar = today + mesaiBasSaati;
                    DateTime bittar = today + mesaiBitSaati;
                    BaslangicTarihiTxt.Text = bastar.ConvertToDatetimeEmptyIfNull();
                    BitisTarihiTxt.Text = bittar.ConvertToDatetimeEmptyIfNull();
                    string bassaat = bastar.ToString("HH:mm");
                    string bitsaat = bittar.ToString("HH:mm");
                    SelectDDLByText(BasSaatDDL, bassaat);
                    SelectDDLByText(BitSaatDDL, bitsaat);
                    UtilityHelper.SetDDLValue(OnayImzaDDL, ProjeConstants.GOREV_IKUZMANI.ToString());
                }


            }
        }
        private void PersonelDDLDoldur(Personel personel)
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
                UtilityHelper.SetDDLValue(BitSaatDDL, bitsaatStr);
            }
            else
            {
                UtilityHelper.SetDDLValue(BitSaatDDL, "17:00");
            }

        }
        private void PerSubeImzaDDLDoldur()
        {

            PerSubeImzaDDL.Items.Clear();
            Personel personel = new Personel();
            DataTable dataTable = personel.SelectCalisanPersonelByBirimIdReturnDataTable(ProjeConstants.PER_SUBE_INT, PersonelTipi.Kadrolu);//SelectCalisanPersonelReturnDataTable(PersonelTipi.Kadrolu);
            foreach (DataRow dataRow in dataTable.Rows)
            {
                int personelId = dataRow["PersonelId"].ConvertToInt();
                string adiSoyadi = dataRow["Adi"].ToString() + " " + dataRow["Soyadi"].ToString();
                string gorev = dataRow["Gorev"].ToString();
                string birim = dataRow["BirimSube"].ToString();
                if (gorev.Contains(ProjeConstants.UNVAN_DIREKTOR)|| gorev.Contains(ProjeConstants.UNVAN_BASUZMAN)|| gorev.Contains(ProjeConstants.UNVAN_KIDEMLIUZMAN))
                {
                    ListItem li = new ListItem(adiSoyadi, personelId.ToString());
                    PerSubeImzaDDL.Items.Add(li);
                }

            }
        }
        private void OnayImzaDDLDoldur()
        {
            OnayImzaDDL.Items.Clear();
            Personel personel = new Personel();
            DataTable dataTable = personel.SelectCalisanPersonelByBirimIdReturnDataTable(ProjeConstants.PER_SUBE_INT, PersonelTipi.Kadrolu);//SelectCalisanPersonelReturnDataTable(PersonelTipi.Kadrolu);
            foreach (DataRow dataRow in dataTable.Rows)
            {
                int personelId = dataRow["PersonelId"].ConvertToInt();
                string adiSoyadi = dataRow["Adi"].ToString() + " " + dataRow["Soyadi"].ToString();
                string gorev = dataRow["Gorev"].ToString();
                string birim = dataRow["BirimSube"].ToString();
                if (gorev.Contains(ProjeConstants.UNVAN_UZMAN) || gorev.Contains(ProjeConstants.UNVAN_KIDEMLIUZMAN))
                { 
                    ListItem li = new ListItem(adiSoyadi, personelId.ToString());
                    OnayImzaDDL.Items.Add(li);
                }
            }
        }
        private void UlkeDDLDoldur(Personel personel)
        {
            UlkeDDL.Items.Clear();
            

            if (personel != null)
            {
                GorevTanim gorevTanim = new GorevTanim();
                gorevTanim=gorevTanim.SelectByPersonelId(personel.Id);
                if (gorevTanim != null)
                {
                    Harcirah harcirah = new Harcirah();
                    List<Harcirah> list = harcirah.SelectByKadroGrupId(gorevTanim.HarcirahGrupId);
                    if (list.Count>0)
                    {
                        GorevGrubuTxt.Text = list[0].KadroGrubu;
                        foreach (Harcirah item in list)
                        {
                            string ulke = item.Ulke;
                            if (UlkeDDL.Items.FindByText(ulke) != null)
                            {
                                continue;
                            }
                            else
                            {
                                ListItem li = new ListItem(item.Ulke, item.Id.ToString());
                                UlkeDDL.Items.Add(li);
                            }
                        } 
                    }

                }
            }
            
        }


        /// <summary>
        /// QueryString'den gelen GorevOnayIdQS ile GorevOnay_Table'dan kayıt getir
        /// Bu bilgileri forma doldur
        /// Güncelle butonunu aç
        /// Kaydet butonunu sakla
        /// </summary>
        private void FillGorevOnayForm(GorevOnay gorevOnay)
        {
            
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

                ParaBirimiTxt.Text= gorevOnay.ParaBirimi.ToString();
                YevmiyeParaBirimiTxt.Text= gorevOnay.ParaBirimi.ToString();
                UtilityHelper.SetDDLValue(PersonelDDL, gorevOnay.PersonelId.ToString());
                UtilityHelper.SetDDLValue(UlasimAraciDDL, gorevOnay.UlasimAraci.ToString());
                string bassaat = gorevOnay.BaslangicTarihi.ToString("HH:mm");
                SelectDDLByText(BasSaatDDL, bassaat);
                string bitsaat = gorevOnay.BitisTarihi.ToString("HH:mm");
                SelectDDLByText(BitSaatDDL, bitsaat);
                BaslangicTarihiTxt.Text = gorevOnay.BaslangicTarihi.ConvertToDatetimeEmptyIfNull();
                BitisTarihiTxt.Text = gorevOnay.BitisTarihi.ConvertToDatetimeEmptyIfNull();

                GorevinYeriTxt.Text = gorevOnay.GorevinYeri.ReturnEmptyIfNull().ToString();
                GorevinSebebiTxt.Text = gorevOnay.GorevinSebebi.ReturnEmptyIfNull().ToString();
                AciklamaTxt.Text = gorevOnay.Aciklama.ReturnEmptyIfNull().ToString();
                UtilityHelper.SetDDLValue(PerSubeImzaDDL, gorevOnay.PerSubeImza.ToString());
                
                UtilityHelper.SetDDLValue(OnayImzaDDL, gorevOnay.OnayImza.ToString());
                PersubeVekilChk.Checked = gorevOnay.PerSubeVekil.ConvertToBool();
                SureTxt.Text = gorevOnay.Sure.ReturnEmptyIfNull().ToString();
                AvansTxt.Text = gorevOnay.Avans.ReturnEmptyIfNull().ToString();
                YevmiyeTxt.Text = gorevOnay.Yevmiye.ReturnEmptyIfNull().ToString();
                GunlukYevmiyeTxt.Text = gorevOnay.GunlukYevmiye.ReturnEmptyIfNull().ToString();
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

        private GorevOnay Kaydet()
        {
            GorevOnay gorevOnay = new GorevOnay();
            Personel personel = PersonelGetir();
            if (personel != null)
            {
                DateTime bastar = BaslangicTarihiTxt.Text.ConvertToDatetime();
                DateTime bittar = BitisTarihiTxt.Text.ConvertToDatetime();
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
                gorevOnay.ParaBirimi = ParaBirimiTxt.Text;
                gorevOnay.UlasimAraci = UlasimAraciDDL.SelectedItem == null ? "" : UlasimAraciDDL.SelectedItem.Value;
                gorevOnay.PersonelId = personel.Id;
                gorevOnay.PerSubeImza = PerSubeImzaDDL.SelectedItem == null ? 0 : PerSubeImzaDDL.SelectedItem.Value.ConvertToInt();
                gorevOnay.PerSubeVekil = PersubeVekilChk.Checked;
                gorevOnay.GorevinSebebi = GorevinSebebiTxt.Text;
                gorevOnay.Sure = SureTxt.Text;
                gorevOnay.Yevmiye = YevmiyeTxt.Text;
                gorevOnay.GunlukYevmiye = GunlukYevmiyeTxt.Text;
                gorevOnay.Id = gorevOnay.Save();

            }
            return gorevOnay;
        }

        private void KaydetModalAc()
        {

            string basSaat = " saat " + (string.IsNullOrEmpty(BasSaatDDL.SelectedItem.Text.Trim())?string.Empty: BasSaatDDL.SelectedItem.Text.Trim());
            string bitSaat = " saat " + (string.IsNullOrEmpty(BitSaatDDL.SelectedItem.Text.Trim())?string.Empty: BitSaatDDL.SelectedItem.Text.Trim());
            string konustr = string.IsNullOrEmpty(GorevinSebebiTxt.Text.Trim()) ? string.Empty : " '" + GorevinSebebiTxt.Text.Trim() + "' konulu";

            MessageTitleLbl.Text = "Görev kaydedilecek";
            MessageTextLbl.Text = BaslangicTarihiTxt.Text + " günü," + basSaat + " ile " + BitisTarihiTxt.Text + " günü " +bitSaat +" arasına " + konustr +" görev kaydedilsin mi?";
            DeleteNowBtn.Visible = false;
            KaydetNowBtn.Visible = true;    
            var openPopup = "OpenModal();";
            UtilityHelper.ScriptCalistir(openPopup);
        }

        private bool ValidateInputValues(int gorevOnayId)
        {
            bool isEmpty = string.IsNullOrEmpty(BaslangicTarihiTxt.Text.ConvertToDatetimeEmptyIfNull()) ||
                string.IsNullOrEmpty(BitisTarihiTxt.Text.ConvertToDatetimeEmptyIfNull()) ||
                (BasSaatDDL.SelectedItem == null) ||
                (BitSaatDDL.SelectedItem == null) ||
                string.IsNullOrEmpty(BasSaatDDL.SelectedItem.Value) ||
                string.IsNullOrEmpty(BitSaatDDL.SelectedItem.Value);
            bool isDateUsed = false;
            // Girilen tarihlerin geçerli olup olmadığını kontrol et
            if (!isEmpty)
            {
                DateTime basTarih = BaslangicTarihiTxt.Text.ConvertToDatetime();
                DateTime bitTarih = BitisTarihiTxt.Text.ConvertToDatetime();
                if (basTarih > bitTarih)
                {
                    MessageHelper.PublishMessage("Başlangıç tarihi bitiş tarihinden sonra olamaz.", ProjeConstants.MESAJ_HATA);
                    isEmpty = true;
                }
            }
            // Girilen tarihler içinde başka bir görev olup olmadığını kontrol et
            if (!isEmpty)
            {
                GorevOnay gorevOnay = new GorevOnay();
                int personelId = PersonelDDL.SelectedItem == null ? 0 : PersonelDDL.SelectedItem.Value.ConvertToInt();
                if (personelId > 0)
                {
                    DateTime basTarih = BaslangicTarihiTxt.Text.ConvertToDatetime();
                    DateTime bitTarih = BitisTarihiTxt.Text.ConvertToDatetime();
                    string basSaat = BasSaatDDL.SelectedItem.Text;
                    basTarih = UtilityHelper.TariheSaatEkle(basTarih, basSaat);
                    string bitSaat = BitSaatDDL.SelectedItem.Text;
                    bitTarih = UtilityHelper.TariheSaatEkle(bitTarih, bitSaat);
                    isDateUsed = gorevOnay.GorevOnayVarMi(personelId, basTarih, bitTarih, gorevOnayId);
                    if (isDateUsed)
                    {
                        MessageHelper.PublishMessage("Bu tarihlerde başka bir görev kaydı bulunmaktadır.", ProjeConstants.MESAJ_HATA);
                    }
                }
            }
            return (!isEmpty && !isDateUsed);
        }
        
        private bool Guncelle()
        {
            bool isUpdated = false;

            GorevOnay gorevOnay = new GorevOnay();
            gorevOnay = gorevOnay.Select<GorevOnay>(GorevOnayIdQS.ConvertToInt());
            if (gorevOnay != null)
            {
                DateTime bastar = BaslangicTarihiTxt.Text.ConvertToDatetime();
                DateTime bittar = BitisTarihiTxt.Text.ConvertToDatetime();
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
                gorevOnay.ParaBirimi = ParaBirimiTxt.Text;
                gorevOnay.UlasimAraci = UlasimAraciDDL.SelectedItem == null ? "" : UlasimAraciDDL.SelectedItem.Value;
                gorevOnay.PerSubeImza = PerSubeImzaDDL.SelectedItem == null ? 0 : PerSubeImzaDDL.SelectedItem.Value.ConvertToInt();
                gorevOnay.PerSubeVekil = PersubeVekilChk.Checked;
                gorevOnay.GorevinSebebi = GorevinSebebiTxt.Text;
                gorevOnay.Sure = SureTxt.Text;
                gorevOnay.Yevmiye = YevmiyeTxt.Text;
                gorevOnay.GunlukYevmiye = GunlukYevmiyeTxt.Text;
                isUpdated = gorevOnay.Update();
            }
            return isUpdated;
        }


        private void YevmiyeGetir()
        {

            GunlukYevmiyeTxt.Text = "0";
            Harcirah harcirah = new Harcirah();
            harcirah = harcirah.Select(UlkeDDL.SelectedItem.Value.ConvertToInt());
            if (harcirah != null)
            {
                GunlukYevmiyeTxt.Text = harcirah.Miktar.ToString("N", culturInfo);
                ParaBirimiTxt.Text = harcirah.ParaBirimi.ToString();
                YevmiyeParaBirimiTxt.Text = harcirah.ParaBirimi.ToString();
                
            }
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
        
        private void HarcirahHesapla()
        {
            string sonuc = "0";
            if (HarcirahHesaplansinChk.Checked)
            {
                int dakika = SureDakikaTxt.Text.ConvertToInt();
                int saat = SureSaatTxt.Text.ConvertToInt() + (dakika > 0 ? 1 : 0);
                int gun = SureGunTxt.Text.ConvertToInt();

                decimal yevmiye = GunlukYevmiyeTxt.Text.ConvertToDecimal();
                decimal artan = saat == 0 ? 0 : (saat > 12 ? 1 : 0.5m);
                SureTxt.Text = artan + gun + " gün";
                sonuc = ((artan + gun) * yevmiye).ToString("N", culturInfo); 
            }
            YevmiyeTxt.Text = sonuc;
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
        #region Events
        protected void SaveBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Personel personel = new Personel();
                personel = PersonelGetir();
                if (personel != null)
                {
                    bool isValid = ValidateInputValues(0);
                    if (isValid)
                    {
                        KaydetModalAc();
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
        protected void UpdateBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Personel personel = new Personel();
                personel = PersonelGetir();
                if (personel != null)
                {
                    bool isValid = ValidateInputValues(GorevOnayIdQS.ConvertToInt());
                    if (isValid)
                    {
                        bool isUpdated = Guncelle();
                        if (isUpdated)
                        {
                            MessageHelper.PublishMessage("Güncellendi", ProjeConstants.MESAJ_BASARILI, 2000);
                        }
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
        protected void DeleteBtn_Click(object sender, EventArgs e)
        {
            MessageTitleLbl.Text = "Lütfen Dikkat: Görev Onayı Silinecek";
            MessageTextLbl.Text = "Görev Onayını Silmek İstediğinizden Emin misiniz?";
            DeleteNowBtn.Visible = true;
            KaydetNowBtn.Visible = false;
            var openPopup = "OpenModal();";
            UtilityHelper.ScriptCalistir(openPopup);
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
        protected void KaydetNowBtn_Click(object sender, EventArgs e)
        {
            try
            {
                GorevOnay gorevOnay = Kaydet();
                if (gorevOnay != null)
                {
                    if (!AuthQS.Equals("IKYS"))
                    {
                        Personel personel = new Personel();
                        personel = personel.Select<Personel>(PersonelIdQS.ConvertToInt());
                        if (personel != null)
                        {
                            IKYSOrtak.GorevOnayEPostasiGonder(personel, gorevOnay.Id, "YurtIci/YurtDisi");
                        }
                    }
                    RedirectToPage(ProjeConstants.PAGE_GOREVONAY_LIST + "?Mesaj=true"+ "&SecilenId="+gorevOnay.Id + (string.IsNullOrEmpty(AuthQS) ? string.Empty : "&Auth=" + ProjeConstants.IKYS_YETKILI_BIRIM));
                }
            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper(exception);
                Exception exceptionInfo = new Exception("Görev Onayı Kaydedilemedi");
                exceptionHelper.Exceptions.Add(exceptionInfo);
                exceptionHelper.PublishException();
            }
        }
        protected void HarcirahHesaplansinChk_CheckedChanged(object sender, EventArgs e)
        {
            HarcirahHesapla();

        } 
        protected void BaslangicTarihiTxt_TextChanged(object sender, EventArgs e)
        {
            HarcirahHesapla();

        }
        protected void BitisTarihiTxt_TextChanged(object sender, EventArgs e)
        {
            HarcirahHesapla();

        }
        protected void HarcirahHesaplaBtn_Click(object sender, EventArgs e)
        {
            HarcirahHesapla();
        }
        protected void PersonelDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            PersonelIdQS = PersonelDDL.SelectedItem.Value;
            Personel personel = PersonelGetir();
            if (personel != null)
            {
                PersonelAdiLbl.Text = personel.Adi + " " + personel.Soyadi;
                UlkeDDLDoldur(personel);
            }
            YevmiyeGetir();
            HarcirahHesapla();
        }        
        protected void BasSaatDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            YevmiyeGetir();
            HarcirahHesapla();
        }
        protected void BitSaatDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            YevmiyeGetir();
            HarcirahHesapla();
        }
        protected void UlkeDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            YevmiyeGetir();
            HarcirahHesapla();
        }
        protected void GorevOnayListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_GOREVONAY_LIST + "?SecilenId=" + GorevOnayIdQS + (string.IsNullOrEmpty(AuthQS) ? string.Empty : "&Auth=" + ProjeConstants.IKYS_YETKILI_BIRIM));
        }
        protected void RaporAlBtn_Click(object sender, EventArgs e)
        {

            string fullUrl = string.Format("{0}?GorevOnayId={1}", ProjeConstants.RAPOR_GOREVONAYBELGESI_URL, GorevOnayIdQS);
            RedirectToPage(fullUrl);
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        #endregion
    }
}
