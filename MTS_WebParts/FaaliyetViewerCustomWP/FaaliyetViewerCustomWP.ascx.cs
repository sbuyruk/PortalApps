using Microsoft.Web.Hosting.Administration;
using Model.IKYS;
using Model.MTS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Web.UI;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;
using System.Data;
using System.Linq;
using Model.Portal;

namespace MTS_WebParts.FaaliyetViewerCustomWP
{
    [ToolboxItemAttribute(false)]
    public partial class FaaliyetViewerCustomWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public FaaliyetViewerCustomWP()
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
            TableHeaders();
            TableRows();
        }

        private void TableRows()
        {
            DateTime date = new DateTime(2022, 12, 01);
            for (int i = 0; i < 5; i++)
            {
                TableRow dayRow = new TableRow();
                for (int j = 1; j <= 7; j++)
                {
                    TableCell dayH1 = new TableCell
                    {
                        Text = GetDayText(date, j),
                        BorderStyle = BorderStyle.Solid,
                        HorizontalAlign = HorizontalAlign.Left,

                    };
                    if ((date.DayOfWeek == DayOfWeek.Saturday) ||
                        (date.DayOfWeek == DayOfWeek.Sunday))
                    {
                        dayH1.BackColor = Color.LightPink;
                    }

                    if (!string.IsNullOrEmpty(dayH1.Text.Trim()) || date.Day > 1)
                    {
                        date = date.AddDays(1);
                    }
                    dayRow.Controls.Add(dayH1);

                }

                CalendarTable.Controls.Add(dayRow);
            }


        }

        private string GetDayText(DateTime date, int day)
        {
            List<FaaliyetListItem> gunlukFaaliyetListesi = new List<FaaliyetListItem>();
            string text = string.Empty;
            int dayInt = (int)date.DayOfWeek;
            if (dayInt == day)
            {
                text = "<h2 align='right'>" + date.Day.ToString() + "</h2> ";
                Faaliyet faaliyet = new Faaliyet();
                List<Faaliyet> faaliyetListesi = faaliyet.SelectByTarihReturnList(date);
                foreach (var item in faaliyetListesi)
                {
                    FaaliyetListItem gunlukFaaliyet = new FaaliyetListItem
                    {
                        Amaci = item.FaaliyetAmaci,
                        BaslangicTarihi = item.BaslangicTarihi,
                        BitisTarihi = item.BitisTarihi,
                        Durumu = item.FaaliyetDurumu,
                        Id = item.Id,
                        Konusu = item.FaaliyetKonusu,
                        Yeri = item.FaaliyetYeriStr,
                        Tipi = item.FaaliyetTipi,
                    };
                    gunlukFaaliyetListesi.Add(gunlukFaaliyet);
                }
                Toplanti toplantiDao = new Toplanti();
                List<Toplanti> toplantiListesi = toplantiDao.SelectByKatilimciTarih(ProjeConstants.GENELMUDUR_PERSONELID, date);
                foreach (var item in toplantiListesi)
                {
                    FaaliyetListItem gunlukFaaliyet = new FaaliyetListItem
                    {
                        Amaci = ProjeConstants.FAALIYET_AMACI_TOPLANTI_INT.ConvertToInt(),
                        BaslangicTarihi = item.BaslangicTarihi,
                        BitisTarihi = item.BitisTarihi,
                        Id = item.Id,
                        Konusu = item.ToplantiKonusu,
                        Yeri = item.ToplantiYeri.ToString(),
                    };
                    gunlukFaaliyetListesi.Add(gunlukFaaliyet);
                }


                gunlukFaaliyetListesi = gunlukFaaliyetListesi.OrderBy(c => c.BaslangicTarihi).ToList();
                foreach (var item in gunlukFaaliyetListesi)
                {

                    string konuStr = " " + item.Konusu;
                    string iptalStrAc = string.Empty;
                    string iptalStrKapa = string.Empty;
                    string renkStr = string.Empty;
                    if (item.Durumu == ProjeConstants.FAALIYET_DURUMU_IPTALEDILDI_INT)
                    {
                        iptalStrAc = " <s>";
                        iptalStrKapa = " </s>";
                    }

                    FaaliyetRengi faaliyetRengi = RenkBelirle(item);
                    renkStr = " style='background-color: " + faaliyetRengi.BackgroundColor + "; color:" + faaliyetRengi.TextColor + "' ";
                    konuStr = iptalStrAc + konuStr + iptalStrKapa;
                    text += @"
                            <p" + renkStr + "><b>*<u>" + item.BaslangicTarihi.ToString("HH:mm") + "</b></u>:" +
                            konuStr + "</p>";
                }

            }

            return text;
        }
        public FaaliyetRengi RenkBelirle(FaaliyetListItem item)
        {

            FaaliyetRengi faaliyetRengi = new FaaliyetRengi();
            switch (item.Amaci.ToString())
            {
                case ProjeConstants.FAALIYET_AMACI_TOPLANTI_INT:
                    {
                        faaliyetRengi.BackgroundColor = Color.Orange.Name;
                        faaliyetRengi.TextColor = Color.White.Name;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_ZIYARET_INT:
                    {
                        faaliyetRengi.BackgroundColor = Color.Blue.Name;
                        faaliyetRengi.TextColor = Color.White.Name;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_DAVET_INT:
                    {
                        faaliyetRengi.BackgroundColor = Color.Green.Name;
                        faaliyetRengi.TextColor = Color.White.Name;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_YILDONUMU_INT:
                    {
                        faaliyetRengi.BackgroundColor = Color.Aqua.Name;
                        faaliyetRengi.TextColor = Color.Black.Name;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_DOGUMGUNU_INT:
                    {
                        faaliyetRengi.BackgroundColor = Color.Aquamarine.Name;
                        faaliyetRengi.TextColor = Color.Black.Name;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_OZELCALISMA_INT:
                    {
                        faaliyetRengi.BackgroundColor = Color.LightBlue.Name;
                        faaliyetRengi.TextColor = Color.White.Name;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_IZIN_INT:
                    {
                        faaliyetRengi.BackgroundColor = Color.Aqua.Name;
                        faaliyetRengi.TextColor = Color.White.Name;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_RESMITATIL_INT:
                    {
                        faaliyetRengi.BackgroundColor = Color.MediumVioletRed.Name;
                        faaliyetRengi.TextColor = Color.White.Name;
                        break;
                    }
                default:
                    break;
            }
            if (item.Durumu.Equals(ProjeConstants.FAALIYET_DURUMU_PLANLANDI_INT.ToString()))
            {
                faaliyetRengi.BackgroundColor = Color.LightGray.Name;
                faaliyetRengi.TextColor = Color.Black.Name;
            }
            return faaliyetRengi;
        }
        private void TableHeaders()
        {
            CalendarTable.Rows.Clear();
            TableHeaderRow thbaslik = new TableHeaderRow();
            TableHeaderCell baslikCell = new TableHeaderCell();


            baslikCell.Text = "Aralık 2022";
            baslikCell.Font.Bold = true;
            baslikCell.ColumnSpan = 7;
            thbaslik.HorizontalAlign = HorizontalAlign.Center;
            thbaslik.Controls.Add(baslikCell);
            CalendarTable.Controls.Add(thbaslik);

            TableHeaderRow dayHeaderRow = new TableHeaderRow();

            TableHeaderCell dayH1 = new TableHeaderCell
            {
                Text = GetDayOfWeek(DayOfWeek.Monday),
                BorderStyle = BorderStyle.Solid,
                HorizontalAlign = HorizontalAlign.Center,
            };

            TableHeaderCell dayH2 = new TableHeaderCell
            {
                Text = GetDayOfWeek(DayOfWeek.Tuesday),
                BorderStyle = BorderStyle.Solid,
                HorizontalAlign = HorizontalAlign.Center,
            };
            TableHeaderCell dayH3 = new TableHeaderCell
            {
                Text = GetDayOfWeek(DayOfWeek.Wednesday),
                BorderStyle = BorderStyle.Solid,
                HorizontalAlign = HorizontalAlign.Center,
            };
            TableHeaderCell dayH4 = new TableHeaderCell
            {
                Text = GetDayOfWeek(DayOfWeek.Thursday),
                BorderStyle = BorderStyle.Solid,
                HorizontalAlign = HorizontalAlign.Center,
            };
            TableHeaderCell dayH5 = new TableHeaderCell
            {
                Text = GetDayOfWeek(DayOfWeek.Friday),
                BorderStyle = BorderStyle.Solid,
                HorizontalAlign = HorizontalAlign.Center,
            };
            TableHeaderCell dayH6 = new TableHeaderCell
            {
                Text = GetDayOfWeek(DayOfWeek.Saturday),
                BorderStyle = BorderStyle.Solid,
                HorizontalAlign = HorizontalAlign.Center,
                BackColor = Color.LightPink,
            };
            TableHeaderCell dayH7 = new TableHeaderCell
            {
                Text = GetDayOfWeek(DayOfWeek.Sunday),
                BorderStyle = BorderStyle.Solid,
                HorizontalAlign = HorizontalAlign.Center,
                BackColor = Color.LightPink,
            };
            dayHeaderRow.Controls.Add(dayH1);
            dayHeaderRow.Controls.Add(dayH2);
            dayHeaderRow.Controls.Add(dayH3);
            dayHeaderRow.Controls.Add(dayH4);
            dayHeaderRow.Controls.Add(dayH5);
            dayHeaderRow.Controls.Add(dayH6);
            dayHeaderRow.Controls.Add(dayH7);

            CalendarTable.Controls.Add(dayHeaderRow);
        }

        private string GetDayOfWeek(DayOfWeek dayOfWeek)
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            string retval = "";
            DateTime date = DateTime.Today;
            for (int i = 0; i < 7; i++)
            {
                if (date.DayOfWeek == dayOfWeek)
                    return date.ToString("dddd"); //date.DayOfWeek.ToString(); //date.DayOfWeek.ToString("dddd");

                else
                    date = date.AddDays(1);
            }
            return retval;
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
                Page.Response.Redirect(newUrl);
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void ExcelBtn_Click(object sender, EventArgs e)
        {

            TableHeaders();
            TableRows();
            ExportToExcel();
        }
        protected void ExportToExcel()
        {
            string filename = "FaaliyetTakvimi" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls";
            Page.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1254");
            Page.Response.Charset = "windows-1254";//ISO-8859-9
            StringWriter tw = new System.IO.StringWriter();
            HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);

            //Get the HTML for the control.             
            CalendarTable.RenderControl(hw);
            //Write the HTML back to the browser.
            //Response.ContentType = application/vnd.ms-excel;
            Page.Response.ContentType = "application/ms-excel";
            Page.Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + "");
            this.EnableViewState = false;
            string s = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\n<html xmlns=\"http://www.w3.org/1999/xhtml\">\n<head>\n<title></title>\n<meta http-equiv=\"Content-Type\" content=\"text/html; charset=windows-1254\" />\n<style>\n</style>\n</head>\n<body>\n";
            Page.Response.Write(s + tw.ToString());
            Page.Response.End();

        }
        public class FaaliyetRengi
        {
            public string BackgroundColor { get; set; }
            public string TextColor { get; set; }
        }

        public class FaaliyetListItem
        {
            public int Id { get; set; }
            public string Tipi { get; set; }
            public string Yeri { get; set; }
            public string Konusu { get; set; }
            public int Amaci { get; set; }
            public int Durumu { get; set; }
            public DateTime BaslangicTarihi { get; set; }
            public DateTime BitisTarihi { get; set; }
        }
    }
}
