using DAO.Repositories.NBYS;
using Model.NBYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.Services.NBYS
{
    public class NakitBagisHareketService
    {
        private readonly NakitBagisHareketRepository repository;

        public NakitBagisHareketService() : this(new NakitBagisHareketRepository())
        {
        }

        public NakitBagisHareketService(NakitBagisHareketRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public NakitBagisHareket GetById(int id)
        {
            return Map(repository.SelectById(id)).FirstOrDefault();
        }

        public List<NakitBagisHareket> GetAll()
        {
            return Map(repository.SelectAll());
        }

        public int Save(NakitBagisHareket bagisHareket)
        {
            if (bagisHareket == null)
                throw new ArgumentNullException("bagisHareket");

            DateTime minTarih = new DateTime(1987, 9, 1);
            if (bagisHareket.BagisTarihi == default(DateTime) || bagisHareket.BagisTarihi < minTarih)
                throw new ArgumentException("BagisTarihi boş olamaz ve 01.09.1987 tarihinden önce olamaz.");

            bagisHareket.OlusturmaTarihi = DateTime.Now;
            bagisHareket.Olusturan = UtilityHelper.GetCurrentUserName();
            bagisHareket.Id = repository.Insert(bagisHareket);

            if (bagisHareket.Id > 0 && ProjeConstants.NBYS_SAVE_LOG)
            {
                new OlayKayit().GirisOlayKaydet(
                    bagisHareket, ProjeConstants.NBYS, ProjeConstants.NBYS_NAKITBAGISHAREKET);
            }
            return bagisHareket.Id;
        }

        public bool Update(NakitBagisHareket bagisHareket)
        {
            if (bagisHareket == null)
                throw new ArgumentNullException("bagisHareket");

            NakitBagisHareket previous = GetById(bagisHareket.Id);
            bool updated = false;
            if (bagisHareket.Id != 0)
            {
                bagisHareket.DegistirmeTarihi = DateTime.Now;
                bagisHareket.Degistiren = UtilityHelper.GetCurrentUserName();
                updated = repository.Update(bagisHareket);
            }
            if (updated && ProjeConstants.NBYS_UPDATE_LOG)
            {
                new OlayKayit().GuncellemeOlayKaydet(
                    bagisHareket, previous, ProjeConstants.NBYS, ProjeConstants.NBYS_NAKITBAGISHAREKET);
            }
            return updated;
        }

        public void UpdateRefund(NakitBagisHareket bagisHareket, Armagan armagan)
        {
            if (bagisHareket == null)
                throw new ArgumentNullException("bagisHareket");

            bagisHareket.DegistirmeTarihi = DateTime.Now;
            bagisHareket.Degistiren = UtilityHelper.GetCurrentUserName();

            // Armagan.GetUpdateSQL'in mevcut tarih davranışını koru.
            if (armagan != null)
                armagan.OlusturmaTarihi = DateTime.Now;

            repository.UpdateRefund(bagisHareket, armagan);
        }

        public void DeleteWithArchive(
            NakitBagisHareket bagisHareket,
            SilinenKayit bagisHareketArchive,
            Armagan armagan,
            SilinenKayit armaganArchive)
        {
            if (bagisHareket == null)
                throw new ArgumentNullException("bagisHareket");
            if (bagisHareketArchive == null)
                throw new ArgumentNullException("bagisHareketArchive");

            bagisHareketArchive.OlusturmaTarihi = DateTime.Now;
            if (armaganArchive != null)
                armaganArchive.OlusturmaTarihi = DateTime.Now;

            repository.DeleteWithArchive(
                bagisHareket.Id,
                bagisHareketArchive,
                armagan != null ? (int?)armagan.Id : null,
                armaganArchive);
        }

        public bool Delete(NakitBagisHareket bagisHareket)
        {
            if (bagisHareket == null)
                throw new ArgumentNullException("bagisHareket");
            if (bagisHareket.Id == 0)
                return false;

            NakitBagisHareket previous = GetById(bagisHareket.Id);
            if (previous == null)
                return false;

            bool deleted = repository.Delete(bagisHareket.Id);
            if (deleted && ProjeConstants.NBYS_DELETE_LOG)
            {
                new OlayKayit().SilmeOlayKaydet(
                    previous, ProjeConstants.NBYS, ProjeConstants.NBYS_NAKITBAGISHAREKET);
            }
            return deleted;
        }

        public List<NakitBagisHareket> GetByBagisciId(int bagisciId)
        {
            return Map(repository.SelectByBagisciId(bagisciId));
        }

        public NakitBagisHareket GetByEkstreAktarmaId(int ekstreAktarmaId)
        {
            return Map(repository.SelectByEkstreAktarmaId(ekstreAktarmaId)).FirstOrDefault();
        }

        public List<NakitBagisHareket> GetByBagisciIdTarih(int bagisciId, DateTime baslangic, DateTime bitis)
        {
            return Map(repository.SelectByBagisciIdTarih(bagisciId, baslangic, bitis));
        }

        public NakitBagisHareket GetLastInYearByBagisciId(int bagisciId, DateTime baslangic)
        {
            return Map(repository.SelectLastByBagisciIdTarih(
                bagisciId, baslangic, baslangic.AddYears(1))).FirstOrDefault();
        }

        public decimal GetTotalByBagisciIdDateRange(DateTime baslangic, DateTime bitis, int bagisciId)
        {
            // Preserve the legacy inclusive range ending at midnight on the end date.
            DataTable table = repository.SelectSumByBagisciIdTarih(bagisciId, baslangic.Date, bitis.Date);
            if (table == null || table.Rows.Count == 0)
                return 0;
            return table.Rows[0]["Toplam"].ReturnZeroIfNull().ConvertToDecimal();
        }

        public List<NakitBagisHareket> GetByBagisciIdDateRange(DateTime baslangic, DateTime bitis, int bagisciId)
        {
            return Map(repository.SelectByBagisciIdTarih(bagisciId, baslangic.Date, bitis.Date));
        }

        public List<NakitBagisHareket> GetArmaganiOlmayanByBagisciId(int bagisciId)
        {
            return Map(repository.SelectArmaganiOlmayanByBagisciId(bagisciId));
        }

        public DataTable Search(string filter, DateTime? bagisTarihi)
        {
            // bagisTarihi legacy API'de de sorguya uygulanmiyordu.
            return repository.SelectByFilter(filter);
        }

        public DataTable ListByDurumTarih(string ay, string yil, int ilId)
        {
            string ayFiltresi = ay == ProjeConstants.HEPSI_INT.ToString() ? null : ay;
            int? ilFiltresi = ilId > ProjeConstants.IL_HEPSI ? (int?)ilId : null;
            return repository.SelectByDurumTarih(ayFiltresi, yil, ilFiltresi);
        }

        public string ListByDurumTarihJson(string ay, string yil, int ilId)
        {
            return new NakitBagisHareket().ToJSON(ListByDurumTarih(ay, yil, ilId));
        }

        public DataTable ListIadeEdilenBagislar(string ay, string yil)
        {
            string ayFiltresi = ay == ProjeConstants.HEPSI_INT.ToString() ? null : ay;
            return repository.SelectIadeEdilenBagislar(ayFiltresi, yil);
        }

        public string ListIadeEdilenBagislarJson(string ay, string yil)
        {
            return new NakitBagisHareket().ToJSON(ListIadeEdilenBagislar(ay, yil));
        }

        public decimal GetTotalByTarihBolge(DateTime baslangic, DateTime bitis, int bolgeId, ref int adet)
        {
            int? bolgeFiltresi = bolgeId == ProjeConstants.HEPSI_INT ||
                bolgeId == ProjeConstants.BOLGE_GENELMUDURLUK_INT ? (int?)null : bolgeId;
            return ReadAggregate(repository.SelectSumByTarihBolge(baslangic, bitis, bolgeFiltresi), ref adet);
        }

        public decimal GetTotalByTarihBanka(DateTime baslangic, DateTime bitis, string banka, ref int adet)
        {
            return ReadAggregate(repository.SelectSumByTarihBanka(baslangic, bitis, banka), ref adet);
        }

        public decimal GetMaximumByTarihBanka(DateTime baslangic, DateTime bitis, string banka)
        {
            DataTable table = repository.SelectMaxByTarihBanka(baslangic, bitis, banka);
            if (table == null || table.Rows.Count == 0)
                return 0;
            return table.Rows[0]["Toplam"].ReturnZeroIfNull().ConvertToDecimal();
        }

        public DataTable GetCountByTarihBolge(int yil, int ay)
        {
            return repository.SelectCountByTarihBolge(yil, ay);
        }

        public DataTable GetCountSumByTarih(DateTime baslangic, DateTime bitis)
        {
            return repository.SelectCountSumByTarih(baslangic, bitis);
        }

        public DataTable GetCountSumByYilIl(int baslangicYili, int bitisYili)
        {
            return repository.SelectCountSumByYilIl(baslangicYili, bitisYili);
        }

        public DataTable GetCountSumByBanka(DateTime baslangic, DateTime bitis)
        {
            return repository.SelectCountSumByBanka(baslangic, bitis);
        }

        public DataTable GetDailyTotalsByBank(DateTime bagisTarihi, string bankaGrup, string dovizCinsi)
        {
            return repository.SelectDailyTotalsByBank(bagisTarihi, bankaGrup, dovizCinsi);
        }

        public decimal GetDailyTotal(DateTime bagisTarihi, string bankaGrup, string dovizCinsi)
        {
            DataTable table = repository.SelectDailySum(bagisTarihi, bankaGrup, dovizCinsi);
            if (table == null || table.Rows.Count == 0)
                return 0;
            return table.Rows[0]["Toplam"].ReturnZeroIfNull().ConvertToDecimal();
        }

        public DataTable GetDailyTlTotalByBankGroup2(DateTime bagisTarihi, string bankaGrup2)
        {
            return repository.SelectDailyTlTotalByBankGroup2(
                bagisTarihi, bankaGrup2, ProjeConstants.DOVIZ_TL);
        }

        public DataTable GetCurrencyDonationsByDateBankGroup2(
            DateTime baslangic, DateTime bitis, string bankaGrup2, string dovizCinsi)
        {
            return repository.SelectCurrencyDonationsByDateBankGroup2(
                baslangic, bitis, bankaGrup2, dovizCinsi);
        }

        public List<string> GetBankGroup2ByDateCurrency(DateTime baslangic, DateTime bitis, string dovizCinsi)
        {
            DataTable table = repository.SelectBankGroup2ByDateCurrency(baslangic, bitis, dovizCinsi);
            if (table == null)
                return new List<string>();
            return table.AsEnumerable().Select(row => row.Field<string>("BankaGrup2")).ToList();
        }

        public DataTable GetCurrencyTotalsByDateBank(DateTime baslangic, DateTime bitis, string bankaGrup)
        {
            return repository.SelectCurrencyTotalsByDateBank(
                baslangic, bitis, bankaGrup, ProjeConstants.DOVIZ_TL);
        }

        public DataTable GetDonorDetailRows(int bagisciId)
        {
            return repository.SelectDonorDetailRows(bagisciId);
        }

        public string GetDonorDetailJson(int bagisciId, ref int rowCount)
        {
            DataTable table = GetDonorDetailRows(bagisciId);
            if (table != null)
                rowCount = table.Rows.Count;
            return new NakitBagisHareket().ToJSON(table);
        }

        public DataTable GetProvinceYearSummary(int ilId, DateTime tarih)
        {
            int? ilFiltresi = ilId > ProjeConstants.IL_HEPSI ? (int?)ilId : null;
            return repository.SelectProvinceYearSummary(ilFiltresi, tarih);
        }

        public DataTable GetByRegionDate(int bolgeId, DateTime baslangic, DateTime bitis)
        {
            int? bolgeFiltresi = bolgeId == ProjeConstants.HEPSI_INT ||
                bolgeId == ProjeConstants.BOLGE_GENELMUDURLUK_INT ? (int?)null : bolgeId;
            return repository.SelectByRegionDate(bolgeFiltresi, baslangic, bitis);
        }

        public DataTable GetDonorDonationDetails(int bagisciId)
        {
            return repository.SelectDonorDonationDetails(bagisciId);
        }

        public DataTable GetDonationReport(
            DateTime? bagisBasTarihi, DateTime? bagisBitTarihi,
            decimal? minBagisMiktari, decimal? maxBagisMiktari,
            int armaganId, DateTime? sonBagisTarihi,
            int ilId, int ilceId,
            bool? sag, bool? belgeIstemiyor, bool? ulasilamiyor, bool? tuzelKisi)
        {
            int? armaganFiltresi = armaganId > ProjeConstants.HEPSI_INT ? (int?)armaganId : null;
            int? ilFiltresi = ilId > ProjeConstants.HEPSI_INT ? (int?)ilId : null;
            int? ilceFiltresi = ilceId > ProjeConstants.HEPSI_INT ? (int?)ilceId : null;

            return repository.SelectDonationReport(
                bagisBasTarihi, bagisBitTarihi,
                minBagisMiktari, maxBagisMiktari,
                armaganFiltresi, sonBagisTarihi,
                ilFiltresi, ilceFiltresi,
                sag, belgeIstemiyor, ulasilamiyor, tuzelKisi);
        }

        private static decimal ReadAggregate(DataTable table, ref int adet)
        {
            if (table == null || table.Rows.Count == 0)
                return 0;
            DataRow row = table.Rows[0];
            adet = row["Adet"].ReturnZeroIfNull().ConvertToInt();
            return row["Toplam"].ReturnZeroIfNull().ConvertToDecimal();
        }

        private static List<NakitBagisHareket> Map(DataTable table)
        {
            return new NakitBagisHareket().ToList<NakitBagisHareket>(table);
        }
    }
}
