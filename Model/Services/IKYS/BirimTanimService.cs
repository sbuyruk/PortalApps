using DAO.Repositories.IKYS;
using Model.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.Services.IKYS
{
    public class BirimTanimService
    {
        private readonly BirimTanimRepository repository;
        public BirimTanimService() : this(new BirimTanimRepository()) { }
        public BirimTanimService(BirimTanimRepository repository) { if (repository == null) throw new ArgumentNullException("repository"); this.repository = repository; }
        public BirimTanim GetById(int id) { return Map(repository.SelectById(id)); }
        public List<BirimTanim> GetAll() { return ToList(repository.SelectAll()); }
        public int Save(BirimTanim item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.IKYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_BIRIMTANIM); return item.Id; }
        public bool Update(BirimTanim item) { if (item == null) throw new ArgumentNullException("item"); BirimTanim old = GetById(item.Id); bool ok = false; if (item.Id != 0) { item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); ok = repository.Update(item); } if (ok && ProjeConstants.IKYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.IKYS, ProjeConstants.IKYS_BIRIMTANIM); return ok; }
        public bool Delete(BirimTanim item) { if (item == null || item.Id == 0) return false; BirimTanim old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.IKYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.IKYS, ProjeConstants.IKYS_BIRIMTANIM); return ok; }
        public List<BirimTanim> GetByAmirId(int amirId) { return ToList(repository.SelectByAmirId(amirId)); }
        public List<BirimTanim> GetByBirimKaldirildi(bool value) { return ToList(repository.SelectByBirimKaldirildi(value)); }
        public DataTable GetAllReturnDataTable() { return repository.SelectAllReturnDataTable(); }
        public List<BirimTanim> GetByParentId(int parentId) { return ToList(repository.SelectByParentId(parentId)); }
        public BirimTanim GetRoot() { return Map(repository.SelectRoot()); }
        private static BirimTanim Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static List<BirimTanim> ToList(DataTable table) { return new BirimTanim().ToList<BirimTanim>(table); }
    }
}
