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
    public class IzinTanimService
    {
        private readonly IzinTanimRepository repository;
        public IzinTanimService() : this(new IzinTanimRepository()) { }
        public IzinTanimService(IzinTanimRepository repository) { if (repository == null) throw new ArgumentNullException("repository"); this.repository = repository; }
        public IzinTanim GetById(int id) { return Map(repository.SelectById(id)); }
        public List<IzinTanim> GetAll() { return ToList(repository.SelectAll()); }
        public int Save(IzinTanim item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.IKYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_IZINTANIM); return item.Id; }
        public bool Update(IzinTanim item) { if (item == null) throw new ArgumentNullException("item"); IzinTanim old = GetById(item.Id); bool ok = false; if (item.Id != 0) { item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); ok = repository.Update(item); } if (ok && ProjeConstants.IKYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.IKYS, ProjeConstants.IKYS_IZINTANIM); return ok; }
        public bool Delete(IzinTanim item) { if (item == null || item.Id == 0) return false; IzinTanim old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.IKYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.IKYS, ProjeConstants.IKYS_IZINTANIM); return ok; }
        private static IzinTanim Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static List<IzinTanim> ToList(DataTable table) { return new IzinTanim().ToList<IzinTanim>(table); }
    }
}
