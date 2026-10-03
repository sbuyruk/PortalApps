using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.Ortak
{
    public class IlRepository
    {
        private readonly DbClass db;

        public IlRepository() : this(new DbClass())
        {
        }

        public IlRepository(DbClass db)
        {
            if (db == null)
                throw new ArgumentNullException("db");
            this.db = db;
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM Il_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery("SELECT * FROM Il_Table"), "");
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

        public DataTable SelectByEnglishName(string name)
        {
            SqlQuery query = new SqlQuery(
                "SELECT * FROM Il_Table WHERE LOWER(IngIlAdi)=LOWER(@IngIlAdi)");
            query.AddParameter("@IngIlAdi", name);
            return db.SelectFromDb(query, "");
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
    }
}
