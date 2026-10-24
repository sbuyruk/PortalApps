using Utility.Logging;
using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.NBYS
{
    public class FTKIslemRepository
    {
        private readonly NbysSqlExecutor sqlReader = new NbysSqlExecutor("FTKIslemRepository");

        public FTKIslemRepository()
        {
        }

        public FTKIslemRepository(PortalLogContext context)
        {
            sqlReader = new NbysSqlExecutor(context, "FTKIslemRepository");
            if (context == null || context.Project != "NBYS") throw new ArgumentException("NBYS log bağlamı gereklidir.", "context");
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM FTKIslem_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return Select(query);
        }

        public DataTable SelectByLocation(int provinceId, int districtId)
        {
            SqlQuery query = new SqlQuery(
                "SELECT * FROM FTKIslem_Table WHERE Ili=@Ili AND Ilcesi=@Ilcesi");
            query.AddParameter("@Ili", provinceId);
            query.AddParameter("@Ilcesi", districtId);
            return Select(query);
        }

        private DataTable Select(SqlQuery query,
            [System.Runtime.CompilerServices.CallerMemberName] string method = "")
        {
            return sqlReader.Select(query, method: method);
        }

    }
}
