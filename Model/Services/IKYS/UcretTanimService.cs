using DAO.Repositories.IKYS;
using Model.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.SqlTypes;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.Services.IKYS
{
    public class UcretTanimService
    {
        private readonly UcretTanimRepository repository;
        public UcretTanimService() : this(new UcretTanimRepository()) { }
        public UcretTanimService(UcretTanimRepository repository) { if (repository == null) throw new ArgumentNullException("repository"); this.repository = repository; }
        public UcretTanim GetById(int id) { return Map(repository.SelectById(id)); }
        public List<UcretTanim> GetAll() { return ToList(repository.SelectAll()); }
        public int Save(UcretTanim item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.IKYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_GOREVONAY); return item.Id; }
        public bool Update(UcretTanim item) { if (item == null) throw new ArgumentNullException("item"); UcretTanim old = GetById(item.Id); bool ok = false; if (item.Id != 0) { item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); ok = repository.Update(item); } if (ok && ProjeConstants.IKYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.IKYS, ProjeConstants.IKYS_GOREVONAY); return ok; }
        public bool Delete(UcretTanim item) { if (item == null || item.Id == 0) return false; UcretTanim old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.IKYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.IKYS, ProjeConstants.IKYS_GOREVONAY); return ok; }
        public List<UcretTanim> GetByKademe(int grupId, int kademe) { return ToList(repository.SelectByKademe(grupId, kademe)); }
        public List<UcretTanim> GetByMaxGrupId() { return ToList(repository.SelectByMaxGrupId()); }
        public DataTable GetByGrup() { return repository.SelectByGrup(); }
        public DataTable GetDerece() { return repository.SelectDerece(); }
        public DataTable GetKademe(int derece, int grupId) { return repository.SelectKademe(derece, grupId); }
        public DataTable GetMaasListesi(int grupId, DateTime tarih) { return repository.SelectMaasListesi(grupId, tarih); }
        public DateTime GetMaxBitisTarihi() { return repository.SelectMaxBitisTarihi().Rows[0][0].ConvertToDatetime(); }
        public decimal GetAgi(DateTime tarih) { return repository.SelectAgi(tarih).Rows[0][0].ConvertToDecimal(); }
        public int GetGrupIdByTarih(DateTime tarih) { return repository.SelectGrupIdByTarih(tarih).Rows[0][0].ConvertToInt(); }
        public bool DeleteByGrupId(int grupId, UcretTanim auditItem) { bool ok = repository.DeleteByGrupId(grupId); if (ok && ProjeConstants.IKYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(auditItem, ProjeConstants.IKYS, ProjeConstants.IKYS_MAASARTISI); return ok; }
        public decimal GetUcretByGrupDereceKademe(Personel personel, int grupId, int derece, int kademe) { DataTable table = repository.SelectUcretByGrupDereceKademe(grupId, derece, kademe, personel.Asker_sivil == ProjeConstants.PER_ASKER_INT); return table.Rows.Count > 0 ? table.Rows[0][0].ConvertToDecimal() : SqlDecimal.Null.Value; }
        private static UcretTanim Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static List<UcretTanim> ToList(DataTable table) { return new UcretTanim().ToList<UcretTanim>(table); }
    }
}
