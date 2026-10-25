using DAO.Ortak;
using System;
using System.Data;
using System.Diagnostics;
using Utility.Logging;

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


        public DataTable SelectById(int id, PortalLogContext context)
        { SqlQuery q = new SqlQuery("SELECT * FROM Harcirah_Table WHERE Id=@Id"); q.AddParameter("@Id", id); return PortalSqlReadExecutor.Select(q, context, "Harcirah.SelectById"); }
        public DataTable SelectById(int id, SqlTransactionContext transaction, PortalLogContext context) { if (transaction == null) throw new ArgumentNullException("transaction"); SqlQuery query = new SqlQuery("SELECT * FROM Harcirah_Table WITH (UPDLOCK, HOLDLOCK) WHERE Id=@Id"); query.AddParameter("@Id", id); Stopwatch clock = Stopwatch.StartNew(); bool ok = false; try { DataTable result = transaction.Select(query); ok = true; return result; } finally { transaction.DeferSqlTrace(context, "SELECT", "Harcirah.SelectById", clock.ElapsedMilliseconds, ok); } }
        public DataTable SelectAll(PortalLogContext context)
        { return PortalSqlReadExecutor.Select(new SqlQuery("SELECT * FROM Harcirah_Table ORDER BY KadroGrupId,Sira"), context, "Harcirah.SelectAll"); }
        public int Insert<T>(T item, SqlTransactionContext transaction, PortalLogContext context) { return TraceInsert(queryBuilder.BuildInsert(item, "Harcirah_Table"), transaction, context); }
        public bool Update<T>(T item, SqlTransactionContext transaction, PortalLogContext context) { return TraceWrite(queryBuilder.BuildUpdate(item, "Harcirah_Table"), "UPDATE", "Harcirah.Update", transaction, context); }
        public bool Delete(int id, SqlTransactionContext transaction, PortalLogContext context) { return TraceWrite(queryBuilder.BuildDelete("Harcirah_Table", id), "DELETE", "Harcirah.Delete", transaction, context); }
        public DataTable SelectByKadroUlkeTarih(int kadroGrupId, string ulke, DateTime tarih, PortalLogContext context)
        {
            string dateFilter = tarih == null ? "" : " AND BaslangicTarihi<=@Tarih"; string countryFilter = string.IsNullOrEmpty(ulke) ? "" : " AND Ulke=@Ulke";
            SqlQuery q = new SqlQuery("SELECT * FROM Harcirah_Table WHERE KadroGrupId=@KadroGrupId" + dateFilter + countryFilter + " ORDER BY KadroGrupId,Sira"); q.AddParameter("@KadroGrupId", kadroGrupId); if (tarih != null) q.AddParameter("@Tarih", tarih); if (!string.IsNullOrEmpty(ulke)) q.AddParameter("@Ulke", ulke); return PortalSqlReadExecutor.Select(q, context, "Harcirah.SelectByKadroUlkeTarih");
        }
        public DataTable SelectByKadroGrupId(int kadroGrupId, PortalLogContext context)
        { SqlQuery q = new SqlQuery("SELECT * FROM Harcirah_Table WHERE KadroGrupId=@KadroGrupId ORDER BY KadroGrupId,Sira"); q.AddParameter("@KadroGrupId", kadroGrupId); return PortalSqlReadExecutor.Select(q, context, "Harcirah.SelectByKadroGrupId"); }
        public DataTable SelectByParaBirimi(string paraBirimi, PortalLogContext context)
        { SqlQuery q = new SqlQuery("SELECT * FROM Harcirah_Table WHERE ParaBirimi=@ParaBirimi"); q.AddParameter("@ParaBirimi", paraBirimi); return PortalSqlReadExecutor.Select(q, context, "Harcirah.SelectByParaBirimi"); }
        public DataTable SelectAllReturnDataTable(PortalLogContext context)
        { return PortalSqlReadExecutor.Select(new SqlQuery("SELECT * FROM Harcirah_Table ORDER BY KadroGrupId,Sira"), context, "Harcirah.SelectAllReturnDataTable"); }
        private DataTable TraceSelect(string name, Func<DataTable> action, PortalLogContext context) { Stopwatch clock = Stopwatch.StartNew(); bool ok = false; try { DataTable result = action(); ok = true; return result; } finally { PortalTechnicalLog.WriteSqlTrace(context, "SELECT", name, clock.ElapsedMilliseconds, ok); } }
        private static int TraceInsert(SqlQuery query, SqlTransactionContext transaction, PortalLogContext context) { if (transaction == null) throw new ArgumentNullException("transaction"); Stopwatch clock = Stopwatch.StartNew(); bool ok = false; try { int id = transaction.Insert(query); ok = id > 0; return id; } finally { transaction.DeferSqlTrace(context, "INSERT", "Harcirah.Insert", clock.ElapsedMilliseconds, ok); } }
        private static bool TraceWrite(SqlQuery query, string operation, string name, SqlTransactionContext transaction, PortalLogContext context) { if (transaction == null) throw new ArgumentNullException("transaction"); Stopwatch clock = Stopwatch.StartNew(); bool ok = false; try { ok = transaction.Update(query); return ok; } finally { transaction.DeferSqlTrace(context, operation, name, clock.ElapsedMilliseconds, ok); } }
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
