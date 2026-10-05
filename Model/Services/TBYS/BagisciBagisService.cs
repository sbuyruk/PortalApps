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
    public class BagisciBagisService
    {
        private readonly BagisciBagisRepository repository;
        public BagisciBagisService() : this(new BagisciBagisRepository()) { }
        public BagisciBagisService(BagisciBagisRepository repository) { if (repository == null) throw new ArgumentNullException("repository"); this.repository = repository; }
        public BagisciBagis GetById(int id) { return Map(repository.SelectById(id)); }
        public List<BagisciBagis> GetAll() { return ToList(repository.SelectAll()); }
        public int Save(BagisciBagis item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGISCIBAGIS); return item.Id; }
        public bool Update(BagisciBagis item) { if (item == null) throw new ArgumentNullException("item"); BagisciBagis old = GetById(item.Id); bool ok = false; if (item.Id != 0) { item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); ok = repository.Update(item); } if (ok && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGISCIBAGIS); return ok; }
        public bool Delete(BagisciBagis item) { if (item == null || item.Id == 0) return false; BagisciBagis old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGISCIBAGIS); return ok; }
        public List<BagisciBagis> GetByBagisId(int bagisId) { return ToList(repository.SelectByBagisId(bagisId)); }
        private static BagisciBagis Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static List<BagisciBagis> ToList(DataTable table) { return new BagisciBagis().ToList<BagisciBagis>(table); }
    }
}
