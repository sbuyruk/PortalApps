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
    public class IsTecrubeService
    {
        private readonly IsTecrubeRepository repository;
        public IsTecrubeService() : this(new IsTecrubeRepository()) { }
        public IsTecrubeService(IsTecrubeRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }
        public IsTecrube GetById(int id) { return Map(repository.SelectById(id)); }
        public List<IsTecrube> GetAll() { return ToList(repository.SelectAll()); }


        public List<IsTecrube> GetByPersonelId(int personelId) { return ToList(repository.SelectByPersonelId(personelId)); }
        public List<IsTecrube> GetByPersonelId(int personelId, PortalLogContext context)
        { return ToList(repository.SelectByPersonelId(personelId, context)); }
        public bool TryRecordWebPartOpened(PortalLogContext context) { return new PortalLogRepository().TryRecordWebPartOpened(context); }
        public int Save(IsTecrube item, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (item == null) throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                item.Id = repository.Insert(item, transaction, context);
                if (item.Id <= 0) throw new InvalidOperationException("Kayıt oluşturulamadı.");
                int auditId = new PortalLogRepository().RecordAudit(context, "INSERT", "IsTecrube", item.Id.ToString(CultureInfo.InvariantCulture), transaction);
                RecordChanges(null, item, auditId, transaction);
                transaction.Complete();
            }
            return item.Id;
        }
        public bool Update(IsTecrube item, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (item == null || item.Id <= 0) return false;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                IsTecrube old = Map(repository.SelectById(item.Id, transaction, context));
                if (old == null) return false;
                item.DegistirmeTarihi = DateTime.Now;
                item.Degistiren = UtilityHelper.GetCurrentUserName();
                if (!repository.Update(item, transaction, context)) return false;
                int auditId = new PortalLogRepository().RecordAudit(context, "UPDATE", "IsTecrube", item.Id.ToString(CultureInfo.InvariantCulture), transaction);
                RecordChanges(old, item, auditId, transaction);
                transaction.Complete();
                return true;
            }
        }
        public bool Delete(IsTecrube item, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (item == null || item.Id <= 0) return false;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                IsTecrube old = Map(repository.SelectById(item.Id, transaction, context));
                if (old == null) return false;
                if (!repository.Delete(item.Id, transaction, context)) return false;
                int auditId = new PortalLogRepository().RecordAudit(context, "DELETE", "IsTecrube", item.Id.ToString(CultureInfo.InvariantCulture), transaction);
                RecordChanges(old, null, auditId, transaction);
                transaction.Complete();
                return true;
            }
        }
        private static void RecordChanges(IsTecrube old, IsTecrube current, int auditId, SqlTransactionContext transaction)
        {
            if (auditId == 0) return;
            PortalLogRepository log = new PortalLogRepository();
            AddChange(log, auditId, "PersonelId", old == null ? null : old.PersonelId.ToString(CultureInfo.InvariantCulture), current == null ? null : current.PersonelId.ToString(CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "BasTar", old == null ? null : AuditDate(old.BasTar), current == null ? null : AuditDate(current.BasTar), transaction);
            AddChange(log, auditId, "BitTar", old == null ? null : AuditDate(old.BitTar), current == null ? null : AuditDate(current.BitTar), transaction);
            AddChange(log, auditId, "Isyeri", old == null ? null : Convert.ToString(old.Isyeri, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.Isyeri, CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "Gorevi", old == null ? null : Convert.ToString(old.Gorevi, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.Gorevi, CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "Adres", old == null ? null : Convert.ToString(old.Adres, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.Adres, CultureInfo.InvariantCulture), transaction);
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
        private static IsTecrube Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static List<IsTecrube> ToList(DataTable table) { return new IsTecrube().ToList<IsTecrube>(table); }

        public IsTecrube GetById(int id, PortalLogContext context)
        { return Map(repository.SelectById(id, context)); }

        public List<IsTecrube> GetAll(PortalLogContext context)
        { return ToList(repository.SelectAll(context)); }
    }
}
