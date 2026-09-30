using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.NBYS
{
    public class FTKRepository
    {
        private const string TableName = "FTK_Table";
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public FTKRepository() : this(new DbClass()) { }

        public FTKRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM FTK_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectLatest(int? regionId, int? provinceId, int? districtId,
            bool districtsOnly, int governorshipDistrictId, DateTime? establishedSince,
            DateTime? updatedSince)
        {
            string filters = string.Empty;
            if (establishedSince.HasValue) filters += " AND A.KurulusTarihi>=@KurulusTarihi";
            if (updatedSince.HasValue) filters += " AND A.GuncellemeTarihi>=@GuncellemeTarihi";
            if (regionId.HasValue) filters += " AND B.BolgeId=@BolgeId";
            if (provinceId.HasValue) filters += " AND A.Ili=@Ili";
            if (districtsOnly) filters += " AND A.Ilcesi<>@Ilcesi";
            else if (districtId.HasValue) filters += " AND A.Ilcesi=@Ilcesi";

            SqlQuery query = new SqlQuery(@"SELECT A.Id FTKId,D.KisaAdi Bolge,* FROM FTK_Table A
                LEFT JOIN Il_Table B ON B.Id=A.Ili
                LEFT JOIN Ilce_Table C ON C.Id=A.Ilcesi AND C.IlId=A.Ili
                LEFT JOIN Bolge_Table D ON D.Id=B.BolgeId
                WHERE Sayac=(SELECT MAX(Sayac) FROM FTK_Table WHERE FTKIslemId=A.FTKIslemId)" + filters + @"
                ORDER BY Ili,Ilcesi,FTKIslemId,KartNo");
            if (establishedSince.HasValue) query.AddParameter("@KurulusTarihi", establishedSince.Value);
            if (updatedSince.HasValue) query.AddParameter("@GuncellemeTarihi", updatedSince.Value);
            if (regionId.HasValue) query.AddParameter("@BolgeId", regionId.Value);
            if (provinceId.HasValue) query.AddParameter("@Ili", provinceId.Value);
            if (districtsOnly || districtId.HasValue)
                query.AddParameter("@Ilcesi", districtsOnly ? governorshipDistrictId : districtId.Value);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByLocationAndUpdateDate(int provinceId, int districtId, DateTime updateDate)
        {
            SqlQuery query = new SqlQuery(@"SELECT * FROM FTK_Table
                WHERE Ili=@Ili AND Ilcesi=@Ilcesi AND GuncellemeTarihi=@GuncellemeTarihi");
            query.AddParameter("@Ili", provinceId);
            query.AddParameter("@Ilcesi", districtId);
            query.AddParameter("@GuncellemeTarihi", updateDate);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectMaxCounter(int provinceId, int districtId)
        {
            SqlQuery query = new SqlQuery(
                "SELECT MAX(Sayac) Sayac FROM FTK_Table WHERE Ili=@Ili AND Ilcesi=@Ilcesi");
            query.AddParameter("@Ili", provinceId);
            query.AddParameter("@Ilcesi", districtId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectEstablishedProvinceCount(int regionId, int governorshipDistrictId)
        {
            SqlQuery query = new SqlQuery(@"SELECT BolgeId,Ili,Ilcesi FROM FTK_Table
                WHERE BolgeId=@BolgeId AND Ilcesi=@Ilcesi GROUP BY BolgeId,Ili,Ilcesi");
            query.AddParameter("@BolgeId", regionId);
            query.AddParameter("@Ilcesi", governorshipDistrictId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectEstablishedDistrictCount(int regionId)
        {
            SqlQuery query = new SqlQuery(@"SELECT BolgeId,Ili,Ilcesi FROM FTK_Table
                WHERE Ilcesi>0 AND BolgeId=@BolgeId GROUP BY BolgeId,Ili,Ilcesi");
            query.AddParameter("@BolgeId", regionId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectUpdatedLocationCount(int regionId, DateTime updatedSince)
        {
            SqlQuery query = new SqlQuery(@"SELECT BolgeId,Ili,Ilcesi FROM FTK_Table A
                WHERE A.Sayac=(SELECT MAX(Sayac) FROM FTK_Table WHERE FTKIslemId=A.FTKIslemId)
                AND BolgeId=@BolgeId AND GuncellemeTarihi>=@GuncellemeTarihi
                GROUP BY BolgeId,Ili,Ilcesi");
            query.AddParameter("@BolgeId", regionId);
            query.AddParameter("@GuncellemeTarihi", updatedSince);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectEstablishedLocationCount(int regionId, DateTime establishedSince)
        {
            SqlQuery query = new SqlQuery(@"SELECT BolgeId,Ili,Ilcesi FROM FTK_Table
                WHERE BolgeId=@BolgeId AND KurulusTarihi>=@KurulusTarihi
                GROUP BY BolgeId,Ili,Ilcesi");
            query.AddParameter("@BolgeId", regionId);
            query.AddParameter("@KurulusTarihi", establishedSince);
            return db.SelectFromDb(query, "");
        }

        public bool DeleteByOperationAndCounter(int operationId, int counter)
        {
            SqlQuery query = new SqlQuery(
                "DELETE FROM FTK_Table WHERE FTKIslemId=@FTKIslemId AND Sayac=@Sayac");
            query.AddParameter("@FTKIslemId", operationId);
            query.AddParameter("@Sayac", counter);
            return db.DeleteFromDb(query, "");
        }

        public DataTable SelectProvincesWithoutFTK(int? regionId, int? provinceId, int governorshipDistrictId)
        {
            string filters = regionId.HasValue ? " AND A.BolgeId=@BolgeId" : string.Empty;
            if (provinceId.HasValue) filters += " AND A.Id=@IlId";
            SqlQuery query = new SqlQuery(@"SELECT A.Id IlId,A.BolgeId,C.Adi Bolge,A.IlAdi
                FROM Il_Table A LEFT JOIN Bolge_Table C ON C.Id=A.BolgeId
                WHERE A.Id BETWEEN 1 AND 81 AND A.IlAdi<>'Bos'" + filters + @"
                AND A.Id NOT IN (SELECT Ili FROM FTK_Table WHERE Ilcesi=@Ilcesi)
                ORDER BY A.BolgeId,A.Id");
            if (regionId.HasValue) query.AddParameter("@BolgeId", regionId.Value);
            if (provinceId.HasValue) query.AddParameter("@IlId", provinceId.Value);
            query.AddParameter("@Ilcesi", governorshipDistrictId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectDistrictsWithoutFTK(int? regionId, int? provinceId, string centralDistrictName)
        {
            string filters = regionId.HasValue ? " AND B.BolgeId=@BolgeId" : string.Empty;
            if (provinceId.HasValue) filters += " AND B.Id=@IlId";
            SqlQuery query = new SqlQuery(@"SELECT A.Id IlceId,A.IlceAdi,C.Adi Bolge,B.Id IlId,B.IlAdi
                FROM Ilce_Table A INNER JOIN Il_Table B ON B.Id=A.IlId
                LEFT JOIN Bolge_Table C ON C.Id=B.BolgeId
                WHERE A.IlceAdi<>@Merkez AND B.Id BETWEEN 1 AND 81 AND B.IlAdi<>'Bos'
                AND A.Id NOT IN (SELECT Ilcesi FROM FTK_Table WHERE Ilcesi>0)" + filters + @"
                ORDER BY B.BolgeId,B.Id,A.Id");
            query.AddParameter("@Merkez", centralDistrictName);
            if (regionId.HasValue) query.AddParameter("@BolgeId", regionId.Value);
            if (provinceId.HasValue) query.AddParameter("@IlId", provinceId.Value);
            return db.SelectFromDb(query, "");
        }

        public int Insert<T>(T entity) { return db.Insert(queryBuilder.BuildInsert(entity, TableName)); }
        public bool Update<T>(T entity) { return db.Update2Db(queryBuilder.BuildUpdate(entity, TableName)); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete(TableName, id), ""); }
    }
}
