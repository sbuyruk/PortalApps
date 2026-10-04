using DAO.Repositories.IKYS;
using Model.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Script.Serialization;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.Services.IKYS
{
    public class IzinHareketService
    {
        private readonly IzinHareketRepository repository;
        public IzinHareketService() : this(new IzinHareketRepository()) { }
        public IzinHareketService(IzinHareketRepository repository) { if (repository == null) throw new ArgumentNullException("repository"); this.repository = repository; }
        public IzinHareket GetById(int id) { return Map(repository.SelectById(id)); }
        public List<IzinHareket> GetAll() { return ToList(repository.SelectAll()); }
        public int Save(IzinHareket item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.IKYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_IZINHAREKET); return item.Id; }
        public bool Update(IzinHareket item) { if (item == null) throw new ArgumentNullException("item"); IzinHareket old = GetById(item.Id); bool ok = false; if (item.Id != 0) { item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); ok = repository.Update(item); } if (ok && ProjeConstants.IKYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.IKYS, ProjeConstants.IKYS_IZINHAREKET); return ok; }
        public bool Delete(IzinHareket item) { if (item == null || item.Id == 0) return false; IzinHareket old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.IKYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.IKYS, ProjeConstants.IKYS_IZINHAREKET); return ok; }
        public List<IzinHareket> GetByTarih(DateTime tarih) { return ToList(repository.SelectByTarih(tarih)); }
        public IzinHareket GetByIzinTalepId(int id) { return Map(repository.SelectByIzinTalepId(id)); }
        public DataTable GetByTarihReturnDataTable(DateTime baslangic, DateTime bitis) { return repository.SelectByTarihReturnDataTable(baslangic, bitis); }
        public IzinHareket GetByPersonelTarih(int personelId, DateTime baslangic, DateTime bitis) { return Map(repository.SelectByPersonelTarih(personelId, baslangic, bitis)); }
        public DataTable GetByIzinTipiTarihReturnDataTable(int izinTipi, DateTime ilkTarih, DateTime bitis) { return repository.SelectByIzinTipiTarih(izinTipi, ilkTarih, bitis); }
        public DataTable GetByIzinDonemiReturnDataTable(int personelId, int izinTipi, DateTime baslangic, DateTime bitis) { return repository.SelectByIzinDonemi(personelId, izinTipi, baslangic, bitis); }
        public DataTable GetByIzinDonemiReturnDataTable(int izinDonemId, int personelId, int izinTipi) { return repository.SelectByPersonelIdDonemId(personelId, izinDonemId, izinTipi); }
        public string GetByPersonelIdDonemIdReturnJson(int personelId, int donemId, int izinTanimId) { return ConvertDataTableToString(repository.SelectByPersonelIdDonemId(personelId, donemId, izinTanimId)); }
        public List<IzinHareket> GetDigerByPersonelId(int personelId) { return ToList(repository.SelectDigerByPersonelId(personelId)); }
        public string ConvertDataTableToString(DataTable table)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer(); List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            if (table != null) foreach (DataRow dataRow in table.Rows) { Dictionary<string, object> row = new Dictionary<string, object>(); foreach (DataColumn column in table.Columns) row.Add(column.ColumnName, dataRow[column]); rows.Add(row); }
            return serializer.Serialize(rows);
        }
        private static IzinHareket Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static List<IzinHareket> ToList(DataTable table) { return new IzinHareket().ToList<IzinHareket>(table); }
    }
}
