using Model.IKYS;
using Model.Ortak;
using Model.Services.IKYS;
using DAO.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;
using Utility.Logging;

namespace IKYS_WebParts.TopluIzinGirisiWP
{
    [ToolboxItemAttribute(false)]
    public partial class TopluIzinGirisiWP : WebPart
    {
        private PortalLogContext LoggingContext { get { return PortalLogContext.FromCurrentRequest("IKYS", "TopluIzinGirisiWP"); } }
        private void ReportError(string operation, Exception error)
        {
            PortalLogContext context = LoggingContext;
            if (error is System.Threading.ThreadAbortException) return;
            PortalTechnicalLog.WriteError(context, operation, error);
            Messages.Error("İşlem tamamlanamadı. Lütfen tekrar deneyin. Başvuru kodu: " + context.OperationId.ToString("N"));
        }
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public TopluIzinGirisiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
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
        private List<int> SecilenPersonelList //=new List<Personel>();
        {
            get
            {
                if (ViewState["SecilenPersonelList"] == null)
                {
                    Personel personel = new Personel();
                    ViewState["SecilenPersonelList"] = new List<int>().ToList();//personel.SelectCalisanPersonel().ToList();

                }
                return (List<int>)ViewState["SecilenPersonelList"];
            }
            set
            {
                ViewState["SecilenPersonelList"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    new IzinHareketService().TryRecordWebPartOpened(LoggingContext);
                    FillIzinTanim();
                    FillIzinBasSaat();
                    FillIzinBitSaat();
                    FillPersonelDDL();
                    SetLayoutByIzinTipi();
                    UyariLbl1.Text = " * Tüm personele izin girmek için, hariç tutmak istediginiz personeli seçerek listeye ekleyiniz ve 'Tüm Personele İzin Gir' seçenegini isaretleyiniz.";
                    UyariLbl2.Text = " * Sadece bir kisim personele izin girecekseniz, izin girmek istediginiz personeli seçerek listeye ekleyiniz ve asagidan 'Sadece Seçilen Personele İzin Gir' seçenegini isaretleyiniz.";
                    UyariLbl3.Text = " * Yapilan islemler geriye alinamayacagindan lütfen kaydetmeden önce dikkatle kontrol ediniz.";
                }
                FillHaricTutulanTable();//CikarBtn event çalismasi için pagekload'da postback sisinda bulunmali
            }
            catch (Exception exception)
            {
                ReportError("Unhandled", exception);
            }
        }
        private void FillPersonelDDL()
        {
            PersonelDDL.Items.Clear();
            Personel personel = new Personel();
            List<Personel> list = new PersonelService().GetActiveEmployees(Personel.PersonelTipi.Kadrolu, LoggingContext);

            foreach (Personel item in list)
            {
                ListItem li = new ListItem(item.Adi.ReturnEmptyIfNull().ToString() + " " + item.Soyadi.ReturnEmptyIfNull().ToString(), item.Id.ReturnZeroIfNull().ToString());
                PersonelDDL.Items.Add(li);
            }
        }
        private void FillIzinTanim()
        {
            IzinTanimDDL.Items.Clear();
            ListItem li = new ListItem("Ücretli", "1");
            IzinTanimDDL.Items.Add(li);
            ListItem li1 = new ListItem("Mazeret", "2");
            IzinTanimDDL.Items.Add(li1);

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
        private void SetLayoutByIzinTipi()
        {
            //izin tipi Mazeret ise
            if (IzinTanimDDL.SelectedItem.Value.ConvertToInt() == (ProjeConstants.IZINTIPI_MAZERET_INT))
            {
                IzinBitTarDiv.Attributes["style"] = "display:none";
                IzinBasSaatDiv.Attributes["style"] = "display:block";
                IzinBitSaatDiv.Attributes["style"] = "display:block";
                IzinHareketDiv.Attributes["Class"] = "card-body alert-info";
                AciklamaLbl.Text = "Mazeret";
            }
            else if (IzinTanimDDL.SelectedItem.Value.ConvertToInt() == (ProjeConstants.IZINTIPI_UCRETLI_INT))
            {

                IzinBitTarDiv.Attributes["style"] = "display:block";
                IzinBasSaatDiv.Attributes["style"] = "display:none";
                IzinBitSaatDiv.Attributes["style"] = "display:none";
                HaricTutulanDiv.Attributes["style"] = "display:block";
                AciklamaLbl.Text = "Açiklama";
                IzinHareketDiv.Attributes["Class"] = "card-body alert-secondary";
            }
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
        /// <summary>
        /// Yeni Olusturulan Izin hareketini kisi bazinda kaydeder.
        /// Girilen Izin baslangiç tarihine bakarak izin dönemi baslangicini bulur,
        /// Buldugu döneme ait IzinDonem_Table'da kayit yoksa, ekler
        /// </summary>
        /// <returns></returns>
        private void YeniIzinHareketiKaydet(Personel personel, int izinTipi, SqlTransactionContext transaction, PortalLogContext context)
        {
            IzinHareket hareket = new IzinHareket();
            hareket.PersonelId = personel.Id;
            hareket.BaslangicTarihi = IzinBasTarTxt.Value.ConvertToDatetime();
            hareket.BitisTarihi = IzinBitTarTxt.Value.ConvertToDatetime();
            hareket.Aciklama = AciklamaTxt.Text;
            hareket.IzinTipi = izinTipi;
            hareket.Birim = ProjeConstants.IZIN_BIRIMI_GUN;
            if (izinTipi == ProjeConstants.IZINTIPI_MAZERET_INT)
            {
                hareket.BitisTarihi = hareket.BaslangicTarihi;
                hareket.BaslangicTarihi = UtilityHelper.TariheSaatEkle(hareket.BaslangicTarihi, IzinBasSaatDDL.SelectedItem.Text);
                hareket.BitisTarihi = UtilityHelper.TariheSaatEkle(hareket.BitisTarihi, IzinBitSaatDDL.SelectedItem.Text);
                hareket.Birim = ProjeConstants.IZIN_BIRIMI_SAAT;
            }
            if (hareket.BitisTarihi < hareket.BaslangicTarihi) throw new InvalidOperationException("İzin bitişi başlangıçtan önce olamaz.");
            hareket.Sure = IKYSOrtak.IzinSuresiHesapla(izinTipi, hareket.BaslangicTarihi, hareket.BitisTarihi, transaction, context);
            if (izinTipi == ProjeConstants.IZINTIPI_MAZERET_INT || izinTipi == ProjeConstants.IZINTIPI_UCRETLI_INT)
            {
                IzinDonemService service = new IzinDonemService();
                IzinDonem donem = service.GetByIzinTarihi(personel.Id, izinTipi, hareket.BaslangicTarihi, transaction, context);
                donem = donem == null
                    ? service.CreateForPersonel(personel, izinTipi, hareket.BaslangicTarihi, CurrentUserName, transaction, context)
                    : service.UpdateForPersonel(personel, izinTipi, hareket.BaslangicTarihi, CurrentUserName, transaction, context);
                if (donem == null) throw new InvalidOperationException("Personelin izin dönemi hazırlanamadı.");
                if (!service.UpdateUsedLeave(donem, hareket.Sure, true, CurrentUserName, transaction, context))
                    throw new InvalidOperationException("İzin bakiyesi güncellenemedi.");
                hareket.IzinDonemId = donem.Id;
            }
            hareket.Olusturan = CurrentUserName;
            if (new IzinHareketService().Save(hareket, transaction, context) <= 0)
                throw new InvalidOperationException("Toplu izin hareketi oluşturulamadı.");
        }

        protected void IzinTanimDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetLayoutByIzinTipi();
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        protected void SaveBtn_Click(object sender, EventArgs e)
        {
            try
            {
                int izinTipi = IzinTanimDDL.SelectedItem == null ? 0 : IzinTanimDDL.SelectedItem.Value.ConvertToInt();
                if (izinTipi == ProjeConstants.IZINTIPI_MAZERET_INT)
                {
                    IzinBitTarTxt.Value = IzinBasTarTxt.Value;
                }
                bool isValidated = !string.IsNullOrEmpty(IzinBasTarTxt.Value.ConvertToDatetimeEmptyIfNull()) &&
                  !string.IsNullOrEmpty(IzinBitTarTxt.Value.ConvertToDatetimeEmptyIfNull());
                if (isValidated)
                {
                    if (UygulanacakGrupRL.SelectedValue.Equals("Tum"))
                    {
                        UyariMesajiLbl.Text = "Tüm personele toplu izin girisi yapilacak (Seçilen personel hariç). Lütfen degisikligi kaydetmeden önce dikkatle kontrol ediniz.";
                        SaveNowBtn.Text = "Tüm personele toplu izin gir (Seçilen personel hariç)";
                        SaveNowBtn.CssClass = "btn btn-outline-danger";
                    }
                    else
                    {
                        UyariMesajiLbl.Text = "Seçilen personele izin girisi yapilacak. Lütfen degisikligi kaydetmeden önce dikkatle kontrol ediniz.";
                        SaveNowBtn.Text = "Sadece seçilen personele izin gir ";
                        SaveNowBtn.CssClass = "btn btn-outline-primary";
                    }

                    var openPopup = "OpenModal();";
                    System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), openPopup, true);
                }
                else
                {
                    Messages.Info("Lütfen izin baslangiç ve bitis tarihi seçiniz");
                }

            }
            catch (Exception exception)
            {
                ReportError("Unhandled", exception);
            }
        }
        /// <summary>
        ///
        /// izinbastartxt dolu mu kontrol et
        /// izinbittartxt dolu mu kontrol et
        /// calisan personel listesini al
        /// izintipini al
        /// loop
        /// haric tutulan listede ise continue;
        /// izinDonemi var mi kontrol et yoksa yeni izin donemi olustur.
        /// kalanizin ve kullanilan izin hanelerini güncelle
        /// yeni izinHareket nesnesi yarat
        /// izinHareket bilgilerini toplu izne göre assign et
        /// izinhareket.save
        /// end loop
        /// </summary>
        /// <returns>bool</returns>
        private bool TopluIzinGir()
        {
            PortalLogContext context = LoggingContext;
            bool tumu = UygulanacakGrupRL.SelectedValue.Equals("Tum");
            bool secilen = UygulanacakGrupRL.SelectedValue.Equals("Secilen");
            if (!tumu && !secilen) throw new InvalidOperationException("İzin uygulanacak grup seçilmelidir.");
            int izinTipi = IzinTanimDDL.SelectedItem == null ? 0 : IzinTanimDDL.SelectedItem.Value.ConvertToInt();
            if (izinTipi <= 0) throw new InvalidOperationException("İzin türü seçilmelidir.");
            DateTime baslangic;
            DateTime bitis;
            if (!DateTime.TryParse(IzinBasTarTxt.Value, System.Globalization.CultureInfo.GetCultureInfo("tr-TR"),
                    System.Globalization.DateTimeStyles.None, out baslangic)
                || !DateTime.TryParse(IzinBitTarTxt.Value, System.Globalization.CultureInfo.GetCultureInfo("tr-TR"),
                    System.Globalization.DateTimeStyles.None, out bitis))
                throw new InvalidOperationException("Geçerli izin başlangıç ve bitiş tarihleri girilmelidir.");
            List<Personel> personeller = new PersonelService().GetActiveEmployees(Personel.PersonelTipi.Kadrolu, context);
            int tamamlanan = 0;
            List<string> basarisizlar = new List<string>();
            foreach (Personel personel in personeller)
            {
                bool listede = SecilenPersonelList.Contains(personel.Id);
                if ((tumu && listede) || (secilen && !listede)) continue;
                try
                {
                    using (SqlTransactionContext transaction = new SqlTransactionContext())
                    {
                        Personel guncel = new PersonelService().GetById(personel.Id, transaction, context);
                        if (guncel == null) throw new InvalidOperationException("Personel bulunamadı.");
                        YeniIzinHareketiKaydet(guncel, izinTipi, transaction, context);
                        transaction.Complete();
                    }
                    tamamlanan++;
                }
                catch (System.Threading.ThreadAbortException) { throw; }
                catch (Exception error)
                {
                    PortalTechnicalLog.WriteError(context, "TopluIzin.Personel." + personel.Id, error);
                    basarisizlar.Add(System.Web.HttpUtility.HtmlEncode(personel.Adi + " " + personel.Soyadi) + " (" + personel.Id + ")");
                }
            }
            if (basarisizlar.Count > 0)
            {
                Messages.Error(tamamlanan + " personelin kaydı tamamlandı; " + basarisizlar.Count
                    + " personelin kaydı tamamlanamadı. Tamamlanan kayıtlar korunmuştur; yeniden yalnız başarısız personelleri seçiniz. Başarısız personeller: "
                    + string.Join(", ", basarisizlar) + ". Başvuru kodu: " + context.OperationId.ToString("N"));
                return false;
            }
            if (tamamlanan == 0)
            {
                Messages.Info("İzin uygulanacak personel bulunamadı.");
                return false;
            }
            return true;
        }
        protected void IzinBasSaatDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillIzinBitSaat();
        }
        protected void PersonelDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            Personel personel = new Personel();
            personel = new PersonelService().GetById(PersonelDDL.SelectedItem.Value.ConvertToInt(), LoggingContext);
            if (personel != null)
            {
                if (!SecilenPersonelList.Contains(personel.Id))
                {
                    SecilenPersonelList.Add(personel.Id);
                    FillHaricTutulanTable();
                }

            }
            else
            {
                Messages.Error("Personel Bulunamadi!");
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_PERSONEL_LIST;
                Page.Response.Redirect(newUrl);
            }

        }
        private void FillHaricTutulanTable()
        {
            SecilenPersonelTable.Rows.Clear();
            int SiraNo = 0;
            foreach (int item in SecilenPersonelList)
            {
                Personel personel = new Personel();
                personel = new PersonelService().GetById(item, LoggingContext);

                if (personel != null)
                {
                    TableRow row = new TableRow();
                    TableCell adSoyadCell = new TableCell();
                    adSoyadCell.Text = personel.Adi + " " + personel.Soyadi;
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
                        SecilenPersonelList.Remove(personel.Id);
                        FillHaricTutulanTable();
                    };
                    SilCell.Controls.Add(SilBtn);
                    row.Controls.Add(SilCell);

                    SecilenPersonelTable.Controls.Add(row);

                }

            }


        }
        protected void SaveNowBtn_Click(object sender, EventArgs e)
        {
            try
            {
                int izinTipi = IzinTanimDDL.SelectedItem == null ? 0 : IzinTanimDDL.SelectedItem.Value.ConvertToInt();
                if (izinTipi == ProjeConstants.IZINTIPI_MAZERET_INT)
                {
                    IzinBitTarTxt.Value = IzinBasTarTxt.Value;
                }
                bool isValidated = !string.IsNullOrEmpty(IzinBasTarTxt.Value.ConvertToDatetimeEmptyIfNull()) &&
                  !string.IsNullOrEmpty(IzinBitTarTxt.Value.ConvertToDatetimeEmptyIfNull());
                bool isSaved = false;
                if (isValidated)
                {
                    isSaved = TopluIzinGir();

                    if (isSaved)
                    {

                        UyariLbl.Text = IzinBasTarTxt.Value.ConvertToDatetime() + " ile " + IzinBitTarTxt.Value.ConvertToDatetime() + " tarihleri arasinda izin girisi yapilmistir. ";
                        IzinBasTarTxt.Value = "";
                        IzinBitTarTxt.Value = "";
                        AciklamaLbl.Text = "";
                        SecilenPersonelList.Clear();
                        FillHaricTutulanTable();

                        //MailGonder(personel);
                        Messages.Success("Toplu izin girisi tamamlandi", 2000);
                    }
                    else
                    {
                        Messages.Error("Toplu izin kaydedilemedi");
                    }
                }
                else
                {
                    Messages.Info("Lütfen izin baslangiç ve bitis tarihi seçiniz");
                }

            }
            catch (Exception exception)
            {
                ReportError("Unhandled", exception);
            }
        }
    }
}
