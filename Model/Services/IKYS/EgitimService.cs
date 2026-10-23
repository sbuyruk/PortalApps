using DAO.Repositories.IKYS;
using DAO.Repositories.Ortak;
using DAO.Ortak;
using Model.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Utility.Logging;

namespace Model.Services.IKYS
{
    public class EgitimService
    {
        private readonly EgitimRepository repository;
        public EgitimService() : this(new EgitimRepository()) { }
        public EgitimService(EgitimRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }
        public Egitim GetById(int id) { return Map(repository.SelectById(id)); }
        public List<Egitim> GetAll() { return ToList(repository.SelectAll()); }


        public List<Egitim> GetByPersonelId(int personelId) { return ToList(repository.SelectByPersonelId(personelId)); }
        public List<Egitim> GetByPersonelId(int personelId, PortalLogContext context)
        { return ToList(repository.SelectByPersonelId(personelId, context)); }
        public bool TryRecordWebPartOpened(PortalLogContext context) { return new PortalLogRepository().TryRecordWebPartOpened(context); }
        public int Save(Egitim item, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (item == null) throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                item.Id = repository.Insert(item, transaction, context);
                if (item.Id <= 0) throw new InvalidOperationException("Kayıt oluşturulamadı.");
                int auditId = new PortalLogRepository().RecordAudit(context, "INSERT", "Egitim", item.Id.ToString(CultureInfo.InvariantCulture), transaction);
                RecordChanges(null, item, auditId, transaction);
                transaction.Complete();
            }
            return item.Id;
        }
        public bool Update(Egitim item, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (item == null || item.Id <= 0) return false;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                Egitim old = Map(repository.SelectById(item.Id, transaction, context));
                if (old == null) return false;
                item.DegistirmeTarihi = DateTime.Now;
                item.Degistiren = UtilityHelper.GetCurrentUserName();
                if (!repository.Update(item, transaction, context)) return false;
                int auditId = new PortalLogRepository().RecordAudit(context, "UPDATE", "Egitim", item.Id.ToString(CultureInfo.InvariantCulture), transaction);
                RecordChanges(old, item, auditId, transaction);
                transaction.Complete();
                return true;
            }
        }
        public bool Delete(Egitim item, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (item == null || item.Id <= 0) return false;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                Egitim old = Map(repository.SelectById(item.Id, transaction, context));
                if (old == null) return false;
                if (!repository.Delete(item.Id, transaction, context)) return false;
                int auditId = new PortalLogRepository().RecordAudit(context, "DELETE", "Egitim", item.Id.ToString(CultureInfo.InvariantCulture), transaction);
                RecordChanges(old, null, auditId, transaction);
                transaction.Complete();
                return true;
            }
        }
        private static void RecordChanges(Egitim old, Egitim current, int auditId, SqlTransactionContext transaction)
        {
            if (auditId == 0) return;
            PortalLogRepository log = new PortalLogRepository();
            AddChange(log, auditId, "PersonelId", old == null ? null : old.PersonelId.ToString(CultureInfo.InvariantCulture), current == null ? null : current.PersonelId.ToString(CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "Seviye", old == null ? null : old.Seviye, current == null ? null : current.Seviye, transaction);
            AddChange(log, auditId, "MezuniyetTar", old == null ? null : AuditDate(old.MezuniyetTar), current == null ? null : AuditDate(current.MezuniyetTar), transaction);
            AddChange(log, auditId, "Okul", old == null ? null : Convert.ToString(old.Okul, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.Okul, CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "Aciklama", old == null ? null : Convert.ToString(old.Aciklama, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.Aciklama, CultureInfo.InvariantCulture), transaction);
        }
        private static string AuditDate(DateTime value)
        {
            return value < new DateTime(1901, 1, 1) ? null : value.ToString("o", CultureInfo.InvariantCulture);
        }
        private static void AddChange(PortalLogRepository log, int auditId, string field, string oldValue, string newValue, SqlTransactionContext transaction)
        {
            if (!string.Equals(oldValue, newValue, StringComparison.Ordinal))
                log.RecordAuditChange(auditId, field, oldValue, newValue, transaction);
        }
        private static Egitim Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static List<Egitim> ToList(DataTable table) { return new Egitim().ToList<Egitim>(table); }

        public Egitim GetById(int id, PortalLogContext context)
        { return Map(repository.SelectById(id, context)); }

        public List<Egitim> GetAll(PortalLogContext context)
        { return ToList(repository.SelectAll(context)); }
    }
}
