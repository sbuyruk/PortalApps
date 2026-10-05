using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.IKYS
{
    public class MaasHareketRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public MaasHareketRepository() : this(new DbClass()) { }
        public MaasHareketRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }
        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM MaasHareket_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM MaasHareket_Table"), ""); }
        public int Insert<T>(T item) { return db.Insert(queryBuilder.BuildInsert(item, "MaasHareket_Table")); }
        public bool Update<T>(T item) { return db.Update2Db(queryBuilder.BuildUpdate(item, "MaasHareket_Table")); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("MaasHareket_Table", id), ""); }
        public DataTable SelectByTarih(DateTime tarih)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM MaasHareket_Table WHERE Tarih=@Tarih ORDER BY Derece,Kademe");
            query.AddParameter("@Tarih", tarih);
            return db.SelectFromDb(query, "");
        }
        public bool DeleteByGrupId(int grupId)
        {
            SqlQuery query = new SqlQuery("DELETE FROM MaasHareket_Table WHERE GrupId=@GrupId");
            query.AddParameter("@GrupId", grupId);
            return db.DeleteFromDb(query, "");
        }
    }
}
