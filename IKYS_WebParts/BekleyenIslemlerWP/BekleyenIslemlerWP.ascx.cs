using Model.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace IKYS_WebParts.BekleyenIslemlerWP
{
    [ToolboxItemAttribute(false)]
    public partial class BekleyenIslemlerWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public BekleyenIslemlerWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
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
        }
        private string AmirBirimIdList
        {
            get
            {
                if (ViewState["AmirBirimIdList"] == null)
                {
                    ViewState["AmirBirimIdList"] = string.Empty;
                }
                return ViewState["AmirBirimIdList"].ToString();
            }
            set
            {
                ViewState["AmirBirimIdList"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    Personel personel = new Personel();//PersonelGetir();
                    personel = personel.Select(1192);
                    if (personel != null && personel.Id > 0)
                    {
                        BirimTanim birimTanim = new BirimTanim();
                        List<BirimTanim> amirOlduguBirimler = birimTanim.SelectByAmirId(personel.Id);
                        if (amirOlduguBirimler != null && amirOlduguBirimler.Count > 0)
                        {
                            AmirBirimIdList = string.Join(",", amirOlduguBirimler.Select(b => b.Id));
                            TabloOlustur();
                        }
                        else
                        {
                            MesajGoster();
                        }
                    }
                    else
                    {
                        MesajGoster();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private void MesajGoster()
        {
            TabloDiv.Style["display"] = "none";
            MesajDiv.Style["display"] = "block";
        }
        private Personel PersonelGetir()
        {
            Personel personel = new Personel();
            string userName = CurrentUserName.Substring(CurrentUserName.LastIndexOf("\\") + 1, CurrentUserName.Length - CurrentUserName.LastIndexOf("\\") - 1);
            personel = personel.SelectByUserName(userName);
            return personel;
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
                List<BekleyenIslemListItem> list = GetDataList();
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
        private List<BekleyenIslemListItem> GetDataList()
        {
            GorevOnay gorevOnay = new GorevOnay();
            DataTable dataTable = gorevOnay.SelectBekleyenAmirOnayiByBirimIdsReturnDataTable(AmirBirimIdList);

            List<BekleyenIslemListItem> list = new List<BekleyenIslemListItem>();

            if (dataTable == null) return list;

            foreach (DataRow row in dataTable.Rows)
            {
                int gorevOnayId = row["GorevOnayId"].ConvertToInt();

                BekleyenIslemListItem item = new BekleyenIslemListItem();
                item.GorevOnayId = gorevOnayId.ToString();
                item.AdiSoyadi = row["AdiSoyadi"].ToString();
                item.BaslangicTarihi = row["BaslangicTarihi"].ConvertToDatetime().ToString("dd.MM.yyyy");
                item.BitisTarihi = row["BitisTarihi"].ConvertToDatetime().ToString("dd.MM.yyyy");
                item.GorevinSebebi = row["GorevinSebebi"].ToString();
                item.GorevinYeri = row["GorevinYeri"].ToString();
                item.UlasimAraci = row["UlasimAraci"].ToString();
                item.Transfer = row["Transfer"].ToString();
                item.Konaklama = row["Konaklama"].ToString();
                item.Aciklama = row["Aciklama"].ToString();
                item.Onayla = "<a href='javascript:OpenOnayla(" + gorevOnayId + ");' class='btn btn-outline-primary'>Onayla</a>";
                item.Incele = "<a href='javascript:OpenIncele(" + gorevOnayId + ");' class='btn btn-outline-secondary'>İncele</a>";

                list.Add(item);
            }
            return list;
        }
        private class BekleyenIslemListItem
        {
            public string GorevOnayId { get; set; }
            public string AdiSoyadi { get; set; }
            public string BaslangicTarihi { get; set; }
            public string BitisTarihi { get; set; }
            public string GorevinSebebi { get; set; }
            public string GorevinYeri { get; set; }
            public string UlasimAraci { get; set; }
            public string Transfer { get; set; }
            public string Konaklama { get; set; }
            public string Aciklama { get; set; }
            public string Onayla { get; set; }
            public string Incele { get; set; }
        }
        private string CreateDataTable(string jsonData)
        {
            string tableString = @"
                if ( jQuery.fn.DataTable.isDataTable('#CustomDataTable') ) {
                    jQuery('#CustomDataTable').DataTable().destroy();
                }
                jQuery('#CustomDataTable tbody').empty();

                jQuery.fn.dataTable.moment('DD.MM.YYYY');//sort date
                jQuery('#CustomDataTable').DataTable({
                    data: " + jsonData + @",
                    columns: [
                        { data: 'AdiSoyadi' },
                        { data: 'BaslangicTarihi' },
                        { data: 'BitisTarihi' },
                        { data: 'GorevinSebebi' },
                        { data: 'GorevinYeri' },
                        { data: 'UlasimAraci' },
                        { data: 'Transfer' },
                        { data: 'Konaklama' },
                        { data: 'Aciklama' },
                        { data: 'Onayla' },
                        { data: 'Incele' },
                    ],
                    columnDefs: [
                        { type: 'turkish', targets: [0,3,4,5] }
                    ],
                    'order': [[1, 'desc']],//sort date desc
                    'language': {
                        'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                        'decimal': ',',
                        'thousands': '.'
                    }
                });
            ";

            return tableString;
        }
        protected void ModalInfoBtn_Click(object sender, EventArgs e)
        {
            try
            {
                GorevOnay gorevOnay = new GorevOnay();
                gorevOnay = gorevOnay.Select(paramGorevOnayIdLbl.Value.ConvertToInt());
                if (gorevOnay != null)
                {
                    Personel personel = new Personel();
                    personel = personel.Select<Personel>(gorevOnay.PersonelId);
                    OnayLbl.Text = personel.Adi + " " + personel.Soyadi + " için "
                        + gorevOnay.BaslangicTarihi.ToString("dd.MM.yyyy") + " - " + gorevOnay.BitisTarihi.ToString("dd.MM.yyyy")
                        + " tarihleri arasındaki görevi onaylamak ya da reddetmek istediğinize emin misiniz?";
                }
                TabloOlustur();
                UtilityHelper.ScriptCalistir("OpenModalOnay();");
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void ModalInceleBtn_Click(object sender, EventArgs e)
        {
            try
            {
                GorevOnay gorevOnay = new GorevOnay();
                gorevOnay = gorevOnay.Select(paramGorevOnayIdLbl.Value.ConvertToInt());
                if (gorevOnay != null)
                {
                    Personel personel = new Personel();
                    personel = personel.Select<Personel>(gorevOnay.PersonelId);
                    InceleLbl.Text = "<b>Adı Soyadı:</b> " + personel.Adi + " " + personel.Soyadi + "<br/>"
                        + "<b>Başlangıç Tarihi:</b> " + gorevOnay.BaslangicTarihi.ToString("dd.MM.yyyy") + "<br/>"
                        + "<b>Bitiş Tarihi:</b> " + gorevOnay.BitisTarihi.ToString("dd.MM.yyyy") + "<br/>"
                        + "<b>Süre:</b> " + gorevOnay.Sure + "<br/>"
                        + "<b>Görevin Sebebi:</b> " + gorevOnay.GorevinSebebi + "<br/>"
                        + "<b>Görevin Yeri:</b> " + gorevOnay.GorevinYeri + "<br/>"
                        + "<b>Ulaşım Aracı:</b> " + gorevOnay.UlasimAraci + "<br/>"
                        + "<b>Transfer:</b> " + gorevOnay.Transfer + "<br/>"
                        + "<b>Konaklama:</b> " + gorevOnay.Konaklama + "<br/>"
                        + "<b>Amir Onayı:</b> " + UtilityHelper.GetEnumDisplayName((GorevOnay.AmirOnayDurumu)gorevOnay.AmirOnayi) + "<br/>"
                        + "<b>Açıklama:</b> " + gorevOnay.Aciklama;
                }
                TabloOlustur();
                UtilityHelper.ScriptCalistir("OpenModalIncele();");
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void OnaylaBtn_Click(object sender, EventArgs e)
        {
            try
            {
                GorevOnay gorevOnay = new GorevOnay();
                gorevOnay = gorevOnay.Select(paramGorevOnayIdLbl.Value.ConvertToInt());
                if (gorevOnay != null)
                {
                    gorevOnay.AmirOnayi = (int)GorevOnay.AmirOnayDurumu.Onaylandi;
                    bool isSuccess = gorevOnay.Update();
                    if (isSuccess)
                    {
                        MessageHelper.PublishMessage("Görev onaylandı.", ProjeConstants.MESAJ_BILGI);
                    }
                    else
                    {
                        MessageHelper.PublishMessage("Onaylama işlemi başarısız oldu.", ProjeConstants.MESAJ_HATA);
                    }
                }
                TabloOlustur();
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void ReddetBtn_Click(object sender, EventArgs e)
        {
            try
            {
                GorevOnay gorevOnay = new GorevOnay();
                gorevOnay = gorevOnay.Select(paramGorevOnayIdLbl.Value.ConvertToInt());
                if (gorevOnay != null)
                {
                    gorevOnay.AmirOnayi = (int)GorevOnay.AmirOnayDurumu.Reddedildi;
                    bool isSuccess = gorevOnay.Update();
                    if (isSuccess)
                    {
                        MessageHelper.PublishMessage("Görev reddedildi.", ProjeConstants.MESAJ_BILGI);
                    }
                    else
                    {
                        MessageHelper.PublishMessage("Reddetme işlemi başarısız oldu.", ProjeConstants.MESAJ_HATA);
                    }
                }
                TabloOlustur();
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
    }
}
