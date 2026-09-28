using DAO.Repositories.NBYS;
using Model.NBYS;
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

        private static List<NakitBagisHareket> Map(DataTable table)
        {
            return new NakitBagisHareket().ToList<NakitBagisHareket>(table);
        }
    }
}
