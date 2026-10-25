using DAO.Ortak;
using System;
using System.Data;
using System.Diagnostics;
using Utility.Logging;

namespace DAO.Repositories.IKYS
{
    public class AileRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;
        public AileRepository() : this(new DbClass()) { }
        public AileRepository(DbClass db) { if (db == null) throw new ArgumentNullException("db"); this.db = db; queryBuilder = new CrudQueryBuilder(); }
        public DataTable SelectById(int id) { SqlQuery q = new SqlQuery("SELECT * FROM Aile_Table WHERE Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectById(int id, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            SqlQuery query = new SqlQuery("SELECT * FROM Aile_Table WITH (UPDLOCK, HOLDLOCK) WHERE Id=@Id");
            query.AddParameter("@Id", id);
            Stopwatch clock = Stopwatch.StartNew();
            bool succeeded = false;
            try { DataTable result = transaction.Select(query); succeeded = true; return result; }
            finally { transaction.DeferSqlTrace(context, "SELECT", "Aile.SelectById", clock.ElapsedMilliseconds, succeeded); }
        }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM Aile_Table"), ""); }

        public int Insert<T>(T item, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            Stopwatch clock = Stopwatch.StartNew();
            bool succeeded = false;
            try { int id = transaction.Insert(queryBuilder.BuildInsert(item, "Aile_Table")); succeeded = id > 0; return id; }
            finally { transaction.DeferSqlTrace(context, "INSERT", "Aile.Insert", clock.ElapsedMilliseconds, succeeded); }
        }

        public bool Update<T>(T item, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            return TraceWrite(queryBuilder.BuildUpdate(item, "Aile_Table"), "UPDATE", "Aile.Update", transaction, context);
        }

        public bool Delete(int id, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            return TraceWrite(queryBuilder.BuildDelete("Aile_Table", id), "DELETE", "Aile.Delete", transaction, context);
        }
        public DataTable SelectByPersonelId(int personelId) { SqlQuery q = new SqlQuery("SELECT * FROM Aile_Table WHERE PersonelId=@PersonelId ORDER BY YakinlikDerecesi"); q.AddParameter("@PersonelId", personelId); return db.SelectFromDb(q, ""); }
        public DataTable SelectByPersonelId(int personelId, PortalLogContext context)
        { SqlQuery q = new SqlQuery("SELECT * FROM Aile_Table WHERE PersonelId=@PersonelId ORDER BY YakinlikDerecesi"); q.AddParameter("@PersonelId", personelId); return PortalSqlReadExecutor.Select(q, context, "Aile.SelectByPersonelId"); }
        public DataTable SelectEnGencCocukByPersonelId(int personelId, int cocukYakinlikDerecesi) { SqlQuery q = new SqlQuery("SELECT * FROM Aile_Table WHERE PersonelId=@PersonelId AND YakinlikDerecesi=@YakinlikDerecesi ORDER BY DogumTar DESC"); q.AddParameter("@PersonelId", personelId); q.AddParameter("@YakinlikDerecesi", cocukYakinlikDerecesi); return db.SelectFromDb(q, ""); }
        private static bool TraceWrite(SqlQuery query, string operation, string name, SqlTransactionContext transaction, PortalLogContext context)
        {
            Stopwatch clock = Stopwatch.StartNew();
            bool succeeded = false;
            try { succeeded = transaction.Update(query); return succeeded; }
            finally { transaction.DeferSqlTrace(context, operation, name, clock.ElapsedMilliseconds, succeeded); }
        }

        public DataTable SelectById(int id, PortalLogContext context)
        { SqlQuery q = new SqlQuery("SELECT * FROM Aile_Table WHERE Id=@Id"); q.AddParameter("@Id", id); return PortalSqlReadExecutor.Select(q, context, "Aile.SelectById"); }

        public DataTable SelectAll(PortalLogContext context)
        { return PortalSqlReadExecutor.Select(new SqlQuery("SELECT * FROM Aile_Table"), context, "Aile.SelectAll"); }

        public DataTable SelectEnGencCocukByPersonelId(int personelId, int cocukYakinlikDerecesi, PortalLogContext context)
        { SqlQuery q = new SqlQuery("SELECT * FROM Aile_Table WHERE PersonelId=@PersonelId AND YakinlikDerecesi=@YakinlikDerecesi ORDER BY DogumTar DESC"); q.AddParameter("@PersonelId", personelId); q.AddParameter("@YakinlikDerecesi", cocukYakinlikDerecesi); return PortalSqlReadExecutor.Select(q, context, "Aile.SelectEnGencCocukByPersonelId"); }
    }
}
