using Model.MTS;
using Model.NBYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.NBYSParametreGirisiWP
{
    [ToolboxItemAttribute(false)]
    public partial class NBYSParametreGirisiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public NBYSParametreGirisiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
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
                        ViewState["Auth"] = ProjeConstants.PARAM_YETKI_FTK;
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
            if (!Page.IsPostBack)
            {
                VeriGirisiDiv.Attributes["style"] = "display:none";
                if (AuthQS.Equals(ProjeConstants.PARAM_YETKI_ADMIN))
                {
                    VeriGirisiDiv.Attributes["style"] = "display:block";
                }
                GrupDDLDoldur();
                TabloyuGuncelle();
            }
        }

        private void GrupDDLDoldur()
        {


            ListItem li0 = new ListItem(ProjeConstants.PARAM_NBYSYONERGE);
            ListItem li1 = new ListItem(ProjeConstants.PARAM_FTKYONERGE);
            ListItem li2= new ListItem(ProjeConstants.PARAM_FTKYAZI);

            GrupDDL.Items.Clear();
            if (AuthQS.Equals(ProjeConstants.PARAM_YETKI_ADMIN))
            {
                GrupDDL.Items.Add(li0);
                GrupDDL.Items.Add(li1);
                GrupDDL.Items.Add(li2);
            }
            else if (AuthQS.Equals(ProjeConstants.PARAM_YETKI_FTK))
            {
                GrupDDL.Items.Add(li2);
            }

        }
        protected void GrupDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabloyuGuncelle();
        }
        private void TabloyuGuncelle()
        {
            string parametreGrubu = GrupDDL.SelectedItem.Value;
            NBYSParametre parametreDao = new NBYSParametre();
            List<NBYSParametre> parametreList = parametreDao.SelectByGrupReturnList(parametreGrubu);

            GrupLbl.Text = parametreGrubu;
            YeniSiraTxt.Text = parametreList.Count > 0 ? (parametreList.Max(x => x.Sira) + 1).ToString() : "1";
            TabloOlustur(parametreList);
        }
        private void TabloOlustur(List<NBYSParametre> parametreList)
        {
            var jsonData = TabloJson(parametreList); //veri çekilip json a çeviriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazırlanıyor.
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler,
                typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), jsString, true);
        }
        private string CreateDataTable(string jsonData)
        {
            string tableString = @"
            if ( $.fn.DataTable.isDataTable('#CustomDataTable') ) {
              $('#CustomDataTable').DataTable().destroy();
            }

            $('#CustomDataTable tbody').empty();
            
            jQuery('#CustomDataTable').DataTable({
            data: " + jsonData + @",
            columns: [
                { data: 'Sira' },
                { data: 'Id' },
                { data: 'Grup' },
                { data: 'Anahtar' },
                { data: 'Deger' },
                { data: 'Duzenle' },
                { data: 'Sil' },
            ],
            'language': {
                'url': 'http://tskgv-portal/OrtakBelgeler/Turkish.txt',
            },
            responsive: true,
            dom: 'frtip',
        });
            ";

            return tableString;
        }
        private string TabloJson(List<NBYSParametre> parametreList)
        {
            string jSon = string.Empty;

            try
            {
                List<ParametreListItem> list = GetDataList(parametreList);
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
        private List<ParametreListItem> GetDataList(List<NBYSParametre> parametreList)
        {
            List<ParametreListItem> parametreListItemList = new List<ParametreListItem>();
            foreach (var item in parametreList)
            {
                ParametreListItem pItem = new ParametreListItem();
                pItem.Sira = item.Sira;
                pItem.Id = item.Id.ToString();
                pItem.Grup = item.Grup;
                pItem.Anahtar = item.Anahtar;
                pItem.Deger = item.Deger;
                pItem.Duzenle = "<a href='#' class='btn btn-outline-primary' onclick=DuzenleSilModalAc(" + item.Id + "," + ProjeConstants.GUNCELLE.ReturnQuotedValue() + ")>Düzenle</a>";
                pItem.Sil = "<a href='#' class='btn btn-outline-danger' onclick=DuzenleSilModalAc(" + item.Id + "," + ProjeConstants.SIL.ReturnQuotedValue() + ")>Sil</a>";
                parametreListItemList.Add(pItem);
            }

            return parametreListItemList;
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }

        private class ParametreListItem
        {
            public int Sira { get; set; }
            public string Id { get; set; }
            public string Grup { get; set; }
            public string Anahtar { get; set; }
            public string Deger { get; set; }
            public string Duzenle { get; set; }
            public string Sil { get; set; }
        }

        protected void DuzenleSilBtn_Click(object sender, EventArgs e)
        {
            GuncelleNowBtn.Visible = false;
            SilNowBtn.Visible = false;
            int parametreId = parametreIdLbl.Value.ConvertToInt();

            string islemTipi = paramIslemTipiLbl.Value;
            if (islemTipi.Equals(ProjeConstants.GUNCELLE))
            {
                if (parametreId > 0)
                {
                    NBYSParametre nbysParam = new NBYSParametre();
                    nbysParam = nbysParam.Select(parametreId);
                    if (nbysParam != null)
                    {
                        AnahtarTxt.Text = nbysParam.Anahtar;
                        AnahtarTxt.Enabled = AuthQS.Equals(ProjeConstants.PARAM_YETKI_ADMIN);
                        DegerTxt.Text = nbysParam.Deger;
                        SiraTxt.Text = nbysParam.Sira.ToString();
                        ModalLbl.Text = "Parametre Düzenleme";
                        SilDiv.Attributes["style"] = "display:none";
                        DuzenleDiv.Attributes["style"] = "display:block";
                        ModalLbl.CssClass = "col-form-label text-primary font-weight-bold";
                        GuncelleNowBtn.Visible = true;
                        SilNowBtn.Visible = false;
                        System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "OpenModalOnay();", true);
                    }
                    else
                    {
                        MessageHelper.PublishMessage("Parametre bulunamadı", ProjeConstants.MESAJ_HATA);
                    }
                }
                else
                {
                    MessageHelper.PublishMessage("Parametre bulunamadı", ProjeConstants.MESAJ_HATA);
                }

            }
            else if (islemTipi.Equals(ProjeConstants.SIL))
            {
                bool silinebilirMi = AuthQS.Equals(ProjeConstants.PARAM_YETKI_ADMIN);


                if (silinebilirMi)
                {
                    ModalLbl.Text = "Parametre Silinecek";
                    SilDiv.Attributes["style"] = "display:block";
                    DuzenleDiv.Attributes["style"] = "display:none";
                    ModalLbl.CssClass = "col-form-label text-danger font-weight-bold";
                    GuncelleNowBtn.Visible = false;
                    SilNowBtn.Visible = true;
                    System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "OpenModalOnay();", true);
                }
                else
                {
                    ModalLbl.Text = "Parametre Silinemez";
                    MessageLbl.Text = "Silmek istediğiniz parametreyi kullanan uygulamalar bulunmaktadır.";
                    SilDiv.Attributes["style"] = "display:block";
                    DuzenleDiv.Attributes["style"] = "display:none";
                    ModalLbl.CssClass = "col-form-label text-info font-weight-bold";
                    GuncelleNowBtn.Visible = false;
                    SilNowBtn.Visible = false;
                    System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "OpenModalOnay();", true);
                }

            }

        }

        protected void GuncelleNowBtn_Click(object sender, EventArgs e)
        {
            int parametreId = parametreIdLbl.Value.ConvertToInt();
            NBYSParametre nbysParametre = new NBYSParametre();
            nbysParametre = nbysParametre.Select(parametreId);
            if (nbysParametre != null)
            {
                NBYSParametre param = AyniIsimdeVarMi(nbysParametre.Grup, AnahtarTxt.Text, DegerTxt.Text);

                if (param!=null)//aynı Deger'li parametre varsa güncellemesin 
                {
                    CloseModal();
                    TabloyuGuncelle();
                    MessageHelper.PublishMessage("Güncellenmedi, aynı isimde parametre zaten var.", ProjeConstants.MESAJ_HATA, 2000);
                }
                else
                {
                    nbysParametre.Anahtar = AnahtarTxt.Text;
                    nbysParametre.Deger = DegerTxt.Text;
                    nbysParametre.Sira = SiraTxt.Text.ConvertToInt();
                    if (nbysParametre.Update())
                    {
                        CloseModal();
                        MessageHelper.PublishMessage("Parametre güncellendi", ProjeConstants.MESAJ_BASARILI, 2000);
                        TabloyuGuncelle();
                    }
                    else
                    {
                        MessageHelper.PublishMessage("Parametre güncellenemedi!", ProjeConstants.MESAJ_HATA);
                    }
                }
            }
            else
            {
                MessageHelper.PublishMessage("Parametre bulunamadı!", ProjeConstants.MESAJ_HATA);
            }
        }

        private void CloseModal()
        {
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "CloseModal();", true);
        }

        protected void SilNowBtn_Click(object sender, EventArgs e)
        {
            int parametreId = parametreIdLbl.Value.ConvertToInt();
            NBYSParametre toplantiParametre = new NBYSParametre();
            toplantiParametre = toplantiParametre.Select(parametreId);
            CloseModal();
            if (toplantiParametre != null)
            {
                bool silindiMi = toplantiParametre.Delete();
                if (silindiMi)
                {
                    TabloyuGuncelle();
                    MessageHelper.PublishMessage("Parametre silindi", ProjeConstants.MESAJ_BASARILI, 2000);
                }
                else
                {
                    MessageHelper.PublishMessage("Parametre silinemedi!", ProjeConstants.MESAJ_HATA);
                }
            }
            else
            {
                MessageHelper.PublishMessage("Parametre bulunamadı!", ProjeConstants.MESAJ_HATA);
            }

        }

        protected void KaydetBtn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(GrupLbl.Text))
            {
                try
                {
                    NBYSParametre parametre = AyniIsimdeVarMi(GrupLbl.Text, YeniAnahtarTxt.Text,YeniDegerTxt.Text);
                    if (parametre!=null)
                    {
                        TabloyuGuncelle();
                        MessageHelper.PublishMessage("Kaydedilemedi, aynı isimde bir parametre zaten var.", ProjeConstants.MESAJ_HATA, 2000);
                    }
                    else
                    {
                        NBYSParametre param = new NBYSParametre();
                        param.Grup = GrupLbl.Text;
                        param.Sira = YeniSiraTxt.Text.ConvertToInt();
                        param.Anahtar = YeniAnahtarTxt.Text;
                        param.Deger = YeniDegerTxt.Text;
                        param.Olusturan = UtilityHelper.GetCurrentUserLoginName();
                        int id = param.Save();
                        if (id > 0)
                        {
                            TabloyuGuncelle();
                            MessageHelper.PublishMessage("Parametre kaydedildi.", ProjeConstants.MESAJ_BASARILI, 2000);
                        }
                    }
                }
                catch (Exception ex)
                {

                    MessageHelper.PublishMessage("Parametre kaydedilemedi." + ex.Message, ProjeConstants.MESAJ_HATA);
                }
            }
            else
            {
                MessageHelper.PublishMessage("Yeni kayıt yapabilmek için parametre grubunu seçmelisiniz.", ProjeConstants.MESAJ_BILGI, 2000);
            }

        }

        private NBYSParametre AyniIsimdeVarMi(string grup, string anahtar, string deger)
        {
            NBYSParametre parametreDAO = new NBYSParametre();
            NBYSParametre param = parametreDAO.SelectByGrupAnahtar(grup, anahtar, deger);
            return param;
        }
    }
}
