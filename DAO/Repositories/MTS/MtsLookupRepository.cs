using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.MTS
{
    public class MtsLookupRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public MtsLookupRepository()
            : this(new DbClass())
        {
        }

        public MtsLookupRepository(DbClass db)
        {
            if (db == null)
                throw new ArgumentNullException("db");

            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(string tableName, int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM " + tableName + " WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAll(string tableName)
        {
            return db.SelectFromDb(new SqlQuery("SELECT * FROM " + tableName + " ORDER BY Id"), "");
        }

        public int Insert<T>(string tableName, T entity)
        {
            return db.Insert(queryBuilder.BuildInsert(entity, tableName));
        }

        public bool Update<T>(string tableName, T entity)
        {
            return db.Update2Db(queryBuilder.BuildUpdate(entity, tableName));
        }

        public bool Delete(string tableName, int id)
        {
            return db.DeleteFromDb(queryBuilder.BuildDelete(tableName, id), "");
        }
    }
}
