using DAO.Ortak;
using System;
using System.Data;
using System.Diagnostics;
using Utility.Logging;

namespace DAO.Repositories.IKYS
{
    public class DereceKademeDegisimRepository
    {
        private readonly DbClass db; private readonly CrudQueryBuilder queryBuilder;
        public DereceKademeDegisimRepository() : this(new DbClass()) { }
        public DereceKademeDegisimRepository(DbClass db) { if (db == null) throw new ArgumentNullException("db"); this.db = db; queryBuilder = new CrudQueryBuilder(); }
        public DataTable SelectById(int id) { SqlQuery q = new SqlQuery("SELECT * FROM DereceKademeDegisim_Table WHERE Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectById(int id, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            SqlQuery query = new SqlQuery("SELECT * FROM DereceKademeDegisim_Table WITH (UPDLOCK, HOLDLOCK) WHERE Id=@Id"); query.AddParameter("@Id", id);
            return TraceSelect(query, "DereceKademeDegisim.SelectById", transaction, context);
        }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM DereceKademeDegisim_Table"), ""); }
        public int Insert<T>(T item) { return db.Insert(queryBuilder.BuildInsert(item, "DereceKademeDegisim_Table")); }
        public int Insert<T>(T item, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction"); Stopwatch clock = Stopwatch.StartNew(); bool succeeded = false;
            try { int id = transaction.Insert(queryBuilder.BuildInsert(item, "DereceKademeDegisim_Table")); succeeded = id > 0; return id; }
            finally { PortalTechnicalLog.WriteSqlTrace(context, "INSERT", "DereceKademeDegisim.Insert", clock.ElapsedMilliseconds, succeeded); }
        }
        public bool Update<T>(T item) { return db.Update2Db(queryBuilder.BuildUpdate(item, "DereceKademeDegisim_Table")); }
        public bool Update<T>(T item, SqlTransactionContext transaction, PortalLogContext context)
        { if (transaction == null) throw new ArgumentNullException("transaction"); return TraceWrite(queryBuilder.BuildUpdate(item, "DereceKademeDegisim_Table"), "UPDATE", "DereceKademeDegisim.Update", transaction, context); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("DereceKademeDegisim_Table", id), ""); }
        public bool Delete(int id, SqlTransactionContext transaction, PortalLogContext context)
        { if (transaction == null) throw new ArgumentNullException("transaction"); return TraceWrite(queryBuilder.BuildDelete("DereceKademeDegisim_Table", id), "DELETE", "DereceKademeDegisim.Delete", transaction, context); }
        public DataTable SelectByPersonelId(int personelId) { SqlQuery q = new SqlQuery("SELECT * FROM DereceKademeDegisim_Table WHERE PersonelId=@PersonelId ORDER BY DegisimTarihi DESC"); q.AddParameter("@PersonelId", personelId); return db.SelectFromDb(q, ""); }
        public DataTable SelectByPersonelId(int personelId, PortalLogContext context)
        { SqlQuery query = new SqlQuery("SELECT * FROM DereceKademeDegisim_Table WHERE PersonelId=@PersonelId ORDER BY DegisimTarihi DESC"); query.AddParameter("@PersonelId", personelId); return TraceSelect(query, "DereceKademeDegisim.SelectByPersonelId", context); }
        private static DataTable TraceSelect(SqlQuery query, string name, SqlTransactionContext transaction, PortalLogContext context)
        { Stopwatch clock = Stopwatch.StartNew(); bool succeeded = false; try { DataTable result = transaction.Select(query); succeeded = true; return result; } finally { PortalTechnicalLog.WriteSqlTrace(context, "SELECT", name, clock.ElapsedMilliseconds, succeeded); } }
        private DataTable TraceSelect(SqlQuery query, string name, PortalLogContext context)
        { Stopwatch clock = Stopwatch.StartNew(); bool succeeded = false; try { DataTable result = db.SelectFromDb(query, ""); succeeded = true; return result; } finally { PortalTechnicalLog.WriteSqlTrace(context, "SELECT", name, clock.ElapsedMilliseconds, succeeded); } }
        private static bool TraceWrite(SqlQuery query, string operation, string name, SqlTransactionContext transaction, PortalLogContext context)
        { Stopwatch clock = Stopwatch.StartNew(); bool succeeded = false; try { succeeded = transaction.Update(query); return succeeded; } finally { PortalTechnicalLog.WriteSqlTrace(context, operation, name, clock.ElapsedMilliseconds, succeeded); } }
    }
}
