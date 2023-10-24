using Model.MTS;
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

namespace MTS_WebParts.FaaliyetParametreWP
{
    [ToolboxItemAttribute(false)]
    public partial class FaaliyetParametreWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public FaaliyetParametreWP()
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
                FaaliyetGrupDDLDoldur();
                TabloyuGuncelle();
            }
        }

        private void FaaliyetGrupDDLDoldur()
        {
            ListItem li1 = new ListItem(ProjeConstants.PARAM_ANIOBJESI);

            ListItem li6 = new ListItem(ProjeConstants.PARAM_RANDEVUYERI);

            FaaliyetGrupDDL.Items.Clear();
            FaaliyetGrupDDL.Items.Add(li1);
            FaaliyetGrupDDL.Items.Add(li6);
        }
        protected void FaaliyetGrupDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabloyuGuncelle();
        }
        private void TabloyuGuncelle()
        {
            string faaliyetParametreGrubu = FaaliyetGrupDDL.SelectedItem.Value;
            FaaliyetParametre faaliyetDao = new FaaliyetParametre();
            List<FaaliyetParametre> faaliyetParametreList = faaliyetDao.SelectByGrupReturnList(faaliyetParametreGrubu);

            GrupLbl.Text = faaliyetParametreGrubu;
            YeniSiraTxt.Text = faaliyetParametreList.Count > 0 ? (faaliyetParametreList.Max(x => x.Sira) + 1).ToString() : "1";
            TabloOlustur(faaliyetParametreList);
        }
        private void TabloOlustur(List<FaaliyetParametre> faaliyetParametreList)
        {
            var jsonData = TabloJson(faaliyetParametreList); //veri çekilip json a çeviriliyor
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
                { data: 'Deger' },
                { data: 'Duzenle' },
                { data: 'Sil' },
            ],
            'language': {
                'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
            },
            responsive: true,
            dom: 'frtip',
        });
            ";

            return tableString;
        }
        private string TabloJson(List<FaaliyetParametre> faaliyetParametreList)
        {
            string jSon = string.Empty;

            try
            {
                List<ParametreListItem> list = GetDataList(faaliyetParametreList);
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
        private List<ParametreListItem> GetDataList(List<FaaliyetParametre> faaliyetParametreList)
        {
            List<ParametreListItem> parametreList = new List<ParametreListItem>();
            foreach (var item in faaliyetParametreList)
            {
                ParametreListItem pItem = new ParametreListItem();
                pItem.Sira = item.Sira;
                pItem.Id = item.Id.ToString();
                pItem.Grup = item.Grup;
                pItem.Deger = item.Deger;
                pItem.Duzenle = "<a href='#' class='btn btn-outline-primary' onclick=DuzenleSilModalAc(" + item.Id + "," + ProjeConstants.GUNCELLE.ReturnQuotedValue() + ")>Düzenle</a>";
                pItem.Sil = "<a href='#' class='btn btn-outline-danger' onclick=DuzenleSilModalAc(" + item.Id + "," + ProjeConstants.SIL.ReturnQuotedValue() + ")>Sil</a>";
                parametreList.Add(pItem);
            }

            return parametreList;
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
                    FaaliyetParametre rp = new FaaliyetParametre();
                    rp = rp.Select(parametreId);
                    if (rp != null)
                    {
                        ParametreTxt.Text = rp.Deger;
                        SiraTxt.Text = rp.Sira.ToString();
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
                bool silinebilirMi = true;
                string faaliyetParametreGrubu = FaaliyetGrupDDL.SelectedItem.Value;

                switch (faaliyetParametreGrubu)
                {
                    case ProjeConstants.PARAM_ANIOBJESI:
                        {
                            AniObjesiDagitim aniOjesiDagitim = new AniObjesiDagitim();
                            List<AniObjesiDagitim> list = aniOjesiDagitim.SelectByAniObjesiId(parametreId);
                            silinebilirMi = list.Count < 1;
                            break;
                        }
                    case ProjeConstants.PARAM_RANDEVUYERI:
                        {
                            Faaliyet faaliyet = new Faaliyet();
                            List<Faaliyet> list = faaliyet.SelectByFaaliyetYeriId(parametreId);
                            silinebilirMi = list.Count < 1;
                            break;
                        }

                }

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
                    MessageLbl.Text = "Silmek istediğiniz parametreyi kullanan faaliyetler bulunmaktadır.";
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
            FaaliyetParametre faaliyetParametre = new FaaliyetParametre();
            faaliyetParametre = faaliyetParametre.Select(parametreId);
            if (faaliyetParametre != null)
            {
                List<FaaliyetParametre> list = AyniIsimdeVarMi(faaliyetParametre.Grup, ParametreTxt.Text);
                bool varMi = false;
                foreach (var item in list)
                {
                    if (item.Id != faaliyetParametre.Id)
                    {
                        varMi = true;
                        break;
                    }
                }
                if (varMi)//aynı Deger'li parametre varsa güncellemesin 
                {
                    CloseModal();
                    TabloyuGuncelle();
                    MessageHelper.PublishMessage("Güncellenmedi, aynı isimde parametre zaten var.", ProjeConstants.MESAJ_HATA, 2000);
                }
                else
                {
                    faaliyetParametre.Deger = ParametreTxt.Text;
                    faaliyetParametre.Sira = SiraTxt.Text.ConvertToInt();
                    if (faaliyetParametre.Update())
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
            FaaliyetParametre faaliyetParametre = new FaaliyetParametre();
            faaliyetParametre = faaliyetParametre.Select(parametreId);
            CloseModal();
            if (faaliyetParametre != null)
            {
                bool silindiMi = faaliyetParametre.Delete();
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
                    List<FaaliyetParametre> list = AyniIsimdeVarMi(GrupLbl.Text, YeniDegerTxt.Text);
                    if (list.Count > 0)
                    {
                        TabloyuGuncelle();
                        MessageHelper.PublishMessage("Kaydedilemedi, aynı isimde bir parametre zaten var.", ProjeConstants.MESAJ_HATA, 2000);
                    }
                    else
                    {
                        FaaliyetParametre faaliyetParametre = new FaaliyetParametre();
                        faaliyetParametre.Grup = GrupLbl.Text;
                        faaliyetParametre.Sira = YeniSiraTxt.Text.ConvertToInt();
                        faaliyetParametre.Deger = YeniDegerTxt.Text;
                        faaliyetParametre.Olusturan = UtilityHelper.GetCurrentUserName();
                        int id = faaliyetParametre.Save();
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

        private List<FaaliyetParametre> AyniIsimdeVarMi(string grup, string deger)
        {
            FaaliyetParametre faaliyetParametreDAO = new FaaliyetParametre();
            List<FaaliyetParametre> list = faaliyetParametreDAO.SelectByGrupDeger(grup, deger);
            return list;
        }
    }
}
