using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.TBYS
{
    public class OdemePlaniRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;
        public OdemePlaniRepository() : this(new DbClass()) { }
        public OdemePlaniRepository(DbClass db) { if (db == null) throw new ArgumentNullException("db"); this.db = db; queryBuilder = new CrudQueryBuilder(); }
        public DataTable SelectById(int id) { SqlQuery q = new SqlQuery("SELECT * FROM OdemePlani_Table WHERE Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM OdemePlani_Table"), ""); }
        public DataTable SelectBySozlesmeId(int id) { SqlQuery q = new SqlQuery("SELECT * FROM OdemePlani_Table WHERE SozlesmeId=@SozlesmeId ORDER BY Sira"); q.AddParameter("@SozlesmeId", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectBySozlesmeIdSira(int id, int sira) { SqlQuery q = new SqlQuery("SELECT * FROM OdemePlani_Table WHERE SozlesmeId=@SozlesmeId AND Sira=@Sira"); q.AddParameter("@SozlesmeId", id); q.AddParameter("@Sira", sira); return db.SelectFromDb(q, ""); }
        public DataTable SelectBySozlesmeIdOdemeTarihi(int id, DateTime tarih) { SqlQuery q = new SqlQuery("SELECT * FROM OdemePlani_Table WHERE Sira!=0 AND SozlesmeId=@SozlesmeId AND @OdemeTarihi BETWEEN VadeBasTar AND DateADD(day,-1,DATEADD(month,1,VadeBasTar)) ORDER BY VadeBasTar"); q.AddParameter("@SozlesmeId", id); q.AddParameter("@OdemeTarihi", tarih); return db.SelectFromDb(q, ""); }
        public DataTable SelectExists(int id) { SqlQuery q = new SqlQuery("SELECT * FROM OdemePlani_Table WHERE SozlesmeId=@SozlesmeId"); q.AddParameter("@SozlesmeId", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectLastBySozlesmeId(int id) { SqlQuery q = new SqlQuery("SELECT * FROM OdemePlani_Table WHERE SozlesmeId=@SozlesmeId ORDER BY VadeBasTar"); q.AddParameter("@SozlesmeId", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectFirstBySozlesmeId(int id) { SqlQuery q = new SqlQuery("SELECT * FROM OdemePlani_Table WHERE SozlesmeId=@SozlesmeId AND Sira=1"); q.AddParameter("@SozlesmeId", id); return db.SelectFromDb(q, ""); }
        public bool DeleteBySozlesmeId(int id) { SqlQuery q = new SqlQuery("DELETE OdemePlani_Table WHERE SozlesmeId=@SozlesmeId"); q.AddParameter("@SozlesmeId", id); return db.DeleteFromDb(q, ""); }
        public int Insert<T>(T entity) { return db.Insert(queryBuilder.BuildInsert(entity, "OdemePlani_Table")); }
        public bool Update<T>(T entity) { return db.Update2Db(queryBuilder.BuildUpdate(entity, "OdemePlani_Table")); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("OdemePlani_Table", id), ""); }
    }
}
