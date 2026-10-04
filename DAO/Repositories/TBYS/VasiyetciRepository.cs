using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.TBYS
{
    public class VasiyetciRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public VasiyetciRepository() : this(new DbClass()) { }

        public VasiyetciRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM Vasiyetci_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery("SELECT * FROM Vasiyetci_Table ORDER BY Adi"), "");
        }

        public DataTable SelectByRegion(int bolgeId, int allRegionId, int headquartersRegionId)
        {
            string regionFilter = bolgeId == allRegionId || bolgeId == headquartersRegionId ? string.Empty : " WHERE B.BolgeId=@BolgeId";
            SqlQuery query = new SqlQuery(@"
                SELECT A.*, B.IlAdi, C.IlceAdi, D.KisaAdi Bolge
                FROM Vasiyetci_Table A
                INNER JOIN Il_Table B on A.IkametIli = B.Id
                LEFT JOIN Ilce_Table C on A.IkametIlcesi = C.Id
                INNER JOIN Bolge_Table D on D.Id = B.BolgeId" + regionFilter + @"
                ORDER BY Adi");
            if (!string.IsNullOrEmpty(regionFilter))
                query.AddParameter("@BolgeId", bolgeId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByIdForJson(int id)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT A.*,A.Id VasiyetciId, B.IlAdi, C.IlceAdi
                FROM Vasiyetci_Table A
                LEFT JOIN Il_Table B on A.IkametIli = B.Id
                LEFT JOIN Ilce_Table C on A.IkametIlcesi = C.Id
                WHERE A.Id=@Id
                ORDER BY A.Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAllForDataTable(bool excludeDeceased, int aliveValue)
        {
            string deceasedFilter = excludeDeceased ? " WHERE SAGVEFAT=@SagVefat" : string.Empty;
            SqlQuery query = new SqlQuery(@"
                SELECT A.*,A.Id VasiyetciId, B.IlAdi, C.IlceAdi, D.KisaAdi Bolge
                FROM Vasiyetci_Table A
                LEFT JOIN Il_Table B on A.IkametIli = B.Id
                LEFT JOIN Ilce_Table C on A.IkametIlcesi = C.Id
                LEFT JOIN Bolge_Table D on D.Id = B.BolgeId" + deceasedFilter);
            if (excludeDeceased)
                query.AddParameter("@SagVefat", aliveValue);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByFilters(bool onlyAlive, bool fullTcKimlikNo, bool fullBirthDate, int deceasedValue)
        {
            string tcFilter = fullTcKimlikNo ? " AND TCKimlikNo IS NOT NULL AND TCKimlikNo > 0" : string.Empty;
            string birthFilter = fullBirthDate ? " AND DogumTarihi IS NOT NULL AND DogumTarihi!='' AND DogumTarihi>'01.01.1900'" : string.Empty;
            string aliveFilter = onlyAlive ? " AND SagVefat!=@SagVefat" : string.Empty;
            SqlQuery query = new SqlQuery(@"
                SELECT *
                FROM Vasiyetci_Table A
                WHERE 1>0" + tcFilter + birthFilter + aliveFilter);
            if (onlyAlive)
                query.AddParameter("@SagVefat", deceasedValue);
            return db.SelectFromDb(query, "");
        }

        public int Insert<T>(T entity)
        {
            return db.Insert(queryBuilder.BuildInsert(entity, "Vasiyetci_Table"));
        }

        public bool Update<T>(T entity)
        {
            return db.Update2Db(queryBuilder.BuildUpdate(entity, "Vasiyetci_Table"));
        }

        public bool Delete(int id)
        {
            return db.DeleteFromDb(queryBuilder.BuildDelete("Vasiyetci_Table", id), "");
        }
    }
}
