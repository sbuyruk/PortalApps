using DAO.Ortak;
using DAO.Repositories.NBYS;
using Model.NBYS;
using Model.Ortak;
using System;
using System.Data;
using System.Linq;
using Utility.Logging;
using Utility.ProjeGlobal;

namespace Model.Services.NBYS
{
    public enum NbysReportWriteResult { Completed, Unchanged, InvalidSelection, NotFound, Forbidden, Changed, NoEntitlement, AlreadyAwarded, InvalidDate, InvalidStatus, DeclinedCertificate, RefundRequiresReview }

    public sealed class NbysReportWriteService
    {
        private readonly PortalLogContext context;
        private readonly NbysBagisciWriteRepository records;
        private readonly NbysReportWriteRepository reports;

        public NbysReportWriteService(PortalLogContext context)
        {
            this.context = context;
            records = new NbysBagisciWriteRepository(context);
            reports = new NbysReportWriteRepository(context);
        }

        public NbysReportWriteResult SetMagazine(int donorId, bool doNotSend)
        {
            if (donorId <= 0) return NbysReportWriteResult.InvalidSelection;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                NakitBagisci donor = First<NakitBagisci>(records.SelectDonor(donorId, transaction));
                if (donor == null) return NbysReportWriteResult.NotFound;
                if (!CanAccess(donor, transaction)) return NbysReportWriteResult.Forbidden;
                if (donor.DergiGonderilmesin == doNotSend) return NbysReportWriteResult.Unchanged;
                reports.SetMagazine(donorId, doNotSend, context.UserLogin, transaction);
                transaction.Complete();
            }
            return NbysReportWriteResult.Completed;
        }

        public NbysReportWriteResult CreateMedal(int donorId, int definitionId, out int giftId)
        {
            giftId = 0;
            if (donorId <= 0 || definitionId <= 0) return NbysReportWriteResult.InvalidSelection;
            int savedId;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                NakitBagisci donor = First<NakitBagisci>(records.SelectDonor(donorId, transaction));
                if (donor == null) return NbysReportWriteResult.NotFound;
                if (!CanAccess(donor, transaction)) return NbysReportWriteResult.Forbidden;
                if (donor.BelgeIstemiyor) return NbysReportWriteResult.DeclinedCertificate;
                var previous = new Armagan().ToList<Armagan>(records.SelectDonorRows("Armagan_Table", donorId, transaction));
                if (previous.Any(x => x.BelgeGecersizMi != 1 && x.ArmaganTanimId == definitionId)) return NbysReportWriteResult.AlreadyAwarded;
                var donations = new NakitBagisHareket().ToList<NakitBagisHareket>(reports.EligibleDonations(donorId, transaction));
                if (donations.Any(x => x.IadeEdildiMi || x.IadeMiktari > 0)) return NbysReportWriteResult.RefundRequiresReview;
                if (donations.Count < 2) return NbysReportWriteResult.NoEntitlement;
                decimal amount = donations.Sum(x => x.BagisMiktari);
                DataTable definitions = records.SelectGiftDefinition(amount, donor.TuzelKisi, transaction);
                if (definitions == null || definitions.Rows.Count != 1) return NbysReportWriteResult.NoEntitlement;
                int actualDefinitionId = Convert.ToInt32(definitions.Rows[0]["Id"]);
                if (actualDefinitionId != definitionId) return NbysReportWriteResult.Changed;
                if (definitionId != ProjeConstants.ARMAGAN_ALTINID && definitionId != ProjeConstants.ARMAGAN_GUMUSID
                    && definitionId != ProjeConstants.ARMAGAN_BRONZID) return NbysReportWriteResult.NoEntitlement;
                DateTime date = donations.Max(x => x.BagisTarihi);
                if (date < new DateTime(1753, 1, 1)) return NbysReportWriteResult.InvalidDate;
                string status = donor.Ulasilamiyor ? ProjeConstants.DURUM_ULASILAMADI : ProjeConstants.DURUM_GONDERILMEDI;
                if (previous.Any(x => x.Durum == ProjeConstants.DURUM_PARAIADE)) status = ProjeConstants.DURUM_DAHAONCEIADE;
                Armagan gift = new Armagan { BagisciId = donorId, ArmaganTanimId = definitionId,
                    Tarih = date, BagisMiktari = amount, DovizCinsi = ProjeConstants.DOVIZ_TL,
                    CokluBagis = true, Durum = status, Olusturan = context.UserLogin, OlusturmaTarihi = DateTime.Now };
                savedId = records.SaveGift(gift, 0, transaction);
                foreach (NakitBagisHareket donation in donations)
                    records.LinkDonation(donation.Id, savedId, context.UserLogin, DateTime.Now, transaction);
                transaction.Complete();
            }
            giftId = savedId;
            return NbysReportWriteResult.Completed;
        }

        public NbysReportWriteResult SetPropertyGift(int id, string number, string status, DateTime date, string description)
        {
            if (id <= 0) return NbysReportWriteResult.InvalidSelection;
            if (date < new DateTime(1753, 1, 1)) return NbysReportWriteResult.InvalidDate;
            if (!new[] { ProjeConstants.DURUM_BOS, ProjeConstants.DURUM_BELGE_ISTEMIYOR, ProjeConstants.DURUM_GONDERILMEDI,
                ProjeConstants.DURUM_GONDERILDI, ProjeConstants.DURUM_ULASILAMADI, ProjeConstants.DURUM_KONTROLEDILDI,
                ProjeConstants.DURUM_ERKENGONDERILDI, ProjeConstants.DURUM_ERTELENDI, ProjeConstants.DURUM_PARAIADE,
                ProjeConstants.DURUM_DAHAONCEIADE }.Contains(status)) return NbysReportWriteResult.InvalidStatus;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                var current = First<Model.TBYS.Bagis>(reports.PropertyDonation(id, transaction));
                if (current == null) return NbysReportWriteResult.NotFound;
                Bolge region = First<Bolge>(records.SelectUserRegion(context.UserLogin, transaction));
                if (region == null || (!IsCentral(region) && !reports.CanAccessPropertyDonor(current.BagisciId, region.Id, transaction)))
                    return NbysReportWriteResult.Forbidden;
                if ((current.ArmaganId ?? "") == (number ?? "") && current.ArmaganDurumu == status && current.ArmaganTarihi == date
                    && (current.ArmaganAciklama ?? "") == (description ?? "")) return NbysReportWriteResult.Unchanged;
                reports.SetPropertyGift(id, number, status, date, description, context.UserLogin, transaction);
                transaction.Complete();
            }
            return NbysReportWriteResult.Completed;
        }

        private bool CanAccess(NakitBagisci donor, SqlTransactionContext transaction)
        {
            Bolge region = First<Bolge>(records.SelectUserRegion(context.UserLogin, transaction));
            if (region == null) return false;
            if (IsCentral(region)) return true;
            DataTable province = records.SelectProvinceRegion(donor.Ili, transaction);
            return region.Id > 0 && province != null && !province.Rows[0].IsNull("BolgeId")
                && Convert.ToInt32(province.Rows[0]["BolgeId"]) == region.Id;
        }

        private static bool IsCentral(Bolge region)
        {
            return region.Id == ProjeConstants.BOLGE_HEPSI_INT || region.Id == ProjeConstants.BOLGE_GENELMUDURLUK_INT;
        }

        private static T First<T>(DataTable rows) where T : new()
        {
            return new Armagan().ToList<T>(rows).FirstOrDefault();
        }
    }
}
