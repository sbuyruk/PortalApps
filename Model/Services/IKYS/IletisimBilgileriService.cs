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
using Utility.Logging;

namespace Model.Services.IKYS
{
    public class IletisimBilgileriService
    {
        private readonly IletisimBilgileriRepository repository;
        public IletisimBilgileriService() : this(new IletisimBilgileriRepository()) { }
        public IletisimBilgileriService(IletisimBilgileriRepository repository) { if (repository == null) throw new ArgumentNullException("repository"); this.repository = repository; }
        public IletisimBilgileri GetById(int id) { return Map(repository.SelectById(id)); }
        public List<IletisimBilgileri> GetAll() { return ToList(repository.SelectAll()); }

        public int Save(IletisimBilgileri item, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName();
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                item.Id = Save(item, transaction, context); transaction.Complete();
            }
            return item.Id;
        }
        public int Save(IletisimBilgileri item, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (context == null) throw new ArgumentNullException("context");
            if (item == null) throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item, transaction, context);
            if (item.Id <= 0) throw new InvalidOperationException("İletişim bilgileri kaydı oluşturulamadı.");
            int auditId = new PortalLogRepository().RecordAudit(context, "INSERT", "IletisimBilgileri", item.Id.ToString(CultureInfo.InvariantCulture), transaction);
            RecordChanges(null, item, auditId, transaction);
            return item.Id;
        }

        public bool Update(IletisimBilgileri item, PortalLogContext context)
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
        public bool Update(IletisimBilgileri item, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (context == null) throw new ArgumentNullException("context");
            if (item == null || item.Id <= 0) return false;
            IletisimBilgileri old = Map(repository.SelectById(item.Id, transaction, context)); item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName();
            if (old == null || !repository.Update(item, transaction, context)) return false;
            int auditId = new PortalLogRepository().RecordAudit(context, "UPDATE", "IletisimBilgileri", item.Id.ToString(CultureInfo.InvariantCulture), transaction);
            RecordChanges(old, item, auditId, transaction);
            return true;
        }

        public bool Delete(IletisimBilgileri item, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (item == null || item.Id <= 0) return false;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                IletisimBilgileri old = Map(repository.SelectById(item.Id, transaction, context)); if (old == null || !repository.Delete(item.Id, transaction, context)) return false;
                int auditId = new PortalLogRepository().RecordAudit(context, "DELETE", "IletisimBilgileri", item.Id.ToString(CultureInfo.InvariantCulture), transaction);
                RecordChanges(old, null, auditId, transaction); transaction.Complete(); return true;
            }
        }
        public IletisimBilgileri GetByPersonelId(int personelId) { return Map(repository.SelectByPersonelId(personelId)); }
        public IletisimBilgileri GetByPersonelId(int personelId, PortalLogContext context)
        { return Map(repository.SelectByPersonelId(personelId, context)); }
        public IletisimBilgileri GetByPersonelId(int personelId, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (context == null) throw new ArgumentNullException("context");
            return Map(repository.SelectByPersonelId(personelId, transaction, context));
        }
        private static void RecordChanges(IletisimBilgileri old, IletisimBilgileri current, int auditId, SqlTransactionContext transaction)
        {
            if (auditId == 0) return;
            PortalLogRepository log = new PortalLogRepository();
            AddChange(log, auditId, "PersonelId", old == null ? null : old.PersonelId.ToString(CultureInfo.InvariantCulture), current == null ? null : current.PersonelId.ToString(CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "Ilcesi", old == null ? null : old.Ilcesi.ToString(CultureInfo.InvariantCulture), current == null ? null : current.Ilcesi.ToString(CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "Adres", old == null ? null : Convert.ToString(old.Adres, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.Adres, CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "Semt", old == null ? null : Convert.ToString(old.Semt, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.Semt, CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "Ili", old == null ? null : Convert.ToString(old.Ili, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.Ili, CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "PostaKodu", old == null ? null : Convert.ToString(old.PostaKodu, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.PostaKodu, CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "DahiliTelefonu", old == null ? null : Convert.ToString(old.DahiliTelefonu, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.DahiliTelefonu, CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "EvTelefonu", old == null ? null : Convert.ToString(old.EvTelefonu, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.EvTelefonu, CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "CepTelefonu", old == null ? null : Convert.ToString(old.CepTelefonu, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.CepTelefonu, CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "CepTelefonu2", old == null ? null : Convert.ToString(old.CepTelefonu2, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.CepTelefonu2, CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "IntranetEPosta", old == null ? null : Convert.ToString(old.IntranetEPosta, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.IntranetEPosta, CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "InternetEPosta", old == null ? null : Convert.ToString(old.InternetEPosta, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.InternetEPosta, CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "OzelEPosta", old == null ? null : Convert.ToString(old.OzelEPosta, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.OzelEPosta, CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "Plaka", old == null ? null : Convert.ToString(old.Plaka, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.Plaka, CultureInfo.InvariantCulture), transaction);
        }
        private static void AddChange(PortalLogRepository log, int auditId, string field, string oldValue, string newValue, SqlTransactionContext transaction) { if (!string.Equals(oldValue, newValue, StringComparison.Ordinal)) log.RecordAuditChange(auditId, field, oldValue, newValue, transaction); }
        private static IletisimBilgileri Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static List<IletisimBilgileri> ToList(DataTable table) { return new IletisimBilgileri().ToList<IletisimBilgileri>(table); }

        public IletisimBilgileri GetById(int id, PortalLogContext context)
        { return Map(repository.SelectById(id, context)); }

        public List<IletisimBilgileri> GetAll(PortalLogContext context)
        { return ToList(repository.SelectAll(context)); }
    }
}
