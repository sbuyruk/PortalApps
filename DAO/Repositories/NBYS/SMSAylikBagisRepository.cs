using DAO.Ortak;
using System;
using System.Data;
using Utility.Logging;

namespace DAO.Repositories.NBYS
{
    public class SMSAylikBagisRepository
    {
        private const string TableName = "SMSAylikBagis_Table";
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;
        private readonly PortalLogContext context;
        private readonly NbysSqlExecutor sql;

        public SMSAylikBagisRepository(PortalLogContext context)
        {
            this.context = context;
            sql = new NbysSqlExecutor(context, "SMSAylikBagisRepository");
            queryBuilder = new CrudQueryBuilder();
        }

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
            return sql == null ? db.SelectFromDb(query, "") : sql.Select(query);
        }

        public DataTable SelectByYear(int year)
        {
            SqlQuery query = new SqlQuery(@"SELECT *
                FROM SMSAylikBagis_Table
                WHERE Yil=@Yil
                ORDER BY Ay");
            query.AddParameter("@Yil", year);
            return sql == null ? db.SelectFromDb(query, "") : sql.Select(query);
        }

        public int Insert<T>(T entity)
        {
            RequireLegacyWrite();
            return db.Insert(queryBuilder.BuildInsert(entity, TableName));
        }

        public bool Update<T>(T entity)
        {
            RequireLegacyWrite();
            return db.Update2Db(queryBuilder.BuildUpdate(entity, TableName));
        }

        public bool Delete(int id)
        {
            RequireLegacyWrite();
            return db.DeleteFromDb(queryBuilder.BuildDelete(TableName, id), "");
        }

        public bool LockYear(int year, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            SqlQuery query = new SqlQuery(@"DECLARE @result int;
                EXEC @result=sp_getapplock @Resource=@Resource,
                    @LockMode=N'Exclusive',@LockOwner=N'Transaction',@LockTimeout=15000;
                SELECT @result Result;");
            query.AddParameter("@Resource", "PortalApps.NBYS.SMS.Year." + year.ToString(System.Globalization.CultureInfo.InvariantCulture));
            DataTable result = sql.Select(query, transaction);
            if (result == null) throw new InvalidOperationException("SMS çizelgesi işlem kilidi alınamadı.");
            int status = Convert.ToInt32(result.Rows[0]["Result"]);
            if (status == -1) return false;
            if (status < 0) throw new InvalidOperationException("SMS çizelgesi işlem kilidi alınamadı.");
            return true;
        }

        public DataTable SelectYearForWrite(int year, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            SqlQuery query = new SqlQuery(@"SELECT * FROM SMSAylikBagis_Table WITH (UPDLOCK,HOLDLOCK)
                WHERE Yil=@Year ORDER BY Ay,Id");
            query.AddParameter("@Year", year);
            return sql.Select(query, transaction);
        }

        public int InsertMonth<T>(T item, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            return new NbysTransactionalWriter(context, TableName).Insert(queryBuilder.BuildInsert(item, TableName), transaction);
        }

        public void UpdateCounts(int id, int turkcell, int vodafone, int telekom, string actor, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            SqlQuery query = new SqlQuery(@"UPDATE SMSAylikBagis_Table SET TurkcellSMSAdedi=@Turkcell,
                VodafoneSMSAdedi=@Vodafone,TurkTelekomSMSAdedi=@Telekom,
                Degistiren=@Actor,DegistirmeTarihi=@Changed WHERE Id=@Id");
            query.AddParameter("@Id", id);
            query.AddParameter("@Turkcell", turkcell);
            query.AddParameter("@Vodafone", vodafone);
            query.AddParameter("@Telekom", telekom);
            query.AddParameter("@Actor", actor);
            query.AddParameter("@Changed", DateTime.Now);
            new NbysTransactionalWriter(context, TableName).Update(query, id, transaction);
        }

        private void RequireTransaction(SqlTransactionContext transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (sql == null) throw new InvalidOperationException("SMS yazma işlemi için açık NBYS log bağlamı gereklidir.");
        }

        private void RequireLegacyWrite()
        {
            if (sql != null) throw new InvalidOperationException("SMS yazma işlemi transaction servisi üzerinden yapılmalıdır.");
        }
    }
}
