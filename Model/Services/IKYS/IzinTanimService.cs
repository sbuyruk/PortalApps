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
using Utility.ProjeGlobal;

namespace Model.Services.IKYS
{
    public class IzinTanimService
    {
        private readonly IzinTanimRepository repository;
        public IzinTanimService() : this(new IzinTanimRepository()) { }
        public IzinTanimService(IzinTanimRepository repository) { if (repository == null) throw new ArgumentNullException("repository"); this.repository = repository; }
        public IzinTanim GetById(int id) { return Map(repository.SelectById(id)); }
        public List<IzinTanim> GetAll() { return ToList(repository.SelectAll()); }
        public int Save(IzinTanim item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.IKYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_IZINTANIM); return item.Id; }
        public bool Update(IzinTanim item) { if (item == null) throw new ArgumentNullException("item"); IzinTanim old = GetById(item.Id); bool ok = false; if (item.Id != 0) { item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); ok = repository.Update(item); } if (ok && ProjeConstants.IKYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.IKYS, ProjeConstants.IKYS_IZINTANIM); return ok; }
        public bool Delete(IzinTanim item) { if (item == null || item.Id == 0) return false; IzinTanim old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.IKYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.IKYS, ProjeConstants.IKYS_IZINTANIM); return ok; }

        public IzinTanim GetById(int id, PortalLogContext context) { return Map(repository.SelectById(id, context)); }
        public List<IzinTanim> GetAll(PortalLogContext context) { return ToList(repository.SelectAll(context)); }

        public int Save(IzinTanim item, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (!PortalLogSettings.IsEnabled(context)) return Save(item);
            if (item == null) throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                item.Id = repository.Insert(item, transaction, context);
                if (item.Id <= 0) throw new InvalidOperationException("Tanım kaydı oluşturulamadı.");
                int auditId = new PortalLogRepository().RecordAudit(context, "INSERT", "IzinTanim", item.Id.ToString(CultureInfo.InvariantCulture), transaction);
                RecordChanges(null, item, auditId, transaction);
                transaction.Complete();
            }
            return item.Id;
        }

        public bool Update(IzinTanim item, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (!PortalLogSettings.IsEnabled(context)) return Update(item);
            if (item == null || item.Id <= 0) return false;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                IzinTanim old = Map(repository.SelectById(item.Id, transaction, context));
                if (old == null) return false;
                item.DegistirmeTarihi = DateTime.Now;
                item.Degistiren = UtilityHelper.GetCurrentUserName();
                if (!repository.Update(item, transaction, context)) return false;
                int auditId = new PortalLogRepository().RecordAudit(context, "UPDATE", "IzinTanim", item.Id.ToString(CultureInfo.InvariantCulture), transaction);
                RecordChanges(old, item, auditId, transaction);
                transaction.Complete();
                return true;
            }
        }

        public bool Delete(IzinTanim item, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (!PortalLogSettings.IsEnabled(context)) return Delete(item);
            if (item == null || item.Id <= 0) return false;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                IzinTanim old = Map(repository.SelectById(item.Id, transaction, context));
                if (old == null || !repository.Delete(item.Id, transaction, context)) return false;
                int auditId = new PortalLogRepository().RecordAudit(context, "DELETE", "IzinTanim", item.Id.ToString(CultureInfo.InvariantCulture), transaction);
                RecordChanges(old, null, auditId, transaction);
                transaction.Complete();
                return true;
            }
        }

        private static void RecordChanges(IzinTanim old, IzinTanim current, int auditId, SqlTransactionContext transaction)
        {
            PortalLogRepository log = new PortalLogRepository();
            AddChange(log, auditId, "Adi", old == null ? null : old.Adi, current == null ? null : current.Adi, transaction);
        }

        private static void AddChange(PortalLogRepository log, int auditId, string field, string oldValue, string newValue, SqlTransactionContext transaction)
        {
            if (!string.Equals(oldValue, newValue, StringComparison.Ordinal))
                log.RecordAuditChange(auditId, field, oldValue, newValue, transaction);
        }


        private static IzinTanim Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static List<IzinTanim> ToList(DataTable table) { return new IzinTanim().ToList<IzinTanim>(table); }
    }
}
