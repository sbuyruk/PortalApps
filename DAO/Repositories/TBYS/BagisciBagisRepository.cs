using DAO.Ortak;
using Utility.Logging;
using System;
using System.Data;

namespace DAO.Repositories.TBYS
{
    public class BagisciBagisRepository
    {
        private readonly TbysSqlExecutor db; private readonly CrudQueryBuilder queryBuilder;
        public BagisciBagisRepository() : this(new DbClass()) { }
        public BagisciBagisRepository(DbClass db) { if (db == null) throw new ArgumentNullException("db"); this.db = new TbysSqlExecutor(db, "BagisciBagisRepository"); queryBuilder = new CrudQueryBuilder(); }
        public BagisciBagisRepository(PortalLogContext context) { db = new TbysSqlExecutor(context, "BagisciBagisRepository"); queryBuilder = new CrudQueryBuilder(); }
        public DataTable SelectById(int id) { SqlQuery q = new SqlQuery("SELECT * FROM BagisciBagis_Table WHERE Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM BagisciBagis_Table"), ""); }
        public int Insert<T>(T item) { return db.Insert(queryBuilder.BuildInsert(item, "BagisciBagis_Table")); }
        public bool Update<T>(T item) { return db.Update2Db(queryBuilder.BuildUpdate(item, "BagisciBagis_Table")); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("BagisciBagis_Table", id), ""); }
        public DataTable SelectByBagisId(int bagisId) { SqlQuery q = new SqlQuery("SELECT * FROM BagisciBagis_Table WHERE BagisId=@BagisId"); q.AddParameter("@BagisId", bagisId); return db.SelectFromDb(q, ""); }

        public DataTable SelectById(int id, SqlTransactionContext transaction, PortalLogContext context) { SqlQuery q = new SqlQuery("SELECT * FROM BagisciBagis_Table WITH (UPDLOCK, HOLDLOCK) WHERE Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, transaction, context); }
        public int Insert<T>(T item, SqlTransactionContext transaction, PortalLogContext context) { return db.Insert(queryBuilder.BuildInsert(item, "BagisciBagis_Table"), transaction, context); }
        public bool Update<T>(T item, SqlTransactionContext transaction, PortalLogContext context) { return db.Update2Db(queryBuilder.BuildUpdate(item, "BagisciBagis_Table"), transaction, context); }
        public bool Delete(int id, SqlTransactionContext transaction, PortalLogContext context) { return db.DeleteFromDb(queryBuilder.BuildDelete("BagisciBagis_Table", id), transaction, context); }
    }
}
