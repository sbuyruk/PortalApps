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
    public class TeminatIslemService
    {
        private readonly TeminatIslemRepository repository;
        public TeminatIslemService() : this(new TeminatIslemRepository()) { }
        public TeminatIslemService(TeminatIslemRepository repository) { if (repository == null) throw new ArgumentNullException("repository"); this.repository = repository; }
        public TeminatIslem GetById(int id) { return Map(repository.SelectById(id)); }
        public List<TeminatIslem> GetAll() { return ToList(repository.SelectAll()); }
        public List<TeminatIslem> GetBySozlesmeId(int id) { return ToList(repository.SelectBySozlesmeId(id)); }
        public List<TeminatIslem> GetByKiraciId(int id) { return ToList(repository.SelectByKiraciId(id)); }
        public List<TeminatIslem> GetByOdemeId(int id) { return ToList(repository.SelectByOdemeId(id)); }
        public decimal GetSumPaidByKiraciId(int id) { DataTable t = repository.SelectSumPaidByKiraciId(id); return t != null && t.Rows.Count > 0 ? t.Rows[0]["Toplam"].ReturnZeroIfNull().ConvertToDecimal() : 0; }
        public DataTable GetSumByKiraciIdGroupByType(int id) { return repository.SelectSumByKiraciIdGroupByType(id); }
        public int Save(TeminatIslem item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_TEMINATISLEM); return item.Id; }
        public bool Update(TeminatIslem item) { if (item == null) throw new ArgumentNullException("item"); TeminatIslem old = GetById(item.Id); bool ok = false; if (item.Id != 0) { item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); ok = repository.Update(item); } if (ok && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.TBYS, ProjeConstants.TBYS_TEMINATISLEM); return ok; }
        public bool Delete(TeminatIslem item) { if (item == null || item.Id == 0) return false; TeminatIslem old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.TBYS, ProjeConstants.TBYS_TEMINATISLEM); return ok; }
        public bool DeleteBySozlesmeId(int id) { return repository.DeleteBySozlesmeId(id); }
        private static List<TeminatIslem> ToList(DataTable t) { return new TeminatIslem().ToList<TeminatIslem>(t); }
        private static TeminatIslem Map(DataTable t) { return ToList(t).FirstOrDefault(); }
    }
}
