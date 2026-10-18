using DAO.Repositories.IKYS;
using DAO.Repositories.Ortak;
using DAO.Ortak;
using Model.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.ProjeGlobal;
using Utility.Logging;
using System.Globalization;

namespace Model.Services.IKYS
{
    public class IsBilgileriService
    {
        private readonly IsBilgileriRepository repository;
        public IsBilgileriService() : this(new IsBilgileriRepository()) { }
        public IsBilgileriService(IsBilgileriRepository repository) { if (repository == null) throw new ArgumentNullException("repository"); this.repository = repository; }
        public IsBilgileri GetById(int id) { return Map(repository.SelectById(id)); }
        public IsBilgileri GetById(int id, SqlTransactionContext transaction, PortalLogContext context) { if (transaction == null) throw new ArgumentNullException("transaction"); if (context == null) throw new ArgumentNullException("context"); return Map(repository.SelectById(id, transaction, context)); }
        public List<IsBilgileri> GetAll() { return ToList(repository.SelectAll()); }
        public int Save(IsBilgileri item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.IKYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_ISBILGILERI); return item.Id; }
        public int Save(IsBilgileri item, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context"); if (!PortalLogSettings.IsEnabled(context)) return Save(item);
            if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName();
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                item.Id = repository.Insert(item, transaction, context); if (item.Id <= 0) throw new InvalidOperationException("İş bilgileri kaydı oluşturulamadı.");
                int auditId = new PortalLogRepository().RecordAudit(context, "INSERT", "IsBilgileri", item.Id.ToString(CultureInfo.InvariantCulture), transaction);
                RecordChanges(null, item, auditId, transaction); transaction.Complete();
            }
            return item.Id;
        }
        public bool Update(IsBilgileri item) { if (item == null) throw new ArgumentNullException("item"); IsBilgileri old = GetById(item.Id); bool ok = false; if (item.Id != 0) { item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); ok = repository.Update(item); } if (ok && ProjeConstants.IKYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.IKYS, ProjeConstants.IKYS_ISBILGILERI); return ok; }
        public bool Update(IsBilgileri item, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context"); if (!PortalLogSettings.IsEnabled(context)) return Update(item);
            if (item == null || item.Id <= 0) return false;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                IsBilgileri old = Map(repository.SelectById(item.Id, transaction, context)); item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName();
                if (old == null || !repository.Update(item, transaction, context)) return false;
                int auditId = new PortalLogRepository().RecordAudit(context, "UPDATE", "IsBilgileri", item.Id.ToString(CultureInfo.InvariantCulture), transaction);
                RecordChanges(old, item, auditId, transaction); transaction.Complete(); return true;
            }
        }
        public bool Update(IsBilgileri item, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (context == null) throw new ArgumentNullException("context");
            if (item == null || item.Id <= 0) return false;
            IsBilgileri old = Map(repository.SelectById(item.Id, transaction, context));
            if (old == null) return false;
            item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName();
            if (!repository.Update(item, transaction, context)) return false;
            int auditId = new PortalLogRepository().RecordAudit(context, "UPDATE", "IsBilgileri", item.Id.ToString(CultureInfo.InvariantCulture), transaction);
            RecordChanges(old, item, auditId, transaction);
            return true;
        }
        public bool Delete(IsBilgileri item) { if (item == null || item.Id == 0) return false; IsBilgileri old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.IKYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.IKYS, ProjeConstants.IKYS_ISBILGILERI); return ok; }
        public bool Delete(IsBilgileri item, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context"); if (!PortalLogSettings.IsEnabled(context)) return Delete(item);
            if (item == null || item.Id <= 0) return false;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                IsBilgileri old = Map(repository.SelectById(item.Id, transaction, context)); if (old == null || !repository.Delete(item.Id, transaction, context)) return false;
                int auditId = new PortalLogRepository().RecordAudit(context, "DELETE", "IsBilgileri", item.Id.ToString(CultureInfo.InvariantCulture), transaction);
                RecordChanges(old, null, auditId, transaction); transaction.Complete(); return true;
            }
        }
        public IsBilgileri GetByPersonelId(int personelId) { return Map(repository.SelectByPersonelId(personelId)); }
        public IsBilgileri GetByPersonelId(int personelId, PortalLogContext context) { return Map(repository.SelectByPersonelId(personelId, context)); }
        public IsBilgileri GetByPersonelId(int personelId, SqlTransactionContext transaction, PortalLogContext context) { if (transaction == null) throw new ArgumentNullException("transaction"); if (context == null) throw new ArgumentNullException("context"); return Map(repository.SelectByPersonelId(personelId, transaction, context)); }
        public IsBilgileri GetByGorevId(int gorevId) { return Map(repository.SelectByGorevId(gorevId)); }
        public IsBilgileri GetByGorevId(int gorevId, PortalLogContext context) { return Map(repository.SelectByGorevId(gorevId, context)); }
        public DataTable GetAllFromIsYeriBilgileri() { return repository.SelectAllFromIsYeriBilgileri(); }
        private static void RecordChanges(IsBilgileri old, IsBilgileri current, int auditId, SqlTransactionContext transaction)
        {
            PortalLogRepository log = new PortalLogRepository();
            AddChange(log, auditId, "PersonelId", old == null ? null : old.PersonelId.ToString(CultureInfo.InvariantCulture), current == null ? null : current.PersonelId.ToString(CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "UnvanId", old == null ? null : old.UnvanId.ToString(CultureInfo.InvariantCulture), current == null ? null : current.UnvanId.ToString(CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "GorevId", old == null ? null : old.GorevId.ToString(CultureInfo.InvariantCulture), current == null ? null : current.GorevId.ToString(CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "BirimId", old == null ? null : old.BirimId.ToString(CultureInfo.InvariantCulture), current == null ? null : current.BirimId.ToString(CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "CalismaDurumu", old == null ? null : old.CalismaDurumu.ToString(CultureInfo.InvariantCulture), current == null ? null : current.CalismaDurumu.ToString(CultureInfo.InvariantCulture), transaction);
        }
        private static void AddChange(PortalLogRepository log, int auditId, string field, string oldValue, string newValue, SqlTransactionContext transaction) { if (!string.Equals(oldValue, newValue, StringComparison.Ordinal)) log.RecordAuditChange(auditId, field, oldValue, newValue, transaction); }
        private static IsBilgileri Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static List<IsBilgileri> ToList(DataTable table) { return new IsBilgileri().ToList<IsBilgileri>(table); }
    }
}
