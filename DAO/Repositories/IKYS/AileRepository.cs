using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.IKYS
{
    public class AileRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;
        public AileRepository() : this(new DbClass()) { }
        public AileRepository(DbClass db) { if (db == null) throw new ArgumentNullException("db"); this.db = db; queryBuilder = new CrudQueryBuilder(); }
        public DataTable SelectById(int id) { SqlQuery q = new SqlQuery("SELECT * FROM Aile_Table WHERE Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM Aile_Table"), ""); }
        public int Insert<T>(T item) { return db.Insert(queryBuilder.BuildInsert(item, "Aile_Table")); }
        public bool Update<T>(T item) { return db.Update2Db(queryBuilder.BuildUpdate(item, "Aile_Table")); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("Aile_Table", id), ""); }
        public DataTable SelectByPersonelId(int personelId) { SqlQuery q = new SqlQuery("SELECT * FROM Aile_Table WHERE PersonelId=@PersonelId ORDER BY YakinlikDerecesi"); q.AddParameter("@PersonelId", personelId); return db.SelectFromDb(q, ""); }
        public DataTable SelectEnGencCocukByPersonelId(int personelId, int cocukYakinlikDerecesi) { SqlQuery q = new SqlQuery("SELECT * FROM Aile_Table WHERE PersonelId=@PersonelId AND YakinlikDerecesi=@YakinlikDerecesi ORDER BY DogumTar DESC"); q.AddParameter("@PersonelId", personelId); q.AddParameter("@YakinlikDerecesi", cocukYakinlikDerecesi); return db.SelectFromDb(q, ""); }
    }
}
