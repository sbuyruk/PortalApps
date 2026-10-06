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

        public int SaveOrUpdate(
            Armagan armagan, DateTime baslangic, DateTime bitis,
            int bagisciId, IList<NakitBagisHareket> bagisHareketleri)
        {
            if (armagan == null)
                throw new ArgumentNullException("armagan");
            if (bagisHareketleri == null)
                throw new ArgumentNullException("bagisHareketleri");

            try
            {
                Armagan existing = GetByBagisciIdDateRange(bagisciId, baslangic, bitis);
                string durum = GetDonorStatus(bagisciId);
                if (!string.IsNullOrEmpty(durum))
                    armagan.Durum = durum;

                int armaganId;
                if (existing != null)
                {
                    // Legacy behavior: an existing period record is retained.
                    Update(armagan);
                    armaganId = existing.Id;
                }
                else
                {
                    armaganId = Save(armagan);
                }

                NakitBagisHareketService bagisHareketService = new NakitBagisHareketService();
                foreach (NakitBagisHareket bagisHareketi in bagisHareketleri)
                {
                    bagisHareketi.ArmaganId = armaganId;
                    try
                    {
                        bagisHareketService.Update(bagisHareketi);
                    }
                    catch (Exception exception)
                    {
                        new ExceptionHelper(exception).PublishException();
                    }
                }
                return armaganId;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private static string GetDonorStatus(int bagisciId)
        {
            NakitBagisci bagisci = new NakitBagisciService().GetById(bagisciId);
            if (bagisci == null)
                return string.Empty;
            if (bagisci.BelgeIstemiyor)
                return ProjeConstants.DURUM_BELGE_ISTEMIYOR;
            if (bagisci.Ulasilamiyor)
                return ProjeConstants.DURUM_ULASILAMADI;
            return string.Empty;
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

        public Armagan GetDuzenliBagisByBelgeSirasi(int bagisciId, int belgeSirasi)
        {
            return Map(repository.SelectByBagisciIdTanimIdBelgeSirasi(
                bagisciId, ProjeConstants.ARMAGAN_DUZENLIBAGISCIBELGESIID, belgeSirasi)).FirstOrDefault();
        }

        public int SaveDuzenliBagisIfMissing(Armagan armagan)
        {
            if (armagan == null)
                throw new ArgumentNullException("armagan");
            if (!armagan.DuzenliBagis || armagan.KacinciBelge < 1
                || armagan.ArmaganTanimId != ProjeConstants.ARMAGAN_DUZENLIBAGISCIBELGESIID)
                throw new ArgumentException("Geçerli bir düzenli bağış yılı belirtilmelidir.", "armagan");

            armagan.OlusturmaTarihi = DateTime.Now;
            armagan.Olusturan = UtilityHelper.GetCurrentUserName();
            DataTable result = repository.InsertDuzenliBagisIfMissing(armagan);
            armagan.Id = Convert.ToInt32(result.Rows[0]["Id"]);
            if (Convert.ToBoolean(result.Rows[0]["Olusturuldu"]) && ProjeConstants.NBYS_SAVE_LOG)
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
