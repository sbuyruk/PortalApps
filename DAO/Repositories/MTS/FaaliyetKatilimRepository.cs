using System.Runtime.CompilerServices;
using Utility.Logging;
using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.MTS
{
    public class FaaliyetKatilimRepository
    {
        private readonly MtsSqlExecutor sql = new MtsSqlExecutor("FaaliyetKatilimRepository");
        private const string TableName = "FaaliyetKatilim_Table";

        public FaaliyetKatilimRepository()
        { }
        public FaaliyetKatilimRepository(PortalLogContext context)
        {
            if (context == null || context.Project != "MTS") throw new ArgumentException("MTS log bağlamı gereklidir.", "context");
            sql = new MtsSqlExecutor(context, "FaaliyetKatilimRepository");
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
        public DataTable SelectAll() { return Read(new SqlQuery("SELECT * FROM " + TableName + " ORDER BY Id"), ""); }

        public DataTable SelectByFaaliyetAndKatilimci(int faaliyetId, int katilimciId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT *
                FROM FaaliyetKatilim_Table
                WHERE FaaliyetId = @FaaliyetId AND KatilimciId = @KatilimciId");
            query.AddParameter("@FaaliyetId", faaliyetId);
            query.AddParameter("@KatilimciId", katilimciId);
            return Read(query, "");
        }

        public DataTable SelectByKatilimciId(int katilimciId)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM FaaliyetKatilim_Table WHERE KatilimciId = @KatilimciId");
            query.AddParameter("@KatilimciId", katilimciId);
            return Read(query, "");
        }

        public DataTable SelectByFaaliyetId(int faaliyetId)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM FaaliyetKatilim_Table WHERE FaaliyetId = @FaaliyetId");
            query.AddParameter("@FaaliyetId", faaliyetId);
            return Read(query, "");
        }
    }
}
