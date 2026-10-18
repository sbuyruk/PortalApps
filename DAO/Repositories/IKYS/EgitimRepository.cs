using DAO.Ortak;
using System;
using System.Data;
using System.Diagnostics;
using Utility.Logging;
namespace DAO.Repositories.IKYS
{
    public class EgitimRepository
    {
        private readonly DbClass db; private readonly CrudQueryBuilder queryBuilder;
        public EgitimRepository() : this(new DbClass()) { }
        public EgitimRepository(DbClass db) { if (db == null) throw new ArgumentNullException("db"); this.db = db; queryBuilder = new CrudQueryBuilder(); }
        public DataTable SelectById(int id) { SqlQuery q = new SqlQuery("SELECT * FROM Egitim_Table WHERE Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectById(int id, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            SqlQuery query = new SqlQuery("SELECT * FROM Egitim_Table WITH (UPDLOCK, HOLDLOCK) WHERE Id=@Id");
            query.AddParameter("@Id", id);
            Stopwatch clock = Stopwatch.StartNew();
            bool succeeded = false;
            try { DataTable result = transaction.Select(query); succeeded = true; return result; }
            finally { PortalTechnicalLog.WriteSqlTrace(context, "SELECT", "Egitim.SelectById", clock.ElapsedMilliseconds, succeeded); }
        }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM Egitim_Table"), ""); }
        public int Insert<T>(T item) { return db.Insert(queryBuilder.BuildInsert(item, "Egitim_Table")); }
        public int Insert<T>(T item, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            Stopwatch clock = Stopwatch.StartNew();
            bool succeeded = false;
            try { int id = transaction.Insert(queryBuilder.BuildInsert(item, "Egitim_Table")); succeeded = id > 0; return id; }
            finally { PortalTechnicalLog.WriteSqlTrace(context, "INSERT", "Egitim.Insert", clock.ElapsedMilliseconds, succeeded); }
        }
        public bool Update<T>(T item) { return db.Update2Db(queryBuilder.BuildUpdate(item, "Egitim_Table")); }
        public bool Update<T>(T item, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            return TraceWrite(queryBuilder.BuildUpdate(item, "Egitim_Table"), "UPDATE", "Egitim.Update", transaction, context);
        }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("Egitim_Table", id), ""); }
        public bool Delete(int id, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            return TraceWrite(queryBuilder.BuildDelete("Egitim_Table", id), "DELETE", "Egitim.Delete", transaction, context);
        }
        public DataTable SelectByPersonelId(int personelId) { SqlQuery q = new SqlQuery("SELECT * FROM Egitim_Table WHERE PersonelId=@PersonelId ORDER BY MezuniyetTar DESC"); q.AddParameter("@PersonelId", personelId); return db.SelectFromDb(q, ""); }
        public DataTable SelectByPersonelId(int personelId, PortalLogContext context)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM Egitim_Table WHERE PersonelId=@PersonelId ORDER BY MezuniyetTar DESC");
            query.AddParameter("@PersonelId", personelId);
            Stopwatch clock = Stopwatch.StartNew();
            bool succeeded = false;
            try { DataTable result = db.SelectFromDb(query, ""); succeeded = true; return result; }
            finally { PortalTechnicalLog.WriteSqlTrace(context, "SELECT", "Egitim.SelectByPersonelId", clock.ElapsedMilliseconds, succeeded); }
        }
        private static bool TraceWrite(SqlQuery query, string operation, string name, SqlTransactionContext transaction, PortalLogContext context)
        {
            Stopwatch clock = Stopwatch.StartNew();
            bool succeeded = false;
            try { succeeded = transaction.Update(query); return succeeded; }
            finally { PortalTechnicalLog.WriteSqlTrace(context, operation, name, clock.ElapsedMilliseconds, succeeded); }
        }
    }
}
