using DAO.Ortak;
using System;
using System.Data;
using System.Diagnostics;
using Utility.Logging;

namespace DAO.Repositories.IKYS
{
    public class MaasHareketRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public MaasHareketRepository() : this(new DbClass()) { }
        public MaasHareketRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }
        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM MaasHareket_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }
        public DataTable SelectById(int id, SqlTransactionContext transaction, PortalLogContext context)
        { if (transaction == null) throw new ArgumentNullException("transaction"); SqlQuery query = new SqlQuery("SELECT * FROM MaasHareket_Table WITH (UPDLOCK, HOLDLOCK) WHERE Id=@Id"); query.AddParameter("@Id", id); return TraceSelect(query, "MaasHareket.SelectById", transaction, context); }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM MaasHareket_Table"), ""); }
        public int Insert<T>(T item) { return db.Insert(queryBuilder.BuildInsert(item, "MaasHareket_Table")); }
        public int Insert<T>(T item, SqlTransactionContext transaction, PortalLogContext context)
        { if (transaction == null) throw new ArgumentNullException("transaction"); Stopwatch clock = Stopwatch.StartNew(); bool succeeded = false; try { int id = transaction.Insert(queryBuilder.BuildInsert(item, "MaasHareket_Table")); succeeded = id > 0; return id; } finally { PortalTechnicalLog.WriteSqlTrace(context, "INSERT", "MaasHareket.Insert", clock.ElapsedMilliseconds, succeeded); } }
        public bool Update<T>(T item) { return db.Update2Db(queryBuilder.BuildUpdate(item, "MaasHareket_Table")); }
        public bool Update<T>(T item, SqlTransactionContext transaction, PortalLogContext context)
        { if (transaction == null) throw new ArgumentNullException("transaction"); return TraceWrite(queryBuilder.BuildUpdate(item, "MaasHareket_Table"), "UPDATE", "MaasHareket.Update", transaction, context); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("MaasHareket_Table", id), ""); }
        public bool Delete(int id, SqlTransactionContext transaction, PortalLogContext context)
        { if (transaction == null) throw new ArgumentNullException("transaction"); return TraceWrite(queryBuilder.BuildDelete("MaasHareket_Table", id), "DELETE", "MaasHareket.Delete", transaction, context); }
        public DataTable SelectByTarih(DateTime tarih)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM MaasHareket_Table WHERE Tarih=@Tarih ORDER BY Derece,Kademe");
            query.AddParameter("@Tarih", tarih);
            return db.SelectFromDb(query, "");
        }
        public DataTable SelectByTarih(DateTime tarih, PortalLogContext context)
        { SqlQuery query = new SqlQuery("SELECT * FROM MaasHareket_Table WHERE Tarih=@Tarih ORDER BY Derece,Kademe"); query.AddParameter("@Tarih", tarih); return TraceSelect(query, "MaasHareket.SelectByTarih", context); }
        public bool DeleteByGrupId(int grupId)
        {
            SqlQuery query = new SqlQuery("DELETE FROM MaasHareket_Table WHERE GrupId=@GrupId");
            query.AddParameter("@GrupId", grupId);
            return db.DeleteFromDb(query, "");
        }
        public bool DeleteByGrupId(int grupId, SqlTransactionContext transaction, PortalLogContext context)
        { if (transaction == null) throw new ArgumentNullException("transaction"); SqlQuery query = new SqlQuery("DELETE FROM MaasHareket_Table WHERE GrupId=@GrupId"); query.AddParameter("@GrupId", grupId); return TraceWrite(query, "DELETE", "MaasHareket.DeleteByGrupId", transaction, context); }
        private DataTable TraceSelect(SqlQuery query, string name, SqlTransactionContext transaction, PortalLogContext context)
        { Stopwatch clock = Stopwatch.StartNew(); bool succeeded = false; try { DataTable result = transaction.Select(query); succeeded = true; return result; } finally { PortalTechnicalLog.WriteSqlTrace(context, "SELECT", name, clock.ElapsedMilliseconds, succeeded); } }
        private DataTable TraceSelect(SqlQuery query, string name, PortalLogContext context)
        { Stopwatch clock = Stopwatch.StartNew(); bool succeeded = false; try { DataTable result = db.SelectFromDb(query, ""); succeeded = true; return result; } finally { PortalTechnicalLog.WriteSqlTrace(context, "SELECT", name, clock.ElapsedMilliseconds, succeeded); } }
        private static bool TraceWrite(SqlQuery query, string operation, string name, SqlTransactionContext transaction, PortalLogContext context)
        { Stopwatch clock = Stopwatch.StartNew(); bool succeeded = false; try { succeeded = transaction.Update(query); return succeeded; } finally { PortalTechnicalLog.WriteSqlTrace(context, operation, name, clock.ElapsedMilliseconds, succeeded); } }
    }
}
