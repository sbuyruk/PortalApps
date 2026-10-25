using DAO.Ortak;
using System;
using System.Data;
using System.Diagnostics;
using Utility.Logging;
namespace DAO.Repositories.IKYS
{
    public class IsTecrubeRepository
    {
        private readonly DbClass db; private readonly CrudQueryBuilder queryBuilder;
        public IsTecrubeRepository() : this(new DbClass()) { }
        public IsTecrubeRepository(DbClass db) { if (db == null) throw new ArgumentNullException("db"); this.db = db; queryBuilder = new CrudQueryBuilder(); }
        public DataTable SelectById(int id) { SqlQuery q = new SqlQuery("SELECT * FROM IsTecrube_Table WHERE Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectById(int id, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            SqlQuery query = new SqlQuery("SELECT * FROM IsTecrube_Table WITH (UPDLOCK, HOLDLOCK) WHERE Id=@Id");
            query.AddParameter("@Id", id);
            Stopwatch clock = Stopwatch.StartNew();
            bool succeeded = false;
            try { DataTable result = transaction.Select(query); succeeded = true; return result; }
            finally { transaction.DeferSqlTrace(context, "SELECT", "IsTecrube.SelectById", clock.ElapsedMilliseconds, succeeded); }
        }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM IsTecrube_Table"), ""); }

        public int Insert<T>(T item, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            Stopwatch clock = Stopwatch.StartNew();
            bool succeeded = false;
            try { int id = transaction.Insert(queryBuilder.BuildInsert(item, "IsTecrube_Table")); succeeded = id > 0; return id; }
            finally { transaction.DeferSqlTrace(context, "INSERT", "IsTecrube.Insert", clock.ElapsedMilliseconds, succeeded); }
        }

        public bool Update<T>(T item, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            return TraceWrite(queryBuilder.BuildUpdate(item, "IsTecrube_Table"), "UPDATE", "IsTecrube.Update", transaction, context);
        }

        public bool Delete(int id, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            return TraceWrite(queryBuilder.BuildDelete("IsTecrube_Table", id), "DELETE", "IsTecrube.Delete", transaction, context);
        }
        public DataTable SelectByPersonelId(int personelId) { SqlQuery q = new SqlQuery("SELECT * FROM IsTecrube_Table WHERE PersonelId=@PersonelId ORDER BY BasTar DESC"); q.AddParameter("@PersonelId", personelId); return db.SelectFromDb(q, ""); }
        public DataTable SelectByPersonelId(int personelId, PortalLogContext context)
        { SqlQuery q = new SqlQuery("SELECT * FROM IsTecrube_Table WHERE PersonelId=@PersonelId ORDER BY BasTar DESC"); q.AddParameter("@PersonelId", personelId); return PortalSqlReadExecutor.Select(q, context, "IsTecrube.SelectByPersonelId"); }
        private static bool TraceWrite(SqlQuery query, string operation, string name, SqlTransactionContext transaction, PortalLogContext context)
        {
            Stopwatch clock = Stopwatch.StartNew();
            bool succeeded = false;
            try { succeeded = transaction.Update(query); return succeeded; }
            finally { transaction.DeferSqlTrace(context, operation, name, clock.ElapsedMilliseconds, succeeded); }
        }

        public DataTable SelectById(int id, PortalLogContext context)
        { SqlQuery q = new SqlQuery("SELECT * FROM IsTecrube_Table WHERE Id=@Id"); q.AddParameter("@Id", id); return PortalSqlReadExecutor.Select(q, context, "IsTecrube.SelectById"); }

        public DataTable SelectAll(PortalLogContext context)
        { return PortalSqlReadExecutor.Select(new SqlQuery("SELECT * FROM IsTecrube_Table"), context, "IsTecrube.SelectAll"); }
    }
}
