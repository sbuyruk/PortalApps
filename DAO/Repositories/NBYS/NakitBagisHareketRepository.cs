using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.NBYS
{
    public class NakitBagisHareketRepository
    {
        private readonly DbClass db;

        public NakitBagisHareketRepository() : this(new DbClass())
        {
        }

        public NakitBagisHareketRepository(DbClass db)
        {
            if (db == null)
                throw new ArgumentNullException("db");
            this.db = db;
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM NakitBagisHareket_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByBagisciId(int bagisciId)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM NakitBagisHareket_Table WHERE BagisciId=@BagisciId");
            query.AddParameter("@BagisciId", bagisciId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByEkstreAktarmaId(int ekstreAktarmaId)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM NakitBagisHareket_Table WHERE EkstreAktarmaId=@EkstreAktarmaId");
            query.AddParameter("@EkstreAktarmaId", ekstreAktarmaId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByBagisciIdTarih(int bagisciId, DateTime baslangic, DateTime bitis)
        {
            SqlQuery query = new SqlQuery(@"SELECT * FROM NakitBagisHareket_Table
                WHERE BagisciId=@BagisciId AND BagisTarihi BETWEEN @Baslangic AND @Bitis");
            query.AddParameter("@BagisciId", bagisciId);
            query.AddParameter("@Baslangic", LegacyDateValue(baslangic));
            query.AddParameter("@Bitis", LegacyDateValue(bitis));
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectLastByBagisciIdTarih(int bagisciId, DateTime baslangic, DateTime bitis)
        {
            SqlQuery query = new SqlQuery(@"SELECT TOP 1 * FROM NakitBagisHareket_Table
                WHERE BagisciId=@BagisciId AND BagisTarihi>=@Baslangic AND BagisTarihi<=@Bitis
                ORDER BY BagisTarihi DESC");
            query.AddParameter("@BagisciId", bagisciId);
            query.AddParameter("@Baslangic", LegacyDateValue(baslangic));
            query.AddParameter("@Bitis", LegacyDateValue(bitis));
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectSumByBagisciIdTarih(int bagisciId, DateTime baslangic, DateTime bitis)
        {
            SqlQuery query = new SqlQuery(@"SELECT SUM(BagisMiktari) Toplam
                FROM NakitBagisHareket_Table
                WHERE BagisciId=@BagisciId AND BagisTarihi BETWEEN @Baslangic AND @Bitis");
            query.AddParameter("@BagisciId", bagisciId);
            query.AddParameter("@Baslangic", LegacyDateValue(baslangic));
            query.AddParameter("@Bitis", LegacyDateValue(bitis));
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectArmaganiOlmayanByBagisciId(int bagisciId)
        {
            SqlQuery query = new SqlQuery(@"SELECT A.*
                FROM NakitBagisHareket_Table A
                LEFT JOIN Armagan_Table B ON B.Id=A.ArmaganId AND B.ArmaganTanimId IN (2,3,4)
                WHERE A.BagisciId=@BagisciId AND B.Id IS NULL
                ORDER BY BagisMiktari DESC");
            query.AddParameter("@BagisciId", bagisciId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByFilter(string filter)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT A.Id BagisHareketId, B.Id ArmaganId, C.Armagan, B.Durum, A.BagisciId, D.Adi BagisciAdi
                    ,A.BagisTarihi, Convert(nvarchar,replace(A.BagisMiktari,'.',',')) BagisMiktari, A.DovizCinsi
                    ,D.Adres + ' ' + F.IlceAdi + ' / ' + E.IlAdi Adres, F.IlAdi Ili, F.IlceAdi Ilcesi
                    ,D.Telefon1 + IIF(ISNULL(D.Telefon1,'')!='' AND ISNULL(D.Telefon2,'')!='',' - ','') + D.Telefon2 Telefon
                    ,A.DovizCinsi, A.ArmaganId, C.Armagan, B.Durum, B.Aciklama, A.IadeEdildiMi,
                    ISNULL(A.IadeMiktari,0) IadeMiktari
                FROM NakitBagisHareket_Table A
                LEFT JOIN Armagan_Table B ON B.Id=A.ArmaganId
                LEFT JOIN ArmaganTanim_Table C ON C.Id=B.ArmaganTanimId
                INNER JOIN NakitBagisci_Table D ON D.Id=A.BagisciId
                LEFT JOIN Il_Table E ON E.Id=D.Ili
                LEFT JOIN Ilce_Table F ON F.Id=D.Ilcesi AND F.IlId=E.Id
                WHERE D.Adi LIKE @Filter
                    OR D.TCKimlikNo LIKE @Filter
                    OR D.Telefon1 LIKE @Filter
                    OR D.Adres LIKE @Filter
                ORDER BY A.BagisTarihi DESC");
            query.AddParameter("@Filter", "%" + (filter ?? string.Empty) + "%");
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByDurumTarih(string ay, string yil, int? ilId)
        {
            string ayKosulu = string.IsNullOrEmpty(ay) ? string.Empty : " AND MONTH(A.BagisTarihi)=@Ay";
            string ilKosulu = ilId.HasValue ? " AND B.Ili=@IlId" : string.Empty;
            SqlQuery query = new SqlQuery(@"
                SELECT A.Id NakitBagisHareketId, A.BagisciId, A.Id BagisId,
                    B.Adi, B.Adres, B.TCKimlikNo, TRIM(B.Telefon1 + ' ' + B.Telefon2) Telefon,
                    Convert(nvarchar,replace(A.BagisMiktari,'.',',')) BagisMiktari,
                    A.DovizCinsi, A.BagisTarihi, A.ArmaganId,
                    ISNULL(E.IlAdi,'') Ili, D.Durum, C.Banka, ISNULL(A.Aciklama,'') Aciklama
                FROM NakitBagisHareket_Table A
                INNER JOIN NakitBagisci_Table B ON B.Id=A.BagisciId
                INNER JOIN BankaTanim_Table C ON C.Id=A.BankaId
                LEFT JOIN Armagan_Table D ON D.Id=A.ArmaganId
                LEFT JOIN Il_Table E ON E.Id=B.Ili
                WHERE YEAR(A.BagisTarihi)=@Yil" + ayKosulu + ilKosulu);
            query.AddParameter("@Yil", yil);
            if (!string.IsNullOrEmpty(ay))
                query.AddParameter("@Ay", ay);
            if (ilId.HasValue)
                query.AddParameter("@IlId", ilId.Value);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectIadeEdilenBagislar(string ay, string yil)
        {
            string ayKosulu = string.IsNullOrEmpty(ay) ? string.Empty : " AND MONTH(A.IadeTarihi)=@Ay";
            SqlQuery query = new SqlQuery(@"
                SELECT A.IadeTarihi,
                    Convert(nvarchar,replace(ISNULL(A.IadeMiktari,0),'.',',')) IadeMiktari,
                    ISNULL(A.IadeSebebi,'') IadeSebebi,
                    B.Adi + ' ' + ISNULL(B.Soyadi,'') BagisciAdiSoyadi,
                    A.BagisTarihi, ISNULL(C.Banka,'') Banka, ISNULL(B.Adres,'') Adres,
                    ISNULL(E.IlAdi,'') Ili, ISNULL(F.IlceAdi,'') Ilcesi
                FROM NakitBagisHareket_Table A
                INNER JOIN NakitBagisci_Table B ON B.Id=A.BagisciId
                LEFT JOIN BankaTanim_Table C ON C.Id=A.BankaId
                LEFT JOIN Il_Table E ON E.Id=B.Ili
                LEFT JOIN Ilce_Table F ON F.Id=B.Ilcesi
                WHERE A.IadeEdildiMi=1 AND YEAR(A.IadeTarihi)=@Yil" + ayKosulu + @"
                ORDER BY A.IadeTarihi DESC");
            query.AddParameter("@Yil", yil);
            if (!string.IsNullOrEmpty(ay))
                query.AddParameter("@Ay", ay);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectSumByTarihBolge(DateTime baslangic, DateTime bitis, int? bolgeId)
        {
            string bolgeKosulu = bolgeId.HasValue ? " AND B.BolgeId=@BolgeId" : string.Empty;
            SqlQuery query = new SqlQuery(@"
                SELECT COUNT(H.Id) Adet, SUM(BagisMiktari) Toplam
                FROM NakitBagisHareket_Table H
                LEFT OUTER JOIN NakitBagisci_Table A ON A.Id=H.BagisciId
                LEFT OUTER JOIN Il_Table B ON B.Id=H.Ili
                WHERE BagisTarihi BETWEEN @Baslangic AND @Bitis" + bolgeKosulu);
            query.AddParameter("@Baslangic", LegacyDateValue(baslangic));
            query.AddParameter("@Bitis", LegacyDateValue(bitis));
            if (bolgeId.HasValue)
                query.AddParameter("@BolgeId", bolgeId.Value);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectSumByTarihBanka(DateTime baslangic, DateTime bitis, string banka)
        {
            string bankaKosulu = string.IsNullOrEmpty(banka)
                ? " AND BankaGrup IS NULL"
                : " AND BankaGrup=@Banka";
            SqlQuery query = new SqlQuery(@"
                SELECT COUNT(H.Id) Adet, SUM(BagisMiktari) Toplam
                FROM NakitBagisHareket_Table H
                LEFT OUTER JOIN NakitBagisci_Table A ON A.Id=H.BagisciId
                LEFT OUTER JOIN BankaTanim_Table B ON B.Id=H.BankaId
                WHERE BagisTarihi BETWEEN @Baslangic AND @Bitis
                    AND BagisciId IN (
                        SELECT BagisciId FROM NakitBagisHareket_Table WHERE BagisTarihi < @Baslangic)
                    " + bankaKosulu);
            query.AddParameter("@Baslangic", LegacyDateValue(baslangic));
            query.AddParameter("@Bitis", LegacyDateValue(bitis));
            if (!string.IsNullOrEmpty(banka))
                query.AddParameter("@Banka", banka);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectMaxByTarihBanka(DateTime baslangic, DateTime bitis, string banka)
        {
            string bankaKosulu = string.IsNullOrEmpty(banka) ? string.Empty : " AND BankaGrup=@Banka";
            SqlQuery query = new SqlQuery(@"
                SELECT MAX(BagisMiktari) Toplam
                FROM NakitBagisHareket_Table H
                LEFT OUTER JOIN NakitBagisci_Table A ON A.Id=H.BagisciId
                LEFT OUTER JOIN BankaTanim_Table B ON B.Id=H.BankaId
                WHERE BagisTarihi BETWEEN @Baslangic AND @Bitis" + bankaKosulu);
            query.AddParameter("@Baslangic", LegacyDateValue(baslangic));
            query.AddParameter("@Bitis", LegacyDateValue(bitis));
            if (!string.IsNullOrEmpty(banka))
                query.AddParameter("@Banka", banka);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectCountByTarihBolge(int yil, int ay)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT SUM(A.BagisMiktari) Toplam, COUNT(A.Id) Adet,
                    D.KisaAdi Bolge, MONTH(A.BagisTarihi) Ay
                FROM NakitBagisHareket_Table A
                INNER JOIN NakitBagisci_Table B ON B.Id=A.BagisciId
                INNER JOIN Il_Table C ON C.Id=B.Ili
                INNER JOIN Bolge_Table D ON D.Id=C.BolgeId
                WHERE YEAR(A.BagisTarihi)=@Yil AND MONTH(A.BagisTarihi)=@Ay
                GROUP BY D.KisaAdi, MONTH(A.BagisTarihi)");
            query.AddParameter("@Yil", yil);
            query.AddParameter("@Ay", ay);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectCountSumByTarih(DateTime baslangic, DateTime bitis)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT COUNT(H.Id) Adet, SUM(BagisMiktari) Toplam,
                    D.Id BolgeId, D.KisaAdi Bolge
                FROM NakitBagisHareket_Table H
                INNER JOIN NakitBagisci_Table B ON B.Id=H.BagisciId
                LEFT JOIN Il_Table C ON C.Id=H.Ili
                LEFT JOIN Bolge_Table D ON D.Id=C.BolgeId
                WHERE BagisTarihi BETWEEN @Baslangic AND @Bitis
                    AND BagisciId IN (
                        SELECT BagisciId FROM NakitBagisHareket_Table WHERE BagisTarihi < @Baslangic)
                GROUP BY D.Id, D.KisaAdi
                ORDER BY D.KisaAdi");
            query.AddParameter("@Baslangic", LegacyDateValue(baslangic));
            query.AddParameter("@Bitis", LegacyDateValue(bitis));
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectCountSumByYilIl(int baslangicYili, int bitisYili)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT C.Bolge, C.IlAdi, YEAR(A.BagisTarihi) Yil,
                    SUM(BagisMiktari) Tutar, COUNT(A.Id) Adet
                FROM NakitBagisHareket_Table A
                LEFT JOIN Il_Table C ON C.Id=A.Ili
                WHERE YEAR(A.BagisTarihi) BETWEEN @BaslangicYili AND @BitisYili
                GROUP BY C.Bolge, C.IlAdi, YEAR(A.BagisTarihi)
                ORDER BY C.Bolge, C.IlAdi, YEAR(A.BagisTarihi)");
            query.AddParameter("@BaslangicYili", baslangicYili);
            query.AddParameter("@BitisYili", bitisYili);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectCountSumByBanka(DateTime baslangic, DateTime bitis)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT COUNT(H.Id) Adet, SUM(BagisMiktari) Toplam, B.BankaGrup Banka
                FROM NakitBagisHareket_Table H
                INNER JOIN NakitBagisci_Table A ON A.Id=H.BagisciId
                INNER JOIN BankaTanim_Table B ON B.Id=H.BankaId
                WHERE BagisTarihi BETWEEN @Baslangic AND @Bitis
                GROUP BY B.BankaGrup
                ORDER BY Toplam DESC, B.BankaGrup");
            query.AddParameter("@Baslangic", LegacyDateValue(baslangic));
            query.AddParameter("@Bitis", LegacyDateValue(bitis));
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectDailyTotalsByBank(DateTime bagisTarihi, string bankaGrup, string dovizCinsi)
        {
            string bankaKosulu = string.IsNullOrEmpty(bankaGrup) ? string.Empty : " AND BankaGrup=@BankaGrup";
            string dovizKosulu = string.IsNullOrEmpty(dovizCinsi) ? string.Empty : " AND DovizCinsi=@DovizCinsi";
            SqlQuery query = new SqlQuery(@"
                SELECT BagisTarihi, SUM(BagisMiktari) ToplamBagis,
                    B.HesapKodu, B.HesapAdi, B.BankaGrup Banka
                FROM NakitBagisHareket_Table A
                LEFT JOIN BankaTanim_Table B ON B.Id=A.BankaId
                WHERE BagisTarihi=@BagisTarihi" + bankaKosulu + dovizKosulu + @"
                GROUP BY BagisTarihi, B.HesapKodu, B.HesapAdi, B.BankaGrup
                ORDER BY BagisTarihi");
            query.AddParameter("@BagisTarihi", LegacyDateValue(bagisTarihi));
            if (!string.IsNullOrEmpty(bankaGrup))
                query.AddParameter("@BankaGrup", bankaGrup);
            if (!string.IsNullOrEmpty(dovizCinsi))
                query.AddParameter("@DovizCinsi", dovizCinsi);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectDailySum(DateTime bagisTarihi, string bankaGrup, string dovizCinsi)
        {
            string bankaKosulu = string.IsNullOrEmpty(bankaGrup) ? string.Empty : " AND BankaGrup=@BankaGrup";
            string dovizKosulu = string.IsNullOrEmpty(dovizCinsi) ? string.Empty : " AND DovizCinsi=@DovizCinsi";
            SqlQuery query = new SqlQuery(@"
                SELECT BagisTarihi, SUM(BagisMiktari) Toplam
                FROM NakitBagisHareket_Table A
                LEFT JOIN BankaTanim_Table B ON B.Id=A.BankaId
                WHERE BagisTarihi=@BagisTarihi" + bankaKosulu + dovizKosulu + @"
                GROUP BY BagisTarihi");
            query.AddParameter("@BagisTarihi", LegacyDateValue(bagisTarihi));
            if (!string.IsNullOrEmpty(bankaGrup))
                query.AddParameter("@BankaGrup", bankaGrup);
            if (!string.IsNullOrEmpty(dovizCinsi))
                query.AddParameter("@DovizCinsi", dovizCinsi);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectDailyTlTotalByBankGroup2(DateTime bagisTarihi, string bankaGrup2, string tlDovizCinsi)
        {
            string bankaKosulu = string.IsNullOrEmpty(bankaGrup2) ? string.Empty : " AND BankaGrup2=@BankaGrup2";
            SqlQuery query = new SqlQuery(@"
                SELECT BagisTarihi, SUM(BagisMiktari + ISNULL(IadeMiktari,0)) ToplamBagis,
                    B.BankaGrup2 Banka
                FROM NakitBagisHareket_Table A
                LEFT JOIN BankaTanim_Table B ON B.Id=A.BankaId
                WHERE DovizCinsi=@DovizCinsi AND BagisTarihi=@BagisTarihi" + bankaKosulu + @"
                GROUP BY BagisTarihi, B.BankaGrup2");
            query.AddParameter("@DovizCinsi", tlDovizCinsi);
            query.AddParameter("@BagisTarihi", LegacyDateValue(bagisTarihi));
            if (!string.IsNullOrEmpty(bankaGrup2))
                query.AddParameter("@BankaGrup2", bankaGrup2);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectCurrencyDonationsByDateBankGroup2(
            DateTime baslangic, DateTime bitis, string bankaGrup2, string dovizCinsi)
        {
            string bankaKosulu = string.IsNullOrEmpty(bankaGrup2) ? string.Empty : " AND BankaGrup2=@BankaGrup2";
            SqlQuery query = new SqlQuery(@"
                SELECT BagisTarihi, A.DovizTutari, A.DovizKuru, A.DovizCinsi,
                    A.BagisMiktari, B.BankaGrup2, B.Banka
                FROM NakitBagisHareket_Table A
                LEFT JOIN BankaTanim_Table B ON B.Id=A.BankaId
                WHERE DovizCinsi=@DovizCinsi
                    AND BagisTarihi>=@Baslangic AND BagisTarihi<=@Bitis" + bankaKosulu);
            query.AddParameter("@DovizCinsi", dovizCinsi);
            query.AddParameter("@Baslangic", LegacyDateValue(baslangic));
            query.AddParameter("@Bitis", LegacyDateValue(bitis));
            if (!string.IsNullOrEmpty(bankaGrup2))
                query.AddParameter("@BankaGrup2", bankaGrup2);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectBankGroup2ByDateCurrency(DateTime baslangic, DateTime bitis, string dovizCinsi)
        {
            string dovizKosulu = string.IsNullOrEmpty(dovizCinsi) ? string.Empty : " AND DovizCinsi=@DovizCinsi";
            SqlQuery query = new SqlQuery(@"
                SELECT BankaGrup2
                FROM NakitBagisHareket_Table A
                INNER JOIN BankaTanim_Table B ON B.Id=A.BankaId
                WHERE BagisTarihi>=@Baslangic AND BagisTarihi<=@Bitis" + dovizKosulu + @"
                GROUP BY BankaGrup2");
            query.AddParameter("@Baslangic", LegacyDateValue(baslangic));
            query.AddParameter("@Bitis", LegacyDateValue(bitis));
            if (!string.IsNullOrEmpty(dovizCinsi))
                query.AddParameter("@DovizCinsi", dovizCinsi);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectCurrencyTotalsByDateBank(
            DateTime baslangic, DateTime bitis, string bankaGrup, string tlDovizCinsi)
        {
            string bankaKosulu = string.IsNullOrEmpty(bankaGrup) || bankaGrup == "0"
                ? string.Empty
                : " AND BankaGrup=@BankaGrup";
            SqlQuery query = new SqlQuery(@"
                SELECT DovizCinsi, SUM(BagisMiktari) TlKarsiligiToplamBagis,
                    SUM(DovizTutari) ToplamDovizTutari, B.BankaGrup Banka
                FROM NakitBagisHareket_Table A
                LEFT JOIN BankaTanim_Table B ON B.Id=A.BankaId
                WHERE DovizCinsi!=@TlDovizCinsi
                    AND BagisTarihi>=@Baslangic AND BagisTarihi<=@Bitis" + bankaKosulu + @"
                GROUP BY DovizCinsi, B.BankaGrup
                ORDER BY Banka");
            query.AddParameter("@TlDovizCinsi", tlDovizCinsi);
            query.AddParameter("@Baslangic", LegacyDateValue(baslangic));
            query.AddParameter("@Bitis", LegacyDateValue(bitis));
            if (!string.IsNullOrEmpty(bankaGrup) && bankaGrup != "0")
                query.AddParameter("@BankaGrup", bankaGrup);
            return db.SelectFromDb(query, "");
        }

        // Preserve ReturnTRDateFormat's culture, precision and MinValue semantics.
        // Parameters carry the value without SQL literal quotes.
        private static string LegacyDateValue(DateTime value)
        {
            return value == DateTime.MinValue ? string.Empty : value.ToString();
        }
    }
}
