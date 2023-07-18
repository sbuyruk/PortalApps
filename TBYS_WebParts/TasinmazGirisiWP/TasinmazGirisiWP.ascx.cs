using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.TasinmazGirisiWP
{
    [ToolboxItemAttribute(false)]
    public partial class TasinmazGirisiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public TasinmazGirisiWP()
        {
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
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
        private string TasinmazIdQS
        {
            get
            {

                if (ViewState["TasinmazId"] == null)
                {
                    if (Page.Request.QueryString["TasinmazId"] != null)
                    {
                        ViewState["TasinmazId"] = Page.Request.QueryString["TasinmazId"];
                    }
                    else
                    {
                        ViewState["TasinmazId"] = string.Empty;
                    }
                }
                return ViewState["TasinmazId"].ToString();
            }

            set
            {
                ViewState["TasinmazId"] = value;
            }
        }
        private string EnvanterdeMiQS
        {
            get
            {

                if (ViewState["EnvanterdeMi"] == null)
                {
                    if (Page.Request.QueryString["EnvanterdeMi"] != null)
                    {
                        ViewState["EnvanterdeMi"] = Page.Request.QueryString["EnvanterdeMi"];
                    }
                    else
                    {
                        ViewState["EnvanterdeMi"] = string.Empty;
                    }
                }
                return ViewState["EnvanterdeMi"].ToString();
            }

            set
            {
                ViewState["EnvanterdeMi"] = value;
            }
        }
        private string PageIndexQS
        {
            get
            {

                if (ViewState["PageIndex"] == null)
                {
                    if (Page.Request.QueryString["PageIndex"] != null)
                    {
                        ViewState["PageIndex"] = Page.Request.QueryString["PageIndex"];
                    }
                    else
                    {
                        ViewState["PageIndex"] = string.Empty;
                    }
                }
                return ViewState["PageIndex"].ToString();
            }

            set
            {
                ViewState["PageIndex"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                DDLleriDoldur();
                if (String.IsNullOrEmpty(DestinationAppQS) || String.Equals(DestinationAppQS, ""))
                {
                    //TasinmazGirisi açılacak TG
                    TasinmazGirisi();
                }
                else if (String.Equals(DestinationAppQS, "TD"))
                {
                    //TasinmazDuzenle açılacak
                    TasinmazDuzenle();
                }
                GorunumAyarlariniYap();

            }
        }
        private void GorunumAyarlariniYap()
        {
            if (DestinationAppQS.Equals("TD"))
            {
                SaveBtn.Visible = false;
                UpdateBtn.Visible = true;
                PrevBtn.Visible = true;
                NextBtn.Visible = true;
                TasinmazKartiBtn.Visible = true;
                OnarimlarBtn.Visible = true;
                EnvanterdenCikarBtn.Visible = true;
                ResimlerBtn.Visible = true;
                BagisciBtn.Visible = true;
                BagimsizBolumBtn.Visible = false;
                SigortaBtn.Visible = false;
                DeleteBtn.Visible = true;
                BackBtn.Visible = true;
                //CardHeader.Attributes["Class"] = "bg-info";
                TitleLbl.CssClass = "col-form-label font-weight-bold mb-1 text-primary";
                TitleLbl.Text = "Taşınmaz Bilgi Güncelleme";
                IdLbl.Visible = true;
            }
            else
            {
                SaveBtn.Visible = true;
                UpdateBtn.Visible = false;
                PrevBtn.Visible = false;
                NextBtn.Visible = false;
                TasinmazKartiBtn.Visible = false;
                OnarimlarBtn.Visible = false;
                EnvanterdenCikarBtn.Visible = false;
                ResimlerBtn.Visible = false;
                BagisciBtn.Visible = false;
                DeleteBtn.Visible = false;
                BackBtn.Visible = false;
                IdLbl.Visible = false;
                //CardHeader.Attributes["Class"] = "bg-success";
                TitleLbl.CssClass = "col-form-label font-weight-bold mb-1 text-danger";
                TitleLbl.Text = "Taşınmaz Girişi";
            }
            if (EnvanterdeMiQS.Equals(ProjeConstants.TASINMAZ_ENVANTERDEN_CIKTI.ToString()))
            {
                SaveBtn.Visible = false;
                UpdateBtn.Visible = true;
                PrevBtn.Visible = false;
                NextBtn.Visible = false;
                TasinmazKartiBtn.Visible = false;
                OnarimlarBtn.Visible = false;
                EnvanterdenCikarBtn.Visible = false;
                ResimlerBtn.Visible = true;
                BagimsizBolumBtn.Visible = false;
                BagisciBtn.Visible = true;
                SigortaBtn.Visible = false;

                KopyalaBtn.Visible = true;
                DeleteBtn.Visible = true;
                BackBtn.Visible = true;
                IdLbl.Visible = true;
                TitleLbl.CssClass = "col-form-label font-weight-bold mb-1 text-secondary";
                TitleLbl.Text = "Envanterden Çıkarılmış Taşınmaz";
            }
            if (KatMulkiyetiDDL.SelectedValue == ProjeConstants.KAT_MULKIYETI_VAR)
            {
                BagimsizBolumBtn.Visible = false;
            }
            else if (KatMulkiyetiDDL.SelectedValue == ProjeConstants.KAT_MULKIYETI_YOK)
            {
                BagimsizBolumBtn.Visible = true;
            }

            if (SigortaDDL.SelectedValue == ProjeConstants.SIGORTA_YOK)
            {
                SigortaBtn.Visible = false;
            }
            else
            {
                SigortaBtn.Visible = true;
            }

        }
        private void TasinmazGirisi()
        {

        }
        private void TasinmazDuzenle()
        {
            bool tasinmazBulunamadi = true;
            int tasinmazId = (TasinmazIdQS.ConvertToInt());
            if (EnvanterdeMiQS.Equals(ProjeConstants.TASINMAZ_ENVANTERDEN_CIKTI.ToString()))
            {
                Tasinmaz envanterdenCikanTasinmaz = new Tasinmaz();
                envanterdenCikanTasinmaz = envanterdenCikanTasinmaz.SelectEnvanterdenCikanTasinmaz(tasinmazId);
                if (envanterdenCikanTasinmaz != null)
                {
                    tasinmazBulunamadi = false;
                    IdLbl.Text = "(Taşınmaz No: "+tasinmazId.ToString()+")";
                    TasinmazFormunuDoldur(envanterdenCikanTasinmaz);

                }
            }
            else
            {

                Tasinmaz tasinmaz = new Tasinmaz();
                tasinmaz = tasinmaz.Select<Tasinmaz>(tasinmazId);
                if (tasinmaz != null)
                {
                    tasinmazBulunamadi = false;
                    IdLbl.Text = "(Taşınmaz No: " + tasinmazId.ToString() + ")";
                    TasinmazFormunuDoldur(tasinmaz);
                }
            }
            if (tasinmazBulunamadi)
                MessageHelper.PublishMessage("Taşınmaz Bulunamadı!", ProjeConstants.MESAJ_HATA);


        }
        private void DDLleriDoldur()
        {
            IlDDLDoldur();
            MulkiyetDDLDoldur();
            EdinmeSekliDDLDoldur();
            KiraDurumuDDLDoldur();
            KullanimDurumuDDLDoldur();
            KullanimSekliDDLDoldur();
            KatMulkiyetiDDLDoldur();
            SigortaDurumuDDLDoldur();
        }
        private void IlDDLDoldur()
        {
            if (IliDDL.SelectedItem == null)
            {
                IliDDL.Items.Clear();
                Il newil = new Il();
                List<Il> list = newil.SelectAll<Il>();
                foreach (Il il in list)
                {
                    IliDDL.Items.Add(new ListItem(il.IlAdi, il.Id.ToString()));
                }
                IlceDDLDoldur();
                BolgeTxtDoldur();
            }

        }
        private void IlceDDLDoldur()
        {
            IlcesiDDL.Items.Clear();
            Ilce pilce = new Ilce();

            List<Ilce> list = pilce.SelectByIlId(IliDDL.SelectedValue.ConvertToInt());
            foreach (Ilce ilce in list)
            {
                IlcesiDDL.Items.Add(new ListItem(ilce.IlceAdi, ilce.Id.ToString()));
            }
        }
        private void BolgeTxtDoldur()
        {
            Il il = new Il();
            il = il.SelectByIlAdi(IliDDL.SelectedItem.ToString());
            if (il != null)
            {
                SorumluBolgeTxt.Text = il.Bolge;
            }
        }
        private void EdinmeSekliDDLDoldur()
        {
            EdinmeSekliDDL.Items.Clear();
            EdinmeSekliDDL.Items.Add("Bağış");
            EdinmeSekliDDL.Items.Add("Vasiyetin Tenfizi");
            EdinmeSekliDDL.Items.Add("Mahkeme Kararı");
            EdinmeSekliDDL.Items.Add("Satın Alma");
            EdinmeSekliDDL.Items.Add("Tashih/Cins Tashihi");
            EdinmeSekliDDL.Items.Add("İmar Uygulaması");
            EdinmeSekliDDL.Items.Add("Kadastro (Yenileme)");
            EdinmeSekliDDL.Items.Add("Trampa/Takas");
            EdinmeSekliDDL.Items.Add("İfraz");
            EdinmeSekliDDL.Items.Add("Toplulaştırma");
            EdinmeSekliDDL.Items.Add("Tevhit");
            EdinmeSekliDDL.Items.Add("Kat Mülkiyeti");
            EdinmeSekliDDL.Items.Add("Kamulaştırma (Tümü)");
            EdinmeSekliDDL.Items.Add("Kamulaştırma (Kısmi)");
            EdinmeSekliDDL.Items.Add("TÜRK KARA KUV.GÜÇ.VAKFI");
            EdinmeSekliDDL.Items.Add("TÜRK DENİZ KUV.GÜÇ.VAKFI");
            EdinmeSekliDDL.Items.Add("TÜRK HAVA KUV.GÜÇ.VAKFI");
        }
        private void KullanimDurumuDDLDoldur()
        {
            KullanimDurumuDDL.Items.Clear();
            KullanimDurumuDDL.Items.Add(ProjeConstants.KULLANIMDURUMU_KIRADA);
            KullanimDurumuDDL.Items.Add(ProjeConstants.KULLANIMDURUMU_KDBOS);
            //KullanimDurumuDDL.Items.Add(ProjeConstants.KULLANIMDURUMU_KDBAGKUL);
            KullanimDurumuDDL.Items.Add(ProjeConstants.KULLANIMDURUMU_KDCM);
            KullanimDurumuDDL.Items.Add(ProjeConstants.KULLANIMDURUMU_KDTAAH);
            KullanimDurumuDDL.Items.Add(ProjeConstants.KULLANIMDURUMU_KDCOKHIS);
            KullanimDurumuDDL.Items.Add(ProjeConstants.KULLANIMDURUMU_KDHUKSOR);
            KullanimDurumuDDL.Items.Add(ProjeConstants.KULLANIMDURUMU_KDKIRAC);
            KullanimDurumuDDL.Items.Add(ProjeConstants.KULLANIMDURUMU_KDVAKKUL);
            KullanimDurumuDDL.Items.Add(ProjeConstants.KULLANIMDURUMU_KDKIRAKABYOK);
            KullanimDurumuDDL.Items.Add(ProjeConstants.KULLANIMDURUMU_RISKLIYAPI_KENTSELDONUSUM);
            KullanimDurumuDDL.Items.Add(ProjeConstants.KULLANIMDURUMU_KATKARSILIGI_YENIYAPI);
            KullanimDurumuDDL.Items.Add(ProjeConstants.KULLANIMDURUMU_DIGER);

        }
        private void KiraDurumuDDLDoldur()
        {
            KiraDurumuDDL.Items.Clear();
            KiraDurumuDDL.Items.Add(ProjeConstants.KIRADURUMU_KIRAYAUYGUN);
            KiraDurumuDDL.Items.Add(ProjeConstants.KIRADURUMU_KIRAYAUYGUNDEGIL);
        }
        private void MulkiyetDDLDoldur()
        {
            MukliyetSekliDDL.Items.Clear();
            MukliyetSekliDDL.Items.Add(ProjeConstants.MULKIYETSEKLI_TM);
            MukliyetSekliDDL.Items.Add(ProjeConstants.MULKIYETSEKLI_CM);

        }
        private void KullanimSekliDDLDoldur()
        {
            KullanimSekliDDL.Items.Clear();
            KullanimSekliDDL.Items.Add(ProjeConstants.KULLANIMSEKLI_ISHANI);
            KullanimSekliDDL.Items.Add(ProjeConstants.KULLANIMSEKLI_APT);
            KullanimSekliDDL.Items.Add(ProjeConstants.KULLANIMSEKLI_ARSA);
            KullanimSekliDDL.Items.Add(ProjeConstants.KULLANIMSEKLI_ISYERI);
            KullanimSekliDDL.Items.Add(ProjeConstants.KULLANIMSEKLI_MEV);
            KullanimSekliDDL.Items.Add(ProjeConstants.KULLANIMSEKLI_MESKEN);
            KullanimSekliDDL.Items.Add(ProjeConstants.KULLANIMSEKLI_TARLA);
            KullanimSekliDDL.Items.Add(ProjeConstants.KULLANIMSEKLI_TESIS);
        }
        private void KatMulkiyetiDDLDoldur()
        {
            KatMulkiyetiDDL.Items.Clear();
            KatMulkiyetiDDL.Items.Add(ProjeConstants.KAT_MULKIYETI_VAR);
            KatMulkiyetiDDL.Items.Add(ProjeConstants.KAT_MULKIYETI_YOK);

        }
        private void SigortaDurumuDDLDoldur()
        {
            SigortaDDL.Items.Clear();
            SigortaDDL.Items.Add(ProjeConstants.SIGORTA_YOK);
            SigortaDDL.Items.Add(ProjeConstants.SIGORTA_DASK_IHTIYARI);
            SigortaDDL.Items.Add(ProjeConstants.SIGORTA_DEPREM_IHTIYARI);

        }
        private bool TasinmazFormunuDoldur(Tasinmaz tasinmaz)
        {
            bool formDolduMu = false;
            try
            {
                AdaNoTxt.Text = tasinmaz.AdaNo;
                ArsaPayiTxt.Text = tasinmaz.ArsaPayi;
                EdinmeSekliDDL.SelectedValue = tasinmaz.EdinmeSekli;
                BagisYiliTxt.Text = tasinmaz.BagisYili;
                CiltNoTxt.Text = tasinmaz.CiltNo;
                CinsiTxt.Text = tasinmaz.Cinsi;
                EmlakBeyanDegeriTxt.Value = tasinmaz.EmlakBeyanDegeri.ToString();
                TapuTasinmazNoTxt.Value = tasinmaz.TapuTasinmazNo;
                InsaYiliTxt.Value = tasinmaz.InsaYili;
                EmlakSicilNoTxt.Text = tasinmaz.EmlakSicilNo;
                TapuTarihiTxt.Value = tasinmaz.TapuTarihi.ConvertToDatetimeEmptyIfNull();
                ListItem ilItem = IliDDL.Items.FindByText(tasinmaz.Ili);
                if (ilItem != null)
                    IliDDL.SelectedValue = ilItem.Value;
                IlceDDLDoldur();
                BolgeTxtDoldur();
                SigortaDurumuDDLDoldur();
                if (IlcesiDDL.Items.FindByText(tasinmaz.Ilcesi) != null)
                    IlcesiDDL.SelectedValue = IlcesiDDL.Items.FindByText(tasinmaz.Ilcesi).Value;
                if (KullanimDurumuDDL.Items.FindByText(tasinmaz.KullanimDurumu) != null)
                    KullanimDurumuDDL.SelectedValue = KullanimDurumuDDL.Items.FindByText(tasinmaz.KullanimDurumu).Value;
                if (KatMulkiyetiDDL.Items.FindByText(tasinmaz.KatMulkiyeti) != null)
                    KatMulkiyetiDDL.SelectedValue = KatMulkiyetiDDL.Items.FindByText(tasinmaz.KatMulkiyeti).Value;
                if (KullanimSekliDDL.Items.FindByText(tasinmaz.KullanimSekli) != null)
                    KullanimSekliDDL.SelectedValue = KullanimSekliDDL.Items.FindByText(tasinmaz.KullanimSekli).Value;
                if (SigortaDDL.Items.FindByText(tasinmaz.SigortaDurumu) != null)
                    SigortaDDL.SelectedValue = SigortaDDL.Items.FindByText(tasinmaz.SigortaDurumu).Value;
                //UtilityHelper.SetDDLValue(IlcesiDDL, tasinmaz.Ilcesi);
                //UtilityHelper.SetDDLValue(KullanimDurumuDDL, tasinmaz.KullanimDurumu);
                //UtilityHelper.SetDDLValue(KatMulkiyetiDDL, tasinmaz.KatMulkiyeti);
                //UtilityHelper.SetDDLValue(KullanimSekliDDL, tasinmaz.KullanimSekli);
                //UtilityHelper.SetDDLValue(SigortaDDL, tasinmaz.SigortaDurumu);
                UtilityHelper.SetDDLValue(KiraDurumuDDL, tasinmaz.KiraDurumu);
                AdresTxt.Text = tasinmaz.Adres;
                if (MukliyetSekliDDL.Items.FindByText(tasinmaz.MulkiyetSekli) != null)
                    MukliyetSekliDDL.SelectedValue = MukliyetSekliDDL.Items.FindByText(tasinmaz.MulkiyetSekli).Value;
                NitelikTxt.Text = tasinmaz.Nitelik;
                BulunduguKatTxt.Text = tasinmaz.BulunduguKat;
                ToplamKatSayisiTxt.Text = tasinmaz.ToplamKatSayisi;
                MetrekareTxt.Text = tasinmaz.Metrekare;
                PaftaNoTxt.Text = tasinmaz.PaftaNo;
                ParselNoTxt.Text = tasinmaz.ParselNo;
                SahifeNoTxt.Text = tasinmaz.SahifeNo;
                TahminiRayicDegeriTxt.Value = tasinmaz.TahminiRayicDegeri.ToString();
                TapuTarihiTxt.Value = tasinmaz.TapuTarihi.ConvertToDatetimeEmptyIfNull();
                VakifHissesiTxt.Text = tasinmaz.VakifHissesi;
                YevmiyeNoTxt.Text = tasinmaz.YevmiyeNo;
                YuzolcumuTxt.Text = tasinmaz.Yuzolcumu;
                AciklamaTxt.Text = tasinmaz.Aciklama;
                EnvantereGirisTarihiTxt.Value = tasinmaz.EnvantereGirisTarihi.ConvertToDatetimeEmptyIfNull();
                MahalleTxt.Text = tasinmaz.Mahalle;
                KoyTxt.Text = tasinmaz.Koy;
                CaddeTxt.Text = tasinmaz.Cadde;
                SokakTxt.Text = tasinmaz.Sokak;
                BagimsizBolumNoTxt.Text = tasinmaz.BagimsizBolumNo;
                MevkiTxt.Text = tasinmaz.Mevki;
                TamHisseTxt.Text = tasinmaz.TamHisse;
                HisseMiktariPayTxt.Text = tasinmaz.HisseMiktariPay;
                HisseMiktariPaydaTxt.Text = tasinmaz.HisseMiktariPayda;

                MahalleTxt.Text = tasinmaz.Mahalle;
                formDolduMu = true;
            }
            catch (Exception ex)
            {
                ExceptionHelper exhelper = new ExceptionHelper();
                Exception message = new Exception("Taşınmaz ekrana getirilemedi");
                exhelper.Exceptions.Add(message);
                exhelper.Exceptions.Add(ex);
                exhelper.PublishException();
                formDolduMu = false;
            }
            return formDolduMu;
        }
        private Tasinmaz SaveTasinmazData2Db()
        {
            Tasinmaz tasinmaz = new Tasinmaz();
            tasinmaz.AdaNo = AdaNoTxt.Text;
            tasinmaz.Adres = AdresTxt.Text;
            tasinmaz.ArsaPayi = ArsaPayiTxt.Text;
            tasinmaz.EdinmeSekli = EdinmeSekliDDL.SelectedValue;
            tasinmaz.BagisYili = BagisYiliTxt.Text;
            tasinmaz.CiltNo = CiltNoTxt.Text;
            tasinmaz.Cinsi = CinsiTxt.Text;
            tasinmaz.EmlakBeyanDegeri = EmlakBeyanDegeriTxt.Value.ConvertToDecimal();
            tasinmaz.TapuTasinmazNo = TapuTasinmazNoTxt.Value;
            tasinmaz.InsaYili = InsaYiliTxt.Value;
            tasinmaz.TahminiRayicDegeri = TahminiRayicDegeriTxt.Value.ConvertToDecimal();
            tasinmaz.EmlakSicilNo = EmlakSicilNoTxt.Text;
            tasinmaz.Ilcesi = IlcesiDDL.SelectedItem.ToString();
            ListItem ilItem = IliDDL.SelectedItem;
            tasinmaz.Ili = ilItem.Text;
            tasinmaz.SorumluBolge = SorumluBolgeTxt.Text;
            tasinmaz.KullanimDurumu = KullanimDurumuDDL.SelectedValue;
            tasinmaz.KiraDurumu = KiraDurumuDDL.SelectedValue;
            tasinmaz.KatMulkiyeti = KatMulkiyetiDDL.SelectedValue;
            tasinmaz.Nitelik = NitelikTxt.Text;
            tasinmaz.BulunduguKat = BulunduguKatTxt.Text;
            tasinmaz.ToplamKatSayisi = ToplamKatSayisiTxt.Text;
            tasinmaz.Metrekare = MetrekareTxt.Text;
            tasinmaz.KullanimSekli = KullanimSekliDDL.SelectedValue;
            tasinmaz.SigortaDurumu = SigortaDDL.SelectedValue;
            tasinmaz.MulkiyetSekli = MukliyetSekliDDL.SelectedValue;
            tasinmaz.PaftaNo = PaftaNoTxt.Text;
            tasinmaz.ParselNo = ParselNoTxt.Text;
            tasinmaz.SahifeNo = SahifeNoTxt.Text;
            tasinmaz.TapuTarihi = TapuTarihiTxt.Value.ConvertToDatetime();
            tasinmaz.EnvantereGirisTarihi = EnvantereGirisTarihiTxt.Value.ConvertToDatetime();
            tasinmaz.VakifHissesi = VakifHissesiTxt.Text;
            tasinmaz.YevmiyeNo = YevmiyeNoTxt.Text;
            tasinmaz.Yuzolcumu = YuzolcumuTxt.Text;
            tasinmaz.Aciklama = AciklamaTxt.Text;
            tasinmaz.EnvanterdeMi = string.IsNullOrEmpty(EnvanterdeMiQS) ? ProjeConstants.TASINMAZ_ENVANTERDE.ConvertToInt() : EnvanterdeMiQS.ConvertToInt();
            tasinmaz.Mahalle = MahalleTxt.Text;
            tasinmaz.Koy = KoyTxt.Text;
            tasinmaz.Cadde = CaddeTxt.Text;
            tasinmaz.Sokak = SokakTxt.Text;
            tasinmaz.BagimsizBolumNo = BagimsizBolumNoTxt.Text;
            tasinmaz.Mevki = MevkiTxt.Text;
            tasinmaz.TamHisse = TamHisseTxt.Text;
            tasinmaz.HisseMiktariPay = HisseMiktariPayTxt.Text;
            tasinmaz.HisseMiktariPayda = HisseMiktariPaydaTxt.Text;
            int id = tasinmaz.Save();
            tasinmaz.Id = id;
            //tasınmaz tablosundaki Bagisci alanı her kaydedildiğinde Ad+soyad olarak güncellesin
            Bagis bagis = new Bagis();
            bagis = bagis.SelectByTasinmazId(id);
            if (bagis != null)
            {
                TasinmazBagisci bagisci = new TasinmazBagisci();
                bagisci = bagisci.Select<TasinmazBagisci>(bagis.BagisciId);
                if (bagisci != null)
                {
                    tasinmaz.BagisciId = bagisci.Id ;
                    tasinmaz.Bagisci = bagisci.Adi + ' ' + bagisci.Soyadi;
                    tasinmaz.Update();
                }
            }
            if (id > 0)
            {
                Sigorta sigorta = new Sigorta();
                sigorta = sigorta.SelectByTasinmazId(tasinmaz.Id);
                if (sigorta == null)//henuz sigorta kaydı yok yeni sigorta yarat
                {
                    sigorta = new Sigorta();
                    sigorta.TasinmazId = tasinmaz.Id;
                    sigorta.SigortaCinsi = tasinmaz.SigortaDurumu;
                    int sigortaid = sigorta.Save();
                    sigorta.Id = sigortaid;
                }
                else
                {
                    sigorta.SigortaCinsi = tasinmaz.SigortaDurumu;
                    sigorta.Update();
                }
                return tasinmaz;
            }

            else return null;
        }
        private bool UpdateTasinmazData2Db(int tId)
        {
            bool isSaved = false;
            Tasinmaz tasinmaz = new Tasinmaz();
            tasinmaz = tasinmaz.Select(tId);
            //tasinmaz.Id = tId;
            if (tasinmaz != null)
            {
                tasinmaz.Adres = AdresTxt.Text;
                tasinmaz.AdaNo = AdaNoTxt.Text;
                tasinmaz.ArsaPayi = ArsaPayiTxt.Text;
                tasinmaz.EdinmeSekli = EdinmeSekliDDL.SelectedValue;
                tasinmaz.BagisYili = BagisYiliTxt.Text;
                tasinmaz.CiltNo = CiltNoTxt.Text;
                tasinmaz.Cinsi = CinsiTxt.Text;
                tasinmaz.EmlakBeyanDegeri = EmlakBeyanDegeriTxt.Value.ConvertToDecimal();
                tasinmaz.TahminiRayicDegeri = TahminiRayicDegeriTxt.Value.ConvertToDecimal();
                tasinmaz.TapuTasinmazNo = TapuTasinmazNoTxt.Value;
                tasinmaz.InsaYili = InsaYiliTxt.Value;
                tasinmaz.EmlakSicilNo = EmlakSicilNoTxt.Text;
                tasinmaz.Ilcesi = IlcesiDDL.SelectedItem.ToString();
                ListItem ilItem = IliDDL.SelectedItem;

                tasinmaz.Ili = ilItem.Text;
                tasinmaz.SorumluBolge = SorumluBolgeTxt.Text;
                tasinmaz.KullanimDurumu = KullanimDurumuDDL.SelectedValue;
                tasinmaz.KiraDurumu = KiraDurumuDDL.SelectedValue;
                tasinmaz.KatMulkiyeti = KatMulkiyetiDDL.SelectedValue;
                tasinmaz.Nitelik = NitelikTxt.Text;
                tasinmaz.BulunduguKat = BulunduguKatTxt.Text;
                tasinmaz.ToplamKatSayisi = ToplamKatSayisiTxt.Text;
                tasinmaz.Metrekare = MetrekareTxt.Text;
                tasinmaz.KullanimSekli = KullanimSekliDDL.SelectedValue;
                tasinmaz.SigortaDurumu = SigortaDDL.SelectedValue;
                tasinmaz.MulkiyetSekli = MukliyetSekliDDL.SelectedValue;
                tasinmaz.PaftaNo = PaftaNoTxt.Text;
                tasinmaz.ParselNo = ParselNoTxt.Text;
                tasinmaz.SahifeNo = SahifeNoTxt.Text;

                tasinmaz.TapuTarihi = TapuTarihiTxt.Value.ConvertToDatetime();
                tasinmaz.EnvantereGirisTarihi = EnvantereGirisTarihiTxt.Value.ConvertToDatetime();
                tasinmaz.VakifHissesi = VakifHissesiTxt.Text;
                tasinmaz.YevmiyeNo = YevmiyeNoTxt.Text;
                tasinmaz.Yuzolcumu = YuzolcumuTxt.Text;
                tasinmaz.Aciklama = AciklamaTxt.Text;
                tasinmaz.EnvanterdeMi = string.IsNullOrEmpty(EnvanterdeMiQS) ? ProjeConstants.TASINMAZ_ENVANTERDE.ConvertToInt() : EnvanterdeMiQS.ConvertToInt();
                tasinmaz.Mahalle = MahalleTxt.Text;
                tasinmaz.Koy = KoyTxt.Text;
                tasinmaz.Cadde = CaddeTxt.Text;
                tasinmaz.Sokak = SokakTxt.Text;
                tasinmaz.BagimsizBolumNo = BagimsizBolumNoTxt.Text;
                tasinmaz.Mevki = MevkiTxt.Text;
                tasinmaz.TamHisse = TamHisseTxt.Text;
                tasinmaz.HisseMiktariPay = HisseMiktariPayTxt.Text;
                tasinmaz.HisseMiktariPayda = HisseMiktariPaydaTxt.Text;
                isSaved = tasinmaz.Update();

                Bagis bagis = new Bagis();
                bagis = bagis.SelectByTasinmazId(tasinmaz.Id);
                if (bagis != null)
                {
                    TasinmazBagisci bagisci = new TasinmazBagisci();
                    bagisci = bagisci.Select<TasinmazBagisci>(bagis.BagisciId);
                    if (bagisci != null)
                    {
                        tasinmaz.BagisciId = bagisci.Id;
                        tasinmaz.Bagisci = bagisci.Adi + ' ' + bagisci.Soyadi;
                        tasinmaz.Update();
                    }

                }

                if (!tasinmaz.EnvanterdeMi.Equals(ProjeConstants.TASINMAZ_ENVANTERDEN_CIKTI))
                {
                    Sigorta sigorta = new Sigorta();
                    sigorta = sigorta.SelectByTasinmazId(tasinmaz.Id);
                    if (sigorta == null)//henuz sigorta kaydı yok yeni sigorta yarat
                    {
                        sigorta = new Sigorta();
                        sigorta.TasinmazId = tasinmaz.Id;
                        sigorta.SigortaCinsi = tasinmaz.SigortaDurumu;
                        sigorta.Save();
                    }
                    else
                    {
                        sigorta.SigortaCinsi = tasinmaz.SigortaDurumu;
                        sigorta.Update();
                    } 
                }
            }

            return isSaved;
        }
        protected void SaveBtn_Click(object sender, EventArgs e)
        {

            Tasinmaz tasinmaz = SaveTasinmazData2Db();
            if (tasinmaz != null)
            {
                string newUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                int queryIndex = newUrl.IndexOf("?");
                if (queryIndex > 0)
                    newUrl = newUrl.Substring(0, queryIndex);
                newUrl = newUrl + "?DestinationApp=TD&TasinmazId=" + tasinmaz.Id;
                //+ "&tasinmazFoto=" + tasinmaz.TasinmazFoto //kaydesttikten sonra tasinmaz resimleri görünsün
                //+ "&tasinmazFoto1=" + tasinmaz.TasinmazFoto1 + "&tasinmazFoto2=" + tasinmaz.TasinmazFoto2
                //+ "&tapuFoto=" + tasinmaz.TapuFoto + "&krokiFoto=" + tasinmaz.KrokiFoto + "&tahkikatFoto=" + tasinmaz.TahkikatFoto;
                Page.Response.Redirect(newUrl, true);
            }
            else
                MessageHelper.PublishMessage("Taşınmaz Kaydı başarısız oldu.-TS001", ProjeConstants.MESAJ_HATA);
        }
        protected void UpdateBtn_Click(object sender, EventArgs e)
        {
            if (UpdateTasinmazData2Db(TasinmazIdQS.ConvertToInt()))
            {
                MessageHelper.PublishMessage("Taşınmaz kaydı güncellendi.", ProjeConstants.MESAJ_BASARILI, 2000);
            }
            else
            {
                MessageHelper.PublishMessage("Taşınmaz Güncellenemedi.-TS001", ProjeConstants.MESAJ_HATA);
            }
        }
        protected void TasinmazKartiBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));
            newUrl += "/" + ProjeConstants.PAGE_TASINMAZ_KARTI + "?SenderApp=TD&TasinmazId=" + TasinmazIdQS + "&PageIndex=" + PageIndexQS + "&EnvanterdeMi=" + EnvanterdeMiQS;
            Page.Response.Redirect(newUrl, true);
        }
        protected void BagimsizBolumBtn_Click(object sender, EventArgs e)
        {
            //Tasinmaz tasinmaz = new Tasinmaz();
            //tasinmaz = tasinmaz.selectById(Utility.getInt(TasinmazIdLbl.Text));
            //string newUrl = "/pages/BagimsizBolum.aspx?SenderApp=TD&tId=" + TasinmazIdLbl.Text;
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));
            newUrl += "/" + ProjeConstants.PAGE_TASINMAZ_BAGIMSIZBOLUM + "?SenderApp=TD&TasinmazId=" + TasinmazIdQS + "&PageIndex=" + PageIndexQS + "&EnvanterdeMi=" + EnvanterdeMiQS;
            Page.Response.Redirect(newUrl, true);
        }
        protected void SigortaBtn_Click(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(TasinmazIdQS))
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));
                newUrl += "/" + ProjeConstants.PAGE_TASINMAZSIGORTA_EKLESIL + "?SenderApp=TD&TasinmazId=" + TasinmazIdQS + "&PageIndex=" + PageIndexQS + "&EnvanterdeMi=" + EnvanterdeMiQS;
                Page.Response.Redirect(newUrl, true);
            }
        }
        protected void OnarimlarBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));
            newUrl += "/" + ProjeConstants.PAGE_TASINMAZONARIM_GIRIS + "?SenderApp=TD&TasinmazId=" + TasinmazIdQS + "&PageIndex=" + PageIndexQS + "&EnvanterdeMi=" + EnvanterdeMiQS;
            Page.Response.Redirect(newUrl, true);
        }
        protected void EnvanterdenCikarBtn_Click(object sender, EventArgs e)
        {
            //string newUrl = "/pages/EnvanterdenCikarma.aspx?SenderApp=TD&tId=" + TasinmazIdLbl.Text;
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));
            newUrl += "/" + ProjeConstants.PAGE_TASINMAZ_ENVANTERDEN_CIKARMA + "?SenderApp=TD&TasinmazId=" + TasinmazIdQS;
            Page.Response.Redirect(newUrl, true);
        }
        protected void BackBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string rootUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));
            string newUrl = rootUrl + "/" + ProjeConstants.PAGE_TASINMAZ_LIST + "?PageIndex=" + PageIndexQS;
            if (EnvanterdeMiQS.Equals(ProjeConstants.TASINMAZ_ENVANTERDEN_CIKTI.ToString()))
            {
                newUrl = rootUrl + "/" + ProjeConstants.PAGE_ENVANTERDENCIKANTASINMAZ_LIST + "?PageIndex=" + PageIndexQS;
            }
            Page.Response.Redirect(newUrl, true);
        }
        protected void DeleteBtn_Click(object sender, EventArgs e)
        {
            ///Taşınmazın siinmesi bir dizi konrol ile yapılabilir. Şu an silme yetkisi kaldırıldı SB 24.06.2021
            if (true)
            {
                MessageHelper.PublishMessage("Taşınmaz silme yetkiniz bulunmamaktadır. Taşınmaz silinemez.", ProjeConstants.MESAJ_HATA);
            }
            else {
#pragma warning disable CS0162 // Unreachable code detected
                Tasinmaz tasinmaz = new Tasinmaz();
#pragma warning restore CS0162 // Unreachable code detected
                tasinmaz = tasinmaz.Select<Tasinmaz>(TasinmazIdQS.ConvertToInt());
                if (tasinmaz != null) //sildikten sonra önceki sayfaya dön
                {

                    if (!SozlesmesiVarMi(tasinmaz))
                    {
                        if (tasinmaz.Delete())
                        {

                            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));
                            newUrl += "/" + ProjeConstants.PAGE_TASINMAZ_LIST;
                            Page.Response.Redirect(newUrl, true);

                        }
                        else
                            MessageHelper.PublishMessage("Taşınmaz silinemedi", ProjeConstants.MESAJ_HATA);
                    }
                    else
                    {
                        MessageHelper.PublishMessage("Taşınmaza ait sözleşme bulunmaktadır. Taşınmaz silinemez.", ProjeConstants.MESAJ_HATA);
                    }
                }
                else
                    MessageHelper.PublishMessage("Taşınmaz bulunamadı", ProjeConstants.MESAJ_HATA); 
            }
        }

        private bool SozlesmesiVarMi(Tasinmaz tasinmaz)
        {
            SozlesmeTasinmaz sozlesmeTasinmaz = new SozlesmeTasinmaz();
            List<SozlesmeTasinmaz> liste = sozlesmeTasinmaz.SelectByTasinmazId(tasinmaz.Id);
            return (liste.Count > 1);
        }

        protected void ResimlerBtn_Click(object sender, EventArgs e)
        {
            Tasinmaz tasinmaz = new Tasinmaz();
            tasinmaz = tasinmaz.Select(TasinmazIdQS.ConvertToInt());
            if (tasinmaz != null)
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));
                newUrl += "/" + ProjeConstants.PAGE_TASINMAZ_RESIMLER + "?TasinmazId=" + TasinmazIdQS + "&PageIndex=" + PageIndexQS + "&EnvanterdeMi=" + EnvanterdeMiQS;
                //"&tasinmazFoto=" + tasinmaz.TasinmazFoto
                //+ "&tasinmazFoto1=" + tasinmaz.TasinmazFoto1 + "&tasinmazFoto2=" + tasinmaz.TasinmazFoto2
                //+ "&tapuFoto=" + tasinmaz.TapuFoto + "&krokiFoto=" + tasinmaz.KrokiFoto + "&tahkikatFoto=" + tasinmaz.TahkikatFoto;
                Page.Response.Redirect(newUrl, true);
            }


        }
        protected void BagisciBtn_Click(object sender, EventArgs e)
        {
            Bagis bagis = new Bagis();
            bagis = bagis.SelectByTasinmazId(TasinmazIdQS.ConvertToInt());

            if (bagis == null)
            {
                MessageHelper.PublishMessage("Bağışçı Bulunamadı. Bu taşınmaz henüz bir bağışçıyla ilişkilendirilmemiş. Lütfen Bağışçı sayfasından bağışçı ataması yapınız.", ProjeConstants.MESAJ_HATA);
            }
            else
            {
                TasinmazBagisci bagisci = new TasinmazBagisci();
                bagisci = bagisci.Select<TasinmazBagisci>(bagis.BagisciId);
                if (bagisci == null)
                {
                    MessageHelper.PublishMessage("Bağışçı Bulunamadı. Bu taşınmazın ilişkilendirilildiği bağışçı bulunamadı. Lütfen Bağışçı sayfasından bağışçı ataması yapınız.", ProjeConstants.MESAJ_HATA);
                }
                else
                {
                    string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                    string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));
                    newUrl += "/" + ProjeConstants.PAGE_TASINMAZBAGISCI_KARTI + "?SenderApp=TD&TasinmazId=" + TasinmazIdQS + "&BagisciId=" + bagisci.Id + "&PageIndex=" + PageIndexQS + "&EnvanterdeMi=" + EnvanterdeMiQS;
                    Page.Response.Redirect(newUrl, true);
                }

            }


        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        protected void NextBtn_Click(object sender, EventArgs e)
        {
            Tasinmaz tasinmaz = new Tasinmaz();
            tasinmaz = tasinmaz.Select<Tasinmaz>(TasinmazIdQS.ConvertToInt());
            Tasinmaz sonrakiTasinmaz = tasinmaz.SelectNext(tasinmaz.Id);
            string newUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            int queryIndex = newUrl.IndexOf("?");
            if (queryIndex > 0)
                newUrl = newUrl.Substring(0, queryIndex);
            newUrl = newUrl + "?DestinationApp=TD&TasinmazId=" + sonrakiTasinmaz.Id + "&PageIndex=" + PageIndexQS + "&EnvanterdeMi=" + EnvanterdeMiQS;
            Page.Response.Redirect(newUrl, true);
        }
        protected void PrevBtn_Click(object sender, EventArgs e)
        {
            Tasinmaz tasinmaz = new Tasinmaz();
            tasinmaz = tasinmaz.Select<Tasinmaz>(TasinmazIdQS.ConvertToInt());
            Tasinmaz oncekiTasinmaz = tasinmaz.SelectPrev(tasinmaz.Id);
            string newUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            int queryIndex = newUrl.IndexOf("?");
            if (queryIndex > 0)
                newUrl = newUrl.Substring(0, queryIndex);
            newUrl = newUrl + "?DestinationApp=TD&TasinmazId=" + oncekiTasinmaz.Id + "&PageIndex=" + PageIndexQS + "&EnvanterdeMi=" + EnvanterdeMiQS;
            Page.Response.Redirect(newUrl, true);
        }
        protected void KatMulkiyetiDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (KatMulkiyetiDDL.SelectedValue == ProjeConstants.KAT_MULKIYETI_VAR)
            {
                BagimsizBolumBtn.Visible = false;
            }
            else if (KatMulkiyetiDDL.SelectedValue == ProjeConstants.KAT_MULKIYETI_YOK)
            {
                BagimsizBolumBtn.Visible = true;
            }
        }
        protected void IliDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            IlceDDLDoldur();
            BolgeTxtDoldur();
        }
        protected void KopyalaBtn_Click(object sender, EventArgs e)
        {

            try
            {
                //Onay Popup Aç

                KopyalaNowBtn.Visible = true;
                var openPopup = "OpenModal();";
                UtilityHelper.ScriptCalistir(openPopup);
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        /// <summary>
        /// Envanterden Çıkmış bir taşınmazı yeni bir taşınmaz olarak kopyalar
        /// yeni kopyalanarak yaratılan taşınmaza önceki taşınmazdan gelen bağışçı, sigorta ve onarım bilgilrini taşır
        /// envanterden çıkmış taşınmazın resimlerini yeni yaratılan taşınmaza verir
        /// Eklendi 10.06.2022 KiraSozlesmesi aktif olan tasinmazları da yeni kopyalanan tasinmaza aktarır
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void KopyalaNowBtn_Click(object sender, EventArgs e)
        {
            //Önce onay popup aç 
            //onay evetse envanterdeMi=1 yap ve Tasinmaz_Table'a ayrı bir kayıt olarak insert et, 
            //Resimleri Kopyalasın mı?? bağımsız bolumleri kopytalasın mı? sigortaları kopyalasın mı?
            Tasinmaz envanterdencikmisTasinmaz = new Tasinmaz();
            envanterdencikmisTasinmaz = envanterdencikmisTasinmaz.SelectEnvanterdenCikanTasinmaz(TasinmazIdQS.ConvertToInt());
            ExceptionHelper eh = new ExceptionHelper();
            UtilityHelper.CopyAndCreateImageFromSPLibrary("100TapuFoto.jpg", "100test.jpg", ProjeConstants.RESIMLER_TASINMAZ, eh);

            if (envanterdencikmisTasinmaz != null)
            {
                //Tasinmaz yarat
                //envanterdemi=0, ve env çıkış bilgilerini yaz
                //tasinmazı kaydet
                int envanterdencikmisTasinmazId = envanterdencikmisTasinmaz.Id;
                Tasinmaz tasinmaz = new Tasinmaz();
                tasinmaz = envanterdencikmisTasinmaz;
                tasinmaz.EnvanterdeMi = ProjeConstants.TASINMAZ_ENVANTERDE.ConvertToInt();//true;
                tasinmaz.EnvanterdenCikmaSebebi = string.Empty;
                tasinmaz.EnvanterdenCikmaBedeli = 0;
                tasinmaz.EnvanterdenCikmaTarihi = string.Empty.ConvertToDatetime();
                int tasinmazId = tasinmaz.Save();

                #region Bagis
                //Bu yeni Id'li tasinmaz için Bagis Tablosunda yeni Bagis nesnesi yarat
                //BagisciId sini gir
                //Bagis nesnesini kaydet
                Bagis eskibagis = new Bagis();
                eskibagis = eskibagis.SelectByTasinmazId(envanterdencikmisTasinmazId);
                if (eskibagis != null)
                {
                    Bagis bagis = new Bagis();
                    bagis.BagisciId = tasinmaz.BagisciId;
                    bagis.TasinmazId = tasinmazId;
                    bagis.BagisTarihi = eskibagis.BagisTarihi;
                    bagis.BagisYili = eskibagis.BagisYili;
                    bagis.Envanterde = ProjeConstants.ENVANTERDE;//true;
                    bagis.Degistiren = CurrentUserName;
                    bagis.Save();
                }
                #endregion

                #region Sigorta
                //Sigorta verilerini de aktar
                Sigorta sigorta = new Sigorta();
                sigorta = sigorta.SelectByTasinmazId(envanterdencikmisTasinmazId);
                if (sigorta != null)
                {
                    sigorta.TasinmazId = tasinmazId;
                    sigorta.Update();
                }
                #endregion
                #region Onarım
                //Onarim verilerini de aktar
                Onarim onarimDao = new Onarim();
                List<Onarim> onarimList = onarimDao.SelectOnarimByTasinmazId(envanterdencikmisTasinmazId);
                foreach (Onarim item in onarimList)
                {
                    if (item != null)
                    {
                        item.TasinmazId = tasinmazId;
                        item.Update();
                    }
                }
                #endregion
                #region Kira İşlemleri
                //SozlesmeTasinmaz_Table'da KiraSozlesmesi Aktif olan ve TasinmazId=EnvanterdenCikanTasinmazId olan var mı
                //Varsa SozlesmeTasinmaz_Table'da tasinmaz Id'sini yeni tasinmazId ile değiştir.
                SozlesmeTasinmaz st = new SozlesmeTasinmaz();
                List<SozlesmeTasinmaz> stList = st.SelectByTasinmazId(envanterdencikmisTasinmazId);
                foreach (var item in stList)
                {
                    item.TasinmazId = tasinmazId;
                    item.Update();
                }
                #endregion
                string newUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                int queryIndex = newUrl.IndexOf("?");
                if (queryIndex > 0)
                    newUrl = newUrl.Substring(0, queryIndex);
                newUrl = newUrl + "?DestinationApp=TD&TasinmazId=" + tasinmaz.Id;

                Page.Response.Redirect(newUrl, true);
            }
            else
                MessageHelper.PublishMessage("Taşınmaz Kaydı başarısız oldu.-TS001", ProjeConstants.MESAJ_HATA);
        }
    }
}
