using DAO.Ortak;
using Utility.Logging;
using System;
using System.Data;

namespace DAO.Repositories.TBYS
{
    public class OnarimRepository
    {
        private readonly TbysSqlExecutor db;
        private readonly CrudQueryBuilder queryBuilder;

        public OnarimRepository()
        {
            db = new TbysSqlExecutor("OnarimRepository");
            queryBuilder = new CrudQueryBuilder();
        }

        public OnarimRepository(PortalLogContext context)
        {
            db = new TbysSqlExecutor(context, "OnarimRepository");
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM Onarim_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery("SELECT * FROM Onarim_Table"), "");
        }

        public DataTable SelectByTasinmazId(int tasinmazId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT * FROM Onarim_Table
                WHERE TasinmazId=@TasinmazId
                ORDER BY TasinmazId");
            query.AddParameter("@TasinmazId", tasinmazId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAllForDataTable()
        {
            return db.SelectFromDb(new SqlQuery(@"
                SELECT A.Id OnarimId, A.TasinmazId,A.YapilanIs,A.HarcamaUsulu,A.OnayTarihi,
                    Convert(nvarchar,replace (A.Tutar,'.',',')) as Tutar, A.Aciklama,
                    B.Adres, B.Ili,B.Ilcesi, B.Ili+' '+B.Ilcesi IliIlcesi
                FROM Onarim_Table A
                INNER JOIN Tasinmaz_Table B ON B.Id=A.TasinmazId
                ORDER BY OnayTarihi DESC"), "");
        }

        public DataTable SelectByOnarimId(int onarimId)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM Onarim_Table WHERE Id=@Id");
            query.AddParameter("@Id", onarimId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectNext(int onarimId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT * FROM Onarim_Table
                WHERE Id > @Id
                ORDER BY Id");
            query.AddParameter("@Id", onarimId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectPrevious(int onarimId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT * FROM Onarim_Table
                WHERE Id < @Id
                ORDER BY Id");
            query.AddParameter("@Id", onarimId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectMaxId()
        {
            return db.SelectFromDb(new SqlQuery("SELECT MAX(Id) Id FROM Onarim_Table"), "");
        }

        public DataTable SelectMinId()
        {
            return db.SelectFromDb(new SqlQuery("SELECT MIN(Id) Id FROM Onarim_Table"), "");
        }

        public DataTable SelectByTasinmazIdWithAddress(int tasinmazId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT A.Id, A.TasinmazId, A.YapilanIs, convert(decimal(10, 2), A.Tutar) Tutar,
                    A.OnayTarihi, A.Aciklama,A.HarcamaUsulu, B.Ili, B.Ilcesi,B.Adres
                FROM Onarim_Table A
                INNER JOIN Tasinmaz_Table B ON B.Id = A.TasinmazId
                WHERE A.TasinmazId=@TasinmazId");
            query.AddParameter("@TasinmazId", tasinmazId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByTasinmazId(int tasinmazId, SqlTransactionContext transaction, PortalLogContext context)
        {
            SqlQuery query = new SqlQuery("SELECT TOP (1) * FROM Onarim_Table WITH (UPDLOCK, HOLDLOCK) WHERE TasinmazId=@TasinmazId ORDER BY Id");
            query.AddParameter("@TasinmazId", tasinmazId);
            return db.SelectFromDb(query, transaction, context);
        }

        public DataTable SelectAllByTasinmazId(int tasinmazId, SqlTransactionContext transaction, PortalLogContext context)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM Onarim_Table WITH (UPDLOCK, HOLDLOCK) WHERE TasinmazId=@TasinmazId ORDER BY Id");
            query.AddParameter("@TasinmazId", tasinmazId);
            return db.SelectFromDb(query, transaction, context);
        }

        public DataTable SelectById(int id, SqlTransactionContext transaction, PortalLogContext context)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM Onarim_Table WITH (UPDLOCK, HOLDLOCK) WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, transaction, context);
        }

        public bool MoveToTasinmaz(int id, int sourceId, int targetId,
            DateTime changedAt, string changedBy, SqlTransactionContext transaction, PortalLogContext context)
        {
            SqlQuery query = new SqlQuery(@"
                UPDATE Onarim_Table SET TasinmazId=@TargetId,
                    DegistirmeTarihi=@ChangedAt, Degistiren=@ChangedBy
                WHERE Id=@Id AND TasinmazId=@SourceId;
                IF @@ROWCOUNT <> 1 THROW 50001, N'İlişkili kayıt taşınamadı.', 1;");
            query.AddParameter("@Id", id);
            query.AddParameter("@SourceId", sourceId);
            query.AddParameter("@TargetId", targetId);
            query.AddParameter("@ChangedAt", changedAt);
            query.AddParameter("@ChangedBy", changedBy);
            return db.Update2Db(query, transaction, context);
        }

        public bool Delete(int id, SqlTransactionContext transaction, PortalLogContext context)
        {
            return db.DeleteFromDb(queryBuilder.BuildDelete("Onarim_Table", id), transaction, context);
        }

        public int Insert<T>(T entity, SqlTransactionContext transaction, PortalLogContext context)
        {
            return db.Insert(queryBuilder.BuildInsert(entity, "Onarim_Table"), transaction, context);
        }

        public bool Update<T>(T entity, SqlTransactionContext transaction, PortalLogContext context)
        {
            return db.Update2Db(queryBuilder.BuildUpdate(entity, "Onarim_Table"), transaction, context);
        }
    }
}
