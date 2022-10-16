using Model.IKYS;
using Model.Ortak;
using System;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace IKYS_WebParts.BirimSemasiWP
{
    [ToolboxItemAttribute(false)]
    public partial class BirimSemasiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public BirimSemasiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string CurrentUserName
        {
            get
            {

                if (ViewState["CurrentUserName"] == null)
                {
                    ViewState["CurrentUserName"] = UtilityHelper.GetCurrentUser();
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
            try
            {
                BirimSemasiGetir();
            }
            catch (Exception ex)
            {
                MessageHelper.PublishMessage(ex.Message,ProjeConstants.MESAJ_HATA);
            }
        }

        private void BirimSemasiGetir()
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));

            var csChartConfig = System.Environment.NewLine + "chart_config = [";
            var csString = @" var config = {
                                container: '#BirimSemasiDiv',

                                connectors:
                                    {
                                        type: 'step'
                                },
                                node:
                                    {
                                        HTMLclass: 'nodeExample1'
                                }
                                },";

            csChartConfig += "config,";

            BirimTanim birimTanim = new BirimTanim();
            var birimListesi = birimTanim.SelectAll<BirimTanim>();

            BirimTanim parent = birimListesi.Where(a => a.ParentId == 0).FirstOrDefault<BirimTanim>();
            Personel ilknode = new Personel();
            ilknode = ilknode.Select<Personel>(parent.AmirId);
            string gmKullaniciAdi = ilknode == null ? "" : ilknode.KullaniciAdi.ReturnEmptyIfNull().ToString();
            string gmAdi = ilknode == null ? "" : ilknode.Adi.ReturnEmptyIfNull().ToString();
            string gmSoyadi = ilknode == null ? "" : ilknode.Soyadi.ReturnEmptyIfNull().ToString();
            string gmAdiSoyadi = gmAdi + " " + gmSoyadi;

            string gmfotostr = string.IsNullOrEmpty(gmKullaniciAdi) ? (!string.IsNullOrEmpty(gmAdi) ? gmAdi.Substring(0, 1) + gmSoyadi:"") : gmKullaniciAdi;
            gmfotostr = string.IsNullOrEmpty(gmfotostr) ? "person" : gmfotostr;
            
            string gmimgUrl = newUrl + "/../" + ProjeConstants.RESIMLER_PERSONEL + "/" + gmfotostr.ReplaceTrChars() + ".jpg";
            string gmerrImgUrl = newUrl + "/../" + ProjeConstants.RESIMLER_PERSONEL + "/person.jpg";

           

            csString += "a" + parent.Id + @" = {
                        text: {
                            name: '" + parent.Adi + @"',
                            title: '" + gmAdiSoyadi + @"',
                            contact: '',
                        },
                        image: '" + gmimgUrl + @"',
                        onerror :this.src = '" + gmerrImgUrl + @"'
                    },";

            csChartConfig += "a" + parent.Id + ",";

            foreach (BirimTanim item in birimListesi)
            {
                Personel birimAmiri = new Personel();
                birimAmiri = birimAmiri.SelectCalisanPersonel(item.AmirId);
                string amirKullaniciAdi = birimAmiri == null ? "" : birimAmiri.KullaniciAdi.ReturnEmptyIfNull().ToString();
                string amirAdi = birimAmiri == null ? "" : birimAmiri.Adi.ReturnEmptyIfNull().ToString();
                string amirSoyadi = birimAmiri == null ? "" : birimAmiri.Soyadi.ReturnEmptyIfNull().ToString();
                string amirAdiSoyadi = amirAdi + " " + amirSoyadi;
             
                string fotostr = string.IsNullOrEmpty(amirKullaniciAdi) ? (!string.IsNullOrEmpty(amirAdi) ? amirAdi.Substring(0, 1) + amirSoyadi : "") : amirKullaniciAdi;
                fotostr = string.IsNullOrEmpty(fotostr) ? "person": fotostr;
                string imgUrl = newUrl + "/../"+ ProjeConstants.RESIMLER_PERSONEL + "/" + fotostr.ReplaceTrChars() + ".jpg";

                string errImgUrl = newUrl + "/../" + ProjeConstants.RESIMLER_PERSONEL + "/person.jpg";
                if (item.ParentId != 0)
                {
                   string aktifPasifStr = (!item.Aktif ? "nodeExample2" : "nodeExample1" );
                   csString += "a" + item.Id + @" = {
                                parent: " + "a" + item.ParentId + @",
                                HTMLclass:'" + aktifPasifStr+ @"',
                                text: {
                                    name: '" + item.Adi + @"',
                                    title: '" + amirAdiSoyadi + @"',
                                },
                                stackChildren: true,
                                image: '" + imgUrl + @"',
                                onerror :this.src = '" +errImgUrl+@"'
                            },";

                    csChartConfig += "a" + item.Id + ",";
                }

            }

            csChartConfig = csChartConfig.Substring(0, csChartConfig.Length - 1);
            csChartConfig += "];" + System.Environment.NewLine;

            csString = csString.Substring(0, csString.Length - 1);

            csString += csChartConfig;
            csString += System.Environment.NewLine + "new Treant(chart_config);";


            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "test1", csString, true);
        }
    }
}
