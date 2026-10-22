using Model.NBYS;
using NBYS_WebParts.Logging;
using Utility.Logging;
using Model.Ortak;
using Model.Services.NBYS;
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
        private PortalLogContext LoggingContext
        {
            get { return PortalLogContext.FromCurrentRequest("NBYS", "NBYSParametreGirisiWP"); }
        }

        private NBYSParametreService Service
        {
            get { return new NBYSParametreService(LoggingContext); }
        }

        private void ExecuteAction(string operation, Action action)
        {
            try { action(); }
            catch (System.Threading.ThreadAbortException) { throw; }
            catch (Exception error)
            {
                NbysWebPartLog.ReportError(LoggingContext, "NBYS.NBYSParametreGirisiWP." + operation, error);
            }
        }

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
            ExecuteAction("Page_Load", () =>
            {
                if (!Page.IsPostBack) NbysWebPartLog.RecordOpened(LoggingContext);
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
            });
        }

        private void GrupDDLDoldur()
        {


            ListItem li0 = new ListItem(ProjeConstants.PARAM_NBYSYONERGE);
            ListItem li1 = new ListItem(ProjeConstants.PARAM_FTKYONERGE);
            ListItem li2 = new ListItem(ProjeConstants.PARAM_FTKYAZI);

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
            ExecuteAction("GrupDDL_SelectedIndexChanged", () =>
            {
                TabloyuGuncelle();
            });
        }
        private void TabloyuGuncelle()
        {
            string parametreGrubu = GrupDDL.SelectedItem.Value;
            List<NBYSParametre> parametreList = Service.GetByGroup(parametreGrubu);

            GrupLbl.Text = parametreGrubu;
            YeniSiraTxt.Text = parametreList.Count > 0 ? (parametreList.Max(x => x.Sira) + 1).ToString() : "1";
            TabloOlustur(parametreList);
        }
        private void TabloOlustur(List<NBYSParametre> parametreList)
        {
            var jsonData = TabloJson(parametreList); //veri çekilip json a çeviriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazirlaniyor.
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
                'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
            },
            responsive: true,
            dom: 'frtip',
        });
            ";

            return tableString;
        }
        private string TabloJson(List<NBYSParametre> parametreList)
        {
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(GetDataList(parametreList));
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
            ExecuteAction("DuzenleSilBtn_Click", () =>
            {
                GuncelleNowBtn.Visible = false;
                SilNowBtn.Visible = false;
                int parametreId = parametreIdLbl.Value.ConvertToInt();

                string islemTipi = paramIslemTipiLbl.Value;
                if (islemTipi.Equals(ProjeConstants.GUNCELLE))
                {
                    if (parametreId > 0)
                    {
                        NBYSParametre nbysParam = Service.GetById(parametreId);
                        if (nbysParam != null)
                        {
                            AnahtarTxt.Text = nbysParam.Anahtar;
                            AnahtarTxt.Enabled = AuthQS.Equals(ProjeConstants.PARAM_YETKI_ADMIN);
                            DegerTxt.Text = nbysParam.Deger;
                            SiraTxt.Text = nbysParam.Sira.ToString();
                            ModalLbl.Text = "Parametre Düzenleme";
                            SilDiv.Attributes["style"] = "display:none";
                            DuzenleDiv.Attributes["style"] = "display:block";
                            ModalLbl.CssClass = "col-form-label text-primary fw-bold";
                            GuncelleNowBtn.Visible = true;
                            SilNowBtn.Visible = false;
                            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "OpenModalOnay();", true);
                        }
                        else
                        {
                            Messages.Warning("Parametre bulunamadı. Listeyi yenileyip tekrar seçin.");
                        }
                    }
                    else
                    {
                        Messages.Warning("Parametre bulunamadı. Listeyi yenileyip tekrar seçin.");
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
                        ModalLbl.CssClass = "col-form-label text-danger fw-bold";
                        GuncelleNowBtn.Visible = false;
                        SilNowBtn.Visible = true;
                        System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "OpenModalOnay();", true);
                    }
                    else
                    {
                        ModalLbl.Text = "Parametre Silinemez";
                        MessageLbl.Text = "Bu parametreyi silme yetkiniz yok.";
                        SilDiv.Attributes["style"] = "display:block";
                        DuzenleDiv.Attributes["style"] = "display:none";
                        ModalLbl.CssClass = "col-form-label text-info fw-bold";
                        GuncelleNowBtn.Visible = false;
                        SilNowBtn.Visible = false;
                        System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "OpenModalOnay();", true);
                    }

                }
            });
        }

        protected void GuncelleNowBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("Update", () =>
            {
                NBYSParametre item = Service.GetById(parametreIdLbl.Value.ConvertToInt());
                if (item == null)
                {
                    Messages.Warning("Parametre bulunamadı. Listeyi yenileyip tekrar seçin.");
                    return;
                }
                item.Anahtar = AnahtarTxt.Text;
                item.Deger = DegerTxt.Text;
                int order;
                if (!int.TryParse(SiraTxt.Text, out order))
                {
                    Messages.Warning("Sıra alanına geçerli bir tam sayı girin.");
                    return;
                }
                item.Sira = order;
                ShowWriteResult(Service.UpdateChecked(item), "Parametre güncellendi.");
            });
        }

        private void CloseModal()
        {
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler,
                typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "CloseModal();", true);
        }

        protected void SilNowBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("Delete", () =>
            {
                NBYSParametre item = new NBYSParametre { Id = parametreIdLbl.Value.ConvertToInt() };
                ShowWriteResult(Service.DeleteChecked(item), "Parametre silindi.");
            });
        }

        protected void KaydetBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("Save", () =>
            {
                if (string.IsNullOrEmpty(GrupLbl.Text))
                {
                    Messages.Warning("Kaydetmek için parametre grubunu seçin.");
                    return;
                }
                int order;
                if (!int.TryParse(YeniSiraTxt.Text, out order))
                {
                    Messages.Warning("Sıra alanına geçerli bir tam sayı girin.");
                    return;
                }
                NBYSParametre item = new NBYSParametre
                {
                    Grup = GrupLbl.Text,
                    Sira = order,
                    Anahtar = YeniAnahtarTxt.Text,
                    Deger = YeniDegerTxt.Text
                };
                ShowWriteResult(Service.SaveChecked(item), "Parametre kaydedildi.");
            });
        }

        private void ShowWriteResult(NBYSParametreWriteResult result, string successMessage)
        {
            if (result == NBYSParametreWriteResult.Duplicate)
            {
                Messages.Warning("Aynı grupta bu anahtar ve değere sahip bir parametre zaten var. Girdiğiniz bilgileri kontrol edin.");
                return;
            }
            if (result == NBYSParametreWriteResult.NotFound)
            {
                Messages.Warning("Parametre bulunamadı. Listeyi yenileyip tekrar seçin.");
                return;
            }
            // The service has already committed. Refresh errors must not imply rollback.
            try
            {
                CloseModal();
                TabloyuGuncelle();
            }
            catch (Exception error)
            {
                if (error is System.Threading.ThreadAbortException) throw;
                NbysWebPartLog.ReportError(LoggingContext, "NBYS.NBYSParametreGirisiWP.RefreshAfterWrite", error,
                    successMessage + " Liste yenilenemedi. Sayfayı yenileyin.");
                return;
            }
            Messages.Success(successMessage, 2000);
        }
    }
}