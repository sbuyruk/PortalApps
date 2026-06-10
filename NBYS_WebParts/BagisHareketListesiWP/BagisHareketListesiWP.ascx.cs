using Model.NBYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.BagisHareketListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class BagisHareketListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public BagisHareketListesiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string SecilenAyQS
        {
            get
            {

                if (ViewState["SecilenAy"] == null)
                {
                    if (Page.Request.QueryString["SecilenAy"] != null)
                    {
                        ViewState["SecilenAy"] = Page.Request.QueryString["SecilenAy"];
                    }
                    else
                    {
                        ViewState["SecilenAy"] = string.Empty;
                    }
                }
                return ViewState["SecilenAy"].ToString();
            }

            set
            {
                ViewState["SecilenAy"] = value;
            }
        }
        private string SecilenYilQS
        {
            get
            {

                if (ViewState["SecilenYil"] == null)
                {
                    if (Page.Request.QueryString["SecilenYil"] != null)
                    {
                        ViewState["SecilenYil"] = Page.Request.QueryString["SecilenYil"];
                    }
                    else
                    {
                        ViewState["SecilenYil"] = string.Empty;
                    }
                }
                return ViewState["SecilenYil"].ToString();
            }

            set
            {
                ViewState["SecilenYil"] = value;
            }
        }
        private string SecilenIlQS
        {
            get
            {

                if (ViewState["SecilenIl"] == null)
                {
                    if (Page.Request.QueryString["SecilenIl"] != null)
                    {
                        ViewState["SecilenIl"] = Page.Request.QueryString["SecilenIl"];
                    }
                    else
                    {
                        ViewState["SecilenIl"] = string.Empty;
                    }
                }
                return ViewState["SecilenIl"].ToString();
            }

            set
            {
                ViewState["SecilenIl"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    DDLDoldur();
                    SetDDLValues();
                    TabloOlustur();
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }


        }
        private void DDLDoldur()
        {
            AyDDLDoldur();
            YilDDLDoldur();
            IlDDLDoldur();
        }
        private void AyDDLDoldur()
        {
            AyDDL.Items.Add(new ListItem(ProjeConstants.HEPSI, ProjeConstants.HEPSI_INT.ToString()));
            AyDDL.Items.Add(new ListItem("Ocak", "1"));
            AyDDL.Items.Add(new ListItem("Şubat", "2"));
            AyDDL.Items.Add(new ListItem("Mart", "3"));
            AyDDL.Items.Add(new ListItem("Nisan", "4"));
            AyDDL.Items.Add(new ListItem("Mayıs", "5"));
            AyDDL.Items.Add(new ListItem("Haziran", "6"));
            AyDDL.Items.Add(new ListItem("Temmuz", "7"));
            AyDDL.Items.Add(new ListItem("Ağustos", "8"));
            AyDDL.Items.Add(new ListItem("Eylül", "9"));
            AyDDL.Items.Add(new ListItem("Ekim", "10"));
            AyDDL.Items.Add(new ListItem("Kasım", "11"));
            AyDDL.Items.Add(new ListItem("Aralık", "12"));

        }
        private void YilDDLDoldur()
        {
            var year = DateTime.Now.Year;
            for (int i = 1987; i <= year; i++)
            {
                YilDDL.Items.Add(new ListItem(i.ToString(), i.ToString()));
            }
        }
        private void IlDDLDoldur()
        {
            if (IliDDL.SelectedItem == null)
            {
                IliDDL.Items.Clear();

                Il pIl = new Il();
                List<Il> list = pIl.SelectAll<Il>();
                IliDDL.Items.Add(new ListItem(ProjeConstants.HEPSI, ProjeConstants.IL_HEPSI.ToString()));
                foreach (Il il in list)
                {
                    IliDDL.Items.Add(new ListItem(il.IlAdi, il.Id.ToString()));
                }
            }

        }
        private void SetDDLValues()
        {
            try
            {
                //açılışta ay ve yılı querystring ile gelen ay ve yıla eşitle, boş geldiyse geçen aya/yıla eşitle

                //ay
                string ay = !string.IsNullOrEmpty(SecilenAyQS) ? SecilenAyQS : DateTime.Today.Month.ReturnEmptyIfNull().ToString();
                ListItem AyItem = new ListItem();
                if (!string.IsNullOrEmpty(ay))
                    AyItem = AyDDL.Items.FindByValue(ay);

                if (AyItem != null)
                {
                    AyDDL.SelectedValue = AyItem.Value;
                    SecilenAyQS = AyItem.Value;
                }

                //yil
                string yil = !string.IsNullOrEmpty(SecilenYilQS) ? SecilenYilQS : DateTime.Today.Year.ReturnEmptyIfNull().ToString();
                ListItem YilItem = new ListItem();
                if (!string.IsNullOrEmpty(yil))
                    YilItem = YilDDL.Items.FindByValue(yil);

                if (YilItem != null)
                {
                    YilDDL.SelectedValue = YilItem.Value;
                    SecilenYilQS = YilItem.Value;
                }

                //il
                string il = !string.IsNullOrEmpty(SecilenIlQS) ? SecilenIlQS : ProjeConstants.IL_HEPSI.ToString();
                ListItem IlItem = new ListItem();
                if (!string.IsNullOrEmpty(il))
                    IlItem = IliDDL.Items.FindByValue(il);

                if (IlItem != null)
                {
                    IliDDL.SelectedValue = IlItem.Value;
                    SecilenIlQS = IlItem.Value;
                }

            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }



        }
        private DataTable GetBagisHareketDataTable()
        {
            NakitBagisHareket nbh = new NakitBagisHareket();
            int ilId = IliDDL.SelectedItem.Value.ConvertToInt();
            DataTable dt = nbh.SelectByDurumTarihReturnDataTable(AyDDL.SelectedItem.Value, YilDDL.SelectedItem.Value, ilId);
            return dt;
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
        protected void IliDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                SecilenIlQS = IliDDL.SelectedItem.Value.ToString();
                TabloOlustur();
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }

        }
        private void TabloOlustur()
        {
            var jsonData = TabloJson(); //veri çekilip json'a çevriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string TabloJson()
        {
            string jSon = string.Empty;

            try
            {
                NakitBagisHareket nbh = new NakitBagisHareket();
                int ilId = IliDDL.SelectedItem.Value.ConvertToInt();
                jSon = nbh.SelectByDurumTarihReturnJson(AyDDL.SelectedItem.Value, YilDDL.SelectedItem.Value, ilId);

            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper();
                exceptionHelper.Exceptions.Add(exception);
                exceptionHelper.PublishException();
            }
            return jSon;
        }
        private string CreateDataTable(string jsonData)
        {
            string tableString = @"
        jQuery(document).ready(function () {

            jQuery('#CustomDataTable').DataTable({
                data: " + jsonData + @",
                columns: [
                    { data: 'BagisId' },
                    { data: 'Adi' },
                    { data: 'BagisTarihi' },
                    { data: 'BagisMiktari', 'width': '10%', 'className': 'text-end' },
                    { data: 'Ili', 'width': '14%' },
                    { data: 'TCKimlikNo' },
                    { data: 'Telefon' },
                    { data: 'Banka' },
                    { data: 'Aciklama' },

                ],
                'order': [[2, 'desc']],
                columnDefs:
                [
                    {
                targets: 2, render: function(data) {
                        return moment(data).format('DD.MM.YYYY');
                    }
                }
                ],
                'language': {
                    'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                    'decimal': ',',
                    'thousands': '.'
                },
                responsive: true,
                dom: 'Bfrtip',
                buttons:
                [
                    {
                extend: 'print',
                        exportOptions:
                    {
                    columns: ':visible'
                        }
                },
                    {
                      extend: 'excel',
                      exportOptions: {
                          columns: ':visible',
                          format: {
                              body: function(data, row, column, node) {
                                  data = $('<p>' + data + '</p>').text();
                                  return $.isNumeric(data.replace(',', '.')) ? data.replace(',', '.') : data;
                              }
                          }
                      },
                },
                    {
                extend: 'pdf',
                        exportOptions:
                    {
                    columns: ':visible'
                        }
                },
                    {
                extend: 'copy',
                        exportOptions:
                    {
                    columns: ':visible'
                        }
                },
                    , 'pageLength', 'colvis'
                ]



            });
        });
        ";
            return tableString;
        }
        protected void YilDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                SecilenYilQS = YilDDL.SelectedItem.Value.ToString();
                TabloOlustur();
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void AyDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                SecilenAyQS = AyDDL.SelectedItem.Value.ToString();
                TabloOlustur();
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void ExcelBtn_Click(object sender, EventArgs e)
        {
            try
            {
                ExportToExcel();
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void ExportToExcel()
        {
            GridView GridView1 = new GridView();
            GridView1.AllowPaging = false;

            GridView1.DataSource = GetBagisHareketDataTable();
            GridView1.DataBind();

            Page.Response.Clear();
            Page.Response.Buffer = true;

            string filename = "BagisListesi" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls";
            Page.Response.AddHeader("content-disposition", "attachment;filename=" + filename);

            Page.Response.ContentType = "application/ms-excel";
            Page.Response.ContentEncoding = System.Text.Encoding.Unicode;
            Page.Response.BinaryWrite(System.Text.Encoding.Unicode.GetPreamble());

            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);

            for (int i = 0; i < GridView1.Rows.Count; i++)
            {
                GridView1.Rows[i].Attributes.Add("class", "textmode");
            }

            GridView1.RenderControl(hw);

            string style = @"<style> .textmode { mso-number-format:\@; } </style>";
            Page.Response.Write(style);
            Page.Response.Write(sw.ToString());
            Page.Response.Flush();
            Page.Response.End();
        }
    }
}
