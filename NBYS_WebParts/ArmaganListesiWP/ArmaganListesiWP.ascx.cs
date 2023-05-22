using Model.NBYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.ArmaganListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class ArmaganListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public ArmaganListesiWP()
        {
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string SecilenIdQS
        {
            get
            {

                if (ViewState["SecilenId"] == null)
                {
                    if (Page.Request.QueryString["SecilenId"] != null)
                    {
                        ViewState["SecilenId"] = Page.Request.QueryString["SecilenId"];
                    }
                    else
                    {
                        ViewState["SecilenId"] = "0";
                    }
                }
                return ViewState["SecilenId"].ToString();
            }

            set
            {
                ViewState["SecilenId"] = value;
            }
        }
        private string SecilenGunQS
        {
            get
            {

                if (ViewState["SecilenGun"] == null)
                {
                    if (Page.Request.QueryString["SecilenGun"] != null)
                    {
                        ViewState["SecilenGun"] = Page.Request.QueryString["SecilenGun"];
                    }
                    else
                    {
                        ViewState["SecilenGun"] = string.Empty;
                    }
                }
                return ViewState["SecilenGun"].ToString();
            }

            set
            {
                ViewState["SecilenGun"] = value;
            }
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
        private string SecilenBastarQS
        {
            get
            {

                if (ViewState["SecilenBastar"] == null)
                {
                    if (Page.Request.QueryString["SecilenBastar"] != null)
                    {
                        ViewState["SecilenBastar"] = Page.Request.QueryString["SecilenBastar"];
                    }
                    else
                    {
                        ViewState["SecilenBastar"] = string.Empty;
                    }
                }
                return ViewState["SecilenBastar"].ToString();
            }

            set
            {
                ViewState["SecilenBastar"] = value;
            }
        }
        private string SecilenBittarQS
        {
            get
            {

                if (ViewState["SecilenBittar"] == null)
                {
                    if (Page.Request.QueryString["SecilenBittar"] != null)
                    {
                        ViewState["SecilenBittar"] = Page.Request.QueryString["SecilenBittar"];
                    }
                    else
                    {
                        ViewState["SecilenBittar"] = string.Empty;
                    }
                }
                return ViewState["SecilenBittar"].ToString();
            }

            set
            {
                ViewState["SecilenBittar"] = value;
            }
        }
        private string SecilenArmaganTanimIdQS
        {
            get
            {

                if (ViewState["SecilenArmaganTanimId"] == null)
                {
                    if (Page.Request.QueryString["SecilenArmaganTanimId"] != null)
                    {
                        ViewState["SecilenArmaganTanimId"] = Page.Request.QueryString["SecilenArmaganTanimId"];
                    }
                    else
                    {
                        ViewState["SecilenArmaganTanimId"] = string.Empty;
                    }
                }
                return ViewState["SecilenArmaganTanimId"].ToString();
            }

            set
            {
                ViewState["SecilenArmaganTanimId"] = value;
            }
        }
        private string SecilenDurumQS
        {
            get
            {

                if (ViewState["SecilenDurum"] == null)
                {
                    if (Page.Request.QueryString["SecilenDurum"] != null)
                    {
                        ViewState["SecilenDurum"] = Page.Request.QueryString["SecilenDurum"];
                    }
                    else
                    {
                        ViewState["SecilenDurum"] = ProjeConstants.HEPSI;
                    }
                }
                return ViewState["SecilenDurum"].ToString();
            }

            set
            {
                ViewState["SecilenDurum"] = value;
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
                        ViewState["SecilenIl"] = ProjeConstants.HEPSI_INT.ToString();
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
            if (!Page.IsPostBack)
            {
                SecilenIdQS = string.IsNullOrEmpty(SecilenIdQS) ? "0" : SecilenIdQS;
                DDLleriDoldur();
                SetDDLValues(); //ay ve yılı querystringden al
                SetSecilenBasTarBitTar();
                TabloOlustur();
            }
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {

        }
        protected void ModalDoldurBtn_Click(object sender, EventArgs e)
        {
            try
            {
                ModalNakitBagisTablosunuDoldur(paramArmaganIdLbl.Value);
                ModalNakitBagisciFormunuDoldur(paramArmaganIdLbl.Value);
                UtilityHelper.ScriptCalistir("SetPageIndex();");
            }
            catch (Exception exception)
            {
                ExceptionHelper ex = new ExceptionHelper(exception);
                ex.PublishException();

            }
        }
        private void ModalNakitBagisTablosunuDoldur(string armaganId)
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);

            ModalArmaganTableHeaders();
            NakitBagisHareket nbh = new NakitBagisHareket();
            DataTable dataTable = nbh.SelectByArmaganIdReturnDataTable(armaganId.ConvertToInt());
            if (dataTable != null)
            {
                DataRow baslikRow = dataTable.Rows[0];
                string armagan = baslikRow["Armagan"].ReturnEmptyIfNull().ToString();
                string dovizCinsi = baslikRow["DovizCinsi"].ReturnEmptyIfNull().ToString();
                decimal armaganTutari = baslikRow["ArmaganTutari"].ConvertToDecimal();
                string armaganTutariStr = armaganTutari > 0 ? armaganTutari.ToString("N", culturInfo) + " " + dovizCinsi : string.Empty;
                ArmaganLbl.Text = armagan + " (" + armaganTutariStr + ")";
                foreach (DataRow row in dataTable.Rows)
                {
                    string bagisTarihi = row["BagisTarihi"].ConvertToDatetime().ConvertToDatetimeEmptyIfNull();
                    string bagisTutari = row["BagisTutari"].ConvertToDecimal().ToString("N", culturInfo);
                    string banka = row["Banka"].ReturnEmptyIfNull().ToString();
                    //string armaganTutari = row["ArmaganTutari"].ConvertToDecimal().ToString("N", culturInfo);
                    string durum = row["Durum"].ReturnEmptyIfNull().ToString();
                    string aciklama = row["Aciklama"].ReturnEmptyIfNull().ToString();
                    SatirEkle(bagisTarihi, bagisTutari, banka, durum, aciklama);
                }
            }
        }
        private void SatirEkle(string bagisTarihi, string bagisTutari, string banka, string durum, string aciklama)
        {
            TableRow row = new TableRow();
            TableCell bagisTarihiCell = new TableCell
            {
                Text = bagisTarihi
            };
            TableCell bagisTutariCell = new TableCell
            {
                Text = bagisTutari
            };
            TableCell bankaCell = new TableCell
            {
                Text = banka
            };
            TableCell durumCell = new TableCell
            {
                Text = durum
            };
            TableCell aciklamaCell = new TableCell
            {
                Text = aciklama
            };
            row.Controls.Add(bagisTarihiCell);
            row.Controls.Add(bagisTutariCell);
            row.Controls.Add(bankaCell);
            row.Controls.Add(durumCell);
            row.Controls.Add(aciklamaCell);
            ModalArmaganTable.Rows.Add(row);
        }
        private void ModalNakitBagisciFormunuDoldur(string nakitBagisciIdStr)
        {
            if (!string.IsNullOrEmpty(nakitBagisciIdStr))
            {
                int armaganId = nakitBagisciIdStr.ConvertToInt();

                Armagan armagan = new Armagan();
                armagan = armagan.Select<Armagan>(armaganId);
                if (armagan != null)
                {
                    NakitBagisci nakitBagisci = new NakitBagisci();
                    nakitBagisci = nakitBagisci.Select<NakitBagisci>(armagan.BagisciId);
                    //NakitBagisciIdLbl.Text = nakitBagisciId.ToString();
                    TableRow row = new TableRow();
                    TableCell AdiCell = new TableCell();
                    TableCell TCKimlikNoCell = new TableCell();
                    TableCell AdresCell = new TableCell();
                    TableCell IlIlceCell = new TableCell();
                    TableCell TelefonCell = new TableCell();
                    TableCell TuzelKisiCell = new TableCell();

                    AdiCell.Text = nakitBagisci.Adi.ReturnEmptyIfNull().ToString();
                    TCKimlikNoCell.Text = nakitBagisci.TCKimlikNo.ReturnEmptyIfNull().ToString();
                    AdresCell.Text = nakitBagisci.Adres.ReturnEmptyIfNull().ToString();

                    int ilId = nakitBagisci.Ili.ConvertToInt();
                    Il il = new Il();
                    il = il.Select<Il>(ilId);
                    if (il != null)
                    {

                        IlIlceCell.Text = il.IlAdi.ReturnEmptyIfNull().ToString();
                    }


                    int ilceId = nakitBagisci.Ilcesi.ConvertToInt();
                    Ilce ilce = new Ilce();
                    ilce = ilce.Select<Ilce>(ilceId);
                    if (ilce != null)
                    {

                        IlIlceCell.Text += " " + ilce.IlceAdi.ReturnEmptyIfNull().ToString();
                    }


                    TelefonCell.Text = nakitBagisci.Telefon1.ReturnEmptyIfNull().ToString();
                    TuzelKisiCell.Text = nakitBagisci.TuzelKisi.ConvertToBool() ? "Evet" : "Hayır";
                    row.Controls.Add(AdiCell);
                    row.Controls.Add(TCKimlikNoCell);
                    row.Controls.Add(AdresCell);
                    row.Controls.Add(IlIlceCell);
                    row.Controls.Add(TelefonCell);
                    row.Controls.Add(TuzelKisiCell);
                    BagisciTable.Controls.Add(row);

                }

            }

        }

        private void DDLleriDoldur()
        {
            GunDDLDoldur();
            AyDDLDoldur();
            YilDDLDoldur();
            DurumDDLDoldur();
            IlDDLDoldur();
            ArmaganTanimDDLDOoldur();
        }
        private void GunDDLDoldur()
        {
            //ARMAGAN PERIODU
            //10 Günde bir
            //GunDDL.Items.Add(new ListItem("Tüm Ay", "0"));
            //GunDDL.Items.Add(new ListItem("1-10", "1"));
            //GunDDL.Items.Add(new ListItem("11-20", "2"));
            //GunDDL.Items.Add(new ListItem("20-Ay Sonu", "3"));

            //15 Günde bir
            GunDDL.Items.Add(new ListItem("Tüm Ay", "0"));
            GunDDL.Items.Add(new ListItem("1-15", "1"));
            GunDDL.Items.Add(new ListItem("16-Ay Sonu", "2"));
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
        private void DurumDDLDoldur()
        {
            DurumDDL.Items.Add(ProjeConstants.HEPSI);
            DurumDDL.Items.Add(ProjeConstants.DURUM_BELGE_ISTEMIYOR);
            DurumDDL.Items.Add(ProjeConstants.DURUM_GONDERILMEDI);
            DurumDDL.Items.Add(ProjeConstants.DURUM_GONDERILDI);
            DurumDDL.Items.Add(ProjeConstants.DURUM_ULASILAMADI);
            DurumDDL.Items.Add(ProjeConstants.DURUM_KONTROLEDILDI);
            DurumDDL.Items.Add(ProjeConstants.DURUM_ERKENGONDERILDI);
            DurumDDL.Items.Add(ProjeConstants.DURUM_ERTELENDI);
            DurumDDL.Items.Add(ProjeConstants.DURUM_IADE);
            DurumDDL.Items.Add(ProjeConstants.DURUM_DAHAONCEIADE);
            DurumDDL.Items.Add(ProjeConstants.DURUM_PARAIADE);
            DurumDDL.Items.Add(ProjeConstants.DURUM_AFETNEDENIYLE_GONDERILMEDI);

            //acilista durumu querystring ile gelene eşitle
            ListItem DurumItem = new ListItem();
            if (!string.IsNullOrEmpty(SecilenDurumQS))
                DurumItem = DurumDDL.Items.FindByValue(SecilenDurumQS);

            if (DurumItem != null)
                DurumDDL.SelectedValue = DurumItem.Value;
        }
        private void IlDDLDoldur()
        {
            if (IliDDL.SelectedItem == null)
            {
                IliDDL.Items.Clear();

                Il pIl = new Il();
                List<Il> list = pIl.SelectAll<Il>();
                IliDDL.Items.Add(new ListItem(ProjeConstants.HEPSI, ProjeConstants.HEPSI_INT.ToString()));
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
                //acilista ay ve yili querystring ile gelen ay ve yıla eşitle boş geldiyse gecen aya/yila eşitle
                //gün
                int gunBolumu = DateTime.Today.Day < 16 ? 1 : 2;
                string gun = !string.IsNullOrEmpty(SecilenGunQS) ? SecilenGunQS : gunBolumu.ToString();
                ListItem gunItem = new ListItem();
                if (!string.IsNullOrEmpty(gun))
                    gunItem = GunDDL.Items.FindByValue(gun);

                if (gunItem != null)
                {
                    GunDDL.SelectedValue = gunItem.Value;
                    SecilenGunQS = gunItem.Value;
                }
                //ay
                string ay = !string.IsNullOrEmpty(SecilenAyQS) ? SecilenAyQS : DateTime.Today.Month.ReturnEmptyIfNull().ToString();
                ListItem ayItem = new ListItem();
                if (!string.IsNullOrEmpty(ay))
                    ayItem = AyDDL.Items.FindByValue(ay);

                if (ayItem != null)
                {
                    AyDDL.SelectedValue = ayItem.Value;
                    SecilenAyQS = ayItem.Value;
                }

                //yil
                string yil = !string.IsNullOrEmpty(SecilenYilQS) ? SecilenYilQS : DateTime.Today.Year.ReturnEmptyIfNull().ToString();
                ListItem yilItem = new ListItem();
                if (!string.IsNullOrEmpty(yil))
                    yilItem = YilDDL.Items.FindByValue(yil);

                if (yilItem != null)
                {
                    YilDDL.SelectedValue = yilItem.Value;
                    SecilenYilQS = yilItem.Value;
                }

                //ArmaganTanim
                string armaganTanim = !string.IsNullOrEmpty(SecilenArmaganTanimIdQS) ? SecilenArmaganTanimIdQS : ProjeConstants.HEPSI;
                ListItem ArmaganTanimItem = new ListItem();
                if (!string.IsNullOrEmpty(armaganTanim))
                    ArmaganTanimItem = ArmaganDDL.Items.FindByValue(armaganTanim);

                if (ArmaganTanimItem != null)
                {
                    ArmaganDDL.SelectedValue = ArmaganTanimItem.Value;
                    SecilenArmaganTanimIdQS = ArmaganTanimItem.Value;
                }

                //Durum
                string durum = !string.IsNullOrEmpty(SecilenDurumQS) ? SecilenDurumQS : ProjeConstants.HEPSI;
                ListItem DurumItem = new ListItem();
                if (!string.IsNullOrEmpty(durum))
                    DurumItem = DurumDDL.Items.FindByValue(durum);

                if (DurumItem != null)
                {
                    DurumDDL.SelectedValue = DurumItem.Value;
                    SecilenDurumQS = DurumItem.Value;
                }
                //Il
                int ili = SecilenIlQS.ConvertToInt()>0 ? SecilenIlQS.ConvertToInt() : ProjeConstants.HEPSI_INT;
                ListItem ilListItem = new ListItem();
                if (ili>0)
                    ilListItem = IliDDL.Items.FindByValue(ili.ToString());

                if (ilListItem != null)
                {
                    IliDDL.SelectedValue = ilListItem.Value;
                    SecilenIlQS = ilListItem.Value;
                }
            }
            catch (Exception)
            {

                //TODO
            }



        }
        private void ArmaganTanimDDLDOoldur()
        {
            if (ArmaganDDL.SelectedItem == null)
            {
                ArmaganDDL.Items.Clear();
                ArmaganTanim armaganTanim = new ArmaganTanim();
                List<ArmaganTanim> list = armaganTanim.SelectAll<ArmaganTanim>();
                //Tum armaganlar secenegi
                ArmaganDDL.Items.Add(new ListItem(ProjeConstants.HEPSI, ProjeConstants.HEPSI));
                foreach (ArmaganTanim arm in list)
                {
                    if (!arm.Id.Equals(ProjeConstants.ARMAGAN_YOKID))//Armagan listesinde Armagan türü "Yok" olanlar zaten gösterilmiyor, o yüzden dropdown listesine "Yok" gelmesin
                        ArmaganDDL.Items.Add(new ListItem(arm.Armagan, arm.Id.ToString()));
                }

                //acilista armagan filtresini querystring ile gelene eşitle
                ListItem ArmaganTanimItem = new ListItem();
                if (!string.IsNullOrEmpty(SecilenArmaganTanimIdQS))
                    ArmaganTanimItem = ArmaganDDL.Items.FindByValue(SecilenArmaganTanimIdQS);

                if (ArmaganTanimItem != null)
                    ArmaganDDL.SelectedValue = ArmaganTanimItem.Value;

            }

        }
        private void SetSecilenBasTarBitTar()
        {
            string gunStr = GunDDL.SelectedItem.Value;
            int ay = AyDDL.SelectedItem.Value.ConvertToInt();
            int yil = YilDDL.SelectedItem.Value.ConvertToInt();

            DateTime bastar = DateTime.Today;
            DateTime bittar = DateTime.Today;

            if (ay == 0)
            {
                bastar = new DateTime(yil, 1, 1);
                bittar = bastar.AddYears(1).AddDays(-1);
            }

            else
            {
                //ARMAGAN PERIODU

                //15 Günde bir
                if (gunStr.Equals("0"))
                {
                    bastar = new DateTime(yil, ay, 1);
                    int songun = bastar.AddMonths(1).AddDays(-1).Day;
                    bittar = new DateTime(yil, ay, songun);
                }
                else if (gunStr.Equals("1"))
                {
                    bastar = new DateTime(yil, ay, 1);
                    bittar = new DateTime(yil, ay, 15);
                }
                else
                {
                    bastar = new DateTime(yil, ay, 16);
                    DateTime sonGun = new DateTime(yil, ay, 1).AddMonths(1).AddDays(-1);
                    bittar = new DateTime(yil, ay, sonGun.Day);
                }

                //10 Günde bir
                //if (gunStr.Equals("0"))
                //{
                //    basTar = new DateTime(yil, ay, 1);
                //    int songun = basTar.AddMonths(1).AddDays(-1).Day;
                //    bitTar = new DateTime(yil, ay, songun);
                //}
                //else if (gunStr.Equals("1"))
                //{
                //    basTar = new DateTime(yil, ay, 1);
                //    bitTar = new DateTime(yil, ay, 10);
                //}
                //else if (gunStr.Equals("2"))
                //{
                //    basTar = new DateTime(yil, ay, 11);
                //    bitTar = new DateTime(yil, ay, 20);
                //}
                //else if (gunStr.Equals("3"))
                //{
                //    basTar = new DateTime(yil, ay, 21);
                //    DateTime basGun = new DateTime(yil, ay, 1).AddMonths(1).AddDays(-1);
                //    bitTar = new DateTime(yil, ay, basGun.Day);
                //}
            }
            SecilenBastarQS = bastar.ConvertToDatetimeEmptyIfNull();
            SecilenBittarQS = bittar.ConvertToDatetimeEmptyIfNull();
        }
        protected void GunDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            SecilenGunQS = GunDDL.SelectedItem.Value.ToString();
            SetSecilenBasTarBitTar();
            TabloOlustur();
        }
        protected void AyDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            SecilenAyQS = AyDDL.SelectedItem.Value.ToString();
            SetSecilenBasTarBitTar();
            TabloOlustur();
        }
        protected void YilDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            SecilenYilQS = YilDDL.SelectedItem.Value.ToString();
            SetSecilenBasTarBitTar();
            TabloOlustur();
        }
        protected void ArmaganDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            SecilenArmaganTanimIdQS = ArmaganDDL.SelectedItem.Value.ToString();
            TabloOlustur();
        }
        protected void DurumDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            SecilenDurumQS = DurumDDL.SelectedItem.Value.ToString();
            TabloOlustur();
        }
        protected void IliDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            SecilenIlQS = IliDDL.SelectedItem.Value.ToString();
            TabloOlustur();
        }
        protected void IadeEdildiYapBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Armagan armagan = new Armagan();
                armagan = armagan.Select<Armagan>(paramArmaganIdLbl.Value.ConvertToInt());
                if (armagan != null)
                {
                    armagan.Durum = ProjeConstants.DURUM_IADE;
                    if (armagan.Update())
                    {
                        TabloOlustur();
                        MessageHelper.PublishMessage(" Armağan Durumu " + ProjeConstants.DURUM_IADE
                            + " Olarak Değiştirildi", ProjeConstants.MESAJ_BASARILI, 2000);
                        //RedirectToPage(ProjeConstants.PAGE_ARMAGAN_LIST + "?Mesaj=true&SecilenAy=" + SecilenAyQS + "&SecilenYıl=" + SecilenYilQS + "&SecilenGun=" + SecilenGunQS +
                        //    "&PageIndex=" + PageIndexLbl.Value + "&SecilenArmaganTanimId=" + SecilenArmaganTanimIdQS + "&SecilenDurum=" + SecilenDurumQS);
                    }
                    else
                    {
                        MessageHelper.PublishMessage(" Armağan Durumu Değiştirilemedi.", ProjeConstants.MESAJ_HATA);
                    }
                }
                else
                {
                    MessageHelper.PublishMessage(" Armağan Bulunamadı ", ProjeConstants.MESAJ_HATA);
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exh = new ExceptionHelper();
                exh.Exceptions.Add(new Exception("Armağan durumu kaydedilirken hata oluştu. "));
                exh.Exceptions.Add(ex);
                exh.PublishException();
            }
        }
        protected void EtiketOlusturBtn_Click(object sender, EventArgs e)
        {
            var queryString = string.Format("?Bastar={0}&Bittar={1}", SecilenBastarQS, SecilenBittarQS);
            RedirectToPage(ProjeConstants.PAGE_ARMAGANETIKET_VIEWER + queryString);
            TabloOlustur();
        }
        protected void ArmaganListesiBtn_Click(object sender, EventArgs e)
        {

            string tarihStr = SecilenBastarQS.ConvertToDatetimeEmptyIfNull() + " - " + SecilenBittarQS.ConvertToDatetimeEmptyIfNull();
            var queryString = string.Format("?Bastar={0}&Bittar={1}&TarihStr={2}", SecilenBastarQS, SecilenBittarQS, tarihStr);
            RedirectToPage(ProjeConstants.PAGE_ARMAGANLISTESI_VIEWER + queryString);
            TabloOlustur();
        }
        private void RedirectToPage(string pageUrl)
        {
            try
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                //string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + pageUrl;

                string newUrl = "http://tskgv-portal/YonetimBirimleri/BasinTanitimHalklaIliskilerSubesi/Sayfalar" + "/" + pageUrl;
                ResponseHelper.Redirect(newUrl, "_blank", "");
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        #region Liste Olusturma
        private void TabloOlustur()
        {
            var jsonData = TabloJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string TabloJson()
        {
            Armagan armagan = new Armagan();

            int rowCount = 0;

            string gunStr = GunDDL.SelectedItem.Value;
            int ay = AyDDL.SelectedItem.Value.ConvertToInt();
            int yil = YilDDL.SelectedItem.Value.ConvertToInt();

            DateTime bastar = DateTime.Today;
            DateTime bittar = DateTime.Today;

            if (ay == 0)
            {
                bastar = new DateTime(yil, 1, 1);
                bittar = bastar.AddYears(1).AddDays(-1);
            }
            else
            {
                //ARMAGAN PERIODU
                //15 Günde bir
                if (gunStr.Equals("0"))
                {
                    bastar = new DateTime(yil, ay, 1);
                    int songun = bastar.AddMonths(1).AddDays(-1).Day;
                    bittar = new DateTime(yil, ay, songun);
                }
                else if (gunStr.Equals("1"))
                {
                    bastar = new DateTime(yil, ay, 1);
                    bittar = new DateTime(yil, ay, 15);
                }
                else
                {
                    bastar = new DateTime(yil, ay, 16);
                    DateTime sonGun = new DateTime(yil, ay, 1).AddMonths(1).AddDays(-1);
                    bittar = new DateTime(yil, ay, sonGun.Day);
                }

                // 10 Günde bir
                //if (gunStr.Equals("0"))
                //{
                //    bastar = new DateTime(yil, ay, 1);
                //    int songun = bastar.AddMonths(1).AddDays(-1).Day;
                //    bittar = new DateTime(yil, ay, songun);
                //}
                //else if (gunStr.Equals("1"))
                //{
                //    bastar = new DateTime(yil, ay, 1);
                //    bittar = new DateTime(yil, ay, 11);
                //}
                //else if (gunStr.Equals("2"))
                //{
                //    bastar = new DateTime(yil, ay, 11);
                //    bittar = new DateTime(yil, ay, 20);
                //}
                //else if (gunStr.Equals("3"))
                //{
                //    bastar = new DateTime(yil, ay, 21);
                //    DateTime basGun = new DateTime(yil, ay, 1).AddMonths(1).AddDays(-1);
                //    bittar = new DateTime(yil, ay, basGun.Day);

            }
            int ili = SecilenIlQS.ConvertToInt();
            var json = armagan.SelectByDurumTarih(DurumDDL.SelectedItem.Text, bastar, bittar, ArmaganDDL.SelectedItem.Value, ref rowCount, ProjeConstants.BOLGE_HEPSI, ili);
            return json;

        }
        private string CreateDataTable(string jsonData)
        {
            string reportTesekkurUrl = "http://tskgv-portal/YonetimBirimleri/BasinTanitimHalklaIliskilerSubesi/Sayfalar" + "/" + ProjeConstants.PAGE_TESEKKURBELGESITEK_VIEWER + "?ArmaganId=";
            string reportBeratUrl = "http://tskgv-portal/YonetimBirimleri/BasinTanitimHalklaIliskilerSubesi/Sayfalar" + "/" + ProjeConstants.PAGE_BERATBELGESITEK_VIEWER + "?ArmaganId=";

            string queryStr = "&SecilenGun=" + SecilenGunQS + "&SecilenAy=" + SecilenAyQS + "&SecilenYil="
                + SecilenYilQS + "&SecilenArmaganTanimId=" + SecilenArmaganTanimIdQS + "&SecilenDurum=" + SecilenDurumQS + "&SecilenIl=" + SecilenIlQS;

            string tableString = @"
                if ( jQuery.fn.DataTable.isDataTable('#CustomDataTable') ) {
                    jQuery('#CustomDataTable').DataTable().destroy();
                }
                jQuery('#CustomDataTable tbody').empty();

                jQuery.fn.dataTable.moment('DD.MM.YYYY');//sort date

                jQuery('#CustomDataTable').DataTable({
                    'initComplete': function (settings, json) {//tablo yüklendiğinde
                        var api = this.api();
                        var row = api.row(function(idx, data, node) { //secilen satıra gider
                            return data['ArmaganId'] ==" + SecilenIdQS + @";
                        });
                        if (row.length > 0)
                        {
                            row.select()
                                .show()
                                .draw(false);
                        }
                    },
                    columnDefs:[
                        {targets:0, render:function(data, type, row, meta){
                            if (row.Durum == 'Parası İade Edildi'){
                                return data;
                            }else
                            {
                                return ('<a href=# onclick=OpenModal('+row.ArmaganId+'); type=button class=\'btn btn-link font-weight-bold\'>'+data+'</a>')
                            }
                        }},
                        {targets:4, render:function(data){
                            return moment(data).format('DD.MM.YYYY');
                        }},
                        {targets:5, render:function(data, type, row, meta){
                            var link= '<span class=grayLayout>'+row.ArmaganBaslik+'</span>';
                            if(row.Durum.toString().indexOf('Kontrol Edildi')>=0){
                                if(row.ArmaganBaslik.toString().indexOf('Teşekkür Belgesi')>=0)
                                {
                                    var url='" + reportTesekkurUrl + @"'+row.ArmaganId;
                                    link='<a target=_blank href='+url+' class=btn-link >'+row.ArmaganBaslik+'</a>';
                                }
                                else if(row.ArmaganBaslik.toString().indexOf('Bronz Madalya ve Beratı')>=0 || 
                                    row.ArmaganBaslik.toString().indexOf('Gümüş Madalya ve Beratı')>=0 || 
                                    row.ArmaganBaslik.toString().indexOf('Altın Madalya ve Beratı')>=0)
                                {
                                    var url='" + reportBeratUrl + @"'+row.ArmaganId;
                                    link='<a target=_blank href='+url+' class=btn-link >'+row.ArmaganBaslik+'</a>';
                                }
                            }
                            return link;
                        }},
                        {targets:7, render:function(data, type, row, meta){
                            var link= '';
                            if (row.Durum == 'Parası İade Edildi'){
                                return 'Belge Geçersiz';
                            }else
                            {
                                link='<a href='+'" + ProjeConstants.PAGE_ARMAGAN_EDIT + @"?ArmaganId='+row.ArmaganId+'&NakitBagisciId='+row.NakitBagisciId+'" + queryStr + @" class=\'btn btn-outline-primary\' >Düzenle</a>';
                            }

                            return link;
                        }},
                        {targets:8, render:function(data, type, row, meta){
                            var link='';
                            if (row.Durum==='Gönderildi')
                            {
                                link='<a href=# onclick=CallButtonClick('+row.ArmaganId + '); class=\'btn btn-outline-danger \'>İade Edildi Yap</a>';
                            }else if (row.Durum == 'Parası İade Edildi')
                            {
                                link= '<span>'+row.IadeMiktari+ ' '+row.DovizCinsi +' İade edildi</span>';
                            }       
                            return link;
                        }}],    
                    data: " + jsonData + @",
                    columns: [
                        { data: 'NakitBagisciAdi'},
                        { data: 'BelgedeYazanIsim', 'width':'10%' },
                        { data: 'NakitBagisciTC' },
                        { data: 'Tutar', 'width':'10%', 'className': 'text-right'},
                        { data: 'Tarih' },
                        { data: 'ArmaganBaslik' , 'width':'20%'},
                        { data: 'Durum' },
                        { data: 'ArmaganId' },
                        { data: 'ArmaganId' },
                    ],
                    'order': [[7, 'asc']],//sort date desc
                    'language': {
                    'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                        'decimal': ',',
                        'thousands': '.'
                    },
                    responsive: true,
                    dom: 'frtip',   
                    stateSave: true,
                    'createdRow': function(row, data, dataIndex) {
                        var durum = data.Durum;
                        if (durum == 'Kontrol Edildi') {

                            $(row).addClass('kontrol-edildi');
                        }
                        else if (durum == 'Gönderildi') {
                            $(row).addClass('gonderildi');
                        }
                        else if (durum == 'Erken Gönderildi') {
                            $(row).addClass('gonderildi');
                        }
                        else if (durum == 'Gönderilmedi') {
                            //beyaz kalsın
                        }
                        else if (durum == 'Parası İade Edildi') {
                            $(row).addClass('belge-gecersiz');
                        }
                        else {
                            $(row).addClass('diger');
                        }
                        // bu iller AFET nedeniyle ekranda renki olsun
                        //var ili = data.IlId;
                        //if ((ili==1) ||(ili==2) ||(ili==21) ||(ili==27) ||(ili==31) ||(ili==44) ||(ili==46) ||(ili==63) ||(ili==79) ||(ili==80) )
                        //{
                        //    $(row).addClass('afet-ili');
                        //}
                    },//set row color
                });

            ";

            return tableString;
        }
        #endregion
        private void ModalArmaganTableHeaders()
        {
            ModalArmaganTable.Rows.Clear();
            TableHeaderRow headerRow = new TableHeaderRow();

            TableHeaderCell bagisTarihiCell = new TableHeaderCell
            {
                Text = "Bağış Tarihi"
            };

            TableHeaderCell BagisMiktariCell = new TableHeaderCell
            {
                Text = "Bağış Miktarı"
            };

            TableHeaderCell bankaCell = new TableHeaderCell
            {
                Text = "Banka"
            };

            TableHeaderCell durumCell = new TableHeaderCell
            {
                Text = "Durum"
            };
            TableHeaderCell aciklamaCell = new TableHeaderCell
            {
                Text = "Açıklama"
            };
            headerRow.Controls.Add(bagisTarihiCell);
            headerRow.Controls.Add(BagisMiktariCell);
            headerRow.Controls.Add(bankaCell);
            headerRow.Controls.Add(durumCell);
            headerRow.Controls.Add(aciklamaCell);

            ModalArmaganTable.Controls.Add(headerRow);

        }
    }
}