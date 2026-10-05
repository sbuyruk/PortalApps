using DAO.Ortak;
using System;
using System.Data;
namespace DAO.Repositories.IKYS
{
    public class IsTecrubeRepository
    {
        private readonly DbClass db; private readonly CrudQueryBuilder queryBuilder;
        public IsTecrubeRepository() : this(new DbClass()) { }
        public IsTecrubeRepository(DbClass db) { if (db == null) throw new ArgumentNullException("db"); this.db = db; queryBuilder = new CrudQueryBuilder(); }
        public DataTable SelectById(int id) { SqlQuery q = new SqlQuery("SELECT * FROM IsTecrube_Table WHERE Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM IsTecrube_Table"), ""); }
        public int Insert<T>(T item) { return db.Insert(queryBuilder.BuildInsert(item, "IsTecrube_Table")); }
        public bool Update<T>(T item) { return db.Update2Db(queryBuilder.BuildUpdate(item, "IsTecrube_Table")); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("IsTecrube_Table", id), ""); }
        public DataTable SelectByPersonelId(int personelId) { SqlQuery q = new SqlQuery("SELECT * FROM IsTecrube_Table WHERE PersonelId=@PersonelId ORDER BY BasTar DESC"); q.AddParameter("@PersonelId", personelId); return db.SelectFromDb(q, ""); }
    }
}
