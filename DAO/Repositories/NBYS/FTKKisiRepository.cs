using Utility.Logging;
using DAO.Ortak;
using System;
using System.Collections.Generic;
using System.Data;

namespace DAO.Repositories.NBYS
{
    public class FTKKisiRepository
    {
        private readonly NbysSqlExecutor sqlReader = new NbysSqlExecutor("FTKKisiRepository");

        public FTKKisiRepository()
        {
        }

        // Reads use centralized SQL tracing when a context is supplied.
        public FTKKisiRepository(PortalLogContext context)
        {
            sqlReader = new NbysSqlExecutor(context, "FTKKisiRepository");
            if (context == null || context.Project != "NBYS")
                throw new ArgumentException("NBYS log bağlamı gereklidir.", "context");
        }

        private DataTable Select(SqlQuery query,
            [System.Runtime.CompilerServices.CallerMemberName] string method = "")
        {
            return sqlReader.Select(query, method: method);
        }
        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM FTKKisi_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return Select(query);
        }

        public DataTable SelectAllWithLocation()
        {
            return Select(new SqlQuery(@"SELECT A.*, B.IlAdi, C.IlceAdi
                FROM FTKKisi_Table A
                LEFT JOIN Il_Table B ON A.Ili=B.Id
                LEFT JOIN Ilce_Table C ON A.Ilcesi=C.Id
                ORDER BY Adi"));
        }

        public DataTable SelectByName(string name, string surname)
        {
            SqlQuery query = new SqlQuery(
                "SELECT * FROM FTKKisi_Table WHERE Adi=@Adi AND Soyadi=@Soyadi");
            query.AddParameter("@Adi", name);
            query.AddParameter("@Soyadi", surname);
            return Select(query);
        }

        public DataTable SelectByIdentityNumber(long identityNumber)
        {
            SqlQuery query = new SqlQuery(
                "SELECT * FROM FTKKisi_Table WHERE TCKimlikNo=@TCKimlikNo");
            query.AddParameter("@TCKimlikNo", identityNumber);
            return Select(query);
        }

        public DataTable SelectByName(string name, string surname, int excludedId)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM FTKKisi_Table WHERE Adi=@Adi AND Soyadi=@Soyadi AND Id<>@Id");
            query.AddParameter("@Adi", name);
            query.AddParameter("@Soyadi", surname);
            query.AddParameter("@Id", excludedId);
            return Select(query);
        }

        public DataTable SelectMembers(int provinceId, int districtId, int operationId,
            bool onlyActive, string activeStatus)
        {
            string operationFilter = operationId < 0 || (sqlReader.HasLogContext && operationId == 0) ? string.Empty : " AND A.Id=@FTKIslemId";
            string activeFilter = onlyActive ? " AND UyelikDurumu=@UyelikDurumu" : string.Empty;
            SqlQuery query = new SqlQuery(@"SELECT A.Id FTKIslemId, C.Id Id, C.Id FTKKisiId, A.*, C.*
                FROM FTKKisi_Table C
                LEFT JOIN FTKIslem_Table A ON A.Ili=C.Ili AND A.Ilcesi=C.Ilcesi
                WHERE C.Ili=@Ili AND C.Ilcesi=@Ilcesi" + operationFilter + activeFilter + @"
                ORDER BY C.FTKGorevi,C.Adi,C.Soyadi");
            query.AddParameter("@Ili", provinceId);
            query.AddParameter("@Ilcesi", districtId);
            if (operationId >= 0) query.AddParameter("@FTKIslemId", operationId);
            if (onlyActive) query.AddParameter("@UyelikDurumu", activeStatus);
            return Select(query);
        }

        public DataTable SelectGovernor(int provinceId, int governorshipDistrictId)
        {
            SqlQuery query = new SqlQuery(@"SELECT * FROM FTKKisi_Table
                WHERE Vali=1 AND Ili=@Ili AND Ilcesi=@Ilcesi");
            query.AddParameter("@Ili", provinceId);
            query.AddParameter("@Ilcesi", governorshipDistrictId);
            return Select(query);
        }

        public DataTable SelectDistrictGovernor(int provinceId, int districtId)
        {
            SqlQuery query = new SqlQuery(@"SELECT * FROM FTKKisi_Table
                WHERE Kaymakam=1 AND Ili=@Ili AND Ilcesi=@Ilcesi");
            query.AddParameter("@Ili", provinceId);
            query.AddParameter("@Ilcesi", districtId);
            return Select(query);
        }

    }
}
