using Microsoft.SharePoint;
using System;
using System.ComponentModel;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Portal_WebParts.GunlukYemekListesiMoveWP
{
    [ToolboxItemAttribute(false)]
    public partial class GunlukYemekListesiMoveWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public GunlukYemekListesiMoveWP()
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
            YemekTablosunuDoldur();
        }
        protected void YemekListesiBtn_Click(object sender, EventArgs e)
        {
            YemekTablosunuDoldur();
        }
        private DataTable VeriGetir()
        {
            DataTable dataTable = new DataTable();
            try
            {
                DataColumn dataColumn = null;

                // create the columns
                dataColumn = new DataColumn("YemekAdi", typeof(string));
                dataTable.Columns.Add(dataColumn);
                dataColumn = new DataColumn("Kalori", typeof(string));
                dataTable.Columns.Add(dataColumn);

                SPWeb web = SPContext.Current.Web;

                if (web != null)
                {
                    SPList list = web.Lists["YemekMenusu"];
                    SPQuery query = new SPQuery();
                    query.RowLimit = 1;
                    query.Query = string.Format(@"<Where>
                                                          <Eq>
                                                             <FieldRef Name='Tarih' />
                                                             <Value Type='DateTime'>
                                                                <Today />
                                                             </Value>
                                                          </Eq>
                                                  </Where>
                                                  <OrderBy>
                                                          <FieldRef Name='ID' Ascending='FALSE' />
                                                  </OrderBy>");
                    SPListItemCollection gununMenusu = list.GetItems(query);

                    if (gununMenusu.Count > 0)
                    {
                        YokDiv.Attributes["style"] = "display : none";
                        double toplamKalori = 0;

                        foreach (SPListItem menu in gununMenusu)
                        {
                            for (int i = 1; i <= 8; i++)
                            {
                                if (!string.IsNullOrEmpty(Convert.ToString(menu["Yemek" + i])))
                                {
                                    DataRow dataRow = dataTable.NewRow();
                                    dataRow["YemekAdi"] = Convert.ToString(menu["Yemek" + i]);
                                    dataRow["Kalori"] = Convert.ToInt32(menu["Yemek" + i + "Kalori"]) + " Kalori";
                                    toplamKalori += Convert.ToDouble(menu["Yemek" + i + "Kalori"]);
                                    dataTable.Rows.Add(dataRow);
                                }
                            }

                        }
                        ToplamLbl.Text = "Toplam " + toplamKalori.ToString() + " kalori";

                    }

                }
                return dataTable;
            }

            catch (Exception )
            {
                return null;
            }

        }
        private void YemekTablosunuDoldur()
        {
            ResimleriYukle();
            YemekTableHeaders();

            DataTable dataTable = VeriGetir();
            int sira = 0;
            if (dataTable != null)
            {
                YokDiv.Attributes["style"] = "display : none";
                TabloDiv.Attributes["style"] = "display : block";
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    TableRow row = new TableRow();

                    string yemekAdi = dataRow["YemekAdi"].ToString();
                    string kalori = dataRow["Kalori"].ToString();

                    TableCell siraCell = new TableCell();
                    siraCell.Text = (++sira).ToString();
                    row.Controls.Add(siraCell);

                    TableCell yemekAdiCell = new TableCell();
                    yemekAdiCell.Text = yemekAdi;
                    row.Controls.Add(yemekAdiCell);

                    TableCell kaloriCell = new TableCell();
                    kaloriCell.Text = kalori;
                    row.Controls.Add(kaloriCell);
                    YemekTable.Controls.Add(row);

                }
            }
            else
            {
                YokDiv.Attributes["style"] = "display : block";
                TabloDiv.Attributes["style"] = "display : none";
                YokLbl.Text = DateTime.Now.ConvertToDatetimeEmptyIfNull() + " tarihi için tanımlanmış yemek menüsü bulunamamıştır.";

            }


        }

        private void ResimleriYukle()
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string rawUrl = System.Web.HttpContext.Current.Request.RawUrl.ToString();
            int index = currentUrl.IndexOf(rawUrl);
            string rootUrl = (index < 0) ? currentUrl : currentUrl.Remove(index, rawUrl.Length);
            string imgUrl = rootUrl + "/" + ProjeConstants.RESIMLER_ORTAK;
            YemekImg.Src = imgUrl + "/yemek.gif";
            YemekImg1.Src = imgUrl + "/yemek1.gif";
            YemekImg2.Src = imgUrl + "/yemek2.gif";
        }

        private void YemekTableHeaders()
        {
            YemekTable.Rows.Clear();
            TableHeaderRow thbaslik = new TableHeaderRow();
            TableHeaderCell baslikCell = new TableHeaderCell();

            string today = DateTime.Today.ConvertToDatetimeEmptyIfNull();
            baslikCell.Text = " Yemek Listesi " + "(" + today + ")";
            baslikCell.Font.Bold = true;
            baslikCell.ColumnSpan = 6;
            thbaslik.HorizontalAlign = HorizontalAlign.Center;
            thbaslik.Controls.Add(baslikCell);
            YemekTable.Controls.Add(thbaslik);

            TableHeaderRow th = new TableHeaderRow();
            TableHeaderCell siraCell = new TableHeaderCell();
            siraCell.Text = "Sıra";
            TableHeaderCell yemekCell = new TableHeaderCell();
            yemekCell.Text = "Yemek Adı";
            TableHeaderCell kaloriCell = new TableHeaderCell();
            kaloriCell.Text = "Kalori";

            th.Controls.Add(siraCell);
            th.Controls.Add(yemekCell);
            th.Controls.Add(kaloriCell);

            YemekTable.Controls.Add(th);
        }
    }
}
