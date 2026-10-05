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
    public class IsBilgileriService
    {
        private readonly IsBilgileriRepository repository;
        public IsBilgileriService() : this(new IsBilgileriRepository()) { }
        public IsBilgileriService(IsBilgileriRepository repository) { if (repository == null) throw new ArgumentNullException("repository"); this.repository = repository; }
        public IsBilgileri GetById(int id) { return Map(repository.SelectById(id)); }
        public List<IsBilgileri> GetAll() { return ToList(repository.SelectAll()); }
        public int Save(IsBilgileri item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.IKYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_ISBILGILERI); return item.Id; }
        public bool Update(IsBilgileri item) { if (item == null) throw new ArgumentNullException("item"); IsBilgileri old = GetById(item.Id); bool ok = false; if (item.Id != 0) { item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); ok = repository.Update(item); } if (ok && ProjeConstants.IKYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.IKYS, ProjeConstants.IKYS_ISBILGILERI); return ok; }
        public bool Delete(IsBilgileri item) { if (item == null || item.Id == 0) return false; IsBilgileri old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.IKYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.IKYS, ProjeConstants.IKYS_ISBILGILERI); return ok; }
        public IsBilgileri GetByPersonelId(int personelId) { return Map(repository.SelectByPersonelId(personelId)); }
        public IsBilgileri GetByGorevId(int gorevId) { return Map(repository.SelectByGorevId(gorevId)); }
        public DataTable GetAllFromIsYeriBilgileri() { return repository.SelectAllFromIsYeriBilgileri(); }
        private static IsBilgileri Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static List<IsBilgileri> ToList(DataTable table) { return new IsBilgileri().ToList<IsBilgileri>(table); }
    }
}
