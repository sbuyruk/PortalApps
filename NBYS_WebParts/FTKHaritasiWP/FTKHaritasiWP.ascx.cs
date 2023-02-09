using Model.NBYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.FTKHaritasiWP
{
    [ToolboxItemAttribute(false)]
    public partial class FTKHaritasiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public FTKHaritasiWP()
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
        protected void FTKUyeleriBtn_Click(object sender, EventArgs e)
        {
            string ingIlAdi = paramLbl.Value;
            Il il = new Il();
            il = il.SelectByIngAdi(ingIlAdi);
            FTKUyeTableDoldur(il);

        }
        private void FTKUyeTableHeaders()
        {
            TableCell siraNoCell = new TableCell();
            siraNoCell.Text = "Sıra";
            FTKUyeTableHeader.Controls.Add(siraNoCell);

            TableCell adiSoyadiCell = new TableCell();
            adiSoyadiCell.Text = "Adı/Soyadı";
            FTKUyeTableHeader.Controls.Add(adiSoyadiCell);

            TableCell goreviCell = new TableCell();
            goreviCell.Text = "Görevi";
            FTKUyeTableHeader.Controls.Add(goreviCell);

        }
        private void FTKUyeTableDoldur(Il il)
        {
            FTKUyeTableHeaders();

            FTK dao = new FTK();
            List<FTK> list = dao.SelectSonFTKListesiByIliIlcesiReturnList(il.Id, ProjeConstants.VALILIK_INT);
            int sira = 1;
            foreach (var item in list)
            {
                string adiSoyadi = item.Adi + " " + item.Soyadi;

                TableRow row = new TableRow();

                TableCell SiraCell = new TableCell();
                SiraCell.Text = (sira++).ToString();
                row.Controls.Add(SiraCell);

                TableCell AdiSoyadiCell = new TableCell();
                AdiSoyadiCell.Text = adiSoyadi;
                row.Controls.Add(AdiSoyadiCell);

                TableCell GoreviCell = new TableCell();
                GoreviCell.Text = item.FTKGorevi;//ParseGorevi(gorevi) ;
                row.Controls.Add(GoreviCell);

                FTKUyeTable.Controls.Add(row);
            }
        }
        private string ParseGorevi(int gorevi)
        {
            if (gorevi == ProjeConstants.FTK_GOREVI_FAHRIBASKAN_INT)
            {
                return ProjeConstants.FTK_GOREVI_FAHRIBASKAN;
            }
            else if (gorevi == ProjeConstants.FTK_GOREVI_FAHRIBASKAN_INT)
            {
                return ProjeConstants.FTK_GOREVI_FAHRIBASKAN;
            }
            else if (gorevi == ProjeConstants.FTK_GOREVI_GENELSEKRETER_INT)
            {
                return ProjeConstants.FTK_GOREVI_GENELSEKRETER;
            }
            else
            {
                return ProjeConstants.FTK_GOREVI_UYE;
            }

        }
        protected void SelectedIlBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string ingIlAdi = paramLbl.Value;
                Il il = new Il();
                il = il.SelectByIngAdi(ingIlAdi);
                FTKUyeTitleLbl.Text = il.IlAdi + " İlinde Bulunan FTK Üyeleri";
                FTKUyeTableDoldur(il);
                //ShowModal("Test");
                ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "OpenModal();", true);
            }
            catch (Exception exception)
            {

                ExceptionHelper eh = new ExceptionHelper(exception);
                eh.PublishException();
            }
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }

    }
}
