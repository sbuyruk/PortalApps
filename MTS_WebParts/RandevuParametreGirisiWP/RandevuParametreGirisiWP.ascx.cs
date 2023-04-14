using Model.MTS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace MTS_WebParts.RandevuParametreGirisiWP
{
    [ToolboxItemAttribute(false)]
    public partial class RandevuParametreGirisiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public RandevuParametreGirisiWP()
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
                RandevuGrupDDLDoldur();
                TabloyuGuncelle();
            }
        }

        private void RandevuGrupDDLDoldur()
        {
            ListItem li1 = new ListItem(ProjeConstants.PARAM_ANIOBJESI);

            ListItem li6 = new ListItem(ProjeConstants.PARAM_RANDEVUYERI);

            RandevuGrupDDL.Items.Clear();
            RandevuGrupDDL.Items.Add(li1);
            RandevuGrupDDL.Items.Add(li6);
        }
        protected void RandevuGrupDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabloyuGuncelle();
        }
        private void TabloyuGuncelle()
        {
            string randevuParametreGrubu = RandevuGrupDDL.SelectedItem.Value;
            RandevuParametre randevuDao = new RandevuParametre();
            List<RandevuParametre> randevuParametreList = randevuDao.SelectByGrupReturnList(randevuParametreGrubu);

            GrupLbl.Text = randevuParametreGrubu;
            YeniSiraTxt.Text= randevuParametreList.Count>0?(randevuParametreList.Max(x => x.Sira)+1).ToString():"1";
            TabloOlustur(randevuParametreList);
        }
        private void TabloOlustur(List<RandevuParametre> randevuParametreList)
        {
            var jsonData = TabloJson(randevuParametreList); //veri çekilip json a çeviriliyor
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
        private string TabloJson(List<RandevuParametre> randevuParametreList)
        {
            string jSon = string.Empty;

            try
            {
                List<ParametreListItem> list = GetDataList(randevuParametreList);
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
        private List<ParametreListItem> GetDataList(List<RandevuParametre> randevuParametreList)
        {
            List<ParametreListItem> parametreList = new List<ParametreListItem>();
            foreach (var item in randevuParametreList)
            {
                ParametreListItem pItem = new ParametreListItem();
                pItem.Sira = item.Sira;
                pItem.Id = item.Id.ToString();
                pItem.Grup = item.Grup;
                pItem.Deger = item.Deger;
                pItem.Duzenle= "<a href='#' class='btn btn-outline-primary' onclick=DuzenleSilModalAc(" + item.Id + ","+ProjeConstants.GUNCELLE.ReturnQuotedValue()+")>Düzenle</a>";
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
                    RandevuParametre rp = new RandevuParametre();
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
                        MessageHelper.PublishMessage("Parametre bulunamadı",ProjeConstants.MESAJ_HATA);
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
                string randevuParametreGrubu = RandevuGrupDDL.SelectedItem.Value;

                switch (randevuParametreGrubu)
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
                            Randevu randevu = new Randevu();
                            List<Randevu> list = randevu.SelectByRandevuYeriId(parametreId);
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
                    MessageLbl.Text = "Silmek istediğiniz parametreyi kullanan randevular bulunmaktadır.";
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
            RandevuParametre randevuParametre = new RandevuParametre();
            randevuParametre = randevuParametre.Select(parametreId);
            if (randevuParametre != null)
            {
                List<RandevuParametre> list = AyniIsimdeVarMi(randevuParametre.Grup, ParametreTxt.Text);
                bool varMi = false;
                foreach (var item in list)
                {
                    if (item.Id != randevuParametre.Id)
                    {
                        varMi = true;
                        break;
                    }
                }
                if (varMi)//aynı Deger'li parametre varsa güncellemesin 
                {
                    CloseModal();
                    TabloyuGuncelle();
                    MessageHelper.PublishMessage("Güncellenmedi, aynı isimde parametre zaten var.", ProjeConstants.MESAJ_HATA,2000);
                }
                else
                {
                    randevuParametre.Deger = ParametreTxt.Text;
                    randevuParametre.Sira = SiraTxt.Text.ConvertToInt();
                    if (randevuParametre.Update())
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
            RandevuParametre randevuParametre = new RandevuParametre();
            randevuParametre = randevuParametre.Select(parametreId);
            CloseModal();
            if (randevuParametre != null)
            {
                bool silindiMi = randevuParametre.Delete();
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
                    List<RandevuParametre> list = AyniIsimdeVarMi(GrupLbl.Text, YeniDegerTxt.Text);
                    if (list.Count > 0)
                    {
                        TabloyuGuncelle();
                        MessageHelper.PublishMessage("Kaydedilemedi, aynı isimde bir parametre zaten var.", ProjeConstants.MESAJ_HATA, 2000);
                    }
                    else
                    {
                        RandevuParametre randevuParametre = new RandevuParametre();
                        randevuParametre.Grup = GrupLbl.Text;
                        randevuParametre.Sira = YeniSiraTxt.Text.ConvertToInt();
                        randevuParametre.Deger = YeniDegerTxt.Text;
                        randevuParametre.Olusturan = UtilityHelper.GetCurrentUserName();
                        int id = randevuParametre.Save();
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

        private List<RandevuParametre> AyniIsimdeVarMi(string grup, string deger)
        {
            RandevuParametre randevuParametreDAO = new RandevuParametre();
            List<RandevuParametre> list = randevuParametreDAO.SelectByGrupDeger(grup, deger);
            return list;
        }
    }
}
