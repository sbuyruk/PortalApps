using DAO.Ortak;
using System;
using System.Data;
using System.Diagnostics;
using Utility.Logging;

namespace DAO.Repositories.IKYS
{
    public class IzinTanimRepository
    {
        private readonly DbClass db; private readonly CrudQueryBuilder queryBuilder;
        public IzinTanimRepository() : this(new DbClass()) { }
        public IzinTanimRepository(DbClass db) { if (db == null) throw new ArgumentNullException("db"); this.db = db; queryBuilder = new CrudQueryBuilder(); }
        public DataTable SelectById(int id) { SqlQuery q = new SqlQuery("SELECT * FROM IzinTanim_Table WHERE Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM IzinTanim_Table"), ""); }
        public int Insert<T>(T item) { return db.Insert(queryBuilder.BuildInsert(item, "IzinTanim_Table")); }
        public bool Update<T>(T item) { return db.Update2Db(queryBuilder.BuildUpdate(item, "IzinTanim_Table")); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("IzinTanim_Table", id), ""); }
        public DataTable SelectById(int id, PortalLogContext context)
        { SqlQuery q = new SqlQuery("SELECT * FROM IzinTanim_Table WHERE Id=@Id"); q.AddParameter("@Id", id); return TraceSelect(q, "IzinTanim.SelectById", context); }

        public DataTable SelectAll(PortalLogContext context)
        { return TraceSelect(new SqlQuery("SELECT * FROM IzinTanim_Table"), "IzinTanim.SelectAll", context); }

        public DataTable SelectById(int id, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            SqlQuery query = new SqlQuery("SELECT * FROM IzinTanim_Table WITH (UPDLOCK, HOLDLOCK) WHERE Id=@Id");
            query.AddParameter("@Id", id);
            Stopwatch clock = Stopwatch.StartNew();
            bool succeeded = false;
            try { DataTable result = transaction.Select(query); succeeded = true; return result; }
            finally { PortalTechnicalLog.WriteSqlTrace(context, "SELECT", "IzinTanim.SelectById", clock.ElapsedMilliseconds, succeeded); }
        }

        public int Insert<T>(T item, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            Stopwatch clock = Stopwatch.StartNew();
            bool succeeded = false;
            try { int id = transaction.Insert(queryBuilder.BuildInsert(item, "IzinTanim_Table")); succeeded = id > 0; return id; }
            finally { PortalTechnicalLog.WriteSqlTrace(context, "INSERT", "IzinTanim.Insert", clock.ElapsedMilliseconds, succeeded); }
        }

        public bool Update<T>(T item, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            return TraceWrite(queryBuilder.BuildUpdate(item, "IzinTanim_Table"), "UPDATE", "IzinTanim.Update", transaction, context);
        }

        public bool Delete(int id, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            return TraceWrite(queryBuilder.BuildDelete("IzinTanim_Table", id), "DELETE", "IzinTanim.Delete", transaction, context);
        }

        private DataTable TraceSelect(SqlQuery query, string name, PortalLogContext context)
        {
            Stopwatch clock = Stopwatch.StartNew();
            bool succeeded = false;
            try { DataTable result = db.SelectFromDb(query, ""); succeeded = true; return result; }
            finally { PortalTechnicalLog.WriteSqlTrace(context, "SELECT", name, clock.ElapsedMilliseconds, succeeded); }
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
