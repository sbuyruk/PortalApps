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
    public class AileService
    {
        private readonly AileRepository repository;
        public AileService() : this(new AileRepository()) { }
        public AileService(AileRepository repository) { if (repository == null) throw new ArgumentNullException("repository"); this.repository = repository; }
        public Aile GetById(int id) { return Map(repository.SelectById(id)); }
        public List<Aile> GetAll() { return ToList(repository.SelectAll()); }
        public int Save(Aile item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.IKYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_AILE); return item.Id; }
        public bool Update(Aile item) { if (item == null) throw new ArgumentNullException("item"); Aile old = GetById(item.Id); bool ok = false; if (item.Id != 0) { item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); ok = repository.Update(item); } if (ok && ProjeConstants.IKYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.IKYS, ProjeConstants.IKYS_AILE); return ok; }
        public bool Delete(Aile item) { if (item == null || item.Id == 0) return false; Aile old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.IKYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.IKYS, ProjeConstants.IKYS_AILE); return ok; }
        public List<Aile> GetByPersonelId(int personelId) { return ToList(repository.SelectByPersonelId(personelId)); }
        public Aile GetEnGencCocukByPersonelId(int personelId) { return Map(repository.SelectEnGencCocukByPersonelId(personelId, ProjeConstants.PER_YAKINLIKDERECESI_COCUK_INT)); }
        private static Aile Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static List<Aile> ToList(DataTable table) { return new Aile().ToList<Aile>(table); }
    }
}
