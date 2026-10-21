using DAO.Ortak;
using Utility.Logging;
using System;
using System.Data;

namespace DAO.Repositories.TBYS
{
    public class BagisciYakinlariRepository
    {
        private readonly TbysSqlExecutor db; private readonly CrudQueryBuilder queryBuilder;
        public BagisciYakinlariRepository() : this(new DbClass()) { }
        public BagisciYakinlariRepository(DbClass db) { if (db == null) throw new ArgumentNullException("db"); this.db = new TbysSqlExecutor(db, "BagisciYakinlariRepository"); queryBuilder = new CrudQueryBuilder(); }
        public BagisciYakinlariRepository(PortalLogContext context)
        {
            db = new TbysSqlExecutor(context, "BagisciYakinlariRepository");
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id) { SqlQuery q = new SqlQuery("SELECT * FROM BagisciYakinlari_Table WHERE Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM BagisciYakinlari_Table"), ""); }
        public int Insert<T>(T item) { return db.Insert(queryBuilder.BuildInsert(item, "BagisciYakinlari_Table")); }
        public bool Update<T>(T item) { return db.Update2Db(queryBuilder.BuildUpdate(item, "BagisciYakinlari_Table")); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("BagisciYakinlari_Table", id), ""); }
        public DataTable SelectByBagisciId(int bagisciId) { SqlQuery q = new SqlQuery("SELECT * FROM BagisciYakinlari_Table WHERE BagisciId=@BagisciId"); q.AddParameter("@BagisciId", bagisciId); return db.SelectFromDb(q, ""); }

        public DataTable SelectById(int id, SqlTransactionContext transaction, PortalLogContext context)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM BagisciYakinlari_Table WITH (UPDLOCK, HOLDLOCK) WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, transaction, context);
        }

        public int Insert<T>(T entity, SqlTransactionContext transaction, PortalLogContext context)
        {
            return db.Insert(queryBuilder.BuildInsert(entity, "BagisciYakinlari_Table"), transaction, context);
        }

        public bool Update<T>(T entity, SqlTransactionContext transaction, PortalLogContext context)
        {
            return db.Update2Db(queryBuilder.BuildUpdate(entity, "BagisciYakinlari_Table"), transaction, context);
        }

        public bool Delete(int id, SqlTransactionContext transaction, PortalLogContext context)
        {
            return db.DeleteFromDb(queryBuilder.BuildDelete("BagisciYakinlari_Table", id), transaction, context);
        }
    }
}
