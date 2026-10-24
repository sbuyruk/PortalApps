using DAO.Ortak;
using System;
using System.Data;
using Utility.Logging;
using Utility.ProjeGlobal;

namespace DAO.Repositories.Portal
{
    // Historical events are read-only. No legacy service, writer or DbClass is used.
    public sealed class PortalEventArchiveRepository
    {
        private readonly PortalSqlExecutor sql;

        public PortalEventArchiveRepository(PortalLogContext context)
        {
            sql = new PortalSqlExecutor(context, "PortalEventArchiveRepository");
        }

        public DataTable SelectDay(DateTime day, string program)
        {
            if (day.Date < new DateTime(1753, 1, 1) || day.Date == DateTime.MaxValue.Date)
                throw new ArgumentException("Geçerli bir arşiv tarihi seçiniz.", "day");
            bool allPrograms = string.IsNullOrEmpty(program) || program == ProjeConstants.HEPSI;
            SqlQuery query = new SqlQuery(@"SELECT Id,Program,IslemTipi,IslemKonusu,IslemTarihi,IslemYapan,Aciklama
                FROM Olay_Table WHERE IslemTarihi>=@Start AND IslemTarihi<@End"
                + (allPrograms ? "" : " AND Program=@Program") + " ORDER BY IslemTarihi DESC,Id DESC");
            query.AddParameter("@Start", day.Date);
            query.AddParameter("@End", day.Date.AddDays(1));
            if (!allPrograms) query.AddParameter("@Program", program);
            return sql.Select(query);
        }
    }
}
