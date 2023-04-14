using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;


namespace TBYS_WebParts.TasinmazBagisWP
{
    [ToolboxItemAttribute(false)]
    public partial class TasinmazBagisWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public TasinmazBagisWP()
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
            TasinmazTablosunuOlustur();
            BagisciTablosunuOlustur();
            BagisTableHeaders();
        }
        private void TasinmazTablosunuOlustur()
        {
            var jsonData = TasinmazTabloJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateDataTableTasinmaz(jsonData); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        
        private string TasinmazTabloJson()
        {
            string jSon = string.Empty;
            try
            {
                List<Tasinmaz> list = GetDataList();
                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;
                jSon = serializer.Serialize(list);
            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper();
                exceptionHelper.Exceptions.Add(exception);
                exceptionHelper.PublishException();
            }
            return jSon;
        }
        private List<Tasinmaz> GetDataList()
        {
            //TasinmazListFactory factory = TasinmazListFactory.Instance;
            //return factory.EnvanterdeOlanTasinmazlar;

            return null;
        }
        private string CreateDataTableTasinmaz(string jsonData)
        {
            string tableString = @"
                if ( jQuery.fn.DataTable.isDataTable('#CustomDataTableTasinmaz') ) {
                    jQuery('#CustomDataTableTasinmaz').DataTable().destroy();
                }
                jQuery('#CustomDataTableTasinmaz tbody').empty();

                jQuery.fn.dataTable.moment('DD.MM.YYYY HH:mm');//sort date
                jQuery('#CustomDataTableTasinmaz').DataTable({
                    data: " + jsonData + @",
                    columns: [
                        { data: 'Id' },
                        { data: 'KullanimSekli' },
                        { data: 'Adres' },
                        { data: 'Ili' },
                        { data: 'Ilcesi' },
                    ],
                    'columnDefs': [
                        { type: 'turkish', targets: [1,2,3,4] },
                    ],
                    'order': [[0, 'asc']],
                    'scrollY': '300px',
                    'language': {
                    'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                        'decimal': ',',
                        'thousands': '.'
                    },
                    responsive: true,
                    dom: 'rt',
                             
                });
            ";
            return tableString;
        }

        private void BagisciTablosunuOlustur()
        {
            var jsonData = BagisciTabloJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateDataTableBagisci(jsonData); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string BagisciTabloJson()
        {
            string jSon = string.Empty;
            try
            {
                List<TasinmazBagisci> list = GetBagisciDataList();
                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;
                jSon = serializer.Serialize(list);
            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper();
                exceptionHelper.Exceptions.Add(exception);
                exceptionHelper.PublishException();
            }
            return jSon;
        }
        private List<TasinmazBagisci> GetBagisciDataList()
        {
            return null;

        }
        private string CreateDataTableBagisci(string jsonData)
        {
            string tableString = @"
                if ( jQuery.fn.DataTable.isDataTable('#CustomDataTableBagisci') ) {
                    jQuery('#CustomDataTableBagisci').DataTable().destroy();
                }
                jQuery('#CustomDataTableBagisci tbody').empty();

                jQuery.fn.dataTable.moment('DD.MM.YYYY HH:mm');//sort date
                jQuery('#CustomDataTableBagisci').DataTable({
                    data: " + jsonData + @",
                    columns: [
                        { data: 'Id' },
                        { data: 'Adi' },
                        { data: 'Soyadi' },
                        { data: 'Adres' },
                        { data: 'Ili' },
                        { data: 'Ilcesi' },
                    ],
                    'columnDefs': [
                        { type: 'turkish', targets: [1,2,3,4,5] },
                    ],
                    'order': [[0, 'asc']],
                    'scrollY': '300px',
                    'language': {
                    'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                        'decimal': ',',
                        'thousands': '.'
                    },
                    responsive: true,
                    dom: 'rt',
                             
                });
            ";
            return tableString;
        }

        #region Bagis Table
        private void BagisTableHeaders()
        {
            BagisTable.Rows.Clear();
            TableHeaderRow thbaslik = new TableHeaderRow();
            TableHeaderCell bagisciBaslikCell = new TableHeaderCell();
            

            bagisciBaslikCell.Text ="Bağışçı(lar)";
            bagisciBaslikCell.Font.Bold = true;
            bagisciBaslikCell.ColumnSpan = 6;
            

            TableHeaderCell tasinmazBaslikCell = new TableHeaderCell();
            tasinmazBaslikCell.Text = "Taşınmaz(lar)";
            tasinmazBaslikCell.Font.Bold = true;
            tasinmazBaslikCell.ColumnSpan = 5;
            
            thbaslik.HorizontalAlign = HorizontalAlign.Center;
            thbaslik.Controls.Add(bagisciBaslikCell);
            thbaslik.Controls.Add(tasinmazBaslikCell);
            BagisTable.Controls.Add(thbaslik);

            TableHeaderRow bagisciTableHeader = new TableHeaderRow();
            TableHeaderCell siraCell = new TableHeaderCell();
            siraCell.Text = "Sıra";
            TableHeaderCell adiSoyadiCell = new TableHeaderCell();
            adiSoyadiCell.Text = "Adı Soyadı";
            TableHeaderCell adresiCell = new TableHeaderCell();
            adresiCell.Text = "Adresi";
            TableHeaderCell iliCell = new TableHeaderCell();
            iliCell.Text = "İli";
            TableHeaderCell ilcesiCell = new TableHeaderCell();
            ilcesiCell.Text = "İlçesi";
            TableHeaderCell duzenleCell = new TableHeaderCell();
            duzenleCell.Text = "Düzenle";

            bagisciTableHeader.Controls.Add(siraCell);
            bagisciTableHeader.Controls.Add(adiSoyadiCell);
            bagisciTableHeader.Controls.Add(adresiCell);
            bagisciTableHeader.Controls.Add(iliCell);
            bagisciTableHeader.Controls.Add(ilcesiCell);
            bagisciTableHeader.Controls.Add(duzenleCell);

            TableHeaderCell tasinmazSiraCell = new TableHeaderCell();
            tasinmazSiraCell.Text = "Sıra";
            TableHeaderCell tasinmazAdresiCell = new TableHeaderCell();
            tasinmazAdresiCell.Text = "Adresi";
            TableHeaderCell tasinmazIliCell = new TableHeaderCell();
            tasinmazIliCell.Text = "İli";
            TableHeaderCell tasinmazIlcesiCell = new TableHeaderCell();
            tasinmazIlcesiCell.Text = "İlçesi";
            TableHeaderCell tasinmazDuzenleCell = new TableHeaderCell();
            tasinmazDuzenleCell.Text = "Düzenle";

            bagisciTableHeader.Controls.Add(tasinmazSiraCell);
            bagisciTableHeader.Controls.Add(tasinmazAdresiCell);
            bagisciTableHeader.Controls.Add(tasinmazIliCell);
            bagisciTableHeader.Controls.Add(tasinmazIlcesiCell);
            bagisciTableHeader.Controls.Add(tasinmazDuzenleCell);

            BagisTable.Controls.Add(bagisciTableHeader);
        }
        #endregion
        protected void CloseBtn_Click(object sender, EventArgs e)
        {

        }

    }
}
