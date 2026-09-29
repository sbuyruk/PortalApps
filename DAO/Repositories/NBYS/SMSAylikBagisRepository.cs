using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.NBYS
{
    public class SMSAylikBagisRepository
    {
        private const string TableName = "SMSAylikBagis_Table";
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public SMSAylikBagisRepository() : this(new DbClass())
        {
        }

        public SMSAylikBagisRepository(DbClass db)
        {
            if (db == null)
                throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM SMSAylikBagis_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByYear(int year)
        {
            SqlQuery query = new SqlQuery(@"SELECT *
                FROM SMSAylikBagis_Table
                WHERE Yil=@Yil
                ORDER BY Ay");
            query.AddParameter("@Yil", year);
            return db.SelectFromDb(query, "");
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
