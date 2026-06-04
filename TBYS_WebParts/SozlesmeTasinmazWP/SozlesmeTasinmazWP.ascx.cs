using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.SozlesmeTasinmazWP
{
    [ToolboxItemAttribute(false)]
    public partial class SozlesmeTasinmazWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public SozlesmeTasinmazWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string KiraSozlesmeIdQS
        {
            get
            {

                if (ViewState["KiraSozlesmeId"] == null)
                {
                    if (Page.Request.QueryString["KiraSozlesmeId"] != null)
                    {
                        ViewState["KiraSozlesmeId"] = Page.Request.QueryString["KiraSozlesmeId"];
                    }
                    else
                    {
                        ViewState["KiraSozlesmeId"] = string.Empty;
                    }
                }
                return ViewState["KiraSozlesmeId"].ToString();
            }

            set
            {
                ViewState["KiraSozlesmeId"] = value;
            }
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
        private string TasinmazIdQS
        {
            get
            {

                if (ViewState["TasinmazId"] == null)
                {
                    if (Page.Request.QueryString["TasinmazId"] != null)
                    {
                        ViewState["TasinmazId"] = Page.Request.QueryString["TasinmazId"];
                    }
                    else
                    {
                        ViewState["TasinmazId"] = string.Empty;
                    }
                }
                return ViewState["TasinmazId"].ToString();
            }

            set
            {
                ViewState["TasinmazId"] = value;
            }
        }
        private string BolumIdQS
        {
            get
            {

                if (ViewState["BolumId"] == null)
                {
                    if (Page.Request.QueryString["BolumId"] != null)
                    {
                        ViewState["BolumId"] = Page.Request.QueryString["BolumId"];
                    }
                    else
                    {
                        ViewState["BolumId"] = string.Empty;
                    }
                }
                return ViewState["BolumId"].ToString();
            }

            set
            {
                ViewState["BolumId"] = value;
            }
        }
        private string SenderAppQS
        {
            get
            {

                if (ViewState["SenderApp"] == null)
                {
                    if (Page.Request.QueryString["SenderApp"] != null)
                    {
                        ViewState["SenderApp"] = Page.Request.QueryString["SenderApp"];
                    }
                    else
                    {
                        ViewState["SenderApp"] = string.Empty;
                    }
                }
                return ViewState["SenderApp"].ToString();
            }

            set
            {
                ViewState["SenderApp"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {

            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            kiraSozlesme = kiraSozlesme.Select<KiraSozlesme>(KiraSozlesmeIdQS.ConvertToInt());
            if (kiraSozlesme != null)
            {
                SecileniKaydet(kiraSozlesme);
                SozlesmeTasinmazTablosunuDoldur(kiraSozlesme);
                SozlesmeTasinmaz st = new SozlesmeTasinmaz();
                List<SozlesmeTasinmaz> list = st.SelectBySozlesmeId(kiraSozlesme.Id);
                TamamBtn.Visible = list.Count > 0;

                SecilenKiraciyiKaydet(kiraSozlesme);
                Kiraci kiraci = new Kiraci();
                kiraci = kiraci.Select<Kiraci>(kiraSozlesme.KiraciId);
                AdiLbl.Text = kiraci.Adi + " " + kiraci.Soyadi + " (" + kiraci.Id + ")";
                KiraciAdiSoyadiLbl.Text = kiraci.Adi + " " + kiraci.Soyadi + " (" + kiraci.Adres + ")";
            }
        }
        private void SozlesmeTasinmazTablosunuDoldur(KiraSozlesme kiraSozlesme)
        {
            //önce tabloyu temizle
            PopUpTable.Rows.Clear();

            SozlesmeTasinmaz st = new SozlesmeTasinmaz();
            List<SozlesmeTasinmaz> list = st.SelectBySozlesmeId(kiraSozlesme.Id);
            SiraNoCell.Text = "Sira";
            AdresCell.Text = "Adres";
            int siraNo = 1;
            foreach (SozlesmeTasinmaz sozlesmeTasinmaz in list)
            {
                TableRow row = new TableRow();

                TableCell PupUpCell0 = new TableCell();

                PupUpCell0.Text = siraNo++ + "";
                row.Controls.Add(PupUpCell0);

                //tabloya kira sözlesme Id ekle
                TableCell SozlesmeIdCell = new TableCell();
                SozlesmeIdCell.Text = kiraSozlesme.Id.ToString();
                row.Controls.Add(SozlesmeIdCell);
                SozlesmeIdCell.Visible = false;

                //tabloya tasinmaz Id ekle
                TableCell TasinmazIdCell = new TableCell();
                TasinmazIdCell.Text = sozlesmeTasinmaz.TasinmazId.ToString();
                row.Controls.Add(TasinmazIdCell);
                TasinmazIdCell.Visible = false;

                //tabloya bagimsiz bolum Id ekle
                TableCell BagimsizBolumIdCell = new TableCell();
                BagimsizBolumIdCell.Text = sozlesmeTasinmaz.BolumId.ToString();
                row.Controls.Add(BagimsizBolumIdCell);
                BagimsizBolumIdCell.Visible = false;

                Tasinmaz tasinmaz = new Tasinmaz();
                tasinmaz = tasinmaz.SelectById(sozlesmeTasinmaz.TasinmazId);
                string adres = string.Empty;
                if (tasinmaz != null)
                {
                    adres = tasinmaz.Adres;
                    BagimsizBolum bb = new BagimsizBolum();
                    bb = bb.Select<BagimsizBolum>(sozlesmeTasinmaz.BolumId);
                    if (bb != null)
                    {
                        adres += " " + bb.BolumNo;
                    }
                    adres += " " + tasinmaz.Ilcesi + " " + tasinmaz.Ili;
                }

                TableCell AdresCell = new TableCell();
                AdresCell.Text = adres;
                row.Controls.Add(AdresCell);

                //TableCell kiralamaAmaciCell = new TableCell();
                //DropDownList kiralamaAmaciDDL = new DropDownList();

                //kiralamaAmaciDDL.ID = "KiralamaAmaciDDL" + siraNo;
                //kiralamaAmaciDDL.CssClass = "form-control";
                //kiralamaAmaciDDL.AutoPostBack = true;
                ////if (!Page.IsPostBack)
                //    KiralamaAmaciDDLDoldur(kiralamaAmaciDDL, sozlesmeTasinmaz.KiralamaAmaci);

                //kiralamaAmaciDDL.SelectedIndexChanged += delegate
                //{
                //    //int tasinmazId = sozlesmeTasinmaz.TasinmazId; //TODO commentledim, bu dogru mu
                //    sozlesmeTasinmaz.KiralamaAmaci=kiralamaAmaciDDL.SelectedItem.Value;
                //    sozlesmeTasinmaz.Update();
                //    string newUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                //    int queryIndex = newUrl.IndexOf("?");
                //    if (queryIndex > 0)
                //        newUrl = newUrl.Substring(0, queryIndex);
                //    Page.Response.Redirect(newUrl + "?KiraSozlesmeId=" + KiraSozlesmeIdQS, true);
                //};
                //kiralamaAmaciCell.Controls.Add(kiralamaAmaciDDL);
                //row.Controls.Add(kiralamaAmaciCell);

                TableCell cikarCell = new TableCell();
                LinkButton cikarBtn = new LinkButton();
                cikarBtn.Text = "Çikar";
                cikarBtn.CssClass = "btn btn-xs btn-danger";
                cikarBtn.Click += delegate
                {
                    //int tasinmazId = sozlesmeTasinmaz.TasinmazId; //TODO commentledim, bu dogru mu
                    sozlesmeTasinmaz.Delete();
                    string newUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                    int queryIndex = newUrl.IndexOf("?");
                    if (queryIndex > 0)
                        newUrl = newUrl.Substring(0, queryIndex);
                    Page.Response.Redirect(newUrl + "?KiraSozlesmeId=" + KiraSozlesmeIdQS, true);
                };
                cikarCell.Controls.Add(cikarBtn);
                row.Controls.Add(cikarCell);

                PopUpTable.Controls.Add(row);
            }
        }
        private void SecileniKaydet(KiraSozlesme kiraSozlesme)
        {
            if (kiraSozlesme != null)
            {
                IdLbl.Text = kiraSozlesme.Id.ToString();
                int tasinmazId = TasinmazIdQS.ConvertToInt();
                if (tasinmazId > 0)
                {
                    Tasinmaz tasinmaz = new Tasinmaz();
                    tasinmaz = tasinmaz.SelectById(tasinmazId);
                    if (tasinmaz != null)
                    {

                        BagimsizBolum babo = new BagimsizBolum();
                        babo = babo.Select<BagimsizBolum>(BolumIdQS.ConvertToInt());
                        //BagimsizBolum bolüm varsa
                        if (babo != null)
                        {
                            SozlesmeTasinmaz st = new SozlesmeTasinmaz();
                            List<SozlesmeTasinmaz> list = st.SelectBySozlesmeIdTasinmazId(kiraSozlesme.Id, tasinmaz.Id, babo.Id);
                            if (list.Count < 1)// bu bagimsizbolum  vt'da yoksa insert et varsa bisey yapma varsa
                            {
                                st.SozlesmeId = kiraSozlesme.Id;
                                st.TasinmazId = tasinmaz.Id;
                                st.BolumId = babo.Id;
                                int tId = st.Save();
                            }
                        }

                        else //bagimsiz bolum parametre olarak gelmediyse 
                        {
                            SozlesmeTasinmaz st = new SozlesmeTasinmaz();
                            List<SozlesmeTasinmaz> list = st.SelectBySozlesmeIdTasinmazId(kiraSozlesme.Id, tasinmaz.Id, 0);
                            if (list.Count < 1)
                            {
                                st.SozlesmeId = kiraSozlesme.Id;
                                st.TasinmazId = tasinmaz.Id;
                                st.BolumId = 0;
                                st.Save();
                            }//if list.count
                        }// else bagimsiz bolum parametre olarak gelmediyse 

                    }//if tasinmaz != null
                }//if !string.IsNullOrEmpty(tasinmazId)
            }
        }
        private void SecilenKiraciyiKaydet(KiraSozlesme kiraSozlesme)
        {
            if ((kiraSozlesme != null) && !string.IsNullOrEmpty(KiraciIdQS))
            {
                IdLbl.Text = kiraSozlesme.Id.ToString();
                int kiraciId = KiraciIdQS.ConvertToInt();
                if (kiraciId > 0)
                {
                    Kiraci kiraci = new Kiraci();
                    kiraci = kiraci.Select<Kiraci>(kiraciId);
                    if (kiraci != null)
                    {
                        kiraSozlesme.KiraciId = kiraci.Id;
                        kiraSozlesme.Update();
                    }
                }
            }
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        protected void TamamBtn_Click(object sender, EventArgs e)
        {
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            kiraSozlesme = kiraSozlesme.Select(KiraSozlesmeIdQS.ConvertToInt());
            string sozlesmePage = ProjeConstants.PAGE_KIRASOZLESMESI;
            if (kiraSozlesme != null)
            {
                if (kiraSozlesme.Aktif)
                {
                    sozlesmePage = ProjeConstants.PAGE_KIRASOZLESMESI;
                }
                else
                {
                    sozlesmePage = ProjeConstants.PAGE_BITENKIRASOZLESMESI;
                }
            }

            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" +
                sozlesmePage + "?SenderApp=" + SenderAppQS + "&KiraSozlesmeId=" + KiraSozlesmeIdQS;
            Page.Response.Redirect(newUrl);

        }
        protected void TasinmazEkleBtn_Click(object sender, EventArgs e)
        {
            TasinmazSecimiModalShow();
        }
        protected void KiraciSecBtn_Click(object sender, EventArgs e)
        {
            KiraciSecimiModalShow();
            ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "KiraciSecimiModal();", true);
        }
        protected void EnvanterdeOlmayanTasinmazEkleBtn_Click(object sender, EventArgs e)
        {
            EnvanterdeOlmayanTasinmazSecimiModalShow();
        }

        #region  Modal Tasinmaz Kiraci Env olmayan Tasinmazlar
        private void TasinmazSecimiModalShow()
        {
            Tasinmaz tasinmaz = new Tasinmaz();
            var jsonData = tasinmaz.SelectTasinmazBolumNoReturnJson(ProjeConstants.TASINMAZ_ENVANTERDE, ProjeConstants.KIRADURUMU_KIRAYAUYGUN);
            var jsString = CreateTasinmazModalDataTable(jsonData); //javascript kodu hazirlaniyor.
            UtilityHelper.ScriptCalistir(jsString);
            UtilityHelper.ScriptCalistir("TasinmazSecimiModal();");
        }
        private string CreateTasinmazModalDataTable(string jsonData)
        {
            string tableString = @"
        jQuery(document).ready(function () {
            if ( jQuery.fn.DataTable.isDataTable('#TasinmazModalDataTable') ) {
                jQuery('#TasinmazModalDataTable').DataTable().destroy();
            }
            jQuery('#TasinmazModalDataTable tbody').empty();

            jQuery.fn.dataTable.moment('DD.MM.YYYY');//sort date
            jQuery('#TasinmazModalDataTable').DataTable({
                data: " + jsonData + @",
                columns: [
                    { data: 'KullanimSekli' },
                    { data: 'MulkiyetSekli' },
                    { data: 'KiraDurumu' },
                    { data: 'AdresIliIlcesi' },
                    { data: 'BolumNo' },
                    { data: 'TasinmazId' },
                ],
                'order': [[0, 'desc']],

                'language': {
                    'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                    'decimal': ',',
                    'thousands': '.'
                },
                columnDefs:[
                    {targets:5, render:function(data, type, row, meta){
                        var linkEkle='<a href=" + ProjeConstants.PAGE_KIRASOZLESME_TASINMAZ + @"?KiraSozlesmeId=" + KiraSozlesmeIdQS +
                            @"&EnvanterdeMi=1&DestinationApp=TD&TasinmazId=' + row.TasinmazId + '&BolumId=' + row.BolumId + ' class=\'btn btn-outline-primary \'>Sözlesmeye Ekle</a>'
                        return linkEkle;
                    }},
                ],
                responsive: true,
                dom: 'frtip',

            });
        });
        ";
            return tableString;
        }
       
        private void KiraciSecimiModalShow()
        {
            Kiraci kiraci = new Kiraci();
            var jsonData = kiraci.SelectAllReturnJson();
            var jsString = CreateKiraciModalDataTable(jsonData); //javascript kodu hazirlaniyor.
            UtilityHelper.ScriptCalistir(jsString);
            UtilityHelper.ScriptCalistir("KiraciSecimiModal();");
        }

        private string CreateKiraciModalDataTable(string jsonData)
        {
            string tableString = @"
        jQuery(document).ready(function () {
            if ( jQuery.fn.DataTable.isDataTable('#KiraciModalDataTable') ) {
                jQuery('#KiraciModalDataTable').DataTable().destroy();
            }
            jQuery('#KiraciModalDataTable tbody').empty();

            jQuery.fn.dataTable.moment('DD.MM.YYYY');//sort date
            jQuery('#KiraciModalDataTable').DataTable({
                data: " + jsonData + @",
                columns: [
                    { data: 'Adi' },
                    { data: 'Adres' },
                    { data: 'IlIlce' },
                    { data: 'KiraciId' },
                ],
                'order': [[0, 'desc']],

                'language': {
                    'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                    'decimal': ',',
                    'thousands': '.'
                },
                columnDefs:[
                    {targets:3, render:function(data, type, row, meta){
                        var linkEkle='<a href=" + ProjeConstants.PAGE_KIRASOZLESME_TASINMAZ + @"?KiraSozlesmeId=" + KiraSozlesmeIdQS +
                            @"&DestinationApp=ST&KiraciId=' + row.KiraciId + ' class=\'btn btn-outline-primary \'>Kiraciyi Seç</a>'
                        return linkEkle;
                    }},
                ],
                responsive: true,
                dom: 'frtip',

            });
        });
        ";
            return tableString;
        }
        private void EnvanterdeOlmayanTasinmazSecimiModalShow()
        {
            Tasinmaz tasinmaz = new Tasinmaz();
            var jsonData = tasinmaz.SelectEnvanterdeOlmayanTasinmazReturnJson();
            var jsString = CreateEnvanterdeOlmayanTasinmazModalDataTable(jsonData); //javascript kodu hazirlaniyor.
            UtilityHelper.ScriptCalistir(jsString);
            UtilityHelper.ScriptCalistir("EnvanterdeOlmayanTasinmazSecimiModal();");
        }
        private string CreateEnvanterdeOlmayanTasinmazModalDataTable(string jsonData)
        {
            string tableString = @"
        jQuery(document).ready(function () {
            if ( jQuery.fn.DataTable.isDataTable('#EnvanterdeOlmayanTasinmazSecimiModal') ) {
                jQuery('#EnvanterdeOlmayanTasinmazModalDataTable').DataTable().destroy();
            }
            jQuery('#EnvanterdeOlmayanTasinmazModalDataTable tbody').empty();

            jQuery.fn.dataTable.moment('DD.MM.YYYY');//sort date
            jQuery('#EnvanterdeOlmayanTasinmazModalDataTable').DataTable({
                data: " + jsonData + @",
                columns: [
                    { data: 'Adres' },
                    { data: 'Ili' },
                    { data: 'Ilcesi' },
                    { data: 'TasinmazId' },
                ],
                'order': [[0, 'desc']],

                'language': {
                    'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                    'decimal': ',',
                    'thousands': '.'
                },
                columnDefs:[
                    {targets:3, render:function(data, type, row, meta){
                        var linkEkle='<a href=" + ProjeConstants.PAGE_KIRASOZLESME_TASINMAZ + @"?KiraSozlesmeId=" + KiraSozlesmeIdQS +
                            @"&EnvanterdeMi=1&DestinationApp=TD&TasinmazId=' + row.TasinmazId + ' class=\'btn btn-outline-primary \'>Sözlesmeye Ekle</a>'
                        return linkEkle;
                    }},
                ],
                responsive: true,
                dom: 'frtip',

            });
        });
        ";
            return tableString;
        }
        #endregion
    }
}
