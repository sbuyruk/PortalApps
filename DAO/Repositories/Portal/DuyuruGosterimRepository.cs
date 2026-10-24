using DAO.Ortak;
using Utility.Logging;
using System.Runtime.CompilerServices;
using System;
using System.Data;

namespace DAO.Repositories.Portal
{
    public class DuyuruGosterimRepository
    {
        private readonly PortalSqlExecutor sql = new PortalSqlExecutor("DuyuruGosterimRepository");
        public DuyuruGosterimRepository(PortalLogContext context)
        {
            sql = new PortalSqlExecutor(context, "DuyuruGosterimRepository");
        }
        private DataTable Select(SqlQuery query, [CallerMemberName] string method = "")
        {
            return sql.Select(query, method: method);
        }

        public DuyuruGosterimRepository()
        {
        }

        public DataTable SelectById(int id)
        {
            var q = new SqlQuery("SELECT * FROM DuyuruGosterim_Table WHERE Id=@Id");
            q.AddParameter("@Id", id);
            return Select(q);
        }

        public DataTable SelectAll()
        {
            return Select(new SqlQuery("SELECT * FROM DuyuruGosterim_Table"));
        }

        public DataTable SelectByDuyuruId(int id)
        {
            var q = new SqlQuery("SELECT * FROM DuyuruGosterim_Table WHERE DuyuruId=@DuyuruId ORDER BY Id DESC");
            q.AddParameter("@DuyuruId", id);
            return Select(q);
        }
    }
}

