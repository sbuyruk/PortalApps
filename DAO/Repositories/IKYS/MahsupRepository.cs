using DAO.Ortak;
using System;
using System.Data;
using System.Diagnostics;
using Utility.Logging;

namespace DAO.Repositories.IKYS
{
    public class MahsupRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public MahsupRepository() : this(new DbClass()) { }
        public MahsupRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM Mahsup_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }
        public DataTable SelectById(int id, SqlTransactionContext transaction, PortalLogContext context)
        { if (transaction == null) throw new ArgumentNullException("transaction"); SqlQuery query = new SqlQuery("SELECT * FROM Mahsup_Table WITH (UPDLOCK, HOLDLOCK) WHERE Id=@Id"); query.AddParameter("@Id", id); return TraceSelect(query, "Mahsup.SelectById", transaction, context); }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM Mahsup_Table"), ""); }

        public int Insert<T>(T item, SqlTransactionContext transaction, PortalLogContext context)
        { if (transaction == null) throw new ArgumentNullException("transaction"); Stopwatch clock = Stopwatch.StartNew(); bool succeeded = false; try { int id = transaction.Insert(queryBuilder.BuildInsert(item, "Mahsup_Table")); succeeded = id > 0; return id; } finally { PortalTechnicalLog.WriteSqlTrace(context, "INSERT", "Mahsup.Insert", clock.ElapsedMilliseconds, succeeded); } }

        public bool Update<T>(T item, SqlTransactionContext transaction, PortalLogContext context)
        { if (transaction == null) throw new ArgumentNullException("transaction"); return TraceWrite(queryBuilder.BuildUpdate(item, "Mahsup_Table"), "UPDATE", "Mahsup.Update", transaction, context); }

        public bool Delete(int id, SqlTransactionContext transaction, PortalLogContext context)
        { if (transaction == null) throw new ArgumentNullException("transaction"); return TraceWrite(queryBuilder.BuildDelete("Mahsup_Table", id), "DELETE", "Mahsup.Delete", transaction, context); }

        public DataTable SelectByDonemId(int donemId)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM Mahsup_Table WHERE KullanildigiDonemId=@DonemId OR MahsupDonemId=@DonemId");
            query.AddParameter("@DonemId", donemId);
            return db.SelectFromDb(query, "");
        }
        public DataTable SelectByDonemId(int donemId, PortalLogContext context)
        { SqlQuery query = new SqlQuery("SELECT * FROM Mahsup_Table WHERE KullanildigiDonemId=@DonemId OR MahsupDonemId=@DonemId"); query.AddParameter("@DonemId", donemId); return TraceSelect(query, "Mahsup.SelectByDonemId", context); }

        public DataTable SelectByPersonelId(int personelId, int izinTipi, int mazeretIzinTipi)
        {
            string typeFilter = "";
            SqlQuery query = new SqlQuery();
            if (izinTipi != 0)
            {
                if (izinTipi == mazeretIzinTipi)
                {
                    typeFilter = " AND IzinTipi=@IzinTipi";
                    query.AddParameter("@IzinTipi", izinTipi);
                }
                else
                {
                    typeFilter = " AND IzinTipi<>@MazeretIzinTipi";
                    query.AddParameter("@MazeretIzinTipi", mazeretIzinTipi);
                }
            }
            query.Sql = "SELECT * FROM Mahsup_Table WHERE PersonelId=@PersonelId" + typeFilter;
            query.AddParameter("@PersonelId", personelId);
            return db.SelectFromDb(query, "");
        }
        public DataTable SelectByPersonelId(int personelId, int izinTipi, int mazeretIzinTipi, PortalLogContext context)
        { string typeFilter = ""; SqlQuery query = new SqlQuery(); if (izinTipi != 0) { if (izinTipi == mazeretIzinTipi) { typeFilter = " AND IzinTipi=@IzinTipi"; query.AddParameter("@IzinTipi", izinTipi); } else { typeFilter = " AND IzinTipi<>@MazeretIzinTipi"; query.AddParameter("@MazeretIzinTipi", mazeretIzinTipi); } } query.Sql = "SELECT * FROM Mahsup_Table WHERE PersonelId=@PersonelId" + typeFilter; query.AddParameter("@PersonelId", personelId); return TraceSelect(query, "Mahsup.SelectByPersonelId", context); }
        private static DataTable TraceSelect(SqlQuery query, string name, SqlTransactionContext transaction, PortalLogContext context)
        { Stopwatch clock = Stopwatch.StartNew(); bool succeeded = false; try { DataTable result = transaction.Select(query); succeeded = true; return result; } finally { PortalTechnicalLog.WriteSqlTrace(context, "SELECT", name, clock.ElapsedMilliseconds, succeeded); } }
        private DataTable TraceSelect(SqlQuery query, string name, PortalLogContext context)
        {
            return PortalSqlReadExecutor.Select(query, context, name);
        }
        private static bool TraceWrite(SqlQuery query, string operation, string name, SqlTransactionContext transaction, PortalLogContext context)
        { Stopwatch clock = Stopwatch.StartNew(); bool succeeded = false; try { succeeded = transaction.Update(query); return succeeded; } finally { PortalTechnicalLog.WriteSqlTrace(context, operation, name, clock.ElapsedMilliseconds, succeeded); } }

        public DataTable SelectById(int id, PortalLogContext context)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM Mahsup_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return PortalSqlReadExecutor.Select(query, context, "Mahsup.SelectById");
        }

        public DataTable SelectAll(PortalLogContext context)
        { return PortalSqlReadExecutor.Select(new SqlQuery("SELECT * FROM Mahsup_Table"), context, "Mahsup.SelectAll"); }
    }
}
