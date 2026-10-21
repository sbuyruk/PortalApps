using DAO.Repositories.TBYS;
using DAO.Ortak;
using DAO.Repositories.Ortak;
using Utility.Logging;
using System.Globalization;
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
    public class SigortaService
    {
        private readonly SigortaRepository repository;

        public SigortaService() : this(new SigortaRepository()) { }

        public SigortaService(SigortaRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public Sigorta GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public List<Sigorta> GetAll()
        {
            return new Sigorta().ToList<Sigorta>(repository.SelectAll());
        }

        public List<Sigorta> GetByTasinmazId(int tasinmazId)
        {
            return new Sigorta().ToList<Sigorta>(repository.SelectByTasinmazId(tasinmazId));
        }

        public List<Sigorta> GetByIdList(int id)
        {
            return new Sigorta().ToList<Sigorta>(repository.SelectById(id));
        }

        public DataTable GetInventoryList()
        {
            return repository.SelectInventoryList();
        }

        public decimal GetInsuranceValueTotal(string sigortaCinsi)
        {
            return ReadTotal(repository.SelectInsuranceValueTotal(sigortaCinsi));
        }

        public decimal GetPremiumTotal(string sigortaCinsi)
        {
            return ReadTotal(repository.SelectPremiumTotal(sigortaCinsi));
        }

        public DataTable GetByTeminatSigortaCinsi(string sigortaCinsi, bool vadesiGelenler, bool isDeprem, bool isYangin, bool isMakine100000,
            bool isMakine5000, bool isJenerator, bool isAsansor, bool isKazan, int bolgeId, DateTime basTarih, DateTime bitTarih)
        {
            return repository.SelectByTeminatSigortaCinsi(sigortaCinsi, vadesiGelenler, isDeprem, isYangin, isMakine100000,
                isMakine5000, isJenerator, isAsansor, isKazan, bolgeId, basTarih, bitTarih);
        }

        public Sigorta GetNext(int sigortaId)
        {
            List<Sigorta> list = new Sigorta().ToList<Sigorta>(repository.SelectNavigationList());
            int index = list.FindIndex(s => s.Id == sigortaId);
            if (index < 0 || list.Count == 0) return new Sigorta();
            return index < list.Count - 1 ? list[index + 1] : list[list.Count - 1];
        }

        public Sigorta GetPrev(int sigortaId)
        {
            List<Sigorta> list = new Sigorta().ToList<Sigorta>(repository.SelectNavigationList());
            int index = list.FindIndex(s => s.Id == sigortaId);
            if (index < 0 || list.Count == 0) return new Sigorta();
            return index > 0 ? list[index - 1] : list[0];
        }

        public Sigorta GetMax()
        {
            return GetById(ReadId(repository.SelectMaxId()));
        }

        public Sigorta GetMin()
        {
            return GetById(ReadId(repository.SelectMinId()));
        }

        public string GetInventoryListAsJson()
        {
            return new Sigorta().ToJSON(GetInventoryList());
        }

        public Sigorta GetLatestByTasinmazId(int tasinmazId)
        {
            return GetByTasinmazId(tasinmazId).FirstOrDefault();
        }

        public int Save(Sigorta item)
        {
            if (item == null) throw new ArgumentNullException("item");

            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);

            if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG)
            {
                OlayKayit olayKayit = new OlayKayit();
                olayKayit.GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_SIGORTA);
            }

            return item.Id;
        }

        public bool Update(Sigorta item)
        {
            if (item == null) throw new ArgumentNullException("item");

            Sigorta oldItem = GetById(item.Id);
            bool isSuccess = false;
            if (item.Id != 0)
            {
                item.DegistirmeTarihi = DateTime.Now;
                item.Degistiren = UtilityHelper.GetCurrentUserName();
                isSuccess = repository.Update(item);
            }

            if (isSuccess && ProjeConstants.TBYS_UPDATE_LOG)
            {
                OlayKayit olayKayit = new OlayKayit();
                olayKayit.GuncellemeOlayKaydet(item, oldItem, ProjeConstants.TBYS, ProjeConstants.TBYS_SIGORTA);
            }

            return isSuccess;
        }

        public bool Delete(Sigorta item)
        {
            if (item == null || item.Id == 0) return false;

            Sigorta oldItem = GetById(item.Id);
            if (oldItem == null) return false;

            bool isDeleted = repository.Delete(item.Id);
            if (isDeleted && ProjeConstants.TBYS_DELETE_LOG)
            {
                OlayKayit olayKayit = new OlayKayit();
                olayKayit.SilmeOlayKaydet(oldItem, ProjeConstants.TBYS, ProjeConstants.TBYS_SIGORTA);
            }

            return isDeleted;
        }

        public Sigorta GetLatestByTasinmazId(int tasinmazId, SqlTransactionContext transaction, PortalLogContext context)
        {
            return Map(repository.SelectLatestByTasinmazId(tasinmazId, transaction, context));
        }

        public int Save(Sigorta item, SqlTransactionContext transaction, PortalLogContext context)
        {
            RequireTransaction(transaction, context);
            if (item == null) throw new ArgumentNullException("item", "Kaydedilecek sigorta gereklidir.");
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item, transaction, context);
            if (item.Id <= 0) throw new InvalidOperationException("Sigorta kaydı oluşturulamadı.");
            DataTable saved = repository.SelectById(item.Id, transaction, context);
            if (saved == null) throw new InvalidOperationException("Kaydedilen sigorta okunamadı.");
            RecordAudit("INSERT", item.Id, null, saved, transaction, context);
            return item.Id;
        }

        public bool Update(Sigorta item, SqlTransactionContext transaction, PortalLogContext context)
        {
            RequireTransaction(transaction, context);
            if (item == null || item.Id <= 0) return false;
            DataTable oldValues = repository.SelectById(item.Id, transaction, context);
            Sigorta old = Map(oldValues);
            if (old == null) return false;
            item.OlusturmaTarihi = old.OlusturmaTarihi;
            item.Olusturan = old.Olusturan;
            item.DegistirmeTarihi = DateTime.Now;
            item.Degistiren = UtilityHelper.GetCurrentUserName();
            if (!repository.Update(item, transaction, context)) return false;
            DataTable saved = repository.SelectById(item.Id, transaction, context);
            if (saved == null) throw new InvalidOperationException("Güncellenen sigorta okunamadı.");
            RecordAudit("UPDATE", item.Id, oldValues, saved, transaction, context);
            return true;
        }

        private static void RequireTransaction(SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction", "Veritabanı işlem bağlamı gereklidir.");
            if (context == null) throw new ArgumentNullException("context", "Bu işlem için TBYS log bağlamı gereklidir.");
        }

        private static void RecordAudit(string operation, int entityId, DataTable oldValues, DataTable current, SqlTransactionContext transaction, PortalLogContext context)
        {
            RequireTransaction(transaction, context);
            PortalLogRepository log = new PortalLogRepository();
            int auditId = log.RecordAudit(context, operation, "Sigorta", entityId.ToString(CultureInfo.InvariantCulture), transaction);
            if (auditId <= 0) return;
            foreach (string field in new[] { "TasinmazId", "BolumId", "SigortaCinsi", "SigortaBasTar", "SigortaBitTar", "SigortaBedeli", "Prim", "KullanimSekli" })
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

        private static Sigorta Map(DataTable table)
        {
            return new Sigorta().ToList<Sigorta>(table).FirstOrDefault();
        }

        private static decimal ReadTotal(DataTable table)
        {
            if (table == null || table.Rows.Count == 0)
                return 0;

            return table.Rows[0]["Toplam"].ReturnZeroIfNull().ConvertToDecimal();
        }

        private static int ReadId(DataTable table)
        {
            if (table == null || table.Rows.Count == 0)
                return 0;

            return table.Rows[0]["Id"].ConvertToInt();
        }
    }
}
