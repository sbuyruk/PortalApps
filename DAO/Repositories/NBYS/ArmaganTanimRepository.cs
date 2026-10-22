using Utility.Logging;
using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.NBYS
{
    public class ArmaganTanimRepository
    {
        public bool HasLogContext { get { return logContext != null; } }
        private const string TableName = "ArmaganTanim_Table";
        private readonly DbClass db;
        private readonly PortalLogContext logContext;
        private readonly CrudQueryBuilder queryBuilder;

        public ArmaganTanimRepository() : this(new DbClass())
        {
        }

        public ArmaganTanimRepository(SqlTransactionContext transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            queryBuilder = new CrudQueryBuilder();
        }

        public ArmaganTanimRepository(PortalLogContext context)
        {
            if (context == null || context.Project != "NBYS") throw new ArgumentException("NBYS log bağlamı gereklidir.", "context");
            logContext = context;
            queryBuilder = new CrudQueryBuilder();
        }

        public ArmaganTanimRepository(DbClass db)
        {
            if (db == null)
                throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM ArmaganTanim_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return logContext == null ? db.SelectFromDb(query, "") : new NbysSqlExecutor(logContext, "ArmaganTanimRepository").Select(query);
        }

        public DataTable SelectById(int id, PortalLogContext context)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM ArmaganTanim_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return PortalSqlReadExecutor.Select(query, context, "ArmaganTanimRepository.SelectById");
        }

        public DataTable SelectAll()
        {
            return Select(new SqlQuery("SELECT * FROM ArmaganTanim_Table"), "");
        }

        public DataTable SelectActive()
        {
            return Select(new SqlQuery(
                "SELECT * FROM ArmaganTanim_Table WHERE Aktif=1"), "");
        }

        public DataTable SelectByAmount(decimal amount, bool isCorporate)
        {
            string limits = isCorporate
                ? "TuzelKisiAltLimit<=@Amount AND TuzelKisiUstLimit>=@Amount"
                : "OzelKisiAltLimit<=@Amount AND OzelKisiUstLimit>=@Amount";
            SqlQuery query = new SqlQuery(
                "SELECT * FROM ArmaganTanim_Table WHERE Aktif=1 AND " + limits);
            query.AddParameter("@Amount", amount);
            return logContext == null ? db.SelectFromDb(query, "") : new NbysSqlExecutor(logContext, "ArmaganTanimRepository").Select(query);
        }

        public DataTable SelectByAmount(
            decimal amount,
            bool isCorporate,
            SqlTransactionContext transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");
            string limits = isCorporate
                ? "TuzelKisiAltLimit<=@Amount AND TuzelKisiUstLimit>=@Amount"
                : "OzelKisiAltLimit<=@Amount AND OzelKisiUstLimit>=@Amount";
            SqlQuery query = new SqlQuery(
                "SELECT * FROM ArmaganTanim_Table WHERE Aktif=1 AND " + limits);
            query.AddParameter("@Amount", amount);
            return SelectInTransaction(query, transaction);
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
            return logContext == null ? db.SelectFromDb(query, "") : new NbysSqlExecutor(logContext, "ArmaganTanimRepository").Select(query, method: method);
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
            return logContext == null ? transaction.Select(query) : new NbysSqlExecutor(logContext, "ArmaganTanimRepository").Select(query, transaction, method);
        }
    }
}
