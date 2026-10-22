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
using System.Data;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;
using ListItem = System.Web.UI.WebControls.ListItem;

namespace NBYS_WebParts.FTKIslemleriWP
{
    [ToolboxItemAttribute(false)]
    public partial class FTKIslemleriWP : WebPart
    {
        private PortalLogContext LoggingContext
        {
            get { return PortalLogContext.FromCurrentRequest("NBYS", "FTKIslemleriWP"); }
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
        public FTKIslemleriWP()
        {
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string FTKIslemIdQS
        {
            get
            {
                if (ViewState["FTKIslemId"] == null)
                {
                    if (Page.Request.QueryString["FTKIslemId"] != null)
                    {
                        ViewState["FTKIslemId"] = Page.Request.QueryString["FTKIslemId"];
                    }
                    else
                    {
                        ViewState["FTKIslemId"] = string.Empty;
                    }
                }
                return ViewState["FTKIslemId"].ToString();
            }
            set
            {
                ViewState["FTKIslemId"] = value;
            }
        }
        private string ValiIdQS
        {
            get
            {
                if (ViewState["ValiId"] == null)
                {
                    if (Page.Request.QueryString["ValiId"] != null)
                    {
                        ViewState["ValiId"] = Page.Request.QueryString["ValiId"];
                    }
                    else
                    {
                        ViewState["ValiId"] = string.Empty;
                    }
                }
                return ViewState["ValiId"].ToString();
            }
            set
            {
                ViewState["ValiId"] = value;
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
        private List<FTKListItem> FTKListQS
        {
            get
            {
                List<FTKListItem> list = ViewState["FTKList"] as List<FTKListItem>;
                if (list == null)
                {
                    list = new List<FTKListItem>();
                    ViewState["FTKList"] = list;
                }
                return list;
            }

            set
            {
                ViewState["FTKList"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            ExecuteAction("Page_Load", () =>
            {
                if (!Page.IsPostBack) NbysWebPartLog.RecordOpened(LoggingContext);

                if (!Page.IsPostBack) // sayfa ilk kez açiliyorsa (bu sayfanin içindeki butona basilma ani hariç)
                {
                    YonergeLnk.HRef = UtilityHelper.YonergeURLGetir(ProjeConstants.PARAM_FTKYONERGE, ProjeConstants.NBYSBELGELERI_LIB, ProjeConstants.PAGE_FTKISLEMLERI, LoggingContext);
                    IlilceBolgeDDLDoldur();
                    FTKListQS.Clear();
                    if (string.IsNullOrEmpty(FTKIslemIdQS))
                    {
                        GirisiAc();
                    }
                    else
                    {
                        DuzenleAc();
                    }
                }
            });
        }
        private bool KuruluFTKVarMi(int ili, int ilcesi)
        {
            FTKService ftkService = new FTKService(LoggingContext);
            List<FTK> list = ftkService.GetLatest(ili, ilcesi);
            return list.Count > 0;
        }
        private bool UyeKaydiVarMi(int ili, int ilcesi)
        {
            FTKKisiService ftkKisiService = new FTKKisiService(LoggingContext);
            List<FTKKisi> list = ftkKisiService.GetMembers(ili, ilcesi, 0, true);
            return list.Count > 0;
        }
        private void IlilceBolgeDDLDoldur()
        {
            IliDDL.Items.Clear();
            List<Il> list = new IlService().GetAll(LoggingContext);
            IliDDL.Items.Add(new System.Web.UI.WebControls.ListItem(string.Empty));
            foreach (Il il in list)
            {
                if (string.IsNullOrEmpty(il.IlAdi.Trim()))
                continue;
                IliDDL.Items.Add(new System.Web.UI.WebControls.ListItem(il.IlAdi, il.Id.ToString()));
            }
            UtilityHelper.SetDDLValue(IliDDL, IliIdQS);
            IlceDDLDoldur();
            BolgeTxtDoldur();
            UtilityHelper.SetDDLValue(IlcesiDDL, IlcesiIdQS);

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
        private void BolgeTxtDoldur()
        {
            Il il = new IlService().GetById(IliDDL.SelectedItem.Value.ConvertToInt(), LoggingContext);
            if (il != null)
            {
                Bolge bolge = new BolgeService().GetById(il.BolgeId, LoggingContext);
                BolgeIdTxt.Text = bolge==null?string.Empty:bolge.Id.ToString();
                SorumluBolgeTxt.Text = bolge == null ? string.Empty : bolge.Adi;
            }
        }
        private void GirisiAc()
        {
            FTKIslemleriFormunuDoldur();
            FTKListesiniVeriTabanindanDoldur();
            if (string.IsNullOrEmpty(FTKIslemIdQS))
            {
                Messages.Info("Yeni FTK bilgilerini girerek kayıt yapabilirsiniz.", 2000);
            }
        }
        private void FTKListesiniVeriTabanindanDoldur()
        {
            TabloOlustur();
        }
        private void TabloOlustur()
        {
            var jsonData = TabloJson(); //veri çekilip json'a çevriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string CreateDataTable(string jsonData)
        {
            string tableString = @"
            if ( $.fn.DataTable.isDataTable('#CustomDataTable') ) {
              $('#CustomDataTable').DataTable().destroy();
            }

            $('#CustomDataTable tbody').empty();

            jQuery('#CustomDataTable').DataTable({
            data: " + jsonData + @",
            columns: [
                { data: 'FTKGoreviId' },
                { data: 'FTKKisiId' },
                { data: 'AdiSoyadi'},
                { data: 'Gorevi'},
                { data: 'Unvani'},
                { data: 'KartNo'},
                { data: 'UyelikDurumu'},
                { data: 'Duzenle'},
            ],
            'createdRow': function(row, data, dataIndex) {
                $(row).addClass(data.Class);
            },//set row color
            'columnDefs': [
                { type: 'turkish', targets: [2,3,4] },
                { 'width': '20%', 'targets': [2,3] },
                {
                    'targets': [0],
                    'visible': false,
                    'searchable': false
                },
            ],
            'order': [[6, 'asc'],[0, 'asc'],[1, 'asc'],[2, 'asc']],
            'scrollY': '300px',
            'scrollCollapse': true,
            'paging': false,
            'language': {
                'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                'decimal': ',',
                'thousands': '.'
            },
            responsive: true,
            dom: 'srt',
            });
            ";

            return tableString;
        }
        private string TabloJson()
        {
            string jSon = string.Empty;

            FTKListQS = GetDataByIlIlce(AktifOlmayanlariGostermeChk.Checked);
            var serializer = new JavaScriptSerializer();
            jSon = serializer.Serialize(FTKListQS);

            return jSon;
        }
        private List<FTKListItem> GetDataByIlIlce(bool aktif)
        {
            int ili = IliDDL.SelectedItem.Value.ConvertToInt();
            int ilcesi = IlcesiDDL.SelectedItem.Value.ConvertToInt();
            FTKKisiService ftkKisiService = new FTKKisiService(LoggingContext);
            DataTable dataTable = ftkKisiService.GetMembersTable(ili, ilcesi, FTKIslemIdQS.ConvertToInt(), aktif);
            List<FTKListItem> list = new List<FTKListItem>();
            if (dataTable != null)
            {
                int siraNo = 1;
                foreach (DataRow row in dataTable.Rows)
                {
                    int ftkkisiId = row["FTKKisiId"].ConvertToInt();
                    string adi = row["Adi"].ToString();
                    string soyadi = row["Soyadi"].ToString();
                    int ftkGoreviId = row["FTKGorevi"].ReturnZeroIfNull().ConvertToInt();
                    string uyelikDurumu = row["UyelikDurumu"].ToString();
                    string unvani = row["Unvani"].ToString();
                    string kartNo = row["KartNo"].ReturnEmptyIfNull().ToString();
                    string aciklama = row["Aciklama"].ReturnEmptyIfNull().ToString();

                    FTKListItem katilimciItem = new FTKListItem();
                    katilimciItem.FTKKisiId = ftkkisiId;
                    katilimciItem.FTKGoreviId = ftkGoreviId;
                    katilimciItem.AdiSoyadi = adi + " " + soyadi;
                    katilimciItem.Gorevi = ParseGorevi(ftkGoreviId);
                    katilimciItem.Unvani = unvani;
                    katilimciItem.KartNo = kartNo;
                    katilimciItem.Aciklama = aciklama;
                    katilimciItem.SiraNo = siraNo++; ;

                    katilimciItem.UyelikDurumu = uyelikDurumu;

                    katilimciItem.Duzenle = "<a href='#' class='btn btn-outline-primary' onclick=FTKKisiDuzenleBtnClick(" + ftkkisiId + ")>Düzenle</a>";

                    katilimciItem.Class = !uyelikDurumu.Equals(ProjeConstants.FTK_UYELIK_DURUMU_AKTIF) ? "aktif-degil" : string.Empty;

                    list.Add(katilimciItem);

                }
            }
            return list;
        }
        private string ParseGorevi(int gorevi)
        {
            if (gorevi == ProjeConstants.FTK_GOREVI_FAHRIBASKAN_INT)
            {
                return ProjeConstants.FTK_GOREVI_FAHRIBASKAN;
            }
            else if (gorevi == ProjeConstants.FTK_GOREVI_BASKAN_INT)
            {
                return ProjeConstants.FTK_GOREVI_BASKAN;
            }
            else if (gorevi == ProjeConstants.FTK_GOREVI_GENELSEKRETER_INT)
            {
                return ProjeConstants.FTK_GOREVI_GENELSEKRETER;
            }
            else
            {
                return ProjeConstants.FTK_GOREVI_UYE;
            }

        }
        private void DuzenleAc()
        {
            if (FTKIslemIdQS.ConvertToInt() > 0)
            {
                FTKIslemleriFormunuDoldur();
                FTKListesiniVeriTabanindanDoldur();
            }
            else
            {
                GirisiAc();
            }
        }
        private void FTKIslemleriFormunuDoldur()
        {
            FTKKurulusTarihiTxt.Text = string.Empty;
            FTKGuncellemeTarihiTxt.Text = string.Empty;
            AciklamaTxt.Text = string.Empty;

            int ili = IliDDL.SelectedItem.Value.ConvertToInt();
            int ilcesi = IlcesiDDL.SelectedItem.Value.ConvertToInt();

            FTKIslemService ftkIslemService = new FTKIslemService(LoggingContext);
            FTKIslem ftkislemleri = ftkIslemService.GetByLocation(ili, ilcesi);

            if (ftkislemleri == null)
            {
                FTKIslemIdQS = string.Empty;

                IdLbl.Text = string.Empty;

                SorumluBolgeTxt.Enabled = false;
                BolgeIdTxt.Enabled = false;
                FTKKurulusTarihiTxt.Text = DateTime.Today.ConvertToDatetimeEmptyIfNull();
                FTKGuncellemeTarihiTxt.Text = DateTime.Today.ConvertToDatetimeEmptyIfNull();
                AciklamaTxt.Text = string.Empty;

            }
            else
            {
                FTKIslemIdQS = ftkislemleri.Id.ToString();
                TitleLbl.Text = "FTK İşlemleri Düzenle";
                IdLbl.Text = " ( FTK No: " + ftkislemleri.Id.ToString() + " )";
                SorumluBolgeTxt.Enabled = false;
                BolgeIdTxt.Enabled = false;
                FTKKurulusTarihiTxt.Text = ftkislemleri.KurulusTarihi.ConvertToDatetimeEmptyIfNull();
                FTKGuncellemeTarihiTxt.Text = ftkislemleri.GuncellemeTarihi.ConvertToDatetimeEmptyIfNull();

                AciklamaTxt.Text = ftkislemleri.Aciklama;

                FTKService ftkService = new FTKService(LoggingContext);
                List<FTK> ftkList = ftkService.GetLatest(ili, ilcesi);
                if (ftkList.Count > 0)
                {
                    FTK ftk = ftkList[0];
                    FTKKurulusTarihiTxt.Font.Bold = ftk.KurulusTarihi == ftkislemleri.KurulusTarihi;
                    FTKGuncellemeTarihiTxt.Font.Bold = ftk.GuncellemeTarihi != ftkislemleri.GuncellemeTarihi;
                }
            }
            ValiTxt.Text = ValiDoldur(ili);
            KaymakamTxt.Text = KaymakamDoldur(ili, ilcesi);
            BaslikGuncelle();
        }
        private string ValiDoldur(int ili)
        {
            string adiSoyadi = string.Empty;
            if (ili > 0)
            {
                FTKKisiService ftkKisiService = new FTKKisiService(LoggingContext);
                FTKKisi kisi = ftkKisiService.GetGovernor(ili);

                if (kisi == null)
                {
                    ValiIdQS = string.Empty;
                }
                else
                {
                    ValiIdQS = kisi.Id.ToString();
                    adiSoyadi = kisi.Adi + " " + kisi.Soyadi;
                }
            }
            else
            {
                Messages.Info("Önce İl seçiniz", 2000);
            }

            return adiSoyadi;
        }
        private string KaymakamDoldur(int ili, int ilcesi)
        {
            string adiSoyadi = string.Empty;

            if (ili > 0 && ilcesi > 0)
            {
                FTKKisiService ftkKisiService = new FTKKisiService(LoggingContext);
                FTKKisi kisi = ftkKisiService.GetDistrictGovernor(ili, ilcesi);

                if (kisi != null)
                {
                    adiSoyadi = kisi.Adi + " " + kisi.Soyadi;
                }

            }

            return adiSoyadi;
        }
        protected void KaydetBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("KaydetBtn_Click", () =>
            {
                ModalTarihDiv.Attributes["style"] = "display:block";
                KaydetNowBtn.Visible = true;
                TumununGoreviniSonlandirNowBtn.Visible = false;
                ModalTabloOlustur(true);
                if (FTKListQS == null || FTKListQS.Count == 0) { Messages.Warning("İşlem yapılacak aktif üye bulunamadı."); return; }
                UtilityHelper.ScriptCalistir("ModalFTKListesiAc()");
            });
        }
        #region Modal FTK Listesi
        private void ModalTabloOlustur(bool aktifMi)
        {
            ModalKurulusTarihiLbl.Text = FTKKurulusTarihiTxt.Text;
            ModalGuncellemeTarihiLbl.Text = FTKGuncellemeTarihiTxt.Text;
            var jsonData = ModalTabloJson(aktifMi); //veri çekilip json'a çevriliyor
            var jsString = CreateModalDataTable(jsonData); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);
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
                { data: 'FTKKisiId' },
                { data: 'AdiSoyadi'},
                { data: 'Gorevi'},
            ],
            'createdRow': function(row, data, dataIndex) {
                $(row).addClass(data.Class);
            },//set row color
            'order': [[2, 'asc'],[0, 'asc'],[1, 'asc']],
            'scrollY': '500px',
            'scrollCollapse': true,
            'paging': false,
            'language': {
                'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                'decimal': ',',
                'thousands': '.'
            },
            responsive: true,
            dom: 'srt',
            });
            ";

            return tableString;
        }
        private string ModalTabloJson(bool aktif)
        {
            string jSon = string.Empty;

            FTKListQS = GetDataByIlIlce(aktif);
            var serializer = new JavaScriptSerializer();
            jSon = serializer.Serialize(FTKListQS);

            return jSon;
        }
        protected void KaydetNowBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("KaydetNowBtn_Click", () =>
            {
                KaydetVeyaGuncelle();
            });
        }
        private void KaydetVeyaGuncelle()
        {
            DateTime established, updated;
            CultureInfo culture = CultureInfo.GetCultureInfo(ProjeConstants.CULTUREINFO);
            if (!DateTime.TryParse(FTKKurulusTarihiTxt.Text, culture, DateTimeStyles.None, out established) || established.Year < 1901
            || !DateTime.TryParse(FTKGuncellemeTarihiTxt.Text, culture, DateTimeStyles.None, out updated) || updated.Year < 1901)
            { Messages.Warning("Geçerli bir kuruluş tarihi ve güncelleme tarihi girin."); return; }
            FTKIslem input = new FTKIslem
            {
                Id = FTKIslemIdQS.ConvertToInt(), Ili = IliDDL.SelectedValue.ConvertToInt(), Ilcesi = IlcesiDDL.SelectedValue.ConvertToInt(),
                KurulusTarihi = established, GuncellemeTarihi = updated, Aciklama = AciklamaTxt.Text
            };
            int savedId;
            NbysFtkWriteResult result = new NbysFtkWriteService(LoggingContext).SaveCommittee(input, KayitDuzeltmesiChk.Checked, out savedId);
            NbysFtkFeedback.Show(LoggingContext, result, "FTK kaydı ve üye geçmişi kaydedildi.", () =>
            {
                FTKIslemIdQS = savedId.ToString();
                UtilityHelper.ScriptCalistir("ModalFTKListesiKapat();");
                DuzenleAc();
            });
        }

        #endregion
        private void RedirectToPage(string pageUrl)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + pageUrl;
            Page.Response.Redirect(newUrl);

        }
        protected void IliDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecuteAction("IliDDL_SelectedIndexChanged", () =>
            {
                IliIdQS = IliDDL.SelectedItem.Value;
                IlceDDLDoldur();
                BolgeTxtDoldur();
                FTKIslemleriFormunuDoldur();
                FTKListesiniVeriTabanindanDoldur();
            });
        }
        protected void IlcesiDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecuteAction("IlcesiDDL_SelectedIndexChanged", () =>
            {
                IlcesiIdQS = IlcesiDDL.SelectedItem.Value;
                FTKIslemleriFormunuDoldur();
                FTKListesiniVeriTabanindanDoldur();
            });
        }
        private void BaslikGuncelle()
        {
            bool ftkVarmi = KuruluFTKVarMi(IliDDL.SelectedItem.Value.ConvertToInt(), IlcesiDDL.SelectedItem.Value.ConvertToInt());
            bool uyeVarMi = UyeKaydiVarMi(IliDDL.SelectedItem.Value.ConvertToInt(), IlcesiDDL.SelectedItem.Value.ConvertToInt());

            if (ftkVarmi)
            {
                TitleLbl.Text = IliDDL.SelectedItem.Text + " " + IlcesiDDL.SelectedItem.Text + " FTK ";
                TitleLbl.CssClass = "col-form-label text-primary fw-bold mb-1";
                KaydetBtn.Text = IliDDL.SelectedItem.Text + " " + IlcesiDDL.SelectedItem.Text + " FTK Güncelle";
                KaydetBtn.CssClass = "btn btn-outline-primary fw-bold ";
                FTKYazilariBtn.Visible = true;
                KaydetBtn.Visible = true;
                UyelerinGoreviniSonlandirBtn.Visible = true;

                ModalFTKListesiBaslikLbl.Text = IliDDL.SelectedItem.Text + " " + IlcesiDDL.SelectedItem.Text + " FTK Listesi Güncellenecek";
                ModalFTKListesiBaslikLbl.CssClass = "col-form-label text-primary fw-bold mb-1";
                KaydetNowBtn.Text = IliDDL.SelectedItem.Text + " " + IlcesiDDL.SelectedItem.Text + " FTK Güncelle";
                KaydetNowBtn.CssClass = "btn btn-outline-primary fw-bold ";
                KayitDuzeltmeDiv.Attributes["style"] = "display:block";
            }
            else if (uyeVarMi)
            {
                TitleLbl.Text = IliDDL.SelectedItem.Text + " " + IlcesiDDL.SelectedItem.Text + " FTK ";
                TitleLbl.CssClass = "col-form-label text-success fw-bold mb-1";
                KaydetBtn.Text = IliDDL.SelectedItem.Text + " " + IlcesiDDL.SelectedItem.Text + " FTK Oluştur";
                KaydetBtn.CssClass = "btn btn-outline-success fw-bold ";
                FTKYazilariBtn.Visible = false;
                KaydetBtn.Visible = true;
                UyelerinGoreviniSonlandirBtn.Visible = true;

                ModalFTKListesiBaslikLbl.Text = IliDDL.SelectedItem.Text + " " + IlcesiDDL.SelectedItem.Text + " FTK Listesi Oluşturulacak";
                ModalFTKListesiBaslikLbl.CssClass = "col-form-label text-success fw-bold mb-1";
                KaydetNowBtn.Text = IliDDL.SelectedItem.Text + " " + IlcesiDDL.SelectedItem.Text + " FTK Oluştur";
                KaydetNowBtn.CssClass = "btn btn-outline-success fw-bold ";

                KayitDuzeltmeDiv.Attributes["style"] = "display:none";
            }
            else
            {
                TitleLbl.Text = IliDDL.SelectedItem.Text + " " + IlcesiDDL.SelectedItem.Text + " FTK ";
                TitleLbl.CssClass = "col-form-label text-danger fw-bold mb-1";
                KaydetBtn.Text = IliDDL.SelectedItem.Text + " " + IlcesiDDL.SelectedItem.Text + " FTK Oluştur";
                KaydetBtn.CssClass = "btn btn-outline-danger fw-bold ";
                FTKYazilariBtn.Visible = false;
                KaydetBtn.Visible = false;
                UyelerinGoreviniSonlandirBtn.Visible = false;

                ModalFTKListesiBaslikLbl.Text = IliDDL.SelectedItem.Text + " " + IlcesiDDL.SelectedItem.Text + " FTK Listesi Oluşturulacak";
                ModalFTKListesiBaslikLbl.CssClass = "col-form-label text-danger fw-bold mb-1";
                KaydetNowBtn.Text = IliDDL.SelectedItem.Text + " " + IlcesiDDL.SelectedItem.Text + " FTK Oluştur";
                KaydetNowBtn.CssClass = "btn btn-outline-danger fw-bold ";

                KayitDuzeltmeDiv.Attributes["style"] = "display:none";
            }
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
        protected void FTKKisiEkleBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("FTKKisiEkleBtn_Click", () =>
            {
                RedirectToPage(ProjeConstants.PAGE_FTKKISI_GIRISI + "?IliId=" + IliDDL.SelectedItem.Value + "&IlcesiId=" + IlcesiDDL.SelectedItem.Value);
            });
        }
        protected void FTKKisiDuzenleBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("FTKKisiDuzenleBtn_Click", () =>
            {
                int ftkUyeId = paramFTKIslemleriUyeIdLbl.Value.ConvertToInt();
                RedirectToPage(ProjeConstants.PAGE_FTKKISI_GIRISI + "?IliId=" + IliIdQS + "&IlcesiId=" + IlcesiIdQS + "&FTKKisiId=" + ftkUyeId);
            });
        }
        [Serializable]
        private class FTKListItem
        {
            public int FTKKisiId { get; set; }
            public int FTKGoreviId { get; set; }
            public string AdiSoyadi { get; set; }
            public string Gorevi { get; set; }
            public string Unvani { get; set; }
            public string UyelikDurumu { get; set; }
            public string Aciklama { get; set; }
            public string Duzenle { get; set; }
            public int SiraNo { get; set; }
            public string KartNo { get; set; }
            public string BaskanSec { get; set; }
            public string GenSekSec { get; set; }
            public string UyeSec { get; set; }
            public string Cikar { get; set; }
            public string Class { get; set; }
        }
        protected void AktifOlmayanlariGostermeChk_CheckedChanged(object sender, EventArgs e)
        {
            ExecuteAction("AktifOlmayanlariGostermeChk_CheckedChanged", () =>
            {
                FTKListesiniVeriTabanindanDoldur();
            });
        }
        protected void FTKListesiBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("FTKListesiBtn_Click", () =>
            {
                RedirectToPage(ProjeConstants.PAGE_FTK_LIST + "?IliId=" + IliDDL.SelectedItem.Value + "&IlcesiId=" + IlcesiDDL.SelectedItem.Value);
            });
        }
        protected void BolgelereGoreFTKRaporuBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("BolgelereGoreFTKRaporuBtn_Click", () =>
            {
                RedirectToPage(ProjeConstants.PAGE_BOLGELEREGORE_FTK_DAGILIMI);
            });
        }

        protected void FTKYazilariBtnBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("FTKYazilariBtnBtn_Click", () =>
            {
                RedirectToPage(ProjeConstants.PAGE_FTK_YAZILARI + "?IliId=" + IliDDL.SelectedItem.Value + "&IlcesiId=" + IlcesiDDL.SelectedItem.Value);
            });
        }

        protected void TumununGoreviniSonlandirNowBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("TumununGoreviniSonlandirNowBtn_Click", () =>
            {
                int[] ids = FTKListQS == null ? new int[0] : FTKListQS.Select(member => member.FTKKisiId).Distinct().ToArray();
                NbysFtkWriteResult result = new NbysFtkWriteService(LoggingContext).Deactivate(
                IliDDL.SelectedValue.ConvertToInt(), IlcesiDDL.SelectedValue.ConvertToInt(), ids);
                NbysFtkFeedback.Show(LoggingContext, result, "Seçilen üyelerin görevleri sonlandırıldı.", () =>
                {
                    UtilityHelper.ScriptCalistir("ModalFTKListesiKapat();");
                    FTKListesiniVeriTabanindanDoldur();
                });
            });
        }

        protected void UyelerinGoreviniSonlandirBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("UyelerinGoreviniSonlandirBtn_Click", () =>
            {
                //ModalTarihDiv.Attributes["style"] = "display:block";
                //KaydetNowBtn.Visible = true;
                //TumununGoreviniSonlandirNowBtn.Visible = false;
                //ModalTabloOlustur(true);
                //UtilityHelper.ScriptCalistir("ModalFTKListesiAc()"); ;

                ModalTarihDiv.Attributes["style"] = "display:none";
                KaydetNowBtn.Visible = false;
                TumununGoreviniSonlandirNowBtn.Visible = true;
                ModalFTKListesiBaslikLbl.Text = "Üyelerin görevleri sonlandırılacak";
                ModalTabloOlustur(true);
                if (FTKListQS == null || FTKListQS.Count == 0) { Messages.Warning("İşlem yapılacak aktif üye bulunamadı."); return; }
                UtilityHelper.ScriptCalistir("ModalFTKListesiAc()");
            });
        }
    }
}
