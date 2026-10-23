using DAO.Repositories.MFYS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.Logging;
using Utility.ProjeGlobal;

namespace Model.Services.MFYS
{
    public sealed class MfysReportService
    {
        private readonly MfysReportRepository repository;
        public MfysReportService(PortalLogContext context) { repository = new MfysReportRepository(context); }

        public List<string> BankGroups(bool secondary) { return Groups(repository.BankGroups(secondary)); }
        public DataTable PaymentReasons() { return repository.PaymentReasons(); }
        public DataTable DailyTlTotals(DateTime from, DateTime until, string bank) { return repository.DailyTlTotals(from, until, bank, ProjeConstants.DOVIZ_TL); }
        public DataTable CurrencyDonations(DateTime from, DateTime until, string bank, string currency) { return repository.CurrencyDonations(from, until, bank, currency); }
        public List<string> CurrencyBankGroups(DateTime from, DateTime until, string currency) { return Groups(repository.CurrencyBankGroups(from, until, currency)); }
        public DataTable DailyDonations(DateTime day, string bank, string currency) { return repository.DailyDonations(day.Date, day.Date.AddDays(1), bank, currency); }
        public DataTable Payments(DateTime from, DateTime until, int reason) { return repository.Payments(from, until, reason == ProjeConstants.HEPSI_INT ? (int?)null : reason); }

        private static List<string> Groups(DataTable table)
        {
            return table == null ? new List<string>() : table.AsEnumerable().Select(row => Convert.ToString(row["Banka"]))
                .Where(value => !string.IsNullOrWhiteSpace(value)).Distinct().ToList();
        }
    }
}
