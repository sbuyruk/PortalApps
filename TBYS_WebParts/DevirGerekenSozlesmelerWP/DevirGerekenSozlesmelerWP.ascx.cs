using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.DevirGerekenSozlesmelerWP
{
    [ToolboxItemAttribute(false)]
    public partial class DevirGerekenSozlesmelerWP : WebPart
    {
        public DevirGerekenSozlesmelerWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }

        private IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);

        protected void Page_Load(object sender, EventArgs e)
        {
            ScriptManager scriptManager = ScriptManager.GetCurrent(Page);
            if (scriptManager != null)
            {
                scriptManager.RegisterAsyncPostBackControl(TumunuDevirAlNowBtn);
            }
        }

        protected void LoadDataBtn_Click(object sender, EventArgs e)
        {
            // Butonu gizle
            TablosunuDoldur();
        }

        /// <summary>
        /// Devir tutari farkli olan aktif sözlesmeleri tek SQL sorgusuyla getirir ve tabloya basar.
        /// </summary>
        private void TablosunuDoldur()
        {
            List<DevirGerekenSozlesmeItem> itemListesi = new List<DevirGerekenSozlesmeItem>();

            KiraSozlesme kiraSozlesmeDao = new KiraSozlesme();
            System.Data.DataTable dt = kiraSozlesmeDao.SelectDevirGerekenAktifSozlesmelerReturnDT();

            if (dt != null)
            {
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    decimal sonAnaPara      = row["SonAnaPara"]      == DBNull.Value ? 0 : Convert.ToDecimal(row["SonAnaPara"]);
                    decimal sonFaizliBakiye = row["SonFaizliBakiye"] == DBNull.Value ? 0 : Convert.ToDecimal(row["SonFaizliBakiye"]);
                    decimal sonFaizTutari   = sonFaizliBakiye - sonAnaPara;

                    decimal devirAnaPara      = row["DevirAnaPara"]      == DBNull.Value ? 0 : Convert.ToDecimal(row["DevirAnaPara"]);
                    decimal devirFaizTutari   = row["DevirFaizTutari"]   == DBNull.Value ? 0 : Convert.ToDecimal(row["DevirFaizTutari"]);
                    decimal devirFaizliBakiye = row["DevirFaizliBakiye"] == DBNull.Value ? 0 : Convert.ToDecimal(row["DevirFaizliBakiye"]);

                    DateTime sozBasTar = row["SozBasTar"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["SozBasTar"]);
                    DateTime sozBitTar = row["SozBitTar"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["SozBitTar"]);

                    itemListesi.Add(new DevirGerekenSozlesmeItem
                    {
                        KiraciAdi         = row["KiraciAdi"].ToString(),
                        KiraciId          = row["KiraciId"].ToString(),
                        SozlesmeId        = row["SozlesmeId"].ToString(),
                        DosyaNo           = row["DosyaNo"].ToString(),
                        TarihAraligi      = sozBasTar.ConvertToDatetimeEmptyIfNull() + "-" + sozBitTar.ConvertToDatetimeEmptyIfNull(),
                        KiraBedeli        = Convert.ToDecimal(row["KiraBedeli"]).ToString("N", culturInfo),
                        DevirAnapara      = sonAnaPara.ToString("N", culturInfo)      + " (" + devirAnaPara.ToString("N", culturInfo)      + ")",
                        DevirFaiz         = sonFaizTutari.ToString("N", culturInfo)   + " (" + devirFaizTutari.ToString("N", culturInfo)   + ")",
                        DevirFaizliBakiye = sonFaizliBakiye.ToString("N", culturInfo) + " (" + devirFaizliBakiye.ToString("N", culturInfo) + ")"
                    });
                }
            }

            TitleLbl.Text = "Devir Gereken Sözlesmeler (" + itemListesi.Count + " adet)";

            var serializer = new JavaScriptSerializer();
            string jsonData = serializer.Serialize(itemListesi);
            UtilityHelper.ScriptCalistir("document.getElementById('CustomDataTable').style.display='';");
            UtilityHelper.ScriptCalistir(CreateDataTable(jsonData));
        }

        private string CreateDataTable(string jsonData)
        {
            return @"
                if (jQuery.fn.DataTable.isDataTable('#CustomDataTable')) {
                    jQuery('#CustomDataTable').DataTable().destroy();
                }
                jQuery('#CustomDataTable tbody').empty();

                jQuery.fn.dataTable.moment('DD.MM.YYYY');
                jQuery('#CustomDataTable').DataTable({
                    data: " + jsonData + @",
                    columns: [
                            { data: 'KiraciAdi' },
                            { data: 'DosyaNo' },
                            { data: 'TarihAraligi' },
                            { data: 'KiraBedeli' },
                            { data: 'DevirAnapara' },
                            { data: 'DevirFaiz' },
                            { data: 'DevirFaizliBakiye' },
                            { data: 'SozlesmeId' },
                            { data: 'SozlesmeId' }
                        ],
                        columnDefs: [
                            {
                                targets: [3, 4, 5, 6],
                                className: 'text-end'
                            },
                            {
                                targets: [7],
                                render: function (data, type, row) {
                                    return '<a href=""#"" onclick=""DevirAl(' + row.SozlesmeId + ', \u0027' + row.KiraciAdi + '\u0027, \u0027' + row.TarihAraligi + '\u0027); return false;"" class=""btn btn-outline-danger btn-sm"">Devir Al</a>';
                                },
                                className: 'text-center'
                            },
                            {
                                targets: [8],
                                render: function (data, type, row) {
                                    return '<a href=""" + ProjeConstants.PAGE_KIRASOZLESMESI + @"?DestinationApp=KS&SenderApp=DGS&KiraSozlesmeId=' + row.SozlesmeId + '&KiraciId=' + row.KiraciId + '"" class=""btn btn-outline-primary btn-sm"">Sözlesme</a>';
                                },
                                className: 'text-center'
                            }
                        ],
                        language: {
                            url: '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                            decimal: ',',
                            thousands: '.'
                        },
                        responsive: true,
                        destroy: true,
                        dom: 'lfrtip'
                    });
            ";
        }

        protected void DevirAlBtn_Click(object sender, EventArgs e)
        {
            try
            {
                UtilityHelper.ScriptCalistir("DevirAlModalKapat();");
                KiraSozlesme kiraSozlesme = new KiraSozlesme();
                kiraSozlesme = kiraSozlesme.Select(HiddenSecilenId.Value.ConvertToInt());
                if (kiraSozlesme == null)
                {
                    MessageHelper.PublishMessage("Sözlesme bulunamadi!", ProjeConstants.MESAJ_HATA);
                    return;
                }

                KiraBakiyeDevriOtomatik bakiyeDevri = new KiraBakiyeDevriOtomatik();
                bool islendi = bakiyeDevri.DevirAl(kiraSozlesme, atlaBayrakKontrolu: true);
                if (islendi)
                {
                    MessageHelper.PublishMessage("Devir alma islemi tamamlandi.", ProjeConstants.MESAJ_BASARILI, 2000);
                }
                else
                {
                    MessageHelper.PublishMessage("Devir alinacak fark bulunamadi.", ProjeConstants.MESAJ_BILGI, 2000);
                }

                TablosunuDoldur();
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }

        protected void TumunuDevirAlNowBtn_Click(object sender, EventArgs e)
        {
            try
            {
                UtilityHelper.ScriptCalistir("DevirAlModalKapat();");
                KiraBakiyeDevriOtomatik bakiyeDevri = new KiraBakiyeDevriOtomatik();
                KiraBakiyeDevriSonuc sonuc = bakiyeDevri.TumAktifSozlesmeleriIsle();

                MessageHelper.PublishMessage(
                    string.Format("Islenen: {0} | Atlanan: {1} | Hatali: {2}",
                        sonuc.IslenenSayisi, sonuc.AtlananSayisi, sonuc.HataliSayisi),
                    ProjeConstants.MESAJ_BASARILI, 4000);

                TablosunuDoldur();
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void OdemePlanlariniGuncelleBtn_Click(object sender, EventArgs e)
        {
            try
            {
                TBYSOrtak.AktifSozleslemelerinBakiyeBorcunuHesapla();
                MessageHelper.PublishMessage("Ödeme planlari güncellendi.", ProjeConstants.MESAJ_BASARILI, 2000);
                TablosunuDoldur();
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private class DevirGerekenSozlesmeItem
        {
            public string KiraciAdi { get; set; }
            public string KiraciId { get; set; }
            public string SozlesmeId { get; set; }
            public string DosyaNo { get; set; }
            public string TarihAraligi { get; set; }
            public string KiraBedeli { get; set; }
            public string DevirAnapara { get; set; }
            public string DevirFaiz { get; set; }
            public string DevirFaizliBakiye { get; set; }
        }
    }
}