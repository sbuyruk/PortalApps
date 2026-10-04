using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.TBYS
{
    public class OdemeAyrintiRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;
        public OdemeAyrintiRepository() : this(new DbClass()) { }
        public OdemeAyrintiRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }
        public DataTable SelectById(int id) { SqlQuery q = new SqlQuery("SELECT * FROM OdemeAyrinti_Table WHERE Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM OdemeAyrinti_Table"), ""); }
        public DataTable SelectBySozlesmeId(int id) { SqlQuery q = new SqlQuery("SELECT * FROM OdemeAyrinti_Table WHERE SozlesmeId=@SozlesmeId ORDER BY OdemePlaniSirasi,OdemeTarihi,Id"); q.AddParameter("@SozlesmeId", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectByOdemeIdOdemePlaniId(int odemeId, int planId) { SqlQuery q = new SqlQuery("SELECT * FROM OdemeAyrinti_Table WHERE OdemeId=@OdemeId AND OdemePlaniId=@OdemePlaniId ORDER BY OdemePlaniSirasi,OdemeTarihi,Id"); q.AddParameter("@OdemeId", odemeId); q.AddParameter("@OdemePlaniId", planId); return db.SelectFromDb(q, ""); }
        public DataTable SelectByPlanAndDelay(int kiraciId, int sozlesmeId, int planId, int delayId, int odemeId) { SqlQuery q = new SqlQuery("SELECT * FROM OdemeAyrinti_Table WHERE KiraciId=@KiraciId AND SozlesmeId=@SozlesmeId AND OdemePlaniId=@OdemePlaniId AND GecikmeZammiId=@GecikmeZammiId AND OdemeId=@OdemeId ORDER BY OdemePlaniSirasi,OdemeTarihi,OdemeId,Id"); q.AddParameter("@KiraciId", kiraciId); q.AddParameter("@SozlesmeId", sozlesmeId); q.AddParameter("@OdemePlaniId", planId); q.AddParameter("@GecikmeZammiId", delayId); q.AddParameter("@OdemeId", odemeId); return db.SelectFromDb(q, ""); }
        public DataTable SelectByPlan(int planId) { SqlQuery q = new SqlQuery("SELECT * FROM OdemeAyrinti_Table WHERE OdemePlaniId=@OdemePlaniId ORDER BY IlkTarih,Id"); q.AddParameter("@OdemePlaniId", planId); return db.SelectFromDb(q, ""); }
        public bool DeleteBySozlesmeId(int id) { SqlQuery q = new SqlQuery("DELETE OdemeAyrinti_Table WHERE SozlesmeId=@SozlesmeId"); q.AddParameter("@SozlesmeId", id); return db.DeleteFromDb(q, ""); }
        public bool DeleteByOdemeIdOdemePlaniId(int odemeId, int planId) { SqlQuery q = new SqlQuery("DELETE OdemeAyrinti_Table WHERE OdemeId=@OdemeId AND OdemePlaniId=@OdemePlaniId"); q.AddParameter("@OdemeId", odemeId); q.AddParameter("@OdemePlaniId", planId); return db.DeleteFromDb(q, ""); }
        public DataTable SelectLastAnaPara(int planId) { SqlQuery q = new SqlQuery("SELECT AnaPara FROM OdemeAyrinti_Table WHERE OdemePlaniId=@OdemePlaniId ORDER BY Id DESC"); q.AddParameter("@OdemePlaniId", planId); return db.SelectFromDb(q, ""); }
        public DataTable SelectSumDelayAmount(int planId) { SqlQuery q = new SqlQuery("SELECT ISNULL(SUM(GecikmeZammiTutari),0) Toplam FROM OdemeAyrinti_Table WHERE OdemePlaniId=@OdemePlaniId"); q.AddParameter("@OdemePlaniId", planId); return db.SelectFromDb(q, ""); }
        public DataTable SelectLastDelayRate(int planId) { SqlQuery q = new SqlQuery("SELECT GecikmeZammiOrani FROM OdemeAyrinti_Table WHERE OdemePlaniId=@OdemePlaniId ORDER BY IlkTarih"); q.AddParameter("@OdemePlaniId", planId); return db.SelectFromDb(q, ""); }
        public int Insert<T>(T entity) { return db.Insert(queryBuilder.BuildInsert(entity, "OdemeAyrinti_Table")); }
        public bool Update<T>(T entity) { return db.Update2Db(queryBuilder.BuildUpdate(entity, "OdemeAyrinti_Table")); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("OdemeAyrinti_Table", id), ""); }
    }
}
