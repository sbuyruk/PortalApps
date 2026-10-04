using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.TBYS
{
    public class SozlesmeTasinmazRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public SozlesmeTasinmazRepository() : this(new DbClass()) { }

        public SozlesmeTasinmazRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM SozlesmeTasinmaz_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery("SELECT * FROM SozlesmeTasinmaz_Table"), "");
        }

        public DataTable SelectBySozlesmeId(int sozlesmeId)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM SozlesmeTasinmaz_Table WHERE SozlesmeId=@SozlesmeId");
            query.AddParameter("@SozlesmeId", sozlesmeId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByTasinmazId(int tasinmazId)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM SozlesmeTasinmaz_Table WHERE TasinmazId=@TasinmazId");
            query.AddParameter("@TasinmazId", tasinmazId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectBySozlesmeIdReturnList(int sozlesmeId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT ROW_NUMBER() OVER (ORDER BY B.Id) AS Sirano, B.Id, B.Id TasinmazId,
                    B.Adres,B.Adres +' '+ ISNULL(C.BolumNo,'')+' '+ B.Ilcesi+'/'+B.Ili AdresBolumNoIliIlcesi,
                    C.BolumNo,C.Id BolumId
                FROM SozlesmeTasinmaz_Table A
                LEFT JOIN Tasinmaz_Table B ON B.Id=A.TasinmazId
                LEFT JOIN BagimsizBolum_Table C ON C.TasinmazId=B.Id AND C.Id=A.BolumId
                WHERE A.SozlesmeId=@SozlesmeId");
            query.AddParameter("@SozlesmeId", sozlesmeId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectSumMetrekareBySozlesmeId(int sozlesmeId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT A.SozlesmeId,
                    SUM(CASE
                        WHEN B.AltBolum = 1 THEN C.BBNetAlan
                        WHEN B.AltBolum = 0 THEN B.Metrekare
                        ELSE 0
                    END) AS Metrekare
                FROM SozlesmeTasinmaz_Table A
                INNER JOIN Tasinmaz_Table B ON B.Id = A.TasinmazId
                LEFT JOIN BagimsizBolum_Table C ON C.Id = A.BolumId
                WHERE A.SozlesmeId=@SozlesmeId
                GROUP BY A.SozlesmeId");
            query.AddParameter("@SozlesmeId", sozlesmeId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectBySozlesmeIdReturnDataTable(int sozlesmeId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT A.SozlesmeId,A.TasinmazId,A.BolumId, B.Adres, C.BolumNo,
                    '@'+ B.Adres+' '+ISNULL(C.BolumNo,'')+' '+ B.Ili+'/'+B.Ilcesi TasinmazAdresi
                FROM SozlesmeTasinmaz_Table A
                INNER JOIN Tasinmaz_Table B ON B.Id=A.TasinmazId
                LEFT JOIN BagimsizBolum_Table C ON C.Id=A.BolumId
                WHERE SozlesmeId=@SozlesmeId
                ORDER BY A.SozlesmeId,A.TasinmazId,A.BolumId");
            query.AddParameter("@SozlesmeId", sozlesmeId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectBySozlesmeIdReturnDT(int sozlesmeId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT A.SozlesmeId,A.TasinmazId,A.BolumId, B.Adres, C.BolumNo,
                    '@'+ B.Adres+' '+ISNULL(C.BolumNo,'')+' '+ B.Ili+'/'+B.Ilcesi TasinmazAdresi
                FROM SozlesmeTasinmaz_Table A
                INNER JOIN Tasinmaz_Table B ON B.Id=A.TasinmazId
                LEFT JOIN BagimsizBolum_Table C ON C.Id=A.BolumId
                WHERE SozlesmeId=@SozlesmeId
                ORDER BY A.SozlesmeId,A.TasinmazId,A.BolumId");
            query.AddParameter("@SozlesmeId", sozlesmeId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectBySozlesmeIdTasinmazId(int sozlesmeId, int tasinmazId, int bolumId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT * FROM SozlesmeTasinmaz_Table
                WHERE SozlesmeId=@SozlesmeId AND TasinmazId=@TasinmazId AND BolumId=@BolumId");
            query.AddParameter("@SozlesmeId", sozlesmeId);
            query.AddParameter("@TasinmazId", tasinmazId);
            query.AddParameter("@BolumId", bolumId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByBolumId(int bolumId)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM SozlesmeTasinmaz_Table WHERE SozlesmeId=@BolumId");
            query.AddParameter("@BolumId", bolumId);
            return db.SelectFromDb(query, "");
        }

        public bool DeleteBySozlesmeId(int sozlesmeId)
        {
            SqlQuery query = new SqlQuery("DELETE FROM SozlesmeTasinmaz_Table WHERE SozlesmeId=@SozlesmeId");
            query.AddParameter("@SozlesmeId", sozlesmeId);
            return db.DeleteFromDb(query, "");
        }

        public int Insert<T>(T entity)
        {
            return db.Insert(queryBuilder.BuildInsert(entity, "SozlesmeTasinmaz_Table"));
        }

        public bool Update<T>(T entity)
        {
            return db.Update2Db(queryBuilder.BuildUpdate(entity, "SozlesmeTasinmaz_Table"));
        }

        public bool Delete(int id)
        {
            return db.DeleteFromDb(queryBuilder.BuildDelete("SozlesmeTasinmaz_Table", id), "");
        }
    }
}
