using DAO.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DAO.Repositories.NBYS
{
    public class NakitBagisHareketRepository
    {
        private const string TableName = "NakitBagisHareket_Table";
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public NakitBagisHareketRepository() : this(new DbClass())
        {
        }

        public NakitBagisHareketRepository(DbClass db)
        {
            if (db == null)
                throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

        public int Insert<T>(T entity)
        {
            return db.Insert(queryBuilder.BuildInsert(entity, TableName));
        }

        public bool Update<T>(T entity)
        {
            return db.Update2Db(queryBuilder.BuildUpdate(entity, TableName));
        }

        public void UpdateRefund<TDonation, TGift>(TDonation donation, TGift gift)
        {
            if ((object)donation == null)
                throw new ArgumentNullException("donation");

            List<SqlQuery> queries = new List<SqlQuery>
            {
                queryBuilder.BuildUpdate(donation, TableName)
            };

            if ((object)gift != null)
                queries.Add(queryBuilder.BuildUpdate(gift, "Armagan_Table"));

            db.ExecuteTransaction(queries);
        }

        public void DeleteWithArchive<TDonationArchive, TGiftArchive>(
            int donationId,
            TDonationArchive donationArchive,
            int? giftId,
            TGiftArchive giftArchive)
        {
            if ((object)donationArchive == null)
                throw new ArgumentNullException("donationArchive");

            List<SqlQuery> queries = new List<SqlQuery>
            {
                queryBuilder.BuildDelete(TableName, donationId),
                queryBuilder.BuildInsert(donationArchive, "SilinenKayit_Table")
            };

            if (giftId.HasValue)
            {
                if ((object)giftArchive == null)
                    throw new ArgumentNullException("giftArchive");

                queries.Add(queryBuilder.BuildDelete("Armagan_Table", giftId.Value));
                queries.Add(queryBuilder.BuildInsert(giftArchive, "SilinenKayit_Table"));
            }

            db.ExecuteTransaction(queries);
        }

        public bool Delete(int id)
        {
            return db.DeleteFromDb(queryBuilder.BuildDelete(TableName, id), "");
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM NakitBagisHareket_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery("SELECT * FROM NakitBagisHareket_Table"), "");
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

        public DataTable SelectDonorDetailRows(int bagisciId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT A.BagisciId, BagisTarihi,
                    REPLACE(CONVERT(varchar,A.BagisMiktari),'.',',') + ' ' + A.DovizCinsi BagisTutari,
                    REPLACE(ISNULL(CONVERT(varchar,B.BagisMiktari),''),'.',',') + ' ' + ISNULL(B.DovizCinsi,'') ArmaganTutari,
                    CONVERT(nvarchar,REPLACE(A.BagisMiktari,'.',',')) BagisMiktari,
                    A.DovizCinsi, ArmaganId, ISNULL(C.Armagan,'') Armagan,
                    ISNULL(B.Durum,'') Durum, ISNULL(B.Aciklama,'') Aciklama,
                    D.Banka, A.Aciklama NBHAciklama
                FROM NakitBagisHareket_Table A
                LEFT JOIN Armagan_Table B ON B.Id=A.ArmaganId
                LEFT JOIN ArmaganTanim_Table C ON C.Id=B.ArmaganTanimId
                LEFT JOIN BankaTanim_Table D ON D.Id=A.BankaId
                WHERE A.BagisciId=@BagisciId
                ORDER BY BagisTarihi DESC");
            query.AddParameter("@BagisciId", bagisciId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectProvinceYearSummary(int? ilId, DateTime tarih)
        {
            string ilKosulu = ilId.HasValue ? " AND D.Id=@IlId" : string.Empty;
            SqlQuery query = new SqlQuery(@"
                SELECT YEAR(BagisTarihi) Yil, SUM(BagisMiktari) BagisToplam,
                    COUNT(BagisMiktari) BagisSayisi, D.IlAdi
                FROM NakitBagisHareket_Table A
                LEFT JOIN NakitBagisci_Table B ON B.Id=A.BagisciId
                LEFT JOIN Il_Table D ON D.Id=B.Ili
                WHERE BagisTarihi>@Tarih" + ilKosulu + @"
                GROUP BY YEAR(BagisTarihi), D.IlAdi
                ORDER BY Yil DESC");
            query.AddParameter("@Tarih", LegacyDateValue(tarih));
            if (ilId.HasValue)
                query.AddParameter("@IlId", ilId.Value);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByRegionDate(int? bolgeId, DateTime baslangic, DateTime bitis)
        {
            string bolgeKosulu = bolgeId.HasValue ? " AND C.BolgeId=@BolgeId" : string.Empty;
            SqlQuery query = new SqlQuery(@"
                SELECT A.BagisTarihi, A.BagisMiktari,
                    B.Id NakitBagisciId, B.Adi, B.Soyadi, B.Telefon1, B.Telefon2,
                    B.Adres, B.BelgeIstemiyor, C.IlAdi Ili, D.IlceAdi Ilcesi,
                    F.Armagan, E.Durum,
                    IIF(E.DuzenliBagis=1, 'Düzenli Bağış',
                        IIF(E.CokluBagis=1, 'Çoklu Bağış', 'Bağış')) CokluBagis
                FROM NakitBagisHareket_Table A
                LEFT JOIN NakitBagisci_Table B ON B.Id=A.BagisciId
                INNER JOIN Il_Table C ON C.Id=B.Ili
                LEFT JOIN Ilce_Table D ON D.Id=B.Ilcesi
                LEFT JOIN Armagan_Table E ON E.Id=A.ArmaganId
                LEFT JOIN ArmaganTanim_Table F ON F.Id=E.ArmaganTanimId
                WHERE BagisTarihi BETWEEN @Baslangic AND @Bitis" + bolgeKosulu + @"
                ORDER BY BagisMiktari DESC, Adi, BagisTarihi DESC");
            query.AddParameter("@Baslangic", LegacyDateValue(baslangic));
            query.AddParameter("@Bitis", LegacyDateValue(bitis));
            if (bolgeId.HasValue)
                query.AddParameter("@BolgeId", bolgeId.Value);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectDonorDonationDetails(int bagisciId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT A.BagisTarihi, A.BagisMiktari,
                    B.Id NakitBagisciId, B.Adi, B.Soyadi, B.Telefon1, B.Telefon2,
                    B.Adres, B.BelgeIstemiyor, C.IlAdi Ili, D.IlceAdi Ilcesi,
                    F.Armagan, E.Durum,
                    IIF(E.DuzenliBagis=1, 'Düzenli Bagis',
                        IIF(E.CokluBagis=1, 'Çoklu Bagis', 'Bagis')) CokluBagis
                FROM NakitBagisHareket_Table A
                LEFT JOIN NakitBagisci_Table B ON B.Id=A.BagisciId
                INNER JOIN Il_Table C ON C.Id=B.Ili
                LEFT JOIN Ilce_Table D ON D.Id=B.Ilcesi
                LEFT JOIN Armagan_Table E ON E.Id=A.ArmaganId
                LEFT JOIN ArmaganTanim_Table F ON F.Id=E.ArmaganTanimId
                WHERE B.Id=@BagisciId
                ORDER BY BagisTarihi DESC");
            query.AddParameter("@BagisciId", bagisciId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectDonationReport(
            DateTime? bagisBasTarihi, DateTime? bagisBitTarihi,
            decimal? minBagisMiktari, decimal? maxBagisMiktari,
            int? armaganId, DateTime? sonBagisTarihi,
            int? ilId, int? ilceId,
            bool? sag, bool? belgeIstemiyor, bool? ulasilamiyor, bool? tuzelKisi)
        {
            StringBuilder where = new StringBuilder(" WHERE 1=1");
            StringBuilder having = new StringBuilder();

            AppendWhere(where, bagisBasTarihi.HasValue, " AND A.BagisTarihi>=@BagisBasTarihi");
            AppendWhere(where, bagisBitTarihi.HasValue, " AND A.BagisTarihi<=@BagisBitTarihi");
            // Legacy behavior: maximum amount filters both individual rows and grouped totals.
            AppendWhere(where, maxBagisMiktari.HasValue, " AND A.BagisMiktari<=@MaxBagisMiktari");
            AppendWhere(where, armaganId.HasValue, " AND A.ArmaganId=@ArmaganId");
            AppendWhere(where, ilId.HasValue, " AND B.Ili=@IlId");
            AppendWhere(where, ilceId.HasValue, " AND B.Ilcesi=@IlceId");
            AppendWhere(where, sag.HasValue, " AND B.Sag=@Sag");
            AppendWhere(where, belgeIstemiyor.HasValue, " AND B.BelgeIstemiyor=@BelgeIstemiyor");
            AppendWhere(where, ulasilamiyor.HasValue, " AND B.Ulasilamiyor=@Ulasilamiyor");
            AppendWhere(where, tuzelKisi.HasValue, " AND B.TuzelKisi=@TuzelKisi");

            AppendHaving(having, minBagisMiktari.HasValue, "SUM(A.BagisMiktari)>=@MinBagisMiktari");
            AppendHaving(having, maxBagisMiktari.HasValue, "SUM(A.BagisMiktari)<=@MaxBagisMiktari");
            AppendHaving(having, sonBagisTarihi.HasValue, "MAX(A.BagisTarihi)>@SonBagisTarihi");

            string havingSql = having.Length == 0 ? string.Empty : " HAVING " + having;
            SqlQuery query = new SqlQuery(@"
                SELECT B.Id NakitBagisciId,
                    ISNULL(B.Adi,'') + ' ' + ISNULL(B.Soyadi,'') AdiSoyadi,
                    REPLACE(CONVERT(NVARCHAR,SUM(A.BagisMiktari)),'.',',') + ' ' +
                        ISNULL(A.DovizCinsi,'TL') ToplamBagisMiktari,
                    SUM(A.BagisMiktari) ToplamBagisMiktariDecimal,
                    MAX(A.BagisTarihi) SonBagisTarihi,
                    ISNULL(E.IlAdi,'') Ili, ISNULL(F.IlceAdi,'') Ilcesi,
                    TRIM(ISNULL(B.Telefon1,'') + ' ' + ISNULL(B.Telefon2,'')) Telefon,
                    ISNULL(B.Adres,'') Adres
                FROM NakitBagisHareket_Table A
                INNER JOIN NakitBagisci_Table B ON B.Id=A.BagisciId
                LEFT JOIN Armagan_Table C ON C.Id=A.ArmaganId
                LEFT JOIN ArmaganTanim_Table D ON D.Id=C.ArmaganTanimId
                LEFT JOIN Il_Table E ON E.Id=B.Ili
                LEFT JOIN Ilce_Table F ON F.Id=B.Ilcesi" + where + @"
                GROUP BY B.Id, B.Adi, B.Soyadi, A.DovizCinsi, E.IlAdi, F.IlceAdi,
                    B.Telefon1, B.Telefon2, B.Adres" + havingSql + @"
                ORDER BY SUM(A.BagisMiktari) DESC");

            if (bagisBasTarihi.HasValue)
                query.AddParameter("@BagisBasTarihi", LegacyDateValue(bagisBasTarihi.Value));
            if (bagisBitTarihi.HasValue)
                query.AddParameter("@BagisBitTarihi", LegacyDateValue(bagisBitTarihi.Value));
            if (minBagisMiktari.HasValue)
                query.AddParameter("@MinBagisMiktari", minBagisMiktari.Value);
            if (maxBagisMiktari.HasValue)
                query.AddParameter("@MaxBagisMiktari", maxBagisMiktari.Value);
            if (armaganId.HasValue)
                query.AddParameter("@ArmaganId", armaganId.Value);
            if (sonBagisTarihi.HasValue)
                query.AddParameter("@SonBagisTarihi", LegacyDateValue(sonBagisTarihi.Value));
            if (ilId.HasValue)
                query.AddParameter("@IlId", ilId.Value);
            if (ilceId.HasValue)
                query.AddParameter("@IlceId", ilceId.Value);
            if (sag.HasValue)
                query.AddParameter("@Sag", sag.Value ? 1 : 0);
            if (belgeIstemiyor.HasValue)
                query.AddParameter("@BelgeIstemiyor", belgeIstemiyor.Value ? 1 : 0);
            if (ulasilamiyor.HasValue)
                query.AddParameter("@Ulasilamiyor", ulasilamiyor.Value ? 1 : 0);
            if (tuzelKisi.HasValue)
                query.AddParameter("@TuzelKisi", tuzelKisi.Value ? 1 : 0);

            return db.SelectFromDb(query, "");
        }

        private static void AppendWhere(StringBuilder builder, bool condition, string clause)
        {
            if (condition)
                builder.Append(clause);
        }

        private static void AppendHaving(StringBuilder builder, bool condition, string clause)
        {
            if (!condition)
                return;
            if (builder.Length > 0)
                builder.Append(" AND ");
            builder.Append(clause);
        }

        // Preserve ReturnTRDateFormat's culture, precision and MinValue semantics.
        // Parameters carry the value without SQL literal quotes.
        private static string LegacyDateValue(DateTime value)
        {
            return value == DateTime.MinValue ? string.Empty : value.ToString();
        }
    }
}
