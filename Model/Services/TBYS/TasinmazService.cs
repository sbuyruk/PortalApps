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
    public class TasinmazService
    {
        private readonly TasinmazRepository repository;

        public TasinmazService() : this(new TasinmazRepository()) { }

        public TasinmazService(TasinmazRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public Tasinmaz GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public Tasinmaz GetInventoryById(int id)
        {
            return Map(repository.SelectInventoryById(id));
        }

        public List<Tasinmaz> GetInventory()
        {
            return new Tasinmaz().ToList<Tasinmaz>(repository.SelectInventory());
        }
        public List<Tasinmaz> GetInventoryByIlAdi(string ilAdi) { return new Tasinmaz().ToList<Tasinmaz>(repository.SelectInventoryByIlAdi(ilAdi)); }
        public Tasinmaz GetOutOfInventoryById(int id) { return Map(repository.SelectOutOfInventoryById(id)); }
        public decimal GetInventoryValueTotal(string column, int bolgeId) { DataTable table = repository.SelectInventoryValueTotal(column, bolgeId, ProjeConstants.HEPSI_INT, ProjeConstants.BOLGE_GENELMUDURLUK_INT); return table != null && table.Rows.Count > 0 ? table.Rows[0]["Toplam"].ToString().ConvertToDecimal() : 0; }
        public decimal GetTahminiRayicToplami(int bolgeId) { return GetInventoryValueTotal("TahminiRayicDegeri", bolgeId); }
        public decimal GetEmlakBeyanDegeriToplami(int bolgeId) { return GetInventoryValueTotal("EmlakBeyanDegeri", bolgeId); }
        public decimal GetMuhasebeyeKayitliDegerToplami(int bolgeId) { return GetInventoryValueTotal("MuhasebeyeKayitliDeger", bolgeId); }
        public decimal GetYaklasikPiyasaToplami(int bolgeId) { return GetInventoryValueTotal("YaklasikPiyasaDegeri", bolgeId); }
        public Tasinmaz GetNext(int id) { return Map(repository.SelectNext(id)) ?? GetMin(); }
        public Tasinmaz GetPrev(int id) { return Map(repository.SelectPrev(id)) ?? GetMax(); }
        public Tasinmaz GetMax() { return GetExtreme(true); }
        public Tasinmaz GetMin() { return GetExtreme(false); }
        private Tasinmaz GetExtreme(bool max) { DataTable table = repository.SelectExtreme(max); if (table == null || table.Rows.Count == 0) return null; return GetById(Convert.ToInt32(table.Rows[0]["Id"])); }

        public int Save(Tasinmaz item)
        {
            if (item == null) throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);
            if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_TASINMAZ);
            return item.Id;
        }

        public bool Update(Tasinmaz item)
        {
            if (item == null) throw new ArgumentNullException("item");
            Tasinmaz oldItem = GetById(item.Id);
            if (item.Id == 0) return false;
            item.DegistirmeTarihi = DateTime.Now;
            item.Degistiren = UtilityHelper.GetCurrentUserName();
            bool ok = repository.Update(item);
            if (ok && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, oldItem, ProjeConstants.TBYS, ProjeConstants.TBYS_TASINMAZ);
            return ok;
        }

        public bool Delete(Tasinmaz item)
        {
            if (item == null || item.Id == 0) return false;
            Tasinmaz oldItem = GetById(item.Id);
            if (oldItem == null) return false;
            bool ok = repository.Delete(item.Id);
            if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(oldItem, ProjeConstants.TBYS, ProjeConstants.TBYS_TASINMAZ);
            return ok;
        }

        private static Tasinmaz Map(DataTable table)
        {
            return new Tasinmaz().ToList<Tasinmaz>(table).FirstOrDefault();
        }
    }
}
