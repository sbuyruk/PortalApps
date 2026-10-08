using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.TBYS
{
    public class BagimsizBolumRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;
        public BagimsizBolumRepository() : this(new DbClass()) { }
        public BagimsizBolumRepository(DbClass db) { if (db == null) throw new ArgumentNullException("db"); this.db = db; queryBuilder = new CrudQueryBuilder(); }
        public DataTable SelectById(int id) { SqlQuery q = new SqlQuery("SELECT * FROM BagimsizBolum_Table WHERE Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM BagimsizBolum_Table"), ""); }
        public DataTable SelectByTasinmazId(int id) { SqlQuery q = new SqlQuery("SELECT * FROM BagimsizBolum_Table WHERE TasinmazId=@TasinmazId"); q.AddParameter("@TasinmazId", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectByBolumNo(string bolumNo) { SqlQuery q = new SqlQuery("SELECT * FROM BagimsizBolum_Table WHERE BolumNo=@BolumNo"); q.AddParameter("@BolumNo", bolumNo); return db.SelectFromDb(q, ""); }
        public DataTable SelectByBolumId(int id) { SqlQuery q = new SqlQuery("SELECT * FROM BagimsizBolum_Table WHERE bolumId=@BolumId"); q.AddParameter("@BolumId", id); return db.SelectFromDb(q, ""); }
        public int Insert<T>(T entity) { return db.Insert(queryBuilder.BuildInsert(entity, "BagimsizBolum_Table")); }
        public bool Update<T>(T entity) { return db.Update2Db(queryBuilder.BuildUpdate(entity, "BagimsizBolum_Table")); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("BagimsizBolum_Table", id), ""); }
    }
}
