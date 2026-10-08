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
    public class KiraBorcuTakipService
    {
        private readonly KiraBorcuTakipRepository repository;
        public KiraBorcuTakipService() : this(new KiraBorcuTakipRepository()) { }
        public KiraBorcuTakipService(KiraBorcuTakipRepository repository) { if (repository == null) throw new ArgumentNullException("repository"); this.repository = repository; }
        public KiraBorcuTakip GetById(int id) { return Map(repository.SelectById(id)); }
        public KiraBorcuTakip GetByKiraciIdAyYil(int id) { DateTime now = DateTime.Today; return Map(repository.SelectByKiraciIdAyYil(id, now.Month, now.Year)); }
        public List<KiraBorcuTakip> GetAll() { return ToList(repository.SelectAll()); }
        public int GetCountByFilters(string takipIslemi, string bolge, int ay, int yil) { return GetCount(repository.SelectCountByFilters(takipIslemi, bolge, ay, yil)); }
        public int GetCountBySozlesmeId(int id, string takipIslemi) { return GetCount(repository.SelectCountBySozlesmeId(id, takipIslemi)); }
        public int Save(KiraBorcuTakip item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_KIRABORCUTAKIP); return item.Id; }
        public bool Update(KiraBorcuTakip item) { if (item == null || item.Id == 0) return false; KiraBorcuTakip old = GetById(item.Id); item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); bool ok = repository.Update(item); if (ok && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.TBYS, ProjeConstants.TBYS_KIRABORCUTAKIP); return ok; }
        public bool Delete(KiraBorcuTakip item) { if (item == null || item.Id == 0) return false; KiraBorcuTakip old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.TBYS, ProjeConstants.TBYS_KIRABORCUTAKIP); return ok; }
        private static List<KiraBorcuTakip> ToList(DataTable t) { return new KiraBorcuTakip().ToList<KiraBorcuTakip>(t); }
        private static KiraBorcuTakip Map(DataTable t) { return ToList(t).FirstOrDefault(); }
        private static int GetCount(DataTable t) { return t != null && t.Rows.Count > 0 ? t.Rows[0]["Adet"].ToString().ConvertToInt() : 0; }
    }
}
