using System.Globalization;
using System.Linq;
using Utility.Logging;
using NBYS_WebParts.Logging;
using Model.NBYS;
using Model.Ortak;
using Model.Services.Ortak;
using Model.Services.NBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.FTKKisiGirisiWP
{
    [ToolboxItemAttribute(false)]
    public partial class FTKKisiGirisiWP : WebPart
    {
        private PortalLogContext LoggingContext
        {
            get { return PortalLogContext.FromCurrentRequest("NBYS", "FTKKisiGirisiWP"); }
        }

        private void ExecuteAction(string operation, Action action)
        {
            NbysWebPartLog.ExecuteAction(LoggingContext, operation, action);
        }

        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public FTKKisiGirisiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string FTKKisiIdQS
        {
            get
            {
                if (ViewState["FTKKisiId"] == null)
                {
                    if (Page.Request.QueryString["FTKKisiId"] != null)
                    {
                        ViewState["FTKKisiId"] = Page.Request.QueryString["FTKKisiId"];
                    }
                    else
                    {
                        ViewState["FTKKisiId"] = string.Empty;
                    }
                }
                return ViewState["FTKKisiId"].ToString();
            }

            set
            {
                ViewState["FTKKisiId"] = value;
            }
        }
        private string IliIdQS
        {
            get
            {
                if (ViewState["IliId"] == null)
                {
                    if (Page.Request.QueryString["IliId"] != null)
                    {
                        ViewState["IliId"] = Page.Request.QueryString["IliId"];
                    }
                    else
                    {
                        ViewState["IliId"] = string.Empty;
                    }
                }
                return ViewState["IliId"].ToString();
            }

            set
            {
                ViewState["IliId"] = value;
            }
        }
        private string IlcesiIdQS
        {
            get
            {
                if (ViewState["IlcesiId"] == null)
                {
                    if (Page.Request.QueryString["IlcesiId"] != null)
                    {
                        ViewState["IlcesiId"] = Page.Request.QueryString["IlcesiId"];
                    }
                    else
                    {
                        ViewState["IlcesiId"] = string.Empty;
                    }
                }
                return ViewState["IlcesiId"].ToString();
            }

            set
            {
                ViewState["IlcesiId"] = value;
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
        protected void Page_Load(object sender, EventArgs e)
        {
            ExecuteAction("Page_Load", () =>
            {
                if (!Page.IsPostBack) NbysWebPartLog.RecordOpened(LoggingContext);

                if (!Page.IsPostBack)
                {
                    YonergeLnk.HRef = UtilityHelper.YonergeURLGetir(ProjeConstants.PARAM_FTKYONERGE, ProjeConstants.NBYSBELGELERI_LIB, ProjeConstants.PAGE_FTKKISI_GIRISI);
                    IlkACilis();
                }
            });
        }
        private void IlkACilis(bool refreshAfterWrite = false)
        {
            IlDDLDoldur();
            IlIlceDoldur();
            FTKGoreviDDLDoldur();
            UyelikDurumuDDLDoldur();
            if (string.IsNullOrEmpty(FTKKisiIdQS))
            {
                TitleLbl.Text = "FTK Kişi Girişi";
                TitleLbl.CssClass = "col-form-label text-success fw-bold mb-1";
                IdLbl.Text = string.Empty;
                KaydetBtn.Visible = true;
                GuncelleBtn.Visible = false;
                YeniKayitBtn.Visible = false;
                UtilityHelper.SetDDLValue(UyelikDurumuDDL, ProjeConstants.FTK_UYELIK_DURUMU_AKTIF);

                SilBtn.Visible = false;

                Messages.Info("Yeni üyenin bilgilerini girin.", 2000);
            }
            else
            {
                TitleLbl.Text = "FTK Kişi Düzenleme";
                TitleLbl.CssClass = "col-form-label text-primary fw-bold mb-1";
                //IdLbl.Text = FTKKisiIdQS.ToString();
                KaydetBtn.Visible = false;
                GuncelleBtn.Visible = true;
                YeniKayitBtn.Visible = true;
                SilBtn.Visible = true;
                FTKKisiFormunuDoldur(!refreshAfterWrite);
            }
        }
        private void IlIlceDoldur()
        {
            if (!string.IsNullOrEmpty(IliIdQS))
            {
                ListItem ilItem = IliDDL.Items.FindByValue(IliDDL.Items.FindByValue(IliIdQS).Value);
                if (ilItem != null)
                {
                    IliDDL.SelectedValue = ilItem.Value;
                    IlceDDLDoldur();
                    if (!string.IsNullOrEmpty(IlcesiIdQS))
                    {
                        if (IlcesiDDL.Items.FindByValue(IlcesiIdQS) != null)
                        IlcesiDDL.SelectedValue = IlcesiDDL.Items.FindByValue(IlcesiIdQS).Value;
                    }
                }
            }

        }
        private void FTKGoreviDDLDoldur()
        {
            if (FTKGoreviDDL.SelectedItem == null)
            {
                FTKGoreviDDL.Items.Clear();

                FTKGoreviDDL.Items.Add(new ListItem(ProjeConstants.FTK_GOREVI_UYE, ProjeConstants.FTK_GOREVI_UYE_INT.ToString()));
                FTKGoreviDDL.Items.Add(new ListItem(ProjeConstants.FTK_GOREVI_BASKAN, ProjeConstants.FTK_GOREVI_BASKAN_INT.ToString()));
                FTKGoreviDDL.Items.Add(new ListItem(ProjeConstants.FTK_GOREVI_GENELSEKRETER, ProjeConstants.FTK_GOREVI_GENELSEKRETER_INT.ToString()));
                FTKGoreviDDL.Items.Add(new ListItem(ProjeConstants.FTK_GOREVI_FAHRIBASKAN, ProjeConstants.FTK_GOREVI_FAHRIBASKAN_INT.ToString()));
            }

        }
        private void UyelikDurumuDDLDoldur()
        {
            UyelikDurumuDDL.Items.Clear();
            UyelikDurumuDDL.Items.Add(new ListItem(ProjeConstants.FTK_UYELIK_DURUMU_AKTIF, ProjeConstants.FTK_UYELIK_DURUMU_AKTIF));
            UyelikDurumuDDL.Items.Add(new ListItem(ProjeConstants.FTK_UYELIK_DURUMU_AKTIF_DEGIL, ProjeConstants.FTK_UYELIK_DURUMU_AKTIF_DEGIL));
        }
        private void IlDDLDoldur()
        {
            IliDDL.Items.Clear();
            List<Il> list = new IlService().GetAll(LoggingContext);
            IliDDL.Items.Add(new ListItem(string.Empty));
            foreach (Il il in list)
            {
                IliDDL.Items.Add(new ListItem(il.IlAdi, il.Id.ToString()));
            }
            IlceDDLDoldur();

        }
        private void IlceDDLDoldur()
        {
            IlcesiDDL.Items.Clear();
            List<Ilce> list = new IlceService().GetByProvinceId(IliDDL.SelectedValue.ConvertToInt(), LoggingContext);
            ListItem li0 = new ListItem(ProjeConstants.VALILIK, ProjeConstants.VALILIK_INT.ToString());
            IlcesiDDL.Items.Add(li0);
            foreach (Ilce ilce in list)
            {
                if (ilce.IlceAdi.ToUpper().Equals(ProjeConstants.ILCE_MERKEZ.ToUpper()))
                continue;
                IlcesiDDL.Items.Add(new ListItem(ilce.IlceAdi, ilce.Id.ToString()));
            }
        }
        private bool FTKKisiFormunuDoldur(bool reportMissing = true)
        {
            FTKKisi ftkkisi = new FTKKisiService(LoggingContext).GetById(FTKKisiIdQS.ConvertToInt());
            bool formDolduMu = false;

            if (ftkkisi != null)
            {
                IdLbl.Text = ftkkisi.Id.ToString();
                AdiTxt.Text = ftkkisi.Adi;
                SoyadiTxt.Text = ftkkisi.Soyadi;
                TCKimlikNoTxt.Text = ftkkisi.TCKimlikNo.ToString();
                DogumTarihiTxt.Text = ftkkisi.DogumTarihi.ConvertToDatetimeEmptyIfNull();
                Telefon1Txt.Text = ftkkisi.Telefon1;
                Telefon2Txt.Text = ftkkisi.Telefon2;

                UnvaniTxt.Text = ftkkisi.Unvani;
                ValiChk.Checked = ftkkisi.Vali;
                KaymakamChk.Checked = ftkkisi.Kaymakam;
                AdresTxt.Text = ftkkisi.Adres;
                AciklamaTxt.Text = ftkkisi.Aciklama;

                FTKKisiIdQS = ftkkisi.Id.ToString();
                ListItem ilItem = IliDDL.Items.FindByValue(ftkkisi.Ili.ToString());
                if (ilItem != null)
                {
                    IliDDL.SelectedValue = ilItem.Value;
                    IlceDDLDoldur();
                    UtilityHelper.SetDDLValue(IliDDL, ftkkisi.Ili.ToString());
                    UtilityHelper.SetDDLValue(IlcesiDDL, ftkkisi.Ilcesi.ToString());
                    IliIdQS = ftkkisi.Ili.ToString();
                    IlcesiIdQS = ftkkisi.Ilcesi.ToString();
                }
                KartNoTxt.Text = ftkkisi.KartNo;
                UtilityHelper.SetDDLValue(FTKGoreviDDL, ftkkisi.FTKGorevi.ToString());
                UtilityHelper.SetDDLValue(UyelikDurumuDDL, ftkkisi.UyelikDurumu);
                formDolduMu = true;
            }
            else
            {
                if (!reportMissing) throw new InvalidOperationException("Kaydedilen kişi yenileme sırasında bulunamadı.");
                GuncelleBtn.Enabled = false; SilBtn.Enabled = false;
                Messages.Warning("Kişi kaydı bulunamadı. Listeyi yenileyip tekrar seçin.");
            }

            return formDolduMu;
        }
        protected void KaydetBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("KaydetBtn_Click", () =>
            {
                SavePerson(false);
            });
        }

        protected void GuncelleBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("GuncelleBtn_Click", () =>
            {
                SavePerson(true);
            });
        }

        private void SavePerson(bool update)
        {
            long identity = 0;
            if (!string.IsNullOrWhiteSpace(TCKimlikNoTxt.Text) && (!long.TryParse(TCKimlikNoTxt.Text, out identity) || identity < 0))
            { Messages.Warning("T.C. kimlik numarasını rakamlarla girin."); return; }
            DateTime birth = new DateTime(1901, 1, 1);
            if (!string.IsNullOrWhiteSpace(DogumTarihiTxt.Text) && (!DateTime.TryParse(DogumTarihiTxt.Text,
            CultureInfo.GetCultureInfo(ProjeConstants.CULTUREINFO), DateTimeStyles.None, out birth) || birth.Year < 1901))
            { Messages.Warning("Geçerli bir doğum tarihi girin veya alanı boş bırakın."); return; }
            int id = 0;
            if (update && (!int.TryParse(FTKKisiIdQS, out id) || id <= 0))
            { Messages.Warning("Güncellenecek kişi geçerli değil. Listeyi yenileyip tekrar seçin."); return; }
            FTKKisi input = new FTKKisi
            {
                Id = id, Adi = AdiTxt.Text.Trim(), Soyadi = SoyadiTxt.Text.Trim(), TCKimlikNo = identity, DogumTarihi = birth,
                Telefon1 = Telefon1Txt.Text, Telefon2 = Telefon2Txt.Text, KartNo = KartNoTxt.Text, Unvani = UnvaniTxt.Text,
                Vali = ValiChk.Checked, Kaymakam = KaymakamChk.Checked, Adres = AdresTxt.Text, Aciklama = AciklamaTxt.Text,
                Ili = IliDDL.SelectedValue.ConvertToInt(), Ilcesi = IlcesiDDL.SelectedValue.ConvertToInt(),
                FTKGorevi = FTKGoreviDDL.SelectedValue.ConvertToInt(), UyelikDurumu = UyelikDurumuDDL.SelectedValue
            };
            int savedId;
            NbysFtkWriteResult result = new NbysFtkWriteService(LoggingContext).SavePerson(input, out savedId);
            NbysFtkFeedback.Show(LoggingContext, result, update ? "Kişi güncellendi." : "Kişi kaydedildi.", () =>
            {
                FTKKisiIdQS = savedId.ToString(); IliIdQS = input.Ili.ToString(); IlcesiIdQS = input.Ilcesi.ToString();
                IlkACilis(true);
            });
        }

        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("CloseBtn_Click", () =>
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
                Page.Response.Redirect(newUrl);
            });
        }
        private bool AdSoyadVarMi(string adi, string soyadi)
        {
            FTKKisi person = new FTKKisiService(LoggingContext).GetByName(adi, soyadi, FTKKisiIdQS.ConvertToInt());
            return person != null;
        }

        protected void SoyadiTxt_TextChanged(object sender, EventArgs e)
        {
            ExecuteAction("SoyadiTxt_TextChanged", () =>
            {
                // A save postback can also raise TextChanged; its result has one notification.
                if (!string.Equals(Page.Request.Form["__EVENTTARGET"], SoyadiTxt.UniqueID, StringComparison.Ordinal)) return;
                if (AdSoyadVarMi(AdiTxt.Text, SoyadiTxt.Text))
                {
                    Messages.Warning("Aynı ad ve soyadla bir kayıt var. Kaydetmeden önce mevcut kişiyi kontrol edin.");
                }
            });
        }
        private void RedirectToPage(string pageUrl)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + pageUrl;
            Page.Response.Redirect(newUrl);

        }
        protected void FtkKisiListesiBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("FtkKisiListesiBtn_Click", () =>
            {
                RedirectToPage(ProjeConstants.PAGE_FTKKISI_LIST + "?SecilenId=" + FTKKisiIdQS);
            });
        }
        protected void FTKFahriBaskanListesiBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("FTKFahriBaskanListesiBtn_Click", () =>
            {
                RedirectToPage(ProjeConstants.PAGE_FTKFAHRIBASKAN_LIST + "?SecilenId=" + FTKKisiIdQS);
            });
        }
        protected void FTKIslemleriBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("FTKIslemleriBtn_Click", () =>
            {
                FTKIslem fTKIslem = new FTKIslemService(LoggingContext).GetByLocation(IliIdQS.ConvertToInt(), IlcesiIdQS.ConvertToInt());
                string ftkIslemId = fTKIslem == null ? string.Empty : fTKIslem.Id.ToString();
                RedirectToPage(ProjeConstants.PAGE_FTKISLEMLERI + "?FTKIslemId=" + ftkIslemId + "&IliId=" + IliIdQS + "&IlcesiId=" + IlcesiIdQS);
            });
        }

        protected void FTKGoreviDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecuteAction("FTKGoreviDDL_SelectedIndexChanged", () =>
            {
                ValiKaymakamSec();
            });
        }

        private void ValiKaymakamSec()
        {
            int iliId = IliDDL.SelectedItem.Value.ConvertToInt();
            int ilcesiId = IlcesiDDL.SelectedItem.Value.ConvertToInt();

            int gorevi = FTKGoreviDDL.SelectedItem.Value.ConvertToInt();
            if (gorevi == ProjeConstants.FTK_GOREVI_FAHRIBASKAN_INT)
            {
                if (iliId > 0)
                {
                    ValiChk.Checked = true;
                }
                else
                {
                    ValiChk.Checked = false;
                }
                if (ilcesiId > 0)
                {
                    KaymakamChk.Checked = true;
                    ValiChk.Checked = false;
                }
                else
                {
                    KaymakamChk.Checked = false;
                }

            }
            else
            {
                ValiChk.Checked = false;
                KaymakamChk.Checked = false;
            }
        }
        protected void IliDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecuteAction("IliDDL_SelectedIndexChanged", () =>
            {
                IlceDDLDoldur();
                ValiKaymakamSec();
            });
        }
        protected void IlcesiDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecuteAction("IlcesiDDL_SelectedIndexChanged", () =>
            {
                ValiKaymakamSec();
            });
        }
        protected void SilBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("SilBtn_Click", () =>
            {
                FTKKisi person = new FTKKisiService(LoggingContext).GetById(FTKKisiIdQS.ConvertToInt());
                if (person == null) { Messages.Warning("Kişi kaydı bulunamadı. Listeyi yenileyin."); return; }
                SilPopupAc(sender);
            });
        }
        private void SilPopupAc(object sender)
        {
            ParamVnLbl.Text = FTKKisiIdQS;
            string openModal = "OpenSilModal();";
            SilMesajiLbl.Visible = true;
            SilMesajiLbl.Text = "Kişi kaydını silmek istiyor musunuz? FTK geçmişinde kullanılan kayıtlar korunur.";
            SilModalBaslikLbl.Text = "Silmeyi Onayla";
            SilNowBtn.Visible = true;
            UtilityHelper.ScriptCalistir(openModal);
        }
        protected void SilNowBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("SilNowBtn_Click", () =>
            {
                NbysFtkWriteResult result = new NbysFtkWriteService(LoggingContext).DeletePerson(FTKKisiIdQS.ConvertToInt());
                NbysFtkFeedback.Show(LoggingContext, result, "Kişi silindi.", () =>
                {
                    UtilityHelper.ScriptCalistir("bootstrap.Modal.getOrCreateInstance(document.getElementById('SilModalDiv')).hide();");
                    KaydetBtn.Visible = false; GuncelleBtn.Visible = false; SilBtn.Visible = false; YeniKayitBtn.Visible = true;
                    TitleLbl.Text = "Kişi silindi";
                });
            });
        }

        protected void YeniKayitBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("YeniKayitBtn_Click", () =>
            {
                RedirectToPage(ProjeConstants.PAGE_FTKKISI_GIRISI + "?IliId=" + IliIdQS + "&IlcesiId=" + IlcesiIdQS);
            });
        }
    }
}
