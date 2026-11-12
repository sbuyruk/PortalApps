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
    public class TeminatIslemService
    {
        private readonly TeminatIslemRepository repository;
        private readonly PortalLogContext context;
        public TeminatIslemService() : this(new TeminatIslemRepository()) { }
        public TeminatIslemService(TeminatIslemRepository repository) { if (repository == null) throw new ArgumentNullException("repository"); this.repository = repository; }
        public TeminatIslemService(PortalLogContext context) : this(new TeminatIslemRepository(context)) { this.context = context; }
        public TeminatIslem GetById(int id) {
            try
            {
                return Map(repository.SelectById(id));
            }
            catch (Exception error)
            {
                PortalLogContext.AttachEntityToError(context, "TeminatIslem", id, error);
                throw;
            }
        }
        public TeminatIslem GetById(int id, SqlTransactionContext transaction) {
            try
            {
                return Map(repository.SelectById(id, transaction, context));
            }
            catch (Exception error)
            {
                PortalLogContext.AttachEntityToError(context, "TeminatIslem", id, error);
                throw;
            }
        }
        public List<TeminatIslem> GetAll() { return ToList(repository.SelectAll()); }
        public List<TeminatIslem> GetBySozlesmeId(int id) {
            try
            {
                return ToList(repository.SelectBySozlesmeId(id));
            }
            catch (Exception error)
            {
                PortalLogContext.AttachEntityToError(context, "KiraSozlesme", id, error);
                throw;
            }
        }
        public List<TeminatIslem> GetByKiraciId(int id) {
            try
            {
                return ToList(repository.SelectByKiraciId(id));
            }
            catch (Exception error)
            {
                PortalLogContext.AttachEntityToError(context, "Kiraci", id, error);
                throw;
            }
        }
        public List<TeminatIslem> GetByOdemeId(int id) {
            try
            {
                return ToList(repository.SelectByOdemeId(id));
            }
            catch (Exception error)
            {
                PortalLogContext.AttachEntityToError(context, "Odeme", id, error);
                throw;
            }
        }
        public List<TeminatIslem> GetByOdemeId(int id, SqlTransactionContext transaction) {
            try
            {
                return ToList(repository.SelectByOdemeId(id, transaction, context));
            }
            catch (Exception error)
            {
                PortalLogContext.AttachEntityToError(context, "Odeme", id, error);
                throw;
            }
        }
        public decimal GetSumPaidByKiraciId(int id) {
            try
            {
                DataTable t = repository.SelectSumPaidByKiraciId(id); return t != null && t.Rows.Count > 0 ? t.Rows[0]["Toplam"].ReturnZeroIfNull().ConvertToDecimal() : 0;
            }
            catch (Exception error)
            {
                PortalLogContext.AttachEntityToError(context, "Kiraci", id, error);
                throw;
            }
        }
        public DataTable GetSumByKiraciIdGroupByType(int id) {
            try
            {
                return repository.SelectSumByKiraciIdGroupByType(id);
            }
            catch (Exception error)
            {
                PortalLogContext.AttachEntityToError(context, "Kiraci", id, error);
                throw;
            }
        }
        public DataTable GetSumByKiraciIdGroupByType(int id, SqlTransactionContext transaction) {
            try
            {
                return repository.SelectSumByKiraciIdGroupByType(id, transaction, context);
            }
            catch (Exception error)
            {
                PortalLogContext.AttachEntityToError(context, "Kiraci", id, error);
                throw;
            }
        }
        public int Save(TeminatIslem item)
        {
            try
            {
                if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
                using (SqlTransactionContext tx = new SqlTransactionContext()) { int id = Save(item, tx); tx.Complete(); return id; }
            }
            catch (Exception error)
            {
                PortalLogContext.AttachEntityToError(context, item != null && item.Id > 0 ? "TeminatIslem" : "Kiraci", item == null ? 0 : (item.Id > 0 ? item.Id : item.KiraciId), error);
                throw;
            }
        }
        public int Save(TeminatIslem item, SqlTransactionContext tx) {
            try
            {
                if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item, tx, context); if (item.Id > 0 && PortalLogSettings.IsEnabled(context)) new PortalLogRepository().RecordAudit(context, "INSERT", "TeminatIslem", item.Id.ToString(), tx); return item.Id;
            }
            catch (Exception error)
            {
                PortalLogContext.AttachEntityToError(context, item != null && item.Id > 0 ? "TeminatIslem" : "Kiraci", item == null ? 0 : (item.Id > 0 ? item.Id : item.KiraciId), error);
                throw;
            }
        }
        public bool Update(TeminatIslem item)
        {
            try
            {
                if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
                using (SqlTransactionContext tx = new SqlTransactionContext()) { bool ok = Update(item, tx); if (ok) tx.Complete(); return ok; }
            }
            catch (Exception error)
            {
                PortalLogContext.AttachEntityToError(context, "TeminatIslem", item == null ? 0 : item.Id, error);
                throw;
            }
        }
        public bool Update(TeminatIslem item, SqlTransactionContext tx) {
            try
            {
                if (item == null || item.Id == 0) return false; item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); bool ok = repository.Update(item, tx, context); if (ok && PortalLogSettings.IsEnabled(context)) new PortalLogRepository().RecordAudit(context, "UPDATE", "TeminatIslem", item.Id.ToString(), tx); return ok;
            }
            catch (Exception error)
            {
                PortalLogContext.AttachEntityToError(context, "TeminatIslem", item == null ? 0 : item.Id, error);
                throw;
            }
        }
        public bool Delete(TeminatIslem item)
        {
            try
            {
                if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
                using (SqlTransactionContext tx = new SqlTransactionContext()) { bool ok = Delete(item, tx); if (ok) tx.Complete(); return ok; }
            }
            catch (Exception error)
            {
                PortalLogContext.AttachEntityToError(context, "TeminatIslem", item == null ? 0 : item.Id, error);
                throw;
            }
        }
        public bool Delete(TeminatIslem item, SqlTransactionContext tx) {
            try
            {
                if (item == null || item.Id == 0) return false; bool ok = repository.Delete(item.Id, tx, context); if (ok && PortalLogSettings.IsEnabled(context)) new PortalLogRepository().RecordAudit(context, "DELETE", "TeminatIslem", item.Id.ToString(), tx); return ok;
            }
            catch (Exception error)
            {
                PortalLogContext.AttachEntityToError(context, "TeminatIslem", item == null ? 0 : item.Id, error);
                throw;
            }
        }
        public bool DeleteBySozlesmeId(int id)
        {
            try
            {
                if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
                using (SqlTransactionContext transaction = new SqlTransactionContext())
                {
                    bool deleted = DeleteBySozlesmeId(id, transaction);
                    if (deleted) transaction.Complete();
                    return deleted;
                }
            }
            catch (Exception error)
            {
                PortalLogContext.AttachEntityToError(context, "KiraSozlesme", id, error);
                throw;
            }
        }

        public bool DeleteBySozlesmeId(int id, SqlTransactionContext transaction)
        {
            try
            {
                DataTable rows = repository.SelectBySozlesmeId(id, transaction, context);
                if (rows == null || rows.Rows.Count == 0) return true;
                if (!repository.DeleteBySozlesmeId(id, transaction, context)) return false;
                foreach (DataRow row in rows.Rows)
                    new PortalLogRepository().RecordAudit(context, "DELETE", "TeminatIslem",
                        Convert.ToString(row["Id"], System.Globalization.CultureInfo.InvariantCulture), transaction);
                return true;
            }
            catch (Exception error)
            {
                PortalLogContext.AttachEntityToError(context, "KiraSozlesme", id, error);
                throw;
            }
        }
        private static List<TeminatIslem> ToList(DataTable t) { return new TeminatIslem().ToList<TeminatIslem>(t); }
        private static TeminatIslem Map(DataTable t) { return ToList(t).FirstOrDefault(); }
    }
}
