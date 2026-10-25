using DAO.Ortak;
using DAO.Repositories.TBYS;
using Model.TBYS;
using System;
using System.Data;
using System.Linq;
using Utility.Logging;
using Utility.ProjeGlobal;

namespace Model.Services.TBYS
{
    public enum KiraSozlesmeYenilemeResult { Completed, NotFound, Inactive, InvalidInput }

    public sealed class KiraSozlesmeYenilemeService
    {
        private readonly PortalLogContext context;

        public KiraSozlesmeYenilemeService(PortalLogContext context)
        {
            if (context == null || context.Project != "TBYS" || string.IsNullOrWhiteSpace(context.UserLogin))
                throw new ArgumentException("TBYS kullanıcı bağlamı gereklidir.", "context");
            this.context = context;
        }

        public KiraSozlesmeYenilemeResult Renew(int contractId, DateTime changedAt, string increaseMonth, out int newId)
        {
            newId = 0;
            if (contractId <= 0 || changedAt < new DateTime(1753, 1, 1))
                return KiraSozlesmeYenilemeResult.InvalidInput;

            int savedId;
            using (var transaction = new SqlTransactionContext())
            {
                var contracts = new KiraSozlesmeService(context);
                // The row lock serializes concurrent renewals of the same source contract.
                KiraSozlesme previous = contracts.GetById(contractId, transaction);
                if (previous == null) return KiraSozlesmeYenilemeResult.NotFound;
                if (!previous.Aktif) return KiraSozlesmeYenilemeResult.Inactive;

                var links = new SozlesmeTasinmazService(context);
                var previousLinks = links.GetBySozlesmeId(contractId, transaction);
                Kiraci tenant = new Kiraci().ToList<Kiraci>(
                    new KiraciRepository(context).SelectById(previous.KiraciId, transaction, context)).FirstOrDefault();
                if (tenant == null) throw new InvalidOperationException("Sözleşmenin kiracı kaydı bulunamadı.");

                DateTime today = DateTime.Today;
                DateTime start = previous.SozBasTar < new DateTime(1901, 1, 1) ? today : previous.SozBasTar;
                if (start.Year > 9997) return KiraSozlesmeYenilemeResult.InvalidInput;
                start = start.AddYears(1);
                decimal rent = previous.KiraBedeli;
                YasalFaiz rate = new YasalFaizService(context).GetByYearMonth(start.Year, start.Month, transaction);
                if (rate != null)
                {
                    decimal increase = start >= ProjeConstants.SINIRLIKIRAARTISI_BASLAMATARIHI
                        && start <= ProjeConstants.SINIRLIKIRAARTISI_BITISTARIHI
                        && tenant.KiralamaAmaci == ProjeConstants.SINIRLIKIRAARTISI_UYGULANACAKTASINMAZCINSI
                            ? ProjeConstants.SINIRLIKIRAARTISI_ORANI : rate.Tufe;
                    rent += rent * increase / 100;
                }

                DateTime balanceDate = previous.SozlesmeDurumu == ProjeConstants.KIRASOZLESME_DURUMU_TAKIP
                    ? previous.DurumDegismeTar : today;
                if (balanceDate < new DateTime(1753, 1, 1)) return KiraSozlesmeYenilemeResult.InvalidInput;
                var plans = new OdemePlaniService(context);
                OdemePlani balance = plans.GetBySozlesmeIdOdemeTarihi(contractId, balanceDate, transaction)
                    ?? plans.GetBySozlesmeId(contractId, transaction).OrderBy(p => p.VadeBasTar).ThenBy(p => p.Id).LastOrDefault();
                DataTable region = new KiraSozlesmeRepository(context).SelectRenewalRegion(tenant.Ili, transaction, context);

                KiraSozlesme next = CopyForRenewal(previous);
                next.IlkSozlesmeTar = previous.IlkSozlesmeTar < new DateTime(1901, 1, 1) ? today : previous.IlkSozlesmeTar;
                next.SozBasTar = start;
                next.SozBitTar = start.AddYears(1);
                next.KiraBedeli = Math.Round(rent);
                next.DevirAnaPara = balance == null ? 0 : balance.AnaPara;
                next.DevirFaizliBakiye = balance == null ? 0 : balance.FaizliBakiye;
                next.DevirFaizTutari = next.DevirFaizliBakiye - next.DevirAnaPara;
                next.ArtisAyi = increaseMonth;
                next.BolgeId = region == null || region.Rows[0].IsNull("BolgeId") ? 0 : Convert.ToInt32(region.Rows[0]["BolgeId"]);
                next.Aktif = true;
                next.SozlesmeDurumu = ProjeConstants.KIRASOZLESME_DURUMU_DEVAM;
                next.Degistiren = context.UserLogin;

                if (!contracts.UpdateActiveStatus(previous, ProjeConstants.KIRASOZLESME_DURUMU_YENILENDI,
                    changedAt, false, transaction)) return KiraSozlesmeYenilemeResult.Inactive;
                savedId = contracts.Save(next, transaction);
                if (savedId <= 0) throw new InvalidOperationException("Yeni sözleşme oluşturulamadı.");
                foreach (SozlesmeTasinmaz link in previousLinks)
                {
                    int linkId = links.Save(new SozlesmeTasinmaz
                    {
                        SozlesmeId = savedId, TasinmazId = link.TasinmazId, BolumId = link.BolumId
                    }, transaction);
                    if (linkId <= 0) throw new InvalidOperationException("Taşınmaz bağlantısı aktarılamadı.");
                }
                transaction.Complete();
            }
            // Expose the new identity only after commit succeeds.
            newId = savedId;
            return KiraSozlesmeYenilemeResult.Completed;
        }

        private static KiraSozlesme CopyForRenewal(KiraSozlesme source)
        {
            // Preserve contract terms, but never reuse the source instance or its audit identity.
            return new KiraSozlesme
            {
                KiraciId = source.KiraciId, OdemeSekli = source.OdemeSekli, TaksitSayisi = source.TaksitSayisi,
                KefilAdiSoyadi = source.KefilAdiSoyadi, KefilTCKimlikNo = source.KefilTCKimlikNo,
                KefilAdresi = source.KefilAdresi, KefilTel = source.KefilTel,
                TeminatCinsi = source.TeminatCinsi, TeminatTutari = source.TeminatTutari,
                OdenenTeminatTutari = source.OdenenTeminatTutari, IadeTeminatTutari = source.IadeTeminatTutari,
                KalanTeminatTutari = source.KalanTeminatTutari, TeminatOdemeTarihi = source.TeminatOdemeTarihi,
                TeminatAciklama = source.TeminatAciklama, DosyaNo = source.DosyaNo,
                Aciklama = source.Aciklama, SozlesmePDFDosyasi = source.SozlesmePDFDosyasi,
                Bolge = source.Bolge, GecikmeZammiTipi = source.GecikmeZammiTipi
            };
        }
    }
}
