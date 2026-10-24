using Utility.Logging;
using DAO.Repositories.NBYS;
using DAO.Ortak;
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
    public class ArmaganService
    {
        private readonly ArmaganRepository repository;

        public ArmaganService(PortalLogContext context) : this(new ArmaganRepository(context)) { }

        public ArmaganService() : this(new ArmaganRepository())
        {
        }

        public ArmaganService(ArmaganRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public Armagan GetById(int id)
        {
            return Map(repository.SelectById(id)).FirstOrDefault();
        }

        public Armagan GetById(int id, PortalLogContext context)
        {
            return Map(repository.SelectById(id, context)).FirstOrDefault();
        }

        public Armagan GetById(int id, SqlTransactionContext transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");
            return Map(repository.SelectById(id, transaction)).FirstOrDefault();
        }

        public List<Armagan> GetAll()
        {
            return Map(repository.SelectAll());
        }

        public Armagan GetByBagisciIdDateRange(int bagisciId, DateTime baslangic, DateTime bitis)
        {
            return Map(repository.SelectByBagisciIdDateRange(bagisciId, baslangic, bitis)).FirstOrDefault();
        }

        public Armagan GetByBagisciIdTanimIdDateRange(
            int bagisciId, int armaganTanimId, DateTime baslangic, DateTime bitis)
        {
            return Map(repository.SelectByBagisciIdTanimIdDateRange(
                bagisciId, armaganTanimId, baslangic, bitis)).FirstOrDefault();
        }

        public List<Armagan> GetByBagisciId(int bagisciId)
        {
            return Map(repository.SelectByBagisciId(bagisciId));
        }

        public List<Armagan> GetByBagisciId(int bagisciId, SqlTransactionContext transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");
            return Map(repository.SelectByBagisciId(bagisciId, transaction));
        }

        public List<Armagan> GetByBagisciIdAndDurum(int bagisciId, string durum)
        {
            return Map(repository.SelectByBagisciIdAndDurum(bagisciId, durum));
        }

        public List<Armagan> GetByBagisciIdAndDurum(
            int bagisciId,
            string durum,
            SqlTransactionContext transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");
            return Map(repository.SelectByBagisciIdAndDurum(bagisciId, durum, transaction));
        }

        public Armagan GetByBagisciIdAndTanimId(int bagisciId, int armaganTanimId)
        {
            return Map(repository.SelectByBagisciIdAndTanimId(bagisciId, armaganTanimId)).FirstOrDefault();
        }

        public int CountByBagisciIdAndTanimId(int bagisciId, int armaganTanimId)
        {
            return repository.CountByBagisciIdAndTanimId(bagisciId, armaganTanimId);
        }

        public int CountByBagisciIdAndTanimId(
            int bagisciId,
            int armaganTanimId,
            SqlTransactionContext transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");
            return repository.CountByBagisciIdAndTanimId(
                bagisciId, armaganTanimId, transaction);
        }

        public DataTable ListByDurumTarih(
            string durum, DateTime baslangic, DateTime bitis, string armaganTanimId,
            int bolgeId, int ilId)
        {
            string durumFiltresi = durum == ProjeConstants.HEPSI ? null : durum;
            int? tanimFiltresi = armaganTanimId == ProjeConstants.HEPSI
                ? (int?)null : Convert.ToInt32(armaganTanimId);
            int? bolgeFiltresi = bolgeId == ProjeConstants.BOLGE_HEPSI_INT ||
                bolgeId == ProjeConstants.BOLGE_GENELMUDURLUK_INT ? (int?)null : bolgeId;
            int? ilFiltresi = ilId == ProjeConstants.HEPSI_INT ? (int?)null : ilId;
            return repository.SelectByDurumTarih(
                durumFiltresi, baslangic, bitis, tanimFiltresi, bolgeFiltresi, ilFiltresi);
        }

        public string ListByDurumTarihJson(
            string durum, DateTime baslangic, DateTime bitis, string armaganTanimId,
            ref int rowCount, int bolgeId, int ilId)
        {
            DataTable table = ListByDurumTarih(
                durum, baslangic, bitis, armaganTanimId, bolgeId, ilId);
            rowCount = table == null ? 0 : table.Rows.Count;
            return new Armagan().ToJSON(table);
        }

        public DataTable CountDurumByBolge(
            DateTime baslangic, DateTime bitis, int armaganTanimId, int bolgeId)
        {
            int? bolgeFiltresi = bolgeId == ProjeConstants.HEPSI_INT ||
                bolgeId == ProjeConstants.BOLGE_HEPSI_INT ||
                bolgeId == ProjeConstants.BOLGE_GENELMUDURLUK_INT ? (int?)null : bolgeId;
            return repository.SelectCountDurumByBolge(
                baslangic, bitis, armaganTanimId, bolgeFiltresi);
        }

        public DataTable CountByBagisTarihiBolge(DateTime baslangic, DateTime bitis)
        {
            return repository.SelectCountByBagisTarihiBolge(baslangic, bitis);
        }

        public string SearchJson(string filter, int excludedDonorId)
        {
            return new Armagan().ToJSON(repository.SelectByFilter(filter, excludedDonorId));
        }

        public string SearchJson(string filter, int excludedDonorId, int regionId)
        {
            int? region = regionId == ProjeConstants.BOLGE_HEPSI_INT || regionId == ProjeConstants.BOLGE_GENELMUDURLUK_INT
                ? (int?)null : regionId;
            return new Armagan().ToJSON(repository.SelectByFilter(filter, excludedDonorId, region));
        }

        public DataTable ListVerilenArmaganlarGroupByBagisci()
        {
            return repository.SelectVerilenArmaganlarGroupByBagisci();
        }

        public int Save(Armagan armagan, SqlTransactionContext transaction)
        {
            if (!repository.HasLogContext) throw new InvalidOperationException("Yazma işlemi için NBYS log bağlamı gereklidir.");
            if (armagan == null)
                throw new ArgumentNullException("armagan");
            if (transaction == null)
                throw new ArgumentNullException("transaction");

            armagan.OlusturmaTarihi = DateTime.Now;
            armagan.Olusturan = UtilityHelper.GetCurrentUserName();
            armagan.Id = repository.Insert(armagan, transaction);

            return armagan.Id;
        }

        public Armagan GetDuzenliBagisByBelgeSirasi(int bagisciId, int belgeSirasi)
        {
            return Map(repository.SelectByBagisciIdTanimIdBelgeSirasi(
                bagisciId, ProjeConstants.ARMAGAN_DUZENLIBAGISCIBELGESIID, belgeSirasi)).FirstOrDefault();
        }

        public bool Update(Armagan armagan, SqlTransactionContext transaction)
        {
            if (!repository.HasLogContext) throw new InvalidOperationException("Yazma işlemi için NBYS log bağlamı gereklidir.");
            if (armagan == null)
                throw new ArgumentNullException("armagan");
            if (transaction == null)
                throw new ArgumentNullException("transaction");
            if (armagan.Id == 0)
                return false;

            armagan.DegistirmeTarihi = DateTime.Now;
            armagan.Degistiren = UtilityHelper.GetCurrentUserName();
            bool updated = repository.Update(armagan, transaction);

            return updated;
        }

        private static List<Armagan> Map(DataTable table)
        {
            return new Armagan().ToList<Armagan>(table);
        }
    }
}
