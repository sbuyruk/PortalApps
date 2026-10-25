using System.Runtime.CompilerServices;
using Utility.Logging;
using DAO.Ortak;
using System;
using System.Data;
using Utility.ProjeGlobal;

namespace DAO.Repositories.MTS
{
    public class MTSKurumGorevRepository
    {
        private readonly MtsSqlExecutor sql = new MtsSqlExecutor("MTSKurumGorevRepository");
        public MTSKurumGorevRepository()
        {
        }

        public MTSKurumGorevRepository(PortalLogContext context)
        {
            if (context == null || context.Project != "MTS") throw new ArgumentException("MTS log bağlamı gereklidir.", "context");
            sql = new MtsSqlExecutor(context, "MTSKurumGorevRepository");
        }

        private DataTable Read(SqlQuery query, string identifier, [CallerMemberName] string method = "")
        {
            return sql.Select(query, method: method);
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM MTSKurumGorev_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return Read(query, "");
        }

        public DataTable SelectAll()
        {
            return Read(new SqlQuery("SELECT * FROM MTSKurumGorev_Table ORDER BY Id"), "");
        }
        public DataTable SelectByKisiId(int kisiId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT B.Adi Kurum, C.Adi Gorev
                FROM MTSKurumGorev_Table A
                LEFT JOIN MTSKurumTanim_Table B ON B.Id = A.MTSKurumTanimId
                LEFT JOIN MTSGorevTanim_Table C ON C.Id = A.MTSGorevTanimId
                WHERE A.KisiId = @KisiId AND A.Durum = @Durum");
            query.AddParameter("@KisiId", kisiId);
            query.AddParameter("@Durum", ProjeConstants.MTSGOREVDURUMU_GOREVDE);
            return Read(query, "");
        }

    }
}
