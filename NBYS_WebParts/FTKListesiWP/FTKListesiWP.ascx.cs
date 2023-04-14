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

namespace NBYS_WebParts.FTKListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class FTKListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public FTKListesiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string KurulusTarihiQS
        {
            get
            {

                if (ViewState["KurulusTarihi"] == null)
                {
                    if (Page.Request.QueryString["KurulusTarihi"] != null)
                    {
                        ViewState["KurulusTarihi"] = Page.Request.QueryString["KurulusTarihi"];
                    }
                    else
                    {
                        ViewState["KurulusTarihi"] = string.Empty;
                    }
                }
                return ViewState["KurulusTarihi"].ToString();
            }

            set
            {
                ViewState["KurulusTarihi"] = value;
            }
        }
        private string GuncellemeTarihiQS
        {
            get
            {

                if (ViewState["GuncellemeTarihi"] == null)
                {
                    if (Page.Request.QueryString["GuncellemeTarihi"] != null)
                    {
                        ViewState["GuncellemeTarihi"] = Page.Request.QueryString["GuncellemeTarihi"];
                    }
                    else
                    {
                        ViewState["GuncellemeTarihi"] = string.Empty;
                    }
                }
                return ViewState["GuncellemeTarihi"].ToString();
            }

            set
            {
                ViewState["GuncellemeTarihi"] = value;
            }
        }
        private string GrupQS
        {
            get
            {

                if (ViewState["Grup"] == null)
                {
                    if (Page.Request.QueryString["Grup"] != null)
                    {
                        ViewState["Grup"] = Page.Request.QueryString["Grup"];
                    }
                    else
                    {
                        ViewState["Grup"] = string.Empty;
                    }
                }
                return ViewState["Grup"].ToString();
            }

            set
            {
                ViewState["Grup"] = value;
            }
        }
        private string BolgeQS
        {
            get
            {

                if (ViewState["Bolge"] == null)
                {
                    if (Page.Request.QueryString["Bolge"] != null)
                    {
                        ViewState["Bolge"] = Page.Request.QueryString["Bolge"];
                    }
                    else
                    {
                        ViewState["Bolge"] = string.Empty;
                    }
                }
                return ViewState["Bolge"].ToString();
            }

            set
            {
                ViewState["Bolge"] = value;
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
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                KurulusTarihiTxt.Text = KurulusTarihiQS;
                GuncellemeTarihiTxt.Text = GuncellemeTarihiQS;
                GrupDDLDoldur();
                UtilityHelper.SetDDLValue(GrupDDL, GrupQS);
                BolgeDDLDoldur();

                if (string.IsNullOrEmpty(BolgeQS))
                {
                    if (IliIdQS.ConvertToInt() > 0)
                    {
                        Il il = new Il();
                        il = il.Select<Il>(IliIdQS.ConvertToInt());
                        BolgeQS = il != null ? il.Bolge : string.Empty;
                    }
                }
                UtilityHelper.SetDDLValue(BolgeDDL, BolgeQS);
                IlDDLDoldur();
                UtilityHelper.SetDDLValue(IliDDL, IliIdQS);
                IlceDDLDoldur();
                UtilityHelper.SetDDLValue(IlcesiDDL, IlcesiIdQS);
                TabloOlustur();
            }
            //TabloOlustur();
        }
        private void GrupDDLDoldur()
        {
            GrupDDL.Items.Clear();

            ListItem li0 = new ListItem(ProjeConstants.FTK_GRUPLAMA_YOK, ProjeConstants.FTK_GRUPLAMA_YOK_INT.ToString());
            ListItem li1 = new ListItem(ProjeConstants.FTK_GRUPLAMA_ILLERE_GORE, ProjeConstants.FTK_GRUPLAMA_IL_ILCEYE_GORE_INT.ToString());
            GrupDDL.Items.Add(li0);
            GrupDDL.Items.Add(li1);

        }
        private void BolgeDDLDoldur()
        {
            BolgeDDL.Items.Clear();

            ListItem li0 = new ListItem(ProjeConstants.BOLGE_HEPSI);
            ListItem li1 = new ListItem(ProjeConstants.BOLGE_GENELMUDURLUK);
            ListItem li2 = new ListItem(ProjeConstants.BOLGE_ISTANBUL);
            ListItem li3 = new ListItem(ProjeConstants.BOLGE_IZMIR);
            ListItem li4 = new ListItem(ProjeConstants.BOLGE_MERSIN);
            BolgeDDL.Items.Add(li0);
            BolgeDDL.Items.Add(li1);
            BolgeDDL.Items.Add(li2);
            BolgeDDL.Items.Add(li3);
            BolgeDDL.Items.Add(li4);
        }
        private void IlDDLDoldur()
        {
            IliDDL.Items.Clear();

            ListItem li0 = new ListItem(ProjeConstants.HEPSI, ProjeConstants.HEPSI_INT.ToString());
            IliDDL.Items.Add(li0);

            string bolge = !string.IsNullOrEmpty(BolgeDDL.SelectedItem.Text) ? BolgeDDL.SelectedItem.Text : string.Empty;

            Il newil = new Il();
            List<Il> list = newil.SelectByBolge(bolge);
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
                List<FTKListItem> list = GetDataList();
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
        private List<FTKListItem> GetDataList()
        {
            List<FTKListItem> ftkList = new List<FTKListItem>();
            FTK ftkDao = new FTK();
            DataTable dataTable = ftkDao.SelectSonFTKListesiByIliIlcesiReturnDataTable(BolgeQS, IliIdQS.ConvertToInt(), IlcesiIdQS.ConvertToInt(),
                //BolgeDDL.SelectedItem.Value, IliDDL.SelectedItem.Value.ConvertToInt(), 
                //IlcesiDDL.SelectedItem.Value.ConvertToInt(),
                KurulusTarihiTxt.Text, GuncellemeTarihiTxt.Text);
            int tempIlceId = 999999;
            int tempIlId = 888888;
            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {

                    int ftkId = row["FTKId"].ConvertToInt();
                    int sayac = row["Sayac"].ConvertToInt();
                    int iliId = row["Ili"].ConvertToInt();
                    int ilcesiId = row["Ilcesi"].ConvertToInt();
                    string kartNo = row["KartNo"].ToString();

                    if (GrupDDL.SelectedItem.Value.ConvertToInt() == ProjeConstants.FTK_GRUPLAMA_IL_ILCEYE_GORE_INT)
                    {
                        if ((IlcesiIdQS.ConvertToInt() == ProjeConstants.VALILIK_INT) && (iliId == tempIlId))// sadece il seçili ise
                        {
                            tempIlceId = ilcesiId;
                            tempIlId = iliId;
                            continue;
                        }
                        if ((iliId == tempIlId) && (ilcesiId == tempIlceId))//sadece ilçe seçili ise
                        {
                            tempIlceId = ilcesiId;
                            tempIlId = iliId;
                            continue;
                        }
                    }
                    tempIlId = iliId;
                    tempIlceId = ilcesiId;
                    string ilAdi = row["IlAdi"].ToString();
                    string ilceAdi = row["IlceAdi"].ToString();
                    DateTime kurulusTarihi = row["KurulusTarihi"].ConvertToDatetime();
                    DateTime guncellemeTarihi = row["GuncellemeTarihi"].ConvertToDatetime();
                    string ftkGorevi = row["FTKGorevi"].ToString();
                    int ftkGoreviId = ParseGorevi(ftkGorevi);
                    string adiSoyadi = row["Adi"].ToString() + " " + row["Soyadi"].ToString();
                    string unvani = row["Unvani"].ReturnEmptyIfNull().ToString();
                    string telefon = row["Telefon"].ReturnEmptyIfNull().ToString();

                    FTKListItem ftkListItem = new FTKListItem();
                    ftkListItem.FTKId = ftkId.ToString();
                    ftkListItem.Sayac = sayac.ToString();
                    ftkListItem.IlId = iliId;
                    ftkListItem.IlceId = ilcesiId;

                    ftkListItem.Ili = ilAdi;
                    ftkListItem.Ilcesi = (ilcesiId == ProjeConstants.VALILIK_INT ? ProjeConstants.VALILIK : ilceAdi);
                    ftkListItem.KurulusTarihi = kurulusTarihi.ConvertToDatetimeEmptyIfNull();
                    ftkListItem.GuncellemeTarihi = guncellemeTarihi.ConvertToDatetimeEmptyIfNull();
                    ftkListItem.FTKGoreviId = ftkGoreviId;
                    ftkListItem.FTKGorevi = ftkGorevi;
                    ftkListItem.AdiSoyadi = adiSoyadi;
                    ftkListItem.Unvani = unvani;
                    ftkListItem.Telefon = telefon;
                    ftkListItem.KartNo = kartNo;

                    ftkList.Add(ftkListItem);

                }
            }
            return ftkList;
        }
        private void TabloyuGoster()
        {
            KurulusTarihiQS = KurulusTarihiTxt.Text;
            GuncellemeTarihiQS = GuncellemeTarihiTxt.Text;
            IliIdQS = IliDDL.SelectedItem.Value;
            IlcesiIdQS = IlcesiDDL.SelectedItem.Value;
            TabloOlustur();
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
                        { data: 'FTKGoreviId' },
                        { data: 'Ili' },
                        { data: 'Ilcesi' },
                        { data: 'KurulusTarihi' },
                        { data: 'GuncellemeTarihi' },
                        { data: 'FTKGorevi' },
                        { data: 'AdiSoyadi' },
                        { data: 'Unvani' },
                        { data: 'Telefon' },
                        { data: 'KartNo' },
                    ],
                    'columnDefs': [
                        { type: 'turkish', targets: [3,4,7,8,9] },
                        //{ 'width': '15%', 'targets': 3 },
                        //{ 'width': '15%', 'targets': 4 },
                        //{ 'width': '15%', 'targets': 5 },
                        //{ 'width': '15%', 'targets': 6 },
                        //{ 'width': '15%', 'targets': 7 },
                        //{ 'width': '15%', 'targets': 8 },
                        //{ 'width': '10%', 'targets': 9 },
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
                        {
                            'targets': [2],
                            'visible': false,
                            'searchable': false
                        },
                    ],
                    'order': [[0, 'asc'],[1, 'asc'],[2, 'asc'],[2, 'asc']],//sort IlId,IlceId
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
        private class FTKListItem
        {
            public string FTKId { get; set; }
            public string Sayac { get; set; }
            public int IlId { get; set; }
            public int IlceId { get; set; }
            public int FTKGoreviId { get; set; }
            public string Ili { get; set; }
            public string Ilcesi { get; set; }
            public string KurulusTarihi { get; set; }
            public string GuncellemeTarihi { get; set; }
            public string FTKGorevi { get; set; }
            public string AdiSoyadi { get; set; }
            public string Unvani { get; set; }
            public string Telefon { get; set; }
            public string KartNo { get; set; }
        }
        protected void BolgeDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            BolgeQS = BolgeDDL.SelectedItem != null ? BolgeDDL.SelectedItem.Value : BolgeQS;
            IlDDLDoldur();
            UtilityHelper.SetDDLValue(IliDDL, IliIdQS);
            TabloyuGoster();

        }
        protected void IliDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            IliIdQS = IliDDL.SelectedItem != null ? IliDDL.SelectedItem.Value : IliIdQS;
            IlceDDLDoldur();
            UtilityHelper.SetDDLValue(IlcesiDDL, IlcesiIdQS);
            TabloyuGoster();
        }
        protected void IlcesiDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            IlcesiIdQS = IlcesiDDL.SelectedItem != null ? IlcesiDDL.SelectedItem.Value : IlcesiIdQS;
            TabloyuGoster();
        }
        protected void GrupDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabloyuGoster();
        }
        protected void KurulusTarihiTxt_TextChanged(object sender, EventArgs e)
        {
            TabloyuGoster();
        }
        protected void FTKGuncellemeTarihiTxt_TextChanged(object sender, EventArgs e)
        {
            TabloyuGoster();
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
        protected void CloseBtn_Click(object sender, EventArgs e)
        {

        }


    }
}
