using MTS_WebParts.Logging;
using Utility.Logging;
using Model.Services.MTS;
using Model.IKYS;
using Model.MTS;
using Model.NBYS;
using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace MTS_WebParts.FaaliyetGirisiWP
{
    [ToolboxItemAttribute(false)]
    public partial class FaaliyetGirisiWP : WebPart
    {
        private PortalLogContext logContext;
        private PortalLogContext LogContext { get { return logContext ?? (logContext = PortalLogContext.FromCurrentRequest("MTS", "FaaliyetGirisiWP")); } }
        private MtsActivityWriteService Writer { get { return new MtsActivityWriteService(LogContext); } }
        private string ActivityVersion { get { return ViewState["ActivityVersion"] as string; } }
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public FaaliyetGirisiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }

        #region Globals

        private string FaaliyetIdQS
        {
            get
            {

                if (ViewState["FaaliyetId"] == null)
                {
                    if (Page.Request.QueryString["FaaliyetId"] != null)
                    {
                        ViewState["FaaliyetId"] = Page.Request.QueryString["FaaliyetId"];
                    }
                    else
                    {
                        ViewState["FaaliyetId"] = string.Empty;
                    }
                }
                return ViewState["FaaliyetId"].ToString();
            }

            set
            {
                ViewState["FaaliyetId"] = value;
            }
        }
        private TempFaaliyet TempFaaliyetQS
        {
            get
            {
                return (TempFaaliyet)ViewState["TempFaaliyet"];
            }

            set
            {
                ViewState["TempFaaliyet"] = value;
            }
        }
        private List<string> KatilimciListQS //{ get; set; }//Postback disinda güncellenmeli
        {
            get
            {
                return (List<string>)ViewState["KatilimciList"];
            }

            set
            {
                ViewState["KatilimciList"] = value;
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
        private string AramaGorusmeIdQS
        {
            get
            {

                if (ViewState["AramaGorusmeId"] == null)
                {
                    if (Page.Request.QueryString["AramaGorusmeId"] != null)
                    {
                        ViewState["AramaGorusmeId"] = Page.Request.QueryString["AramaGorusmeId"];
                    }
                    else
                    {
                        ViewState["AramaGorusmeId"] = string.Empty;
                    }
                }
                return ViewState["AramaGorusmeId"].ToString();
            }

            set
            {
                ViewState["AramaGorusmeId"] = value;
            }
        }
        private string DisIrtibatIdQS
        {
            get
            {

                if (ViewState["DisIrtibatId"] == null)
                {
                    if (Page.Request.QueryString["DisIrtibatId"] != null)
                    {
                        ViewState["DisIrtibatId"] = Page.Request.QueryString["DisIrtibatId"];
                    }
                    else
                    {
                        ViewState["DisIrtibatId"] = string.Empty;
                    }
                }
                return ViewState["DisIrtibatId"].ToString();
            }

            set
            {
                ViewState["DisIrtibatId"] = value;
            }
        }
        private string InitialDateQS
        {
            get
            {

                if (ViewState["InitialDate"] == null)
                {
                    if (Page.Request.QueryString["InitialDate"] != null)
                    {
                        ViewState["InitialDate"] = Page.Request.QueryString["InitialDate"];
                    }
                    else
                    {
                        ViewState["InitialDate"] = string.Empty;
                    }
                }
                return ViewState["InitialDate"].ToString();
            }

            set
            {
                ViewState["InitialDate"] = value;
            }
        }
        #endregion

        #region PageLoad, DDL, Form vs
        protected void Page_Load(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "Page_Load", () =>
            {
                if (!Page.IsPostBack) MtsWebPartLog.RecordOpened(LogContext);

                if (!Page.IsPostBack) // sayfa ilk kez açiliyorsa
                {
                    lblTime.Text = DateTime.Now.ToString("HH:mm ss");
                    RefreshTimer.Interval = 10000;
                    RefreshTimer.Enabled = true;

                    FaaliyetTipiDDLDoldur();
                    FaaliyetAmaciDDLDoldur();
                    FaaliyetDurumuDDLDoldur();
                    FaaliyetYeriDoldur();
                    BaslangicSaatiDDLDoldur();
                    BitisSaatiDDLDoldur();
                    Faaliyet faaliyet = new Faaliyet();
                    faaliyet = new Model.Services.MTS.FaaliyetService(LogContext).GetById(FaaliyetIdQS.ConvertToInt());
                    if (faaliyet == null)
                    {
                        if (FaaliyetIdQS.ConvertToInt() > 0) throw new MtsInputException("Faaliyet bulunamadı. Listeden yeniden seçin.");
                        GirisiAc();
                    }
                    else
                    {

                        if (faaliyet != null)
                        {
                            TempFaaliyetQS = new TempFaaliyet(faaliyet);
                            DuzenleAc(faaliyet);
                            KatilimciBilgileriniDoldur(faaliyet, true);
                        }
                    }
                }
            });
        }
        private void FaaliyetFormunuDoldur(Faaliyet faaliyet)
        {
            if (faaliyet != null)
            {
                ViewState["ActivityVersion"] = MtsContactWriteService.Version(faaliyet);
                ViewState["ActivityStale"] = false;
                IdLbl.Text = " ( Faaliyet No: " + faaliyet.Id.ToString() + " )";
                TumGunChk.Checked = faaliyet.TumGun;
                AcikTarihChk.Checked = faaliyet.AcikTarih;
                FaaliyetKonusuTxt.Text = faaliyet.FaaliyetKonusu;
                BaslangicTarihiTxt.Text = faaliyet.BaslangicTarihi.ConvertToDatetimeEmptyIfNull();
                InitialDateQS = faaliyet.BaslangicTarihi.ToString("yyyy-MM-dd");
                BitisTarihiTxt.Text = faaliyet.BitisTarihi.ConvertToDatetimeEmptyIfNull();
                AciklamaTxt.Text = faaliyet.Aciklama;
                YoneticiNotuTxt.Text = faaliyet.YoneticiNotu;
                OzelKalemTakvimiChk.Checked= faaliyet.TakvimeIslendi;

                UtilityHelper.SetDDLValue(FaaliyetTipiDDL, faaliyet.FaaliyetTipi);
                UtilityHelper.SetDDLValue(FaaliyetAmaciDDL, faaliyet.FaaliyetAmaciId.ToString());
                UtilityHelper.SetDDLValue(FaaliyetDurumuDDL, faaliyet.FaaliyetDurumu.ToString());
                //UtilityHelper.SetDDLValue(FaaliyetYeriDDL, faaliyet.FaaliyetYeri.ToString());
                FaaliyetYeriTxt.Text = faaliyet.FaaliyetYeriStr;
                UtilityHelper.SetDDLValue(BasSaatDDL, faaliyet.BaslangicSaati);
                UtilityHelper.SetDDLValue(BitSaatDDL, faaliyet.BitisSaati);

                DisIrtibatIdQS = faaliyet.DisIrtibatId.ToString();
            }

        }


        private void GirisiAc()
        {
            StopTimer();
            ViewState["NewActivityGuid"] = Guid.NewGuid();
            KatilimciBilgileriDiv.Attributes["style"] = "display:none";
            KaydetBtn.Visible = true;
            GuncelleBtn.Visible = false;
            FaaliyetSilBtn.Visible = false;
            FaaliyetKartiBtn.Visible = false;
            TitleLbl.Text = "Yeni Faaliyet";
            TitleLbl.CssClass = "col-form-label text-success fw-bold mb-1";
            BaslangicTarihiTxt.Text = DateTime.Today.ConvertToDatetimeEmptyIfNull();
            BitisTarihiTxt.Text = DateTime.Today.ConvertToDatetimeEmptyIfNull();
            FaaliyetIdQS = string.Empty;
            AramaGorusmeIdQS = string.Empty;
            DisIrtibatIdQS = string.Empty;
            MesajQS = string.Empty;
            if (KatilimciListQS != null)
                KatilimciListQS.Clear();
            TempFaaliyetQS = null;

        }
        private void DuzenleAc(Faaliyet faaliyet)
        {
            if (FaaliyetIdQS.ConvertToInt() > 0)
            {
                KatilimciBilgileriDiv.Attributes["style"] = "display:block";
                KaydetBtn.Visible = false;
                GuncelleBtn.Visible = true;
                FaaliyetKartiBtn.Visible = true;
                FaaliyetSilBtn.Visible = true;
                FaaliyetFormunuDoldur(faaliyet);
                MesajQS = string.Empty;
            }
            else
            {
                GirisiAc();
                Messages.Info("Faaliyet bulunamadı. Listeden yeniden seçin.");
            }

        }
        private void BaslangicSaatiDDLDoldur()
        {
            BasSaatDDL.Items.Clear();
            for (int minute = 0; minute < 24 * 60; minute += 5) BasSaatDDL.Items.Add(TimeSpan.FromMinutes(minute).ToString(@"hh\:mm"));
            UtilityHelper.SetDDLValue(BasSaatDDL, "08:00");
        }
        protected void BasSaatDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "BasSaatDDL_SelectedIndexChanged", () =>
            {
                BitisSaatiDDLDoldur();
            });
        }
        private void BitisSaatiDDLDoldur()
        {
            BitSaatDDL.Items.Clear();
            for (int minute = 0; minute < 24 * 60; minute += 5) BitSaatDDL.Items.Add(TimeSpan.FromMinutes(minute).ToString(@"hh\:mm"));
            TimeSpan start;
            if (!TimeSpan.TryParse(BasSaatDDL.SelectedValue, out start)) start = TimeSpan.FromHours(8);
            TimeSpan end = start.Add(TimeSpan.FromMinutes(30));
            if (end.TotalHours >= 24) end = new TimeSpan(23, 55, 0);
            UtilityHelper.SetDDLValue(BitSaatDDL, end.ToString(@"hh\:mm"));
        }
        private int CurrentActivityId()
        {
            int id;
            if (object.Equals(ViewState["ActivityStale"], true)) throw new MtsInputException("Faaliyet bilgileri değişti. Sayfayı yenileyin.");
            if (object.Equals(ViewState["ActivityDeleted"], true) || !int.TryParse(FaaliyetIdQS, out id) || id <= 0)
                throw new MtsInputException("İşlem için kayıtlı bir faaliyet seçin.");
            return id;
        }
        private int ModalActivityId()
        {
            int id = CurrentActivityId();
            if (paramFaaliyetIdLbl.Value.ConvertToInt() != id) throw new MtsInputException("Faaliyeti yeniden açıp katılımcıyı seçin.");
            return id;
        }
        private static int PositiveQuantity(string text)
        {
            int quantity;
            if (!int.TryParse(text, out quantity) || quantity <= 0) throw new MtsInputException("Adedi pozitif bir tam sayı olarak girin.");
            return quantity;
        }
        private static void RequireActivity(MtsActivityResult result)
        {
            switch (result)
            {
                case MtsActivityResult.Completed: return;
                case MtsActivityResult.AlreadyCompleted: return;
                case MtsActivityResult.NotFound: throw new MtsInputException("Kayıt bulunamadı. Listeyi yenileyin.");
                case MtsActivityResult.Changed: throw new MtsInputException("Kayıt veya stok değişti. Sayfayı ve seçim penceresini yeniden açın.");
                case MtsActivityResult.Duplicate: throw new MtsInputException("Bu kişi zaten faaliyete eklenmiş.");
                case MtsActivityResult.InUse: throw new MtsInputException("İlişkili arama, katılım veya anı objesi kaydı var. İşlem yapılamadı.");
                case MtsActivityResult.InsufficientStock: throw new MtsInputException("Seçilen depoda yeterli stok yok. Depoyu veya adedi değiştirin.");
                case MtsActivityResult.InvalidRelation: throw new MtsInputException("Katılım, dağıtım veya depo ilişkisi geçersiz ya da birden fazla stok kaydı var. Kaydı kontrol edin.");
                case MtsActivityResult.Restricted: throw new MtsInputException("Bu kişi için randevu kısıtı var. Kişi bilgilerini kontrol edin.");
                default: throw new MtsInputException("Tarihleri, seçimleri ve adetleri kontrol edin. İade adedi verilen miktarı aşamaz.");
            }
        }
        private void CaptureStockVersion()
        {
            ViewState["StockVersion"] = Writer.StockVersion(DepoDDL.SelectedValue.ConvertToInt(), StokluAniObjesiDDL.SelectedValue.ConvertToInt());
            if (ViewState["StockVersion"] == null) throw new MtsInputException("Depoda tek bir geçerli stok kaydı bulunamadı. Seçimi kontrol edin.");
        }
        private void RefreshActivity()
        {
            var current = new FaaliyetService(LogContext).GetById(CurrentActivityId());
            if (current == null) throw new MtsInputException("Faaliyet bulunamadı. Listeyi yenileyin.");
            if (MtsContactWriteService.Version(current) != ActivityVersion)
            {
                DisableComponents(); StopTimer();
                throw new MtsInputException("Faaliyet bilgileri değişti. Formu yeniden açın.");
            }
            TempFaaliyetQS = new TempFaaliyet(current);
            KatilimciBilgileriniDoldur(current, true);
        }
        private void ReloadActivity()
        {
            var current = new FaaliyetService(LogContext).GetById(CurrentActivityId());
            if (current == null) throw new MtsInputException("Faaliyet bulunamadı. Listeyi yenileyin.");
            TempFaaliyetQS = new TempFaaliyet(current);
            FaaliyetFormunuDoldur(current); KatilimciBilgileriniDoldur(current, true);
        }
        private void FinishActivity(string message, Action afterCommit)
        {
            try { afterCommit(); }
            catch (System.Threading.ThreadAbortException) { throw; }
            catch (MtsInputException warning) { Messages.Warning(message + " " + warning.Message); return; }
            catch (Exception error)
            {
                PortalTechnicalLog.WriteError(LogContext, "MTS.Faaliyet.AfterCommit", error);
                Messages.Warning(message + " Ekran veya bildirim adımı tamamlanamadı. İşlemi tekrar kaydetmeden önce kontrol edin. Başvuru kodu: " + LogContext.OperationId.ToString("N"));
                return;
            }
            Messages.Success(message);
        }
        private void SaveActivity(bool update)
        {
            if (object.Equals(ViewState["ActivityDeleted"], true)) throw new MtsInputException("Faaliyet silindi. Listeden devam edin.");
            if (!update && FaaliyetIdQS.ConvertToInt() > 0) throw new MtsInputException("Faaliyet zaten kaydedildi. Güncelleme düğmesini kullanın.");
            if (string.IsNullOrWhiteSpace(FaaliyetKonusuTxt.Text)) throw new MtsInputException("Faaliyet konusunu girin.");
            DateTime start, end; TimeSpan startTime, endTime;
            var culture = CultureInfo.GetCultureInfo("tr-TR");
            if (!DateTime.TryParse(BaslangicTarihiTxt.Text, culture, DateTimeStyles.None, out start)
                || !DateTime.TryParse(BitisTarihiTxt.Text, culture, DateTimeStyles.None, out end)
                || !TimeSpan.TryParseExact(BasSaatDDL.SelectedValue, @"hh\:mm", CultureInfo.InvariantCulture, out startTime)
                || !TimeSpan.TryParseExact(BitSaatDDL.SelectedValue, @"hh\:mm", CultureInfo.InvariantCulture, out endTime))
                throw new MtsInputException("Faaliyetin başlangıç ve bitiş tarihlerini ve saatlerini kontrol edin.");
            start = start.Date.Add(startTime); end = end.Date.Add(endTime);
            if (start < new DateTime(1753, 1, 1) || end <= start) throw new MtsInputException("Bitiş, başlangıçtan sonra olmalıdır. Tarih ve saatleri kontrol edin.");
            int id = update ? CurrentActivityId() : 0;
            var previous = update ? new FaaliyetService(LogContext).GetById(id) : null;
            var input = new Faaliyet { Id = id, FaaliyetTipi = FaaliyetTipiDDL.SelectedValue,
                FaaliyetAmaciId = FaaliyetAmaciDDL.SelectedValue.ConvertToInt(), FaaliyetDurumu = FaaliyetDurumuDDL.SelectedValue.ConvertToInt(),
                FaaliyetKonusu = FaaliyetKonusuTxt.Text, FaaliyetYeriStr = FaaliyetYeriTxt.Text,
                TumGun = TumGunChk.Checked, AcikTarih = AcikTarihChk.Checked, BaslangicTarihi = start, BitisTarihi = end,
                BaslangicSaati = BasSaatDDL.SelectedValue, BitisSaati = BitSaatDDL.SelectedValue,
                Aciklama = AciklamaTxt.Text, YoneticiNotu = YoneticiNotuTxt.Text,
                TakvimeIslendi = OzelKalemTakvimiChk.Checked, DisIrtibatId = DisIrtibatIdQS.ConvertToInt() };
            if (!update)
            {
                if (!(ViewState["NewActivityGuid"] is Guid)) throw new MtsInputException("Yeni faaliyet formunu yeniden açın.");
                input.UniqueId = (Guid)ViewState["NewActivityGuid"];
            }
            Faaliyet saved;
            var result = Writer.Save(input, ActivityVersion, out saved);
            RequireActivity(result);
            FaaliyetIdQS = saved.Id.ToString(); ViewState["ActivityVersion"] = MtsContactWriteService.Version(saved);
            TempFaaliyetQS = new TempFaaliyet(saved); KaydetBtn.Visible = false; GuncelleBtn.Visible = true;
            FinishActivity(result == MtsActivityResult.AlreadyCompleted ? "Faaliyet zaten kaydedilmiş. Güncel kayıt açıldı."
                : update ? "Faaliyet güncellendi." : "Faaliyet kaydedildi.", () =>
            {
                DuzenleAc(saved); KatilimciBilgileriniDoldur(saved, true); RefreshTimer.Enabled = true;
                if (result == MtsActivityResult.AlreadyCompleted) return;
                if (saved.TakvimeIslendi && !saved.AcikTarih)
                    MtsActivityNotificationService.Send(saved, ProjeConstants.PARAM_OZELKALEMMAILADRESI, saved.FaaliyetDurumu == ProjeConstants.FAALIYET_DURUMU_IPTALEDILDI_INT);
                else if (previous != null && previous.TakvimeIslendi && !previous.AcikTarih)
                    MtsActivityNotificationService.Send(previous, ProjeConstants.PARAM_OZELKALEMMAILADRESI, true);
                if (update) KatilimcilaraTakvimDavetiGonder(saved);
            });
        }

        protected void KaydetBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "KaydetBtn_Click", () =>
            {
                SaveActivity(false);
            });
        }



        protected void GuncelleBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "GuncelleBtn_Click", () =>
            {
                SaveActivity(true);
            });
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "CloseBtn_Click", () =>
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path);
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
                Page.Response.Redirect(newUrl);
            });
        }
        protected void FaaliyetSilBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "FaaliyetSilBtn_Click", () =>
            {
                Faaliyet faaliyet = new Faaliyet();
                faaliyet = new Model.Services.MTS.FaaliyetService(LogContext).GetById(FaaliyetIdQS.ConvertToInt());
                if (faaliyet != null)
                {
                    if (!BaglantisiVarMi(faaliyet))
                    {
                        FaaliyetSilPopupAc(sender);
                    }

                }
                else
                {
                    Messages.Warning("Faaliyet bulunamadı. Listeden yeniden seçin.");
                }
            });
        }

        private bool BaglantisiVarMi(Faaliyet faaliyet)
        {
            bool linked = new AramaGorusmeService(LogContext).GetByFaaliyetId(faaliyet.Id) != null
                || new FaaliyetKatilimService(LogContext).GetByFaaliyetId(faaliyet.Id).Count > 0
                || new AniObjesiDagitimService(LogContext).GetByFaaliyetId(faaliyet.Id).Count > 0;
            if (linked) throw new MtsInputException("Faaliyetin arama, katılımcı veya anı objesi bağlantısı var. Faaliyet silinemez.");
            return false;
        }

        protected void KisiListesiBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "KisiListesiBtn_Click", () =>
            {
                RedirectToPage(ProjeConstants.PAGE_KISI_LIST);
            });
        }
        protected void FaaliyetListesiBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "FaaliyetListesiBtn_Click", () =>
            {
                RedirectToPage(ProjeConstants.PAGE_FAALIYET_LIST + "?SecilenId=" + FaaliyetIdQS);
            });
        }
        protected void AcikTarihliFaaliyetListesiBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "AcikTarihliFaaliyetListesiBtn_Click", () =>
            {
                RedirectToPage(ProjeConstants.PAGE_ACIKTARIHLIFAALIYET_LIST);
            });
        }
        protected void FaaliyetTakvimiBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "FaaliyetTakvimiBtn_Click", () =>
            {
                RedirectToPage(ProjeConstants.PAGE_FAALIYET_TAKVIM + "?InitialDate=" + InitialDateQS);
            });
        }
        private void FaaliyetSilPopupAc(object sender)
        {
            ParamVnLbl.Text = FaaliyetIdQS;
            string openModal = "OpenSilModal();";
            SilMesajiLbl.Visible = true;
            SilMesajiLbl.Text = "Faaliyete ait tüm bilgiler silinecek. Silmek istediginizden emin misiniz?";
            SilModalBaslikLbl.Text = "Faaliyet Silinecek";
            FaaliyetSilNowBtn.Visible = true;
            UtilityHelper.ScriptCalistir(openModal);
        }
        /// FaaliyetKatilim_Tabeledan bu faaliyet Id'li kayitlari sil
        /// AramaGorusme_Table'da bu faaliyetId'li kayitlarin faaliyetId'sini 0 yap
        /// AniObjesiDagitim_Table'dan bu faaliyetId'li olanlari sil
        protected void FaaliyetSilNowBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "FaaliyetSilNowBtn_Click", () =>
            {
                Faaliyet deleted;
                RequireActivity(Writer.Delete(CurrentActivityId(), ActivityVersion, out deleted));
                ViewState["ActivityDeleted"] = true;
                DisableComponents(); StopTimer();
                FinishActivity("Faaliyet silindi. Faaliyet listesinden devam edebilirsiniz.", () =>
                {
                    if (deleted.TakvimeIslendi) MtsActivityNotificationService.Send(deleted, ProjeConstants.PARAM_OZELKALEMMAILADRESI, true);
                    CloseModals();
                });
            });
        }

        private void FaaliyetTipiDDLDoldur()
        {
            ListItem li = new ListItem(ProjeConstants.RANDEVU_VERILEN);
            ListItem li2 = new ListItem(ProjeConstants.RANDEVU_ALINAN);
            ListItem li3 = new ListItem(ProjeConstants.RANDEVU_DIGER);

            FaaliyetTipiDDL.Items.Clear();
            FaaliyetTipiDDL.Items.Add(li);
            FaaliyetTipiDDL.Items.Add(li2);
            FaaliyetTipiDDL.Items.Add(li3);
        }
        private void FaaliyetAmaciDDLDoldur()
        {
            ListItem li5 = new ListItem(ProjeConstants.FAALIYET_AMACI_TOPLANTI, ProjeConstants.FAALIYET_AMACI_TOPLANTI_INT);
            ListItem li = new ListItem(ProjeConstants.FAALIYET_AMACI_ZIYARET, ProjeConstants.FAALIYET_AMACI_ZIYARET_INT);
            ListItem li7 = new ListItem(ProjeConstants.FAALIYET_AMACI_GORUSME, ProjeConstants.FAALIYET_AMACI_GORUSME_INT);
            ListItem li2 = new ListItem(ProjeConstants.FAALIYET_AMACI_DAVET, ProjeConstants.FAALIYET_AMACI_DAVET_INT);
            ListItem li3 = new ListItem(ProjeConstants.FAALIYET_AMACI_YILDONUMU, ProjeConstants.FAALIYET_AMACI_YILDONUMU_INT);
            ListItem li4 = new ListItem(ProjeConstants.FAALIYET_AMACI_OZELCALISMA, ProjeConstants.FAALIYET_AMACI_OZELCALISMA_INT);
            ListItem li6 = new ListItem(ProjeConstants.FAALIYET_AMACI_IZIN, ProjeConstants.FAALIYET_AMACI_IZIN_INT);
            ListItem li8 = new ListItem(ProjeConstants.FAALIYET_AMACI_SEYAHAT, ProjeConstants.FAALIYET_AMACI_SEYAHAT_INT);
            ListItem li9 = new ListItem(ProjeConstants.FAALIYET_AMACI_BILGI, ProjeConstants.FAALIYET_AMACI_BILGI_INT);


            FaaliyetAmaciDDL.Items.Clear();
            FaaliyetAmaciDDL.Items.Add(li5);
            FaaliyetAmaciDDL.Items.Add(li);
            FaaliyetAmaciDDL.Items.Add(li7);
            FaaliyetAmaciDDL.Items.Add(li2);
            FaaliyetAmaciDDL.Items.Add(li3);
            FaaliyetAmaciDDL.Items.Add(li4);
            FaaliyetAmaciDDL.Items.Add(li6);
            FaaliyetAmaciDDL.Items.Add(li8);
            FaaliyetAmaciDDL.Items.Add(li9);
        }
        private void FaaliyetDurumuDDLDoldur()
        {
            ListItem li = new ListItem(ProjeConstants.FAALIYET_DURUMU_PLANLANDI, ProjeConstants.FAALIYET_DURUMU_PLANLANDI_INT.ToString());
            ListItem li2 = new ListItem(ProjeConstants.FAALIYET_DURUMU_ONAYLANDI, ProjeConstants.FAALIYET_DURUMU_ONAYLANDI_INT.ToString());
            ListItem li3 = new ListItem(ProjeConstants.FAALIYET_DURUMU_IPTALEDILDI, ProjeConstants.FAALIYET_DURUMU_IPTALEDILDI_INT.ToString());

            FaaliyetDurumuDDL.Items.Clear();
            FaaliyetDurumuDDL.Items.Add(li);
            FaaliyetDurumuDDL.Items.Add(li2);
            FaaliyetDurumuDDL.Items.Add(li3);
        }

        private void FaaliyetYeriDoldur()
        {
            List<string> places = new FaaliyetService(LogContext).GetDistinctPlaces();
            string json = new JavaScriptSerializer().Serialize(places).Replace("<", "\\u003c").Replace(">", "\\u003e").Replace("&", "\\u0026");
            UtilityHelper.ScriptCalistir("setDataSet(" + json + ");");
        }
        private void StokluAniObjesiDDLDoldur()
        {
            StokluAniObjesiDDL.Items.Clear();
            DataTable dataTable = new Model.Services.MTS.AniObjesiTanimService(LogContext).GetStokluAniObjesiList();
            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    int aniObjesiId = row["AniObjesiId"].ConvertToInt();
                    string aniObjesiAdi = row["Adi"].ReturnEmptyIfNull().ToString();
                    int toplam = row["Toplam"].ConvertToInt();
                    ListItem li = new ListItem(aniObjesiAdi.ToString() + " (" + toplam + ")", aniObjesiId.ToString());
                    StokluAniObjesiDDL.Items.Add(li);
                }

            }

        }
        private void DepoDDLDoldur()
        {
            DepoDDL.Items.Clear();
            int secilenAniObjesiId = StokluAniObjesiDDL != null && StokluAniObjesiDDL.SelectedItem != null ? StokluAniObjesiDDL.SelectedItem.Value.ConvertToInt() : 0;
            DataTable dataTable = new Model.Services.MTS.DepoTanimService(LogContext).GetStokluAniObjesiList(secilenAniObjesiId);
            if (dataTable != null)
            {
                int i = 0;
                foreach (DataRow row in dataTable.Rows)
                {
                    i++;
                    int depoId = row["DepoId"].ConvertToInt();
                    string depoAdi = row["DepoAdi"].ReturnEmptyIfNull().ToString();
                    int adet = row["Adet"].ConvertToInt();
                    ListItem li = new ListItem(depoAdi.ToString() + " (" + adet + ")", depoId.ToString());
                    li.Attributes.Add("adet", adet.ToString());
                    DepoDDL.Items.Add(li);
                }
            }
        }
        private void RedirectToPage(string pageUrl)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path);
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + pageUrl;
            Page.Response.Redirect(newUrl,false);

        }
        private void DisIrtibatNoktasiDoldur(int kisiId, bool buttonsEnabled)
        {
            Kisi kisi = new Kisi();
            kisi = new Model.Services.MTS.KisiService(LogContext).GetById(kisiId);
            if (kisi != null)
            {
                DisIrtibatLbl.Text = System.Web.HttpUtility.HtmlEncode("İrtibat Noktası: " + kisi.Adi + " " + kisi.Soyadi);
                DisIrtibatIdQS = kisi.Id.ToString();
                DisIrtibatCikarBtn.Visible = buttonsEnabled;
            }
            else
            {
                DisIrtibatLbl.Text = string.Empty;
                DisIrtibatCikarBtn.Visible = false;
            }
        }
        protected void IrtibatSecBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "IrtibatSecBtn_Click", () =>
            {
                int personId = paramFaaliyetKatilimciIdLbl.Value.ConvertToInt();
                if (personId <= 0) throw new MtsInputException("Listeden bir irtibat kişisi seçin.");
                RequireActivity(Writer.SetContact(CurrentActivityId(), ActivityVersion, personId));
                FinishActivity("İrtibat kişisi güncellendi.", ReloadActivity);
            });
        }
        protected void FaaliyetDurumuDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "FaaliyetDurumuDDL_SelectedIndexChanged", () =>
            {
                //FaaliyetDurumuImg.ImageUrl = FaaliyetDurumuImgGetir(FaaliyetDurumuDDL.SelectedItem.Value.ConvertToInt());
            });
        }
        #region Katilimci Tablosu
        private void TabloOlustur(bool buttonsEnabled)
        {
            var jsonData = TabloJson(buttonsEnabled); //veri çekilip json a çeviriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazirlaniyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string CreateDataTable(string jsonData)
        {
            string tableString = @"
             if ( jQuery.fn.DataTable.isDataTable('#CustomDataTable') ) {
                    jQuery('#CustomDataTable').DataTable().destroy();
                }
                jQuery('#CustomDataTable tbody').empty();

            jQuery('#CustomDataTable').DataTable({
            data: " + jsonData + @",
            columns: [
                { data: 'AdiSoyadi', render: jQuery.fn.dataTable.render.text() },
                { data: 'Kurumu', render: jQuery.fn.dataTable.render.text() },
                { data: 'KatilimciTipi', visible: false },
                { data: 'AniObjesi' },
                { data: 'TakvimDavetiBtn' },
                { data: 'KisiKarti' },
                { data: 'Cikar' },
            ],
            'order': [[2, 'desc']],
            'language': {
                'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                'decimal': ',',
                'thousands': '.'
            },
            responsive: true,
            dom: 'rt',
            'lengthMenu': [[-1], ['All']],
            'createdRow': function (row, data, dataIndex) {
                if (data.KatilimciTipi == 1) {
                    $(row).addClass('icKatilimci');

                } else if (data.KatilimciTipi == 2) {
                    $(row).addClass('disKatilimci');

                } else if (data.KatilimciTipi == 3) {
                    $(row).addClass('nakitBagisci');

                } else if (data.KatilimciTipi == 4) {
                    $(row).addClass('tasinmazBagisci');

                }
            }
        });
            ";

            return tableString;
        }
        private string TabloJson(bool buttonsEnable)
        {
            string jSon = string.Empty;

            List<KatilimciListItem> list = GetDataList(buttonsEnable);
            if (buttonsEnable)
            {
                KatilimciListQS = list.Select(a => a.KatilimciId + " " + a.ObjectState).ToList();
            }

            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
            jSon = serializer.Serialize(list);

            return jSon.Replace("<", "\\u003c").Replace(">", "\\u003e").Replace("&", "\\u0026");
        }
        private List<KatilimciListItem> GetDataList(bool buttonsEnable)
        {

            Faaliyet faaliyetDao = new Faaliyet();
            DataTable dataTable = new Model.Services.MTS.FaaliyetService(LogContext).GetParticipantList(FaaliyetIdQS.ConvertToInt(), ProjeConstants.HEPSI_INT, ProjeConstants.HEPSI, ProjeConstants.NULL_TARIH, ProjeConstants.NULL_TARIH, ProjeConstants.HEPSI);
            int SiraNo = 1;
            List<KatilimciListItem> list = new List<KatilimciListItem>();
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    int katilimId = row["KatilimId"].ReturnZeroIfNull().ConvertToInt();
                    int katilimciId = row["KatilimciId"].ReturnZeroIfNull().ConvertToInt();
                    if (katilimciId < 1)
                        continue;
                    int katilimciTipi = row["KatilimciTipi"].ReturnZeroIfNull().ConvertToInt();
                    string adi = row["Adi"].ToString();
                    string soyadi = row["Soyadi"].ToString();
                    string kurumu = row["Kurumu"].ReturnEmptyIfNull().ToString();
                    string takvimDaveti = row["TakvimDaveti"].ReturnEmptyIfNull().ToString();
                    string eposta = row["EPosta"].ReturnEmptyIfNull().ToString();

                    KatilimciListItem katilimciItem = new KatilimciListItem();
                    katilimciItem.Sirano = SiraNo++.ToString();
                    katilimciItem.KatilimciId = katilimciId.ToString();
                    katilimciItem.Adi = adi;
                    katilimciItem.Soyadi = soyadi;
                    katilimciItem.AdiSoyadi = (adi + " " + soyadi.Trim());
                    katilimciItem.Kurumu = kurumu;
                    katilimciItem.KatilimciTipi = katilimciTipi.ToString();
                    katilimciItem.TakvimDavetiBtn = TakvimDavetiBtnOlustur(katilimId, takvimDaveti, katilimciId, eposta);


                    string stoksuzAniObjeleri = AniObjeleriniGetir(katilimciItem.KatilimciId.ConvertToInt(),
                        FaaliyetIdQS.ConvertToInt(), ProjeConstants.MTS_ANIOBJESISTOKSUZ);
                    string stokluAniObjeleri = AniObjeleriniGetir(katilimciItem.KatilimciId.ConvertToInt(),
                        FaaliyetIdQS.ConvertToInt(), ProjeConstants.MTS_ANIOBJESISTOKLU);
                    string getirilenAniObjeleri = GetirilenAniObjeleriniGetir(katilimciItem.KatilimciId.ConvertToInt(),
                        FaaliyetIdQS.ConvertToInt());

                    katilimciItem.ObjectState = stokluAniObjeleri + "|" + stoksuzAniObjeleri + "|" + getirilenAniObjeleri;
                    if (buttonsEnable)
                    {
                        katilimciItem.AniObjesiStoksuz = "<a href='#' class='btn btn-outline-primary' onclick=StoksuzAniObjesiBtnClick(" + katilimciId + "," + FaaliyetIdQS + ")>Stoksuz Ani Objesi</a>" +
                            "<br>" + stoksuzAniObjeleri;
                        katilimciItem.AniObjesiStoklu = "<a href='#' class='btn btn-outline-success' onclick=StokluAniObjesiBtnClick(" + katilimciId + "," + FaaliyetIdQS + ")>Stoklu Ani Objesi</a>" +
                            "<br>" + stokluAniObjeleri;
                        katilimciItem.GetirilenAniObjesi = "<a href='#' class='btn btn-outline-info' onclick=GetirilenAniObjesiBtnClick(" + katilimciId + "," + FaaliyetIdQS + ")>Getirilen Ani Objesi </a>" +
                            "<br>" + getirilenAniObjeleri;

                        katilimciItem.AniObjesi = katilimciItem.AniObjesiStoklu + "<hr>" + katilimciItem.AniObjesiStoksuz + "<hr>" + katilimciItem.GetirilenAniObjesi;
                    }
                    else
                    {
                        katilimciItem.AniObjesiStoksuz = stoksuzAniObjeleri;
                        katilimciItem.AniObjesiStoklu = stokluAniObjeleri;
                        katilimciItem.GetirilenAniObjesi = getirilenAniObjeleri;
                    }


                    katilimciItem.KisiKarti = "<a  target='_blank' href=" + ProjeConstants.PAGE_KISI_KARTI + "?KatilimciId=" + katilimciId + " class='btn btn-outline-info'>Kişi Kartı</a>";
                    if (buttonsEnable)
                        katilimciItem.Cikar = "<a href='#' class='btn btn-outline-danger' onclick=KatilimciCikarBtnClick(" + katilimId + ")>Çıkar</a>";

                    list.Add(katilimciItem);
                }
            }
            return list;
        }
        #endregion

        protected void FaaliyetKartiBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "FaaliyetKartiBtn_Click", () =>
            {
                RedirectToPage(ProjeConstants.PAGE_FAALIYET_KARTI + "?FaaliyetId=" + FaaliyetIdQS);
            });
        }

        protected void AcikTarihChk_CheckedChanged(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "AcikTarihChk_CheckedChanged", () =>
            {
                AcikTarihliKontrolu();
            });
        }

        private void AcikTarihliKontrolu()
        {
            Faaliyet faaliyet = new Faaliyet();
            faaliyet = new Model.Services.MTS.FaaliyetService(LogContext).GetById(FaaliyetIdQS.ConvertToInt());
            if (faaliyet == null && AcikTarihChk.Checked)
            {
                OzelKalemTakvimiChk.Checked = false;
                Messages.Info("Açık tarihli faaliyet, kesin tarih verilene kadar özel kalem takvimine gönderilmeyecek.");
            }
        }

        protected void OzelKalemTakvimiChk_CheckedChanged(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "OzelKalemTakvimiChk_CheckedChanged", () =>
            {
                AcikTarihliKontrolu();
            });
        }
        #endregion

        #region Modal katilimci seçme ve ekleme
        protected void KatilimciEkleBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "KatilimciEkleBtn_Click", () =>
            {
                KatilimciModalAc();
            });
        }
        protected void KatilimciCikarBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "KatilimciCikarBtn_Click", () =>
            {
                RequireActivity(Writer.RemoveParticipant(CurrentActivityId(), paramFaaliyetKatilimIdLbl.Value.ConvertToInt()));
                FinishActivity("Katılımcı faaliyetten çıkarıldı.", RefreshActivity);
            });
        }
        private void KatilimciModalAc()
        {
            TabloModalOlustur();
            UtilityHelper.ScriptCalistir("KatilimciSecimiModal();");
        }
        private void TabloModalOlustur()
        {
            var jsonData = TabloModalJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateModalDataTable(jsonData); //javascript kodu hazirlaniyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string TabloModalJson()
        {
            string jSon = string.Empty;

            List<KatilimciListItem> list = GetModalDataList();
            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
            jSon = serializer.Serialize(list);

            return jSon.Replace("<", "\\u003c").Replace(">", "\\u003e").Replace("&", "\\u0026");
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
                { data: 'AdiSoyadi', render: jQuery.fn.dataTable.render.text() },
                { data: 'Kurumu', render: jQuery.fn.dataTable.render.text() },
                { data: 'KatilimciSec' },
                { data: 'IrtibatSec' }
            ],
            'language': {
                'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
            },
            responsive: true,
            dom: 'fpirt',
            'createdRow': function (row, data, dataIndex) {
                if (data.KatilimciTipi == 1) {
                    $(row).addClass('icKatilimci');

                } else if (data.KatilimciTipi == null || data.KatilimciTipi ==0 || data.KatilimciTipi == 2) {
                    $(row).addClass('disKatilimci');

                } else if (data.KatilimciTipi == 3) {
                    $(row).addClass('nakitBagisci');

                } else if (data.KatilimciTipi == 4) {
                    $(row).addClass('tasinmazBagisci');
                }
            }
        });
            ";

            return tableString;
        }
        private List<KatilimciListItem> GetModalDataList()
        {

            Kisi kisi = new Kisi();
            DataTable dataTable = new Model.Services.MTS.KisiService(LogContext).GetUnselectedParticipants(FaaliyetIdQS.ConvertToInt());


            int SiraNo = 1;
            List<KatilimciListItem> list = new List<KatilimciListItem>();
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    string katilimciId = row["KatilimciId"].ToString();
                    int katilimciTipi = row["KatilimciTipi"].ConvertToInt();
                    string adi = row["Adi"].ToString();
                    string soyadi = row["Soyadi"].ToString();
                    string kurumu = row["Kurumu"].ToString();
                    bool randevuKisiti = row["RandevuKisiti"].ReturnFalseIfNull().ConvertToBool();

                    KatilimciListItem katilimciItem = new KatilimciListItem();
                    katilimciItem.Sirano = SiraNo++.ToString();
                    katilimciItem.KatilimciId = katilimciId;
                    katilimciItem.AdiSoyadi = (adi + " " + soyadi.Trim());
                    katilimciItem.Adi = adi;
                    katilimciItem.Soyadi = soyadi;
                    katilimciItem.Kurumu = kurumu;
                    katilimciItem.KatilimciTipi = katilimciTipi.ToString();

                    if (randevuKisiti)//randevu kisiti varsa faaliyete ekle çikmasin
                    {
                        katilimciItem.KatilimciSec = "RK";
                        katilimciItem.IrtibatSec = "RK";
                    }
                    else
                    {

                        katilimciItem.KatilimciSec = "<a href='#' class='btn btn-outline-primary' onclick=KatilimciSecildiBtnClick(" + katilimciId + "," + FaaliyetIdQS + ")>Faaliyete Ekle</a>";
                        katilimciItem.IrtibatSec = "<a href='#' class='btn btn-outline-primary' onclick=IrtibatSecBtnClick(" + katilimciId + "," + FaaliyetIdQS + ")>Irtibat Ekle</a>";
                    }


                    list.Add(katilimciItem);
                }
            }
            return list;
        }
        protected void SecilenKatilimciyiKaydetNowBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "SecilenKatilimciyiKaydetNowBtn_Click", () =>
            {
                RequireActivity(Writer.AddParticipant(ModalActivityId(), paramFaaliyetKatilimciIdLbl.Value.ConvertToInt()));
                FinishActivity("Katılımcı faaliyete eklendi.", () => { RefreshActivity(); CloseModals(); });
            });
        }
        private void KatilimciBilgileriniDoldur(Faaliyet faaliyet, bool buttonsEnabled)
        {
            if (faaliyet != null)
            {
                AramaGorusmeBilgileriniDoldur(faaliyet);
                DisIrtibatNoktasiDoldur(faaliyet.DisIrtibatId, buttonsEnabled);

                TabloOlustur(buttonsEnabled);
                if (buttonsEnabled)
                {
                    TempFaaliyetQS.DisIrtibatId = faaliyet.DisIrtibatId;
                }


            }

        }
        #endregion

        #region Arama/Gorusme bilgileri
        private void AramaGorusmeBilgileriniDoldur(Faaliyet faaliyet)
        {
            AramaGorusme aramaGorusme = new Model.Services.MTS.AramaGorusmeService(LogContext).GetByFaaliyetId(faaliyet.Id);
            if (aramaGorusme != null)
            {
                AramaGorusmeIdQS = aramaGorusme.Id.ToString();
                AramaGorusmeLbl.Text = "Arama/Görüsme No:" + aramaGorusme.Id;

                AramaGorusmeDiv.Attributes["style"] = "display:block";
            }
            else
            {
                AramaGorusmeDiv.Attributes["style"] = "display:none";
                AramaGorusmeIdQS = "0";
            }

        }
        protected void AramaGorusmeBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "AramaGorusmeBtn_Click", () =>
            {
                if (AramaGorusmeIdQS.ConvertToInt() > 0)
                {
                    RedirectToPage(ProjeConstants.PAGE_ARAMAGORUSME_GIRIS + "?AramaGorusmeId=" + AramaGorusmeIdQS);
                }
                else
                {
                    Messages.Warning("Arama/görüşme bağlantısı bulunamadı. Kaydı listeden kontrol edin.");
                }
            });
        }
        #endregion

        #region Ani Objesi
        protected void AniObjesiSecBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "AniObjesiSecBtn_Click", () =>
            {
                int katilimciId = paramFaaliyetKatilimciIdLbl.Value.ConvertToInt();
                int faaliyetId = paramFaaliyetIdLbl.Value.ConvertToInt();
                if (katilimciId > 0)
                {

                        Kisi kisi = new Kisi();
                        kisi = new Model.Services.MTS.KisiService(LogContext).GetById(katilimciId);
                        if (kisi != null)
                        {
                            AniObjesiHeaderLbl.InnerText = "Ani Objesi Seçimi (" + kisi.Adi + " " + kisi.Soyadi + ")";
                            AniObjeleriTablosunuDoldur(faaliyetId, katilimciId);
                        }
                        else
                        {
                            Messages.Warning("Kişi bulunamadı. Listeden yeniden seçin.");
                        }

                }
                else
                {
                    Messages.Warning("Katılımcı bulunamadı. Listeden yeniden seçin.");
                }
            });
        }
        protected void StokluAniObjesiSecBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "StokluAniObjesiSecBtn_Click", () =>
            {
                int distributionId = paramStokluAniObjesiDagitimIdLbl.Value.ConvertToInt();
                ViewState["StockVersion"] = ViewState["ReturnVersion"] = null;
                if (distributionId > 0)
                {
                    var distribution = new AniObjesiDagitimService(LogContext).GetById(distributionId);
                    if (distribution == null || distribution.FaaliyetId != CurrentActivityId())
                        throw new MtsInputException("Dağıtım kaydı bulunamadı veya bu faaliyete ait değil.");
                    var item = new AniObjesiTanimService(LogContext).GetById(distribution.AniObjesiId);
                    var warehouse = new DepoTanimService(LogContext).GetById(distribution.CikisDepoId);
                    if (item == null || warehouse == null) throw new MtsInputException("Anı objesi veya çıkış deposu bulunamadı. Kaydı kontrol edin.");
                    IadeEdilecekAdetTxt.Text = distribution.Adet.ToString();
                    IadeEdilecekAniObjesiTxt.Text = item.Adi; IadeEdilecekDepoTxt.Text = warehouse.Adi;
                    ViewState["ReturnVersion"] = MtsContactWriteService.Version(distribution);
                    DagitimDiv.Attributes["style"] = "display:none"; IadeDiv.Attributes["style"] = "display:block";
                }
                else
                {
                    ModalActivityId();
                    var person = new KisiService(LogContext).GetById(paramFaaliyetKatilimciIdLbl.Value.ConvertToInt());
                    if (person == null) throw new MtsInputException("Listeden geçerli bir katılımcı seçin.");
                    StokluAniObjesiModalTitle.InnerText = "Stoklu Anı Objesi Seçimi (" + person.Adi + " " + person.Soyadi + ")";
                    DagitimDiv.Attributes["style"] = "display:block"; IadeDiv.Attributes["style"] = "display:none";
                    StokluAniObjesiDDLDoldur(); DepoDDLDoldur(); CaptureStockVersion();
                }
                UtilityHelper.ScriptCalistir("StokluAniObjesiModal();");
            });
        }
        protected void GetirilenAniObjesiSecBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "GetirilenAniObjesiSecBtn_Click", () =>
            {
                int katilimciId = paramFaaliyetKatilimciIdLbl.Value.ConvertToInt();
                int faaliyetId = paramFaaliyetIdLbl.Value.ConvertToInt();

                if (katilimciId > 0)
                {

                        Kisi kisi = new Kisi();
                        kisi = new Model.Services.MTS.KisiService(LogContext).GetById(katilimciId);
                        if (kisi != null)
                        {
                            GetirilenAniObjesiModalTitle.InnerText = "Getirilen Ani Objesi (" + kisi.Adi + " " + kisi.Soyadi + ")";
                            GetirilenAniObjesiniDoldur(faaliyetId, katilimciId);
                        }
                        else
                        {
                            Messages.Warning("Kişi bulunamadı. Listeden yeniden seçin.");
                        }


                }
                else
                {
                    Messages.Warning("Katılımcı bulunamadı. Listeden yeniden seçin.");
                }
            });
        }
        public void GetirilenAniObjesiniDoldur(int faaliyetId, int katilimciId)
        {
            if (faaliyetId != CurrentActivityId()) throw new MtsInputException("Faaliyeti yeniden açıp katılımcıyı seçin.");
            ViewState["ReceivedVersion"] = Writer.ObjectVersion(faaliyetId, katilimciId);
            paramFaaliyetIdLbl.Value = faaliyetId.ToString();
            paramFaaliyetKatilimciIdLbl.Value = katilimciId.ToString();

            GetirilenAniObjesiTxt.Text = string.Empty;
            AniObjesiDagitim getirilenAniObjesi = new AniObjesiDagitim();
            getirilenAniObjesi = new Model.Services.MTS.AniObjesiDagitimService(LogContext).GetGetirilen(faaliyetId, katilimciId);
            if (getirilenAniObjesi != null)
            {
                GetirilenAniObjesiTxt.Text = getirilenAniObjesi.GetirilenAniObjesi;
            }
            UtilityHelper.ScriptCalistir("GetirilenAniObjesiModal();");
        }
        public JavaScriptSerializer javaSerial = new JavaScriptSerializer();
        /// <summary>
        /// Ani Objelerini modal pencereye getirir
        /// Seçilen katilimci ve faaliyetya göre AniObjesiDagitim_Table ile FaaliyetYeri_Table join edilerek sorgulanir,
        /// Daha önceden isaretlenmis olanlar frontend'de javascript arrayde tutulur ve ekrana dolu olarak gelir.
        /// Checkbox check degeri degistiginde,  sadece frontend deki array'e eklenir veya çikarilir.
        /// Kaydete basildiginda array paramArray'e aktarilir, kaydetNowBtn click çalisir, devami btn_click içinde yapilir
        /// </summary>
        /// <param name="faaliyetId"></param>
        /// <param name="katilimciId"></param>
        private void AniObjeleriTablosunuDoldur(int faaliyetId, int katilimciId)
        {
            if (faaliyetId != CurrentActivityId()) throw new MtsInputException("Faaliyeti yeniden açıp katılımcıyı seçin.");
            ViewState["ObjectVersion"] = Writer.ObjectVersion(faaliyetId, katilimciId);
            paramFaaliyetIdLbl.Value = faaliyetId.ToString();
            paramFaaliyetKatilimciIdLbl.Value = katilimciId.ToString();

            AniObjesiDagitim aniObjesiDagitim = new AniObjesiDagitim();
            DataTable dataTable = new Model.Services.MTS.AniObjesiDagitimService(LogContext).GetDistributionTable(faaliyetId, katilimciId, ProjeConstants.MTS_ANIOBJESISTOKSUZ);
            paramAniObjesiIdArray.Value = paramAniObjesiAdetArray.Value = string.Empty;
            CustomAniObjesiModalDataTable.Rows.Clear();
            if (dataTable != null)
            {
                //CheckBox[] aniObjesiChk = new CheckBox[AniObjesiList.Count];
                int counter = 0;
                bool artti = false;

                string aniObjesiIdStr = string.Empty;
                string aniObjesiAdetStr = string.Empty;
                while (counter < dataTable.Rows.Count)
                {
                    DataRow row = dataTable.Rows[counter];

                    TableRow tableRow = new TableRow();
                    for (int i = 0; i < 4; i++)
                    {
                        row = dataTable.Rows[counter];

                        int aniObjesiId = row["AniObjesiId"].ReturnZeroIfNull().ConvertToInt();
                        int sira = row["Sira"].ReturnZeroIfNull().ConvertToInt();
                        string deger = row["Adi"].ToString();
                        int aniObjesiDagitimId = row["AniObjesiDagitimId"].ReturnZeroIfNull().ConvertToInt();
                        int adet = row["Adet"].ReturnZeroIfNull().ConvertToInt();

                        TableCell degerCell = new TableCell();

                        HtmlGenericControl newdiv = new HtmlGenericControl("DIV");
                        newdiv.ID = "newDiv" + aniObjesiId;

                        HtmlGenericControl label = new HtmlGenericControl("label");
                        newdiv.Controls.Add(label);

                        CheckBox chkBox = new CheckBox();
                        chkBox.ID = "chkBox" + aniObjesiId;
                        chkBox.Text = System.Web.HttpUtility.HtmlEncode(deger);
                        chkBox.Checked = aniObjesiDagitimId > 0;
                        if (chkBox.Checked)
                        {
                            aniObjesiIdStr += "," + aniObjesiId.ToString();
                            aniObjesiAdetStr += "," + adet.ToString();
                            newdiv.Attributes.Add("class", "checkbox fw-bold text-danger");
                        }
                        else
                        {
                            newdiv.Attributes.Add("class", "checkbox");
                        }

                        label.Controls.Add(chkBox);
                        degerCell.Controls.Add(newdiv);

                        TableCell adetCell = new TableCell();
                        TextBox adetTxt = new TextBox();
                        adetTxt.CssClass = "form-control input-integer";
                        adetTxt.ID = "adet" + aniObjesiId;
                        adetTxt.Width = 50;
                        adetTxt.Attributes.Add("type", "number");
                        adetTxt.Attributes.Add("step", "1");
                        adetTxt.Text = adet.ToString();

                        adetTxt.Enabled = chkBox.Checked;
                        adetTxt.Attributes.Add("onchange", "ChangeAniObjesiAdet(" + aniObjesiId + "," + chkBox.ID.ReturnQuotedValue() + "," + adetTxt.Text + ",this," + newdiv.ID.ReturnQuotedValue() + ");");

                        adetCell.Controls.Add(adetTxt);

                        chkBox.Attributes.Add("onclick", "AddRemoveAniObjesiIdToList(" + aniObjesiId + ",this," + newdiv.ID.ReturnQuotedValue() + "," + adetTxt.Text + "," + adetTxt.ID.ReturnQuotedValue() + ");");

                        tableRow.Controls.Add(degerCell);
                        tableRow.Controls.Add(adetCell);
                        if (dataTable.Rows.Count > counter + 1)
                        {
                            counter++;
                            artti = true;
                        }
                        else
                        {
                            artti = false;
                            break;
                        }
                    }
                    CustomAniObjesiModalDataTable.Rows.Add(tableRow);
                    if (artti)
                    {

                        artti = false;
                    }
                    else
                    {
                        counter++;
                    }

                }
                paramAniObjesiIdArray.Value = aniObjesiIdStr.Length > 0 ? aniObjesiIdStr.Substring(1, aniObjesiIdStr.Length - 1) : string.Empty;
                paramAniObjesiAdetArray.Value = aniObjesiAdetStr.Length > 0 ? aniObjesiAdetStr.Substring(1, aniObjesiAdetStr.Length - 1) : string.Empty;
            }
            else
            {
                //paramAniObjesiIdArray.Value = string.Empty;
                //paramAniObjesiAdetArray.Value = string.Empty;
            }

            UtilityHelper.ScriptCalistir("ArrayDoldur();");
            UtilityHelper.ScriptCalistir("AniObjesiModal();");
        }
        /// <summary>
        /// ilk olarak bu Faaliyet ve KatilimciId için AniObjesiDagitim_Table'daki kayitlari al (list1)
        /// sonra
        ///     id ve adet arraylerini al (list2)
        ///     list1de olup da list2de olmayan kayitlari sil
        ///     list1de ve list2de olan kayitlari list2 adetinde göre update et
        ///     list1de olmayip lis2de olanlari insert et
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SecilenleriKaydetBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "SecilenleriKaydetBtn_Click", () =>
            {
                int activityId = ModalActivityId(), personId = paramFaaliyetKatilimciIdLbl.Value.ConvertToInt();
                var quantities = new Dictionary<int, int>();
                string[] ids = string.IsNullOrWhiteSpace(paramAniObjesiIdArray.Value) ? new string[0] : paramAniObjesiIdArray.Value.Split(',');
                string[] amounts = string.IsNullOrWhiteSpace(paramAniObjesiAdetArray.Value) ? new string[0] : paramAniObjesiAdetArray.Value.Split(',');
                if (ids.Length != amounts.Length || ids.Length > 500) throw new MtsInputException("Anı objesi seçimlerini yeniden açıp kontrol edin.");
                for (int i = 0; i < ids.Length; i++)
                {
                    int id, quantity;
                    if (!int.TryParse(ids[i], out id) || id <= 0 || !int.TryParse(amounts[i], out quantity) || quantity < 0 || quantities.ContainsKey(id))
                        throw new MtsInputException("Anı objesi adetlerini sıfır veya pozitif tam sayı olarak girin.");
                    quantities.Add(id, quantity);
                }
                RequireActivity(Writer.SaveObjects(activityId, personId, quantities, ViewState["ObjectVersion"] as string));
                ViewState["ObjectVersion"] = null;
                FinishActivity("Stoksuz anı objesi seçimleri kaydedildi.", () => { RefreshActivity(); CloseModals(); });
            });
        }
        protected void GetirilenAniObjesiKaydetBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "GetirilenAniObjesiKaydetBtn_Click", () =>
            {
                RequireActivity(Writer.SaveReceived(ModalActivityId(), paramFaaliyetKatilimciIdLbl.Value.ConvertToInt(),
                    GetirilenAniObjesiTxt.Text, ViewState["ReceivedVersion"] as string));
                ViewState["ReceivedVersion"] = null;
                FinishActivity("Getirilen anı objesi bilgisi kaydedildi.", () => { RefreshActivity(); CloseModals(); });
            });
        }
        protected void StokluAniObjesiKaydetBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "StokluAniObjesiKaydetBtn_Click", () =>
            {
                RequireActivity(Writer.Deliver(ModalActivityId(), paramFaaliyetKatilimciIdLbl.Value.ConvertToInt(),
                    DepoDDL.SelectedValue.ConvertToInt(), StokluAniObjesiDDL.SelectedValue.ConvertToInt(),
                    PositiveQuantity(StokluAdetTxt.Text), ViewState["StockVersion"] as string));
                ViewState["StockVersion"] = null;
                FinishActivity("Anı objesi verildi ve depo stoku güncellendi.", () => { RefreshActivity(); CloseModals(); });
            });
        }
        protected void StokluAniObjesiIadeEtBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "StokluAniObjesiIadeEtBtn_Click", () =>
            {
                RequireActivity(Writer.Return(CurrentActivityId(), paramStokluAniObjesiDagitimIdLbl.Value.ConvertToInt(),
                    PositiveQuantity(IadeEdilecekAdetTxt.Text), ViewState["ReturnVersion"] as string));
                ViewState["ReturnVersion"] = null;
                FinishActivity("Anı objesi çıkış deposuna iade edildi.", () => { RefreshActivity(); CloseModals(); });
            });
        }
        protected void StokluAniObjesiDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "StokluAniObjesiDDL_SelectedIndexChanged", () =>
            {
                DepoDDLDoldur(); CaptureStockVersion();
            });
        }
        protected void DepoDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "DepoDDL_SelectedIndexChanged", () =>
            {
                CaptureStockVersion();
            });
        }



        private string AniObjeleriniGetir(int katilimciId, int faaliyetId, string stokluMu)
        {
            AniObjesiDagitim aniobjesiDagitim = new AniObjesiDagitim();
            DataTable dataTable = new Model.Services.MTS.AniObjesiDagitimService(LogContext).GetByParticipantActivity(katilimciId, faaliyetId, stokluMu);
            string aniObjeleri = string.Empty;

            if (dataTable != null)
            {
                if (dataTable != null)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        int aniObjesiDagitimId = row["AniObjesiDagitimId"].ReturnZeroIfNull().ConvertToInt();
                        int depoId = row["CikisDepoId"].ReturnZeroIfNull().ConvertToInt();
                        string aniObjesi = System.Web.HttpUtility.HtmlEncode(row["Adi"].ReturnEmptyIfNull().ToString());
                        string adet = row["Adet"].ReturnZeroIfNull().ToString();
                        if (stokluMu.Equals(ProjeConstants.MTS_ANIOBJESISTOKLU))
                        {
                            aniObjeleri += !string.IsNullOrEmpty(aniObjesi) ? "<br> * "
                                               + "<a href='#' class='btn btn-link' onclick=StokluAniObjesiDuzenleClick(" + aniObjesiDagitimId + ")>" + aniObjesi + " (" + adet + "), " + "</a>"
                                               : string.Empty;
                        }
                        else
                        {
                            aniObjeleri += !string.IsNullOrEmpty(aniObjesi) ? "<br> * " + aniObjesi + "(" + adet + "), " : string.Empty;
                        }
                        //aniObjeleri += !string.IsNullOrEmpty(aniObjesi) ? "<br> * " + aniObjesi + "(" + adet + "), " : string.Empty;
                    }
                }
            }
            return aniObjeleri;
        }
        private string GetirilenAniObjeleriniGetir(int katilimciId, int faaliyetId)
        {
            AniObjesiDagitim aniobjesiDagitim = new AniObjesiDagitim();
            string aniObjeleri = new Model.Services.MTS.AniObjesiDagitimService(LogContext).GetGetirilenText(katilimciId, faaliyetId);
            return System.Web.HttpUtility.HtmlEncode(aniObjeleri ?? string.Empty);
        }

        protected void DisIrtibatCikarBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "DisIrtibatCikarBtn_Click", () =>
            {
                RequireActivity(Writer.SetContact(CurrentActivityId(), ActivityVersion, 0));
                FinishActivity("İrtibat kişisi kaldırıldı.", ReloadActivity);
            });
        }
        #endregion
        #region Takvim Daveti
        private string TakvimDavetiBtnOlustur(int katilimId,string takvimDaveti, int katilimciId, string eposta)
        {
            if (string.IsNullOrWhiteSpace(eposta)) return "E-posta adresi yok";
            if (takvimDaveti == ProjeConstants.MTSTAKVIMDAVETIYESI_GONDERILDI) return "Davet gönderildi";
            string script = "TakvimDavetiyesiModalAc(" + katilimId + "," + FaaliyetIdQS.ConvertToInt() + "," + katilimciId + ","
                + new JavaScriptSerializer().Serialize(eposta) + ");return false;";
            return "<a href='#' class='btn btn-outline-info' onclick=\"" + System.Web.HttpUtility.HtmlAttributeEncode(script) + "\">Davet gönder</a>";
        }

        protected void TakvimDavetiGonderNowBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "TakvimDavetiGonderNowBtn_Click", () =>
            {
                int activityId = ModalActivityId();
                int participationId = paramFaaliyetKatilimIdLbl.Value.ConvertToInt();
                var participation = new FaaliyetKatilimService(LogContext).GetById(participationId);
                if (participation == null || participation.FaaliyetId != activityId) throw new MtsInputException("Katılım kaydı bulunamadı. Listeyi yenileyin.");
                if (participation.TakvimDaveti == ProjeConstants.MTSTAKVIMDAVETIYESI_GONDERILDI)
                    throw new MtsInputException("Bu katılımcıya davet gönderilmiş. Tekrar göndermeden önce kontrol edin.");
                var activity = new FaaliyetService(LogContext).GetById(activityId);
                if (activity == null || activity.AcikTarih) throw new MtsInputException("Önce faaliyete kesin bir tarih verin.");
                bool sent = false;
                try
                {
                    TakvimDavetiHazirla(activityId, participationId, participation.KatilimciId, ProjeConstants.KAYDET);
                    sent = true;
                    RefreshActivity(); CloseModals();
                    Messages.Success("Takvim daveti gönderildi.");
                }
                catch (MtsInputException) when (!sent) { throw; }
                catch (Exception error)
                {
                    PortalTechnicalLog.WriteError(LogContext, "MTS.Faaliyet.Invitation", error);
                    Messages.Warning((sent ? "Takvim daveti gönderildi; ekran yenilenemedi. " : "Takvim daveti veya gönderim durumu tamamlanamadı. ")
                        + "Yeniden göndermeden önce kontrol edin. Başvuru kodu: " + LogContext.OperationId.ToString("N"));
                }
            });
        }

        private void TakvimDavetiHazirla(int faaliyetId,int katilimId, int katilimciId, string islemTipi)
        {
            var activity = new FaaliyetService(LogContext).GetById(faaliyetId);
            var participation = new FaaliyetKatilimService(LogContext).GetById(katilimId);
            if (activity == null || participation == null || participation.FaaliyetId != faaliyetId || participation.KatilimciId != katilimciId)
                throw new MtsInputException("Katılım kaydı bu faaliyete ait değil. Listeyi yenileyin.");
            var person = new KisiService(LogContext).GetById(participation.KatilimciId);
            if (person == null || string.IsNullOrWhiteSpace(person.EPosta)) throw new MtsInputException("Katılımcının e-posta adresi yok. Kişi bilgilerini kontrol edin.");
            if (activity.AcikTarih && islemTipi == ProjeConstants.KAYDET) throw new MtsInputException("Açık tarihli faaliyet için takvim daveti gönderilemez.");
            MtsActivityNotificationService.Send(activity, person.EPosta, activity.AcikTarih || activity.FaaliyetDurumu == ProjeConstants.FAALIYET_DURUMU_IPTALEDILDI_INT);
            // SMTP has succeeded; a subsequent database error must not be described as unsent mail.
            try { RequireActivity(Writer.MarkInvitation(faaliyetId, katilimId, MtsContactWriteService.Version(activity))); }
            catch (Exception error) { throw new InvalidOperationException("Takvim daveti gönderildi; gönderim durumu kaydedilemedi. Yeniden göndermeden önce kontrol edin.", error); }
        }

        private void KatilimcilaraTakvimDavetiGonder(Faaliyet faaliyet)
        {
            List<FaaliyetKatilim> list = new Model.Services.MTS.FaaliyetKatilimService(LogContext).GetByFaaliyetId(faaliyet.Id);
            foreach (var item in list)
            {
                if (string.IsNullOrEmpty(item.TakvimDaveti) || item.TakvimDaveti.Equals(ProjeConstants.MTSTAKVIMDAVETIYESI_GONDERILMEDI))
                {

                }
                else
                {
                    TakvimDavetiHazirla(item.FaaliyetId, item.Id, item.KatilimciId, ProjeConstants.GUNCELLE);
                }
            }

        }
        #endregion

        #region timer
        protected void RefreshTimer_Tick(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "RefreshTimer_Tick", () =>
            {
                var current = new FaaliyetService(LogContext).GetById(FaaliyetIdQS.ConvertToInt());
                if (current == null)
                {
                    DisableComponents(); CloseModals(); StopTimer();
                    Messages.Warning("Faaliyet artık bulunmuyor. Faaliyet listesinden devam edin.");
                    return;
                }
                var snapshot = new TempFaaliyet(current);
                if (!snapshot.Equals(TempFaaliyetQS))
                {
                    DisableComponents(); CloseModals(); StopTimer();
                    AddRefreshLink(current.Id, TopBarDiv, "Faaliyet bilgileri değişti. Sayfayı yenilemek için tıklayın.", snapshot);
                    KatilimciBilgileriniDoldur(current, false);
                    return;
                }
                var latest = GetDataList(false).Select(x => x.KatilimciId + " " + x.ObjectState).ToList();
                // Compare content, independent of action buttons that may be hidden in a stale form.
                var before = KatilimciListQS ?? new List<string>();
                if (before.Except(latest).Any() || latest.Except(before).Any())
                {
                    DisableComponents(); CloseModals(); StopTimer();
                    AddRefreshLink(current.Id, TopBarDiv, "Katılımcı veya anı objesi bilgileri değişti. Listeyi yenilemek için tıklayın.", snapshot);
                    KatilimciBilgileriniDoldur(current, false);
                }
                lblTime.Text = DateTime.Now.ToString("HH:mm ss");
            });
        }

        private void CloseModals()
        {
            UtilityHelper.ScriptCalistir("CloseModals();");
        }

        private void StopTimer()
        {
            RefreshTimer.Enabled = false;
        }

        private void AddRefreshLink(int faaliyetId, HtmlGenericControl divName, string message, TempFaaliyet tempFaaliyetSonHali)
        {
            HyperLink refreshLink = HyperLinkGetir(faaliyetId, message);

            divName.Controls.Add(refreshLink);

        }

        private void DisableComponents()
        {
            ViewState["ActivityStale"] = true;
            FaaliyetSilBtn.Visible = KatilimciEkleBtn.Visible = false;
            FaaliyetTipiDDL.Enabled = false;
            FaaliyetAmaciDDL.Enabled = false;
            BaslangicTarihiTxt.Enabled = false;
            BasSaatDDL.Enabled = false;
            BitisTarihiTxt.Enabled = false;
            BitSaatDDL.Enabled = false;
            FaaliyetYeriTxt.Enabled = false;
            FaaliyetDurumuDDL.Enabled = false;
            FaaliyetKonusuTxt.Enabled = false;
            TumGunChk.Enabled = false;
            AcikTarihChk.Enabled = false;
            AciklamaTxt.Enabled = false;
            KaydetBtn.Visible = false;
            GuncelleBtn.Visible = false;
            DisIrtibatCikarBtn.Visible = false;
        }
        private HyperLink HyperLinkGetir(int faaliyetId, string message)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path);
            HyperLink hyperLink = new HyperLink
            {
                Text = message,
                CssClass = "form-control btn btn-warning fw-bold",
            };

            if (faaliyetId > 0)
            {
                string linkUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_FAALIYET_GIRIS + "?FaaliyetId=" + faaliyetId;

                hyperLink.NavigateUrl = linkUrl;
            }
            return hyperLink;
        }
        #endregion

        #region classes
        private class KatilimciListItem
        {
            public string ObjectState { get; set; }
            public string Sirano { get; set; }
            public string KatilimciId { get; set; }
            public string KatilimciTipi { get; set; }
            public string AdiSoyadi { get; set; }
            public string Adi { get; set; }
            public string Soyadi { get; set; }
            public string Kurumu { get; set; }
            public string AniObjesi { get; set; }
            public string AniObjesiStoklu { get; set; }
            public string AniObjesiStoksuz { get; set; }
            public string GetirilenAniObjesi { get; set; }
            public string TakvimDavetiBtn{ get; set; }
            public string EPosta{ get; set; }

            public string KisiKarti { get; set; }
            public string Cikar { get; set; }
            public string KatilimciSec { get; set; }
            public string IrtibatSec { get; set; }
            public bool RandevuKisiti { get; set; } = false;

        }
        [Serializable]
        private class TempFaaliyet : IEquatable<TempFaaliyet>
        {
            public TempFaaliyet(Faaliyet faaliyet)
            {
                Id = faaliyet.Id;
                FaaliyetTipi = faaliyet.FaaliyetTipi;
                FaaliyetAmaciId = faaliyet.FaaliyetAmaciId;
                FaaliyetKonusu = faaliyet.FaaliyetKonusu;
                FaaliyetYeriStr = faaliyet.FaaliyetYeriStr;
                FaaliyetDurumu = faaliyet.FaaliyetDurumu;
                TumGun = faaliyet.TumGun;
                AcikTarih = faaliyet.AcikTarih;
                DisIrtibatId = faaliyet.DisIrtibatId;
                BaslangicTarihi = faaliyet.BaslangicTarihi;
                BitisTarihi = faaliyet.BitisTarihi;
                BaslangicSaati = faaliyet.BaslangicSaati;
                BitisSaati = faaliyet.BitisSaati;
                Aciklama = faaliyet.Aciklama;
                YoneticiNotu = faaliyet.YoneticiNotu;
                TakvimeIslendi = faaliyet.TakvimeIslendi;
            }

            public int Id { get; set; }
            public string FaaliyetTipi { get; set; }
            public int FaaliyetAmaciId { get; set; }
            public string FaaliyetKonusu { get; set; }
            public string FaaliyetYeriStr { get; set; }
            public int FaaliyetDurumu { get; set; }
            public bool TumGun { get; set; }
            public bool AcikTarih { get; set; }
            public int DisIrtibatId { get; set; }
            public DateTime BaslangicTarihi { get; set; }
            public DateTime BitisTarihi { get; set; }
            public string BaslangicSaati { get; set; }
            public string BitisSaati { get; set; }
            public string Aciklama { get; set; }
            public string YoneticiNotu { get; set; }
            public bool TakvimeIslendi{ get; set; }
            public override bool Equals(object obj)
            {
                var other = obj as TempFaaliyet;

                if (other == null)
                    return false;

                if (FaaliyetTipi != other.FaaliyetTipi
                    || FaaliyetAmaciId != other.FaaliyetAmaciId
                    || FaaliyetKonusu != other.FaaliyetKonusu
                    || !string.Equals(FaaliyetYeriStr, other.FaaliyetYeriStr, StringComparison.Ordinal)
                    || TumGun != other.TumGun
                    || AcikTarih != other.AcikTarih
                    || FaaliyetDurumu != other.FaaliyetDurumu
                    || TakvimeIslendi != other.TakvimeIslendi
                    || DisIrtibatId != other.DisIrtibatId
                    || BaslangicTarihi != other.BaslangicTarihi
                    || BitisTarihi != other.BitisTarihi
                    || BaslangicSaati != other.BaslangicSaati
                    || BitisSaati != other.BitisSaati
                    || Aciklama != other.Aciklama
                    || YoneticiNotu != other.YoneticiNotu)
                    return false;

                return true;
            }

            public bool Equals(TempFaaliyet other)
            {
                return !(other is null) &&
                       FaaliyetTipi == other.FaaliyetTipi &&
                       FaaliyetAmaciId == other.FaaliyetAmaciId &&
                       FaaliyetKonusu == other.FaaliyetKonusu &&
                       FaaliyetYeriStr == other.FaaliyetYeriStr &&
                       FaaliyetDurumu == other.FaaliyetDurumu &&
                       TumGun == other.TumGun &&
                       AcikTarih == other.AcikTarih &&
                       DisIrtibatId == other.DisIrtibatId &&
                       BaslangicTarihi == other.BaslangicTarihi &&
                       BitisTarihi == other.BitisTarihi &&
                       BaslangicSaati == other.BaslangicSaati &&
                       BitisSaati == other.BitisSaati &&
                       Aciklama == other.Aciklama &&
                       YoneticiNotu == other.YoneticiNotu &&
                       TakvimeIslendi == other.TakvimeIslendi;
            }

            public override int GetHashCode()
            {
                int hashCode = 477006145;
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FaaliyetTipi);
                hashCode = hashCode * -1521134295 + FaaliyetAmaciId.GetHashCode();
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FaaliyetKonusu);
                hashCode = hashCode * -1521134295 + FaaliyetYeriStr.GetHashCode();
                hashCode = hashCode * -1521134295 + FaaliyetDurumu.GetHashCode();
                hashCode = hashCode * -1521134295 + TumGun.GetHashCode();
                hashCode = hashCode * -1521134295 + AcikTarih.GetHashCode();
                hashCode = hashCode * -1521134295 + DisIrtibatId.GetHashCode();
                hashCode = hashCode * -1521134295 + BaslangicTarihi.GetHashCode();
                hashCode = hashCode * -1521134295 + BitisTarihi.GetHashCode();
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(BaslangicSaati);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(BitisSaati);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Aciklama);
                return hashCode;
            }

            public static bool operator ==(TempFaaliyet left, TempFaaliyet right)
            {
                return EqualityComparer<TempFaaliyet>.Default.Equals(left, right);
            }

            public static bool operator !=(TempFaaliyet left, TempFaaliyet right)
            {
                return !(left == right);
            }
        }
        #endregion


    }
}
