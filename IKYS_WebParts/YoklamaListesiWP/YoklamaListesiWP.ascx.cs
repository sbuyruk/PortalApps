using Model.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace IKYS_WebParts.YoklamaListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class YoklamaListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public YoklamaListesiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string PersonelIdQS
        {
            get
            {

                if (ViewState["PersonelId"] == null)
                {
                    if (Page.Request.QueryString["PersonelId"] != null)
                    {
                        ViewState["PersonelId"] = Page.Request.QueryString["PersonelId"];
                    }
                    else
                    {
                        ViewState["PersonelId"] = string.Empty;
                    }
                }
                return ViewState["PersonelId"].ToString();
            }

            set
            {
                ViewState["PersonelId"] = value;
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
        private string AuthQS
        {
            get
            {

                if (ViewState["Auth"] == null || string.IsNullOrEmpty(ViewState["Auth"].ToString()))
                {
                    if (Page.Request.QueryString["Auth"] != null)
                    {
                        ViewState["Auth"] = Page.Request.QueryString["Auth"];
                    }
                    else
                    {
                        ViewState["Auth"] = string.Empty;
                    }
                }
                return ViewState["Auth"].ToString();
            }

            set
            {
                ViewState["Auth"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                if (!Page.IsPostBack)
                {
                    if (AuthQS.Equals("IKYS"))
                    {
                        TabloOlustur(ProjeConstants.HEPSI_INT);
                    }

                    else
                    {
                        Personel personel = PersonelGetir();
                        if (personel != null)
                        {
                            TabloOlustur(personel.Id);
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private void TabloOlustur(int personelId)
        {
            var jsonData = TabloJson(personelId); //veri çekilip json a çeviriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);
        }

        private string TabloJson(int personelId)
        {
            string jSon = string.Empty;


            try
            {
                List<YoklamaListItem> list = GetDataList(personelId);
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
        private List<YoklamaListItem> GetDataList(int personelId)
        {

            DataTable dataTable = GetDataTable(personelId);

            List<YoklamaListItem> list = new List<YoklamaListItem>();
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);


            foreach (DataRow row in dataTable.Rows)
            {
                int yoklamaId = row["YoklamaId"].ConvertToInt();
                int buPersonelId = row["PersonelId"].ConvertToInt();
                string adiSoyadi = row["AdiSoyadi"].ToString();
                string bulunmamaSebebi = row["BulunmamaSebebi"].ToString();
                string baslangicTarihi = row["BaslangicTarihi"].ConvertToDatetime().ToString("dd.MM.yyyy HH:mm");
                string bitisTarihi = row["BitisTarihi"].ConvertToDatetime().ToString("dd.MM.yyyy HH:mm");
                string aciklama = row["Aciklama"].ToString();



                YoklamaListItem yoklamaListItem = new YoklamaListItem();
                yoklamaListItem.YoklamaId = yoklamaId.ToString();
                yoklamaListItem.AdiSoyadi = adiSoyadi;
                yoklamaListItem.BulunmamaSebebi = bulunmamaSebebi;
                yoklamaListItem.BaslangicTarihi = baslangicTarihi;
                yoklamaListItem.BitisTarihi = bitisTarihi;
                yoklamaListItem.Aciklama = aciklama;
                yoklamaListItem.Secildi = SecilenIdQS.Equals(yoklamaListItem.YoklamaId);


                if (AuthQS.Equals("IKYS"))
                {
                    yoklamaListItem.Duzenle = "<a href=" + ProjeConstants.PAGE_YOKLAMA_GIRIS +
                        "?Auth=IKYS&DestinationApp=YokD&YoklamaId=" + yoklamaId +
                        "&PersonelId=" + buPersonelId + " class='btn btn-outline-primary'>Düzenle</a>";
                }


                list.Add(yoklamaListItem);
            }
            return list;
        }
        private class YoklamaListItem
        {
            public string YoklamaId { get; set; }
            public string PersonelId { get; set; }
            public string AdiSoyadi { get; set; }
            public string BulunmamaSebebi { get; set; }
            public string BaslangicTarihi { get; set; }
            public string BitisTarihi { get; set; }
            public string Aciklama { get; set; }
            public string Duzenle { get; set; }
            public bool Secildi { get; set; }
        }
        private DataTable GetDataTable(int personelId)
        {
            Yoklama yoklama = new Yoklama();
            DataTable dataTable = yoklama.SelectAllReturnDataTable(personelId);
            return dataTable;
        }
        private string CreateDataTable(string jsonData)
        {
            string tableString = @"
                if ( jQuery.fn.DataTable.isDataTable('#CustomDataTable') ) {
                    jQuery('#CustomDataTable').DataTable().destroy();
                }
                jQuery('#CustomDataTable tbody').empty();

                jQuery.fn.dataTable.moment('DD.MM.YYYY HH:mm');//sort date
                jQuery('#CustomDataTable').DataTable({
                    'initComplete': function (settings, json) {//tablo yüklendiğinde
                        var api = this.api();
                        var row = api.row(function (idx, data, node) { //secilen toplantıya gider
                        return data['Secildi'] == true;
                        });
                        if (row.length > 0)
                        {
                            row.select()
                                .show()
                                .draw(false);
                        }
                    },
                    data: " + jsonData + @",
                    columns: [
                        { data: 'YoklamaId' },
                        { data: 'AdiSoyadi' },
                        { data: 'BulunmamaSebebi' },
                        { data: 'BaslangicTarihi'},
                        { data: 'BitisTarihi' },
                        { data: 'Aciklama' },
                        { data: 'Duzenle' },

                    ],
                    columnDefs: [
                        { type: 'turkish', targets: [1,2] }
                    ],
                    'order': [[3, 'desc']],//sort date desc
                    'language': {
                    'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                        'decimal': ',',
                        'thousands': '.'
                    },
                    responsive: true,
                    dom: 'frtip',                    
                });
            ";

            return tableString;
        }

        private Personel PersonelGetir()
        {
            Personel personel = new Personel();

            if (!string.IsNullOrEmpty(PersonelIdQS))
            {
                personel = personel.Select<Personel>(PersonelIdQS.ConvertToInt());

            }
            else
            {
                string userName = CurrentUserName.Substring(CurrentUserName.LastIndexOf("\\") + 1, CurrentUserName.Length - CurrentUserName.LastIndexOf("\\") - 1);
                personel = personel.SelectByUserName(userName);
            }
            PersonelIdQS = personel.Id.ToString();
            return personel;
        }
        protected void BackBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_PERSONEL_LIST;
            Page.Response.Redirect(newUrl);
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
    }
}
