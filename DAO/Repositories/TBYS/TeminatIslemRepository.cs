using DAO.Ortak;
using Utility.Logging;
using System;
using System.Data;

namespace DAO.Repositories.TBYS
{
    public class TeminatIslemRepository
    {
        private readonly TbysSqlExecutor db;
        private readonly CrudQueryBuilder queryBuilder;

        public TeminatIslemRepository()
        {
            db = new TbysSqlExecutor("TeminatIslemRepository");
            queryBuilder = new CrudQueryBuilder();
        }

        public TeminatIslemRepository(PortalLogContext context) { db = new TbysSqlExecutor(context, "TeminatIslemRepository"); queryBuilder = new CrudQueryBuilder(); }

        public DataTable SelectById(int id)
        {
            SqlQuery q = new SqlQuery("SELECT * FROM TeminatIslem_Table WHERE Id=@Id");
            q.AddParameter("@Id", id);
            return db.SelectFromDb(q, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery("SELECT * FROM TeminatIslem_Table"), "");
        }

        public DataTable SelectBySozlesmeId(int id)
        {
            SqlQuery q = new SqlQuery("SELECT * FROM TeminatIslem_Table WHERE SozlesmeId=@SozlesmeId ORDER BY TeminatIslemTarihi");
            q.AddParameter("@SozlesmeId", id);
            return db.SelectFromDb(q, "");
        }

        public DataTable SelectBySozlesmeId(int id, SqlTransactionContext transaction, PortalLogContext context)
        {
            SqlQuery q = new SqlQuery("SELECT * FROM TeminatIslem_Table WITH (UPDLOCK, HOLDLOCK) WHERE SozlesmeId=@SozlesmeId ORDER BY IslemTarihi, Id");
            q.AddParameter("@SozlesmeId", id);
            return db.SelectFromDb(q, transaction, context);
        }

        public DataTable SelectByKiraciId(int id)
        {
            SqlQuery q = new SqlQuery("SELECT * FROM TeminatIslem_Table WHERE KiraciId=@KiraciId ORDER BY IslemTarihi DESC");
            q.AddParameter("@KiraciId", id);
            return db.SelectFromDb(q, "");
        }

        public DataTable SelectByOdemeId(int id)
        {
            SqlQuery q = new SqlQuery("SELECT * FROM TeminatIslem_Table WHERE OdemeId=@OdemeId ORDER BY IslemTarihi DESC");
            q.AddParameter("@OdemeId", id);
            return db.SelectFromDb(q, "");
        }

        public DataTable SelectSumPaidByKiraciId(int id)
        {
            SqlQuery q = new SqlQuery("SELECT SUM(IslemTutari) Toplam FROM TeminatIslem_Table WHERE IslemTipi='Teminat Ödemesi' AND KiraciId=@KiraciId");
            q.AddParameter("@KiraciId", id);
            return db.SelectFromDb(q, "");
        }

        public DataTable SelectSumByKiraciIdGroupByType(int id)
        {
            SqlQuery q = new SqlQuery("SELECT SUM(IslemTutari) IslemToplami, IslemTipi FROM TeminatIslem_Table WHERE KiraciId=@KiraciId GROUP BY IslemTipi");
            q.AddParameter("@KiraciId", id);
            return db.SelectFromDb(q, "");
        }
        public DataTable SelectSumByKiraciIdGroupByType(int id, SqlTransactionContext transaction, PortalLogContext context)
        {
            SqlQuery q = new SqlQuery("SELECT SUM(IslemTutari) IslemToplami, IslemTipi FROM TeminatIslem_Table WITH (UPDLOCK, HOLDLOCK) WHERE KiraciId=@KiraciId GROUP BY IslemTipi");
            q.AddParameter("@KiraciId", id);
            return db.SelectFromDb(q, transaction, context);
        }

        public bool DeleteBySozlesmeId(int id, SqlTransactionContext transaction, PortalLogContext context)
        {
            SqlQuery q = new SqlQuery("DELETE TeminatIslem_Table WHERE SozlesmeId=@SozlesmeId");
            q.AddParameter("@SozlesmeId", id);
            return db.DeleteFromDb(q, transaction, context);
        }

        public DataTable SelectById(int id, SqlTransactionContext transaction, PortalLogContext context) { SqlQuery q = new SqlQuery("SELECT * FROM TeminatIslem_Table WITH (UPDLOCK, HOLDLOCK) WHERE Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, transaction, context); }
        public int Insert<T>(T entity, SqlTransactionContext transaction, PortalLogContext context) { return db.Insert(queryBuilder.BuildInsert(entity, "TeminatIslem_Table"), transaction, context); }
        public bool Update<T>(T entity, SqlTransactionContext transaction, PortalLogContext context) { return db.Update2Db(queryBuilder.BuildUpdate(entity, "TeminatIslem_Table"), transaction, context); }
        public bool Delete(int id, SqlTransactionContext transaction, PortalLogContext context) { return db.DeleteFromDb(queryBuilder.BuildDelete("TeminatIslem_Table", id), transaction, context); }
    }
}
