using DAO.Ortak;
using System;
using System.Data;
using Utility.Logging;

namespace DAO.Repositories.TBYS
{
    public class SozlesmeTasinmazRepository
    {
        private readonly TbysSqlExecutor db;
        private readonly CrudQueryBuilder queryBuilder;

        public SozlesmeTasinmazRepository() : this(new DbClass()) { }

        public SozlesmeTasinmazRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = new TbysSqlExecutor(db, "SozlesmeTasinmazRepository");
            queryBuilder = new CrudQueryBuilder();
        }

        public SozlesmeTasinmazRepository(PortalLogContext context)
        {
            db = new TbysSqlExecutor(context, "SozlesmeTasinmazRepository");
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectByBolumId(int bolumId, SqlTransactionContext transaction, PortalLogContext context)
        {
            SqlQuery query = new SqlQuery("SELECT TOP (1) * FROM SozlesmeTasinmaz_Table WITH (UPDLOCK, HOLDLOCK) WHERE BolumId=@BolumId");
            query.AddParameter("@BolumId", bolumId);
            return db.SelectFromDb(query, transaction, context);
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

        public DataTable SelectByTasinmazId(int tasinmazId, SqlTransactionContext transaction, PortalLogContext context)
        {
            SqlQuery query = new SqlQuery("SELECT TOP (1) * FROM SozlesmeTasinmaz_Table WITH (UPDLOCK, HOLDLOCK) WHERE TasinmazId=@TasinmazId ORDER BY Id");
            query.AddParameter("@TasinmazId", tasinmazId);
            return db.SelectFromDb(query, transaction, context);
        }

        public DataTable SelectAllByTasinmazId(int tasinmazId, SqlTransactionContext transaction, PortalLogContext context)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM SozlesmeTasinmaz_Table WITH (UPDLOCK, HOLDLOCK) WHERE TasinmazId=@TasinmazId ORDER BY Id");
            query.AddParameter("@TasinmazId", tasinmazId);
            return db.SelectFromDb(query, transaction, context);
        }

        public DataTable SelectById(int id, SqlTransactionContext transaction, PortalLogContext context)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM SozlesmeTasinmaz_Table WITH (UPDLOCK, HOLDLOCK) WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, transaction, context);
        }

        public bool MoveToTasinmaz(int id, int sourceId, int targetId, object bolumId,
            DateTime changedAt, string changedBy, SqlTransactionContext transaction, PortalLogContext context)
        {
            SqlQuery query = new SqlQuery(@"
                UPDATE SozlesmeTasinmaz_Table SET TasinmazId=@TargetId, BolumId=@BolumId,
                    DegistirmeTarihi=@ChangedAt, Degistiren=@ChangedBy
                WHERE Id=@Id AND TasinmazId=@SourceId;
                IF @@ROWCOUNT <> 1 THROW 50001, N'İlişkili kayıt taşınamadı.', 1;");
            query.AddParameter("@Id", id);
            query.AddParameter("@SourceId", sourceId);
            query.AddParameter("@TargetId", targetId);
            query.AddParameter("@BolumId", bolumId);
            query.AddParameter("@ChangedAt", changedAt);
            query.AddParameter("@ChangedBy", changedBy);
            return db.Update2Db(query, transaction, context);
        }
    }
}
