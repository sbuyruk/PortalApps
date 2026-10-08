using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.MTS
{
    public class AniObjesiTanimRepository
    {
        private const string TableName = "AniObjesiTanim_Table";
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public AniObjesiTanimRepository() : this(new DbClass()) { }

        public AniObjesiTanimRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM " + TableName + " WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery("SELECT * FROM " + TableName + " ORDER BY Id"), "");
        }

        public int Insert<T>(T entity) { return db.Insert(queryBuilder.BuildInsert(entity, TableName)); }
        public bool Update<T>(T entity) { return db.Update2Db(queryBuilder.BuildUpdate(entity, TableName)); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete(TableName, id), ""); }

        public DataTable SelectStokluAniObjeleri(string stokluMu)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT A.Id AniObjesiId, A.Adi, SUM(B.SonAdet) Toplam
                FROM AniObjesiTanim_Table A
                INNER JOIN DepoStok_Table B ON B.AniObjesiId = A.Id AND B.SonAdet > 0
                WHERE A.StokluMu = @StokluMu
                GROUP BY A.Id, A.Adi
                ORDER BY A.Adi");
            query.AddParameter("@StokluMu", stokluMu);
            return db.SelectFromDb(query, "");
        }
    }
}
