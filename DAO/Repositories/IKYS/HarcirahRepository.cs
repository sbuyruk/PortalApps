using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.IKYS
{
    public class HarcirahRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;
        public HarcirahRepository() : this(new DbClass()) { }
        public HarcirahRepository(DbClass db) { if (db == null) throw new ArgumentNullException("db"); this.db = db; queryBuilder = new CrudQueryBuilder(); }
        public DataTable SelectById(int id) { SqlQuery q = new SqlQuery("SELECT * FROM Harcirah_Table WHERE Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM Harcirah_Table ORDER BY KadroGrupId,Sira"), ""); }
        public int Insert<T>(T item) { return db.Insert(queryBuilder.BuildInsert(item, "Harcirah_Table")); }
        public bool Update<T>(T item) { return db.Update2Db(queryBuilder.BuildUpdate(item, "Harcirah_Table")); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("Harcirah_Table", id), ""); }
        public DataTable SelectByKadroUlkeTarih(int kadroGrupId, string ulke, DateTime tarih)
        {
            string dateFilter = tarih == null ? "" : " AND BaslangicTarihi<=@Tarih"; string countryFilter = string.IsNullOrEmpty(ulke) ? "" : " AND Ulke=@Ulke";
            SqlQuery q = new SqlQuery("SELECT * FROM Harcirah_Table WHERE KadroGrupId=@KadroGrupId" + dateFilter + countryFilter + " ORDER BY KadroGrupId,Sira"); q.AddParameter("@KadroGrupId", kadroGrupId); if (tarih != null) q.AddParameter("@Tarih", tarih); if (!string.IsNullOrEmpty(ulke)) q.AddParameter("@Ulke", ulke); return db.SelectFromDb(q, "");
        }
        public DataTable SelectByKadroGrupId(int kadroGrupId) { SqlQuery q = new SqlQuery("SELECT * FROM Harcirah_Table WHERE KadroGrupId=@KadroGrupId ORDER BY KadroGrupId,Sira"); q.AddParameter("@KadroGrupId", kadroGrupId); return db.SelectFromDb(q, ""); }
        public DataTable SelectByParaBirimi(string paraBirimi) { SqlQuery q = new SqlQuery("SELECT * FROM Harcirah_Table WHERE ParaBirimi=@ParaBirimi"); q.AddParameter("@ParaBirimi", paraBirimi); return db.SelectFromDb(q, ""); }
        public DataTable SelectAllReturnDataTable() { return db.SelectFromDb(new SqlQuery("SELECT * FROM Harcirah_Table ORDER BY KadroGrupId,Sira"), ""); }
    }
}
