using DAO.Repositories.IKYS;
using DAO.Repositories.Ortak;
using DAO.Ortak;
using System.Globalization;
using Utility.Logging;
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


        public List<UcretTanim> GetByKademe(int grupId, int kademe) { return ToList(repository.SelectByKademe(grupId, kademe)); }
        public List<UcretTanim> GetByMaxGrupId() { return ToList(repository.SelectByMaxGrupId()); }
        public DataTable GetByGrup() { return repository.SelectByGrup(); }
        public DataTable GetDerece() { return repository.SelectDerece(); }
        public DataTable GetKademe(int derece, int grupId) { return repository.SelectKademe(derece, grupId); }
        public DataTable GetMaasListesi(int grupId, DateTime tarih) { return repository.SelectMaasListesi(grupId, tarih); }
        public DateTime GetMaxBitisTarihi() { return repository.SelectMaxBitisTarihi().Rows[0][0].ConvertToDatetime(); }
        public decimal GetAgi(DateTime tarih) { return repository.SelectAgi(tarih).Rows[0][0].ConvertToDecimal(); }
        public int GetGrupIdByTarih(DateTime tarih) { return repository.SelectGrupIdByTarih(tarih).Rows[0][0].ConvertToInt(); }

        public decimal GetUcretByGrupDereceKademe(Personel personel, int grupId, int derece, int kademe) { DataTable table = repository.SelectUcretByGrupDereceKademe(grupId, derece, kademe, personel.Asker_sivil == ProjeConstants.PER_ASKER_INT); return table.Rows.Count > 0 ? table.Rows[0][0].ConvertToDecimal() : SqlDecimal.Null.Value; }

        public UcretTanim GetById(int id, PortalLogContext context)
        { return Map(repository.SelectById(id, context)); }
        public List<UcretTanim> GetAll(PortalLogContext context)
        { return ToList(repository.SelectAll(context)); }

        public int Save(UcretTanim item, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (item == null) throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                item.Id = repository.Insert(item, transaction, context);
                if (item.Id <= 0) throw new InvalidOperationException("Tanım kaydı oluşturulamadı.");
                int auditId = new PortalLogRepository().RecordAudit(context, "INSERT", "UcretTanim", item.Id.ToString(CultureInfo.InvariantCulture), transaction);
                RecordChanges(null, item, auditId, transaction);
                transaction.Complete();
            }
            return item.Id;
        }

        public bool Update(UcretTanim item, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (item == null || item.Id <= 0) return false;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                UcretTanim old = Map(repository.SelectById(item.Id, transaction, context));
                if (old == null) return false;
                item.DegistirmeTarihi = DateTime.Now;
                item.Degistiren = UtilityHelper.GetCurrentUserName();
                if (!repository.Update(item, transaction, context)) return false;
                int auditId = new PortalLogRepository().RecordAudit(context, "UPDATE", "UcretTanim", item.Id.ToString(CultureInfo.InvariantCulture), transaction);
                RecordChanges(old, item, auditId, transaction);
                transaction.Complete();
                return true;
            }
        }

        public bool Delete(UcretTanim item, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (item == null || item.Id <= 0) return false;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                UcretTanim old = Map(repository.SelectById(item.Id, transaction, context));
                if (old == null || !repository.Delete(item.Id, transaction, context)) return false;
                int auditId = new PortalLogRepository().RecordAudit(context, "DELETE", "UcretTanim", item.Id.ToString(CultureInfo.InvariantCulture), transaction);
                RecordChanges(old, null, auditId, transaction);
                transaction.Complete();
                return true;
            }
        }

        private static void RecordChanges(UcretTanim old, UcretTanim current, int auditId, SqlTransactionContext transaction)
        {
            if (auditId == 0) return;
            PortalLogRepository log = new PortalLogRepository();
            AddChange(log, auditId, "GrupId", old == null ? null : old.GrupId.ToString(CultureInfo.InvariantCulture), current == null ? null : current.GrupId.ToString(CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "Derece", old == null ? null : old.Derece.ToString(CultureInfo.InvariantCulture), current == null ? null : current.Derece.ToString(CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "Kademe", old == null ? null : old.Kademe.ToString(CultureInfo.InvariantCulture), current == null ? null : current.Kademe.ToString(CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "BaslangicTarihi", old == null ? null : AuditDate(old.BaslangicTarihi), current == null ? null : AuditDate(current.BaslangicTarihi), transaction);
            AddChange(log, auditId, "BitisTarihi", old == null ? null : AuditDate(old.BitisTarihi), current == null ? null : AuditDate(current.BitisTarihi), transaction);
            AddChange(log, auditId, "Unvan", old == null ? null : Convert.ToString(old.Unvan, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.Unvan, CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "AltUcret", old == null ? null : Convert.ToString(old.AltUcret, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.AltUcret, CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "UstUcret", old == null ? null : Convert.ToString(old.UstUcret, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.UstUcret, CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "AskerUcret", old == null ? null : Convert.ToString(old.AskerUcret, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.AskerUcret, CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "Agi", old == null ? null : Convert.ToString(old.Agi, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.Agi, CultureInfo.InvariantCulture), transaction);
        }

        private static void AddChange(PortalLogRepository log, int auditId, string field, string oldValue, string newValue, SqlTransactionContext transaction)
        {
            if (!string.Equals(oldValue, newValue, StringComparison.Ordinal))
                log.RecordAuditChange(auditId, field, oldValue, newValue, transaction);
        }

        private static string AuditDate(DateTime value)
        {
            return value < new DateTime(1901, 1, 1) ? null : value.ToString("o", CultureInfo.InvariantCulture);
        }


        public List<UcretTanim> GetByKademe(int grupId, int kademe, PortalLogContext context)
        { return ToList(repository.SelectByKademe(grupId, kademe, context)); }
        public List<UcretTanim> GetByMaxGrupId(PortalLogContext context)
        { return ToList(repository.SelectByMaxGrupId(context)); }
        public DataTable GetByGrup(PortalLogContext context)
        { return repository.SelectByGrup(context); }
        public DataTable GetDerece(PortalLogContext context)
        { return repository.SelectDerece(context); }
        public DataTable GetKademe(int derece, int grupId, PortalLogContext context)
        { return repository.SelectKademe(derece, grupId, context); }
        public DataTable GetMaasListesi(int grupId, DateTime tarih, PortalLogContext context)
        { return repository.SelectMaasListesi(grupId, tarih, context); }
        public DateTime GetMaxBitisTarihi(PortalLogContext context)
        { return repository.SelectMaxBitisTarihi(context).Rows[0][0].ConvertToDatetime(); }
        public decimal GetAgi(DateTime tarih, PortalLogContext context)
        { return repository.SelectAgi(tarih, context).Rows[0][0].ConvertToDecimal(); }
        public int GetGrupIdByTarih(DateTime tarih, PortalLogContext context)
        { return repository.SelectGrupIdByTarih(tarih, context).Rows[0][0].ConvertToInt(); }
        public decimal GetUcretByGrupDereceKademe(Personel personel, int grupId, int derece, int kademe, PortalLogContext context)
        { DataTable table = repository.SelectUcretByGrupDereceKademe(grupId, derece, kademe, personel.Asker_sivil == ProjeConstants.PER_ASKER_INT, context); return table.Rows.Count > 0 ? table.Rows[0][0].ConvertToDecimal() : SqlDecimal.Null.Value; }

        public List<UcretTanim> GetByGrupId(int grupId, PortalLogContext context) { return ToList(repository.SelectByGrupId(grupId, context)); }
        public bool TryRecordWebPartOpened(PortalLogContext context) { return new PortalLogRepository().TryRecordWebPartOpened(context); }

        public bool DeleteByGrupId(int grupId, UcretTanim auditItem, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (grupId <= 0) return false;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                List<UcretTanim> old = ToList(repository.SelectByGrupId(grupId, transaction, context));
                if (old.Count == 0 || !repository.DeleteByGrupId(grupId, transaction, context)) return false;
                PortalLogRepository log = new PortalLogRepository();
                foreach (UcretTanim deleted in old)
                {
                    int auditId = log.RecordAudit(context, "DELETE", "UcretTanim", deleted.Id.ToString(CultureInfo.InvariantCulture), transaction);
                    RecordChanges(deleted, null, auditId, transaction);
                }
                transaction.Complete();
                return true;
            }
        }

        private static UcretTanim Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static List<UcretTanim> ToList(DataTable table) { return new UcretTanim().ToList<UcretTanim>(table); }

        public DataTable GetMaasListesi(int grupId, DateTime tarih, SqlTransactionContext transaction, PortalLogContext context)
        { return repository.SelectMaasListesi(grupId, tarih, transaction, context); }

        public int GetGrupIdByTarih(DateTime tarih, SqlTransactionContext transaction, PortalLogContext context)
        { return repository.SelectGrupIdByTarih(tarih, transaction, context).Rows[0][0].ConvertToInt(); }

        public decimal GetUcretByGrupDereceKademe(Personel personel, int grupId, int derece, int kademe, SqlTransactionContext transaction, PortalLogContext context)
        { DataTable table = repository.SelectUcretByGrupDereceKademe(grupId, derece, kademe, personel.Asker_sivil == ProjeConstants.PER_ASKER_INT, transaction, context); return table.Rows.Count > 0 ? table.Rows[0][0].ConvertToDecimal() : SqlDecimal.Null.Value; }
    }
}
