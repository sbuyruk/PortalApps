using MTS_WebParts.Logging;
using Utility.Logging;
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
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;
using Model.Services.MTS;

namespace MTS_WebParts.AramaGirisiWP
{
    [ToolboxItemAttribute(false)]
    public partial class AramaGirisiWP : WebPart
    {
        private PortalLogContext logContext;
        private PortalLogContext LogContext { get { return logContext ?? (logContext = PortalLogContext.FromCurrentRequest("MTS", "AramaGirisiWP")); } }
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public AramaGirisiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
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
        private string ArayanIdQS
        {
            get
            {

                if (ViewState["ArayanId"] == null)
                {
                    if (Page.Request.QueryString["ArayanId"] != null)
                    {
                        ViewState["ArayanId"] = Page.Request.QueryString["ArayanId"];
                    }
                    else
                    {
                        ViewState["ArayanId"] = string.Empty;
                    }
                }
                return ViewState["ArayanId"].ToString();
            }

            set
            {
                ViewState["ArayanId"] = value;
            }
        }
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
        protected void Page_Load(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "Page_Load", () =>
            {
                if (!Page.IsPostBack) MtsWebPartLog.RecordOpened(LogContext);
                IlkAcilis();
            });
        }
        private void IlkAcilis()
        {
            if (!Page.IsPostBack)
            {
                AramaGorusmeDDLDoldur();
                //FaaliyetBtnEnable();
                if (AramaGorusmeIdQS.ConvertToInt() > 0)
                {
                    //düzenleme
                    DuzenleAc();
                }
                else
                {
                    //Yeni Giris
                    GirisiAc();
                }
                FaaliyetBtnEnable();
            }
        }
        private void GirisiAc()
        {
            TitleLbl.Text = "Yeni Arama/Görüşme Girişi";
            TitleLbl.CssClass = "col-form-label text-success fw-bold mb-1";
            KaydetBtn.Visible = false;
            GuncelleBtn.Visible = false;
            SilBtn.Visible = false;
            KatilimciSecBtn.Visible = true;

            TarihTxt.Text = DateTime.Today.ConvertToDatetimeEmptyIfNull();
            SaatTxt.Text = DateTime.Now.ToString("HH:mm");
            AciklamaTxt.Text = string.Empty;
            bool katilimciBulundu = KatilimciBilgileriniDoldur();
            if (katilimciBulundu)
            {
                KaydetBtn.Visible = true;
            }
            else
            {
                KaydetBtn.Visible = false;
                GuncelleBtn.Visible = false;
                SilBtn.Visible = false;
                Utility.HelperClasses.Messages.Warning("Arama/görüşme için bir kişi seçin.");
            }
        }
        private void DuzenleAc()
        {
            TitleLbl.Text = "Arama/Görüşme Düzenleme";
            TitleLbl.CssClass = "col-form-label text-primary fw-bold mb-1";
            IdLbl.Text = "( Arama/Görüsme No: " + AramaGorusmeIdQS + " )";
            KaydetBtn.Visible = false;
            GuncelleBtn.Visible = false;
            SilBtn.Visible = false;
            KatilimciSecBtn.Visible = false;
            AramaGorusme aramaGorusme = new AramaGorusmeService(LogContext).GetById(AramaGorusmeIdQS.ConvertToInt());
            if (aramaGorusme != null)
            {
                ViewState["CallVersion"] = MtsContactWriteService.Version(aramaGorusme);
                FaaliyetIdQS = aramaGorusme.FaaliyetId.ToString();
                if (aramaGorusme.FaaliyetId > 0)
                {
                    RandevuIstendiChk.Enabled = false;
                }
                ArayanIdQS = aramaGorusme.ArayanId.ToString();
                bool katilimciBulundu = KatilimciBilgileriniDoldur();
                if (katilimciBulundu)
                {
                    TarihTxt.Text = aramaGorusme.Tarih.ConvertToDatetimeEmptyIfNull();
                    SaatTxt.Text = aramaGorusme.Tarih.ToString("HH:mm");
                    KonuTxt.Text = aramaGorusme.Konu;
                    AciklamaTxt.Text = aramaGorusme.Aciklama;
                    GorusmeSaglandiChk.Checked = aramaGorusme.GorusmeSaglandi;
                    RandevuIstendiChk.Checked = aramaGorusme.RandevuIstendi;
                    UtilityHelper.SetDDLValue(GorusmeSekliDDL, aramaGorusme.GorusmeSekli);
                    GuncelleBtn.Visible = true;
                    SilBtn.Visible = true;
                }
                else
                {
                    KaydetBtn.Visible = false;
                    GuncelleBtn.Visible = false;
                    SilBtn.Visible = false;
                    Utility.HelperClasses.Messages.Warning("Arama/görüşme için bir kişi seçin.");
                }
            }
            else
            {
                throw new MtsInputException("Arama kaydı bulunamadı. Listeyi yenileyin.");
            }

        }
        private void AramaGorusmeDDLDoldur()
        {
            ListItem li = new ListItem(ProjeConstants.ARAMAGORUSME_GELENTELEFON);
            ListItem li1 = new ListItem(ProjeConstants.ARAMAGORUSME_GIDENTELEFON);
            ListItem li2 = new ListItem(ProjeConstants.ARAMAGORUSME_YUZYUZEGORUSME);
            ListItem li3 = new ListItem(ProjeConstants.ARAMAGORUSME_YONETICIDIREKTIFI);
            GorusmeSekliDDL.Items.Clear();
            GorusmeSekliDDL.Items.Add(li);
            GorusmeSekliDDL.Items.Add(li1);
            GorusmeSekliDDL.Items.Add(li2);
            GorusmeSekliDDL.Items.Add(li3);
        }
        private bool KatilimciBilgileriniDoldur()
        {
            bool katilimciBulundu = false;
            Kisi kisi = new Kisi();
            kisi = new Model.Services.MTS.KisiService(LogContext).GetById(ArayanIdQS.ConvertToInt());
            if (kisi != null)
            {
                AdiSoyadiLnk.Text = System.Web.HttpUtility.HtmlEncode((kisi.Adi + " " + kisi.Soyadi).Trim() + " (" + kisi.Kurumu + " " + kisi.Unvani + ")");
                string currentUrl = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path);
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/";
                AdiSoyadiLnk.NavigateUrl = newUrl + ProjeConstants.PAGE_KISI_KARTI + "?KatilimciId=" + ArayanIdQS;
                katilimciBulundu = true;

                RandevuKisitliChk.Checked = kisi.RandevuKisiti.ReturnFalseIfNull().ConvertToBool();
                RandevuIstendiChk.Enabled = !RandevuKisitliChk.Checked;
            }
            else
            {
                katilimciBulundu = false;

            }
            return katilimciBulundu;
        }
        private void RedirectToPage(string pageUrl)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path);
            int index = currentUrl.LastIndexOf("?") < 0 ? currentUrl.Length : currentUrl.LastIndexOf("?");

            string rawUrl = currentUrl.Substring(0, index);
            string newUrl = rawUrl.Substring(0, rawUrl.LastIndexOf("/")) + "/" + pageUrl;
            Page.Response.Redirect(newUrl);

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
        protected void FaaliyetBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "FaaliyetBtn_Click", () =>
            {
                //faaliyet ilgisi var mi bak (varsa popup ? olarak faaliyet bilgilerini göster faaliyetya git butonina basarak o faaliyetya git)
                //
                //yoksa yeni faaliyet yaratayim mi diye sor, evetse yeni faaliyet yarat, yeni faaliyetnun irtibat kisisine bu kisiyi ekle
                if (FaaliyetIdQS.ConvertToInt() > 0)
                {
                    RedirectToPage(ProjeConstants.PAGE_FAALIYET_GIRIS + "?FaaliyetId=" + FaaliyetIdQS);
                }
                else
                {
                    ModalLbl.Text = "Yeni Faaliyet Oluşturulacak";
                    ModalLbl.CssClass = "col-form-label text-success fw-bold";
                    MessageLbl.Text = "Bu arama/görüşmeye bağlı bir faaliyet yok. Yeni faaliyet oluşturulmasını onaylıyor musunuz?";
                    OnaylaBtn.Text = "Yeni Faaliyet Oluştur";
                    OnaylaBtn.CssClass = "btn btn-outline-success";

                    kaydetGuncelleSilHdn.Value = ProjeConstants.YENI;
                    var openPopup = "OpenModalOnay();";
                    UtilityHelper.ScriptCalistir(openPopup);

                }
            });
        }
        protected void KisiListesiBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "KisiListesiBtn_Click", () =>
            {
                RedirectToPage(ProjeConstants.PAGE_KISI_LIST + "?SecilenId=" + ArayanIdQS);
            });
        }
        protected void FaaliyetListesiBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "FaaliyetListesiBtn_Click", () =>
            {
                RedirectToPage(ProjeConstants.PAGE_FAALIYET_LIST + "?SecilenId=" + FaaliyetIdQS);
            });
        }
        protected void FaaliyetTakvimiBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "FaaliyetTakvimiBtn_Click", () =>
            {
                RedirectToPage(ProjeConstants.PAGE_FAALIYET_TAKVIM + "?InitialDate=" + TarihTxt.Text);
            });
        }
        protected void AramaListesiBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "AramaListesiBtn_Click", () =>
            {
                RedirectToPage(ProjeConstants.PAGE_ARAMAGORUSME_LIST + "?SecilenId=" + AramaGorusmeIdQS);
            });
        }
        protected void KaydetBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "KaydetBtn_Click", () =>
            {
                SaveCall(false);
            });
        }
        protected void GuncelleBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "GuncelleBtn_Click", () =>
            {
                SaveCall(true);
            });
        }
        private void SaveCall(bool update)
        {
            if (object.Equals(ViewState["CallDeleted"], true)) throw new MtsInputException("Arama kaydı silindi. Listeden devam edin.");
            if (!update && AramaGorusmeIdQS.ConvertToInt() > 0) throw new MtsInputException("Kayıt oluşturuldu. Güncelleme düğmesini kullanın.");
            if (string.IsNullOrWhiteSpace(KonuTxt.Text)) throw new MtsInputException("Arama/görüşme konusunu girin.");
            int personId = ArayanIdQS.ConvertToInt();
            if (personId <= 0) throw new MtsInputException("Arama/görüşme için bir kişi seçin.");
            DateTime date; TimeSpan time;
            if (!DateTime.TryParse(TarihTxt.Text, CultureInfo.GetCultureInfo("tr-TR"), DateTimeStyles.None, out date)
                || !TimeSpan.TryParseExact(SaatTxt.Text, @"hh\:mm", CultureInfo.InvariantCulture, out time)
                || date < new DateTime(1753, 1, 1) || time.TotalHours >= 24)
                throw new MtsInputException("Geçerli tarih ve saat girin. Saat biçimi 14:30 gibi olmalıdır.");
            int id = update ? AramaGorusmeIdQS.ConvertToInt() : 0;
            if (update && id <= 0) throw new MtsInputException("Güncellenecek arama kaydını seçin.");
            var input = new AramaGorusme { Id = id, ArayanId = personId, Aciklama = AciklamaTxt.Text,
                Konu = KonuTxt.Text, GorusmeSekli = GorusmeSekliDDL.SelectedValue, GorusmeSaglandi = GorusmeSaglandiChk.Checked,
                RandevuIstendi = RandevuIstendiChk.Checked, Tarih = date.Date.Add(time) };
            int savedId;
            var result = new MtsContactWriteService(LogContext).SaveCall(input, ViewState["CallVersion"] as string, out savedId);
            MtsWebPartLog.RequireCompleted(result);
            AramaGorusmeIdQS = savedId.ToString();
            KaydetBtn.Visible = false;
            MtsWebPartLog.FinishCommitted(LogContext, update ? "Arama/görüşme güncellendi." : "Arama/görüşme kaydedildi.", () =>
            { DuzenleAc(); FaaliyetBtnEnable(); });
        }

        protected void SilBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "SilBtn_Click", () =>
            {
                var call = new AramaGorusmeService(LogContext).GetById(AramaGorusmeIdQS.ConvertToInt());
                if (call == null) throw new MtsInputException("Arama kaydı bulunamadı. Listeyi yenileyin.");
                ModalLbl.Text = "Arama/görüşme kaydı silinecek";
                ModalLbl.CssClass = "col-form-label text-danger fw-bold";
                MessageLbl.Text = (call.FaaliyetId > 0 ? "İlişkili faaliyet silinmeyecek. " : string.Empty)
                    + "Arama/görüşme kaydını silmek istediğinizden emin misiniz?";
                OnaylaBtn.Text = "Arama/görüşmeyi sil";
                OnaylaBtn.CssClass = "btn btn-outline-danger";
                kaydetGuncelleSilHdn.Value = ProjeConstants.SIL;
                UtilityHelper.ScriptCalistir("OpenModalOnay();");
            });
        }
        protected void TarihTxt_TextChanged(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "TarihTxt_TextChanged", () =>
            {

            });
        }

        private void FaaliyetBtnEnable()
        {
            FaaliyetBtn.Visible = false;
            if (AramaGorusmeIdQS.ConvertToInt() <= 0) return;
            var call = new AramaGorusmeService(LogContext).GetById(AramaGorusmeIdQS.ConvertToInt());
            if (call == null) throw new MtsInputException("Arama kaydı bulunamadı. Listeyi yenileyin.");
            FaaliyetIdQS = call.FaaliyetId.ToString();
            if (call.FaaliyetId > 0)
            {
                FaaliyetBtn.Text = "İlgili faaliyet";
                FaaliyetBtn.CssClass = "btn btn-outline-primary";
                FaaliyetBtn.Visible = true;
            }
            else if (call.RandevuIstendi && !RandevuKisitliChk.Checked)
            {
                FaaliyetBtn.Text = "Faaliyet oluştur";
                FaaliyetBtn.CssClass = "btn btn-outline-secondary";
                FaaliyetBtn.Visible = true;
            }
        }
        protected void OnaylaBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "OnaylaBtn_Click", () =>
            {
                int callId = AramaGorusmeIdQS.ConvertToInt();
                if (callId <= 0) throw new MtsInputException("İşlem için kayıtlı bir arama seçin.");
                if (kaydetGuncelleSilHdn.Value == ProjeConstants.YENI)
                {
                    int activityId;
                    var result = new MtsContactWriteService(LogContext).CreateActivity(0, callId, ViewState["CallVersion"] as string,
                        false, true, null, out activityId);
                    MtsWebPartLog.RequireCompleted(result);
                    FaaliyetIdQS = activityId.ToString();
                    OnaylaBtn.Visible = false;
                    MtsWebPartLog.FinishCommitted(LogContext, "Faaliyet oluşturuldu.", () =>
                        RedirectToPage(ProjeConstants.PAGE_FAALIYET_GIRIS + "?FaaliyetId=" + activityId));
                }
                else if (kaydetGuncelleSilHdn.Value == ProjeConstants.SIL)
                {
                    var result = new MtsContactWriteService(LogContext).DeleteCall(callId, ViewState["CallVersion"] as string);
                    MtsWebPartLog.RequireCompleted(result);
                    ViewState["CallDeleted"] = true;
                    KaydetBtn.Visible = GuncelleBtn.Visible = SilBtn.Visible = OnaylaBtn.Visible = FaaliyetBtn.Visible = false;
                    Utility.HelperClasses.Messages.Success("Arama/görüşme kaydı silindi. İlişkili faaliyet korunur. Listeden devam edebilirsiniz.");
                }
                else throw new MtsInputException("İşlemi yeniden açıp onaylayın.");
            });
        }
        protected void YeniKisiBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "YeniKisiBtn_Click", () =>
            {
                RedirectToPage(ProjeConstants.PAGE_KISI_GIRIS);
            });
        }
        #region Katilimci Seçimi
        protected void KatilimciSecBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "KatilimciSecBtn_Click", () =>
            {
                KatilimciModalAc();
            });
        }
        private void KatilimciModalAc()
        {
            TabloModalOlustur();
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "KatilimciSecimiModal();", true);
        }
        private void TabloModalOlustur()
        {
            var jsonData = TabloModalJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateModalDataTable(jsonData); //javascript kodu hazirlaniyor.
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler,
                typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), jsString, true);
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
                { data: 'KatilimciSec' }
            ],
            'language': {
                'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
            },
            responsive: true,
            dom: 'fpirt',
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
        private List<KatilimciListItem> GetModalDataList()
        {
            Kisi kisi = new Kisi();
            DataTable dataTableDis = new Model.Services.MTS.KisiService(LogContext).GetUnselectedParticipants(ProjeConstants.HEPSI_INT);

            int SiraNo = 1;
            List<KatilimciListItem> list = new List<KatilimciListItem>();
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            if (dataTableDis != null)
            {
                foreach (DataRow row in dataTableDis.Rows)
                {
                    string katilimciId = row["KatilimciId"].ToString();
                    int katilimciTipi = row["KatilimciTipi"].ConvertToInt();
                    string kurumu = row["Kurumu"].ReturnEmptyIfNull().ToString();
                    string adi = row["Adi"].ToString();
                    string soyadi = row["Soyadi"].ToString();

                    KatilimciListItem katilimciItem = new KatilimciListItem();
                    katilimciItem.Sirano = SiraNo++.ToString();
                    katilimciItem.KatilimciId = katilimciId;
                    katilimciItem.AdiSoyadi = (adi + " " + soyadi.Trim());
                    katilimciItem.Adi = adi;
                    katilimciItem.Soyadi = soyadi;
                    katilimciItem.Kurumu = kurumu;
                    katilimciItem.KatilimciTipi = katilimciTipi.ToString();

                    katilimciItem.KatilimciSec = "<a href='#' class='btn btn-outline-primary' onclick=KatilimciSecildiBtnClick(" + katilimciId + ")>SEÇ</a>";

                    list.Add(katilimciItem);
                }
            }
            return list;
        }

        protected void SecilenKatilimciyiGetirBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "SecilenKatilimciyiGetirBtn_Click", () =>
            {
                int personId;
                if (!int.TryParse(paramFaaliyetKatilimciIdLbl.Value, out personId) || personId <= 0)
                    throw new MtsInputException("Listeden bir kişi seçin.");
                RedirectToPage(ProjeConstants.PAGE_ARAMAGORUSME_GIRIS + "?ArayanId=" + personId);
            });
        }
        private class KatilimciListItem
        {
            public string Sirano { get; set; }
            public string KatilimciId { get; set; }
            public string KatilimciTipi { get; set; }
            public string AdiSoyadi { get; set; }
            public string Adi { get; set; }
            public string Soyadi { get; set; }
            public string Kurumu { get; set; }
            public string AniObjesi { get; set; }
            public string KisiKarti { get; set; }
            public string Cikar { get; set; }
            public string KatilimciSec { get; set; }
            public string IrtibatSec { get; set; }

        }
        #endregion

        protected void RandevuIstendiChk_CheckedChanged(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "RandevuIstendiChk_CheckedChanged", () =>
            {
                AramaGorusme aramaGorusme = new AramaGorusmeService(LogContext).GetById(AramaGorusmeIdQS.ConvertToInt());
                if (aramaGorusme != null)
                {
                    if (aramaGorusme.RandevuIstendi && RandevuIstendiChk.Checked)
                    {
                        FaaliyetBtn.Visible = true;
                    }
                    else
                    {
                        FaaliyetBtn.Visible = false;
                    }
                }
            });
        }
    }
}

