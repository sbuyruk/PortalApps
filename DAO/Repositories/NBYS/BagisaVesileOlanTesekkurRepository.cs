using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.NBYS
{
    public class BagisaVesileOlanTesekkurRepository
    {
        private const string TableName = "BagisaVesileOlanTesekkur_Table";
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public BagisaVesileOlanTesekkurRepository() : this(new DbClass())
        {
        }

        public BagisaVesileOlanTesekkurRepository(DbClass db)
        {
            if (db == null)
                throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery(
                "SELECT * FROM BagisaVesileOlanTesekkur_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectForList()
        {
            return db.SelectFromDb(new SqlQuery(@"SELECT *,
                CONVERT(varchar,FORMAT(BelgeTarihi,'dd.MM.yyyy')) BelgeTarihiDDMMYYYY
                FROM BagisaVesileOlanTesekkur_Table"), "");
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
