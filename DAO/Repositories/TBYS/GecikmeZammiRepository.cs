using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.TBYS
{
    public class GecikmeZammiRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public GecikmeZammiRepository() : this(new DbClass()) { }

        public GecikmeZammiRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM GecikmeZammi_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery(@"
                SELECT *
                FROM GecikmeZammi_Table
                ORDER BY BaslangicTarihi DESC"), "");
        }

        public DataTable SelectChangedBetween(DateTime ilkOdemeTarihi, DateTime sonOdemeTarihi)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT *
                FROM GecikmeZammi_Table
                WHERE BaslangicTarihi <= @SonOdemeTarihi AND ISNULL(BitisTarihi,@SonOdemeTarihi) >= @IlkOdemeTarihi
                ORDER BY BaslangicTarihi");
            query.AddParameter("@IlkOdemeTarihi", ilkOdemeTarihi);
            query.AddParameter("@SonOdemeTarihi", sonOdemeTarihi);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByStartDate(DateTime baslangicTarihi)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT *
                FROM GecikmeZammi_Table
                WHERE BaslangicTarihi <= @BaslangicTarihi
                ORDER BY BaslangicTarihi DESC");
            query.AddParameter("@BaslangicTarihi", baslangicTarihi);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByDateRange(DateTime vadeBaslangicTarihi, DateTime vadeBitisTarihi)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT *
                FROM GecikmeZammi_Table
                WHERE BaslangicTarihi <= @VadeBitisTarihi AND (BitisTarihi is null OR BitisTarihi>=@VadeBaslangicTarihi)
                ORDER BY BaslangicTarihi DESC");
            query.AddParameter("@VadeBaslangicTarihi", vadeBaslangicTarihi);
            query.AddParameter("@VadeBitisTarihi", vadeBitisTarihi);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectLatestByDate(DateTime sonOdemeTarihi)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT *
                FROM GecikmeZammi_Table
                WHERE BaslangicTarihi <= @SonOdemeTarihi
                ORDER BY BaslangicTarihi DESC");
            query.AddParameter("@SonOdemeTarihi", sonOdemeTarihi);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectPrevious(DateTime tarih)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT *
                FROM GecikmeZammi_Table
                WHERE BaslangicTarihi < @Tarih
                ORDER BY BaslangicTarihi DESC");
            query.AddParameter("@Tarih", tarih);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectNext(DateTime tarih)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT *
                FROM GecikmeZammi_Table
                WHERE BaslangicTarihi > @Tarih
                ORDER BY BaslangicTarihi");
            query.AddParameter("@Tarih", tarih);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectLatest()
        {
            return db.SelectFromDb(new SqlQuery(@"
                SELECT *
                FROM GecikmeZammi_Table
                ORDER BY BaslangicTarihi DESC"), "");
        }

        public int Insert<T>(T entity)
        {
            return db.Insert(queryBuilder.BuildInsert(entity, "GecikmeZammi_Table"));
        }

        public bool Update<T>(T entity)
        {
            return db.Update2Db(queryBuilder.BuildUpdate(entity, "GecikmeZammi_Table"));
        }

        public bool Delete(int id)
        {
            return db.DeleteFromDb(queryBuilder.BuildDelete("GecikmeZammi_Table", id), "");
        }
    }
}
