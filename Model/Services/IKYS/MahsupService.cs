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
    public class MahsupService
    {
        private readonly MahsupRepository repository;
        public MahsupService() : this(new MahsupRepository()) { }
        public MahsupService(MahsupRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }
        public Mahsup GetById(int id) { return Map(repository.SelectById(id)); }
        public List<Mahsup> GetAll() { return ToList(repository.SelectAll()); }
        public int Save(Mahsup item)
        {
            if (item == null) throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);
            if (item.Id > 0 && ProjeConstants.IKYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_MAHSUP);
            return item.Id;
        }
        public bool Update(Mahsup item)
        {
            if (item == null) throw new ArgumentNullException("item");
            Mahsup old = GetById(item.Id);
            bool ok = false;
            if (item.Id != 0)
            {
                item.DegistirmeTarihi = DateTime.Now;
                item.Degistiren = UtilityHelper.GetCurrentUserName();
                ok = repository.Update(item);
            }
            if (ok && ProjeConstants.IKYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.IKYS, ProjeConstants.IKYS_MAHSUP);
            return ok;
        }
        public bool Delete(Mahsup item)
        {
            if (item == null || item.Id == 0) return false;
            Mahsup old = GetById(item.Id);
            if (old == null) return false;
            bool ok = repository.Delete(item.Id);
            if (ok && ProjeConstants.IKYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.IKYS, ProjeConstants.IKYS_MAHSUP);
            return ok;
        }
        public List<Mahsup> GetByDonemId(int donemId) { return ToList(repository.SelectByDonemId(donemId)); }
        public List<Mahsup> GetByDonemId(int donemId, PortalLogContext context) { return ToList(repository.SelectByDonemId(donemId, context)); }
        public List<Mahsup> GetByPersonelId(int personelId, int izinTipi) { return ToList(repository.SelectByPersonelId(personelId, izinTipi, ProjeConstants.IZINTIPI_MAZERET_INT)); }
        public List<Mahsup> GetByPersonelId(int personelId, int izinTipi, PortalLogContext context) { return ToList(repository.SelectByPersonelId(personelId, izinTipi, ProjeConstants.IZINTIPI_MAZERET_INT, context)); }
        public int Save(Mahsup item, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context"); if (!PortalLogSettings.IsEnabled(context)) return Save(item); if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName();
            using (SqlTransactionContext transaction = new SqlTransactionContext()) { item.Id = repository.Insert(item, transaction, context); if (item.Id <= 0) throw new InvalidOperationException("Mahsup kaydı oluşturulamadı."); int auditId = new PortalLogRepository().RecordAudit(context, "INSERT", "Mahsup", item.Id.ToString(CultureInfo.InvariantCulture), transaction); RecordChanges(null, item, auditId, transaction); transaction.Complete(); } return item.Id;
        }
        public bool Update(Mahsup item, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context"); if (!PortalLogSettings.IsEnabled(context)) return Update(item); if (item == null || item.Id <= 0) return false;
            using (SqlTransactionContext transaction = new SqlTransactionContext()) { Mahsup old = Map(repository.SelectById(item.Id, transaction, context)); if (old == null) return false; item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); if (!repository.Update(item, transaction, context)) return false; int auditId = new PortalLogRepository().RecordAudit(context, "UPDATE", "Mahsup", item.Id.ToString(CultureInfo.InvariantCulture), transaction); RecordChanges(old, item, auditId, transaction); transaction.Complete(); return true; }
        }
        public bool Delete(Mahsup item, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context"); if (!PortalLogSettings.IsEnabled(context)) return Delete(item); if (item == null || item.Id <= 0) return false;
            using (SqlTransactionContext transaction = new SqlTransactionContext()) { Mahsup old = Map(repository.SelectById(item.Id, transaction, context)); if (old == null || !repository.Delete(item.Id, transaction, context)) return false; int auditId = new PortalLogRepository().RecordAudit(context, "DELETE", "Mahsup", item.Id.ToString(CultureInfo.InvariantCulture), transaction); RecordChanges(old, null, auditId, transaction); transaction.Complete(); return true; }
        }
        private static void RecordChanges(Mahsup old, Mahsup current, int auditId, SqlTransactionContext transaction)
        { PortalLogRepository log = new PortalLogRepository(); AddChange(log, auditId, "PersonelId", old == null ? null : old.PersonelId.ToString(CultureInfo.InvariantCulture), current == null ? null : current.PersonelId.ToString(CultureInfo.InvariantCulture), transaction); AddChange(log, auditId, "IzinHareketId", old == null ? null : old.IzinHareketId.ToString(CultureInfo.InvariantCulture), current == null ? null : current.IzinHareketId.ToString(CultureInfo.InvariantCulture), transaction); AddChange(log, auditId, "IzinTipi", old == null ? null : old.IzinTipi.ToString(CultureInfo.InvariantCulture), current == null ? null : current.IzinTipi.ToString(CultureInfo.InvariantCulture), transaction); AddChange(log, auditId, "KullanildigiDonemId", old == null ? null : old.KullanildigiDonemId.ToString(CultureInfo.InvariantCulture), current == null ? null : current.KullanildigiDonemId.ToString(CultureInfo.InvariantCulture), transaction); AddChange(log, auditId, "MahsupDonemId", old == null ? null : old.MahsupDonemId.ToString(CultureInfo.InvariantCulture), current == null ? null : current.MahsupDonemId.ToString(CultureInfo.InvariantCulture), transaction); }
        private static void AddChange(PortalLogRepository log, int auditId, string field, string oldValue, string newValue, SqlTransactionContext transaction) { if (!string.Equals(oldValue, newValue, StringComparison.Ordinal)) log.RecordAuditChange(auditId, field, oldValue, newValue, transaction); }
        private static Mahsup Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static List<Mahsup> ToList(DataTable table) { return new Mahsup().ToList<Mahsup>(table); }
    }
}
