using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.IKYS
{
    public class IsBilgileriRepository
    {
        private readonly DbClass db; private readonly CrudQueryBuilder queryBuilder;
        public IsBilgileriRepository() : this(new DbClass()) { }
        public IsBilgileriRepository(DbClass db) { if (db == null) throw new ArgumentNullException("db"); this.db = db; queryBuilder = new CrudQueryBuilder(); }
        public DataTable SelectById(int id) { SqlQuery q = new SqlQuery("SELECT * FROM IsBilgileri_Table WHERE Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM IsBilgileri_Table"), ""); }
        public int Insert<T>(T item) { return db.Insert(queryBuilder.BuildInsert(item, "IsBilgileri_Table")); }
        public bool Update<T>(T item) { return db.Update2Db(queryBuilder.BuildUpdate(item, "IsBilgileri_Table")); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("IsBilgileri_Table", id), ""); }
        public DataTable SelectByPersonelId(int personelId) { SqlQuery q = new SqlQuery("SELECT * FROM IsBilgileri_Table WHERE PersonelId=@PersonelId ORDER BY UnvanId"); q.AddParameter("@PersonelId", personelId); return db.SelectFromDb(q, ""); }
        public DataTable SelectByGorevId(int gorevId) { SqlQuery q = new SqlQuery("SELECT * FROM IsBilgileri_Table WHERE GorevId=@GorevId ORDER BY UnvanId"); q.AddParameter("@GorevId", gorevId); return db.SelectFromDb(q, ""); }
        public DataTable SelectAllFromIsYeriBilgileri() { return db.SelectFromDb(new SqlQuery("SELECT * FROM IS_YERI_BILGILERI"), ""); }
    }
}
