using Model.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace IKYS_WebParts.PersonelListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class PersonelListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public PersonelListesiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
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
                        ViewState["SecilenId"] = string.Empty;
                    }
                }
                return ViewState["SecilenId"].ToString();
            }

            set
            {
                ViewState["SecilenId"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    TabloOlustur();
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private void TabloOlustur()
        {
            var jsonData = TabloJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);
        }

        private string TabloJson()
        {
            string jSon = string.Empty;

            try
            {
                List<PersonelListItem> list = GetDataList();
                var serializer = new JavaScriptSerializer();
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
        private List<PersonelListItem> GetDataList()
        {

            DataTable dataTable = GetDataTable();

            List<PersonelListItem> list = new List<PersonelListItem>();

            foreach (DataRow row in dataTable.Rows)
            {
                int personelId = row["PersonelId"].ConvertToInt();
                int protokolSiraNo = row["ProtokolSiraNo"].ReturnZeroIfNull().ConvertToInt();
                string adi = row["Adi"].ToString();
                string soyadi = row["Soyadi"].ToString();
                string unvan = row["Unvan"].ToString();
                string birimSube = row["BirimSube"].ToString();

                int sicilNo = row["SicilNo"].ReturnZeroIfNull().ConvertToInt();
                string tahsili = row["TahsilDurumu"].ToString();
                string kullaniciAdi = row["KullaniciAdi"].ToString(); 
                string tCKimlikNo = row["TCKimlikNo"].ReturnZeroIfNull().ToString();
                string anneAdi = row["AnneAdi"].ReturnEmptyIfNull().ToString();
                string babaAdi = row["BabaAdi"].ReturnEmptyIfNull().ToString();
                string dogumYeri = row["DogumYeri"].ReturnEmptyIfNull().ToString();
                string dogumTar = row["DogumTar"].ReturnEmptyIfNull().ConvertToDatetime().ConvertToDatetimeEmptyIfNull();
                string medeniHali = row["MedeniHali"].ReturnEmptyIfNull().ConvertToInt() == 1 ? "Evli" : "Bekar";
                string evlilikTar = row["EvlilikTar"].ReturnEmptyIfNull().ConvertToDatetime().ConvertToDatetimeEmptyIfNull();
                string cinsiyet = row["Cinsiyet"].ReturnEmptyIfNull().ToString();
                string kanGrubu = row["KanGrubu"].ReturnEmptyIfNull().ToString();
                string baslamaTar = row["BaslamaTar"].ReturnEmptyIfNull().ConvertToDatetime().ConvertToDatetimeEmptyIfNull();
                string izinDonemiBasTar = row["IzinDonemiBasTar"].ReturnEmptyIfNull().ConvertToDatetime().ConvertToDatetimeEmptyIfNull();
                string sGKSicilNo = row["SGKSicilNo"].ReturnEmptyIfNull().ToString();
                string vakifOncesiPrimGunSayisi = row["VakifOncesiPrimGunSayisi"].ReturnEmptyIfNull().ToString();
                string emeklilikTarihi = row["EmeklilikTarihi"].ReturnEmptyIfNull().ConvertToDatetime().ConvertToDatetimeEmptyIfNull();
                string calismaDurumu = row["CalismaDurumu"].ReturnEmptyIfNull().ConvertToInt()==0?"Ayrıldı":"Çalışıyor";
                string ayrilmaTar = row["AyrilmaTar"].ReturnEmptyIfNull().ConvertToDatetime().ConvertToDatetimeEmptyIfNull();
                string ayrilmaSebebi = row["AyrilmaSebebi"].ReturnEmptyIfNull().ToString();
                string ceptelefonu = row["CepTelefonu"].ReturnEmptyIfNull().ToString();
                string adres = row["Adres"].ReturnEmptyIfNull().ToString();
                string ikametIli = row["IkametIli"].ReturnEmptyIfNull().ToString();
                string ikametIlcesi = row["IkametIlcesi"].ReturnEmptyIfNull().ToString();
                string esi = row["Esi"].ReturnEmptyIfNull().ToString();
                string esTcKimlikNo = row["EsTcKimlikNo"].ReturnEmptyIfNull().ToString();
                string esTelefon = row["EsTelefon"].ReturnEmptyIfNull().ToString();


                PersonelListItem personelListItem = new PersonelListItem();
                personelListItem.PersonelId = personelId.ToString();
                personelListItem.ProtokolSiraNo = protokolSiraNo;
                personelListItem.Adi = adi;
                personelListItem.Soyadi = soyadi;
                personelListItem.Unvan = unvan;
                personelListItem.BirimSube = birimSube;
                personelListItem.Secildi = SecilenIdQS.Equals(personelListItem.PersonelId);

                personelListItem.PersonelKarti = "<a href=" + ProjeConstants.PAGE_PERSONEL_KARTI + "?PersonelId=" + personelId + " class='btn btn-outline-primary'>Per.Kartı</a>";
                personelListItem.KisiselSayfa = "<a href=" + ProjeConstants.PAGE_KISISELSAYFA + "?PersonelId=" + personelId + " class='btn btn-outline-primary'>Kişis.Say.</a>";
                personelListItem.Duzenle = "<a href=" + ProjeConstants.PAGE_PERSONEL_EDIT + "?DestinationApp=PerD&PersonelId=" + personelId + " class='btn btn-outline-primary'>Düzenle</a>";
                //ekleneneler
                personelListItem.SicilNo = sicilNo;
                personelListItem.Tahsili = tahsili;
                personelListItem.KullaniciAdi = kullaniciAdi;
                personelListItem.TCKimlikNo = tCKimlikNo;
                personelListItem.AnneAdi = anneAdi;
                personelListItem.BabaAdi = babaAdi;
                personelListItem.DogumYeri = dogumYeri;
                personelListItem.DogumTar = dogumTar;
                personelListItem.MedeniHali = medeniHali;
                personelListItem.EvlilikTar = evlilikTar;
                personelListItem.Cinsiyet = cinsiyet;
                personelListItem.KanGrubu = kanGrubu;
                personelListItem.BaslamaTar = baslamaTar;
                personelListItem.IzinDonemiBasTar = izinDonemiBasTar;
                personelListItem.SGKSicilNo = sGKSicilNo;
                personelListItem.VakifOncesiPrimGunSayisi = vakifOncesiPrimGunSayisi;
                personelListItem.EmeklilikTarihi = emeklilikTarihi;
                personelListItem.CalismaDurumu = calismaDurumu;
                personelListItem.AyrilmaTar = ayrilmaTar;
                personelListItem.AyrilmaSebebi = ayrilmaSebebi;
                personelListItem.CepTelefonu= ceptelefonu;
                personelListItem.Adres = adres;
                personelListItem.IkametIli = ikametIli;
                personelListItem.IkametIlcesi = ikametIlcesi;
                personelListItem.Esi= esi;
                personelListItem.EsTcKimlikNo = esTcKimlikNo;
                personelListItem.EsTelefon = esTelefon;
                //
                list.Add(personelListItem);
            }
            return list;
        }
        private string CreateDataTable(string jsonData)
        {
            string tableString = @"
                 jQuery(document).ready(function () {

                        jQuery('#CustomDataTable').DataTable({
                            'initComplete': function (settings, json) {//tablo yüklendiğinde
                                var api = this.api();
                                var row = api.row(function (idx, data, node) { //secilen Id'ye gider
                                    return data['Secildi'] == true;
                                });
                                if (row.length > 0) {
                                    row.select()
                                        .show()
                                        .draw(false);
                                }
                            },
                            data: " + jsonData + @",
                            columns: [
                                { data: 'ProtokolSiraNo' },
                                { data: 'Adi' },
                                { data: 'Soyadi' },
                                { data: 'Unvan' },
                                { data: 'BirimSube' },
                                { data: 'PersonelKarti'},
                                { data: 'KisiselSayfa'},
                                { data: 'Duzenle'},
                                { data: 'SicilNo' },
                                { data: 'Tahsili' },
                                { data: 'KullaniciAdi' },
                                { data: 'TCKimlikNo' },
                                { data: 'AnneAdi' },
                                { data: 'BabaAdi' },
                                { data: 'DogumYeri' },
                                { data: 'DogumTar' },
                                { data: 'MedeniHali' },
                                { data: 'EvlilikTar' },
                                { data: 'Cinsiyet' },
                                { data: 'KanGrubu' },
                                { data: 'BaslamaTar' },
                                { data: 'IzinDonemiBasTar' },
                                { data: 'SGKSicilNo' },
                                { data: 'VakifOncesiPrimGunSayisi' },
                                { data: 'EmeklilikTarihi' },
                                { data: 'CalismaDurumu' },
                                { data: 'CepTelefonu' },
                                { data: 'Adres' },
                                { data: 'IkametIli' },
                                { data: 'IkametIlcesi' },
                                { data: 'Esi' },
                                { data: 'EsTcKimlikNo' },
                                { data: 'EsTelefon' },

                            ],
                            columnDefs: [
                                { type: 'turkish', targets:[1,2] },
                                { type: 'num', targets: 0 },
                                { 'visible': false, targets: [8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32]},
                            ],
                            'order': [[0, 'asc']],// Sıralı
                            'language': {
                                'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                                'decimal': ',',
                                'thousands': '.'
                            },
                            responsive: true,
                            dom: 'Bfrtip',
                            buttons: [
                                {
                                    extend: 'print',
                                    exportOptions: {
                                        columns: ':visible'
                                    }
                                },
                                {
                                    extend: 'excel',
                                    exportOptions: {
                                        columns: ':visible'
                                    }
                                },
                                {
                                    extend: 'pdf',
                                    exportOptions: {
                                        columns: ':visible'
                                    }
                                },
                                {
                                    extend: 'copy',
                                    exportOptions: {
                                        columns: ':visible'
                                    }
                                },
                                , 'pageLength', 'colvis'
                            ]
                        });
                    });";
            return tableString;
        }
        private DataTable GetDataTable()
        {
            Personel personel = new Personel();
            DataTable dataTable = personel.SelectCalisanPersonelListesiReturnDataTable();
            return dataTable;
        }
        private class PersonelListItem
        {
            public string PersonelId { get; set; }
            
            public string Adi { get; set; }
            public string Soyadi { get; set; }
            public string PersonelKarti { get; set; }
            public string KisiselSayfa { get; set; }
            public string Duzenle { get; set; }
            public int SicilNo { get; set; }
            public string Tahsili { get; set; }
            public string KullaniciAdi { get; set; }
            public string Asker_sivil { get; set; }
            public bool Secildi{ get; set; }
            //Kimlik
            public string TCKimlikNo { get; set; }
            public string AnneAdi { get; set; }
            public string BabaAdi { get; set; }
            public string DogumYeri { get; set; }
            public string DogumTar { get; set; }
            public string MedeniHali { get; set; }
            public string EvlilikTar { get; set; }
            public string Cinsiyet { get; set; }
            public string EskiSoyadi { get; set; }
            public string KanGrubu { get; set; }
            public bool DogumGunuKutlama { get; set; }
            public bool EvlilikKutlama { get; set; }
            //isBilgileri
            public int UnvanId { get; set; }
            public int GorevId { get; set; }
            public int BirimId { get; set; }
            public string BaslamaTar { get; set; }
            public string IzinDonemiBasTar { get; set; }
            public string CalismaDurumu { get; set; }
            public string AyrilmaTar { get; set; }
            public string AyrilmaSebebi { get; set; }
            public int ProtokolSiraNo { get; set; }
            public string SGKSicilNo { get; set; }
            public string SGKBasTar { get; set; }
            public string VakifOncesiPrimGunSayisi { get; set; }
            public string EmeklilikTarihi { get; set; }
            public string Unvan { get; set; }
            public string BirimSube { get; set; }
            //iletisim
            public string CepTelefonu { get; set; }
            public string Adres { get; set; }
            public string IkametIli { get; set; }
            public string IkametIlcesi { get; set; }
            //Aile
            public string Esi { get; set; }
            public string EsTcKimlikNo { get; set; }
            public string EsTelefon { get; set; }
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
        protected void ExportToExcel()
        {

            GridView GridView1 = new GridView();
            GridView1.AllowPaging = false;

            GridView1.DataSource = GetDataTable();
            GridView1.DataBind();

            Page.Response.Clear();
            Page.Response.Buffer = true;
            Page.Response.AddHeader("content-disposition",
             "attachment;filename=PersonelListesi" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls");
            Page.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1254");
            Page.Response.Charset = "windows-1254";//ISO-8859-9
            Page.Response.ContentType = "application/vnd.ms-excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);

            for (int i = 0; i < GridView1.Rows.Count; i++)
            {
                //Apply text style to each Row
                GridView1.Rows[i].Attributes.Add("class", "textmode");
            }
            GridView1.RenderControl(hw);

            //style to format numbers to string
            string style = @"<style> .textmode { mso-number-format:\@; } </style>";
            Page.Response.Write(style);
            Page.Response.Output.Write(sw.ToString());
            Page.Response.Flush();
            Page.Response.End();
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
                Page.Response.Redirect(newUrl);
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
    }
}
