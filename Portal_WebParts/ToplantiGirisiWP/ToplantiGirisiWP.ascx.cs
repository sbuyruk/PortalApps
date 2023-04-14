using Model.IKYS;
using Model.MTS;
using Model.Ortak;
using Model.Portal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Portal_WebParts.ToplantiGirisiWP
{
    [ToolboxItemAttribute(false)]
    public partial class ToplantiGirisiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public ToplantiGirisiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string ToplantiIdQS
        {
            get
            {
                if (ViewState["ToplantiId"] == null)
                {
                    if (Page.Request.QueryString["ToplantiId"] != null)
                    {
                        ViewState["ToplantiId"] = Page.Request.QueryString["ToplantiId"];
                    }
                    else
                    {
                        ViewState["ToplantiId"] = string.Empty;
                    }
                }
                return ViewState["ToplantiId"].ToString();
            }
            set
            {
                ViewState["ToplantiId"] = value;
            }
        }
        private static Toplanti ToplantiIlkHaliQS { get; set; }
        private string InitialDateQS
        {
            get
            {

                if (ViewState["InitialDate"] == null)
                {
                    if (Page.Request.QueryString["InitialDate"] != null)
                    {
                        ViewState["InitialDate"] = Page.Request.QueryString["InitialDate"];
                    }
                    else
                    {
                        ViewState["InitialDate"] = string.Empty;
                    }
                }
                return ViewState["InitialDate"].ToString();
            }

            set
            {
                ViewState["InitialDate"] = value;
            }
        }
        private string MesajQS
        {
            get
            {

                if (ViewState["Mesaj"] == null)
                {
                    if (Page.Request.QueryString["Mesaj"] != null)
                    {
                        ViewState["Mesaj"] = Page.Request.QueryString["Mesaj"];
                    }
                    else
                    {
                        ViewState["Mesaj"] = string.Empty;
                    }
                }
                return ViewState["Mesaj"].ToString();
            }

            set
            {
                ViewState["Mesaj"] = value;
            }
        }
        private string PersonelIdQS
        {
            get
            {

                if (ViewState["PersonelId"] == null)
                {
                    if (Page.Request.QueryString["PersonelId"] != null)
                    {
                        ViewState["PersonelId"] = Page.Request.QueryString["PersonelId"];
                    }
                    else
                    {
                        ViewState["PersonelId"] = string.Empty;
                    }
                }
                return ViewState["PersonelId"].ToString();
            }

            set
            {
                ViewState["PersonelId"] = value;
            }
        }
        private List<int> BilgiIdListQS
        {
            get
            {

                if (ViewState["BilgiIdList"] == null)
                {
                    if (Page.Request.QueryString["BilgiIdList"] != null)
                    {
                        ViewState["BilgiIdList"] = Page.Request.QueryString["BilgiIdList"];
                    }
                    else
                    {
                        ViewState["BilgiIdList"] = new List<int>();
                    }
                }
                return (List<int>)ViewState["BilgiIdList"];
            }

            set
            {
                ViewState["BilgiIdList"] = value;
            }
        }
        private List<int> KatilimciIdListQS
        {
            get
            {

                if (ViewState["KatilimciIdList"] == null)
                {
                    if (Page.Request.QueryString["KatilimciIdList"] != null)
                    {
                        ViewState["KatilimciIdList"] = Page.Request.QueryString["KatilimciIdList"];
                    }
                    else
                    {
                        ViewState["KatilimciIdList"] = new List<int>();
                    }
                }
                return (List<int>)ViewState["KatilimciIdList"];
            }

            set
            {
                ViewState["KatilimciIdList"] = value;
            }
        }
        private List<int> CikanKatilimciIdListQS
        {
            get
            {

                if (ViewState["CikanKatilimciId"] == null)
                {
                    if (Page.Request.QueryString["CikanKatilimciId"] != null)
                    {
                        ViewState["CikanKatilimciId"] = Page.Request.QueryString["CikanKatilimciId"];
                    }
                    else
                    {
                        ViewState["CikanKatilimciId"] = new List<int>();
                    }
                }
                return (List<int>)ViewState["CikanKatilimciId"];
            }

            set
            {
                ViewState["CikanKatilimciId"] = value;
            }
        }
        private string CurrentUserName
        {
            get
            {

                if (ViewState["CurrentUserName"] == null)
                {
                    ViewState["CurrentUserName"] = UtilityHelper.GetCurrentUserLoginName();
                }
                return ViewState["CurrentUserName"].ToString();
            }

            set
            {
                ViewState["CurrentUserName"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack) // sayfa ilk kez açılıyorsa (bu sayfanın içindeki butona basılma anı hariç)
                {
                    ToplantiYeriDoldurDDL();

                    if (string.IsNullOrEmpty(ToplantiIdQS))
                    {
                        GirisiAc();
                    }
                    else
                    {
                        DuzenleAc();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exhelper = new ExceptionHelper(ex);
                exhelper.PublishException();
            }
        }
        private void ToplantiYeriIslemleri(Toplanti toplanti)
        {
            string sv = ProjeConstants.BOS;

            if (toplanti != null)
            {
                if (toplanti.ToplantiYeri > 0)
                    sv = toplanti.ToplantiYeri.ToString();

                UtilityHelper.SetDDLValue(ToplantiYeriDDL, sv);

                if (ToplantiYeriDDL.SelectedItem.Text.Equals(ProjeConstants.PARAM_DIGER))
                {
                    ToplantiYeriDigerTxt.Value = toplanti.ToplantiYeriDiger;
                    ToplantiYeriDigerDiv.Attributes["style"] = "display:block";
                }
                else
                {
                    ToplantiYeriDigerDiv.Attributes["style"] = "display:none";
                }
            }
        }
        private void ToplantiFormunuDoldur()
        {
            Toplanti toplanti = new Toplanti();
            toplanti = toplanti.Select(ToplantiIdQS.ConvertToInt());
            if (toplanti != null)
            {
                TitleLbl.Text = "Toplantı Düzenleme";
                TitleLbl.CssClass = "col-form-label text-primary font-weight-bold mb-1";
                IdLbl.Text = " ( Toplantı No: " + toplanti.Id.ToString() + " )";
                KatilimciBilgileriDiv.Attributes["style"] = "display:block";
                KaydetBtn.Visible = false;
                GuncelleBtn.Visible = true;
                ToplantiKatilimTutanagiBtn.Visible = true;
                ToplantiSilBtn.Visible = true;

                ToplantiKonusuTxt.Text = toplanti.ToplantiKonusu;
                BaslangicTarihiTxt.Text = toplanti.BaslangicTarihi.ConvertToDatetimeEmptyIfNull();
                InitialDateQS = toplanti.BaslangicTarihi.ToString("yyyy-MM-dd");
                BitisTarihiTxt.Text = toplanti.BitisTarihi.ConvertToDatetimeEmptyIfNull();
                AciklamaTxt.Text = toplanti.Aciklama;
                DisKatilimcilarTxt.Text = toplanti.DisKatilimcilar;
                CevrimIciTplantiChk.Checked = toplanti.CevrimIci;
                IkramOnayiChk.Checked = toplanti.IkramOnayi;
                IkramMalzemesiTxt.Text = toplanti.IkramMalzemesi;

                ToplantiYeriIslemleri(toplanti);
                ToplantiIlkHalineKopyala(toplanti);

                BaslangicSaatiDDLDoldur();
                UtilityHelper.SetDDLValue(ToplantiYeriDDL, toplanti.ToplantiYeri.ToString());
                UtilityHelper.SetDDLValue(BasSaatDDL, toplanti.BaslangicSaati);
                BitisSaatiDDLDoldur();
                UtilityHelper.SetDDLValue(BitSaatDDL, toplanti.BitisSaati);
            }

        }
        private void ToplantiIlkHalineKopyala(Toplanti toplanti)
        {
            if (ToplantiIlkHaliQS == null)
                ToplantiIlkHaliQS = new Toplanti();
            UtilityHelper.CopyProperties(toplanti, ToplantiIlkHaliQS);
        }
        private void KatilimciBilgileriniDoldur()
        {
            TabloOlustur(false);
        }
        private void KatilimciBilgileriniVeriTabanindanDoldur()
        {
            TabloOlustur(true);
        }
        private void GirisiAc()
        {
            if (ToplantiyaYetkiliMi())
            {
                KatilimciBilgileriDiv.Attributes["style"] = "display:block";
                KaydetBtn.Visible = true;
                GuncelleBtn.Visible = false;
                ToplantiKatilimTutanagiBtn.Visible = false;
                ToplantiSilBtn.Visible = false;
                TitleLbl.Text = "Yeni Toplantı";
                TitleLbl.CssClass = "col-form-label text-success font-weight-bold mb-1";
                BaslangicTarihiTxt.Text = DateTime.Today.ConvertToDatetimeEmptyIfNull();
                BitisTarihiTxt.Text = DateTime.Today.ConvertToDatetimeEmptyIfNull();
                BaslangicSaatiDDLDoldur();
                BitisSaatiDDLDoldur();
                if (!string.IsNullOrEmpty(MesajQS))
                {
                    MessageHelper.PublishMessage("Toplantı Silindi, e-posta gönderildi.", ProjeConstants.MESAJ_BASARILI, 2000);
                    MesajQS = string.Empty;
                }
                //dummy tabloda p.no gözükmesin diye
                KatilimciBilgileriniDoldur();
            }
            else
            {
                BilesenleriEkisizlestir();
            }

        }
        private void BilesenleriEkisizlestir()
        {
            ToplantiKonusuTxt.Enabled = false;
            BaslangicTarihiTxt.Enabled = false;
            BitisTarihiTxt.Enabled = false;
            ToplantiYeriDDL.Enabled = false;
            ToplantiYeriDigerTxt.Disabled = true;
            CevrimIciTplantiChk.Enabled = false;
            BasSaatDDL.Enabled = false;
            BitSaatDDL.Enabled = false;
            AciklamaTxt.Enabled = false;
            DisKatilimcilarTxt.Enabled = false;
            KatilimciEkleBtn.Enabled = false;
            KaydetBtn.Visible = false;
            GuncelleBtn.Visible = false;
            ToplantiKatilimTutanagiBtn.Visible = false;
            ToplantiSilBtn.Visible = false;
            KatilimciModalAcBtn.Visible = false;
            MessageHelper.PublishMessage("Toplantı işlemleri için yetkilendirilmediğinizden dolayı işlem yapamazsınız.", ProjeConstants.MESAJ_BILGI, 2000);
        }
        private bool ToplantiyaYetkiliMi()
        {
            bool yetkiliMi = false;
            Personel personel = new Personel();
            personel = PersonelGetir();
            if (personel != null)
            {
                ToplantiParametre toplantiParametre = new ToplantiParametre();
                string adiSoyadi = personel.Adi + " " + personel.Soyadi;
                List<ToplantiParametre> liste = toplantiParametre.SelectByGrupDeger(ProjeConstants.PARAM_TOPLANTIYETKILISI, adiSoyadi);
                yetkiliMi = liste.Count > 0;
            }

            return yetkiliMi;
        }
        private void DuzenleAc()
        {
            if (ToplantiyaYetkiliMi())
            {
                if (ToplantiIdQS.ConvertToInt() > 0)
                {
                    ToplantiFormunuDoldur();
                    KatilimciBilgileriniVeriTabanindanDoldur();
                }
                else
                {
                    GirisiAc();
                    MessageHelper.PublishMessage("Toplantı bulunamadı. Yeni toplantı girebilirsiniz.", ProjeConstants.MESAJ_BILGI, 2000);
                }
            }
            else
            {
                BilesenleriEkisizlestir();
            }
        }
        private void ToplantiYeriDoldurDDL()
        {
            ToplantiYeriDDL.Items.Clear();
            ToplantiParametre toplantiParametre = new ToplantiParametre();
            List<ToplantiParametre> list = toplantiParametre.SelectByGrupReturnList(ProjeConstants.PARAM_TOPLANTIYERI);

            ListItem li0 = new ListItem(ProjeConstants.BOS, ProjeConstants.BOS);
            ToplantiYeriDDL.Items.Add(li0);
            foreach (var item in list)
            {
                ListItem li = new ListItem(item.Deger, item.Id.ToString());
                ToplantiYeriDDL.Items.Add(li);
            }
            UtilityHelper.SetDDLValue(ToplantiYeriDDL, "");
        }

        private void BaslangicSaatiDDLDoldur()
        {
            BasSaatDDL.Items.Clear();
            if (string.IsNullOrEmpty(ToplantiYeriDDL.SelectedItem.Text))
            {
                ListItem li0 = new ListItem(string.Empty);
                BasSaatDDL.Items.Add(li0);
                ListItem li1 = new ListItem("Önce Toplantı Yeri Seçiniz...");
                BasSaatDDL.Items.Add(li1);
                li1.Attributes.Add("class", ".warning-item");
                li1.Attributes["disabled"] = "disabled";
            }
            else
            {
                TimeSpan aralikTS = ProjeConstants.TOPLANTI_MINUMUMSURESI;
                TimeSpan bittarTS = ProjeConstants.TOPLANTI_BITISZAMANI;

                TimeSpan bastarTS = ProjeConstants.TOPLANTI_BASLAMAZAMANI;
                if (DateTime.Today.ConvertToDatetimeEmptyIfNull().Equals(BaslangicTarihiTxt.Text.Trim()))
                {
                    decimal minD = (decimal)DateTime.Now.Minute / 10;
                    int min = (Math.Ceiling(minD)*10).ConvertToInt()+ aralikTS.Minutes;
                    bastarTS = new TimeSpan(DateTime.Now.Hour, min, 0);
                }

                TimeSpan nextTS = bastarTS;
                //bos satır ekle
                ListItem li0 = new ListItem(string.Empty);
                BasSaatDDL.Items.Add(li0);

                while (nextTS < bittarTS)
                {
                    string bastarStr = string.Format("{0:00}:{1:00}", nextTS.Hours, nextTS.Minutes);
                    ListItem li = new ListItem(bastarStr);
                    BasSaatDDL.Items.Add(li);
                    nextTS += aralikTS;
                }
                SetDisabledItemsBasSaatDDL();
            }

        }
        private void SetDisabledItemsBasSaatDDL()
        {
            if ((!string.IsNullOrEmpty(ToplantiYeriDDL.SelectedItem.Text)) &&
                (ToplantiYeriDDL.SelectedItem.Value.ConvertToInt() != ProjeConstants.PARAM_DIGER_INT) &&
                (!string.IsNullOrEmpty(BaslangicTarihiTxt.Text)))
            {
                foreach (ListItem item in BasSaatDDL.Items)
                {
                    if (!string.IsNullOrEmpty(item.Value))
                    {
                        DateTime baslangicTarihi = UtilityHelper.TariheSaatEkle(BaslangicTarihiTxt.Text.ConvertToDatetime(), item.Value);
                        Toplanti kayitliToplanti = new Toplanti();
                        kayitliToplanti = kayitliToplanti.SelectByBaslangicTarihi(ToplantiYeriDDL.SelectedItem.Value.ConvertToInt(), baslangicTarihi);

                        if ((kayitliToplanti != null) && (kayitliToplanti.ToplantiYeri != ProjeConstants.PARAM_DIGER_INT))
                        {
                            if (kayitliToplanti.Id != ToplantiIdQS.ConvertToInt()) //toplanti düzenlemesi yapılan toplantının saatlerini disable etmesin
                            //if (!toplanti.Id.ToString().Equals(ToplantiYeriDDL.SelectedItem.Value.ConvertToInt()))
                            {
                                item.Attributes.Add("class", "disabled-item");
                                item.Attributes["disabled"] = "disabled";
                            }
                        }
                    }
                }
            }
        }
        protected void BasSaatDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            BitisSaatiDDLDoldur();
            SetDisabledItemsBasSaatDDL();
        }
        private void BitisSaatiDDLDoldur()
        {
            BitSaatDDL.Items.Clear();
            if ((!string.IsNullOrEmpty(BasSaatDDL.SelectedItem.Text)) &&
                (!string.IsNullOrEmpty(ToplantiYeriDDL.SelectedItem.Text)))
            {
                string basSaatStr = BasSaatDDL.SelectedItem.Text;
                if (BitisTarihiTxt.Text.ConvertToDatetime() > BaslangicTarihiTxt.Text.ConvertToDatetime())
                {
                    basSaatStr = ProjeConstants.TOPLANTI_BASLAMAZAMANI.ToString();
                }
                TimeSpan bastarTS = basSaatStr.ConvertToTimeSpan();
                TimeSpan aralikTS = ProjeConstants.TOPLANTI_MINUMUMSURESI;
                TimeSpan bittarTS = ProjeConstants.TOPLANTI_BITISZAMANI;

                TimeSpan nextTS = bastarTS;
                //bos satır ekle
                ListItem li0 = new ListItem(string.Empty);
                BitSaatDDL.Items.Add(li0);
                do
                {
                    nextTS += aralikTS;
                    string bittarStr = string.Format("{0:00}:{1:00}", nextTS.Hours, nextTS.Minutes);
                    ListItem li = new ListItem(bittarStr);

                    if (ToplantiYeriDDL.SelectedItem.Value.ConvertToInt() != ProjeConstants.PARAM_DIGER_INT)
                    {
                        if (!string.IsNullOrEmpty(BitisTarihiTxt.Text))
                        {
                            DateTime bitisTarihi = UtilityHelper.TariheSaatEkle(BitisTarihiTxt.Text.ConvertToDatetime(), bittarStr);
                            Toplanti toplanti = new Toplanti();
                            toplanti = toplanti.SelectByBitisTarihi(ToplantiYeriDDL.SelectedItem.Value.ConvertToInt(), bitisTarihi);

                            if ((toplanti != null) && (toplanti.ToplantiYeri != ProjeConstants.PARAM_DIGER_INT))
                            {
                                if (!toplanti.Id.ToString().Equals(ToplantiIdQS)) //kendisi hariç toplantılar
                                {
                                    break;
                                }
                            }
                        }
                    }

                    BitSaatDDL.Items.Add(li);

                } while (nextTS < bittarTS);
                TimeSpan bittarValTS = bittarTS + aralikTS;
                string bittarVal = string.Format("{0:00}:{1:00}", bittarValTS.Hours, bittarValTS.Minutes);
                UtilityHelper.SetDDLValue(BitSaatDDL, bittarVal);
            }
        }
        protected void BaslangicTarihiTxt_TextChanged(object sender, EventArgs e)
        {
            BaslangicSaatiDDLDoldur();
        }
        protected void BitisTarihiTxt_TextChanged(object sender, EventArgs e)
        {
            BitisSaatiDDLDoldur();
        }
        private void KatilimcilariVeriTabaninaKaydet(int toplantiId)
        {
            if (toplantiId > 0)
            {
                foreach (var katilimciId in CikanKatilimciIdListQS)
                {
                    if (katilimciId > 0)
                    {
                        ToplantiKatilim silinecekToplantiKatilim = new ToplantiKatilim();

                        silinecekToplantiKatilim = silinecekToplantiKatilim.Select(katilimciId, toplantiId);
                        if (silinecekToplantiKatilim != null)
                        {
                            silinecekToplantiKatilim.Delete();
                        }
                    }
                }
                foreach (var katilimciId in KatilimciIdListQS)
                {
                    if (katilimciId > 0)
                    {
                        ToplantiKatilim toplantiKatilim = new ToplantiKatilim();

                        toplantiKatilim = toplantiKatilim.Select(katilimciId, toplantiId);
                        if (toplantiKatilim == null)
                        {
                            toplantiKatilim = new ToplantiKatilim
                            {
                                KatilimciId = katilimciId,
                                ToplantiId = toplantiId,
                                Bilgi = BilgiIdListQS.Contains(katilimciId)
                            };
                            toplantiKatilim.Save();
                        }
                        else
                        {
                            toplantiKatilim.Bilgi = BilgiIdListQS.Contains(katilimciId);
                            toplantiKatilim.Update();

                        }
                    }
                }
            }
        }
        private void EPostaIslemleri(Toplanti toplanti, string islemTipi, List<ToplantiKatilim> oncekiKatilimciListesi)
        {

            if (true)
            {
                MTSOrtak.ToplantiKatilimcilarinaEPostaGonder(toplanti, islemTipi, oncekiKatilimciListesi);
                if (islemTipi.Equals(ProjeConstants.KAYDET))
                {
                    if (toplanti.CevrimIci)
                        MTSOrtak.BilgiSistemMailGrubunaEPostaGonder(toplanti, islemTipi);
                    if (toplanti.IkramOnayi)
                        MTSOrtak.IkramMailGrubunaEPostaGonder(toplanti, islemTipi);
                }
                else if (islemTipi.Equals(ProjeConstants.GUNCELLE))
                {
                    MTSOrtak.ToplantidanCikanKatilimcilaraEPostaGonder(toplanti, CikanKatilimciIdListQS, oncekiKatilimciListesi);
                    if ((toplanti.CevrimIci) || (CevrimIciDegistiMi(toplanti)))
                        MTSOrtak.BilgiSistemMailGrubunaEPostaGonder(toplanti, islemTipi);
                    if ((toplanti.IkramOnayi) || (IkramOnayiDegistiMi(toplanti)))
                        MTSOrtak.IkramMailGrubunaEPostaGonder(toplanti, islemTipi);
                }
                else if (islemTipi.Equals(ProjeConstants.SIL))
                {
                    if (toplanti.CevrimIci)
                        MTSOrtak.BilgiSistemMailGrubunaEPostaGonder(toplanti, islemTipi);
                    if (toplanti.IkramOnayi)
                        MTSOrtak.IkramMailGrubunaEPostaGonder(toplanti, islemTipi);
                }
                MTSOrtak.ToplantiYetkilisineEPostaGonder(toplanti, islemTipi);
                MTSOrtak.ToplantiMailGrubunaEPostaGonder(toplanti, islemTipi);
            }
        }
        private bool CevrimIciDegistiMi(Toplanti toplanti)
        {
            return (toplanti?.CevrimIci != ToplantiIlkHaliQS.CevrimIci);
        }
        private bool IkramOnayiDegistiMi(Toplanti toplanti)
        {
            bool degistiMi = toplanti?.IkramOnayi != ToplantiIlkHaliQS.IkramOnayi || toplanti?.IkramMalzemesi != ToplantiIlkHaliQS.IkramMalzemesi;
            return degistiMi;
        }
        private bool ToplantiDegistiMi(Toplanti toplanti)
        {
            ToplantiKatilim tk = new ToplantiKatilim();
            List<ToplantiKatilim> list = tk.SelectBytoplantiId(toplanti.Id);
            List<int> katilimcilar = new List<int>();
            if (list.Count > 0)
            {
                katilimcilar = list.Select(x => x.KatilimciId).ToList();
            }
            bool katilimcilarAyniMi = (KatilimciIdListQS.All(katilimcilar.Contains)) && (KatilimciIdListQS.Count == katilimcilar.Count);
            //Bilgi değişti mi bunu da dikkate almalı
            List<int> bilgiListesi = new List<int>();
            if (list.Count > 0)
            {
                bilgiListesi = list.Where(result => result.Bilgi).Select(result => result.KatilimciId).ToList();
            }

            bool bilgiAyniMi = (BilgiIdListQS.All(bilgiListesi.Contains)) && (BilgiIdListQS.Count == bilgiListesi.Count);

            return !katilimcilarAyniMi || !bilgiAyniMi ||
                (toplanti?.BaslangicSaati != ToplantiIlkHaliQS.BaslangicSaati) ||
                (toplanti?.BaslangicTarihi != ToplantiIlkHaliQS.BaslangicTarihi) ||
                (toplanti?.BitisSaati != ToplantiIlkHaliQS.BitisSaati) ||
                (toplanti?.BitisTarihi != ToplantiIlkHaliQS.BitisTarihi) ||
                (toplanti?.CevrimIci != ToplantiIlkHaliQS.CevrimIci) ||
                (toplanti?.IkramOnayi != ToplantiIlkHaliQS.IkramOnayi) ||
                (toplanti?.IkramMalzemesi != ToplantiIlkHaliQS.IkramMalzemesi) ||
                (toplanti?.DisKatilimcilar != ToplantiIlkHaliQS.DisKatilimcilar) ||
                (toplanti?.Koordinator != ToplantiIlkHaliQS.Koordinator) ||
                (toplanti?.ToplantiKonusu != ToplantiIlkHaliQS.ToplantiKonusu) ||
                (toplanti?.ToplantiYeri != ToplantiIlkHaliQS.ToplantiYeri) ||
                (toplanti?.ToplantiYeriDiger != ToplantiIlkHaliQS.ToplantiYeriDiger);
        }
        private Personel PersonelGetir()
        {
            Personel personel = new Personel();

            if (!string.IsNullOrEmpty(PersonelIdQS))
            {
                personel = personel.Select<Personel>(PersonelIdQS.ConvertToInt());

            }
            else
            {
                string userName = CurrentUserName.Substring(CurrentUserName.LastIndexOf("\\") + 1, CurrentUserName.Length - CurrentUserName.LastIndexOf("\\") - 1);
                personel = personel.SelectByUserName(userName);
                PersonelIdQS = personel.Id.ToString();
            }

            return personel;
        }
        private int KoordinatorGetir()
        {
            int birim = 0;
            Personel personel = new Personel();
            personel = PersonelGetir();
            if (personel != null)
            {
                IsBilgileri ib = new IsBilgileri();
                ib = ib.SelectByPersonelId(personel.Id);

                if (ib != null)
                {
                    birim = ib.BirimId;
                }
            }
            return birim;
        }
        private void KaydetOnayPopupAc()
        {
            DateTime toplantiBaslangici = UtilityHelper.TariheSaatEkle(BaslangicTarihiTxt.Text.ConvertToDatetime(), BasSaatDDL.SelectedItem.Value);
            DateTime toplantiBitisi = UtilityHelper.TariheSaatEkle(BitisTarihiTxt.Text.ConvertToDatetime(), BitSaatDDL.SelectedItem.Value);
            if (toplantiBaslangici < DateTime.Now)
            {
                KaydetMesajBasligiLbl.Text = "Toplantı başlangıç saati geçti.";
                KaydetMesajiLbl.Text = "Toplantı başlangıç saati geçti. Kayıt işlemini iptal edip saati değiştirebilirsiniz. Değişiklik yapmadan kaydetmek istiyor musunuz?";
            }
            else
            {
                KaydetMesajBasligiLbl.Text = "Toplantı kaydedilecek";
                KaydetMesajiLbl.Text =string.Empty;

            }
            BaslamaTarihiCell.Text = BaslangicTarihiTxt.Text;
            BaslamaSaatiCell.Text = BasSaatDDL.SelectedItem.Text;
            BitisTarihiCell.Text = BitisTarihiTxt.Text;
            BitisSaatiCell.Text = BitSaatDDL.SelectedItem.Text;
            KonuCell.Text = ToplantiKonusuTxt.Text;
            YeriCell.Text = ToplantiYeriDDL.SelectedItem.Text;
            KaydetNowBtn.Visible = true;
            GuncelleNowBtn.Visible = false;
            string openModal = "OpenOnayModal();";
            UtilityHelper.ScriptCalistir(openModal);
        }
        private void GuncelleOnayPopupAc()
        {
            
            DateTime toplantiBaslangici = UtilityHelper.TariheSaatEkle(BaslangicTarihiTxt.Text.ConvertToDatetime(), BasSaatDDL.SelectedItem.Value);
            if (toplantiBaslangici < DateTime.Now)
            {
                KaydetMesajBasligiLbl.Text = "Toplantı başlangıç saati geçti.";
                KaydetMesajiLbl.Text = "Toplantı başlangıç saati geçti. Güncelleme işlemini iptal edip saati değiştirebilirsiniz. Değişiklik yapmadan güncellemek istiyor musunuz?";
            }
            else
            {
                KaydetMesajBasligiLbl.Text = "Toplantı güncellenecek";
                KaydetMesajiLbl.Text = string.Empty;

            }
            BaslamaTarihiCell.Text = BaslangicTarihiTxt.Text;
            BaslamaSaatiCell.Text = BasSaatDDL.SelectedItem.Text;
            BitisTarihiCell.Text = BitisTarihiTxt.Text;
            BitisSaatiCell.Text = BitSaatDDL.SelectedItem.Text;
            KonuCell.Text = ToplantiKonusuTxt.Text;
            YeriCell.Text = ToplantiYeriDDL.SelectedItem.Text;
            KaydetNowBtn.Visible = false;
            GuncelleNowBtn.Visible = true;
            string openModal = "OpenOnayModal();";
            UtilityHelper.ScriptCalistir(openModal);
        }
        protected void KaydetBtn_Click(object sender, EventArgs e)
        {
            KaydetOnayPopupAc();
        }
        protected void KaydetNowBtn_Click(object sender, EventArgs e)
        {
            
            int toplantiId = 0;
            try
            {
                Toplanti toplanti = new Toplanti();
                toplanti.ToplantiKonusu = ToplantiKonusuTxt.Text;
                toplanti.ToplantiYeri = ToplantiYeriDDL.SelectedValue.ConvertToInt();
                toplanti.ToplantiYeriDiger = ToplantiYeriDigerTxt.Value;
                toplanti.ToplantiYetkilisi = PersonelGetir().Id;
                toplanti.Koordinator = KoordinatorGetir();
                DateTime baslangictarihi = BaslangicTarihiTxt.Text.ConvertToDatetime();
                DateTime bitistarihi = BitisTarihiTxt.Text.ConvertToDatetime();
                string bassaat = BasSaatDDL.SelectedItem.Value;
                string bitsaat = BitSaatDDL.SelectedItem.Value;
                toplanti.BaslangicTarihi = UtilityHelper.TariheSaatEkle(baslangictarihi, bassaat);
                toplanti.BitisTarihi = UtilityHelper.TariheSaatEkle(bitistarihi, bitsaat);
                toplanti.BaslangicSaati = bassaat;
                toplanti.BitisSaati = bitsaat;
                toplanti.Aciklama = AciklamaTxt.Text;
                toplanti.DisKatilimcilar = DisKatilimcilarTxt.Text;
                toplanti.CevrimIci = CevrimIciTplantiChk.Checked;
                toplanti.IkramOnayi = IkramOnayiChk.Checked;
                toplanti.IkramMalzemesi = IkramMalzemesiTxt.Text;
                toplanti.UniqueId = Guid.NewGuid();
                toplanti.Olusturan = UtilityHelper.GetCurrentUserName();

                if (string.IsNullOrEmpty(ToplantiKonusuTxt.Text))
                {
                    MessageHelper.PublishMessage("Toplantı Konusu Boş Olamaz.", ProjeConstants.MESAJ_HATA, 2000);
                }
                else
                {
                    toplantiId = toplanti.Id = toplanti.Save();
                    if (toplantiId > 0)
                    {
                        ToplantiKatilim toplantiKatilim = new ToplantiKatilim();
                        List<ToplantiKatilim> oncekiKatilimciListesi = toplantiKatilim.SelectBytoplantiId(toplanti.Id);

                        KatilimcilariVeriTabaninaKaydet(toplantiId);

                        ToplantiIdQS = toplantiId.ToString();
                        KatilimciBilgileriDiv.Attributes["style"] = "display:block";
                        InitialDateQS = toplanti.BaslangicTarihi.ToString("yyyy-MM-dd");
                        EPostaIslemleri(toplanti, ProjeConstants.KAYDET, oncekiKatilimciListesi);


                        RedirectToPage(ProjeConstants.PAGE_TOPLANTI_LIST + "?SecilenToplantiId=" + toplanti.Id);
                    }
                    else
                    {
                        MessageHelper.PublishMessage("Toplantı Kaydedilemedi.", ProjeConstants.MESAJ_HATA);
                    }
                }
            }
            catch (Exception exception)
            {

                ExceptionHelper eh = new ExceptionHelper(exception);
                eh.PublishException();

            }
           
        }
       
        protected void GuncelleBtn_Click(object sender, EventArgs e)
        {
            GuncelleOnayPopupAc();
        }
        protected void GuncelleNowBtn_Click(object sender, EventArgs e)
        {
            DateTime tar1 = DateTime.Now;
            DateTime tar2 = DateTime.Now;
            DateTime tar3 = DateTime.Now;
            DateTime tar4 = DateTime.Now;
            DateTime tar5 = DateTime.Now;
            DateTime tar6 = DateTime.Now;
            DateTime tar7 = DateTime.Now;
            DateTime tar8 = DateTime.Now;
            bool guncellendiMi = false;
            try
            {
                Toplanti toplanti = new Toplanti();
                toplanti = toplanti.Select(ToplantiIdQS.ConvertToInt());
                toplanti.Degistiren = UtilityHelper.GetCurrentUserName();
                if (toplanti == null)
                {
                    MessageHelper.PublishMessage("Toplantı bulunamadı", ProjeConstants.MESAJ_HATA, 5000);
                }
                else
                {
                    toplanti.ToplantiKonusu = ToplantiKonusuTxt.Text;
                    toplanti.ToplantiYeri = ToplantiYeriDDL.SelectedValue.ConvertToInt();
                    toplanti.ToplantiYeriDiger = ToplantiYeriDigerTxt.Value;
                    toplanti.ToplantiYetkilisi = PersonelGetir().Id;
                    toplanti.Koordinator = KoordinatorGetir();
                    DateTime baslangictarihi = BaslangicTarihiTxt.Text.ConvertToDatetime();
                    DateTime bitistarihi = BitisTarihiTxt.Text.ConvertToDatetime();
                    string bassaat = BasSaatDDL.SelectedItem.Value;
                    string bitsaat = BitSaatDDL.SelectedItem.Value;
                    toplanti.BaslangicTarihi = UtilityHelper.TariheSaatEkle(baslangictarihi, bassaat);
                    toplanti.BitisTarihi = UtilityHelper.TariheSaatEkle(bitistarihi, bitsaat);
                    toplanti.BaslangicSaati = bassaat;
                    toplanti.BitisSaati = bitsaat;
                    toplanti.Aciklama = AciklamaTxt.Text;
                    toplanti.DisKatilimcilar = DisKatilimcilarTxt.Text;
                    toplanti.CevrimIci = CevrimIciTplantiChk.Checked;
                    toplanti.IkramOnayi = IkramOnayiChk.Checked;
                    toplanti.IkramMalzemesi = IkramMalzemesiTxt.Text;
                    toplanti.Degistiren = UtilityHelper.GetCurrentUserName();
                    if (string.IsNullOrEmpty(ToplantiKonusuTxt.Text))
                    {
                        MessageHelper.PublishMessage("Toplantı Konusu Boş Olamaz.", ProjeConstants.MESAJ_HATA, 2000);
                    }
                    else
                    {
                        tar2 = DateTime.Now;
                        if (ToplantiDegistiMi(toplanti))
                        {
                            tar3 = DateTime.Now;
                            guncellendiMi = toplanti.Update();
                            if (guncellendiMi)
                            {
                                tar4 = DateTime.Now;
                                InitialDateQS = toplanti.BaslangicTarihi.ToString("yyyy-MM-dd");
                                ToplantiKatilim toplantiKatilim = new ToplantiKatilim();
                                List<ToplantiKatilim> oncekiKatilimciListesi = toplantiKatilim.SelectBytoplantiId(toplanti.Id);
                                KatilimcilariVeriTabaninaKaydet(toplanti.Id);
                                tar5 = DateTime.Now;
                                MessageHelper.PublishMessage("Toplantı güncellendi, e-posta gönderildi.", ProjeConstants.MESAJ_BASARILI, 2000);
                                tar6 = DateTime.Now;
                                EPostaIslemleri(toplanti, ProjeConstants.GUNCELLE, oncekiKatilimciListesi);
                                tar7 = DateTime.Now;
                                ToplantiIlkHalineKopyala(toplanti);
                                tar8 = DateTime.Now;
                                CikanKatilimciIdListQS = new List<int>();
                                RedirectToPage(ProjeConstants.PAGE_TOPLANTI_LIST + "?SecilenToplantiId=" + toplanti.Id);
                            }
                            else
                            {
                                MessageHelper.PublishMessage("Toplantı güncellenemedi", ProjeConstants.MESAJ_HATA, 5000);

                            }
                        }
                        else
                        {
                            RedirectToPage(ProjeConstants.PAGE_TOPLANTI_LIST + "?SecilenToplantiId=" + toplanti.Id);
                            MessageHelper.PublishMessage("Toplantıda henüz değişiklik yapmadınız.", ProjeConstants.MESAJ_BILGI, 2000);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exhelper = new ExceptionHelper(ex);
                exhelper.Exceptions.Add(new Exception("Toplantı güncellenemedi."));
                exhelper.PublishException();
            }
            //MessageHelper.PublishMessage(
            //    "Tar1:" + tar1.ToString("HH:mm ss fff")+ System.Environment.NewLine +
            //    " Tar2:" + tar2.ToString("HH:mm ss fff")+ System.Environment.NewLine +
            //    " Tar3:" + tar3.ToString("HH:mm ss fff")+ System.Environment.NewLine +
            //    " Tar4:" + tar4.ToString("HH:mm ss fff")+ System.Environment.NewLine +
            //    " Tar5:" + tar5.ToString("HH:mm ss fff") + System.Environment.NewLine +
            //    " Tar6:" + tar6.ToString("HH:mm ss fff") + System.Environment.NewLine +
            //    " Tar7:" + tar7.ToString("HH:mm ss fff") + System.Environment.NewLine +
            //    " Tar8:" + tar8.ToString("HH:mm ss fff"), ProjeConstants.MESAJ_BILGI);
        }
        private void ToplantiSilPopupAc(object sender)
        {
            ParamVnLbl.Text = ToplantiIdQS;
            string openModal = "OpenSilModal();";
            SilMesajiLbl.Visible = true;
            SilMesajiLbl.Text = "Toplantıya ait tüm bilgiler silinecek ve toplantı katılımcılarına iptal e-postası gönderilecektir. </br>Silmek istediğinizden eminmisiniz?";
            SilModalBaslikLbl.Text = "Toplantı Silinecek";
            ToplantiSilNowBtn.Visible = true;
            UtilityHelper.ScriptCalistir(openModal);
        }
        protected void ToplantiSilBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Toplanti toplanti = new Toplanti();
                toplanti = toplanti.Select(ToplantiIdQS.ConvertToInt());
                if (toplanti != null)
                {
                    ToplantiSilPopupAc(sender);
                }
            }
            catch (Exception e1)
            {
                ExceptionHelper eh = new ExceptionHelper();
                Exception e2 = new Exception("Kişi silinemedi");
                eh.Exceptions.Add(e2);
                eh.Exceptions.Add(e1);
                eh.PublishException();
            }
        }
        protected void ToplantiSilNowBtn_Click(object sender, EventArgs e)
        {
            bool silindi = false;
            try
            {
                Toplanti toplanti = new Toplanti();
                toplanti = toplanti.Select(ToplantiIdQS.ConvertToInt());

                Toplanti kopyaToplanti = new Toplanti();
                UtilityHelper.CopyProperties(toplanti, kopyaToplanti);

                if (toplanti != null)
                {
                    silindi = toplanti.Delete();
                    if (silindi)
                    {
                        ToplantiKatilim toplantiKatilim = new ToplantiKatilim();
                        List<ToplantiKatilim> oncekiKatilimciListesi = toplantiKatilim.SelectBytoplantiId(toplanti.Id);
                        EPostaIslemleri(kopyaToplanti, ProjeConstants.SIL, oncekiKatilimciListesi);
                        toplantiKatilim = new ToplantiKatilim();
                        bool katilimSilindi = toplantiKatilim.DeleteByToplantiId(ToplantiIdQS.ConvertToInt());

                        RedirectToPage(ProjeConstants.PAGE_TOPLANTI_LIST);
                    }
                }
                if (!silindi)
                {
                    MessageHelper.PublishMessage("Toplantı Silinemedi", ProjeConstants.MESAJ_HATA);
                }
            }
            catch (Exception e1)
            {
                ExceptionHelper eh = new ExceptionHelper();
                Exception e2 = new Exception("Toplantı silinemedi");
                eh.Exceptions.Add(e2);
                eh.Exceptions.Add(e1);
                eh.PublishException();
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
        protected void KatilimciEkleBtn_Click(object sender, EventArgs e)
        {
            int katilimciId = paramToplantiKatilimciIdLbl.Value.ConvertToInt();
            bool bilgiMi = paramBilgiLbl.Value.ConvertToBool();
            if (katilimciId > 0)
            {
                if (!KatilimciIdListQS.Contains(katilimciId))
                    KatilimciIdListQS.Add(katilimciId);

                if (CikanKatilimciIdListQS.Contains(katilimciId))
                    CikanKatilimciIdListQS.Remove(katilimciId);

                if ((bilgiMi) && (!BilgiIdListQS.Contains(katilimciId)))
                    BilgiIdListQS.Add(katilimciId);
            }
            else
            {
                MessageHelper.PublishMessage("Katılımcı Bulunamadı", ProjeConstants.MESAJ_HATA);
            }

            KatilimciBilgileriniDoldur();
        }
        // Katılımcı Seçme ve Ekleme
        protected void KatilimciModalAcBtn_Click(object sender, EventArgs e)
        {
            TabloModalOlustur();
            UtilityHelper.ScriptCalistir("KatilimciSecimiModal();");
        }
        protected void KatilimciCikarBtn_Click(object sender, EventArgs e)
        {

            int katilimciId = paramToplantiKatilimciIdLbl.Value.ConvertToInt();
            if (katilimciId > 0)
            {
                if (KatilimciIdListQS.Contains(katilimciId))
                {
                    KatilimciIdListQS.Remove(katilimciId);
                }
                if (BilgiIdListQS.Contains(katilimciId))
                {
                    BilgiIdListQS.Remove(katilimciId);
                }
                if (!CikanKatilimciIdListQS.Contains(katilimciId))
                {
                    ToplantiKatilim toplantiKatilim = new ToplantiKatilim();
                    toplantiKatilim = toplantiKatilim.Select(katilimciId, ToplantiIdQS.ConvertToInt());
                    if (toplantiKatilim != null)
                        CikanKatilimciIdListQS.Add(katilimciId);
                }
            }
            else
            {
                MessageHelper.PublishMessage("Katılımcı Bulunamadı", ProjeConstants.MESAJ_HATA);
            }

            KatilimciBilgileriniDoldur();

        }
        private void TabloModalOlustur()
        {
            var jsonData = TabloModalJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateModalDataTable(jsonData); //javascript kodu hazırlanıyor.
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler,
                typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), jsString, true);
        }
        private string TabloModalJson()
        {
            string jSon = string.Empty;

            try
            {
                List<KatilimciListItem> list = GetModalDataList();
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
        private string CreateModalDataTable(string jsonData)
        {
            string tableString = @"
            if ( $.fn.DataTable.isDataTable('#CustomModalDataTable') ) {
              $('#CustomModalDataTable').DataTable().destroy();
            }
            $('#CustomModalDataTable tbody').empty();

            jQuery('#CustomModalDataTable').DataTable({
            data: " + jsonData + @",
            columns: [
                { data: 'SiraNo', 'width': '10%' },
                { data: 'ProtokolSiraNo'},
                { data: 'AdiSoyadi' },
                { data: 'KatilimciSec' },
                { data: 'BilgiSec' },
            ],
            'order': [[1, 'asc']],
            'language': {
                'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
            },
            responsive: true,
            dom: 'fpirt',
            'columnDefs': [
                { 'type': 'num', 'targets': 1 },
                {
                    'targets': [ 1 ],
                    'visible': false,
                    'searchable': false
                },
            ]
            });
            ";

            return tableString;
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
        private void TabloOlustur(bool veritabanindanMi)
        {
            var jsonData = TabloJson(veritabanindanMi); //veri çekilip json a çeviriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string CreateDataTable(string jsonData)
        {
            string tableString = @"jQuery('#CustomDataTable').DataTable({
            data: " + jsonData + @",
            columns: [
                { data: 'SiraNo', 'width': '10%' },
                { data: 'ProtokolSiraNo'},
                { data: 'AdiSoyadi' },
                { data: 'BilgiSec' },              
                { data: 'Cikar' },
            ],
            'order': [[1, 'asc']],
            'scrollY': '270px',
            'scrollCollapse': true,
            'paging': false,
            'language': {
                'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                'decimal': ',',
                'thousands': '.'
            },
            responsive: true,
            dom: 'srt',
            'columnDefs': [
                { 'type': 'num', 'targets': 0 },
                {
                    'targets': [ 1 ],
                    'visible': false,
                    'searchable': false
                },
            ]
            });
            ";

            return tableString;
        }
        private string TabloJson(bool veritabanindanMi)
        {
            string jSon = string.Empty;

            try
            {
                List<KatilimciListItem> list = GetDataList1();
                if (veritabanindanMi)
                {
                    list = GetDataList();
                }
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
        private List<KatilimciListItem> GetDataList()
        {
            Toplanti toplantiDao = new Toplanti();
            DataTable dataTable = toplantiDao.SelectAllByKatilimciToplantiReturnDataTable(ToplantiIdQS.ConvertToInt(), string.Empty);
            int SiraNo = 1;
            List<KatilimciListItem> list = new List<KatilimciListItem>();
            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    string toplantiId = row["ToplantiId"].ToString();
                    string katilimciId = row["KatilimciId"].ToString();
                    bool bilgi = row["Bilgi"].ConvertToBool();
                    int protokolsirano = row["ProtokolSiraNo"].ConvertToInt();
                    string adi = row["Adi"].ToString();
                    string soyadi = row["Soyadi"].ToString();


                    KatilimciListItem katilimciItem = new KatilimciListItem();
                    katilimciItem.SiraNo = SiraNo++.ToString();
                    katilimciItem.ProtokolSiraNo = protokolsirano;
                    katilimciItem.KatilimciId = katilimciId;
                    katilimciItem.BilgiSec = bilgi ? "Bilgi" : "Katılımcı";
                    katilimciItem.AdiSoyadi = adi + " " + soyadi;
                    katilimciItem.Cikar = "<a href='#' class='btn btn-outline-danger' onclick=KatilimciCikarBtnClick(" + katilimciId + ")>Çıkar</a>";

                    list.Add(katilimciItem);
                    KatilimciIdListQS.Add(katilimciItem.KatilimciId.ConvertToInt());
                    if (bilgi)
                        BilgiIdListQS.Add(katilimciItem.KatilimciId.ConvertToInt());
                }
            }
            return list;
        }
        private List<KatilimciListItem> GetDataList1()
        {
            int SiraNo = 1;
            List<KatilimciListItem> list = new List<KatilimciListItem>();
            foreach (int katilimciId in KatilimciIdListQS)
            {
                Personel personelDao = new Personel();

                DataTable dataTable = personelDao.SelectPersonelReturnDataTable(katilimciId);
                if (dataTable != null)
                {
                    string adiSoyadi = dataTable.Rows[0]["Adi"].ReturnEmptyIfNull().ToString() + " " + dataTable.Rows[0]["Soyadi"].ReturnEmptyIfNull().ToString();
                    int protokolSiraNo = dataTable.Rows[0]["ProtokolSiraNo"].ReturnZeroIfNull().ConvertToInt();

                    KatilimciListItem katilimciItem = new KatilimciListItem();
                    katilimciItem.SiraNo = SiraNo++.ToString();
                    katilimciItem.ProtokolSiraNo = protokolSiraNo;
                    katilimciItem.KatilimciId = katilimciId.ToString();
                    katilimciItem.AdiSoyadi = adiSoyadi;
                    if (BilgiIdListQS.Contains(katilimciId))
                        katilimciItem.BilgiSec = "Bilgi";
                    else
                        katilimciItem.BilgiSec = "Katılımcı";
                    //katilimciItem.ProtokolSiraNo = protokolsirano;
                    katilimciItem.Cikar = "<a href='#' class='btn btn-outline-danger' onclick=KatilimciCikarBtnClick(" + katilimciId + ")>Çıkar</a>";
                    list.Add(katilimciItem);
                }
            }

            return list;
        }
        private List<KatilimciListItem> GetModalDataList()
        {
            Personel personel = new Personel();
            DataTable dataTableModal = personel.SelectSecilmemisIcKatilimcilarByToplantiIdReturnDT(ToplantiIdQS.ConvertToInt());

            int SiraNo = 1;
            List<KatilimciListItem> list = new List<KatilimciListItem>();
            if (dataTableModal != null)
            {
                foreach (DataRow row in dataTableModal.Rows)
                {

                    int katilimciId = row["KatilimciId"].ConvertToInt();

                    if (KatilimciIdListQS.Contains(katilimciId.ConvertToInt())) //Zaten Eklenenler modal listeye gelmesin
                        continue;
                    else
                    {
                        string adi = row["Adi"].ToString();
                        string soyadi = row["Soyadi"].ToString();


                        KatilimciListItem katilimciItem = new KatilimciListItem();
                        katilimciItem.SiraNo = SiraNo++.ToString();
                        katilimciItem.KatilimciId = katilimciId.ToString();
                        katilimciItem.AdiSoyadi = (adi + " " + soyadi.Trim());

                        katilimciItem.KatilimciSec = "<a href='#' class='btn btn-outline-primary' onclick=KatilimciSecildiBtnClick($(this).parents('tr')," + katilimciId + "," + "false" + ");>Toplantıya Ekle</a>";
                        katilimciItem.BilgiSec = "<a href='#' class='btn btn-outline-secondary' onclick=KatilimciSecildiBtnClick($(this).parents('tr')," + katilimciId + "," + "true" + ");>Bilgi Ver</a>";


                        list.Add(katilimciItem);
                    }
                }
            }
            return list;
        }
        private class KatilimciListItem
        {
            public string SiraNo { get; set; }
            public string KatilimciId { get; set; }
            public string AdiSoyadi { get; set; }
            public string Cikar { get; set; }
            public string KatilimciSec { get; set; }
            public int ProtokolSiraNo { get; set; }
            public string BilgiSec { get; set; }
        }
        protected void ToplantiListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_TOPLANTI_LIST + "?SecilenToplantiId=" + ToplantiIdQS);
        }
        protected void ToplantiTakvimiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_TOPLANTI_TAKVIM + "?InitialDate=" + InitialDateQS);
        }
        protected void ToplantiYeriDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ToplantiYeriDDL.SelectedItem.Text.Equals(ProjeConstants.PARAM_DIGER))
            {
                ToplantiYeriDigerDiv.Attributes["style"] = "display:block";
            }
            else
            {
                ToplantiYeriDigerDiv.Attributes["style"] = "display:none";
                ToplantiYeriDigerTxt.Value = string.Empty;
            }

            BaslangicSaatiDDLDoldur();
            BitisSaatiDDLDoldur();

        }

        protected void ToplantiKatilimTutanagiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_TOPLANTI_KATILIMTUTANAGI + "?ToplantiId=" + ToplantiIdQS);
        }
    }
}
