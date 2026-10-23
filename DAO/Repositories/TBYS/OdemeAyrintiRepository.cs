using DAO.Ortak;
using Utility.Logging;
using System;
using System.Data;

namespace DAO.Repositories.TBYS
{
    public class OdemeAyrintiRepository
    {
        private readonly TbysSqlExecutor db;
        private readonly CrudQueryBuilder queryBuilder;

        public OdemeAyrintiRepository()
            : this(new DbClass())
        {
        }

        public OdemeAyrintiRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = new TbysSqlExecutor(db, "OdemeAyrintiRepository");
            queryBuilder = new CrudQueryBuilder();
        }

        public OdemeAyrintiRepository(PortalLogContext context) { db = new TbysSqlExecutor(context, "OdemeAyrintiRepository"); queryBuilder = new CrudQueryBuilder(); }

        public DataTable SelectById(int id)
        {
            SqlQuery q = new SqlQuery("SELECT * FROM OdemeAyrinti_Table WHERE Id=@Id");
            q.AddParameter("@Id", id);
            return db.SelectFromDb(q, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery("SELECT * FROM OdemeAyrinti_Table"), "");
        }

        public DataTable SelectBySozlesmeId(int id)
        {
            SqlQuery q = new SqlQuery("SELECT * FROM OdemeAyrinti_Table WHERE SozlesmeId=@SozlesmeId ORDER BY OdemePlaniSirasi,OdemeTarihi,Id");
            q.AddParameter("@SozlesmeId", id);
            return db.SelectFromDb(q, "");
        }

        public DataTable SelectByOdemeIdOdemePlaniId(int odemeId, int planId)
        {
            SqlQuery q = new SqlQuery("SELECT * FROM OdemeAyrinti_Table WHERE OdemeId=@OdemeId AND OdemePlaniId=@OdemePlaniId ORDER BY OdemePlaniSirasi,OdemeTarihi,Id");
            q.AddParameter("@OdemeId", odemeId);
            q.AddParameter("@OdemePlaniId", planId);
            return db.SelectFromDb(q, "");
        }
        public DataTable SelectByOdemeIdOdemePlaniId(int odemeId, int planId, SqlTransactionContext transaction, PortalLogContext context)
        {
            SqlQuery q = new SqlQuery("SELECT * FROM OdemeAyrinti_Table WITH (UPDLOCK, HOLDLOCK) WHERE OdemeId=@OdemeId AND OdemePlaniId=@OdemePlaniId ORDER BY OdemePlaniSirasi,OdemeTarihi,OdemeId,Id");
            q.AddParameter("@OdemeId", odemeId);
            q.AddParameter("@OdemePlaniId", planId);
            return db.SelectFromDb(q, transaction, context);
        }

        public DataTable SelectByPlanAndDelay(int kiraciId, int sozlesmeId, int planId, int delayId, int odemeId)
        {
            SqlQuery q = new SqlQuery("SELECT * FROM OdemeAyrinti_Table WHERE KiraciId=@KiraciId AND SozlesmeId=@SozlesmeId AND OdemePlaniId=@OdemePlaniId AND GecikmeZammiId=@GecikmeZammiId AND OdemeId=@OdemeId ORDER BY OdemePlaniSirasi,OdemeTarihi,OdemeId,Id");
            q.AddParameter("@KiraciId", kiraciId);
            q.AddParameter("@SozlesmeId", sozlesmeId);
            q.AddParameter("@OdemePlaniId", planId);
            q.AddParameter("@GecikmeZammiId", delayId);
            q.AddParameter("@OdemeId", odemeId);
            return db.SelectFromDb(q, "");
        }
        public DataTable SelectByPlanAndDelay(int kiraciId, int sozlesmeId, int planId, int delayId, int odemeId, SqlTransactionContext transaction, PortalLogContext context)
        {
            SqlQuery q = new SqlQuery("SELECT * FROM OdemeAyrinti_Table WITH (UPDLOCK, HOLDLOCK) WHERE KiraciId=@KiraciId AND SozlesmeId=@SozlesmeId AND OdemePlaniId=@OdemePlaniId AND GecikmeZammiId=@GecikmeZammiId AND OdemeId=@OdemeId ORDER BY OdemePlaniSirasi,OdemeTarihi,OdemeId,Id");
            q.AddParameter("@KiraciId", kiraciId);
            q.AddParameter("@SozlesmeId", sozlesmeId);
            q.AddParameter("@OdemePlaniId", planId);
            q.AddParameter("@GecikmeZammiId", delayId);
            q.AddParameter("@OdemeId", odemeId);
            return db.SelectFromDb(q, transaction, context);
        }

        public DataTable SelectByPlan(int planId)
        {
            SqlQuery q = new SqlQuery("SELECT * FROM OdemeAyrinti_Table WHERE OdemePlaniId=@OdemePlaniId ORDER BY IlkTarih,Id");
            q.AddParameter("@OdemePlaniId", planId);
            return db.SelectFromDb(q, "");
        }

        public bool DeleteBySozlesmeId(int id)
        {
            SqlQuery q = new SqlQuery("DELETE OdemeAyrinti_Table WHERE SozlesmeId=@SozlesmeId");
            q.AddParameter("@SozlesmeId", id);
            return db.DeleteFromDb(q, "");
        }

        public bool DeleteByOdemeIdOdemePlaniId(int odemeId, int planId)
        {
            SqlQuery q = new SqlQuery("DELETE OdemeAyrinti_Table WHERE OdemeId=@OdemeId AND OdemePlaniId=@OdemePlaniId");
            q.AddParameter("@OdemeId", odemeId);
            q.AddParameter("@OdemePlaniId", planId);
            return db.DeleteFromDb(q, "");
        }
        public bool DeleteByOdemeIdOdemePlaniId(int odemeId, int planId, SqlTransactionContext transaction, PortalLogContext context)
        {
            SqlQuery q = new SqlQuery("DELETE OdemeAyrinti_Table WHERE OdemeId=@OdemeId AND OdemePlaniId=@OdemePlaniId");
            q.AddParameter("@OdemeId", odemeId);
            q.AddParameter("@OdemePlaniId", planId);
            return db.DeleteFromDb(q, transaction, context);
        }

        public DataTable SelectLastAnaPara(int planId)
        {
            SqlQuery q = new SqlQuery("SELECT AnaPara FROM OdemeAyrinti_Table WHERE OdemePlaniId=@OdemePlaniId ORDER BY Id DESC");
            q.AddParameter("@OdemePlaniId", planId);
            return db.SelectFromDb(q, "");
        }

        public DataTable SelectSumDelayAmount(int planId)
        {
            SqlQuery q = new SqlQuery("SELECT ISNULL(SUM(GecikmeZammiTutari),0) Toplam FROM OdemeAyrinti_Table WHERE OdemePlaniId=@OdemePlaniId");
            q.AddParameter("@OdemePlaniId", planId);
            return db.SelectFromDb(q, "");
        }
        public DataTable SelectSumDelayAmount(int planId, SqlTransactionContext transaction, PortalLogContext context)
        {
            SqlQuery q = new SqlQuery("SELECT ISNULL(SUM(GecikmeZammiTutari),0) Toplam FROM OdemeAyrinti_Table WITH (UPDLOCK, HOLDLOCK) WHERE OdemePlaniId=@OdemePlaniId");
            q.AddParameter("@OdemePlaniId", planId);
            return db.SelectFromDb(q, transaction, context);
        }

        public DataTable SelectLastDelayRate(int planId)
        {
            SqlQuery q = new SqlQuery("SELECT GecikmeZammiOrani FROM OdemeAyrinti_Table WHERE OdemePlaniId=@OdemePlaniId ORDER BY IlkTarih");
            q.AddParameter("@OdemePlaniId", planId);
            return db.SelectFromDb(q, "");
        }
        public DataTable SelectLastDelayRate(int planId, SqlTransactionContext transaction, PortalLogContext context)
        {
            SqlQuery q = new SqlQuery("SELECT GecikmeZammiOrani FROM OdemeAyrinti_Table WITH (UPDLOCK, HOLDLOCK) WHERE OdemePlaniId=@OdemePlaniId ORDER BY IlkTarih");
            q.AddParameter("@OdemePlaniId", planId);
            return db.SelectFromDb(q, transaction, context);
        }

        public int Insert<T>(T entity)
        {
            return db.Insert(queryBuilder.BuildInsert(entity, "OdemeAyrinti_Table"));
        }

        public bool Update<T>(T entity)
        {
            return db.Update2Db(queryBuilder.BuildUpdate(entity, "OdemeAyrinti_Table"));
        }

        public bool Delete(int id)
        {
            return db.DeleteFromDb(queryBuilder.BuildDelete("OdemeAyrinti_Table", id), "");
        }

        public DataTable SelectById(int id, SqlTransactionContext transaction, PortalLogContext context) { SqlQuery q = new SqlQuery("SELECT * FROM OdemeAyrinti_Table WITH (UPDLOCK, HOLDLOCK) WHERE Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, transaction, context); }
        public int Insert<T>(T entity, SqlTransactionContext transaction, PortalLogContext context) { return db.Insert(queryBuilder.BuildInsert(entity, "OdemeAyrinti_Table"), transaction, context); }
        public bool Update<T>(T entity, SqlTransactionContext transaction, PortalLogContext context) { return db.Update2Db(queryBuilder.BuildUpdate(entity, "OdemeAyrinti_Table"), transaction, context); }
        public bool Delete(int id, SqlTransactionContext transaction, PortalLogContext context) { return db.DeleteFromDb(queryBuilder.BuildDelete("OdemeAyrinti_Table", id), transaction, context); }
        public bool DeleteBySozlesmeId(int id, SqlTransactionContext transaction, PortalLogContext context) { SqlQuery q = new SqlQuery("DELETE OdemeAyrinti_Table WHERE SozlesmeId=@SozlesmeId"); q.AddParameter("@SozlesmeId", id); return db.DeleteFromDb(q, transaction, context); }
        public DataTable SelectBySozlesmeId(int id, SqlTransactionContext transaction, PortalLogContext context)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM OdemeAyrinti_Table WITH (UPDLOCK, HOLDLOCK) WHERE SozlesmeId=@SozlesmeId ORDER BY Id");
            query.AddParameter("@SozlesmeId", id);
            return db.SelectFromDb(query, transaction, context);
        }
    }
}
