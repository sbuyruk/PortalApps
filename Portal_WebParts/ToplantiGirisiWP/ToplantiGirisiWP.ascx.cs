using Portal_WebParts.Logging;
using Utility.Logging;
using Model.Services.Portal;
using Model.IKYS;
using Model.Services.IKYS;
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
        private PortalLogContext logContext;
        private PortalLogContext LogContext { get { return logContext ?? (logContext = PortalLogContext.FromCurrentRequest("Portal", "ToplantiGirisiWP")); } }
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
        private string MeetingVersion
        {
            get { return ViewState["MeetingVersion"] as string; }
            set { ViewState["MeetingVersion"] = value; }
        }
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
            PortalWebPartLog.ExecuteAction(LogContext, "Page_Load", () =>
            {
                if (!Page.IsPostBack) PortalWebPartLog.RecordOpened(LogContext);
                if (!Page.IsPostBack) // sayfa ilk kez açiliyorsa (bu sayfanin içindeki butona basilma ani hariç)
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
            });
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
            var loaded = new PortalToplantiWriteService(LogContext).Load(ToplantiIdQS.ConvertToInt());
            if (loaded == null) throw new PortalInputException("Toplantı bulunamadı. Lütfen listeyi yenileyin.");
            toplanti = loaded.After;
            MeetingVersion = loaded.Version;
            if (toplanti != null)
            {
                TitleLbl.Text = "Toplantı Düzenleme";
                TitleLbl.CssClass = "col-form-label text-primary fw-bold mb-1";
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
                KatilimciIdListQS = loaded.Participants.Select(p => p.KatilimciId).Distinct().ToList();
                BilgiIdListQS = loaded.Participants.Where(p => p.Bilgi).Select(p => p.KatilimciId).Distinct().ToList();

                BaslangicSaatiDDLDoldur();
                UtilityHelper.SetDDLValue(ToplantiYeriDDL, toplanti.ToplantiYeri.ToString());
                UtilityHelper.SetDDLValue(BasSaatDDL, toplanti.BaslangicSaati);
                BitisSaatiDDLDoldur();
                UtilityHelper.SetDDLValue(BitSaatDDL, toplanti.BitisSaati);
            }

        }

        private void KatilimciBilgileriniDoldur()
        {
            TabloOlustur();
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
                TitleLbl.CssClass = "col-form-label text-success fw-bold mb-1";
                BaslangicTarihiTxt.Text = DateTime.Today.ConvertToDatetimeEmptyIfNull();
                BitisTarihiTxt.Text = DateTime.Today.ConvertToDatetimeEmptyIfNull();
                BaslangicSaatiDDLDoldur();
                BitisSaatiDDLDoldur();

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
            throw new PortalInputException("Toplantı işlemleri için yetkilendirilmediğinizden dolayı işlem yapamazsınız.");
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
                List<ToplantiParametre> liste = new Model.Services.Portal.ToplantiParametreService(LogContext).GetByGroupValue(ProjeConstants.PARAM_TOPLANTIYETKILISI, adiSoyadi);
                yetkiliMi = liste.Count > 0 || new Model.Services.Portal.ToplantiParametreService(LogContext).GetByGroupValue(ProjeConstants.PARAM_TOPLANTIYONETICISI, adiSoyadi).Count > 0;
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
                    KatilimciBilgileriniDoldur();
                }
                else
                {
                    GirisiAc();
                    throw new PortalInputException("Toplantı bulunamadı. Yeni toplantı girebilirsiniz.");
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
            List<ToplantiParametre> list = new Model.Services.Portal.ToplantiParametreService(LogContext).GetByGroup(ProjeConstants.PARAM_TOPLANTIYERI);

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
            DateTime date;
            var culture = System.Globalization.CultureInfo.GetCultureInfo("tr-TR");
            if (!DateTime.TryParse(BaslangicTarihiTxt.Text, culture, System.Globalization.DateTimeStyles.None, out date)
                || !DateTime.TryParse(BitisTarihiTxt.Text, culture, System.Globalization.DateTimeStyles.None, out date))
                throw new PortalInputException("Geçerli başlangıç ve bitiş tarihlerini girin.");
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
                //bos satir ekle
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
                        kayitliToplanti = new Model.Services.Portal.ToplantiService(LogContext).GetByStart(ToplantiYeriDDL.SelectedItem.Value.ConvertToInt(), baslangicTarihi);

                        if ((kayitliToplanti != null) && (kayitliToplanti.ToplantiYeri != ProjeConstants.PARAM_DIGER_INT))
                        {
                            if (kayitliToplanti.Id != ToplantiIdQS.ConvertToInt()) //toplanti düzenlemesi yapilan toplantinin saatlerini disable etmesin
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
            PortalWebPartLog.ExecuteAction(LogContext, "BasSaatDDL_SelectedIndexChanged", () =>
            {
                BitisSaatiDDLDoldur();
                SetDisabledItemsBasSaatDDL();
            });
        }
        private void BitisSaatiDDLDoldur()
        {
            DateTime date;
            var culture = System.Globalization.CultureInfo.GetCultureInfo("tr-TR");
            if (!DateTime.TryParse(BaslangicTarihiTxt.Text, culture, System.Globalization.DateTimeStyles.None, out date)
                || !DateTime.TryParse(BitisTarihiTxt.Text, culture, System.Globalization.DateTimeStyles.None, out date))
                throw new PortalInputException("Geçerli başlangıç ve bitiş tarihlerini girin.");
            BitSaatDDL.Items.Clear();
            if (BasSaatDDL.SelectedItem != null && ToplantiYeriDDL.SelectedItem != null && (!string.IsNullOrEmpty(BasSaatDDL.SelectedItem.Text)) &&
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
                //bos satir ekle
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
                        toplanti = new Model.Services.Portal.ToplantiService(LogContext).GetByEnd(ToplantiYeriDDL.SelectedItem.Value.ConvertToInt(), bitisTarihi);

                            if ((toplanti != null) && (toplanti.ToplantiYeri != ProjeConstants.PARAM_DIGER_INT))
                            {
                                if (!toplanti.Id.ToString().Equals(ToplantiIdQS)) //kendisi hariç toplantilar
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
            PortalWebPartLog.ExecuteAction(LogContext, "BaslangicTarihiTxt_TextChanged", () =>
            {
                BaslangicSaatiDDLDoldur();
            });
        }
        protected void BitisTarihiTxt_TextChanged(object sender, EventArgs e)
        {
            PortalWebPartLog.ExecuteAction(LogContext, "BitisTarihiTxt_TextChanged", () =>
            {
                BitisSaatiDDLDoldur();
            });
        }

        private Personel PersonelGetir()
        {
            string login = LogContext.UserLogin ?? string.Empty;
            var person = new PersonelService().GetByUserName(login.Substring(login.LastIndexOf('\\') + 1), LogContext);
            if (person == null) throw new PortalInputException("Personel kaydınız bulunamadı. Lütfen sistem yöneticisine başvurun.");
            return person;
        }

        private void KaydetOnayPopupAc()
        {
            if (object.Equals(ViewState["MeetingCompleted"], true))
                throw new PortalInputException("İşlem tamamlandı. Toplantı listesinden devam edin.");
            var input = ReadMeetingInput(false);
            DateTime toplantiBaslangici = input.BaslangicTarihi;
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
            KonuCell.Text = System.Web.HttpUtility.HtmlEncode(ToplantiKonusuTxt.Text);
            YeriCell.Text = ToplantiYeriDDL.SelectedItem.Text;
            KaydetNowBtn.Visible = true;
            GuncelleNowBtn.Visible = false;
            string openModal = "OpenOnayModal();";
            UtilityHelper.ScriptCalistir(openModal);
        }
        private void GuncelleOnayPopupAc()
        {
            if (object.Equals(ViewState["MeetingCompleted"], true))
                throw new PortalInputException("İşlem tamamlandı. Toplantı listesinden devam edin.");
            var input = ReadMeetingInput(true);
            DateTime toplantiBaslangici = input.BaslangicTarihi;
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
            KonuCell.Text = System.Web.HttpUtility.HtmlEncode(ToplantiKonusuTxt.Text);
            YeriCell.Text = ToplantiYeriDDL.SelectedItem.Text;
            KaydetNowBtn.Visible = false;
            GuncelleNowBtn.Visible = true;
            string openModal = "OpenOnayModal();";
            UtilityHelper.ScriptCalistir(openModal);
        }
        protected void KaydetBtn_Click(object sender, EventArgs e)
        {
            PortalWebPartLog.ExecuteAction(LogContext, "KaydetBtn_Click", () =>
            {
                KaydetOnayPopupAc();
            });
        }

        private Toplanti ReadMeetingInput(bool update)
        {
            if (string.IsNullOrWhiteSpace(ToplantiKonusuTxt.Text)) throw new PortalInputException("Toplantı konusunu girin.");
            DateTime start, end;
            TimeSpan startTime, endTime;
            var culture = System.Globalization.CultureInfo.GetCultureInfo("tr-TR");
            if (!DateTime.TryParse(BaslangicTarihiTxt.Text, culture, System.Globalization.DateTimeStyles.None, out start)
                || !DateTime.TryParse(BitisTarihiTxt.Text, culture, System.Globalization.DateTimeStyles.None, out end)
                || !TimeSpan.TryParse(BasSaatDDL.SelectedValue, out startTime) || !TimeSpan.TryParse(BitSaatDDL.SelectedValue, out endTime)
                || startTime < TimeSpan.Zero || startTime >= TimeSpan.FromDays(1) || endTime < TimeSpan.Zero || endTime >= TimeSpan.FromDays(1))
                throw new PortalInputException("Geçerli başlangıç ve bitiş tarihi ile saatini seçin.");
            start = start.Date.Add(startTime); end = end.Date.Add(endTime);
            if (end <= start) throw new PortalInputException("Bitiş zamanı başlangıçtan sonra olmalıdır.");
            int place;
            if (!int.TryParse(ToplantiYeriDDL.SelectedValue, out place) || place <= 0) throw new PortalInputException("Toplantı yerini seçin.");
            if (place == ProjeConstants.PARAM_DIGER_INT && string.IsNullOrWhiteSpace(ToplantiYeriDigerTxt.Value))
                throw new PortalInputException("Diğer toplantı yerini belirtin.");
            if (update && ToplantiIdQS.ConvertToInt() <= 0) throw new PortalInputException("Güncellenecek toplantıyı seçin.");
            return new Toplanti { Id = update ? ToplantiIdQS.ConvertToInt() : 0,
                ToplantiKonusu = ToplantiKonusuTxt.Text.Trim(), ToplantiYeri = place, ToplantiYeriDiger = ToplantiYeriDigerTxt.Value,
                BaslangicTarihi = start, BitisTarihi = end, BaslangicSaati = BasSaatDDL.SelectedValue, BitisSaati = BitSaatDDL.SelectedValue,
                Aciklama = AciklamaTxt.Text, DisKatilimcilar = DisKatilimcilarTxt.Text, CevrimIci = CevrimIciTplantiChk.Checked,
                IkramOnayi = IkramOnayiChk.Checked, IkramMalzemesi = IkramMalzemesiTxt.Text };
        }
        private void SaveMeeting(bool update)
        {
            if (object.Equals(ViewState["MeetingCompleted"], true))
                throw new PortalInputException("İşlem zaten tamamlandı. Yeni işlem için toplantı listesini açın.");
            Toplanti input = ReadMeetingInput(update);
            PortalToplantiChange change;
            PortalToplantiResult result;
            try
            {
                result = new PortalToplantiWriteService(LogContext).Save(input, KatilimciIdListQS,
                    BilgiIdListQS, MeetingVersion, out change);
            }
            catch (Exception error)
            {
                PortalWebPartLog.ReportError(LogContext, "Portal.Toplanti.Save", error,
                    "İşlemin sonucu doğrulanamadı. Tekrar kaydetmeden önce toplantı listesini kontrol edin.");
                return;
            }
            if (result == PortalToplantiResult.Unchanged)
            {
                Messages.Info("Toplantıda değişiklik yapılmadı.");
                return;
            }
            RequireCompleted(result);
            FinishMeeting(change, update ? "Toplantı güncellendi." : "Toplantı kaydedildi.");
        }
        private void FinishMeeting(PortalToplantiChange change, string message)
        {
            // The database commit has completed. Disable repeat submits before SMTP or UI work.
            ViewState["MeetingCompleted"] = true;
            ToplantiIdQS = (change.After ?? change.Before).Id.ToString();
            KaydetBtn.Visible = GuncelleBtn.Visible = ToplantiSilBtn.Visible = false;
            KaydetNowBtn.Visible = GuncelleNowBtn.Visible = ToplantiSilNowBtn.Visible = false;
            try
            {
                UtilityHelper.ScriptCalistir("['ModalOnayDiv','ModalSilDiv'].forEach(function(id){var element=document.getElementById(id);if(element){var modal=bootstrap.Modal.getInstance(element);if(modal)modal.hide();}});");
                if (new PortalToplantiNotificationService(LogContext).Send(change))
                    Messages.Success(message + " E-posta bildirimleri gönderildi. Toplantı listesinden devam edebilirsiniz.");
                else
                    Messages.Warning(message + " Bazı e-posta bildirimleri gönderilemedi. İşlemi tekrar kaydetmeyin; sistem yöneticisine başvurun. Başvuru kodu: " + LogContext.OperationId.ToString("N"));
            }
            catch (Exception error)
            {
                PortalWebPartLog.ReportError(LogContext, "Portal.Toplanti.AfterCommit", error,
                    message + " Bildirimler tamamlanamadı. İşlemi tekrar kaydetmeyin; sistem yöneticisine başvurun.");
            }
        }
        private static void RequireCompleted(PortalToplantiResult result)
        {
            if (result == PortalToplantiResult.Completed) return;
            switch (result)
            {
                case PortalToplantiResult.NotFound: throw new PortalInputException("Toplantı bulunamadı. Lütfen listeyi yenileyin.");
                case PortalToplantiResult.Forbidden: throw new PortalInputException("Bu toplantı için işlem yetkiniz bulunmuyor.");
                case PortalToplantiResult.Changed: throw new PortalInputException("Toplantı başka bir işlemde değiştirildi. Listeyi yenileyip tekrar açın.");
                case PortalToplantiResult.Overlap: throw new PortalInputException("Seçilen yer ve saatlerde başka bir toplantı var. Lütfen yer veya zamanı değiştirin.");
                default: throw new PortalInputException("Toplantı yeri, tarihleri ve katılımcıları kontrol edin.");
            }
        }

        protected void KaydetNowBtn_Click(object sender, EventArgs e)
        {
            PortalWebPartLog.ExecuteAction(LogContext, "KaydetNowBtn_Click", () =>
            {
                SaveMeeting(false);
            });
        }

        protected void GuncelleBtn_Click(object sender, EventArgs e)
        {
            PortalWebPartLog.ExecuteAction(LogContext, "GuncelleBtn_Click", () =>
            {
                GuncelleOnayPopupAc();
            });
        }
        protected void GuncelleNowBtn_Click(object sender, EventArgs e)
        {
            PortalWebPartLog.ExecuteAction(LogContext, "GuncelleNowBtn_Click", () =>
            {
                SaveMeeting(true);
            });
        }
        private void ToplantiSilPopupAc(object sender)
        {
            if (object.Equals(ViewState["MeetingCompleted"], true))
                throw new PortalInputException("İşlem tamamlandı. Toplantı listesinden devam edin.");
            ParamVnLbl.Text = ToplantiIdQS;
            string openModal = "OpenSilModal();";
            SilMesajiLbl.Visible = true;
            SilMesajiLbl.Text = "Toplantı ve katılımcı kayıtları silinecek. Ardından iptal bildirimleri gönderilecek. </br>Silmek istediğinizden emin misiniz?";
            SilModalBaslikLbl.Text = "Toplantı Silinecek";
            ToplantiSilNowBtn.Visible = true;
            UtilityHelper.ScriptCalistir(openModal);
        }
        protected void ToplantiSilBtn_Click(object sender, EventArgs e)
        {
            PortalWebPartLog.ExecuteAction(LogContext, "ToplantiSilBtn_Click", () =>
            {
                if (ToplantiIdQS.ConvertToInt() <= 0) throw new PortalInputException("Silinecek toplantıyı seçin.");
                ToplantiSilPopupAc(sender);
            });
        }
        protected void ToplantiSilNowBtn_Click(object sender, EventArgs e)
        {
            PortalWebPartLog.ExecuteAction(LogContext, "ToplantiSilNowBtn_Click", () =>
            {
                PortalToplantiChange change;
                PortalToplantiResult result;
                try
                {
                    result = new PortalToplantiWriteService(LogContext).Delete(ToplantiIdQS.ConvertToInt(), MeetingVersion, out change);
                }
                catch (Exception error)
                {
                    PortalWebPartLog.ReportError(LogContext, "Portal.Toplanti.Delete", error,
                        "Silme işleminin sonucu doğrulanamadı. Tekrar işlem yapmadan önce toplantı listesini kontrol edin.");
                    return;
                }
                RequireCompleted(result);
                FinishMeeting(change, "Toplantı silindi.");
            });
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            PortalWebPartLog.ExecuteAction(LogContext, "CloseBtn_Click", () =>
            {
                            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
                Page.Response.Redirect(newUrl);
            });
        }
        protected void KatilimciEkleBtn_Click(object sender, EventArgs e)
        {
            PortalWebPartLog.ExecuteAction(LogContext, "KatilimciEkleBtn_Click", () =>
            {
                int katilimciId = paramToplantiKatilimciIdLbl.Value.ConvertToInt();
                bool bilgiMi = paramBilgiLbl.Value.ConvertToBool();
                if (katilimciId > 0)
                {
                    if (!KatilimciIdListQS.Contains(katilimciId))
                        KatilimciIdListQS.Add(katilimciId);

                    if (CikanKatilimciIdListQS.Contains(katilimciId))
                        CikanKatilimciIdListQS.Remove(katilimciId);

                    if (!bilgiMi) BilgiIdListQS.Remove(katilimciId);
                    if ((bilgiMi) && (!BilgiIdListQS.Contains(katilimciId)))
                        BilgiIdListQS.Add(katilimciId);
                }
                else
                {
                    throw new PortalInputException("Katılımcı bulunamadı. Lütfen yeniden seçim yapın.");
                }

                KatilimciBilgileriniDoldur();
            });
        }
        // Katılımcı Seçme ve Ekleme
        protected void KatilimciModalAcBtn_Click(object sender, EventArgs e)
        {
            PortalWebPartLog.ExecuteAction(LogContext, "KatilimciModalAcBtn_Click", () =>
            {
                TabloModalOlustur();
                UtilityHelper.ScriptCalistir("KatilimciSecimiModal();");
            });
        }
        protected void KatilimciCikarBtn_Click(object sender, EventArgs e)
        {
            PortalWebPartLog.ExecuteAction(LogContext, "KatilimciCikarBtn_Click", () =>
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
                        toplantiKatilim = new Model.Services.Portal.ToplantiKatilimService(LogContext).GetByParticipantMeeting(katilimciId, ToplantiIdQS.ConvertToInt());
                        if (toplantiKatilim != null)
                            CikanKatilimciIdListQS.Add(katilimciId);
                    }
                }
                else
                {
                    throw new PortalInputException("Katılımcı bulunamadı. Lütfen yeniden seçim yapın.");
                }

                KatilimciBilgileriniDoldur();
            });
        }
        private void TabloModalOlustur()
        {
            var jsonData = TabloModalJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateModalDataTable(jsonData); //javascript kodu hazirlaniyor.
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler,
                typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), jsString, true);
        }
        private string TabloModalJson()
        {
            string jSon = string.Empty;

            List<KatilimciListItem> list = GetModalDataList();
            var serializer = new JavaScriptSerializer();
            jSon = serializer.Serialize(list).Replace("<", "\\u003c").Replace(">", "\\u003e").Replace("&", "\\u0026");

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
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + pageUrl;
            Page.Response.Redirect(newUrl);

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
        private string TabloJson()
        {
            string jSon = string.Empty;

            List<KatilimciListItem> list = GetDataList1();
            var serializer = new JavaScriptSerializer();
            jSon = serializer.Serialize(list).Replace("<", "\\u003c").Replace(">", "\\u003e").Replace("&", "\\u0026");

            return jSon;
        }

        private List<KatilimciListItem> GetDataList1()
        {
            int SiraNo = 1;
            List<KatilimciListItem> list = new List<KatilimciListItem>();
            foreach (int katilimciId in KatilimciIdListQS)
            {
                Personel personelDao = new Personel();

            DataTable dataTable = new PersonelService().GetPersonelReturnDataTable(katilimciId, LogContext);
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
            DataTable dataTableModal = new PersonelService().GetUnselectedMeetingParticipants(ToplantiIdQS.ConvertToInt(), LogContext);

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
            PortalWebPartLog.ExecuteAction(LogContext, "ToplantiListesiBtn_Click", () =>
            {
                RedirectToPage(ProjeConstants.PAGE_TOPLANTI_LIST + "?SecilenToplantiId=" + ToplantiIdQS);
            });
        }
        protected void ToplantiTakvimiBtn_Click(object sender, EventArgs e)
        {
            PortalWebPartLog.ExecuteAction(LogContext, "ToplantiTakvimiBtn_Click", () =>
            {
                RedirectToPage(ProjeConstants.PAGE_TOPLANTI_TAKVIM + "?InitialDate=" + InitialDateQS);
            });
        }
        protected void ToplantiYeriDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            PortalWebPartLog.ExecuteAction(LogContext, "ToplantiYeriDDL_SelectedIndexChanged", () =>
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
            });
        }

        protected void ToplantiKatilimTutanagiBtn_Click(object sender, EventArgs e)
        {
            PortalWebPartLog.ExecuteAction(LogContext, "ToplantiKatilimTutanagiBtn_Click", () =>
            {
                RedirectToPage(ProjeConstants.PAGE_TOPLANTI_KATILIMTUTANAGI + "?ToplantiId=" + ToplantiIdQS);
            });
        }
    }
}
