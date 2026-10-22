using DAO.Ortak;
using System;
using System.Data;
using Utility.Logging;

namespace DAO.Repositories.NBYS
{
    public class BankaTanimRepository
    {
        public bool HasLogContext { get { return logContext != null; } }
        private const string TableName = "BankaTanim_Table";
        private readonly DbClass db;
        private readonly PortalLogContext logContext;
        private readonly CrudQueryBuilder queryBuilder;

        public BankaTanimRepository() : this(new DbClass())
        {
        }

        public BankaTanimRepository(SqlTransactionContext transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            queryBuilder = new CrudQueryBuilder();
        }

        public BankaTanimRepository(PortalLogContext context)
        {
            if (context == null || context.Project != "NBYS") throw new ArgumentException("NBYS log bağlamı gereklidir.", "context");
            logContext = context;
            queryBuilder = new CrudQueryBuilder();
        }

        public BankaTanimRepository(DbClass db)
        {
            if (db == null)
                throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM BankaTanim_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return logContext == null ? db.SelectFromDb(query, "") : new NbysSqlExecutor(logContext, "BankaTanimRepository").Select(query);
        }

        public DataTable SelectAll()
        {
            return Select(new SqlQuery(
                "SELECT * FROM BankaTanim_Table ORDER BY Banka"), "");
        }

        public DataTable SelectByName(string name)
        {
            SqlQuery query = new SqlQuery(
                "SELECT * FROM BankaTanim_Table WHERE Banka=@Banka");
            query.AddParameter("@Banka", name);
            return logContext == null ? db.SelectFromDb(query, "") : new NbysSqlExecutor(logContext, "BankaTanimRepository").Select(query);
        }

        public DataTable SelectByName(string name, SqlTransactionContext transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");
            SqlQuery query = new SqlQuery(
                "SELECT * FROM BankaTanim_Table WHERE Banka=@Banka");
            query.AddParameter("@Banka", name);
            return SelectInTransaction(query, transaction);
        }

        public DataTable SelectGroups()
        {
            return Select(new SqlQuery(
                "SELECT BankaGrup FROM BankaTanim_Table GROUP BY BankaGrup"), "");
        }

        public DataTable SelectGroups(PortalLogContext context)
        {
            SqlQuery query = new SqlQuery(
                "SELECT BankaGrup FROM BankaTanim_Table GROUP BY BankaGrup");
            return new NbysSqlExecutor(context, "BankaTanimRepository").Select(query);
        }

        public DataTable SelectGroups2()
        {
            return Select(new SqlQuery(
                "SELECT BankaGrup2 FROM BankaTanim_Table GROUP BY BankaGrup2"), "");
        }

        public int Insert<T>(T entity)
        {
            return LegacyWriteDb.Insert(queryBuilder.BuildInsert(entity, TableName));
        }

        public bool Update<T>(T entity)
        {
            return LegacyWriteDb.Update2Db(queryBuilder.BuildUpdate(entity, TableName));
        }

        public bool Delete(int id)
        {
            return LegacyWriteDb.DeleteFromDb(queryBuilder.BuildDelete(TableName, id), "");
        }
        private DataTable Select(SqlQuery query, string ignored,
            [System.Runtime.CompilerServices.CallerMemberName] string method = "")
        {
            return logContext == null ? db.SelectFromDb(query, "") : new NbysSqlExecutor(logContext, "BankaTanimRepository").Select(query, method: method);
        }
        private DbClass LegacyWriteDb
        {
            get
            {
                if (logContext != null) throw new InvalidOperationException("Yazma işlemi için transaction gereklidir.");
                return db;
            }
        }
        private DataTable SelectInTransaction(SqlQuery query, SqlTransactionContext transaction,
            [System.Runtime.CompilerServices.CallerMemberName] string method = "")
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            return logContext == null ? transaction.Select(query) : new NbysSqlExecutor(logContext, "BankaTanimRepository").Select(query, transaction, method);
        }
    }
}
