using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Model.IKYS;
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

        /// <summary>
        /// KiraSozlesmesini parametre olarak alır.
        /// Bu KSnin ödeme planında;
        ///     Gecikme varsa geciken her ayın KiraBedeli'ne işlem yapılan güne kadar yasal FaizOranına Göre faizTutarı oluşturur
        ///     Faiz tutarını Faizlibakiyeye ekler
        ///     AnaPara gecikmesine faiz oluşturur, eski faizlere tekrar faiz uygulamaz
        /// Fesih durumunda vadesi gelmeyen aların hesaplanan değerlerini 0 yapar
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
            List<OdemePlani> list = odemePlaniDao.SelectBySozlesmeId(kiraSozlesme.Id);
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
            DateTime islemTarihi = kiraSozlesme.SozBitTar;//today; SB 24.02.2021 Sözleşmenin son ayına kadar faiz hesabını yapsın  Zekayi Bey ileriye dönük simülasyon gibi kullanmak istiyor 
            foreach (OdemePlani odemePlani in list)
            {
                if (odemePlani.Sira == 0)
                {
                    continue;//devir satırı için hesap yapma
                }
                else
                {
                    DateTime vadeBasTar = odemePlani.VadeBasTar;//vade tarihinden itibaren
                    #region Yasal Faiz Kontrolü
                    YasalFaiz yasalFaiz = new YasalFaiz();
                    yasalFaiz = yasalFaiz.SelectByYilAy(vadeBasTar.Year, vadeBasTar.Month);
                    if (yasalFaiz == null)
                    {
                        Exception ex1 = new Exception(vadeBasTar.Month + "/" + vadeBasTar.Year + " Ayı için Yasal Faiz Oranı girilmediğinden Faiz Hesaplanamıyor.");
                        Exception ex2 = new Exception("Yasal faiz menusunden " + vadeBasTar.Year + " yılı için faiz oranı girdikten sonra tekrar deneyiniz");
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

                            if (islemTarihi.AddMonths(-1) < vadeBasTar && islemTarihi >= vadeBasTar) // işlem tarihi  önceki vadeBastarihinden büyük ve şimdiki vade tarihinden küçük eşitse // 
                            {

                                faizTutari = 0;
                            }

                            faizTutari = faizTutari > 0 ? 0 : faizTutari;//artı faiz olmasın, kiracının alacağı varsa faiz uyulamasın 
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
                                //devir satırını güncellemesin
                            }
                            else
                            {
                                //anaparadan artan varsa faiz ile mahsuplaş
                                if (anaParaToplami > 0 && faizToplami < 0)
                                {
                                    decimal fark = anaParaToplami + faizToplami;
                                    if (fark > 0)//anaparadan faiz çıkınca artan miktar var
                                    {

                                        anaParaToplami = anaParaToplami + faizToplami;
                                        anaParaToplami = anaParaToplami < 0 ? 0 : anaParaToplami;
                                        faizToplami = 0;
                                    }
                                    else if (fark < 0)//anaparadan faiz çıkınca hala faiz borcu var var 
                                    {

                                        faizToplami = anaParaToplami + faizToplami;
                                        faizToplami = faizToplami > 0 ? 0 : faizToplami;
                                        anaParaToplami = 0;
                                    }
                                    else if (fark == 0)//anaparadan faiz çıkınca kalan 0
                                    {
                                        anaParaToplami = 0;
                                        faizToplami = 0;
                                    }
                                }

                                Odeme odemeDao = new Odeme();
                                var odenenTutar = odemeDao.SelectSumBySozlesmeIdOdemePlaniId(kiraSozlesme.Id, odemePlani.Id);
                                odemePlani.OdenenTutar = odenenTutar;
                                odemePlani.FaizliBakiye = faizliBakiye;
                                odemePlani.FaizOrani = faizOrani;
                                odemePlani.FaizTutari = faizTutari;
                                odemePlani.AnaPara = anaParaToplami;
                                odemePlani.Update();
                            }
                            #endregion
                        }
                    }
                }

            }

        }
        public static void AktifSozleslemelerinBakiyeBorcunuHesapla()
        {
            DateTime saat = DateTime.Now;
            KiraSozlesme kiraSozlesmeDao = new KiraSozlesme();
            List<KiraSozlesme> kiraSozlesmeListesi = kiraSozlesmeDao.SelectAllAktifSozlesme();
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
            odemeAyrinti.DeleteBySozlesmeId(kiraSozlesme.Id);

            //bu kiracıya ait tüm ödeme planlarını al tarih sıralı
            OdemePlani op = new OdemePlani();
            List<OdemePlani> opList = op.SelectBySozlesmeId(kiraSozlesme.Id);
            var faizliBakiyeToplami = 0m;
            var anaParaBakiye = 0m;
            foreach (var odemePlani in opList)
            {
                if (odemePlani.Sira == 0)
                {
                    
                    anaParaBakiye = odemePlani.AnaPara;
                    faizliBakiyeToplami = odemePlani.FaizliBakiye;
                    continue;
                }
                int sayac = 0;
                GunlukGecikmeZammiHesaplaRecursive(kiraSozlesme, odemePlani, odemePlani.VadeBasTar, odemePlani.VadeBitTar, ref anaParaBakiye, ref faizliBakiyeToplami, ref sayac);
                OdemeAyrinti odemeAyrintiDao = new OdemeAyrinti();
                anaParaBakiye = anaParaBakiye - odemePlani.KiraBedeli;
                var gecikmeZammiTutari = odemeAyrintiDao.SelectSumGecikmeZammiTutariByOdemePlaniId(odemePlani.Id);
                var gecikmeZammiOrani = odemeAyrintiDao.SelectSonGecikmeZammiTutariByOdemePlaniId(odemePlani.Id);

                odemePlani.AnaPara = anaParaBakiye;
                odemePlani.FaizTutari = gecikmeZammiTutari;
                odemePlani.FaizliBakiye = anaParaBakiye + faizliBakiyeToplami;
                odemePlani.FaizOrani = gecikmeZammiOrani;

                //ödeme planı Id ye ve sozId ye göre al
                Odeme odemeDao = new Odeme();
                var odenenTutar = odemeDao.SelectSumBySozlesmeIdOdemePlaniId(kiraSozlesme.Id, odemePlani.Id);
                odemePlani.OdenenTutar = odenenTutar;
                odemePlani.Update();
                if ((DateTime.Today > odemePlani.VadeBasTar) && (DateTime.Today < odemePlani.VadeBitTar))
                {
                    break;
                }
            }
        }
        private static void OdemeAyrintiKaydetVeyaGuncelle(DateTime ilkTarih, DateTime sonTarih, KiraSozlesme kiraSozlesme, OdemePlani odemePlani, GecikmeZammi gz, int odemeId, DateTime odemeTarihi,
            DateTime sonOdemeTarihi, int aySayisi, int gunSayisi,
            decimal anaPara, decimal odenenTutar, decimal kalanAnaPara, decimal gecikmeZammiTutari, string aciklama)
        {
            string saveOrUpdate = "update";

            OdemeAyrinti odemeAyrinti = new OdemeAyrinti();
            odemeAyrinti = odemeAyrinti.Select(kiraSozlesme, odemePlani.Id, gz.Id, odemeId);
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
                odemeAyrinti.Save();
            }
            else
            {
                odemeAyrinti.Update();
            }

        }
        public static void GunlukGecikmeZammiHesaplaRecursive(KiraSozlesme kiraSozlesme, OdemePlani odemePlani, DateTime ilkTarih, DateTime ikinciTarih,
            ref decimal anaPara, ref decimal faizliBakiyeToplami, ref int sayac)
        {
            //burası sigorta, eğer recursif fonksiyondan çıkmazsa diye
            if (sayac++ > 31)
            {
                MessageHelper.PublishMessage("OdemePlanıId=" + odemePlani.Id + " olan ve " + odemePlani.Ay + " Ayına ait gecikme zammı hesabında hata oluştu.", ProjeConstants.MESAJ_HATA);
                //TODO burada ödeme planının değerleri boş gözüksün
                return;
            }
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            GecikmeZammi gzDao = new GecikmeZammi();
            List<GecikmeZammi> gzList = gzDao.SelectByBaslangicTarihi(ilkTarih);
            if ((gzList.Count == 0) || (ikinciTarih == ilkTarih)) //ikinciTarih == ilkTarih olursa ayın son günü yapılan ödemeyi dikkate almıyor
            {
                return;
            }
            else if (gzList.Count >= 1)
            {
                GecikmeZammi gecikmeZammi = gzList[0];
                ilkTarih = (gecikmeZammi.BaslangicTarihi < odemePlani.VadeBasTar) ? (odemePlani.VadeBasTar < ilkTarih ? ilkTarih : odemePlani.VadeBasTar) : gecikmeZammi.BaslangicTarihi;
                ikinciTarih = gecikmeZammi.BitisTarihi > ProjeConstants.NULL_TARIH ? (gecikmeZammi.BitisTarihi < odemePlani.VadeBitTar ? gecikmeZammi.BitisTarihi : odemePlani.VadeBitTar) : odemePlani.VadeBitTar;

                Odeme odemeDao = new Odeme();
                List<Odeme> odemeList = odemeDao.SelectByKiraciVadeBasTarVadeBitTar(kiraSozlesme.Id, kiraSozlesme.KiraciId, ilkTarih, ikinciTarih);
                if (odemeList.Count >= 1)
                {
                    Odeme odeme = odemeList[0];
                    if (odeme != null)
                    {
                        ikinciTarih = odeme.OdemeTarihi;
                        int aySayisi = (ikinciTarih - ilkTarih).Days >= (odemePlani.VadeBitTar - odemePlani.VadeBasTar).Days ? 1 : 0;
                        int gunSayisi = 0;
                        if (aySayisi < 1)
                        {
                            gunSayisi = (ikinciTarih.AddDays(1) - ilkTarih).Days;
                            gunSayisi = gunSayisi < 0 ? 0 : gunSayisi;
                        }

                        GecikmeZammiHesabi(kiraSozlesme, odemePlani, ilkTarih, ikinciTarih, odeme, aySayisi, gunSayisi, gecikmeZammi, ref anaPara, ref faizliBakiyeToplami);
                        if (odeme != null)
                        {
                            ilkTarih = odeme.OdemeTarihi.AddDays(1);
                        }
                        else
                        {
                            ilkTarih = gecikmeZammi.BitisTarihi > ProjeConstants.NULL_TARIH ? (odemePlani.VadeBitTar > gecikmeZammi.BitisTarihi ? gecikmeZammi.BitisTarihi.AddDays(1) : odemePlani.VadeBitTar) : odemePlani.VadeBitTar;
                        }
                        ikinciTarih = odemePlani.VadeBitTar;
                        GunlukGecikmeZammiHesaplaRecursive(kiraSozlesme, odemePlani, ilkTarih, ikinciTarih, ref anaPara, ref faizliBakiyeToplami, ref sayac);
                    }

                }
                else //odeme yapılmamışsa
                {
                    int aySayisi = (ikinciTarih - ilkTarih).Days >= (odemePlani.VadeBitTar - odemePlani.VadeBasTar).Days ? 1 : 0;
                    int gunSayisi = 0;
                    if (aySayisi < 1)
                    {
                        gunSayisi = (ikinciTarih.AddDays(1) - ilkTarih).Days;
                        gunSayisi = gunSayisi < 0 ? 0 : gunSayisi;
                    }

                    GecikmeZammiHesabi(kiraSozlesme, odemePlani, ilkTarih, ikinciTarih, null, aySayisi, gunSayisi, gecikmeZammi, ref anaPara, ref faizliBakiyeToplami);
                    
                    ilkTarih = gecikmeZammi.BitisTarihi > ProjeConstants.NULL_TARIH ? (odemePlani.VadeBitTar > gecikmeZammi.BitisTarihi ? gecikmeZammi.BitisTarihi.AddDays(1) : odemePlani.VadeBitTar) : odemePlani.VadeBitTar;
                    ikinciTarih = odemePlani.VadeBitTar;
                    GunlukGecikmeZammiHesaplaRecursive(kiraSozlesme, odemePlani, ilkTarih, ikinciTarih, ref anaPara, ref faizliBakiyeToplami, ref sayac);
                }

            }

        }
       
        private static void GecikmeZammiHesabi(KiraSozlesme kiraSozlesme, OdemePlani odemePlani, DateTime ilkTarih, DateTime ikinciTarih, Odeme odeme, int aySayisi, int gunSayisi, GecikmeZammi gz,
            ref decimal anaPara, ref decimal faizliBakiyeToplami)
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

                gecikmeZammiTutari = gecikmeZammiTutari > 0 ? 0 : gecikmeZammiTutari;// +faiz olmasın
                OdemeAyrintiKaydetVeyaGuncelle(ilkTarih, ikinciTarih, kiraSozlesme, odemePlani, gz, odemeId, odemeTarihi, odemePlani.VadeBitTar, aySayisi, gunSayisi,
                           anaPara, odenenTutar, kalanAnaPara, gecikmeZammiTutari, aciklama);
                faizliBakiyeToplami += gecikmeZammiTutari;
                anaPara = kalanAnaPara;
            }


        }
        public static bool OdemeYap(KiraSozlesme kiraSozlesme,DateTime odemetarihi, decimal odenenTutar, string aciklama, ref int odemeId)
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
                    List<OdemePlani> list = odemePlaniDao.SelectBySozlesmeId(kiraSozlesme.Id);
                    if (list.Count > 0)
                    {
                        OdemePlani odemePlani = list[1];//ilk taksit, lits[0] da devir kaydı var

                        if (odemeTarihi < odemePlani.OdemeBasTar)// ödeme başlama tarihinden önce ödeme yapılmış
                        {
                            odemePlani = odemePlani.SelectIlkOdemePlaniBySozlesmeId(kiraSozlesme.Id);//odemeyi ilk OdemePlanina kaydet 
                            Odeme odeme = new Odeme();
                            odeme = odeme.OdemeyiKaydetOdemePlaniniGuncelle(kiraSozlesme, odemePlani, odemeTarihi, odenenTutar, aciklama, UtilityHelper.GetCurrentUser());
                            odemeYapildiMi = true;
                            odemeId = odeme.Id;
                        }
                        else if (odemeTarihi > odemePlani.OdemeBitTar)//ödeme bitiş tarihinden sonra ödeme yapılmış
                        {
                            //
                            // Taksit SB
                            // sözleşme yıllıksa ve ödeme sözleşme bitmeden yapılmışsa
                            if ((kiraSozlesme.OdemeSekli == ProjeConstants.KIRA_ODMSEKLI_YILLIK) &&
                                odemeTarihi < kiraSozlesme.SozBitTar)
                            {
                                if (kiraSozlesme.SozBitTar >= odemeTarihi)
                                {
                                    odemePlani = odemePlani.SelectBySozlesmeIdOdemeTarihi(kiraSozlesme.Id, odemeTarihi);
                                }
                                else //1 taksitte ödenenlerde odemem plani tarihe göre bulunanamayabiliyor o yüzden son takside eklesin
                                {
                                    odemePlani = odemePlani.SelectSonOdemePlaniBySozlesmeId(kiraSozlesme.Id);  //odemeyi son OdemePlanina kaydet   
                                }
                                if (odemePlani == null)
                                    throw (new Exception("Ödeme Planı mevcut değil."));
                                else
                                {
                                    Odeme odeme = new Odeme();
                                    odeme = odeme.OdemeyiKaydetOdemePlaniniGuncelle(kiraSozlesme, odemePlani, odemeTarihi, odenenTutar, aciklama, UtilityHelper.GetCurrentUser());
                                    odemeYapildiMi = true;
                                    odemeId = odeme.Id;
                                }
                            }
                            else

                            {
                                //throw (new Exception("Ödeme Tarihi Sözleşme bitişinden sonra olamaz."));
                                odemePlani = odemePlani.SelectSonOdemePlaniBySozlesmeId(kiraSozlesme.Id);  //odemeyi son OdemePlanina kaydet   
                                if (odemePlani != null)
                                {
                                    Odeme odeme = new Odeme();
                                    odeme = odeme.OdemeyiKaydetOdemePlaniniGuncelle(kiraSozlesme, odemePlani, odemeTarihi, odenenTutar, aciklama, UtilityHelper.GetCurrentUser());
                                    odemeYapildiMi = true;
                                    odemeId = odeme.Id;
                                }
                                else
                                {
                                    throw (new Exception("Ödeme Planı Bulunamadı."));
                                }

                            }

                        }
                        else //odeme baş- bit arasında yapılmış
                        {
                            odemePlani = odemePlani.SelectBySozlesmeIdOdemeTarihi(kiraSozlesme.Id, odemeTarihi);
                            if (odemePlani == null)
                            {
                                odemePlani = odemePlani.SelectSonOdemePlaniBySozlesmeId(kiraSozlesme.Id);  //odemeyi son OdemePlanina kaydet   
                            }
                            if (odemePlani == null)
                                throw (new Exception("Ödeme Planı mevcut değil."));
                            else
                            {
                                Odeme odeme = new Odeme();
                                odeme = odeme.OdemeyiKaydetOdemePlaniniGuncelle(kiraSozlesme, odemePlani, odemeTarihi, odenenTutar, aciklama, UtilityHelper.GetCurrentUser());
                                odemeYapildiMi = true;
                                odemeId = odeme.Id;
                            }
                        }

                    }
                    else //odemePlani listesi boş
                        throw (new Exception("Ödeme Planı mevcut değil."));
                }
                else //kiraSozlesme null
                    throw (new Exception("Kira Sözleşmesi mevcut değil."));
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.Exceptions.Add(new Exception("Ödeme Yapılamadı."));
                exHelper.PublishException();
            }
            return odemeYapildiMi;
        }

        public static void TeminatIslemleriniHesaplaVeKaydet(KiraSozlesme kiraSozlesme)
        {
            decimal toplamOdeme = 0m;
            decimal toplamIade = 0m;
            TeminatIslem teminatIslemDao = new TeminatIslem();
            DataTable dataTable = teminatIslemDao.SelectSumIslemTutariByKiraciIdGroupByIslemTipi(kiraSozlesme.KiraciId);
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
            kiraSozlesme.Update();
        }
        #endregion TBYS

    }
}
