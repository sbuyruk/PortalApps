using DAO.Ortak;
using System;
using System.Data;
using System.Diagnostics;
using Utility.Logging;

namespace DAO.Repositories.IKYS
{
    public class BirimTanimRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;
        public BirimTanimRepository() : this(new DbClass()) { }
        public BirimTanimRepository(DbClass db) { if (db == null) throw new ArgumentNullException("db"); this.db = db; queryBuilder = new CrudQueryBuilder(); }
        public DataTable SelectById(int id) { SqlQuery q = new SqlQuery("SELECT * FROM BirimTanim_Table WHERE Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectById(int id, PortalLogContext context)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM BirimTanim_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return TraceSelect(query, "BirimTanim.SelectById", context);
        }
        public DataTable SelectById(int id, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            SqlQuery query = new SqlQuery("SELECT * FROM BirimTanim_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            Stopwatch clock = Stopwatch.StartNew();
            bool succeeded = false;
            try { DataTable result = transaction.Select(query); succeeded = true; return result; }
            finally { PortalTechnicalLog.WriteSqlTrace(context, "SELECT", "BirimTanim.SelectById", clock.ElapsedMilliseconds, succeeded); }
        }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM BirimTanim_Table ORDER BY ParentId,Sira"), ""); }
        public DataTable SelectAll(PortalLogContext context) { return TraceSelect(new SqlQuery("SELECT * FROM BirimTanim_Table ORDER BY ParentId,Sira"), "BirimTanim.SelectAll", context); }
        public int Insert<T>(T item) { return db.Insert(queryBuilder.BuildInsert(item, "BirimTanim_Table")); }
        public int Insert<T>(T item, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            Stopwatch clock = Stopwatch.StartNew();
            bool succeeded = false;
            try { int id = transaction.Insert(queryBuilder.BuildInsert(item, "BirimTanim_Table")); succeeded = id > 0; return id; }
            finally { PortalTechnicalLog.WriteSqlTrace(context, "INSERT", "BirimTanim.Insert", clock.ElapsedMilliseconds, succeeded); }
        }
        public bool Update<T>(T item) { return db.Update2Db(queryBuilder.BuildUpdate(item, "BirimTanim_Table")); }
        public bool Update<T>(T item, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            return TraceWrite(queryBuilder.BuildUpdate(item, "BirimTanim_Table"), "UPDATE", "BirimTanim.Update", transaction, context);
        }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("BirimTanim_Table", id), ""); }
        public bool Delete(int id, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            return TraceWrite(queryBuilder.BuildDelete("BirimTanim_Table", id), "DELETE", "BirimTanim.Delete", transaction, context);
        }
        public DataTable SelectByAmirId(int amirId) { SqlQuery q = new SqlQuery("SELECT * FROM BirimTanim_Table WHERE AmirId=@AmirId ORDER BY ParentId,Sira"); q.AddParameter("@AmirId", amirId); return db.SelectFromDb(q, ""); }
        public DataTable SelectByBirimKaldirildi(bool kaldirildi) { SqlQuery q = new SqlQuery("SELECT * FROM BirimTanim_Table WHERE BirimKaldirildi=@Kaldirildi ORDER BY ParentId,Sira"); q.AddParameter("@Kaldirildi", kaldirildi); return db.SelectFromDb(q, ""); }
        public DataTable SelectByBirimKaldirildi(bool kaldirildi, PortalLogContext context)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM BirimTanim_Table WHERE BirimKaldirildi=@Kaldirildi ORDER BY ParentId,Sira");
            query.AddParameter("@Kaldirildi", kaldirildi);
            return TraceSelect(query, "BirimTanim.SelectByBirimKaldirildi", context);
        }
        public DataTable SelectAllReturnDataTable() { return db.SelectFromDb(new SqlQuery("SELECT A.Id BirimId,A.Adi BirimAdi,A.KisaAdi BirimKisaAdi,A.Sira,B.Adi UstBirim,ISNULL(C.Adi,'')+' '+ISNULL(C.Soyadi,'') BirimAmiri FROM BirimTanim_Table A LEFT JOIN BirimTanim_Table B ON B.Id=A.ParentId LEFT JOIN Personel_Table C ON C.Id=A.AmirId ORDER BY A.Sira"), ""); }
        public DataTable SelectAllReturnDataTable(PortalLogContext context)
        {
            return TraceSelect(new SqlQuery("SELECT A.Id BirimId,A.Adi BirimAdi,A.KisaAdi BirimKisaAdi,A.Sira,B.Adi UstBirim,ISNULL(C.Adi,'')+' '+ISNULL(C.Soyadi,'') BirimAmiri FROM BirimTanim_Table A LEFT JOIN BirimTanim_Table B ON B.Id=A.ParentId LEFT JOIN Personel_Table C ON C.Id=A.AmirId ORDER BY A.Sira"), "BirimTanim.SelectList", context);
        }
        public DataTable SelectByParentId(int parentId) { SqlQuery q = new SqlQuery("SELECT * FROM BirimTanim_Table WHERE ParentId=@ParentId ORDER BY Sira"); q.AddParameter("@ParentId", parentId); return db.SelectFromDb(q, ""); }
        public DataTable SelectRoot() { return db.SelectFromDb(new SqlQuery("SELECT * FROM BirimTanim_Table WHERE ParentId=0 ORDER BY Sira"), ""); }

        private DataTable TraceSelect(SqlQuery query, string name, PortalLogContext context)
        {
            Stopwatch clock = Stopwatch.StartNew();
            bool succeeded = false;
            try { DataTable result = db.SelectFromDb(query, ""); succeeded = true; return result; }
            finally { PortalTechnicalLog.WriteSqlTrace(context, "SELECT", name, clock.ElapsedMilliseconds, succeeded); }
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
