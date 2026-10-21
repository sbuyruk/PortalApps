using DAO.Ortak;
using System;
using System.Data;
using Utility.Logging;

namespace DAO.Repositories.TBYS
{
    public class BagimsizBolumRepository
    {
        private readonly TbysSqlExecutor db;
        private readonly CrudQueryBuilder queryBuilder;
        public BagimsizBolumRepository() : this(new DbClass()) { }
        public BagimsizBolumRepository(DbClass db) { if (db == null) throw new ArgumentNullException("db"); this.db = new TbysSqlExecutor(db, "BagimsizBolumRepository"); queryBuilder = new CrudQueryBuilder(); }
        public BagimsizBolumRepository(PortalLogContext context) { db = new TbysSqlExecutor(context, "BagimsizBolumRepository"); queryBuilder = new CrudQueryBuilder(); }
        public DataTable SelectById(int id) { SqlQuery q = new SqlQuery("SELECT * FROM BagimsizBolum_Table WHERE Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM BagimsizBolum_Table"), ""); }
        public DataTable SelectByTasinmazId(int id) { SqlQuery q = new SqlQuery("SELECT * FROM BagimsizBolum_Table WHERE TasinmazId=@TasinmazId"); q.AddParameter("@TasinmazId", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectByBolumNo(string bolumNo) { SqlQuery q = new SqlQuery("SELECT * FROM BagimsizBolum_Table WHERE BolumNo=@BolumNo"); q.AddParameter("@BolumNo", bolumNo); return db.SelectFromDb(q, ""); }
        public DataTable SelectByBolumId(int id) { SqlQuery q = new SqlQuery("SELECT * FROM BagimsizBolum_Table WHERE bolumId=@BolumId"); q.AddParameter("@BolumId", id); return db.SelectFromDb(q, ""); }
        public int Insert<T>(T entity) { return db.Insert(queryBuilder.BuildInsert(entity, "BagimsizBolum_Table")); }
        public bool Update<T>(T entity) { return db.Update2Db(queryBuilder.BuildUpdate(entity, "BagimsizBolum_Table")); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("BagimsizBolum_Table", id), ""); }

        public DataTable SelectById(int id, SqlTransactionContext transaction, PortalLogContext context)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM BagimsizBolum_Table WITH (UPDLOCK, HOLDLOCK) WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, transaction, context);
        }

        public int Insert<T>(T entity, SqlTransactionContext transaction, PortalLogContext context)
        {
            return db.Insert(queryBuilder.BuildInsert(entity, "BagimsizBolum_Table"), transaction, context);
        }

        public bool Update<T>(T entity, SqlTransactionContext transaction, PortalLogContext context)
        {
            return db.Update2Db(queryBuilder.BuildUpdate(entity, "BagimsizBolum_Table"), transaction, context);
        }

        public bool Delete(int id, SqlTransactionContext transaction, PortalLogContext context)
        {
            return db.DeleteFromDb(queryBuilder.BuildDelete("BagimsizBolum_Table", id), transaction, context);
        }

        public DataTable SelectByTasinmazId(int tasinmazId, SqlTransactionContext transaction, PortalLogContext context)
        {
            SqlQuery query = new SqlQuery("SELECT TOP (1) * FROM BagimsizBolum_Table WITH (UPDLOCK, HOLDLOCK) WHERE TasinmazId=@TasinmazId ORDER BY Id");
            query.AddParameter("@TasinmazId", tasinmazId);
            return db.SelectFromDb(query, transaction, context);
        }

        public DataTable SelectAllByTasinmazId(int tasinmazId, SqlTransactionContext transaction, PortalLogContext context)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM BagimsizBolum_Table WITH (UPDLOCK, HOLDLOCK) WHERE TasinmazId=@TasinmazId ORDER BY Id");
            query.AddParameter("@TasinmazId", tasinmazId);
            return db.SelectFromDb(query, transaction, context);
        }
    }
}
