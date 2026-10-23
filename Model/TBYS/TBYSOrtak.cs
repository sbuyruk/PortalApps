using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using DAO.Ortak;
using Model.IKYS;
using Model.Services.TBYS;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using Utility.HelperClasses;
using Utility.Logging;
using Utility.ProjeGlobal;

namespace Model.Ortak
{
    public class TBYSOrtak
    {
        public TBYSOrtak()
        {

        }

        #region TBYS

        public static bool BakiyeBorcHesapla(KiraSozlesme kiraSozlesme, PortalLogContext context)
        {
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                bool updated = BakiyeBorcHesapla(kiraSozlesme, context, transaction);
                if (updated) transaction.Complete();
                return updated;
            }
        }

        public static bool BakiyeBorcHesapla(KiraSozlesme kiraSozlesme, PortalLogContext context, SqlTransactionContext transaction)
        {
            if (context == null) throw new ArgumentNullException("context", "Log bağlamı gereklidir.");
            if (transaction == null) throw new ArgumentNullException("transaction", "Veritabanı işlem bağlamı gereklidir.");
            if (kiraSozlesme == null) return false;
            KiraSozlesme current = new KiraSozlesmeService(context).GetById(kiraSozlesme.Id, transaction);
            if (current == null) return false;
            return current.GecikmeZammiTipi == ProjeConstants.KIRASOZLESME_GECIKMEZAMMI_GUNLUK
                ? BakiyeBorcHesaplaGunlukGecikmeZammi(current, context, transaction)
                : BakiyeBorcHesaplaAylik(current, context, transaction);
        }

        public static void AktifSozleslemelerinBakiyeBorcunuHesapla(PortalLogContext context)
        {
            foreach (KiraSozlesme item in new KiraSozlesmeService(context).GetAllActive())
                if (!BakiyeBorcHesapla(item, context))
                    throw new InvalidOperationException("Sözleşmenin ödeme planı güncellenemedi.");
        }

        /// <summary>
        /// KiraSozlesmesini parametre olarak alir.
        /// Bu KSnin ödeme planinda;
        ///     Gecikme varsa geciken her ayin KiraBedeli'ne islem yapilan güne kadar yasal FaizOranina Göre faizTutari olusturur
        ///     Faiz tutarini Faizlibakiyeye ekler
        ///     AnaPara gecikmesine faiz olusturur, eski faizlere tekrar faiz uygulamaz
        /// Fesih durumunda vadesi gelmeyen alarin hesaplanan degerlerini 0 yapar
        /// </summary>
        /// <param name="kiraSozlesme"></param>
        /// <returns></returns>

        public static bool BakiyeBorcHesaplaAylik(KiraSozlesme kiraSozlesme, PortalLogContext context)
        {
            if (kiraSozlesme == null || context == null) return false;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                bool updated = BakiyeBorcHesaplaAylik(kiraSozlesme, context, transaction);
                if (updated) transaction.Complete();
                return updated;
            }
        }

        private static bool BakiyeBorcHesaplaAylik(KiraSozlesme kiraSozlesme, PortalLogContext context, SqlTransactionContext transaction)
        {
            decimal faizToplami = 0m;
            decimal faizOrani = ProjeConstants.YASAL_FAIZ_ORANI;
            decimal anaParaToplami = 0m;
            decimal faizliBakiye = 0m;
            decimal devirAnaPara = 0m;
            decimal devirFaiz = 0m;
            Model.Services.TBYS.OdemePlaniService planService = new Model.Services.TBYS.OdemePlaniService(context);
            List<OdemePlani> planlar = planService.GetBySozlesmeId(kiraSozlesme.Id, transaction);
            if (planlar.Count > 0)
            {
                devirAnaPara = planlar[0].AnaPara;
                devirFaiz = planlar[0].FaizTutari;
            }

            foreach (OdemePlani odemePlani in planlar)
            {
                if (odemePlani.Sira == 0 || odemePlani.VadeBasTar > kiraSozlesme.SozBitTar) continue;
                YasalFaiz yasalFaiz = new YasalFaizService(context).GetByYearMonth(odemePlani.VadeBasTar.Year, odemePlani.VadeBasTar.Month, transaction);
                if (yasalFaiz == null) throw new InvalidOperationException("İlgili dönem için yasal faiz oranı bulunamadı.");
                faizOrani = yasalFaiz.FaizOrani <= 0 ? faizOrani : yasalFaiz.FaizOrani;
                decimal borc = -odemePlani.KiraBedeli;
                decimal anaPara = odemePlani.OdenenTutar + borc;
                anaParaToplami += anaPara + devirAnaPara;
                devirAnaPara = 0;
                decimal faizTutari = Math.Round(anaParaToplami * faizOrani / 100);
                if (kiraSozlesme.SozBitTar.AddMonths(-1) < odemePlani.VadeBasTar) faizTutari = 0;
                faizTutari = faizTutari > 0 ? 0 : faizTutari;
                faizToplami += faizTutari + devirFaiz;
                devirFaiz = 0;
                faizliBakiye = anaParaToplami + faizToplami;
                if (anaParaToplami > 0 && faizToplami < 0)
                {
                    decimal fark = anaParaToplami + faizToplami;
                    if (fark > 0) { anaParaToplami = Math.Max(0, fark); faizToplami = 0; }
                    else if (fark < 0) { faizToplami = Math.Min(0, fark); anaParaToplami = 0; }
                    else { anaParaToplami = 0; faizToplami = 0; }
                }
                odemePlani.OdenenTutar = new Model.Services.TBYS.OdemeService(context).GetSumBySozlesmeIdOdemePlaniId(kiraSozlesme.Id, odemePlani.Id, transaction);
                odemePlani.FaizliBakiye = faizliBakiye;
                odemePlani.FaizOrani = faizOrani;
                odemePlani.FaizTutari = faizTutari;
                odemePlani.AnaPara = anaParaToplami;
                if (!planService.Update(odemePlani, transaction)) return false;
            }
            return true;
        }

        public static bool BakiyeBorcHesaplaGunlukGecikmeZammi(KiraSozlesme kiraSozlesme, PortalLogContext context)
        {
            if (kiraSozlesme == null || context == null) return false;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                bool updated = BakiyeBorcHesaplaGunlukGecikmeZammi(kiraSozlesme, context, transaction);
                if (updated) transaction.Complete();
                return updated;
            }
        }

        private static bool BakiyeBorcHesaplaGunlukGecikmeZammi(KiraSozlesme kiraSozlesme, PortalLogContext context, SqlTransactionContext transaction)
        {
            Model.Services.TBYS.OdemeAyrintiService ayrintiService = new Model.Services.TBYS.OdemeAyrintiService(context);
            if (!ayrintiService.DeleteBySozlesmeId(kiraSozlesme.Id, transaction)) return false;
            Model.Services.TBYS.OdemePlaniService planService = new Model.Services.TBYS.OdemePlaniService(context);
            List<OdemePlani> planlar = planService.GetBySozlesmeId(kiraSozlesme.Id, transaction);
            decimal faizToplami = 0m;
            decimal anaParaToplami = 0m;
            decimal devirFaiz = 0m;
            foreach (OdemePlani odemePlani in planlar)
            {
                if (odemePlani.Sira == 0)
                {
                    anaParaToplami = odemePlani.AnaPara;
                    devirFaiz = odemePlani.FaizTutari;
                    continue;
                }

                GunlukGecikmeZammiHesaplaRecursiveDegil(kiraSozlesme, odemePlani, ref anaParaToplami, context, transaction);
                anaParaToplami -= odemePlani.KiraBedeli;
                decimal gecikmeZammiTutari = ayrintiService.GetSumDelayAmount(odemePlani.Id, transaction);
                decimal gecikmeZammiOrani = ayrintiService.GetLastDelayRate(odemePlani.Id, transaction);
                faizToplami += gecikmeZammiTutari + devirFaiz;
                devirFaiz = 0;
                odemePlani.AnaPara = anaParaToplami;
                odemePlani.FaizTutari = gecikmeZammiTutari;
                odemePlani.FaizliBakiye = anaParaToplami + faizToplami;
                odemePlani.FaizOrani = gecikmeZammiOrani;
                odemePlani.OdenenTutar = new Model.Services.TBYS.OdemeService(context).GetSumBySozlesmeIdOdemePlaniId(kiraSozlesme.Id, odemePlani.Id, transaction);
                if (!planService.Update(odemePlani, transaction)) return false;
                if (odemePlani.VadeBitTar >= DateTime.Today) break;
            }
            return true;
        }

        private static void GunlukGecikmeZammiHesaplaRecursiveDegil(KiraSozlesme kiraSozlesme, OdemePlani odemePlani,
            ref decimal anaPara, PortalLogContext context, SqlTransactionContext transaction)
        {
            DateTime ilkTarih = odemePlani.VadeBasTar;
            DateTime ikinciTarih = odemePlani.VadeBitTar;
            List<GecikmeZammi> gecikmeZammiListesi = new GecikmeZammiService(context).GetByDateRange(ilkTarih, ikinciTarih, transaction);
            foreach (GecikmeZammi gecikmeZammi in gecikmeZammiListesi)
            {
                List<Odeme> odemeler = new Model.Services.TBYS.OdemeService(context)
                    .GetByKiraciVadeBasTarVadeBitTar(kiraSozlesme.Id, kiraSozlesme.KiraciId, ilkTarih, ikinciTarih, transaction);
                if (odemeler.Count > 0)
                {
                    foreach (Odeme odeme in odemeler)
                    {
                        ikinciTarih = odeme.OdemeTarihi;
                        int aySayisi = (ikinciTarih - ilkTarih).Days >= (odemePlani.VadeBitTar - odemePlani.VadeBasTar).Days ? 1 : 0;
                        int gunSayisi = aySayisi < 1 ? Math.Max(0, (ikinciTarih.AddDays(1) - ilkTarih).Days) : 0;
                        GecikmeZammiHesabi(ilkTarih, ikinciTarih, kiraSozlesme, odemePlani, gecikmeZammi, odeme, aySayisi, gunSayisi, ref anaPara, context, transaction);
                        ilkTarih = odeme.OdemeTarihi.AddDays(1);
                    }
                    ilkTarih = ikinciTarih.AddDays(1);
                    ikinciTarih = gecikmeZammi.BitisTarihi > ProjeConstants.NULL_TARIH ? gecikmeZammi.BitisTarihi : odemePlani.VadeBitTar;
                    GecikmeZammiHesabi(ilkTarih, ikinciTarih, kiraSozlesme, odemePlani, gecikmeZammi, null, 0, Math.Max(0, (ikinciTarih.AddDays(1) - ilkTarih).Days), ref anaPara, context, transaction);
                }
                else
                {
                    int aySayisi = (ikinciTarih - ilkTarih).Days >= (odemePlani.VadeBitTar - odemePlani.VadeBasTar).Days ? 1 : 0;
                    int gunSayisi = aySayisi < 1 ? Math.Max(0, (ikinciTarih.AddDays(1) - ilkTarih).Days) : 0;
                    GecikmeZammiHesabi(ilkTarih, ikinciTarih, kiraSozlesme, odemePlani, gecikmeZammi, null, aySayisi, gunSayisi, ref anaPara, context, transaction);
                }
            }
        }
        private static void GecikmeZammiHesabi(DateTime ilkTarih, DateTime ikinciTarih, KiraSozlesme kiraSozlesme, OdemePlani odemePlani, GecikmeZammi gz, Odeme odeme, int aySayisi, int gunSayisi, ref decimal anaPara, PortalLogContext context, SqlTransactionContext transaction)
        {
            if (odemePlani.Sira <= 0) return;
            IFormatProvider cultureInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            decimal odenenTutar = odeme == null ? 0 : odeme.OdenenTutar;
            decimal kalanAnaPara = anaPara + odenenTutar;
            decimal gecikmeZammiTutari = anaPara * aySayisi * gz.ZamOrani / 100 + anaPara * gunSayisi * (gz.ZamOrani / 30) / 100;
            string ayAciklama = anaPara.ToString("N", cultureInfo) + "TL x " + aySayisi + "ay x " + gz.ZamOrani.ToString("N", cultureInfo) + "/100 ";
            string gunAciklama = anaPara.ToString("N", cultureInfo) + "TL x " + gunSayisi + "gün x (" + gz.ZamOrani.ToString("N", cultureInfo) + "/30)/100 ";
            string aciklama = (anaPara * aySayisi * gz.ZamOrani / 100) < 0 ? ayAciklama : gunAciklama;
            gecikmeZammiTutari = gecikmeZammiTutari > 0 ? 0 : gecikmeZammiTutari;
            OdemeAyrintiKaydetVeyaGuncelle(ilkTarih, ikinciTarih, kiraSozlesme, odemePlani, gz, odeme, aySayisi, gunSayisi, kalanAnaPara, gecikmeZammiTutari, aciklama, context, transaction);
            anaPara = kalanAnaPara;
        }

        private static void OdemeAyrintiKaydetVeyaGuncelle(DateTime ilkTarih, DateTime sonTarih, KiraSozlesme kiraSozlesme, OdemePlani odemePlani, GecikmeZammi gz, Odeme odeme, int aySayisi, int gunSayisi, decimal kalanAnaPara, decimal gecikmeZammiTutari, string aciklama, PortalLogContext context, SqlTransactionContext transaction)
        {
            int odemeId = odeme == null ? 0 : odeme.Id;
            OdemeAyrintiService service = new OdemeAyrintiService(context);
            OdemeAyrinti ayrinti = service.GetByPlanAndDelay(kiraSozlesme, odemePlani.Id, gz.Id, odemeId, transaction) ?? new OdemeAyrinti();
            ayrinti.IlkTarih = ilkTarih;
            ayrinti.SonTarih = sonTarih;
            ayrinti.KiraciId = kiraSozlesme.KiraciId;
            ayrinti.SozlesmeId = odemePlani.SozlesmeId;
            ayrinti.OdemePlaniId = odemePlani.Id;
            ayrinti.OdemeId = odemeId;
            ayrinti.GecikmeZammiId = gz.Id;
            ayrinti.OdemePlaniSirasi = odemePlani.Sira;
            ayrinti.SonOdemeTarihi = odemePlani.VadeBitTar;
            ayrinti.GecikmeZammiDegisimTar = gz.BaslangicTarihi;
            ayrinti.OdemeTarihi = odeme == null ? new DateTime() : odeme.OdemeTarihi;
            ayrinti.AySayisi = aySayisi;
            ayrinti.GunSayisi = gunSayisi;
            ayrinti.GecikmeZammiOrani = gz.ZamOrani;
            ayrinti.AnaPara = kalanAnaPara - (odeme == null ? 0 : odeme.OdenenTutar);
            ayrinti.OdenenTutar = odeme == null ? 0 : odeme.OdenenTutar;
            ayrinti.KalanAnaPara = kalanAnaPara;
            ayrinti.GecikmeZammiTutari = gecikmeZammiTutari;
            ayrinti.Aciklama = aciklama;
            if (ayrinti.Id == 0) service.Save(ayrinti, transaction); else service.Update(ayrinti, transaction);
        }

        public static bool OdemeYap(KiraSozlesme kiraSozlesme, DateTime odemetarihi, decimal odenenTutar, string aciklama, ref int odemeId, PortalLogContext context)
        {
            odemeId = 0;
            if (kiraSozlesme == null || context == null) return false;
            int savedId;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                Model.Services.TBYS.OdemePlaniService odemePlaniService = new Model.Services.TBYS.OdemePlaniService(context);
                List<OdemePlani> planlar = odemePlaniService.GetBySozlesmeId(kiraSozlesme.Id, transaction);
                if (planlar == null || planlar.Count < 2) return false;

                DateTime odemeTarihi = odemetarihi;
                OdemePlani odemePlani = planlar[1];
                if (odemeTarihi < odemePlani.OdemeBasTar)
                {
                    odemePlani = planlar.FirstOrDefault(x => x.Sira > 0);
                }
                else if (odemeTarihi > odemePlani.OdemeBitTar)
                {
                    if (kiraSozlesme.OdemeSekli == ProjeConstants.KIRA_ODMSEKLI_YILLIK && odemeTarihi < kiraSozlesme.SozBitTar)
                    {
                        odemePlani = odemePlaniService.GetBySozlesmeIdOdemeTarihi(kiraSozlesme.Id, odemeTarihi, transaction)
                            ?? planlar.LastOrDefault(x => x.Sira > 0);
                    }
                    else
                    {
                        odemePlani = planlar.LastOrDefault(x => x.Sira > 0);
                    }
                }
                else
                {
                    odemePlani = odemePlaniService.GetBySozlesmeIdOdemeTarihi(kiraSozlesme.Id, odemeTarihi, transaction)
                        ?? planlar.LastOrDefault(x => x.Sira > 0);
                }

                if (odemePlani == null) return false;
                Odeme odeme = new Model.Services.TBYS.OdemeService(context)
                    .OdemeyiKaydetOdemePlaniniGuncelle(kiraSozlesme, odemePlani, odemeTarihi, odenenTutar, aciklama, UtilityHelper.GetCurrentUserLoginName(), transaction);
                if (odeme == null || odeme.Id <= 0) return false;
                savedId = odeme.Id;
                transaction.Complete();
            }

            odemeId = savedId;
            return true;
        }

        public static bool TeminatIslemleriniHesaplaVeKaydet(KiraSozlesme kiraSozlesme, PortalLogContext context)
        {
            if (kiraSozlesme == null || context == null) return false;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                bool updated = TeminatIslemleriniHesaplaVeKaydet(kiraSozlesme, context, transaction);
                if (updated) transaction.Complete();
                return updated;
            }
        }

        public static bool TeminatIslemleriniHesaplaVeKaydet(KiraSozlesme kiraSozlesme, PortalLogContext context, SqlTransactionContext transaction)
        {
            if (kiraSozlesme == null) return false;
            kiraSozlesme = new KiraSozlesmeService(context).GetById(kiraSozlesme.Id, transaction);
            if (kiraSozlesme == null) return false;
            decimal toplamOdeme = 0m;
            decimal toplamIade = 0m;
            DataTable dataTable = new Model.Services.TBYS.TeminatIslemService(context).GetSumByKiraciIdGroupByType(kiraSozlesme.KiraciId, transaction);
            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    decimal islemToplami = row["IslemToplami"].ReturnZeroIfNull().ConvertToDecimal();
                    switch (row["IslemTipi"].ToString())
                    {
                        case ProjeConstants.TEMINAT_ODEMESI:
                        case ProjeConstants.TEMINAT_GECICITEMINATODEMESI:
                            toplamOdeme += islemToplami;
                            break;
                        case ProjeConstants.TEMINAT_KIRACIYAIADE:
                        case ProjeConstants.TEMINAT_KIRAYAMAHSUP:
                        case ProjeConstants.TEMINAT_HASARAMAHSUP:
                        case ProjeConstants.TEMINAT_AIDATAMAHSUP:
                        case ProjeConstants.TEMINAT_VAKFABAGIS:
                            toplamIade += islemToplami;
                            break;
                    }
                }
            }
            kiraSozlesme.OdenenTeminatTutari = toplamOdeme;
            kiraSozlesme.IadeTeminatTutari = toplamIade;
            kiraSozlesme.KalanTeminatTutari = toplamOdeme - toplamIade;
            return new Model.Services.TBYS.KiraSozlesmeService(context).Update(kiraSozlesme, transaction);
        }
        #endregion TBYS

    }
}
