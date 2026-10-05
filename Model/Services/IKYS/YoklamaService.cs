using DAO.Repositories.IKYS;
using Model.IKYS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Model.Ortak;
using Utility.ProjeGlobal;

namespace Model.Services.IKYS
{
    public class YoklamaService
    {
        private readonly YoklamaRepository repository;

        public YoklamaService() : this(new YoklamaRepository()) { }

        public YoklamaService(YoklamaRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public Yoklama GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public int Save(Yoklama item)
        {
            if (item == null) throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);
            if (item.Id > 0 && ProjeConstants.IKYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_YOKLAMA);
            return item.Id;
        }

        public bool Update(Yoklama item)
        {
            if (item == null) throw new ArgumentNullException("item");
            Yoklama old = GetById(item.Id);
            bool ok = false;
            if (item.Id != 0)
            {
                item.DegistirmeTarihi = DateTime.Now;
                item.Degistiren = UtilityHelper.GetCurrentUserName();
                ok = repository.Update(item);
            }
            if (ok && ProjeConstants.IKYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.IKYS, ProjeConstants.IKYS_YOKLAMA);
            return ok;
        }

        public bool Delete(Yoklama item)
        {
            if (item == null || item.Id == 0) return false;
            Yoklama old = GetById(item.Id);
            if (old == null) return false;
            bool ok = repository.Delete(item.Id);
            if (ok && ProjeConstants.IKYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.IKYS, ProjeConstants.IKYS_YOKLAMA);
            return ok;
        }

        public List<Yoklama> GetAll()
        {
            return new Yoklama().ToList<Yoklama>(repository.SelectAll());
        }

        public List<Yoklama> GetByPersonelIdAndDate(int personelId, DateTime start, DateTime end)
        {
            return new Yoklama().ToList<Yoklama>(
                repository.SelectByPersonelIdAndDate(personelId, start, end));
        }

        public DataTable GetAllByPersonelId(int personelId)
        {
            return repository.SelectAllByPersonelId(personelId);
        }

        public DataTable GetByTarih(DateTime start, DateTime end) { return repository.SelectByTarih(start, end); }
        public DataTable GetByTarihAndReasons(string reasonIds, DateTime start, DateTime end) { return repository.SelectByTarihAndReasons(reasonIds, start, end); }

        public string GetAllByPersonelIdAsJson(int personelId)
        {
            return new Yoklama().ToJSON(GetAllByPersonelId(personelId));
        }

        private static Yoklama Map(DataTable table)
        {
            return new Yoklama().ToList<Yoklama>(table).FirstOrDefault();
        }
    }
}
