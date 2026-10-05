using DAO.Ortak;
using System;
using System.Data;
namespace DAO.Repositories.IKYS
{
    public class KursRepository
    {
        private readonly DbClass db; private readonly CrudQueryBuilder queryBuilder;
        public KursRepository() : this(new DbClass()) { }
        public KursRepository(DbClass db) { if (db == null) throw new ArgumentNullException("db"); this.db = db; queryBuilder = new CrudQueryBuilder(); }
        public DataTable SelectById(int id) { SqlQuery q = new SqlQuery("SELECT * FROM Kurs_Table WHERE Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM Kurs_Table"), ""); }
        public int Insert<T>(T item) { return db.Insert(queryBuilder.BuildInsert(item, "Kurs_Table")); }
        public bool Update<T>(T item) { return db.Update2Db(queryBuilder.BuildUpdate(item, "Kurs_Table")); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("Kurs_Table", id), ""); }
        public DataTable SelectByPersonelId(int personelId) { SqlQuery q = new SqlQuery("SELECT * FROM Kurs_Table WHERE PersonelId=@PersonelId ORDER BY Tarih DESC"); q.AddParameter("@PersonelId", personelId); return db.SelectFromDb(q, ""); }
    }
}
