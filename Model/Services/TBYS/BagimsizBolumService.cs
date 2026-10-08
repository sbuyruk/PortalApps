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
    public class BagimsizBolumService
    {
        private readonly BagimsizBolumRepository repository;
        public BagimsizBolumService() : this(new BagimsizBolumRepository()) { }
        public BagimsizBolumService(BagimsizBolumRepository repository) { if (repository == null) throw new ArgumentNullException("repository"); this.repository = repository; }
        public BagimsizBolum GetById(int id) { return Map(repository.SelectById(id)); }
        public List<BagimsizBolum> GetAll() { return ToList(repository.SelectAll()); }
        public List<BagimsizBolum> GetByTasinmazId(int id) { return ToList(repository.SelectByTasinmazId(id)); }
        public List<BagimsizBolum> GetByBolumNo(string bolumNo) { return ToList(repository.SelectByBolumNo(bolumNo)); }
        public BagimsizBolum GetByBolumId(int id) { return Map(repository.SelectByBolumId(id)); }
        public int Save(BagimsizBolum item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGIMSIZBOLUM); return item.Id; }
        public bool Update(BagimsizBolum item) { if (item == null || item.Id == 0) return false; BagimsizBolum old = GetById(item.Id); item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); bool ok = repository.Update(item); if (ok && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGIMSIZBOLUM); return ok; }
        public bool Delete(BagimsizBolum item) { if (item == null || item.Id == 0) return false; BagimsizBolum old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGIMSIZBOLUM); return ok; }
        private static List<BagimsizBolum> ToList(DataTable t) { return new BagimsizBolum().ToList<BagimsizBolum>(t); }
        private static BagimsizBolum Map(DataTable t) { return ToList(t).FirstOrDefault(); }
    }
}
