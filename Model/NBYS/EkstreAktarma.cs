using DAO.Ortak;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.NBYS
{
    [Serializable]
    public class EkstreAktarma : ParentClass
    {
        public long TCKimlikNo { get; set; }
        public string Telefon1 { get;set; }
        public string Telefon2{ get;set; }
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public string Ili { get; set; }
        public string Ilcesi { get; set; }
        public string Adres { get; set; }
        public string Aciklama { get; set; }
        public DateTime BagisTarihi { get; set; }
        public string BankaAdi { get; set; }
        public decimal Tutar { get; set; }
        public string DosyaYolu { get; set; }
        public bool AktarildiMi { get; set; }
        public string DovizCinsi { get; set; }
        public string Eposta { get; set; }
        public string PostaKodu { get; set; }
        public DateTime IslemTarihi { get; set; }
        public bool TuzelKisi { get; set; }
        public bool ElleKayit { get; set; }
        public int NakitBagisHareketId { get; set; }
        public int NakitBagisciId { get; set; }
        public bool BelgeIstemiyor { get; set; }
        public string FisNo { get; set; }
        public override bool Delete()
        {
            string sqlString = string.Format(@"DELETE 
                               FROM EkstreAktarma_Table
                               WHERE Id={0}", Id);

            bool isSuccess = dao.DeleteFromDb(sqlString, this);

            return isSuccess;
        }
        public override int Save()
        {
            try
            {
                GenericEntity<EkstreAktarma> genericEntity = new GenericEntity<EkstreAktarma>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);

                this.Id = id;
                return id;
            }
            catch (Exception)
            {

                throw;
            }

        }
        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM EkstreAktarma_Table 
                               WHERE  Id={0}", id);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<EkstreAktarma> list = ToList<EkstreAktarma>(dataTable);
            EkstreAktarma ekstreAktarma = new EkstreAktarma();
            ekstreAktarma = list.FirstOrDefault();
            return (T)Convert.ChangeType(ekstreAktarma, typeof(T));
        }
        public List<EkstreAktarma> SelectByIslemTarihi(DateTime processTime)
        {
            //string sqlString = string.Format(@"SELECT *
            //                                 FROM EkstreAktarma_Table
            //                                 WHERE  DATEDIFF(day,IslemTarihi,{0})=0",processTime.ReturnTRDateFormat());
            string sqlString = string.Format(@"
                SELECT *,
                    Telefon1 + IIF(ISNULL(Telefon1,'')!='' AND ISNULL(Telefon2,'')!='',' - ','') + Telefon2 Telefon
                FROM EkstreAktarma_Table
                WHERE IslemTarihi={0}
                ORDER BY AktarildiMi, BagisTarihi desc, Adi
                ", processTime.ReturnTRDateFormat());
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<EkstreAktarma> list = ToList<EkstreAktarma>(dataTable);

            return list;
        }

        public DataTable SelectByIslemTarihi(DateTime islemTarihi, ref int rowCount, bool aktarilanlarHaric, string banka)
        {
            string aktarilanlarHaricStr = aktarilanlarHaric ? " AND AktarildiMi=0 " : "";
            banka = banka.Equals(ProjeConstants.HEPSI) ? string.Empty : banka;//hepsi geldiyse bankastr boş olsun            
            string bankaStr = string.IsNullOrEmpty(banka) ? "" : " AND BankaAdi= " + banka.ReturnQuotedValue(); ;
            string sqlString = string.Format(@"
                SELECT Id EkstreAktarmaId, BankaAdi, TCKimlikNo, Adi, Soyadi, ISNULL(Adi,'')  +' ' +ISNULL(Soyadi,'') AdiSoyadi,
                    BagisTarihi, Tutar, DovizCinsi, AktarildiMi,Adres, Aciklama,Telefon1,Telefon2,
                    Telefon1 + IIF(ISNULL(Telefon1,'')!='' AND ISNULL(Telefon2,'')!='',' - ','') + Telefon2 Telefon
                FROM EkstreAktarma_Table
                WHERE IslemTarihi={0}
                {1}
                {2}
                ORDER BY AktarildiMi,  BagisTarihi desc, Id, Adi
                ", islemTarihi.ReturnTRDateFormat(), aktarilanlarHaricStr, bankaStr);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            //List<EkstreAktarma> list = ToList<EkstreAktarma>(dataTable);
            rowCount = dataTable != null ? dataTable.Rows.Count : 0;
            return dataTable;
        }
        public DataTable SelectByIslemTarihiReturnDT(DateTime islemTarihi, bool aktarilanlarHaric, string banka)
        {
            string aktarilanlarHaricStr = aktarilanlarHaric ? " AND AktarildiMi=0 " : "";
            banka = banka.Equals(ProjeConstants.HEPSI) ? string.Empty : banka;//hepsi geldiyse bankastr boş olsun            
            string bankaStr = string.IsNullOrEmpty(banka) ? "" : " AND BankaAdi= " + banka.ReturnQuotedValue(); ;
            string sqlString = string.Format(@"
                SELECT COUNT(A.Id) KayitAdedi, A.Id EkstreAktarmaId, A.BankaAdi, A.TCKimlikNo, A.Adi,A.Soyadi, ISNULL(A.Adi,'')  +' ' +ISNULL(A.Soyadi,'') AdiSoyadi,
                    A.BagisTarihi, A.Tutar, A.DovizCinsi, A.AktarildiMi,A.Adres, A.Aciklama,
                    A.Telefon1,A.Telefon2,B.Telefon1 Btelefon1,B.Telefon2 Btelefon2,B.TCKimlikNo BTCKimlikNo,B.Id NBId,
                    A.Telefon1 + IIF(ISNULL(A.Telefon1,'')!='' AND ISNULL(A.Telefon2,'')!='',' - ','') + A.Telefon2 Telefon
                FROM EkstreAktarma_Table A
				LEFT JOIN NakitBagisci_Table b on LTRIM(RTRIM(UPPER(B.Adi))) =LTRIM(RTRIM(UPPER(A.Adi)))
                WHERE IslemTarihi={0}
                    {1}
                    {2}
                GROUP BY A.Id,A.BankaAdi, A.TCKimlikNo, A.Adi,A.Soyadi, 
                    A.BagisTarihi, A.Tutar, A.DovizCinsi, A.AktarildiMi,A.Adres, A.Aciklama,
                    A.Telefon1,A.Telefon2  ,B.Telefon1,B.Telefon2,B.TCKimlikNo,B.Id
                ORDER BY AktarildiMi, BagisTarihi desc, A.Id, A.Adi
                ", islemTarihi.ReturnTRDateFormat(), aktarilanlarHaricStr, bankaStr);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            //List<EkstreAktarma> list = ToList<EkstreAktarma>(dataTable);
            return dataTable;
        }
        //public EkstreAktarma SelectByFisNoBanka(string banka, string fisNo )
        //{
        //    string sqlString = string.Format(@"
        //        SELECT *
        //        FROM EkstreAktarma_Table
        //        WHERE BankaAdi={0} AND FisNo={1}                
        //        ", banka.ReturnQuotedValue(), fisNo.ReturnQuotedValue());
        //    DataTable dataTable = dao.selectFromDb(sqlString, "");
        //    List<EkstreAktarma> list = ToList<EkstreAktarma>(dataTable);
        //    EkstreAktarma ea = new EkstreAktarma();
        //    ea = list.FirstOrDefault();
        //    return ea;
        //}
        public List<EkstreAktarma> SelectByFisNoBanka(string bankaLike, string fisNo)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM EkstreAktarma_Table
                WHERE BankaAdi LIKE '%{0}%' AND FisNo={1}                
                ", bankaLike, fisNo.ReturnQuotedValue());
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<EkstreAktarma> list = ToList<EkstreAktarma>(dataTable);
            //EkstreAktarma ea = new EkstreAktarma();
            //ea = list.FirstOrDefault();
            return list;
        }
        public EkstreAktarma SelectByTelNoBankaBagisTarihi(string banka, string fisNo)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM EkstreAktarma_Table
                WHERE BankaAdi={0} AND FisNo={1}                
                ", banka.ReturnQuotedValue(), fisNo.ReturnQuotedValue());
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<EkstreAktarma> list = ToList<EkstreAktarma>(dataTable);
            EkstreAktarma ea = new EkstreAktarma();
            ea = list.FirstOrDefault();
            return ea;
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM EkstreAktarma_Table");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<EkstreAktarma> list = ToList<EkstreAktarma>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public override bool Update()
        {
            bool isSuccess = false;
            try
            {
                if (Id != 0)
                {
                    GenericEntity<EkstreAktarma> genericEntity = new GenericEntity<EkstreAktarma>(ProjeConstants.SQL_UPDATE);
                    DegistirmeTarihi = DateTime.Now;
                    string sqlString = genericEntity.GetQuery(this);
                    isSuccess = dao.Update2Db(sqlString);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return isSuccess;
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
                        var tutarTl = line.Substring(230, 15).TrimEnd();//TODO: sonunda bir sıfır fazla mı? yoksa son iki rakam küsürat mı? sorulacak

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
                        ekstreAktarma.Save();

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
                        var tarih = row[0].ReturnEmptyIfNull().ToString();//borc satırı için işlem yapma
                        var aciklama = row[4].ReturnEmptyIfNull().ToString();
                        if (string.IsNullOrEmpty(tarih))// ilk kolon boş ise dosya bitti çık
                            break;
                        try
                        {
                            var fisNo = row[5].ToString().Trim();
                            var tutar = row[2].ReturnEmptyIfNull().ToString().Replace(".", ",").ConvertToDecimal();//row[6].ReturnEmptyIfNull().ToString();
                            var bagisTarihi = row[0].ReturnEmptyIfNull().ToString().ConvertToDatetime();
                            if (BuKayitDahaOnceGirilmisMiByFisNo(//ProjeConstants.BANKA_AKBANKEKSTRE, fisNo, aciklama)) //fiş numarasından kontrol et kayıt girilmişse atla
                                "Akbank", fisNo, aciklama, bagisTarihi, tutar)) //fiş numarasından kontrol et kayıt girilmişse atla
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
                                    ekstreAktarma.Save();
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
                //if (count == 0)//ilk satırda tarih bilgisi var
                //{
                //    tarih = line.Substring(10, 8);
                //}
                if (count > 0 && count < (lines.Count - 1)) //ilk satır ve son satır alınmayacak
                {
                    try
                    {
                        EkstreAktarma ekstreAktarma = new EkstreAktarma();

                        var ad = line.Substring(1, 30).TrimEnd();

                        if (!string.IsNullOrEmpty(ad)) //en son satırı almamak için
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
                            ekstreAktarma.DovizCinsi = dovizCinsi;
                            ekstreAktarma.Ili = il;
                            ekstreAktarma.Ilcesi = ilce;
                            ekstreAktarma.IslemTarihi = processTime;
                            ekstreAktarma.DovizCinsi = ProjeConstants.DOVIZ_TL;
                            ekstreAktarma.Olusturan = currentUser;

                            ekstreAktarma.Save();

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
                                || detay.Contains("22100000000")//swap açılış kapanış
                                || detay.Contains("EF3077117 TÜRK SİLAHLI")) //vakıf hesabına virman
                            {
                                continue;
                            }

                            string adi = string.Empty;
                            var adres = string.Empty;// adres bilgisi gelmiyor

                            if (detay.Contains("CEP ŞUBE-HVL-Bağış -"))
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

                            ekstreAktarma.Save();
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
                        //holder[4] Genel Bağış yazıyor. bu bilgiye ihtiyaç yok
                        ekstreAktarma.Ili = holder[5];
                        ekstreAktarma.Ilcesi = holder[6];
                        ekstreAktarma.Adres = holder[7].ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo);
                        string aciklama = holder[8];
                        ekstreAktarma.Aciklama = aciklama;
                        //holder[9] dosyanın tarihini içeriyor
                        ekstreAktarma.Tutar = holder[10].ReturnEmptyIfNull().ToString().Replace(".", ",").ConvertToDecimal();
                        ekstreAktarma.BagisTarihi = ConvertTextToDateTime(holder[11]);
                        ekstreAktarma.AktarildiMi = false;
                        ekstreAktarma.BankaAdi = ProjeConstants.BANKA_HALKBANK;
                        ekstreAktarma.DovizCinsi = ProjeConstants.DOVIZ_TL; //döviz cinsi hep tl oluyor
                        ekstreAktarma.IslemTarihi = processTime;
                        ekstreAktarma.Olusturan = currentUser;
                        ekstreAktarma.BelgeIstemiyor = ekstreAktarma.Aciklama.Contains(ProjeConstants.DURUM_BELGE_ISTEMIYOR) || ekstreAktarma.Adres.Contains(ProjeConstants.DURUM_BELGE_ISTEMIYOR) ? true : false;
                        ekstreAktarma.Save();

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
                        var bagisTarihi = row[0].ReturnEmptyIfNull().ToString();//borc satırı için işlem yapma
                        if (string.IsNullOrEmpty(bagisTarihi))// ilk kolon boş ise dosya bitti çık
                            break;

                        string hata = string.Empty;
                        try
                        {
                            EkstreAktarma ekstreAktarma = new EkstreAktarma();
                            var detay = row[1].ReturnEmptyIfNull().ToString();
                            var tutar = row[2].ReturnZeroIfNull().ToString().Replace(".", ",");
                            if (detay.Contains("50060309TSK") || (tutar.ReturnZeroIfNull().ConvertToDecimal() < 0))
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
                                    var nameHolder = holder[1].ReturnEmptyIfNull().ToString().TrimEnd();
                                    var TCHolder = holder[2].ReturnEmptyIfNull().ToString().TrimEnd();
                                    ekstreAktarma.Adi = nameHolder.ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo);
                                    ekstreAktarma.TCKimlikNo = TCHolder.ReturnZeroIfNull().ToString().Trim().ConvertToLong();
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
                                ekstreAktarma.Save();
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

        /// <summary>
        /// int liste aalıp EkstreAktarma Listesi döndürür
        /// </summary>
        /// <param name="checkedRows"></param>
        /// <returns></returns>
        public List<EkstreAktarma> selectByIdList(List<int> checkedRows, ref int rowCount)
        {
            List<EkstreAktarma> eaList = new List<EkstreAktarma>();
            string idListStr = string.Empty;
            int counter = 0;
            foreach (int itemId in checkedRows)
            {
                if (counter++ == checkedRows.Count)
                    idListStr += itemId;
                else
                    idListStr += itemId + ",";
            }
            idListStr = !string.IsNullOrEmpty(idListStr) ? idListStr.Substring(0, idListStr.Length - 1) : string.Empty;//son virgülü at
            if (!string.IsNullOrEmpty(idListStr))
            {
                string sqlString = string.Format(@"
                SELECT *
                FROM EkstreAktarma_Table
                WHERE Id in ({0})
                ORDER BY AktarildiMi, BagisTarihi desc, Adi
                ", idListStr);
                DataTable dataTable = dao.selectFromDb(sqlString, "");
                List<EkstreAktarma> list = ToList<EkstreAktarma>(dataTable);
                rowCount = dataTable != null ? dataTable.Rows.Count : 0;
                return list;
            }
            else
            {
                return new List<EkstreAktarma>();
            }

        }
        public List<EkstreAktarma> selectByIdList(string idListStr)
        {
            if (!string.IsNullOrEmpty(idListStr))
            {
                string sqlString = string.Format(@"
                SELECT *
                FROM EkstreAktarma_Table
                WHERE Id in ({0})
                ORDER BY AktarildiMi,Id DESC, BagisTarihi desc, Adi
                ", idListStr);
                DataTable dataTable = dao.selectFromDb(sqlString, "");
                List<EkstreAktarma> list = ToList<EkstreAktarma>(dataTable);

                return list;
            }
            else
            {
                return new List<EkstreAktarma>();
            }

        }
        public List<EkstreAktarma> selectByEkstreIdList(string idListStr, ref int rowCount)
        {
            //List<EkstreAktarma> eaList = new List<EkstreAktarma>();
            //string idListStr = string.Empty;
            //int counter = 0;
            //foreach (string itemId in idList)
            //{
            //    if (counter++ == idList.Length)
            //        idListStr += itemId;
            //    else
            //        idListStr += itemId + ",";
            //}
            //idListStr = !string.IsNullOrEmpty(idListStr) ? idListStr.Substring(0, idListStr.Length - 1) : string.Empty;//son virgülü at
            if (!string.IsNullOrEmpty(idListStr))
            {
                string sqlString = string.Format(@"
                SELECT *
                FROM EkstreAktarma_Table
                WHERE Id in ({0})
                ORDER BY AktarildiMi, BagisTarihi desc, Adi
                ", idListStr);
                DataTable dataTable = dao.selectFromDb(sqlString, "");
                List<EkstreAktarma> list = ToList<EkstreAktarma>(dataTable);
                rowCount = dataTable != null ? dataTable.Rows.Count : 0;
                return list;
            }
            else
            {
                return new List<EkstreAktarma>();
            }

        }
        public List<EkstreAktarma> selectAktarilmayanlarByBankaAdiAndIslemTarihi(string bankaAdi, string islemTarihi)
        {
            List<EkstreAktarma> eaList = new List<EkstreAktarma>();

            if (!string.IsNullOrEmpty(bankaAdi))
            {
                string sqlString = string.Format(@"
                SELECT *
                FROM EkstreAktarma_Table
                WHERE IslemTarihi='{0}' and AktarildiMi=0 and BankaAdi='{1}'
                ORDER BY AktarildiMi, BagisTarihi desc, Adi
                ", islemTarihi, bankaAdi);

                DataTable dataTable = dao.selectFromDb(sqlString, "");
                List<EkstreAktarma> list = ToList<EkstreAktarma>(dataTable);

                return list;
            }
            else
            {
                return new List<EkstreAktarma>();
            }

        }

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
                        //TC bilgisi dosyada yer almıyor



                        ekstreAktarma.Save();
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
                            string islemTipi = row[6].ReturnEmptyIfNull().ToString();
                            string tutar = row[2].ReturnZeroIfNull().ToString().Replace(".", ",");
                            if ((islemTipi.Equals("Havale") || islemTipi.Equals("EFT")
                                || islemTipi.Equals("FAST"))
                                && tutar.ConvertToDecimal() > 0)
                            {
                                string tarih = row[0].ReturnEmptyIfNull().ToString();
                                DateTime bagisTarihi = tarih.Substring(0, 10).ConvertToDatetime();

                                var detay = row[7].ReturnEmptyIfNull().ToString();
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

                                ekstreAktarma.Save();
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
                            var borc_alacak = row[15].ReturnEmptyIfNull().ToString();//borc satırı için işlem yapma
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

                            ekstreAktarma.Save();
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
                İşlem No            :  Sadece o işleme/bağışa mahsus bir işlem numarası.
                Bağış Tarihi        :  Bağışın hesaba yatırıldığı tarih
                Adı                 :  Bağış yapan kişinin adı
                Soyadı              :  Bağış yapan kişinin soyadı
                Adres               :  Bağış yapan kişinin adresi
                İlçe                :  Bağış yapan kişinin ilçesi
                İli                 :  Bağış yapan kişinin ili
                TC Kimlik No        :  Bağış yapan kişinin TC Kimlik numarası
                Telefon             :  Bağış yapan kişinin telefon numarası
                Bağış Tutarı        :  Bağış tutarı
                Açıklama-Not        :  Bağış yapan kişinin notu, açıklaması

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
                        //İşlem Nu
                        ekstreAktarma.FisNo = holder[1].ToString();
                        ekstreAktarma.BagisTarihi = holder[2].ReturnEmptyIfNull().ToString().Replace("/", ".").ConvertToDatetime();
                        //Ad soyad birlikte ada yazılacak
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
                        ekstreAktarma.Save();

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
                        var hesapNo = row[0].ReturnEmptyIfNull().ToString();//borc satırı için işlem yapma
                        var islem = row[5].ReturnEmptyIfNull().ToString();
                        var tutar = row[6].ReturnEmptyIfNull().ToString().Replace(".", ",").ConvertToDecimal();//row[6].ReturnEmptyIfNull().ToString();
                        if (string.IsNullOrEmpty(hesapNo))// ilk kolon boş ise dosya bitti çık
                        {
                            break;
                        }
                        else if (islem.Contains("Batch Yatan")//Kart ile bağıştan gelen toplam miktar, zaten ayrıca girişi yapılıyor, dikkate alma
                           || islem.Contains("Batch Komisyonu")
                           || hesapNo.Equals("HESAP NO")
                           || tutar < 0)
                            continue;
                        try
                        {
                            var borc_alacak = row[15].ReturnEmptyIfNull().ToString();//borc satırı için işlem yapma
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

                            ekstreAktarma.Save();
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

                            string ayrac1 = "Genel Bağış";

                            if (detay.Contains(ayrac1))
                            {
                                char splitText = ',';
                                var holder = detay.Split(splitText);
                                var nameHolder = holder[1].ReturnEmptyIfNull().ToString().Trim();
                                var telefonHolder = holder[2].ReturnEmptyIfNull().ToString().Trim();
                                ekstreAktarma.Adi = nameHolder.ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo);
                                ekstreAktarma.Telefon1 = UtilityHelper.TelefonFormatla(telefonHolder.ReturnEmptyIfNull().ToString());
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

                            ekstreAktarma.Save();
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
        //public static ExceptionHelper SaveVakifBankKatilimFile(Stream fileStream, DateTime processTime, string currentUser)
        //{
        //    CultureInfo culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
        //    ExceptionHelper exceptionHelper = new ExceptionHelper();
        //    try
        //    {
        //        var vakifBankData = ExcelHelper.ReadXLSXAsDataTable(fileStream, true);
        //        if (vakifBankData != null)
        //        {

        //            foreach (DataRow row in vakifBankData.Rows)
        //            {
        //                EkstreAktarma ekstreAktarma = new EkstreAktarma();
        //                try
        //                {
        //                    var bagisTarihi = row[0].ReturnEmptyIfNull().ToString();
        //                    var tutar = row[3].ReturnEmptyIfNull().ToString();
        //                    var detay = row[4].ReturnEmptyIfNull().ToString();
        //                    var adres = string.Empty;// adres bilgisi gelmiyor

        //                    ekstreAktarma.Tutar = tutar.ConvertToDecimal();
        //                    ekstreAktarma.BagisTarihi = bagisTarihi.ConvertToDatetime();
        //                    ekstreAktarma.Aciklama = detay;
        //                    ekstreAktarma.Adres = adres.ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo); ;
        //                    ekstreAktarma.BelgeIstemiyor = ekstreAktarma.Aciklama.Contains(ProjeConstants.DURUM_BELGE_ISTEMIYOR) || ekstreAktarma.Adres.Contains(ProjeConstants.DURUM_BELGE_ISTEMIYOR) ? true : false;
        //                    ekstreAktarma.BankaAdi = ProjeConstants.BANKA_VAKIF_KATILIM;
        //                    ekstreAktarma.IslemTarihi = processTime;
        //                    ekstreAktarma.DovizCinsi = ProjeConstants.DOVIZ_TL;
        //                    ekstreAktarma.Olusturan = currentUser;

        //                    ekstreAktarma.Save();
        //                }
        //                catch (Exception ex)
        //                {
        //                    Exception exceprion = new Exception(string.Format("HATA SATIRI {0}:{1} ->", ProjeConstants.BANKA_VAKIF_KATILIM, ""), ex);
        //                    exceptionHelper.Exceptions.Add(exceprion);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        exceptionHelper.Exceptions.Add(ex);
        //    }
        //    return exceptionHelper;
        //}

        public static ExceptionHelper SaveTEBBankFile(Stream filePath, DateTime processTime, string currentUser)
        {
            ExceptionHelper exceptionHelper = new ExceptionHelper();
            CultureInfo culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            List<string> lines = HelperFunctions.ConvertTextFileToList(filePath);
            int count = 0;
            var tarih = string.Empty;
            foreach (var line in lines)
            {
                //if (count == 0)//ilk satırda tarih bilgisi var
                //{
                //    tarih = line.Substring(10, 8);
                //}
                if (count > 0 && count < (lines.Count - 1)) //ilk satır ve son satır alınmayacak
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
                            tarih = line.Substring(320, 8).Trim();//20190617 YYYYMMDD formatı
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

                            ekstreAktarma.Save();

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
                    if (count > 0) //ilk satır alınmıyor
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
                        //ekstreAktarma.Ilcesi = ilce; ziraat bankasından gelen ilce kodları tutmuyor. bu yuzden ilceyi yazmasın
                        ekstreAktarma.Eposta = eposta;
                        ekstreAktarma.IslemTarihi = islemTarihi;
                        ekstreAktarma.DovizCinsi = ProjeConstants.DOVIZ_TL;
                        ekstreAktarma.Olusturan = currentUser;
                        ekstreAktarma.FisNo = fisNo;
                        if (BuKayitDahaOnceGirilmisMiByFisNo(//ProjeConstants.BANKA_ZIRAAT,fisNo,aciklama)) 
                            "Ziraat", fisNo, aciklama, tarih.ReturnEmptyIfNull().ConvertToDatetime(), tutar)) //fiş numarasından kontrol et kayıt girilmemişse exception dondur değilse kaydet
                        {
                            Exception ex = new Exception(fisNo + " Numaralı fiş daha önce girildiğinden tekrar aktarılmadı.");
                            exceptionHelper.Exceptions.Add(ex);
                        }
                        else
                        {
                            ekstreAktarma.Save();

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
                        if (kaydiAtla)// onaylı olmayanları atla
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
                                    //alt satır kapatıldı. Ay sonu ve başında hesabı tutturamıyo (Deniz ÖZKAN) bagis tarihini kendi seçsin S.B. 30.12.2020 
                                    //ekstreAktarma.BagisTarihi = bagisTarihi.AddDays(1);//burada bağış tarihine bir gün eklenmesinin sebebi, bankanın gün içindeki bağışları vakıf hesabına ertesi gün kaydetmesi nedeniyle oluşan tutarsızlığı gidermektir. 15.06.2020 SB
                                    ekstreAktarma.BagisTarihi = bagisTarihi;
                                    ekstreAktarma.Aciklama = aciklama;
                                    ekstreAktarma.BelgeIstemiyor = belgeIstiyorMu.Equals("-") ? true : false;
                                    ekstreAktarma.FisNo = transactionId;
                                    ekstreAktarma.BankaAdi = ProjeConstants.BANKA_KARTILEBAGIS;
                                    ekstreAktarma.IslemTarihi = islemTarihi;
                                    ekstreAktarma.DovizCinsi = ProjeConstants.DOVIZ_TL;
                                    ekstreAktarma.Olusturan = currentUser;
                                    if (BuKayitDahaOnceGirilmisMiByFisNo(//transactionId numarasından kontrol et kayıt girilmemişse exception dondur değilse kaydet
                                        "Kart ile Bağış", transactionId, aciklama, bagisTarihi, tutar)) //transactionId numarasından kontrol et kayıt girilmemişse exception dondur değilse kaydet
                                    {
                                        Exception ex = new Exception(transactionId + " Numaralı kayıt daha önce girildiğinden tekrar aktarılmadı.");
                                        exceptionHelper.Exceptions.Add(ex);
                                    }
                                    else
                                    {
                                        ekstreAktarma.Save();
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Exception exception = new Exception(string.Format("HATA SATIRI {0}:{1} ->", ProjeConstants.BANKA_KARTILEBAGIS, "Dosya Sıra No=" + kayitNo), ex);
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
                //if (count == 0)//ilk satırda tarih bilgisi var
                //{
                //    tarih = line.Substring(10, 8);
                //}
                if (count >= 0 && count < (lines.Count - 1)) //ilk satır ve son satır alınmayacak
                {
                    try
                    {
                        //ilk 4 karakteri al
                        var tag = line.Substring(0, 4).TrimEnd();
                        switch (tag)
                        {
                            case ":20:": // :20: referans numarası
                                var referans = line.Substring(5, line.Length - 5); //- 5 mi olmalı?
                                break;
                            case ":28:":

                                break;

                            case ":60F":// açılış bilgileri

                                break;
                            case ":61:":// Bu günkü işlem
                                var tarih = line.Substring(5, 6);
                                var girisTarihi = line.Substring(11, 4); //aagg
                                var borcAlacak = line.Substring(15, 1); // b/a
                                var dovizCinsi = line.Substring(16, 1); //Y : YTL galiba
                                //mt940 stadardında tutar 15 char ama ziraat buna uymuyor o nedenle satırın kalanını ayrı pars edeceğiz
                                var geriKalan = line.Substring(17, line.Length - 17);
                                var holder = geriKalan.Split(',');
                                var tutarTamsayı = holder.Length > 0 ? holder[0].ConvertToInt() : 0;
                                var tutarKurus = holder[1].ToString().Substring(0, 2).ConvertToInt();
                                var tutar = tutarTamsayı + "," + tutarKurus;

                                var fisNo = holder[1].ToString().Split('/')[1].ToString();
                                break;
                            case ":86:": //bugunku islemin açıklaması

                                break;
                            case ":62F": //kapanış bilgileri

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
        private static bool BuKayitDahaOnceGirilmisMiByFisNo(string bankaLike, string fisNo, string aciklama, DateTime bagisTarihi, decimal tutar)
        {
            bool kaydedilmisMi = false;
            EkstreAktarma ekstreAktarmaDao = new EkstreAktarma();
            List<EkstreAktarma> ekstreAktarmaList = ekstreAktarmaDao.SelectByFisNoBanka(bankaLike, fisNo);
            foreach (var ekstreAktarma in ekstreAktarmaList)
            {
                if (ekstreAktarma != null)
                {
                    string randomSayi = new Random().NextDouble().ToString();
                    string tcKimlikNo = ekstreAktarma.TCKimlikNo.ToString().Equals("0") ? randomSayi : ekstreAktarma.TCKimlikNo.ToString();
                    string adi = string.IsNullOrEmpty(ekstreAktarma.Adi) ? randomSayi : ekstreAktarma.Adi;
                    string telefon1 = string.IsNullOrEmpty(ekstreAktarma.Telefon1) ? randomSayi : UtilityHelper.TelefonFormatla(ekstreAktarma.Telefon1.ReturnEmptyIfNull().ToString()); 
                    string telefon2 = string.IsNullOrEmpty(ekstreAktarma.Telefon2) ? randomSayi : UtilityHelper.TelefonFormatla(ekstreAktarma.Telefon2.ReturnEmptyIfNull().ToString());

                    //eğer bu kayıtın fisnosu daha önce girilmişse bir de islemtarihi, tutar, isim,tc ve telefondan kontrol et 
                    if ((bagisTarihi == ekstreAktarma.BagisTarihi) && (tutar == ekstreAktarma.Tutar))
                    {
                        if (aciklama.Equals(ekstreAktarma.Aciklama)
                            || aciklama.Contains(adi)
                            || (aciklama.Contains(tcKimlikNo))
                            || (aciklama.Contains(telefon1))
                            || (aciklama.Contains(telefon2)))
                        {
                            kaydedilmisMi = true;
                            break;
                        }
                        else
                            kaydedilmisMi = false;
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

                var ziraatEkstreData = ExcelHelper.ReadXLSXAsDataTable(fileStream, ProjeConstants.BANKA_ZIRAAT_ILKKACSATIRHARIC, ProjeConstants.BANKA_ZIRAAT_SONKACSATIRHARIC, true);
                if (ziraatEkstreData != null)
                {


                    foreach (DataRow row in ziraatEkstreData.Rows)
                    {
                        kayitNo++;
                        EkstreAktarma ekstreAktarma = new EkstreAktarma();
                        var tarih = row[0].ReturnEmptyIfNull().ToString();//borc satırı için işlem yapma

                        if (string.IsNullOrEmpty(tarih))// ilk kolon boş ise dosya bitti çık
                            break;
                        try
                        {
                            var aciklama = row[2].ReturnEmptyIfNull().ToString();//herşey açıklamanın içinde
                            if (aciklama.Contains("Tsk Güçlendirme Vakfı Bağış/"))
                            {
                                continue;
                            }
                            else
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
                                    if (BuKayitDahaOnceGirilmisMiByFisNo(//ProjeConstants.BANKA_ZIRAATEKSTRE, fisNo,aciklama)) //fiş numarasından kontrol et kayıt girilmemişse exception dondur değilse kaydet
                                        "Ziraat", fisNo, aciklama, bagisTarihi, tutar)) //fiş numarasından kontrol et kayıt girilmemişse exception dondur değilse kaydet
                                    {
                                        Exception ex = new Exception(fisNo + " Numaralı fiş daha önce girildiğinden tekrar aktarılmadı.");
                                        exceptionHelper.Exceptions.Add(ex);
                                    }
                                    else
                                    {
                                        if (aciklama.Contains("Tsk Güçlendirme Vakfı Bağış/"))
                                        {
                                            var holder = aciklama.Split('/');
                                            int holderLength = holder.Length - 1;
                                            var adi = holderLength < 1 ? "" : holder[1].ReturnEmptyIfNull().ToString().TrimEnd().Split(':')[1].ReturnEmptyIfNull().ToString();
                                            var holder3 = holderLength < 3 ? "" : holder[3].ReturnEmptyIfNull().ToString().TrimEnd();
                                            var tc = string.IsNullOrEmpty(holder3) ? "" : holder3.Split(':')[1].ReturnEmptyIfNull().ToString();
                                            var holder4 = holderLength < 4 ? "" : holder[4].ReturnEmptyIfNull().ToString().TrimEnd();
                                            var telstr = string.IsNullOrEmpty(holder4) ? "" : holder4.Split(':')[1].ReturnEmptyIfNull().ToString();
                                            int length = telstr.Length;
                                            var telefon1 = string.IsNullOrEmpty(telstr) ? "" : telstr.Substring(1, length - 1);

                                            ekstreAktarma.Adi = adi;
                                            ekstreAktarma.TCKimlikNo = tc.ConvertToLong();
                                            ekstreAktarma.Telefon1 = UtilityHelper.TelefonFormatla(telefon1.ReturnEmptyIfNull().ToString()); ;

                                        }
                                        ekstreAktarma.Save();

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
                    var aciklama = row[2].ReturnEmptyIfNull().ToString();
                    hata = aciklama;
                    if (!string.IsNullOrEmpty(aciklama) && !aciklama.Contains("3 57968 1"))//virman değilse
                    {
                        var tarih = row[0].ReturnEmptyIfNull().ToString().ConvertToDatetime();


                        var tutar = row[3].ReturnEmptyIfNull().ToString().Replace(".", ",");
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

                            var id = ekstreAktarma.Save();
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
        public bool CheckIsExistByBankaAdiAndIslemTarihi(string bankaAdi, DateTime islemTarihi)
        {
            bool isExist = false;
            //string sqlString = string.Format(@"SELECT *
            //                                    FROM EkstreAktarma_Table
            //                                    WHERE BankaAdi='{0}' AND DATEDIFF(day,IslemTarihi,{1})=0", bankaAdi, islemTarihi.ReturnTRDateFormat());
            string sqlString = string.Format(@"SELECT *
                                                FROM EkstreAktarma_Table
                                                WHERE BankaAdi='{0}' AND IslemTarihi={1} AND ElleKayit != 1", bankaAdi, islemTarihi.ReturnTRDateFormat());

            DataTable dataTable = dao.selectFromDb(sqlString, "");

            if (dataTable != null)
            {
                if (dataTable.Rows.Count > 0)
                {
                    isExist = true;
                }

            }
            return isExist;
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
        public static ExceptionHelper SaveAll(List<EkstreAktarma> listEkstreAktarma, string currentUser)
        {
            ExceptionHelper exceptionHelper = new ExceptionHelper();


            foreach (EkstreAktarma ekstreAktarma in listEkstreAktarma)
            {

                try
                {
                    SaveEkstreAktarma(ekstreAktarma, currentUser);
                }
                catch (Exception e)
                {
                    exceptionHelper.Exceptions.Add(e);
                }
                //break;

            }

            return exceptionHelper;
        }
        private static void WriteText(string text)
        {
            using (StreamWriter sw = new StreamWriter("C:\\test\\debug.txt", true))
            {
                sw.WriteLine(text);
            }
        }
        private static void SaveEkstreAktarma(EkstreAktarma ekstreAktarma, string currentUser)
        {
            //WriteText("SaveEkstreAktarma-0");

            if (!ekstreAktarma.AktarildiMi) //zaten aktarılmış olanlar bir kez daha aktarılmasın
            {
                var nakitBagisciId = 0;

                if (ekstreAktarma.NakitBagisciId > 0)//NakitBagisciId si dolu olarak gelen ekstreAktarma (Yani elle yeni kayıt girisi yapilmis ve NakitBagisci popup window dan secilmis )
                {
                    nakitBagisciId = ekstreAktarma.NakitBagisciId;
                }
                else
                {
                    nakitBagisciId = SaveNakitBagisciFromEkstre(ekstreAktarma, currentUser); // NakitBagisci_Table tablosuna aktarım
                    //WriteText("SaveEkstreAktarma-1");
                }

                var nakitBagisHareketId = SaveBagisFromEkstreAktarma(ekstreAktarma, nakitBagisciId, currentUser); //NakitBagisHareket_Table tablosuna aktarım
                //WriteText("SaveEkstreAktarma-2");

                bool isArmaganSaved = SaveArmagan(ekstreAktarma.BagisTarihi, ekstreAktarma.TuzelKisi, nakitBagisciId, nakitBagisHareketId, currentUser);//Armagan tablosuna aktarım
                //WriteText("SaveEkstreAktarma-3");

                if (nakitBagisHareketId > 0)
                {
                    NakitBagisci nakitBagisci = new NakitBagisci();
                    nakitBagisci = nakitBagisci.Select<NakitBagisci>(nakitBagisciId);
                    if (nakitBagisci.Adi.Contains(ProjeConstants.NAKITBAGISCI_BILINMEYEN))// adı bilinmeyen bagisci için eksrteAktarma tablosuna aciklama yaz 
                    {
                        ekstreAktarma.Aciklama += "-NBYS- Adı BİLİNMEYEN bağışçı olduğundan armağan oluşturulmadı ";
                    }

                    ekstreAktarma.NakitBagisHareketId = nakitBagisHareketId;
                    ekstreAktarma.AktarildiMi = true;
                    ekstreAktarma.Update();
                }

            }
        }

        public static bool SaveArmagan(DateTime bagisTarihi, bool tuzelKisiMi, int nakitBagisciId, int nakitbagisHareketId, string currentUser)
        {
            //WriteText("SaveArmagan-0");
            bool isArmaganSaved = false;
            NakitBagisci nakitBagisci = new NakitBagisci();
            nakitBagisci = nakitBagisci.Select<NakitBagisci>(nakitBagisciId);
            if ((nakitBagisci == null) || (nakitBagisci.Adi.Contains(ProjeConstants.NAKITBAGISCI_BILINMEYEN)))//nakit bagışçı nuul veya bilinmeyen ise armagan üretmesin
            {
                isArmaganSaved = false;
            }
            else
            {
                NakitBagisHareket nakitBagisHareket = new NakitBagisHareket();
                /// SB Ay bazında değil de ayı iki parçaya bölerek armağan gönderme uygulamasına geçildi, 17 Nisan 2019. 
                /// Bu nedenle aşağıdaki satır kapatıldı
                /// decimal toplamBagis = nakitBagisHareket.GetSumBagisMiktariByNakitBagisciIdAndBagisTarihi(bagisTarihi, nakitBagisciId);
                /// yerine baslama bitiş tarihi alan metot yazıldı

                //ARMAGAN PERIODU
                DateTime bastar = new DateTime();
                DateTime bittar = new DateTime();
                int bagisGunu = bagisTarihi.Day;

                //10 Günde bir
                //if (bagisGunu < 11)
                //{
                //    basTar = new DateTime(bagisTarihi.Year, bagisTarihi.Month, 1);
                //    bitTar = new DateTime(bagisTarihi.Year, bagisTarihi.Month, 10);
                //}else if (bagisGunu > 10 && bagisGunu < 21)
                //{
                //    basTar = new DateTime(bagisTarihi.Year, bagisTarihi.Month, 11);
                //    bitTar = new DateTime(bagisTarihi.Year, bagisTarihi.Month, 20);
                //}
                //else
                //{
                //    basTar = new DateTime(bagisTarihi.Year, bagisTarihi.Month, 21);
                //    DateTime ilkGun = new DateTime(basTar.Year, basTar.Month, 1);
                //    bitTar = ilkGun.AddMonths(1).AddDays(-1);
                //}

                //15 Günde bir
                if (bagisGunu < 16)
                {
                    bastar = new DateTime(bagisTarihi.Year, bagisTarihi.Month, 1);
                    bittar = new DateTime(bagisTarihi.Year, bagisTarihi.Month, 15);
                }
                else
                {
                    bastar = new DateTime(bagisTarihi.Year, bagisTarihi.Month, 16);
                    DateTime ilkGun = new DateTime(bastar.Year, bastar.Month, 1);
                    bittar = ilkGun.AddMonths(1).AddDays(-1);
                }
                decimal toplamBagis = nakitBagisHareket.GetSumBagisMiktariByNakitBagisciIdBetweenBasTarBitTar(bastar, bittar, nakitBagisciId);
                
                ArmaganTanim hakedilenArmaganTanim = new ArmaganTanim();
                hakedilenArmaganTanim = hakedilenArmaganTanim.SelectByTutar(toplamBagis, tuzelKisiMi); //toplam tutar gidecek

                if (hakedilenArmaganTanim != null && hakedilenArmaganTanim.Id != ProjeConstants.ARMAGAN_YOKID)
                {
                    //Armagan armagan = new Armagan();
                    ////bagis tarihi ve BagisciId ye göre armagan tablosunu sorgula, bu kişi varsa update yoksa insert etmek için
                    //armagan = armagan.SelectByBagisciIdAndBagisTarihi(bastar, bittar, nakitBagisciId);
                    //if (armagan == null)
                    //    armagan = new Armagan();

                    //armagan.BagisciId = nakitBagisciId;
                    ////armagan.BagisId = nakitBagisHareketId;
                    //armagan.BagisMiktari = toplamBagis;
                    //armagan.DovizCinsi = ProjeConstants.DOVIZ_TL;
                    //armagan.Tarih = bagisTarihi;
                    //armagan.ArmaganTanimId = hakedilenArmaganTanim.Id;
                    //armagan.Olusturan = currentUser;
                    //armagan.Durum = nakitBagisci.BelgeIstemiyor ? ProjeConstants.DURUM_BELGE_ISTEMIYOR : ProjeConstants.DURUM_GONDERILMEDI;

                    //Armagan iadeEdilmisArmagan = new Armagan();
                    //List<Armagan> iadeEdilmisArmaganListesi = iadeEdilmisArmagan.SelectByBagisciIdAndDurum(nakitBagisci.Id, ProjeConstants.DURUM_IADE);
                    //if (iadeEdilmisArmaganListesi.Count > 0)
                    //{
                    //    armagan.Durum = ProjeConstants.DURUM_DAHAONCEIADE;
                    //}

                    //if (hakedilenArmaganTanim.Id != ProjeConstants.ARMAGAN_TESEKKURID) //yeni hakkettiği armağan teşekkür ise eski armağanı sorgulama (Olumsuz)
                    //{
                    //    //yeni hakkettiği armağan teşekkür değilise, eski armağanı sorgula, yeni armağanı daha önce almış mı, evet ise teşekkür ver
                    //    DateTime armaganSorgulamaBasTar = new DateTime(2005, 1, 1);
                    //    DateTime armaganSorgulamaBitTar = bastar;
                    //    Armagan eskiArmagan = armagan.SelectByBagisciIdBagisTarihi(armagan.BagisciId, hakedilenArmaganTanim.Id, armaganSorgulamaBasTar, armaganSorgulamaBitTar);
                    //    if (eskiArmagan != null)
                    //        armagan.ArmaganTanimId = ProjeConstants.ARMAGAN_TESEKKURID;

                    //}
                    //isArmaganSaved = armagan.SaveOrUpdate(bastar, bittar, bagisTarihi, nakitBagisciId, nakikbagisHareketId);
                    NakitBagisHareket newNbh = new NakitBagisHareket();
                    List<NakitBagisHareket> newNbhList = newNbh.SelectByBagisciIdTarih(nakitBagisciId, bastar, bittar);
                    int armaganId = ArmaganiKaydetVeyaGuncelle(bastar,bittar,nakitBagisciId,bagisTarihi,toplamBagis,hakedilenArmaganTanim.Id,currentUser,nakitBagisci, newNbhList);
                    isArmaganSaved = armaganId > 0;
                }
                else
                {
                    isArmaganSaved = true;
                }
            }


            return isArmaganSaved;
        }

        public static int ArmaganiKaydetVeyaGuncelle(DateTime bastar, DateTime bittar,int nakitBagisciId ,DateTime bagisTarihi, decimal toplamBagis, 
            int hakedilenArmaganTanimId, string currentUser, NakitBagisci nakitBagisci,List<NakitBagisHareket> nakitBagisHareketListesi)
        {
            int armaganId = 0;
            Armagan armagan = new Armagan();
            //bagis tarihi ve BagisciId ye göre armagan tablosunu sorgula, bu kişi varsa update yoksa insert etmek için
            armagan = armagan.SelectByBagisciIdAndBagisTarihi(bastar, bittar, nakitBagisciId);
            if (armagan == null)
                armagan = new Armagan();

            armagan.BagisciId = nakitBagisciId;
            //armagan.BagisId = nakitBagisHareketId;
            armagan.BagisMiktari = toplamBagis;
            armagan.DovizCinsi = ProjeConstants.DOVIZ_TL;
            armagan.Tarih = bagisTarihi;
            armagan.ArmaganTanimId = hakedilenArmaganTanimId;
            armagan.Olusturan = currentUser;
            armagan.Durum = nakitBagisci.BelgeIstemiyor ? ProjeConstants.DURUM_BELGE_ISTEMIYOR : ProjeConstants.DURUM_GONDERILMEDI;

            Armagan iadeEdilmisArmagan = new Armagan();
            List<Armagan> iadeEdilmisArmaganListesi = iadeEdilmisArmagan.SelectByBagisciIdAndDurum(nakitBagisci.Id, ProjeConstants.DURUM_IADE);
            if (iadeEdilmisArmaganListesi.Count > 0)
            {
                armagan.Durum = ProjeConstants.DURUM_DAHAONCEIADE;
            }

            if (hakedilenArmaganTanimId != ProjeConstants.ARMAGAN_TESEKKURID) //yeni hakkettiği armağan teşekkür ise eski armağanı sorgulama (Olumsuz)
            {
                //yeni hakkettiği armağan teşekkür değilise, eski armağanı sorgula, yeni armağanı daha önce almış mı, evet ise teşekkür ver
                DateTime armaganSorgulamaBasTar = new DateTime(2005, 1, 1);
                DateTime armaganSorgulamaBitTar = bastar;
                Armagan eskiArmagan = armagan.SelectByBagisciIdBagisTarihi(armagan.BagisciId, hakedilenArmaganTanimId, armaganSorgulamaBasTar, armaganSorgulamaBitTar);
                if (eskiArmagan != null)
                    armagan.ArmaganTanimId = ProjeConstants.ARMAGAN_TESEKKURID;

            }
            armaganId = armagan.SaveOrUpdate(bastar, bittar, bagisTarihi, nakitBagisciId, nakitBagisHareketListesi);
            return armaganId;
        }
        private static int SaveNakitBagisciFromEkstre(EkstreAktarma ekstreAktarma, string currentUser)
        {
            var nakitBagisciId = 0;
            NakitBagisci nakitBagisci = new NakitBagisci();
            if (ekstreAktarma.Adi != ProjeConstants.NAKITBAGISCI_BILINMEYEN)
            {
                nakitBagisci = nakitBagisci.SelectByTcKimlikno(ekstreAktarma.TCKimlikNo); //tc kimlik no bagisci tablosunda var mı kontrol ediliyor.

                if (nakitBagisci != null)//bagisci tablosunda var
                {
                    nakitBagisciId = nakitBagisci.Id;
                    nakitBagisciId = SaveBagisciFromEkstreAktarma(nakitBagisci, ekstreAktarma, false, currentUser);
                }
                else //bagisci tablosunda tc kimlikno ile bulunamadı
                {
                    nakitBagisci = new NakitBagisci();

                    nakitBagisciId = GetUserIdByTelefonAndAd(ekstreAktarma);// telefon ve isime bakıyor

                    if (nakitBagisciId == 0) //bu kişi kesin olarak yeni bağişci (Telefon ve isimden bulunamadı ise)
                    {
                        nakitBagisciId = SaveBagisciFromEkstreAktarma(nakitBagisci, ekstreAktarma, true, currentUser);
                    }
                    else //nakitBagisciId boş veya 0 değil, öyleyse bu bağışçıyı select edelim 22.ocak.2020
                    {
                        nakitBagisci = nakitBagisci.Select<NakitBagisci>(nakitBagisciId);
                        nakitBagisciId = SaveBagisciFromEkstreAktarma(nakitBagisci, ekstreAktarma, false, currentUser);
                    }
                }
            }
            else
            {
                nakitBagisciId = BilinmeyenBagisciOlusturAtomicId();// GetBilinmeyenBagisciId();
            }
            return nakitBagisciId;
        }
        private static int BilinmeyenBagisciOlusturAtomicId()
        {
            DbClass db = new DbClass();
            int yeniId = 0;
            NakitBagisci nbToSave = new NakitBagisci();
            nbToSave = new NakitBagisci();
            nbToSave.Olusturan = UtilityHelper.GetCurrentUserLoginName();

            //herbir nesne için bir dbo yarat
            //önce nakitbağışçıyı kaydet
            DBObject nb1Dbo = new DBObject();
            nb1Dbo.SQLString = nbToSave.GetInsertSQL("");
            nb1Dbo.SQLType = ProjeConstants.SQL_INSERT;
            nb1Dbo.IsFilled = true;
            db.DBObjectList.Add(nb1Dbo);

            //şimdi update et
            DBObject nb2Dbo = new DBObject();

            //guncellenecek alanları nesnelerde guncelle
            //sonra Id ile bilinmeyenden Ad üret ve güncelle
            NakitBagisci nbToUpdate = new NakitBagisci();
            nbToUpdate.Degistiren = UtilityHelper.GetCurrentUserLoginName();
            nbToUpdate.Adi = ProjeConstants.NAKITBAGISCI_BILINMEYEN + "_{0}";
            nbToUpdate.Aciklama = nbToUpdate.Aciklama + ProjeConstants.NAKITBAGISCI_BILINMEYEN + " Bağışçı Id= " + "{0}";

            //dbo ayarla
            nb2Dbo.SQLString = nbToUpdate.GetUpdateSQL("{0}");
            nb2Dbo.SQLType = ProjeConstants.SQL_UPDATE;
            nb2Dbo.UseReturnIdAsParam = true;
            nb2Dbo.DbObjectParamIndex = 0;
            nb2Dbo.IsFilled = true;
            db.DBObjectList.Add(nb2Dbo);

            //hazırlanan sorguları çalıştır
            List<DBObject> savedDBOList = db.ExecuteTransaction();
            if (savedDBOList.Count > 0)
            {
                yeniId = nb1Dbo.ReturnId;
            }
            return yeniId;

        }
        //private static int GetBilinmeyenBagisciId()
        //{
        //    int id = 0;
        //    NakitBagisci nb = new NakitBagisci();
        //    nb = nb.SelectByAd(ProjeConstants.NAKITBAGISCI_BILINMEYEN).FirstOrDefault();
        //    if(nb!=null)
        //    {
        //        id = nb.Id;
        //    }
        //    else
        //    {
        //        nb = new NakitBagisci();
        //        nb.Adi = ProjeConstants.NAKITBAGISCI_BILINMEYEN;
        //        id = nb.Save();
        //    }

        //    return id;

        //}
        private static int GetUserIdByTelefonAndAd(EkstreAktarma ekstreAktarma)
        {
            var nakitBagisciId = 0;
            var nakitBagisci = new NakitBagisci();
            var adi = ekstreAktarma.Adi;

            if (!string.IsNullOrEmpty(adi)) // adi boş ise yeni bağışçı olarak kabul edilecek
            {
                var telefon1 = UtilityHelper.TelefonFormatla(ekstreAktarma.Telefon1.ReturnEmptyIfNull().ToString()); 
                if (!string.IsNullOrEmpty(telefon1)) //telefon1 boş değil ise ise kontrol ediliyor adı ve telefon1 bulunur ise aynı kişi olarak kabul ediliyor, yeni kayıt atılmıyor
                {
                    nakitBagisciId = GetUserId(nakitBagisci, adi, telefon1);
                    if (nakitBagisciId != 0)
                    {
                        return nakitBagisciId;
                    }
                }
                if (nakitBagisciId < 1)
                {
                    var telefon2 = UtilityHelper.TelefonFormatla(ekstreAktarma.Telefon2.ReturnEmptyIfNull().ToString()); //telefon1 ve adı eşleşmedi ise, telefon2 ve adi eşleişiyor mu bakılıyor
                    if (!string.IsNullOrEmpty(telefon2))
                    {
                        nakitBagisciId = telefon2.Length > 3 ? GetUserId(nakitBagisci, adi, telefon2) : 0;
                    }
                }
            }
            return nakitBagisciId;
        }
        private static int GetUserId(NakitBagisci nakitBagisci, string adi, string telefon)
        {
            var nakitBagisciId = 0;
            if (telefon.Length > 3)
            {
                nakitBagisci = nakitBagisci.SelectByAdAndTelefon(adi, telefon);
                if (nakitBagisci == null)
                {
                    string dokuzlu = telefon.Substring(0, 2);
                    string sifirli = telefon.Substring(0, 1);

                    if (dokuzlu.Equals("90"))
                    {
                        nakitBagisci = new NakitBagisci();
                        string telefon1 = telefon.Substring(2);
                        nakitBagisci = nakitBagisci.SelectByAdAndTelefon(adi, telefon1);
                    }
                    else if (sifirli.Equals("0"))
                    {
                        nakitBagisci = new NakitBagisci();
                        string telefon1 = telefon.Substring(1);
                        nakitBagisci = nakitBagisci.SelectByAdAndTelefon(adi, telefon1);
                    }
                    else
                    {
                        nakitBagisci = new NakitBagisci();
                        string bosluksuzTelefon = telefon.Replace(" ", "");
                        nakitBagisci = nakitBagisci.SelectByAdAndTelefon(adi, bosluksuzTelefon);
                    }
                }
            };
            if (nakitBagisci != null)//bagisci tablosunda var
            {
                nakitBagisciId = nakitBagisci.Id;
            }
            return nakitBagisciId;
        }
        private static int GetIlId(string value)
        {
            //il ziraat bankası dosyasında id olarak geliyor. diğer bankalarda il adı olarak geliyor.
            int ilId = 0;

            var isInt = Int32.TryParse(value, out ilId);
            if (!isInt)
            {
                Il il = new Il();
                il = il.SelectByIlAdi(value);
                if (il != null)
                {
                    ilId = il.Id;
                }
            }

            return ilId;
        }
        private static int GetIlceId(EkstreAktarma ekstreAktarma)
        {
            //
            int ilceId = 0;

            var isInt = Int32.TryParse(ekstreAktarma.Ilcesi, out ilceId);
            if (isInt)
            {
                Ilce ilce = new Ilce();
                ilce = ilce.SelectByIlceId(ilceId);
                if (ilce != null)
                {
                    ilceId = ilce.Id;
                }
            }

            return ilceId;
        }
        private static int SaveBagisFromEkstreAktarma(EkstreAktarma ekstreAktarma, int nakitBagisciId, string currentUser)
        {
            CultureInfo culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            int nakitBagisHareketId = 0;
            BankaTanim bankaTanim = new BankaTanim();
            bankaTanim = bankaTanim.SelectByBankaName(ekstreAktarma.BankaAdi);

            Ilce ilce = new Ilce();
            ilce = ilce.SelectByIlNameAndIlceName(ekstreAktarma.Ili, ekstreAktarma.Ilcesi);

            NakitBagisHareket nakitBagisHareket = new NakitBagisHareket();
            nakitBagisHareket.BagisciId = nakitBagisciId;
            nakitBagisHareket.BagisMiktari = ekstreAktarma.Tutar;
            nakitBagisHareket.BagisTarihi = ekstreAktarma.BagisTarihi;
            if (bankaTanim != null)
            {
                nakitBagisHareket.BankaId = bankaTanim.Id;
            }
            nakitBagisHareket.Adresi = ekstreAktarma.Adres.ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo);
            nakitBagisHareket.DovizCinsi = ekstreAktarma.DovizCinsi;
            nakitBagisHareket.Telefon = UtilityHelper.TelefonFormatla(ekstreAktarma.Telefon1.ReturnEmptyIfNull().ToString());
            nakitBagisHareket.Ili = GetIlId(ekstreAktarma.Ili);
            nakitBagisHareket.Ilcesi = ilce != null ? ilce.Id : 0;
            nakitBagisHareket.Olusturan = currentUser;
            nakitBagisHareket.Aciklama = ekstreAktarma.Aciklama;

            var id = nakitBagisHareket.Save();
            if (id != 0)
            {

                nakitBagisHareketId = nakitBagisHareket.Id;
            }
            else
            {
                //TODO: hata mesajı verilecek
            }

            return nakitBagisHareketId;
        }
        private static int SaveBagisciFromEkstreAktarma(NakitBagisci nakitBagisci, EkstreAktarma ekstreAktarma, bool isNew, string currentUser)
        {
            CultureInfo culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            if (nakitBagisci == null)
            {
                nakitBagisci = new NakitBagisci();
            }
            if (isNew)
            {
                Ilce ilce = new Ilce();
                ilce = ilce.SelectByIlNameAndIlceName(ekstreAktarma.Ili, ekstreAktarma.Ilcesi);

                nakitBagisci.Adi = ekstreAktarma.Adi.ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo);
                nakitBagisci.Adres = ekstreAktarma.Adres.ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo);
                nakitBagisci.Ilcesi = ilce != null ? ilce.Id.ToString() : GetIlceId(ekstreAktarma).ToString();//ekstreAktarma.Ilcesi;
                nakitBagisci.Ili = GetIlId(ekstreAktarma.Ili).ToString();//ekstreAktarma.Ili;
                nakitBagisci.TCKimlikNo = (nakitBagisci.TCKimlikNo < 1) && (ekstreAktarma.TCKimlikNo > 0) ? ekstreAktarma.TCKimlikNo : nakitBagisci.TCKimlikNo;
                nakitBagisci.Telefon1 = UtilityHelper.TelefonFormatla(ekstreAktarma.Telefon1.ReturnEmptyIfNull().ToString());
                nakitBagisci.Telefon2 = UtilityHelper.TelefonFormatla(ekstreAktarma.Telefon2.ReturnEmptyIfNull().ToString());
                nakitBagisci.TuzelKisi = ekstreAktarma.TuzelKisi;
                nakitBagisci.Olusturan = currentUser;
                nakitBagisci.Sag = true;
                nakitBagisci.Ulasilamiyor = false; //brası tamam çünkü bu yeni biri
                nakitBagisci.BelgeIstemiyor = false;
                nakitBagisci.BelgeIstemiyor = ekstreAktarma.BelgeIstemiyor == true ? true : false;
                if (string.IsNullOrEmpty(ekstreAktarma.Telefon1) &&
                    string.IsNullOrEmpty(ekstreAktarma.Telefon2) &&
                    string.IsNullOrEmpty(ekstreAktarma.Adres))
                {
                    nakitBagisci.Ulasilamiyor = true;
                }

                nakitBagisci.Save();

            }//eski bagisci
            else
            {
                Ilce ilce = new Ilce();
                ilce = ilce.SelectByIlNameAndIlceName(ekstreAktarma.Ili, ekstreAktarma.Ilcesi);

                nakitBagisci.Adi = string.IsNullOrEmpty(nakitBagisci.Adi) ? ekstreAktarma.Adi.ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo) : nakitBagisci.Adi;
                nakitBagisci.Adres = string.IsNullOrEmpty(nakitBagisci.Adres) ? ekstreAktarma.Adres.ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo) : nakitBagisci.Adres;
                nakitBagisci.Ilcesi = string.IsNullOrEmpty(nakitBagisci.Ilcesi) ? ilce != null ? ilce.Id.ToString() : GetIlceId(ekstreAktarma).ToString() : nakitBagisci.Ilcesi;//ekstreAktarma.Ilcesi;
                nakitBagisci.Ili = string.IsNullOrEmpty(nakitBagisci.Ili) ? GetIlId(ekstreAktarma.Ili).ToString() : nakitBagisci.Ili;//ekstreAktarma.Ili;
                nakitBagisci.TCKimlikNo = nakitBagisci.TCKimlikNo < 1 ? ekstreAktarma.TCKimlikNo : nakitBagisci.TCKimlikNo;
                nakitBagisci.Telefon1 = string.IsNullOrEmpty(nakitBagisci.Telefon1) ? UtilityHelper.TelefonFormatla(ekstreAktarma.Telefon1.ReturnEmptyIfNull().ToString()) : nakitBagisci.Telefon1;
                nakitBagisci.Telefon2 = string.IsNullOrEmpty(nakitBagisci.Telefon2) ? UtilityHelper.TelefonFormatla(ekstreAktarma.Telefon2.ReturnEmptyIfNull().ToString()) : nakitBagisci.Telefon2;
                nakitBagisci.TuzelKisi = ekstreAktarma.TuzelKisi;
                nakitBagisci.Olusturan = currentUser;
                nakitBagisci.Sag = true;
                //nakitBagisci.Ulasilamiyor = false; kapandı, çünkü  SB 02.04.2021 Bağışçının ulaşılamıyor bilgisini bozuyor
                //nakitBagisci.BelgeIstemiyor = false; kapandı, çünkü  SB 02.04.2021 Bağışçının BelgeIstemiyor bilgisini bozuyor
                nakitBagisci.BelgeIstemiyor = ekstreAktarma.BelgeIstemiyor == true ? true : nakitBagisci.BelgeIstemiyor;
                if (string.IsNullOrEmpty(ekstreAktarma.Telefon1) &&
                    string.IsNullOrEmpty(ekstreAktarma.Telefon2) &&
                    string.IsNullOrEmpty(ekstreAktarma.Adres))
                {
                    nakitBagisci.Ulasilamiyor = true;
                }

                nakitBagisci.Update();
            }

            return nakitBagisci.Id;
        }
    }
}
