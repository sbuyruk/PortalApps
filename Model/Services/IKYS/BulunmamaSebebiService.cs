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
    public class BulunmamaSebebiService
    {
        private readonly BulunmamaSebebiRepository repository;
        public BulunmamaSebebiService() : this(new BulunmamaSebebiRepository()) { }
        public BulunmamaSebebiService(BulunmamaSebebiRepository repository) { if (repository == null) throw new ArgumentNullException("repository"); this.repository = repository; }
        public BulunmamaSebebi GetById(int id) { return Map(repository.SelectById(id)); }
        public List<BulunmamaSebebi> GetAll() { return ToList(repository.SelectAll()); }
        public List<BulunmamaSebebi> GetAll(PortalLogContext context) { return ToList(repository.SelectAll(context)); }
        public int Save(BulunmamaSebebi item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.IKYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_BULUNMAMASEBEBI); return item.Id; }
        public bool Update(BulunmamaSebebi item) { if (item == null) throw new ArgumentNullException("item"); BulunmamaSebebi old = GetById(item.Id); bool ok = false; if (item.Id != 0) { item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); ok = repository.Update(item); } if (ok && ProjeConstants.IKYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.IKYS, ProjeConstants.IKYS_BULUNMAMASEBEBI); return ok; }
        public bool Delete(BulunmamaSebebi item) { if (item == null || item.Id == 0) return false; BulunmamaSebebi old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.IKYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.IKYS, ProjeConstants.IKYS_BULUNMAMASEBEBI); return ok; }
        public int Save(BulunmamaSebebi item, PortalLogContext context) { if (context == null) throw new ArgumentNullException("context"); if (!PortalLogSettings.IsEnabled(context)) return Save(item); if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); using (SqlTransactionContext t = new SqlTransactionContext()) { item.Id = repository.Insert(item, t, context); if (item.Id <= 0) throw new InvalidOperationException("Bulunmama sebebi kaydı oluşturulamadı."); int a = new PortalLogRepository().RecordAudit(context, "INSERT", "BulunmamaSebebi", item.Id.ToString(CultureInfo.InvariantCulture), t); RecordChanges(null, item, a, t); t.Complete(); } return item.Id; }
        public bool Update(BulunmamaSebebi item, PortalLogContext context) { if (context == null) throw new ArgumentNullException("context"); if (!PortalLogSettings.IsEnabled(context)) return Update(item); if (item == null || item.Id <= 0) return false; using (SqlTransactionContext t = new SqlTransactionContext()) { BulunmamaSebebi old = Map(repository.SelectById(item.Id, t, context)); if (old == null) return false; item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); if (!repository.Update(item, t, context)) return false; int a = new PortalLogRepository().RecordAudit(context, "UPDATE", "BulunmamaSebebi", item.Id.ToString(CultureInfo.InvariantCulture), t); RecordChanges(old, item, a, t); t.Complete(); return true; } }
        public bool Delete(BulunmamaSebebi item, PortalLogContext context) { if (context == null) throw new ArgumentNullException("context"); if (!PortalLogSettings.IsEnabled(context)) return Delete(item); if (item == null || item.Id <= 0) return false; using (SqlTransactionContext t = new SqlTransactionContext()) { BulunmamaSebebi old = Map(repository.SelectById(item.Id, t, context)); if (old == null || !repository.Delete(item.Id, t, context)) return false; int a = new PortalLogRepository().RecordAudit(context, "DELETE", "BulunmamaSebebi", item.Id.ToString(CultureInfo.InvariantCulture), t); RecordChanges(old, null, a, t); t.Complete(); return true; } }
        private static void RecordChanges(BulunmamaSebebi old, BulunmamaSebebi current, int auditId, SqlTransactionContext t) { PortalLogRepository l = new PortalLogRepository(); Add(l, auditId, "Adi", old == null ? null : old.Adi, current == null ? null : current.Adi, t); }
        private static void Add(PortalLogRepository l, int a, string f, string o, string n, SqlTransactionContext t) { if (!string.Equals(o, n, StringComparison.Ordinal)) l.RecordAuditChange(a, f, o, n, t); }
        private static BulunmamaSebebi Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static List<BulunmamaSebebi> ToList(DataTable table) { return new BulunmamaSebebi().ToList<BulunmamaSebebi>(table); }
    }
}
