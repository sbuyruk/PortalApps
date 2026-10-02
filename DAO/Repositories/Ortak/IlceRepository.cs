using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.Ortak
{
    public class IlceRepository
    {
        private readonly DbClass db;

        public IlceRepository() : this(new DbClass())
        {
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

        public DataTable SelectByProvinceId(int provinceId)
        {
            SqlQuery query = new SqlQuery(
                "SELECT * FROM Ilce_Table WHERE IlId=@IlId ORDER BY IlceAdi");
            query.AddParameter("@IlId", provinceId);
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
    }
}
