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
    public class OdemePlaniService
    {
        private readonly OdemePlaniRepository repository;
        public OdemePlaniService() : this(new OdemePlaniRepository()) { }
        public OdemePlaniService(OdemePlaniRepository repository) { if (repository == null) throw new ArgumentNullException("repository"); this.repository = repository; }
        public OdemePlani GetById(int id) { return Map(repository.SelectById(id)); }
        public List<OdemePlani> GetAll() { return ToList(repository.SelectAll()); }
        public List<OdemePlani> GetBySozlesmeId(int id) { return ToList(repository.SelectBySozlesmeId(id)); }
        public OdemePlani GetBySozlesmeIdSira(int id, int sira) { return Map(repository.SelectBySozlesmeIdSira(id, sira)); }
        public OdemePlani GetBySozlesmeIdOdemeTarihi(int id, DateTime tarih) { return Map(repository.SelectBySozlesmeIdOdemeTarihi(id, tarih)); }
        public bool Exists(int id) { return ToList(repository.SelectExists(id)).Count > 0; }
        public OdemePlani GetLastBySozlesmeId(int id) { return ToList(repository.SelectLastBySozlesmeId(id)).LastOrDefault(); }
        public OdemePlani GetFirstBySozlesmeId(int id) { return Map(repository.SelectFirstBySozlesmeId(id)); }
        public int Save(OdemePlani item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEMEPLANI); return item.Id; }
        public bool Update(OdemePlani item) { if (item == null) throw new ArgumentNullException("item"); OdemePlani old = GetById(item.Id); bool ok = false; if (item.Id != 0) { item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); ok = repository.Update(item); } if (ok && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEMEPLANI); return ok; }
        public bool Delete(OdemePlani item) { if (item == null || item.Id == 0) return false; OdemePlani old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEMEPLANI); return ok; }
        public bool DeleteBySozlesmeId(OdemePlani item, int id) { bool ok = repository.DeleteBySozlesmeId(id); if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEMEPLANI); return ok; }
        private static List<OdemePlani> ToList(DataTable t) { return new OdemePlani().ToList<OdemePlani>(t); }
        private static OdemePlani Map(DataTable t) { return ToList(t).FirstOrDefault(); }
    }
}
