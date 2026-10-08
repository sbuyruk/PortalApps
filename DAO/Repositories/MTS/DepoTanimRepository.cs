using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.MTS
{
    public class DepoTanimRepository
    {
        private const string TableName = "DepoTanim_Table";
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public DepoTanimRepository() : this(new DbClass()) { }

        public DepoTanimRepository(DbClass db)
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

        public DataTable SelectStokluAniObjesiDepolari(string stokluMu, int aniObjesiId)
        {
            string filter = aniObjesiId > 0 ? " AND C.Id = @AniObjesiId" : "";
            SqlQuery query = new SqlQuery(@"
                SELECT C.Id AniObjesiId, C.Adi AniObjesiAdi,
                       A.Id DepoId, A.Adi DepoAdi, SUM(B.SonAdet) Adet
                FROM DepoTanim_Table A
                INNER JOIN DepoStok_Table B ON B.DepoId = A.Id AND B.SonAdet > 0
                INNER JOIN AniObjesiTanim_Table C ON C.Id = B.AniObjesiId
                WHERE C.StokluMu = @StokluMu" + filter + @"
                GROUP BY A.Id, A.Adi, C.Id, C.Adi
                ORDER BY A.Id");
            query.AddParameter("@StokluMu", stokluMu);
            if (aniObjesiId > 0)
                query.AddParameter("@AniObjesiId", aniObjesiId);
            return db.SelectFromDb(query, "");
        }
    }
}
