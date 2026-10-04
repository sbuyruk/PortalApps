using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.TBYS
{
    public class OdemeRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;
        public OdemeRepository() : this(new DbClass()) { }
        public OdemeRepository(DbClass db) { if (db == null) throw new ArgumentNullException("db"); this.db = db; queryBuilder = new CrudQueryBuilder(); }
        public DataTable SelectById(int id) { SqlQuery q = new SqlQuery("SELECT * FROM Odeme_Table WHERE Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM Odeme_Table"), ""); }
        public DataTable SelectByKiraciId(int id) { SqlQuery q = new SqlQuery("SELECT * FROM Odeme_Table WHERE KiraciId=@KiraciId ORDER BY OdemeTarihi DESC"); q.AddParameter("@KiraciId", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectBySozlesmeIdOdemePlaniId(int sozlesmeId, int planId) { SqlQuery q = new SqlQuery("SELECT * FROM Odeme_Table WHERE SozlesmeId=@SozlesmeId AND OdemePlaniId=@OdemePlaniId ORDER BY OdemeTarihi"); q.AddParameter("@SozlesmeId", sozlesmeId); q.AddParameter("@OdemePlaniId", planId); return db.SelectFromDb(q, ""); }
        public bool DeleteBySozlesmeId(int id) { SqlQuery q = new SqlQuery("DELETE Odeme_Table WHERE SozlesmeId=@SozlesmeId"); q.AddParameter("@SozlesmeId", id); return db.DeleteFromDb(q, ""); }
        public int Insert<T>(T entity) { return db.Insert(queryBuilder.BuildInsert(entity, "Odeme_Table")); }
        public bool Update<T>(T entity) { return db.Update2Db(queryBuilder.BuildUpdate(entity, "Odeme_Table")); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("Odeme_Table", id), ""); }
    }
}
