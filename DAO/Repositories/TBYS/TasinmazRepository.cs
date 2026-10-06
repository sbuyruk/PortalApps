using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.TBYS
{
    public class TasinmazRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public TasinmazRepository() : this(new DbClass()) { }

        public TasinmazRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM Tasinmaz_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectInventoryById(int id)
        {
            SqlQuery query = new SqlQuery(
                "SELECT * FROM Tasinmaz_Table WHERE EnvanterdeMi=1 AND Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectInventory()
        {
            return db.SelectFromDb(new SqlQuery(
                "SELECT * FROM Tasinmaz_Table WHERE EnvanterdeMi=1"), "");
        }

        public int Insert<T>(T item) { return db.Insert(queryBuilder.BuildInsert(item, "Tasinmaz_Table")); }
        public bool Update<T>(T item) { return db.Update2Db(queryBuilder.BuildUpdate(item, "Tasinmaz_Table")); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("Tasinmaz_Table", id), ""); }
    }
}
