using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.TBYS
{
    public class BagisciYakinlariRepository
    {
        private readonly DbClass db; private readonly CrudQueryBuilder queryBuilder;
        public BagisciYakinlariRepository() : this(new DbClass()) { }
        public BagisciYakinlariRepository(DbClass db) { if (db == null) throw new ArgumentNullException("db"); this.db = db; queryBuilder = new CrudQueryBuilder(); }
        public DataTable SelectById(int id) { SqlQuery q = new SqlQuery("SELECT * FROM BagisciYakinlari_Table WHERE Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM BagisciYakinlari_Table"), ""); }
        public int Insert<T>(T item) { return db.Insert(queryBuilder.BuildInsert(item, "BagisciYakinlari_Table")); }
        public bool Update<T>(T item) { return db.Update2Db(queryBuilder.BuildUpdate(item, "BagisciYakinlari_Table")); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("BagisciYakinlari_Table", id), ""); }
        public DataTable SelectByBagisciId(int bagisciId) { SqlQuery q = new SqlQuery("SELECT * FROM BagisciYakinlari_Table WHERE BagisciId=@BagisciId"); q.AddParameter("@BagisciId", bagisciId); return db.SelectFromDb(q, ""); }
    }
}
