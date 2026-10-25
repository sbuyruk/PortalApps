using System.Runtime.CompilerServices;
using Utility.Logging;
using DAO.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using Utility.ProjeGlobal;

namespace DAO.Repositories.MTS
{
    public class AniObjesiDagitimRepository
    {
        private readonly MtsSqlExecutor sql = new MtsSqlExecutor("AniObjesiDagitimRepository");
        private const string TableName = "AniObjesiDagitim_Table";

        public AniObjesiDagitimRepository()
        { }
        public AniObjesiDagitimRepository(PortalLogContext context)
        {
            if (context == null || context.Project != "MTS") throw new ArgumentException("MTS log bağlamı gereklidir.", "context");
            sql = new MtsSqlExecutor(context, "AniObjesiDagitimRepository");
        }
        private DataTable Read(SqlQuery query, string identifier, [CallerMemberName] string method = "")
        {
            return sql.Select(query, method: method);
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM " + TableName + " WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return Read(query, "");
        }
        public DataTable SelectAll() { return Read(new SqlQuery("SELECT * FROM " + TableName + " ORDER BY Id"), ""); }

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
            return Read(query, "");
        }

        public DataTable SelectEventDistribution(int faaliyetId, int katilimciId, Utility.Logging.PortalLogContext context)
        {
            var query = new SqlQuery(@"SELECT A.Adi, B.Adet FROM AniObjesiTanim_Table A
                INNER JOIN AniObjesiDagitim_Table B ON B.AniObjesiId=A.Id
                WHERE B.FaaliyetId=@FaaliyetId AND B.KatilimciId=@KatilimciId AND B.Adet>0 ORDER BY A.Id");
            query.AddParameter("@FaaliyetId", faaliyetId);
            query.AddParameter("@KatilimciId", katilimciId);
            return PortalSqlReadExecutor.Select(query, context, "AniObjesiDagitimRepository.SelectEventDistribution");
        }

        public DataTable SelectGetirilen(int faaliyetId, int katilimciId, int getirilenValue)
        {
            SqlQuery query = new SqlQuery(@"SELECT * FROM AniObjesiDagitim_Table
                WHERE FaaliyetId = @FaaliyetId AND KatilimciId = @KatilimciId
                  AND VerilenAlinan = @VerilenAlinan ORDER BY Id");
            query.AddParameter("@FaaliyetId", faaliyetId);
            query.AddParameter("@KatilimciId", katilimciId);
            query.AddParameter("@VerilenAlinan", getirilenValue);
            return Read(query, "");
        }

        public DataTable SelectGetirilen(int faaliyetId, int katilimciId, int getirilenValue, Utility.Logging.PortalLogContext context)
        {
            SqlQuery query = new SqlQuery(@"SELECT * FROM AniObjesiDagitim_Table
                WHERE FaaliyetId = @FaaliyetId AND KatilimciId = @KatilimciId
                  AND VerilenAlinan = @VerilenAlinan ORDER BY Id");
            query.AddParameter("@FaaliyetId", faaliyetId);
            query.AddParameter("@KatilimciId", katilimciId);
            query.AddParameter("@VerilenAlinan", getirilenValue);
            return PortalSqlReadExecutor.Select(query, context, "AniObjesiDagitimRepository.SelectGetirilen");
        }

        public DataTable SelectGetirilenText(int katilimciId, int faaliyetId, int aniObjesiId)
        {
            SqlQuery query = new SqlQuery(@"SELECT GetirilenAniObjesi FROM AniObjesiDagitim_Table
                WHERE KatilimciId = @KatilimciId AND FaaliyetId = @FaaliyetId AND AniObjesiId = @AniObjesiId");
            query.AddParameter("@KatilimciId", katilimciId);
            query.AddParameter("@FaaliyetId", faaliyetId);
            query.AddParameter("@AniObjesiId", aniObjesiId);
            return Read(query, "");
        }

    }
}
