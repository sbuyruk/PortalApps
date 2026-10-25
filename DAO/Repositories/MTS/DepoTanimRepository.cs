using System.Runtime.CompilerServices;
using Utility.Logging;
using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.MTS
{
    public class DepoTanimRepository
    {
        private readonly MtsSqlExecutor sql = new MtsSqlExecutor("DepoTanimRepository");
        private const string TableName = "DepoTanim_Table";

        public DepoTanimRepository()
        { }

        public DepoTanimRepository(PortalLogContext context)
        {
            if (context == null || context.Project != "MTS") throw new ArgumentException("MTS log bağlamı gereklidir.", "context");
            sql = new MtsSqlExecutor(context, "DepoTanimRepository");
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

        public DataTable SelectAll()
        {
            return Read(new SqlQuery("SELECT * FROM " + TableName + " ORDER BY Id"), "");
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
            return Read(query, "");
        }
    }
}
