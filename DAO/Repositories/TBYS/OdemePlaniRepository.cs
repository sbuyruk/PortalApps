using DAO.Ortak;
using Utility.Logging;
using System;
using System.Data;

namespace DAO.Repositories.TBYS
{
    public class OdemePlaniRepository
    {
        private readonly TbysSqlExecutor db;
        private readonly CrudQueryBuilder queryBuilder;

        public OdemePlaniRepository()
        {
            db = new TbysSqlExecutor("OdemePlaniRepository");
            queryBuilder = new CrudQueryBuilder();
        }

        public OdemePlaniRepository(PortalLogContext context)
        {
            db = new TbysSqlExecutor(context, "OdemePlaniRepository");
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            SqlQuery q = new SqlQuery("SELECT * FROM OdemePlani_Table WHERE Id=@Id");
            q.AddParameter("@Id", id);
            return db.SelectFromDb(q, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery("SELECT * FROM OdemePlani_Table"), "");
        }

        public DataTable SelectBySozlesmeId(int id)
        {
            SqlQuery q = new SqlQuery("SELECT * FROM OdemePlani_Table WHERE SozlesmeId=@SozlesmeId ORDER BY Sira");
            q.AddParameter("@SozlesmeId", id);
            return db.SelectFromDb(q, "");
        }

        public DataTable SelectBySozlesmeIdSira(int id, int sira)
        {
            SqlQuery q = new SqlQuery("SELECT * FROM OdemePlani_Table WHERE SozlesmeId=@SozlesmeId AND Sira=@Sira");
            q.AddParameter("@SozlesmeId", id);
            q.AddParameter("@Sira", sira);
            return db.SelectFromDb(q, "");
        }

        public DataTable SelectBySozlesmeIdOdemeTarihi(int id, DateTime tarih)
        {
            SqlQuery q = new SqlQuery("SELECT * FROM OdemePlani_Table WHERE Sira!=0 AND SozlesmeId=@SozlesmeId AND @OdemeTarihi >= VadeBasTar AND @OdemeTarihi < DATEADD(month,1,VadeBasTar) ORDER BY VadeBasTar");
            q.AddParameter("@SozlesmeId", id);
            q.AddParameter("@OdemeTarihi", tarih);
            return db.SelectFromDb(q, "");
        }
        public DataTable SelectBySozlesmeIdOdemeTarihi(int id, DateTime tarih, SqlTransactionContext transaction, PortalLogContext context)
        {
            SqlQuery q = new SqlQuery("SELECT * FROM OdemePlani_Table WITH (UPDLOCK, HOLDLOCK) WHERE Sira!=0 AND SozlesmeId=@SozlesmeId AND @OdemeTarihi >= VadeBasTar AND @OdemeTarihi < DATEADD(month,1,VadeBasTar) ORDER BY VadeBasTar");
            q.AddParameter("@SozlesmeId", id);
            q.AddParameter("@OdemeTarihi", tarih);
            return db.SelectFromDb(q, transaction, context);
        }

        public DataTable SelectExists(int id)
        {
            SqlQuery q = new SqlQuery("SELECT * FROM OdemePlani_Table WHERE SozlesmeId=@SozlesmeId");
            q.AddParameter("@SozlesmeId", id);
            return db.SelectFromDb(q, "");
        }

        public DataTable SelectLastBySozlesmeId(int id)
        {
            SqlQuery q = new SqlQuery("SELECT * FROM OdemePlani_Table WHERE SozlesmeId=@SozlesmeId ORDER BY VadeBasTar");
            q.AddParameter("@SozlesmeId", id);
            return db.SelectFromDb(q, "");
        }

        public DataTable SelectFirstBySozlesmeId(int id)
        {
            SqlQuery q = new SqlQuery("SELECT * FROM OdemePlani_Table WHERE SozlesmeId=@SozlesmeId AND Sira=1");
            q.AddParameter("@SozlesmeId", id);
            return db.SelectFromDb(q, "");
        }

        public DataTable SelectById(int id, SqlTransactionContext transaction, PortalLogContext context)
        {
            SqlQuery q = new SqlQuery("SELECT * FROM OdemePlani_Table WITH (UPDLOCK, HOLDLOCK) WHERE Id=@Id");
            q.AddParameter("@Id", id);
            return db.SelectFromDb(q, transaction, context);
        }

        public DataTable SelectBySozlesmeId(int id, SqlTransactionContext transaction, PortalLogContext context)
        {
            SqlQuery q = new SqlQuery("SELECT * FROM OdemePlani_Table WITH (UPDLOCK, HOLDLOCK) WHERE SozlesmeId=@SozlesmeId ORDER BY Sira");
            q.AddParameter("@SozlesmeId", id);
            return db.SelectFromDb(q, transaction, context);
        }

        public int Insert<T>(T entity, SqlTransactionContext transaction, PortalLogContext context)
        {
            return db.Insert(queryBuilder.BuildInsert(entity, "OdemePlani_Table"), transaction, context);
        }

        public bool Update<T>(T entity, SqlTransactionContext transaction, PortalLogContext context)
        {
            return db.Update2Db(queryBuilder.BuildUpdate(entity, "OdemePlani_Table"), transaction, context);
        }

        public bool Delete(int id, SqlTransactionContext transaction, PortalLogContext context)
        {
            return db.DeleteFromDb(queryBuilder.BuildDelete("OdemePlani_Table", id), transaction, context);
        }

        public bool DeleteBySozlesmeId(int id, SqlTransactionContext transaction, PortalLogContext context)
        {
            SqlQuery q = new SqlQuery("DELETE OdemePlani_Table WHERE SozlesmeId=@SozlesmeId");
            q.AddParameter("@SozlesmeId", id);
            return db.DeleteFromDb(q, transaction, context);
        }

        public bool HasPaymentLinks(int contractId, SqlTransactionContext transaction, PortalLogContext context)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT 1 AS InUse WHERE
                    EXISTS (SELECT 1 FROM Odeme_Table WITH (UPDLOCK, HOLDLOCK) WHERE SozlesmeId=@Id)
                    OR EXISTS (SELECT 1 FROM OdemePlani_Table P WITH (UPDLOCK, HOLDLOCK)
                        INNER JOIN Odeme_Table O WITH (UPDLOCK, HOLDLOCK) ON O.OdemePlaniId=P.Id
                        WHERE P.SozlesmeId=@Id)
                    OR EXISTS (SELECT 1 FROM OdemePlani_Table WITH (UPDLOCK, HOLDLOCK)
                        WHERE SozlesmeId=@Id AND ISNULL(OdenenTutar,0)<>0)
                    OR EXISTS (SELECT 1 FROM OdemeAyrinti_Table WITH (UPDLOCK, HOLDLOCK)
                        WHERE SozlesmeId=@Id AND ISNULL(OdemeId,0)>0)
                    OR EXISTS (SELECT 1 FROM OdemePlani_Table P WITH (UPDLOCK, HOLDLOCK)
                        INNER JOIN OdemeAyrinti_Table A WITH (UPDLOCK, HOLDLOCK) ON A.OdemePlaniId=P.Id
                        WHERE P.SozlesmeId=@Id AND (ISNULL(A.OdemeId,0)>0 OR ISNULL(A.SozlesmeId,0)<>@Id))");
            query.AddParameter("@Id", contractId);
            return db.SelectFromDb(query, transaction, context) != null;
        }
    }
}
