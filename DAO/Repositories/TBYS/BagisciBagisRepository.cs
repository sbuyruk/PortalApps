using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.TBYS
{
    public class BagisciBagisRepository
    {
        private readonly DbClass db; private readonly CrudQueryBuilder queryBuilder;
        public BagisciBagisRepository() : this(new DbClass()) { }
        public BagisciBagisRepository(DbClass db) { if (db == null) throw new ArgumentNullException("db"); this.db = db; queryBuilder = new CrudQueryBuilder(); }
        public DataTable SelectById(int id) { SqlQuery q = new SqlQuery("SELECT * FROM BagisciBagis_Table WHERE Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM BagisciBagis_Table"), ""); }
        public int Insert<T>(T item) { return db.Insert(queryBuilder.BuildInsert(item, "BagisciBagis_Table")); }
        public bool Update<T>(T item) { return db.Update2Db(queryBuilder.BuildUpdate(item, "BagisciBagis_Table")); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("BagisciBagis_Table", id), ""); }
        public DataTable SelectByBagisId(int bagisId) { SqlQuery q = new SqlQuery("SELECT * FROM BagisciBagis_Table WHERE BagisId=@BagisId"); q.AddParameter("@BagisId", bagisId); return db.SelectFromDb(q, ""); }
    }
}
