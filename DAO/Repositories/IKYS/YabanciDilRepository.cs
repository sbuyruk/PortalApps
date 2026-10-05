using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.IKYS
{
    public class YabanciDilRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;
        public YabanciDilRepository() : this(new DbClass()) { }
        public YabanciDilRepository(DbClass db) { if (db == null) throw new ArgumentNullException("db"); this.db = db; queryBuilder = new CrudQueryBuilder(); }
        public DataTable SelectById(int id) { SqlQuery q = new SqlQuery("SELECT * FROM YabanciDil_Table WHERE Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM YabanciDil_Table"), ""); }
        public int Insert<T>(T item) { return db.Insert(queryBuilder.BuildInsert(item, "YabanciDil_Table")); }
        public bool Update<T>(T item) { return db.Update2Db(queryBuilder.BuildUpdate(item, "YabanciDil_Table")); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("YabanciDil_Table", id), ""); }
        public DataTable SelectByPersonelId(int personelId) { SqlQuery q = new SqlQuery("SELECT * FROM YabanciDil_Table WHERE PersonelId=@PersonelId ORDER BY SinavTarihi DESC"); q.AddParameter("@PersonelId", personelId); return db.SelectFromDb(q, ""); }
    }
}
