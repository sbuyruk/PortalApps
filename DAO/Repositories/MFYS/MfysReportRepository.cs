using DAO.Ortak;
using System;
using System.Data;
using System.Runtime.CompilerServices;
using Utility.Logging;

namespace DAO.Repositories.MFYS
{
    // Report-only access retains MFYS context while reading NBYS/TBYS source tables.
    public sealed class MfysReportRepository
    {
        private readonly PortalLogContext context;

        public MfysReportRepository(PortalLogContext context)
        {
            if (context == null || context.Project != "MFYS")
                throw new ArgumentException("MFYS log bağlamı gereklidir.", "context");
            this.context = context;
        }

        private DataTable Select(SqlQuery query, [CallerMemberName] string method = "")
        {
            return PortalSqlReadExecutor.Select(query, context, "MfysReportRepository." + method);
        }

        public DataTable BankGroups(bool secondary)
        {
            string column = secondary ? "BankaGrup2" : "BankaGrup";
            return Select(new SqlQuery("SELECT DISTINCT " + column + " Banka FROM BankaTanim_Table WHERE "
                + column + " IS NOT NULL AND LTRIM(RTRIM(" + column + "))<>'' ORDER BY Banka"));
        }

        public DataTable PaymentReasons()
        {
            return Select(new SqlQuery("SELECT Id,OdemeSebebi FROM OdemeSebebiTanim_Table ORDER BY OdemeSebebi,Id"));
        }

        public DataTable DailyTlTotals(DateTime from, DateTime until, string bank, string currency)
        {
            var query = new SqlQuery(@"SELECT CAST(A.BagisTarihi AS date) BagisTarihi,
                    SUM(A.BagisMiktari + ISNULL(A.IadeMiktari,0)) ToplamBagis
                FROM NakitBagisHareket_Table A LEFT JOIN BankaTanim_Table B ON B.Id=A.BankaId
                WHERE A.BagisTarihi>=@From AND A.BagisTarihi<@Until AND A.DovizCinsi=@Currency"
                + (string.IsNullOrEmpty(bank) ? "" : " AND B.BankaGrup2=@Bank")
                + " GROUP BY CAST(A.BagisTarihi AS date) ORDER BY BagisTarihi");
            Range(query, from, until);
            query.AddParameter("@Currency", currency);
            if (!string.IsNullOrEmpty(bank)) query.AddParameter("@Bank", bank);
            return Select(query);
        }

        public DataTable CurrencyDonations(DateTime from, DateTime until, string bank, string currency)
        {
            var query = new SqlQuery(@"SELECT A.BagisTarihi,A.DovizTutari,A.DovizKuru,A.DovizCinsi,A.BagisMiktari,B.Banka
                FROM NakitBagisHareket_Table A LEFT JOIN BankaTanim_Table B ON B.Id=A.BankaId
                WHERE A.BagisTarihi>=@From AND A.BagisTarihi<@Until AND A.DovizCinsi=@Currency"
                + (string.IsNullOrEmpty(bank) ? "" : " AND B.BankaGrup2=@Bank") + " ORDER BY A.BagisTarihi,A.Id");
            Range(query, from, until);
            query.AddParameter("@Currency", currency);
            if (!string.IsNullOrEmpty(bank)) query.AddParameter("@Bank", bank);
            return Select(query);
        }

        public DataTable CurrencyBankGroups(DateTime from, DateTime until, string currency)
        {
            var query = new SqlQuery(@"SELECT DISTINCT B.BankaGrup2 Banka
                FROM NakitBagisHareket_Table A INNER JOIN BankaTanim_Table B ON B.Id=A.BankaId
                WHERE A.BagisTarihi>=@From AND A.BagisTarihi<@Until AND A.DovizCinsi=@Currency
                    AND B.BankaGrup2 IS NOT NULL AND LTRIM(RTRIM(B.BankaGrup2))<>'' ORDER BY Banka");
            Range(query, from, until);
            query.AddParameter("@Currency", currency);
            return Select(query);
        }

        public DataTable DailyDonations(DateTime from, DateTime until, string bank, string currency)
        {
            var query = new SqlQuery(@"SELECT B.HesapKodu,B.HesapAdi,B.BankaGrup Banka,SUM(A.BagisMiktari) ToplamBagis,
                    SUM(ISNULL(A.DovizTutari,0)) DovizTutari
                FROM NakitBagisHareket_Table A LEFT JOIN BankaTanim_Table B ON B.Id=A.BankaId
                WHERE A.BagisTarihi>=@From AND A.BagisTarihi<@Until AND A.DovizCinsi=@Currency"
                + (string.IsNullOrEmpty(bank) ? "" : " AND B.BankaGrup=@Bank")
                + " GROUP BY B.HesapKodu,B.HesapAdi,B.BankaGrup ORDER BY B.HesapKodu,B.HesapAdi,B.BankaGrup");
            Range(query, from, until);
            query.AddParameter("@Currency", currency);
            if (!string.IsNullOrEmpty(bank)) query.AddParameter("@Bank", bank);
            return Select(query);
        }

        public DataTable Payments(DateTime from, DateTime until, int? reason)
        {
            // Filter the effective row reason: a split owns its reason; only unsplit payments use the parent reason.
            var query = new SqlQuery(@"SELECT A.OdemeTarihi,A.OdemeSebebiId,B.OdemeSebebi OdemeSebebiA,D.OdemeSebebi OdemeSebebiB,
                    A.Adi AdiA,E.Adi AdiB,E.Soyadi,A.Tutar TutarA,C.Tutar TutarB,A.Aciklama AciklamaA,C.Aciklama AciklamaB
                FROM KiraEkstreAktarma_Table A LEFT JOIN OdemeSebebiTanim_Table B ON B.Id=A.OdemeSebebiId
                LEFT JOIN OdemeAyristirma_Table C ON C.KiraEkstreAktarmaId=A.Id
                LEFT JOIN OdemeSebebiTanim_Table D ON D.Id=C.OdemeSebebiId LEFT JOIN Kiraci_Table E ON E.Id=A.KiraciId
                WHERE A.OdemeTarihi>=@From AND A.OdemeTarihi<@Until"
                + (reason.HasValue ? " AND ((C.Id IS NULL AND A.OdemeSebebiId=@Reason) OR C.OdemeSebebiId=@Reason)" : "")
                + " ORDER BY A.OdemeTarihi,A.Id,C.Id");
            Range(query, from, until);
            if (reason.HasValue) query.AddParameter("@Reason", reason.Value);
            return Select(query);
        }

        private static void Range(SqlQuery query, DateTime from, DateTime until)
        {
            if (from < new DateTime(1753, 1, 1) || until <= from)
                throw new ArgumentException("Rapor tarih aralığı geçersiz.");
            query.AddParameter("@From", from);
            query.AddParameter("@Until", until);
        }
    }
}
