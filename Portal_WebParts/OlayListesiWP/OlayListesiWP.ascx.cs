using Model.MTS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Portal_WebParts.OlayListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class OlayListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public OlayListesiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private int OlaySayisiQS
        {
            get
            {

                if (ViewState["OlaySayisi"] == null)
                {
                    if (Page.Request.QueryString["OlaySayisi"] != null)
                    {
                        ViewState["OlaySayisi"] = Page.Request.QueryString["OlaySayisi"];
                    }
                    else
                    {
                        ViewState["OlaySayisi"] = 0;
                    }
                }
                return ViewState["OlaySayisi"].ConvertToInt();
            }

            set
            {
                ViewState["OlaySayisi"] = value;
            }
        }
        private DateTime SorguZamaniQS
        {
            get
            {
                if (ViewState["SorguZamani"] == null)
                {
                    ViewState["SorguZamani"] = false;
                }
                return ViewState["SorguZamani"].ConvertToDatetime();
            }

            set
            {
                ViewState["SorguZamani"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                RefreshTimer.Interval = 10000;
                RefreshTimer.Enabled = true;
                TabloOlustur();
            }
            var jsonData = TabloJson();
            UtilityHelper.ScriptCalistir("ClearTableData();");
            UtilityHelper.ScriptCalistir("SetTableData(" + jsonData + ");");
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
                SorguZamaniQS = DateTime.Today;
                List<Olay> olayList = GetDataList(SorguZamaniQS);
                List<OlayListItem> oliList = OlayListItemDoldur(olayList);
                var serializer = new JavaScriptSerializer();
                jSon = serializer.Serialize(oliList);
            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper();
                exceptionHelper.Exceptions.Add(exception);
                exceptionHelper.PublishException();
            }
            return jSon;
        }
        private List<Olay> GetDataList(DateTime tarih)
        {
            Olay olayDao = new Olay();
            List<Olay> list = olayDao.SelectByTarihReturnList(tarih, ProjeConstants.MTS);
            return list;
        }
        private List<OlayListItem> OlayListItemDoldur(List<Olay> list)
        {
            List<OlayListItem> olayList = new List<OlayListItem>();
            foreach (var item in list)
            {
                OlayListItem oli = new OlayListItem();
                oli.Aciklama = item.Aciklama;
                oli.IslemKonusu = item.IslemKonusu;
                oli.IslemTarihi = item.IslemTarihi.ConvertToDDMMYYYHHmmFormat();
                oli.IslemTipi = item.IslemTipi;
                oli.IslemYapan = item.IslemYapan;
                oli.OlayId = item.Id.ToString();
                olayList.Add(oli);
            }
            return olayList;
        }
        private void KayanListeyEkle(List<Olay> list)
        {
            foreach (var item in list)
            {
                string aciklama = ParseAciklama(item);
                OlayUl.Controls.Add(new LiteralControl("<li> * " + aciklama + "</li>"));
                OlayUl.Controls.Add(new LiteralControl("<li>...</li>"));
            }
        }
        public Randevu FindRandevu(string aciklama)
        {
            var words = aciklama.Split().Select(x => x.Trim(ProjeConstants.DELIMITER));
            int id = 0;
            foreach (string item in words)
            {
                bool contains = item.Contains("RandevuId");
                if (contains)
                {
                    id = item.Split('=')[1].ConvertToInt();
                    break;
                }
            }
            if (id > 0)
            {
                Randevu randevu = new Randevu();
                randevu = randevu.Select(id);
                return randevu;
            }
            else
                return null;
        }
        private string ParseAciklama(Olay olay)
        {
            Randevu randevu = FindRandevu(olay.Aciklama);
            string randevuStr = randevu == null ?
                string.Empty :
                randevu.RandevuKonusu + " konulu ve " + randevu.BaslangicTarihi + " tarihli faaliyetin ";
            string retval = "(" + olay.IslemTarihi.ConvertToDDMMYYYHHmmFormat() + ") " +
               randevuStr + "  <strong>" +
               olay.IslemKonusu + "</strong> bölümünde " +
               olay.IslemYapan + " tarafından  <strong> " +
               olay.IslemTipi + "</strong> işlemi yapılmıştır.";
            return retval;
        }
        private string CreateDataTable(string jsonData)
        {
            string tableString = @"
                function SetTableData(myset) {
                     table.rows.add(myset);
                }
                function ClearTableData() {
                    table.clear();
                }
                jQuery.fn.dataTable.moment('DD.MM.YYYY HH:mm');//sort date
                var table=jQuery('#CustomDataTable').DataTable({
                   
                    data: " + jsonData + @",
                    columns: [
                        { data: 'IslemTarihi' },
                        { data: 'IslemKonusu'},
                        { data: 'IslemTipi' },
                        { data: 'Aciklama' },
                        { data: 'IslemYapan'},
                    ],
                    'columnDefs': [
                        { type: 'turkish', targets: [1,2,3,4] },
                    ],
                    'order': [[0, 'desc']],//sort date desc
                    'language': {
                    'url': 'http://tskgv-portal/OrtakBelgeler/Turkish.txt',
                        'decimal': ',',
                        'thousands': '.'
                    },
                    responsive: true,
                    dom: 'frtip',                    
                    
                });

            ";

            return tableString;
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
        protected void RefreshTimer_Tick(object sender, EventArgs e)
        {

            List<Olay> list = GetDataList(SorguZamaniQS);
            List<OlayListItem> listItems = OlayListItemDoldur(list);

            if (listItems.Count > 0)
            {
                var jsonData = TabloJson();
                UtilityHelper.ScriptCalistir("ClearTableData();");
                UtilityHelper.ScriptCalistir("SetTableData(" + jsonData + ");");
                KayanListeyEkle(list);
            }
        }
        private class OlayListItem
        {
            public string OlayId { get; set; }
            public string IslemTarihi { get; set; }
            public string IslemKonusu { get; set; }
            public string IslemTipi { get; set; }
            public string Aciklama { get; set; }
            public string IslemYapan { get; set; }
        }
    }
}
