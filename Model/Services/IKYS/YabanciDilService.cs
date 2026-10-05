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
    public class YabanciDilService
    {
        private readonly YabanciDilRepository repository;
        public YabanciDilService() : this(new YabanciDilRepository()) { }
        public YabanciDilService(YabanciDilRepository repository) { if (repository == null) throw new ArgumentNullException("repository"); this.repository = repository; }
        public YabanciDil GetById(int id) { return Map(repository.SelectById(id)); }
        public List<YabanciDil> GetAll() { return ToList(repository.SelectAll()); }
        public int Save(YabanciDil item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.IKYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_YABANCIDIL); return item.Id; }
        public bool Update(YabanciDil item) { if (item == null) throw new ArgumentNullException("item"); YabanciDil old = GetById(item.Id); bool ok = false; if (item.Id != 0) { item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); ok = repository.Update(item); } if (ok && ProjeConstants.IKYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.IKYS, ProjeConstants.IKYS_YABANCIDIL); return ok; }
        public bool Delete(YabanciDil item) { if (item == null || item.Id == 0) return false; YabanciDil old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.IKYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.IKYS, ProjeConstants.IKYS_YABANCIDIL); return ok; }
        public List<YabanciDil> GetByPersonelId(int personelId) { return ToList(repository.SelectByPersonelId(personelId)); }
        private static YabanciDil Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static List<YabanciDil> ToList(DataTable table) { return new YabanciDil().ToList<YabanciDil>(table); }
    }
}
