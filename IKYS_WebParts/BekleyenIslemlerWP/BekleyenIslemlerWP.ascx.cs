using Model.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace IKYS_WebParts.BekleyenIslemlerWP
{
    [ToolboxItemAttribute(false)]
    public partial class BekleyenIslemlerWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public BekleyenIslemlerWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
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
        }
        private string AmirBirimIdList
        {
            get
            {
                if (ViewState["AmirBirimIdList"] == null)
                {
                    ViewState["AmirBirimIdList"] = string.Empty;
                }
                return ViewState["AmirBirimIdList"].ToString();
            }
            set
            {
                ViewState["AmirBirimIdList"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    //publish etmeden önce Burayı değiştirmeyi unutma
                    Personel personel = new Personel();//PersonelGetir();
                    personel = personel.Select(1192);
                    if (personel != null && personel.Id > 0)
                    {
                        BirimTanim birimTanim = new BirimTanim();
                        List<BirimTanim> amirOlduguBirimler = birimTanim.SelectByAmirId(personel.Id);
                        if (amirOlduguBirimler != null && amirOlduguBirimler.Count > 0)
                        {
                            AmirBirimIdList = string.Join(",", amirOlduguBirimler.Select(b => b.Id));
                            TabloOlustur();
                        }
                        else
                        {
                            MesajGoster();
                        }
                    }
                    else
                    {
                        MesajGoster();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private void MesajGoster()
        {
            TabloDiv.Style["display"] = "none";
            MesajDiv.Style["display"] = "block";
        }
        private Personel PersonelGetir()
        {
            Personel personel = new Personel();
            string userName = CurrentUserName.Substring(CurrentUserName.LastIndexOf("\\") + 1, CurrentUserName.Length - CurrentUserName.LastIndexOf("\\") - 1);
            personel = personel.SelectByUserName(userName);
            return personel;
        }
        private void TabloOlustur()
        {
            var jsonData = TabloJson();
            var jsString = CreateDataTable(jsonData);
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string TabloJson()
        {
            string jSon = string.Empty;
            try
            {
                List<BekleyenIslemListItem> list = GetDataList();
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
        private List<BekleyenIslemListItem> GetDataList()
        {
            GorevOnay gorevOnay = new GorevOnay();
            DataTable dataTable = gorevOnay.SelectBekleyenAmirOnayiByBirimIdsReturnDataTable(AmirBirimIdList);

            List<BekleyenIslemListItem> list = new List<BekleyenIslemListItem>();

            if (dataTable == null) return list;

            foreach (DataRow row in dataTable.Rows)
            {
                int gorevOnayId = row["GorevOnayId"].ConvertToInt();

                BekleyenIslemListItem item = new BekleyenIslemListItem();
                item.GorevOnayId = gorevOnayId.ToString();
                item.AdiSoyadi = row["AdiSoyadi"].ToString();
                item.BaslangicTarihi = row["BaslangicTarihi"].ConvertToDatetime().ToString("dd.MM.yyyy");
                item.BitisTarihi = row["BitisTarihi"].ConvertToDatetime().ToString("dd.MM.yyyy");
                item.GorevinSebebi = row["GorevinSebebi"].ToString();
                item.GorevinYeri = row["GorevinYeri"].ToString();
                item.UlasimAraci = row["UlasimAraci"].ToString();
                item.Transfer = row["Transfer"].ToString();
                item.Konaklama = row["Konaklama"].ToString();
                item.Aciklama = row["Aciklama"].ToString();
                item.AmirOnayi = row["AmirOnayi"].ReturnZeroIfNull().ConvertToInt();
                item.OnayRedAciklama = row["OnayRedAciklama"].ReturnEmptyIfNull().ToString();

                if (item.AmirOnayi == (int)GorevOnay.AmirOnayDurumu.OnayBekliyor)
                    item.AmirOnayiSiraNo = 0;
                //else if (item.AmirOnayi == (int)GorevOnay.AmirOnayDurumu.Reddedildi)
                //    item.AmirOnayiSiraNo = 1;
                //else if (item.AmirOnayi == (int)GorevOnay.AmirOnayDurumu.Onaylandi)
                //    item.AmirOnayiSiraNo = 2;
                else
                    item.AmirOnayiSiraNo = 3;
                if (item.OdendiMi)
                {
                    item.Onayla = "<span class='text-success'>Ödendi</span>";
                    item.Reddet = string.Empty;
                }
                else if (item.AmirOnayi == (int)GorevOnay.AmirOnayDurumu.Onaylandi)
                {
                    item.Onayla = UtilityHelper.GetEnumDisplayName(GorevOnay.AmirOnayDurumu.Onaylandi);
                    item.Reddet =string.Empty;
                }
                else if (item.AmirOnayi == (int)GorevOnay.AmirOnayDurumu.Reddedildi)
                {
                    item.Onayla = UtilityHelper.GetEnumDisplayName(GorevOnay.AmirOnayDurumu.Reddedildi);
                    item.Reddet = System.Environment.NewLine + item.OnayRedAciklama;
                }
                else
                {
                    item.Onayla = "<a href='javascript:OpenOnayla(" + gorevOnayId + ");' class='btn btn-outline-primary'>Onayla</a>";
                    item.Reddet = "<a href='javascript:OpenReddet(" + gorevOnayId + ");' class='btn btn-outline-danger'>Reddet</a>";
                }
                item.Incele = "<a href='javascript:OpenIncele(" + gorevOnayId + ");' class='btn btn-outline-secondary'>İncele</a>";

                list.Add(item);
            }
            return list;
        }
        private class BekleyenIslemListItem
        {
            public string GorevOnayId { get; set; }
            public string AdiSoyadi { get; set; }
            public string BaslangicTarihi { get; set; }
            public string BitisTarihi { get; set; }
            public string GorevinSebebi { get; set; }
            public string GorevinYeri { get; set; }
            public string UlasimAraci { get; set; }
            public string Transfer { get; set; }
            public string Konaklama { get; set; }
            public string Aciklama { get; set; }
            public string Onayla { get; set; }
            public string Reddet { get; set; }
            public string Incele { get; set; }
            public int AmirOnayi { get; set; }
            public int AmirOnayiSiraNo { get; set; }
            public string OnayRedAciklama { get; set; }
            public bool OdendiMi { get; set; } = false;

        }
        private string CreateDataTable(string jsonData)
        {
            string tableString = @"
                if ( jQuery.fn.DataTable.isDataTable('#CustomDataTable') ) {
                    jQuery('#CustomDataTable').DataTable().destroy();
                }
                jQuery('#CustomDataTable tbody').empty();

                jQuery.fn.dataTable.moment('DD.MM.YYYY');//sort date
                jQuery('#CustomDataTable').DataTable({
                    data: " + jsonData + @",
                    columns: [
                        { data: 'AdiSoyadi' },
                        { data: 'BaslangicTarihi' },
                        { data: 'BitisTarihi' },
                        { data: 'GorevinSebebi' },
                        { data: 'GorevinYeri' },
                        { data: 'UlasimAraci' },
                        { data: 'Transfer' },
                        { data: 'Konaklama' },
                        { data: 'Aciklama' },
                        { data: 'Onayla' },
                        { data: 'Reddet' },
                        { data: 'Incele' },
                        { data: 'AmirOnayi', visible: false },
                        { data: 'AmirOnayiSiraNo', visible: false },
                    ],
                    columnDefs: [
                        { type: 'turkish', targets: [0,3,4,5] }
                    ],
                    'order': [[13, 'asc'], [1, 'desc']],//once AmirOnayi oncelik sirasi, sonra tarih desc
                    'createdRow': function (row, data, dataIndex) {
                        if (data.AmirOnayi == 1) {
                            jQuery(row).addClass('table-success');
                        }
                        else if (data.AmirOnayi == 2) {
                            jQuery(row).addClass('table-danger');
                        }
                    },
                    'language': {
                        'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                        'decimal': ',',
                        'thousands': '.'
                    }
                });
            ";

            return tableString;
        }
        protected void ModalOnaylaBtn_Click(object sender, EventArgs e)
        {
            try
            {
                GorevOnay gorevOnay = new GorevOnay();
                gorevOnay = gorevOnay.Select(paramGorevOnayIdLbl.Value.ConvertToInt());
                if (gorevOnay != null)
                {
                    Personel personel = new Personel();
                    personel = personel.Select<Personel>(gorevOnay.PersonelId);
                    OnayLbl.Text = personel.Adi + " " + personel.Soyadi + " için "
                        + gorevOnay.BaslangicTarihi.ToString("dd.MM.yyyy") + " - " + gorevOnay.BitisTarihi.ToString("dd.MM.yyyy")
                        + " tarihleri arasındaki görevi onaylamak istediğinize emin misiniz?";
                }
                TabloOlustur();
                UtilityHelper.ScriptCalistir("OpenModalOnay();");
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void ModalReddetBtn_Click(object sender, EventArgs e)
        {
            try
            {
                GorevOnay gorevOnay = new GorevOnay();
                gorevOnay = gorevOnay.Select(paramGorevOnayIdLbl.Value.ConvertToInt());
                if (gorevOnay != null)
                {
                    Personel personel = new Personel();
                    personel = personel.Select<Personel>(gorevOnay.PersonelId);
                    ReddetLbl.Text = personel.Adi + " " + personel.Soyadi + " için "
                        + gorevOnay.BaslangicTarihi.ToString("dd.MM.yyyy") + " - " + gorevOnay.BitisTarihi.ToString("dd.MM.yyyy")
                        + " tarihleri arasındaki görevi reddetmek istediğinize emin misiniz?";
                }
                TabloOlustur();
                UtilityHelper.ScriptCalistir("OpenModalReddet();");
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void ModalInceleBtn_Click(object sender, EventArgs e)
        {
            try
            {
                GorevOnay gorevOnay = new GorevOnay();
                gorevOnay = gorevOnay.Select(paramGorevOnayIdLbl.Value.ConvertToInt());
                if (gorevOnay != null)
                {
                    Personel personel = new Personel();
                    personel = personel.Select<Personel>(gorevOnay.PersonelId);

                    GorevInfoTable.Controls.Clear();

                    AddGorevInfoRow("Adı Soyadı", personel.Adi + " " + personel.Soyadi);
                    AddGorevInfoRow("Başlangıç Tarihi", gorevOnay.BaslangicTarihi.ToString("dd.MM.yyyy"));
                    AddGorevInfoRow("Bitiş Tarihi", gorevOnay.BitisTarihi.ToString("dd.MM.yyyy"));
                    AddGorevInfoRow("Süre", gorevOnay.Sure.ToString());
                    AddGorevInfoRow("Görevin Sebebi", gorevOnay.GorevinSebebi);
                    AddGorevInfoRow("Görevin Yeri", gorevOnay.GorevinYeri);
                    AddGorevInfoRow("Ulaşım Aracı", gorevOnay.UlasimAraci);
                    AddGorevInfoRow("Transfer", gorevOnay.Transfer);
                    AddGorevInfoRow("Konaklama", gorevOnay.Konaklama);
                    AddGorevInfoRow("Amir Onayı", UtilityHelper.GetEnumDisplayName((GorevOnay.AmirOnayDurumu)gorevOnay.AmirOnayi));
                    AddGorevInfoRow("Onay/Red Açıklama", gorevOnay.OnayRedAciklama);
                    AddGorevInfoRow("Açıklama", gorevOnay.Aciklama);
                }
                TabloOlustur();
                UtilityHelper.ScriptCalistir("OpenModalIncele();");
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }

        private void AddGorevInfoRow(string baslik, string deger)
        {
            HtmlTableRow row = new HtmlTableRow();

            HtmlTableCell baslikCell = new HtmlTableCell
            {
                InnerText = baslik
            };
            baslikCell.Attributes["style"] = "font-weight:bold; width:25%;";

            HtmlTableCell degerCell = new HtmlTableCell
            {
                InnerText = deger ?? string.Empty
            };

            row.Cells.Add(baslikCell);
            row.Cells.Add(degerCell);

            GorevInfoTable.Rows.Add(row);
        }
        protected void OnaylaBtn_Click(object sender, EventArgs e)
        {
            try
            {
                GorevOnay gorevOnay = new GorevOnay();
                gorevOnay = gorevOnay.Select(paramGorevOnayIdLbl.Value.ConvertToInt());
                if (gorevOnay != null)
                {
                    gorevOnay.AmirOnayi = (int)GorevOnay.AmirOnayDurumu.Onaylandi;
                    bool isSuccess = gorevOnay.Update();
                    if (isSuccess)
                    {
                        OnaylandiMailiGonder(gorevOnay);
                        MessageHelper.PublishMessage("Görev onaylandı.", ProjeConstants.MESAJ_BILGI);
                    }
                    else
                    {                       
                        MessageHelper.PublishMessage("Onaylama işlemi başarısız oldu.", ProjeConstants.MESAJ_HATA);
                    }
                }
                TabloOlustur();
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void ReddetBtn_Click(object sender, EventArgs e)
        {
            try
            {
                GorevOnay gorevOnay = new GorevOnay();
                gorevOnay = gorevOnay.Select(paramGorevOnayIdLbl.Value.ConvertToInt());
                if (gorevOnay != null)
                {
                    gorevOnay.OnayRedAciklama = ReddetAciklamaTxt.Text;
                    gorevOnay.AmirOnayi = (int)GorevOnay.AmirOnayDurumu.Reddedildi;
                    bool isSuccess = gorevOnay.Update();
                    if (isSuccess)
                    {
                        ReddedildiMailiGonder(gorevOnay);
                        MessageHelper.PublishMessage("Görev reddedildi.", ProjeConstants.MESAJ_BILGI);
                    }
                    else
                    {
                        MessageHelper.PublishMessage("Reddetme işlemi başarısız oldu.", ProjeConstants.MESAJ_HATA);
                    }
                }
                TabloOlustur();
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private void OnaylandiMailiGonder(GorevOnay gorevOnay)
        {
            try
            {
                Personel personel = new Personel();
                personel = personel.Select<Personel>(gorevOnay.PersonelId);
                if (personel != null)
                {
                    List<string> emailList = new List<string>();
                    //Bu personelin Amirini bul
                    IsBilgileri isBilgileri = new IsBilgileri();
                    isBilgileri = isBilgileri.SelectByPersonelId(personel.Id);
                    Personel amir = new Personel();
                    if (isBilgileri != null)
                    {
                        BirimTanim birimTanim = new BirimTanim();
                        birimTanim = birimTanim.Select<BirimTanim>(isBilgileri.BirimId);
                        amir = amir.Select(birimTanim.AmirId);
                        if (birimTanim != null)
                        {
                            amir = amir.Select(birimTanim.AmirId);
                            IletisimBilgileri amirIsBilgileri = new IletisimBilgileri();
                            amirIsBilgileri = amirIsBilgileri.SelectByPersonelId(amir.Id);
                            emailList.Add(amirIsBilgileri.InternetEPosta);
                        }
                        IletisimBilgileri ib = new IletisimBilgileri();
                        ib = ib.SelectByPersonelId(personel.Id);
                        emailList.Add(ib.InternetEPosta);
                    }
                    //amire mail gönder
                    //personele  mail gönder
                    IKYSOrtak.GorevOnayEPostasiGonder(personel, gorevOnay.Id, "YurtIçi/YurtDisi", emailList, "Onay");

                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private void ReddedildiMailiGonder(GorevOnay gorevOnay)
        {
            try
            {
                Personel personel = new Personel();
                personel = personel.Select<Personel>(gorevOnay.PersonelId);
                if (personel != null)
                {
                    List<string> emailList = new List<string>();
                    //Bu personelin Amirini bul
                    IsBilgileri isBilgileri = new IsBilgileri();
                    isBilgileri = isBilgileri.SelectByPersonelId(personel.Id);
                    Personel amir = new Personel();
                    if (isBilgileri != null)
                    {
                        BirimTanim birimTanim = new BirimTanim();
                        birimTanim = birimTanim.Select<BirimTanim>(isBilgileri.BirimId);
                        amir = amir.Select(birimTanim.AmirId);
                        if (birimTanim != null)
                        {
                            amir = amir.Select(birimTanim.AmirId);
                            IletisimBilgileri amirIsBilgileri = new IletisimBilgileri();
                            amirIsBilgileri = amirIsBilgileri.SelectByPersonelId(amir.Id);
                            emailList.Add(amirIsBilgileri.InternetEPosta);
                        }
                        IletisimBilgileri ib = new IletisimBilgileri();
                        ib = ib.SelectByPersonelId(personel.Id);
                        emailList.Add(ib.InternetEPosta);
                    }
                    //amire mail gönder
                    //personele  mail gönder
                    IKYSOrtak.GorevOnayEPostasiGonder(personel, gorevOnay.Id, "YurtIçi/YurtDisi", emailList, "Red");

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
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
    }
}
