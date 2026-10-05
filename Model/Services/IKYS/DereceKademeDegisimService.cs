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
    public class DereceKademeDegisimService
    {
        private readonly DereceKademeDegisimRepository repository;
        public DereceKademeDegisimService() : this(new DereceKademeDegisimRepository()) { }
        public DereceKademeDegisimService(DereceKademeDegisimRepository repository) { if (repository == null) throw new ArgumentNullException("repository"); this.repository = repository; }
        public DereceKademeDegisim GetById(int id) { return Map(repository.SelectById(id)); }
        public List<DereceKademeDegisim> GetAll() { return ToList(repository.SelectAll()); }
        public int Save(DereceKademeDegisim item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.IKYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_GOREVONAY); return item.Id; }
        public bool Update(DereceKademeDegisim item) { if (item == null) throw new ArgumentNullException("item"); DereceKademeDegisim old = GetById(item.Id); bool ok = false; if (item.Id != 0) { item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); ok = repository.Update(item); } if (ok && ProjeConstants.IKYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.IKYS, ProjeConstants.IKYS_GOREVONAY); return ok; }
        public bool Delete(DereceKademeDegisim item) { if (item == null || item.Id == 0) return false; DereceKademeDegisim old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.IKYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.IKYS, ProjeConstants.IKYS_GOREVONAY); return ok; }
        public DataTable GetAllByPersonelIdReturnDataTable(int personelId) { return repository.SelectByPersonelId(personelId); }
        public DereceKademeDegisim GetByPersonelId(int personelId) { return Map(repository.SelectByPersonelId(personelId)); }
        private static DereceKademeDegisim Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static List<DereceKademeDegisim> ToList(DataTable table) { return new DereceKademeDegisim().ToList<DereceKademeDegisim>(table); }
    }
}
