using DAO.Ortak;
using System;
using System.Data;
using System.Diagnostics;
using Utility.Logging;

namespace DAO.Repositories.IKYS
{
    public class BulunmamaSebebiRepository
    {
        private readonly DbClass db; private readonly CrudQueryBuilder queryBuilder;
        public BulunmamaSebebiRepository() : this(new DbClass()) { }
        public BulunmamaSebebiRepository(DbClass db) { if (db == null) throw new ArgumentNullException("db"); this.db = db; queryBuilder = new CrudQueryBuilder(); }
        public DataTable SelectById(int id) { SqlQuery q = new SqlQuery("SELECT * FROM BulunmamaSebebi_Table WHERE Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectById(int id, SqlTransactionContext transaction, PortalLogContext context) { if (transaction == null) throw new ArgumentNullException("transaction"); SqlQuery q = new SqlQuery("SELECT * FROM BulunmamaSebebi_Table WITH (UPDLOCK, HOLDLOCK) WHERE Id=@Id"); q.AddParameter("@Id", id); return TraceSelect(q, "BulunmamaSebebi.SelectById", transaction, context); }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM BulunmamaSebebi_Table"), ""); }
        public DataTable SelectAll(PortalLogContext context) { return TraceSelect(new SqlQuery("SELECT * FROM BulunmamaSebebi_Table"), "BulunmamaSebebi.SelectAll", context); }

        public int Insert<T>(T item, SqlTransactionContext transaction, PortalLogContext context) { if (transaction == null) throw new ArgumentNullException("transaction"); Stopwatch c = Stopwatch.StartNew(); bool ok = false; try { int id = transaction.Insert(queryBuilder.BuildInsert(item, "BulunmamaSebebi_Table")); ok = id > 0; return id; } finally { PortalTechnicalLog.WriteSqlTrace(context, "INSERT", "BulunmamaSebebi.Insert", c.ElapsedMilliseconds, ok); } }

        public bool Update<T>(T item, SqlTransactionContext transaction, PortalLogContext context) { if (transaction == null) throw new ArgumentNullException("transaction"); return TraceWrite(queryBuilder.BuildUpdate(item, "BulunmamaSebebi_Table"), "UPDATE", "BulunmamaSebebi.Update", transaction, context); }

        public bool Delete(int id, SqlTransactionContext transaction, PortalLogContext context) { if (transaction == null) throw new ArgumentNullException("transaction"); return TraceWrite(queryBuilder.BuildDelete("BulunmamaSebebi_Table", id), "DELETE", "BulunmamaSebebi.Delete", transaction, context); }
        private DataTable TraceSelect(SqlQuery q, string name, PortalLogContext context)
        {
            return PortalSqlReadExecutor.Select(q, context, name);
        }
        private static DataTable TraceSelect(SqlQuery q, string name, SqlTransactionContext t, PortalLogContext context) { Stopwatch c = Stopwatch.StartNew(); bool ok = false; try { DataTable r = t.Select(q); ok = true; return r; } finally { PortalTechnicalLog.WriteSqlTrace(context, "SELECT", name, c.ElapsedMilliseconds, ok); } }
        private static bool TraceWrite(SqlQuery q, string operation, string name, SqlTransactionContext t, PortalLogContext context) { Stopwatch c = Stopwatch.StartNew(); bool ok = false; try { ok = t.Update(q); return ok; } finally { PortalTechnicalLog.WriteSqlTrace(context, operation, name, c.ElapsedMilliseconds, ok); } }

        public DataTable SelectById(int id, PortalLogContext context)
        { SqlQuery q = new SqlQuery("SELECT * FROM BulunmamaSebebi_Table WHERE Id=@Id"); q.AddParameter("@Id", id); return PortalSqlReadExecutor.Select(q, context, "BulunmamaSebebi.SelectById"); }
    }
}
