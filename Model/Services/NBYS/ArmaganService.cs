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
    public class ArmaganService
    {
        private readonly ArmaganRepository repository;

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

        public List<Armagan> GetByBagisciIdAndDurum(int bagisciId, string durum)
        {
            return Map(repository.SelectByBagisciIdAndDurum(bagisciId, durum));
        }

        public Armagan GetByBagisciIdAndTanimId(int bagisciId, int armaganTanimId)
        {
            return Map(repository.SelectByBagisciIdAndTanimId(bagisciId, armaganTanimId)).FirstOrDefault();
        }

        public int CountByBagisciIdAndTanimId(int bagisciId, int armaganTanimId)
        {
            return repository.CountByBagisciIdAndTanimId(bagisciId, armaganTanimId);
        }

        public bool UpdateDurumByBolge(
            string fromDurum, string toDurum, string baslangic, string bitis,
            int armaganTanimId, int bolgeId)
        {
            int? bolgeFiltresi = bolgeId == ProjeConstants.BOLGE_HEPSI_INT ||
                bolgeId == ProjeConstants.BOLGE_GENELMUDURLUK_INT ? (int?)null : bolgeId;
            return repository.UpdateDurumByBolge(
                fromDurum, toDurum, baslangic, bitis, armaganTanimId, bolgeFiltresi);
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

        public DataTable ListVerilenArmaganlarGroupByBagisci()
        {
            return repository.SelectVerilenArmaganlarGroupByBagisci();
        }

        public int Save(Armagan armagan)
        {
            if (armagan == null)
                throw new ArgumentNullException("armagan");

            armagan.OlusturmaTarihi = DateTime.Now;
            armagan.Olusturan = UtilityHelper.GetCurrentUserName();
            armagan.Id = repository.Insert(armagan);

            if (armagan.Id > 0 && ProjeConstants.NBYS_SAVE_LOG)
            {
                new OlayKayit().GirisOlayKaydet(
                    armagan, ProjeConstants.NBYS, ProjeConstants.NBYS_ARMAGAN);
            }
            return armagan.Id;
        }

        public bool Update(Armagan armagan)
        {
            if (armagan == null)
                throw new ArgumentNullException("armagan");

            Armagan previous = GetById(armagan.Id);
            bool updated = false;
            if (armagan.Id != 0)
            {
                armagan.DegistirmeTarihi = DateTime.Now;
                armagan.Degistiren = UtilityHelper.GetCurrentUserName();
                updated = repository.Update(armagan);
            }
            if (updated && ProjeConstants.NBYS_UPDATE_LOG)
            {
                new OlayKayit().GuncellemeOlayKaydet(
                    armagan, previous, ProjeConstants.NBYS, ProjeConstants.NBYS_ARMAGAN);
            }
            return updated;
        }

        public bool Delete(Armagan armagan)
        {
            if (armagan == null)
                throw new ArgumentNullException("armagan");
            if (armagan.Id == 0)
                return false;

            Armagan previous = GetById(armagan.Id);
            if (previous == null)
                return false;

            bool deleted = repository.Delete(armagan.Id);
            if (deleted && ProjeConstants.NBYS_DELETE_LOG)
            {
                new OlayKayit().SilmeOlayKaydet(
                    previous, ProjeConstants.NBYS, ProjeConstants.NBYS_ARMAGAN);
            }
            return deleted;
        }

        private static List<Armagan> Map(DataTable table)
        {
            return new Armagan().ToList<Armagan>(table);
        }
    }
}
