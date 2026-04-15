using System;
using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;
using Utility.ProjeGlobal;
using System.Web;
using System.IO;
using System.Web.UI;
using System.Text;
using Model.IKYS;
using Model.Ortak;
using System.Data;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Globalization;
using static Model.IKYS.Personel;
using System.Web.Script.Serialization;
using Utility.HelperClasses;
using System.Drawing;
using Microsoft.SharePoint;

namespace IKYS_WebParts.MaasOlusturmaWP
{
    [ToolboxItemAttribute(false)]
    public partial class MaasOlusturmaWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public MaasOlusturmaWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        #region Global Variables
        private IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                DateTime tarih = DateTime.Today;
                int ay = tarih.Month;
                // ay eğer Ocak, Nisan, Temmuz ve Ekim aylarından birine eşitse IkramiyeChk.Checked=true; yap
                IkramiyeChk.Checked = false; // başlangıçta ikramiye seçeneği kapalı olacak
                if (ay == 12 || ay == 3 || ay == 6 || ay == 9)
                {
                    IkramiyeChk.Checked = true; //ikramiye ödenecek    
                }
                else
                {
                    IkramiyeChk.Checked = false; // Diğer aylarda ikramiye ödenmeyecek
                }

                TarihDDLDoldur();
                UtilityHelper.SetDDLValue(TarihDDL, tarih.ToString("dd.MM.yyyy"));
                
                TabloOlustur();
            }
            GorunumuAyarla();

        }

        private void GorunumuAyarla()
        {
            //bu tarihe ait kayıt MaasHareket_Table'da var mı kontrol et  
            MaasHareket maasHareket = new MaasHareket();
            maasHareket = maasHareket.SelectByTarih(TarihDDL.SelectedItem == null ? DateTime.Today : TarihDDL.SelectedItem.Value.ConvertToDatetime());
            if (maasHareket != null)
            {
                //eğer varsa, o tarihe ait maaş listesi zaten oluşturulmuş demektir.  
                //o yüzden bu tarihe ait maaş listesi tekrar oluşturulmayacak.  
                // oluşturulan kayıtlar MaasHareket_Table'dan çekilecek.  
                MaasOlusturBtn.Visible = false;
                MaasSilBtn.Visible = true;
                //set background color of TablesDiv to Grey
                TablesDiv.Style["background-color"] = "lightgrey";
                //set background color of TablesDiv to Grey


            }
            else
            {
                TablesDiv.Style["background-color"] = "white";
                //set background color of TablesDiv to White
                MaasOlusturBtn.Visible = true;
                MaasSilBtn.Visible = false;
            }
        }
        #region Methods
        private void TabloOlustur()
        {
            var jsonData = TabloJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);
        }

        private string TabloJson()
        {
            string jSon = string.Empty;

            try
            {
                List<MaasListItem> list = GetDataList();
                var serializer = new JavaScriptSerializer();
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
        private DataTable GetDataTable()
        {
            UtilityHelper.SetDDLValue(TarihDDL, (string.IsNullOrEmpty(TarihDDL.SelectedItem.Value)?DateTime.Today.ToString("dd.MM.yyyy"): TarihDDL.SelectedItem.Value));
            DateTime tarih = TarihDDL.SelectedItem.Value.ConvertToDatetime();

            UcretTanim ucretTanim = new UcretTanim();
            int grupId= ucretTanim.SelectGrupIdByTarih(tarih);
            DataTable dataTable = ucretTanim.SelectMaasListesi(grupId,tarih);
            return dataTable;
        }
        private List<MaasListItem> GetDataList()
        {
            UtilityHelper.SetDDLValue(TarihDDL, (string.IsNullOrEmpty(TarihDDL.SelectedItem.Value) ? DateTime.Today.ToString("dd.MM.yyyy") : TarihDDL.SelectedItem.Value));
            DateTime tarih = TarihDDL.SelectedItem.Value.ConvertToDatetime();
            //ilk olarak bu tarihe ait kayıt MaasHareket_Table'da var mı kontrol et  
            MaasHareket maasHareket = new MaasHareket();
            maasHareket = maasHareket.SelectByTarih(tarih);
            if (maasHareket != null)
            {
                //eğer varsa, o tarihe ait maaş listesi zaten oluşturulmuş demektir.  
                //o yüzden bu tarihe ait maaş listesi tekrar oluşturulmayacak.  
                // oluşturulan kayıtlar MaasHareket_Table'dan çekilecek.  
                List<MaasHareket> maaslist = maasHareket.SelectMaasListesiByTarih(tarih);
                List<MaasListItem> hazirliste = new List<MaasListItem>();
                foreach (MaasHareket item in maaslist)
                {
                    MaasListItem listItem = new MaasListItem();
                    listItem.PersonelId = item.PersonelId;
                    listItem.Adi = item.Adi.Trim();
                    listItem.Soyadi = item.Soyadi.Trim();
                    listItem.AdiSoyadi = item.Adi + " " + item.Soyadi;

                    listItem.Unvan = item.Unvan;
                    listItem.Derece = item.Derece;
                    listItem.Kademe = item.Kademe;
                    listItem.DereceKademeIlerlemeTarihi = item.DereceKademeIlerlemeTarihi.ToString("dd.MM.yyyy");

                    listItem.DereceKademe = listItem.Derece + "/" + listItem.Kademe;
                    listItem.ProtokolSiraNo = item.ProtokolSiraNo;
                    listItem.GrupId = item.GrupId;
                    listItem.Ucret = item.Ucret.ToString("N", culturInfo);
                    listItem.Ikramiye = item.Ikramiye.ToString("N", culturInfo);
                    listItem.Agi = item.Agi.ToString("N", culturInfo);
                    listItem.Toplam = (item.Ucret + item.Agi).ToString("N", culturInfo);
                    hazirliste.Add(listItem);
                }
                return hazirliste;
            }
            else
            {
                DataTable dataTable = GetDataTable();
                List<MaasListItem> yeniliste = new List<MaasListItem>();
                if (dataTable != null)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        int personelId = row["PersonelId"].ConvertToInt();
                        int derece = row["Derece"].ConvertToInt();
                        int kademe = row["Kademe"].ConvertToInt();
                        int protokolSiraNo = row["ProtokolSiraNo"].ReturnZeroIfNull().ConvertToInt();
                        string adi = row["Adi"].ToString();
                        string soyadi = row["Soyadi"].ToString();
                        string unvan = row["Unvan"].ToString();
                        decimal ucret = row["Ucret"].ConvertToDecimal();
                        decimal ikramiye = IkramiyeChk.Checked?ucret:0;
                        decimal agi = row["Agi"].ConvertToDecimal();
                        int grupId = row["GrupId"].ConvertToInt();
                        DateTime dereceKademeIlerlemeTarihi = row["DereceKademeIlerlemeTarihi"].ReturnEmptyIfNull().ConvertToDatetime();

                        MaasListItem listItem = new MaasListItem();
                        listItem.PersonelId = personelId;
                        listItem.Adi = adi.Trim();
                        listItem.Soyadi = soyadi.Trim();
                        listItem.AdiSoyadi = adi.Trim() + " " + soyadi.Trim();
                        listItem.Unvan = unvan;
                        listItem.DereceKademeIlerlemeTarihi = dereceKademeIlerlemeTarihi.ToString("dd.MM.yyyy");
                        listItem.DereceKademeIlerlemeTarihiOrj = dereceKademeIlerlemeTarihi.ToString("dd.MM.yyyy");
                        listItem.Derece = derece;
                        listItem.Kademe = kademe;
                        listItem.Ucret = ucret.ToString("N", culturInfo);
                        listItem.Ikramiye = ikramiye.ToString("N", culturInfo);
                        //DereceKademeIlerlemeTarihi'nin ay'ı TarihDDLDen seçilen tarihin ay'ı ile aynıysa
                        //Kademeyi bir artır, kademe 10'dan büyük vey aeşitse artırma
                        if (kademe < 10)
                        {
                            if (dereceKademeIlerlemeTarihi.Month == tarih.Month && dereceKademeIlerlemeTarihi.Year < tarih.Year) // aynı yılda iki kez maaş oluşturulmayacak, bu yüzden tarih yılı kontrolü de yapılıyor
                            {
                                listItem.DereceKademeIlerlemeTarihi = new DateTime(tarih.Year, tarih.Month, dereceKademeIlerlemeTarihi.Day).ToString("dd.MM.yyyy"); // DereceKademeDegisim'in tarihi, maaşın oluşturulduğu tarih olacak
                                listItem.Kademe = kademe + 1; // Kademe'yi 1 artır
                                Personel personel = new Personel();
                                personel = personel.Select(personelId);
                                //maaşı yeni kademeye göre bul
                                UcretTanim ucretTanim = new UcretTanim();
                                ucret = ucretTanim.SelectUcretByGrupDereceKademe(personel,grupId, derece, listItem.Kademe);
                                listItem.Ucret = ucret.ToString("N", culturInfo); // Ucret'i güncelle
                                listItem.Ikramiye = ucret.ToString("N", culturInfo);
                                ikramiye = IkramiyeChk.Checked ? ucret : 0;
                            }
                        }

                        listItem.DereceKademe = listItem.Derece + "/" + listItem.Kademe;
                        listItem.ProtokolSiraNo = protokolSiraNo;
                        listItem.GrupId = grupId;
                        
                       
                        listItem.Agi = agi.ToString("N", culturInfo);
                        listItem.Toplam = (ucret + ikramiye + agi).ToString("N", culturInfo);
                        yeniliste.Add(listItem);
                    }
                    return yeniliste;
                }
            }
            // Ensure all code paths return a value  
            return new List<MaasListItem>();
        }
        private string CreateDataTable(string jsonData)
        {
            //DataTable için gerekli javascript kodu oluşturuluyor
            // jsonData içinde gelen DereceKademeIlerlemeTarihi'nin ay bölümü TarihDDL.SelectedItem.Value ile aynı ise, tablonun o satırını kırmızı bold yazsın

            // TarihDDL.SelectedItem.Value C# tarafında, bunu JS'ye geçir
            string selectedTarih = TarihDDL.SelectedItem != null ? TarihDDL.SelectedItem.Value : DateTime.Today.ToString("dd.MM.yyyy");
            string ikramiyeGorunsun = IkramiyeChk.Checked ? "{ targets:6, visible:true}," : "{ targets:6, visible:false},";
            string tableString = @"
                if ( jQuery.fn.DataTable.isDataTable('#CustomDataTable') ) {
                    jQuery('#CustomDataTable').DataTable().destroy();
                }
                jQuery('#CustomDataTable tbody').empty();
                var selectedTarih = '" + selectedTarih + @"';
                var selectedMonth = selectedTarih.split('.')[1];
                var selectedYear = selectedTarih.split('.')[2];
                jQuery(document).ready(function () {
                    jQuery('#CustomDataTable').DataTable({
                        data: " + jsonData + @",
                        columns: [
                            { data: 'ProtokolSiraNo' },
                            { data: 'AdiSoyadi' },
                            { data: 'Unvan' },
                            { data: 'DereceKademeIlerlemeTarihi' },
                            { data: 'DereceKademe' },
                            { data: 'Ucret',className: 'dt-body-right dt-head-right'},
                            { data: 'Ikramiye',className: 'dt-body-right dt-head-right'},
                            { data: 'Agi',className: 'dt-body-right dt-head-right'},
                            { data: 'Toplam', className: 'dt-body-right dt-head-right'},
                        ],
                        columnDefs: [
                            { type: 'turkish', targets:[1,2] },
                            { type: 'num', targets: [5,6,7,8] },
                            " + ikramiyeGorunsun + @"
                        ],
                        'order': [[0, 'asc']],// Sıralı
                        'language': {
                            'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                            'decimal': ',',
                            'thousands': '.'
                        },
                        pageLength: 100,
                        responsive: true,
                        destroy: true,
                        dom: 'Bfrtip',
                        buttons: [
                            {
                                extend: 'print',
                                exportOptions: {
                                    columns: ':visible'
                                }
                            },
                            {
                                extend: 'excel',
                                exportOptions: {
                                    columns: ':visible'
                                }
                            },
                            {
                                extend: 'pdf',
                                exportOptions: {
                                    columns: ':visible'
                                }
                            },
                            {
                                extend: 'copy',
                                exportOptions: {
                                    columns: ':visible'
                                }
                            },
                            , 'pageLength', 'colvis'
                        ],
                        'createdRow': function(row, data, dataIndex) {
                            if(data.DereceKademeIlerlemeTarihi){
                                var tarihArr = data.DereceKademeIlerlemeTarihi.split('.');
                                if(tarihArr.length === 3){
                                    var rowMonth = tarihArr[1];
                                    var rowYear = tarihArr[2];
                                    if(rowMonth === selectedMonth ){

// With this line to ensure the text color is set to red using !important:
jQuery(row).find('td').css({'color':'red','font-weight':'bold'});
                                    }
                                }
                            }
                        }
                    });
                });";
            return tableString;
        }
        private void TarihDDLDoldur()
        {
            TarihDDL.Items.Clear();
            // Tarih DDL'si 1 ay sonrasından başlayarak, azalarak her ayın son gününü listeye doldurulacak
            // Başlangıç tarihi olarak bugünün yılının 1 ay sonrası alınır
            // ve her ayın son günü eklenir.
            // Örnek: Eğer bugün 15 Eylül 2023 ise, başlangıç tarihi 1 Ekim 2023 olur ve
            // 31 Ekim, 30 Kasım, 31 Aralık gibi tarihler eklenir.

            DateTime startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(1).AddDays(-1);


            UcretTanim ucretTanim = new UcretTanim();
            DataTable dataTable = ucretTanim.SelectByGrup();
            foreach (DataRow row in dataTable.Rows)
            {
                DateTime bastar = row["BaslangicTarihi"].ConvertToDatetime();
                DateTime bittar = row["BitisTarihi"].ConvertToDatetime();
                int grupId = row["GrupId"].ConvertToInt();
                DateTime songun = new DateTime(bittar.Year,bittar.Month,1).AddMonths(1).AddDays(-1);
                while (songun >= bastar) { 
                    if (songun>startDate)
                    {
                        songun = new DateTime(songun.Year,songun.Month,1).AddDays(-1);
                        continue;
                    }
                    ListItem li = new ListItem(songun.ConvertToDatetimeEmptyIfNull() );
                    TarihDDL.Items.Add(li);
                    songun = new DateTime(songun.Year, songun.Month, 1).AddDays(-1);
                }
            }

        }
        private void TabloyuKaydet()
        {
            UtilityHelper.SetDDLValue(TarihDDL, (string.IsNullOrEmpty(TarihDDL.SelectedItem.Value) ? DateTime.Today.ToString("dd.MM.yyyy") : TarihDDL.SelectedItem.Value));
            DateTime tarih = TarihDDL.SelectedItem.Value.ConvertToDatetime();
            List<MaasListItem> list=GetDataList();

            foreach (MaasListItem item in list)
            {
                MaasHareket maasHareket = new MaasHareket();
                maasHareket.Adi = item.Adi;
                maasHareket.Soyadi = item.Soyadi;
                maasHareket.Unvan = item.Unvan;
                maasHareket.DereceKademeIlerlemeTarihi = item.DereceKademeIlerlemeTarihiOrj.ConvertToDatetime();
                if (maasHareket.DereceKademeIlerlemeTarihi.Month == tarih.Month && maasHareket.DereceKademeIlerlemeTarihi.Year < tarih.Year)
                {
                    
                    //Kademeyi bir artır, kademe 10'dan büyük vey aeşitse artırma
                    if (item.Kademe < 10)
                    {
                        // Bunu yapmak için: DereceKademeDegisim_Table'a bu personel için bir kayıt ekle
                        DereceKademeDegisim dereceKademeDegisim = new DereceKademeDegisim();

                        dereceKademeDegisim.Derece = item.Derece;
                        //dereceKademeDegisim.Aciklama alanına içinde bulunduğumuz ay/yıl maaşı oluşturulurken otomatik güncellendiğini yaz
                        dereceKademeDegisim.Aciklama = tarih.ToString("MMMM yyyy") + " maaşı oluşturulurken otomatik güncellendi.";
                        dereceKademeDegisim.Kademe = item.Kademe; // Kademe getDataList içinde 1 artırıldı
                        dereceKademeDegisim.Derece = item.Derece; 
                        dereceKademeDegisim.PersonelId = item.PersonelId;
                        dereceKademeDegisim.DegisimTarihi = new DateTime(tarih.Year, tarih.Month, item.DereceKademeIlerlemeTarihi.ConvertToDatetime().Day); // DereceKademeDegisim'in tarihi, maaşın oluşturulduğu tarih olacak
                        dereceKademeDegisim.Degisim = "Kademe Yükseltme";
                        dereceKademeDegisim.Derece = item.Derece; // Derece'yi de aynı şekilde al
                        dereceKademeDegisim.Save();
                        maasHareket.DereceKademeIlerlemeTarihi = dereceKademeDegisim.DegisimTarihi;
                        //IsBligileri_Table'da Derece,Kademe ve DereceKademeIlerlemeTarihi alanları güncellensin
                        //IsBilgileri personelIsBilgileri = new IsBilgileri();
                        //personelIsBilgileri = personelIsBilgileri.SelectByPersonelId(item.PersonelId);
                        //if (personelIsBilgileri != null)
                        //{
                        //    personelIsBilgileri.Derece = item.Derece;
                        //    personelIsBilgileri.Kademe = dereceKademeDegisim.Kademe;
                        //    personelIsBilgileri.DereceKademeIlerlemeTarihi = dereceKademeDegisim.DegisimTarihi;
                        //    personelIsBilgileri.Update();
                        //}
                        item.Kademe = dereceKademeDegisim.Kademe;
                    }
                }
                maasHareket.Derece = item.Derece;
                maasHareket.Kademe = item.Kademe;
                maasHareket.Ucret = item.Ucret.ConvertToDecimal();
                maasHareket.Agi = item.Agi.ConvertToDecimal();
                maasHareket.ToplamUcret = item.Toplam.ConvertToDecimal();
                maasHareket.ProtokolSiraNo = item.ProtokolSiraNo;
                maasHareket.GrupId = item.GrupId;
                maasHareket.Tarih = TarihDDL.SelectedItem.Value.ConvertToDatetime();
                maasHareket.Save();
            }
        }

        private void TabloyuSil()
        {
            MaasHareket maasHareket = new MaasHareket();
            // ilk olarak bu tarihe ait kayıt MaasHareket_Table'da var mı kontrol et
            maasHareket = maasHareket.SelectByTarih(TarihDDL.SelectedItem.Value.ConvertToDatetime());
            if (maasHareket == null)
            {
                MessageHelper.PublishMessage("Silinecek bir maaş kaydı bulunamadı.", ProjeConstants.MESAJ_BILGI);
                return;
            }
            // Eğer varsa, grupId'sini belirle
            int grupId = maasHareket.GrupId;
            // MaasHareket_Table'dan grupId'ye göre sil
            bool isDeleted = maasHareket.DeleteByGrupId(grupId);

        }
        private void KaydetModalAc()
        {

            MessageTitleLbl.Text = "Maaş Oluşturma";
            MessageTextLbl.Text = TarihDDL.SelectedItem.Text + " Maaşı kaydedilsin ve sabitlensin mi?";
            KaydetNowBtn.Visible = true;
            SilNowBtn.Visible = true;
            var openPopup = "OpenModal();";
            UtilityHelper.ScriptCalistir(openPopup);
            TabloOlustur();
        }
        
        private void SilModalAc()
        {

            MessageTitleLbl.Text = "Maaş Silme";
            MessageTextLbl.Text = TarihDDL.SelectedItem.Text + " Maaşı silinsin mi?";
            KaydetNowBtn.Visible = false;
            SilNowBtn.Visible = true;
            var openPopup = "OpenModal();";
            UtilityHelper.ScriptCalistir(openPopup);
            TabloOlustur();
        }
        #endregion
        #region Events
        protected void MaasOlusturBtn_Click(object sender, EventArgs e)
        {
            KaydetModalAc();
        }
        protected void MaasSilBtn_Click(object sender, EventArgs e)
        {
            SilModalAc();
        }
        protected void KaydetNowBtn_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime tarih = TarihDDL.SelectedItem.Value.ConvertToDatetime();

                MaasHareket maasHareket = new MaasHareket();
                maasHareket = maasHareket.SelectByTarih(tarih);
                if (maasHareket == null)
                {
                    TabloyuKaydet();
                    TabloOlustur();
                }
                GorunumuAyarla();
            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper(exception);
                Exception exceptionInfo = new Exception("Maaş kaydedilemedi");
                exceptionHelper.Exceptions.Add(exceptionInfo);
                exceptionHelper.PublishException();
            }
        }
        protected void SilNowBtn_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime tarih = TarihDDL.SelectedItem.Value.ConvertToDatetime();

                MaasHareket maasHareket = new MaasHareket();
                maasHareket = maasHareket.SelectByTarih(tarih);
                if (maasHareket != null)
                {
                    TabloyuSil();
                    TabloOlustur();
                }
                GorunumuAyarla();
            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper(exception);
                Exception exceptionInfo = new Exception("Maaş kaydedilemedi");
                exceptionHelper.Exceptions.Add(exceptionInfo);
                exceptionHelper.PublishException();
            }
        }
        protected void TarihDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            DateTime tarih =TarihDDL.SelectedItem.Value.ConvertToDatetime();
            int ay = tarih.Month;
            // ay eğer Ocak, Nisan, Temmuz ve Ekim aylarından birine eşitse IkramiyeChk.Checked=true; yap
            IkramiyeChk.Checked = false; // başlangıçta ikramiye seçeneği kapalı olacak
            if (ay == 12 || ay == 3 || ay == 6 || ay == 9)
            {
                IkramiyeChk.Checked = true; //ikramiye ödenecek    
            }
            else
            {
                IkramiyeChk.Checked = false; // Diğer aylarda ikramiye ödenmeyecek
            }
            TabloOlustur();
            GorunumuAyarla();
        }

        protected void IkramiyeChk_CheckedChanged(object sender, EventArgs e)
        {
            DateTime tarih = TarihDDL.SelectedItem.Value.ConvertToDatetime();
            int ay = tarih.Month;
            // ay eğer Ocak, Nisan, Temmuz ve Ekim aylarından birine eşitse IkramiyeChk.Checked=true; yap
            IkramiyeChk.Checked = false; // başlangıçta ikramiye seçeneği kapalı olacak
            if (ay == 12 || ay == 3 || ay == 6 || ay == 9)
            {
                IkramiyeChk.Checked = true; //ikramiye ödenecek    
            }
            else
            {
                IkramiyeChk.Checked = false; // Diğer aylarda ikramiye ödenmeyecek
            }
            TabloOlustur();
            GorunumuAyarla();
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        #endregion
        #region class
        private class MaasListItem
        {
            public string Adi { get; set; }
            public string Soyadi { get; set; }
            public string AdiSoyadi { get; set; }
            public string Unvan { get; set; }
            public int ProtokolSiraNo { get; set; }
            public string DereceKademeIlerlemeTarihi { get; set; }
            public string DereceKademeIlerlemeTarihiOrj { get; set; }
            public int Derece{ get; set; }
            public int Kademe { get; set; }
            public string DereceKademe { get; set; }
            public string Ucret { get; set; }
            public string Ikramiye { get; set; }
            public string Agi { get; set; }
            public string Toplam { get; set; }
            public int GrupId  { get; set; }
            public int PersonelId  { get; set; }
        }
        #endregion
    }
}
