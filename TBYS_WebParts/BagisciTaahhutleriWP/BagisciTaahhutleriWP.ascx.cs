using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.BagisciTaahhutleriWP
{
    [ToolboxItemAttribute(false)]
    public partial class BagisciTaahhutleriWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public BagisciTaahhutleriWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string BagisciIdQS
        {
            get
            {

                if (ViewState["BagisciId"] == null)
                {
                    if (Page.Request.QueryString["BagisciId"] != null)
                    {
                        ViewState["BagisciId"] = Page.Request.QueryString["BagisciId"];
                    }
                    else
                    {
                        ViewState["BagisciId"] = string.Empty;
                    }
                }
                return ViewState["BagisciId"].ToString();
            }

            set
            {
                ViewState["BagisciId"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    TasinmazBagisci bagisci = new TasinmazBagisci();
                    bagisci = bagisci.Select<TasinmazBagisci>(BagisciIdQS.ConvertToInt());
                    if (bagisci != null)
                    {
                        TasinmazDDLDoldur(bagisci);
                        IlDDLDoldur();
                        IlceDDLDoldur();
                        SagVefatDDLDoldur();
                        TaahhutTablosunuDoldur(bagisci);

                        BagisciyiTaahhutListesineEkleBtnGoster(bagisci);

                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void BagisciyiTaahhutListesineEkleBtnGoster(TasinmazBagisci bagisci)
        {
            BagisciyiTaahhutListesineEkleBtn.Visible = false;

            TasinmazTaahhut tt = new TasinmazTaahhut();
            tt = tt.SelectByTCKimlikNo(bagisci.TCKimlikNo);
            if (tt == null || tt.TCKimlikNo == 0)
            {
                BagisciyiTaahhutListesineEkleBtn.Visible = true;
            }
            if (bagisci.TCKimlikNo == 0)
            {
                MessageHelper.PublishMessage("Bağışçının TC Kimlik Numarası girilmemiş.", ProjeConstants.MESAJ_BILGI, 2000);
            }
        }

        private void IlDDLDoldur()
        {
            IliDDL.Items.Clear();

            Il newil = new Il();
            List<Il> list = newil.SelectAll<Il>();
            foreach (Il il in list)
            {
                if (string.IsNullOrEmpty(il.IlAdi.Trim()))
                    continue;
                IliDDL.Items.Add(new ListItem(il.IlAdi, il.Id.ToString()));
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
        private void TasinmazDDLDoldur(TasinmazBagisci bagisci)
        {
            TasinmazDDL.Items.Clear();
            Bagis bagis = new Bagis();
            DataTable dataTable = bagis.SelectTasinmazByBagisciIdReturnDT(bagisci.Id);
            if (dataTable != null)
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    string tasinmazIdStr = dataRow["TasinmazId"].ToString();
                    string cinsi = dataRow["Cinsi"].ToString();
                    string ilIlce = dataRow["IlIlce"].ToString();
                    string adres = dataRow["Adres"].ToString();
                    string mulkiyetSekli = dataRow["MulkiyetSekli"].ToString();
                    string kiraDurumu = dataRow["KiraDurumu"].ToString();
                    string tasinmazStr = adres + " " + ilIlce + " (" + cinsi + ", " + mulkiyetSekli + ")";
                    ListItem li = new ListItem(tasinmazStr, tasinmazIdStr);
                    TasinmazDDL.Items.Add(li);
                }
            }
        }
        private void SagVefatDDLDoldur()
        {
            ListItem li = new ListItem(ProjeConstants.BAGISCI_SAG);
            ListItem li2 = new ListItem(ProjeConstants.BAGISCI_VEFAT);

            SagVefatDDL.Items.Clear();
            SagVefatDDL.Items.Add(li);
            SagVefatDDL.Items.Add(li2);
        }
        private void TaahhutTableHeaders()
        {
            TaahhutTable.Rows.Clear();
            TableHeaderRow th = new TableHeaderRow();
            TableHeaderCell siraCell = new TableHeaderCell();
            siraCell.Text = "Sıra";
            TableHeaderCell AdiSoyadiCell = new TableHeaderCell();
            AdiSoyadiCell.Text = "Adı Soyadı";
            TableHeaderCell IliIlcesiCell = new TableHeaderCell();
            IliIlcesiCell.Text = "İli İlçesi";
            TableHeaderCell TasinmazCell = new TableHeaderCell();
            TasinmazCell.Text = "Taşınmaz";
            TableHeaderCell AciklamaCell = new TableHeaderCell();
            AciklamaCell.Text = "Taahhüt Açıklama";
            TableHeaderCell DuzenleCell = new TableHeaderCell();
            DuzenleCell.Text = "Düzenle";
            TableHeaderCell SilCell = new TableHeaderCell();
            SilCell.Text = "Sil";

            th.Controls.Add(siraCell);
            th.Controls.Add(AdiSoyadiCell);
            th.Controls.Add(IliIlcesiCell);
            th.Controls.Add(TasinmazCell);
            th.Controls.Add(AciklamaCell);
            th.Controls.Add(DuzenleCell);
            th.Controls.Add(SilCell);
            TaahhutTable.Controls.Add(th);
        }
        private void TaahhutTablosunuDoldur(TasinmazBagisci bagisci)
        {
            //Column headers
            TaahhutTableHeaders();
            TasinmazTaahhut tt = new TasinmazTaahhut();
            List<TasinmazTaahhut> list = tt.SelectByBagisciId(bagisci.Id);
            int SiraNo = 1;
            foreach (TasinmazTaahhut item in list)
            {
                TableRow row = new TableRow();

                TableCell SiranoCell = new TableCell();

                SiranoCell.Text = SiraNo++ + "";
                row.Controls.Add(SiranoCell);

                TableCell AdiSoyadiCell = new TableCell();
                AdiSoyadiCell.Text = item.Adi + " " + item.Soyadi;
                row.Controls.Add(AdiSoyadiCell);

                TableCell IliIlcesiCell = new TableCell();
                Ilce ilce = new Ilce();
                ilce = ilce.Select<Ilce>(item.Ilcesi);
                IliIlcesiCell.Text = ilce != null ? ilce.IlAdi + "/" + ilce.IlceAdi : string.Empty;
                row.Controls.Add(IliIlcesiCell);

                TableCell TasinmazCell = new TableCell();
                Tasinmaz tasinmaz = new Tasinmaz();
                tasinmaz = tasinmaz.Select<Tasinmaz>(item.TasinmazId);
                TasinmazCell.Text = tasinmaz != null ? tasinmaz.Adres + " " + tasinmaz.Ilcesi + "/" + tasinmaz.Ili : string.Empty;
                row.Controls.Add(TasinmazCell);

                TableCell AciklamaCell = new TableCell();
                AciklamaCell.Text = item.TaahhutAciklama;
                AciklamaCell.Width = new Unit("40%");
                row.Controls.Add(AciklamaCell);

                TableCell DuzenleCell = new TableCell();
                string duzenleLink = "<a href=# onclick=OpenModalTaahhut(" + item.Id + "); class=\'btn btn-outline-primary \'> Düzenle</a>";
                DuzenleCell.Text = duzenleLink;
                row.Controls.Add(DuzenleCell);

                TableCell SilCell = new TableCell();
                string silLink = "<a href=# onclick=OpenTaahhutSilModal(" + item.Id + "); class=\'btn btn-outline-danger \'> Sil</a>";
                SilCell.Text = silLink;
                row.Controls.Add(SilCell);

                TaahhutTable.Controls.Add(row);
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
        protected void BagisciBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_TASINMAZBAGISCI_GIRIS + "?DestinationApp=TBD&BagisciId=" + BagisciIdQS);
        }
        protected void IliDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            IlceDDLDoldur();
        }
        protected void SagVefatDDL_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (SagVefatDDL.SelectedItem.Value.Equals(ProjeConstants.BAGISCI_VEFAT))
            {
                VefatTarihiTxt.Value = string.Empty;
                VefatTarihiDiv.Attributes["style"] = "display:block";
            }
            else
            {
                VefatTarihiDiv.Attributes["style"] = "display:none";
            }
        }
        protected void TaahhutModalDoldurBtnBtn_Click(object sender, EventArgs e)
        {
            TaahhutKaydetBtn.Visible = false;
            TaahhutGuncelleBtn.Visible = false;

            int taahhutId = ParamTaahhutIdLbl.Value.ConvertToInt();
            TasinmazTaahhut tasinmazTaahhut = new TasinmazTaahhut();
            tasinmazTaahhut = tasinmazTaahhut.Select<TasinmazTaahhut>(taahhutId);
            if (tasinmazTaahhut != null)
            {
                AdiTxt.Text = tasinmazTaahhut.Adi;
                SoyadiTxt.Text = tasinmazTaahhut.Soyadi;
                TCKimlikNoTxt.Text = tasinmazTaahhut.TCKimlikNo.ToString();
                DogumTarihiTxt.Text = tasinmazTaahhut.DogumTarihi.ConvertToDatetimeEmptyIfNull();
                TelefonTxt.Text = tasinmazTaahhut.Telefon.ToString();
                AdresTxt.Text = tasinmazTaahhut.Adres.ToString();
                EvrakTarihiTxt.Text = tasinmazTaahhut.EvrakTarihi.ConvertToDatetimeEmptyIfNull();
                EvrakSayisiTxt.Text = tasinmazTaahhut.EvrakSayisi;
                UtilityHelper.SetDDLValue(SagVefatDDL, tasinmazTaahhut.Sag_vefat);
                if (SagVefatDDL.SelectedItem.Text.Equals(ProjeConstants.BAGISCI_SAG))
                {
                    VefatTarihiTxt.Value = String.Empty;
                    VefatTarihiDiv.Attributes["style"] = "display:none";
                }
                else if (SagVefatDDL.SelectedItem.Text.Equals(ProjeConstants.BAGISCI_VEFAT))
                {
                    VefatTarihiTxt.Value = tasinmazTaahhut.VefatTarihi.ConvertToDatetimeEmptyIfNull();
                    VefatTarihiDiv.Attributes["style"] = "display:block";
                }

                TaahhutAciklamaTxt.Text = string.IsNullOrEmpty(tasinmazTaahhut.TaahhutAciklama)? "Vakıf tarafından taahhütname verilmiştir.": tasinmazTaahhut.TaahhutAciklama;

                TaahhutGuncelleBtn.Visible = true;

                UtilityHelper.SetDDLValue(TasinmazDDL, tasinmazTaahhut.TasinmazId.ToString());
                UtilityHelper.SetDDLValue(IliDDL, tasinmazTaahhut.Ili.ToString());
                IlceDDLDoldur();
                UtilityHelper.SetDDLValue(IlcesiDDL, tasinmazTaahhut.Ilcesi.ToString());
            }
            else
            {
                AdiTxt.Text = SoyadiTxt.Text = TCKimlikNoTxt.Text = DogumTarihiTxt.Text = TelefonTxt.Text =
                    AdresTxt.Text = EvrakTarihiTxt.Text = EvrakSayisiTxt.Text = string.Empty;
                TaahhutAciklamaTxt.Text = "Vakıf tarafından taahhütname verilmiştir.";

                TaahhutKaydetBtn.Visible = true;
            }
        }
        protected void TaahhutKaydetBtn_Click(object sender, EventArgs e)
        {
            try
            {
                TasinmazTaahhut tt = new TasinmazTaahhut();

                tt.BagisciId = BagisciIdQS.ConvertToInt();
                tt.Adi = AdiTxt.Text;
                tt.Adres = AdresTxt.Text;
                tt.DogumTarihi = DogumTarihiTxt.Text.ConvertToDatetime();
                tt.EvrakSayisi = EvrakSayisiTxt.Text;
                tt.EvrakTarihi = EvrakTarihiTxt.Text.ConvertToDatetime();
                tt.Ili = IliDDL.SelectedItem.Value.ConvertToInt();
                tt.Ilcesi = IlcesiDDL.SelectedItem.Value.ConvertToInt();
                tt.Olusturan = UtilityHelper.GetCurrentUserLoginName();
                tt.OlusturmaTarihi = DateTime.Today;
                tt.Sag_vefat = SagVefatDDL.SelectedItem.Text;
                if (SagVefatDDL.SelectedItem.Text.Equals(ProjeConstants.BAGISCI_VEFAT))
                {
                    tt.VefatTarihi=VefatTarihiTxt.Value.ConvertToDatetime();
                }
                tt.Soyadi = SoyadiTxt.Text;
                tt.TaahhutAciklama = TaahhutAciklamaTxt.Text;
                tt.TasinmazId = TasinmazDDL.SelectedItem.Value.ConvertToInt();
                tt.TCKimlikNo = TCKimlikNoTxt.Text.ConvertToLong();
                tt.Telefon = TelefonTxt.Text;
                int id = tt.Save();
                if (id > 0)
                    MessageHelper.PublishMessage("Kaydedildi", ProjeConstants.MESAJ_BASARILI, 2000);
                RedirectToPage(ProjeConstants.PAGE_TASINMAZBAGISCI_TAAHHUTLERI+ "?BagisciId=" + BagisciIdQS);

            }
            catch (Exception ex)
            {

                ExceptionHelper eh = new ExceptionHelper(ex);
                eh.PublishException();
            }
        }
        protected void TaahhutGuncelle_Click(object sender, EventArgs e)
        {
            try
            {
                int taahhutId = ParamTaahhutIdLbl.Value.ConvertToInt();
                TasinmazTaahhut tt = new TasinmazTaahhut();
                tt = tt.Select<TasinmazTaahhut>(taahhutId);
                if (tt != null)
                {
                    tt.Adi = AdiTxt.Text;
                    tt.Adres = AdresTxt.Text;
                    tt.DogumTarihi = DogumTarihiTxt.Text.ConvertToDatetime();
                    tt.EvrakSayisi = EvrakSayisiTxt.Text;
                    tt.EvrakTarihi = EvrakTarihiTxt.Text.ConvertToDatetime();
                    tt.Ili = IliDDL.SelectedItem.Value.ConvertToInt();
                    tt.Ilcesi = IlcesiDDL.SelectedItem.Value.ConvertToInt();
                    tt.Degistiren = UtilityHelper.GetCurrentUserLoginName();
                    tt.DegistirmeTarihi = DateTime.Today;
                    tt.Soyadi = SoyadiTxt.Text;
                    tt.Sag_vefat = SagVefatDDL.SelectedItem.Text;
                    if (SagVefatDDL.SelectedItem.Text.Equals(ProjeConstants.BAGISCI_SAG))
                    {
                        tt.VefatTarihi = DateTime.MinValue;
                    }
                    else if (SagVefatDDL.SelectedItem.Text.Equals(ProjeConstants.BAGISCI_VEFAT))
                    {
                        tt.VefatTarihi = VefatTarihiTxt.Value.ConvertToDatetime();
                    }
                    tt.TaahhutAciklama = TaahhutAciklamaTxt.Text;
                    tt.TasinmazId = TasinmazDDL.SelectedItem.Value.ConvertToInt();
                    tt.TCKimlikNo = TCKimlikNoTxt.Text.ConvertToLong();
                    tt.Telefon = TelefonTxt.Text;
                    //tt.TaahhutPdfAdi = TaahhutPdfAdi.Text;
                    if (tt.Update())
                        MessageHelper.PublishMessage("Kaydedildi", ProjeConstants.MESAJ_BASARILI, 2000);
                    else
                        MessageHelper.PublishMessage("Kaydedilemedi", ProjeConstants.MESAJ_HATA);
                    RedirectToPage(ProjeConstants.PAGE_TASINMAZBAGISCI_TAAHHUTLERI+ "?BagisciId=" + BagisciIdQS);
                }
            }
            catch (Exception ex)
            {

                ExceptionHelper eh = new ExceptionHelper(ex);
                eh.PublishException();
            }
        }
        private void RedirectToPage(string pageUrl)
        {
            try
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                int index = currentUrl.LastIndexOf("?") < 0 ? currentUrl.Length : currentUrl.LastIndexOf("?");

                string rawUrl = currentUrl.Substring(0, index);
                string newUrl = rawUrl.Substring(0, rawUrl.LastIndexOf("/")) + "/" + pageUrl;
                Page.Response.Redirect(newUrl);
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void TaahhutSilNowBtn_Click(object sender, EventArgs e)
        {
            int taahhutId = ParamTaahhutIdLbl.Value.ConvertToInt();
            TasinmazTaahhut tt = new TasinmazTaahhut();
            tt = tt.Select<TasinmazTaahhut>(taahhutId);
            if (tt != null)
            {
                if (tt.Delete())
                    RedirectToPage(ProjeConstants.PAGE_TASINMAZBAGISCI_TAAHHUTLERI + "?BagisciId=" + BagisciIdQS);

            }
            else
                MessageHelper.PublishMessage("Taahhüt bulunamadı", ProjeConstants.MESAJ_BILGI);
        }
        protected void BagisciyiTaahhutListesineEkleBtn_Click(object sender, EventArgs e)
        {
            try
            {
                TasinmazBagisci tasinmazBagisci = new TasinmazBagisci();
                tasinmazBagisci = tasinmazBagisci.Select<TasinmazBagisci>(BagisciIdQS.ConvertToInt());
                if (tasinmazBagisci != null )
                {
                    TasinmazTaahhut tt = new TasinmazTaahhut();
                    tt = tt.SelectByTCKimlikNo(tasinmazBagisci.TCKimlikNo);
                    if (tt == null || tasinmazBagisci.TCKimlikNo == 0)
                    {
                        TasinmazTaahhut yeniTaahhut = new TasinmazTaahhut();
                        yeniTaahhut.Adi = tasinmazBagisci.Adi;
                        yeniTaahhut.Adres = tasinmazBagisci.Adres;
                        yeniTaahhut.BagisciId = tasinmazBagisci.Id;
                        yeniTaahhut.DogumTarihi = tasinmazBagisci.DogumTarihi;
                        Ilce ilce = new Ilce();
                        ilce = ilce.SelectByIlAndIlceAdi(tasinmazBagisci.Ilcesi, tasinmazBagisci.Ili);
                        if (ilce != null)
                        {
                            yeniTaahhut.Ilcesi = ilce.Id;
                            yeniTaahhut.Ili = ilce.IlId;
                        }
                        yeniTaahhut.Olusturan = UtilityHelper.GetCurrentUserLoginName();
                        yeniTaahhut.Sag_vefat = tasinmazBagisci.Sag_vefat;
                        yeniTaahhut.Soyadi = tasinmazBagisci.Soyadi;
                        yeniTaahhut.TaahhutAciklama = "Bağışçıya taahhüt verilmiştir.";
                        yeniTaahhut.TasinmazId = TasinmazDDL.SelectedItem.Value.ConvertToInt();
                        yeniTaahhut.TCKimlikNo = tasinmazBagisci.TCKimlikNo;
                        yeniTaahhut.Telefon = tasinmazBagisci.Telefon1;
                        if (tasinmazBagisci.VefatTarihi != null)
                        {
                            yeniTaahhut.VefatTarihi = tasinmazBagisci.VefatTarihi;
                        }
                        if (yeniTaahhut.Save() > 0)
                        {
                            RedirectToPage(ProjeConstants.PAGE_TASINMAZBAGISCI_TAAHHUTLERI + "?BagisciId=" + BagisciIdQS);
                        }
                    }
                    else
                    {
                        MessageHelper.PublishMessage("Bağışçı zaten listede var.",ProjeConstants.MESAJ_BILGI, 2000);
                    }
                }
                else
                {
                    MessageHelper.PublishMessage("Bağışçı bulunamadı.", ProjeConstants.MESAJ_BILGI, 2000);
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper(ex);
                exceptionHelper.PublishException();
            }

            
        }
    }
}
