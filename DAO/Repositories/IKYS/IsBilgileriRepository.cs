using DAO.Ortak;
using System;
using System.Data;
using System.Diagnostics;
using Utility.Logging;

namespace DAO.Repositories.IKYS
{
    public class IsBilgileriRepository
    {
        private readonly DbClass db; private readonly CrudQueryBuilder queryBuilder;
        public IsBilgileriRepository() : this(new DbClass()) { }
        public IsBilgileriRepository(DbClass db) { if (db == null) throw new ArgumentNullException("db"); this.db = db; queryBuilder = new CrudQueryBuilder(); }
        public DataTable SelectById(int id) { SqlQuery q = new SqlQuery("SELECT * FROM IsBilgileri_Table WHERE Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectById(int id, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            SqlQuery query = new SqlQuery("SELECT * FROM IsBilgileri_Table WITH (UPDLOCK, HOLDLOCK) WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return TraceSelect(query, "IsBilgileri.SelectById", transaction, context);
        }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM IsBilgileri_Table"), ""); }
        public int Insert<T>(T item) { return db.Insert(queryBuilder.BuildInsert(item, "IsBilgileri_Table")); }
        public int Insert<T>(T item, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            Stopwatch clock = Stopwatch.StartNew(); bool succeeded = false;
            try { int id = transaction.Insert(queryBuilder.BuildInsert(item, "IsBilgileri_Table")); succeeded = id > 0; return id; }
            finally { PortalTechnicalLog.WriteSqlTrace(context, "INSERT", "IsBilgileri.Insert", clock.ElapsedMilliseconds, succeeded); }
        }
        public bool Update<T>(T item) { return db.Update2Db(queryBuilder.BuildUpdate(item, "IsBilgileri_Table")); }
        public bool Update<T>(T item, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            return TraceWrite(queryBuilder.BuildUpdate(item, "IsBilgileri_Table"), "UPDATE", "IsBilgileri.Update", transaction, context);
        }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("IsBilgileri_Table", id), ""); }
        public bool Delete(int id, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            return TraceWrite(queryBuilder.BuildDelete("IsBilgileri_Table", id), "DELETE", "IsBilgileri.Delete", transaction, context);
        }
        public DataTable SelectByPersonelId(int personelId) { SqlQuery q = new SqlQuery("SELECT * FROM IsBilgileri_Table WHERE PersonelId=@PersonelId ORDER BY UnvanId"); q.AddParameter("@PersonelId", personelId); return db.SelectFromDb(q, ""); }
        public DataTable SelectByPersonelId(int personelId, PortalLogContext context)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM IsBilgileri_Table WHERE PersonelId=@PersonelId ORDER BY UnvanId");
            query.AddParameter("@PersonelId", personelId);
            return PortalSqlReadExecutor.Select(query, context, "IsBilgileri.SelectByPersonelId");
        }
        public DataTable SelectByPersonelId(int personelId, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            SqlQuery query = new SqlQuery("SELECT * FROM IsBilgileri_Table WITH (UPDLOCK, HOLDLOCK) WHERE PersonelId=@PersonelId ORDER BY UnvanId");
            query.AddParameter("@PersonelId", personelId);
            return TraceSelect(query, "IsBilgileri.SelectByPersonelId", transaction, context);
        }
        public DataTable SelectByGorevId(int gorevId) { SqlQuery q = new SqlQuery("SELECT * FROM IsBilgileri_Table WHERE GorevId=@GorevId ORDER BY UnvanId"); q.AddParameter("@GorevId", gorevId); return db.SelectFromDb(q, ""); }
        public DataTable SelectByGorevId(int gorevId, PortalLogContext context)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM IsBilgileri_Table WHERE GorevId=@GorevId ORDER BY UnvanId");
            query.AddParameter("@GorevId", gorevId);
            Stopwatch clock = Stopwatch.StartNew();
            bool succeeded = false;
            try { DataTable result = db.SelectFromDb(query, ""); succeeded = true; return result; }
            finally { PortalTechnicalLog.WriteSqlTrace(context, "SELECT", "IsBilgileri.SelectByGorevId", clock.ElapsedMilliseconds, succeeded); }
        }
        public DataTable SelectByGorevId(int gorevId, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            SqlQuery query = new SqlQuery("SELECT * FROM IsBilgileri_Table WITH (UPDLOCK, HOLDLOCK) WHERE GorevId=@GorevId ORDER BY UnvanId");
            query.AddParameter("@GorevId", gorevId);
            return TraceSelect(query, "IsBilgileri.SelectByGorevId", transaction, context);
        }
        public bool UpdateGorevId(int id, int gorevId, string degistiren, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            SqlQuery query = new SqlQuery("UPDATE IsBilgileri_Table SET GorevId=@GorevId, DegistirmeTarihi=@DegistirmeTarihi, Degistiren=@Degistiren WHERE Id=@Id");
            query.AddParameter("@Id", id);
            query.AddParameter("@GorevId", gorevId);
            query.AddParameter("@DegistirmeTarihi", DateTime.Now);
            query.AddParameter("@Degistiren", degistiren);
            Stopwatch clock = Stopwatch.StartNew();
            bool succeeded = false;
            try { succeeded = transaction.Update(query); return succeeded; }
            finally { PortalTechnicalLog.WriteSqlTrace(context, "UPDATE", "IsBilgileri.UpdateGorevId", clock.ElapsedMilliseconds, succeeded); }
        }
        public DataTable SelectAllFromIsYeriBilgileri() { return db.SelectFromDb(new SqlQuery("SELECT * FROM IS_YERI_BILGILERI"), ""); }
        private static DataTable TraceSelect(SqlQuery query, string name, SqlTransactionContext transaction, PortalLogContext context)
        {
            Stopwatch clock = Stopwatch.StartNew();
            bool succeeded = false;
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
