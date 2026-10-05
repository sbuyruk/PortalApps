using DAO.Ortak;
using System;
using System.Data;
namespace DAO.Repositories.IKYS
{
    public class EgitimSeviyesiRepository
    {
        private readonly DbClass db; private readonly CrudQueryBuilder queryBuilder;
        public EgitimSeviyesiRepository() : this(new DbClass()) { }
        public EgitimSeviyesiRepository(DbClass db) { if (db == null) throw new ArgumentNullException("db"); this.db = db; queryBuilder = new CrudQueryBuilder(); }
        public DataTable SelectById(int id) { SqlQuery q = new SqlQuery("SELECT * FROM EgitimSeviyesi_Table WHERE Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM EgitimSeviyesi_Table"), ""); }
        public int Insert<T>(T item) { return db.Insert(queryBuilder.BuildInsert(item, "EgitimSeviyesi_Table")); }
        public bool Update<T>(T item) { return db.Update2Db(queryBuilder.BuildUpdate(item, "EgitimSeviyesi_Table")); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("EgitimSeviyesi_Table", id), ""); }
        public DataTable SelectByPersonelId(int personelId) { SqlQuery q = new SqlQuery("SELECT * FROM EgitimSeviyesi_Table WHERE PersonelId=@PersonelId ORDER BY Id"); q.AddParameter("@PersonelId", personelId); return db.SelectFromDb(q, ""); }
    }
}
