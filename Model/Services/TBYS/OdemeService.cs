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
    public class OdemeService
    {
        private readonly OdemeRepository repository;
        public OdemeService() : this(new OdemeRepository()) { }
        public OdemeService(OdemeRepository repository) { if (repository == null) throw new ArgumentNullException("repository"); this.repository = repository; }
        public Odeme GetById(int id) { return Map(repository.SelectById(id)); }
        public List<Odeme> GetAll() { return ToList(repository.SelectAll()); }
        public List<Odeme> GetByKiraciId(int id) { return ToList(repository.SelectByKiraciId(id)); }
        public List<Odeme> GetBySozlesmeIdOdemePlaniId(int sozlesmeId, int planId) { return ToList(repository.SelectBySozlesmeIdOdemePlaniId(sozlesmeId, planId)); }
        public DataTable GetByKiraciAyYil(int kiraciId, int ay, int yil) { return repository.SelectByKiraciAyYil(kiraciId, ay, yil); }
        public DataTable GetByKiraciAyYilReturnDataTable(int bolgeId, int kiraciId, DateTime basTarih, DateTime bitTarih) { return repository.SelectByKiraciAyYilReturnDataTable(bolgeId, kiraciId, basTarih, UtilityHelper.TariheSaatEkle(bitTarih, "23:59:59")); }
        public DataTable GetByAyYilReturnDataTable(int ay, int yil) { return repository.SelectByAyYilReturnDataTable(ay, yil); }
        public List<Odeme> GetByKiraciVadeBasTarVadeBitTar(int sozlesmeId, int kiraciId, DateTime ilkTarih, DateTime ikinciTarih)
        {
            DateTime basTarih = UtilityHelper.TariheSaatEkle(new DateTime(ilkTarih.Year, ilkTarih.Month, ilkTarih.Day), "00:00");
            DateTime bitTarih = UtilityHelper.TariheSaatEkle(new DateTime(ikinciTarih.Year, ikinciTarih.Month, ikinciTarih.Day), "23:59");
            return ToList(repository.SelectByKiraciVadeBasTarVadeBitTar(sozlesmeId, kiraciId, basTarih, bitTarih));
        }
        public decimal GetSumBySozlesmeIdOdemePlaniId(int sozlesmeId, int odemePlaniId)
        {
            DataTable dataTable = repository.SelectSumBySozlesmeIdOdemePlaniId(sozlesmeId, odemePlaniId);
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                return dataTable.Rows[0]["Toplam"].ToString().ConvertToDecimal();
            }
            return 0;
        }
        public int Save(Odeme item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEME); return item.Id; }
        public bool Update(Odeme item) { if (item == null) throw new ArgumentNullException("item"); Odeme old = GetById(item.Id); bool ok = false; if (item.Id != 0) { item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); ok = repository.Update(item); } if (ok && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEME); return ok; }
        public bool Delete(Odeme item) { if (item == null || item.Id == 0) return false; Odeme old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEME); return ok; }
        public bool DeleteBySozlesmeId(int id) { return repository.DeleteBySozlesmeId(id); }
        private static List<Odeme> ToList(DataTable t) { return new Odeme().ToList<Odeme>(t); }
        private static Odeme Map(DataTable t) { return ToList(t).FirstOrDefault(); }
    }
}
