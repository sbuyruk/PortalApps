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
using Utility.ProjeGlobal;
using Utility.Logging;

namespace Model.Services.IKYS
{
    public class KimlikService
    {
        private readonly KimlikRepository repository;
        public KimlikService() : this(new KimlikRepository()) { }
        public KimlikService(KimlikRepository repository) { if (repository == null) throw new ArgumentNullException("repository"); this.repository = repository; }
        public Kimlik GetById(int id) { return Map(repository.SelectById(id)); }
        public List<Kimlik> GetAll() { return ToList(repository.SelectAll()); }
        public int Save(Kimlik item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.IKYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_KIMLIK); return item.Id; }
        public int Save(Kimlik item, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName();
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                item.Id = Save(item, transaction, context); transaction.Complete();
            }
            return item.Id;
        }
        public int Save(Kimlik item, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (context == null) throw new ArgumentNullException("context");
            if (item == null) throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item, transaction, context);
            if (item.Id <= 0) throw new InvalidOperationException("Kimlik kaydı oluşturulamadı.");
            int auditId = new PortalLogRepository().RecordAudit(context, "INSERT", "Kimlik", item.Id.ToString(CultureInfo.InvariantCulture), transaction);
            RecordChanges(null, item, auditId, transaction);
            return item.Id;
        }
        public bool Update(Kimlik item) { if (item == null) throw new ArgumentNullException("item"); Kimlik old = GetById(item.Id); bool ok = false; if (item.Id != 0) { item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); ok = repository.Update(item); } if (ok && ProjeConstants.IKYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.IKYS, ProjeConstants.IKYS_KIMLIK); return ok; }
        public bool Update(Kimlik item, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (item == null || item.Id <= 0) return false;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                bool updated = Update(item, transaction, context);
                if (updated) transaction.Complete();
                return updated;
            }
        }
        public bool Update(Kimlik item, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (context == null) throw new ArgumentNullException("context");
            if (item == null || item.Id <= 0) return false;
            Kimlik old = Map(repository.SelectById(item.Id, transaction, context)); item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName();
            if (old == null || !repository.Update(item, transaction, context)) return false;
            int auditId = new PortalLogRepository().RecordAudit(context, "UPDATE", "Kimlik", item.Id.ToString(CultureInfo.InvariantCulture), transaction);
            RecordChanges(old, item, auditId, transaction);
            return true;
        }
        public bool Delete(Kimlik item) { if (item == null || item.Id == 0) return false; Kimlik old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.IKYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.IKYS, ProjeConstants.IKYS_KIMLIK); return ok; }
        public Kimlik GetByPersonelId(int personelId) { return Map(repository.SelectByPersonelId(personelId)); }
        public Kimlik GetByPersonelId(int personelId, PortalLogContext context) { return Map(repository.SelectByPersonelId(personelId, context)); }
        public Kimlik GetByPersonelId(int personelId, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (context == null) throw new ArgumentNullException("context");
            return Map(repository.SelectByPersonelId(personelId, transaction, context));
        }
        public Kimlik GetByTcKimlikNo(string kimlikNo) { return Map(repository.SelectByTcKimlikNo(kimlikNo)); }
        public Kimlik GetByTcKimlikNo(string kimlikNo, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            return Map(repository.SelectByTcKimlikNo(kimlikNo, context));
        }
        public DataTable GetAllFromKimlik() { return repository.SelectAllFromKimlik(); }
        private static void RecordChanges(Kimlik old, Kimlik current, int auditId, SqlTransactionContext transaction)
        {
            if (auditId == 0) return;
            PortalLogRepository log = new PortalLogRepository();
            AddChange(log, auditId, "PersonelId", old == null ? null : old.PersonelId.ToString(CultureInfo.InvariantCulture), current == null ? null : current.PersonelId.ToString(CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "MedeniHali", old == null ? null : old.MedeniHali, current == null ? null : current.MedeniHali, transaction);
            AddChange(log, auditId, "Cinsiyet", old == null ? null : old.Cinsiyet, current == null ? null : current.Cinsiyet, transaction);
            AddChange(log, auditId, "KanGrubu", old == null ? null : old.KanGrubu, current == null ? null : current.KanGrubu, transaction);
        }
        private static void AddChange(PortalLogRepository log, int auditId, string field, string oldValue, string newValue, SqlTransactionContext transaction) { if (!string.Equals(oldValue, newValue, StringComparison.Ordinal)) log.RecordAuditChange(auditId, field, oldValue, newValue, transaction); }
        private static Kimlik Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static List<Kimlik> ToList(DataTable table) { return new Kimlik().ToList<Kimlik>(table); }
    }
}
