using DAO.Repositories.IKYS;
using DAO.Repositories.Ortak;
using DAO.Ortak;
using Model.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Globalization;
using Utility.HelperClasses;
using Utility.Logging;

namespace Model.Services.IKYS
{
    public class HarcirahService
    {
        private readonly HarcirahRepository repository;
        public HarcirahService() : this(new HarcirahRepository()) { }
        public HarcirahService(HarcirahRepository repository) { if (repository == null) throw new ArgumentNullException("repository"); this.repository = repository; }
        public Harcirah GetById(int id) { return Map(repository.SelectById(id)); }
        public Harcirah GetById(int id, PortalLogContext context)
        { return Map(repository.SelectById(id, context)); }
        public List<Harcirah> GetAll() { return ToList(repository.SelectAll()); }
        public List<Harcirah> GetAll(PortalLogContext context)
        { return ToList(repository.SelectAll(context)); }

        public int Save(Harcirah item, PortalLogContext context) { if (context == null) throw new ArgumentNullException("context"); if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); using (SqlTransactionContext transaction = new SqlTransactionContext()) { item.Id = repository.Insert(item, transaction, context); int auditId = new PortalLogRepository().RecordAudit(context, "INSERT", "Harcirah", item.Id.ToString(CultureInfo.InvariantCulture), transaction); RecordChanges(null, item, auditId, transaction); transaction.Complete(); } return item.Id; }

        public bool Update(Harcirah item, PortalLogContext context) { if (context == null) throw new ArgumentNullException("context"); if (item == null || item.Id <= 0) return false; using (SqlTransactionContext transaction = new SqlTransactionContext()) { Harcirah old = Map(repository.SelectById(item.Id, transaction, context)); if (old == null) return false; item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); if (!repository.Update(item, transaction, context)) return false; int auditId = new PortalLogRepository().RecordAudit(context, "UPDATE", "Harcirah", item.Id.ToString(CultureInfo.InvariantCulture), transaction); RecordChanges(old, item, auditId, transaction); transaction.Complete(); return true; } }

        public bool Delete(Harcirah item, PortalLogContext context) { if (context == null) throw new ArgumentNullException("context"); if (item == null || item.Id <= 0) return false; using (SqlTransactionContext transaction = new SqlTransactionContext()) { Harcirah old = Map(repository.SelectById(item.Id, transaction, context)); if (old == null || !repository.Delete(item.Id, transaction, context)) return false; int auditId = new PortalLogRepository().RecordAudit(context, "DELETE", "Harcirah", item.Id.ToString(CultureInfo.InvariantCulture), transaction); RecordChanges(old, null, auditId, transaction); transaction.Complete(); return true; } }
        public List<Harcirah> GetByKadroUlkeTarih(int kadroGrupId, string ulke, DateTime tarih) { return ToList(repository.SelectByKadroUlkeTarih(kadroGrupId, ulke, tarih)); }
        public List<Harcirah> GetByKadroUlkeTarih(int kadroGrupId, string ulke, DateTime tarih, PortalLogContext context)
        { return ToList(repository.SelectByKadroUlkeTarih(kadroGrupId, ulke, tarih, context)); }
        public Harcirah GetByKadroGrupId(int kadroGrupId) { return Map(repository.SelectByKadroGrupId(kadroGrupId)); }
        public Harcirah GetByKadroGrupId(int kadroGrupId, PortalLogContext context)
        { return Map(repository.SelectByKadroGrupId(kadroGrupId, context)); }
        public Harcirah GetByParaBirimi(string paraBirimi) { return Map(repository.SelectByParaBirimi(paraBirimi)); }
        public Harcirah GetByParaBirimi(string paraBirimi, PortalLogContext context)
        { return Map(repository.SelectByParaBirimi(paraBirimi, context)); }
        public DataTable GetAllReturnDataTable() { return repository.SelectAllReturnDataTable(); }
        public DataTable GetAllReturnDataTable(PortalLogContext context)
        { return repository.SelectAllReturnDataTable(context); }
        public bool TryRecordWebPartOpened(PortalLogContext context) { return new PortalLogRepository().TryRecordWebPartOpened(context); }
        private static void RecordChanges(Harcirah old, Harcirah current, int auditId, SqlTransactionContext transaction) { if (auditId == 0) return; PortalLogRepository log = new PortalLogRepository(); AddChange(log, auditId, "KadroGrupId", old == null ? (object)null : old.KadroGrupId, current == null ? (object)null : current.KadroGrupId, transaction); AddChange(log, auditId, "SeriId", old == null ? (object)null : old.SeriId, current == null ? (object)null : current.SeriId, transaction); AddChange(log, auditId, "Sira", old == null ? (object)null : old.Sira, current == null ? (object)null : current.Sira, transaction); AddChange(log, auditId, "Ulke", old == null ? null : old.Ulke, current == null ? null : current.Ulke, transaction); AddChange(log, auditId, "Miktar", old == null ? (object)null : old.Miktar, current == null ? (object)null : current.Miktar, transaction); AddChange(log, auditId, "ParaBirimi", old == null ? null : old.ParaBirimi, current == null ? null : current.ParaBirimi, transaction);
            AddChange(log, auditId, "KadroGrubu", old == null ? null : Convert.ToString(old.KadroGrubu, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.KadroGrubu, CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "Aciklama", old == null ? null : Convert.ToString(old.Aciklama, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.Aciklama, CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "BaslangicTarihi", old == null ? null : old.BaslangicTarihi.ToString("o", CultureInfo.InvariantCulture), current == null ? null : current.BaslangicTarihi.ToString("o", CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "BitisTarihi", old == null ? null : old.BitisTarihi.ToString("o", CultureInfo.InvariantCulture), current == null ? null : current.BitisTarihi.ToString("o", CultureInfo.InvariantCulture), transaction);
        }
        private static void AddChange(PortalLogRepository log, int auditId, string field, object oldValue, object newValue, SqlTransactionContext transaction) { string oldText = oldValue == null ? null : Convert.ToString(oldValue, CultureInfo.InvariantCulture); string newText = newValue == null ? null : Convert.ToString(newValue, CultureInfo.InvariantCulture); if (!string.Equals(oldText, newText, StringComparison.Ordinal)) log.RecordAuditChange(auditId, field, oldText, newText, transaction); }
        private static Harcirah Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static List<Harcirah> ToList(DataTable table) { return new Harcirah().ToList<Harcirah>(table); }
    }
}
