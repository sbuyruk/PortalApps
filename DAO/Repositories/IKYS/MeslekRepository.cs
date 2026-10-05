using DAO.Ortak;
using System;
using System.Data;
namespace DAO.Repositories.IKYS
{
    public class MeslekRepository
    {
        private readonly DbClass db; private readonly CrudQueryBuilder queryBuilder;
        public MeslekRepository() : this(new DbClass()) { }
        public MeslekRepository(DbClass db) { if (db == null) throw new ArgumentNullException("db"); this.db = db; queryBuilder = new CrudQueryBuilder(); }
        public DataTable SelectById(int id) { SqlQuery q = new SqlQuery("SELECT * FROM Meslek_Table WHERE Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM Meslek_Table"), ""); }
        public int Insert<T>(T item) { return db.Insert(queryBuilder.BuildInsert(item, "Meslek_Table")); }
        public bool Update<T>(T item) { return db.Update2Db(queryBuilder.BuildUpdate(item, "Meslek_Table")); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("Meslek_Table", id), ""); }
        public DataTable SelectByPersonelId(int personelId) { SqlQuery q = new SqlQuery("SELECT * FROM Meslek_Table WHERE PersonelId=@PersonelId ORDER BY Id"); q.AddParameter("@PersonelId", personelId); return db.SelectFromDb(q, ""); }
    }
}
