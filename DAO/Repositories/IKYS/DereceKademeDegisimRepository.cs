using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.IKYS
{
    public class DereceKademeDegisimRepository
    {
        private readonly DbClass db; private readonly CrudQueryBuilder queryBuilder;
        public DereceKademeDegisimRepository() : this(new DbClass()) { }
        public DereceKademeDegisimRepository(DbClass db) { if (db == null) throw new ArgumentNullException("db"); this.db = db; queryBuilder = new CrudQueryBuilder(); }
        public DataTable SelectById(int id) { SqlQuery q = new SqlQuery("SELECT * FROM DereceKademeDegisim_Table WHERE Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM DereceKademeDegisim_Table"), ""); }
        public int Insert<T>(T item) { return db.Insert(queryBuilder.BuildInsert(item, "DereceKademeDegisim_Table")); }
        public bool Update<T>(T item) { return db.Update2Db(queryBuilder.BuildUpdate(item, "DereceKademeDegisim_Table")); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("DereceKademeDegisim_Table", id), ""); }
        public DataTable SelectByPersonelId(int personelId) { SqlQuery q = new SqlQuery("SELECT * FROM DereceKademeDegisim_Table WHERE PersonelId=@PersonelId ORDER BY DegisimTarihi DESC"); q.AddParameter("@PersonelId", personelId); return db.SelectFromDb(q, ""); }
    }
}
