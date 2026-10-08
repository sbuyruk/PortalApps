using DAO.Repositories.TBYS;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.Services.TBYS
{
    public class KiraciService
    {
        private readonly KiraciRepository repository;

        public KiraciService() : this(new KiraciRepository()) { }

        public KiraciService(KiraciRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public Kiraci GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public List<Kiraci> GetAll()
        {
            return new Kiraci().ToList<Kiraci>(repository.SelectAll());
        }

        public List<Kiraci> GetActiveTenants()
        {
            return new Kiraci().ToList<Kiraci>(repository.SelectActiveTenants());
        }
        public int Save(Kiraci item)
        {
            if (item == null) throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);
            if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_KIRACI);
            return item.Id;
        }
        public bool Update(Kiraci item)
        {
            if (item == null || item.Id == 0) return false;
            Kiraci old = GetById(item.Id);
            item.DegistirmeTarihi = DateTime.Now;
            item.Degistiren = UtilityHelper.GetCurrentUserName();
            bool ok = repository.Update(item);
            if (ok && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.TBYS, ProjeConstants.TBYS_KIRACI);
            return ok;
        }
        public bool Delete(Kiraci item)
        {
            if (item == null || item.Id == 0) return false;
            Kiraci old = GetById(item.Id);
            if (old == null) return false;
            bool ok = repository.Delete(item.Id);
            if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.TBYS, ProjeConstants.TBYS_KIRACI);
            return ok;
        }
        public string GetAllReturnJson() { return new Kiraci().ToJSON(repository.SelectAllReturnJson()); }
        public DataTable GetAllReturnDataTable(string secim, int bolgeId) { return repository.SelectAllReturnDataTable(secim, bolgeId); }
        public DataTable GetByBolge(string aktif, string bolge) { return repository.SelectByBolge(aktif, bolge); }
        public DataTable GetWithoutActiveContract(int bolgeId) { return repository.SelectWithoutActiveContract(bolgeId); }
        public DataTable GetByFilter(string filter) { return repository.SelectByFilter(filter); }
        public string GetByFilterJson(string filter) { return new Kiraci().ToJSON(repository.SelectByFilterSimple(filter)); }
        public Kiraci GetNext(int id) { DataTable table = repository.SelectNext(id); return table != null ? Map(table) : GetMin(); }
        public Kiraci GetPrevious(int id) { DataTable table = repository.SelectPrevious(id); return table != null ? Map(table) : GetMax(); }
        public Kiraci GetMax() { DataTable table = repository.SelectMax(); return table == null ? null : Map(table); }
        public Kiraci GetMin() { DataTable table = repository.SelectMin(); return table == null ? null : Map(table); }
        public List<Kiraci> GetByName(string adi, string soyadi) { return ToList(repository.SelectByName(adi, soyadi)); }

        private static Kiraci Map(DataTable table)
        {
            return new Kiraci().ToList<Kiraci>(table).FirstOrDefault();
        }
        private static List<Kiraci> ToList(DataTable table) { return new Kiraci().ToList<Kiraci>(table); }
    }
}
