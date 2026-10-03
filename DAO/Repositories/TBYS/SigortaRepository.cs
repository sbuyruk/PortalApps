using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.TBYS
{
    public class SigortaRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public SigortaRepository() : this(new DbClass()) { }

        public SigortaRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM Sigorta_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery("SELECT * FROM Sigorta_Table"), "");
        }

        public DataTable SelectByTasinmazId(int tasinmazId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT * FROM Sigorta_Table
                WHERE TasinmazId=@TasinmazId
                ORDER BY SigortaBitTar DESC");
            query.AddParameter("@TasinmazId", tasinmazId);
            return db.SelectFromDb(query, "");
        }

        public int Insert<T>(T entity)
        {
            return db.Insert(queryBuilder.BuildInsert(entity, "Sigorta_Table"));
        }

        public bool Update<T>(T entity)
        {
            return db.Update2Db(queryBuilder.BuildUpdate(entity, "Sigorta_Table"));
        }

        public bool Delete(int id)
        {
            return db.DeleteFromDb(queryBuilder.BuildDelete("Sigorta_Table", id), "");
        }
    }
}
