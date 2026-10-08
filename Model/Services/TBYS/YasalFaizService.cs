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
    public class YasalFaizService
    {
        private readonly YasalFaizRepository repository;

        public YasalFaizService() : this(new YasalFaizRepository()) { }
        public YasalFaizService(YasalFaizRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public YasalFaiz GetById(int id) { return Map(repository.SelectById(id)); }
        public List<YasalFaiz> GetAll() { return ToList(repository.SelectAll()); }
        public List<YasalFaiz> GetByYear(int year) { return ToList(repository.SelectByYear(year)); }
        public YasalFaiz GetByYearMonth(int year, int month) { return Map(repository.SelectByYearMonth(year, month)); }
        public decimal GetLatestRate() { return GetLatestValue(repository.SelectLatestRate("FaizOrani"), x => x.FaizOrani); }
        public decimal GetLatestTufe() { return GetLatestValue(repository.SelectLatestRate("Tufe"), x => x.Tufe); }
        public decimal GetLatestUfe() { return GetLatestValue(repository.SelectLatestRate("Ufe"), x => x.Ufe); }
        public int Save(YasalFaiz item)
        {
            if (item == null) throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);
            if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_YASALFAIZ);
            return item.Id;
        }
        public bool Update(YasalFaiz item)
        {
            if (item == null || item.Id == 0) return false;
            YasalFaiz old = GetById(item.Id);
            item.DegistirmeTarihi = DateTime.Now;
            item.Degistiren = UtilityHelper.GetCurrentUserName();
            bool ok = repository.Update(item);
            if (ok && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.TBYS, ProjeConstants.TBYS_YASALFAIZ);
            return ok;
        }
        public bool Delete(YasalFaiz item)
        {
            if (item == null || item.Id == 0) return false;
            YasalFaiz old = GetById(item.Id);
            if (old == null) return false;
            bool ok = repository.Delete(item.Id);
            if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.TBYS, ProjeConstants.TBYS_YASALFAIZ);
            return ok;
        }
        private static List<YasalFaiz> ToList(DataTable table) { return new YasalFaiz().ToList<YasalFaiz>(table); }
        private static YasalFaiz Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static decimal GetLatestValue(DataTable table, Func<YasalFaiz, decimal> valueSelector) { YasalFaiz item = Map(table); return item == null ? 0 : valueSelector(item); }
    }
}
