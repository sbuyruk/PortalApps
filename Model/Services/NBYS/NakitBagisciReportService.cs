using DAO.Repositories.NBYS;
using System;
using System.Data;
using Utility.ProjeGlobal;

namespace Model.Services.NBYS
{
    public class NakitBagisciReportService
    {
        private readonly NakitBagisciReportRepository repository;

        public NakitBagisciReportService()
            : this(new NakitBagisciReportRepository())
        {
        }

        public NakitBagisciReportService(NakitBagisciReportRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            this.repository = repository;
        }

        public DataTable GetAdresRaporu(
            DateTime baslangicTarihi,
            DateTime bitisTarihi,
            int bagisciSayisi,
            bool belgeIstemeyenlerHaric,
            bool adresiBosOlanlarHaric,
            bool postadanIadelerHaric,
            bool dergiGonderilmesinlerHaric,
            bool ulasilamayanlarHaric,
            bool sadeceYeniBagiscilar)
        {
            NakitBagisciAdresRaporKriteri kriter = new NakitBagisciAdresRaporKriteri
            {
                BaslangicTarihi = baslangicTarihi,
                BitisTarihi = bitisTarihi,
                TLGecisTarihi = ProjeConstants.TL_GECIS_TARIHI,
                BagisciSayisi = bagisciSayisi,
                BelgeIstemeyenlerHaric = belgeIstemeyenlerHaric,
                AdresiBosOlanlarHaric = adresiBosOlanlarHaric,
                PostadanIadelerHaric = postadanIadelerHaric,
                DergiGonderilmesinlerHaric = dergiGonderilmesinlerHaric,
                UlasilamayanlarHaric = ulasilamayanlarHaric,
                SadeceYeniBagiscilar = sadeceYeniBagiscilar,
                ParaIadeDurumu = ProjeConstants.DURUM_PARAIADE,
                DahaOnceIadeDurumu = ProjeConstants.DURUM_DAHAONCEIADE
            };

            return repository.SelectByBagisTarihiBagisSayisi(kriter);
        }
    }
}
