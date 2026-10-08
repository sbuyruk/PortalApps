using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.TBYS
{
    public class YasalFaizRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public YasalFaizRepository()
            : this(new DbClass())
        {
        }

        public YasalFaizRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            SqlQuery q = new SqlQuery("SELECT * FROM YasalFaiz_Table WHERE Id=@Id");
            q.AddParameter("@Id", id);
            return db.SelectFromDb(q, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery("SELECT * FROM YasalFaiz_Table"), "");
        }

        public DataTable SelectByYear(int year)
        {
            SqlQuery q = new SqlQuery("SELECT * FROM YasalFaiz_Table WHERE Yil=@Yil");
            q.AddParameter("@Yil", year);
            return db.SelectFromDb(q, "");
        }

        public DataTable SelectByYearMonth(int year, int month)
        {
            SqlQuery q = new SqlQuery("SELECT * FROM YasalFaiz_Table WHERE Yil=@Yil AND Ay=@Ay");
            q.AddParameter("@Yil", year);
            q.AddParameter("@Ay", month);
            return db.SelectFromDb(q, "");
        }

        public DataTable SelectLatestRate(string columnName)
        {
            return db.SelectFromDb(new SqlQuery("SELECT * FROM YasalFaiz_Table WHERE " + columnName + " IS NOT NULL AND " + columnName + " > 0 ORDER BY Yil DESC, Ay DESC"), "");
        }

        public int Insert<T>(T entity)
        {
            return db.Insert(queryBuilder.BuildInsert(entity, "YasalFaiz_Table"));
        }

        public bool Update<T>(T entity)
        {
            return db.Update2Db(queryBuilder.BuildUpdate(entity, "YasalFaiz_Table"));
        }

        public bool Delete(int id)
        {
            return db.DeleteFromDb(queryBuilder.BuildDelete("YasalFaiz_Table", id), "");
        }
    }
}
