using Model.IKYS;
using Model.Services.IKYS;
using Model.Ortak;
using Portal_WebParts.Logging;
using Utility.Logging;
using Model.Services.Portal;
using Model.Portal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Portal_WebParts.DuyuruPopupWP
{
    [ToolboxItemAttribute(false)]
    public partial class DuyuruPopupWP : WebPart
    {
        private PortalLogContext LoggingContext
        {
            get { return PortalLogContext.FromCurrentRequest("Portal", "DuyuruPopupWP"); }
        }
        private void ExecuteAction(string operation, Action action)
        {
            PortalWebPartLog.ExecuteAction(LoggingContext, operation, action);
        }
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public DuyuruPopupWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string DuyuruIdQS
        {
            get
            {

                if (ViewState["DuyuruId"] == null)
                {
                    if (Page.Request.QueryString["DuyuruId"] != null)
                    {
                        ViewState["DuyuruId"] = Page.Request.QueryString["DuyuruId"];
                    }
                    else
                    {
                        ViewState["DuyuruId"] = string.Empty;
                    }
                }
                return ViewState["DuyuruId"].ToString();
            }

            set
            {
                ViewState["DuyuruId"] = value;
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
            ExecuteAction("Page_Load", () =>
            {
                PortalWebPartLog.RecordOpened(LoggingContext);
                //if (!Page.IsPostBack)
                {
                    List<Duyuru> duyuruListesi = GetDuyuruList();
                    if (duyuruListesi.Count > 0)
                    {
                        Personel personel = new Personel();
                        string userName = CurrentUserName.Substring(CurrentUserName.LastIndexOf("\\") + 1, CurrentUserName.Length - CurrentUserName.LastIndexOf("\\") - 1);
                        personel = new PersonelService().GetByUserName(userName, LoggingContext);
                        if (personel != null)
                            foreach (Duyuru duyuru in duyuruListesi)
                            {
                                ModalFormuOlusturVeGoster(duyuru, true);
                                DuyuruKayanYaziDoldur(duyuru, personel);
                            }
                    }
                }
            });
        }

        private List<Duyuru> GetDuyuruList()
        {
            Duyuru duyuruDao = new Duyuru();
            DateTime now = DateTime.Now;
            List<Duyuru> duyuruListesi = new List<Duyuru>();
            List<Duyuru> kisiselDuyuruListesi = new List<Duyuru>();
            List<Duyuru> tekrarYokDuyuruList = new Model.Services.Portal.DuyuruService(LoggingContext).GetByRepeat(now, ProjeConstants.DUYURU_TEKRAR_YOK);
            List<Duyuru> yillikDuyuruList = new Model.Services.Portal.DuyuruService(LoggingContext).GetByRepeat(now, ProjeConstants.DUYURU_TEKRARLA_YIL);
            List<Duyuru> aylikDuyuruList = new Model.Services.Portal.DuyuruService(LoggingContext).GetByRepeat(now, ProjeConstants.DUYURU_TEKRARLA_AY);
            List<Duyuru> haftalikDuyuruList = new Model.Services.Portal.DuyuruService(LoggingContext).GetByRepeat(now, ProjeConstants.DUYURU_TEKRARLA_HAFTA);
            duyuruListesi.AddRange(tekrarYokDuyuruList);
            duyuruListesi.AddRange(yillikDuyuruList);
            duyuruListesi.AddRange(aylikDuyuruList);
            duyuruListesi.AddRange(haftalikDuyuruList);

            kisiselDuyuruListesi = new List<Duyuru>();

            Personel personel = new Personel();
            string userName = CurrentUserName.Substring(CurrentUserName.LastIndexOf("\\") + 1, CurrentUserName.Length - CurrentUserName.LastIndexOf("\\") - 1);
            personel = new PersonelService().GetByUserName(userName, LoggingContext);
            if (personel != null)
            {
                string personelIdStr = personel.Id.ToString();
                List<Duyuru> kisiselListe = duyuruListesi.Where(x => PortalDuyuruWriteService.IsRecipient(x.DuyuruAlicilari, personel.Id)).ToList();

                kisiselDuyuruListesi = kisiselListe;
            }

            return kisiselDuyuruListesi;
        }

        private void DuyuruKayanYaziDoldur(Duyuru duyuru, Personel personel)
        {
            string[] alicilar = (duyuru.DuyuruAlicilari ?? string.Empty).Split(',');
            KayanYaziyaDuyuruEkle(duyuru);
        }

        private void KayanYaziyaDuyuruEkle(Duyuru duyuru)
        {
            HtmlGenericControl duyuruLi = new HtmlGenericControl("li");
            duyuruLi.ID = "duyuruLi" + duyuru.Id.ToString();
            DuyuruListUL.Controls.Add(duyuruLi);

            HtmlGenericControl duyuruA = new HtmlGenericControl("a");
            duyuruA.ID = "duyuruBtn" + duyuru.Id.ToString();
            duyuruA.InnerText = duyuru.Baslik;

            duyuruA.Attributes.Add("onclick", "CallButtonClick(" + duyuru.Id + ");");
            duyuruLi.Controls.Add(duyuruA);
        }


        private void RememberVersion(int id)
        {
            var ids = ViewState["Portal.PresentedVersions"] as List<int> ?? new List<int>();
            if (!ids.Contains(id) && ids.Count < 200) ids.Add(id);
            ViewState["Portal.PresentedVersions"] = ids;
        }
        private void ModalFormuOlusturVeGoster(Duyuru duyuru, bool startup)
        {
            if (startup && !duyuru.Popup)
            {
                //ilk açilista popup = false ise gösterme
            }
            else
            {
                DuyuruGosterim dg = new DuyuruGosterim();
                dg = new Model.Services.Portal.DuyuruGosterimService(LoggingContext).GetByDuyuruId(duyuru.Id);
                int dgId = 0;
                if (dg != null)
                {
                    RememberVersion(dg.Id);
                    duyuru.Metin = dg.Metin;
                    duyuru.Baslik = dg.Baslik;
                    duyuru.Resim = dg.Resim;
                    dgId = dg.Id;
                }


                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string hostUrl = currentUrl.Substring(0, currentUrl.LastIndexOf(System.Web.HttpContext.Current.Request.Url.AbsolutePath));
                string imgUrl = string.IsNullOrEmpty(duyuru.Resim)
                    ?string.Empty
                    : hostUrl + ProjeConstants.PATH_RESIMLER_DUYURU + duyuru.Resim.ReplaceTrChars() + ".jpg";

                HtmlGenericControl modalDiv = new HtmlGenericControl("div");
                modalDiv.ID = "modalDiv" + duyuru.Id.ToString();
                modalDiv.Attributes["class"] = "modal fade text-center duyuru-modal";
                modalDiv.Attributes["role"] = "dialog";
                ModalPlaceHolder.Controls.Add(modalDiv);

                HtmlGenericControl modalDialogDiv = new HtmlGenericControl("div");
                modalDialogDiv.ID = "modalDialogDiv" + duyuru.Id.ToString();
                modalDialogDiv.Attributes["class"] = "modal-dialog";
                modalDiv.Controls.Add(modalDialogDiv);

                HtmlGenericControl modalContentDiv = new HtmlGenericControl("div");
                modalContentDiv.ID = "modalContentDiv" + duyuru.Id.ToString();
                modalContentDiv.Attributes["class"] = "modal-content";
                modalDialogDiv.Controls.Add(modalContentDiv);

                HtmlGenericControl modalHeaderDiv = new HtmlGenericControl("div");
                modalHeaderDiv.ID = "modalHeaderDiv" + duyuru.Id.ToString();
                modalHeaderDiv.Attributes["class"] = "modal-header text-center";
                modalContentDiv.Controls.Add(modalHeaderDiv);

                HtmlGenericControl headerLbl = new HtmlGenericControl("h2");
                headerLbl.ID = "headerLbl" + duyuru.Id.ToString();
                headerLbl.Attributes["class"] = "col-form-label text-success fw-bold";
                headerLbl.Attributes.Add("style", "margin: auto;");

                headerLbl.InnerText = " D U Y U R U ";
                modalHeaderDiv.Controls.Add(headerLbl);

                HtmlGenericControl baslikDiv = new HtmlGenericControl("div");
                baslikDiv.ID = "baslikDiv" + duyuru.Id.ToString();
                baslikDiv.Attributes["class"] = "form-group fw-bold";
                modalContentDiv.Controls.Add(baslikDiv);

                HtmlGenericControl baslikLbl = new HtmlGenericControl("label");
                baslikLbl.ID = "baslikLbl" + duyuru.Id.ToString();
                baslikLbl.Attributes["class"] = "text-center cal-form-label";
                baslikDiv.InnerText = duyuru.Baslik;
                baslikDiv.Controls.Add(baslikLbl);

                if (!string.IsNullOrEmpty(imgUrl))
                {
                    HtmlGenericControl resimDiv = new HtmlGenericControl("div");
                    resimDiv.ID = "resimDiv" + duyuru.Id.ToString();
                    resimDiv.Attributes["class"] = "form-group";

                    modalContentDiv.Controls.Add(resimDiv);

                    HtmlGenericControl resimImg = new HtmlGenericControl("img");
                    resimImg.ID = "resimImg" + duyuru.Id.ToString();
                    resimImg.Attributes["class"] = "img-thumbnail";
                    resimImg.Attributes["src"] = imgUrl;
                    resimImg.Attributes["style"] = "height:300px";
                    resimDiv.Controls.Add(resimImg);
                }

                HtmlGenericControl metinDiv = new HtmlGenericControl("div");
                metinDiv.ID = "metinDiv" + duyuru.Id.ToString();
                metinDiv.Attributes["class"] = "form-group border m-2 p-2";
                modalContentDiv.Controls.Add(metinDiv);

                metinDiv.InnerHtml = duyuru.Metin;


                HtmlGenericControl modalFooterDiv = new HtmlGenericControl("div");
                modalFooterDiv.ID = "modalFooterDiv" + duyuru.Id.ToString();
                modalFooterDiv.Attributes["class"] = "modal-footer";
                modalContentDiv.Controls.Add(modalFooterDiv);

                HtmlGenericControl okudumDiv = new HtmlGenericControl("div");
                okudumDiv.ID = "okudumDiv" + duyuru.Id.ToString();
                okudumDiv.Attributes["class"] = "modal-footer";
                modalFooterDiv.Controls.Add(okudumDiv);

                CheckBox OkudumChk = new CheckBox();
                OkudumChk.ID = "OkudumChk" + duyuru.Id.ToString();
                OkudumChk.Text = "Okudum, bir daha gösterme";
                OkudumChk.CssClass = "FontSmall";//ascx içinde style
                OkudumChk.Checked = true;

                okudumDiv.Controls.Add(OkudumChk);


                HtmlGenericControl KapatBtn = new HtmlGenericControl("a");
                KapatBtn.ID = "KapatBtn" + duyuru.Id.ToString();
                KapatBtn.InnerText = "Kapat";

                KapatBtn.Attributes.Add("class", "btn btn-outline-secondary float-end");
                KapatBtn.Attributes.Add("style", "background-color:transparent");
                KapatBtn.Attributes.Add("data-bs-dismiss", "modal");
                KapatBtn.Attributes.Add("data-bs-toggle", "tooltip");
                string chkId = OkudumChk.ClientID;
                KapatBtn.Attributes.Add("onclick", "DuyuruyuKapatClicked(" + duyuru.Id + "," + dgId + ",'" + okudumDiv.ClientID + "','" + chkId + "'," + startup.ToString().ToLowerInvariant() + ");");

                modalFooterDiv.Controls.Add(KapatBtn);

                HtmlGenericControl btnIcon = new HtmlGenericControl("i");
                btnIcon.ID = "btnIcon" + duyuru.Id.ToString();
                btnIcon.Attributes.Add("class", "fa fa-check");
                KapatBtn.Controls.Add(btnIcon);
                modalFooterDiv.Controls.Add(KapatBtn);

                //HtmlGenericControl KapatBtn = new HtmlGenericControl("a");
                //KapatBtn.ID = "KapatBtn" + duyuru.Id.ToString();
                //KapatBtn.InnerText = "Kapat";

                //KapatBtn.Attributes.Add("class", "btn btn-outline-secondary ");
                //KapatBtn.Attributes.Add(" data-bs-dismiss", "modal");
                //KapatBtn.Attributes.Add("onclick", "KutlamayiKapatClicked(" + 0 + "," + sonDuyuru + ");");
                //modalFooterDiv.Controls.Add(KapatBtn);



                //LinkButton OkudumBtn = new LinkButton();
                //OkudumBtn.Text = "Okudum";
                //OkudumBtn.CssClass = "btn btn-secondary float-end";
                //TableUpdatePanel.ContentTemplateContainer.Controls.Add(OkudumBtn);
                //OkudumBtn.Click += delegate
                //{
                //    var okudum = "KutlamayiKapatClicked(" + duyuru.Id + ");";
                //    System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), okudum, true);
                //};
                //modalFooterDiv.Controls.Add(OkudumBtn);

                string clickedx = "true";
                if (startup)
                {
                    clickedx = "false";
                }
                if (!Page.IsPostBack || !startup) //kayan duyuruya tiklandiysa göster ama modal butona basildiginda olan postbackde göstermes
                {
                    var openDuyuruPopupModal = "OpenDuyuruPopupModal(document.getElementById('" + modalDiv.ClientID + "')," + duyuru.Id + "," + dgId + "," + clickedx + ",'" + okudumDiv.ClientID + "');";
                    UtilityHelper.ScriptCalistir(openDuyuruPopupModal);
                }
            }
        }

        protected void OpenPopupBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("OpenPopupBtn_Click", () =>
            {
                ModalPlaceHolder.Controls.Clear();
                int id = paramDuyuruIdLbl.Value.ConvertToInt();
                Duyuru duyuru = GetDuyuruList().FirstOrDefault(item => item.Id == id);
                if (duyuru == null) throw new PortalInputException("Duyuru bulunamadı veya bu duyuruyu görüntüleme yetkiniz yok.");
                ModalFormuOlusturVeGoster(duyuru, false);
            });
        }

        protected void DuyuruyuOkudumBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("DuyuruyuOkudumBtn_Click", () =>
            {
                string[] tokens = (paramOkunanDuyuruListesiLbl.Value ?? string.Empty).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length == 0) return;
                if (tokens.Length > 200) throw new PortalInputException("Duyuru seçimi geçerli değil. Sayfayı yenileyin.");
                var ids = new List<int>();
                var presented = ViewState["Portal.PresentedVersions"] as List<int> ?? new List<int>();
                foreach (string token in tokens)
                {
                    int id;
                    if (!int.TryParse(token, out id) || !presented.Contains(id))
                        throw new PortalInputException("Duyuru seçimi değişmiş. Sayfayı yenileyip tekrar açın.");
                    ids.Add(id);
                }
                PortalDuyuruResult result = new PortalDuyuruWriteService(LoggingContext).MarkRead(ids);
                if (result != PortalDuyuruResult.Completed && result != PortalDuyuruResult.Unchanged)
                    Messages.Warning("Duyurular okundu olarak işaretlenemedi. Duyuru veya alıcı bilgisi değişmiş olabilir; sayfayı yenileyin.");
                else
                {
                    try
                    {
                        foreach (int id in ids.Distinct())
                            UtilityHelper.ScriptCalistir("setCookie('DuyuruGosterildi" + id + "',new Date());");
                    }
                    catch (System.Threading.ThreadAbortException) { throw; }
                    catch (Exception error)
                    {
                        PortalWebPartLog.ReportError(LoggingContext, "Portal.DuyuruPopupWP.RefreshAfterRead", error,
                            "Okundu bilgisi kayıtlı; ekran güncellenemedi. Sayfayı yenileyin.");
                    }
                }
                paramOkunanDuyuruListesiLbl.Value = string.Empty;
            });
        }
    }
}
