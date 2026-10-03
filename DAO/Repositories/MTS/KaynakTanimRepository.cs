using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.MTS
{
    public class KaynakTanimRepository
    {
        private const string TableName = "KaynakTanim_Table";
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public KaynakTanimRepository() : this(new DbClass())
        {
        }

        public KaynakTanimRepository(DbClass db)
        {
            if (db == null)
                throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM KaynakTanim_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery(
                "SELECT * FROM KaynakTanim_Table ORDER BY Adi"), "");
        }

        public int Insert<T>(T entity)
        {
            return db.Insert(queryBuilder.BuildInsert(entity, TableName));
        }

        public bool Update<T>(T entity)
        {
            return db.Update2Db(queryBuilder.BuildUpdate(entity, TableName));
        }

        public bool Delete(int id)
        {
            return db.DeleteFromDb(queryBuilder.BuildDelete(TableName, id), "");
        }
    }
}
