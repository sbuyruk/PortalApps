using Portal_WebParts.Logging;
using Utility.Logging;
using Model.Services.Portal;
using Model.MTS;
using Model.Ortak;
using Model.Portal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Portal_WebParts.ToplantiParametreGirisiWP
{
    [ToolboxItemAttribute(false)]
    public partial class ToplantiParametreGirisiWP : WebPart
    {
        private PortalLogContext logContext;
        private PortalLogContext LogContext { get { return logContext ?? (logContext = PortalLogContext.FromCurrentRequest("Portal", "ToplantiParametreGirisiWP")); } }
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public ToplantiParametreGirisiWP()
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
            PortalWebPartLog.ExecuteAction(LogContext, "Page_Load", () =>
            {
                if (!Page.IsPostBack) PortalWebPartLog.RecordOpened(LogContext);
                if (!Page.IsPostBack)
                {
                    ToplantiGrupDDLDoldur();
                    TabloyuGuncelle();
                }
            });
        }

        private void ToplantiGrupDDLDoldur()
        {

            ToplantiGrupDDL.Items.Clear();
            ToplantiGrupDDL.Items.Add(new ListItem(ProjeConstants.PARAM_TOPLANTIYERI));
        }
        protected void ToplantiGrupDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            PortalWebPartLog.ExecuteAction(LogContext, "ToplantiGrupDDL_SelectedIndexChanged", () =>
            {
                TabloyuGuncelle();
            });
        }
        private void TabloyuGuncelle()
        {
            string toplantiParametreGrubu = ToplantiGrupDDL.SelectedItem.Value;
            ToplantiParametre toplantiDao = new ToplantiParametre();
            List<ToplantiParametre> toplantiParametreList = new Model.Services.Portal.ToplantiParametreService(LogContext).GetByGroup(toplantiParametreGrubu);

            GrupLbl.Text = toplantiParametreGrubu;
            YeniSiraTxt.Text = toplantiParametreList.Count > 0 ? (toplantiParametreList.Max(x => x.Sira) + 1).ToString() : "1";
            TabloOlustur(toplantiParametreList);
        }
        private void TabloOlustur(List<ToplantiParametre> toplantiParametreList)
        {
            var jsonData = TabloJson(toplantiParametreList); //veri çekilip json a çeviriliyor
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
        private string TabloJson(List<ToplantiParametre> toplantiParametreList)
        {
            string jSon = string.Empty;

            List<ParametreListItem> list = GetDataList(toplantiParametreList);
            var serializer = new JavaScriptSerializer();
            jSon = serializer.Serialize(list).Replace("<", "\\u003c").Replace(">", "\\u003e").Replace("&", "\\u0026");

            return jSon;
        }
        private List<ParametreListItem> GetDataList(List<ToplantiParametre> toplantiParametreList)
        {
            List<ParametreListItem> parametreList = new List<ParametreListItem>();
            foreach (var item in toplantiParametreList)
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
            PortalWebPartLog.ExecuteAction(LogContext, "CloseBtn_Click", () =>
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
                Page.Response.Redirect(newUrl);
            });
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
            PortalWebPartLog.ExecuteAction(LogContext, "DuzenleSilBtn_Click", () =>
            {
                GuncelleNowBtn.Visible = false;
                SilNowBtn.Visible = false;
                int parametreId = parametreIdLbl.Value.ConvertToInt();

                string islemTipi = paramIslemTipiLbl.Value;
                if (islemTipi.Equals(ProjeConstants.GUNCELLE))
                {
                    if (parametreId > 0)
                    {
                        ToplantiParametre rp = new Model.Services.Portal.ToplantiParametreService(LogContext).GetById(parametreId);
                        if (rp != null)
                        {
                            ParametreTxt.Text = rp.Deger;
                            SiraTxt.Text = rp.Sira.ToString();
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
                            throw new PortalInputException("Parametre bulunamadı.");
                        }
                    }
                    else
                    {
                        throw new PortalInputException("Parametre bulunamadı.");
                    }

                }
                else if (islemTipi.Equals(ProjeConstants.SIL))
                {
                    bool silinebilirMi = true;
                    string toplantiParametreGrubu = ToplantiGrupDDL.SelectedItem.Value;

                    switch (toplantiParametreGrubu)
                    {

                        case ProjeConstants.PARAM_TOPLANTIYERI:
                            {
                                Toplanti toplanti = new Toplanti();
                                List<Toplanti> list = new Model.Services.Portal.ToplantiService(LogContext).GetByYeri(parametreId);
                                silinebilirMi = list.Count < 1;
                                break;
                            }

                    }

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
                        MessageLbl.Text = "Silmek istediginiz parametreyi kullanan randevular bulunmaktadir.";
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
            PortalWebPartLog.ExecuteAction(LogContext, "GuncelleNowBtn_Click", () =>
            {
                int order;
                if (!int.TryParse(SiraTxt.Text, out order)) throw new PortalInputException("Sıra alanına tam sayı girin.");
                SaveParameter(new ToplantiParametre { Id = parametreIdLbl.Value.ConvertToInt(), Deger = ParametreTxt.Text, Sira = order }, false, "Parametre güncellendi.", true);
            });
        }

        private void SaveParameter(ToplantiParametre item, bool delete, string message, bool existing)
        {
            if (existing && item.Id <= 0) throw new PortalInputException("İşlem yapılacak parametreyi seçin.");
            PortalToplantiResult result;
            try { result = new PortalToplantiWriteService(LogContext).SaveParameter(item, delete); }
            catch (Exception error)
            {
                PortalWebPartLog.ReportError(LogContext, "Portal.ToplantiParameter.Save", error,
                    "İşlemin sonucu doğrulanamadı. Tekrar işlem yapmadan önce parametre listesini yenileyin.");
                return;
            }
            switch (result)
            {
                case PortalToplantiResult.Completed: break;
                case PortalToplantiResult.Duplicate: throw new PortalInputException("Bu grupta aynı adlı parametre zaten var. Farklı bir ad girin.");
                case PortalToplantiResult.NotFound: throw new PortalInputException("Parametre bulunamadı. Lütfen listeyi yenileyin.");
                case PortalToplantiResult.InUse: throw new PortalInputException("Bu parametre kullanımda veya sistem için gerekli. İşlem yapılamadı.");
                case PortalToplantiResult.Forbidden: throw new PortalInputException("İşlem için oturumunuzu yenileyin.");
                default: throw new PortalInputException("Parametre grubunu seçin ve adını girin.");
            }
            try
            {
                CloseModal();
                YeniDegerTxt.Text = string.Empty;
                TabloyuGuncelle();
            }
            catch (Exception error)
            {
                PortalWebPartLog.ReportError(LogContext, "Portal.ToplantiParameter.Refresh", error,
                    message + " Liste yenilenemedi. İşlemi tekrar kaydetmeyin; sayfayı yenileyin.");
                return;
            }
            Messages.Success(message);
        }
        private void CloseModal()
        {
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "CloseModal();", true);
        }

        protected void SilNowBtn_Click(object sender, EventArgs e)
        {
            PortalWebPartLog.ExecuteAction(LogContext, "SilNowBtn_Click", () =>
            {
                SaveParameter(new ToplantiParametre { Id = parametreIdLbl.Value.ConvertToInt() }, true, "Parametre silindi.", true);
            });
        }

        protected void KaydetBtn_Click(object sender, EventArgs e)
        {
            PortalWebPartLog.ExecuteAction(LogContext, "KaydetBtn_Click", () =>
            {
                int order;
                if (!int.TryParse(YeniSiraTxt.Text, out order)) throw new PortalInputException("Sıra alanına tam sayı girin.");
                SaveParameter(new ToplantiParametre { Grup = GrupLbl.Text, Deger = YeniDegerTxt.Text, Sira = order }, false, "Parametre kaydedildi.", false);
            });
        }

    }
}
