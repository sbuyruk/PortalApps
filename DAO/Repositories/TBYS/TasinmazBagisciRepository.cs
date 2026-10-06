using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.TBYS
{
    public class TasinmazBagisciRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;
        public TasinmazBagisciRepository() : this(new DbClass()) { }
        public TasinmazBagisciRepository(DbClass db) { if (db == null) throw new ArgumentNullException("db"); this.db = db; queryBuilder = new CrudQueryBuilder(); }
        public DataTable SelectById(int id) { SqlQuery q = new SqlQuery("SELECT * FROM TasinmazBagisci_Table WHERE Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM TasinmazBagisci_Table"), ""); }
        public DataTable SelectAllBySagVefat(string sag) { SqlQuery q = new SqlQuery("SELECT * FROM TasinmazBagisci_Table" + (string.IsNullOrEmpty(sag) ? string.Empty : " WHERE Sag_vefat=@SagVefat")); if (!string.IsNullOrEmpty(sag)) q.AddParameter("@SagVefat", sag); return db.SelectFromDb(q, ""); }
        public DataTable SelectByBolge(int bolgeId, bool allRegions) { SqlQuery q = new SqlQuery("SELECT * FROM TasinmazBagisci_Table A LEFT JOIN Il_Table B ON B.IlAdi=A.Ili" + (allRegions ? string.Empty : " WHERE BolgeId=@BolgeId")); if (!allRegions) q.AddParameter("@BolgeId", bolgeId); return db.SelectFromDb(q, ""); }
        public DataTable SelectByFilters(bool isSagVefat, bool isCiplakMulkiyet, bool isTCKimlikNoFull, bool isDogumTarihiFull, string sagValue, string ciplakMulkiyetValue)
        {
            string tc = isTCKimlikNoFull ? " AND TCKimlikNo IS NOT NULL AND TCKimlikNo > 0" : string.Empty;
            string dogum = isDogumTarihiFull ? " AND DogumTarihi IS NOT NULL AND DogumTarihi!='' AND DogumTarihi>'01.01.1900'" : string.Empty;
            string sag = isSagVefat ? " AND Sag_vefat=@SagVefat" : string.Empty;
            string ciplak = isCiplakMulkiyet ? " AND A.Id IN (SELECT B.BagisciId FROM Bagis_Table B INNER JOIN Tasinmaz_Table C ON C.Id=B.TasinmazId AND C.MulkiyetSekli=@MulkiyetSekli)" : string.Empty;
            SqlQuery q = new SqlQuery("SELECT * FROM TasinmazBagisci_Table A WHERE 1>0" + tc + dogum + sag + ciplak);
            if (isSagVefat) q.AddParameter("@SagVefat", sagValue);
            if (isCiplakMulkiyet) q.AddParameter("@MulkiyetSekli", ciplakMulkiyetValue);
            return db.SelectFromDb(q, "");
        }
        public DataTable SelectByIlAdi(string ilAdi) { SqlQuery q = new SqlQuery("SELECT * FROM TasinmazBagisci_Table WHERE Ili=@Ili"); q.AddParameter("@Ili", ilAdi); return db.SelectFromDb(q, ""); }
        public int Insert<T>(T item) { return db.Insert(queryBuilder.BuildInsert(item, "TasinmazBagisci_Table")); }
        public bool Update<T>(T item) { return db.Update2Db(queryBuilder.BuildUpdate(item, "TasinmazBagisci_Table")); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("TasinmazBagisci_Table", id), ""); }
    }
}
