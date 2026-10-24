using DAO.Ortak;
using System;
using System.Data;
using Utility.Logging;

namespace DAO.Repositories.NBYS
{
    public class BankaTanimRepository
    {
        private readonly NbysSqlExecutor sqlReader = new NbysSqlExecutor("BankaTanimRepository");

        public BankaTanimRepository()
        {
        }

        public BankaTanimRepository(PortalLogContext context)
        {
            sqlReader = new NbysSqlExecutor(context, "BankaTanimRepository");
            if (context == null || context.Project != "NBYS") throw new ArgumentException("NBYS log bağlamı gereklidir.", "context");
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM BankaTanim_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return sqlReader.Select(query);
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
            return sqlReader.Select(query);
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

        private DataTable Select(SqlQuery query, string ignored,
            [System.Runtime.CompilerServices.CallerMemberName] string method = "")
        {
            return sqlReader.Select(query, method: method);
        }

        private DataTable SelectInTransaction(SqlQuery query, SqlTransactionContext transaction,
            [System.Runtime.CompilerServices.CallerMemberName] string method = "")
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            return sqlReader.Select(query, transaction, method);
        }
    }
}
