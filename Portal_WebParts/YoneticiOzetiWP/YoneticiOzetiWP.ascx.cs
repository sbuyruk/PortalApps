using DocumentFormat.OpenXml.Vml;
using Model.NBYS;
using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
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
            TitleLbl.InnerText = "Yönetici Özeti " +"("+ DateTime.Now.ToString("d", culturInfo)+")";
            TasinmazAdetleriniGetir();
            KiralikAdetleriniGetir();
        }

        private void KiralikAdetleriniGetir()
        {
            Tasinmaz tasinmaz = new Tasinmaz();
            DataTable dataTable = tasinmaz.SelectKirayaUygunTumTasinmazlar();
            if (dataTable != null) {
                int toplamKirayaUygun = 0;
                foreach (DataRow row in dataTable.Rows)
                {
                    int envanterdeMi = row["EnvanterdeMi"].ReturnZeroIfNull().ConvertToInt();
                    int anaTasinmaz = Convert.ToInt32(row["AnaTasinmaz"]);
                    int altBolum = Convert.ToInt32(row["AltBolum"]);
                    int toplam = anaTasinmaz + altBolum;
                    toplamKirayaUygun += toplam;
                    if (envanterdeMi == 1)
                    {
                        KirayaUygunAnaTasinmazLbl.Text = anaTasinmaz.ToString() + " Adet";
                        KirayaUygunAltBolumLbl.Text = altBolum.ToString() + " Adet";
                        
                    }
                    else if (envanterdeMi == 2)
                    {
                        EnvanterDisiLbl.Text = toplam.ToString() + " Adet";
                    }
                }
                ToplamKirayaUygunLbl.Text = toplamKirayaUygun.ToString() + " Adet";
            }
        }

        private void TasinmazAdetleriniGetir()
        {
            Tasinmaz tasinmaz = new Tasinmaz();
            DataTable dataTable = tasinmaz.ToplamTasinmazAdediGetir();
            int cm = 0;
            int tm=0;
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    string mulkiyetSekli = row["MulkiyetSekli"].ToString();
                    int adet = Convert.ToInt32(row["Adet"]);
                    if (mulkiyetSekli == "ÇM")
                    {
                        cm += adet;
                    }
                    else if (mulkiyetSekli == "TM")
                    {
                        tm += adet;
                    }
                }
            }
            CiplakMulkToplamLbl.Text = cm.ToString() +" Adet";
            TamMulkToplamLbl.Text = tm.ToString() +" Adet";
            ToplamTasinmazLbl.Text = (cm + tm).ToString() +" Adet";
        }
        protected void TasinmazDurumuBtn_Click(object sender, EventArgs e)
        {
            try
            {
                var openPopup = "OpenModal('TasinmazDurumuModal');";
                UtilityHelper.ScriptCalistir(openPopup);
                TasinmazDurumuTablosunuDoldur();
            }
            catch (Exception ex)
            {
                MessageHelper.PublishMessage("Taşınmaz Durumu Tablosu Doldurulurken Hata Oluştu. Hata : "+ ex, ProjeConstants.MESAJ_HATA);
            }
        }
        protected void KiraDurumuBtn_Click(object sender, EventArgs e)
        {
            try
            {
                var openPopup = "OpenModal('KiraDurumuModal');";
                UtilityHelper.ScriptCalistir(openPopup);
                KiraDurumuTablosunuDoldur();
            }
            catch (Exception ex)
            {
                MessageHelper.PublishMessage("Kira Durumu Tablosu Doldurulurken Hata Oluştu. Hata : "+ ex, ProjeConstants.MESAJ_HATA);
            }
        }
        protected void KirayaUygunOlmayanDurumuBtn_Click(object sender, EventArgs e)
        {
            try
            {
                var openPopup = "OpenModal('KirayaUygunOlmayanDurumuModal');";
                UtilityHelper.ScriptCalistir(openPopup);
                KiraDurumuTablosunuDoldur();
            }
            catch (Exception ex)
            {
                MessageHelper.PublishMessage("Kiraya Uygun Olmayanlar Tablosu Doldurulurken Hata Oluştu. Hata : "+ ex, ProjeConstants.MESAJ_HATA);
            }
        }
        protected void KiraGelirleriBtn_Click(object sender, EventArgs e)
        {
            try
            {
                var openPopup = "OpenModal('KiraGeliriModal');";
                UtilityHelper.ScriptCalistir(openPopup);
                KiraGeliriTablosunuDoldur();
            }
            catch (Exception ex)
            {
                MessageHelper.PublishMessage("Kira Durumu Tablosu Doldurulurken Hata Oluştu. Hata : "+ ex, ProjeConstants.MESAJ_HATA);
            }
        }



        protected void TasinmazDurumuTablosunuDoldur()
        {
            try
            {
                // Find totals row (contains TopBaslikCell)
                TableRow totalsRow = TopBaslikCell.Parent as TableRow;
                int headerRows = 2; // first two header rows are static
                int totalsIndex = -1;
                for (int i = 0; i < TasDurTable.Rows.Count; i++)
                {
                    if (TasDurTable.Rows[i] == totalsRow)
                    {
                        totalsIndex = i;
                        break;
                    }
                }

                if (totalsIndex == -1)
                {
                    // if not found, append to end
                    totalsIndex = TasDurTable.Rows.Count;
                }

                // Remove any existing region data rows between headers and totals row
                for (int i = totalsIndex - 1; i >= headerRows; i--)
                {
                    TasDurTable.Rows.RemoveAt(i);
                }

                // Prepare totals accumulators
                int totalTM = 0;
                int totalCM = 0;
                int totalApt = 0;
                int totalMes = 0;
                int totalIsy = 0;
                int totalArs = 0;
                int totalTar = 0;

                Tasinmaz tasinmaz = new Tasinmaz();

                // Get active regions based on user permission/context
                Bolge bolgeDao = new Bolge();
                var bolgeList = bolgeDao.SelectAktifBolgeler(ProjeConstants.HEPSI_INT);

                // Insert a row per bolge before totals row
                int insertIndex = headerRows;
                foreach (var bolge in bolgeList)
                {
                    if (bolge.Id == ProjeConstants.BOLGE_GENELMUDURLUK_INT ||
                        bolge.Id == ProjeConstants.BOLGE_YURTDISI_INT)
                        continue;
                    int ankTM = tasinmaz.SelectTasinmazAdetByBolgeMulkiyetSekli(bolge.Id, ProjeConstants.MULKIYETSEKLI_TM);
                    int ankCM = tasinmaz.SelectTasinmazAdetByBolgeMulkiyetSekli(bolge.Id, ProjeConstants.MULKIYETSEKLI_CM);

                    // usage counts (APT includes ISHANI)
                    string kiraDurumuStr = string.Empty;
                    int apt = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(bolge.Id, ProjeConstants.KULLANIMSEKLI_APT, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
                    int ishani = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(bolge.Id, ProjeConstants.KULLANIMSEKLI_ISHANI, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
                    int mes = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(bolge.Id, ProjeConstants.KULLANIMSEKLI_MESKEN, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
                    int isy = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(bolge.Id, ProjeConstants.KULLANIMSEKLI_ISYERI, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
                    int ars = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(bolge.Id, ProjeConstants.KULLANIMSEKLI_ARSA, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
                    int tar = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(bolge.Id, ProjeConstants.KULLANIMSEKLI_TARLA, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);

                    int tm = ankTM;
                    int cm = ankCM;
                    int tmcmTop = tm + cm;
                    int aptTotal = apt + ishani;

                    // Build row
                    TableRow row = new TableRow { HorizontalAlign = HorizontalAlign.Center };

                    TableCell bolgeCell = new TableCell { CssClass = "btn-primary", Text = string.IsNullOrEmpty(bolge.KisaAdi) ? bolge.Adi : bolge.KisaAdi };
                    row.Cells.Add(bolgeCell);

                    TableCell tmCell = new TableCell { Text = tm.ReturnEmptyIfZeroOrNull().ToString() };
                    row.Cells.Add(tmCell);

                    TableCell cmCell = new TableCell { Text = cm.ReturnEmptyIfZeroOrNull().ToString() };
                    row.Cells.Add(cmCell);

                    TableCell tmcmTopCell = new TableCell { Text = tmcmTop.ReturnEmptyIfZeroOrNull().ToString() };
                    row.Cells.Add(tmcmTopCell);

                    TableCell aptCell = new TableCell { Text = aptTotal.ReturnEmptyIfZeroOrNull().ToString() };
                    row.Cells.Add(aptCell);

                    TableCell mesCell = new TableCell { Text = mes.ReturnEmptyIfZeroOrNull().ToString() };
                    row.Cells.Add(mesCell);

                    TableCell isyCell = new TableCell { Text = isy.ReturnEmptyIfZeroOrNull().ToString() };
                    row.Cells.Add(isyCell);

                    TableCell arsCell = new TableCell { Text = ars.ReturnEmptyIfZeroOrNull().ToString() };
                    row.Cells.Add(arsCell);

                    TableCell tarCell = new TableCell { Text = tar.ReturnEmptyIfZeroOrNull().ToString() };
                    row.Cells.Add(tarCell);

                    // Insert before totals row
                    TasDurTable.Rows.AddAt(insertIndex++, row);

                    // Accumulate totals
                    totalTM += tm;
                    totalCM += cm;
                    totalApt += aptTotal;
                    totalMes += mes;
                    totalIsy += isy;
                    totalArs += ars;
                    totalTar += tar;
                }

                // Fill totals cells (Top*)
                TopTMCell.Text = totalTM.ReturnEmptyIfZeroOrNull().ToString();
                TopCMCell.Text = totalCM.ReturnEmptyIfZeroOrNull().ToString();
                TopTMCMTopCell.Text = (totalTM + totalCM).ReturnEmptyIfZeroOrNull().ToString();
                TopAptCell.Text = totalApt.ReturnEmptyIfZeroOrNull().ToString();
                TopMesCell.Text = totalMes.ReturnEmptyIfZeroOrNull().ToString();
                TopIsyCell.Text = totalIsy.ReturnEmptyIfZeroOrNull().ToString();
                TopArsCell.Text = totalArs.ReturnEmptyIfZeroOrNull().ToString();
                TopTarCell.Text = totalTar.ReturnEmptyIfZeroOrNull().ToString();
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void KiraDurumuTablosunuDoldur()
        {
            try
            {
                // Find totals row (contains KTopBaslikCell)
                TableRow totalsRow = KTopBaslikCell.Parent as TableRow;
                int headerRows = 2; // first two header rows are static
                int totalsIndex = -1;
                for (int i = 0; i < KiraDurTable.Rows.Count; i++)
                {
                    if (KiraDurTable.Rows[i] == totalsRow)
                    {
                        totalsIndex = i;
                        break;
                    }
                }

                if (totalsIndex == -1)
                {
                    // if not found, append to end
                    totalsIndex = TasDurTable.Rows.Count;
                }

                // Remove any existing region data rows between headers and totals row
                for (int i = totalsIndex - 1; i >= headerRows; i--)
                {
                    TasDurTable.Rows.RemoveAt(i);
                }

                // Prepare totals accumulators

                int totalApt = 0;
                int totalMes = 0;
                int totalIsy = 0;
                int totalArs = 0;
                int totalTar = 0;

                Tasinmaz tasinmaz = new Tasinmaz();

                // Get active regions based on user permission/context
                Bolge bolgeDao = new Bolge();
                var bolgeList = bolgeDao.SelectAktifBolgeler(ProjeConstants.HEPSI_INT);

                // Insert a row per bolge before totals row
                int insertIndex = headerRows;
                foreach (var bolge in bolgeList)
                {
                    if (bolge.Id == ProjeConstants.BOLGE_GENELMUDURLUK_INT ||
                        bolge.Id == ProjeConstants.BOLGE_YURTDISI_INT)
                        continue;

                    // usage counts (APT includes ISHANI)
                    string kiraDurumuStr = string.Empty;
                    int apt = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKirayaUygunluk(bolge.Id, ProjeConstants.KULLANIMSEKLI_APT, ProjeConstants.KIRADURUMU_KIRAYAUYGUN, ProjeConstants.MULKIYETSEKLI_HEPSI);
                    int ishani = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKirayaUygunluk(bolge.Id, ProjeConstants.KULLANIMSEKLI_ISHANI, ProjeConstants.KIRADURUMU_KIRAYAUYGUN, ProjeConstants.MULKIYETSEKLI_HEPSI);
                    int mes = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKirayaUygunluk(bolge.Id, ProjeConstants.KULLANIMSEKLI_MESKEN, ProjeConstants.KIRADURUMU_KIRAYAUYGUN, ProjeConstants.MULKIYETSEKLI_HEPSI);
                    int isy = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKirayaUygunluk(bolge.Id, ProjeConstants.KULLANIMSEKLI_ISYERI, ProjeConstants.KIRADURUMU_KIRAYAUYGUN, ProjeConstants.MULKIYETSEKLI_HEPSI);
                    int ars = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKirayaUygunluk(bolge.Id, ProjeConstants.KULLANIMSEKLI_ARSA, ProjeConstants.KIRADURUMU_KIRAYAUYGUN, ProjeConstants.MULKIYETSEKLI_HEPSI);
                    int tar = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKirayaUygunluk(bolge.Id, ProjeConstants.KULLANIMSEKLI_TARLA, ProjeConstants.KIRADURUMU_KIRAYAUYGUN, ProjeConstants.MULKIYETSEKLI_HEPSI);


                    int total = apt + ishani + mes + isy + ars + tar;

                    // Build row
                    TableRow row = new TableRow { HorizontalAlign = HorizontalAlign.Center };

                    TableCell bolgeCell = new TableCell { CssClass = "btn-primary", Text = string.IsNullOrEmpty(bolge.KisaAdi) ? bolge.Adi : bolge.KisaAdi };
                    row.Cells.Add(bolgeCell);


                    TableCell aptCell = new TableCell { Text = (apt+ishani).ReturnEmptyIfZeroOrNull().ToString() };
                    row.Cells.Add(aptCell);

                    TableCell mesCell = new TableCell { Text = mes.ReturnEmptyIfZeroOrNull().ToString() };
                    row.Cells.Add(mesCell);

                    TableCell isyCell = new TableCell { Text = isy.ReturnEmptyIfZeroOrNull().ToString() };
                    row.Cells.Add(isyCell);

                    TableCell arsCell = new TableCell { Text = ars.ReturnEmptyIfZeroOrNull().ToString() };
                    row.Cells.Add(arsCell);

                    TableCell tarCell = new TableCell { Text = tar.ReturnEmptyIfZeroOrNull().ToString() };
                    row.Cells.Add(tarCell);

                    TableCell tmcmTopCell = new TableCell { Text = total.ReturnEmptyIfZeroOrNull().ToString() };
                    row.Cells.Add(tmcmTopCell);

                    // Insert before totals row
                    KiraDurTable.Rows.AddAt(insertIndex++, row);

                    // Accumulate totals

                    totalApt += apt+ishani;
                    totalMes += mes;
                    totalIsy += isy;
                    totalArs += ars;
                    totalTar += tar;
                }

                // Fill totals cells (Top*)
                //KTopCell.Text = total.ReturnEmptyIfZeroOrNull().ToString();
                KTopAptCell.Text = totalApt.ReturnEmptyIfZeroOrNull().ToString();
                KTopMesCell.Text = totalMes.ReturnEmptyIfZeroOrNull().ToString();
                KTopIsyCell.Text = totalIsy.ReturnEmptyIfZeroOrNull().ToString();
                KTopArsCell.Text = totalArs.ReturnEmptyIfZeroOrNull().ToString();
                KTopTarCell.Text = totalTar.ReturnEmptyIfZeroOrNull().ToString();
                KTopCell.Text = (totalApt + totalMes + totalIsy + totalArs + totalTar).ReturnEmptyIfZeroOrNull().ToString();
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private void KiraGeliriTablosunuDoldur()
        {
            throw new NotImplementedException();
        }

    }
}
