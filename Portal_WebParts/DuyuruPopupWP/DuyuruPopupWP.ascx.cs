using Model.IKYS;
using Model.Ortak;
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
        //private List<Duyuru> DuyuruListesi
        //{
        //    get
        //    {

        //        if (ViewState["DuyuruListesi"] == null)
        //        {
        //            Duyuru kisiselDuyuruListesi = new Duyuru();
        //            DateTime now = DateTime.Now;
        //            DuyuruListesi = new List<Duyuru>();
        //            List<Duyuru> tekrarYokDuyuruList = duyuruDao.SelectByTarihReturnList(now, ProjeConstants.DUYURU_TEKRAR_YOK);
        //            List<Duyuru> yillikDuyuruList = duyuruDao.SelectByTarihReturnList(now, ProjeConstants.DUYURU_TEKRARLA_YIL);
        //            List<Duyuru> aylikDuyuruList = duyuruDao.SelectByTarihReturnList(now, ProjeConstants.DUYURU_TEKRARLA_AY);
        //            List<Duyuru> haftalikDuyuruList = duyuruDao.SelectByTarihReturnList(now, ProjeConstants.DUYURU_TEKRARLA_HAFTA);
        //            DuyuruListesi.AddRange(tekrarYokDuyuruList);
        //            DuyuruListesi.AddRange(yillikDuyuruList);
        //            DuyuruListesi.AddRange(aylikDuyuruList);
        //            DuyuruListesi.AddRange(haftalikDuyuruList);

        //            Personel personel = new Personel();
        //            string userName = CurrentUserName.Substring(CurrentUserName.LastIndexOf("\\") + 1, CurrentUserName.Length - CurrentUserName.LastIndexOf("\\") - 1);
        //            personel = personel.SelectByUserName(userName);
        //            if (personel != null)
        //            {
        //                string personelIdStr = personel.Id.ToString();
        //                List<Duyuru> kisiselListe = DuyuruListesi.Where(x => x.DuyuruAlicilari.Contains("," + personelIdStr + ",")).ToList();

        //                ViewState["DuyuruListesi"] = kisiselListe;
        //            }
        //            ViewState["DuyuruListesi"] = DuyuruListesi;

        //        }
        //        return (List<Duyuru>)ViewState["DuyuruListesi"];
        //    }

        //    set
        //    {
        //        ViewState["DuyuruListesi"] = value;
        //    }
        //}
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!Page.IsPostBack)
            {
                List<Duyuru> duyuruListesi = GetDuyuruList();
                if (duyuruListesi.Count > 0)
                {
                    Personel personel = new Personel();
                    string userName = CurrentUserName.Substring(CurrentUserName.LastIndexOf("\\") + 1, CurrentUserName.Length - CurrentUserName.LastIndexOf("\\") - 1);
                    personel = personel.SelectByUserName(userName);
                    if (personel != null)
                        foreach (Duyuru duyuru in duyuruListesi)
                        {
                            ModalFormuOlusturVeGoster(duyuru, true);
                            DuyuruKayanYaziDoldur(duyuru, personel);
                        }
                }
            }
        }

        private List<Duyuru> GetDuyuruList()
        {
            Duyuru duyuruDao = new Duyuru();
            DateTime now = DateTime.Now;
            List<Duyuru> duyuruListesi = new List<Duyuru>();
            List<Duyuru> kisiselDuyuruListesi = new List<Duyuru>();
            List<Duyuru> tekrarYokDuyuruList = duyuruDao.SelectByTarihReturnList(now, ProjeConstants.DUYURU_TEKRAR_YOK);
            List<Duyuru> yillikDuyuruList = duyuruDao.SelectByTarihReturnList(now, ProjeConstants.DUYURU_TEKRARLA_YIL);
            List<Duyuru> aylikDuyuruList = duyuruDao.SelectByTarihReturnList(now, ProjeConstants.DUYURU_TEKRARLA_AY);
            List<Duyuru> haftalikDuyuruList = duyuruDao.SelectByTarihReturnList(now, ProjeConstants.DUYURU_TEKRARLA_HAFTA);
            duyuruListesi.AddRange(tekrarYokDuyuruList);
            duyuruListesi.AddRange(yillikDuyuruList);
            duyuruListesi.AddRange(aylikDuyuruList);
            duyuruListesi.AddRange(haftalikDuyuruList);

            kisiselDuyuruListesi = duyuruListesi;

            Personel personel = new Personel();
            string userName = CurrentUserName.Substring(CurrentUserName.LastIndexOf("\\") + 1, CurrentUserName.Length - CurrentUserName.LastIndexOf("\\") - 1);
            personel = personel.SelectByUserName(userName);
            if (personel != null)
            {
                string personelIdStr = personel.Id.ToString();
                List<Duyuru> kisiselListe = duyuruListesi.Where(x => x.DuyuruAlicilari.Contains("," + personelIdStr + ",")).ToList();

                kisiselDuyuruListesi = kisiselListe;
            }

            return kisiselDuyuruListesi;
        }

        private void DuyuruKayanYaziDoldur(Duyuru duyuru, Personel personel)
        {
            string[] alicilar = duyuru.DuyuruAlicilari.Split(',');
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


        private void ModalFormuOlusturVeGoster(Duyuru duyuru, bool startup)
        {
            if (startup && !duyuru.Popup)
            {
                //ilk açılışta popup = false ise gösterme
            }
            else
            {
                DuyuruGosterim dg = new DuyuruGosterim();
                dg = dg.SelectByDuyuruId(duyuru.Id);
                int dgId = 0;
                if (dg != null)
                {
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
                modalDiv.Attributes["class"] = "modal fade text-center";
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
                headerLbl.Attributes["class"] = "col-form-label text-success font-weight-bold";
                headerLbl.Attributes.Add("style", "margin: auto;");

                headerLbl.InnerText = " D U Y U R U ";
                modalHeaderDiv.Controls.Add(headerLbl);

                HtmlGenericControl baslikDiv = new HtmlGenericControl("div");
                baslikDiv.ID = "baslikDiv" + duyuru.Id.ToString();
                baslikDiv.Attributes["class"] = "form-group font-weight-bold";
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
                KapatBtn.Attributes.Add("onclick", "DuyuruyuKapatClicked(" + duyuru.Id + "," + dgId + ",'" + okudumDiv + "','" + chkId + "','" + startup + "');");

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
                if (!Page.IsPostBack || !startup) //kayan duyuruya tıklandıysa göster ama modal butona basıldığında olan postbackde göstermes
                {
                    var openDuyuruPopupModal = "OpenDuyuruPopupModal(" + modalDiv.ClientID + "," + duyuru.Id + "," + dgId + "," + clickedx + ",'" + okudumDiv.ClientID + "');";
                    UtilityHelper.ScriptCalistir(openDuyuruPopupModal);
                }
            }
        }

        protected void OpenPopupBtn_Click(object sender, EventArgs e)
        {
            ModalPlaceHolder.Controls.Clear();
            if (!string.IsNullOrEmpty(paramDuyuruIdLbl.Value))
            {
                Duyuru duyuru = new Duyuru();
                duyuru = duyuru.Select(paramDuyuruIdLbl.Value.ConvertToInt());
                if (duyuru != null)
                {
                    ModalFormuOlusturVeGoster(duyuru, false);
                }
            }
        }

        protected void DuyuruyuOkudumBtn_Click(object sender, EventArgs e)
        {

            string[] okunanDuyurular = paramOkunanDuyuruListesiLbl.Value.Split(',');
            List<string> okunanDuyuruListesi = new List<string>(okunanDuyurular);
            foreach (string item in okunanDuyuruListesi)
            {
                DuyuruGosterim dg = new DuyuruGosterim();
                dg = dg.Select<DuyuruGosterim>(item.ConvertToInt());
                if (dg != null)
                {
                    DuyuruOkuma duyuruOkuma = new DuyuruOkuma();
                    duyuruOkuma.Olusturan = UtilityHelper.GetCurrentUserLoginName();
                    duyuruOkuma.OkumaTarihi = DateTime.Now;
                    Personel personel = new Personel();
                    string userName = CurrentUserName.Substring(CurrentUserName.LastIndexOf("\\") + 1, CurrentUserName.Length - CurrentUserName.LastIndexOf("\\") - 1);
                    personel = personel.SelectByUserName(userName);
                    duyuruOkuma.PersonelId = personel == null ? 0 : personel.Id;
                    duyuruOkuma.DuyuruId = dg.DuyuruId;
                    duyuruOkuma.DuyuruGosterimId = dg.Id;
                    duyuruOkuma.Save();
                }
            }
        }
    }
}
