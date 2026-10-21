using DAO.Repositories.TBYS;
using DAO.Ortak;
using DAO.Repositories.Ortak;
using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;
using Utility.Logging;

namespace Model.Services.TBYS
{
    public class VasiyeteKonuVarlikService
    {
        private readonly VasiyeteKonuVarlikRepository repository;
        private readonly PortalLogContext context;

        public VasiyeteKonuVarlikService() : this(new VasiyeteKonuVarlikRepository()) { }

        public VasiyeteKonuVarlikService(VasiyeteKonuVarlikRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public VasiyeteKonuVarlikService(PortalLogContext context) : this(new VasiyeteKonuVarlikRepository(context)) { this.context = context; }

        public VasiyeteKonuVarlik GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public VasiyeteKonuVarlik GetById(int id, PortalLogContext context) { return Map(new VasiyeteKonuVarlikRepository(context).SelectById(id)); }

        public List<VasiyeteKonuVarlik> GetAll()
        {
            return ToList(repository.SelectAll());
        }

        public List<VasiyeteKonuVarlik> GetByVasiyetciId(int vasiyetciId)
        {
            return ToList(repository.SelectByVasiyetciId(vasiyetciId));
        }

        public List<VasiyeteKonuVarlik> GetByVasiyetciId(int vasiyetciId, PortalLogContext context) { return ToList(new VasiyeteKonuVarlikRepository(context).SelectByVasiyetciId(vasiyetciId)); }

        public int Save(VasiyeteKonuVarlik item)
        {
            if (context != null) using (SqlTransactionContext tx = new SqlTransactionContext()) { int id = Save(item, tx); tx.Complete(); return id; }
            if (item == null) throw new ArgumentNullException("item");

            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);

            if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG)
            {
                OlayKayit olayKayit = new OlayKayit();
                olayKayit.GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_VASIYETEKONUVARLIK);
            }

            return item.Id;
        }

        public int Save(VasiyeteKonuVarlik item, SqlTransactionContext tx) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item, tx, context); if (item.Id > 0 && PortalLogSettings.IsEnabled(context)) new PortalLogRepository().RecordAudit(context, "INSERT", "VasiyeteKonuVarlik", item.Id.ToString(), tx); return item.Id; }

        public bool Update(VasiyeteKonuVarlik item)
        {
            if (context != null) using (SqlTransactionContext tx = new SqlTransactionContext()) { bool ok = Update(item, tx); if (ok) tx.Complete(); return ok; }
            if (item == null) throw new ArgumentNullException("item");

            VasiyeteKonuVarlik oldItem = GetById(item.Id);
            bool isSuccess = false;
            if (item.Id != 0)
            {
                item.DegistirmeTarihi = DateTime.Now;
                item.Degistiren = UtilityHelper.GetCurrentUserName();
                isSuccess = repository.Update(item);
            }

            if (isSuccess && ProjeConstants.TBYS_UPDATE_LOG)
            {
                OlayKayit olayKayit = new OlayKayit();
                olayKayit.GuncellemeOlayKaydet(item, oldItem, ProjeConstants.TBYS, ProjeConstants.TBYS_VASIYETEKONUVARLIK);
            }

            return isSuccess;
        }

        public bool Update(VasiyeteKonuVarlik item, SqlTransactionContext tx) { if (item == null || item.Id == 0) return false; item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); bool ok = repository.Update(item, tx, context); if (ok && PortalLogSettings.IsEnabled(context)) new PortalLogRepository().RecordAudit(context, "UPDATE", "VasiyeteKonuVarlik", item.Id.ToString(), tx); return ok; }

        public bool Delete(VasiyeteKonuVarlik item)
        {
            if (context != null) using (SqlTransactionContext tx = new SqlTransactionContext()) { bool ok = Delete(item, tx); if (ok) tx.Complete(); return ok; }
            if (item == null || item.Id == 0) return false;

            VasiyeteKonuVarlik oldItem = GetById(item.Id);
            if (oldItem == null) return false;

            bool isDeleted = repository.Delete(item.Id);
            if (isDeleted && ProjeConstants.TBYS_DELETE_LOG)
            {
                OlayKayit olayKayit = new OlayKayit();
                olayKayit.SilmeOlayKaydet(oldItem, ProjeConstants.TBYS, ProjeConstants.TBYS_VASIYETEKONUVARLIK);
            }

            return isDeleted;
        }

        public bool Delete(VasiyeteKonuVarlik item, SqlTransactionContext tx) { if (item == null || item.Id == 0) return false; bool ok = repository.Delete(item.Id, tx, context); if (ok && PortalLogSettings.IsEnabled(context)) new PortalLogRepository().RecordAudit(context, "DELETE", "VasiyeteKonuVarlik", item.Id.ToString(), tx); return ok; }

        private static List<VasiyeteKonuVarlik> ToList(DataTable table)
        {
            return new VasiyeteKonuVarlik().ToList<VasiyeteKonuVarlik>(table);
        }

        private static VasiyeteKonuVarlik Map(DataTable table)
        {
            return ToList(table).FirstOrDefault();
        }
    }
}
