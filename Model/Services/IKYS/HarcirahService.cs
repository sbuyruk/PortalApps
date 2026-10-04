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
    public class HarcirahService
    {
        private readonly HarcirahRepository repository;
        public HarcirahService() : this(new HarcirahRepository()) { }
        public HarcirahService(HarcirahRepository repository) { if (repository == null) throw new ArgumentNullException("repository"); this.repository = repository; }
        public Harcirah GetById(int id) { return Map(repository.SelectById(id)); }
        public List<Harcirah> GetAll() { return ToList(repository.SelectAll()); }
        public int Save(Harcirah item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.IKYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_HARCIRAH); return item.Id; }
        public bool Update(Harcirah item) { if (item == null) throw new ArgumentNullException("item"); Harcirah old = GetById(item.Id); bool ok = false; if (item.Id != 0) { item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); ok = repository.Update(item); } if (ok && ProjeConstants.IKYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.IKYS, ProjeConstants.IKYS_HARCIRAH); return ok; }
        public bool Delete(Harcirah item) { if (item == null || item.Id == 0) return false; Harcirah old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.IKYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.IKYS, ProjeConstants.IKYS_HARCIRAH); return ok; }
        public List<Harcirah> GetByKadroUlkeTarih(int kadroGrupId, string ulke, DateTime tarih) { return ToList(repository.SelectByKadroUlkeTarih(kadroGrupId, ulke, tarih)); }
        public Harcirah GetByKadroGrupId(int kadroGrupId) { return Map(repository.SelectByKadroGrupId(kadroGrupId)); }
        public Harcirah GetByParaBirimi(string paraBirimi) { return Map(repository.SelectByParaBirimi(paraBirimi)); }
        public DataTable GetAllReturnDataTable() { return repository.SelectAllReturnDataTable(); }
        private static Harcirah Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static List<Harcirah> ToList(DataTable table) { return new Harcirah().ToList<Harcirah>(table); }
    }
}
