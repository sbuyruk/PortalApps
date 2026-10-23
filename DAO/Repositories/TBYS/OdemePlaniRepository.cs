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
            SqlQuery q = new SqlQuery("SELECT * FROM OdemePlani_Table WHERE Sira!=0 AND SozlesmeId=@SozlesmeId AND @OdemeTarihi BETWEEN VadeBasTar AND DateADD(day,-1,DATEADD(month,1,VadeBasTar)) ORDER BY VadeBasTar");
            q.AddParameter("@SozlesmeId", id);
            q.AddParameter("@OdemeTarihi", tarih);
            return db.SelectFromDb(q, "");
        }
        public DataTable SelectBySozlesmeIdOdemeTarihi(int id, DateTime tarih, SqlTransactionContext transaction, PortalLogContext context)
        {
            SqlQuery q = new SqlQuery("SELECT * FROM OdemePlani_Table WITH (UPDLOCK, HOLDLOCK) WHERE Sira!=0 AND SozlesmeId=@SozlesmeId AND @OdemeTarihi BETWEEN VadeBasTar AND DateADD(day,-1,DATEADD(month,1,VadeBasTar)) ORDER BY VadeBasTar");
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
    }
}
