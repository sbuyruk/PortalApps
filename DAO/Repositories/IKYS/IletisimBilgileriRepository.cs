using DAO.Ortak;
using System;
using System.Data;
using System.Diagnostics;
using Utility.Logging;

namespace DAO.Repositories.IKYS
{
    public class IletisimBilgileriRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;
        public IletisimBilgileriRepository() : this(new DbClass()) { }
        public IletisimBilgileriRepository(DbClass db) { if (db == null) throw new ArgumentNullException("db"); this.db = db; queryBuilder = new CrudQueryBuilder(); }
        public DataTable SelectById(int id) { SqlQuery q = new SqlQuery("SELECT * FROM IletisimBilgileri_Table WHERE Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectById(int id, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            SqlQuery query = new SqlQuery("SELECT * FROM IletisimBilgileri_Table WITH (UPDLOCK, HOLDLOCK) WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return TraceSelect(query, "IletisimBilgileri.SelectById", transaction, context);
        }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM IletisimBilgileri_Table"), ""); }
        public int Insert<T>(T item) { return db.Insert(queryBuilder.BuildInsert(item, "IletisimBilgileri_Table")); }
        public int Insert<T>(T item, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            Stopwatch clock = Stopwatch.StartNew(); bool succeeded = false;
            try { int id = transaction.Insert(queryBuilder.BuildInsert(item, "IletisimBilgileri_Table")); succeeded = id > 0; return id; }
            finally { PortalTechnicalLog.WriteSqlTrace(context, "INSERT", "IletisimBilgileri.Insert", clock.ElapsedMilliseconds, succeeded); }
        }
        public bool Update<T>(T item) { return db.Update2Db(queryBuilder.BuildUpdate(item, "IletisimBilgileri_Table")); }
        public bool Update<T>(T item, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            return TraceWrite(queryBuilder.BuildUpdate(item, "IletisimBilgileri_Table"), "UPDATE", "IletisimBilgileri.Update", transaction, context);
        }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("IletisimBilgileri_Table", id), ""); }
        public bool Delete(int id, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            return TraceWrite(queryBuilder.BuildDelete("IletisimBilgileri_Table", id), "DELETE", "IletisimBilgileri.Delete", transaction, context);
        }
        public DataTable SelectByPersonelId(int personelId) { SqlQuery q = new SqlQuery("SELECT * FROM IletisimBilgileri_Table WHERE PersonelId=@PersonelId ORDER BY Adres"); q.AddParameter("@PersonelId", personelId); return db.SelectFromDb(q, ""); }
        public DataTable SelectByPersonelId(int personelId, PortalLogContext context)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM IletisimBilgileri_Table WHERE PersonelId=@PersonelId ORDER BY Adres");
            query.AddParameter("@PersonelId", personelId);
            return PortalSqlReadExecutor.Select(query, context, "IletisimBilgileri.SelectByPersonelId");
        }
        public DataTable SelectByPersonelId(int personelId, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            SqlQuery query = new SqlQuery("SELECT * FROM IletisimBilgileri_Table WITH (UPDLOCK, HOLDLOCK) WHERE PersonelId=@PersonelId ORDER BY Adres");
            query.AddParameter("@PersonelId", personelId);
            return TraceSelect(query, "IletisimBilgileri.SelectByPersonelId", transaction, context);
        }
        private static DataTable TraceSelect(SqlQuery query, string name, SqlTransactionContext transaction, PortalLogContext context)
        {
            Stopwatch clock = Stopwatch.StartNew(); bool succeeded = false;
            try { DataTable result = transaction.Select(query); succeeded = true; return result; }
            finally { PortalTechnicalLog.WriteSqlTrace(context, "SELECT", name, clock.ElapsedMilliseconds, succeeded); }
        }
        private static bool TraceWrite(SqlQuery query, string operation, string name, SqlTransactionContext transaction, PortalLogContext context)
        {
            Stopwatch clock = Stopwatch.StartNew(); bool succeeded = false;
            try { succeeded = transaction.Update(query); return succeeded; }
            finally { PortalTechnicalLog.WriteSqlTrace(context, operation, name, clock.ElapsedMilliseconds, succeeded); }
        }
    }
}
