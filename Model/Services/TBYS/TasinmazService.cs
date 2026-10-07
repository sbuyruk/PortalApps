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
        public string GetAddressByBolumId(int tasinmazId, int bolumId)
        {
            DataTable table = repository.SelectAddressByBolumId(tasinmazId, bolumId);
            if (table == null || table.Rows.Count == 0) return string.Empty;
            DataRow row = table.Rows[0];
            return row["Adres"].ReturnEmptyIfNull() + " " + row["BolumNo"].ReturnEmptyIfNull() + " " + row["Ilcesi"].ReturnEmptyIfNull() + "/" + row["Ili"].ReturnEmptyIfNull();
        }
        public DataTable GetByBolge(int bolgeId)
        {
            return repository.SelectByBolge(bolgeId, ProjeConstants.HEPSI_INT, ProjeConstants.BOLGE_GENELMUDURLUK_INT);
        }
        public DataTable GetWithoutDonor(string[] exitReasons)
        {
            return repository.SelectWithoutDonor(exitReasons);
        }
        public string GetSectionNumbersAsJson(int inventoryState, string rentalEligibility)
        {
            return new Tasinmaz().ToJSON(repository.SelectSectionNumbers(inventoryState, rentalEligibility));
        }
        public int GetCountBySigorta(int bolgeId, string countColumn, string propertyColumn, string propertyValue, string sigortaValue, bool includeOutOfInventory)
        {
            DataTable table = repository.SelectCountBySigorta(bolgeId, countColumn, propertyColumn, propertyValue, sigortaValue, includeOutOfInventory, ProjeConstants.HEPSI_INT, ProjeConstants.BOLGE_GENELMUDURLUK_INT);
            return table != null && table.Rows.Count > 0 ? table.Rows[0]["Adet"].ToString().ConvertToInt() : 0;
        }
        public DataTable GetSectionsByTasinmazId(int tasinmazId) { return repository.SelectSectionsByTasinmazId(tasinmazId); }
        public DataTable GetTotalByOwnership() { return repository.SelectTotalByOwnership(); }
        public DataTable GetRentalEligibleTotals() { return repository.SelectRentalEligibleTotals(); }
        public string GetOutOfInventoryListAsJson() { return new Tasinmaz().ToJSON(repository.SelectOutOfInventoryList(false)); }
        public DataTable GetOutOfInventoryList() { return repository.SelectOutOfInventoryList(true); }
        public DataTable GetAllInventoryReport() { return repository.SelectAllInventoryReport(); }
        public DataTable GetAllOutOfInventoryReport() { return repository.SelectAllOutOfInventoryReport(); }

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
        public decimal GetFilteredValueTotal(string column, string filterColumn, string value) { DataTable table = repository.SelectFilteredValueTotal(column, filterColumn, value); return table != null && table.Rows.Count > 0 ? table.Rows[0]["Toplam"].ToString().ConvertToDecimal() : 0; }
        public decimal GetEmlakBeyanBySigorta(string value) { return GetFilteredValueTotal("EmlakBeyanDegeri", "SigortaDurumu", value); }
        public decimal GetTahminiRayicBySigorta(string value) { return GetFilteredValueTotal("TahminiRayicDegeri", "SigortaDurumu", value); }
        public decimal GetTahminiRayicByKirayaUygunluk(string value) { return GetFilteredValueTotal("TahminiRayicDegeri", "KirayaUygunluk", value); }
        public decimal GetEmlakBeyanByKirayaUygunluk(string value) { return GetFilteredValueTotal("EmlakBeyanDegeri", "KirayaUygunluk", value); }
        public int GetCountByIl(string countColumn, string ilAdi, string mulkiyetSekli, string value, string valueColumn)
        {
            DataTable table = repository.SelectCountByIl(countColumn, ilAdi, mulkiyetSekli, value, valueColumn);
            return table != null && table.Rows.Count > 0 ? table.Rows[0]["Adet"].ToString().ConvertToInt() : 0;
        }
        public int GetCountByIlKullanim(string ilAdi, string mulkiyetSekli, string kullanimSekli) { return GetCountByIl("KullanimSekli", ilAdi, mulkiyetSekli, kullanimSekli, "KullanimSekli"); }
        public int GetCountByIlCinsi(string ilAdi, string mulkiyetSekli, string cinsi) { return GetCountByIl("Cinsi", ilAdi, mulkiyetSekli, cinsi, "Cinsi"); }
        public int GetCountByBolgeMulkiyet(int bolgeId, string mulkiyetSekli)
        {
            DataTable table = repository.SelectCountByBolge("MulkiyetSekli", bolgeId, mulkiyetSekli, "MulkiyetSekli", ProjeConstants.HEPSI_INT, ProjeConstants.BOLGE_GENELMUDURLUK_INT);
            return table != null && table.Rows.Count > 0 ? table.Rows[0]["Adet"].ToString().ConvertToInt() : 0;
        }
        public int GetCountByBolgeFilters(string countColumn, string primaryColumn, string primaryValue, string kiraDurumu, string mulkiyetSekli, string kirayaUygunluk, int bolgeId)
        {
            DataTable table = repository.SelectCountByBolgeFilters(countColumn, primaryColumn, primaryValue, kiraDurumu, mulkiyetSekli, kirayaUygunluk, bolgeId, ProjeConstants.HEPSI_INT, ProjeConstants.BOLGE_GENELMUDURLUK_INT);
            return table != null && table.Rows.Count > 0 ? table.Rows[0]["Adet"].ToString().ConvertToInt() : 0;
        }
        public Tasinmaz GetNext(int id) { return Map(repository.SelectNext(id)) ?? GetMin(); }
        public Tasinmaz GetPrev(int id) { return Map(repository.SelectPrev(id)) ?? GetMax(); }
        public Tasinmaz GetMax() { return GetExtreme(true); }
        public Tasinmaz GetMin() { return GetExtreme(false); }
        private Tasinmaz GetExtreme(bool max) { DataTable table = repository.SelectExtreme(max); if (table == null || table.Rows.Count == 0) return null; return GetById(Convert.ToInt32(table.Rows[0]["Id"])); }

        // Legacy report operations are exposed here while their Model wrappers are removed.
        public decimal SelectTahminiRayicToplami(int bolgeId) { return GetTahminiRayicToplami(bolgeId); }
        public decimal SelectEmlakBeyanDegeriToplami(int bolgeId) { return GetEmlakBeyanDegeriToplami(bolgeId); }
        public decimal SelectMuhasebeyeKayitliDegerToplami(int bolgeId) { return GetMuhasebeyeKayitliDegerToplami(bolgeId); }
        public decimal SelectYaklasikPiyasaToplami(int bolgeId) { return GetYaklasikPiyasaToplami(bolgeId); }
        public decimal SelectEmlakBeyanDegeriToplamiBySigorta(string sigorta) { return GetEmlakBeyanBySigorta(sigorta); }
        public decimal SelectTahminiRayicToplamiBySigorta(string sigorta) { return GetTahminiRayicBySigorta(sigorta); }
        public decimal SelectTahminiRayicToplamiByKirayaUygunluk(string kirayaUygunluk) { return GetTahminiRayicByKirayaUygunluk(kirayaUygunluk); }
        public decimal SelectEmlakBeyanToplamiByKirayaUygunluk(string kirayaUygunluk) { return GetEmlakBeyanByKirayaUygunluk(kirayaUygunluk); }
        public int SelectTasinmazAdetByBolgeMulkiyetSekli(int bolgeId, string mulkiyetSekli) { return GetCountByBolgeMulkiyet(bolgeId, mulkiyetSekli); }
        public int SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(int bolgeId, string kullanimSekli, string kiraDurumu, string mulkiyetSekli, string kirayaUygunluk = null) { return GetCountByBolgeFilters("KullanimSekli", "KullanimSekli", kullanimSekli, kiraDurumu, mulkiyetSekli, kirayaUygunluk, bolgeId); }
        public int SelectTasinmazAdetByBolgeCinsiKiraDurumu(int bolgeId, string cinsi, string kiraDurumu, string mulkiyetSekli, string kirayaUygunluk = null) { return GetCountByBolgeFilters("KullanimSekli", "Cinsi", cinsi, kiraDurumu, mulkiyetSekli, kirayaUygunluk, bolgeId); }
        public int SelectTasinmazAdetByBolgeKullanimSekliKirayaUygunluk(int bolgeId, string kullanimSekli, string kirayaUygunluk, string mulkiyetSekli) { return GetCountByBolgeFilters("KullanimSekli", "KullanimSekli", kullanimSekli, null, mulkiyetSekli, kirayaUygunluk, bolgeId); }
        public int SelectTasinmazAdetByBolgeCinsiKirayaUygunluk(int bolgeId, string cinsi, string kirayaUygunluk, string mulkiyetSekli) { return GetCountByBolgeFilters("Cinsi", "Cinsi", cinsi, null, mulkiyetSekli, kirayaUygunluk, bolgeId); }
        public int SelectTasinmazAdetByBolgeKirayaUygunluk(int bolgeId, string kirayaUygunluk, string mulkiyetSekli) { return GetCountByBolgeFilters("KullanimSekli", null, null, null, mulkiyetSekli, kirayaUygunluk, bolgeId); }
        public int SelectTasinmazAdetByBolgeKirayaUygunlukCinsi(int bolgeId, string kirayaUygunluk, string mulkiyetSekli) { return GetCountByBolgeFilters("Cinsi", null, null, null, mulkiyetSekli, kirayaUygunluk, bolgeId); }
        public int SelectTasinmazAdetByIliMulkiyetSekliKullanimSekli(string ilAdi, string mulkiyetSekli, string kullanimSekli) { return GetCountByIlKullanim(ilAdi, mulkiyetSekli, kullanimSekli); }
        public int SelectTasinmazAdetByIliMulkiyetSekliCinsi(string ilAdi, string mulkiyetSekli, string cinsi) { return GetCountByIlCinsi(ilAdi, mulkiyetSekli, cinsi); }
        public int SelectTasinmazAdetByBolgeMulkiyetSekliSigorta(int bolgeId, string mulkiyetSekli, string sigorta) { return GetCountBySigorta(bolgeId, "MulkiyetSekli", "MulkiyetSekli", mulkiyetSekli, sigorta, false); }
        public int SelectTasinmazAdetByBolgeKullanimSekliSigorta(int bolgeId, string kullanimSekli, string sigorta) { return GetCountBySigorta(bolgeId, "KullanimSekli", "KullanimSekli", kullanimSekli, sigorta, false); }
        public int SelectTasinmazAdetByBolgeKullanimSekliSigortaYeni(int bolgeId, string kullanimSekli, string sigorta) { return GetCountBySigorta(bolgeId, "KullanimSekli", "KullanimSekli", kullanimSekli, sigorta, true); }
        public DataTable SelectByBolgeReturnJson(int bolgeId) { return GetByBolge(bolgeId); }
        public string SelectByIdBolumId(int tasinmazId, int bolumId) { return GetAddressByBolumId(tasinmazId, bolumId); }
        public DataTable SelectBagiscisiOlmayanTasinmazlarByBagsciIdReturnDT() { return GetWithoutDonor(Tasinmaz.SatisVsDahilEnvanterdenCikmaSebepleri); }
        public string SelectTasinmazBolumNoReturnJson(int envanterde, string kirayaUygunluk) { return GetSectionNumbersAsJson(envanterde, kirayaUygunluk); }
        public string SelectEnvanterdeOlmayanTasinmazReturnJson() { return GetOutOfInventoryListAsJson(); }
        public DataTable SelectEnvanterdeOlmayanTasinmazReturnDataTable() { return GetOutOfInventoryList(); }
        public DataTable SelectAllReturnDataTable() { return GetAllInventoryReport(); }
        public DataTable SelectAllEnvanterdenCikanReturnDataTable() { return GetAllOutOfInventoryReport(); }
        public DataTable SelectBolumByTasinmazId(int tasinmazId) { return GetSectionsByTasinmazId(tasinmazId); }
        public Tasinmaz Select(int id) { return GetById(id); }
        public Tasinmaz SelectById(int id) { return GetById(id); }
        public Tasinmaz SelectEnvanterdenCikanTasinmaz(int id) { return GetOutOfInventoryById(id); }
        public List<Tasinmaz> SelectByIlAdi(string ilAdi) { return GetInventoryByIlAdi(ilAdi); }
        public Tasinmaz SelectNext(int id) { return GetNext(id); }
        public Tasinmaz SelectPrev(int id) { return GetPrev(id); }
        public Tasinmaz SelectMax() { return GetMax(); }
        public Tasinmaz SelectMin() { return GetMin(); }

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
