using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.MTS
{
    public class MtsLookupRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public MtsLookupRepository()
            : this(new DbClass())
        {
        }

        public MtsLookupRepository(DbClass db)
        {
            if (db == null)
                throw new ArgumentNullException("db");

            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(string tableName, int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM " + tableName + " WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAll(string tableName)
        {
            return db.SelectFromDb(new SqlQuery("SELECT * FROM " + tableName + " ORDER BY Id"), "");
        }

        public int Insert<T>(string tableName, T entity)
        {
            return db.Insert(queryBuilder.BuildInsert(entity, tableName));
        }

        public bool Update<T>(string tableName, T entity)
        {
            return db.Update2Db(queryBuilder.BuildUpdate(entity, tableName));
        }

        public bool Delete(string tableName, int id)
        {
            return db.DeleteFromDb(queryBuilder.BuildDelete(tableName, id), "");
        }

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

        public DataTable SelectDepoStok(int depoId, int aniObjesiId, string stokluMu)
        {
            string depoFilter = depoId > 0 ? " AND A.DepoId = @DepoId" : "";
            string aniObjesiFilter = aniObjesiId > 0 ? " AND B.Id = @AniObjesiId" : "";
            string stokluFilter = string.IsNullOrEmpty(stokluMu) ? "" : " AND B.StokluMu = @StokluMu";
            SqlQuery query = new SqlQuery(@"
                SELECT A.*
                FROM DepoStok_Table A
                INNER JOIN AniObjesiTanim_Table B ON B.Id = A.AniObjesiId
                WHERE A.SonAdet > 0" + depoFilter + aniObjesiFilter + stokluFilter + @"
                ORDER BY A.Id");
            if (depoId > 0)
                query.AddParameter("@DepoId", depoId);
            if (aniObjesiId > 0)
                query.AddParameter("@AniObjesiId", aniObjesiId);
            if (!string.IsNullOrEmpty(stokluMu))
                query.AddParameter("@StokluMu", stokluMu);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectFaaliyetKatilim(int faaliyetId, int katilimciId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT *
                FROM FaaliyetKatilim_Table
                WHERE FaaliyetId = @FaaliyetId AND KatilimciId = @KatilimciId");
            query.AddParameter("@FaaliyetId", faaliyetId);
            query.AddParameter("@KatilimciId", katilimciId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectFaaliyetKatilimByKatilimciId(int katilimciId)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM FaaliyetKatilim_Table WHERE KatilimciId = @KatilimciId");
            query.AddParameter("@KatilimciId", katilimciId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectFaaliyetKatilimByFaaliyetId(int faaliyetId)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM FaaliyetKatilim_Table WHERE FaaliyetId = @FaaliyetId");
            query.AddParameter("@FaaliyetId", faaliyetId);
            return db.SelectFromDb(query, "");
        }
    }
}
