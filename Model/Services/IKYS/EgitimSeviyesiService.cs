using DAO.Repositories.IKYS;
using Model.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.ProjeGlobal;

namespace Model.Services.IKYS
{
    public class EgitimSeviyesiService
    {
        private readonly EgitimSeviyesiRepository repository;
        public EgitimSeviyesiService() : this(new EgitimSeviyesiRepository()) { }
        public EgitimSeviyesiService(EgitimSeviyesiRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }
        public EgitimSeviyesi GetById(int id) { return Map(repository.SelectById(id)); }
        public List<EgitimSeviyesi> GetAll() { return ToList(repository.SelectAll()); }
        public int Save(EgitimSeviyesi item)
        {
            if (item == null) throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);
            if (item.Id > 0 && ProjeConstants.IKYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_EGITIMSEVIYESI);
            return item.Id;
        }
        public bool Update(EgitimSeviyesi item)
        {
            if (item == null) throw new ArgumentNullException("item");
            EgitimSeviyesi old = GetById(item.Id);
            bool ok = false;
            if (item.Id != 0)
            {
                item.DegistirmeTarihi = DateTime.Now;
                item.Degistiren = UtilityHelper.GetCurrentUserName();
                ok = repository.Update(item);
            }
            if (ok && ProjeConstants.IKYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.IKYS, ProjeConstants.IKYS_EGITIMSEVIYESI);
            return ok;
        }
        public bool Delete(EgitimSeviyesi item)
        {
            if (item == null || item.Id == 0) return false;
            EgitimSeviyesi old = GetById(item.Id);
            if (old == null) return false;
            bool ok = repository.Delete(item.Id);
            if (ok && ProjeConstants.IKYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.IKYS, ProjeConstants.IKYS_EGITIMSEVIYESI);
            return ok;
        }
        public List<EgitimSeviyesi> GetByPersonelId(int personelId) { return ToList(repository.SelectByPersonelId(personelId)); }
        private static EgitimSeviyesi Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static List<EgitimSeviyesi> ToList(DataTable table) { return new EgitimSeviyesi().ToList<EgitimSeviyesi>(table); }
    }
}
