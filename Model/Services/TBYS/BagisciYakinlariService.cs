using DAO.Repositories.TBYS;
using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.ProjeGlobal;

namespace Model.Services.TBYS
{
    public class BagisciYakinlariService
    {
        private readonly BagisciYakinlariRepository repository;
        public BagisciYakinlariService() : this(new BagisciYakinlariRepository()) { }
        public BagisciYakinlariService(BagisciYakinlariRepository repository) { if (repository == null) throw new ArgumentNullException("repository"); this.repository = repository; }
        public BagisciYakinlari GetById(int id) { return Map(repository.SelectById(id)); }
        public List<BagisciYakinlari> GetAll() { return ToList(repository.SelectAll()); }
        public int Save(BagisciYakinlari item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGISCIYAKINLARI); return item.Id; }
        public bool Update(BagisciYakinlari item) { if (item == null) throw new ArgumentNullException("item"); BagisciYakinlari old = GetById(item.Id); bool ok = false; if (item.Id != 0) { item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); ok = repository.Update(item); } if (ok && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGISCIYAKINLARI); return ok; }
        public bool Delete(BagisciYakinlari item) { if (item == null || item.Id == 0) return false; BagisciYakinlari old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGISCIYAKINLARI); return ok; }
        public List<BagisciYakinlari> GetByBagisciId(int bagisciId) { return ToList(repository.SelectByBagisciId(bagisciId)); }
        private static BagisciYakinlari Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static List<BagisciYakinlari> ToList(DataTable table) { return new BagisciYakinlari().ToList<BagisciYakinlari>(table); }
    }
}
