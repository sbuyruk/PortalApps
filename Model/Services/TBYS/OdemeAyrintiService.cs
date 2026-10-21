using DAO.Repositories.TBYS;
using DAO.Ortak;
using DAO.Repositories.Ortak;
using Utility.Logging;
using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.Services.TBYS
{
    public class OdemeAyrintiService
    {
        private readonly OdemeAyrintiRepository repository;
        private readonly PortalLogContext context;
        public OdemeAyrintiService() : this(new OdemeAyrintiRepository()) { }
        public OdemeAyrintiService(OdemeAyrintiRepository repository) { if (repository == null) throw new ArgumentNullException("repository"); this.repository = repository; }
        public OdemeAyrintiService(PortalLogContext context) : this(new OdemeAyrintiRepository(context)) { this.context = context; }
        public OdemeAyrinti GetById(int id) { return Map(repository.SelectById(id)); }
        public List<OdemeAyrinti> GetAll() { return ToList(repository.SelectAll()); }
        public List<OdemeAyrinti> GetBySozlesmeId(int id) { return ToList(repository.SelectBySozlesmeId(id)); }
        public List<OdemeAyrinti> GetByOdemeIdOdemePlaniId(int odemeId, int planId) { return ToList(repository.SelectByOdemeIdOdemePlaniId(odemeId, planId)); }
        public OdemeAyrinti GetByPlanAndDelay(KiraSozlesme sozlesme, int planId, int delayId, int odemeId) { return Map(repository.SelectByPlanAndDelay(sozlesme.KiraciId, sozlesme.Id, planId, delayId, odemeId)); }
        public List<OdemeAyrinti> GetBySozlesme(KiraSozlesme sozlesme) { return GetBySozlesmeId(sozlesme.Id); }
        public List<OdemeAyrinti> GetByPlan(OdemePlani plan) { return ToList(repository.SelectByPlan(plan.Id)); }
        public decimal GetLastAnaPara(int planId) { DataTable t = repository.SelectLastAnaPara(planId); return t != null && t.Rows.Count > 0 ? t.Rows[0]["AnaPara"].ReturnZeroIfNull().ConvertToDecimal() : 0; }
        public decimal GetSumDelayAmount(int planId) { DataTable t = repository.SelectSumDelayAmount(planId); return t != null && t.Rows.Count > 0 ? t.Rows[0]["Toplam"].ReturnZeroIfNull().ConvertToDecimal() : 0; }
        public decimal GetLastDelayRate(int planId) { return ToList(repository.SelectLastDelayRate(planId)).LastOrDefault()?.GecikmeZammiOrani ?? 0; }
        public int Save(OdemeAyrinti item) { if (context != null) using (SqlTransactionContext tx = new SqlTransactionContext()) { int id = Save(item, tx); tx.Complete(); return id; } if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEMEAYRINTI); return item.Id; }
        public int Save(OdemeAyrinti item, SqlTransactionContext tx) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item, tx, context); if (item.Id > 0) Audit("INSERT", item.Id, tx); return item.Id; }
        public bool Update(OdemeAyrinti item) { if (context != null) using (SqlTransactionContext tx = new SqlTransactionContext()) { bool ok = Update(item, tx); if (ok) tx.Complete(); return ok; } if (item == null) throw new ArgumentNullException("item"); OdemeAyrinti old = GetById(item.Id); bool result = false; if (item.Id != 0) { item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); result = repository.Update(item); } if (result && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEMEAYRINTI); return result; }
        public bool Update(OdemeAyrinti item, SqlTransactionContext tx) { if (item == null || item.Id == 0) return false; item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); bool ok = repository.Update(item, tx, context); if (ok) Audit("UPDATE", item.Id, tx); return ok; }
        public bool Delete(OdemeAyrinti item) { if (context != null) using (SqlTransactionContext tx = new SqlTransactionContext()) { bool ok = Delete(item, tx); if (ok) tx.Complete(); return ok; } if (item == null || item.Id == 0) return false; OdemeAyrinti old = GetById(item.Id); if (old == null) return false; bool result = repository.Delete(item.Id); if (result && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEMEAYRINTI); return result; }
        public bool Delete(OdemeAyrinti item, SqlTransactionContext tx) { if (item == null || item.Id == 0) return false; bool ok = repository.Delete(item.Id, tx, context); if (ok) Audit("DELETE", item.Id, tx); return ok; }
        private void Audit(string operation, int id, SqlTransactionContext tx) { if (PortalLogSettings.IsEnabled(context)) new PortalLogRepository().RecordAudit(context, operation, "OdemeAyrinti", id.ToString(), tx); }
        public bool DeleteBySozlesmeId(int id) { if (context != null) using (SqlTransactionContext tx = new SqlTransactionContext()) { bool ok = DeleteBySozlesmeId(id, tx); if (ok) tx.Complete(); return ok; } return repository.DeleteBySozlesmeId(id); }
        public bool DeleteBySozlesmeId(int id, SqlTransactionContext tx) { return repository.DeleteBySozlesmeId(id, tx, context); }
        public bool DeleteByOdemeIdOdemePlaniId(int odemeId, int planId) { return repository.DeleteByOdemeIdOdemePlaniId(odemeId, planId); }
        private static List<OdemeAyrinti> ToList(DataTable t) { return new OdemeAyrinti().ToList<OdemeAyrinti>(t); }
        private static OdemeAyrinti Map(DataTable t) { return ToList(t).FirstOrDefault(); }
    }
}
