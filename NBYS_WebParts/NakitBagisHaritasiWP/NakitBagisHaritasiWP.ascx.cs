using Model.NBYS;
using Model.Ortak;
using System;
using System.ComponentModel;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;

namespace NBYS_WebParts.NakitBagisHaritasiWP
{
    [ToolboxItemAttribute(false)]
    public partial class NakitBagisHaritasiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public NakitBagisHaritasiWP()
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
        }
        protected void KayitGetirBtn_Click(object sender, EventArgs e)
        {
            string ingIlAdi = paramLbl.Value;
            Il il = new Il();
            il = il.SelectByIngAdi(ingIlAdi);
            FillData2IlinfoTable(il.Id);

        }

        private void createHeaderColumns()
        {
            TableCell siranoCell = new TableCell();
            siranoCell.CssClass = "btn-default";
            siranoCell.Text = "Sırano";
            IlinfoTableHeader.Controls.Add(siranoCell);

            TableCell yilCell = new TableCell();
            yilCell.CssClass = "btn-default";
            yilCell.Text = "Yıl";
            IlinfoTableHeader.Controls.Add(yilCell);

            TableCell bagisSayisiCell = new TableCell();
            bagisSayisiCell.CssClass = "btn-default";
            bagisSayisiCell.Text = "Bağış Adedi";
            IlinfoTableHeader.Controls.Add(bagisSayisiCell);

            TableCell bagisToplamiCell = new TableCell();
            bagisToplamiCell.CssClass = "btn-default";
            bagisToplamiCell.CssClass = "text-end";
            bagisToplamiCell.Text = "Bağış Miktarı";
            IlinfoTableHeader.Controls.Add(bagisToplamiCell);

        }

        private void FillData2IlinfoTable(int ilId)
        {
            //createHeaderColumns();
            NakitBagisHareket nbh = new NakitBagisHareket();
            DateTime now = DateTime.Now.AddYears(-4);
            DateTime sorguTar = new DateTime(now.Year, 1, 1);
            DataTable dataTable = nbh.SelectByIliAndYil(ilId, sorguTar);

            int SiraNo = 1;
            if (dataTable != null)
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    string yil = dataRow["Yil"].ReturnEmptyIfNull().ToString();
                    string bagisSayisi = dataRow["BagisSayisi"].ReturnEmptyIfNull().ToString();
                    string bagisToplam = dataRow["BagisToplam"].ReturnZeroIfNull().ConvertToDecimal().ToString("##,##");

                    TableRow tableRow = new TableRow();
                    TableCell SiraNoCell = new TableCell();
                    SiraNoCell.Text = SiraNo++ + "";
                    tableRow.Controls.Add(SiraNoCell);


                    TableCell YilCell = new TableCell();
                    YilCell.Text = yil;
                    YilCell.CssClass = "text-center";
                    tableRow.Controls.Add(YilCell);

                    TableCell BagisSayisiCell = new TableCell();
                    BagisSayisiCell.Text = bagisSayisi;
                    BagisSayisiCell.CssClass = "text-center";
                    tableRow.Controls.Add(BagisSayisiCell);

                    TableCell BagisToplamiCell = new TableCell();
                    //BagisToplamiCell.CssClass = "input-money text-end";
                    //TextBox tb = new TextBox();
                    //tb.CssClass = "input-money text-end";
                    ////tb.ReadOnly = true;
                    //tb.Text = bagisToplam;
                    //BagisToplamiCell.Controls.Add(tb);
                    BagisToplamiCell.CssClass = "input-money text-end";
                    BagisToplamiCell.Text = bagisToplam;
                    tableRow.Controls.Add(BagisToplamiCell);

                    IlinfoTable.Controls.Add(tableRow);
                }
            }
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {

        }
        protected void SelectedIlBtn_Click(object sender, EventArgs e)
        {
            string ingIlAdi = paramLbl.Value;
            Il il = new Il();
            il = il.SelectByIngAdi(ingIlAdi);
            TitleLbl.CssClass = "btn-primary";
            TitleLbl.Text = il.IlAdi + " İli Nakit Bağış Bilgileri ";

            FillData2IlinfoTable(il.Id);
            //ShowModal("Test");
            ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "OpenModal();", true);
        }
    }
}
