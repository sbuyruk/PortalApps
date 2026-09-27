using DAO.Repositories.NBYS;
using Model.NBYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.ProjeGlobal;

namespace Model.Services.NBYS
{
    public class DuzenliNakitBagisciService
    {
        private readonly DuzenliNakitBagisciRepository repository;

        public DuzenliNakitBagisciService() : this(new DuzenliNakitBagisciRepository())
        {
        }

        public DuzenliNakitBagisciService(DuzenliNakitBagisciRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public DuzenliNakitBagisci GetById(int id)
        {
            return Map(repository.SelectById(id)).FirstOrDefault();
        }

        public List<DuzenliNakitBagisci> GetAll()
        {
            return Map(repository.SelectAll());
        }

        public DuzenliNakitBagisci GetActiveByBagisciId(int bagisciId)
        {
            if (bagisciId < 1)
                return null;
            return Map(repository.SelectActiveByBagisciId(bagisciId)).FirstOrDefault();
        }

        public int Save(DuzenliNakitBagisci bagisci)
        {
            if (bagisci == null)
                throw new ArgumentNullException("bagisci");
            bagisci.OlusturmaTarihi = DateTime.Now;
            bagisci.Olusturan = UtilityHelper.GetCurrentUserName();
            bagisci.Id = repository.Insert(bagisci);
            if (bagisci.Id > 0 && ProjeConstants.NBYS_SAVE_LOG)
            {
                new OlayKayit().GirisOlayKaydet(
                    bagisci, ProjeConstants.NBYS, ProjeConstants.NBYS_DUZENLINAKITBAGISCI);
            }
            return bagisci.Id;
        }

        public bool Update(DuzenliNakitBagisci bagisci)
        {
            if (bagisci == null)
                throw new ArgumentNullException("bagisci");
            DuzenliNakitBagisci previous = GetById(bagisci.Id);
            bool updated = false;
            if (bagisci.Id != 0)
            {
                bagisci.DegistirmeTarihi = DateTime.Now;
                bagisci.Degistiren = UtilityHelper.GetCurrentUserName();
                updated = repository.Update(bagisci);
            }
            if (updated && ProjeConstants.NBYS_UPDATE_LOG)
            {
                new OlayKayit().GuncellemeOlayKaydet(
                    bagisci, previous, ProjeConstants.NBYS, ProjeConstants.NBYS_DUZENLINAKITBAGISCI);
            }
            return updated;
        }

        public bool Delete(DuzenliNakitBagisci bagisci)
        {
            if (bagisci == null)
                throw new ArgumentNullException("bagisci");
            if (bagisci.Id == 0)
                return false;
            DuzenliNakitBagisci previous = GetById(bagisci.Id);
            if (previous == null)
                return false;
            bool deleted = repository.Delete(bagisci.Id);
            if (deleted && ProjeConstants.NBYS_DELETE_LOG)
            {
                new OlayKayit().SilmeOlayKaydet(
                    previous, ProjeConstants.NBYS, ProjeConstants.NBYS_DUZENLINAKITBAGISCI);
            }
            return deleted;
        }

        private static List<DuzenliNakitBagisci> Map(DataTable table)
        {
            return new DuzenliNakitBagisci().ToList<DuzenliNakitBagisci>(table);
        }
    }
}
