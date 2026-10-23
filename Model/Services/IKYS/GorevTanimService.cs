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
using Utility.HelperClasses;
using Utility.Logging;
using static Model.IKYS.Personel;

namespace Model.Services.IKYS
{
    public class GorevTanimService
    {
        private readonly GorevTanimRepository repository;
        public GorevTanimService() : this(new GorevTanimRepository()) { }
        public GorevTanimService(GorevTanimRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }
        public GorevTanim GetById(int id) { return Map(repository.SelectById(id)); }
        public GorevTanim GetById(int id, PortalLogContext context)
        { return Map(repository.SelectById(id, context)); }
        public GorevTanim GetById(int id, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (context == null) throw new ArgumentNullException("context");
            return Map(repository.SelectById(id, transaction, context));
        }
        public List<GorevTanim> GetAll() { return ToList(repository.SelectAll()); }
        public List<GorevTanim> GetAll(PortalLogContext context)
        { return ToList(repository.SelectAll(context)); }
        public bool TryRecordWebPartOpened(PortalLogContext context) { return new PortalLogRepository().TryRecordWebPartOpened(context); }

        public int Save(GorevTanim item, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (item == null) throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                item.Id = repository.Insert(item, transaction, context);
                if (item.Id <= 0) throw new InvalidOperationException("Görev kaydı oluşturulamadı.");
                int auditId = new PortalLogRepository().RecordAudit(context, "INSERT", "GorevTanim", item.Id.ToString(CultureInfo.InvariantCulture), transaction);
                RecordChanges(null, item, auditId, transaction);
                transaction.Complete();
            }
            return item.Id;
        }

        public bool Update(GorevTanim item, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (item == null || item.Id <= 0) return false;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                GorevTanim old = Map(repository.SelectById(item.Id, transaction, context));
                if (old == null) return false;
                item.DegistirmeTarihi = DateTime.Now;
                item.Degistiren = UtilityHelper.GetCurrentUserName();
                if (!repository.Update(item, transaction, context)) return false;
                PortalLogRepository log = new PortalLogRepository();
                int auditId = log.RecordAudit(context, "UPDATE", "GorevTanim", item.Id.ToString(CultureInfo.InvariantCulture), transaction);
                RecordChanges(old, item, auditId, transaction);

                IsBilgileriRepository workRepository = new IsBilgileriRepository();
                if (old.PersonelId != item.PersonelId)
                    ChangeAssignment(workRepository, old.PersonelId, 0, transaction, context, log);
                ChangeAssignment(workRepository, item.PersonelId, item.Id, transaction, context, log);
                transaction.Complete();
                return true;
            }
        }
        public bool Update(GorevTanim item, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (context == null) throw new ArgumentNullException("context");
            if (item == null || item.Id <= 0) return false;
            GorevTanim old = Map(repository.SelectById(item.Id, transaction, context));
            if (old == null) return false;
            item.DegistirmeTarihi = DateTime.Now;
            item.Degistiren = UtilityHelper.GetCurrentUserName();
            if (!repository.Update(item, transaction, context)) return false;
            PortalLogRepository log = new PortalLogRepository();
            int auditId = log.RecordAudit(context, "UPDATE", "GorevTanim", item.Id.ToString(CultureInfo.InvariantCulture), transaction);
            RecordChanges(old, item, auditId, transaction);
            IsBilgileriRepository workRepository = new IsBilgileriRepository();
            if (old.PersonelId != item.PersonelId)
                ChangeAssignment(workRepository, old.PersonelId, 0, transaction, context, log);
            ChangeAssignment(workRepository, item.PersonelId, item.Id, transaction, context, log);
            return true;
        }

        public bool Delete(GorevTanim item, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (item == null || item.Id <= 0) return false;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                GorevTanim old = Map(repository.SelectById(item.Id, transaction, context));
                if (old == null) return false;
                IsBilgileriRepository workRepository = new IsBilgileriRepository();
                if (workRepository.SelectByGorevId(item.Id, transaction, context) != null) return false;
                if (!repository.Delete(item.Id, transaction, context)) return false;
                int auditId = new PortalLogRepository().RecordAudit(context, "DELETE", "GorevTanim", item.Id.ToString(CultureInfo.InvariantCulture), transaction);
                RecordChanges(old, null, auditId, transaction);
                transaction.Complete();
                return true;
            }
        }
        public GorevTanim GetByPersonelId(int personelId) { return Map(repository.SelectByPersonelId(personelId)); }
        public GorevTanim GetByPersonelId(int personelId, PortalLogContext context)
        { return Map(repository.SelectByPersonelId(personelId, context)); }
        public GorevTanim GetByPersonelId(int personelId, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (context == null) throw new ArgumentNullException("context");
            return Map(repository.SelectByPersonelId(personelId, transaction, context));
        }
        public List<GorevTanim> GetByBirimId(int birimId) { return ToList(repository.SelectByBirimId(birimId)); }
        public GorevTanim GetByGorevId(int gorevId) { return GetById(gorevId); }
        public GorevTanim GetByGorevId(int gorevId, PortalLogContext context)
        { return GetById(gorevId, context); }
        public DataTable GetAllReturnDataTable(PersonelTipi personelTipi) { return repository.SelectAllReturnDataTable(personelTipi == PersonelTipi.Tumu ? (int?)null : (int)personelTipi); }
        public DataTable GetAllReturnDataTable(PersonelTipi personelTipi, PortalLogContext context)
        { return repository.SelectAllReturnDataTable(personelTipi == PersonelTipi.Tumu ? (int?)null : (int)personelTipi, context); }
        private static void ChangeAssignment(IsBilgileriRepository workRepository, int personelId, int gorevId,
            SqlTransactionContext transaction, PortalLogContext context, PortalLogRepository log)
        {
            if (personelId <= 0) return;
            IsBilgileri work = new IsBilgileri().ToList<IsBilgileri>(workRepository.SelectByPersonelId(personelId, transaction, context)).FirstOrDefault();
            if (work == null || work.GorevId == gorevId) return;
            int oldGorevId = work.GorevId;
            if (!workRepository.UpdateGorevId(work.Id, gorevId, UtilityHelper.GetCurrentUserName(), transaction, context))
                throw new InvalidOperationException("Personel görev bilgisi güncellenemedi.");
            int auditId = log.RecordAudit(context, "UPDATE", "IsBilgileri", work.Id.ToString(CultureInfo.InvariantCulture), transaction);
            if (auditId == 0) return;
            log.RecordAuditChange(auditId, "GorevId", oldGorevId.ToString(CultureInfo.InvariantCulture), gorevId.ToString(CultureInfo.InvariantCulture), transaction);
        }
        private static void RecordChanges(GorevTanim old, GorevTanim current, int auditId, SqlTransactionContext transaction)
        {
            if (auditId == 0) return;
            PortalLogRepository log = new PortalLogRepository();
            AddChange(log, auditId, "BirimId", old == null ? null : old.BirimId.ToString(CultureInfo.InvariantCulture), current == null ? null : current.BirimId.ToString(CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "PersonelId", old == null ? null : old.PersonelId.ToString(CultureInfo.InvariantCulture), current == null ? null : current.PersonelId.ToString(CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "Adi", old == null ? null : old.Adi, current == null ? null : current.Adi, transaction);
            AddChange(log, auditId, "KisaAdi", old == null ? null : old.KisaAdi, current == null ? null : current.KisaAdi, transaction);
            AddChange(log, auditId, "Vekil", old == null ? null : old.Vekil.ToString(), current == null ? null : current.Vekil.ToString(), transaction);
            AddChange(log, auditId, "Aktif", old == null ? null : old.Aktif.ToString(), current == null ? null : current.Aktif.ToString(), transaction);
            AddChange(log, auditId, "HarcirahGrupId", old == null ? null : old.HarcirahGrupId.ToString(CultureInfo.InvariantCulture), current == null ? null : current.HarcirahGrupId.ToString(CultureInfo.InvariantCulture), transaction);
        }
        private static void AddChange(PortalLogRepository log, int auditId, string field, string oldValue, string newValue, SqlTransactionContext transaction)
        {
            if (!string.Equals(oldValue, newValue, StringComparison.Ordinal))
                log.RecordAuditChange(auditId, field, oldValue, newValue, transaction);
        }
        private static GorevTanim Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static List<GorevTanim> ToList(DataTable table) { return new GorevTanim().ToList<GorevTanim>(table); }

        public List<GorevTanim> GetByBirimId(int birimId, PortalLogContext context)
        { return ToList(repository.SelectByBirimId(birimId, context)); }
    }
}
