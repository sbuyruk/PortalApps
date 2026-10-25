using System.Runtime.CompilerServices;
using Utility.Logging;
using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.MTS
{
    public class MTSUnvanTanimRepository
    {
        private readonly MtsSqlExecutor sql = new MtsSqlExecutor("MTSUnvanTanimRepository");
        private const string TableName = "MTSUnvanTanim_Table";

        public MTSUnvanTanimRepository()
        { }

        public MTSUnvanTanimRepository(PortalLogContext context)
        {
            if (context == null || context.Project != "MTS") throw new ArgumentException("MTS log bağlamı gereklidir.", "context");
            sql = new MtsSqlExecutor(context, "MTSUnvanTanimRepository");
        }
        private DataTable Read(SqlQuery query, string identifier, [CallerMemberName] string method = "")
        {
            return sql.Select(query, method: method);
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM " + TableName + " WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return Read(query, "");
        }

        public DataTable SelectAll()
        {
            return Read(new SqlQuery("SELECT * FROM " + TableName + " ORDER BY Id"), "");
        }

    }
}
