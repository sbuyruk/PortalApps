using Model.IKYS;
using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace IKYS_WebParts.DereceKademeListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class DereceKademeListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public DereceKademeListesiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None; // Set the ChromeType to None to remove the default chrome
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
            if (!Page.IsPostBack)
            {

                FillPersonelDDL();
                PersonelIdQS = string.IsNullOrEmpty(PersonelIdQS) ? PersonelDDL.SelectedItem.Value : PersonelIdQS; // Eğer PersonelIdQS boş ise, geçerli kullanıcı Id'sini kullan
                Personel personel = PersonelGetir();
                if (personel != null)
                {
                    GecerliDereceKademeGetir(personel);
                    DereceKademeTabloOlustur(personel);
                    ButtonGorunurlugunuAyarla(personel);
                    DereceDDLDoldur();
                    KademeDDLDoldur();
                }
            }

        }

        private void ButtonGorunurlugunuAyarla(Personel personel)
        {
            // Personelin geçerli kademesi >=10 ise, Kademe dropdown visible= false olsun
            if (GecerliKademeTxt.Text.ConvertToInt() >= 10)
            {
                KademeYukseltPanel.Visible = false;
            }
            else
            {
                KademeYukseltPanel.Visible = true;
            }
            // Personelin geçerli derecesi <=1 ise, Derece dropdown visible= false olsun
            if (GecerliDereceTxt.Text.ConvertToInt() <= 1)
            {
                DereceYukseltPanel.Visible = false;
            }
            else
            {
                DereceYukseltPanel.Visible = true;
            }
        }
        #region Personel Getirme
        private void FillPersonelDDL()
        {
            PersonelDDL.Items.Clear();
            Personel personel = new Personel();
            List<Personel> list = personel.SelectCalisanPersonel();

            foreach (Personel item in list)
            {
                ListItem li = new ListItem(item.Adi.ReturnEmptyIfNull().ToString() + " " + item.Soyadi.ReturnEmptyIfNull().ToString(), item.Id.ReturnZeroIfNull().ToString());
                PersonelDDL.Items.Add(li);
            }
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
                PersonelIdQS = personel.Id.ToString();
            }

            return personel;
        }
        #endregion
        #region DereceKademe Listesi
        private void DereceKademeTabloOlustur(Personel personel)
        {
            var jsonData = DereceKademeTabloJson(personel); //veri çekilip json a çeviriliyor
            var jsString = CreateDereceKademeDataTable(jsonData); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string DereceKademeTabloJson(Personel personel)
        {
            string jSon = string.Empty;
            try
            {
                List<DereceKademeListItem> list = GetDereceKademeDataList(personel);
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
        private string CreateDereceKademeDataTable(string jsonData)
        {
            string tableString = @"
             jQuery(document).ready(function () {
                if ( jQuery.fn.DataTable.isDataTable('#DereceKademeDataTable') ) {
                    jQuery('#DereceKademeDataTable').DataTable().destroy();
                }
                jQuery('#DereceKademeDataTable tbody').empty();
                jQuery.fn.dataTable.moment('DD.MM.YYYY');//sort date
                jQuery('#DereceKademeDataTable').DataTable({
                    data: " + jsonData + @",
                    pageLength: 5,
                    columns: [
                        { data: 'DegisimTarihi' },
                        { data: 'Degisim' },
                        { data: 'Derece' },
                        { data: 'Kademe' },
                        { data: 'Aciklama' },
                        { data: 'Duzenle' }
                    ],
                    'order': [[0, 'desc']],//sort date desc
                    'language': {
                        'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                        'decimal': ',',
                        'thousands': '.'
                    },
                    responsive: true,
                    dom: 'frtip',               
                });
            });
            ";
            return tableString;
        }
        private List<DereceKademeListItem> GetDereceKademeDataList(Personel personel)
        {
            if (personel == null)
            {
                MessageHelper.PublishMessage("Personel bulunamadı", ProjeConstants.MESAJ_HATA);
                return new List<DereceKademeListItem>();
            }
            else
            {
                List<DereceKademeListItem> list = new List<DereceKademeListItem>();
                DereceKademeDegisim dereceKademe = new DereceKademeDegisim();
                DataTable dataTable = dereceKademe.SelectAllByPersonelIdReturnDT(personel.Id);
                string adSoyad = (personel.Adi + " " + personel.Soyadi).Trim();
                foreach (DataRow row in dataTable.Rows)
                {
                    string dereceKademeId = row["Id"].ToString();
                    string degisim = row["Degisim"].ToString();
                    string derece = row["Derece"].ToString();
                    string kademe = row["Kademe"].ToString();
                    string aciklama = row["Aciklama"].ToString();
                    string degisimTarihi = ((DateTime)row["DegisimTarihi"]).ToString("yyyy-MM-dd"); // JS için uygun tarih formatı

                    // JS parametrelerini güvenli hale getir (tırnak içine al)
                    string jsDegisim = "\"" + HttpUtility.JavaScriptStringEncode(degisim) + "\"";
                    string jsDerece = "\"" + HttpUtility.JavaScriptStringEncode(derece) + "\"";
                    string jsKademe = "\"" + HttpUtility.JavaScriptStringEncode(kademe) + "\"";
                    string jsAciklama = "\"" + HttpUtility.JavaScriptStringEncode(aciklama) + "\"";
                    string jsTarih = "\"" + degisimTarihi + "\"";

                    string jsAdSoyad = "\"" + HttpUtility.JavaScriptStringEncode(personel.Adi + " " + personel.Soyadi) + "\"";

                    string js = $"OpenModal({dereceKademeId},{personel.Id}, {jsDegisim}, {jsTarih}, {jsDerece}, {jsKademe}, {jsAciklama}, {jsAdSoyad})";



                    DereceKademeListItem item = new DereceKademeListItem
                    {
                        DegisimTarihi = degisimTarihi,
                        Degisim = degisim,
                        Derece = derece,
                        Kademe = kademe,
                        Aciklama = aciklama,
                        Duzenle = $"<a href=\"javascript:void(0);\" onclick='{js}' class=\"btn btn-primary\">Düzenle</a>"
                    };

                    list.Add(item);
                }

                return list;
            }
        }
        private void GecerliDereceKademeGetir(Personel personel)
        {
            if (personel == null)
            {
                MessageHelper.PublishMessage("Personel bulunamadı", ProjeConstants.MESAJ_HATA);
            }
            else
            {
                DereceKademeDegisim dereceKademe = new DereceKademeDegisim();
                dereceKademe = dereceKademe.SelectByPersonelId(personel.Id);
                GecerliDereceTxt.Text = dereceKademe?.Derece.ToString() ?? "Bulunamadı";
                GecerliKademeTxt.Text = dereceKademe?.Kademe.ToString() ?? "Bulunamadı";
            }
        }
        private class DereceKademeListItem
        {
            public string DegisimTarihi { get; set; }
            public string Degisim { get; set; }
            public string Derece { get; set; }
            public string Kademe { get; set; }
            public string Aciklama { get; set; }
            public string Duzenle { get; set; }


        }

        private void UpdateDereceKademeDegisim(int id, string degisim, DateTime degisimTarihi, int derece, int kademe, string aciklama)
        {

            try
            {
                // Bu kısmı kendi veritabanı ya da SharePoint listesi güncelleme kodu ile tamamlayın
                DereceKademeDegisim dereceKademeDegisim = new DereceKademeDegisim();
                dereceKademeDegisim = dereceKademeDegisim.Select(id);
                if (dereceKademeDegisim == null)
                {
                    MessageHelper.PublishMessage("Derece ve Kademe Değişikliği Bulunamadı.", ProjeConstants.MESAJ_HATA);
                    return;
                }
                dereceKademeDegisim.Degisim = degisim;
                dereceKademeDegisim.DegisimTarihi = degisimTarihi;
                dereceKademeDegisim.Derece = derece;
                dereceKademeDegisim.Kademe = kademe;
                dereceKademeDegisim.Aciklama = aciklama;
                dereceKademeDegisim.Update();
                MessageHelper.PublishMessage("Derece ve Kademe Değişikliği Güncellendi.", ProjeConstants.MESAJ_BASARILI);
            }
            catch (Exception)
            {
                MessageHelper.PublishMessage("Derece ve Kademe Değişikliği Güncellenemedi.", ProjeConstants.MESAJ_HATA);
            }
        }
        private void SaveDereceKademeDegisim(int id, string degisim, DateTime degisimTarihi, int derece, int kademe, string aciklama)
        {
            try
            {
                DereceKademeDegisim dereceKademeDegisim = new DereceKademeDegisim
                {
                    PersonelId = PersonelDDL.SelectedItem.Value.ConvertToInt(),
                    Degisim = degisim,
                    DegisimTarihi = degisimTarihi,
                    Derece = derece,
                    Kademe = kademe,
                    Aciklama = aciklama,
                };

                dereceKademeDegisim.Save();
                MessageHelper.PublishMessage("Derece ve Kademe Değişikliği Kaydedildi.", ProjeConstants.MESAJ_BASARILI);
            }
            catch (Exception)
            {
                MessageHelper.PublishMessage("Derece ve Kademe Değişikliği Kaydedilemedi.", ProjeConstants.MESAJ_HATA);
            }
        }
        private void DereceDDLDoldur()
        {
            int gecerliDerece=GecerliDereceTxt.Text.ConvertToInt();
            DereceDDL.Items.Clear();
            UcretTanim ucretTanim = new UcretTanim();
            DataTable dataTable = ucretTanim.SelectDerece();
            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    int derece = row["Derece"].ReturnZeroIfNull().ConvertToInt();
                    if ( derece > gecerliDerece)
                    {
                        continue;
                    }
                    string unvan = row["Unvan"].ReturnZeroIfNull().ToString();
                    DereceDDL.Items.Add(new ListItem(derece + " - " + unvan, derece.ToString()));
                }
            }
        }
        private void KademeDDLDoldur()
        {
            int gecerlikademe= GecerliKademeTxt.Text.ConvertToInt();
            KademeDDL.Items.Clear();
            UcretTanim ucretTanim = new UcretTanim();
            int derece = DereceDDL.SelectedItem.Value.ConvertToInt();
            DataTable dataTable = ucretTanim.SelectKademe(derece, 1);
            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    int kademe = row["Kademe"].ReturnZeroIfNull().ConvertToInt();
                    if (kademe < gecerlikademe)
                    {
                        continue;
                    }
                    KademeDDL.Items.Add(new ListItem(" - " + kademe + " -", kademe.ToString()));
                }
            }
        }
        #endregion
        #region Events

        protected void KaydetBtn_Click(object sender, EventArgs e)
        {
            // Modal'dan gelen veriler
            string degisim = editDegisim.SelectedValue;
            DateTime degisimTarihi = DateTime.Parse(editTarih.Text);
            int personelId = int.Parse(PersonelDDL.SelectedItem.Value);
            int derece = int.Parse(DereceDDL.SelectedItem.Value);
            int kademe = int.Parse(KademeDDL.SelectedItem.Value);
            string aciklama = editAciklama.Text;
            //kademe == Gecerli kademe  VE derece==gecerli derece ise kayıt yapmasın
            if (GecerliKademeTxt.Text.ConvertToInt() == kademe && GecerliDereceTxt.Text.ConvertToInt() == derece)
            {
                MessageHelper.PublishMessage("Kayıt yapabilmek için geçerli derece ve kademeden farklı bir değer seçmelisiniz.", ProjeConstants.MESAJ_HATA);
            }
            else
            {
                SaveDereceKademeDegisim(kademe, degisim, degisimTarihi, derece, kademe, aciklama);
            }

            BilgileriDoldur();
        }
        protected void GuncelleBtn_Click(object sender, EventArgs e)
        {
            // Modal'dan gelen veriler
            int id = int.Parse(editId.Value);
            string degisim = editDegisim.SelectedValue;
            DateTime degisimTarihi = DateTime.Parse(editTarih.Text);
            int derece = int.Parse(DereceDDL.SelectedItem.Value);
            int kademe = int.Parse(KademeDDL.SelectedItem.Value);
            string aciklama = editAciklama.Text;

            // Veriyi güncelleme işlemi yapılacak
            // Örnek: Veritabanına veya SharePoint listesine veri güncelleme
            UpdateDereceKademeDegisim(id, degisim, degisimTarihi, derece, kademe, aciklama);
            BilgileriDoldur();

        }

        protected void PersonelDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            BilgileriDoldur();

        }

        private void BilgileriDoldur()
        {
            Personel personel = new Personel();
            personel = personel.Select<Personel>(PersonelDDL.SelectedItem.Value.ConvertToInt());
            if (personel != null)
            {
                PersonelIdQS = personel.Id.ToString();
                GecerliDereceKademeGetir(personel);
                DereceKademeTabloOlustur(personel);
                ButtonGorunurlugunuAyarla(personel);
                DereceDDLDoldur();
                KademeDDLDoldur();
            }
            else
            {
                MessageHelper.PublishMessage("Personel Bulunamadı!", ProjeConstants.MESAJ_HATA);
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_PERSONEL_LIST;
                Page.Response.Redirect(newUrl);
            }
        }

        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        #endregion
    }

}
