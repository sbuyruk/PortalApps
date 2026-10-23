using DAO.Ortak;
using System;
using System.Data;
using System.Diagnostics;
using Utility.Logging;

namespace DAO.Repositories.IKYS
{
    public class GorevTanimRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public GorevTanimRepository() : this(new DbClass()) { }
        public GorevTanimRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM GorevTanim_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }
        public DataTable SelectById(int id, PortalLogContext context)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM GorevTanim_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            if (context != null && context.Project == "Portal")
                return PortalSqlReadExecutor.Select(query, context, "GorevTanimRepository.SelectById");
            return TraceSelect(query, "GorevTanim.SelectById", context);
        }
        public DataTable SelectById(int id, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            SqlQuery query = new SqlQuery("SELECT * FROM GorevTanim_Table WITH (UPDLOCK, HOLDLOCK) WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return TraceSelect(query, "GorevTanim.SelectById", transaction, context);
        }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM GorevTanim_Table"), ""); }
        public DataTable SelectAll(PortalLogContext context) { return TraceSelect(new SqlQuery("SELECT * FROM GorevTanim_Table"), "GorevTanim.SelectAll", context); }
        public int Insert<T>(T item) { return db.Insert(queryBuilder.BuildInsert(item, "GorevTanim_Table")); }
        public int Insert<T>(T item, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            Stopwatch clock = Stopwatch.StartNew();
            bool succeeded = false;
            try { int id = transaction.Insert(queryBuilder.BuildInsert(item, "GorevTanim_Table")); succeeded = id > 0; return id; }
            finally { PortalTechnicalLog.WriteSqlTrace(context, "INSERT", "GorevTanim.Insert", clock.ElapsedMilliseconds, succeeded); }
        }
        public bool Update<T>(T item) { return db.Update2Db(queryBuilder.BuildUpdate(item, "GorevTanim_Table")); }
        public bool Update<T>(T item, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            return TraceWrite(queryBuilder.BuildUpdate(item, "GorevTanim_Table"), "UPDATE", "GorevTanim.Update", transaction, context);
        }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("GorevTanim_Table", id), ""); }
        public bool Delete(int id, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            return TraceWrite(queryBuilder.BuildDelete("GorevTanim_Table", id), "DELETE", "GorevTanim.Delete", transaction, context);
        }

        public DataTable SelectByPersonelId(int personelId)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM GorevTanim_Table WHERE PersonelId=@PersonelId ORDER BY Id");
            query.AddParameter("@PersonelId", personelId);
            return db.SelectFromDb(query, "");
        }
        public DataTable SelectByPersonelId(int personelId, PortalLogContext context)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM GorevTanim_Table WHERE PersonelId=@PersonelId ORDER BY Id");
            query.AddParameter("@PersonelId", personelId);
            return TraceSelect(query, "GorevTanim.SelectByPersonelId", context);
        }
        public DataTable SelectByPersonelId(int personelId, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            SqlQuery query = new SqlQuery("SELECT * FROM GorevTanim_Table WITH (UPDLOCK, HOLDLOCK) WHERE PersonelId=@PersonelId ORDER BY Id");
            query.AddParameter("@PersonelId", personelId);
            return TraceSelect(query, "GorevTanim.SelectByPersonelId", transaction, context);
        }
        public DataTable SelectByBirimId(int birimId)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM GorevTanim_Table WHERE BirimId=@BirimId ORDER BY Id");
            query.AddParameter("@BirimId", birimId);
            return db.SelectFromDb(query, "");
        }
        public DataTable SelectAllReturnDataTable(int? personelTipi)
        {
            return db.SelectFromDb(CreateListQuery(personelTipi), "");
        }
        public DataTable SelectAllReturnDataTable(int? personelTipi, PortalLogContext context)
        {
            return TraceSelect(CreateListQuery(personelTipi), "GorevTanim.SelectList", context);
        }
        private static SqlQuery CreateListQuery(int? personelTipi)
        {
            string personelFilter = personelTipi.HasValue ? " AND B.Tipi=@Tipi" : "";
            string sql = @"
                SELECT A.Id GorevTanimId, A.Adi GorevAdi, A.KisaAdi GorevKisaAdi, C.Sira,
                       ISNULL(B.Adi,'') + ' ' + ISNULL(B.Soyadi,'') Personel,
                       C.Adi BirimAdi
                FROM GorevTanim_Table A
                LEFT JOIN Personel_Table B ON B.Id=A.PersonelId" + personelFilter + @"
                LEFT JOIN BirimTanim_Table C ON C.Id=A.BirimId
                WHERE A.Id>0";
            SqlQuery query = new SqlQuery();
            if (personelTipi.HasValue)
            {
                query.AddParameter("@Tipi", personelTipi.Value);
            }
            query.Sql = sql + " ORDER BY C.Sira,A.Id";
            return query;
        }
        private DataTable TraceSelect(SqlQuery query, string name, PortalLogContext context)
        {
            Stopwatch clock = Stopwatch.StartNew();
            bool succeeded = false;
            try { DataTable result = db.SelectFromDb(query, ""); succeeded = true; return result; }
            finally { PortalTechnicalLog.WriteSqlTrace(context, "SELECT", name, clock.ElapsedMilliseconds, succeeded); }
        }
        private static DataTable TraceSelect(SqlQuery query, string name, SqlTransactionContext transaction, PortalLogContext context)
        {
            Stopwatch clock = Stopwatch.StartNew();
            bool succeeded = false;
            try { DataTable result = transaction.Select(query); succeeded = true; return result; }
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
