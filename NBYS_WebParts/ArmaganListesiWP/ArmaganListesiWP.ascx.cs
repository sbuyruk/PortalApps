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
using System.Globalization;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.ArmaganListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class ArmaganListesiWP : WebPart
    {
        private PortalLogContext LoggingContext
        {
            get { return PortalLogContext.FromCurrentRequest("NBYS", "ArmaganListesiWP"); }
        }
        private bool canProcess = true;
        private Bolge userRegion;
        private void ExecuteAction(string operation, Action action)
        {
            NbysWebPartLog.ExecuteAction(LoggingContext, operation, () => { if (canProcess) action(); });
        }
        private void StopForWarning(string message)
        {
            canProcess = false;
            Messages.Warning(message);
        }
        private bool CanReadDonor(int donorId)
        {
            return new NbysBagisciWriteService(LoggingContext).CanReadDonor(donorId, userRegion);
        }

        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public ArmaganListesiWP()
        {
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private int BolgeIdQS
        {
            get { return ViewState["BolgeId"] is int ? (int)ViewState["BolgeId"] : -1; }
            set { ViewState["BolgeId"] = value; }
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
        private string SecilenIdQS
        {
            get
            {

                if (ViewState["SecilenId"] == null)
                {
                    if (Page.Request.QueryString["SecilenId"] != null)
                    {
                        ViewState["SecilenId"] = Page.Request.QueryString["SecilenId"];
                    }
                    else
                    {
                        ViewState["SecilenId"] = "0";
                    }
                }
                int selectedId;
                return int.TryParse(Convert.ToString(ViewState["SecilenId"]), out selectedId) && selectedId > 0 ? selectedId.ToString(CultureInfo.InvariantCulture) : "0";
            }

            set
            {
                ViewState["SecilenId"] = value;
            }
        }

        private string SecilenAyQS
        {
            get
            {

                if (ViewState["SecilenAy"] == null)
                {
                    if (Page.Request.QueryString["SecilenAy"] != null)
                    {
                        ViewState["SecilenAy"] = Page.Request.QueryString["SecilenAy"];
                    }
                    else
                    {
                        ViewState["SecilenAy"] = string.Empty;
                    }
                }
                return ViewState["SecilenAy"].ToString();
            }

            set
            {
                ViewState["SecilenAy"] = value;
            }
        }
        private string SecilenYilQS
        {
            get
            {

                if (ViewState["SecilenYil"] == null)
                {
                    if (Page.Request.QueryString["SecilenYil"] != null)
                    {
                        ViewState["SecilenYil"] = Page.Request.QueryString["SecilenYil"];
                    }
                    else
                    {
                        ViewState["SecilenYil"] = string.Empty;
                    }
                }
                return ViewState["SecilenYil"].ToString();
            }

            set
            {
                ViewState["SecilenYil"] = value;
            }
        }
        private string SecilenBastarQS
        {
            get
            {

                if (ViewState["SecilenBastar"] == null)
                {
                    if (Page.Request.QueryString["SecilenBastar"] != null)
                    {
                        ViewState["SecilenBastar"] = Page.Request.QueryString["SecilenBastar"];
                    }
                    else
                    {
                        ViewState["SecilenBastar"] = string.Empty;
                    }
                }
                return ViewState["SecilenBastar"].ToString();
            }

            set
            {
                ViewState["SecilenBastar"] = value;
            }
        }
        private string SecilenBittarQS
        {
            get
            {

                if (ViewState["SecilenBittar"] == null)
                {
                    if (Page.Request.QueryString["SecilenBittar"] != null)
                    {
                        ViewState["SecilenBittar"] = Page.Request.QueryString["SecilenBittar"];
                    }
                    else
                    {
                        ViewState["SecilenBittar"] = string.Empty;
                    }
                }
                return ViewState["SecilenBittar"].ToString();
            }

            set
            {
                ViewState["SecilenBittar"] = value;
            }
        }
        private string SecilenArmaganTanimIdQS
        {
            get
            {

                if (ViewState["SecilenArmaganTanimId"] == null)
                {
                    if (Page.Request.QueryString["SecilenArmaganTanimId"] != null)
                    {
                        ViewState["SecilenArmaganTanimId"] = Page.Request.QueryString["SecilenArmaganTanimId"];
                    }
                    else
                    {
                        ViewState["SecilenArmaganTanimId"] = string.Empty;
                    }
                }
                return ViewState["SecilenArmaganTanimId"].ToString();
            }

            set
            {
                ViewState["SecilenArmaganTanimId"] = value;
            }
        }
        private string SecilenDurumQS
        {
            get
            {

                if (ViewState["SecilenDurum"] == null)
                {
                    if (Page.Request.QueryString["SecilenDurum"] != null)
                    {
                        ViewState["SecilenDurum"] = Page.Request.QueryString["SecilenDurum"];
                    }
                    else
                    {
                        ViewState["SecilenDurum"] = ProjeConstants.HEPSI;
                    }
                }
                return ViewState["SecilenDurum"].ToString();
            }

            set
            {
                ViewState["SecilenDurum"] = value;
            }
        }
        private string SecilenIlQS
        {
            get
            {

                if (ViewState["SecilenIl"] == null)
                {
                    if (Page.Request.QueryString["SecilenIl"] != null)
                    {
                        ViewState["SecilenIl"] = Page.Request.QueryString["SecilenIl"];
                    }
                    else
                    {
                        ViewState["SecilenIl"] = ProjeConstants.HEPSI_INT.ToString();
                    }
                }
                return ViewState["SecilenIl"].ToString();
            }

            set
            {
                ViewState["SecilenIl"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            ExecuteAction("Page_Load", () =>
            {
                if (!Page.IsPostBack) NbysWebPartLog.RecordOpened(LoggingContext);
                userRegion = new NbysBagisciWriteService(LoggingContext).GetUserRegion();
                if (userRegion == null) { StopForWarning("Bölge bilginiz bulunamadı. Yetki bilgilerinizi kontrol ettirin."); return; }
                BolgeIdQS = userRegion.Id;

                if (!Page.IsPostBack)
                {
                    YonergeLnk.HRef = UtilityHelper.YonergeURLGetir(ProjeConstants.PARAM_NBYSYONERGE, ProjeConstants.NBYSBELGELERI_LIB, ProjeConstants.PAGE_ARMAGANOLUSTURMA, LoggingContext);


                    SecilenIdQS = string.IsNullOrEmpty(SecilenIdQS) ? "0" : SecilenIdQS;
                    DDLleriDoldur();
                    SetDDLValues(); //ay ve yili querystringden al
                    SetSecilenBasTarBitTar();
                    TabloOlustur();
                }
            });
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("CloseBtn_Click", () =>
            {
            });
        }
        protected void ModalDoldurBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("ModalDoldurBtn_Click", () =>
            {
                int id;
                if (!int.TryParse(paramArmaganIdLbl.Value, out id) || id <= 0)
                { Messages.Warning("Detayını görmek istediğiniz armağanı seçin."); return; }
                Armagan gift = new ArmaganService(LoggingContext).GetById(id);
                if (gift == null) { Messages.Warning("Armağan kaydı bulunamadı. Listeyi yenileyin."); return; }
                if (!CanReadDonor(gift.BagisciId)) { Messages.Warning("Bu armağanın bağışçı detayını görüntüleme yetkiniz yok."); return; }
                ModalNakitBagisciFormunuDoldur(gift);
                TabloModalOlustur(gift.BagisciId.ToString());
            });
        }
        private void TabloModalOlustur(string nakitBagisciId)
        {
            var jsonData = GetModalDataJson(nakitBagisciId); //veri çekilip json'a çevriliyor
            var jsString = CreateModalDataTable(jsonData, nakitBagisciId.ConvertToInt()); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string CreateModalDataTable(string jsonData, int nakitBagisciId)
        {
            NakitBagisci nb = new NakitBagisciService(LoggingContext).GetById(nakitBagisciId);
            string bagisciAdi = nb != null ? (nb.Adi + nb.Soyadi).ReplaceTrChars() : "Bagisci";
            string filename = bagisciAdi + "-" + DateTime.Today.Day + "-" + DateTime.Today.Month + "-" + DateTime.Today.Year;
            string tableString = @"
        jQuery(document).ready(function () {
            if ( jQuery.fn.DataTable.isDataTable('#CustomModalDataTable') ) {
                jQuery('#CustomModalDataTable').DataTable().destroy();
            }
            jQuery('#CustomModalDataTable tbody').empty();

            jQuery.fn.dataTable.moment('DD.MM.YYYY');//sort date
            jQuery('#CustomModalDataTable').DataTable({
                data: " + jsonData + @",
                columns: [
                    { data: 'BagisTarihi' },
                    { data: 'BagisMiktari', 'width': '10%', 'className': 'text-end' },
                    { data: 'Armagan' },
                    { data: 'ArmaganTutari' },
                    { data: 'NBHAciklama' },
                ],
                'order': [[0, 'desc']],

                'language': {
                    'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                    'decimal': ',',
                    'thousands': '.'
                },
                columnDefs:[
                    {targets:0, render:function(data){
                        return moment(data).format('DD.MM.YYYY');
                    }},
                ],
                responsive: true,
                dom: 'frtip',
            });
        });
        ";
            return tableString;
        }
        private string GetModalDataJson(string nakitBagisciId)
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            int rowCount = 0;
            var json = new NakitBagisHareketService(LoggingContext).GetDonorDetailJson(nakitBagisciId.ConvertToInt(), ref rowCount);
            decimal toplamTutar = new NakitBagisHareketService(LoggingContext).GetTotalByBagisciIdDateRange(
                ProjeConstants.BAGIS_SORGU_BASTAR.ConvertToDatetime(), DateTime.Today, nakitBagisciId.ConvertToInt());
            BagisBilgileriLbl.Text = rowCount < 1 ? "Bağış bulunmamaktadır" :
                "Bağışçının " + ProjeConstants.BAGIS_SORGU_BASTAR.ConvertToDatetime() + " tarihinden itibaren " + rowCount + " defada yaptığı toplam " + toplamTutar.ToString("N", culturInfo) + " TL bağışı bulunmaktadır";
            return json;
        }
        //private void ModalNakitBagisTablosunuDoldur(string armaganId)
        //{
        //    IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);

        //    ModalArmaganTableHeaders();
        //    NakitBagisHareket nbh = new NakitBagisHareket();
        //    DataTable dataTable = nbh.SelectByArmaganIdReturnDataTable(armaganId.ConvertToInt());
        //    if (dataTable != null)
        //    {
        //        DataRow baslikRow = dataTable.Rows[0];
        //        string armagan = baslikRow["Armagan"].ReturnEmptyIfNull().ToString();
        //        string dovizCinsi = baslikRow["DovizCinsi"].ReturnEmptyIfNull().ToString();
        //        decimal armaganTutari = baslikRow["ArmaganTutari"].ConvertToDecimal();
        //        string armaganTutariStr = armaganTutari > 0 ? armaganTutari.ToString("N", culturInfo) + " " + dovizCinsi : string.Empty;
        //        ArmaganLbl.Text = armagan + " (" + armaganTutariStr + ")";
        //        foreach (DataRow row in dataTable.Rows)
        //        {
        //            string bagisTarihi = row["BagisTarihi"].ConvertToDatetime().ConvertToDatetimeEmptyIfNull();
        //            string bagisTutari = row["BagisTutari"].ConvertToDecimal().ToString("N", culturInfo);
        //            string banka = row["Banka"].ReturnEmptyIfNull().ToString();
        //            //string armaganTutari = row["ArmaganTutari"].ConvertToDecimal().ToString("N", culturInfo);
        //            string durum = row["Durum"].ReturnEmptyIfNull().ToString();
        //            string aciklama = row["Aciklama"].ReturnEmptyIfNull().ToString();
        //            SatirEkle(bagisTarihi, bagisTutari, banka, durum, aciklama);
        //        }
        //    }
        //}
        //private void SatirEkle(string bagisTarihi, string bagisTutari, string banka, string durum, string aciklama)
        //{
        //    TableRow row = new TableRow();
        //    TableCell bagisTarihiCell = new TableCell
        //    {
        //        Text = bagisTarihi
        //    };
        //    TableCell bagisTutariCell = new TableCell
        //    {
        //        Text = bagisTutari
        //    };
        //    TableCell bankaCell = new TableCell
        //    {
        //        Text = banka
        //    };
        //    TableCell durumCell = new TableCell
        //    {
        //        Text = durum
        //    };
        //    TableCell aciklamaCell = new TableCell
        //    {
        //        Text = aciklama
        //    };
        //    row.Controls.Add(bagisTarihiCell);
        //    row.Controls.Add(bagisTutariCell);
        //    row.Controls.Add(bankaCell);
        //    row.Controls.Add(durumCell);
        //    row.Controls.Add(aciklamaCell);
        //    ModalArmaganTable.Rows.Add(row);
        //}
        private void ModalNakitBagisciFormunuDoldur(Armagan armagan)
        {
            if (armagan!=null)
            {

                if (armagan != null)
                {
                    NakitBagisci nakitBagisci = new NakitBagisciService(LoggingContext).GetById(armagan.BagisciId);
                    //NakitBagisciIdLbl.Text = nakitBagisciId.ToString();
                    TableRow row = new TableRow();
                    TableCell AdiCell = new TableCell();
                    TableCell TCKimlikNoCell = new TableCell();
                    TableCell AdresCell = new TableCell();
                    TableCell IlIlceCell = new TableCell();
                    TableCell TelefonCell = new TableCell();
                    TableCell TuzelKisiCell = new TableCell();

                    AdiCell.Text = nakitBagisci.Adi.ReturnEmptyIfNull().ToString();
                    TCKimlikNoCell.Text = nakitBagisci.TCKimlikNo.ReturnEmptyIfNull().ToString();
                    AdresCell.Text = nakitBagisci.Adres.ReturnEmptyIfNull().ToString();

                    int ilId = nakitBagisci.Ili.ConvertToInt();
                    Il il = new IlService().GetById(ilId, LoggingContext);
                    if (il != null)
                    {

                        IlIlceCell.Text = il.IlAdi.ReturnEmptyIfNull().ToString();
                    }


                    int ilceId = nakitBagisci.Ilcesi.ConvertToInt();
                    Ilce ilce = new IlceService().GetById(ilceId, LoggingContext);
                    if (ilce != null)
                    {

                        IlIlceCell.Text += " " + ilce.IlceAdi.ReturnEmptyIfNull().ToString();
                    }


                    TelefonCell.Text = nakitBagisci.Telefon1.ReturnEmptyIfNull().ToString();
                    TuzelKisiCell.Text = nakitBagisci.TuzelKisi.ConvertToBool() ? "Evet" : "Hayır";
                    row.Controls.Add(AdiCell);
                    row.Controls.Add(TCKimlikNoCell);
                    row.Controls.Add(AdresCell);
                    row.Controls.Add(IlIlceCell);
                    row.Controls.Add(TelefonCell);
                    row.Controls.Add(TuzelKisiCell);
                    BagisciTable.Controls.Add(row);

                }

            }

        }

        private void DDLleriDoldur()
        {
            AyDDLDoldur();
            YilDDLDoldur();
            DurumDDLDoldur();
            IlDDLDoldur();
            ArmaganTanimDDLDoldur();
        }

        private void AyDDLDoldur()
        {
            AyDDL.Items.Add(new ListItem(ProjeConstants.HEPSI, ProjeConstants.HEPSI_INT.ToString()));
            AyDDL.Items.Add(new ListItem("Ocak", "1"));
            AyDDL.Items.Add(new ListItem("Şubat", "2"));
            AyDDL.Items.Add(new ListItem("Mart", "3"));
            AyDDL.Items.Add(new ListItem("Nisan", "4"));
            AyDDL.Items.Add(new ListItem("Mayıs", "5"));
            AyDDL.Items.Add(new ListItem("Haziran", "6"));
            AyDDL.Items.Add(new ListItem("Temmuz", "7"));
            AyDDL.Items.Add(new ListItem("Ağustos", "8"));
            AyDDL.Items.Add(new ListItem("Eylül", "9"));
            AyDDL.Items.Add(new ListItem("Ekim", "10"));
            AyDDL.Items.Add(new ListItem("Kasım", "11"));
            AyDDL.Items.Add(new ListItem("Aralık", "12"));

        }
        private void YilDDLDoldur()
        {
            var year = DateTime.Now.Year;
            for (int i = 1987; i <= year; i++)
            {
                YilDDL.Items.Add(new ListItem(i.ToString(), i.ToString()));
            }
        }
        private void DurumDDLDoldur()
        {
            DurumDDL.Items.Add(ProjeConstants.HEPSI);
            DurumDDL.Items.Add(ProjeConstants.DURUM_BELGE_ISTEMIYOR);
            DurumDDL.Items.Add(ProjeConstants.DURUM_GONDERILMEDI);
            DurumDDL.Items.Add(ProjeConstants.DURUM_GONDERILDI);
            DurumDDL.Items.Add(ProjeConstants.DURUM_ULASILAMADI);
            DurumDDL.Items.Add(ProjeConstants.DURUM_KONTROLEDILDI);
            DurumDDL.Items.Add(ProjeConstants.DURUM_ERKENGONDERILDI);
            DurumDDL.Items.Add(ProjeConstants.DURUM_ERTELENDI);
            //DurumDDL.Items.Add(ProjeConstants.DURUM_IADE);
            DurumDDL.Items.Add(ProjeConstants.DURUM_DAHAONCEIADE);
            DurumDDL.Items.Add(ProjeConstants.DURUM_PARAIADE);
            DurumDDL.Items.Add(ProjeConstants.DURUM_AFETNEDENIYLE_GONDERILMEDI);
            DurumDDL.Items.Add(ProjeConstants.DURUM_EDEVLETTENBELGEGONDERILDI);

            //acilista durumu querystring ile gelene esitle
            ListItem DurumItem = new ListItem();
            if (!string.IsNullOrEmpty(SecilenDurumQS))
                DurumItem = DurumDDL.Items.FindByValue(SecilenDurumQS);

            if (DurumItem != null)
                DurumDDL.SelectedValue = DurumItem.Value;
        }
        private void IlDDLDoldur()
        {
            if (IliDDL.SelectedItem == null)
            {
                IliDDL.Items.Clear();

                List<Il> list = new IlService().GetByRegion(BolgeIdQS, false, LoggingContext);
                IliDDL.Items.Add(new ListItem(ProjeConstants.HEPSI, ProjeConstants.HEPSI_INT.ToString()));
                foreach (Il il in list)
                {
                    IliDDL.Items.Add(new ListItem(il.IlAdi, il.Id.ToString()));
                }
            }

        }
        private static void SelectFilter(DropDownList list, string value, string fallback)
        {
            ListItem item = list.Items.FindByValue(value) ?? list.Items.FindByValue(fallback);
            if (item == null) throw new InvalidOperationException("Liste filtresi bulunamadı.");
            list.ClearSelection(); item.Selected = true;
        }
        private void SetDDLValues()
        {
            SelectFilter(AyDDL, SecilenAyQS, DateTime.Today.Month.ToString(CultureInfo.InvariantCulture));
            SelectFilter(YilDDL, SecilenYilQS, DateTime.Today.Year.ToString(CultureInfo.InvariantCulture));
            SelectFilter(ArmaganDDL, SecilenArmaganTanimIdQS, ProjeConstants.HEPSI);
            SelectFilter(DurumDDL, SecilenDurumQS, ProjeConstants.HEPSI);
            SelectFilter(IliDDL, SecilenIlQS, ProjeConstants.HEPSI_INT.ToString());
            SecilenAyQS = AyDDL.SelectedValue;
            SecilenYilQS = YilDDL.SelectedValue;
            SecilenArmaganTanimIdQS = ArmaganDDL.SelectedValue;
            SecilenDurumQS = DurumDDL.SelectedValue;
            SecilenIlQS = IliDDL.SelectedValue;
        }
        private void ArmaganTanimDDLDoldur()
        {
            if (ArmaganDDL.SelectedItem == null)
            {
                ArmaganDDL.Items.Clear();
                List<ArmaganTanim> list = new ArmaganTanimService(LoggingContext).GetAll();
                //Tum armaganlar secenegi
                ArmaganDDL.Items.Add(new ListItem(ProjeConstants.HEPSI, ProjeConstants.HEPSI));
                foreach (ArmaganTanim arm in list)
                {
                    if (!arm.Id.Equals(ProjeConstants.ARMAGAN_YOKID))//Armagan listesinde Armagan türü "Yok" olanlar zaten gösterilmiyor, o yüzden dropdown listesine "Yok" gelmesin
                        ArmaganDDL.Items.Add(new ListItem(arm.Armagan, arm.Id.ToString()));
                }

                //acilista armagan filtresini querystring ile gelene esitle
                ListItem ArmaganTanimItem = new ListItem();
                if (!string.IsNullOrEmpty(SecilenArmaganTanimIdQS))
                    ArmaganTanimItem = ArmaganDDL.Items.FindByValue(SecilenArmaganTanimIdQS);

                if (ArmaganTanimItem != null)
                    ArmaganDDL.SelectedValue = ArmaganTanimItem.Value;

            }

        }
        private void SetSecilenBasTarBitTar()
        {
            int ay = AyDDL.SelectedItem.Value.ConvertToInt();
            int yil = YilDDL.SelectedItem.Value.ConvertToInt();

            DateTime bastar = DateTime.Today;
            DateTime bittar = DateTime.Today;

            if (ay == 0)
            {
                bastar = new DateTime(yil, 1, 1);
                bittar = bastar.AddYears(1).AddDays(-1);
            }

            else
            {
                bastar = new DateTime(yil, ay, 1);
                DateTime sonGun = new DateTime(yil, ay, 1).AddMonths(1).AddDays(-1);
                bittar = new DateTime(yil, ay, sonGun.Day);
            }
            SecilenBastarQS = bastar.ConvertToDatetimeEmptyIfNull();
            SecilenBittarQS = bittar.ConvertToDatetimeEmptyIfNull();
        }

        protected void AyDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecuteAction("AyDDL_SelectedIndexChanged", () =>
            {
                SecilenAyQS = AyDDL.SelectedItem.Value.ToString();
                SetSecilenBasTarBitTar();
                TabloOlustur();
            });
        }
        protected void YilDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecuteAction("YilDDL_SelectedIndexChanged", () =>
            {
                SecilenYilQS = YilDDL.SelectedItem.Value.ToString();
                SetSecilenBasTarBitTar();
                TabloOlustur();
            });
        }
        protected void ArmaganDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecuteAction("ArmaganDDL_SelectedIndexChanged", () =>
            {
                SecilenArmaganTanimIdQS = ArmaganDDL.SelectedItem.Value.ToString();
                TabloOlustur();
            });
        }
        protected void DurumDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecuteAction("DurumDDL_SelectedIndexChanged", () =>
            {
                SecilenDurumQS = DurumDDL.SelectedItem.Value.ToString();
                TabloOlustur();
            });
        }
        protected void IliDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecuteAction("IliDDL_SelectedIndexChanged", () =>
            {
                SecilenIlQS = IliDDL.SelectedItem.Value.ToString();
                TabloOlustur();
            });
        }
        protected void IadeEdildiYapBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("IadeEdildiYapBtn_Click", () =>
            {
                int id;
                if (!int.TryParse(paramArmaganIdLbl.Value, out id) || id <= 0)
                { Messages.Warning("Belgesi iade edilen armağan kaydını seçin."); return; }
                NbysGiftWriteResult result = new NbysGiftWriteService(LoggingContext).MarkCertificateReturned(id);
                NbysGiftFeedback.Show(LoggingContext, result, "Armağanın durumu ‘Belgesi İade Edildi’ olarak güncellendi.", TabloOlustur);
            });
        }
        protected void EtiketOlusturBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("EtiketOlusturBtn_Click", () =>
            {
                var queryString = string.Format("?Bastar={0}&Bittar={1}", SecilenBastarQS, SecilenBittarQS);
                RedirectToPage(ProjeConstants.PAGE_ARMAGANETIKET_VIEWER + queryString);
            });
        }
        protected void ArmaganListesiBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("ArmaganListesiBtn_Click", () =>
            {
                string tarihStr = SecilenBastarQS.ConvertToDatetimeEmptyIfNull() + " - " + SecilenBittarQS.ConvertToDatetimeEmptyIfNull();
                var queryString = string.Format("?Bastar={0}&Bittar={1}&TarihStr={2}", SecilenBastarQS, SecilenBittarQS, tarihStr);
                RedirectToPage(ProjeConstants.PAGE_ARMAGANLISTESI_VIEWER + queryString);
            });
        }
        private void RedirectToPage(string pageUrl)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            //string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + pageUrl;

            string newUrl = "http://tskgv-portal/YonetimBirimleri/BasinTanitimHalklaIliskilerSubesi/Sayfalar" + "/" + pageUrl;
            ResponseHelper.Redirect(newUrl, "_blank", "");
        }
        #region Liste Olusturma
        private void TabloOlustur()
        {
            var jsonData = TabloJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazirlaniyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string TabloJson()
        {
            int rowCount = 0;

            int ay = AyDDL.SelectedItem.Value.ConvertToInt();
            int yil = YilDDL.SelectedItem.Value.ConvertToInt();

            DateTime bastar = DateTime.Today;
            DateTime bittar = DateTime.Today;

            if (ay == 0)
            {
                bastar = new DateTime(yil, 1, 1);
                bittar = bastar.AddYears(1).AddDays(-1);
            }
            else
            {

                bastar = new DateTime(yil, ay, 1);
                DateTime basGun = new DateTime(yil, ay, 1).AddMonths(1).AddDays(-1);
                bittar = new DateTime(yil, ay, basGun.Day);

            }
            int ili = IliDDL.SelectedValue.ConvertToInt();
            var json = new ArmaganService(LoggingContext).ListByDurumTarihJson(DurumDDL.SelectedItem.Text, bastar, bittar, ArmaganDDL.SelectedItem.Value, ref rowCount, BolgeIdQS, ili);
            return json;

        }
        private string CreateDataTable(string jsonData)
        {
            string reportTesekkurUrl = "http://tskgv-portal/YonetimBirimleri/BasinTanitimHalklaIliskilerSubesi/Sayfalar" + "/" + ProjeConstants.PAGE_TESEKKURBELGESITEK_VIEWER + "?ArmaganId=";
            string reportBeratUrl = "http://tskgv-portal/YonetimBirimleri/BasinTanitimHalklaIliskilerSubesi/Sayfalar" + "/" + ProjeConstants.PAGE_BERATBELGESITEK_VIEWER + "?ArmaganId=";
            bool yetkiliMi = BolgeIdQS == ProjeConstants.HEPSI_INT || BolgeIdQS == ProjeConstants.BOLGE_GENELMUDURLUK_INT;
            string belgeStr = string.Empty;
            if (yetkiliMi)
            {
                belgeStr = @"
                    if(row.Durum.toString().indexOf('Kontrol Edildi')>=0){
                        if(row.ArmaganBaslik.toString().indexOf('Teşekkür Belgesi')>=0)
                        {
                            var url='" + reportTesekkurUrl + @"'+row.ArmaganId;
                            link='<a target=_blank href='+url+' class=btn-link >'+row.ArmaganBaslik+'</a>';
                        }
                        else if(row.ArmaganBaslik.toString().indexOf('Bronz Madalya ve Berati')>=0 || 
                            row.ArmaganBaslik.toString().indexOf('Gümüş Madalya ve Berati')>=0 || 
                            row.ArmaganBaslik.toString().indexOf('Altın Madalya ve Berati')>=0)
                        {
                            var url='" + reportBeratUrl + @"'+row.ArmaganId;
                            link='<a target=_blank href='+url+' class=btn-link >'+row.ArmaganBaslik+'</a>';
                        }
                    }
                ";
            }

            string queryStr = "&SecilenAy=" + SecilenAyQS + "&SecilenYil="
                + SecilenYilQS + "&SecilenArmaganTanimId=" + SecilenArmaganTanimIdQS + "&SecilenDurum=" + System.Web.HttpUtility.UrlEncode(SecilenDurumQS) + "&SecilenIl=" + SecilenIlQS;

            string tableString = @"
                if ( jQuery.fn.DataTable.isDataTable('#CustomDataTable') ) {
                    jQuery('#CustomDataTable').DataTable().destroy();
                }
                jQuery('#CustomDataTable tbody').empty();

                jQuery.fn.dataTable.moment('DD.MM.YYYY');//sort date

                jQuery('#CustomDataTable').DataTable({
                    'initComplete': function (settings, json) {//tablo yüklendiğinde
                        var api = this.api();
                        var row = api.row(function(idx, data, node) { //seçilen satıra gider
                            return data['ArmaganId'] ==" + SecilenIdQS + @";
                        });
                        if (row.length > 0)
                        {
                            row.select()
                                .show()
                                .draw(false);
                        }
                    },
                    columnDefs:[
                        {targets:0, render:function(data, type, row, meta){
                            if (row.Durum == 'Parasi Iade Edildi'){
                                return data;
                            }else
                            {
                                return ('<a href=# onclick=OpenModal('+row.ArmaganId+'); type=button class=\'btn btn-link fw-bold\'>'+data+'</a>')
                            }
                        }},
                        {targets:4, render:function(data){
                            return moment(data).format('DD.MM.YYYY');
                        }},
                        {targets:5, render:function(data, type, row, meta){
                            var link= '<span class=grayLayout>'+row.ArmaganBaslik+'</span>';
                            "+
                            belgeStr
                            +@"
                            return link;
                        }},
                        {targets:8, render:function(data, type, row, meta){
                            var link= '';
                            if (row.Durum == 'Parasi Iade Edildi'){
                                return 'Belge Geçersiz';
                            }else
                            {
                                link='<a href='+'" + ProjeConstants.PAGE_ARMAGAN_EDIT + @"?ArmaganId='+row.ArmaganId+'&NakitBagisciId='+row.NakitBagisciId+'" + queryStr + @" class=\'btn btn-outline-primary\' >Düzenle</a>';
                            }

                            return link;
                        }},
                        {targets:9, render:function(data, type, row, meta){
                            var link='';
                            if (row.Durum==='Gönderildi')
                            {
                                link='<a href=# onclick=CallButtonClick('+row.ArmaganId + '); class=\'btn btn-outline-danger \'>Belge İadesi</a>';
                            }else if (row.Durum == 'Parasi Iade Edildi')
                            {
                                link= '<span>'+row.IadeMiktari+ ' '+row.DovizCinsi +' İade edildi</span>';
                            }       
                            return link;
                        }}],
                    data: " + jsonData + @",
                    columns: [
                        { data: 'NakitBagisciAdi'},
                        { data: 'BelgedeYazanIsim', 'width':'10%' },
                        { data: 'NakitBagisciTC' },
                        { data: 'Tutar', 'width':'10%', 'className': 'text-end'},
                        { data: 'Tarih' },
                        { data: 'ArmaganBaslik' , 'width':'20%'},
                        { data: 'Durum' },
                        { data: 'CokluBagis' },
                        { data: 'ArmaganId' },
                        { data: 'ArmaganId' },
                    ],
                    'order': [[4, 'asc']],//sort date desc
                    'language': {
                    'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                        'decimal': ',',
                        'thousands': '.'
                    },
                    responsive: true,
                    dom: 'frtip',   
                    //stateSave: true,
                    'createdRow': function(row, data, dataIndex) {
                        var durum = data.Durum;
                        if (durum == 'Kontrol Edildi') {

                            $(row).addClass('kontrol-edildi');
                        }
                        else if (durum == 'Gönderildi') {
                            $(row).addClass('gonderildi');
                        }
                        else if (durum == 'Erken Gönderildi') {
                            $(row).addClass('gonderildi');
                        }
                        else if (durum == 'Gönderilmedi') {
                            //beyaz kalsın
                        }
                        else if (durum == 'Parasi Iade Edildi') {
                            $(row).addClass('belge-gecersiz');
                        }
                        else {
                            $(row).addClass('diger');
                        }
                        // bu iller AFET nedeniyle ekranda renkli olsun
                        //var ili = data.IlId;
                        //if ((ili==1) ||(ili==2) ||(ili==21) ||(ili==27) ||(ili==31) ||(ili==44) ||(ili==46) ||(ili==63) ||(ili==79) ||(ili==80) )
                        //{
                        //    $(row).addClass('afet-ili');
                        //}
                    },//set row color
                });

            ";

            return tableString;
        }
        #endregion
        //private void ModalArmaganTableHeaders()
        //{
        //    ModalArmaganTable.Rows.Clear();
        //    TableHeaderRow headerRow = new TableHeaderRow();

        //    TableHeaderCell bagisTarihiCell = new TableHeaderCell
        //    {
        //        Text = "Bagis Tarihi"
        //    };

        //    TableHeaderCell BagisMiktariCell = new TableHeaderCell
        //    {
        //        Text = "Bagis Miktari"
        //    };

        //    TableHeaderCell bankaCell = new TableHeaderCell
        //    {
        //        Text = "Banka"
        //    };

        //    TableHeaderCell durumCell = new TableHeaderCell
        //    {
        //        Text = "Durum"
        //    };
        //    TableHeaderCell aciklamaCell = new TableHeaderCell
        //    {
        //        Text = "Açiklama"
        //    };
        //    headerRow.Controls.Add(bagisTarihiCell);
        //    headerRow.Controls.Add(BagisMiktariCell);
        //    headerRow.Controls.Add(bankaCell);
        //    headerRow.Controls.Add(durumCell);
        //    headerRow.Controls.Add(aciklamaCell);

        //    ModalArmaganTable.Controls.Add(headerRow);

        //}
    }
}
