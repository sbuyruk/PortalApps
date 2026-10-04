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
    public class KimlikService
    {
        private readonly KimlikRepository repository;
        public KimlikService() : this(new KimlikRepository()) { }
        public KimlikService(KimlikRepository repository) { if (repository == null) throw new ArgumentNullException("repository"); this.repository = repository; }
        public Kimlik GetById(int id) { return Map(repository.SelectById(id)); }
        public List<Kimlik> GetAll() { return ToList(repository.SelectAll()); }
        public int Save(Kimlik item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.IKYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_KIMLIK); return item.Id; }
        public bool Update(Kimlik item) { if (item == null) throw new ArgumentNullException("item"); Kimlik old = GetById(item.Id); bool ok = false; if (item.Id != 0) { item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); ok = repository.Update(item); } if (ok && ProjeConstants.IKYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.IKYS, ProjeConstants.IKYS_KIMLIK); return ok; }
        public bool Delete(Kimlik item) { if (item == null || item.Id == 0) return false; Kimlik old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.IKYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.IKYS, ProjeConstants.IKYS_KIMLIK); return ok; }
        public Kimlik GetByPersonelId(int personelId) { return Map(repository.SelectByPersonelId(personelId)); }
        public Kimlik GetByTcKimlikNo(string kimlikNo) { return Map(repository.SelectByTcKimlikNo(kimlikNo)); }
        public DataTable GetAllFromKimlik() { return repository.SelectAllFromKimlik(); }
        private static Kimlik Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static List<Kimlik> ToList(DataTable table) { return new Kimlik().ToList<Kimlik>(table); }
    }
}
