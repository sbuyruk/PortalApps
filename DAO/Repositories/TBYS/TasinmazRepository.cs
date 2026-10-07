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
        public DataTable SelectInventoryByIlAdi(string ilAdi) { SqlQuery q = new SqlQuery("SELECT * FROM Tasinmaz_Table WHERE EnvanterdeMi=1 AND Ili=@IlAdi"); q.AddParameter("@IlAdi", ilAdi); return db.SelectFromDb(q, ""); }
        public DataTable SelectOutOfInventoryById(int id) { SqlQuery q = new SqlQuery("SELECT *,Convert(nvarchar,replace (EnvanterdenCikmaBedeli,'.',',')) as EnvanterdenCikmaBedeli FROM Tasinmaz_Table WHERE EnvanterdeMi=0 AND Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectNext(int id) { SqlQuery q = new SqlQuery("SELECT * FROM Tasinmaz_Table WHERE EnvanterdeMi=1 AND Id>@Id ORDER BY Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectPrev(int id) { SqlQuery q = new SqlQuery("SELECT * FROM Tasinmaz_Table WHERE EnvanterdeMi=1 AND Id<@Id ORDER BY Id DESC"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectExtreme(bool max) { return db.SelectFromDb(new SqlQuery("SELECT " + (max ? "MAX" : "MIN") + "(Id) Id FROM Tasinmaz_Table WHERE EnvanterdeMi=1"), ""); }

        public int Insert<T>(T item) { return db.Insert(queryBuilder.BuildInsert(item, "Tasinmaz_Table")); }
        public bool Update<T>(T item) { return db.Update2Db(queryBuilder.BuildUpdate(item, "Tasinmaz_Table")); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("Tasinmaz_Table", id), ""); }
    }
}
