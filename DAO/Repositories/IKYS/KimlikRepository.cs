using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.IKYS
{
    public class KimlikRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;
        public KimlikRepository() : this(new DbClass()) { }
        public KimlikRepository(DbClass db) { if (db == null) throw new ArgumentNullException("db"); this.db = db; queryBuilder = new CrudQueryBuilder(); }
        public DataTable SelectById(int id) { SqlQuery q = new SqlQuery("SELECT * FROM Kimlik_Table WHERE Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM Kimlik_Table"), ""); }
        public int Insert<T>(T item) { return db.Insert(queryBuilder.BuildInsert(item, "Kimlik_Table")); }
        public bool Update<T>(T item) { return db.Update2Db(queryBuilder.BuildUpdate(item, "Kimlik_Table")); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("Kimlik_Table", id), ""); }
        public DataTable SelectByPersonelId(int personelId) { SqlQuery q = new SqlQuery("SELECT * FROM Kimlik_Table WHERE PersonelId=@PersonelId ORDER BY Id"); q.AddParameter("@PersonelId", personelId); return db.SelectFromDb(q, ""); }
        public DataTable SelectByTcKimlikNo(string kimlikNo) { SqlQuery q = new SqlQuery("SELECT * FROM Kimlik_Table WHERE TCKimlikNo=@TCKimlikNo ORDER BY Id"); q.AddParameter("@TCKimlikNo", kimlikNo); return db.SelectFromDb(q, ""); }
        public DataTable SelectAllFromKimlik() { return db.SelectFromDb(new SqlQuery("SELECT * FROM KIMLIK_BILGILERI"), ""); }
    }
}
