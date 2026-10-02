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
    }
}
