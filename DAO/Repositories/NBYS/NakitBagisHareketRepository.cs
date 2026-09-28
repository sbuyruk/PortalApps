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

        // Preserve ReturnTRDateFormat's culture, precision and MinValue semantics.
        // Parameters carry the value without SQL literal quotes.
        private static string LegacyDateValue(DateTime value)
        {
            return value == DateTime.MinValue ? string.Empty : value.ToString();
        }
    }
}
