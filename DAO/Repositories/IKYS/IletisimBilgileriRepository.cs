using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.IKYS
{
    public class IletisimBilgileriRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;
        public IletisimBilgileriRepository() : this(new DbClass()) { }
        public IletisimBilgileriRepository(DbClass db) { if (db == null) throw new ArgumentNullException("db"); this.db = db; queryBuilder = new CrudQueryBuilder(); }
        public DataTable SelectById(int id) { SqlQuery q = new SqlQuery("SELECT * FROM IletisimBilgileri_Table WHERE Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM IletisimBilgileri_Table"), ""); }
        public int Insert<T>(T item) { return db.Insert(queryBuilder.BuildInsert(item, "IletisimBilgileri_Table")); }
        public bool Update<T>(T item) { return db.Update2Db(queryBuilder.BuildUpdate(item, "IletisimBilgileri_Table")); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("IletisimBilgileri_Table", id), ""); }
        public DataTable SelectByPersonelId(int personelId) { SqlQuery q = new SqlQuery("SELECT * FROM IletisimBilgileri_Table WHERE PersonelId=@PersonelId ORDER BY Adres"); q.AddParameter("@PersonelId", personelId); return db.SelectFromDb(q, ""); }
    }
}
