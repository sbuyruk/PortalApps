using DAO.Ortak;
using System;
using System.Data;
using Utility.Logging;

namespace DAO.Repositories.NBYS
{
    public class BagisaVesileOlanTesekkurRepository
    {
        private readonly NbysSqlExecutor sqlReader = new NbysSqlExecutor("BagisaVesileOlanTesekkurRepository");
        private const string TableName = "BagisaVesileOlanTesekkur_Table";

        private readonly CrudQueryBuilder queryBuilder;
        private readonly PortalLogContext context;

        public BagisaVesileOlanTesekkurRepository(PortalLogContext context)
        {
            sqlReader = new NbysSqlExecutor(context, "BagisaVesileOlanTesekkurRepository");
            if (context == null || context.Project != "NBYS") throw new ArgumentException("NBYS log bağlamı gereklidir.", "context");
            this.context = context;
            queryBuilder = new CrudQueryBuilder();
        }

        public BagisaVesileOlanTesekkurRepository()
        {
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery(
                "SELECT * FROM BagisaVesileOlanTesekkur_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return sqlReader.Select(query);
        }

        public DataTable SelectForList()
        {
            if (context != null)
                return sqlReader.Select(new SqlQuery(@"SELECT Id,Adi,Soyadi,
                    VerilmeSebebi,BelgeTarihi,ImzalayanAdiSoyadi,Aciklama,
                    CONVERT(varchar,FORMAT(BelgeTarihi,'dd.MM.yyyy')) BelgeTarihiDDMMYYYY
                    FROM BagisaVesileOlanTesekkur_Table ORDER BY BelgeTarihi DESC,Id DESC"));
            return sqlReader.Select(new SqlQuery(@"SELECT *,
                CONVERT(varchar,FORMAT(BelgeTarihi,'dd.MM.yyyy')) BelgeTarihiDDMMYYYY
                FROM BagisaVesileOlanTesekkur_Table"));
        }

        public DataTable SelectById(int id, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            SqlQuery query = new SqlQuery("SELECT * FROM BagisaVesileOlanTesekkur_Table WITH (UPDLOCK, HOLDLOCK) WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return sqlReader.Select(query, transaction);
        }

        public int Insert<T>(T entity, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            return new NbysTransactionalWriter(context, TableName).Insert(queryBuilder.BuildInsert(entity, TableName,
                new[] { "BelgeTarihi" }), transaction);
        }

        public void Update<T>(T entity, int id, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            new NbysTransactionalWriter(context, TableName).Update(queryBuilder.BuildUpdate(entity, TableName,
                new[] { "BelgeTarihi", "OlusturmaTarihi" }), id, transaction);
        }

        public void Delete(int id, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            new NbysTransactionalWriter(context, TableName).Delete(id, transaction);
        }

        private void RequireTransaction(SqlTransactionContext transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (context == null) throw new InvalidOperationException("Merkezi kayıt işlemi için log bağlamı gereklidir.");
        }

    }
}
