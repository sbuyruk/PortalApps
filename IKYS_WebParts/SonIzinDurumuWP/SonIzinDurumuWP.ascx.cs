using Model.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace IKYS_WebParts.SonIzinDurumuWP
{
    [ToolboxItemAttribute(false)]
    public partial class SonIzinDurumuWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public SonIzinDurumuWP()
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
            try
            {
                if (!Page.IsPostBack)
                {
                    TitleLbl.Text = "Son İzin Durumu ("+ DateTime.Now.ConvertToDDMMYYYHHmmFormat()+")";
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
                string adi = row["Adi"].ToString();
                string soyadi = row["Soyadi"].ToString();
                PersonelListItem personelListItem = new PersonelListItem();
                personelListItem.PersonelId = personelId.ToString();
                personelListItem.Adi = adi;
                personelListItem.Soyadi = soyadi;


                personelListItem.GecmisDonemlerdenKalanIzin = GecmisDonemlerdenKalanIzinToplamıGetir(personelId);
                personelListItem.SonIzinDonemindenKalanIzin = SonIzindenKalanIzinToplamıGetir(personelId);
                personelListItem.NetKalanIzin = personelListItem.GecmisDonemlerdenKalanIzin + personelListItem.SonIzinDonemindenKalanIzin;

                list.Add(personelListItem);
            }
            return list;
        }
        private string CreateDataTable(string jsonData)
        {
            string tableString = @"
                 jQuery(document).ready(function () {

                        jQuery('#CustomDataTable').DataTable({
                            data: " + jsonData + @",
                            columns: [
                                { data: 'Adi','width': '20%' },
                                { data: 'Soyadi','width': '20%' },
                                { data: 'GecmisDonemlerdenKalanIzin' },
                                { data: 'SonIzinDonemindenKalanIzin' },
                                { data: 'NetKalanIzin'},


                            ],
                            columnDefs: [
                                { type: 'turkish', targets:[1,2] },
                                
                            ],
                           
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
            public int GecmisDonemlerdenKalanIzin { get; set; }
            public int SonIzinDonemindenKalanIzin { get; set; }
            public int NetKalanIzin { get; set; }
        }

        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        private int GecmisDonemlerdenKalanIzinToplamıGetir(int personelId)
        {

            IzinDonem izinDonemDao = new IzinDonem();
            int kalanIzinToplami = 0;

            DataTable dataTable = izinDonemDao.SelectSUMKalanIzinByPersonelId(personelId, true);
            if (dataTable != null)
            {
                DataRow dataRow = dataTable.Rows[0];

                kalanIzinToplami = dataRow["KalanIzinToplami"].ConvertToInt();
            }

            return kalanIzinToplami;
        }
        private int SonIzindenKalanIzinToplamıGetir(int personelId)
        {
            //izinDonemini bul
            IzinDonem izinDonemi = new IzinDonem();

            DateTime tarih = DateTime.Now;
            DateTime kontrolEdilecekTarih = DateTime.Today;
            izinDonemi = izinDonemi.SelectByIzinTarihi(personelId, ProjeConstants.IZINTIPI_UCRETLI_INT, kontrolEdilecekTarih);
            
            int kalanIzinToplami = izinDonemi==null?0:izinDonemi.KalanIzin.ConvertToInt();

           

            return kalanIzinToplami;
        }
          
    }
}