using Utility.Logging;
using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.NBYS
{
    public class ArmaganTanimRepository
    {
        private readonly NbysSqlExecutor sqlReader = new NbysSqlExecutor("ArmaganTanimRepository");

        public ArmaganTanimRepository()
        {
        }

        public ArmaganTanimRepository(PortalLogContext context)
        {
            sqlReader = new NbysSqlExecutor(context, "ArmaganTanimRepository");
            if (context == null || context.Project != "NBYS") throw new ArgumentException("NBYS log bağlamı gereklidir.", "context");
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM ArmaganTanim_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return sqlReader.Select(query);
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
            return sqlReader.Select(query);
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
