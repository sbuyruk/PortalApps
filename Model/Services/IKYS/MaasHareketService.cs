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
using Utility.ProjeGlobal;
using Utility.Logging;

namespace Model.Services.IKYS
{
    public class MaasHareketService
    {
        private readonly MaasHareketRepository repository;
        public MaasHareketService() : this(new MaasHareketRepository()) { }
        public MaasHareketService(MaasHareketRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }
        public MaasHareket GetById(int id) { return Map(repository.SelectById(id)); }
        public List<MaasHareket> GetAll() { return ToList(repository.SelectAll()); }
        public int Save(MaasHareket item)
        {
            if (item == null) throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);
            if (item.Id > 0 && ProjeConstants.IKYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_GOREVONAY);
            return item.Id;
        }
        public bool Update(MaasHareket item)
        {
            if (item == null) throw new ArgumentNullException("item");
            MaasHareket old = GetById(item.Id);
            bool ok = false;
            if (item.Id != 0)
            {
                item.DegistirmeTarihi = DateTime.Now;
                item.Degistiren = UtilityHelper.GetCurrentUserName();
                ok = repository.Update(item);
            }
            if (ok && ProjeConstants.IKYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.IKYS, ProjeConstants.IKYS_GOREVONAY);
            return ok;
        }
        public bool Delete(MaasHareket item)
        {
            if (item == null || item.Id == 0) return false;
            MaasHareket old = GetById(item.Id);
            if (old == null) return false;
            bool ok = repository.Delete(item.Id);
            if (ok && ProjeConstants.IKYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.IKYS, ProjeConstants.IKYS_GOREVONAY);
            return ok;
        }
        public MaasHareket GetByTarih(DateTime tarih) { return Map(repository.SelectByTarih(tarih)); }
        public MaasHareket GetByTarih(DateTime tarih, PortalLogContext context) { return Map(repository.SelectByTarih(tarih, context)); }
        public List<MaasHareket> GetMaasListesiByTarih(DateTime tarih) { return ToList(repository.SelectByTarih(tarih)); }
        public List<MaasHareket> GetMaasListesiByTarih(DateTime tarih, PortalLogContext context) { return ToList(repository.SelectByTarih(tarih, context)); }
        public bool DeleteByGrupId(MaasHareket item, int grupId)
        {
            bool ok = repository.DeleteByGrupId(grupId);
            if (ok && ProjeConstants.IKYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_GOREVONAY);
            return ok;
        }
        public int Save(MaasHareket item, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context"); if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName();
            using (SqlTransactionContext transaction = new SqlTransactionContext()) { item.Id = repository.Insert(item, transaction, context); if (item.Id <= 0) throw new InvalidOperationException("Maaş hareketi kaydı oluşturulamadı."); int auditId = new PortalLogRepository().RecordAudit(context, "INSERT", "MaasHareket", item.Id.ToString(CultureInfo.InvariantCulture), transaction); RecordChanges(null, item, auditId, transaction); transaction.Complete(); } return item.Id;
        }
        public int Save(MaasHareket item, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (context == null) throw new ArgumentNullException("context");
            if (item == null) throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item, transaction, context);
            if (item.Id <= 0) throw new InvalidOperationException("Maaş hareketi kaydı oluşturulamadı.");
            int auditId = new PortalLogRepository().RecordAudit(context, "INSERT", "MaasHareket", item.Id.ToString(CultureInfo.InvariantCulture), transaction);
            RecordChanges(null, item, auditId, transaction);
            return item.Id;
        }
        public bool Update(MaasHareket item, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context"); if (item == null || item.Id <= 0) return false;
            using (SqlTransactionContext transaction = new SqlTransactionContext()) { MaasHareket old = Map(repository.SelectById(item.Id, transaction, context)); if (old == null) return false; item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); if (!repository.Update(item, transaction, context)) return false; int auditId = new PortalLogRepository().RecordAudit(context, "UPDATE", "MaasHareket", item.Id.ToString(CultureInfo.InvariantCulture), transaction); RecordChanges(old, item, auditId, transaction); transaction.Complete(); return true; }
        }
        public bool Delete(MaasHareket item, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context"); if (item == null || item.Id <= 0) return false;
            using (SqlTransactionContext transaction = new SqlTransactionContext()) { MaasHareket old = Map(repository.SelectById(item.Id, transaction, context)); if (old == null || !repository.Delete(item.Id, transaction, context)) return false; int auditId = new PortalLogRepository().RecordAudit(context, "DELETE", "MaasHareket", item.Id.ToString(CultureInfo.InvariantCulture), transaction); RecordChanges(old, null, auditId, transaction); transaction.Complete(); return true; }
        }
        public bool DeleteByGrupId(MaasHareket item, int grupId, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            using (SqlTransactionContext transaction = new SqlTransactionContext()) { if (!repository.DeleteByGrupId(grupId, transaction, context)) return false; int auditId = new PortalLogRepository().RecordAudit(context, "DELETE", "MaasHareket", grupId.ToString(CultureInfo.InvariantCulture), transaction); transaction.Complete(); return true; }
        }
        private static void RecordChanges(MaasHareket old, MaasHareket current, int auditId, SqlTransactionContext transaction)
        { if (auditId == 0) return; PortalLogRepository log = new PortalLogRepository(); AddChange(log, auditId, "PersonelId", old == null ? null : old.PersonelId.ToString(CultureInfo.InvariantCulture), current == null ? null : current.PersonelId.ToString(CultureInfo.InvariantCulture), transaction); AddChange(log, auditId, "Tarih", old == null ? null : AuditDate(old.Tarih), current == null ? null : AuditDate(current.Tarih), transaction); AddChange(log, auditId, "Derece", old == null ? null : old.Derece.ToString(CultureInfo.InvariantCulture), current == null ? null : current.Derece.ToString(CultureInfo.InvariantCulture), transaction); AddChange(log, auditId, "Kademe", old == null ? null : old.Kademe.ToString(CultureInfo.InvariantCulture), current == null ? null : current.Kademe.ToString(CultureInfo.InvariantCulture), transaction); AddChange(log, auditId, "GrupId", old == null ? null : old.GrupId.ToString(CultureInfo.InvariantCulture), current == null ? null : current.GrupId.ToString(CultureInfo.InvariantCulture), transaction); AddChange(log, auditId, "ProtokolSiraNo", old == null ? null : old.ProtokolSiraNo.ToString(CultureInfo.InvariantCulture), current == null ? null : current.ProtokolSiraNo.ToString(CultureInfo.InvariantCulture), transaction); }
        private static string AuditDate(DateTime value) { return value < new DateTime(1901, 1, 1) ? null : value.ToString("o", CultureInfo.InvariantCulture); }
        private static void AddChange(PortalLogRepository log, int auditId, string field, string oldValue, string newValue, SqlTransactionContext transaction) { if (!string.Equals(oldValue, newValue, StringComparison.Ordinal)) log.RecordAuditChange(auditId, field, oldValue, newValue, transaction); }
        private static MaasHareket Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static List<MaasHareket> ToList(DataTable table) { return new MaasHareket().ToList<MaasHareket>(table); }
    }
}
