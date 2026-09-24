using DAO.Repositories.NBYS;
using System;
using System.Data;
using Utility.HelperClasses;
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

        public DataTable GetCokDefaBagisYapanBagiscilar(decimal minimumBagisMiktari)
        {
            return repository.SelectBagisciGroupByBagisAdedi(
                minimumBagisMiktari,
                ProjeConstants.NAKITBAGISCI_BILINMEYEN,
                ProjeConstants.COKBAGISYAPAN_BASLAMATARIHI.ReturnQuotedValue().ToString(),
                ProjeConstants.COKBAGISYAPAN_SONBAGISI_KAC_AY_ONCE_YAPTI);
        }

        public DataTable GetDuzenliBagiscilar(
            DateTime baslangicTarihi,
            DateTime bitisTarihi,
            string durum)
        {
            bool sadeceBelgeOlusturulmadi = durum.Equals(ProjeConstants.DURUM_BELGEOLUSTURULMADI);
            bool durumFiltrele = !sadeceBelgeOlusturulmadi && !durum.Equals(ProjeConstants.HEPSI);

            return repository.SelectDuzenliBagisci(
                baslangicTarihi,
                bitisTarihi,
                sadeceBelgeOlusturulmadi,
                durumFiltrele,
                durum);
        }

        public DataTable GetSecilmemisFaaliyetKatilimcilari()
        {
            return repository.SelectSecilmemisKatilimcilar(
                DateTime.Today.AddYears(-2),
                ProjeConstants.NAKITBAGISCI_SORGUBAGISTUTARI);
        }
    }
}
