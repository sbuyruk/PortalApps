using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.TasinmazBagisHaritasiWP
{
    [ToolboxItemAttribute(false)]
    public partial class TasinmazBagisHaritasiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public TasinmazBagisHaritasiWP()
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
        protected void BagiscilarBtn_Click(object sender, EventArgs e)
        {
            string ingIlAdi = paramLbl.Value;
            Il il = new Il();
            il = il.SelectByIngAdi(ingIlAdi);
            BagisciTableDoldur(il.IlAdi);

        }
        private void TasinmazTableHeaders()
        {
            TableCell siranoCell = new TableCell();
            siranoCell.Text = "Sıra";
            TasinmazTableHeader.Controls.Add(siranoCell);

            TableCell cinsiCell = new TableCell();
            cinsiCell.Text = "Cinsi";
            TasinmazTableHeader.Controls.Add(cinsiCell);

            TableCell ilcesiCell = new TableCell();
            ilcesiCell.Text = "İlçe";
            TasinmazTableHeader.Controls.Add(ilcesiCell);

            TableCell mulkiyetCell = new TableCell();
            mulkiyetCell.Text = "Mülk. Şekli";
            TasinmazTableHeader.Controls.Add(mulkiyetCell);

            TableCell kullanimCell = new TableCell();
            kullanimCell.Text = "Kullanım Durumu";
            TasinmazTableHeader.Controls.Add(kullanimCell);

        }
        private void TasinmazTableDoldur(string ilstr)
        {
            List<Tasinmaz> list = GetTasinmazData(ilstr);
            int SiraNo = 1;
            TasinmazTableHeaders();
            foreach (Tasinmaz tasinmaz in list)
            {
                TableRow row = new TableRow();

                TableCell SiraNoCell = new TableCell();

                SiraNoCell.Text = SiraNo++ + "";
                row.Controls.Add(SiraNoCell);

                TableCell CinsiCell = new TableCell();
                CinsiCell.Text = tasinmaz.Cinsi.ToString();
                row.Controls.Add(CinsiCell);

                TableCell IlcesiCell = new TableCell();
                IlcesiCell.Text = tasinmaz.Ilcesi.ToString();
                row.Controls.Add(IlcesiCell);

                TableCell MulkiyetCell = new TableCell();
                MulkiyetCell.Text = tasinmaz.MulkiyetSekli.ToString();
                row.Controls.Add(MulkiyetCell);

                TableCell KullanimCell = new TableCell();
                KullanimCell.Text = tasinmaz.KullanimDurumu.ToString();
                row.Controls.Add(KullanimCell);

                TasinmazTable.Controls.Add(row);
            }
        }
        private void BagisciTableHeaders()
        {
            TableCell siranoCell = new TableCell();
            siranoCell.Text = "Sıra";
            BagisciTableHeader.Controls.Add(siranoCell);

            TableCell adiSoyadiCell = new TableCell();
            adiSoyadiCell.Text = "Adi-Soyadi";
            BagisciTableHeader.Controls.Add(adiSoyadiCell);

            TableCell ilcesiCell = new TableCell();
            ilcesiCell.Text = "İlçe";
            BagisciTableHeader.Controls.Add(ilcesiCell);

            TableCell telefonCell = new TableCell();
            telefonCell.Text = "Telefon";
            BagisciTableHeader.Controls.Add(telefonCell);

            TableCell kullanimCell = new TableCell();
            kullanimCell.Text = "Sağ/Vefat";
            BagisciTableHeader.Controls.Add(kullanimCell);

        }
        private void BagisciTableDoldur(string ilAdi)
        {
            BagisciTableHeaders();
            TasinmazBagisci dao = new TasinmazBagisci();
            List<TasinmazBagisci> list = GetBagisciData(ilAdi);
            int SiraNo = 1;
            foreach (TasinmazBagisci bagisci in list)
            {
                TableRow row = new TableRow();

                TableCell SiraNoCell = new TableCell();

                SiraNoCell.Text = SiraNo++ + "";
                row.Controls.Add(SiraNoCell);

                TableCell AdiSoyadiCell = new TableCell();
                AdiSoyadiCell.Text = bagisci.Adi.ToString() + " " + bagisci.Soyadi.ToString();
                row.Controls.Add(AdiSoyadiCell);

                TableCell IlcesiCell = new TableCell();
                IlcesiCell.Text = bagisci.Ilcesi.ToString();
                row.Controls.Add(IlcesiCell);

                TableCell TelefonCell = new TableCell();
                TelefonCell.Text = bagisci.Telefon1.ToString() + " - " + bagisci.Telefon2.ToString();
                row.Controls.Add(TelefonCell);

                TableCell SagVefatCell = new TableCell();
                SagVefatCell.Text = bagisci.Sag_vefat.ToString();
                if (bagisci.Sag_vefat.Equals("Vefat"))
                {
                    row.BackColor = Color.LightGray;
                    row.ForeColor = Color.Gray;
                }
                row.Controls.Add(SagVefatCell);

                BagisciTable.Controls.Add(row);
            }
        }
        private List<TasinmazBagisci> GetBagisciData(string ilAdi)
        {

            TasinmazBagisci dao = new TasinmazBagisci();
            List<TasinmazBagisci> list = dao.SelectByIlAdi(ilAdi);

            return list;
        }
        private List<Tasinmaz> GetTasinmazData(string ilAdi)
        {
            Tasinmaz dao = new Tasinmaz();
            List<Tasinmaz> list = dao.SelectByIlAdi(ilAdi);

            return list;
        }
        protected void SelectedIlBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string ingIlAdi = paramLbl.Value;
                Il il = new Il();
                il = il.SelectByIngAdi(ingIlAdi);
                TasinmazTitleLbl.Text = il.IlAdi + " İlinde Bulunan Taşınmazlar";
                BagisciTitleLbl.Text = il.IlAdi + " İlinde İkamet Eden Bağışçılar";
                TasinmazTableDoldur(il.IlAdi);
                BagisciTableDoldur(il.IlAdi);
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
