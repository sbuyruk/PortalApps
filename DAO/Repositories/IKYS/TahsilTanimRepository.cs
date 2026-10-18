using DAO.Ortak;
using System;
using System.Data;
using System.Diagnostics;
using Utility.Logging;

namespace DAO.Repositories.IKYS
{
    public class TahsilTanimRepository
    {
        private readonly DbClass db; private readonly CrudQueryBuilder queryBuilder;
        public TahsilTanimRepository() : this(new DbClass()) { }
        public TahsilTanimRepository(DbClass db) { if (db == null) throw new ArgumentNullException("db"); this.db = db; queryBuilder = new CrudQueryBuilder(); }
        public DataTable SelectById(int id) { SqlQuery q = new SqlQuery("SELECT * FROM TahsilTanim_Table WHERE Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectById(int id, SqlTransactionContext transaction, PortalLogContext context) { if (transaction == null) throw new ArgumentNullException("transaction"); SqlQuery q = new SqlQuery("SELECT * FROM TahsilTanim_Table WITH (UPDLOCK, HOLDLOCK) WHERE Id=@Id"); q.AddParameter("@Id", id); return TraceSelect(q, "TahsilTanim.SelectById", transaction, context); }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM TahsilTanim_Table"), ""); }
        public DataTable SelectAll(PortalLogContext context) { return TraceSelect(new SqlQuery("SELECT * FROM TahsilTanim_Table"), "TahsilTanim.SelectAll", context); }
        public int Insert<T>(T item) { return db.Insert(queryBuilder.BuildInsert(item, "TahsilTanim_Table")); }
        public int Insert<T>(T item, SqlTransactionContext transaction, PortalLogContext context) { if (transaction == null) throw new ArgumentNullException("transaction"); Stopwatch c = Stopwatch.StartNew(); bool ok = false; try { int id = transaction.Insert(queryBuilder.BuildInsert(item, "TahsilTanim_Table")); ok = id > 0; return id; } finally { PortalTechnicalLog.WriteSqlTrace(context, "INSERT", "TahsilTanim.Insert", c.ElapsedMilliseconds, ok); } }
        public bool Update<T>(T item) { return db.Update2Db(queryBuilder.BuildUpdate(item, "TahsilTanim_Table")); }
        public bool Update<T>(T item, SqlTransactionContext transaction, PortalLogContext context) { if (transaction == null) throw new ArgumentNullException("transaction"); return TraceWrite(queryBuilder.BuildUpdate(item, "TahsilTanim_Table"), "UPDATE", "TahsilTanim.Update", transaction, context); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("TahsilTanim_Table", id), ""); }
        public bool Delete(int id, SqlTransactionContext transaction, PortalLogContext context) { if (transaction == null) throw new ArgumentNullException("transaction"); return TraceWrite(queryBuilder.BuildDelete("TahsilTanim_Table", id), "DELETE", "TahsilTanim.Delete", transaction, context); }
        private DataTable TraceSelect(SqlQuery q, string name, PortalLogContext context) { Stopwatch c = Stopwatch.StartNew(); bool ok = false; try { DataTable r = db.SelectFromDb(q, ""); ok = true; return r; } finally { PortalTechnicalLog.WriteSqlTrace(context, "SELECT", name, c.ElapsedMilliseconds, ok); } }
        private static DataTable TraceSelect(SqlQuery q, string name, SqlTransactionContext t, PortalLogContext context) { Stopwatch c = Stopwatch.StartNew(); bool ok = false; try { DataTable r = t.Select(q); ok = true; return r; } finally { PortalTechnicalLog.WriteSqlTrace(context, "SELECT", name, c.ElapsedMilliseconds, ok); } }
        private static bool TraceWrite(SqlQuery q, string operation, string name, SqlTransactionContext t, PortalLogContext context) { Stopwatch c = Stopwatch.StartNew(); bool ok = false; try { ok = t.Update(q); return ok; } finally { PortalTechnicalLog.WriteSqlTrace(context, operation, name, c.ElapsedMilliseconds, ok); } }
    }
}
