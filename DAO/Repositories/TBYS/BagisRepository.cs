using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.TBYS
{
    public class BagisRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public BagisRepository() : this(new DbClass()) { }

        public BagisRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM Bagis_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery("SELECT * FROM Bagis_Table"), "");
        }

        public DataTable SelectByBagisciId(int bagisciId)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM Bagis_Table WHERE BagisciId=@BagisciId");
            query.AddParameter("@BagisciId", bagisciId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByTasinmazId(int tasinmazId)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM Bagis_Table WHERE TasinmazId=@TasinmazId");
            query.AddParameter("@TasinmazId", tasinmazId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByBagisciIdGroupByKullanimSekli(int bagisciId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT B.KullanimSekli,COUNT(A.Id) Adet,B.Ili, SUM(B.TahminiRayicDegeri) TahminiRayic
                FROM Bagis_Table A
                    INNER JOIN Tasinmaz_Table B ON B.Id=A.TasinmazId
                WHERE A.BagisciId=@BagisciId
                GROUP BY B.KullanimSekli,B.Ili");
            query.AddParameter("@BagisciId", bagisciId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByBagisciIdAsJson(int bagisciId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT A.Id BagisciId, A.Adi, A.Soyadi, C.Id TasinmazId, C.Adres
                FROM TasinmazBagisci_Table A
                    INNER JOIN Bagis_Table B ON B.BagisciId=A.Id
                    INNER JOIN Tasinmaz_Table C ON C.Id=B.TasinmazId AND C.EnvanterdeMi=1
                WHERE B.BagisciId=@BagisciId");
            query.AddParameter("@BagisciId", bagisciId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectTasinmazByBagisciId(int bagisciId, string[] envanterdenCikmaSebepleri)
        {
            bool includeSatisVsDahil = envanterdenCikmaSebepleri != null;
            string sebepInClause = includeSatisVsDahil
                ? string.Join(",", CreateParameterNames(envanterdenCikmaSebepleri.Length))
                : string.Empty;
            string inventoryFilter = includeSatisVsDahil
                ? "AND (A.EnvanterdeMi=1 OR (A.EnvanterdeMi=0 AND A.EnvanterdenCikmaSebebi IN (" + sebepInClause + ")))"
                : "AND A.EnvanterdeMi=1";
            string envanterColumns = includeSatisVsDahil ? ",A.EnvanterdeMi,A.EnvanterdenCikmaSebebi" : string.Empty;
            SqlQuery query = new SqlQuery(@"
                SELECT ROW_NUMBER() OVER (ORDER BY A.Id,B.BagisTarihi) AS Sirano,
                    A.Id TasinmazId,A.TahminiRayicDegeri,A.Cinsi,A.KullanimSekli,A.Adres,
                    A.MulkiyetSekli,A.KiraDurumu,A.EmlakBeyanDegeri,A.TahminiRayicDegeri" + envanterColumns + @",
                    B.Id BagisId,B.BagisYili,
                    D.IlceAdi +'-'+C.IlAdi IlIlce
                FROM Bagis_Table B
                INNER JOIN Tasinmaz_Table A ON A.Id=B.TasinmazId
                LEFT JOIN Il_Table C ON C.IlAdi=A.Ili
                LEFT JOIN Ilce_Table D ON D.IlceAdi=A.Ilcesi AND D.IlId=C.Id
                WHERE B.BagisciId=@BagisciId " + inventoryFilter + @"
                ORDER BY A.Id,B.BagisTarihi");
            query.AddParameter("@BagisciId", bagisciId);
            if (includeSatisVsDahil)
            {
                for (int i = 0; i < envanterdenCikmaSebepleri.Length; i++)
                    query.AddParameter("@Sebep" + i, envanterdenCikmaSebepleri[i]);
            }
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectSumTahminiRayicByBagisciId(int bagisciId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT SUM(TahminiRayicDegeri) Toplam
                FROM Bagis_Table A
                INNER JOIN Tasinmaz_Table B ON B.Id=A.TasinmazId AND B.EnvanterdeMi=1
                WHERE A.BagisciId=@BagisciId");
            query.AddParameter("@BagisciId", bagisciId);
            return db.SelectFromDb(query, "");
        }

        public int Insert<T>(T item) { return db.Insert(queryBuilder.BuildInsert(item, "Bagis_Table")); }
        public bool Update<T>(T item) { return db.Update2Db(queryBuilder.BuildUpdate(item, "Bagis_Table")); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("Bagis_Table", id), ""); }

        private static string[] CreateParameterNames(int count)
        {
            string[] names = new string[count];
            for (int i = 0; i < count; i++) names[i] = "@Sebep" + i;
            return names;
        }
    }
}
