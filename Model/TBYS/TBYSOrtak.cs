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

        public static void BakiyeBorcHesapla(KiraSozlesme kiraSozlesme)
        {
            bool isGecikmeZammiGunluk = kiraSozlesme.GecikmeZammiTipi.Equals(ProjeConstants.KIRASOZLESME_GECIKMEZAMMI_GUNLUK);
            if (isGecikmeZammiGunluk)
            {
                TBYSOrtak.BakiyeBorcHesaplaGunlukGecikmeZammi(kiraSozlesme);
            }
            else
            {
                TBYSOrtak.BakiyeBorcHesaplaAylik(kiraSozlesme);
            }
        }

        public static bool BakiyeBorcHesapla(KiraSozlesme kiraSozlesme, PortalLogContext context)
        {
            if (kiraSozlesme == null || context == null) return false;
            if (kiraSozlesme.GecikmeZammiTipi.Equals(ProjeConstants.KIRASOZLESME_GECIKMEZAMMI_GUNLUK))
                return BakiyeBorcHesaplaGunlukGecikmeZammi(kiraSozlesme, context);
            return BakiyeBorcHesaplaAylik(kiraSozlesme, context);
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
        public static void BakiyeBorcHesaplaAylik(KiraSozlesme kiraSozlesme)
        {
            DateTime today = DateTime.Today;

            decimal faizToplami = 0;
            decimal faizOrani = 1;
            decimal anaParaToplami = 0;
            decimal faizliBakiye = 0;
            decimal devirAnaPara = 0;
            decimal devirFaiz = 0;
            faizOrani = ProjeConstants.YASAL_FAIZ_ORANI;

            # region devir eden anapara ve faiz varsa dikkate al
            OdemePlani odemePlaniDao = new OdemePlani();
            List<OdemePlani> list = new Model.Services.TBYS.OdemePlaniService().GetBySozlesmeId(kiraSozlesme.Id);
            if (list.Count > 0)
            {
                OdemePlani odemePlaniDevir = list[0];
                if (odemePlaniDevir != null)
                {
                    devirAnaPara = odemePlaniDevir.AnaPara;
                    devirFaiz = odemePlaniDevir.FaizTutari;
                }
            }

            #endregion
            DateTime islemTarihi = kiraSozlesme.SozBitTar;//today; SB 24.02.2021 Sözlesmenin son ayina kadar faiz hesabini yapsin  Zekayi Bey ileriye dönük simülasyon gibi kullanmak istiyor 
            foreach (OdemePlani odemePlani in list)
            {
                if (odemePlani.Sira == 0)
                {
                    continue;//devir satiri için hesap yapma
                }
                else
                {
                    DateTime vadeBasTar = odemePlani.VadeBasTar;//vade tarihinden itibaren
                    #region Yasal Faiz Kontrolü
                    YasalFaiz yasalFaiz = new YasalFaiz();
                    yasalFaiz = new Model.Services.TBYS.YasalFaizService().GetByYearMonth(vadeBasTar.Year, vadeBasTar.Month);
                    if (yasalFaiz == null)
                    {
                        Exception ex1 = new Exception(vadeBasTar.Month + "/" + vadeBasTar.Year + " Ayi için Yasal Faiz Orani girilmediginden Faiz Hesaplanamiyor.");
                        Exception ex2 = new Exception("Yasal faiz menusunden " + vadeBasTar.Year + " yili için faiz orani girdikten sonra tekrar deneyiniz");
                        ExceptionHelper exhelper = new ExceptionHelper();
                        exhelper.Exceptions.Add(ex1);
                        exhelper.Exceptions.Add(ex2);

                        exhelper.PublishException();
                    }
                    #endregion
                    else
                    {
                        faizOrani = yasalFaiz.FaizOrani <= 0 ? faizOrani : yasalFaiz.FaizOrani;

                        if ((vadeBasTar != null) && (vadeBasTar <= islemTarihi))
                        {

                            decimal borc = (-1) * odemePlani.KiraBedeli;
                            #region
                            decimal anaPara = odemePlani.OdenenTutar + borc;

                            anaParaToplami += anaPara + devirAnaPara;
                            devirAnaPara = 0; //devirAnapara bir kere eklensin
                            #endregion

                            #region FaiztutariHesapla
                            decimal faizTutari = 0;
                            faizTutari = Math.Round(anaParaToplami * faizOrani / 100);

                            if (islemTarihi.AddMonths(-1) < vadeBasTar && islemTarihi >= vadeBasTar) // islem tarihi  önceki vadeBastarihinden büyük ve simdiki vade tarihinden küçük esitse // 
                            {

                                faizTutari = 0;
                            }

                            faizTutari = faizTutari > 0 ? 0 : faizTutari;//arti faiz olmasin, kiracinin alacagi varsa faiz uyulamasin 
                            #endregion

                            #region AnaPara_Faiz_FaizliBakiyeyi_Bul
                            //anaParaToplami += kalan;
                            faizToplami += faizTutari + devirFaiz;
                            devirFaiz = 0;//devirFaiz bir kere eklensin;
                            faizliBakiye = anaParaToplami + faizToplami;
                            #endregion

                            #region OdemePlaniTablosunuGuncelle
                            if (odemePlani.Sira == 0)
                            {
                                //devir satirini güncellemesin
                            }
                            else
                            {
                                //anaparadan artan varsa faiz ile mahsuplas
                                if (anaParaToplami > 0 && faizToplami < 0)
                                {
                                    decimal fark = anaParaToplami + faizToplami;
                                    if (fark > 0)//anaparadan faiz çikinca artan miktar var
                                    {

                                        anaParaToplami = anaParaToplami + faizToplami;
                                        anaParaToplami = anaParaToplami < 0 ? 0 : anaParaToplami;
                                        faizToplami = 0;
                                    }
                                    else if (fark < 0)//anaparadan faiz çikinca hala faiz borcu var var 
                                    {

                                        faizToplami = anaParaToplami + faizToplami;
                                        faizToplami = faizToplami > 0 ? 0 : faizToplami;
                                        anaParaToplami = 0;
                                    }
                                    else if (fark == 0)//anaparadan faiz çikinca kalan 0
                                    {
                                        anaParaToplami = 0;
                                        faizToplami = 0;
                                    }
                                }

                                Odeme odemeDao = new Odeme();
                                var odenenTutar = new Model.Services.TBYS.OdemeService().GetSumBySozlesmeIdOdemePlaniId(kiraSozlesme.Id, odemePlani.Id);
                                odemePlani.OdenenTutar = odenenTutar;
                                odemePlani.FaizliBakiye = faizliBakiye;
                                odemePlani.FaizOrani = faizOrani;
                                odemePlani.FaizTutari = faizTutari;
                                odemePlani.AnaPara = anaParaToplami;
                                new Model.Services.TBYS.OdemePlaniService().Update(odemePlani);
                            }
                            #endregion
                        }
                    }
                }

            }

        }
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
                if (yasalFaiz == null) continue;
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

        public static void AktifSozleslemelerinBakiyeBorcunuHesapla()
        {
            DateTime saat = DateTime.Now;
            KiraSozlesme kiraSozlesmeDao = new KiraSozlesme();
            List<KiraSozlesme> kiraSozlesmeListesi = new Model.Services.TBYS.KiraSozlesmeService().GetAllActive();
            DateTime saat1 = DateTime.Now;
            foreach (KiraSozlesme kiraSozlesme in kiraSozlesmeListesi)
            {
                if (kiraSozlesme != null)
                {
                    TBYSOrtak.BakiyeBorcHesapla(kiraSozlesme);
                }
            }
        }
        public static void BakiyeBorcHesaplaGunlukGecikmeZammi(KiraSozlesme kiraSozlesme)
        {
            OdemeAyrinti odemeAyrinti = new OdemeAyrinti();
            new Model.Services.TBYS.OdemeAyrintiService().DeleteBySozlesmeId(kiraSozlesme.Id);

            //bu kiraciya ait tüm ödeme planlarini al tarih sirali
            OdemePlani op = new OdemePlani();
            List<OdemePlani> opList = new Model.Services.TBYS.OdemePlaniService().GetBySozlesmeId(kiraSozlesme.Id);
            var faizToplami = 0m;
            var faizTutari = 0m;
            var anaParaToplami = 0m;
            var devirFaiz = 0m;
            foreach (var odemePlani in opList)
            {
                if (odemePlani.Sira == 0)
                {

                    anaParaToplami = odemePlani.AnaPara;
                    devirFaiz = odemePlani.FaizTutari;
                    continue;
                }
                //int sayac = 0;
                //GunlukGecikmeZammiHesaplaRecursive(kiraSozlesme, odemePlani, odemePlani.VadeBasTar, odemePlani.VadeBitTar, ref anaParaBakiye, ref faizliBakiyeToplami, ref sayac);
                OdemeAyrinti odemeAyrintiDao = new OdemeAyrinti();
                
                GunlukGecikmeZammiHesaplaRecursiveDegil(kiraSozlesme, odemePlani, ref anaParaToplami);
                anaParaToplami = anaParaToplami - odemePlani.KiraBedeli;
                var gecikmeZammiTutari = new Model.Services.TBYS.OdemeAyrintiService().GetSumDelayAmount(odemePlani.Id);
                var gecikmeZammiOrani = new Model.Services.TBYS.OdemeAyrintiService().GetLastDelayRate(odemePlani.Id);

                faizToplami += gecikmeZammiTutari + devirFaiz;
                devirFaiz = 0;//devirFaiz bir kere eklensin;

                //anaparadan artan varsa faiz ile mahsuplas
                if (anaParaToplami > 0 && faizToplami < 0)
                {
                    decimal fark = anaParaToplami + faizToplami;
                    if (fark > 0)//anaparadan faiz çikinca artan miktar var
                    {

                        anaParaToplami = anaParaToplami + faizToplami;
                        anaParaToplami = anaParaToplami < 0 ? 0 : anaParaToplami;
                        faizToplami = 0;
                    }
                    else if (fark < 0)//anaparadan faiz çikinca hala faiz borcu var var 
                    {

                        faizToplami = anaParaToplami + faizToplami;
                        faizToplami = faizToplami > 0 ? 0 : faizToplami;
                        anaParaToplami = 0;
                    }
                    else if (fark == 0)//anaparadan faiz çikinca kalan 0
                    {
                        anaParaToplami = 0;
                        faizToplami = 0;
                    }

                }
                
                odemePlani.AnaPara = anaParaToplami;
                odemePlani.FaizTutari = gecikmeZammiTutari;
                odemePlani.FaizliBakiye = anaParaToplami+faizToplami;// anaParaToplami + gecikmeZammiTutari +devirFaiz ;
                odemePlani.FaizOrani = gecikmeZammiOrani;

                //ödeme plani Id ye ve sozId ye göre al
                Odeme odemeDao = new Odeme();
                var odenenTutar = new Model.Services.TBYS.OdemeService().GetSumBySozlesmeIdOdemePlaniId(kiraSozlesme.Id, odemePlani.Id);
                odemePlani.OdenenTutar = odenenTutar;
                new Model.Services.TBYS.OdemePlaniService().Update(odemePlani);
                //if ((DateTime.Today > odemePlani.VadeBasTar) && (DateTime.Today < odemePlani.VadeBitTar))
                if (odemePlani.VadeBitTar >= DateTime.Today ) // vade dolmadiysa hesaplama yapma
                {
                    break;
                }
            }
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
        private static void OdemeAyrintiKaydetVeyaGuncelle(DateTime ilkTarih, DateTime sonTarih, KiraSozlesme kiraSozlesme, OdemePlani odemePlani, GecikmeZammi gz, int odemeId, DateTime odemeTarihi,
            DateTime sonOdemeTarihi, int aySayisi, int gunSayisi,
            decimal anaPara, decimal odenenTutar, decimal kalanAnaPara, decimal gecikmeZammiTutari, string aciklama)
        {
            string saveOrUpdate = "update";

            OdemeAyrinti odemeAyrinti = new OdemeAyrinti();
            odemeAyrinti = new Model.Services.TBYS.OdemeAyrintiService().GetByPlanAndDelay(kiraSozlesme, odemePlani.Id, gz.Id, odemeId);
            if (odemeAyrinti == null)
            {
                odemeAyrinti = new OdemeAyrinti();
                saveOrUpdate = "save";
            }
            odemeAyrinti.IlkTarih = ilkTarih;
            odemeAyrinti.SonTarih = sonTarih;
            odemeAyrinti.KiraciId = kiraSozlesme.KiraciId;
            odemeAyrinti.SozlesmeId = odemePlani.SozlesmeId;
            odemeAyrinti.OdemePlaniId = odemePlani.Id;
            odemeAyrinti.OdemeId = odemeId;
            odemeAyrinti.GecikmeZammiId = gz.Id;
            odemeAyrinti.OdemePlaniSirasi = odemePlani.Sira;
            odemeAyrinti.SonOdemeTarihi = sonOdemeTarihi;
            odemeAyrinti.GecikmeZammiDegisimTar = gz.BaslangicTarihi;
            odemeAyrinti.OdemeTarihi = odemeTarihi;
            odemeAyrinti.AySayisi = aySayisi;
            odemeAyrinti.GunSayisi = gunSayisi;
            odemeAyrinti.GecikmeZammiOrani = gz.ZamOrani;
            odemeAyrinti.AnaPara = anaPara;
            odemeAyrinti.OdenenTutar = odenenTutar;
            odemeAyrinti.KalanAnaPara = kalanAnaPara;
            odemeAyrinti.GecikmeZammiTutari = gecikmeZammiTutari;
            odemeAyrinti.Aciklama = aciklama;
            if (saveOrUpdate.Equals("save"))
            {
                new Model.Services.TBYS.OdemeAyrintiService().Save(odemeAyrinti);
            }
            else
            {
                new Model.Services.TBYS.OdemeAyrintiService().Update(odemeAyrinti);
            }

        }
        public static void GunlukGecikmeZammiHesaplaRecursiveDegil(KiraSozlesme kiraSozlesme, OdemePlani odemePlani,
            ref decimal anaPara)
        {
            DateTime ilkTarih = odemePlani.VadeBasTar;
            DateTime ikinciTarih = odemePlani.VadeBitTar;
            GecikmeZammiService gecikmeZammiService = new GecikmeZammiService();
            List<GecikmeZammi> gzList = gecikmeZammiService.GetByDateRange(ilkTarih, ikinciTarih);
            if (gzList.Count >= 1)
            {
                foreach (var gecikmeZammi in gzList) //gecikme zammi dongusu
                {
                    // bu sürede ödeme var mi
                    Odeme odemeDao = new Odeme();
                    List<Odeme> odemeList = new Model.Services.TBYS.OdemeService().GetByKiraciVadeBasTarVadeBitTar(kiraSozlesme.Id, kiraSozlesme.KiraciId, ilkTarih, ikinciTarih);
                    if (odemeList.Count > 0)//odeme var
                    {
                        //ödeme yapilan tarihe kadarki olusan faizler
                        foreach (var odeme1 in odemeList)//odeme dongusu
                        {
                            ikinciTarih = odeme1.OdemeTarihi;
                            int aySayisi = (ikinciTarih - ilkTarih).Days >= (odemePlani.VadeBitTar - odemePlani.VadeBasTar).Days ? 1 : 0;
                            int kalangunSayisi = 0;
                            if (aySayisi < 1)
                            {
                                kalangunSayisi = (ikinciTarih.AddDays(1) - ilkTarih).Days;
                                kalangunSayisi = kalangunSayisi < 0 ? 0 : kalangunSayisi;
                            }
                            GecikmeZammiHesabi(kiraSozlesme, odemePlani, ilkTarih, ikinciTarih, odeme1, aySayisi, kalangunSayisi, gecikmeZammi, ref anaPara);
                            ilkTarih = odeme1.OdemeTarihi.AddDays(1);
                        }
                        //son ödemeden vade bitimine kadar kalan anaparanin faizi
                        ilkTarih = ikinciTarih.AddDays(1);
                        ikinciTarih = gecikmeZammi.BitisTarihi > ProjeConstants.NULL_TARIH ? gecikmeZammi.BitisTarihi : odemePlani.VadeBitTar;
                        
                        int gunSayisi = (ikinciTarih.AddDays(1) - ilkTarih).Days;
                        gunSayisi = gunSayisi < 0 ? 0 : gunSayisi;
                        GecikmeZammiHesabi(kiraSozlesme, odemePlani, ilkTarih, ikinciTarih, null, 0, gunSayisi, gecikmeZammi, ref anaPara);
                        
                    }
                    else // hiç ödeme yok
                    {
                        int aySayisi = (ikinciTarih - ilkTarih).Days >= (odemePlani.VadeBitTar - odemePlani.VadeBasTar).Days ? 1 : 0;
                        int gunSayisi = 0;
                        if (aySayisi < 1)
                        {
                            gunSayisi = (ikinciTarih.AddDays(1) - ilkTarih).Days;
                            gunSayisi = gunSayisi < 0 ? 0 : gunSayisi;
                        }
                        GecikmeZammiHesabi(kiraSozlesme, odemePlani, ilkTarih, ikinciTarih, null, aySayisi, gunSayisi, gecikmeZammi, ref anaPara);
                    }

                }
            }
            else //gecikme zammi listesi bos
            {
                MessageHelper.PublishMessage("Gecikme zammi orani bulunamadi, islem yapabilmek için Gecikme Zammi Oranlarini giriniz.", ProjeConstants.MESAJ_HATA);
            }
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
        //public static void GunlukGecikmeZammiHesaplaRecursive(KiraSozlesme kiraSozlesme, OdemePlani odemePlani, DateTime ilkTarih, DateTime ikinciTarih,
        //    ref decimal anaPara, ref decimal faizliBakiyeToplami, ref int sayac)
        //{
        //    // eger recursif fonksiyondan çikmazsa diye
        //    if (sayac++ > 31)
        //    {
        //        MessageHelper.PublishMessage("OdemePlaniId=" + odemePlani.Id + " olan ve " + odemePlani.Ay + " Ayina ait gecikme zammi hesabinda hata olustu.", ProjeConstants.MESAJ_HATA);
        //        //TODO burada ödeme planinin degerleri bos gözüksün
        //        return;
        //    }
        //    IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
        //    GecikmeZammi gzDao = new GecikmeZammi();
        //    List<GecikmeZammi> gzList = gzDao.SelectByBaslangicTarihi(ilkTarih);
        //    if ((gzList.Count == 0) || (ikinciTarih == ilkTarih)) //ikinciTarih == ilkTarih olursa ayin son günü yapilan ödemeyi dikkate almiyor
        //    {
        //        return;
        //    }
        //    else if (gzList.Count >= 1)
        //    {
        //        GecikmeZammi gecikmeZammi = gzList[0];
        //        ilkTarih = (gecikmeZammi.BaslangicTarihi < odemePlani.VadeBasTar) ? (odemePlani.VadeBasTar < ilkTarih ? ilkTarih : odemePlani.VadeBasTar) : gecikmeZammi.BaslangicTarihi;
        //        ikinciTarih = gecikmeZammi.BitisTarihi > ProjeConstants.NULL_TARIH ? (gecikmeZammi.BitisTarihi < odemePlani.VadeBitTar ? gecikmeZammi.BitisTarihi : odemePlani.VadeBitTar) : odemePlani.VadeBitTar;

        //        Odeme odemeDao = new Odeme();
        //        List<Odeme> odemeList = new Model.Services.TBYS.OdemeService().GetByKiraciVadeBasTarVadeBitTar(kiraSozlesme.Id, kiraSozlesme.KiraciId, ilkTarih, ikinciTarih);
        //        if (odemeList.Count >= 1)
        //        {
        //            Odeme odeme = odemeList[0];
        //            if (odeme != null)
        //            {
        //                ikinciTarih = odeme.OdemeTarihi;
        //                int aySayisi = (ikinciTarih - ilkTarih).Days >= (odemePlani.VadeBitTar - odemePlani.VadeBasTar).Days ? 1 : 0;
        //                int gunSayisi = 0;
        //                if (aySayisi < 1)
        //                {
        //                    gunSayisi = (ikinciTarih.AddDays(1) - ilkTarih).Days;
        //                    gunSayisi = gunSayisi < 0 ? 0 : gunSayisi;
        //                }

        //                GecikmeZammiHesabi(kiraSozlesme, odemePlani, ilkTarih, ikinciTarih, odeme, aySayisi, gunSayisi, gecikmeZammi, ref anaPara, ref faizliBakiyeToplami);
        //                if (odeme != null)
        //                {
        //                    ilkTarih = odeme.OdemeTarihi.AddDays(1);
        //                }
        //                else
        //                {
        //                    ilkTarih = gecikmeZammi.BitisTarihi > ProjeConstants.NULL_TARIH ? (odemePlani.VadeBitTar > gecikmeZammi.BitisTarihi ? gecikmeZammi.BitisTarihi.AddDays(1) : odemePlani.VadeBitTar) : odemePlani.VadeBitTar;
        //                }
        //                ikinciTarih = odemePlani.VadeBitTar;
        //                GunlukGecikmeZammiHesaplaRecursive(kiraSozlesme, odemePlani, ilkTarih, ikinciTarih, ref anaPara, ref faizliBakiyeToplami, ref sayac);
        //            }

        //        }
        //        else //odeme yapilmamissa
        //        {
        //            int aySayisi = (ikinciTarih - ilkTarih).Days >= (odemePlani.VadeBitTar - odemePlani.VadeBasTar).Days ? 1 : 0;
        //            int gunSayisi = 0;
        //            if (aySayisi < 1)
        //            {
        //                gunSayisi = (ikinciTarih.AddDays(1) - ilkTarih).Days;
        //                gunSayisi = gunSayisi < 0 ? 0 : gunSayisi;
        //            }

        //            GecikmeZammiHesabi(kiraSozlesme, odemePlani, ilkTarih, ikinciTarih, null, aySayisi, gunSayisi, gecikmeZammi, ref anaPara, ref faizliBakiyeToplami);

        //            ilkTarih = gecikmeZammi.BitisTarihi > ProjeConstants.NULL_TARIH ? (odemePlani.VadeBitTar > gecikmeZammi.BitisTarihi ? gecikmeZammi.BitisTarihi.AddDays(1) : odemePlani.VadeBitTar) : odemePlani.VadeBitTar;
        //            ikinciTarih = odemePlani.VadeBitTar;
        //            GunlukGecikmeZammiHesaplaRecursive(kiraSozlesme, odemePlani, ilkTarih, ikinciTarih, ref anaPara, ref faizliBakiyeToplami, ref sayac);
        //        }

        //    }

        //}

        private static void GecikmeZammiHesabi(KiraSozlesme kiraSozlesme, OdemePlani odemePlani, DateTime ilkTarih, DateTime ikinciTarih, Odeme odeme, int aySayisi, int gunSayisi, GecikmeZammi gz,
            ref decimal anaPara)
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            var gunZamTutari = 0M;
            var ayZamTutari = 0M;
            //var anaPara = odemePlani.AnaPara;
            decimal zamOrani = gz.ZamOrani;
            decimal odenenTutar = odeme == null ? 0 : odeme.OdenenTutar;
            int odemeId = odeme == null ? 0 : odeme.Id;
            DateTime odemeTarihi = odeme == null ? new DateTime() : odeme.OdemeTarihi;
            if (odemePlani.Sira > 0)
            {
                var kalanAnaPara = anaPara + odenenTutar;// + çünkü ana para - geliyor
                ayZamTutari = anaPara * aySayisi * zamOrani / 100;
                gunZamTutari = anaPara * gunSayisi * (zamOrani / 30) / 100;

                var gecikmeZammiTutari = (ayZamTutari + gunZamTutari);

                var ayAciklama = (anaPara).ToString("N", culturInfo) + "TL x " + aySayisi.ToString() + "ay x " + zamOrani.ToString("N", culturInfo) + "/100 ";
                var gunAciklama = (anaPara).ToString("N", culturInfo) + "TL x " + gunSayisi.ToString() + "gün x (" + zamOrani.ToString("N", culturInfo) + "/30)/100 ";
                var aciklama = ayZamTutari < 0 ? ayAciklama : gunZamTutari < 0 ? gunAciklama : "";

                gecikmeZammiTutari = gecikmeZammiTutari > 0 ? 0 : gecikmeZammiTutari;// +faiz olmasin
                OdemeAyrintiKaydetVeyaGuncelle(ilkTarih, ikinciTarih, kiraSozlesme, odemePlani, gz, odemeId, odemeTarihi, odemePlani.VadeBitTar, aySayisi, gunSayisi,
                           anaPara, odenenTutar, kalanAnaPara, gecikmeZammiTutari, aciklama);
                anaPara = kalanAnaPara;
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
        public static bool OdemeYap(KiraSozlesme kiraSozlesme, DateTime odemetarihi, decimal odenenTutar, string aciklama, ref int odemeId)
        {
            odemeId = 0;
            bool odemeYapildiMi = false;
            try
            {
                if (kiraSozlesme != null)
                {
                    DateTime odemeTarihi = odemetarihi;
                    int yil = odemeTarihi.Year;
                    int ay = odemeTarihi.Month;

                    OdemePlani odemePlaniDao = new OdemePlani();
                    List<OdemePlani> list = new Model.Services.TBYS.OdemePlaniService().GetBySozlesmeId(kiraSozlesme.Id);
                    if (list.Count > 0)
                    {
                        OdemePlani odemePlani = list[1];//ilk taksit, lits[0] da devir kaydi var

                        if (odemeTarihi < odemePlani.OdemeBasTar)// ödeme baslama tarihinden önce ödeme yapilmis
                        {
                            odemePlani = new Model.Services.TBYS.OdemePlaniService().GetFirstBySozlesmeId(kiraSozlesme.Id);//odemeyi ilk OdemePlanina kaydet
                            Odeme odeme = new Odeme();
                            odeme = new Model.Services.TBYS.OdemeService().OdemeyiKaydetOdemePlaniniGuncelle(kiraSozlesme, odemePlani, odemeTarihi, odenenTutar, aciklama, UtilityHelper.GetCurrentUserLoginName());
                            odemeYapildiMi = true;
                            odemeId = odeme.Id;
                        }
                        else if (odemeTarihi > odemePlani.OdemeBitTar)//ödeme bitis tarihinden sonra ödeme yapilmis
                        {
                            //
                            // Taksit SB
                            // sözlesme yilliksa ve ödeme sözlesme bitmeden yapilmissa
                            if ((kiraSozlesme.OdemeSekli == ProjeConstants.KIRA_ODMSEKLI_YILLIK) &&
                                odemeTarihi < kiraSozlesme.SozBitTar)
                            {
                                if (kiraSozlesme.SozBitTar >= odemeTarihi)
                                {
                                    odemePlani = new Model.Services.TBYS.OdemePlaniService().GetBySozlesmeIdOdemeTarihi(kiraSozlesme.Id, odemeTarihi);
                                }
                                else //1 taksitte ödenenlerde odemem plani tarihe göre bulunanamayabiliyor o yüzden son takside eklesin
                                {
                                    odemePlani = new Model.Services.TBYS.OdemePlaniService().GetLastBySozlesmeId(kiraSozlesme.Id);  //odemeyi son OdemePlanina kaydet
                                }
                                if (odemePlani == null)
                                    throw (new Exception("Ödeme Plani mevcut degil."));
                                else
                                {
                                    Odeme odeme = new Odeme();
                                    odeme = new Model.Services.TBYS.OdemeService().OdemeyiKaydetOdemePlaniniGuncelle(kiraSozlesme, odemePlani, odemeTarihi, odenenTutar, aciklama, UtilityHelper.GetCurrentUserLoginName());
                                    odemeYapildiMi = true;
                                    odemeId = odeme.Id;
                                }
                            }
                            else

                            {
                                //throw (new Exception("Ödeme Tarihi Sözlesme bitisinden sonra olamaz."));
                                odemePlani = new Model.Services.TBYS.OdemePlaniService().GetLastBySozlesmeId(kiraSozlesme.Id);  //odemeyi son OdemePlanina kaydet
                                if (odemePlani != null)
                                {
                                    Odeme odeme = new Odeme();
                                    odeme = new Model.Services.TBYS.OdemeService().OdemeyiKaydetOdemePlaniniGuncelle(kiraSozlesme, odemePlani, odemeTarihi, odenenTutar, aciklama, UtilityHelper.GetCurrentUserLoginName());
                                    odemeYapildiMi = true;
                                    odemeId = odeme.Id;
                                }
                                else
                                {
                                    throw (new Exception("Ödeme Plani Bulunamadi."));
                                }

                            }

                        }
                        else //odeme bas- bit arasinda yapilmis
                        {
                            odemePlani = new Model.Services.TBYS.OdemePlaniService().GetBySozlesmeIdOdemeTarihi(kiraSozlesme.Id, odemeTarihi);
                            if (odemePlani == null)
                            {
                                odemePlani = new Model.Services.TBYS.OdemePlaniService().GetLastBySozlesmeId(kiraSozlesme.Id);  //odemeyi son OdemePlanina kaydet
                            }
                            if (odemePlani == null)
                                throw (new Exception("Ödeme Plani mevcut degil."));
                            else
                            {
                                Odeme odeme = new Odeme();
                                odeme = new Model.Services.TBYS.OdemeService().OdemeyiKaydetOdemePlaniniGuncelle(kiraSozlesme, odemePlani, odemeTarihi, odenenTutar, aciklama, UtilityHelper.GetCurrentUserLoginName());
                                odemeYapildiMi = true;
                                odemeId = odeme.Id;
                            }
                        }

                    }
                    else //odemePlani listesi bos
                        throw (new Exception("Ödeme Plani mevcut degil."));
                }
                else //kiraSozlesme null
                    throw (new Exception("Kira Sözlesmesi mevcut degil."));
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.Exceptions.Add(new Exception("Ödeme Yapilamadi."));
                exHelper.PublishException();
            }
            return odemeYapildiMi;
        }

        public static bool OdemeYap(KiraSozlesme kiraSozlesme, DateTime odemetarihi, decimal odenenTutar, string aciklama, ref int odemeId, PortalLogContext context)
        {
            odemeId = 0;
            if (kiraSozlesme == null || context == null) return false;
            try
            {
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
                    odemeId = odeme.Id;
                    transaction.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.Exceptions.Add(new Exception("Ödeme Yapılamadı."));
                exHelper.PublishException();
                return false;
            }
        }

        public static void TeminatIslemleriniHesaplaVeKaydet(KiraSozlesme kiraSozlesme)
        {
            decimal toplamOdeme = 0m;
            decimal toplamIade = 0m;
            TeminatIslem teminatIslemDao = new TeminatIslem();
            DataTable dataTable = new Model.Services.TBYS.TeminatIslemService().GetSumByKiraciIdGroupByType(kiraSozlesme.KiraciId);
            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    string islemTipi = row["IslemTipi"].ToString();
                    decimal islemToplami = row["IslemToplami"].ReturnZeroIfNull().ConvertToDecimal();


                    switch (islemTipi)
                    {
                        case ProjeConstants.TEMINAT_ODEMESI:
                            {
                                toplamOdeme += islemToplami;
                                break;
                            }
                        case ProjeConstants.TEMINAT_GECICITEMINATODEMESI:
                            {
                                toplamOdeme += islemToplami;
                                break;
                            }
                        case ProjeConstants.TEMINAT_KIRACIYAIADE:
                            {
                                toplamIade += islemToplami;
                                break;
                            }
                        case ProjeConstants.TEMINAT_KIRAYAMAHSUP:
                            {
                                toplamIade += islemToplami;
                                break;
                            }
                        case ProjeConstants.TEMINAT_HASARAMAHSUP:
                            {
                                toplamIade += islemToplami;
                                break;
                            }
                        case ProjeConstants.TEMINAT_AIDATAMAHSUP:
                            {
                                toplamIade += islemToplami;
                                break;
                            }
                        case ProjeConstants.TEMINAT_VAKFABAGIS:
                            {
                                toplamIade += islemToplami;
                                break;
                            }
                        default:
                            break;
                    }


                }

            }
            kiraSozlesme.OdenenTeminatTutari = toplamOdeme;
            kiraSozlesme.IadeTeminatTutari = toplamIade;
            kiraSozlesme.KalanTeminatTutari = toplamOdeme - toplamIade;
            new Model.Services.TBYS.KiraSozlesmeService().Update(kiraSozlesme);
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

        private static bool TeminatIslemleriniHesaplaVeKaydet(KiraSozlesme kiraSozlesme, PortalLogContext context, SqlTransactionContext transaction)
        {
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
