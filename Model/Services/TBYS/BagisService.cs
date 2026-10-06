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
    public class BagisService
    {
        private readonly BagisRepository repository;

        public BagisService() : this(new BagisRepository()) { }
        public BagisService(BagisRepository repository) { if (repository == null) throw new ArgumentNullException("repository"); this.repository = repository; }

        public Bagis GetById(int id) { return Map(repository.SelectById(id)); }
        public List<Bagis> GetAll() { return ToList(repository.SelectAll()); }
        public List<Bagis> GetByBagisciId(int bagisciId) { return ToList(repository.SelectByBagisciId(bagisciId)); }
        public Bagis GetByTasinmazId(int tasinmazId) { return Map(repository.SelectByTasinmazId(tasinmazId)); }
        public DataTable GetByBagisciIdGroupByKullanimSekli(int bagisciId) { return repository.SelectByBagisciIdGroupByKullanimSekli(bagisciId); }
        public string GetByBagisciIdAsJson(int bagisciId) { return new Bagis().ToJSON(repository.SelectByBagisciIdAsJson(bagisciId)); }
        public DataTable GetTasinmazByBagisciId(int bagisciId) { return repository.SelectTasinmazByBagisciId(bagisciId, null); }
        public string GetTasinmazByBagisciIdAsJson(int bagisciId) { return new Bagis().ToJSON(GetTasinmazByBagisciId(bagisciId)); }
        public DataTable GetSatisVsDahilTasinmazByBagisciId(int bagisciId)
        {
            return repository.SelectTasinmazByBagisciId(bagisciId, Tasinmaz.SatisVsDahilEnvanterdenCikmaSebepleri);
        }
        public decimal GetSumTahminiRayicByBagisciId(int bagisciId)
        {
            DataTable table = repository.SelectSumTahminiRayicByBagisciId(bagisciId);
            return table != null && table.Rows.Count > 0 ? table.Rows[0]["Toplam"].ToString().ConvertToDecimal() : 0;
        }
        public int Save(Bagis item)
        {
            if (item == null) throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);
            if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGIS);
            return item.Id;
        }
        public bool Update(Bagis item)
        {
            if (item == null) throw new ArgumentNullException("item");
            Bagis oldItem = GetById(item.Id);
            if (item.Id == 0) return false;
            item.DegistirmeTarihi = DateTime.Now;
            item.Degistiren = UtilityHelper.GetCurrentUserName();
            bool ok = repository.Update(item);
            if (ok && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, oldItem, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGIS);
            return ok;
        }
        public bool Delete(Bagis item)
        {
            if (item == null || item.Id == 0) return false;
            Bagis oldItem = GetById(item.Id);
            if (oldItem == null) return false;
            bool ok = repository.Delete(item.Id);
            if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(oldItem, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGIS);
            return ok;
        }
        private static List<Bagis> ToList(DataTable table) { return new Bagis().ToList<Bagis>(table); }
        private static Bagis Map(DataTable table) { return ToList(table).FirstOrDefault(); }
    }
}
