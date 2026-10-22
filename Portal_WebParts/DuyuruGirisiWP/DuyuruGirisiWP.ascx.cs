using Microsoft.SharePoint;
using Model.IKYS;
using Model.Services.IKYS;
using Model.Ortak;
using Portal_WebParts.Logging;
using Utility.Logging;
using Model.Services.Portal;
using Model.Portal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Portal_WebParts.DuyuruGirisiWP
{
    [ToolboxItemAttribute(false)]
    public partial class DuyuruGirisiWP : WebPart
    {
        private PortalLogContext LoggingContext
        {
            get { return PortalLogContext.FromCurrentRequest("Portal", "DuyuruGirisiWP"); }
        }
        private void ExecuteAction(string operation, Action action)
        {
            PortalWebPartLog.ExecuteAction(LoggingContext, operation, action);
        }
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public DuyuruGirisiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private List<PersonelItem> SecilenPersonelList
        {
            get
            {
                if (ViewState["SecilenPersonelList"] == null)
                {
                    ViewState["SecilenPersonelList"] = new List<PersonelItem>().ToList();//personel.SelectCalisanPersonel().ToList();

                }
                return (List<PersonelItem>)ViewState["SecilenPersonelList"];
            }
            set
            {
                ViewState["SecilenPersonelList"] = value;
            }
        }
        private List<PersonelItem> SecilmeyenPersonelList
        {
            get
            {
                if (ViewState["SecilmeyenPersonelList"] == null)
                {
                    ViewState["SecilmeyenPersonelList"] = new List<PersonelItem>().ToList();//personel.SelectCalisanPersonel().ToList();

                }
                return (List<PersonelItem>)ViewState["SecilmeyenPersonelList"];
            }
            set
            {
                ViewState["SecilmeyenPersonelList"] = value;
            }
        }
        private string DuyuruIdQS
        {
            get
            {

                if (ViewState["DuyuruId"] == null)
                {
                    if (Page.Request.QueryString["DuyuruId"] != null)
                    {
                        ViewState["DuyuruId"] = Page.Request.QueryString["DuyuruId"];
                    }
                    else
                    {
                        ViewState["DuyuruId"] = string.Empty;
                    }
                }
                return ViewState["DuyuruId"].ToString();
            }

            set
            {
                ViewState["DuyuruId"] = value;
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
            ExecuteAction("Page_Load", () =>
            {
                PortalWebPartLog.RecordOpened(LoggingContext);
                var manager = System.Web.UI.ScriptManager.GetCurrent(Page);
                if (manager != null) { manager.RegisterPostBackControl(KaydetBtn); manager.RegisterPostBackControl(GuncelleBtn); }
                if (!Page.IsPostBack)
                {
                    bool editing = DuyuruIdQS.ConvertToInt() > 0;
                    KaydetBtn.Visible = !editing;
                    GuncelleBtn.Visible = SilBtn.Visible = editing;
                    if (editing) DuyuruDuzenlemeyiAc();
                    else DuyuruGirisiAc();
                }
                SecilenPersonelTablosunuDoldur();
            });
        }

        private void DuyuruDuzenlemeyiAc()
        {
            TitleLbl.Text = "Duyuru Düzenleme";
            TitleLbl.CssClass = "col-form-label text-primary fw-bold mb-1";
            BaslangicSaatiDDLDoldur();
            BitisSaatiDDLDoldur();
            TekrarlaDDLDoldur();
            SecilenSecilmeyenPersonelListesiniDoldur(DuyuruIdQS.ConvertToInt());

            DuyuruFormunuDoldur();
            PersonelDDLDoldur();
        }
        private void SecilenSecilmeyenPersonelListesiniDoldur(int duyuruId)
        {
            SecilmeyenPersonelList.Clear();
            SecilenPersonelList.Clear();
            Personel personelDao = new Personel();
            List<Personel> list = new PersonelService().GetActiveEmployees(Personel.PersonelTipi.Kadrolu, LoggingContext);

            Duyuru duyuru = new Duyuru();
            duyuru = new Model.Services.Portal.DuyuruService(LoggingContext).GetById(DuyuruIdQS.ConvertToInt());
            if (duyuru != null)
            {
                int sirano = 1;
                foreach (Personel personel in list)
                {

                    PersonelItem pI = new PersonelItem();
                    pI.Id = personel.Id;
                    pI.Adi = personel.Adi;
                    pI.Soyadi = personel.Soyadi;
                    pI.SiraNo = sirano++;
                    if (PortalDuyuruWriteService.IsRecipient(duyuru.DuyuruAlicilari, personel.Id))
                    {
                        if (!SecilenPersonelList.Contains(pI))
                        {
                            SecilenPersonelList.Add(pI);
                        }
                    }
                    else
                    {
                        if (!SecilmeyenPersonelList.Contains(pI))
                        {
                            SecilmeyenPersonelList.Add(pI);
                        }
                    }

                }
            }
        }
        private void DuyuruFormunuDoldur()
        {
            Duyuru duyuru = new Duyuru();
            duyuru = new Model.Services.Portal.DuyuruService(LoggingContext).GetById(DuyuruIdQS.ConvertToInt());
            if (duyuru == null) throw new PortalInputException("Duyuru bulunamadı. Listeyi yenileyip tekrar seçin.");
            BaslikTxt.Text = duyuru.Baslik;
            MetinTxt.Text= duyuru.Metin;
            DateTime basTar = duyuru.YayinBasTar;
            DateTime bitTar = duyuru.YayinBitTar;
            string basSaat = basTar.ToString("HH") + ":" + basTar.ToString("mm");
            string bitSaat = bitTar.ToString("HH") + ":" + bitTar.ToString("mm");
            YayinBasTarTxt.Value = basTar.ConvertToDatetimeEmptyIfNull();
            YayinBitTarTxt.Value = bitTar.ConvertToDatetimeEmptyIfNull();
            AktifChk.Checked = duyuru.Aktif;
            PopupChk.Checked = duyuru.Popup;

            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string hostUrl = currentUrl.Substring(0, currentUrl.LastIndexOf(System.Web.HttpContext.Current.Request.Url.AbsolutePath));
            string imgUrl = string.IsNullOrEmpty(duyuru.Resim)
                ? hostUrl + ProjeConstants.PATH_RESIMLER_DUYURU +"duyuru-resmi-yok.jpg"
                : hostUrl + ProjeConstants.PATH_RESIMLER_DUYURU + duyuru.Resim.ReplaceTrChars() + ".jpg";

            DisplayImage.ImageUrl = imgUrl;

            UtilityHelper.SetDDLValue(BasSaatDDL, basSaat);
            UtilityHelper.SetDDLValue(BitSaatDDL, bitSaat);
            UtilityHelper.SetDDLValue(TekrarlaDDL, duyuru.Tekrar);
        }
        private void DuyuruGirisiAc()
        {
            if (!Page.IsPostBack)
            {
                TitleLbl.Text = "Yeni Duyuru";
                TitleLbl.CssClass = "col-form-label text-success fw-bold mb-1";
                YayinBasTarTxt.Value = DateTime.Today.ConvertToDatetimeEmptyIfNull();
                YayinBitTarTxt.Value = DateTime.Today.ConvertToDatetimeEmptyIfNull();
                BaslangicSaatiDDLDoldur();
                BitisSaatiDDLDoldur();

                TekrarlaDDLDoldur();
                TümPersoneliSecilenmeyenPersonelListesieEkle();
                PersonelDDLDoldur();
            }

            SecilenPersonelTablosunuDoldur();
        }
        private void BaslangicSaatiDDLDoldur()
        {
            BasSaatDDL.Items.Clear();
            TimeSpan bastarTS = new TimeSpan(6, 0, 0);
            TimeSpan aralikTS = new TimeSpan(0, 30, 0);
            TimeSpan bittarTS = new TimeSpan(20, 0, 0);

            TimeSpan nextTS = bastarTS;

            while (nextTS < bittarTS)
            {
                string bastarStr = string.Format("{0:00}:{1:00}", nextTS.Hours, nextTS.Minutes);
                ListItem li = new ListItem(bastarStr);
                BasSaatDDL.Items.Add(li);
                nextTS = nextTS + aralikTS;
            }
        }
        private void BitisSaatiDDLDoldur()
        {
            BitSaatDDL.Items.Clear();
            string basSaatStr = BasSaatDDL.SelectedItem == null ? "20:00" : BasSaatDDL.SelectedItem.Text;
            TimeSpan bastarTS = basSaatStr.ConvertToTimeSpan();
            TimeSpan aralikTS = new TimeSpan(0, 30, 0);
            TimeSpan bittarTS = new TimeSpan(20, 0, 0);

            TimeSpan nextTS = bastarTS;

            do
            {
                nextTS = nextTS + aralikTS;
                string bittarStr = string.Format("{0:00}:{1:00}", nextTS.Hours, nextTS.Minutes);
                ListItem li = new ListItem(bittarStr);
                BitSaatDDL.Items.Add(li);

            } while (nextTS < bittarTS);

            if (BitSaatDDL.Items.FindByText("20:00") != null)
                BitSaatDDL.SelectedValue = BitSaatDDL.Items.FindByText("20:00").Value;
        }
        private void TekrarlaDDLDoldur()
        {
            TekrarlaDDL.Items.Clear();

            ListItem li = new ListItem(ProjeConstants.DUYURU_TEKRAR_YOK);
            //ListItem li1 = new ListItem(ProjeConstants.DUYURU_TEKRAR_SAAT);
            //ListItem li2 = new ListItem(ProjeConstants.DUYURU_TEKRAR_GUN);
            ListItem li3 = new ListItem(ProjeConstants.DUYURU_TEKRARLA_HAFTA);
            ListItem li4 = new ListItem(ProjeConstants.DUYURU_TEKRARLA_AY);
            ListItem li5 = new ListItem(ProjeConstants.DUYURU_TEKRARLA_YIL);
            TekrarlaDDL.Items.Add(li);
            //TekrarlaDDL.Items.Add(li1);
            //TekrarlaDDL.Items.Add(li2);
            TekrarlaDDL.Items.Add(li3);
            TekrarlaDDL.Items.Add(li4);
            TekrarlaDDL.Items.Add(li5);

        }
        private void PersonelDDLDoldur()
        {
            PersonelDDL.Items.Clear();
            ListItem bosLi = new ListItem(ProjeConstants.BOS, ProjeConstants.BOS_INT.ToString());
            PersonelDDL.Items.Add(bosLi);
            SecilmeyenPersonelList = SecilmeyenPersonelList.OrderBy(t => t.SiraNo).ToList();
            foreach (PersonelItem item in SecilmeyenPersonelList)
            {
                ListItem li = new ListItem(item.Adi.ReturnEmptyIfNull().ToString() + " " + item.Soyadi.ReturnEmptyIfNull().ToString(), item.Id.ReturnZeroIfNull().ToString());
                PersonelDDL.Items.Add(li);
            }
        }
        private void TümPersoneliSecilenPersonelListesineEkle()
        {
            SecilmeyenPersonelList.Clear();
            SecilenPersonelList.Clear();
            Personel personelDao = new Personel();
            List<Personel> list = new PersonelService().GetActiveEmployees(Personel.PersonelTipi.Kadrolu, LoggingContext);
            int sirano = 1;
            foreach (Personel personel in list)
            {
                PersonelItem pI = new PersonelItem();
                pI.Id = personel.Id;
                pI.Adi = personel.Adi;
                pI.Soyadi = personel.Soyadi;
                pI.SiraNo = sirano++;
                SecilenPersonelList.Add(pI);
            }
        }
        private void TümPersoneliSecilenmeyenPersonelListesieEkle()
        {
            SecilenPersonelList.Clear();

            SecilmeyenPersonelList.Clear();
            Personel personelDao = new Personel();
            List<Personel> list = new PersonelService().GetActiveEmployees(Personel.PersonelTipi.Kadrolu, LoggingContext);
            int sirano = 1;
            foreach (Personel personel in list)
            {
                PersonelItem pI = new PersonelItem();
                pI.Id = personel.Id;
                pI.Adi = personel.Adi;
                pI.Soyadi = personel.Soyadi;
                pI.SiraNo = sirano++;
                SecilmeyenPersonelList.Add(pI);
            }
        }
        private void SecilenPersonelTablosunuDoldur()
        {
            SecilenPersonelTable.Rows.Clear();
            int SiraNo = 0;
            SecilenPersonelList = SecilenPersonelList.OrderBy(t => t.SiraNo).ToList();
            foreach (PersonelItem pI in SecilenPersonelList)
            {
                if (pI != null)
                {
                    TableRow row = new TableRow();
                    TableCell adSoyadCell = new TableCell();
                    adSoyadCell.Text = System.Web.HttpUtility.HtmlEncode(pI.Adi + " " + pI.Soyadi);
                    row.Controls.Add(adSoyadCell);

                    TableCell SilCell = new TableCell();
                    LinkButton SilBtn = new LinkButton();
                    SilBtn.Text = "Çıkar";

                    SilBtn.ID = "SilBtn" + pI.Id;
                    SilBtn.CssClass = "btn btn-outline-danger";
                    SilBtn.CausesValidation = false;
                    SilBtn.Click += delegate
                    {
                        ExecuteAction("RemoveRecipient", () =>
                        {
                        SecilenPersonelList.Remove(pI);
                        if (!SecilmeyenPersonelList.Contains(pI))
                        {
                            SecilmeyenPersonelList.Add(pI);
                        }
                        PersonelDDLDoldur();
                        SecilenPersonelTablosunuDoldur();
                        });
                    };
                    SilCell.Controls.Add(SilBtn);
                    row.Controls.Add(SilCell);

                    SecilenPersonelTable.Controls.Add(row);

                }

            }


        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("CloseBtn_Click", () => RedirectToPage(ProjeConstants.PAGE_DUYURU_LIST));
        }
        protected void BasSaatDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecuteAction("BasSaatDDL_SelectedIndexChanged", () =>
            {
                BitisSaatiDDLDoldur();
            });
        }
        protected void BitSaatDDL_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        protected void TekrarlaDDL_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        protected void PersonelDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecuteAction("PersonelDDL_SelectedIndexChanged", () =>
            {
                int id = PersonelDDL.SelectedValue.ConvertToInt();
                if (id <= 0) return;
                PersonelItem item = SecilmeyenPersonelList.FirstOrDefault(person => person.Id == id);
                if (item == null) throw new PortalInputException("Personel seçimi geçerli değil. Listeyi yenileyin.");
                SecilenPersonelList.Add(item);
                SecilmeyenPersonelList.Remove(item);
                PersonelDDLDoldur();
                SecilenPersonelTablosunuDoldur();
            });
        }
        protected void KaydetBtn_Click(object sender, EventArgs e) {
            ExecuteAction("KaydetBtn_Click", () =>
            {
                SaveAnnouncement(false);
            });
        }
        protected void GuncelleBtn_Click(object sender, EventArgs e) {
            ExecuteAction("GuncelleBtn_Click", () =>
            {
                SaveAnnouncement(true);
            });
        }
        protected void SilBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("SilBtn_Click", () =>
            {
                SilLbl.Text = "Duyuru silinecek";
                SilmeMesajiLbl.Text = "Duyuruyu silmek istediğinizden emin misiniz?";
                DeleteNowBtn.Visible = true;
                UtilityHelper.ScriptCalistir("OpenModal();");
            });
        }
        protected void DuyuruListBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("DuyuruListBtn_Click", () =>
            {
                RedirectToPage(ProjeConstants.PAGE_DUYURU_LIST + "?SecilenId=" + DuyuruIdQS.ConvertToInt());
            });
        }
        private void SaveAnnouncement(bool update)
        {
            Duyuru input = ReadForm(update);
            bool imageAttempted = false, imageUploaded = false, committed = false;
            try
            {
                if (xFileUpload.HasFile)
                {
                    imageAttempted = true;
                    input.Resim = PortalDuyuruImage.Upload(xFileUpload);
                    imageUploaded = true;
                }
                int savedId;
                PortalDuyuruResult result = new PortalDuyuruWriteService(LoggingContext).Save(input, imageUploaded, out savedId);
                if (result != PortalDuyuruResult.Completed)
                {
                    if (imageUploaded)
                        Messages.Warning("Resim kitaplığa yüklendi; duyuru kaydedilmedi. Kaydı ve resim kitaplığını kontrol edin.");
                    else ShowWarning(result);
                    return;
                }
                committed = true;
                DuyuruIdQS = savedId.ToString();
                DestinationAppQS = "DD";
                KaydetBtn.Visible = false;
                GuncelleBtn.Visible = true;
                SilBtn.Visible = true;
                DuyuruFormunuDoldur();
                Messages.Success(update ? "Duyuru güncellendi." : "Duyuru kaydedildi.", 4000);
            }
            catch (PortalInputException) when (!committed) { throw; }
            catch (System.Threading.ThreadAbortException) { throw; }
            catch (Exception error)
            {
                string message = committed ? "Duyuru kaydedildi; ekran yenilenemedi. Sayfayı yenileyin."
                    : imageAttempted ? "Duyuru işlemi tamamlanamadı. Kaydı ve resim kitaplığını kontrol edip tekrar deneyin."
                    : "Duyuru kaydedilemedi. Listeyi kontrol edip tekrar deneyin.";
                PortalWebPartLog.ReportError(LoggingContext, "Portal.DuyuruGirisiWP.Save", error, message);
            }
        }

        private Duyuru ReadForm(bool update)
        {
            DateTime start, end;
            TimeSpan startTime, endTime;
            var culture = new System.Globalization.CultureInfo("tr-TR");
            if (string.IsNullOrWhiteSpace(BaslikTxt.Text) || string.IsNullOrWhiteSpace(MetinTxt.Text))
                throw new PortalInputException("Duyuru başlığını ve metnini girin.");
            if (!DateTime.TryParse(YayinBasTarTxt.Value, culture, System.Globalization.DateTimeStyles.None, out start)
                || !DateTime.TryParse(YayinBitTarTxt.Value, culture, System.Globalization.DateTimeStyles.None, out end)
                || !TimeSpan.TryParse(BasSaatDDL.SelectedValue, out startTime)
                || !TimeSpan.TryParse(BitSaatDDL.SelectedValue, out endTime))
                throw new PortalInputException("Yayın başlangıç ve bitiş tarihlerini ve saatlerini kontrol edin.");
            start = start.Date.Add(startTime); end = end.Date.Add(endTime);
            if (start < new DateTime(1753, 1, 1) || end < start)
                throw new PortalInputException("Yayın bitişi başlangıçtan önce olamaz. Tarih ve saatleri kontrol edin.");
            if (SecilenPersonelList.Count == 0)
                throw new PortalInputException("Duyuru için en az bir alıcı seçin.");
            int id = update ? DuyuruIdQS.ConvertToInt() : 0;
            if (update && id <= 0) throw new PortalInputException("Duyuru bulunamadı. Listeden tekrar seçin.");
            return new Duyuru { Id = id, Baslik = BaslikTxt.Text, Metin = MetinTxt.Text,
                YayinBasTar = start, YayinBitTar = end, Tekrar = TekrarlaDDL.SelectedValue,
                DuyuruAlicilari = DuyuruAlicilariniAl(), Aktif = AktifChk.Checked, Popup = PopupChk.Checked };
        }
        private void ShowWarning(PortalDuyuruResult result)
        {
            if (result == PortalDuyuruResult.NotFound) Messages.Warning("Duyuru bulunamadı. Listeyi yenileyip tekrar seçin.");
            else if (result == PortalDuyuruResult.Forbidden) Messages.Warning("Bu duyuru işlemi için yetkiniz yok.");
            else Messages.Warning("Duyuru bilgilerini ve alıcıları kontrol edin.");
        }
        private string DuyuruAlicilariniAl()
        {
            StringBuilder idstr = new StringBuilder();
            foreach (PersonelItem item in SecilenPersonelList)
            {
                string value = item.Id.ToString();
                if (idstr.ToString() == string.Empty)
                    idstr.AppendFormat(",{0},", value);
                else
                {
                    idstr.AppendFormat("{0},", value);
                }
            }
            return idstr.ToString();
        }
        protected void DeleteNowBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("DeleteNowBtn_Click", () =>
            {
                PortalDuyuruResult result = new PortalDuyuruWriteService(LoggingContext).Delete(DuyuruIdQS.ConvertToInt());
                if (result != PortalDuyuruResult.Completed) { ShowWarning(result); return; }
                KaydetBtn.Visible = GuncelleBtn.Visible = SilBtn.Visible = DeleteNowBtn.Visible = false;
                Messages.Success("Duyuru silindi. Duyuru listesine dönebilirsiniz.", 4000);
            });
        }
        protected void SecilenResmiSilBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("SecilenResmiSilBtn_Click", () =>
            {
                PortalDuyuruResult result = new PortalDuyuruWriteService(LoggingContext).ClearImage(DuyuruIdQS.ConvertToInt());
                if (result == PortalDuyuruResult.Unchanged) { Messages.Info("Duyuruda kaldırılacak resim yok."); return; }
                if (result != PortalDuyuruResult.Completed) { ShowWarning(result); return; }
                try { DuyuruFormunuDoldur(); }
                catch (System.Threading.ThreadAbortException) { throw; }
                catch (Exception error)
                {
                    PortalWebPartLog.ReportError(LoggingContext, "Portal.DuyuruGirisiWP.RefreshAfterImage", error,
                        "Resim duyurudan kaldırıldı; ekran yenilenemedi. Sayfayı yenileyin.");
                    return;
                }
                Messages.Success("Resim duyurudan kaldırıldı.", 4000);
            });
        }
        private void RedirectToPage(string pageUrl)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path);
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + pageUrl;
            Page.Response.Redirect(newUrl);
        }
        protected void HepsiniEkleBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("HepsiniEkleBtn_Click", () =>
            {
                TümPersoneliSecilenPersonelListesineEkle();
                SecilenPersonelTablosunuDoldur();
                PersonelDDLDoldur();
            });
        }

        protected void HepsiniCikarBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("HepsiniCikarBtn_Click", () =>
            {
                TümPersoneliSecilenmeyenPersonelListesieEkle();
                SecilenPersonelTablosunuDoldur();
                PersonelDDLDoldur();
            });
        }
        [Serializable]
        class PersonelItem
        {
            public int SiraNo { get; set; }
            public string Adi { get; set; }
            public string Soyadi { get; set; }
            public int Id { get; set; }
            public PersonelItem()
            {

            }
        }


    }
}
