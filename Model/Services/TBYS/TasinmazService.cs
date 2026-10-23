using DAO.Repositories.TBYS;
using DAO.Ortak;
using DAO.Repositories.Ortak;
using System.Globalization;
using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;
using Utility.Logging;

namespace Model.Services.TBYS
{
    public class TasinmazService
    {
        private readonly TasinmazRepository repository;
        private readonly PortalLogContext context;
        private readonly BagisRepository donationRepository;
        private readonly TasinmazBagisciRepository donorRepository;
        private readonly BagimsizBolumRepository sectionRepository;
        private readonly SozlesmeTasinmazRepository contractRepository;
        private readonly OnarimRepository repairRepository;
        private readonly SigortaService insuranceService;
        private readonly SigortaRepository insuranceRepository;
        private readonly BagimsizBolumService sectionService;

        public TasinmazService() : this(new TasinmazRepository()) { }

        public TasinmazService(PortalLogContext context) : this(new TasinmazRepository(context))
        {
            this.context = context;
            donationRepository = new BagisRepository(context);
            donorRepository = new TasinmazBagisciRepository(context);
            sectionRepository = new BagimsizBolumRepository(context);
            contractRepository = new SozlesmeTasinmazRepository(context);
            repairRepository = new OnarimRepository(context);
            insuranceRepository = new SigortaRepository(context);
            insuranceService = new SigortaService(context);
            sectionService = new BagimsizBolumService(context);
        }

        public TasinmazService(TasinmazRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public Tasinmaz GetById(int id)
        {
            return Map(repository.SelectById(id));
        }
        public Tasinmaz GetById(int id, PortalLogContext context)
        {
            return Map(new TasinmazRepository(context).SelectById(id));
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
        public Tasinmaz Select(int id, PortalLogContext context) { return GetById(id, context); }
        public Tasinmaz SelectById(int id) { return GetById(id); }
        public Tasinmaz SelectEnvanterdenCikanTasinmaz(int id) { return GetOutOfInventoryById(id); }
        public List<Tasinmaz> SelectByIlAdi(string ilAdi) { return GetInventoryByIlAdi(ilAdi); }
        public Tasinmaz SelectNext(int id) { return GetNext(id); }
        public Tasinmaz SelectPrev(int id) { return GetPrev(id); }
        public Tasinmaz SelectMax() { return GetMax(); }
        public Tasinmaz SelectMin() { return GetMin(); }

        public int Save(Tasinmaz item)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                int id = Save(item, transaction);
                transaction.Complete();
                return id;
            }
        }

        public bool Update(Tasinmaz item)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                bool updated = Update(item, transaction);
                if (updated) transaction.Complete();
                return updated;
            }
        }

        public bool Delete(Tasinmaz item)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                bool deleted = Delete(item, transaction);
                if (deleted) transaction.Complete();
                return deleted;
            }
        }

        public int CopyOutOfInventory(int sourceId)
        {
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                int id = CopyOutOfInventory(sourceId, transaction);
                transaction.Complete();
                return id;
            }
        }

        public int CopyOutOfInventory(int sourceId, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            Tasinmaz target = Map(repository.SelectById(sourceId, transaction, context));
            if (target == null || target.EnvanterdeMi != ProjeConstants.TASINMAZ_ENVANTERDEN_CIKTI)
                throw new InvalidOperationException("Kopyalanacak envanter dışı taşınmaz bulunamadı.");

            // Read and lock the entire transfer set before creating the new property.
            DataTable sourceSections = sectionRepository.SelectAllByTasinmazId(sourceId, transaction, context);
            DataTable sourceDonation = donationRepository.SelectByTasinmazId(sourceId, transaction, context);
            DataTable sourceInsurance = insuranceRepository.SelectLatestByTasinmazId(sourceId, transaction, context);
            DataTable sourceRepairs = repairRepository.SelectAllByTasinmazId(sourceId, transaction, context);
            DataTable sourceContracts = contractRepository.SelectAllByTasinmazId(sourceId, transaction, context);

            Bagis oldDonation = new Bagis().ToList<Bagis>(sourceDonation).FirstOrDefault();
            if (oldDonation != null)
            {
                TasinmazBagisci donor = new TasinmazBagisci().ToList<TasinmazBagisci>(
                    donorRepository.SelectById(oldDonation.BagisciId, transaction, context)).FirstOrDefault();
                if (donor == null) throw new InvalidOperationException("Taşınmazın ilişkili bağışçısı bulunamadı.");
                target.BagisciId = donor.Id;
                target.Bagisci = donor.Adi + ' ' + donor.Soyadi;
            }
            target.Id = 0;
            target.EnvanterdeMi = ProjeConstants.TASINMAZ_ENVANTERDE;
            target.EnvanterdenCikmaSebebi = string.Empty;
            target.EnvanterdenCikmaBedeli = 0;
            target.EnvanterdenCikmaTarihi = DateTime.MinValue;
            target.Degistiren = null;
            target.DegistirmeTarihi = DateTime.MinValue;
            int targetId = Save(target, transaction);

            Dictionary<int, int> sectionIds = new Dictionary<int, int>();
            if (sourceSections != null)
            {
                foreach (DataRow row in sourceSections.Rows)
                {
                    BagimsizBolum section = new BagimsizBolum().ToList<BagimsizBolum>(SingleRow(row)).First();
                    int oldId = section.Id;
                    section.Id = 0;
                    section.TasinmazId = targetId;
                    section.Degistiren = null;
                    section.DegistirmeTarihi = DateTime.MinValue;
                    sectionIds.Add(oldId, sectionService.Save(section, transaction));
                }
            }
            if (oldDonation != null) CopyDonation(oldDonation, targetId, transaction);
            if (sourceInsurance != null)
            {
                int insuranceId = Convert.ToInt32(sourceInsurance.Rows[0]["Id"]);
                object sectionId = RemapSection(sourceInsurance.Rows[0]["BolumId"], sectionIds);
                insuranceRepository.MoveToTasinmaz(insuranceId, sourceId, targetId, sectionId,
                    DateTime.Now, UtilityHelper.GetCurrentUserName(), transaction, context);
                DataTable saved = insuranceRepository.SelectById(insuranceId, transaction, context);
                RecordRelationAudit("UPDATE", "Sigorta", insuranceId, sourceInsurance, saved,
                    new[] { "TasinmazId", "BolumId" }, transaction);
            }
            if (sourceRepairs != null)
            {
                foreach (DataRow row in sourceRepairs.Rows)
                {
                    int id = Convert.ToInt32(row["Id"]);
                    repairRepository.MoveToTasinmaz(id, sourceId, targetId,
                        DateTime.Now, UtilityHelper.GetCurrentUserName(), transaction, context);
                    DataTable saved = repairRepository.SelectById(id, transaction, context);
                    RecordRelationAudit("UPDATE", "Onarim", id, SingleRow(row), saved,
                        new[] { "TasinmazId" }, transaction);
                }
            }
            if (sourceContracts != null)
            {
                foreach (DataRow row in sourceContracts.Rows)
                {
                    int id = Convert.ToInt32(row["Id"]);
                    object sectionId = RemapSection(row["BolumId"], sectionIds);
                    contractRepository.MoveToTasinmaz(id, sourceId, targetId, sectionId,
                        DateTime.Now, UtilityHelper.GetCurrentUserName(), transaction, context);
                    DataTable saved = contractRepository.SelectById(id, transaction, context);
                    RecordRelationAudit("UPDATE", "SozlesmeTasinmaz", id, SingleRow(row), saved,
                        new[] { "SozlesmeId", "TasinmazId", "BolumId" }, transaction);
                }
            }
            return targetId;
        }

        private void CopyDonation(Bagis source, int targetId, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            Bagis donation = new Bagis
            {
                BagisciId = source.BagisciId, TasinmazId = targetId,
                BagisTarihi = source.BagisTarihi, BagisYili = source.BagisYili,
                Envanterde = ProjeConstants.ENVANTERDE,
                OlusturmaTarihi = DateTime.Now, Olusturan = UtilityHelper.GetCurrentUserName()
            };
            int id = donationRepository.Insert(donation, transaction, context);
            if (id <= 0) throw new InvalidOperationException("Kopyalanan taşınmazın bağış kaydı oluşturulamadı.");
            DataTable saved = donationRepository.SelectById(id, transaction, context);
            RecordRelationAudit("INSERT", "Bagis", id, null, saved,
                new[] { "BagisciId", "TasinmazId", "BagisTarihi", "BagisYili", "Envanterde" }, transaction);
        }

        private void RecordRelationAudit(string operation, string entityName, int id, DataTable oldValues,
            DataTable current, string[] fields, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            if (current == null || current.Rows.Count != 1)
                throw new InvalidOperationException("İlişkili kaydın işlem sonucu doğrulanamadı.");
            PortalLogRepository log = new PortalLogRepository();
            int auditId = log.RecordAudit(context, operation, entityName, id.ToString(CultureInfo.InvariantCulture), transaction);
            if (auditId == 0) return;
            if (auditId <= 0) return;
            foreach (string field in fields)
            {
                string oldValue = AuditValue(oldValues, field);
                string newValue = AuditValue(current, field);
                if (!string.Equals(oldValue, newValue, StringComparison.Ordinal))
                    log.RecordAuditChange(auditId, field, oldValue, newValue, transaction);
            }
        }

        private static object RemapSection(object value, Dictionary<int, int> sectionIds)
        {
            if (value == DBNull.Value) return DBNull.Value;
            int sourceId = Convert.ToInt32(value);
            if (sourceId == 0) return 0;
            int targetId;
            if (!sectionIds.TryGetValue(sourceId, out targetId))
                throw new InvalidOperationException("İlişkili bağımsız bölüm kaynak taşınmaza ait değil; kopyalama tamamlanamadı.");
            return targetId;
        }

        private static DataTable SingleRow(DataRow row)
        {
            DataTable table = row.Table.Clone();
            table.ImportRow(row);
            return table;
        }

        public int SaveWithRelations(Tasinmaz item, bool synchronizeDonor)
        {
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                int id = Save(item, transaction);
                if (synchronizeDonor && SynchronizeDonor(item, transaction) && !Update(item, transaction))
                    throw new InvalidOperationException("Taşınmazın bağışçı bilgisi güncellenemedi.");
                SynchronizeInsurance(item, transaction);
                transaction.Complete();
                return id;
            }
        }

        public bool UpdateWithRelations(Tasinmaz item, bool synchronizeDonor, bool synchronizeInsurance)
        {
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                RequireTransaction(transaction);
                if (item == null || item.Id <= 0 || repository.SelectById(item.Id, transaction, context) == null) return false;
                if (synchronizeDonor) SynchronizeDonor(item, transaction);
                if (!Update(item, transaction)) return false;
                if (synchronizeInsurance) SynchronizeInsurance(item, transaction);
                transaction.Complete();
                return true;
            }
        }

        public int Save(Tasinmaz item, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            if (item == null) throw new ArgumentNullException("item", "Kaydedilecek taşınmaz gereklidir.");
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item, transaction, context);
            if (item.Id <= 0) throw new InvalidOperationException("Taşınmaz kaydı oluşturulamadı.");
            DataTable saved = repository.SelectById(item.Id, transaction, context);
            if (saved == null) throw new InvalidOperationException("Kaydedilen taşınmaz okunamadı.");
            RecordAudit("INSERT", item.Id, null, saved, transaction);
            return item.Id;
        }

        public bool Update(Tasinmaz item, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            if (item == null || item.Id <= 0) return false;
            DataTable oldValues = repository.SelectById(item.Id, transaction, context);
            Tasinmaz old = Map(oldValues);
            if (old == null) return false;
            item.OlusturmaTarihi = old.OlusturmaTarihi;
            item.Olusturan = old.Olusturan;
            item.DegistirmeTarihi = DateTime.Now;
            item.Degistiren = UtilityHelper.GetCurrentUserName();
            if (!repository.Update(item, transaction, context)) return false;
            DataTable saved = repository.SelectById(item.Id, transaction, context);
            if (saved == null) throw new InvalidOperationException("Güncellenen taşınmaz okunamadı.");
            RecordAudit("UPDATE", item.Id, oldValues, saved, transaction);
            return true;
        }

        public bool Delete(Tasinmaz item, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            if (item == null || item.Id <= 0) return false;
            DataTable oldValues = repository.SelectById(item.Id, transaction, context);
            if (oldValues == null) return false;
            // Hold dependency locks until the delete and its audit commit together.
            if (sectionRepository.SelectByTasinmazId(item.Id, transaction, context) != null ||
                donationRepository.SelectByTasinmazId(item.Id, transaction, context) != null ||
                contractRepository.SelectByTasinmazId(item.Id, transaction, context) != null ||
                repairRepository.SelectByTasinmazId(item.Id, transaction, context) != null ||
                insuranceService.GetLatestByTasinmazId(item.Id, transaction, context) != null) return false;
            if (!repository.Delete(item.Id, transaction, context)) return false;
            RecordAudit("DELETE", item.Id, oldValues, null, transaction);
            return true;
        }

        private bool SynchronizeDonor(Tasinmaz item, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            Bagis donation = new Bagis().ToList<Bagis>(donationRepository.SelectByTasinmazId(item.Id, transaction, context)).FirstOrDefault();
            if (donation == null) return false;
            TasinmazBagisci donor = new TasinmazBagisci().ToList<TasinmazBagisci>(donorRepository.SelectById(donation.BagisciId, transaction, context)).FirstOrDefault();
            if (donor == null) return false;
            string name = donor.Adi + ' ' + donor.Soyadi;
            bool changed = item.BagisciId != donor.Id || !string.Equals(item.Bagisci, name, StringComparison.Ordinal);
            item.BagisciId = donor.Id;
            item.Bagisci = name;
            return changed;
        }

        private void SynchronizeInsurance(Tasinmaz item, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            Sigorta insurance = insuranceService.GetLatestByTasinmazId(item.Id, transaction, context);
            if (insurance == null)
            {
                insurance = new Sigorta { TasinmazId = item.Id, SigortaCinsi = item.SigortaDurumu };
                insuranceService.Save(insurance, transaction, context);
            }
            else if (!string.Equals(insurance.SigortaCinsi, item.SigortaDurumu, StringComparison.Ordinal))
            {
                insurance.SigortaCinsi = item.SigortaDurumu;
                if (!insuranceService.Update(insurance, transaction, context))
                    throw new InvalidOperationException("Taşınmazın sigorta bilgisi güncellenemedi.");
            }
        }

        private void RequireTransaction(SqlTransactionContext transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction", "Veritabanı işlem bağlamı gereklidir.");
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
        }

        private void RecordAudit(string operation, int entityId, DataTable oldValues, DataTable current, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            PortalLogRepository log = new PortalLogRepository();
            int auditId = log.RecordAudit(context, operation, "Tasinmaz", entityId.ToString(CultureInfo.InvariantCulture), transaction);
            if (auditId == 0) return;
            if (auditId <= 0) return;
            // Keep personal names, addresses, document references and free text out of audit detail.
            foreach (string field in new[] { "IlId", "IlceId", "SigortaDurumu", "MulkiyetSekli", "KiraDurumu", "EdinmeSekli",
                "MuhasebeyeKayitliDeger", "TahminiRayicDegeri", "EmlakBeyanDegeri", "YaklasikPiyasaDegeri", "TapuTarihi",
                "KullanimSekli", "EnvanterdeMi", "EnvantereGirisTarihi", "EnvanterdenCikmaSebebi", "EnvanterdenCikmaTarihi",
                "EnvanterdenCikmaBedeli", "BagisciId", "Metrekare", "KirayaUygunluk", "KatMulkiyeti", "KatIrtifaki", "AltBolum",
                "ToplamMetrekare", "ZeminHisse", "BBBrutAlan", "BBNetAlan", "TapuIslemTarihi", "BagimsizBolumSayisi", "MalikSayisi", "SatisPlani" })
            {
                string oldValue = AuditValue(oldValues, field);
                string newValue = AuditValue(current, field);
                if (!string.Equals(oldValue, newValue, StringComparison.Ordinal))
                    log.RecordAuditChange(auditId, field, oldValue, newValue, transaction);
            }
        }

        private static string AuditValue(DataTable table, string field)
        {
            if (table == null || table.Rows.Count == 0 || table.Rows[0][field] == DBNull.Value) return null;
            object value = table.Rows[0][field];
            if (value is DateTime) return ((DateTime)value).ToString("o", CultureInfo.InvariantCulture);
            return value is decimal ? ((decimal)value).ToString("G29", CultureInfo.InvariantCulture)
                : Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        private static Tasinmaz Map(DataTable table)
        {
            return new Tasinmaz().ToList<Tasinmaz>(table).FirstOrDefault();
        }
    }
}
