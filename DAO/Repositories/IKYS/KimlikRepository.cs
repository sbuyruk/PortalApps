using DAO.Ortak;
using System;
using System.Data;
using System.Diagnostics;
using Utility.Logging;

namespace DAO.Repositories.IKYS
{
    public class KimlikRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;
        public KimlikRepository() : this(new DbClass()) { }
        public KimlikRepository(DbClass db) { if (db == null) throw new ArgumentNullException("db"); this.db = db; queryBuilder = new CrudQueryBuilder(); }
        public DataTable SelectById(int id) { SqlQuery q = new SqlQuery("SELECT * FROM Kimlik_Table WHERE Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectById(int id, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            SqlQuery query = new SqlQuery("SELECT * FROM Kimlik_Table WITH (UPDLOCK, HOLDLOCK) WHERE Id=@Id"); query.AddParameter("@Id", id);
            Stopwatch clock = Stopwatch.StartNew(); bool succeeded = false;
            try { DataTable result = transaction.Select(query); succeeded = true; return result; }
            finally { PortalTechnicalLog.WriteSqlTrace(context, "SELECT", "Kimlik.SelectById", clock.ElapsedMilliseconds, succeeded); }
        }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM Kimlik_Table"), ""); }
        public int Insert<T>(T item) { return db.Insert(queryBuilder.BuildInsert(item, "Kimlik_Table")); }
        public int Insert<T>(T item, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            Stopwatch clock = Stopwatch.StartNew(); bool succeeded = false;
            try { int id = transaction.Insert(queryBuilder.BuildInsert(item, "Kimlik_Table")); succeeded = id > 0; return id; }
            finally { PortalTechnicalLog.WriteSqlTrace(context, "INSERT", "Kimlik.Insert", clock.ElapsedMilliseconds, succeeded); }
        }
        public bool Update<T>(T item) { return db.Update2Db(queryBuilder.BuildUpdate(item, "Kimlik_Table")); }
        public bool Update<T>(T item, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            Stopwatch clock = Stopwatch.StartNew(); bool succeeded = false;
            try { succeeded = transaction.Update(queryBuilder.BuildUpdate(item, "Kimlik_Table")); return succeeded; }
            finally { PortalTechnicalLog.WriteSqlTrace(context, "UPDATE", "Kimlik.Update", clock.ElapsedMilliseconds, succeeded); }
        }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("Kimlik_Table", id), ""); }
        public DataTable SelectByPersonelId(int personelId) { SqlQuery q = new SqlQuery("SELECT * FROM Kimlik_Table WHERE PersonelId=@PersonelId ORDER BY Id"); q.AddParameter("@PersonelId", personelId); return db.SelectFromDb(q, ""); }
        public DataTable SelectByPersonelId(int personelId, PortalLogContext context)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM Kimlik_Table WHERE PersonelId=@PersonelId ORDER BY Id");
            query.AddParameter("@PersonelId", personelId);
            if (context != null && context.Project == "Portal")
                return PortalSqlReadExecutor.Select(query, context, "KimlikRepository.SelectByPersonelId");
            Stopwatch clock = Stopwatch.StartNew();
            bool succeeded = false;
            try { DataTable result = db.SelectFromDb(query, ""); succeeded = true; return result; }
            finally { PortalTechnicalLog.WriteSqlTrace(context, "SELECT", "Kimlik.SelectByPersonelId", clock.ElapsedMilliseconds, succeeded); }
        }
        public DataTable SelectByPersonelId(int personelId, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            SqlQuery query = new SqlQuery("SELECT * FROM Kimlik_Table WITH (UPDLOCK, HOLDLOCK) WHERE PersonelId=@PersonelId ORDER BY Id");
            query.AddParameter("@PersonelId", personelId);
            Stopwatch clock = Stopwatch.StartNew(); bool succeeded = false;
            try { DataTable result = transaction.Select(query); succeeded = true; return result; }
            finally { PortalTechnicalLog.WriteSqlTrace(context, "SELECT", "Kimlik.SelectByPersonelId", clock.ElapsedMilliseconds, succeeded); }
        }
        public DataTable SelectByTcKimlikNo(string kimlikNo) { SqlQuery q = new SqlQuery("SELECT * FROM Kimlik_Table WHERE TCKimlikNo=@TCKimlikNo ORDER BY Id"); q.AddParameter("@TCKimlikNo", kimlikNo); return db.SelectFromDb(q, ""); }
        public DataTable SelectByTcKimlikNo(string kimlikNo, PortalLogContext context)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM Kimlik_Table WHERE TCKimlikNo=@TCKimlikNo ORDER BY Id");
            query.AddParameter("@TCKimlikNo", kimlikNo);
            Stopwatch clock = Stopwatch.StartNew(); bool succeeded = false;
            try { DataTable result = db.SelectFromDb(query, ""); succeeded = true; return result; }
            finally { PortalTechnicalLog.WriteSqlTrace(context, "SELECT", "Kimlik.SelectByTcKimlikNo", clock.ElapsedMilliseconds, succeeded); }
        }
        public DataTable SelectAllFromKimlik() { return db.SelectFromDb(new SqlQuery("SELECT * FROM KIMLIK_BILGILERI"), ""); }
    }
}
