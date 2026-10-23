using MTS_WebParts.Logging;
using Utility.Logging;
using System.Web.UI.WebControls;
using Model.MTS;
using Model.Ortak;
using Model.Services.Ortak;
using Model.Services.MTS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;
using System.Web.UI;

namespace MTS_WebParts.KisiGirisiWP
{
    [ToolboxItemAttribute(false)]
    public partial class KisiGirisiWP : WebPart
    {
        private PortalLogContext logContext;
        private PortalLogContext LogContext { get { return logContext ?? (logContext = PortalLogContext.FromCurrentRequest("MTS", "KisiGirisiWP")); } }
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public KisiGirisiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string KisiIdQS
        {
            get
            {

                if (ViewState["KisiId"] == null)
                {
                    if (Page.Request.QueryString["KisiId"] != null)
                    {
                        ViewState["KisiId"] = Page.Request.QueryString["KisiId"];
                    }
                    else
                    {
                        ViewState["KisiId"] = string.Empty;
                    }
                }
                return ViewState["KisiId"].ToString();
            }

            set
            {
                ViewState["KisiId"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "Page_Load", () =>
            {
                if (!Page.IsPostBack) MtsWebPartLog.RecordOpened(LogContext);
                if (!Page.IsPostBack)// sayfa ilk kez açiliyorsa (bu sayfanin içindeki butona basilma ani hariç)
                {

                    MTSUnvanDDLDoldur();
                    IlDDDoldurL();


                    if (KisiIdQS.ConvertToInt()>0)
                    {
                        Kisi kisi = new Kisi();
                        kisi = new Model.Services.MTS.KisiService(LogContext).GetById(KisiIdQS.ConvertToInt());
                        if (kisi != null)
                        {

                            IdLbl.Text = kisi == null ? string.Empty : "( " + kisi.Id + ProjeConstants.FAALIYET_KATILIMCI_DIS + " )";
                            KaydetBtn.Visible = false;
                            GuncelleBtn.Visible = true;
                            //KisiyiSilBtn.Visible = true;
                            FaaliyetGirBtn.Visible = true;
                            TitleLbl.Text = "Kişi Düzenle";
                            TitleLbl.CssClass = "col-form-label text-primary fw-bold mb-1";
                            FormuDoldur();
                        }
                        else
                        {
                            KaydetBtn.Visible = GuncelleBtn.Visible = KisiyiSilBtn.Visible = FaaliyetGirBtn.Visible = false;
                    throw new MtsInputException("Kişi bulunamadı. Listeden yeniden seçin.");
                        }
                    }
                    else
                    {
                        TitleLbl.Text = "Kişi Girişi";
                        TitleLbl.CssClass = "col-form-label text-success fw-bold mb-1";
                        IdLbl.Text = string.Empty;
                        KaydetBtn.Visible = true;
                        GuncelleBtn.Visible = false;
                        KisiyiSilBtn.Visible = false;
                        FaaliyetGirBtn.Visible = false;
                        Utility.HelperClasses.Messages.Info("Yeni kişinin bilgilerini girip kaydedebilirsiniz.");
                    }
                }
            });
        }

        private void MTSUnvanDDLDoldur()
        {

            if (MTSUnvanTanimDDL.SelectedItem == null)
            {
                MTSUnvanTanimDDL.Items.Clear();
                List<MTSUnvanTanim> list = new MTSUnvanTanimService(LogContext).GetAll();
                MTSUnvanTanimDDL.Items.Add(new ListItem("", ""));
                foreach (MTSUnvanTanim item in list)
                {
                    MTSUnvanTanimDDL.Items.Add(new ListItem(item.KisaAdi, item.Id.ToString()));
                }
            }


        }

        private void IlDDDoldurL()
        {
            if (IliDDL.SelectedItem == null)
            {
                IliDDL.Items.Clear();
                List<Il> list = new IlService().GetAll(LogContext);
                foreach (Il il in list)
                {
                    IliDDL.Items.Add(new ListItem(il.IlAdi, il.Id.ToString()));
                }
                IlceDDLDoldur();
            }

        }
        private void IlceDDLDoldur()
        {
            IlcesiDDL.Items.Clear();
            List<Ilce> list = new IlceService().GetByProvinceId(IliDDL.SelectedValue.ConvertToInt(), LogContext);
            foreach (Ilce ilce in list)
            {
                IlcesiDDL.Items.Add(new ListItem(ilce.IlceAdi, ilce.Id.ToString()));
            }
        }
        protected void IliDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "IliDDL_SelectedIndexChanged", () =>
            {
                IlceDDLDoldur();
            });
        }
        private void FormuDoldur()
        {
            if (KisiIdQS.ConvertToInt() > 0)
            {

                Kisi kisi = new Kisi();
                kisi = new Model.Services.MTS.KisiService(LogContext).GetById(KisiIdQS.ConvertToInt());
                if (kisi != null)
                {
                    ViewState["PersonVersion"] = MtsContactWriteService.Version(kisi);
                    AdiTxt.Text = kisi.Adi;
                    SoyadiTxt.Text = kisi.Soyadi;
                    TCKimlikNoTxt.Text = kisi.TCKimlikNo.ToString();
                    KurumuTxt.Text = kisi.Kurumu;
                    UnvaniTxt.Text = kisi.Unvani;
                    GoreviTxt.Text = kisi.Gorevi;
                    Telefon1Txt.Text = kisi.Telefon1;
                    Telefon2Txt.Text = kisi.Telefon2;
                    Telefon3Txt.Text = kisi.Telefon3;

                    Dahili1Txt.Text = kisi.Dahili1;
                    Dahili2Txt.Text = kisi.Dahili2;
                    Dahili3Txt.Text = kisi.Dahili3;

                    DogumTarihiTxt.Text = kisi.DogumTarihi.ConvertToDatetimeEmptyIfNull();
                    KutlamaChk.Checked = kisi.Kutlama;
                    RandevuKisitiChk.Checked = kisi.RandevuKisiti;
                    TelAciklama1Txt.Text = kisi.TelAciklama1;
                    TelAciklama2Txt.Text = kisi.TelAciklama2;
                    TelAciklama3Txt.Text = kisi.TelAciklama3;

                    AdresTxt.Text = kisi.Adres;

                    UtilityHelper.SetDDLValue(MTSUnvanTanimDDL, kisi.MTSUnvanTanimId.ToString()) ;
                    ListItem ilItem = IliDDL.Items.FindByValue(kisi.Ili.ToString());
                    if (ilItem != null)
                    {
                        IliDDL.SelectedValue = ilItem.Value;
                        IlceDDLDoldur();
                        if (IlcesiDDL.Items.FindByValue(kisi.Ilcesi.ToString()) != null)
                            IlcesiDDL.SelectedValue = IlcesiDDL.Items.FindByValue(kisi.Ilcesi.ToString()).Value;
                    }

                    AdresTxt.Text = kisi.Adres;
                    AciklamaTxt.Text = kisi.Aciklama;
                    string kurum = string.Empty;
                    string gorev = string.Empty;
                    string kurumGorevStr = new Model.Services.MTS.MTSKurumGorevService(LogContext).GetByKisiId(kisi.Id, ref kurum, ref gorev);
                    if (string.IsNullOrEmpty(kurumGorevStr))
                    {
                        MTSKurumTanimTxt.Text = kisi.Kurumu;
                        MTSGorevTanimTxt.Text = kisi.Gorevi;
                    }
                    else
                    {
                        MTSKurumTanimTxt.Text = kurum;
                        MTSGorevTanimTxt.Text = gorev;
                    }
                }
                else
                {
                    KaydetBtn.Visible = GuncelleBtn.Visible = KisiyiSilBtn.Visible = FaaliyetGirBtn.Visible = false;
                    throw new MtsInputException("Kişi bulunamadı. Listeden yeniden seçin.");
                }

            }
        }
        protected void KaydetBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "KaydetBtn_Click", () =>
            {
                SavePerson(false);
            });
        }

        private void SavePerson(bool update)
        {
            if (object.Equals(ViewState["PersonDeleted"], true)) throw new MtsInputException("Kişi silindi. Listeden devam edin.");
            if (!update && KisiIdQS.ConvertToInt() > 0) throw new MtsInputException("Kayıt oluşturuldu. Güncelleme düğmesini kullanın.");
            if (string.IsNullOrWhiteSpace(AdiTxt.Text)) throw new MtsInputException("Kişinin adını girin.");
            if (string.IsNullOrWhiteSpace(SoyadiTxt.Text)) throw new MtsInputException("Kişinin soyadını girin.");
            long identity = 0;
            string identityText = TCKimlikNoTxt.Text.Trim();
            if (identityText.Length > 0 && (!long.TryParse(identityText, out identity) || identity < 0))
                throw new MtsInputException("T.C. kimlik numarasını rakamlarla girin.");
            DateTime birthday = DateTime.MinValue;
            if (!string.IsNullOrWhiteSpace(DogumTarihiTxt.Text) && (!DateTime.TryParse(DogumTarihiTxt.Text,
                CultureInfo.GetCultureInfo("tr-TR"), DateTimeStyles.None, out birthday) || birthday > DateTime.Today || birthday < new DateTime(1753, 1, 1)))
                throw new MtsInputException("Geçerli bir doğum tarihi girin.");
            var input = new Kisi { Id = update ? KisiIdQS.ConvertToInt() : 0,
                Adi = AdiTxt.Text, Soyadi = SoyadiTxt.Text, TCKimlikNo = identity,
                MTSUnvanTanimId = MTSUnvanTanimDDL.SelectedValue.ConvertToInt(),
                Kurumu = KurumuTxt.Text, Unvani = UnvaniTxt.Text, Gorevi = GoreviTxt.Text,
                Telefon1 = Telefon1Txt.Text, Telefon2 = Telefon2Txt.Text, Telefon3 = Telefon3Txt.Text,
                Dahili1 = Dahili1Txt.Text, Dahili2 = Dahili2Txt.Text, Dahili3 = Dahili3Txt.Text,
                TelAciklama1 = TelAciklama1Txt.Text, TelAciklama2 = TelAciklama2Txt.Text, TelAciklama3 = TelAciklama3Txt.Text,
                DogumTarihi = birthday, Kutlama = KutlamaChk.Checked, RandevuKisiti = RandevuKisitiChk.Checked,
                Adres = AdresTxt.Text, Ili = IliDDL.SelectedValue.ConvertToInt(), Ilcesi = IlcesiDDL.SelectedValue.ConvertToInt(), Aciklama = AciklamaTxt.Text };
            if (update && input.Id <= 0) throw new MtsInputException("Güncellenecek kişiyi listeden seçin.");
            int savedId;
            var result = new MtsContactWriteService(LogContext).SavePerson(input, ViewState["PersonVersion"] as string, out savedId);
            MtsWebPartLog.RequireCompleted(result);
            KisiIdQS = savedId.ToString();
            KaydetBtn.Visible = false; GuncelleBtn.Visible = FaaliyetGirBtn.Visible = true;
            MtsWebPartLog.FinishCommitted(LogContext, update ? "Kişi güncellendi." : "Kişi kaydedildi.", FormuDoldur);
        }

        private bool AdSoyadVarMi(string adi, string soyadi)
        {
            Kisi kisiDao = new Kisi();
            kisiDao = new Model.Services.MTS.KisiService(LogContext).GetByName(adi, soyadi);
            if (kisiDao == null)
            {
                return false;
            }
            else
                return true;
        }

        private bool KayitVarMi(string tckimlikno)
        {
            if (string.IsNullOrEmpty(tckimlikno))
            {
                return false;
            }
            else
            {
                Kisi kisiDao = new Kisi();
                kisiDao = new Model.Services.MTS.KisiService(LogContext).GetByTcKimlikNo(tckimlikno);
                if (kisiDao == null)
                {
                    return false;
                }
                else
                    return true;
            }

        }

        private void RedirectToPage(string pageUrl)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path);
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + pageUrl;
            Page.Response.Redirect(newUrl);

        }
        protected void GuncelleBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "GuncelleBtn_Click", () =>
            {
                SavePerson(true);
            });
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "CloseBtn_Click", () =>
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path);
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));// + "/" + ProjeConstants.PAGE_HOME;
                Page.Response.Redirect(newUrl);
            });
        }
        protected void KisiyiSilBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "KisiyiSilBtn_Click", () =>
            {
                //Kisi Silme Kaldirildi
                Kisi kisi = new Kisi();
                kisi = new Model.Services.MTS.KisiService(LogContext).GetById(KisiIdQS.ConvertToInt());
                if (kisi != null)
                {
                    KisiyiSilPopupAc(sender, kisi);
                }
            });
        }

        private void KisiyiSilPopupAc(object sender, Kisi kisi)
        {
            ParamVnLbl.Text = KisiIdQS;
            string openModal = "OpenModalOnay();";
            KisiSilNowBtn.Visible = false;
            OnaylaBtn.Visible = false;
            KisiSilNowBtn.CssClass = "btn btn-outline-danger";
            ModalBaslikLbl.CssClass = "col-form-label text-danger fw-bold";
            FaaliyetKatilimciChk.Visible = false;
            IrtibatPersoneliChk.Visible = false;
            bool silinebilirMi = true;
            OnayMesajiLbl.Text = string.Empty;


            bool averilenAniObjesiVarmi = VerilenAniObjesiVarMi(KisiIdQS.ConvertToInt());
            if (averilenAniObjesiVarmi)
            {
                silinebilirMi = false;
                OnayMesajiLbl.Text += " Kişiye verilen anı objesi kaydı var." + "</br>";
            }
            bool getirilenAniObjesiVarmi = GetirilenAniObjesiVarMi(KisiIdQS.ConvertToInt());
            if (getirilenAniObjesiVarmi)
            {
                silinebilirMi = false;
                OnayMesajiLbl.Text += " Kişinin getirdiği anı objesi kaydı var." + "</br>";
            }


            List<FaaliyetKatilim> list = new Model.Services.MTS.FaaliyetKatilimService(LogContext).GetByKatilimciId(KisiIdQS.ConvertToInt());

            if (list.Count > 0)
            {
                silinebilirMi = false;

                OnayMesajiLbl.Text += "Kişinin faaliyet katılımı var." + "</br>";


            }
            List<AramaGorusme> aramalistlist = new Model.Services.MTS.AramaGorusmeService(LogContext).GetByArayanId(KisiIdQS.ConvertToInt());

            if (aramalistlist.Count > 0)
            {
                silinebilirMi = false;

                OnayMesajiLbl.Text += "Kişinin arama/görüşme kaydı var." + "</br>";


            }
            if (silinebilirMi)
            {
                kaydetGuncelleSilHdn.Value = ProjeConstants.SIL;
                OnayMesajiLbl.Visible = true;
                OnayMesajiLbl.Text = System.Web.HttpUtility.HtmlEncode(kisi.Adi + " " + kisi.Soyadi) +" adlı kişiyi silmek istediğinizden emin misiniz?";
                ModalBaslikLbl.Text = System.Web.HttpUtility.HtmlEncode(kisi.Adi + " " + kisi.Soyadi) + " Silinecek";
                KisiSilNowBtn.Visible = true;
            }
            else
            {
                kaydetGuncelleSilHdn.Value = ProjeConstants.BILGI;
                OnayMesajiLbl.Visible = true;
                ModalBaslikLbl.Text = System.Web.HttpUtility.HtmlEncode(kisi.Adi + " " + kisi.Soyadi) + " Silinemiyor";
            }
            UtilityHelper.ScriptCalistir(openModal);
        }

        private bool VerilenAniObjesiVarMi(int kisiId)
        {
            AniObjesiDagitim aniObjesiDagitim = new AniObjesiDagitim();
            List<AniObjesiDagitim> aniObjesiDagitimList = new Model.Services.MTS.AniObjesiDagitimService(LogContext).GetByParticipant(kisiId, ProjeConstants.ANIOBJESI_VERILEN_INT.ToString());
            return aniObjesiDagitimList.Count > 0;
        }
        private bool GetirilenAniObjesiVarMi(int kisiId)
        {
            AniObjesiDagitim aniObjesiDagitim = new AniObjesiDagitim();
            List<AniObjesiDagitim> aniObjesiDagitimList = new Model.Services.MTS.AniObjesiDagitimService(LogContext).GetByParticipant(kisiId, ProjeConstants.ANIOBJESI_GETIRILEN_INT.ToString());
            return aniObjesiDagitimList.Count > 0;
        }
        protected void KisiSilNowBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "KisiSilNowBtn_Click", () =>
            {
                var result = new MtsContactWriteService(LogContext).DeletePerson(KisiIdQS.ConvertToInt(), ViewState["PersonVersion"] as string);
                MtsWebPartLog.RequireCompleted(result);
                KaydetBtn.Visible = GuncelleBtn.Visible = KisiSilNowBtn.Visible = KisiyiSilBtn.Visible = FaaliyetGirBtn.Visible = false;
                ViewState["PersonDeleted"] = true;
                Utility.HelperClasses.Messages.Success("Kişi silindi. Kişi listesinden devam edebilirsiniz.");
            });
        }
        protected void FaaliyetGirBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "FaaliyetGirBtn_Click", () =>
            {
                Kisi kisi = new Kisi();
                kisi = new Model.Services.MTS.KisiService(LogContext).GetById(KisiIdQS.ConvertToInt());
                if (kisi != null)
                {
                    FaaliyetGirisiPopupAc(sender);
                }
            });
        }

        private void FaaliyetGirisiPopupAc(object sender)
        {
            ParamVnLbl.Text = KisiIdQS;
            string openModal = "OpenModalOnay();";
            kaydetGuncelleSilHdn.Value = ProjeConstants.YENI;
            OnayMesajiLbl.Visible = true;
            OnayMesajiLbl.Text = "Yeni faaliyet oluşturulmasını onaylıyor musunuz?";
            ModalBaslikLbl.Text = "Yeni Faaliyet Oluşturulacak";
            KisiSilNowBtn.Visible = false;
            OnaylaBtn.Visible = true;
            OnaylaBtn.CssClass = "btn btn-outline-success";
            FaaliyetKatilimciChk.Visible = true;
            IrtibatPersoneliChk.Visible = true;
            ModalBaslikLbl.CssClass = "col-form-label text-success fw-bold";

            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), openModal, true);

        }

        protected void OnaylaBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "OnaylaBtn_Click", () =>
            {
                if (!string.Equals(kaydetGuncelleSilHdn.Value, ProjeConstants.YENI, StringComparison.Ordinal))
                    throw new MtsInputException("İşlemi yeniden açıp onaylayın.");
                if (ViewState["CreatedActivityId"] != null)
                {
                    RedirectToPage(ProjeConstants.PAGE_FAALIYET_GIRIS + "?FaaliyetId=" + ViewState["CreatedActivityId"]);
                    return;
                }
                int activityId;
                var result = new MtsContactWriteService(LogContext).CreateActivity(KisiIdQS.ConvertToInt(), 0, null,
                    FaaliyetKatilimciChk.Checked, IrtibatPersoneliChk.Checked, AciklamaTxt.Text, out activityId);
                MtsWebPartLog.RequireCompleted(result);
                ViewState["CreatedActivityId"] = activityId;
                OnaylaBtn.Visible = false;
                MtsWebPartLog.FinishCommitted(LogContext, "Faaliyet oluşturuldu.", () =>
                    RedirectToPage(ProjeConstants.PAGE_FAALIYET_GIRIS + "?FaaliyetId=" + activityId));
            });
        }
        protected void KisiListesiBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "KisiListesiBtn_Click", () =>
            {
                RedirectToPage(ProjeConstants.PAGE_KISI_LIST + "?SecilenId=" + KisiIdQS);
            });
        }
        protected void FaaliyetListesiBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "FaaliyetListesiBtn_Click", () =>
            {
                RedirectToPage(ProjeConstants.PAGE_FAALIYET_LIST);
            });
        }
        protected void FaaliyetTakvimiBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "FaaliyetTakvimiBtn_Click", () =>
            {
                RedirectToPage(ProjeConstants.PAGE_FAALIYET_TAKVIM);
            });
        }
        protected void AramaListesiBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "AramaListesiBtn_Click", () =>
            {
                RedirectToPage(ProjeConstants.PAGE_ARAMAGORUSME_LIST);
            });
        }

        protected void AdiTxt_TextChanged(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "AdiTxt_TextChanged", () =>
            {
                if (AdSoyadVarMi(AdiTxt.Text, SoyadiTxt.Text))
                {
                    Utility.HelperClasses.Messages.Warning("Aynı ad ve soyadla kayıt var. Kaydetmeden önce kişi listesini kontrol edin.");
                }
            });
        }
    }
}
