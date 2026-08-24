using Model.IKYS;
using Model.Ortak;
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
            this.ChromeType=PartChromeType.None;
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
            Personel personel = new Personel();// IKYSOrtak.PersonelGetir(CurrentUserName);
            personel = personel.Select(1192);
            //GorevOnay entity'sinde bu GorevOnay.AmirId==personel.Id ile eşleşen ve GorevOnay.AmirOnayi==AmirOnayDurumu.OnayBekliyor olan
            //kayıtlar varsa bu kayıtları bir listeye koy
            if (personel != null && personel.Id > 0)
            {
                BirimTanim birimTanim = new BirimTanim();
                List<BirimTanim> amirOlduguBirimler = birimTanim.SelectByAmirId(personel.Id);
                string amirOnayMesaji=string.Empty;
                if (amirOlduguBirimler != null && amirOlduguBirimler.Count > 0)
                {
                    AmirOnayLbl.Text = amirOnayMesaji= "Sizin Onayınızı Bekleyen " + amirOlduguBirimler.Count + " Görev Onayı Var.";
                    string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                    string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_BEKLEYENISLEMLER;
                    amirOnayMesaji += "<br/>Onay Bekleyen Görev Onayları İçin <a href='" + newUrl + "'>Tıklayınız</a>";

                    //UtilityHelper.ScriptCalistir("ShowBildirimModal();");
                    MessageHelper.PublishMessage(amirOnayMesaji, ProjeConstants.MESAJ_BILGI);
                }
            }
            else
            {
                MessageHelper.PublishMessage("Kullanıcı bilgisi alınamadı.", ProjeConstants.MESAJ_BILGI,2000);
            }
            
        }
    }
}
