using DAO.Repositories.TBYS;
using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.Services.TBYS
{
    public class TasinmazTaahhutService
    {
        private readonly TasinmazTaahhutRepository repository;

        public TasinmazTaahhutService() : this(new TasinmazTaahhutRepository()) { }

        public TasinmazTaahhutService(TasinmazTaahhutRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public TasinmazTaahhut GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public List<TasinmazTaahhut> GetAll()
        {
            return ToList(repository.SelectAll());
        }

        public List<TasinmazTaahhut> GetByBagisciId(int bagisciId)
        {
            return ToList(repository.SelectByBagisciId(bagisciId));
        }

        public TasinmazTaahhut GetByTcKimlikNo(long tcKimlikNo)
        {
            return Map(repository.SelectByTcKimlikNo(tcKimlikNo));
        }

        public List<TasinmazTaahhut> GetByFilters(bool isSagVefat, bool isTCKimlikNoFull, bool isDogumTarihiFull, int bolgeId)
        {
            return ToList(repository.SelectByFilters(
                isSagVefat,
                isTCKimlikNoFull,
                isDogumTarihiFull,
                bolgeId,
                ProjeConstants.BAGISCI_SAG,
                ProjeConstants.BOLGE_HEPSI_INT,
                ProjeConstants.BOLGE_GENELMUDURLUK_INT));
        }

        public List<TasinmazTaahhut> GetByIlAdi(string ilAdi)
        {
            return ToList(repository.SelectByIlAdi(ilAdi));
        }

        public string GetAllCountDonationAsJson()
        {
            return new TasinmazTaahhut().ToJSON(repository.SelectAllCountDonationAsJson());
        }

        public DataTable GetAllCountDonation(bool excludeDeceased, bool excludeHidden)
        {
            return repository.SelectAllCountDonation(excludeDeceased, excludeHidden, ProjeConstants.BAGISCI_SAG);
        }

        public int Save(TasinmazTaahhut item)
        {
            if (item == null) throw new ArgumentNullException("item");

            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);

            if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG)
            {
                OlayKayit olayKayit = new OlayKayit();
                olayKayit.GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_TASINMAZTAAHHUT);
            }

            return item.Id;
        }

        public bool Update(TasinmazTaahhut item)
        {
            if (item == null) throw new ArgumentNullException("item");

            TasinmazTaahhut oldItem = GetById(item.Id);
            bool isSuccess = false;
            if (item.Id != 0)
            {
                item.DegistirmeTarihi = DateTime.Now;
                item.Degistiren = UtilityHelper.GetCurrentUserName();
                isSuccess = repository.Update(item);
            }

            if (isSuccess && ProjeConstants.TBYS_UPDATE_LOG)
            {
                OlayKayit olayKayit = new OlayKayit();
                olayKayit.GuncellemeOlayKaydet(item, oldItem, ProjeConstants.TBYS, ProjeConstants.TBYS_TASINMAZTAAHHUT);
            }

            return isSuccess;
        }

        public bool Delete(TasinmazTaahhut item)
        {
            if (item == null || item.Id == 0) return false;

            TasinmazTaahhut oldItem = GetById(item.Id);
            if (oldItem == null) return false;

            bool isDeleted = repository.Delete(item.Id);
            if (isDeleted && ProjeConstants.TBYS_DELETE_LOG)
            {
                OlayKayit olayKayit = new OlayKayit();
                olayKayit.SilmeOlayKaydet(oldItem, ProjeConstants.TBYS, ProjeConstants.TBYS_TASINMAZTAAHHUT);
            }

            return isDeleted;
        }

        private static List<TasinmazTaahhut> ToList(DataTable table)
        {
            return new TasinmazTaahhut().ToList<TasinmazTaahhut>(table);
        }

        private static TasinmazTaahhut Map(DataTable table)
        {
            return ToList(table).FirstOrDefault();
        }
    }
}
