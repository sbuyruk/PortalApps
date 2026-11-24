using Microsoft.SharePoint;
using Model.Ortak;
using Model.Services.TBYS;
using Model.TBYS;
using System;
using Utility.Logging;
using TBYS_WebParts.Logging;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;


namespace TBYS_WebParts.TasinmazResimleriWP
{
    [ToolboxItemAttribute(false)]
    public partial class TasinmazResimleriWP : WebPart
    {
        private const string TapuSenediDosyaKoku = "TapuSenedi";
        private const string VakiflarTasinmazBildirimiDosyaKoku = "VakiflarTasinmazBildirimi";

        private PortalLogContext LoggingContext
        {
            get
            {
                return PortalLogContext.FromCurrentRequest("TBYS", "TasinmazResimleriWP");
            }
        }

        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public TasinmazResimleriWP()
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!Page.IsPostBack) TbysWebPartLog.RecordOpened(LoggingContext);
            try
            {
                base.OnLoad(e);
            }
            catch (System.Threading.ThreadAbortException)
            {
                throw;
            }
            catch (Exception error)
            {
                TbysWebPartLog.ReportError(LoggingContext, "TBYS.TasinmazResimleriWP.Page_Load", error);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            SaveBtn.Text = "Resimleri/Belgeleri Kaydet";
            FileUpload5.Attributes["onchange"] = "readURL(this,'Image6')";
            this.ChromeType = PartChromeType.None;
        }
        public string SPImageListName { get; private set; }
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
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!string.IsNullOrEmpty(MesajQS))
            //{
            //    MesajQS = string.Empty;
            //    ResimleriDoldur();
            //}
            if (!Page.IsPostBack)
            {
                if (MesajQS.ToLower().Equals("true"))
                    Messages.Success("Kaydetme işlemi tamamlandı.", 2000);
                else if (MesajQS.ToLower().Equals("false"))
                    Messages.Error("Kaydetme işlemi tamamlanamadı.");
                ResimleriDoldur();
            }

        }
        private void ResimleriDoldur()
        {
            Tasinmaz tasinmaz = new Tasinmaz();
            tasinmaz = new TasinmazService(LoggingContext).Select(TasinmazIdQS.ConvertToInt());
            if (tasinmaz != null)
            {
                EnvanterdeMiQS = tasinmaz.EnvanterdeMi.ToString();
                AdiLbl.CssClass = "d-block fs-6 fw-normal text-muted mt-1";
                AdiLbl.Text = HttpUtility.HtmlEncode(string.Format(
                    "Adres: {0} | İlçe: {1} | İl: {2}",
                    string.IsNullOrWhiteSpace(tasinmaz.Adres) ? "Belirtilmemiş" : tasinmaz.Adres,
                    string.IsNullOrWhiteSpace(tasinmaz.Ilcesi) ? "Belirtilmemiş" : tasinmaz.Ilcesi,
                    string.IsNullOrWhiteSpace(tasinmaz.Ili) ? "Belirtilmemiş" : tasinmaz.Ili));

                string newUrl = UtilityHelper.TbysURLGetir() + "/" + ProjeConstants.RESIMLER_TASINMAZ + "/";

                string imgUrl1 = newUrl + (!string.IsNullOrEmpty(tasinmaz.TasinmazFoto) ? tasinmaz.TasinmazFoto : ProjeConstants.PARAM_TASINMAZ_TASINMAZFOTO) + ".jpg";
                string imgUrl2 = newUrl + (!string.IsNullOrEmpty(tasinmaz.TasinmazFoto1) ? tasinmaz.TasinmazFoto1 : ProjeConstants.PARAM_TASINMAZ_TASINMAZFOTO1) + ".jpg";
                string imgUrl3 = newUrl + (!string.IsNullOrEmpty(tasinmaz.TasinmazFoto2) ? tasinmaz.TasinmazFoto2 : ProjeConstants.PARAM_TASINMAZ_TASINMAZFOTO2) + ".jpg";
                string imgUrl4 = newUrl + (!string.IsNullOrEmpty(tasinmaz.TahkikatFoto) ? tasinmaz.TahkikatFoto : ProjeConstants.PARAM_TASINMAZ_TAHKIKATFOTO) + ".jpg";
                string imgUrl5 = newUrl + (!string.IsNullOrEmpty(tasinmaz.KrokiFoto) ? tasinmaz.KrokiFoto : ProjeConstants.PARAM_TASINMAZ_KROKIFOTO) + ".jpg";
                string imgUrl6 = newUrl + (!string.IsNullOrEmpty(tasinmaz.TapuFoto) ? tasinmaz.TapuFoto : ProjeConstants.PARAM_TASINMAZ_TAPUFOTO) + ".jpg";
                string imgUrl7 = newUrl + (!string.IsNullOrEmpty(tasinmaz.TasinmazFoto3) ? tasinmaz.TasinmazFoto3 : ProjeConstants.PARAM_TASINMAZ_TASINMAZFOTO3) + ".jpg";
                string imgUrl8 = newUrl + (!string.IsNullOrEmpty(tasinmaz.TasinmazFoto4) ? tasinmaz.TasinmazFoto4 : ProjeConstants.PARAM_TASINMAZ_TASINMAZFOTO4) + ".jpg";

                Image1.ImageUrl = imgUrl1;
                Image2.ImageUrl = imgUrl2;
                Image3.ImageUrl = imgUrl3;
                Image4.ImageUrl = imgUrl4;
                Image5.ImageUrl = imgUrl5;
                Image6.ImageUrl = imgUrl6;
                Image7.ImageUrl = imgUrl7;
                Image8.ImageUrl = imgUrl8;

                // onerror için base URL'i client-side'a gönder
                TasinmazResimBaseUrl.Value = newUrl;

                PDFGoster(tasinmaz.Id);
                GuncellemeTarihiLbl.Text = tasinmaz.DegistirmeTarihi != null ? "Son Güncelleme: " + tasinmaz.DegistirmeTarihi.ToString("dd.MM.yyyy HH:mm") : string.Empty;
                GuncelleyenLbl.Text = tasinmaz.Degistiren != null ? "Son Güncelleyen: " + tasinmaz.Degistiren : string.Empty;
            }
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_HOME);
        }
        protected void BackBtn_Click(object sender, EventArgs e)
        {
            string hedefSayfa = EnvanterdeMiQS.Equals(ProjeConstants.MULKIYETTE_OLMAYAN_TASINMAZ.ToString())
                ? ProjeConstants.PAGE_MULKIYETIOLMAYANTASINMAZ_GIRIS
                : ProjeConstants.PAGE_TASINMAZ_GIRIS;
            RedirectToPage(hedefSayfa + "?DestinationApp=TD&TasinmazId=" + HttpUtility.UrlEncode(TasinmazIdQS) +
                "&EnvanterdeMi=" + HttpUtility.UrlEncode(EnvanterdeMiQS));
        }

        protected void SaveBtn_Click(object sender, EventArgs e)
        {
            TasinmazService service = new TasinmazService(LoggingContext);
            Tasinmaz tasinmaz = service.Select(TasinmazIdQS.ConvertToInt());
            if (tasinmaz == null)
            {
                Messages.Error("Taşınmaz bulunamadı.");
                return;
            }

            bool fotograflarKaydedildi = FotograflariKaydet(tasinmaz, service);
            bool belgelerKaydedildi = PDFKaydet(tasinmaz.Id);
            if (fotograflarKaydedildi && belgelerKaydedildi)
            {
                RedirectToPage(ProjeConstants.PAGE_TASINMAZ_RESIMLER + "?Mesaj=true&DestinationApp=TD&TasinmazId=" + tasinmaz.Id +
                    "&EnvanterdeMi=" + HttpUtility.UrlEncode(EnvanterdeMiQS));
                return;
            }

            ResimleriDoldur();
        }

        private bool FotograflariKaydet(Tasinmaz tasinmaz, TasinmazService service)
        {
            int tasinmazId = tasinmaz.Id;
            Dictionary<string, string> yapilanYuklemeler = new Dictionary<string, string>();
            ExceptionHelper hatalar = new ExceptionHelper();
            FotoYukle(tasinmazId, FileUpload0, nameof(Tasinmaz.TasinmazFoto), yapilanYuklemeler, hatalar);
            FotoYukle(tasinmazId, FileUpload1, nameof(Tasinmaz.TasinmazFoto1), yapilanYuklemeler, hatalar);
            FotoYukle(tasinmazId, FileUpload2, nameof(Tasinmaz.TasinmazFoto2), yapilanYuklemeler, hatalar);
            FotoYukle(tasinmazId, FileUpload3, nameof(Tasinmaz.TahkikatFoto), yapilanYuklemeler, hatalar);
            FotoYukle(tasinmazId, FileUpload4, nameof(Tasinmaz.KrokiFoto), yapilanYuklemeler, hatalar);
            FotoYukle(tasinmazId, FileUpload5, nameof(Tasinmaz.TapuFoto), yapilanYuklemeler, hatalar);
            FotoYukle(tasinmazId, FileUpload6, nameof(Tasinmaz.TasinmazFoto3), yapilanYuklemeler, hatalar);
            FotoYukle(tasinmazId, FileUpload7, nameof(Tasinmaz.TasinmazFoto4), yapilanYuklemeler, hatalar);

            if (hatalar.HasException())
                TbysWebPartLog.ReportErrors(LoggingContext, "TBYS.TasinmazResimleriWP.FotoYukle", hatalar.Exceptions);

            List<string> eskiDosyaAdlari;
            try
            {
                eskiDosyaAdlari = service.UpdatePhotos(tasinmazId, yapilanYuklemeler);
                if (eskiDosyaAdlari == null)
                    throw new InvalidOperationException("Fotoğraf bilgileri kaydedilemedi.");
            }
            catch (Exception ex)
            {
                if (ex is System.Threading.ThreadAbortException) throw;
                TbysWebPartLog.ReportError(LoggingContext, "TBYS.TasinmazResimleriWP.FotograflariKaydet", ex);
                KullanilmayanFotoDosyalariniTemizle(service, yapilanYuklemeler.Values);
                return false;
            }

            KullanilmayanFotoDosyalariniTemizle(service, eskiDosyaAdlari);
            string adres = HttpUtility.HtmlEncode(tasinmaz.Adres + " " + tasinmaz.Ili + "/" + tasinmaz.Ilcesi);
            foreach (string fotoAdi in yapilanYuklemeler.Values)
                BildirimEPostasiGonder("<b>" + adres + "</b> adresindeki taşınmazın fotoğrafı güncellendi. Dosya: <b>" + fotoAdi + ".jpg</b>");

            return !hatalar.HasException();
        }

        private void KullanilmayanFotoDosyalariniTemizle(TasinmazService service, ICollection<string> dosyaAdlari)
        {
            if (dosyaAdlari.Count == 0) return;

            try
            {
                service.RemoveUnusedPhotoFiles(dosyaAdlari, dosyaAdi => FotoDosyasiniGeriDonusumeTasi(dosyaAdi));
            }
            catch (Exception ex)
            {
                if (ex is System.Threading.ThreadAbortException) throw;
                TbysWebPartLog.WriteError(LoggingContext, "TBYS.TasinmazResimleriWP.KullanilmayanFotoDosyalariniTemizle", ex);
            }
        }

        private void FotoYukle(int tasinmazId, FileUpload yukleme, string alan,
            IDictionary<string, string> yapilanYuklemeler, ExceptionHelper hatalar)
        {
            if (!yukleme.HasFile) return;

            // A new upload must not overwrite the file currently referenced by any record.
            string dosyaAdi = tasinmazId + "_" + Guid.NewGuid().ToString("N");
            try
            {
                SPImageListName = ProjeConstants.RESIMLER_TASINMAZ;
                ExceptionHelper yuklemeHatalari = UtilityHelper.uploadFile2SP(yukleme, "", dosyaAdi, SPImageListName,
                    new ExceptionHelper(), ProjeConstants.RESIM_DIGER_EN, ProjeConstants.RESIM_DIGER_BOY, UtilityHelper.TbysURLGetir());
                if (!yuklemeHatalari.HasException())
                {
                    yapilanYuklemeler.Add(alan, dosyaAdi);
                    return;
                }

                foreach (Exception hata in yuklemeHatalari.Exceptions)
                    hatalar.Exceptions.Add(hata);
            }
            catch (Exception ex)
            {
                if (ex is System.Threading.ThreadAbortException) throw;
                hatalar.Exceptions.Add(ex);
            }
        }

        private void RedirectToPage(string pageUrl)
        {
            try
            {
                string currentUrl = Page.Request.Url.GetLeftPart(UriPartial.Path);
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + pageUrl;
                Page.Response.Redirect(newUrl);
            }
            catch (Exception ex)
            {
                if (ex is System.Threading.ThreadAbortException) throw;
                TbysWebPartLog.WriteError(LoggingContext, "TBYS.TasinmazResimleriWP.RedirectToPage", ex);
                TbysWebPartLog.ReportError(LoggingContext, "TBYS.TasinmazResimleriWP.RedirectToPage", ex);
            }
        }

        /// <summary>
        /// Tasinmaz resim/belge islemleri için bildirim e-postasi gönderir
        /// </summary>
        private void BildirimEPostasiGonder(string islemAciklamasi)
        {
            try
            {
                string currentUser = UtilityHelper.GetCurrentUserName();
                string from = "tbys@tskgv.org.tr";
                string to = "zcalis@tskgv.org.tr";
                if (!string.IsNullOrEmpty(currentUser))
                {
                    if (to.Equals(currentUser + "@tskgv.org.tr"))
                    {
                        return; //zcalis 'e kendi islemi için mail gitmesin
                    }
                }
                string subject = "Taşınmaz Resim/Belge İşlemi - Taşınmaz Id: " + TasinmazIdQS;
                string body = "<p>Taşınmaz Id: <b>" + TasinmazIdQS + "</b> numaralı taşınmazda "
                    + "<strong>" + currentUser + "</strong> tarafından aşağıdaki işlem gerçekleştirilmiştir:</p>"
                    + "<p>" + islemAciklamasi + "</p>" ;
                string smtpAdresi = UtilityHelper.ParametreDegeriSorgula(ProjeConstants.PARAM_SMTP_ADRESI_LBL, LoggingContext);
                MailHelper.EPostaGonderVeyaHataVer(from, to, subject, body, smtpAdresi ?? ProjeConstants.PARAM_ALTERNATIVE_SMTP_IP_ADRESI);
            }
            catch (Exception ex)
            {
                if (ex is System.Threading.ThreadAbortException) throw;
                TbysWebPartLog.WriteError(LoggingContext, "TBYS.TasinmazResimleriWP.BildirimEPostasiGonder", ex);
                Messages.Warning("İşlem tamamlandı ancak bildirim e-postası gönderilemedi. Başvuru kodu: " + LoggingContext.OperationId.ToString("N"));
            }
        }

        protected void ResimSil1Btn_Click(object sender, EventArgs e)
        {
            ResmiSil(nameof(Tasinmaz.TasinmazFoto), Image1, tasinmaz => tasinmaz.TasinmazFoto);
        }
        protected void ResimSil2Btn_Click(object sender, EventArgs e)
        {
            ResmiSil(nameof(Tasinmaz.TasinmazFoto1), Image2, tasinmaz => tasinmaz.TasinmazFoto1);
        }
        protected void ResimSil3Btn_Click(object sender, EventArgs e)
        {
            ResmiSil(nameof(Tasinmaz.TasinmazFoto2), Image3, tasinmaz => tasinmaz.TasinmazFoto2);
        }
        protected void ResimSil4Btn_Click(object sender, EventArgs e)
        {
            ResmiSil(nameof(Tasinmaz.TahkikatFoto), Image4, tasinmaz => tasinmaz.TahkikatFoto);
        }
        protected void ResimSil5Btn_Click(object sender, EventArgs e)
        {
            ResmiSil(nameof(Tasinmaz.KrokiFoto), Image5, tasinmaz => tasinmaz.KrokiFoto);
        }
        protected void ResimSil6Btn_Click(object sender, EventArgs e)
        {
            ResmiSil(nameof(Tasinmaz.TapuFoto), Image6, tasinmaz => tasinmaz.TapuFoto);
        }
        protected void ResimSil7Btn_Click(object sender, EventArgs e)
        {
            ResmiSil(nameof(Tasinmaz.TasinmazFoto3), Image7, tasinmaz => tasinmaz.TasinmazFoto3);
        }
        protected void ResimSil8Btn_Click(object sender, EventArgs e)
        {
            ResmiSil(nameof(Tasinmaz.TasinmazFoto4), Image8, tasinmaz => tasinmaz.TasinmazFoto4);
        }

        private void ResmiSil(string alan, Image resim, Func<Tasinmaz, string> fotoAdiGetir)
        {
            Guid geriDonusumId = Guid.Empty;
            try
            {
                TasinmazService service = new TasinmazService(LoggingContext);
                Tasinmaz tasinmaz = service.Select(TasinmazIdQS.ConvertToInt());
                if (tasinmaz == null)
                {
                    Messages.Error("Taşınmaz bulunamadı.");
                    return;
                }

                string fotoAdi = fotoAdiGetir(tasinmaz);
                if (string.IsNullOrWhiteSpace(fotoAdi) || string.Equals(fotoAdi, alan, StringComparison.OrdinalIgnoreCase))
                {
                    Messages.Info("Silinecek fotoğraf bulunamadı.", 2000);
                    return;
                }

                string kitaplikUrl = UtilityHelper.TbysURLGetir() + "/" + ProjeConstants.RESIMLER_TASINMAZ + "/";
                if (resim.ImageUrl != kitaplikUrl + fotoAdi + ".jpg" ||
                    !service.RemovePhoto(tasinmaz.Id, alan, fotoAdi, dosyaAdi => geriDonusumId = FotoDosyasiniGeriDonusumeTasi(dosyaAdi)))
                {
                    Messages.Warning("Fotoğraf değişmiş olabilir. Sayfayı yenileyip tekrar deneyiniz.");
                    return;
                }

                resim.ImageUrl = kitaplikUrl + alan + ".jpg";
                Messages.Success("Resim taşınmazdan kaldırıldı.", 2000);
                BildirimEPostasiGonder("Resim taşınmazdan kaldırıldı. Dosya: <b>" + HttpUtility.HtmlEncode(fotoAdi) + ".jpg</b>");
            }
            catch (Exception ex)
            {
                if (ex is System.Threading.ThreadAbortException) throw;
                FotoDosyasiniGeriYukle(geriDonusumId);
                TbysWebPartLog.ReportError(LoggingContext, "TBYS.TasinmazResimleriWP.ResmiSil", ex);
            }
        }

        private static Guid FotoDosyasiniGeriDonusumeTasi(string dosyaAdi)
        {
            if (dosyaAdi != Path.GetFileName(dosyaAdi))
                throw new InvalidOperationException("Fotoğraf dosyasının adı geçersiz.");

            using (SPSite site = new SPSite(UtilityHelper.TbysURLGetir()))
            using (SPWeb web = site.OpenWeb())
            {
                SPList kitaplik = web.Lists[ProjeConstants.RESIMLER_TASINMAZ];
                string dosyaUrl = kitaplik.RootFolder.ServerRelativeUrl + "/" + Uri.EscapeDataString(dosyaAdi) + ".jpg";
                SPFile dosya = web.GetFile(dosyaUrl);
                return dosya != null && dosya.Exists ? dosya.Recycle() : Guid.Empty;
            }
        }

        private void FotoDosyasiniGeriYukle(Guid geriDonusumId)
        {
            if (geriDonusumId == Guid.Empty) return;

            try
            {
                using (SPSite site = new SPSite(UtilityHelper.TbysURLGetir()))
                using (SPWeb web = site.OpenWeb())
                    web.RecycleBin.Restore(new[] { geriDonusumId });
            }
            catch (Exception ex)
            {
                if (ex is System.Threading.ThreadAbortException) throw;
                TbysWebPartLog.WriteError(LoggingContext, "TBYS.TasinmazResimleriWP.FotoDosyasiniGeriYukle", ex);
            }
        }

        #region pdf yukle goster
        private bool PDFKaydet(int tasinmazId)
        {
            bool kaydedildi = BelgeKaydet(TapuSenediYukleFU, TapuSenediDosyaKoku, "Tapu Senedi", tasinmazId);
            kaydedildi &= BelgeKaydet(EmlakBeyaniYukleFU, ProjeConstants.DOSYA_EMLAKBEYAN_FORMU, "Emlak Beyanı", tasinmazId);
            kaydedildi &= BelgeKaydet(YapiKayitYukleFU, ProjeConstants.DOSYA_YAPIKAYIT_BELGESI, "Yapı Kayıt Belgesi", tasinmazId);
            kaydedildi &= BelgeKaydet(TapuKayitYukleFU, ProjeConstants.DOSYA_TAPUKAYIT_BELGESI, "Tapu Kayıt Belgesi", tasinmazId);
            kaydedildi &= BelgeKaydet(ImarDurumuYukleFU, ProjeConstants.DOSYA_IMARDURUMU_BELGESI, "İmar Durumu Belgesi", tasinmazId);
            kaydedildi &= BelgeKaydet(TasinmazRaporuYukleFU, ProjeConstants.DOSYA_TASINMAZ_RAPORU, "Taşınmaz Raporu", tasinmazId);
            kaydedildi &= BelgeKaydet(VakiflarTasinmazBildirimiYukleFU, VakiflarTasinmazBildirimiDosyaKoku, "Vakıflar Taşınmaz Bildirimi", tasinmazId);
            return kaydedildi;
        }

        private void PDFGoster(int tasinmazId)
        {
            try
            {
                BelgeGoster(TapuSenediDosyaKoku, tasinmazId, TapuSenediYukleFU, TapuSenediDosyaLnk, TapuSenediSilBtn);
                BelgeGoster(ProjeConstants.DOSYA_EMLAKBEYAN_FORMU, tasinmazId, EmlakBeyaniYukleFU, EmlakBeyaniDosyaLnk, EmlakBeyaniSilBtn);
                BelgeGoster(ProjeConstants.DOSYA_YAPIKAYIT_BELGESI, tasinmazId, YapiKayitYukleFU, YapiKayitDosyaLnk, YapiKayitSilBtn);
                BelgeGoster(ProjeConstants.DOSYA_TAPUKAYIT_BELGESI, tasinmazId, TapuKayitYukleFU, TapuKayitDosyaLnk, TapuKayitSilBtn);
                BelgeGoster(ProjeConstants.DOSYA_IMARDURUMU_BELGESI, tasinmazId, ImarDurumuYukleFU, ImarDurumuDosyaLnk, ImarDurumuSilBtn);
                BelgeGoster(ProjeConstants.DOSYA_TASINMAZ_RAPORU, tasinmazId, TasinmazRaporuYukleFU, TasinmazRaporuDosyaLnk, TasinmazRaporuSilBtn);
                BelgeGoster(VakiflarTasinmazBildirimiDosyaKoku, tasinmazId, VakiflarTasinmazBildirimiYukleFU, VakiflarTasinmazBildirimiDosyaLnk, VakiflarTasinmazBildirimiSilBtn);
            }
            catch (Exception ex)
            {
                if (ex is System.Threading.ThreadAbortException) throw;
                TbysWebPartLog.ReportError(LoggingContext, "TBYS.TasinmazResimleriWP.PDFGoster", ex);
            }
        }

        private bool BelgeKaydet(FileUpload yukleme, string dosyaKoku, string belgeAdi, int tasinmazId)
        {
            if (!yukleme.HasFile)
                return true;

            try
            {
                if (!string.Equals(Path.GetExtension(yukleme.FileName), ".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    Messages.Error(belgeAdi + " için PDF uzantılı bir dosya seçiniz.");
                    return false;
                }

                string dosyaAdi = dosyaKoku + tasinmazId + ".pdf";
                if (!UtilityHelper.UploadFileToSharePoint(yukleme, ProjeConstants.TBYSBELGELERI_LIB, dosyaAdi, LoggingContext))
                {
                    Messages.Error(belgeAdi + " yüklenemedi.");
                    return false;
                }

                Messages.Success(belgeAdi + " yüklendi.", 2000);
                BildirimEPostasiGonder(belgeAdi + " yüklendi. Dosya: <b>" + dosyaAdi + "</b>");
                return true;
            }
            catch (Exception ex)
            {
                if (ex is System.Threading.ThreadAbortException) throw;
                TbysWebPartLog.WriteError(LoggingContext, "TBYS.TasinmazResimleriWP.BelgeKaydet", ex);
                Messages.Error(belgeAdi + " yüklenemedi. Başvuru kodu: " + LoggingContext.OperationId.ToString("N"));
                return false;
            }
        }

        private void BelgeGoster(string dosyaKoku, int tasinmazId, FileUpload yukleme, HtmlAnchor baglanti, LinkButton silme)
        {
            string dosyaAdi = dosyaKoku + tasinmazId + ".pdf";
            string belgeKitapligiUrl = UtilityHelper.TbysBelgelerURLGetir();
            bool dosyaVar = UtilityHelper.DosyaVarMi(belgeKitapligiUrl, ProjeConstants.TBYSBELGELERI_LIB, dosyaAdi);
            if (dosyaVar)
            {
                baglanti.HRef = belgeKitapligiUrl + "/" + dosyaAdi;
                baglanti.Target = "_blank";
            }

            baglanti.Visible = dosyaVar;
            silme.Visible = dosyaVar;
            yukleme.Visible = !dosyaVar;
        }

        protected void TapuSenediSilBtn_Click(object sender, EventArgs e)
        {
            EkPdfSil(TapuSenediDosyaKoku, "Tapu Senedi", TapuSenediYukleFU, TapuSenediDosyaLnk, TapuSenediSilBtn);
        }

        protected void VakiflarTasinmazBildirimiSilBtn_Click(object sender, EventArgs e)
        {
            EkPdfSil(VakiflarTasinmazBildirimiDosyaKoku, "Vakıflar Taşınmaz Bildirimi", VakiflarTasinmazBildirimiYukleFU, VakiflarTasinmazBildirimiDosyaLnk, VakiflarTasinmazBildirimiSilBtn);
        }

        private void EkPdfSil(string dosyaKoku, string belgeAdi, FileUpload yukleme, HtmlAnchor baglanti, LinkButton silme)
        {
            try
            {
                string dosyaAdi = dosyaKoku + TasinmazIdQS.ConvertToInt() + ".pdf";
                if (!UtilityHelper.DeleteFileFromSharePointLib(ProjeConstants.PATH_TBYS_URL, ProjeConstants.TBYSBELGELERI_LIB, dosyaAdi, LoggingContext))
                {
                    Messages.Error(belgeAdi + " silinemedi.");
                    return;
                }

                baglanti.Visible = false;
                silme.Visible = false;
                yukleme.Visible = true;
                Messages.Success(belgeAdi + " silindi.", 2000);
                BildirimEPostasiGonder(belgeAdi + " silindi. Dosya: <b>" + dosyaAdi + "</b>");
            }
            catch (Exception ex)
            {
                if (ex is System.Threading.ThreadAbortException) throw;
                TbysWebPartLog.ReportError(LoggingContext, "TBYS.TasinmazResimleriWP.EkPdfSil", ex);
            }
        }

        protected void EmlakBeyaniSilBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string dosyaAdi = ProjeConstants.DOSYA_EMLAKBEYAN_FORMU + TasinmazIdQS + ".pdf";
                if (UtilityHelper.DeleteFileFromSharePointLib(ProjeConstants.PATH_TBYS_URL, ProjeConstants.TBYSBELGELERI_LIB, dosyaAdi, LoggingContext))
                {
                    Messages.Success(dosyaAdi + " Silindi", 2000);
                    EmlakBeyaniSilBtn.Visible = false;
                    EmlakBeyaniDosyaLnk.Visible = false;
                    EmlakBeyaniYukleFU.Visible = true;
                    BildirimEPostasiGonder("Emlak Beyan Formu silindi. Dosya: <b>" + dosyaAdi + "</b>");
                }
                else
                {
                    Messages.Error(dosyaAdi + " Silinemedi");
                }
            }
            catch (Exception ex)
            {
                if (ex is System.Threading.ThreadAbortException) throw;
                TbysWebPartLog.WriteError(LoggingContext, "TBYS.TasinmazResimleriWP.EmlakBeyaniSilBtn_Click", ex);
                TbysWebPartLog.ReportError(LoggingContext, "TBYS.TasinmazResimleriWP.EmlakBeyaniSilBtn_Click", ex);
            }
        }
        protected void YapiKayitSilBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string dosyaAdi = ProjeConstants.DOSYA_YAPIKAYIT_BELGESI + TasinmazIdQS + ".pdf";
                if (UtilityHelper.DeleteFileFromSharePointLib(ProjeConstants.PATH_TBYS_URL, ProjeConstants.TBYSBELGELERI_LIB, dosyaAdi, LoggingContext))
                {
                    Messages.Success(dosyaAdi + " Silindi", 2000);
                    YapiKayitSilBtn.Visible = false;
                    YapiKayitDosyaLnk.Visible = false;
                    YapiKayitYukleFU.Visible = true;
                    BildirimEPostasiGonder("Yapı Kayıt Belgesi silindi. Dosya: <b>" + dosyaAdi + "</b>");
                }
                else
                {
                    Messages.Error(dosyaAdi + " Silinemedi");
                }
            }
            catch (Exception ex)
            {
                if (ex is System.Threading.ThreadAbortException) throw;
                TbysWebPartLog.WriteError(LoggingContext, "TBYS.TasinmazResimleriWP.YapiKayitSilBtn_Click", ex);
                TbysWebPartLog.ReportError(LoggingContext, "TBYS.TasinmazResimleriWP.YapiKayitSilBtn_Click", ex);
            }
        }
        protected void TapuKayitSilBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string dosyaAdi = ProjeConstants.DOSYA_TAPUKAYIT_BELGESI + TasinmazIdQS + ".pdf";
                if (UtilityHelper.DeleteFileFromSharePointLib(ProjeConstants.PATH_TBYS_URL, ProjeConstants.TBYSBELGELERI_LIB, dosyaAdi, LoggingContext))
                {
                    Messages.Success(dosyaAdi + " Silindi", 2000);
                    TapuKayitSilBtn.Visible = false;
                    TapuKayitDosyaLnk.Visible = false;
                    TapuKayitYukleFU.Visible = true;
                    BildirimEPostasiGonder("Tapu Kayıt Belgesi silindi. Dosya: <b>" + dosyaAdi + "</b>");
                }
                else
                {
                    Messages.Error(dosyaAdi + " Silinemedi");
                }
            }
            catch (Exception ex)
            {
                if (ex is System.Threading.ThreadAbortException) throw;
                TbysWebPartLog.WriteError(LoggingContext, "TBYS.TasinmazResimleriWP.TapuKayitSilBtn_Click", ex);
                TbysWebPartLog.ReportError(LoggingContext, "TBYS.TasinmazResimleriWP.TapuKayitSilBtn_Click", ex);
            }
        }
        protected void ImarDurumuSilBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string dosyaAdi = ProjeConstants.DOSYA_IMARDURUMU_BELGESI+ TasinmazIdQS + ".pdf";
                if (UtilityHelper.DeleteFileFromSharePointLib(ProjeConstants.PATH_TBYS_URL, ProjeConstants.TBYSBELGELERI_LIB, dosyaAdi, LoggingContext))
                {
                    Messages.Success(dosyaAdi + " Silindi", 2000);
                    ImarDurumuSilBtn.Visible = false;
                    ImarDurumuDosyaLnk.Visible = false;
                    ImarDurumuYukleFU.Visible = true;
                    BildirimEPostasiGonder("İmar Durumu Belgesi silindi. Dosya: <b>" + dosyaAdi + "</b>");
                }
                else
                {
                    Messages.Error(dosyaAdi + " Silinemedi");
                }
            }
            catch (Exception ex)
            {
                if (ex is System.Threading.ThreadAbortException) throw;
                TbysWebPartLog.WriteError(LoggingContext, "TBYS.TasinmazResimleriWP.ImarDurumuSilBtn_Click", ex);
                TbysWebPartLog.ReportError(LoggingContext, "TBYS.TasinmazResimleriWP.ImarDurumuSilBtn_Click", ex);
            }
        }
        protected void TasinmazRaporuSilBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string dosyaAdi = ProjeConstants.DOSYA_TASINMAZ_RAPORU+ TasinmazIdQS + ".pdf";
                if (UtilityHelper.DeleteFileFromSharePointLib(ProjeConstants.PATH_TBYS_URL, ProjeConstants.TBYSBELGELERI_LIB, dosyaAdi, LoggingContext))
                {
                    Messages.Success(dosyaAdi + " Silindi", 2000);
                    TasinmazRaporuSilBtn.Visible = false;
                    TasinmazRaporuDosyaLnk.Visible = false;
                    TasinmazRaporuYukleFU.Visible = true;
                    BildirimEPostasiGonder("Taşınmaz Raporu silindi. Dosya: <b>" + dosyaAdi + "</b>");
                }
                else
                {
                    Messages.Error(dosyaAdi + " Silinemedi");
                }
            }
            catch (Exception ex)
            {
                if (ex is System.Threading.ThreadAbortException) throw;
                TbysWebPartLog.WriteError(LoggingContext, "TBYS.TasinmazResimleriWP.TasinmazRaporuSilBtn_Click", ex);
                TbysWebPartLog.ReportError(LoggingContext, "TBYS.TasinmazResimleriWP.TasinmazRaporuSilBtn_Click", ex);
            }
        }
        #endregion
    }
}
