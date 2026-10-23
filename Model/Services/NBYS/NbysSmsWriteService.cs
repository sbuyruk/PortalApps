using DAO.Ortak;
using DAO.Repositories.NBYS;
using Model.NBYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Linq;
using Utility.Logging;
using Utility.ProjeGlobal;

namespace Model.Services.NBYS
{
    public enum NbysSmsWriteResult { Completed, Unchanged, InvalidYear, InvalidSelection, InvalidCounts, NotFound, Forbidden, Conflict, InvalidYearData, Busy }

    public sealed class NbysSmsWriteService
    {
        private readonly PortalLogContext context;
        private readonly SMSAylikBagisRepository repository;
        private readonly NbysBagisciWriteRepository access;

        public NbysSmsWriteService(PortalLogContext context)
        {
            this.context = context;
            repository = new SMSAylikBagisRepository(context);
            access = new NbysBagisciWriteRepository(context);
        }

        public bool CanWrite()
        {
            if (string.IsNullOrWhiteSpace(context.UserLogin)) return false;
            return IsCentral(new Bolge().ToList<Bolge>(access.SelectUserRegion(context.UserLogin)).FirstOrDefault());
        }

        private bool CanWrite(SqlTransactionContext transaction)
        {
            if (string.IsNullOrWhiteSpace(context.UserLogin)) return false;
            return IsCentral(new Bolge().ToList<Bolge>(access.SelectUserRegion(context.UserLogin, transaction)).FirstOrDefault());
        }

        private static bool IsCentral(Bolge region)
        {
            // Monthly operator counts are national totals; there is no donor/province owner on this record.
            return region != null && (region.Id == ProjeConstants.BOLGE_HEPSI_INT || region.Id == ProjeConstants.BOLGE_GENELMUDURLUK_INT);
        }

        public NbysSmsWriteResult EnsureYear(int year, out int createdMonths)
        {
            createdMonths = 0;
            if (!ValidYear(year)) return NbysSmsWriteResult.InvalidYear;
            int created = 0;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                if (!CanWrite(transaction)) return NbysSmsWriteResult.Forbidden;
                if (!repository.LockYear(year, transaction)) return NbysSmsWriteResult.Busy;
                List<SMSAylikBagis> current = Rows(year, transaction);
                if (!ValidRows(current)) return NbysSmsWriteResult.InvalidYearData;
                for (int month = 1; month <= 12; month++)
                {
                    if (current.Any(existingMonth => existingMonth.Ay == month)) continue;
                    SMSAylikBagis item = new SMSAylikBagis
                    {
                        Yil = year, Ay = month, DovizCinsi = ProjeConstants.DOVIZ_TL, SMSTutari = DefaultUnitAmount(year, month),
                        IslemTarihi = DateTime.Now, OlusturmaTarihi = DateTime.Now, Olusturan = context.UserLogin, Aciklama = string.Empty
                    };
                    repository.InsertMonth(item, transaction);
                    created++;
                }
                if (created == 0) return NbysSmsWriteResult.Unchanged;
                transaction.Complete();
            }
            // A new count is exposed only after Dispose/commit has also succeeded.
            createdMonths = created;
            return NbysSmsWriteResult.Completed;
        }

        public NbysSmsWriteResult UpdateCounts(SMSAylikBagis original, int turkcell, int vodafone, int telekom)
        {
            if (original == null || original.Id <= 0 || original.Ay < 1 || original.Ay > 12) return NbysSmsWriteResult.InvalidSelection;
            if (!ValidYear(original.Yil)) return NbysSmsWriteResult.InvalidYear;
            if (turkcell < 0 || vodafone < 0 || telekom < 0) return NbysSmsWriteResult.InvalidCounts;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                if (!CanWrite(transaction)) return NbysSmsWriteResult.Forbidden;
                if (!repository.LockYear(original.Yil, transaction)) return NbysSmsWriteResult.Busy;
                List<SMSAylikBagis> year = Rows(original.Yil, transaction);
                if (!ValidRows(year)) return NbysSmsWriteResult.InvalidYearData;
                SMSAylikBagis current = year.FirstOrDefault(item => item.Id == original.Id);
                if (current == null) return NbysSmsWriteResult.NotFound;
                if (current.Ay != original.Ay) return NbysSmsWriteResult.Conflict;
                if (current.TurkcellSMSAdedi == turkcell && current.VodafoneSMSAdedi == vodafone && current.TurkTelekomSMSAdedi == telekom)
                    return NbysSmsWriteResult.Unchanged;
                if (current.TurkcellSMSAdedi != original.TurkcellSMSAdedi || current.VodafoneSMSAdedi != original.VodafoneSMSAdedi
                    || current.TurkTelekomSMSAdedi != original.TurkTelekomSMSAdedi || current.SMSTutari != original.SMSTutari
                    || current.DegistirmeTarihi != original.DegistirmeTarihi || current.OlusturmaTarihi != original.OlusturmaTarihi
                    || current.IslemTarihi != original.IslemTarihi || (current.DovizCinsi ?? "") != (original.DovizCinsi ?? ""))
                    return NbysSmsWriteResult.Conflict;
                repository.UpdateCounts(current.Id, turkcell, vodafone, telekom, context.UserLogin, transaction);
                transaction.Complete();
            }
            return NbysSmsWriteResult.Completed;
        }

        private List<SMSAylikBagis> Rows(int year, SqlTransactionContext transaction)
        {
            return new SMSAylikBagis().ToList<SMSAylikBagis>(repository.SelectYearForWrite(year, transaction));
        }

        public static bool ValidRows(IEnumerable<SMSAylikBagis> rows)
        {
            return rows.All(item => item.Ay >= 1 && item.Ay <= 12) && !rows.GroupBy(item => item.Ay).Any(group => group.Count() > 1);
        }

        public static bool ValidYear(int year)
        {
            return year >= 2019 && year <= DateTime.Today.Year;
        }

        private static decimal DefaultUnitAmount(int year, int month)
        {
            DateTime date = new DateTime(year, month, 1);
            if (date >= ProjeConstants.SMS_2024) return ProjeConstants.SMS_TUTAR_2024;
            if (date >= ProjeConstants.SMS_2023) return ProjeConstants.SMS_TUTAR_2023;
            return ProjeConstants.SMS_TUTAR_2022;
        }
    }
}
