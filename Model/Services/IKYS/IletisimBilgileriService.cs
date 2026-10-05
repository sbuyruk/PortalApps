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
    public class IletisimBilgileriService
    {
        private readonly IletisimBilgileriRepository repository;
        public IletisimBilgileriService() : this(new IletisimBilgileriRepository()) { }
        public IletisimBilgileriService(IletisimBilgileriRepository repository) { if (repository == null) throw new ArgumentNullException("repository"); this.repository = repository; }
        public IletisimBilgileri GetById(int id) { return Map(repository.SelectById(id)); }
        public List<IletisimBilgileri> GetAll() { return ToList(repository.SelectAll()); }
        public int Save(IletisimBilgileri item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.IKYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_ILETISIMBILGILERI); return item.Id; }
        public bool Update(IletisimBilgileri item) { if (item == null) throw new ArgumentNullException("item"); IletisimBilgileri old = GetById(item.Id); bool ok = false; if (item.Id != 0) { item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); ok = repository.Update(item); } if (ok && ProjeConstants.IKYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.IKYS, ProjeConstants.IKYS_ILETISIMBILGILERI); return ok; }
        public bool Delete(IletisimBilgileri item) { if (item == null || item.Id == 0) return false; IletisimBilgileri old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.IKYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.IKYS, ProjeConstants.IKYS_ILETISIMBILGILERI); return ok; }
        public IletisimBilgileri GetByPersonelId(int personelId) { return Map(repository.SelectByPersonelId(personelId)); }
        private static IletisimBilgileri Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static List<IletisimBilgileri> ToList(DataTable table) { return new IletisimBilgileri().ToList<IletisimBilgileri>(table); }
    }
}
