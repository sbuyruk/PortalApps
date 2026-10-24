using DAO.Ortak;
using Utility.Logging;
using System.Runtime.CompilerServices;
using System;
using System.Data;

namespace DAO.Repositories.Portal
{
    public class DuyuruOkumaRepository
    {
        private readonly PortalSqlExecutor sql = new PortalSqlExecutor("DuyuruOkumaRepository");
        public DuyuruOkumaRepository(PortalLogContext context)
        {
            sql = new PortalSqlExecutor(context, "DuyuruOkumaRepository");
        }
        private DataTable Select(SqlQuery query, [CallerMemberName] string method = "")
        {
            return sql.Select(query, method: method);
        }

        public DuyuruOkumaRepository()
        {
        }

        public DataTable SelectById(int id)
        {
            var q = new SqlQuery("SELECT * FROM DuyuruOkuma_Table WHERE Id=@Id");
            q.AddParameter("@Id", id);
            return Select(q);
        }

        public DataTable SelectAll()
        {
            return Select(new SqlQuery("SELECT * FROM DuyuruOkuma_Table"));
        }

        public DataTable SelectByDuyuruId(int id)
        {
            var q = new SqlQuery("SELECT A.Id,B.Adi,B.Soyadi,A.OkumaTarihi FROM DuyuruOkuma_Table A INNER JOIN Personel_Table B ON B.Id=A.PersonelId WHERE A.DuyuruId=@DuyuruId ORDER BY OkumaTarihi DESC");
            q.AddParameter("@DuyuruId", id);
            return Select(q);
        }
    }
}

