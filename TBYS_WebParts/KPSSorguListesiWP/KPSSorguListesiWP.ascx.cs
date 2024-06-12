using Microsoft.SharePoint;
using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.KPSSorguListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class KPSSorguListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public KPSSorguListesiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }

        private List<SorgulanacakKisi> SorgulanacakKisilistesiQS
        {
            get
            {

                if (ViewState["SorgulanacakKisilistesi"] == null)
                {
                    if (Page.Request.QueryString["SorgulanacakKisilistesi"] != null)
                    {
                        ViewState["SorgulanacakKisilistesi"] = Page.Request.QueryString["SorgulanacakKisilistesi"];
                    }
                    else
                    {
                        ViewState["SorgulanacakKisilistesi"] = string.Empty;
                    }
                }
                return (List<SorgulanacakKisi>) ViewState["SorgulanacakKisilistesi"]; 
            }

            set
            {
                ViewState["SorgulanacakKisilistesi"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                TabloOlustur();
            }
            //TabloOlustur();
        }
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
                SorgulanacakKisilistesiQS = GetDataList();
                var serializer = new JavaScriptSerializer();
                jSon = serializer.Serialize(SorgulanacakKisilistesiQS);
            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper();
                exceptionHelper.Exceptions.Add(exception);
                exceptionHelper.PublishException();
            }
            return jSon;
        }
        private List<SorgulanacakKisi> GetDataList()
        {
            TasinmazBagisci bagisciDao = new TasinmazBagisci();

            List<TasinmazBagisci> list = bagisciDao.SelectByFilters(SagVefatChk.Checked, CiplakMukiyetChk.Checked,
                TCKimlikChk.Checked, DogumTarihiChk.Checked);
            List<SorgulanacakKisi> SorgulanacakBagisciListesi = list.Select(a => new SorgulanacakKisi()
            {
                KimlikNo = a.TCKimlikNo,
                Adi = a.Adi,
                Soyadi = a.Soyadi,
                DogumTarihi = a.DogumTarihi,
                SagVefat=a.Sag_vefat,
                Sorgulanan= "Bağışçı",
                Cikar = "<a class='btn btn-outline-danger' onclick=CikarButtonClick(" + a.TCKimlikNo + ");>ÇIKAR</a>"
            }).ToList();
            var sorgulanacakTaahhutList = new List<SorgulanacakKisi>();
            if (TaahhutChk.Checked)
            {
                TasinmazTaahhut ttDao = new TasinmazTaahhut();

                List<TasinmazTaahhut> ttlist = ttDao.SelectByFilters(SagVefatChk.Checked, 
                    TCKimlikChk.Checked, DogumTarihiChk.Checked, ProjeConstants.BOLGE_HEPSI_INT);
                sorgulanacakTaahhutList = ttlist.Select(a => new SorgulanacakKisi()
                {
                    KimlikNo = a.TCKimlikNo,
                    Adi = a.Adi,
                    Soyadi = a.Soyadi,
                    DogumTarihi = a.DogumTarihi,
                    SagVefat = a.Sag_vefat,
                    VefatTarihi=a.VefatTarihi,
                    Sorgulanan = "Taahhüt Verilen Kişi",
                    Cikar= "<a class='btn btn-outline-danger' onclick=CikarButtonClick(" + a.TCKimlikNo + ");>ÇIKAR</a>"
            }).ToList();


            }
            
            var sorgulanacakVasiyetcitList = new List<SorgulanacakKisi>();
            if (VasiyetciChk.Checked)
            {
                Vasiyetci vasiyetciDao = new Vasiyetci();

                List<Vasiyetci> vasiyetcilist = vasiyetciDao.SelectByFilters(SagVefatChk.Checked,
                    TCKimlikChk.Checked, DogumTarihiChk.Checked);
                sorgulanacakVasiyetcitList = vasiyetcilist.Select(a => new SorgulanacakKisi()
                {
                    KimlikNo = a.TCKimlikNo,
                    Adi = a.Adi,
                    Soyadi = a.Soyadi,
                    DogumTarihi = a.DogumTarihi,
                    SagVefat = a.SagVefat,
                    VefatTarihi = a.VefatTarihi,
                    Sorgulanan = "Vasiyetçi",
                    Cikar = "<a class='btn btn-outline-danger' onclick=CikarButtonClick(" + a.TCKimlikNo + ");>ÇIKAR</a>"
                }).ToList();


            }
            List<SorgulanacakKisi> sonucListe = SorgulanacakBagisciListesi.Union(sorgulanacakTaahhutList).ToList().Union(sorgulanacakVasiyetcitList).ToList();

            return sonucListe;
        }
        private string CreateDataTable(string jsonData)
        {
            string tableString = @"
                jQuery.fn.dataTable.moment('DD.MM.YYYY');//sort date
                jQuery('#CustomDataTable').DataTable({
                    data: " + jsonData + @",
                    columns: [
                        { data: 'Sorgulanan' },
                        { data: 'Adi' },
                        { data: 'Soyadi' },
                        { data: 'KimlikNo' },
                        { data: 'DogumTarihi' },
                        { data: 'Cikar' },
                    ],
                    'columnDefs': [
                        { type: 'turkish', targets: [0,1,2] },
                        {targets:4, render:function(data){
                            return moment(data).format('DD.MM.YYYY');}
                        },
                    ],
                    'order': [[0, 'asc']],//sort 
                    'language': {
                    'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                        'decimal': ',',
                        'thousands': '.'
                    },
                    responsive: true,
                    stateSave: true,
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
                    

                });

            ";

            return tableString;
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        protected void TCKimlikChk_CheckedChanged(object sender, EventArgs e)
        {
            TabloOlustur();
        }
        protected void DogumTarihiChk_CheckedChanged(object sender, EventArgs e)
        {
            TabloOlustur();
        }
        protected void CiplakMukiyetChk_CheckedChanged(object sender, EventArgs e)
        {
            TabloOlustur();
        }
        protected void SagVefatChk_CheckedChanged(object sender, EventArgs e)
        {
            TabloOlustur();
        }
        protected void TaahhutChk_CheckedChanged(object sender, EventArgs e)
        {
            TabloOlustur();
        }
        protected void VasiyetciChk_CheckedChanged(object sender, EventArgs e)
        {
            TabloOlustur();
        }
        protected void DosyayaKaydetBtn_Click(object sender, EventArgs e)
        {

            const string fileName = "SorguListesi.json";
            using (var memoryStrm = new MemoryStream())
            {
                using (var writer = new StreamWriter(memoryStrm))
                {
                    memoryStrm.Position = 0;
                    writer.Flush();  //added flush
                    writer.WriteLine("[");
                    string settingsString = System.Text.Encoding.ASCII.GetString(memoryStrm.ToArray());
                    int toplamAdet = SorgulanacakKisilistesiQS.Count - 1;
                    int sayac = 0;
                    foreach (var item in SorgulanacakKisilistesiQS)
                    {
                        sayac++;
                        writer.WriteLine("{");
                        writer.WriteLine("Sira".ReturnDoubleQuotedValue().ToString() + ":" + sayac.ToString() + ",");
                        writer.WriteLine("Adi".ReturnDoubleQuotedValue().ToString() + ":" + item.Adi.ReturnDoubleQuotedValue().ToString() + ",");
                        writer.WriteLine("Soyadi".ReturnDoubleQuotedValue().ToString() + ":" + item.Soyadi.ReturnDoubleQuotedValue().ToString() + ",");
                        writer.WriteLine("KimlikNo".ReturnDoubleQuotedValue().ToString() + ":" + item.KimlikNo.ReturnDoubleQuotedValue().ToString() + ",");
                        writer.WriteLine("DogumGun".ReturnDoubleQuotedValue().ToString() + ":" + item.DogumTarihi.Day.ReturnDoubleQuotedValue().ToString() + ",");
                        writer.WriteLine("DogumAy".ReturnDoubleQuotedValue().ToString() + ":" + item.DogumTarihi.Month.ReturnDoubleQuotedValue().ToString() + ",");
                        writer.WriteLine("DogumYil".ReturnDoubleQuotedValue().ToString() + ":" + item.DogumTarihi.Year.ReturnDoubleQuotedValue().ToString());
                        writer.WriteLine("}" + (sayac <= toplamAdet ? "," : string.Empty));
                        writer.Flush();
                    }
                    writer.WriteLine("]");
                    writer.Flush();
                    settingsString = System.Text.Encoding.ASCII.GetString(memoryStrm.ToArray());
                    try
                    {
                        using (SPSite spSite = new SPSite(SPContext.Current.Web.Url))
                        {
                            //Get the document library object
                            SPList docLib = SPContext.Current.Web.Lists[ProjeConstants.TBYSBELGELERI_LIB];
                            SPFile file = docLib.RootFolder.Files.Add(fileName, memoryStrm, true);
                            settingsString = System.Text.Encoding.ASCII.GetString(memoryStrm.ToArray());
                            file.Update();
                        }

                    }
                    catch (Exception ex)
                    {

                        ExceptionHelper exh = new ExceptionHelper(ex);
                        exh.PublishException();
                    }
                }
            }
            string dosyaUrl = SPContext.Current.Web.Url + @"/" + ProjeConstants.TBYSBELGELERI_LIB + @"/" + fileName;


            string rootUrl = UtilityHelper.RootURLGetir();

            DosyaLnk.Text = fileName;
            DosyaLnk.NavigateUrl = rootUrl + "/_layouts/15/download.aspx?SourceUrl=" + dosyaUrl;
            DosyaLnk.Visible = true;
            TabloOlustur();
        }
        [Serializable]
        private class SorgulanacakKisi : IEquatable<SorgulanacakKisi>
        {
            private string adi;
            private string soyadi;
            private long kimlikNo;
            private DateTime dogumTarihi;            
            private string sagVefat;
            private int sagVefatKod;
            private string sorgulanan;
            private DateTime vefatTarihi;

            public long KimlikNo { get => kimlikNo; set => kimlikNo = value; }
            public string Adi{ get => adi; set => adi= value; }
            public string Soyadi { get => soyadi; set => soyadi = value; }
            public DateTime DogumTarihi { get => dogumTarihi; set => dogumTarihi = value; }
            
            public string SagVefat { get => sagVefat; set => sagVefat = value; }
            public DateTime VefatTarihi { get => vefatTarihi; set => vefatTarihi = value; }
            public int SagVefatKod { get => sagVefatKod; set => sagVefatKod = value; }
            public string Sorgulanan { get => sorgulanan; set => sorgulanan =  value; }
   
            public string Cikar { get; set; }

            public bool Equals(SorgulanacakKisi other)
            {
                if (other is null)
                    return false;
                bool result = this.KimlikNo != 0 && this.KimlikNo == other.KimlikNo;// && this.Soyadi == other.Soyadi;
                return (result);
            }
            public override bool Equals(object obj) => Equals(obj as SorgulanacakKisi);
            public override int GetHashCode() => (KimlikNo).GetHashCode();
        }

        protected void CikarNowBtn_Click(object sender, EventArgs e)
        {
            try
            {
                long kimlikNo= paramTcKimlikNo.Value.ConvertToLong();
                var stuffToRemove = SorgulanacakKisilistesiQS.SingleOrDefault(s => s.KimlikNo == kimlikNo);
                if (stuffToRemove.KimlikNo != 0)
                {
                    SorgulanacakKisilistesiQS.Remove(stuffToRemove);
                }
                var serializer = new JavaScriptSerializer();
                string jSon = serializer.Serialize(SorgulanacakKisilistesiQS);
                var jsString = CreateDataTable(jSon); //javascript kodu hazırlanıyor.
                UtilityHelper.ScriptCalistir(jsString);
            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper();
                exceptionHelper.Exceptions.Add(exception);
                exceptionHelper.PublishException();
            }
        }
    }
}