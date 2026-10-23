using Utility.Logging;
using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.Ortak
{
    public class IlRepository
    {
        private readonly DbClass db;
        private readonly PortalLogContext logContext;

        public IlRepository() : this(new DbClass())
        {
        }

        public IlRepository(PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            logContext = context;
        }

        public IlRepository(DbClass db)
        {
            if (db == null)
                throw new ArgumentNullException("db");
            this.db = db;
        }

        public IlRepository(SqlTransactionContext transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM Il_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectById(int id, PortalLogContext context)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM Il_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return PortalSqlReadExecutor.Select(query, context, "IlRepository.SelectById");
        }

        public DataTable SelectById(int id, SqlTransactionContext transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            SqlQuery query = new SqlQuery("SELECT * FROM Il_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return SelectInTransaction(query, transaction);
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery("SELECT * FROM Il_Table"), "");
        }

        public DataTable SelectAll(PortalLogContext context)
        {
            return PortalSqlReadExecutor.Select(new SqlQuery("SELECT * FROM Il_Table"), context, "IlRepository.SelectAll");
        }

        public DataTable SelectAllOrderByRegion()
        {
            return db.SelectFromDb(new SqlQuery(
                "SELECT * FROM Il_Table ORDER BY Bolge, IlAdi"), "");
        }

        public DataTable SelectByName(string name)
        {
            SqlQuery query = new SqlQuery(
                "SELECT * FROM Il_Table WHERE LOWER(IlAdi)=LOWER(@IlAdi)");
            query.AddParameter("@IlAdi", name);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByName(string name, PortalLogContext context)
        {
            SqlQuery query = new SqlQuery(
                "SELECT * FROM Il_Table WHERE LOWER(IlAdi)=LOWER(@IlAdi)");
            query.AddParameter("@IlAdi", name);
            return PortalSqlReadExecutor.Select(query, context, "IlRepository.SelectByName");
        }

        public DataTable SelectByName(string name, SqlTransactionContext transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");
            SqlQuery query = new SqlQuery(
                "SELECT * FROM Il_Table WHERE LOWER(IlAdi)=LOWER(@IlAdi)");
            query.AddParameter("@IlAdi", name);
            return SelectInTransaction(query, transaction);
        }

        public DataTable SelectByEnglishName(string name)
        {
            SqlQuery query = new SqlQuery(
                "SELECT * FROM Il_Table WHERE LOWER(IngIlAdi)=LOWER(@IngIlAdi)");
            query.AddParameter("@IngIlAdi", name);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByEnglishName(string name, PortalLogContext context)
        {
            SqlQuery query = new SqlQuery(
                "SELECT * FROM Il_Table WHERE LOWER(IngIlAdi)=LOWER(@IngIlAdi)");
            query.AddParameter("@IngIlAdi", name);
            return PortalSqlReadExecutor.Select(query, context, "IlRepository.SelectByEnglishName");
        }

        public DataTable SelectByRegion(int regionId, bool orderByName)
        {
            bool all = regionId == Utility.ProjeGlobal.ProjeConstants.HEPSI_INT ||
                regionId == Utility.ProjeGlobal.ProjeConstants.BOLGE_GENELMUDURLUK_INT;
            SqlQuery query = new SqlQuery(
                "SELECT * FROM Il_Table WHERE Id BETWEEN 1 AND 81" +
                (all ? "" : " AND BolgeId=@BolgeId") +
                (orderByName ? " ORDER BY IlAdi" : ""));
            if (!all)
                query.AddParameter("@BolgeId", regionId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByRegion(int regionId, bool orderByName, PortalLogContext context)
        {
            bool all = regionId == Utility.ProjeGlobal.ProjeConstants.HEPSI_INT ||
                regionId == Utility.ProjeGlobal.ProjeConstants.BOLGE_GENELMUDURLUK_INT;
            SqlQuery query = new SqlQuery(
                "SELECT * FROM Il_Table WHERE Id BETWEEN 1 AND 81" +
                (all ? "" : " AND BolgeId=@BolgeId") +
                (orderByName ? " ORDER BY IlAdi" : ""));
            if (!all)
                query.AddParameter("@BolgeId", regionId);
            return PortalSqlReadExecutor.Select(query, context, "IlRepository.SelectByRegion");
        }

        public DataTable SelectCountByRegion(int regionId)
        {
            bool all = regionId == Utility.ProjeGlobal.ProjeConstants.HEPSI_INT ||
                regionId == Utility.ProjeGlobal.ProjeConstants.BOLGE_GENELMUDURLUK_INT;
            SqlQuery query = new SqlQuery(
                "SELECT COUNT(Id) Adet FROM Il_Table WHERE Id BETWEEN 1 AND 81" +
                (all ? "" : " AND BolgeId=@BolgeId"));
            if (!all)
                query.AddParameter("@BolgeId", regionId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectCountByRegion(int regionId, PortalLogContext context)
        {
            bool all = regionId == Utility.ProjeGlobal.ProjeConstants.HEPSI_INT ||
                regionId == Utility.ProjeGlobal.ProjeConstants.BOLGE_GENELMUDURLUK_INT;
            SqlQuery query = new SqlQuery(
                "SELECT COUNT(Id) Adet FROM Il_Table WHERE Id BETWEEN 1 AND 81" +
                (all ? "" : " AND BolgeId=@BolgeId"));
            if (!all)
                query.AddParameter("@BolgeId", regionId);
            return PortalSqlReadExecutor.Select(query, context, "IlRepository.SelectCountByRegion");
        }

        public DataTable SelectWithFTK()
        {
            return db.SelectFromDb(new SqlQuery(
                "SELECT A.* FROM Il_Table A " +
                "WHERE A.Id BETWEEN 0 AND 81 AND A.IlAdi != '' " +
                "AND A.Id IN (SELECT Ili FROM FTK_Table) ORDER BY A.IlAdi"), "");
        }

        public DataTable SelectFTKRegions()
        {
            return db.SelectFromDb(new SqlQuery(
                "SELECT A.Bolge FROM Il_Table A " +
                "WHERE A.Id BETWEEN 0 AND 81 AND A.IlAdi != '' " +
                "AND A.Id IN (SELECT Ili FROM FTK_Table) " +
                "GROUP BY A.Bolge ORDER BY A.Bolge"), "");
        }
        private DataTable SelectInTransaction(SqlQuery query, SqlTransactionContext transaction,
            [System.Runtime.CompilerServices.CallerMemberName] string method = "")
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            return logContext == null ? transaction.Select(query) : PortalSqlReadExecutor.Select(query, logContext, "IlRepository." + method, transaction);
        }
    }
}
