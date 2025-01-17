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
using static Model.IKYS.Personel;

namespace IKYS_WebParts.ProtokolSirasiWP
{
    [ToolboxItemAttribute(false)]
    public partial class ProtokolSirasiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public ProtokolSirasiWP()
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
            if (!Page.IsPostBack)
            {
                TabloOlustur();
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
        private string CreateDataTable(string jsonData)
        {
            string tableString = @"
                 jQuery(document).ready(function () {
$('#CustomDataTable').on( 'draw.dt', function () {
    //alert( 'Table redrawn' );
} );
                        jQuery('#CustomDataTable').DataTable({                            
                            data: " + jsonData + @",
                            columns: [
                                { data: 'ProtokolSiraNo' },
                                { data: 'PersonelId' },
                                { data: 'Adi' },
                                { data: 'Soyadi' },
                                { data: 'Unvan' },
                                { data: 'Gorev' },
                                { data: 'BirimSube' },

                            ],
                            columnDefs: [
                                { type: 'turkish', targets:[2,3,4,5,6] },
                                { type: 'num', targets: 0 },
                            ],
                            'language': {
                                'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                                'decimal': ',',
                                'thousands': '.'
                            },
                            responsive: true,
                            paging:false,
                            dom: 'Bfrti',
                            rowReorder: {
                                selector: 'tr',
                                update: false
                            },
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
        private List<PersonelListItem> GetDataList()
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            DateTime islemTarihiDateTime = new DateTime(2020, 1, 9);

            Personel personelDao = new Personel();
            DataTable dataTable = personelDao.SelectCalisanPersonelReturnDataTable(PersonelTipi.Kadrolu);

            List<PersonelListItem> returnlist = new List<PersonelListItem>();
            if (dataTable != null)
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    PersonelListItem personelListItem = new PersonelListItem();

                    personelListItem.ProtokolSiraNo = dataRow["ProtokolSiraNo"].ToString();
                    personelListItem.PersonelId = dataRow["PersonelId"].ToString();
                    personelListItem.Adi = dataRow["Adi"].ToString();
                    personelListItem.Soyadi = dataRow["Soyadi"].ToString();
                    personelListItem.SicilNo = dataRow["SicilNo"].ToString();
                    personelListItem.Unvan = dataRow["Unvan"].ToString();
                    personelListItem.Gorev = dataRow["Gorev"].ToString();
                    personelListItem.BirimSube = dataRow["BirimSube"].ToString();
                    returnlist.Add(personelListItem);
                }
            }
            return returnlist;
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
        private class PersonelListItem
        {
            public string ProtokolSiraNo { get; set; }
            public string PersonelId { get; set; }
            public string Adi { get; set; }
            public string Soyadi { get; set; }
            public string SicilNo { get; set; }
            public string Unvan { get; set; }
            public string Gorev { get; set; }
            public string BirimSube { get; set; }
        }

        protected void KaydetNowBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string value = paramProtokolArray.Value;
                string[] idList = value.Split(',');
                int sira = 1;
                foreach (string item in idList)
                {
                    int personelId = item.ConvertToInt();
                    if (personelId > 0)
                    {
                        IsBilgileri isb = new IsBilgileri();
                        isb = isb.SelectByPersonelId(personelId);
                        isb.ProtokolSiraNo = sira++;
                        isb.Update();
                    }

                }
                TabloOlustur();
                MessageHelper.PublishMessage("Sıralama kaydedildi", ProjeConstants.MESAJ_BASARILI, 2000);
            }
            catch (Exception)
            {
                MessageHelper.PublishMessage("Sıralama kaydedilemedi", ProjeConstants.MESAJ_HATA);
            }

        }
    }
}
