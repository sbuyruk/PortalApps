using DAO.Ortak;
using System;
using System.Data;
using System.Diagnostics;
using Utility.Logging;

namespace DAO.Repositories.Ortak
{
    public class IlceRepository
    {
        private readonly DbClass db;
        private readonly PortalLogContext logContext;

        public IlceRepository() : this(new DbClass())
        {
        }

        public IlceRepository(SqlTransactionContext transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");

        }

        public IlceRepository(PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            logContext = context;
        }

        public IlceRepository(DbClass db)
        {
            if (db == null)
                throw new ArgumentNullException("db");
            this.db = db;
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery(
                "SELECT * FROM Ilce_Table WHERE Id=@Id ORDER BY IlceAdi");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectById(int id, PortalLogContext context)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM Ilce_Table WHERE Id=@Id ORDER BY IlceAdi");
            query.AddParameter("@Id", id);
            if (context != null && context.Project == "NBYS")
                return PortalSqlReadExecutor.Select(query, context, "IlceRepository.SelectById");
            Stopwatch clock = Stopwatch.StartNew();
            bool succeeded = false;
            try { DataTable result = db.SelectFromDb(query, ""); succeeded = true; return result; }
            finally { PortalTechnicalLog.WriteSqlTrace(context, "SELECT", "Ilce.SelectById", clock.ElapsedMilliseconds, succeeded); }
        }

        public DataTable SelectById(int id, SqlTransactionContext transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");
            SqlQuery query = new SqlQuery(
                "SELECT * FROM Ilce_Table WHERE Id=@Id ORDER BY IlceAdi");
            query.AddParameter("@Id", id);
            return SelectInTransaction(query, transaction);
        }

        public DataTable SelectByProvinceId(int provinceId)
        {
            SqlQuery query = new SqlQuery(
                "SELECT * FROM Ilce_Table WHERE IlId=@IlId ORDER BY IlceAdi");
            query.AddParameter("@IlId", provinceId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByProvinceId(int provinceId, PortalLogContext context)
        {
            SqlQuery query = new SqlQuery(
                "SELECT * FROM Ilce_Table WHERE IlId=@IlId ORDER BY IlceAdi");
            query.AddParameter("@IlId", provinceId);
            return PortalSqlReadExecutor.Select(query, context, "IlceRepository.SelectByProvinceId");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery(
                "SELECT * FROM Ilce_Table ORDER BY IlceAdi"), "");
        }

        public DataTable SelectByProvinceName(string provinceName)
        {
            SqlQuery query = new SqlQuery(
                "SELECT * FROM Ilce_Table WHERE IlAdi=@IlAdi ORDER BY IlceAdi");
            query.AddParameter("@IlAdi", provinceName);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectDistrictFromLegacyTable(int districtId)
        {
            SqlQuery query = new SqlQuery(
                "SELECT * FROM ILCELER WHERE ILCE_ID=@IlceId ORDER BY IlceAdi");
            query.AddParameter("@IlceId", districtId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByProvinceAndDistrictName(string provinceName, string districtName)
        {
            SqlQuery query = new SqlQuery(
                "SELECT * FROM Ilce_Table " +
                "WHERE LOWER(IlceAdi)=LOWER(@IlceAdi) " +
                "AND LOWER(IlAdi)=LOWER(@IlAdi) ORDER BY IlceAdi");
            query.AddParameter("@IlceAdi", districtName);
            query.AddParameter("@IlAdi", provinceName);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByProvinceAndDistrictName(
            string provinceName,
            string districtName,
            SqlTransactionContext transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");
            SqlQuery query = new SqlQuery(
                "SELECT * FROM Ilce_Table " +
                "WHERE LOWER(IlceAdi)=LOWER(@IlceAdi) " +
                "AND LOWER(IlAdi)=LOWER(@IlAdi) ORDER BY IlceAdi");
            query.AddParameter("@IlceAdi", districtName);
            query.AddParameter("@IlAdi", provinceName);
            return SelectInTransaction(query, transaction);
        }

        public DataTable SelectByProvinceAndDistrictContains(string provinceName, string districtName)
        {
            SqlQuery query = new SqlQuery(
                "SELECT * FROM Ilce_Table WHERE IlAdi=@IlAdi " +
                "AND IlceAdi LIKE @IlceAdi ORDER BY IlceAdi");
            query.AddParameter("@IlAdi", provinceName);
            query.AddParameter("@IlceAdi", "%" + districtName + "%");
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectCountByRegion(int regionId)
        {
            SqlQuery query = new SqlQuery(
                "SELECT COUNT(A.Id) Adet FROM Ilce_Table A " +
                "INNER JOIN Il_Table B ON B.Id=A.IlId " +
                "WHERE B.Id BETWEEN 1 AND 81 AND B.BolgeId=@BolgeId " +
                "AND IlceAdi!=@Merkez");
            query.AddParameter("@BolgeId", regionId);
            query.AddParameter("@Merkez", Utility.ProjeGlobal.ProjeConstants.ILCE_MERKEZ);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectWithFTK(int provinceId)
        {
            SqlQuery query = new SqlQuery(
                "SELECT A.* FROM Ilce_Table A INNER JOIN Il_Table B ON B.Id=A.IlId " +
                "WHERE A.IlceAdi!=@Merkez AND B.Id BETWEEN 0 AND 81 AND B.IlAdi != '' " +
                "AND A.Id IN (SELECT Ilcesi FROM FTK_Table WHERE Ilcesi > 0)" +
                (provinceId < 1 ? "" : " AND B.Id=@IlId") + " ORDER BY A.IlceAdi");
            query.AddParameter("@Merkez", Utility.ProjeGlobal.ProjeConstants.ILCE_MERKEZ);
            if (provinceId >= 1)
                query.AddParameter("@IlId", provinceId);
            return db.SelectFromDb(query, "");
        }
        private DataTable SelectInTransaction(SqlQuery query, SqlTransactionContext transaction,
            [System.Runtime.CompilerServices.CallerMemberName] string method = "")
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            return logContext == null ? transaction.Select(query) : PortalSqlReadExecutor.Select(query, logContext, "IlceRepository." + method, transaction);
        }
    }
}
