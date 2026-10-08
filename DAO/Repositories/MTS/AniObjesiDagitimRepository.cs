using DAO.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using Utility.ProjeGlobal;

namespace DAO.Repositories.MTS
{
    public class AniObjesiDagitimRepository
    {
        private const string TableName = "AniObjesiDagitim_Table";
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public AniObjesiDagitimRepository() : this(new DbClass()) { }
        public AniObjesiDagitimRepository(DbClass db)
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
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM " + TableName + " ORDER BY Id"), ""); }
        public int Insert<T>(T entity) { return db.Insert(queryBuilder.BuildInsert(entity, TableName)); }
        public bool Update<T>(T entity) { return db.Update2Db(queryBuilder.BuildUpdate(entity, TableName)); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete(TableName, id), ""); }

        public DataTable SelectByFilter(int faaliyetId, int katilimciId, int aniObjesiId, string stokluMu, string verilenGetirilen, string mode)
        {
            SqlQuery query;
            if (mode == "return")
            {
                query = new SqlQuery(@"SELECT A.Id AniObjesiId, A.Sira, A.Adi, B.Id AniObjesiDagitimId,
                    B.Adet, B.FaaliyetId, B.KatilimciId FROM AniObjesiTanim_Table A
                    LEFT JOIN AniObjesiDagitim_Table B ON B.AniObjesiId = A.Id
                        AND B.FaaliyetId = @FaaliyetId AND B.KatilimciId = @KatilimciId ORDER BY A.Id");
            }
            else if (mode == "stok")
            {
                query = new SqlQuery(@"SELECT A.Id AniObjesiId, A.Sira, A.Adi, B.Id AniObjesiDagitimId,
                    B.Adet, B.FaaliyetId, B.KatilimciId FROM AniObjesiTanim_Table A
                    LEFT JOIN AniObjesiDagitim_Table B ON B.AniObjesiId = A.Id
                        AND B.FaaliyetId = @FaaliyetId AND B.KatilimciId = @KatilimciId
                    WHERE A.StokluMu = @StokluMu ORDER BY A.Id");
            }
            else if (mode == "stoksuz")
            {
                query = new SqlQuery(@"SELECT B.Id, A.Id AniObjesiId, ISNULL(B.Adet, 0) Adet,
                    B.FaaliyetId, B.KatilimciId FROM AniObjesiTanim_Table A
                    LEFT JOIN AniObjesiDagitim_Table B ON B.AniObjesiId = A.Id
                    AND B.FaaliyetId = @FaaliyetId AND B.KatilimciId = @KatilimciId
                    WHERE A.StokluMu = @StokluMu");
            }
            else if (mode == "kisi")
            {
                query = new SqlQuery(@"SELECT B.Id, A.Id AniObjesiId, ISNULL(B.Adet, 0) Adet,
                    B.FaaliyetId, B.KatilimciId, B.VerilenAlinan FROM AniObjesiTanim_Table A
                    LEFT JOIN AniObjesiDagitim_Table B ON B.AniObjesiId = A.Id AND B.KatilimciId = @KatilimciId
                    WHERE 1 > 0");
                if (!string.Equals(verilenGetirilen, ProjeConstants.ANIOBJESI_VERILENGETIRILEN))
                    query.Sql += " AND B.VerilenAlinan = @VerilenAlinan";
            }
            else if (mode == "katilimciFaaliyet")
            {
                query = new SqlQuery(@"SELECT A.Id AniObjesiDagitimId, B.Id AniObjesiId, B.Adi,
                    A.Adet, A.CikisDepoId FROM AniObjesiDagitim_Table A
                    INNER JOIN AniObjesiTanim_Table B ON B.Id = A.AniObjesiId
                    WHERE A.KatilimciId = @KatilimciId AND A.FaaliyetId = @FaaliyetId");
                if (!string.IsNullOrEmpty(stokluMu))
                    query.Sql += " AND B.StokluMu = @StokluMu";
            }
            else if (mode == "activity")
                query = new SqlQuery("SELECT * FROM AniObjesiDagitim_Table WHERE FaaliyetId = @FaaliyetId");
            else
                query = new SqlQuery("SELECT * FROM AniObjesiDagitim_Table WHERE AniObjesiId = @AniObjesiId AND FaaliyetId = @FaaliyetId AND KatilimciId = @KatilimciId");

            query.AddParameter("@FaaliyetId", faaliyetId);
            query.AddParameter("@KatilimciId", katilimciId);
            if (mode == "stok" || mode == "stoksuz" || mode == "katilimciFaaliyet")
                query.AddParameter("@StokluMu", stokluMu);
            if (mode == "kisi" && !string.Equals(verilenGetirilen, ProjeConstants.ANIOBJESI_VERILENGETIRILEN))
                query.AddParameter("@VerilenAlinan", verilenGetirilen);
            if (mode == "single")
                query.AddParameter("@AniObjesiId", aniObjesiId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectGetirilen(int faaliyetId, int katilimciId, int getirilenValue)
        {
            SqlQuery query = new SqlQuery(@"SELECT * FROM AniObjesiDagitim_Table
                WHERE FaaliyetId = @FaaliyetId AND KatilimciId = @KatilimciId
                  AND VerilenAlinan = @VerilenAlinan ORDER BY Id");
            query.AddParameter("@FaaliyetId", faaliyetId);
            query.AddParameter("@KatilimciId", katilimciId);
            query.AddParameter("@VerilenAlinan", getirilenValue);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectGetirilenText(int katilimciId, int faaliyetId, int aniObjesiId)
        {
            SqlQuery query = new SqlQuery(@"SELECT GetirilenAniObjesi FROM AniObjesiDagitim_Table
                WHERE KatilimciId = @KatilimciId AND FaaliyetId = @FaaliyetId AND AniObjesiId = @AniObjesiId");
            query.AddParameter("@KatilimciId", katilimciId);
            query.AddParameter("@FaaliyetId", faaliyetId);
            query.AddParameter("@AniObjesiId", aniObjesiId);
            return db.SelectFromDb(query, "");
        }

        public int DeleteByActivityAndParticipant(int faaliyetId, int katilimciId, List<int> aniObjesiIds, out DataTable deletedItems)
        {
            List<string> parameters = new List<string>();
            SqlQuery selectQuery = new SqlQuery("SELECT * FROM AniObjesiDagitim_Table WHERE FaaliyetId = @FaaliyetId AND KatilimciId = @KatilimciId");
            selectQuery.AddParameter("@FaaliyetId", faaliyetId);
            selectQuery.AddParameter("@KatilimciId", katilimciId);
            for (int i = 0; i < aniObjesiIds.Count; i++)
            {
                string parameter = "@AniObjesiId" + i;
                parameters.Add(parameter);
                selectQuery.Sql += (i == 0 ? " AND AniObjesiId IN (" : ", ") + parameter;
                selectQuery.AddParameter(parameter, aniObjesiIds[i]);
            }
            selectQuery.Sql += ")";
            deletedItems = db.SelectFromDb(selectQuery, "");
            if (deletedItems == null || deletedItems.Rows.Count == 0) return 0;
            SqlQuery deleteQuery = new SqlQuery("DELETE FROM AniObjesiDagitim_Table WHERE FaaliyetId = @FaaliyetId AND KatilimciId = @KatilimciId AND AniObjesiId IN (" + string.Join(", ", parameters) + ")");
            deleteQuery.AddParameter("@FaaliyetId", faaliyetId);
            deleteQuery.AddParameter("@KatilimciId", katilimciId);
            for (int i = 0; i < aniObjesiIds.Count; i++) deleteQuery.AddParameter("@AniObjesiId" + i, aniObjesiIds[i]);
            return db.DeleteFromDb(deleteQuery, "", true);
        }
    }
}
