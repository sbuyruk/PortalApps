using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.TBYS
{
    public class TeminatIslemRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public TeminatIslemRepository()
            : this(new DbClass())
        {
        }

        public TeminatIslemRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

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

        public bool DeleteBySozlesmeId(int id)
        {
            SqlQuery q = new SqlQuery("DELETE TeminatIslem_Table WHERE SozlesmeId=@SozlesmeId");
            q.AddParameter("@SozlesmeId", id);
            return db.DeleteFromDb(q, "");
        }

        public int Insert<T>(T entity)
        {
            return db.Insert(queryBuilder.BuildInsert(entity, "TeminatIslem_Table"));
        }

        public bool Update<T>(T entity)
        {
            return db.Update2Db(queryBuilder.BuildUpdate(entity, "TeminatIslem_Table"));
        }

        public bool Delete(int id)
        {
            return db.DeleteFromDb(queryBuilder.BuildDelete("TeminatIslem_Table", id), "");
        }
    }
}
