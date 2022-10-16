using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.MulkiyetiOlmayanTasinmazGirisiWP
{
    [ToolboxItemAttribute(false)]
    public partial class MulkiyetiOlmayanTasinmazGirisiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public MulkiyetiOlmayanTasinmazGirisiWP()
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
        private string CurrentUserName
        {
            get
            {

                if (ViewState["CurrentUserName"] == null)
                {
                    ViewState["CurrentUserName"] = UtilityHelper.GetCurrentUser();
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
            EnvanterdeMiQS = "2";
        }
        private void GorunumAyarlariniYap()
        {
            if (DestinationAppQS.Equals("TD"))
            {
                SaveBtn.Visible = false;
                UpdateBtn.Visible = true;
                OnarimlarBtn.Visible = true;
                ResimlerBtn.Visible = true;
                BagimsizBolumBtn.Visible = false;
                SigortaBtn.Visible = false;
                DeleteBtn.Visible = true;
                BackBtn.Visible = true;
                //CardHeader.Attributes["Class"] = "bg-info";
                TitleLbl.CssClass = "col-form-label font-weight-bold mb-1 text-primary";
                TitleLbl.Text = "Mülkiyeti Olmayan Taşınmaz Bilgi Güncelleme";
                IdLbl.Visible = true;
            }
            else
            {
                SaveBtn.Visible = true;
                UpdateBtn.Visible = false;
                OnarimlarBtn.Visible = false;
                ResimlerBtn.Visible = false;
                DeleteBtn.Visible = false;
                BackBtn.Visible = false;
                IdLbl.Visible = false;
                //CardHeader.Attributes["Class"] = "bg-success";
                TitleLbl.CssClass = "col-form-label font-weight-bold mb-1 text-danger";
                TitleLbl.Text = "Mülkiyeti Olmayan Taşınmaz Girişi";
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

            Tasinmaz tasinmaz = new Tasinmaz();
            tasinmaz = tasinmaz.Select(tasinmazId);
            if (tasinmaz != null)
            {
                tasinmazBulunamadi = false;
                IdLbl.Text = tasinmazId.ToString();
                TasinmazFormunuDoldur(tasinmaz);
            }
            if (tasinmazBulunamadi)
                MessageHelper.PublishMessage("Taşınmaz Bulunamadı!", ProjeConstants.MESAJ_HATA);
        }
        private void DDLleriDoldur()
        {
            IlDDLDoldur();
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
                CinsiTxt.Text = tasinmaz.Cinsi;

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
                AdresTxt.Text = tasinmaz.Adres;

                AciklamaTxt.Text = tasinmaz.Aciklama;

                MahalleTxt.Text = tasinmaz.Mahalle;
                KoyTxt.Text = tasinmaz.Koy;
                CaddeTxt.Text = tasinmaz.Cadde;
                SokakTxt.Text = tasinmaz.Sokak;


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
            tasinmaz.Adres = AdresTxt.Text;
            tasinmaz.Cinsi = CinsiTxt.Text;
            tasinmaz.Ilcesi = IlcesiDDL.SelectedItem.ToString();
            ListItem ilItem = IliDDL.SelectedItem;
            tasinmaz.Ili = ilItem.Text;
            tasinmaz.SorumluBolge = SorumluBolgeTxt.Text;
            tasinmaz.KullanimDurumu = KullanimDurumuDDL.SelectedValue;
            tasinmaz.KatMulkiyeti = KatMulkiyetiDDL.SelectedValue;
            tasinmaz.KullanimSekli = KullanimSekliDDL.SelectedValue;
            tasinmaz.SigortaDurumu = SigortaDDL.SelectedValue;
            tasinmaz.Aciklama = AciklamaTxt.Text;
            tasinmaz.EnvanterdeMi = string.IsNullOrEmpty(EnvanterdeMiQS) ? ProjeConstants.TASINMAZ_ENVANTERDE : EnvanterdeMiQS.ConvertToInt();
            tasinmaz.Mahalle = MahalleTxt.Text;
            tasinmaz.Koy = KoyTxt.Text;
            tasinmaz.Cadde = CaddeTxt.Text;
            tasinmaz.Sokak = SokakTxt.Text;
            tasinmaz.EnvanterdeMi = ProjeConstants.MULKIYETTE_OLMAYAN_TASINMAZ;
            int id = tasinmaz.Save();
            tasinmaz.Id = id;

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
        private bool updateTasinmazData2Db(int tId)
        {
            bool isSaved = false;
            Tasinmaz tasinmaz = new Tasinmaz();
            tasinmaz = tasinmaz.Select(tId);
            //tasinmaz.Id = tId;
            if (tasinmaz != null)
            {
                tasinmaz.Adres = AdresTxt.Text;

                tasinmaz.Cinsi = CinsiTxt.Text;

                tasinmaz.Ilcesi = IlcesiDDL.SelectedItem.ToString();
                ListItem ilItem = IliDDL.SelectedItem;

                tasinmaz.Ili = ilItem.Text;
                tasinmaz.SorumluBolge = SorumluBolgeTxt.Text;
                tasinmaz.KullanimDurumu = KullanimDurumuDDL.SelectedValue;
                tasinmaz.KatMulkiyeti = KatMulkiyetiDDL.SelectedValue;

                tasinmaz.KullanimSekli = KullanimSekliDDL.SelectedValue;
                tasinmaz.SigortaDurumu = SigortaDDL.SelectedValue;

                tasinmaz.Aciklama = AciklamaTxt.Text;
                tasinmaz.Mahalle = MahalleTxt.Text;
                tasinmaz.Koy = KoyTxt.Text;
                tasinmaz.Cadde = CaddeTxt.Text;
                tasinmaz.Sokak = SokakTxt.Text;
                tasinmaz.EnvanterdeMi = ProjeConstants.MULKIYETTE_OLMAYAN_TASINMAZ;
                isSaved = tasinmaz.Update();
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
                newUrl = newUrl + "?DestinationApp=TD&TasinmazId=" + tasinmaz.Id + "&EnvanterdeMi=" + EnvanterdeMiQS;
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
            if (updateTasinmazData2Db(TasinmazIdQS.ConvertToInt()))
            {
                SigortaBtn.Visible = true;
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
            //else
            //{
            //    MessageHelper.PublishMessage("Bu taşınmazın Sigortası Bulunamadı. Lütfen sigorta Durumunu seçip kaydettikten sonra tekrar deneyiniz.", ProjeConstants.MESAJ_HATA);
            //}

        }
        protected void OnarimlarBtn_Click(object sender, EventArgs e)
        {
            //Tasinmaz tasinmaz = new Tasinmaz();
            //tasinmaz = tasinmaz.selectById(Utility.getInt(TasinmazIdLbl.Text));
            //string newUrl = "/pages/TasinmazOnarim.aspx?SenderApp=TD&tId=" + TasinmazIdLbl.Text;
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
            string newUrl = rootUrl + "/" + ProjeConstants.PAGE_PAGE_MULKIYETIOLMAYANTASINMAZ_LIST + "?PageIndex=" + PageIndexQS;

            Page.Response.Redirect(newUrl, true);
        }
        protected void DeleteBtn_Click(object sender, EventArgs e)
        {
            Tasinmaz tasinmaz = new Tasinmaz();
            tasinmaz = tasinmaz.Select(TasinmazIdQS.ConvertToInt());
            if (tasinmaz != null) //sildikten sonra önceki sayfaya dön
            {
                if (tasinmaz.Delete())
                {
                    string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                    string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));
                    newUrl += "/" + ProjeConstants.PAGE_PAGE_MULKIYETIOLMAYANTASINMAZ_LIST;
                    Page.Response.Redirect(newUrl, true);
                }
                else
                    MessageHelper.PublishMessage("Taşınmaz Silinemedi", ProjeConstants.MESAJ_HATA);
            }
            else
                MessageHelper.PublishMessage("Taşınmaz Silinemedi", ProjeConstants.MESAJ_HATA);
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
    }
}
