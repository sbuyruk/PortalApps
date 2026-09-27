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

        // Preserve ReturnTRDateFormat's culture, precision and MinValue semantics.
        // Parameters carry the value without SQL literal quotes.
        private static string LegacyDateValue(DateTime value)
        {
            return value == DateTime.MinValue ? string.Empty : value.ToString();
        }
    }
}
