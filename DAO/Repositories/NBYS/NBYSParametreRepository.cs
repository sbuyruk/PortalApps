using DAO.Ortak;
using System;
using System.Data;
using Utility.Logging;

namespace DAO.Repositories.NBYS
{
    public class NBYSParametreRepository
    {
        private const string TableName = "NBYSParametre_Table";

        private readonly NbysSqlExecutor sql = new NbysSqlExecutor("NBYSParametreRepository");
        private readonly CrudQueryBuilder queryBuilder;

        public NBYSParametreRepository()
        {
            queryBuilder = new CrudQueryBuilder();
        }

        public NBYSParametreRepository(PortalLogContext context)
        {
            sql = new NbysSqlExecutor(context, "NBYSParametreRepository");
            queryBuilder = new CrudQueryBuilder();
        }

        private DataTable Select(SqlQuery query, [System.Runtime.CompilerServices.CallerMemberName] string method = "")
        {
            return sql.Select(query, method: method);
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM NBYSParametre_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return Select(query);
        }

        public DataTable SelectAll()
        {
            return Select(new SqlQuery(
                "SELECT * FROM NBYSParametre_Table ORDER BY Sira,Deger"));
        }

        public DataTable SelectByGroup(string group)
        {
            SqlQuery query = new SqlQuery(@"SELECT * FROM NBYSParametre_Table
                WHERE Grup=@Grup ORDER BY Sira,Deger");
            query.AddParameter("@Grup", group);
            return Select(query);
        }

        public DataTable SelectByGroupAndKey(string group, string key)
        {
            SqlQuery query = new SqlQuery(@"SELECT * FROM NBYSParametre_Table
                WHERE Grup=@Grup AND Anahtar=@Anahtar ORDER BY Sira,Anahtar");
            query.AddParameter("@Grup", group);
            query.AddParameter("@Anahtar", key);
            return Select(query);
        }

        public DataTable SelectByGroupKeyAndValue(string group, string key, string value)
        {
            SqlQuery query = new SqlQuery(@"SELECT * FROM NBYSParametre_Table
                WHERE Grup=@Grup AND Anahtar=@Anahtar AND Deger=@Deger
                ORDER BY Sira,Anahtar");
            query.AddParameter("@Grup", group);
            query.AddParameter("@Anahtar", key);
            query.AddParameter("@Deger", value);
            return Select(query);
        }

        public DataTable SelectById(int id, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            SqlQuery query = new SqlQuery("SELECT * FROM NBYSParametre_Table WITH (UPDLOCK, HOLDLOCK) WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return sql.Select(query, transaction);
        }

        public DataTable SelectDuplicate(string group, string key, string value, int excludedId,
            SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            SqlQuery query = new SqlQuery(@"SELECT * FROM NBYSParametre_Table WITH (UPDLOCK, HOLDLOCK)
                WHERE Grup=@Grup AND Anahtar=@Anahtar AND Deger=@Deger AND Id<>@ExcludedId");
            query.AddParameter("@Grup", group);
            query.AddParameter("@Anahtar", key);
            query.AddParameter("@Deger", value);
            query.AddParameter("@ExcludedId", excludedId);
            return sql.Select(query, transaction);
        }

        public int Insert<T>(T entity, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            return sql.Insert(queryBuilder.BuildInsert(entity, TableName), transaction);
        }

        public bool Update<T>(T entity, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            return sql.Write(queryBuilder.BuildUpdate(entity, TableName), "UPDATE", transaction);
        }

        public bool Delete(int id, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            return sql.Write(queryBuilder.BuildDelete(TableName, id), "DELETE", transaction);
        }

        private void RequireTransaction(SqlTransactionContext transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction", "İşlem bağlamı gereklidir.");
            if (!sql.HasLogContext) throw new InvalidOperationException("Bu işlem için NBYS log bağlamı gereklidir.");
        }

    }
}
