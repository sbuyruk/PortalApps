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
    public class OdemeService
    {
        private readonly OdemeRepository repository;
        public OdemeService() : this(new OdemeRepository()) { }
        public OdemeService(OdemeRepository repository) { if (repository == null) throw new ArgumentNullException("repository"); this.repository = repository; }
        public Odeme GetById(int id) { return Map(repository.SelectById(id)); }
        public List<Odeme> GetAll() { return ToList(repository.SelectAll()); }
        public List<Odeme> GetByKiraciId(int id) { return ToList(repository.SelectByKiraciId(id)); }
        public List<Odeme> GetBySozlesmeIdOdemePlaniId(int sozlesmeId, int planId) { return ToList(repository.SelectBySozlesmeIdOdemePlaniId(sozlesmeId, planId)); }
        public int Save(Odeme item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEME); return item.Id; }
        public bool Update(Odeme item) { if (item == null) throw new ArgumentNullException("item"); Odeme old = GetById(item.Id); bool ok = false; if (item.Id != 0) { item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); ok = repository.Update(item); } if (ok && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEME); return ok; }
        public bool Delete(Odeme item) { if (item == null || item.Id == 0) return false; Odeme old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEME); return ok; }
        public bool DeleteBySozlesmeId(int id) { return repository.DeleteBySozlesmeId(id); }
        private static List<Odeme> ToList(DataTable t) { return new Odeme().ToList<Odeme>(t); }
        private static Odeme Map(DataTable t) { return ToList(t).FirstOrDefault(); }
    }
}
