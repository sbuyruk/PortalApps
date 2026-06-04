using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;


namespace TBYS_WebParts.KiraciGirisiWP
{
    [ToolboxItemAttribute(false)]
    public partial class KiraciGirisiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public KiraciGirisiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string KiraciIdQS
        {
            get
            {

                if (ViewState["KiraciId"] == null)
                {
                    if (Page.Request.QueryString["KiraciId"] != null)
                    {
                        ViewState["KiraciId"] = Page.Request.QueryString["KiraciId"];
                    }
                    else
                    {
                        ViewState["KiraciId"] = string.Empty;
                    }
                }
                return ViewState["KiraciId"].ToString();
            }

            set
            {
                ViewState["KiraciId"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
           
            if (KiraciIdQS.ConvertToInt() > 0)
            {
                Kiraci kiraci = new Kiraci();
                kiraci = kiraci.Select<Kiraci>(KiraciIdQS.ConvertToInt());
                if (!Page.IsPostBack)
                {
                    IlDDLDoldur();
                    KiralamaAmaciDDLDoldur();
                    if (kiraci != null)
                    {
                        //KiraciDuzenle açilacak
                        OpenKiraciDuzenle(kiraci);
                        EnableOdemeSozlesmeBtns(kiraci);

                    }
                    else
                    {
                        MessageHelper.PublishMessage("Kiraci bulunamadi!", ProjeConstants.MESAJ_HATA);
                    }
                }
                SozlesmelerTablosunuDoldur(kiraci.Id);
            }
            else
            {
                if (!Page.IsPostBack)
                {
                    IlDDLDoldur();
                    KiralamaAmaciDDLDoldur();
                    OpenKiraciGirisi();
                }
            }
        }
        private void OpenKiraciDuzenle(Kiraci kiraci)
        {
            SaveBtn.Visible = false;
            UpdateBtn.Visible = true;
            SilBtn.Visible = true;
            TitleLbl.Text = "Kiraci Güncelleme";
            IdLbl.Visible = true;

            FillKiraci2Form(kiraci);
            PrevBtn.Visible = true;
            NextBtn.Visible = true;
        }
        private void OpenKiraciGirisi()
        {
            TitleLbl.Text = "Kiraci Girisi";
            SaveBtn.Visible = true;
            UpdateBtn.Visible = false;
            PrevBtn.Visible = false;
            NextBtn.Visible = false;
        }
        private bool FillKiraci2Form(Kiraci kiraci)
        {
            bool dataFilledToModal = true;
            try
            {
                IdLbl.Text = " (Kiraci No: " + kiraci.Id.ToString() + ")";
                AdiTxt.Text = kiraci.Adi;
                SoyadiTxt.Text = kiraci.Soyadi;
                TCKimlikNoTxt.Text = kiraci.TCKimlikNo;
                SemtTxt.Text = kiraci.Semt;
                ListItem ilItem = IliDDL.Items.FindByValue(IliDDL.Items.FindByValue(kiraci.IlId.ToString()).Value);
                if (ilItem != null)
                    IliDDL.SelectedValue = ilItem.Value;
                IlceDDLDoldur();
                FillBolgeTxt();
                if (KiralamaAmaciDDL.Items.FindByText(kiraci.KiralamaAmaci) != null)
                    KiralamaAmaciDDL.SelectedValue = KiralamaAmaciDDL.Items.FindByText(kiraci.KiralamaAmaci).Value;
                if (IlcesiDDL.Items.FindByText(kiraci.Ilcesi) != null)
                    IlcesiDDL.SelectedValue = IlcesiDDL.Items.FindByText(kiraci.Ilcesi).Value;
                AdresTxt.Text = kiraci.Adres;
                AciklamaTxt.Text = kiraci.Aciklama;
                VergiDairesiTxt.Text = kiraci.VergiDairesi;
                VergiNoTxt.Text = kiraci.VergiNo;
                TelefonTxt.Text = kiraci.Telefon;
                EpostaTxt.Text = kiraci.Eposta;
            }
            catch (Exception)
            {
                MessageHelper.PublishMessage("Kiraci ekrana getirilemedi", ProjeConstants.MESAJ_HATA);
                dataFilledToModal = false;
            }
            return dataFilledToModal;
        }
        private void EnableOdemeSozlesmeBtns(Kiraci kiraci)
        {
            if (kiraci != null)//bu kiraci varsa
            {
                BitenSozlesmeOlusturBtn.Visible = true;
                KiraSozlesme kiraSozlesme = new KiraSozlesme();
                kiraSozlesme = kiraSozlesme.SelectAktifSozlesmeByKiraciId(kiraci.Id);
                if (kiraSozlesme != null) //bu sozlesme varsa
                {
                    KiraKartiBtn.Visible = true;
                    OdemePlaniGoruntuleBtn.Visible = false;
                    OdemeYapBtn.Visible = false;
                    OdemePlani odemePlani = new OdemePlani();
                    bool odemePlaniVarMi = odemePlani.OdemePlaniVarMi(kiraSozlesme.Id);
                    if (odemePlaniVarMi)
                    {
                        OdemePlaniGoruntuleBtn.Visible = true;
                        OdemeYapBtn.Visible = true;
                    }
                }
                else
                {
                    YeniSozlesmeOlusturBtn.Visible = true;
                    KiraKartiBtn.Visible = false;
                }
            }
        }
        private Kiraci SaveKiraciData2Db()
        {
            Kiraci kiraci = new Kiraci();
            kiraci.Adi = AdiTxt.Text;
            kiraci.Soyadi = SoyadiTxt.Text;
            kiraci.TCKimlikNo = TCKimlikNoTxt.Text;
            kiraci.Semt = SemtTxt.Text;
            kiraci.Ilcesi = IlcesiDDL.SelectedItem.Text.ToString();
            kiraci.IlceId = IlcesiDDL.SelectedItem.Value.ConvertToInt();
           
            kiraci.Ili = IliDDL.SelectedItem.Text.ToString();
            kiraci.IlId = IliDDL.SelectedItem.Value.ConvertToInt();
            kiraci.Adres = AdresTxt.Text;
            kiraci.Aciklama = AciklamaTxt.Text;
            kiraci.VergiDairesi = VergiDairesiTxt.Text;
            kiraci.VergiNo = VergiNoTxt.Text;
            kiraci.Telefon = TelefonTxt.Text;
            kiraci.Eposta = EpostaTxt.Text;
            kiraci.KiralamaAmaci = KiralamaAmaciDDL.SelectedItem.ToString();
            int id = kiraci.Save();
            kiraci.Id = id;
            if (id > 0)
                return kiraci;
            else
                return null;
        }
        private Kiraci UpdateKiraciData2Db()
        {
            bool isUpdated = false;
            Kiraci kiraci = new Kiraci();
            kiraci = kiraci.Select<Kiraci>(KiraciIdQS.ConvertToInt());
            kiraci.Adi = AdiTxt.Text;
            kiraci.Soyadi = SoyadiTxt.Text;
            kiraci.TCKimlikNo = TCKimlikNoTxt.Text;
            kiraci.Semt = SemtTxt.Text;
            kiraci.Ilcesi = IlcesiDDL.SelectedItem.Text.ToString();
            kiraci.IlceId = IlcesiDDL.SelectedItem.Value.ConvertToInt();

            kiraci.Ili = IliDDL.SelectedItem.Text.ToString();
            kiraci.IlId = IliDDL.SelectedItem.Value.ConvertToInt();
            kiraci.Adres = AdresTxt.Text;
            kiraci.Aciklama = AciklamaTxt.Text;
            kiraci.VergiDairesi = VergiDairesiTxt.Text;
            kiraci.VergiNo = VergiNoTxt.Text;
            kiraci.Telefon = TelefonTxt.Text;
            kiraci.Eposta = EpostaTxt.Text;
            kiraci.KiralamaAmaci = KiralamaAmaciDDL.SelectedItem.ToString();
            isUpdated = kiraci.Update();
            return kiraci;
        }
        private void IlDDLDoldur()
        {
            IliDDL.Items.Clear();

            Il newil = new Il();
            List<Il> list = newil.SelectAll<Il>();
            foreach (Il il in list)
            {
                if (string.IsNullOrEmpty(il.IlAdi.Trim()))
                    continue;
                IliDDL.Items.Add(new ListItem(il.IlAdi, il.Id.ToString()));
            }
            IlceDDLDoldur();
            FillBolgeTxt();
        }
        private void IlceDDLDoldur()
        {
            IlcesiDDL.Items.Clear();
            Ilce pilce = new Ilce();

            List<Ilce> list = pilce.SelectByIlId(IliDDL.SelectedValue.ConvertToInt());
            foreach (Ilce ilce in list)
            {
                IlcesiDDL.Items.Add(new ListItem(ilce.IlceAdi, ilce.Id.ToString()));
            }
        }
       
      
        private void KiralamaAmaciDDLDoldur()
        {
            KiralamaAmaciDDL.Items.Clear();
            KiralamaAmaciDDL.Items.Add("Mesken");
            KiralamaAmaciDDL.Items.Add("Isyeri");
            KiralamaAmaciDDL.Items.Add("Arsa");
            KiralamaAmaciDDL.Items.Add("Tarla");
            KiralamaAmaciDDL.Items.Add("Bis");
            KiralamaAmaciDDL.Items.Add("Tesis");
        }
        private void FillBolgeTxt()
        {

            Bolge bolge = BolgeGetir();
            SorumluBolgeTxt.Text = bolge != null ? bolge.KisaAdi : "";
            SorumluBolgeIdTxt.Text = bolge != null ? bolge.Id.ToString() : "";
        }
        private void SozlesmelerTablosunuDoldur(int kiraciId)
        {
            TabloOlustur();
        }
        private void TabloOlustur()
        {
            var jsonData = TabloJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazirlaniyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string CreateDataTable(string jsonData)
        {
            string tableString = @"jQuery('#CustomDataTable').DataTable({
            data: " + jsonData + @",
                columns: [
                    { data: 'DosyaNo', 'width': '10%' },
                    { data: 'TarihAraligi'},
                    { data: 'OdemeSekli' },
                    { data: 'KiraBedeli' },              
                    { data: 'Adres' },
                    { data: 'Sozlesme' },
                    { data: 'OdemePlani' },
                    { data: 'SozlesmeDurumu' },
                ],
                'order': [[0, 'asc']],
                'scrollY': '270px',
                'scrollCollapse': true,
                'paging': false,
                'language': {
                    'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                    'decimal': ',',
                    'thousands': '.'
                },
                responsive: true,
                destroy: true,
                dom: 'srt',
                'createdRow': function(row, data, dataIndex) {
                    if (!data.Aktif)
                    {
                        $(row).addClass('eski-sozlesme');

                    }
                    if (!data.SozlesmeBasladi)
                    {
                        $(row).addClass('ileri-tarihli-sozlesme');

                    }
                },//set row color
            });
            ";

            return tableString;
        }
        private string TabloJson()
        {
            string jSon = string.Empty;

            try
            {
                List<KiraSozlesmeListItem> list = GetDataList();
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
        private List<KiraSozlesmeListItem> GetDataList()
        {

            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            List<KiraSozlesmeListItem> list = new List<KiraSozlesmeListItem>();
            DataTable dataTable = kiraSozlesme.SelectKiraSozlesmeListReturnDT(KiraciIdQS.ConvertToInt(), ProjeConstants.KIRASOZLESME_AKTIF_HEPSI_INT, ProjeConstants.BOLGE_HEPSI_INT);
            if (dataTable != null)
            {
                int SiraNo = 1;
                KiraSozlesmeListItem tempSozlesmeItem = null;

                IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
                DataView dataView = new DataView(dataTable);
                dataView.Sort = "SozBitTar DESC,SozBasTar DESC,DosyaNo";
                int tempSozlesmeId = 0;
                int tasinmazAdedi = 0;
                string ilkAdres = string.Empty;
                foreach (DataRowView row in dataView)
                {
                    string ili = string.Empty;
                    string ilcesi = string.Empty;
                    string adres = string.Empty;

                    string dosyaNo = row["DosyaNo"].ToString();
                    bool aktif = row["Aktif"].ConvertToBool();
                    string kiraciId = row["KiraciId"].ToString();
                    string kiraciAdi = row["KiraciAdi"].ToString();
                    string kiraciSoyadi = row["KiraciSoyadi"].ToString();
                    string ilkSozlesmeTar = row["IlkSozlesmeTar"].ToString();
                    string sozBasTar = row["SozBasTar"].ToString();
                    string sozBitTar = row["SozBitTar"].ToString();
                    string odemeSekli = row["odemeSekli"].ToString();
                    string artisAyi = row["ArtisAyi"].ToString();
                    decimal kiraBedeli = row["KiraBedeli"].ConvertToDecimal();
                    string sozlesmeDurumu = row["SozlesmeDurumu"].ToString();
                    int sozlesmeBasladi = row["SozlesmeBasladi"].ReturnZeroIfNull().ConvertToInt();
                    int kiraSozlesmeId = row["KiraSozlesmeId"].ConvertToInt();
                    string BolumNo = row["BolumNo"].ToString();
                    adres = row["Adres"].ToString() + " " + BolumNo;
                    ili = row["Ili"].ToString();
                    ilcesi = row["Ilcesi"].ToString();
                    if (tasinmazAdedi == 0)
                    {
                        ilkAdres = adres;
                    }
                    if (tempSozlesmeId == kiraSozlesmeId)
                    {
                        tempSozlesmeId = kiraSozlesmeId;

                        list.Remove(tempSozlesmeItem);


                        //tempSozlesmeItem.Adres += "@" + adres;
                        tasinmazAdedi++;
                        tempSozlesmeItem.Adres = "@" + ilkAdres + "( Toplam " + tasinmazAdedi + " adet tasinmaz.)";
                        list.Add(tempSozlesmeItem);
                    }
                    else
                    {
                        KiraSozlesmeListItem sozlesmeItem = new KiraSozlesmeListItem();
                        sozlesmeItem.Sirano = SiraNo++.ToString();
                        sozlesmeItem.DosyaNo = dosyaNo;
                        sozlesmeItem.Aktif = aktif;
                        sozlesmeItem.SozlesmeId = kiraSozlesmeId.ToString();
                        sozlesmeItem.KiraciId = kiraciId;
                        sozlesmeItem.KiraciAdi = kiraciAdi + " " + kiraciSoyadi;
                        sozlesmeItem.SozlesmeTarihi = ilkSozlesmeTar;
                        sozlesmeItem.SozlesmeBasTar = sozBasTar.ConvertToDatetimeEmptyIfNull();
                        sozlesmeItem.SozlesmeBitTar = sozBitTar.ConvertToDatetimeEmptyIfNull();
                        sozlesmeItem.TarihAraligi = sozBasTar.ConvertToDatetimeEmptyIfNull() + "-" + sozBitTar.ConvertToDatetimeEmptyIfNull();
                        sozlesmeItem.ArtisAyi = artisAyi;
                        sozlesmeItem.OdemeSekli = odemeSekli;
                        sozlesmeItem.KiraBedeli = kiraBedeli.ToString("N", culturInfo);
                        sozlesmeItem.SozlesmeDurumu = sozlesmeDurumu;
                        sozlesmeItem.SozlesmeBasladi = sozlesmeBasladi.ToString();
                        sozlesmeItem.Adres = "- " + adres;
                        sozlesmeItem.Ilcesi = ilcesi;
                        sozlesmeItem.Ili = ili;
                        sozlesmeItem.Sozlesme = "<a href=" + ProjeConstants.PAGE_KIRASOZLESMESI + "?KiraSozlesmeId=" + kiraSozlesmeId + " class='btn btn-outline-secondary'>Sözlesme</a>";
                        sozlesmeItem.OdemePlani = "<a href=" + ProjeConstants.PAGE_ODEMEPLANI + "?KiraSozlesmeId=" + kiraSozlesmeId + " class='btn btn-outline-secondary'>Ödeme Plani</a>";
                        list.Add(sozlesmeItem);
                        tempSozlesmeItem = sozlesmeItem;
                        tasinmazAdedi = 1;
                    }
                    tempSozlesmeId = kiraSozlesmeId;

                }
            }

            return list;
        }
        protected void IliDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            IlceDDLDoldur();
            FillBolgeTxt();
        }
        protected void SaveBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Kiraci kiraci = SaveKiraciData2Db();
                if (kiraci != null) 
                {
                    RedirectToPage(ProjeConstants.PAGE_KIRACI_GIRIS + "?KiraciId=" + kiraci.Id);
                }
                else
                {
                    MessageHelper.PublishMessage("Kiraci Kaydedilemedi", ProjeConstants.MESAJ_HATA);
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void UpdateBtn_Click(object sender, EventArgs e)
        {
            Kiraci kiraci = UpdateKiraciData2Db();
            if (kiraci != null)
            {
                KiraSozlesme ks = new KiraSozlesme();
                Bolge bolge = BolgeGetir();
                bool ksUpdateed = ks.UpdateByKiraciId(bolge.Id, kiraci.Id);
                SozlesmelerTablosunuDoldur(kiraci.Id);
                MessageHelper.PublishMessage("Kiraci Güncellendi", ProjeConstants.MESAJ_BASARILI, 2000);
            }
            else
            {
                MessageHelper.PublishMessage("Kiraci Güncellenemedi", ProjeConstants.MESAJ_HATA);
            }
        }
        private Bolge BolgeGetir()
        {
            int ilId = IliDDL.SelectedItem.Value.ConvertToInt();
            Il Il = new Il();
            Il = Il.Select<Il>(ilId);
            Bolge bolge = new Bolge();
            bolge = bolge.Select(Il.BolgeId);
            return bolge;
        }
        protected void SilBtn_Click(object sender, EventArgs e)
        {
            Kiraci kiraci = new Kiraci();
            kiraci = kiraci.Select<Kiraci>(KiraciIdQS.ConvertToInt());
            if (kiraci != null)//bu kiraci varsa
            {
                KiraSozlesme kiraSozlesme = new KiraSozlesme();
                kiraSozlesme = kiraSozlesme.SelectSozlesmeByKiraciId(kiraci.Id);
                if (kiraSozlesme != null) //bu sozlesme varsa
                {
                    WarnDiv.Attributes["style"] = "display : block";
                    DeleteDiv.Attributes["style"] = "display : none";
                }
                else
                {
                    DeleteDiv.Attributes["style"] = "display : block";
                    WarnDiv.Attributes["style"] = "display : none";
                }
                ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "DeleteModalOnay();", true);
            }

        }
        protected void OdemePlaniGoruntuleBtn_Click(object sender, EventArgs e)
        {
            Kiraci kiraci = new Kiraci();
            kiraci = kiraci.Select<Kiraci>(KiraciIdQS.ConvertToInt());
            if (kiraci != null)//bu kiraci varsa
            {
                KiraSozlesme kiraSozlesme = new KiraSozlesme();
                kiraSozlesme = kiraSozlesme.SelectAktifSozlesmeByKiraciId(kiraci.Id);
                if (kiraSozlesme != null) //bu sozlesme varsa
                {
                    OdemePlani odemePlani = new OdemePlani();
                    bool odemePlaniVarMi = odemePlani.OdemePlaniVarMi(kiraSozlesme.Id);
                    if (odemePlani != null)
                    {
                        OdemePlaniGoruntule(kiraSozlesme, kiraci);
                        SozlesmelerTablosunuDoldur(kiraci.Id);
                    }
                }
            }
        }
        private void OdemePlaniGoruntule(KiraSozlesme kiraSozlesme, Kiraci kiraci)
        {
            TitleLbl.Text = " Kiraci : " + kiraci.Adi + " " + kiraci.Soyadi;
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);

            OdemePlani odemePlaniDao = new OdemePlani();
            List<OdemePlani> list = odemePlaniDao.SelectBySozlesmeId(kiraSozlesme.Id);
            if (list.Count > 0)
            {
                DevirLbl.Text = "Devir Anapara : " + kiraSozlesme.DevirAnaPara.ToString("N", culturInfo) + "      "
                    + "Devir Faiz : " + kiraSozlesme.DevirFaizTutari.ToString("N", culturInfo)
                    + "Devir FaizliBakiye : " + kiraSozlesme.DevirFaizliBakiye.ToString("N", culturInfo);
            }
            foreach (OdemePlani odemePlani in list)
            {
                if (odemePlani.Sira == 0)
                {
                    continue;
                }
                TableRow row = new TableRow();
                TableCell SiraNoCell = new TableCell();
                SiraNoCell.Text = odemePlani.Sira.ToString(); ;
                row.Controls.Add(SiraNoCell);

                TableCell YilCell = new TableCell();
                YilCell.CssClass = "text-end";
                YilCell.Text = odemePlani.Yil.ToString();
                row.Controls.Add(YilCell);

                TableCell AyCell = new TableCell();
                AyCell.Text = odemePlani.Ay.ToString();
                row.Controls.Add(AyCell);

                TableCell KiraBedeliCell = new TableCell();
                KiraBedeliCell.CssClass = "text-end";
                KiraBedeliCell.Text = odemePlani.KiraBedeli.ToString("N", culturInfo);
                row.Controls.Add(KiraBedeliCell);

                TableCell OdenenTutarCell = new TableCell();
                OdenenTutarCell.CssClass = "text-end";
                OdenenTutarCell.Text = odemePlani.OdenenTutar.ToString("N", culturInfo);
                row.Controls.Add(OdenenTutarCell);

                OdemePlaniTable.Controls.Add(row);
            }
            var jsString = @"
                var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('OdemePlaniModal'));
                myModalInstance.show();
            ";
            UtilityHelper.ScriptCalistir(jsString);
        }
        protected void YeniSozlesmeOlusturBtn_Click(object sender, EventArgs e)
        {
            Kiraci kiraci = new Kiraci();
            kiraci = kiraci.Select<Kiraci>(KiraciIdQS.ConvertToInt());
            if (kiraci != null)//bu kiraci varsa
            {
                KiraSozlesme kiraSozlesme = new KiraSozlesme();
                kiraSozlesme = kiraSozlesme.SelectAktifSozlesmeByKiraciId(kiraci.Id);
                if (kiraSozlesme != null) //bu sozlesme varsa
                {
                    MessageHelper.PublishMessage("Zaten Bir kira sözlesmesi mevcut", ProjeConstants.MESAJ_HATA);
                }
                ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "YeniSozlesmeModalOnay();", true);
            }
        }
        protected void BitenSozlesmeOlusturBtn_Click(object sender, EventArgs e)
        {
            Kiraci kiraci = new Kiraci();
            kiraci = kiraci.Select<Kiraci>(KiraciIdQS.ConvertToInt());
            if (kiraci != null)//bu kiraci varsa
            {
                ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "BitenSozlesmeModalOnay();", true);
            }
        }
        protected void YeniSozlesmeOnayBtn_Click(object sender, EventArgs e)
        {
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            kiraSozlesme.KiraciId = KiraciIdQS.ConvertToInt();
            kiraSozlesme.Aktif = true;
            kiraSozlesme.SozlesmeDurumu = ProjeConstants.KIRASOZLESME_DURUMU_DEVAM;
            kiraSozlesme.Id = kiraSozlesme.Save();
            if (kiraSozlesme.Id > 0)
            {

                RedirectToPage(ProjeConstants.PAGE_KIRASOZLESME_TASINMAZ + "?SenderApp=KD&KiraSozlesmeId=" + kiraSozlesme.Id);
            }
            else
            {
                MessageHelper.PublishMessage("Sözlesme Kaydi Açilamadi", ProjeConstants.MESAJ_HATA);
            }
        }
        protected void BitenSozlesmeOnayBtn_Click(object sender, EventArgs e)
        {
            KiraSozlesme bitenKiraSozlesmeDao = new KiraSozlesme();
            bitenKiraSozlesmeDao = bitenKiraSozlesmeDao.SelectBitenSozlesmeByKiraciId(KiraciIdQS.ConvertToInt());
            KiraSozlesme aktifKiraSozlesme = new KiraSozlesme();
            aktifKiraSozlesme = aktifKiraSozlesme.SelectAktifSozlesmeByKiraciId(KiraciIdQS.ConvertToInt());

            KiraSozlesme masterKs = new KiraSozlesme();
            masterKs = aktifKiraSozlesme != null ? aktifKiraSozlesme : bitenKiraSozlesmeDao;// != null ? bitenKiraSozlesmeDao : new KiraSozlesme();


            KiraSozlesme yeniKayitbitenKiraSozlesme = new KiraSozlesme();

            if (masterKs != null)
            {
                yeniKayitbitenKiraSozlesme.ArtisAyi = masterKs.ArtisAyi;
                yeniKayitbitenKiraSozlesme.DosyaNo = masterKs.DosyaNo;
                yeniKayitbitenKiraSozlesme.IlkSozlesmeTar = masterKs.IlkSozlesmeTar;
                yeniKayitbitenKiraSozlesme.KefilAdiSoyadi = masterKs.KefilAdiSoyadi;
                yeniKayitbitenKiraSozlesme.KefilAdresi = masterKs.KefilAdresi;
                yeniKayitbitenKiraSozlesme.KefilTCKimlikNo = masterKs.KefilTCKimlikNo;
                yeniKayitbitenKiraSozlesme.KefilTel = masterKs.KefilTel;
                yeniKayitbitenKiraSozlesme.Aciklama = masterKs.Aciklama;
                yeniKayitbitenKiraSozlesme.SozlesmePDFDosyasi = masterKs.SozlesmePDFDosyasi;

                yeniKayitbitenKiraSozlesme.OdemeSekli = masterKs.OdemeSekli;
                yeniKayitbitenKiraSozlesme.TaksitSayisi = masterKs.TaksitSayisi;
                yeniKayitbitenKiraSozlesme.TeminatAciklama = masterKs.TeminatAciklama;
                yeniKayitbitenKiraSozlesme.TeminatCinsi = masterKs.TeminatCinsi;
                yeniKayitbitenKiraSozlesme.IadeTeminatTutari = masterKs.IadeTeminatTutari;
                yeniKayitbitenKiraSozlesme.TeminatOdemeTarihi = masterKs.TeminatOdemeTarihi;
                yeniKayitbitenKiraSozlesme.TeminatTutari = masterKs.TeminatTutari;
                yeniKayitbitenKiraSozlesme.OdenenTeminatTutari = masterKs.OdenenTeminatTutari;
                yeniKayitbitenKiraSozlesme.KalanTeminatTutari = masterKs.KalanTeminatTutari;

                yeniKayitbitenKiraSozlesme.SozBasTar = masterKs.SozBasTar.AddYears(-1);
                yeniKayitbitenKiraSozlesme.SozBitTar = masterKs.SozBitTar.AddYears(-1);
            }
            yeniKayitbitenKiraSozlesme.KiraciId = KiraciIdQS.ConvertToInt();
            yeniKayitbitenKiraSozlesme.KiraBedeli = 0;
            yeniKayitbitenKiraSozlesme.Olusturan = UtilityHelper.GetCurrentUserLoginName();

            yeniKayitbitenKiraSozlesme.DevirAnaPara = 0;
            yeniKayitbitenKiraSozlesme.DevirFaizliBakiye = 0;
            yeniKayitbitenKiraSozlesme.DevirFaizTutari = 0;
            yeniKayitbitenKiraSozlesme.DurumDegismeTar = yeniKayitbitenKiraSozlesme.SozBitTar.ConvertToDatetimeEmptyIfNull().Equals("") ? DateTime.Today : yeniKayitbitenKiraSozlesme.SozBitTar;
            yeniKayitbitenKiraSozlesme.SozlesmeDurumu = ProjeConstants.KIRASOZLESME_DURUMU_YENILENDI;

            yeniKayitbitenKiraSozlesme.Olusturan = UtilityHelper.GetCurrentUserLoginName();
            yeniKayitbitenKiraSozlesme.Aktif = false;
            yeniKayitbitenKiraSozlesme.Save();
            if (yeniKayitbitenKiraSozlesme.Id > 0)
            {

                int birSozlesmeId = aktifKiraSozlesme != null ? aktifKiraSozlesme.Id : bitenKiraSozlesmeDao != null ? bitenKiraSozlesmeDao.Id : 0;

                if (birSozlesmeId > 0)
                {
                    SozlesmeTasinmaz st = new SozlesmeTasinmaz();
                    List<SozlesmeTasinmaz> stList = st.SelectBySozlesmeId(birSozlesmeId);
                    foreach (SozlesmeTasinmaz item in stList)
                    {
                        SozlesmeTasinmaz ekST = new SozlesmeTasinmaz();
                        ekST.SozlesmeId = yeniKayitbitenKiraSozlesme.Id;
                        ekST.BolumId = item.BolumId;
                        ekST.TasinmazId = item.TasinmazId;
                        ekST.Olusturan = UtilityHelper.GetCurrentUserLoginName();
                        ekST.Save();
                    }

                    RedirectToPage(ProjeConstants.PAGE_BITENKIRASOZLESMESI + "?DestinationApp=KS&SenderApp=KD&KiraSozlesmeId=" + yeniKayitbitenKiraSozlesme.Id);
                }
                else
                {
                    RedirectToPage(ProjeConstants.PAGE_KIRASOZLESME_TASINMAZ + "?SenderApp=KD&KiraSozlesmeId=" + yeniKayitbitenKiraSozlesme.Id );
                }

            }
            else
            {
                MessageHelper.PublishMessage("Sözlesme Kaydi Açilamadi", ProjeConstants.MESAJ_HATA);
            }
        }
        protected void KiraKartiBtn_Click(object sender, EventArgs e)
        {
            Kiraci kiraci = new Kiraci();
            kiraci = kiraci.Select<Kiraci>(KiraciIdQS.ConvertToInt());
            if (kiraci != null)//bu kiraci varsa
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));
                newUrl += "/" + ProjeConstants.PAGE_KIRAKARTI + "?SenderApp=KD&KiraciId=" + kiraci.Id;
                Page.Response.Redirect(newUrl, true);

            }
        }
        protected void OdemeYapBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Kiraci kiraci = new Kiraci();
                kiraci = kiraci.Select<Kiraci>(KiraciIdQS.ConvertToInt());
                if (kiraci != null)
                {
                    KiraSozlesme kiraSozlesme = new KiraSozlesme();
                    kiraSozlesme = kiraSozlesme.SelectAktifSozlesmeByKiraciId(kiraci.Id);
                    if (kiraSozlesme != null) //bu sozlesme varsa
                    {
                        OdemePlani odemePlani = new OdemePlani();
                        string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                        string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_ODEMEPLANI;
                        newUrl = newUrl + "?SenderApp=KD&KiraciId=" + kiraci.Id + "&KiraSozlesmeId=" + kiraSozlesme.Id;
                        Page.Response.Redirect(newUrl, true);
                    }
                    else
                        MessageHelper.PublishMessage("Bu kiraciya ait sözlesme kaydi bulunamadi", ProjeConstants.MESAJ_HATA);
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }


        }
        protected void PrevBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Kiraci kiraci = new Kiraci();
                kiraci = kiraci.Select<Kiraci>(KiraciIdQS.ConvertToInt());
                Kiraci oncekiKiraci = kiraci.SelectNext();
                if (oncekiKiraci != null)
                {
                    RedirectToPage(ProjeConstants.PAGE_KIRACI_GIRIS + "?DestinationApp=KD&KiraciId=" + oncekiKiraci.Id);
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void NextBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Kiraci kiraci = new Kiraci();
                kiraci = kiraci.Select<Kiraci>(KiraciIdQS.ConvertToInt());
                Kiraci sonrakiKiraci = kiraci.SelectNext();
                if (sonrakiKiraci != null)
                {
                    RedirectToPage(ProjeConstants.PAGE_KIRACI_GIRIS + "?DestinationApp=KD&KiraciId=" + sonrakiKiraci.Id);
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }

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
        protected void BackBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_KIRACI_LIST;
                Page.Response.Redirect(newUrl);
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private void RedirectToPage(string pageUrl)
        {
            try
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + pageUrl;
                Page.Response.Redirect(newUrl);
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }

        protected void KiraciListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_KIRACI_LIST + "?SecilenId=" + KiraciIdQS);
        }
        protected void BakiyeDevirBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_KIRA_BAKIYEDEVRI + "?KiraciId=" + KiraciIdQS);
        }
        private class KiraSozlesmeListItem
        {
            public string Sirano { get; set; }
            public string DosyaNo { get; set; }
            public bool Aktif { get; set; }
            public string SozlesmeId { get; set; }
            public string KiraciId { get; set; }
            public string KiraciAdi { get; set; }
            public string SozlesmeTarihi { get; set; }
            public string SozlesmeBasTar { get; set; }
            public string SozlesmeBitTar { get; set; }
            public string TarihAraligi { get; set; }
            public string ArtisAyi { get; set; }
            public string OdemeSekli { get; set; }
            public string KiraBedeli { get; set; }
            public string SozlesmeDurumu { get; set; }
            public string SozlesmeBasladi { get; set; }
            public string Ili { get; set; }
            public string Ilcesi { get; set; }
            public string Adres { get; set; }
            public string Sozlesme { get; set; }
            public string OdemePlani { get; set; }
        }

        protected void DeleteNowBtn_Click(object sender, EventArgs e)
        {
            Kiraci kiraci = new Kiraci();
            kiraci = kiraci.Select<Kiraci>(KiraciIdQS.ConvertToInt());
            ///Kiracinin siinmesi bir dizi konrol ile yapilabilir. 
            if (KiraciSilinebilirMi(kiraci))
            {
                if (kiraci.Delete())
                {
                    RedirectToPage(ProjeConstants.PAGE_KIRACI_LIST + "?Mesaj=true");
                }
            } 

        }

        private bool KiraciSilinebilirMi(Kiraci kiraci)
        {
            bool silinebilirMi = false;
            try
            {
                if (kiraci==null)
                {
                    MessageHelper.PublishMessage("Kiraci bulunamadi.",ProjeConstants.MESAJ_HATA);
                }
                else
                {
                    //Sözlesmesi Var mi
                    if (SozlesmesiVarMi(kiraci))
                    {
                        MessageHelper.PublishMessage("Kiraciya ait sözlesme bulundugundan kiraci silinemez.", ProjeConstants.MESAJ_HATA);
                    }
                    else
                    {
                        if (OdemesiVarMi(kiraci))
                        {
                            MessageHelper.PublishMessage("Kiraci daha önce ödeme yaptigindan kiraci silinemez.", ProjeConstants.MESAJ_HATA);
                        }
                        else
                        {
                            silinebilirMi = true;
                        }
                    }
                }
                
            }
            catch (Exception)
            {

                throw;
            }
            return silinebilirMi;
        }

        private bool OdemesiVarMi(Kiraci kiraci)
        {
            bool odemesiVarMi = true;
            Odeme odeme = new Odeme();
            List<Odeme> list = odeme.SelectByKiraciId(kiraci.Id);
            if (list.Count<1)
            {
                odemesiVarMi = false;
            }
            return odemesiVarMi;
        }

        private bool SozlesmesiVarMi(Kiraci kiraci)
        {
            bool sozlesmesiVarMi = true;
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            kiraSozlesme = kiraSozlesme.SelectSozlesmeByKiraciId(kiraci.Id);
            if (kiraSozlesme == null)
            {
                sozlesmesiVarMi = false;
            }
            return sozlesmesiVarMi;

        }
    }
}
