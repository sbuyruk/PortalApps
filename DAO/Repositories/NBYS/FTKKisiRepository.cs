using DAO.Ortak;
using System;
using System.Collections.Generic;
using System.Data;

namespace DAO.Repositories.NBYS
{
    public class FTKKisiRepository
    {
        private const string TableName = "FTKKisi_Table";
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public FTKKisiRepository() : this(new DbClass()) { }

        public FTKKisiRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM FTKKisi_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAllWithLocation()
        {
            return db.SelectFromDb(new SqlQuery(@"SELECT A.*, B.IlAdi, C.IlceAdi
                FROM FTKKisi_Table A
                LEFT JOIN Il_Table B ON A.Ili=B.Id
                LEFT JOIN Ilce_Table C ON A.Ilcesi=C.Id
                ORDER BY Adi"), "");
        }

        public DataTable SelectByName(string name, string surname)
        {
            SqlQuery query = new SqlQuery(
                "SELECT * FROM FTKKisi_Table WHERE Adi=@Adi AND Soyadi=@Soyadi");
            query.AddParameter("@Adi", name);
            query.AddParameter("@Soyadi", surname);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByIdentityNumber(long identityNumber)
        {
            SqlQuery query = new SqlQuery(
                "SELECT * FROM FTKKisi_Table WHERE TCKimlikNo=@TCKimlikNo");
            query.AddParameter("@TCKimlikNo", identityNumber);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectMembers(int provinceId, int districtId, int operationId,
            bool onlyActive, string activeStatus)
        {
            string operationFilter = operationId < 0 ? string.Empty : " AND A.Id=@FTKIslemId";
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
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectGovernor(int provinceId, int governorshipDistrictId)
        {
            SqlQuery query = new SqlQuery(@"SELECT * FROM FTKKisi_Table
                WHERE Vali=1 AND Ili=@Ili AND Ilcesi=@Ilcesi");
            query.AddParameter("@Ili", provinceId);
            query.AddParameter("@Ilcesi", governorshipDistrictId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectDistrictGovernor(int provinceId, int districtId)
        {
            SqlQuery query = new SqlQuery(@"SELECT * FROM FTKKisi_Table
                WHERE Kaymakam=1 AND Ili=@Ili AND Ilcesi=@Ilcesi");
            query.AddParameter("@Ili", provinceId);
            query.AddParameter("@Ilcesi", districtId);
            return db.SelectFromDb(query, "");
        }

        public bool UpdateMembershipStatus(IList<int> ids, string status)
        {
            List<string> parameters = new List<string>();
            for (int i = 0; i < ids.Count; i++) parameters.Add("@Id" + i);
            SqlQuery query = new SqlQuery(@"UPDATE FTKKisi_Table SET UyelikDurumu=@UyelikDurumu
                WHERE Id IN (" + string.Join(", ", parameters) + ")");
            query.AddParameter("@UyelikDurumu", status);
            for (int i = 0; i < ids.Count; i++) query.AddParameter(parameters[i], ids[i]);
            return db.Update2Db(query);
        }

        public int Insert<T>(T entity) { return db.Insert(queryBuilder.BuildInsert(entity, TableName)); }
        public bool Update<T>(T entity) { return db.Update2Db(queryBuilder.BuildUpdate(entity, TableName)); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete(TableName, id), ""); }
    }
}
