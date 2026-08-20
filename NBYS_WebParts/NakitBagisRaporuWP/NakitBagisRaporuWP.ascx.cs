using Model.NBYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.NakitBagisRaporuWP
{
    [ToolboxItemAttribute(false)]
    public partial class NakitBagisRaporuWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public NakitBagisRaporuWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    BagisTarihiBaslangicTxt.Text = DateTime.Now.AddYears(-10).ToString("yyyy-MM-dd");
                    BagisMiktariMinTxt.Text = "200";
                    SonBagisDDLDoldur();
                    SonBagisDDL.SelectedItem.Value = "3";
                    BagisTarihiBitisTxt.Text= DateTime.Now.ToString("yyyy-MM-dd");
                    SonBagisDDLDoldur();
                    SagDDLDoldur();
                    UlasilamiyorDDLDoldur();
                    TuzelKisiDDLDDLDoldur();    
                    BelgeIstemiyorDDLDoldur();
                    IlleriDoldur();
                    IlceleriDoldur();
                    ArmaganlariDoldur();
                    TabloOlustur();
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }

        private void IlleriDoldur()
        {
            IliDDL.Items.Clear();
            IliDDL.Items.Add(new ListItem("Hepsi", ProjeConstants.HEPSI_INT.ToString()));

            Il il = new Il();
            List<Il> iller = il.SelectAll<Il>().Where(x => !string.IsNullOrWhiteSpace(x.IlAdi)).OrderBy(x => x.IlAdi).ToList();
            foreach (var item in iller)
            {
                IliDDL.Items.Add(new ListItem(item.IlAdi, item.Id.ToString()));
            }
        }

        private void IlceleriDoldur()
        {
            IlcesiDDL.Items.Clear();
            IlcesiDDL.Items.Add(new ListItem("Hepsi", ProjeConstants.HEPSI_INT.ToString()));

            int ilId = IliDDL.SelectedItem.Value.ConvertToInt();
            if (ilId <= ProjeConstants.HEPSI_INT) return;

            Ilce ilce = new Ilce();
            List<Ilce> ilceler = ilce.SelectByIlId(ilId).OrderBy(x => x.IlceAdi).ToList();
            foreach (var item in ilceler)
            {
                IlcesiDDL.Items.Add(new ListItem(item.IlceAdi, item.Id.ToString()));
            }
        }
        private void SonBagisDDLDoldur()
        {
            SonBagisDDL.Items.Clear();
            SonBagisDDL.Items.Add(new ListItem("1 Ay içinde", "1"));
            SonBagisDDL.Items.Add(new ListItem("3 Ay içinde", "2"));
            SonBagisDDL.Items.Add(new ListItem("6 Ay içinde", "3"));
            SonBagisDDL.Items.Add(new ListItem("1 Yıl içinde", "4"));
            SonBagisDDL.Items.Add(new ListItem("2 Yıl içinde", "5"));
            SonBagisDDL.Items.Add(new ListItem("3 Yıl içinde", "6"));
            SonBagisDDL.Items.Add(new ListItem("5 Yıl içinde", "7"));
            SonBagisDDL.Items.Add(new ListItem("10 Yıl içinde", "10"));
        }
        private void SagDDLDoldur()
        {
            SagDDL.Items.Clear();
            SagDDL.Items.Add(new ListItem("Hepsi", ProjeConstants.HEPSI));
            SagDDL.Items.Add(new ListItem("Sağ", ProjeConstants.BAGISCI_SAG_INT));
            SagDDL.Items.Add(new ListItem("Vefat Etmiş", ProjeConstants.BAGISCI_VEFAT_INT));
        }
        private void BelgeIstemiyorDDLDoldur()
        {
            BelgeIstemiyorDDL.Items.Clear();
            BelgeIstemiyorDDL.Items.Add(new ListItem("Hepsi", ProjeConstants.HEPSI));
            BelgeIstemiyorDDL.Items.Add(new ListItem("Belge İstemeyenler", "1"));
            BelgeIstemiyorDDL.Items.Add(new ListItem("Belge İsteyenler", "0"));
        }
        private void UlasilamiyorDDLDoldur()
        {
            UlasilamiyorDDL.Items.Clear();
            UlasilamiyorDDL.Items.Add(new ListItem("Hepsi", ProjeConstants.HEPSI));
            UlasilamiyorDDL.Items.Add(new ListItem("Ulaşılabilenler", "0"));
            UlasilamiyorDDL.Items.Add(new ListItem("Ulaşılamayanlar", "1"));
        }
        private void TuzelKisiDDLDDLDoldur()
        {
            TuzelKisiDDL.Items.Clear();
            TuzelKisiDDL.Items.Add(new ListItem("Hepsi", ProjeConstants.HEPSI));
            TuzelKisiDDL.Items.Add(new ListItem("Gerçek Kişiler", "0"));
            TuzelKisiDDL.Items.Add(new ListItem("Tüzel Kişiler", "1"));
        }
        private void ArmaganlariDoldur()
        {
            ArmaganDDL.Items.Clear();
            ArmaganDDL.Items.Add(new ListItem("Hepsi", ProjeConstants.HEPSI_INT.ToString()));

            ArmaganTanim armaganTanim = new ArmaganTanim();
            List<ArmaganTanim> armaganlar = armaganTanim.SelectAktifArmaganTanim().OrderBy(x => x.Armagan).ToList();
            foreach (var item in armaganlar)
            {
                ArmaganDDL.Items.Add(new ListItem(item.Armagan, item.Id.ToString()));
            }
        }

        protected void IliDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                IlceleriDoldur();
                //TabloOlustur();
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }

        protected void ListeleBtn_Click(object sender, EventArgs e)
        {
            try
            {
                TabloOlustur();
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }

        protected void TemizleBtn_Click(object sender, EventArgs e)
        {
            try
            {
                BagisTarihiBaslangicTxt.Text = string.Empty;
                BagisTarihiBitisTxt.Text = string.Empty;
                BagisMiktariMinTxt.Text = "0";
                BagisMiktariMaxTxt.Text = string.Empty;
                ArmaganDDL.SelectedIndex = 0;
                SagDDL.SelectedIndex = 0;
                BelgeIstemiyorDDL.SelectedIndex = 0;
                UlasilamiyorDDL.SelectedIndex = 0;
                TuzelKisiDDL.SelectedIndex = 0;

                IliDDL.SelectedIndex = 0;
                IlceleriDoldur();

                TabloOlustur();
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }

        protected void ExcelBtn_Click(object sender, EventArgs e)
        {
            try
            {
                ExportToExcel();
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }

        private void ExportToExcel()
        {
            GridView GridView1 = new GridView();
            GridView1.AllowPaging = false;
            List<NakitBagisRaporuListItem> list = GetDataList();
            GridView1.DataSource = list;
            GridView1.DataBind();

            Page.Response.Clear();
            Page.Response.Buffer = true;
            Page.Response.AddHeader("content-disposition", "attachment;filename=NakitBagisRaporu.xls");
            Page.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1254");
            Page.Response.Charset = "windows-1254";
            Page.Response.ContentType = "application/vnd.ms-excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);

            for (int i = 0; i < GridView1.Rows.Count; i++)
            {
                GridView1.Rows[i].Attributes.Add("class", "textmode");
            }
            GridView1.RenderControl(hw);

            string style = @"<style> .textmode { mso-number-format:\@; } </style>";
            Page.Response.Write(style);
            Page.Response.Output.Write(sw.ToString());
            Page.Response.Flush();
            Page.Response.End();
        }

        private void TabloOlustur()
        {
            var jsonData = TabloJson();
            var jsString = CreateDataTable(jsonData);
            UtilityHelper.ScriptCalistir(jsString);
        }

        private string TabloJson()
        {
            string jSon = string.Empty;

            try
            {
                List<NakitBagisRaporuListItem> list = GetDataList();
                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;
                jSon = serializer.Serialize(list);
            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper();
                exceptionHelper.Exceptions.Add(exception);
                exceptionHelper.PublishException();
            }
            return jSon;
        }

        private string CreateDataTable(string jsonData)
        {
            string tableString = @"
                if (jQuery.fn.DataTable.isDataTable('#CustomDataTable')) {
                    jQuery('#CustomDataTable').DataTable().destroy();
                }
                jQuery('#CustomDataTable tbody').empty();

                jQuery.fn.dataTable.moment('DD.MM.YYYY');//sort date

                jQuery(document).ready(function () {
                    jQuery('#CustomDataTable').DataTable({
                        data: " + jsonData + @",
                        columns: [
                            { data: 'AdiSoyadi' },
                            { data: 'ToplamBagisMiktari' },
                            { data: 'SonBagisTarihi' },
                            { data: 'Ili' },
                            { data: 'Ilcesi' }, 
                            { data: 'Telefon' },
                            { data: 'Adres' }
                        ],
                        'order': [1, 'desc'],
                        'columnDefs': [
                            { targets: 0, className: 'btn-link' },
                            { targets: 1, className: 'bolded text-end' },
                            { 'width': '20%', 'targets': 0 },
                            { 'width': '10%', 'targets': 1 },
                            { 'width': '10%', 'targets': 2 },
                            { 'width': '10%', 'targets': 3 },
                            { 'width': '10%', 'targets': 4 },   
                            { 'width': '15%', 'targets': 5 },
                            { 'width': '25%', 'targets': 6 }
                        ],
                        'language': {
                            'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                            'decimal': ',',
                            'thousands': '.'
                        },
                        responsive: false,
                        autoWidth: false,
                        dom: 'Bfrtip',
                        buttons: [
                            {
                                extend: 'print',
                                exportOptions: { columns: ':visible' }
                            },
                            {
                                extend: 'excel',
                                exportOptions: {
                                    columns: ':visible',
                                    format: {
                                        body: function(data, row, column, node) {
                                            data = $('<p>' + data + '</p>').text();
                                            return $.isNumeric(data.replace(',', '.')) ? data.replace(',', '.') : data;
                                        }
                                    }
                                }
                            },
                            {
                                extend: 'pdf',
                                exportOptions: { columns: ':visible' }
                            },
                            {
                                extend: 'copy',
                                exportOptions: { columns: ':visible' }
                            },
                            'pageLength',
                            'colvis'
                        ]
                    });
                });
            ";

            return tableString;
        }

        private List<NakitBagisRaporuListItem> GetDataList()
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);

            DateTime? basTarih = TryParseDate(BagisTarihiBaslangicTxt.Text);
            DateTime? bitTarih = TryParseDate(BagisTarihiBitisTxt.Text);

            DateTime? SonBagisTarihi = SonBagisTarihiniGetir(SonBagisDDL.SelectedItem != null ? SonBagisDDL.SelectedItem.Value : "3");

            decimal ? minMiktar = TryParseDecimal(BagisMiktariMinTxt.Text);
            decimal? maxMiktar = TryParseDecimal(BagisMiktariMaxTxt.Text);

            int armaganId = ArmaganDDL.SelectedItem != null ? ArmaganDDL.SelectedItem.Value.ConvertToInt() : ProjeConstants.HEPSI_INT;

            int ilId = IliDDL.SelectedItem != null ? IliDDL.SelectedItem.Value.ConvertToInt() : ProjeConstants.HEPSI_INT;
            int ilceId = IlcesiDDL.SelectedItem != null ? IlcesiDDL.SelectedItem.Value.ConvertToInt() : ProjeConstants.HEPSI_INT;

            bool? sag = TryParseBool(SagDDL.SelectedItem != null ? (SagDDL.SelectedItem.Value.Equals(ProjeConstants.HEPSI)?null: SagDDL.SelectedItem.Value) : string.Empty);
            bool? belgeIstemiyor = TryParseBool(BelgeIstemiyorDDL.SelectedItem != null ? (BelgeIstemiyorDDL.SelectedItem.Value.Equals(ProjeConstants.HEPSI)?null: BelgeIstemiyorDDL.SelectedItem.Value) : string.Empty);
            bool? ulasilamiyor = TryParseBool(UlasilamiyorDDL.SelectedItem != null ? (UlasilamiyorDDL.SelectedItem.Value.Equals(ProjeConstants.HEPSI)?null: UlasilamiyorDDL.SelectedItem.Value) : string.Empty);
            bool? tuzelKisi = TryParseBool(TuzelKisiDDL.SelectedItem != null ? (TuzelKisiDDL.SelectedItem.Value.Equals(ProjeConstants.HEPSI)?null: TuzelKisiDDL.SelectedItem.Value) : string.Empty);

            NakitBagisHareket nakitBagisHareket = new NakitBagisHareket();
            DataTable dataTable = nakitBagisHareket.SelectNakitBagisRaporu(
                basTarih, bitTarih,
                minMiktar, maxMiktar,
                armaganId, SonBagisTarihi,
                ilId, ilceId,
                sag, belgeIstemiyor, ulasilamiyor, tuzelKisi);

            List<NakitBagisRaporuListItem> list = new List<NakitBagisRaporuListItem>();
            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    NakitBagisRaporuListItem item = new NakitBagisRaporuListItem();
                    string adiSoyadi = row["AdiSoyadi"].ReturnEmptyIfNull().ToString();
                    if (adiSoyadi.Contains(ProjeConstants.NAKITBAGISCI_BILINMEYEN))
                        continue;
                    int nakitBagisciId = row["NakitBagisciId"].ConvertToInt();
                    item.AdiSoyadi = "<a class='btn btn-link' onclick=OpenModal(" + nakitBagisciId + ");>" + (adiSoyadi).Trim() + "</a>";
                    item.ToplamBagisMiktari = row["ToplamBagisMiktariDecimal"].ReturnZeroIfNull().ConvertToDecimal().ToString("N", culturInfo);
                    item.SonBagisTarihi = row["SonBagisTarihi"].ReturnEmptyIfNull().ConvertToDatetimeEmptyIfNull();
                    item.Ili = row["Ili"].ReturnEmptyIfNull().ToString();
                    item.Ilcesi = row["Ilcesi"].ReturnEmptyIfNull().ToString();
                    item.Telefon = row["Telefon"].ReturnEmptyIfNull().ToString();
                    item.Adres = row["Adres"].ReturnEmptyIfNull().ToString();
                    list.Add(item);
                }
            }
            return list;
        }

        private DateTime? TryParseDate(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            DateTime result;
            if (DateTime.TryParse(value, out result)) return result;
            return null;
        }

        private decimal? TryParseDecimal(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            decimal result;
            if (decimal.TryParse(value, out result)) return result;
            return null;
        }

        private bool? TryParseBool(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            if (value == "1") return true;
            if (value == "0") return false;
            return null;
        }

        private DateTime? SonBagisTarihiniGetir(string secimDegeri)
        {
            if (string.IsNullOrWhiteSpace(secimDegeri))
                return null;

            switch (secimDegeri)
            {
                case "1": // 1 Ay
                    return DateTime.Now.AddMonths(-1);
                case "2": // 3 Ay
                    return DateTime.Now.AddMonths(-3);
                case "3": // 6 Ay
                    return DateTime.Now.AddMonths(-6);
                case "4": // 1 Yıl
                    return DateTime.Now.AddYears(-1);
                case "5": // 2 Yıl
                    return DateTime.Now.AddYears(-2);
                case "6": // 3 Yıl
                    return DateTime.Now.AddYears(-3);
                case "7": // 5 Yıl
                    return DateTime.Now.AddYears(-5);
                case "10": // 10 Yıl
                    return DateTime.Now.AddYears(-10);
                default:
                    return null;
            }
        }

        private class NakitBagisRaporuListItem
        {
            public string AdiSoyadi { get; set; }
            public string ToplamBagisMiktari { get; set; }
            public string SonBagisTarihi { get; set; }
            public string Ili { get; set; }
            public string Ilcesi { get; set; }
            public string Telefon { get; set; }
            public string Adres { get; set; }
        }
        #region Modal işlemleri
        protected void ModalDoldurBtn_Click(object sender, EventArgs e)
        {
            try
            {
                ModalTabloOlustur();
            }
            catch (Exception exception)
            {
                ExceptionHelper ex = new ExceptionHelper(exception);
                ex.PublishException();

            }
        }
        private void ModalTabloOlustur()
        {

            var jsonData = ModalTabloJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateModalDataTable(jsonData); //javascript kodu hazirlaniyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string ModalTabloJson()
        {
            string jSon = string.Empty;

            try
            {
                List<NakitBagisciListItem> list = GetModalDataList();
                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;
                jSon = serializer.Serialize(list);
            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper();
                exceptionHelper.Exceptions.Add(exception);
                exceptionHelper.PublishException();
            }
            return jSon;
        }
        private string CreateModalDataTable(string jsonData)
        {
            string tableString = @"
                

                jQuery.fn.dataTable.moment('DD.MM.YYYY');//sort date
                jQuery(document).ready(function () {
                    jQuery('#CustomModalDataTable').DataTable({
                        data: " + jsonData + @",
                        columns: [
                            { data: 'BagisTarihi' },
                            { data: 'BagisMiktari' },
                            { data: 'Armagan' },
                            { data: 'CokluBagis' },
                            { data: 'ArmaganDurumu' },
                        ],
                        'order': [[0, 'desc']],//sort

                        'language': {
                            'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                            'decimal': ',',
                            'thousands': '.'
                        },
                        responsive: true,
                        dom: 'rtp',
                        });
                    });

            ";

            return tableString;
        }
        private List<NakitBagisciListItem> GetModalDataList()
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            NakitBagisHareket nakitBagis = new NakitBagisHareket();
            DataTable dataTable = nakitBagis.SelectByNakitBagisciId(paramNakitBagisciIdLbl.Value.ConvertToInt());

            List<NakitBagisciListItem> list = new List<NakitBagisciListItem>();
            DataRow row0 = dataTable.Rows[0];
            int nakitBagisciId0 = row0["NakitBagisciId"].ConvertToInt();
            string adi0 = row0["Adi"].ToString();
            string soyadi0 = row0["Soyadi"].ToString();
            AdiLbl.Text = (adi0 + " " + soyadi0).Trim() + " (" + nakitBagisciId0 + ")";
            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    int nakitBagisciId = row["NakitBagisciId"].ConvertToInt();
                    string adi = row["Adi"].ToString();
                    string soyadi = row["Soyadi"].ToString();
                    string bagisTarihi = row["BagisTarihi"].ConvertToDatetime().ToString("dd.MM.yyyy");
                    decimal bagisMiktari = row["BagisMiktari"].ReturnZeroIfNull().ConvertToDecimal();
                    string telefon = row["Telefon1"].ToString() + " " + row["Telefon2"].ToString();
                    string ili = row["Ili"].ToString();
                    string ilcesi = row["Ilcesi"].ReturnEmptyIfNull().ToString();
                    string adres = row["Adres"].ToString();
                    string armagan = row["Armagan"].ToString();
                    string armaganDurumu = row["Durum"].ToString();
                    string cokluBagis = row["CokluBagis"].ToString();
                    bool belgeIstemiyor = row["BelgeIstemiyor"].ConvertToBool();


                    NakitBagisciListItem bagisItem = new NakitBagisciListItem();
                    bagisItem.AdiSoyadi = (adi + " " + soyadi).Trim();
                    
                    bagisItem.BagisTarihi = bagisTarihi;
                    bagisItem.BagisMiktari = bagisMiktari.ToString("N", culturInfo);

                    bagisItem.Telefon = telefon;
                    bagisItem.Ili = ili;
                    bagisItem.Ilcesi = ilcesi;
                    bagisItem.Adres = adres;
                    bagisItem.NakitBagisciId = nakitBagisciId;
                    bagisItem.BelgeIstemiyor = belgeIstemiyor ? "Belge Istemiyor" : string.Empty;
                    bagisItem.Armagan = armagan;
                    bagisItem.CokluBagis = cokluBagis;
                    bagisItem.ArmaganDurumu = armaganDurumu;
                    bagisItem.NakitBagisciId = nakitBagisciId;

                    list.Add(bagisItem);
                }
            }
            return list;
        }
        private class NakitBagisciListItem
        {
            public int NakitBagisciId { get; set; }
            public string AdiSoyadi { get; set; }
            public string BagisTarihi { get; set; }
            public string BagisMiktari { get; set; }
            public string Telefon { get; set; }
            public string Adres { get; set; }
            public string Ili { get; set; }
            public string Ilcesi { get; set; }
            public string Armagan { get; set; }
            public string CokluBagis { get; set; }
            public string ArmaganDurumu { get; set; }
            public string BelgeIstemiyor { get; set; }
        }
        #endregion
    }
}
