using DAO.Ortak;
using System;
using System.Data;
using System.Diagnostics;
using Utility.Logging;

namespace DAO.Repositories.IKYS
{
    public class ResmiTatilRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;
        public ResmiTatilRepository() : this(new DbClass()) { }
        public ResmiTatilRepository(DbClass db) { if (db == null) throw new ArgumentNullException("db"); this.db = db; queryBuilder = new CrudQueryBuilder(); }
        public DataTable SelectById(int id) { SqlQuery q = new SqlQuery("SELECT * FROM ResmiTatil_Table WHERE Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectById(int id, PortalLogContext context)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM ResmiTatil_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return TraceSelect(query, "ResmiTatil.SelectById", context);
        }
        public DataTable SelectById(int id, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            SqlQuery query = new SqlQuery("SELECT * FROM ResmiTatil_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return TraceTransactionSelect(query, "ResmiTatil.SelectById", transaction, context);
        }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM ResmiTatil_Table"), ""); }
        public DataTable SelectAllReturnDataTable() { return db.SelectFromDb(new SqlQuery("SELECT Id ResimiTatilId,Gun,Ay,Yil,Tatil,BaslamaTarihi,BitisTarihi,IlanTarihi,IptalTarihi FROM ResmiTatil_Table ORDER BY BaslamaTarihi DESC"), ""); }
        public DataTable SelectAllReturnDataTable(PortalLogContext context)
        {
            return TraceSelect(new SqlQuery("SELECT Id ResimiTatilId,Gun,Ay,Yil,Tatil,BaslamaTarihi,BitisTarihi,IlanTarihi,IptalTarihi FROM ResmiTatil_Table ORDER BY BaslamaTarihi DESC"), "ResmiTatil.SelectAll", context);
        }
        public int Insert<T>(T item) { return db.Insert(queryBuilder.BuildInsert(item, "ResmiTatil_Table")); }
        public int Insert<T>(T item, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            return TraceInsert(queryBuilder.BuildInsert(item, "ResmiTatil_Table"), "ResmiTatil.Insert", transaction, context);
        }
        public bool Update<T>(T item) { return db.Update2Db(queryBuilder.BuildUpdate(item, "ResmiTatil_Table")); }
        public bool Update<T>(T item, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            return TraceWrite(queryBuilder.BuildUpdate(item, "ResmiTatil_Table"), "UPDATE", "ResmiTatil.Update", transaction, context);
        }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("ResmiTatil_Table", id), ""); }
        public bool Delete(int id, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            return TraceWrite(queryBuilder.BuildDelete("ResmiTatil_Table", id), "DELETE", "ResmiTatil.Delete", transaction, context);
        }
        public DataTable SelectByTarih(DateTime baslangic, DateTime bitis) { SqlQuery q = new SqlQuery("SELECT * FROM ResmiTatil_Table WHERE Yil=0 OR BaslamaTarihi BETWEEN @Baslangic AND @Bitis ORDER BY BaslamaTarihi"); q.AddParameter("@Baslangic", baslangic); q.AddParameter("@Bitis", bitis); return db.SelectFromDb(q, ""); }
        public DataTable SelectByTarih(DateTime baslangic, DateTime bitis, PortalLogContext context)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM ResmiTatil_Table WHERE Yil=0 OR BaslamaTarihi BETWEEN @Baslangic AND @Bitis ORDER BY BaslamaTarihi");
            query.AddParameter("@Baslangic", baslangic);
            query.AddParameter("@Bitis", bitis);
            return TraceSelect(query, "ResmiTatil.SelectByTarih", context);
        }
        public DataTable SelectBySonIkiYil(DateTime baslangic, DateTime bitis) { SqlQuery q = new SqlQuery("SELECT * FROM ResmiTatil_Table WHERE Yil=0 OR BaslamaTarihi BETWEEN @Baslangic AND @Bitis ORDER BY MONTH(BaslamaTarihi),DAY(BaslamaTarihi)"); q.AddParameter("@Baslangic", baslangic); q.AddParameter("@Bitis", bitis); return db.SelectFromDb(q, ""); }
        public DataTable SelectByYil(int yil) { SqlQuery q = new SqlQuery("SELECT * FROM ResmiTatil_Table WHERE Yil=0 OR Yil=@Yil ORDER BY MONTH(BaslamaTarihi),DAY(BaslamaTarihi)"); q.AddParameter("@Yil", yil); return db.SelectFromDb(q, ""); }
        public DataTable SelectByDate(DateTime tarih)
        {
            SqlQuery q = new SqlQuery("SELECT * FROM ResmiTatil_Table WHERE ((DAY(BaslamaTarihi)<=@Gun AND DAY(BitisTarihi)>=@Gun AND Ay=@Ay AND Yil=0) AND (IlanTarihi IS NULL OR IlanTarihi<=@Tarih) AND (IptalTarihi IS NULL OR IptalTarihi>=@Tarih)) OR (BaslamaTarihi<=@Tarih AND BitisTarihi>=@Tarih)");
            q.AddParameter("@Gun", tarih.Day); q.AddParameter("@Ay", tarih.Month); q.AddParameter("@Tarih", tarih); return db.SelectFromDb(q, "");
        }

        private DataTable TraceSelect(SqlQuery query, string name, PortalLogContext context)
        {
            Stopwatch clock = Stopwatch.StartNew();
            bool succeeded = false;
            try { DataTable result = db.SelectFromDb(query, ""); succeeded = true; return result; }
            finally { PortalTechnicalLog.WriteSqlTrace(context, "SELECT", name, clock.ElapsedMilliseconds, succeeded); }
        }

        private static DataTable TraceTransactionSelect(SqlQuery query, string name,
            SqlTransactionContext transaction, PortalLogContext context)
        {
            Stopwatch clock = Stopwatch.StartNew();
            bool succeeded = false;
            try { DataTable result = transaction.Select(query); succeeded = true; return result; }
            finally { PortalTechnicalLog.WriteSqlTrace(context, "SELECT", name, clock.ElapsedMilliseconds, succeeded); }
        }

        private static int TraceInsert(SqlQuery query, string name,
            SqlTransactionContext transaction, PortalLogContext context)
        {
            Stopwatch clock = Stopwatch.StartNew();
            bool succeeded = false;
            try { int id = transaction.Insert(query); succeeded = id > 0; return id; }
            finally { PortalTechnicalLog.WriteSqlTrace(context, "INSERT", name, clock.ElapsedMilliseconds, succeeded); }
        }

        private static bool TraceWrite(SqlQuery query, string operation, string name,
            SqlTransactionContext transaction, PortalLogContext context)
        {
            Stopwatch clock = Stopwatch.StartNew();
            bool succeeded = false;
            try { succeeded = transaction.Update(query); return succeeded; }
            finally { PortalTechnicalLog.WriteSqlTrace(context, operation, name, clock.ElapsedMilliseconds, succeeded); }
        }
    }
}
