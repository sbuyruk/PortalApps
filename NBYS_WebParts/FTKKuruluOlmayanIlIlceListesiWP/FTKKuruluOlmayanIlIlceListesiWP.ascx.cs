using Model.NBYS;
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

namespace NBYS_WebParts.FTKKuruluOlmayanIlIlceListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class FTKKuruluOlmayanIlIlceListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public FTKKuruluOlmayanIlIlceListesiWP()
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
            get
            {

                if (ViewState["BolgeId"] == null)
                {
                    if (Page.Request.QueryString["BolgeId"] != null)
                    {
                        ViewState["BolgeId"] = Page.Request.QueryString["BolgeId"];
                    }
                    else
                    {
                        ViewState["BolgeId"] = string.Empty;
                    }
                }
                return ViewState["BolgeId"].ReturnZeroIfNull().ConvertToInt();
            }

            set
            {
                ViewState["BolgeId"] = value;
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
            if (!Page.IsPostBack)
            {
                Bolge bolge = IKYSOrtak.BolgeGetirByUserName(CurrentUserName);
                BolgeIdQS = bolge == null ? 0 : bolge.Id;
                BolgeDDLDoldur();

                UtilityHelper.SetDDLValue(BolgeDDL, BolgeIdQS.ToString());
                IlDDLDoldur();
                UtilityHelper.SetDDLValue(IliDDL, IliIdQS);
                IlceDDLDoldur();
                UtilityHelper.SetDDLValue(IlcesiDDL, IlcesiIdQS);
                TabloOlustur();
            }

        }
        private void BolgeDDLDoldur()
        {
            BolgeDDL.Items.Clear();
            Bolge bolgeDao = new Bolge();
            List<Bolge> list = bolgeDao.SelectAktifBolgeler(BolgeIdQS);
            foreach (Bolge item in list)
            {
                if (string.IsNullOrEmpty(item.Adi.Trim()))
                    continue;
                BolgeDDL.Items.Add(new ListItem(item.Adi, item.Id.ToString()));
            }


        }
        private void IlDDLDoldur()
        {
            IliDDL.Items.Clear();

            ListItem li0 = new ListItem(ProjeConstants.HEPSI, ProjeConstants.HEPSI_INT.ToString());
            IliDDL.Items.Add(li0);

            int bolgeId = !string.IsNullOrEmpty(BolgeDDL.SelectedItem.Value) ? BolgeDDL.SelectedItem.Value.ConvertToInt() : 0;

            Il newil = new Il();
            List<Il> list = newil.SelectByBolgeId(bolgeId);
            foreach (Il il in list)
            {
                if (string.IsNullOrEmpty(il.IlAdi.Trim()))
                    continue;
                IliDDL.Items.Add(new ListItem(il.IlAdi, il.Id.ToString()));
            }
        }
        private void IlceDDLDoldur()
        {
            IlcesiDDL.Items.Clear();
            Ilce pilce = new Ilce();


            ListItem li0 = new ListItem(ProjeConstants.HEPSI, ProjeConstants.HEPSI_INT.ToString());
            ListItem li1 = new ListItem(ProjeConstants.VALILIK, ProjeConstants.VALILIK_INT.ToString());
            ListItem li2 = new ListItem(ProjeConstants.SADECE_ILCELER, ProjeConstants.SADECE_ILCELER_INT.ToString());
            IlcesiDDL.Items.Add(li0);
            IlcesiDDL.Items.Add(li1);
            IlcesiDDL.Items.Add(li2);

            List<Ilce> list = pilce.SelectByIlId(IliDDL.SelectedValue.ConvertToInt());
            foreach (Ilce ilce in list)
            {
                if (ilce.IlceAdi.ToUpper().Equals(ProjeConstants.ILCE_MERKEZ.ToUpper()))
                    continue;
                IlcesiDDL.Items.Add(new ListItem(ilce.IlceAdi, ilce.Id.ToString()));
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


                List<IlIlceItem> list = new List<IlIlceItem>();
                if (IlcesiIdQS.ConvertToInt() != ProjeConstants.SADECE_ILCELER_INT)
                {
                    List<IlIlceItem> illist = GetIlDataList();
                    list.AddRange(illist);
                }

                IlcesiIdQS = IlcesiDDL.SelectedValue;
                if (IlcesiIdQS.ConvertToInt() != ProjeConstants.VALILIK_INT)
                {
                    List<IlIlceItem> ilcelist = new List<IlIlceItem>();
                    ilcelist = GetIlceDataList();
                    list.AddRange(ilcelist);
                }

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
        private List<IlIlceItem> GetIlDataList()
        {
            List<IlIlceItem> liste = new List<IlIlceItem>();
            FTK ftkDao = new FTK();
            DataTable dataTable = ftkDao.SelectFTKKuruluOlmayanIller(BolgeDDL.SelectedItem.Value.ConvertToInt(), IliDDL.SelectedItem.Value.ConvertToInt());
            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    string bolge = row["Bolge"].ToString();
                    int iliId = row["IlId"].ConvertToInt();
                    int ilcesiId = ProjeConstants.VALILIK_INT;

                    string ilAdi = row["IlAdi"].ToString();
                    string ilceAdi = ProjeConstants.VALILIK;


                    IlIlceItem ilIlceListItem = new IlIlceItem();
                    ilIlceListItem.Bolge = bolge;
                    ilIlceListItem.IlId = iliId;
                    ilIlceListItem.IlceId = ilcesiId;
                    ilIlceListItem.Ili = ilAdi;
                    ilIlceListItem.Ilcesi = (ilcesiId == ProjeConstants.VALILIK_INT ? ProjeConstants.VALILIK : ilceAdi);

                    liste.Add(ilIlceListItem);
                }
            }
            return liste;
        }
        private List<IlIlceItem> GetIlceDataList()
        {
            List<IlIlceItem> liste = new List<IlIlceItem>();
            FTK ftkDao = new FTK();
            DataTable dataTable = ftkDao.SelectFTKKuruluOlmayanIlceler(BolgeDDL.SelectedItem.Value.ConvertToInt(), IliDDL.SelectedItem.Value.ConvertToInt());
            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    string bolge = row["Bolge"].ToString();
                    int iliId = row["IlId"].ConvertToInt();
                    int ilcesiId = row["IlceId"].ConvertToInt();

                    string ilAdi = row["IlAdi"].ToString();
                    string ilceAdi = row["IlceAdi"].ToString();

                    IlIlceItem ilIlceListItem = new IlIlceItem();
                    ilIlceListItem.Bolge = bolge;
                    ilIlceListItem.IlId = iliId;
                    ilIlceListItem.IlceId = ilcesiId;
                    ilIlceListItem.Ili = ilAdi;
                    ilIlceListItem.Ilcesi = ilceAdi;//(ilcesiId == ProjeConstants.VALILIK_INT ? ProjeConstants.VALILIK : ilceAdi);

                    liste.Add(ilIlceListItem);
                }
            }
            return liste;
        }
        private int ParseGorevi(string gorevi)
        {
            if (gorevi == ProjeConstants.FTK_GOREVI_FAHRIBASKAN)
            {
                return ProjeConstants.FTK_GOREVI_FAHRIBASKAN_INT;
            }
            else if (gorevi == ProjeConstants.FTK_GOREVI_BASKAN)
            {
                return ProjeConstants.FTK_GOREVI_BASKAN_INT;
            }
            else if (gorevi == ProjeConstants.FTK_GOREVI_GENELSEKRETER)
            {
                return ProjeConstants.FTK_GOREVI_GENELSEKRETER_INT;
            }
            else
            {
                return ProjeConstants.FTK_GOREVI_UYE_INT;
            }

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
                    data: " + jsonData + @",
                    columns: [
                        { data: 'IlId' },
                        { data: 'IlceId' },
                        { data: 'Bolge' },
                        { data: 'Ili' },
                        { data: 'Ilcesi' },
                    ],
                    'columnDefs': [
                        { type: 'turkish', targets: [2,3,4] },
                        {
                            'targets': [0],
                            'visible': false,
                            'searchable': false
                        },
                        {
                            'targets': [1],
                            'visible': false,
                            'searchable': false
                        },
                    ],
                    'order': [[2, 'asc'],[0, 'asc'],[1, 'asc']],//sort bolge, IlId,IlceId
                    'scrollY': '300px',
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
                    ],                                        
                });
            ";
            return tableString;
        }
        private void RedirectToPage(string pageUrl)
        {
            try
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + pageUrl;
                Page.Response.Redirect(newUrl);
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private class IlIlceItem
        {
            public int IlId { get; set; }
            public int IlceId { get; set; }
            public string Bolge { get; set; }
            public string Ili { get; set; }
            public string Ilcesi { get; set; }
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {

        }
        protected void IliDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            IliIdQS = IliDDL.SelectedItem.Value;
            IlceDDLDoldur();
            TabloOlustur();
        }
        protected void IlcesiDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            IlcesiIdQS = IlcesiDDL.SelectedItem.Value;
            TabloOlustur();
        }

        protected void BolgeDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            IlDDLDoldur();
            TabloOlustur();
        }
        protected void FTKIslemleriBtn_Click(object sender, EventArgs e)
        {
            FTKIslem fTKIslem = new FTKIslem();
            fTKIslem = fTKIslem.SelectByIliIlcesi(IliIdQS.ConvertToInt(), IlcesiIdQS.ConvertToInt());
            string ftkIslemId = fTKIslem == null ? string.Empty : fTKIslem.Id.ToString();
            RedirectToPage(ProjeConstants.PAGE_FTKISLEMLERI + "?FTKIslemId=" + ftkIslemId + "&IliId=" + IliIdQS + "&IlcesiId=" + IlcesiIdQS);
        }
        protected void BolgelereGoreFTKRaporuBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_BOLGELEREGORE_FTK_DAGILIMI);
        }
        protected void YonergeBtn_Click(object sender, EventArgs e)
        {

        }
    }
}
