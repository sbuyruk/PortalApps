using Model.IKYS;
using Model.Ortak;
using Model.Services.IKYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Services.Description;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;
using static Model.IKYS.GorevOnay;

namespace Portal_WebParts.BildirimWP
{
    [ToolboxItemAttribute(false)]
    public partial class BildirimWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public BildirimWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            if (ProjeConstants.AMIRONAYIETKINMI)
            {
                base.Render(writer);
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
            if (!ProjeConstants.AMIRONAYIETKINMI)
            {
                return;
            }
            Personel personel = IKYSOrtak.PersonelGetir(CurrentUserName);
            //Personel personel = new Personel();// 
            //personel = personel.Select(1192);
            //GorevOnay entity'sinde bu GorevOnay.AmirId==personel.Id ile eşleşen ve GorevOnay.AmirOnayi==AmirOnayDurumu.OnayBekliyor olan
            //kayıtlar varsa bu kayıtları bir listeye koy
            if (personel != null && personel.Id > 0)
            {
                BirimTanim birimTanim = new BirimTanim();
                List<BirimTanim> amirOlduguBirimler = birimTanim.SelectByAmirId(personel.Id);
                foreach (var item in amirOlduguBirimler)
                {
                    List<GorevOnay> gorevOnayListesi = new GorevOnayService().SelectByBirimIdAndDurum(item.Id, AmirOnayDurumu.OnayBekliyor);
                    if (gorevOnayListesi != null && gorevOnayListesi.Count > 0)
                    {
                        string amirOnayMesaji = string.Empty;
                        if (amirOlduguBirimler != null && amirOlduguBirimler.Count > 0)
                        {
                            AmirOnayLbl.Text = amirOnayMesaji = "Sizin Onayınızı Bekleyen Görev(ler) Var.";
                            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_BEKLEYENISLEMLER;
                            amirOnayMesaji += "<br/>Onay Bekleyen Görev Listesi İçin <a href='" + newUrl + "'>Tıklayınız</a>";

                            //UtilityHelper.ScriptCalistir("ShowBildirimModal();");
                            MessageHelper.PublishMessage(amirOnayMesaji, ProjeConstants.MESAJ_BILGI);
                        }
                    }
                    else
                    {
                        MessageHelper.PublishMessage("Kullanıcı bilgisi alınamadı.", ProjeConstants.MESAJ_BILGI, 2000);
                    }

                }
            }
        }
    }
}
