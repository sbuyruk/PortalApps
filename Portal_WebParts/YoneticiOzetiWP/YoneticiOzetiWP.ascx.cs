using Model.NBYS;
using System;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Portal_WebParts.YoneticiOzetiWP
{
    [ToolboxItemAttribute(false)]
    public partial class YoneticiOzetiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public YoneticiOzetiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType= PartChromeType.None;
        }
        private IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
        protected void Page_Load(object sender, EventArgs e)
        {

            GetNakitBagisOzeti();
        }

        private void GetNakitBagisOzeti()
        {
            NakitBagisHareket nbh = new NakitBagisHareket();
            DateTime songun = DateTime.Today;
            DataTable dataTable= nbh.SelectSUMByTarih(songun,songun);
            decimal sonBagisTutari = 0;
            int sonBagisciAdedi = 0;
            if (dataTable != null )
            {
                if (dataTable.Rows.Count > 0)
                {
                    DataRow row = dataTable.Rows[0];
                    sonBagisTutari = row["ToplamBagisTutari"].ToString().ConvertToDecimal();
                    sonBagisciAdedi = row["ToplamBagisciAdedi"].ToString().ConvertToInt();
                    while (sonBagisTutari < 1)
                    {
                        songun = songun.AddDays(-1);
                        dataTable = nbh.SelectSUMByTarih(songun, songun);
                        row = dataTable.Rows[0];
                        sonBagisTutari = row["ToplamBagisTutari"].ToString().ConvertToDecimal();
                        sonBagisciAdedi = row["ToplamBagisciAdedi"].ToString().ConvertToInt();
                    }
                }
            }

            DateTime birOncekiGun = songun.AddDays(-1);
            DataTable dataTable1 = nbh.SelectSUMByTarih(birOncekiGun, birOncekiGun);
            decimal birOncekiGunBagisTutari = 0;
            int birOncekiGunBagisciAdedi = 0;
            if (dataTable1 != null)
            {
                if (dataTable1.Rows.Count > 0)
                {
                    DataRow row = dataTable1.Rows[0];
                    birOncekiGunBagisTutari = row["ToplamBagisTutari"].ToString().ConvertToDecimal();
                    birOncekiGunBagisciAdedi = row["ToplamBagisciAdedi"].ToString().ConvertToInt();
                    while (birOncekiGunBagisTutari < 1)
                    {
                        birOncekiGun = birOncekiGun.AddDays(-1);
                        dataTable = nbh.SelectSUMByTarih(birOncekiGun, birOncekiGun);
                        row = dataTable.Rows[0];
                        birOncekiGunBagisTutari = row["ToplamBagisTutari"].ToString().ConvertToDecimal();
                        birOncekiGunBagisciAdedi = row["ToplamBagisciAdedi"].ToString().ConvertToInt();
                    }
                }
            }

           
            decimal fark = sonBagisTutari - birOncekiGunBagisTutari;
            
            decimal yuzde= fark * 100 /
                (sonBagisTutari > 0 ? sonBagisTutari : (birOncekiGunBagisTutari * 100));

            string artis = fark > 0 
                ? "<strong> %" + yuzde.ToString("N",culturInfo) + "</strong> artışı ifade eder." 
                : (fark < 0 ? "<strong> %" + Math.Abs(yuzde).ToString("N", culturInfo) + "</strong> azalışı ifade eder." : " aynıdır." );

            string birOncekiGunBagisBilgisiStr = "* Vakfımız banka hesaplarına daha önce, <strong>" + birOncekiGun.ConvertToDatetimeEmptyIfNull() 
                + "</strong> tarihinde, <strong>" + birOncekiGunBagisciAdedi + "</strong> adet bağışçı tarafından yatırılan bağış toplamı <strong>"
                + birOncekiGunBagisTutari.ToString("N",culturInfo)+ " TL. </strong> olarak gerçekleşmişti.";
            
            string sonBagisBilgisiStr = "* Vakfımız banka hesaplarına en son <strong>" + songun.ConvertToDatetimeEmptyIfNull() +
                "</strong> tarhinde, <strong>" + sonBagisciAdedi + "</strong> adet bağışçı tarafından yatırılan bağış toplamı <strong>" 
                + sonBagisTutari.ToString("N",culturInfo)+ " TL.</strong> olarak gerçekleşmiştir. Bu miktar, bir önceki güne göre " + artis;

            SonNakitBagislarLbl.Text = sonBagisBilgisiStr;
            BirOncekiGununNakitBagislariLbl.Text = birOncekiGunBagisBilgisiStr;
        }
    }
}
