using Model.NBYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.Services.NBYS
{
    public static class EkstreAktarmaImportService
    {
        private static int SaveEkstreAktarma(EkstreAktarma item)
        {
            if (item.NakitBagisciId <= 0)
                item.NakitBagisciId = new EkstreBagisciEslestirmeService().FindDonorId(item);
            return new EkstreAktarmaService().Save(item);
        }

        public static ExceptionHelper SaveAkBankFile(Stream fileStream, DateTime processTime, string currentUser)
        {
            ExceptionHelper exceptionHelper = new ExceptionHelper();
            CultureInfo culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            List<string> lines = HelperFunctions.ConvertTextFileToList(fileStream);
            foreach (var line in lines)
            {

                try
                {
                    EkstreAktarma ekstreAktarma = new EkstreAktarma();
                    //var fisNo = line.Substring(51, 72 - 51).TrimEnd();
                    var ad = line.Substring(51, 72 - 51).TrimEnd();
                    if (!string.IsNullOrEmpty(ad)) //en son satırı almamak için
                    {
                        var soyisim = line.Substring(72, 93 - 73).TrimEnd();
                        var tc = line.Substring(93, 105 - 93).TrimEnd();
                        var tel = line.Substring(105, 123 - 105).TrimEnd();
                        var adres = line.Substring(123, 178 - 123).TrimEnd();
                        var postaKodu = line.Substring(178, 183 - 178).TrimEnd();
                        var ülke = line.Substring(183, 7).TrimEnd();
                        var tutarTl = line.Substring(230, 15).TrimEnd();//TODO: sonunda bir sıfır fazla mı? yoksa son iki rakam küsürat mi? sorulacak

                        var tutarKr = line.Substring(245, 2).TrimEnd();
                        var tutar = tutarTl + "," + tutarKr;
                        var eposta = line.Substring(247, 292 - 252).TrimEnd();
                        var tarih = line.Substring(292, 302 - 292).TrimEnd();
                        var aciklama = line.Substring(310, 350 - 310).TrimEnd();

                        ekstreAktarma.TCKimlikNo = tc.ConvertToLong();
                        ekstreAktarma.Adi = string.Format("{0} {1}", ad.Substring(1, ad.Length - 1), soyisim).ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo);

                        ekstreAktarma.Adres = adres.ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo);
                        ekstreAktarma.Telefon1 = UtilityHelper.TelefonFormatla(tel.ReturnEmptyIfNull().ToString());
                        ekstreAktarma.Tutar = tutar.ConvertToDecimal();
                        ekstreAktarma.Aciklama = aciklama;
                        ekstreAktarma.BelgeIstemiyor = ekstreAktarma.Aciklama.Contains(ProjeConstants.DURUM_BELGE_ISTEMIYOR) || ekstreAktarma.Adres.Contains(ProjeConstants.DURUM_BELGE_ISTEMIYOR) ? true : false;
                        ekstreAktarma.BagisTarihi = ConvertAkbankTextToDateTime(tarih);
                        ekstreAktarma.AktarildiMi = false;
                        ekstreAktarma.BankaAdi = ProjeConstants.BANKA_AKBANK;
                        ekstreAktarma.PostaKodu = postaKodu;
                        ekstreAktarma.Eposta = eposta;
                        ekstreAktarma.IslemTarihi = processTime;
                        ekstreAktarma.DovizCinsi = ProjeConstants.DOVIZ_TL;
                        ekstreAktarma.Olusturan = currentUser;
                        //ekstreAktarma.FisNo = fisNo;
                        SaveEkstreAktarma(ekstreAktarma);

                    }
                }
                catch (Exception ex)
                {
                    Exception exceprion = new Exception(string.Format("HATA SATIRI {0}:{1} ->", ProjeConstants.BANKA_AKBANK, line), ex);
                    exceptionHelper.Exceptions.Add(exceprion);
                }

            }
            return exceptionHelper;
        }
        public static ExceptionHelper SaveAkBankEkstreFile(Stream fileStream, DateTime islemTarihi, string currentUser)
        {
            ExceptionHelper exceptionHelper = new ExceptionHelper();
            CultureInfo culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            try
            {

                var akbankEkstreData = ExcelHelper.ReadXLSXAsDataTable(fileStream, ProjeConstants.BANKA_AKBANKEKSTRE_ILKKACSATIRHARIC, ProjeConstants.BANKA_AKBANKEKSTRE_SONKACSATIRHARIC, true);
                if (akbankEkstreData != null)
                {
                    foreach (DataRow row in akbankEkstreData.Rows)
                    {
                        EkstreAktarma ekstreAktarma = new EkstreAktarma();
                        var tarih = row[0].ReturnEmptyIfNull().ToString();//borc satiri için islem yapma
                        var aciklama = row[5].ReturnEmptyIfNull().ToString();
                        if (string.IsNullOrEmpty(tarih))// ilk kolon bos ise dosya bitti çik
                            break;
                        try
                        {
                            var fisNo = row[6].ToString().Trim();
                            var tutar = row[2].ReturnEmptyIfNull().ToString().Replace(".", ",").ConvertToDecimal();//row[6].ReturnEmptyIfNull().ToString();
                            var bagisTarihi = row[0].ReturnEmptyIfNull().ToString().ConvertToDatetime();
                            if (BuKayitDahaOnceGirilmisMiByFisNo(//ProjeConstants.BANKA_AKBANKEKSTRE, fisNo, aciklama)) //fis numarasindan kontrol et kayit girilmisse atla
                                "Akbank", fisNo, aciklama, bagisTarihi, tutar)) //fis numarasindan kontrol et kayit girilmisse atla
                            {
                                continue;
                            }
                            else
                            {
                                if (tutar > 0)
                                {
                                    ekstreAktarma.Tutar = tutar.ConvertToDecimal();
                                    ekstreAktarma.BagisTarihi = bagisTarihi;
                                    ekstreAktarma.Aciklama = aciklama;
                                    ekstreAktarma.BelgeIstemiyor = ekstreAktarma.Aciklama.Contains(ProjeConstants.DURUM_BELGE_ISTEMIYOR) ? true : false;
                                    ekstreAktarma.BankaAdi = ProjeConstants.BANKA_AKBANKEKSTRE;
                                    ekstreAktarma.IslemTarihi = islemTarihi;
                                    ekstreAktarma.DovizCinsi = ProjeConstants.DOVIZ_TL;
                                    ekstreAktarma.Olusturan = currentUser;
                                    ekstreAktarma.FisNo = fisNo;
                                    SaveEkstreAktarma(ekstreAktarma);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Exception exception = new Exception(string.Format("HATA SATIRI {0}:{1} ->", ProjeConstants.BANKA_AKBANKEKSTRE, ""), ex);
                            exceptionHelper.Exceptions.Add(exception);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                exceptionHelper.Exceptions.Add(ex);
            }
            return exceptionHelper;
        }
        public static ExceptionHelper SaveGarantiBankFile(Stream filePath, DateTime processTime, string currentUser)
        {
            ExceptionHelper exceptionHelper = new ExceptionHelper();
            CultureInfo culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            List<string> lines = HelperFunctions.ConvertTextFileToList(filePath);
            int count = 0;
            var tarih = string.Empty;
            foreach (var line in lines)
            {
                //if (count == 0)//ilk satirda tarih bilgisi var
                //{
                //    tarih = line.Substring(10, 8);
                //}
                if (count > 0 && count < (lines.Count - 1)) //ilk satir ve son satir alinmayacak
                {
                    try
                    {
                        EkstreAktarma ekstreAktarma = new EkstreAktarma();

                        var ad = line.Substring(1, 30).TrimEnd();

                        if (!string.IsNullOrEmpty(ad)) //en son satiri almamak için
                        {
                            var adres = line.Substring(30, 61).TrimEnd();
                            var il = line.Substring(91, 20).TrimEnd();
                            var ilce = line.Substring(111, 20).TrimEnd();
                            // var ulke = line.Substring(131, 15).TrimEnd();
                            var postaKodu = line.Substring(146, 9).TrimEnd();
                            var tel = line.Substring(155, 15).TrimEnd();
                            var eposta = line.Substring(170, 30).TrimEnd();
                            //var tutar = line.Substring(415, 10).TrimEnd();

                            var tutarTl = line.Substring(415, 7).TrimEnd();
                            var tutarTl0 = string.IsNullOrEmpty(tutarTl.Trim()) ? "0" : tutarTl;
                            if (tutarTl0.Contains(","))
                            {
                                tutarTl0 = tutarTl0.Replace(",", "");
                            }
                            var tutarKr = line.Substring(423, 2).TrimEnd();
                            var tutar = tutarTl0 + "," + tutarKr;

                            var dovizCinsi = line.Substring(425, 2).TrimEnd();
                            var aciklama = line.Substring(427, 51).TrimEnd();
                            var tc = line.Substring(499, 11).TrimStart();
                            tarih = line.Substring(401, 8).Trim();

                            if (tutar.Contains('.') && !tutar.Contains(','))
                            {
                                tutar = tutar.Replace('.', ',');
                            }

                            ekstreAktarma.TCKimlikNo = tc.ConvertToLong();
                            ekstreAktarma.Adi = ad.ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo); ;
                            ekstreAktarma.Adres = adres.ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo); ;
                            ekstreAktarma.Telefon1 = UtilityHelper.TelefonFormatla(tel.ReturnEmptyIfNull().ToString()); ;
                            ekstreAktarma.Tutar = tutar.ConvertToDecimal();
                            ekstreAktarma.Aciklama = aciklama;
                            ekstreAktarma.BelgeIstemiyor = ekstreAktarma.Aciklama.Contains(ProjeConstants.DURUM_BELGE_ISTEMIYOR) || ekstreAktarma.Adres.Contains(ProjeConstants.DURUM_BELGE_ISTEMIYOR) ? true : false;
                            ekstreAktarma.BagisTarihi = ConverYYYMMDDToDateTime(tarih);
                            ekstreAktarma.AktarildiMi = false;
                            ekstreAktarma.BankaAdi = ProjeConstants.BANKA_GARANTI;
                            ekstreAktarma.PostaKodu = postaKodu;
                            ekstreAktarma.Eposta = eposta;
                            ekstreAktarma.DovizCinsi = dovizCinsi?? ProjeConstants.DOVIZ_TL;
                            ekstreAktarma.Ili = il;
                            ekstreAktarma.Ilcesi = ilce;
                            ekstreAktarma.IslemTarihi = processTime;
                            ekstreAktarma.Olusturan = currentUser;

                            SaveEkstreAktarma(ekstreAktarma);

                        }
                    }
                    catch (Exception ex)
                    {
                        Exception exceprion = new Exception(string.Format("HATA SATIRI {0}:{1} ->", ProjeConstants.BANKA_GARANTI, line), ex);
                        exceptionHelper.Exceptions.Add(exceprion);
                    }
                }
                count++;
            }
            return exceptionHelper;
        }
        public static ExceptionHelper SaveGarantiEkstreFile(Stream fileStream, DateTime processTime, string currentUser)
        {
            ExceptionHelper exceptionHelper = new ExceptionHelper();
            CultureInfo culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);


            var garantiEkstreData = ExcelHelper.ReadXLSXAsDataTable(fileStream, ProjeConstants.BANKA_GARANTIEKSTRE_ILKKACSATIRHARIC, ProjeConstants.BANKA_GARANTIEKSTRE_SONKACSATIRHARIC, true);


            if (garantiEkstreData != null)
            {

                foreach (DataRow row in garantiEkstreData.Rows)
                {
                    EkstreAktarma ekstreAktarma = new EkstreAktarma();
                    try
                    {
                        string tarihStr = row[0].ReturnZeroIfNull().ToString();
                        if (tarihStr.Contains("Tarih"))
                        {
                            continue;
                        }
                        else
                        {
                            //long tarihLong = long.Parse(tarihStr.Substring(0, tarihStr.IndexOf(".")));
                            //DateTime bagisTarihi = DateTime.FromOADate(tarihLong);
                            DateTime bagisTarihi = tarihStr.ConvertToDatetime();
                            var detay = row[1].ReturnEmptyIfNull().ToString();
                            string tutar = row[3].ReturnZeroIfNull().ToString().Replace(".", ",");
                            if (detay.Contains("TSKGÇL BAĞIŞI") || tutar.ConvertToDecimal() < 0
                                || detay.Contains("901.00114.98.00012")//stopaj kesintisi
                                || detay.Contains("22100000000")//swap açilis kapanis
                                || detay.Contains("EF3077117 TÜRK SİLAHLI")) //vakif hesabina virman
                            {
                                continue;
                            }

                            string adi = string.Empty;
                            var adres = string.Empty;// adres bilgisi gelmiyor

                            if (detay.Contains("--HVL-CEP ŞUBE"))
                            {
                                adi = detay.Substring(0, detay.IndexOf("--HVL-CEP ŞUBE")).Trim();
                            }
                            else if (detay.Contains("-FAST-"))
                            {
                                adi = detay.Substring(0, detay.IndexOf("-FAST-")).Trim();
                            }
                            else if (detay.Contains("-bağış-HVL-CEP ŞUBE"))
                            {
                                adi = detay.Substring(0, detay.IndexOf("-bağış-HVL-CEP ŞUBE")).Trim();
                            }
                            else if (detay.Contains("CEP ŞUBE-HVL-Bağış -"))
                            {
                                var splitText = new string[] { "CEP ŞUBE-HVL-Bağış -" };
                                var holder = detay.Split(splitText, StringSplitOptions.None);
                                adi = holder[1].ReturnEmptyIfNull().ToString().TrimEnd();
                            }
                            else if (detay.Contains("CEP ŞUBE-HVL-BAĞIŞ -"))
                            {
                                var splitText = new string[] { "CEP ŞUBE-HVL-BAĞIŞ -" };
                                var holder = detay.Split(splitText, StringSplitOptions.None);
                                adi = holder[1].ReturnEmptyIfNull().ToString().TrimEnd();
                            }
                            else if (detay.Contains("CEP ŞUBE-HVL--"))
                            {
                                var splitText = new string[] { "CEP ŞUBE-HVL--" };
                                var holder = detay.Split(splitText, StringSplitOptions.None);
                                adi = holder[1].ReturnEmptyIfNull().ToString().TrimEnd();
                            }
                            else if (detay.Contains("CEP ŞUBE-HVL-"))
                            {
                                char splitText = '-';
                                var holder = detay.Split(splitText);
                                adi = holder[3].ReturnEmptyIfNull().ToString().TrimEnd();
                                int result;
                                if (int.TryParse(adi, out result))
                                {
                                    adi = string.Empty;
                                }
                            }
                            else if (detay.Contains("INT-HVL-bağış -"))
                            {
                                var splitText = new string[] { "INT-HVL-bağış -" };
                                var holder = detay.Split(splitText, StringSplitOptions.None);
                                adi = holder[1].ReturnEmptyIfNull().ToString().TrimEnd();
                            }
                            else if (detay.Contains("INT-HVL-BAĞIŞ -"))
                            {
                                var splitText = new string[] { "INT-HVL-BAĞIŞ -" };
                                var holder = detay.Split(splitText, StringSplitOptions.None);
                                adi = holder[1].ReturnEmptyIfNull().ToString().TrimEnd();
                            }
                            else if (detay.Contains("INT-HVL--"))
                            {
                                var splitText = new string[] { "INT-HVL--" };
                                var holder = detay.Split(splitText, StringSplitOptions.None);
                                adi = holder[1].ReturnEmptyIfNull().ToString().TrimEnd();
                            }
                            else if (detay.Contains("INT-HVL-"))
                            {
                                char splitText = '-';
                                var holder = detay.Split(splitText);
                                adi = holder[3].ReturnEmptyIfNull().ToString().TrimEnd();
                                int result;
                                if (int.TryParse(adi, out result))
                                {
                                    adi = string.Empty;
                                }
                            }

                            ekstreAktarma.Adi = adi.Trim().ToUpper(culturInfo);
                            ekstreAktarma.Tutar = tutar.ConvertToDecimal();
                            ekstreAktarma.BagisTarihi = bagisTarihi.ConvertToDatetime();
                            ekstreAktarma.Aciklama = detay;
                            ekstreAktarma.Adres = adres.ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo); ;
                            ekstreAktarma.BelgeIstemiyor = ekstreAktarma.Aciklama.Contains(ProjeConstants.DURUM_BELGE_ISTEMIYOR) || ekstreAktarma.Adres.Contains(ProjeConstants.DURUM_BELGE_ISTEMIYOR) ? true : false;
                            ekstreAktarma.BankaAdi = ProjeConstants.BANKA_GARANTIEKSTRE;
                            ekstreAktarma.IslemTarihi = processTime;
                            ekstreAktarma.DovizCinsi = ProjeConstants.DOVIZ_TL;
                            ekstreAktarma.Olusturan = currentUser;

                            SaveEkstreAktarma(ekstreAktarma);
                        }
                    }
                    catch (Exception ex)
                    {
                        Exception exceprion = new Exception(string.Format("HATA SATIRI {0}:{1} ->", ProjeConstants.BANKA_GARANTIEKSTRE, ""), ex);
                        exceptionHelper.Exceptions.Add(exceprion);
                    }
                }
            }

            return exceptionHelper;

        }
        public static ExceptionHelper SaveHalkbankFile(Stream filePath, DateTime processTime, string currentUser)
        {
            ExceptionHelper exceptionHelper = new ExceptionHelper();
            CultureInfo culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            List<string> lines = HelperFunctions.ConvertTextFileToList(filePath);
            foreach (var line in lines)
            {
                try
                {
                    var holder = line.Split(';');

                    if (holder.Length >= 12)
                    {
                        EkstreAktarma ekstreAktarma = new EkstreAktarma();

                        ekstreAktarma.TCKimlikNo = holder[1].ConvertToLong();
                        ekstreAktarma.Telefon1 = UtilityHelper.TelefonFormatla(holder[2].ReturnEmptyIfNull().ToString());
                        ekstreAktarma.Adi = RemoveWhiteSpaces(holder[3]).ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo);
                        //holder[4] Genel Bagis yaziyor. bu bilgiye ihtiyaç yok
                        ekstreAktarma.Ili = holder[5];
                        ekstreAktarma.Ilcesi = holder[6];
                        ekstreAktarma.Adres = holder[7].ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo);
                        string aciklama = holder[8];
                        ekstreAktarma.Aciklama = aciklama;
                        //holder[9] dosyanin tarihini içeriyor
                        ekstreAktarma.Tutar = holder[10].ReturnEmptyIfNull().ToString().Replace(".", ",").ConvertToDecimal();
                        ekstreAktarma.BagisTarihi = ConvertTextToDateTime(holder[11]);
                        ekstreAktarma.AktarildiMi = false;
                        ekstreAktarma.BankaAdi = ProjeConstants.BANKA_HALKBANK;
                        ekstreAktarma.DovizCinsi = ProjeConstants.DOVIZ_TL; //döviz cinsi hep tl oluyor
                        ekstreAktarma.IslemTarihi = processTime;
                        ekstreAktarma.Olusturan = currentUser;
                        ekstreAktarma.BelgeIstemiyor = ekstreAktarma.Aciklama.Contains(ProjeConstants.DURUM_BELGE_ISTEMIYOR) || ekstreAktarma.Adres.Contains(ProjeConstants.DURUM_BELGE_ISTEMIYOR) ? true : false;
                        SaveEkstreAktarma(ekstreAktarma);

                    }

                }
                catch (Exception ex)
                {
                    Exception exceprion = new Exception(string.Format("HATA SATIRI {0}:{1} ->", ProjeConstants.BANKA_HALKBANK, line), ex);
                    exceptionHelper.Exceptions.Add(exceprion);
                }
            }
            return exceptionHelper;
        }
        public static ExceptionHelper SaveHalkbank2File(Stream fileStream, DateTime processTime, string currentUser)
        {
            ExceptionHelper exceptionHelper = new ExceptionHelper();
            CultureInfo culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            try
            {
                var ekstreData = ExcelHelper.ReadXLSXAsDataTable(fileStream, ProjeConstants.BANKA_HALKBANK2_ILKKACSATIRHARIC, ProjeConstants.BANKA_HALKBANK2_SONKACSATIRHARIC, true);
                if (ekstreData != null)
                {

                    foreach (DataRow row in ekstreData.Rows)
                    {
                        var bagisTarihi = row[0].ReturnEmptyIfNull().ToString();//borc satiri için islem yapma
                        if (string.IsNullOrEmpty(bagisTarihi))// ilk kolon bos ise dosya bitti çik
                            break;

                        string hata = string.Empty;
                        try
                        {
                            EkstreAktarma ekstreAktarma = new EkstreAktarma();
                            var detay = row[1].ReturnEmptyIfNull().ToString();
                            var tutar = row[2].ReturnZeroIfNull().ToString().Replace(".", ",");
                            if (detay.Contains("50060309TSK")
                                || (detay.Contains("5528790009947647 KREDİ KARTI OTOMATİK VİRMAN"))
                                || (tutar.ReturnZeroIfNull().ConvertToDecimal() < 0))
                            {
                                continue;
                            }
                            else
                            {
                                string ayrac = "/";

                                if (detay.Contains(ayrac))
                                {
                                    //var holder = detay.Split(splitText, StringSplitOptions.None);
                                    //detay.Split(splitText, StringSplitOptions.None);
                                    //var nameHolder = holder[0].ReturnEmptyIfNull().ToString().TrimEnd();
                                    var nameHolder = detay.Substring(0, 19).ReturnEmptyIfNull().ToString().TrimEnd();
                                    if (!(nameHolder.Any(char.IsDigit)))
                                    {
                                        ekstreAktarma.Adi = nameHolder.ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo);
                                    }
                                }
                                else
                                {
                                    var splitText = new string[] { "OTURUM ÜCRETİ", "TC:" };
                                    var holder = detay.Split(splitText, StringSplitOptions.None);
                                    if (holder.Length > 1)
                                    {
                                        var nameHolder = holder[1].ReturnEmptyIfNull().ToString().TrimEnd();
                                        ekstreAktarma.Adi = nameHolder.ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo);
                                        if (holder.Length > 2)
                                        {
                                            var TCHolder = holder[2].ReturnEmptyIfNull().ToString().TrimEnd();
                                            ekstreAktarma.TCKimlikNo = TCHolder.ReturnZeroIfNull().ToString().Trim().ConvertToLong();
                                        }
                                    }
                                }
                                ekstreAktarma.Tutar = tutar.ConvertToDecimal();
                                ekstreAktarma.BagisTarihi = bagisTarihi.ConvertToDatetime();
                                ekstreAktarma.Aciklama = detay;
                                ekstreAktarma.BelgeIstemiyor = ekstreAktarma.Aciklama.Contains(ProjeConstants.DURUM_BELGE_ISTEMIYOR) ? true : false;
                                ekstreAktarma.BankaAdi = ProjeConstants.BANKA_HALKBANK2;
                                ekstreAktarma.IslemTarihi = processTime;
                                ekstreAktarma.DovizCinsi = ProjeConstants.DOVIZ_TL;
                                ekstreAktarma.Olusturan = currentUser;
                                hata = detay;
                                SaveEkstreAktarma(ekstreAktarma);
                            }

                        }
                        catch (Exception ex)
                        {
                            Exception exceprion = new Exception(string.Format("HATA SATIRI {0}:{1} ->", ProjeConstants.BANKA_HALKBANK2, hata), ex);
                            exceptionHelper.Exceptions.Add(exceprion);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                exceptionHelper.Exceptions.Add(ex);
            }
            return exceptionHelper;
        }
        public static ExceptionHelper SaveFinansbankFile(Stream fileStream, DateTime processTime, string currentUser)
        {
            ExceptionHelper exceptionHelper = new ExceptionHelper();
            CultureInfo culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            List<string> lines = HelperFunctions.ConvertTextFileToList(fileStream);
            foreach (var line in lines)
            {

                try
                {

                    //var fisNo = line.Substring(51, 72 - 51).TrimEnd();
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var soyadi = line.Substring(30, 30).Trim();
                    var adi = line.Substring(0, 30).Trim();
                    var item = new EkstreAktarma
                    {
                        Adi = string.Format("{0} {1}", adi, soyadi).ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo),
                        Adres = line.Substring(60, 60).Trim(),
                        Telefon1 = line.Substring(135, 10).Trim(),
                        DovizCinsi = line.Substring(150, 3).Trim(),
                        Tutar = line.Substring(153, 15).ReturnZeroIfNull().ToString().Replace(".", ",").ConvertToDecimal(),
                        TCKimlikNo = line.Substring(168, 21).ConvertToLong(),
                        Ili = line.Substring(189, 30).Trim(),
                        Ilcesi = line.Substring(219, 35).Trim(),
                        IslemTarihi = processTime,
                        BankaAdi = ProjeConstants.BANKA_FINANSBANK,

                        //BagisTarihi = DateTime.Today,
                        AktarildiMi = false,
                        ElleKayit = false,
                        TuzelKisi = false,
                        BelgeIstemiyor = false,
                        NakitBagisHareketId = 0,
                        NakitBagisciId = 0,
                        FisNo = string.Empty,
                        Eposta = string.Empty,
                        PostaKodu = string.Empty,
                        Aciklama = string.Empty,
                        Olusturan = currentUser
                    };
                    //item.FisNo = fisNo;
                    //if (BuKayitDahaOnceGirilmisMiByFisNo(ProjeConstants.BANKA_FINANSBANK,
                    //    fisNo, aciklama, tarih.ReturnEmptyIfNull().ConvertToDatetime(), item.Tutar)) //fis numarasindan kontrol et kayit girilmemisse exception dondur degilse kaydet
                    //{
                    //    Exception ex = new Exception(fisNo + " Numarali fis daha önce girildiginden tekrar aktarilmadi.");
                    //    exceptionHelper.Exceptions.Add(ex);
                    //}
                    //else
                    //{
                    //    ekstreAktarma.Save();

                    //}
                    SaveEkstreAktarma(item);
                }
                catch (Exception ex)
                {
                    Exception exceprion = new Exception(string.Format("HATA SATIRI {0}:{1} ->", ProjeConstants.BANKA_FINANSBANK, line), ex);
                    exceptionHelper.Exceptions.Add(exceprion);
                }

            }
            return exceptionHelper;
        }
        public static ExceptionHelper SaveFinansbankEkstreFile(Stream fileStream, DateTime islemTarihi, string currentUser)
        {
            int kayitNo = 0;
            ExceptionHelper exceptionHelper = new ExceptionHelper();
            CultureInfo culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            try
            {

                var data = ExcelHelper.ReadXLSXAsDataTable(fileStream,
                    ProjeConstants.BANKA_FINANSBANKEKSTRE_ILKKACSATIRHARIC, ProjeConstants.BANKA_FINANSBANKEKSTRE_SONKACSATIRHARIC, false);
                if (data != null)
                {
                    foreach (DataRow row in data.Rows)
                    {
                        kayitNo++;
                        EkstreAktarma ekstreAktarma = new EkstreAktarma();
                        var tarih = row[0].ReturnEmptyIfNull().ToString();//borc satiri için islem yapma

                        if (string.IsNullOrEmpty(tarih))// ilk kolon bos ise dosya bitti çik
                            break;
                        try
                        {
                            var fisAaciklama = row[8].ReturnEmptyIfNull().ToString();//hersey açiklamanin içinde
                            string adi = string.Empty;
                            string aciklama = string.Empty;
                            if (!string.IsNullOrEmpty(fisAaciklama))
                            {
                                var parts = fisAaciklama.Split('|');
                                foreach (var part in parts)
                                {
                                    var trimmed = part.Trim();
                                    if (trimmed.StartsWith("BAĞIŞÇI ADI SOYADI:"))
                                        adi = trimmed.Substring("BAĞIŞÇI ADI SOYADI:".Length).Trim();
                                    else if (trimmed.StartsWith("AÇIKLAMA:"))
                                        aciklama = trimmed.Substring("AÇIKLAMA:".Length).Trim();
                                }
                                ekstreAktarma.Aciklama = aciklama;
                                ekstreAktarma.Adi = adi;
                            }

                                var bagisTarihi = row[0].ReturnEmptyIfNull().ToString().ConvertToDatetime();
                                var fisNo = row[7].ToString().Trim();
                                var tutar = row[5].ReturnZeroIfNull().ToString().Replace(".", ",").ConvertToDecimal();//row[6].ReturnEmptyIfNull().ToString();
                                if (tutar > 0)
                                {
                                    ekstreAktarma.Tutar = tutar.ConvertToDecimal();
                                    ekstreAktarma.BagisTarihi = bagisTarihi;

                                    ekstreAktarma.BelgeIstemiyor = ekstreAktarma.Aciklama.Contains(ProjeConstants.DURUM_BELGE_ISTEMIYOR) ? true : false;
                                    ekstreAktarma.BankaAdi = ProjeConstants.BANKA_FINANSBANKEKSTRE;
                                    ekstreAktarma.IslemTarihi = islemTarihi;
                                    ekstreAktarma.DovizCinsi = ProjeConstants.DOVIZ_TL;
                                    ekstreAktarma.Olusturan = currentUser;
                                    ekstreAktarma.FisNo = fisNo;
                                    if (BuKayitDahaOnceGirilmisMiByFisNo(ProjeConstants.BANKA_FINANSBANK ,fisNo, aciklama, bagisTarihi, tutar))
                                    //fis numarasindan kontrol et kayit girilmemisse exception dondur degilse kaydet
                                    {
                                        Exception ex = new Exception(fisNo + " Numaralı fiş daha önce girildiginden tekrar aktarılmadı.");
                                        exceptionHelper.Exceptions.Add(ex);
                                    }
                                    else
                                    {
                                        SaveEkstreAktarma(ekstreAktarma);
                                    }
                                }

                        }
                        catch (Exception ex)
                        {
                            Exception exception = new Exception(string.Format("HATA SATIRI {0}:{1} ->", ProjeConstants.BANKA_FINANSBANKEKSTRE, kayitNo), ex);
                            exceptionHelper.Exceptions.Add(exception);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                exceptionHelper.Exceptions.Add(ex);
            }
            return exceptionHelper;
        }
        /// <summary>
        /// int liste aalip EkstreAktarma Listesi döndürür
        /// </summary>
        /// <param name="checkedRows"></param>
        /// <returns></returns>
        public static ExceptionHelper SaveIsBankFile(Stream filePath, DateTime processTime, string currentUser)
        {
            ExceptionHelper exceptionHelper = new ExceptionHelper();
            CultureInfo culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            List<string> lines = HelperFunctions.ConvertTextFileToList(filePath);
            foreach (var line in lines)
            {
                var holder = line.Split('!');

                if (holder.Length >= 12)
                {
                    try
                    {
                        var tutar = holder[8].ReturnEmptyIfNull().ToString();
                        if (tutar.Contains('.') && !tutar.Contains(','))
                        {
                            tutar = tutar.Replace('.', ',');
                        }

                        EkstreAktarma ekstreAktarma = new EkstreAktarma();
                        ekstreAktarma.BagisTarihi = ConvertIsBankasiTextToDateTime(holder[0]);
                        ekstreAktarma.Adi = string.Format("{0} {1}", RemoveWhiteSpaces(holder[1]), RemoveWhiteSpaces(holder[2])).ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo); ;
                        ekstreAktarma.Adres = RemoveWhiteSpaces(holder[3]).ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo); ;
                        ekstreAktarma.Ilcesi = holder[4].TrimEnd();
                        ekstreAktarma.Ili = holder[5].TrimEnd();
                        ekstreAktarma.Telefon2 = UtilityHelper.TelefonFormatla(holder[6].ReturnEmptyIfNull().ToString());
                        ekstreAktarma.Telefon1 = UtilityHelper.TelefonFormatla(holder[7].ReturnEmptyIfNull().ToString());
                        ekstreAktarma.Tutar = tutar.ConvertToDecimal();
                        ekstreAktarma.Aciklama = RemoveWhiteSpaces(holder[9]);
                        ekstreAktarma.BelgeIstemiyor = ekstreAktarma.Aciklama.Contains(ProjeConstants.DURUM_BELGE_ISTEMIYOR) || ekstreAktarma.Adres.Contains(ProjeConstants.DURUM_BELGE_ISTEMIYOR) ? true : false;
                        ekstreAktarma.DovizCinsi = ProjeConstants.DOVIZ_TL;
                        ekstreAktarma.AktarildiMi = false;
                        ekstreAktarma.BankaAdi = ProjeConstants.BANKA_ISBANK;
                        ekstreAktarma.IslemTarihi = processTime;
                        ekstreAktarma.Olusturan = currentUser;
                        //TC bilgisi dosyada yer almiyor



                        SaveEkstreAktarma(ekstreAktarma);
                    }
                    catch (Exception ex)
                    {
                        Exception exceprion = new Exception(string.Format("HATA SATIRI {0}:{1} ->", ProjeConstants.BANKA_ISBANK, line), ex);
                        exceptionHelper.Exceptions.Add(exceprion);
                    }

                }
                else
                {
                    exceptionHelper.Exceptions.Add(new Exception(line + " Kayıt aktarılamadı. " + ProjeConstants.BANKA_ISBANK));
                }
            }
            return exceptionHelper;

        }

        public static ExceptionHelper SaveIsBankEkstreFile(Stream fileStream, DateTime processTime, string currentUser)
        {
            ExceptionHelper exceptionHelper = new ExceptionHelper();
            CultureInfo culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);


            var isbankEkstreData = ExcelHelper.ReadXLSXAsDataTable(fileStream, ProjeConstants.BANKA_ISBANKEKSTRE_ILKKACSATIRHARIC, ProjeConstants.BANKA_ISBANKEKSTRE_SONKACSATIRHARIC, true);


            if (isbankEkstreData != null)
            {

                foreach (DataRow row in isbankEkstreData.Rows)
                {
                    EkstreAktarma ekstreAktarma = new EkstreAktarma();
                    try
                    {
                        string tarihStr = row[0].ReturnZeroIfNull().ToString();
                        if (tarihStr.Contains("Tarih/Saat"))
                        {
                            continue;
                        }
                        else
                        {
                            string islemTipi = row[7].ReturnEmptyIfNull().ToString();
                            string tutar = row[3].ReturnZeroIfNull().ToString().Replace(".", ",");
                            if ((islemTipi.Equals("Havale")
                                //|| islemTipi.Equals("Para Transferi") //para transferi yazanlar günlük dosyada geliyor
                                || islemTipi.Equals("EFT")
                                || islemTipi.Equals("FAST"))
                                && tutar.ConvertToDecimal() > 0)
                            {
                                string tarih = row[0].ReturnEmptyIfNull().ToString(); //ilk sütunda bağış tarihi var sonraki Valor tarihi
                                DateTime bagisTarihi = tarih.Substring(0, 10).ConvertToDatetime();

                                var detay = row[8].ReturnEmptyIfNull().ToString();
                                string adi = string.Empty;
                                var adres = string.Empty;// adres bilgisi gelmiyor
                                if (islemTipi.Equals("FAST"))
                                {
                                    if (detay.Contains("*") && detay.IndexOf("*") > 0)
                                    {
                                        adi = detay.Substring(0, detay.IndexOf("*"));
                                    }
                                }
                                else if (detay.Contains("BAĞIŞ") && detay.IndexOf("BAĞIŞ") > 0)
                                {
                                    adi = detay.Substring(0, detay.IndexOf("BAĞIŞ"));
                                }
                                else if (detay.Contains("TARAFINDAN"))
                                {
                                    adi = detay.Substring(0, detay.IndexOf("TARAFINDAN"));
                                }
                                else if (detay.Contains("*"))
                                {
                                    char splitText = '*';
                                    var holder = detay.Split(splitText);
                                    adi = holder[1].ReturnEmptyIfNull().ToString().TrimEnd();
                                    int result;
                                    if (int.TryParse(adi, out result))
                                    {
                                        adi = string.Empty;
                                    }
                                }
                                else if (detay.Contains("/"))
                                {
                                    char splitText = '/';
                                    var holder = detay.Split(splitText);
                                    adi = holder[0].ReturnEmptyIfNull().ToString().TrimEnd();
                                    int result;
                                    if (int.TryParse(adi, out result))
                                    {
                                        adi = string.Empty;
                                    }
                                }


                                ekstreAktarma.Adi = adi.Trim().ToUpper(culturInfo);
                                ekstreAktarma.Tutar = tutar.ConvertToDecimal();
                                ekstreAktarma.BagisTarihi = bagisTarihi.ConvertToDatetime();
                                ekstreAktarma.Aciklama = detay;
                                ekstreAktarma.Adres = adres.ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo); ;
                                ekstreAktarma.BelgeIstemiyor = ekstreAktarma.Aciklama.Contains(ProjeConstants.DURUM_BELGE_ISTEMIYOR) || ekstreAktarma.Adres.Contains(ProjeConstants.DURUM_BELGE_ISTEMIYOR) ? true : false;
                                ekstreAktarma.BankaAdi = ProjeConstants.BANKA_ISBANKEKSTRE;
                                ekstreAktarma.IslemTarihi = processTime;
                                ekstreAktarma.DovizCinsi = ProjeConstants.DOVIZ_TL;
                                ekstreAktarma.Olusturan = currentUser;

                                SaveEkstreAktarma(ekstreAktarma);
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Exception exceprion = new Exception(string.Format("HATA SATIRI {0}:{1} ->", ProjeConstants.BANKA_VAKIF_KATILIM, ""), ex);
                        exceptionHelper.Exceptions.Add(exceprion);
                    }
                }
            }

            return exceptionHelper;

        }
        public static ExceptionHelper SaveYKBEkstreFile(Stream fileStream, DateTime processTime, string currentUser)
        {
            ExceptionHelper exceptionHelper = new ExceptionHelper();
            CultureInfo culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);


            var YKBEkstreData = ExcelHelper.ReadXLSXAsDataTable(fileStream, ProjeConstants.BANKA_YKBEKSTRE_ILKKACSATIRHARIC, ProjeConstants.BANKA_YKBEKSTRE_SONKACSATIRHARIC, false);


            if (YKBEkstreData != null)
            {

                foreach (DataRow row in YKBEkstreData.Rows)
                {
                    EkstreAktarma ekstreAktarma = new EkstreAktarma();
                    try
                    {
                        string saat = row[1].ReturnEmptyIfNull().ToString();
                        string tutar = row[6].ReturnZeroIfNull().ToString().Replace(".", ",");
                        string fisno = row[4].ReturnZeroIfNull().ToString().Replace(".", ",");
                        if (tutar.ConvertToDecimal() > 0)
                        {
                            string tarih = row[0].ReturnEmptyIfNull().ToString();
                            DateTime bagisTarihi = tarih.Substring(0, 10).ConvertToDatetime();

                            var detay = row[5].ReturnEmptyIfNull().ToString();
                            string adi = string.Empty;
                            var adres = string.Empty;// adres bilgisi gelmiyor
                            if (detay.Contains("/"))
                            {
                                char splitText = '/';
                                var holder = detay.Split(splitText);
                                adi = holder[1].ReturnEmptyIfNull().ToString().TrimEnd();
                                int result;
                                if (int.TryParse(adi, out result))
                                {
                                    adi = string.Empty;
                                }
                            }
                            else if (detay.Contains("TSK Güçlendirme Vakfı - Genel Bağış / "))
                            {
                                adi = detay.Replace("TSK Güçlendirme Vakfı - Genel Bağış / ", "").Substring(0, (detay.IndexOf(" / "))).Trim();
                            }
                            else if (detay.Contains("GELEN HAVALE -"))
                            {
                                if (detay.Contains(" -"))
                                {
                                    char splitText = '-';
                                    var holder = detay.Split(splitText);
                                    adi = holder[1].ReturnEmptyIfNull().ToString().TrimEnd();
                                    int result;
                                    if (int.TryParse(adi, out result))
                                    {
                                        adi = string.Empty;
                                    }
                                }
                            }
                            else if (detay.Contains("GELEN FAST -"))
                            {
                                if (detay.Contains(" -"))
                                {
                                    char splitText = '-';
                                    var holder = detay.Split(splitText);
                                    adi = holder[1].ReturnEmptyIfNull().ToString().TrimEnd();
                                    int result;
                                    if (int.TryParse(adi, out result))
                                    {
                                        adi = string.Empty;
                                    }
                                }
                            }
                            else if (detay.Contains("GELEN EFT -"))
                            {
                                if (detay.Contains(" -"))
                                {
                                    char splitText = '-';
                                    var holder = detay.Split(splitText);
                                    adi = holder[1].ReturnEmptyIfNull().ToString().TrimEnd();
                                    int result;
                                    if (int.TryParse(adi, out result))
                                    {
                                        adi = string.Empty;
                                    }
                                }
                            }
                            else if (detay.Contains("-Bağış"))
                            {
                                adi = detay.Substring(0, detay.IndexOf("-Bağış")).Replace("-Bağış", "").Replace(" -", ""); ;
                            }
                            else if (detay.Contains("BAĞIŞ  GONDERICI:"))
                            {
                                adi = detay.Replace("BAĞIŞ  GONDERICI:","");

                            }

                            adi = adi.Replace(" bağış", "").Replace("/", "");

                            ekstreAktarma.Adi = adi.Trim().ToUpper(culturInfo);
                            ekstreAktarma.Tutar = tutar.ConvertToDecimal();
                            ekstreAktarma.BagisTarihi = bagisTarihi.ConvertToDatetime();
                            ekstreAktarma.Aciklama = detay;
                            ekstreAktarma.Adres = adres.ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo); ;
                            ekstreAktarma.BelgeIstemiyor = ekstreAktarma.Aciklama.Contains(ProjeConstants.DURUM_BELGE_ISTEMIYOR) || ekstreAktarma.Adres.Contains(ProjeConstants.DURUM_BELGE_ISTEMIYOR) ? true : false;
                            ekstreAktarma.BankaAdi = ProjeConstants.BANKA_YKBEKSTRE;
                            ekstreAktarma.IslemTarihi = processTime;
                            ekstreAktarma.DovizCinsi = ProjeConstants.DOVIZ_TL;
                            ekstreAktarma.Olusturan = currentUser;
                            ekstreAktarma.FisNo = fisno;
                            if (BuKayitDahaOnceGirilmisMiByFisNo(ProjeConstants.BANKA_YKBEKSTRE, fisno, detay, tarih.ReturnEmptyIfNull().ConvertToDatetime(), tutar.ConvertToDecimal())) //Referans numarasindan kontrol et kayit girilmemisse exception dondur degilse kaydet
                            {
                                Exception ex = new Exception(fisno + " Numaralı fiş daha önce girildiğinden tekrar aktarılmadı.");
                                exceptionHelper.Exceptions.Add(ex);
                            }
                            else
                            {
                                SaveEkstreAktarma(ekstreAktarma);

                            }
                        }
                        else
                        {
                            continue;
                        }

                    }
                    catch (Exception ex)
                    {
                        Exception exceprion = new Exception(string.Format("HATA SATIRI {0}:{1} ->", ProjeConstants.BANKA_YKBEKSTRE, ""), ex);
                        exceptionHelper.Exceptions.Add(exceprion);
                    }
                }
            }

            return exceptionHelper;

        }

        public static ExceptionHelper SaveVakifBankFile(Stream fileStream, DateTime processTime, string currentUser)
        {
            ExceptionHelper exceptionHelper = new ExceptionHelper();
            CultureInfo culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            try
            {
                var splitText = new string[] { "Bağışçı Ad Soyad:", ", Cep Telefon Numarası:", ", Müşteri Notu:", ", İşlem No:" };
                var vakifBankData = ExcelHelper.ReadXLSXAsDataTable(fileStream, ProjeConstants.BANKA_VAKIF_ILKKACSATIRHARIC, ProjeConstants.BANKA_VAKIF_SONKACSATIRHARIC, true);
                if (vakifBankData != null)
                {

                    foreach (DataRow row in vakifBankData.Rows)
                    {
                        EkstreAktarma ekstreAktarma = new EkstreAktarma();
                        try
                        {
                            var borc_alacak = row[15].ReturnEmptyIfNull().ToString();//borc satiri için islem yapma
                            if (borc_alacak.Equals("B"))
                            {
                                continue;
                            }
                            //dummy rows
                            //var hesapno = row[0].ReturnEmptyIfNull().ToString();
                            //var fisnono = row[1].ReturnEmptyIfNull().ToString();
                            //var hareketTar = row[2].ReturnEmptyIfNull().ToString();
                            //

                            var bagisTarihi = row[3].ReturnEmptyIfNull().ToString();
                            //dummy rows
                            //var kartno = row[4].ReturnEmptyIfNull().ToString();
                            //var islem = row[5].ReturnEmptyIfNull().ToString();
                            //
                            var tutar = row[6].ReturnEmptyIfNull().ToString().Replace(".", ",").ConvertToDecimal();//row[6].ReturnEmptyIfNull().ToString();
                            //dummy rows
                            //var bakiye = row[7].ReturnEmptyIfNull().ToString();
                            //var kanal = row[8].ReturnEmptyIfNull().ToString();
                            //var islemno = row[9].ReturnEmptyIfNull().ToString();
                            //var referans = row[10].ReturnEmptyIfNull().ToString();
                            //var havale = row[11].ReturnEmptyIfNull().ToString();
                            //var refno = row[12].ReturnEmptyIfNull().ToString();
                            //var tckn = row[13].ReturnEmptyIfNull().ToString();
                            //var vkn = row[14].ReturnEmptyIfNull().ToString();
                            var b_a = row[15].ReturnEmptyIfNull().ToString();
                            //
                            var detay = row[16].ReturnEmptyIfNull().ToString();
                            var ad = string.Empty;
                            var tel1 = string.Empty;
                            var aciklama = string.Empty;
                            var adres = string.Empty;//adres bilgisi gelmiyor

                            var holder = detay.Split(splitText, StringSplitOptions.None);

                            if (holder.Length > 4)
                            {
                                ad = holder[1].ReturnEmptyIfNull().ToString();
                                tel1 = holder[2].ReturnEmptyIfNull().ToString();
                                aciklama = holder[3].ReturnEmptyIfNull().ToString();
                            }

                            ekstreAktarma.Adi = ad.ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo); ;
                            ekstreAktarma.Telefon1 = UtilityHelper.TelefonFormatla(tel1.ReturnEmptyIfNull().ToString());
                            ekstreAktarma.Tutar = tutar.ConvertToDecimal();
                            ekstreAktarma.BagisTarihi = bagisTarihi.ConvertToDatetime();
                            ekstreAktarma.Adres = adres.ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo); ;
                            ekstreAktarma.Aciklama = aciklama;
                            ekstreAktarma.BelgeIstemiyor = ekstreAktarma.Aciklama.Contains(ProjeConstants.DURUM_BELGE_ISTEMIYOR) || ekstreAktarma.Adres.Contains(ProjeConstants.DURUM_BELGE_ISTEMIYOR) ? true : false;
                            ekstreAktarma.BankaAdi = ProjeConstants.BANKA_VAKIF;
                            ekstreAktarma.IslemTarihi = processTime;
                            ekstreAktarma.DovizCinsi = ProjeConstants.DOVIZ_TL;
                            ekstreAktarma.Olusturan = currentUser;

                            SaveEkstreAktarma(ekstreAktarma);
                        }
                        catch (Exception ex)
                        {
                            Exception exceprion = new Exception(string.Format("HATA SATIRI {0}:{1} ->", ProjeConstants.BANKA_VAKIF, ""), ex);
                            exceptionHelper.Exceptions.Add(exceprion);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                exceptionHelper.Exceptions.Add(ex);
            }
            return exceptionHelper;
        }
        public static ExceptionHelper SaveVakifBankGunlukTextFile(Stream filePath, DateTime processTime, string currentUser)
        {
            /**
                D                   :  Satır Başı
                Islem No            :  Sadece o işleme/bağışa mahsus bir işlem numarası.
                Bagis Tarihi        :  Bağışın hesaba yatırıldığı tarih
                Adi                 :  Bağış yapan kişinin adı
                Soyadi              :  Bağış yapan kişinin soyadı
                Adres               :  Bağış yapan kişinin adresi
                Ilçe                :  Bağış yapan kişinin ilçesi
                Ili                 :  Bağış yapan kişinin ili
                TC Kimlik No        :  Bağış yapan kişinin TC Kimlik numarası
                Telefon             :  Bağış yapan kişinin telefon numarası
                Bagis Tutari        :  Bağış tutarı
                Açiklama-Not        :  Bağış yapan kişinin notu, açıklaması

             **/
            ExceptionHelper exceptionHelper = new ExceptionHelper();
            CultureInfo culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            List<string> lines = HelperFunctions.ConvertTextFileToList(filePath);
            foreach (var line in lines)
            {
                try
                {
                    var holder = line.Split('!');

                    if (holder.Length >= 11)
                    {
                        EkstreAktarma ekstreAktarma = new EkstreAktarma();
                        //Islem Nu
                        ekstreAktarma.FisNo = holder[1].ToString();
                        ekstreAktarma.BagisTarihi = holder[2].ReturnEmptyIfNull().ToString().Replace("/", ".").ConvertToDatetime();
                        //Ad soyad birlikte ada yazilacak
                        ekstreAktarma.Adi = RemoveWhiteSpaces(holder[3]).ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo) +
                            " " + RemoveWhiteSpaces(holder[4]).ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo);
                        ekstreAktarma.Adres = holder[5].ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo);
                        ekstreAktarma.Ilcesi = RemoveWhiteSpaces(holder[6]).ReturnEmptyIfNull().ToString().Trim();
                        ekstreAktarma.Ili = RemoveWhiteSpaces(holder[7]).ReturnEmptyIfNull().ToString().Trim();
                        ekstreAktarma.TCKimlikNo = RemoveWhiteSpaces(holder[8]).ReturnEmptyIfNull().ToString().Trim().ConvertToLong();
                        ekstreAktarma.Telefon1 = UtilityHelper.TelefonFormatla(holder[9].ReturnEmptyIfNull().ToString());
                        ekstreAktarma.Tutar = holder[10].ReturnEmptyIfNull().ToString().Replace(".", ",").ConvertToDecimal();
                        ekstreAktarma.Aciklama = holder[11];

                        ekstreAktarma.AktarildiMi = false;
                        ekstreAktarma.BankaAdi = ProjeConstants.BANKA_VAKIF;// GUNLUK;
                        ekstreAktarma.DovizCinsi = ProjeConstants.DOVIZ_TL; //döviz cinsi hep tl oluyor
                        ekstreAktarma.IslemTarihi = processTime;
                        ekstreAktarma.Olusturan = currentUser;
                        ekstreAktarma.BelgeIstemiyor = ekstreAktarma.Aciklama.Contains(ProjeConstants.DURUM_BELGE_ISTEMIYOR) || ekstreAktarma.Adres.Contains(ProjeConstants.DURUM_BELGE_ISTEMIYOR) ? true : false;
                        SaveEkstreAktarma(ekstreAktarma);

                    }

                }
                catch (Exception ex)
                {
                    Exception exceprion = new Exception(string.Format("HATA SATIRI {0}:{1} ->", ProjeConstants.BANKA_VAKIF, line), ex);//GUNLUK
                    exceptionHelper.Exceptions.Add(exceprion);
                }
            }
            return exceptionHelper;
        }
        public static ExceptionHelper SaveVakifBank2File(Stream fileStream, DateTime processTime, string currentUser)
        {
            ExceptionHelper exceptionHelper = new ExceptionHelper();
            CultureInfo culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            try
            {

                var vakifBank2Data = ExcelHelper.ReadXLSXAsDataTable(fileStream, ProjeConstants.BANKA_VAKIF2_ILKKACSATIRHARIC, ProjeConstants.BANKA_VAKIF2_SONKACSATIRHARIC, true);
                if (vakifBank2Data != null)
                {
                    foreach (DataRow row in vakifBank2Data.Rows)
                    {
                        EkstreAktarma ekstreAktarma = new EkstreAktarma();
                        var hesapNo = row[0].ReturnEmptyIfNull().ToString();//borc satiri için islem yapma
                        var islem = row[5].ReturnEmptyIfNull().ToString();
                        var tutar = row[6].ReturnEmptyIfNull().ToString().Replace(".", ",").ConvertToDecimal();//row[6].ReturnEmptyIfNull().ToString();
                        if (string.IsNullOrEmpty(hesapNo))// ilk kolon bos ise dosya bitti çik
                        {
                            break;
                        }
                        else if (islem.Contains("Batch Yatan")//Kart ile bagistan gelen toplam miktar, zaten ayrica girisi yapiliyor, dikkate alma
                           || islem.Contains("Batch Komisyonu")
                           || hesapNo.Equals("HESAP NO")
                           || tutar < 0)
                            continue;
                        try
                        {
                            var borc_alacak = row[15].ReturnEmptyIfNull().ToString();//borc satiri için islem yapma
                            if (borc_alacak.Equals("B"))
                            {
                                continue;
                            }
                            //dummy rows
                            //var hesapno = row[0].ReturnEmptyIfNull().ToString();
                            //var fisnono = row[1].ReturnEmptyIfNull().ToString();
                            //var hareketTar = row[2].ReturnEmptyIfNull().ToString();
                            //

                            var bagisTarihi = row[3].ReturnEmptyIfNull().ToString();
                            //dummy rows
                            //var kartno = row[4].ReturnEmptyIfNull().ToString();
                            //

                            //dummy rows
                            //var bakiye = row[7].ReturnEmptyIfNull().ToString();
                            var kanal = row[8].ReturnEmptyIfNull().ToString();
                            //var islemno = row[9].ReturnEmptyIfNull().ToString();
                            //var referans = row[10].ReturnEmptyIfNull().ToString();
                            //var havale = row[11].ReturnEmptyIfNull().ToString();
                            //var refno = row[12].ReturnEmptyIfNull().ToString();
                            //var tckn = row[13].ReturnEmptyIfNull().ToString();
                            //var vkn = row[14].ReturnEmptyIfNull().ToString();
                            var b_a = row[15].ReturnEmptyIfNull().ToString();
                            //
                            var detay = row[16].ReturnEmptyIfNull().ToString();
                            var ad = string.Empty;
                            var tel1 = string.Empty;
                            var aciklama = string.Empty;
                            var adres = string.Empty;//adres bilgisi yok

                            var splitText = new string[] { "sorgu numaralı", "tarafından" };
                            var holder = detay.Split(splitText, StringSplitOptions.None);

                            if (kanal.Equals("Mobil Bankacılık") || kanal.Equals("İnternet"))
                            {
                                var splitTextM = new string[] { "nolu", "hesabından" };
                                holder = detay.Split(splitTextM, StringSplitOptions.None);
                            }
                            if (holder.Length > 2)
                            {
                                ad = holder[1].ReturnEmptyIfNull().ToString();
                                aciklama = detay.ToString();
                            }

                            ekstreAktarma.Adi = ad.ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo); ;
                            ekstreAktarma.Telefon1 = UtilityHelper.TelefonFormatla(tel1.ReturnEmptyIfNull().ToString());
                            ekstreAktarma.Tutar = tutar.ConvertToDecimal();
                            ekstreAktarma.BagisTarihi = bagisTarihi.ConvertToDatetime();
                            ekstreAktarma.Aciklama = aciklama;
                            ekstreAktarma.Adres = adres.ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo); ;
                            ekstreAktarma.BelgeIstemiyor = ekstreAktarma.Aciklama.Contains(ProjeConstants.DURUM_BELGE_ISTEMIYOR) || ekstreAktarma.Adres.Contains(ProjeConstants.DURUM_BELGE_ISTEMIYOR) ? true : false;
                            ekstreAktarma.BankaAdi = ProjeConstants.BANKA_VAKIF2;
                            ekstreAktarma.IslemTarihi = processTime;
                            ekstreAktarma.DovizCinsi = ProjeConstants.DOVIZ_TL;
                            ekstreAktarma.Olusturan = currentUser;

                            SaveEkstreAktarma(ekstreAktarma);
                        }
                        catch (Exception ex)
                        {
                            Exception exception = new Exception(string.Format("HATA SATIRI {0}:{1} ->", ProjeConstants.BANKA_VAKIF, ""), ex);
                            exceptionHelper.Exceptions.Add(exception);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                exceptionHelper.Exceptions.Add(ex);
            }
            return exceptionHelper;
        }
        public static ExceptionHelper SaveVakifKatilimFile(Stream fileStream, DateTime processTime, string currentUser)
        {
            CultureInfo culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            ExceptionHelper exceptionHelper = new ExceptionHelper();
            try
            {
                var vakifBankData = ExcelHelper.ReadXLSXAsDataTable(fileStream, ProjeConstants.BANKA_VAKIFKATILIM_ILKKACSATIRHARIC, ProjeConstants.BANKA_VAKIFKATILIM_SONKACSATIRHARIC, true);
                if (vakifBankData != null)
                {

                    foreach (DataRow row in vakifBankData.Rows)
                    {
                        EkstreAktarma ekstreAktarma = new EkstreAktarma();
                        try
                        {
                            long tarihLong = long.Parse(row[0].ReturnZeroIfNull().ToString());
                            DateTime bagisTarihi = DateTime.FromOADate(tarihLong);
                            var detay = row[1].ReturnEmptyIfNull().ToString();
                            var tutar = row[2].ReturnZeroIfNull().ToString().Replace(".", ",");
                            var adres = string.Empty;// adres bilgisi gelmiyor


                            if (detay.Contains("198700 - 1 no")) //vakıf hesabına transfer
                            {
                                continue;
                            }
                            string ayrac1 = "Genel Bağış";
                            string ayrac2 = "Amir: ";
                            string ayrac3 = "Amir= ";
                            if (detay.Contains(ayrac1))
                            {
                                char splitText = ',';
                                var holder = detay.Split(splitText);
                                var nameHolder = holder[1].ReturnEmptyIfNull().ToString().Trim();
                                var telefonHolder = holder.Count()>2 ? holder[2].ReturnEmptyIfNull().ToString().Trim() : string.Empty;
                                ekstreAktarma.Adi = nameHolder.ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo);
                                ekstreAktarma.Telefon1 = UtilityHelper.TelefonFormatla(telefonHolder.ReturnEmptyIfNull().ToString());
                            }
                            else if (detay.Contains(ayrac2))
                            {
                                string sonAyirac = " - Lehdar: ";
                                int baslangic = detay.IndexOf(ayrac2) + ayrac2.Length;
                                int bitis = detay.IndexOf(sonAyirac, baslangic);

                                if (bitis > baslangic)
                                {
                                    ekstreAktarma.Adi = detay.Substring(baslangic, bitis - baslangic).Trim();
                                }
                            }
                            else if (detay.Contains(ayrac3))
                            {
                                string sonAyirac = " Aciklama=";
                                int baslangic = detay.IndexOf(ayrac3) + ayrac3.Length;
                                int bitis = detay.IndexOf(sonAyirac, baslangic);

                                if (bitis > baslangic)
                                {
                                    ekstreAktarma.Adi = detay.Substring(baslangic, bitis - baslangic).Trim();
                                }
                            }
                            ekstreAktarma.Tutar = tutar.ConvertToDecimal();
                            ekstreAktarma.BagisTarihi = bagisTarihi.ConvertToDatetime();
                            ekstreAktarma.Aciklama = detay;
                            ekstreAktarma.Adres = adres.ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo); ;
                            ekstreAktarma.BelgeIstemiyor = ekstreAktarma.Aciklama.Contains(ProjeConstants.DURUM_BELGE_ISTEMIYOR) || ekstreAktarma.Adres.Contains(ProjeConstants.DURUM_BELGE_ISTEMIYOR) ? true : false;
                            ekstreAktarma.BankaAdi = ProjeConstants.BANKA_VAKIF_KATILIM;
                            ekstreAktarma.IslemTarihi = processTime;
                            ekstreAktarma.DovizCinsi = ProjeConstants.DOVIZ_TL;
                            ekstreAktarma.Olusturan = currentUser;

                            SaveEkstreAktarma(ekstreAktarma);
                        }
                        catch (Exception ex)
                        {
                            Exception exceprion = new Exception(string.Format("HATA SATIRI {0}:{1} ->", ProjeConstants.BANKA_VAKIF_KATILIM, ""), ex);
                            exceptionHelper.Exceptions.Add(exceprion);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                exceptionHelper.Exceptions.Add(ex);
            }
            return exceptionHelper;
        }
        public static ExceptionHelper SaveTEBBankFile(Stream filePath, DateTime processTime, string currentUser)
        {
            ExceptionHelper exceptionHelper = new ExceptionHelper();
            CultureInfo culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            List<string> lines = HelperFunctions.ConvertTextFileToList(filePath);
            int count = 0;
            var tarih = string.Empty;
            foreach (var line in lines)
            {
                //if (count == 0)//ilk satirda tarih bilgisi var
                //{
                //    tarih = line.Substring(10, 8);
                //}
                if (count > 0 && count < (lines.Count - 1)) //ilk satir ve son satir alinmayacak
                {
                    try
                    {
                        EkstreAktarma ekstreAktarma = new EkstreAktarma();

                        //TCKN

                        var kod = line.Substring(0, 1).TrimEnd();

                        if (kod.Equals("D"))
                        {
                            var tc = line.Substring(1, 11).TrimEnd();
                            var ad = line.Substring(12, 40).TrimEnd();
                            var adres = line.Substring(52, 200).TrimEnd();
                            var tel = line.Substring(252, 15).TrimEnd();
                            var eposta = line.Substring(267, 50).TrimEnd();
                            //dummy bagisturu
                            var bagisturu = line.Substring(317, 3).TrimEnd();
                            tarih = line.Substring(320, 8).Trim();//20190617 YYYYMMDD formati
                            var tutarTl = line.Substring(328, 15).TrimEnd();// 0000000000111.00
                            var tutar = string.IsNullOrEmpty(tutarTl.Trim()) ? "0" : tutarTl;
                            if (tutar.Contains("."))
                            {
                                tutar = tutar.Replace(".", ",");
                            }
                            var dovizCinsi = line.Substring(343, 3).TrimEnd();
                            var aciklama = line.Substring(346, 255).TrimEnd();

                            //var il = line.Substring(267, 20).TrimEnd();
                            //var ilce = line.Substring(287, 20).TrimEnd();

                            ekstreAktarma.TCKimlikNo = tc.ConvertToLong();
                            ekstreAktarma.Adi = ad.ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo); ;
                            ekstreAktarma.Adres = adres.ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo); ;
                            ekstreAktarma.Telefon1 = UtilityHelper.TelefonFormatla(tel.ReturnEmptyIfNull().ToString()); ;
                            ekstreAktarma.Eposta = eposta;
                            ekstreAktarma.BagisTarihi = ConverYYYMMDDToDateTime(tarih);
                            ekstreAktarma.Tutar = tutar.ConvertToDecimal();
                            ekstreAktarma.DovizCinsi = dovizCinsi;
                            ekstreAktarma.Aciklama = aciklama;
                            ekstreAktarma.BelgeIstemiyor = ekstreAktarma.Aciklama.Contains(ProjeConstants.DURUM_BELGE_ISTEMIYOR) || ekstreAktarma.Adres.Contains(ProjeConstants.DURUM_BELGE_ISTEMIYOR) ? true : false;
                            ekstreAktarma.AktarildiMi = false;
                            ekstreAktarma.BankaAdi = ProjeConstants.BANKA_TEB;


                            //ekstreAktarma.Ili = il;
                            //ekstreAktarma.Ilcesi = ilce;
                            ekstreAktarma.IslemTarihi = processTime;
                            ekstreAktarma.DovizCinsi = ProjeConstants.DOVIZ_TL;
                            ekstreAktarma.Olusturan = currentUser;

                            SaveEkstreAktarma(ekstreAktarma);

                        }
                    }
                    catch (Exception ex)
                    {
                        Exception exceprion = new Exception(string.Format("HATA SATIRI {0}:{1} ->", ProjeConstants.BANKA_TEB, line), ex);
                        exceptionHelper.Exceptions.Add(exceprion);
                    }
                }
                count++;
            }
            return exceptionHelper;
        }
        public static ExceptionHelper SaveZiraatFile(Stream fileStream, DateTime islemTarihi, string currentUser)
        {
            ExceptionHelper exceptionHelper = new ExceptionHelper();
            CultureInfo culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            List<string> lines = HelperFunctions.ConvertTextFileToList(fileStream);
            int count = 0;
            foreach (var line in lines)
            {
                try
                {
                    if (count > 0) //ilk satir alinmiyor
                    {
                        EkstreAktarma ekstreAktarma = new EkstreAktarma();

                        //var ad = line.Substring(44, 20).TrimEnd();
                        //var soyisim = line.Substring(65, 20).TrimEnd();
                        //var tc = line.Substring(32, 11).TrimEnd();
                        //var tel1 = line.Substring(86, 10).Trim();
                        //var tel2 = line.Substring(179, 20).Trim();
                        //var adres = line.Substring(128, 50).TrimEnd();
                        //var tutar = line.Substring(240, 10).TrimEnd();
                        //var eposta = line.Substring(200, 30).TrimEnd();
                        //var tarih = line.Substring(231, 8).Trim();
                        //var aciklama = line.Substring(97, 30).TrimEnd();
                        //var il = line.Substring(0, 15).TrimEnd();
                        //var ilce = line.Substring(16, 15).TrimEnd();
                        var holder = line.Split(';');
                        var ad = holder[3].Trim();
                        var soyisim = holder[4].Trim();
                        var tc = holder[2].Trim();
                        var tel1 = holder[5].Trim();
                        var tel2 = holder[8].Trim();
                        var adres = holder[7].Trim();
                        decimal tutar = holder[11].ReturnZeroIfNull().ToString().Trim().Replace(".", ",").ConvertToDecimal();
                        var eposta = holder[9].Trim();
                        var tarih = holder[15].Trim();
                        var aciklama = holder[6].Trim();
                        var il = holder[0].Trim();
                        var ilce = holder[1].Trim();
                        var fisNo = holder[13].Trim();

                        ekstreAktarma.TCKimlikNo = tc.ConvertToLong();
                        ekstreAktarma.Adi = string.Format("{0} {1}", ad, soyisim).ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo);
                        ekstreAktarma.Adres = adres.ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo); ;
                        ekstreAktarma.Telefon1 = UtilityHelper.TelefonFormatla(tel1.ReturnEmptyIfNull().ToString());
                        ekstreAktarma.Telefon2 = UtilityHelper.TelefonFormatla(tel2.ReturnEmptyIfNull().ToString());
                        ekstreAktarma.Tutar = tutar;
                        ekstreAktarma.Aciklama = aciklama;
                        ekstreAktarma.BelgeIstemiyor = ekstreAktarma.Aciklama.Contains(ProjeConstants.DURUM_BELGE_ISTEMIYOR) || ekstreAktarma.Adres.Contains(ProjeConstants.DURUM_BELGE_ISTEMIYOR) ? true : false;
                        ekstreAktarma.BagisTarihi = tarih.ConvertToDatetime();// ConverYYYMMDDToDateTime(tarih);
                        ekstreAktarma.AktarildiMi = false;
                        ekstreAktarma.BankaAdi = ProjeConstants.BANKA_ZIRAAT;
                        ekstreAktarma.Ili = GetIlNameById(il);
                        //ekstreAktarma.Ilcesi = ilce; ziraat bankasindan gelen ilce kodlari tutmuyor. bu yuzden ilceyi yazmasin
                        ekstreAktarma.Eposta = eposta;
                        ekstreAktarma.IslemTarihi = islemTarihi;
                        ekstreAktarma.DovizCinsi = ProjeConstants.DOVIZ_TL;
                        ekstreAktarma.Olusturan = currentUser;
                        ekstreAktarma.FisNo = fisNo;
                        if (BuKayitDahaOnceGirilmisMiByFisNo(//ProjeConstants.BANKA_ZIRAAT,fisNo,aciklama))
                            "Ziraat", fisNo, aciklama, tarih.ReturnEmptyIfNull().ConvertToDatetime(), tutar)) //fis numarasindan kontrol et kayit girilmemisse exception dondur degilse kaydet
                        {
                            Exception ex = new Exception(fisNo + " Numaralı fiş daha önce girildiginden tekrar aktarılmadı.");
                            exceptionHelper.Exceptions.Add(ex);
                        }
                        else
                        {
                            SaveEkstreAktarma(ekstreAktarma);

                        }

                    }
                }
                catch (Exception ex)
                {
                    Exception exception = new Exception(string.Format("HATA SATIRI {0}:{1} ->", ProjeConstants.BANKA_ZIRAAT, line), ex);
                    exceptionHelper.Exceptions.Add(exception);
                }
                count++;
            }
            return exceptionHelper;
        }
        public static ExceptionHelper SaveKartIleFile(Stream fileStream, DateTime islemTarihi, string currentUser, DateTime bagisTarihi)
        {
            int kayitNo = ProjeConstants.BANKA_KARTILE_ILKKACSATIRHARIC;
            ExceptionHelper exceptionHelper = new ExceptionHelper();
            CultureInfo culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            try
            {

                var kartIleData = ExcelHelper.ReadXLSXAsDataTableKartIle(fileStream, ProjeConstants.BANKA_KARTILE_ILKKACSATIRHARIC, ProjeConstants.BANKA_KARTILE_SONKACSATIRHARIC, true);
                if (kartIleData != null)
                {


                    foreach (DataRow row in kartIleData.Rows)
                    {
                        kayitNo++;
                        EkstreAktarma ekstreAktarma = new EkstreAktarma();
                        var adi = row[0].ReturnEmptyIfNull().ToString();
                        var onayli = row[11].ReturnEmptyIfNull().ToString();
                        bool kaydiAtla = !onayli.Equals("ONAYLI");
                        if (kaydiAtla)// onayli olmayanlari atla
                            continue;
                        try
                        {

                            {
                                var soyAdi = row[1].ReturnEmptyIfNull().ToString();
                                var tcKimlikNo = row[2].ReturnEmptyIfNull().ToString();
                                var telefon2 = row[3].ReturnEmptyIfNull().ToString();

                                var adres = row[4].ReturnEmptyIfNull().ToString();
                                var il = row[5].ReturnEmptyIfNull().ToString();
                                var ulke = row[6].ReturnEmptyIfNull().ToString();
                                var ilce = row[7].ReturnEmptyIfNull().ToString();
                                var eposta = row[8].ReturnEmptyIfNull().ToString();
                                var telefon1 = row[9].ReturnEmptyIfNull().ToString();
                                var tutar = row[10].ReturnEmptyIfNull().ToString().Replace(".", ",").ConvertToDecimal();

                                var aciklama = row[13].ReturnEmptyIfNull().ToString();

                                var transactionId = row[14].ToString().Trim();
                                //var bagisTarihi = row[16].ReturnEmptyIfNull().ToString().ConvertToDatetime();
                                var belgeIstiyorMu = row[17].ToString().Trim();

                                if (tutar > 0)
                                {
                                    ekstreAktarma.Adi = adi + " " + soyAdi;
                                    ekstreAktarma.TCKimlikNo = tcKimlikNo.ConvertToLong();
                                    ekstreAktarma.Telefon1 = UtilityHelper.TelefonFormatla(telefon1.ReturnEmptyIfNull().ToString());
                                    ekstreAktarma.Telefon2 = UtilityHelper.TelefonFormatla(telefon2.ReturnEmptyIfNull().ToString());
                                    ekstreAktarma.Adres = adres;
                                    ekstreAktarma.Ili = il;
                                    string ulkestr = ulke.ToUpper();
                                    if (ulkestr.Equals("TURKEY") || ulkestr.Equals("TÜRKİYE"))
                                    {

                                    }
                                    else if (ulkestr.Equals("ALMANYA") || ulkestr.Equals("GERMANY"))
                                    {
                                        ekstreAktarma.Ili = "ALMANYA";
                                    }
                                    else
                                    {
                                        ekstreAktarma.Ili = "YURTDIŞI";
                                    }
                                    ekstreAktarma.Ilcesi = ilce;
                                    ekstreAktarma.Adres = adres;
                                    ekstreAktarma.Eposta = eposta;
                                    ekstreAktarma.AktarildiMi = false;
                                    ekstreAktarma.Tutar = tutar.ConvertToDecimal();
                                    //alt satir kapatildi. Ay sonu ve basinda hesabi tutturamiyo (Deniz ÖZKAN) bagis tarihini kendi seçsin S.B. 30.12.2020
                                    //ekstreAktarma.BagisTarihi = bagisTarihi.AddDays(1);//burada bagis tarihine bir gün eklenmesinin sebebi, bankanin gün içindeki bagislari vakif hesabina ertesi gün kaydetmesi nedeniyle olusan tutarsizligi gidermektir. 15.06.2020 SB
                                    ekstreAktarma.BagisTarihi = bagisTarihi;
                                    ekstreAktarma.Aciklama = aciklama;
                                    ekstreAktarma.BelgeIstemiyor = belgeIstiyorMu.Equals("-") ? true : false;
                                    ekstreAktarma.FisNo = transactionId;
                                    ekstreAktarma.BankaAdi = ProjeConstants.BANKA_KARTILEBAGIS;
                                    ekstreAktarma.IslemTarihi = islemTarihi;
                                    ekstreAktarma.DovizCinsi = ProjeConstants.DOVIZ_TL;
                                    ekstreAktarma.Olusturan = currentUser;
                                    if (BuKayitDahaOnceGirilmisMiByFisNo(//transactionId numarasindan kontrol et kayit girilmemisse exception dondur degilse kaydet
                                        "Kart ile Bağış", transactionId, aciklama, bagisTarihi, tutar)) //transactionId numarasindan kontrol et kayit girilmemisse exception dondur degilse kaydet
                                    {
                                        Exception ex = new Exception(transactionId + " Numaralı kayıt daha önce girildiginden tekrar aktarılmadı.");
                                        exceptionHelper.Exceptions.Add(ex);
                                    }
                                    else
                                    {
                                        SaveEkstreAktarma(ekstreAktarma);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Exception exception = new Exception(string.Format("HATA SATIRI {0}:{1} ->", ProjeConstants.BANKA_KARTILEBAGIS, "Dosya Sira No=" + kayitNo), ex);
                            exceptionHelper.Exceptions.Add(exception);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                exceptionHelper.Exceptions.Add(ex);
            }
            return exceptionHelper;
        }
        public static ExceptionHelper SaveZiraatMT940File(Stream filePath, DateTime processTime, string currentUser)
        {
            ExceptionHelper exceptionHelper = new ExceptionHelper();
            CultureInfo culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            List<string> lines = HelperFunctions.ConvertTextFileToList(filePath);
            int count = 0;

            foreach (var line in lines)
            {
                //if (count == 0)//ilk satirda tarih bilgisi var
                //{
                //    tarih = line.Substring(10, 8);
                //}
                if (count >= 0 && count < (lines.Count - 1)) //ilk satir ve son satir alinmayacak
                {
                    try
                    {
                        //ilk 4 karakteri al
                        var tag = line.Substring(0, 4).TrimEnd();
                        switch (tag)
                        {
                            case ":20:": // :20: referans numarasi
                                var referans = line.Substring(5, line.Length - 5); //- 5 mi olmali?
                                break;
                            case ":28:":

                                break;

                            case ":60F":// açilis bilgileri

                                break;
                            case ":61:":// Bu günkü islem
                                var tarih = line.Substring(5, 6);
                                var girisTarihi = line.Substring(11, 4); //aagg
                                var borcAlacak = line.Substring(15, 1); // b/a
                                var dovizCinsi = line.Substring(16, 1); //Y : YTL galiba
                                //mt940 stadardinda tutar 15 char ama ziraat buna uymuyor o nedenle satirin kalanini ayri pars edecegiz
                                var geriKalan = line.Substring(17, line.Length - 17);
                                var holder = geriKalan.Split(',');
                                var tutarTamsayi = holder.Length > 0 ? holder[0].ConvertToInt() : 0;
                                var tutarKurus = holder[1].ToString().Substring(0, 2).ConvertToInt();
                                var tutar = tutarTamsayi + "," + tutarKurus;

                                var fisNo = holder[1].ToString().Split('/')[1].ToString();
                                break;
                            case ":86:": //bugunku islemin açiklamasi

                                break;
                            case ":62F": //kapanis bilgileri

                                break;
                        }

                        //EkstreAktarma ekstreAktarma = new EkstreAktarma();

                        ////TCKN




                        //    ekstreAktarma.TCKimlikNo = tc.ConvertToLong();
                        //    ekstreAktarma.Adi = ad.ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo); ;
                        //    ekstreAktarma.Adres = adres.ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo); ;
                        //    ekstreAktarma.Telefon1 = UtilityHelper.TelefonFormatla(tel.ReturnEmptyIfNull().ToString()); ;
                        //    ekstreAktarma.Eposta = eposta;
                        //    ekstreAktarma.BagisTarihi = ConverYYYMMDDToDateTime(tarih);
                        //    ekstreAktarma.Tutar = tutar.ConvertToDecimal();
                        //    ekstreAktarma.DovizCinsi = dovizCinsi;
                        //    ekstreAktarma.Aciklama = aciklama;
                        //    ekstreAktarma.BelgeIstemiyor = ekstreAktarma.Aciklama.Contains(ProjeConstants.DURUM_BELGE_ISTEMIYOR) || ekstreAktarma.Adres.Contains(ProjeConstants.DURUM_BELGE_ISTEMIYOR) ? true : false;
                        //    ekstreAktarma.AktarildiMi = false;
                        //    ekstreAktarma.BankaAdi = ProjeConstants.BANKA_TEB;


                        //    //ekstreAktarma.Ili = il;
                        //    //ekstreAktarma.Ilcesi = ilce;
                        //    ekstreAktarma.IslemTarihi = processTime;
                        //    ekstreAktarma.DovizCinsi = ProjeConstants.DOVIZ_TL;
                        //    ekstreAktarma.Olusturan = currentUser;

                        //    ekstreAktarma.Save();

                    }
                    catch (Exception ex)
                    {
                        Exception exceprion = new Exception(string.Format("HATA SATIRI {0}:{1} ->", ProjeConstants.BANKA_ZIRAATMT940, line), ex);
                        exceptionHelper.Exceptions.Add(exceprion);
                    }
                }
                count++;
            }
            return exceptionHelper;
        }
        public static ExceptionHelper SaveEDevletFile(Stream fileStream, DateTime islemTarihi, string currentUser)
        {
            int kayitNo = ProjeConstants.BANKA_EDEVLET_ILKKACSATIRHARIC;
            ExceptionHelper exceptionHelper = new ExceptionHelper();
            CultureInfo culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            try
            {
                var edevletData = ExcelHelper.ReadXLSXAsDataTableKartIle(fileStream, ProjeConstants.BANKA_EDEVLET_ILKKACSATIRHARIC,
                    ProjeConstants.BANKA_EDEVLET_SONKACSATIRHARIC, true);
                if (edevletData != null)
                {
                    foreach (DataRow row in edevletData.Rows)
                    {
                        kayitNo++;
                        EkstreAktarma ekstreAktarma = new EkstreAktarma();

                        try
                        {
                            var odemeId = row[0].ReturnEmptyIfNull().ToString();
                            var adi = row[1].ReturnEmptyIfNull().ToString();
                            //var soyAdi = row[1].ReturnEmptyIfNull().ToString();
                            var tcKimlikNo = row[2].ReturnEmptyIfNull().ToString();
                            var telefon = row[3].ReturnEmptyIfNull().ToString();
                            var eposta = row[4].ReturnEmptyIfNull().ToString();

                            var tutar = row[5].ReturnZeroIfNull().ToString().Replace("₺", "").Replace(".", ",").ConvertToDecimal();
                            var aciklama = row[6].ReturnEmptyIfNull().ToString();
                            var adres = row[7].ReturnEmptyIfNull().ToString();
                            var bagisKanali = row[8].ReturnEmptyIfNull().ToString();
                            var odemeMetodu = row[9].ReturnEmptyIfNull().ToString();
                            //var il = row[5].ReturnEmptyIfNull().ToString();
                            //var ulke = row[6].ReturnEmptyIfNull().ToString();
                            //var ilce = row[7].ReturnEmptyIfNull().ToString();

                            var bagisTipi = row[10].ReturnEmptyIfNull().ToString();
                            var bagisTarihi = row[11].ReturnEmptyIfNull().ToString().ConvertToDatetime();
                            var sonIslemTarihi = row[12].ReturnEmptyIfNull().ToString().ConvertToDatetime();

                            if (tutar > 0)
                            {
                                ekstreAktarma.Adi = adi.Trim();
                                ekstreAktarma.TCKimlikNo = tcKimlikNo.ConvertToLong();
                                ekstreAktarma.Telefon1 = UtilityHelper.TelefonFormatla(telefon.ReturnEmptyIfNull().ToString());
                                ekstreAktarma.Adres = adres;
                                //ekstreAktarma.Ili = il;
                                //ekstreAktarma.Ilcesi = ilce;
                                ekstreAktarma.Eposta = eposta;
                                ekstreAktarma.AktarildiMi = false;
                                ekstreAktarma.Tutar = tutar.ConvertToDecimal();
                                //Alt satir kapatildi 08.05.2025 SB
                                //ekstreAktarma.BagisTarihi = bagisTarihi.AddDays(1);//burada bagis tarihine bir gün eklenmesinin sebebi, bankanin gün içindeki bagislari vakif hesabina ertesi gün kaydetmesi nedeniyle olusan tutarsizligi gidermektir. 15.06.2020 SB
                                ekstreAktarma.BagisTarihi = bagisTarihi; // Bankadan ayrica gelen bilgi olmadigindan gün eklemesi kaldirildi
                                ekstreAktarma.Aciklama = aciklama;
                                ekstreAktarma.FisNo = odemeId;
                                if (bagisKanali.ReturnEmptyIfNull().Equals("E-Devlet"))
                                    ekstreAktarma.BankaAdi = ProjeConstants.BANKA_EDEVLETBAGIS;
                                else if (bagisKanali.ReturnEmptyIfNull().Equals("Kiosk"))
                                    ekstreAktarma.BankaAdi = ProjeConstants.BANKA_KIOSK;
                                else if (bagisKanali.ReturnEmptyIfNull().Equals("Sistem içi"))
                                    ekstreAktarma.BankaAdi = ProjeConstants.BANKA_KARTILEBAGIS;
                                else
                                    continue;// diger kanallardan gelen bagislari kaydetme zaten var
                                ekstreAktarma.IslemTarihi = islemTarihi;
                                ekstreAktarma.DovizCinsi = ProjeConstants.DOVIZ_TL;
                                ekstreAktarma.BagisTipi = bagisTipi;
                                ekstreAktarma.Olusturan = currentUser;
                                if (BuKayitDahaOnceGirilmisMiByFisNo(//odemeId numarasindan kontrol et kayit girilmemisse exception dondur degilse kaydet
                                    "EDevlet ile Bağış", odemeId, aciklama, ekstreAktarma.BagisTarihi, tutar)) //transactionId numarasindan kontrol et kayit girilmemisse exception dondur degilse kaydet
                                {
                                    Exception ex = new Exception(odemeId + " Numaralı kayıt daha önce girildiginden tekrar aktarılmadı.");
                                    exceptionHelper.Exceptions.Add(ex);
                                }
                                else if (BuKayitDahaOnceGirilmisMiByFisNo(//odemeId numarasindan kontrol et kayit girilmemisse exception dondur degilse kaydet
                                    "Kart ile Bağış", odemeId, aciklama, ekstreAktarma.BagisTarihi, tutar)) //transactionId numarasindan kontrol et kayit girilmemisse exception dondur degilse kaydet
                                {
                                    Exception ex = new Exception(odemeId + " Numaralı kayıt daha önce girildiginden tekrar aktarılmadı.");
                                    exceptionHelper.Exceptions.Add(ex);
                                }
                                else if (BuKayitDahaOnceGirilmisMiByFisNo(//odemeId numarasindan kontrol et kayit girilmemisse exception dondur degilse kaydet
                                    "Kiosk", odemeId, aciklama, ekstreAktarma.BagisTarihi, tutar)) //transactionId numarasindan kontrol et kayit girilmemisse exception dondur degilse kaydet
                                {
                                    Exception ex = new Exception(odemeId + " Numaralı kayıt daha önce girildiginden tekrar aktarılmadı.");
                                    exceptionHelper.Exceptions.Add(ex);
                                }
                                else
                                {
                                    SaveEkstreAktarma(ekstreAktarma);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            Exception exception = new Exception(string.Format("HATA SATIRI {0}:{1} ->", ProjeConstants.BANKA_EDEVLETBAGIS, "Dosya Sıra No=" + kayitNo), ex);
                            exceptionHelper.Exceptions.Add(exception);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                exceptionHelper.Exceptions.Add(ex);
            }
            return exceptionHelper;
        }
        private static bool BuKayitDahaOnceGirilmisMiByFisNo(string bankaLike, string fisNo, string aciklama, DateTime bagisTarihi, decimal tutar)
        {
            bool kaydedilmisMi = false;
            List<EkstreAktarma> ekstreAktarmaList = new EkstreAktarmaService()
                .GetByBankAndReceipt(bankaLike, fisNo);
            foreach (var ekstreAktarma in ekstreAktarmaList)
            {
                if (ekstreAktarma != null)
                {
                    //eger bu kayitin fisnosu daha önce girilmisse bir de bagistarihi ve tutardan kontrol et
                    if ((bagisTarihi == ekstreAktarma.BagisTarihi) && (Math.Round(tutar,2) == Math.Round(ekstreAktarma.Tutar,2)))
                    {
                        kaydedilmisMi = true;
                    }
                    else
                        kaydedilmisMi = false;
                }
            }

            return kaydedilmisMi;
        }

        public static ExceptionHelper SaveZiraatEkstreFile(Stream fileStream, DateTime islemTarihi, string currentUser)
        {
            int kayitNo = 0;
            ExceptionHelper exceptionHelper = new ExceptionHelper();
            CultureInfo culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            try
            {

                var ziraatEkstreData = ExcelHelper.ReadXLSXAsDataTable(fileStream, ProjeConstants.BANKA_ZIRAATEKSTRE_ILKKACSATIRHARIC, ProjeConstants.BANKA_ZIRAATEKSTRE_SONKACSATIRHARIC, true);
                if (ziraatEkstreData != null)
                {


                    foreach (DataRow row in ziraatEkstreData.Rows)
                    {
                        kayitNo++;
                        EkstreAktarma ekstreAktarma = new EkstreAktarma();
                        var tarih = row[0].ReturnEmptyIfNull().ToString();//borc satiri için islem yapma

                        if (string.IsNullOrEmpty(tarih))// ilk kolon bos ise dosya bitti çik
                            break;
                        try
                        {
                            var aciklama = row[2].ReturnEmptyIfNull().ToString();//hersey açiklamanin içinde
                            //if (aciklama.Contains("Tsk Güçlendirme Vakfi Bagis/"))
                            //{
                            //    continue;
                            //}
                            //else
                            {
                                var bagisTarihi = row[0].ReturnEmptyIfNull().ToString().ConvertToDatetime();
                                var fisNo = row[1].ToString().Trim();
                                var tutar = row[3].ReturnEmptyIfNull().ToString().Replace(".", ",").ConvertToDecimal();//row[6].ReturnEmptyIfNull().ToString();
                                if (tutar > 0)
                                {
                                    ekstreAktarma.Tutar = tutar.ConvertToDecimal();
                                    ekstreAktarma.BagisTarihi = bagisTarihi;
                                    ekstreAktarma.Aciklama = aciklama;
                                    ekstreAktarma.BelgeIstemiyor = ekstreAktarma.Aciklama.Contains(ProjeConstants.DURUM_BELGE_ISTEMIYOR) ? true : false;
                                    ekstreAktarma.BankaAdi = ProjeConstants.BANKA_ZIRAATEKSTRE;
                                    ekstreAktarma.IslemTarihi = islemTarihi;
                                    ekstreAktarma.DovizCinsi = ProjeConstants.DOVIZ_TL;
                                    ekstreAktarma.Olusturan = currentUser;
                                    ekstreAktarma.FisNo = fisNo;
                                    if (BuKayitDahaOnceGirilmisMiByFisNo(//ProjeConstants.BANKA_ZIRAATEKSTRE, fisNo,aciklama)) //fis numarasindan kontrol et kayit girilmemisse exception dondur degilse kaydet
                                        "Ziraat", fisNo, aciklama, bagisTarihi, tutar)) //fis numarasindan kontrol et kayit girilmemisse exception dondur degilse kaydet
                                    {
                                        Exception ex = new Exception(fisNo + " Numaralı fiş daha önce girildiginden tekrar aktarılmadı.");
                                        exceptionHelper.Exceptions.Add(ex);
                                    }
                                    else
                                    {
                                        if (aciklama.Contains("6031741-5087"))
                                        {
                                            continue; //6031741-5087 referans numarasi ile gelenleri atla
                                        }
                                        else if (aciklama.Contains("- Pazarcık Depremi Bağış"))
                                        {
                                            // Örnek: "AHMET YILMAZ - Pazarcık Depremi Bağış"
                                            var splitText = new string[] { "- Pazarcık Depremi Bağış" };
                                            var holder = aciklama.Split(splitText, StringSplitOptions.None);
                                            var adi = holder[0].ReturnEmptyIfNull().ToString().TrimEnd();
                                            ekstreAktarma.Adi = adi;
                                        }
                                        else if (aciklama.Contains("Tsk Güçlendirme Vakfı Bağış/"))
                                        {
                                            // Örnek: "Tsk Güçlendirme Vakfi Bağış/Ad-Soyad: SAKINE GENÇOSMAN /Tckn/Vkn: 58957418782 /Gsm: 5313668361 ..."
                                            var holder = aciklama.Split('/');
                                            int holderLength = holder.Length - 1;
                                            var adi = holderLength < 1 ? "" : holder[1].ReturnEmptyIfNull().ToString().TrimEnd().Split(':')[1].ReturnEmptyIfNull().ToString();
                                            var holder3 = holderLength < 3 ? "" : holder[3].ReturnEmptyIfNull().ToString().TrimEnd();
                                            var tc = string.IsNullOrEmpty(holder3) ? "" : holder3.Split(':')[1].ReturnEmptyIfNull().ToString();
                                            var holder4 = holderLength < 4 ? "" : holder[4].ReturnEmptyIfNull().ToString().TrimEnd();
                                            var telstr = string.IsNullOrEmpty(holder4) ? "" : holder4.Split(':')[1].ReturnEmptyIfNull().ToString();
                                            var telefon1 = string.IsNullOrEmpty(telstr) ? "" : telstr.Trim().Split(' ')[0];

                                            ekstreAktarma.Adi = adi;
                                            ekstreAktarma.TCKimlikNo = tc.ConvertToLong();
                                            ekstreAktarma.Telefon1 = UtilityHelper.TelefonFormatla(telefon1.ReturnEmptyIfNull().ToString());
                                        }
                                        else if (aciklama.Contains("Ziraat Mobil Havale"))
                                        {
                                            // Örnek 1: "GÖZDE HILAL ÖÇALAN Ziraat Mobil Havale"
                                            // Örnek 2: "MUSTAFA ÇETINKAYA bagis MUSTAFA ÇETINKAYA Ziraat Mobil Havale"
                                            // Örnek 3: "AHMET DERECI AHMET DERECI Ziraat Mobil Havale" (tekrar eden isim)
                                            var oncesi = aciklama.Substring(0, aciklama.IndexOf("Ziraat Mobil Havale")).TrimEnd();
                                            if (oncesi.Contains(" bagis "))
                                                oncesi = oncesi.Substring(0, oncesi.IndexOf(" bagis ")).TrimEnd();
                                            var kelimeler = oncesi.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                            int yariKelime = kelimeler.Length / 2;
                                            if (yariKelime > 0
                                                && string.Join(" ", kelimeler, 0, yariKelime) == string.Join(" ", kelimeler, yariKelime, kelimeler.Length - yariKelime))
                                                oncesi = string.Join(" ", kelimeler, 0, yariKelime);
                                            ekstreAktarma.Adi = oncesi.Trim().ToUpper(culturInfo);
                                        }
                                        else if (aciklama.StartsWith("Gönd: "))
                                        {
                                            // Örnek: "Gönd: HALIL BARUT bagis 0205-Kuveyt Türk Katilim Bankasi A.S. FAST islemi"
                                            var sonrasi = aciklama.Substring("Gönd: ".Length);
                                            var kelimeler = sonrasi.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                            ekstreAktarma.Adi = (kelimeler.Length >= 2 ? kelimeler[0] + " " + kelimeler[1] : sonrasi).Trim().ToUpper(culturInfo);
                                        }
                                        else if (aciklama.Contains("Genel Bağış"))
                                        {
                                            // Örnek: "Ismail Senkal Genel Bağış Tel:0534 520 2096 Mail:... T.C:10366911490 ..."
                                            var adi = aciklama.Substring(0, aciklama.IndexOf("Genel Bağış")).Trim();
                                            ekstreAktarma.Adi = adi.ToUpper(culturInfo);
                                            if (aciklama.Contains("Tel:"))
                                            {
                                                int telBaslangic = aciklama.IndexOf("Tel:") + 4;
                                                string telKalan = aciklama.Substring(telBaslangic).Trim();
                                                int telBitis = telKalan.Length;
                                                foreach (var ayrac in new[] { " Mail:", " T.C:", " Tckn", " /Gsm" })
                                                {
                                                    int idx = telKalan.IndexOf(ayrac);
                                                    if (idx >= 0 && idx < telBitis) telBitis = idx;
                                                }
                                                ekstreAktarma.Telefon1 = UtilityHelper.TelefonFormatla(telKalan.Substring(0, telBitis).Replace(" ", ""));
                                            }
                                            if (aciklama.Contains("T.C:"))
                                            {
                                                var tcKalan = aciklama.Substring(aciklama.IndexOf("T.C:") + 4).Trim();
                                                ekstreAktarma.TCKimlikNo = tcKalan.Split(new char[] { ' ', '/' })[0].ConvertToLong();
                                            }
                                        }
                                        SaveEkstreAktarma(ekstreAktarma);

                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Exception exception = new Exception(string.Format("HATA SATIRI {0}:{1} ->", ProjeConstants.BANKA_ZIRAATEKSTRE, kayitNo), ex);
                            exceptionHelper.Exceptions.Add(exception);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                exceptionHelper.Exceptions.Add(ex);
            }
            return exceptionHelper;
        }

        public static ExceptionHelper SaveZiraatKatilimFile(Stream fileStream, DateTime processTime, string currentUser)
        {

            CultureInfo culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            ExceptionHelper exceptionHelper = new ExceptionHelper();
            var ziraatKatilimEkstreData = ExcelHelper.ReadXLSXAsDataTable(fileStream, ProjeConstants.BANKA_ZIRAATKATILIM_ILKKACSATIRHARIC, ProjeConstants.BANKA_ZIRAATKATILIM_SONKACSATIRHARIC, true);
            string hata = string.Empty;
            foreach (DataRow row in ziraatKatilimEkstreData.Rows)
            {
                try
                {
                    //var aciklama = row[1].ReturnEmptyIfNull().ToString();
                    //hata = aciklama;
                    //if (!string.IsNullOrEmpty(aciklama) && !aciklama.Contains("Tahsilat Virmani"))//virman degilse
                    //{
                    //    //var kanal = row[1].ReturnEmptyIfNull().ToString();
                    //    //if (!kanal.Equals("BATCH"))//batch islemleri alma
                    //    {
                    //        var tarih = row[0].ReturnEmptyIfNull().ToString().ConvertToDatetime();


                    //        var tutar = row[2].ReturnEmptyIfNull().ToString().Replace(".", ",");
                    //        var adres = string.Empty;// adres bilgisi gelmiyor
                    var aciklama = row[1].ReturnEmptyIfNull().ToString(); //[2] idi 1 oldu
                    hata = aciklama;
                    if (!string.IsNullOrEmpty(aciklama) && !aciklama.Contains("Tahsilat Virmanı"))//Contains("3 57968 1"))//virman degilse
                    {
                        var kanal = row[1].ReturnEmptyIfNull().ToString();
                        if (!kanal.Equals("BATCH"))//batch islemleri alma
                        {
                            var tarih = row[0].ReturnEmptyIfNull().ToString().ConvertToDatetime();


                            var tutar = row[2].ReturnEmptyIfNull().ToString().Replace(".", ",");//[3] idi 2 oldu
                            var adres = string.Empty;// adres bilgisi gelmiyor

                            if (tutar.ReturnZeroIfNull().ConvertToDecimal() > 0)
                            {
                                EkstreAktarma ekstreAktarma = new EkstreAktarma();

                                ekstreAktarma.BankaAdi = ProjeConstants.BANKA_ZIRAAT_KATILIM;
                                ekstreAktarma.BagisTarihi = tarih;
                                ekstreAktarma.DovizCinsi = ProjeConstants.DOVIZ_TL;
                                ekstreAktarma.Tutar = tutar.ConvertToDecimal();
                                ekstreAktarma.Aciklama = aciklama;
                                ekstreAktarma.Adres = adres.ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo); ;
                                ekstreAktarma.BelgeIstemiyor = ekstreAktarma.Aciklama.Contains(ProjeConstants.DURUM_BELGE_ISTEMIYOR) || ekstreAktarma.Adres.Contains(ProjeConstants.DURUM_BELGE_ISTEMIYOR) ? true : false;
                                ekstreAktarma.IslemTarihi = processTime;
                                ekstreAktarma.Olusturan = currentUser;

                                var id = SaveEkstreAktarma(ekstreAktarma);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Exception exceprion = new Exception(string.Format("HATA SATIRI {0}: rowindex={1} ->", ProjeConstants.BANKA_ZIRAAT_KATILIM, hata), ex);
                    exceptionHelper.Exceptions.Add(exceprion);
                }
            }
            return exceptionHelper;
        }
        public static ExceptionHelper SaveSMSVakifFile(Stream fileStream, DateTime islemTarihi, string currentUser)
        {
            int counter = ProjeConstants.BANKA_SMSVAKIF_ILKKACSATIRHARIC;
            ExceptionHelper exceptionHelper = new ExceptionHelper();
            CultureInfo culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            try
            {
                var smsVakifData = ExcelHelper.ReadXLSXAsDataTableKartIle(fileStream, ProjeConstants.BANKA_SMSVAKIF_ILKKACSATIRHARIC,
                    ProjeConstants.BANKA_SMSVAKIF_SONKACSATIRHARIC, true);
                if (smsVakifData != null)
                {
                    foreach (DataRow row in smsVakifData.Rows)
                    {
                        counter++;
                        EkstreAktarma ekstreAktarma = new EkstreAktarma();

                        try
                        {
                            var islem = row[5].ReturnEmptyIfNull().ToString();
                            if (islem.Equals("Gelen EFT Otomatik Yatan"))
                            {

                                var islemNo = row[9].ReturnEmptyIfNull().ToString();
                                var aciklama = row[16].ReturnEmptyIfNull().ToString();
                                var adi = aciklama.Contains("TT MOBİL") ? "TT MOBİL" : (aciklama.Contains("VODAFONE") ? "VODAFONE" : (aciklama.Contains("TURKCELL") ? "TURKCELL" : string.Empty));

                                var tutar = row[6].ReturnZeroIfNull().ToString().Replace("₺", "").Replace(".", ",").ConvertToDecimal();
                                var tcKimlikNo = aciklama.Contains("TT MOBİL") ? 30088888888 : (aciklama.Contains("VODAFONE") ? 30077777777 : (aciklama.Contains("TURKCELL") ? 30066666666 : 0));

                                var bagisTarihi = row[2].ReturnEmptyIfNull().ToString().ConvertToDatetime();
                                var sonIslemTarihi = row[3].ReturnEmptyIfNull().ToString().ConvertToDatetime();

                                if (tutar > 0)
                                {
                                    ekstreAktarma.Adi = adi.Trim();
                                    ekstreAktarma.AktarildiMi = false;
                                    ekstreAktarma.Tutar = tutar.ConvertToDecimal();
                                    ekstreAktarma.BagisTarihi = bagisTarihi;
                                    ekstreAktarma.Aciklama = aciklama;
                                    ekstreAktarma.FisNo = islemNo;
                                    ekstreAktarma.BankaAdi = ProjeConstants.BANKA_SMSVAKIF;
                                    ekstreAktarma.IslemTarihi = islemTarihi;
                                    ekstreAktarma.DovizCinsi = ProjeConstants.DOVIZ_TL;
                                    ekstreAktarma.Olusturan = currentUser;
                                    if (BuKayitDahaOnceGirilmisMiByFisNo(//odemeId numarasindan kontrol et kayit girilmemisse exception dondur degilse kaydet
                                        ProjeConstants.BANKA_SMSVAKIF, islemNo, aciklama, ekstreAktarma.BagisTarihi, tutar)) //transactionId numarasindan kontrol et kayit girilmemisse exception dondur degilse kaydet
                                    {
                                        Exception ex = new Exception(islemNo + " Numaralı kayıt daha önce girildiğinden tekrar aktarılmadı.");
                                        exceptionHelper.Exceptions.Add(ex);
                                    }
                                    else
                                    {
                                        SaveEkstreAktarma(ekstreAktarma);
                                    }
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            Exception exception = new Exception(string.Format("HATA SATIRI {0}:{1} ->", ProjeConstants.BANKA_KIOSK, "Dosya Sıra No=" + counter), ex);
                            exceptionHelper.Exceptions.Add(exception);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                exceptionHelper.Exceptions.Add(ex);
            }
            return exceptionHelper;
        }
        public static ExceptionHelper SaveAlbarakaFile(Stream fileStream, DateTime islemTarihi, string currentUser)
        {
            int kayitNo = 0;
            ExceptionHelper exceptionHelper = new ExceptionHelper();
            CultureInfo culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            try
            {
                // Dosya yapisi:
                // Satir 1-7 : meta/logo/baslik bilgileri (atlanir)
                // Satir 8   : kolon basliklari (A:Tarih, B:Kanal, C:Fis No, D:Açiklama, E:Tutar(TL), F:Güncel Bakiye)
                // Satir 9+  : veri satirlari
                var data = ExcelHelper.ReadXLSXAsDataTable(fileStream,
                    ProjeConstants.BANKA_ALBARAKA_ILKKACSATIRHARIC,
                    ProjeConstants.BANKA_ALBARAKA_SONKACSATIRHARIC, true);

                if (data != null)
                {
                    foreach (DataRow row in data.Rows)
                    {
                        kayitNo++;
                        var tarihStr = row[0].ReturnEmptyIfNull().ToString();
                        if (string.IsNullOrEmpty(tarihStr))
                            break;

                        try
                        {
                            var kanal = row[1].ReturnEmptyIfNull().ToString().Trim();
                            var fisNo = row[2].ReturnEmptyIfNull().ToString().Trim();
                            var aciklama = row[3].ReturnEmptyIfNull().ToString().Trim();
                            var tutar = row[4].ReturnZeroIfNull().ToString().Replace(".", ",").ConvertToDecimal();

                            // Sadece pozitif tutarlar alinir; bakiye satirlari vb. atlanir
                            if (tutar <= 0)
                                continue;

                            // "AD SOYAD / GENEL BAGIS" formatindan ad soyad çikar
                            string adi = string.Empty;
                            if (aciklama.Contains("/"))
                            {
                                adi = aciklama.Substring(0, aciklama.IndexOf("/")).Trim();
                            }
                            else
                            {
                                adi = aciklama.Trim();
                            }

                            // Tarih: "01.04.2026 21:08" formati — sadece tarih kismi alinir
                            string tarihKismi = tarihStr.Contains(" ") ? tarihStr.Substring(0, tarihStr.IndexOf(" ")) : tarihStr;
                            DateTime bagisTarihi = tarihKismi.ConvertToDatetime();

                            if (BuKayitDahaOnceGirilmisMiByFisNo("Albaraka", fisNo, aciklama, bagisTarihi, tutar))
                            {
                                Exception ex = new Exception(fisNo + " Numaralı fiş daha önce girildiğinden tekrar aktarılmadı.");
                                exceptionHelper.Exceptions.Add(ex);
                                continue;
                            }

                            EkstreAktarma ekstreAktarma = new EkstreAktarma();
                            ekstreAktarma.Adi = adi.ToUpper(culturInfo);
                            ekstreAktarma.Tutar = tutar;
                            ekstreAktarma.BagisTarihi = bagisTarihi;
                            ekstreAktarma.Aciklama = aciklama;
                            ekstreAktarma.FisNo = fisNo;
                            ekstreAktarma.BelgeIstemiyor = aciklama.Contains(ProjeConstants.DURUM_BELGE_ISTEMIYOR);
                            ekstreAktarma.BankaAdi = ProjeConstants.BANKA_ALBARAKA;
                            ekstreAktarma.IslemTarihi = islemTarihi;
                            ekstreAktarma.DovizCinsi = ProjeConstants.DOVIZ_TL;
                            ekstreAktarma.Olusturan = currentUser;
                            SaveEkstreAktarma(ekstreAktarma);
                        }
                        catch (Exception ex)
                        {
                            Exception exception = new Exception(string.Format("HATA SATIRI {0}:{1} ->", ProjeConstants.BANKA_ALBARAKA, kayitNo), ex);
                            exceptionHelper.Exceptions.Add(exception);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                exceptionHelper.Exceptions.Add(ex);
            }
            return exceptionHelper;
        }

        private static string GetIlNameById(string ilId)
        {
            int id = 0;
            string ilAdi = ilId;
            bool isInteger = Int32.TryParse(ilId, out id);
            if (isInteger)
            {
                Il il = new Il();
                il = il.Select<Il>(id);
                if (il != null)
                {
                    ilAdi = il.IlAdi;
                }
            }
            return ilAdi;
        }
        #region ConvertTextToDatetime
        private static DateTime ConvertAkbankTextToDateTime(string value)
        {
            DateTime result = new DateTime();
            if (!string.IsNullOrEmpty(value))
            {
                var holder = value.Split('.');
                if (holder.Length > 2)
                {
                    result = new DateTime(holder[2].ConvertToInt(), holder[1].ConvertToInt(), holder[0].ConvertToInt());
                }
            }

            return result;
        }
        private static DateTime ConvertIsBankasiTextToDateTime(string value)
        {
            DateTime result = new DateTime();
            if (!string.IsNullOrEmpty(value))
            {
                var holder = value.Split('/');
                if (holder.Length > 2)
                {
                    result = new DateTime(holder[2].ConvertToInt(), holder[1].ConvertToInt(), holder[0].ConvertToInt());
                }
            }

            return result;
        }

        private static DateTime ConverYYYMMDDToDateTime(string value)
        {
            DateTime result = new DateTime();
            if (!string.IsNullOrEmpty(value))
            {
                if (value.Length == 8)
                {
                    result = new DateTime(value.Substring(0, 4).ConvertToInt(), value.Substring(4, 2).ConvertToInt(), value.Substring(6, 2).ConvertToInt());
                }
            }

            return result;
        }

        private static DateTime ConvertTextToDateTime(string value)
        {
            DateTime result = new DateTime();
            if (!string.IsNullOrEmpty(value))
            {
                var holder = value.Split('-');
                if (holder.Length > 2)
                {
                    result = new DateTime(holder[0].ConvertToInt(), holder[1].ConvertToInt(), holder[2].ConvertToInt());
                }
            }
            return result;
        }
        #endregion
        private static string RemoveWhiteSpaces(string value)
        {
            var holder = value.Split(' ');
            var result = string.Empty;

            foreach (var item in holder)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    result += item + " ";
                }
            }
            result = result.TrimEnd();

            return result;
        }
    }
}
