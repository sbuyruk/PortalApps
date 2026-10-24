using Utility.Logging;
using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.Portal
{
    public class ToplantiKatilimRepository
    {
        private readonly PortalSqlExecutor sql = new PortalSqlExecutor("ToplantiKatilimRepository");
        public ToplantiKatilimRepository(PortalLogContext context)
        {
            sql = new PortalSqlExecutor(context, "ToplantiKatilimRepository");
        }
        private DataTable Select(SqlQuery query, [System.Runtime.CompilerServices.CallerMemberName] string method = "")
        {
            return sql.Select(query, method: method);
        }

        public ToplantiKatilimRepository()
        {
        }

        public DataTable SelectById(int id)
        {
            var q = new SqlQuery("SELECT * FROM ToplantiKatilim_Table WHERE Id=@Id");
            q.AddParameter("@Id", id);
            return Select(q);
        }

        public DataTable SelectAll()
        {
            return Select(new SqlQuery("SELECT * FROM ToplantiKatilim_Table"));
        }

        public DataTable SelectByParticipantMeeting(int k, int t)
        {
            var q = new SqlQuery("SELECT * FROM ToplantiKatilim_Table WHERE KatilimciId=@KatilimciId AND ToplantiId=@ToplantiId");
            q.AddParameter("@KatilimciId", k);
            q.AddParameter("@ToplantiId", t);
            return Select(q);
        }

        public DataTable SelectByMeeting(int t)
        {
            var q = new SqlQuery("SELECT * FROM ToplantiKatilim_Table WHERE ToplantiId=@ToplantiId");
            q.AddParameter("@ToplantiId", t);
            return Select(q);
        }
    }
}
