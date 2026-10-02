using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.Ortak
{
    public class BolgeRepository
    {
        private readonly DbClass db;

        public BolgeRepository() : this(new DbClass())
        {
        }

        public BolgeRepository(DbClass db)
        {
            if (db == null)
                throw new ArgumentNullException("db");
            this.db = db;
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM Bolge_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByDonorId(int donorId)
        {
            SqlQuery query = new SqlQuery(
                "SELECT A.* FROM Bolge_Table A " +
                "INNER JOIN Il_Table C ON C.BolgeId=A.Id " +
                "INNER JOIN NakitBagisci_Table B ON B.Ili=C.Id " +
                "WHERE B.Id=@BagisciId");
            query.AddParameter("@BagisciId", donorId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectActive(int regionId)
        {
            bool all = regionId == Utility.ProjeGlobal.ProjeConstants.HEPSI_INT ||
                regionId == Utility.ProjeGlobal.ProjeConstants.BOLGE_GENELMUDURLUK_INT;
            SqlQuery query = new SqlQuery("SELECT * FROM Bolge_Table WHERE Aktif=1" +
                (all ? "" : " AND Id=@BolgeId"));
            if (!all)
                query.AddParameter("@BolgeId", regionId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectFirst()
        {
            return db.SelectFromDb(new SqlQuery("SELECT * FROM Bolge_Table"), "");
        }
    }
}
