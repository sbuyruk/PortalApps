using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.TBYS
{
    public class VasiyeteKonuVarlikRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public VasiyeteKonuVarlikRepository() : this(new DbClass()) { }

        public VasiyeteKonuVarlikRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM VasiyeteKonuVarlik_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery(@"
                SELECT *
                FROM VasiyeteKonuVarlik_Table
                ORDER BY VasiyetciId"), "");
        }

        public DataTable SelectByVasiyetciId(int vasiyetciId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT *
                FROM VasiyeteKonuVarlik_Table
                WHERE VasiyetciId=@VasiyetciId
                ORDER BY VasiyetciId");
            query.AddParameter("@VasiyetciId", vasiyetciId);
            return db.SelectFromDb(query, "");
        }

        public int Insert<T>(T entity)
        {
            return db.Insert(queryBuilder.BuildInsert(entity, "VasiyeteKonuVarlik_Table"));
        }

        public bool Update<T>(T entity)
        {
            return db.Update2Db(queryBuilder.BuildUpdate(entity, "VasiyeteKonuVarlik_Table"));
        }

        public bool Delete(int id)
        {
            return db.DeleteFromDb(queryBuilder.BuildDelete("VasiyeteKonuVarlik_Table", id), "");
        }
    }
}
