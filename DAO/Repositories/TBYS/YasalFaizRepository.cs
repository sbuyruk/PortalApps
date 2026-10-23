using DAO.Ortak;
using System;
using System.Data;
using Utility.Logging;

namespace DAO.Repositories.TBYS
{
    public class YasalFaizRepository
    {
        private readonly TbysSqlExecutor db;
        private readonly CrudQueryBuilder queryBuilder;

        public YasalFaizRepository()
        {
            db = new TbysSqlExecutor("YasalFaizRepository");
            queryBuilder = new CrudQueryBuilder();
        }

        public YasalFaizRepository(PortalLogContext context)
        {
            db = new TbysSqlExecutor(context, "YasalFaizRepository");
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

        public DataTable SelectById(int id, SqlTransactionContext transaction, PortalLogContext context)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM YasalFaiz_Table WITH (UPDLOCK, HOLDLOCK) WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, transaction, context);
        }

        public DataTable SelectByYear(int year, SqlTransactionContext transaction, PortalLogContext context)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM YasalFaiz_Table WITH (UPDLOCK, HOLDLOCK) WHERE Yil=@Yil ORDER BY Ay, Id");
            query.AddParameter("@Yil", year);
            return db.SelectFromDb(query, transaction, context);
        }

        public DataTable SelectByYearMonth(int year, int month, SqlTransactionContext transaction, PortalLogContext context)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM YasalFaiz_Table WITH (UPDLOCK, HOLDLOCK) WHERE Yil=@Yil AND Ay=@Ay");
            query.AddParameter("@Yil", year);
            query.AddParameter("@Ay", month);
            return db.SelectFromDb(query, transaction, context);
        }

        public DataTable SelectLatestRate(string columnName, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (columnName != "FaizOrani" && columnName != "Tufe" && columnName != "Ufe")
                throw new ArgumentException("Geçersiz oran alanı.", "columnName");
            SqlQuery query = new SqlQuery("SELECT TOP (1) * FROM YasalFaiz_Table WITH (UPDLOCK, HOLDLOCK) WHERE "
                + columnName + " IS NOT NULL AND " + columnName + " > 0 ORDER BY Yil DESC, Ay DESC, Id DESC");
            return db.SelectFromDb(query, transaction, context);
        }

        public int Insert<T>(T entity, SqlTransactionContext transaction, PortalLogContext context)
        {
            return db.Insert(queryBuilder.BuildInsert(entity, "YasalFaiz_Table"), transaction, context);
        }

        public bool Update<T>(T entity, SqlTransactionContext transaction, PortalLogContext context)
        {
            return db.Update2Db(queryBuilder.BuildUpdate(entity, "YasalFaiz_Table"), transaction, context);
        }

        public bool Delete(int id, SqlTransactionContext transaction, PortalLogContext context)
        {
            return db.DeleteFromDb(queryBuilder.BuildDelete("YasalFaiz_Table", id), transaction, context);
        }
    }
}
