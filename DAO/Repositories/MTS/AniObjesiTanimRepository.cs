using System.Runtime.CompilerServices;
using Utility.Logging;
using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.MTS
{
    public class AniObjesiTanimRepository
    {
        private readonly MtsSqlExecutor sql = new MtsSqlExecutor("AniObjesiTanimRepository");
        private const string TableName = "AniObjesiTanim_Table";

        public AniObjesiTanimRepository()
        { }

        public AniObjesiTanimRepository(PortalLogContext context)
        {
            if (context == null || context.Project != "MTS") throw new ArgumentException("MTS log bağlamı gereklidir.", "context");
            sql = new MtsSqlExecutor(context, "AniObjesiTanimRepository");
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
            return Read(query, "");
        }
    }
}
