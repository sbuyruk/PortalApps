using Model.IKYS;
using Model.Ortak;
using Model.Services.IKYS;
using DAO.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;
using Utility.Logging;

namespace IKYS_WebParts.IzinHareketEditWP
{
    [ToolboxItemAttribute(false)]
    public partial class IzinHareketEditWP : WebPart
    {
        private PortalLogContext LoggingContext { get { return PortalLogContext.FromCurrentRequest("IKYS", "IzinHareketEditWP"); } }
        private void ReportError(string operation, Exception error)
        {
            if (error == null || error is System.Threading.ThreadAbortException) return;
            PortalLogContext context = LoggingContext;
            PortalTechnicalLog.WriteError(context, operation, error);
            MessageHelper.PublishMessage("İşlem tamamlanamadı. Lütfen tekrar deneyin. Başvuru kodu: " + context.OperationId.ToString("N"), ProjeConstants.MESAJ_HATA);
        }
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public IzinHareketEditWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
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
        private string IzinHareketIdQS
        {
            get
            {

                if (ViewState["IzinHareketId"] == null)
                {
                    if (Page.Request.QueryString["IzinHareketId"] != null)
                    {
                        ViewState["IzinHareketId"] = Page.Request.QueryString["IzinHareketId"];
                    }
                    else
                    {
                        ViewState["IzinHareketId"] = string.Empty;
                    }
                }
                return ViewState["IzinHareketId"].ToString();
            }

            set
            {
                ViewState["IzinHareketId"] = value;
            }
        }
        private string IzinTanimIdQS
        {
            get
            {

                if (ViewState["IzinTanimId"] == null)
                {
                    if (Page.Request.QueryString["IzinTanimId"] != null)
                    {
                        ViewState["IzinTanimId"] = Page.Request.QueryString["IzinTanimId"];
                    }
                    else
                    {
                        ViewState["IzinTanimId"] = string.Empty;
                    }
                }
                return ViewState["IzinTanimId"].ToString();
            }

            set
            {
                ViewState["IzinTanimId"] = value;
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
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Personel personel = PersonelGetir();
                if (!Page.IsPostBack)
                {
                        new IzinHareketService().TryRecordWebPartOpened(LoggingContext);
                    if (personel != null)
                    {
                        PersonelAdiLbl.Text = personel.Adi + " " + personel.Soyadi;
                        FillVekilImzaDDL();
                        FillAmirImzaDDL();
                        FillOnayImzaDDL();
                        FillIzinBasSaat();
                        FillIzinBitSaat();
                        FillIzinHareketForm();
                        GorunumuAyarla();
                    }
                
                
                }
                int gunFarki = (DateTime.Today - IzinBitTarTxt.Value.ConvertToDatetime()).Days;
                if (gunFarki > ProjeConstants.IZINDUZENLEMESURESI_GUN)
                {
                    UpdateBtn.Visible = false;
                    DeleteBtn.Visible = false;
                    IzinBasTarTxt.Disabled = true;
                    IzinBitTarTxt.Disabled = true;
                    IzinBasSaatDDL.Enabled = false;
                    IzinBitSaatDDL.Enabled = false;
                    AdresTxt.Enabled = false;
                    AciklamaTxt.Enabled = false;
                    AmirImzaDDL.Enabled = false;
                    VekilImzaDDL.Enabled = false;
                    MessageHelper.PublishMessage("İzin düzenleme süresi geçtiği için değişiklik yapılamaz.", ProjeConstants.MESAJ_BILGI,3000);
                }
            }
            catch (System.Threading.ThreadAbortException) { throw; }
            catch (Exception error) { ReportError("Page_Load", error); }
        }
        private void GorunumuAyarla()
        {
            if (AuthQS.Equals("IKYS"))
            {
                IzinHareketListesiBtn.Visible = true;
            }
            else
            {
                IzinHareketListesiBtn.Visible = false;
            }
            if (DestinationAppQS.Equals("DIL"))
            {
                UpdateBtn.Visible = false;
                DeleteBtn.Visible = false;
                IzinIptalDilekceBtn.Visible = true;
                IzinDegisDilekceBtn.Visible = true;
            }
            else
            {
                UpdateBtn.Visible = true;
                DeleteBtn.Visible = true;
                IzinIptalDilekceBtn.Visible = false;
                IzinDegisDilekceBtn.Visible = false;
            }
        }
        private void FillIzinHareketForm()
        {
            IzinHareket izinHareket = new IzinHareketService().GetById(IzinHareketIdQS.ConvertToInt(), LoggingContext);
            if (izinHareket != null)
            {
                IzinHareketIdLbl.Text = izinHareket.Id.ReturnEmptyIfNull().ToString();
                IzinTanim it = new IzinTanimService().GetById(izinHareket.IzinTipi);
                IzinTipiIdLbl.Text = it.Id.ToString();
                IzinTipiLbl.Text = it.Adi;
                SelectDDLValue(VekilImzaDDL, izinHareket.VekilImza.ToString());
                SelectDDLValue(AmirImzaDDL, izinHareket.AmirImza.ToString());
                SelectDDLValue(OnayImzaDDL, izinHareket.OnayImza.ToString());
                string bassaat = izinHareket.BaslangicTarihi.ToString("HH:mm");
                SelectDDLByText(IzinBasSaatDDL, bassaat);
                string bitsaat = izinHareket.BitisTarihi.ToString("HH:mm");
                SelectDDLByText(IzinBitSaatDDL, bitsaat);

                IzinBasTarTxt.Value = izinHareket.BaslangicTarihi.ConvertToDatetimeEmptyIfNull();
                IzinBitTarTxt.Value = izinHareket.BitisTarihi.ConvertToDatetimeEmptyIfNull();
                AdresTxt.Text = izinHareket.Adres.ReturnEmptyIfNull().ToString();
                AciklamaTxt.Text = izinHareket.Aciklama.ReturnEmptyIfNull().ToString();
                SetLayoutByIzinTipi();
            }
            else
            {
                MessageHelper.PublishMessage("İzin kaydı bulunamadı", ProjeConstants.MESAJ_BILGI);
            }


        }
        private void SetLayoutByIzinTipi()
        {
            AmirDiv.Attributes["style"] = "display:inline";
            VekilDiv.Attributes["style"] = "display:inline";
            OnayDiv.Attributes["style"] = "display:inline";
            IzinBitTarDiv.Attributes["style"] = "display:inline";
            IzinBasSaatDiv.Attributes["style"] = "display:none";
            IzinBitSaatDiv.Attributes["style"] = "display:none";
            IzinTalepDiv.Attributes["Class"] = "card-body alert-danger";
            //izin tipi Mazeret ise
            if (IzinTanimIdQS.ConvertToInt() == (ProjeConstants.IZINTIPI_MAZERET_INT))
            {
                VekilDiv.Attributes["style"] = "display:none";
                OnayDiv.Attributes["style"] = "display:none";
                IzinBitTarDiv.Attributes["style"] = "display:none";
                IzinBasSaatDiv.Attributes["style"] = "display:inline";
                IzinBitSaatDiv.Attributes["style"] = "display:inline";
                IzinTalepDiv.Attributes["Class"] = "card-body alert-warning";
            }
            else if (IzinTanimIdQS.ConvertToInt() == (ProjeConstants.IZINTIPI_UCRETLI_INT))
            {
                IzinTalepDiv.Attributes["Class"] = "card-body alert-secondary";
            }else if (IzinTanimIdQS.ConvertToInt() == (ProjeConstants.IZINTIPI_SUTIZNI_INT))
            {
                VekilDiv.Attributes["style"] = "display:none";
                AmirDiv.Attributes["style"] = "display:none";
                OnayDiv.Attributes["style"] = "display:none";
                IzinBitTarDiv.Attributes["style"] = "display:block";
                IzinBasSaatDiv.Attributes["style"] = "display:inline";
                IzinBitSaatDiv.Attributes["style"] = "display:none";
                IzinTalepDiv.Attributes["Class"] = "card-body alert-info";
            }
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
        private void FillVekilImzaDDL()
        {
            VekilImzaDDL.Items.Clear();
            List<Personel> list = new PersonelService().GetActiveEmployees(Personel.PersonelTipi.Kadrolu);
            ListItem bosLi = new ListItem("", "0");
            VekilImzaDDL.Items.Add(bosLi);
            foreach (Personel item in list)
            {
                ListItem li = new ListItem(item.Adi.ReturnEmptyIfNull().ToString() + " " + item.Soyadi.ReturnEmptyIfNull().ToString(), item.Id.ReturnZeroIfNull().ToString());
                VekilImzaDDL.Items.Add(li);
            }
        }
        private void FillAmirImzaDDL()
        {
            AmirImzaDDL.Items.Clear();
            List<Personel> list = new PersonelService().GetActiveEmployees(Personel.PersonelTipi.Kadrolu);
            ListItem bosLi = new ListItem("", "0");
            AmirImzaDDL.Items.Add(bosLi);
            foreach (Personel item in list)
            {
                ListItem li = new ListItem(item.Adi.ReturnEmptyIfNull().ToString() + " " + item.Soyadi.ReturnEmptyIfNull().ToString(), item.Id.ReturnZeroIfNull().ToString());
                AmirImzaDDL.Items.Add(li);
            }
        }
        private void FillOnayImzaDDL()
        {
            OnayImzaDDL.Items.Clear();
            List<Personel> list = new PersonelService().GetActiveEmployees(Personel.PersonelTipi.Kadrolu);
            ListItem bosLi = new ListItem("", "0");
            OnayImzaDDL.Items.Add(bosLi);
            foreach (Personel item in list)
            {
                ListItem li = new ListItem(item.Adi.ReturnEmptyIfNull().ToString() + " " + item.Soyadi.ReturnEmptyIfNull().ToString(), item.Id.ReturnZeroIfNull().ToString());
                OnayImzaDDL.Items.Add(li);
            }

        }
        private void FillIzinBasSaat()
        {
            IzinBasSaatDDL.Items.Clear();
            TimeSpan bastarTS = new TimeSpan(7, 0, 0);
            TimeSpan aralikTS = new TimeSpan(0, 5, 0);
            TimeSpan bittarTS = new TimeSpan(17, 0, 0);

            TimeSpan oglenArasiBastarTS = new TimeSpan(12, 0, 0);
            TimeSpan oglenArasiBittarTS = new TimeSpan(13, 0, 0);

            TimeSpan nextTS = bastarTS;

            while (nextTS < bittarTS)
            {

                if ((nextTS >= oglenArasiBastarTS) && (nextTS < oglenArasiBittarTS))
                {
                    nextTS = nextTS + aralikTS;
                    continue;
                }
                else
                {
                    string bastarStr = string.Format("{0:00}:{1:00}", nextTS.Hours, nextTS.Minutes);
                    ListItem li = new ListItem(bastarStr);
                    IzinBasSaatDDL.Items.Add(li);
                    nextTS = nextTS + aralikTS;
                }
            }
        }
        private void FillIzinBitSaat()
        {
            IzinBitSaatDDL.Items.Clear();
            string bassaatStr = IzinBasSaatDDL.SelectedItem == null ? "07:05" : IzinBasSaatDDL.SelectedItem.Text;
            TimeSpan bastarTS = bassaatStr.ConvertToTimeSpan();
            TimeSpan aralikTS = new TimeSpan(0, 5, 0);
            TimeSpan bittarTS = new TimeSpan(17, 0, 0);

            TimeSpan oglenArasiBastarTS = new TimeSpan(12, 0, 0);
            TimeSpan oglenArasiBittarTS = new TimeSpan(13, 0, 0);

            TimeSpan nextTS = bastarTS;

            do
            {
                nextTS = nextTS + aralikTS;
                if ((nextTS > oglenArasiBastarTS) && (nextTS <= oglenArasiBittarTS))
                {
                    continue;
                }
                else
                {
                    string bastarStr = string.Format("{0:00}:{1:00}", nextTS.Hours, nextTS.Minutes);
                    ListItem li = new ListItem(bastarStr);
                    IzinBitSaatDDL.Items.Add(li);
                }

            } while (nextTS < bittarTS);
        }
        private bool IzinHareketGuncelle(int izinHareketId)
        {
            PortalLogContext context = LoggingContext;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                IzinHareketService hareketService = new IzinHareketService();
                IzinHareket izinHareket = hareketService.GetById(izinHareketId, transaction, context);
                if (izinHareket == null) return false;
                Personel personel = new PersonelService().GetById(izinHareket.PersonelId, transaction, context);
                if (personel == null) throw new InvalidOperationException("İzin kaydına ait personel bulunamadı.");

                string izinSuresiOnceki = izinHareket.Sure;
                izinHareket.BaslangicTarihi = IzinBasTarTxt.Value.ConvertToDatetime();
                izinHareket.Adres = AdresTxt.Text;
                if (izinHareket.IzinTipi != ProjeConstants.IZINTIPI_MAZERET_INT)
                {
                    izinHareket.BitisTarihi = IzinBitTarTxt.Value.ConvertToDatetime() + new TimeSpan(17, 0, 0);
                    izinHareket.Birim = ProjeConstants.IZIN_BIRIMI_GUN;
                    izinHareket.VekilImza = VekilImzaDDL.SelectedItem.Value.ConvertToInt();
                    izinHareket.AmirImza = AmirImzaDDL.SelectedItem.Value.ConvertToInt();
                    izinHareket.OnayImza = OnayImzaDDL.SelectedItem.Value.ConvertToInt();
                }
                else
                {
                    DateTime tarih = izinHareket.BaslangicTarihi;
                    izinHareket.BaslangicTarihi = UtilityHelper.TariheSaatEkle(tarih, IzinBasSaatDDL.SelectedItem.Text);
                    izinHareket.BitisTarihi = UtilityHelper.TariheSaatEkle(tarih, IzinBitSaatDDL.SelectedItem.Text);
                    izinHareket.Birim = ProjeConstants.IZIN_BIRIMI_SAAT;
                    izinHareket.AmirImza = AmirImzaDDL.SelectedItem.Value.ConvertToInt();
                }
                string yeniSure = IKYSOrtak.IzinSuresiHesapla(izinHareket.IzinTipi,
                    izinHareket.BaslangicTarihi, izinHareket.BitisTarihi, transaction, context);
                izinHareket.Sure = yeniSure;

                if (izinHareket.IzinTipi == ProjeConstants.IZINTIPI_MAZERET_INT
                    || izinHareket.IzinTipi == ProjeConstants.IZINTIPI_UCRETLI_INT)
                {
                    IzinDonemService donemService = new IzinDonemService();
                    IzinDonem oncekiDonem = donemService.GetById(izinHareket.IzinDonemId, transaction, context);
                    if (oncekiDonem == null) throw new InvalidOperationException("İzin kaydının mevcut dönemi bulunamadı.");
                    IzinDonem yeniDonem = donemService.GetByIzinTarihi(personel.Id, izinHareket.IzinTipi,
                        izinHareket.BaslangicTarihi, transaction, context);
                    yeniDonem = yeniDonem == null
                        ? donemService.CreateForPersonel(personel, izinHareket.IzinTipi, izinHareket.BaslangicTarihi, CurrentUserName, transaction, context)
                        : donemService.UpdateForPersonel(personel, izinHareket.IzinTipi, izinHareket.BaslangicTarihi, CurrentUserName, transaction, context);
                    if (yeniDonem == null) throw new InvalidOperationException("İzin kaydının yeni dönemi hazırlanamadı.");
                    IzinDonem dusulecekDonem = oncekiDonem.Id == yeniDonem.Id ? yeniDonem : oncekiDonem;
                    if (!donemService.UpdateUsedLeave(dusulecekDonem, izinSuresiOnceki, false, CurrentUserName, transaction, context))
                        return false;
                    if (!donemService.UpdateUsedLeave(yeniDonem, yeniSure, true, CurrentUserName, transaction, context))
                        return false;

                    izinHareket.IzinDonemId = yeniDonem.Id;
                    bool ozetVar = !string.IsNullOrEmpty(izinHareket.OncekiIzinStr)
                        && !string.IsNullOrEmpty(izinHareket.KullanilanIzinStr)
                        && !string.IsNullOrEmpty(izinHareket.KalanIzinStr);
                    if (ozetVar)
                    {
                        izinHareket.OncekiIzinStr = OncekiIzniHesapla(yeniDonem, yeniSure);
                        izinHareket.KullanilanIzinStr = yeniSure;
                        izinHareket.KalanIzinStr = yeniDonem.KalanIzin;
                    }
                }
                izinHareket.Degistiren = CurrentUserName;
                if (!hareketService.Update(izinHareket, transaction, context)) return false;
                if (izinHareket.IzinTalepId > 0 && !IzinTalebiniGüncelle(izinHareket.IzinTalepId, izinHareket, transaction))
                    return false;
                transaction.Complete();
                return true;
            }
        }
        public string OncekiIzniHesapla(IzinDonem izinDonemi, string sure)
        {
            string hesaplananIzin = string.Empty;


            if (izinDonemi.IzinTipi == ProjeConstants.IZINTIPI_UCRETLI_INT) //yalnizca izin tipi ÜCRETLI izinse  kalan izin süresini hesapla
            {
                if (izinDonemi != null)
                {
                    int kullanilanIzin = izinDonemi.KullanilanIzin.ConvertToInt();
                    int sonuc = kullanilanIzin + sure.ConvertToInt();
                    hesaplananIzin = sonuc.ToString();
                }
            }
            if (izinDonemi.IzinTipi == ProjeConstants.IZINTIPI_MAZERET_INT)
            {
                if (izinDonemi != null)
                {
                    TimeSpan kullanilanIzin = izinDonemi.KullanilanIzin.ConvertToTimeSpan();
                    _ = new TimeSpan(0, 0, 0);
                    TimeSpan sonuc = kullanilanIzin + sure.ConvertToTimeSpan();
                    hesaplananIzin = sonuc.ToString();
                }
            }
            return hesaplananIzin;
        }
        private bool IzinHareketSil(int izinHareketId)
        {
            PortalLogContext context = LoggingContext;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                IzinHareketService hareketService = new IzinHareketService();
                IzinHareket izinHareket = hareketService.GetById(izinHareketId, transaction, context);
                if (izinHareket == null) return false;
                if (izinHareket.IzinTipi == ProjeConstants.IZINTIPI_MAZERET_INT
                    || izinHareket.IzinTipi == ProjeConstants.IZINTIPI_UCRETLI_INT)
                {
                    IzinDonemService donemService = new IzinDonemService();
                    IzinDonem donem = donemService.GetById(izinHareket.IzinDonemId, transaction, context);
                    if (donem == null) throw new InvalidOperationException("İzin kaydının mevcut dönemi bulunamadı.");
                    if (!donemService.UpdateUsedLeave(donem, izinHareket.Sure, false, CurrentUserName, transaction, context))
                        return false;
                }
                if (!hareketService.Delete(izinHareket, transaction, context)) return false;
                if (izinHareket.IzinTalepId > 0)
                {
                    IzinTalepService talepService = new IzinTalepService();
                    IzinTalep talep = talepService.GetById(izinHareket.IzinTalepId, transaction, context);
                    if (talep != null && !talepService.Delete(talep, transaction, context)) return false;
                }
                transaction.Complete();
                return true;
            }
        }
        private Personel PersonelGetir()
        {
            Personel personel = new Personel();

            if (!string.IsNullOrEmpty(PersonelIdQS))
            {
                personel = new PersonelService().GetById(PersonelIdQS.ConvertToInt(), LoggingContext);

            }
            else
            {
                string userName = CurrentUserName.Substring(CurrentUserName.LastIndexOf("\\") + 1, CurrentUserName.Length - CurrentUserName.LastIndexOf("\\") - 1);
                personel = new PersonelService().GetByUserName(userName, LoggingContext);
            }
            if (personel != null) PersonelIdQS = personel.Id.ToString();
            return personel;
        }
        protected void IzinBasSaatDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillIzinBitSaat();
        }
        protected void UpdateBtn_Click(object sender, EventArgs e)
        {
            try
            {
                bool updated = IzinHareketGuncelle(IzinHareketIdQS.ConvertToInt());
                MessageHelper.PublishMessage(updated ? "İzin kaydı güncellendi."
                    : "İzin kaydı güncellenemedi. Değişiklikler kaydedilmedi.",
                    updated ? ProjeConstants.MESAJ_BASARILI : ProjeConstants.MESAJ_HATA,
                    updated ? 2000 : 0);
            }
            catch (System.Threading.ThreadAbortException) { throw; }
            catch (Exception error) { ReportError("IzinHareket.Update", error); }
        }
        private bool IzinTalebiniGüncelle(int izinTalepId, IzinHareket izinHareket, SqlTransactionContext transaction)
        {
            IzinTalepService izinTalepService = new IzinTalepService();
            IzinTalep izinTalep = izinTalepService.GetById(izinTalepId, transaction, LoggingContext);
            if (izinTalep == null) return true;
            izinTalep.Aciklama += System.Environment.NewLine + CurrentUserName + " tarafindan " + DateTime.Now.ReturnTRDateFormat() + " tarihinde izin kaydi güncellendi";
            izinTalep.Adres = izinHareket.Adres; izinTalep.AmirImza = izinHareket.AmirImza; izinTalep.BaslangicTarihi = izinHareket.BaslangicTarihi; izinTalep.BitisTarihi = izinHareket.BitisTarihi; izinTalep.Birim = izinHareket.Birim; izinTalep.IzinDonemId = izinHareket.IzinDonemId; izinTalep.IzinTipi = izinHareket.IzinTipi; izinTalep.OnayImza = izinHareket.OnayImza; izinTalep.Sure = izinHareket.Sure; izinTalep.VekilImza = izinHareket.VekilImza; izinTalep.Degistiren = CurrentUserName;
            return izinTalepService.Update(izinTalep, transaction, LoggingContext);
        }
        protected void IzinHareketListesiBtn_Click(object sender, EventArgs e)
        {
            if (AuthQS.Equals("IKYS"))
                RedirectToPage(ProjeConstants.PAGE_IZINHAREKET_LIST + "?SecilenId=" + IzinHareketIdQS);
        }
        protected void KisiselSayfaBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_KISISELSAYFA + "?PersonelId=" + PersonelIdQS);
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        protected void DeleteBtn_Click(object sender, EventArgs e)
        {
            try
            {
                //popup aç
                var openPopup = "OpenModalOnay();";
                System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), openPopup, true);

            }
            catch (Exception exception)
            {
                ReportError("Unhandled", exception);
            }
        }
        protected void DeleteNowBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (IzinHareketSil(IzinHareketIdQS.ConvertToInt()))
                {
                    MessageHelper.PublishMessage("İzin kaydı silindi.", ProjeConstants.MESAJ_BASARILI, 2000);
                    RedirectToPage(ProjeConstants.PAGE_IZINHAREKET_LIST);
                }
                else
                    MessageHelper.PublishMessage("İzin kaydı silinemedi. Değişiklikler kaydedilmedi.", ProjeConstants.MESAJ_HATA);
            }
            catch (System.Threading.ThreadAbortException) { throw; }
            catch (Exception error) { ReportError("IzinHareket.Delete", error); }
        }
        private void RedirectToPage(string pageUrl)
        {
            try
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + pageUrl;
                Page.Response.Redirect(newUrl);
            }
            catch (System.Threading.ThreadAbortException) { throw; }
            catch (Exception ex)
            {
                ReportError("Unhandled", ex);
            }
        }
        protected void IzinDegisDilekceBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string yeniIzinBasTar = IzinBasTarTxt.Value.ConvertToDatetimeEmptyIfNull();
                string yeniIzinBitTar = IzinBitTarTxt.Value.ConvertToDatetimeEmptyIfNull();
                IzinHareket izinHareket = new IzinHareketService().GetById(IzinHareketIdQS.ConvertToInt(), LoggingContext);
                if (izinHareket != null)
                {
                    var fullUrl = string.Format("{0}?IzinHareketId={1}&AmirId={2}&OnaylayanId={3}&YeniIzinBasTar={4}&YeniIzinBitTar={5}",
                               ProjeConstants.RAPOR_UCRETLIIZINDEGISIKLIKDILEKCE, IzinHareketIdQS, AmirImzaDDL.SelectedItem.Value, OnayImzaDDL.SelectedItem.Value, yeniIzinBasTar, yeniIzinBitTar);
                    RedirectToPage(fullUrl);
                }
            }
            catch (Exception exception)
            {
                ReportError("Unhandled", exception);
            }

        }
        protected void IzinIptalDilekceBtn_Click(object sender, EventArgs e)
        {

            try
            {
                IzinHareket izinHareket = new IzinHareketService().GetById(IzinHareketIdQS.ConvertToInt(), LoggingContext);
                if (izinHareket != null)
                {
                    var fullUrl = string.Format("{0}?IzinHareketId={1}&AmirId={2}&OnaylayanId={3}",
                               ProjeConstants.RAPOR_UCRETLIIZINIPTALDILEKCE, IzinHareketIdQS, AmirImzaDDL.SelectedItem.Value, OnayImzaDDL.SelectedItem.Value);
                    RedirectToPage(fullUrl);
                }



            }
            catch (Exception exception)
            {
                ReportError("Unhandled", exception);
            }



        }
    }
}
