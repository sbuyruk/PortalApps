using DAO.Repositories.TBYS;
using DAO.Ortak;
using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.ProjeGlobal;
using Utility.Logging;

namespace Model.Services.TBYS
{
    public class BagisciBagisService
    {
        private readonly BagisciBagisRepository repository;
        private readonly PortalLogContext context;
        public BagisciBagisService() : this(new BagisciBagisRepository()) { }
        public BagisciBagisService(PortalLogContext context) : this(new BagisciBagisRepository(context)) { this.context = context; }
        public BagisciBagisService(BagisciBagisRepository repository) { if (repository == null) throw new ArgumentNullException("repository"); this.repository = repository; }
        public BagisciBagis GetById(int id) {
            try
            {
                return Map(repository.SelectById(id));
            }
            catch (Exception error)
            {
                PortalLogContext.AttachEntityToError(context, "BagisciBagis", id, error);
                throw;
            }
        }
        public List<BagisciBagis> GetAll() { return ToList(repository.SelectAll()); }

        public List<BagisciBagis> GetByBagisId(int bagisId) {
            try
            {
                return ToList(repository.SelectByBagisId(bagisId));
            }
            catch (Exception error)
            {
                PortalLogContext.AttachEntityToError(context, "Bagis", bagisId, error);
                throw;
            }
        }
        public BagisciBagis GetById(int id, SqlTransactionContext transaction, PortalLogContext context) {
            try
            {
                RequireTransaction(transaction, context); return Map(repository.SelectById(id, transaction, context));
            }
            catch (Exception error)
            {
                PortalLogContext.AttachEntityToError(context, "BagisciBagis", id, error);
                throw;
            }
        }
        public int Save(BagisciBagis item, SqlTransactionContext transaction, PortalLogContext context) {
            try
            {
                RequireTransaction(transaction, context); if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item, transaction, context); if (item.Id > 0) RecordAudit("INSERT", item.Id, transaction, context); return item.Id;
            }
            catch (Exception error)
            {
                PortalLogContext.AttachEntityToError(context, item != null && item.Id > 0 ? "BagisciBagis" : "TasinmazBagisci", item == null ? 0 : (item.Id > 0 ? item.Id : item.BagisciId), error);
                throw;
            }
        }
        public bool Update(BagisciBagis item, SqlTransactionContext transaction, PortalLogContext context) {
            try
            {
                RequireTransaction(transaction, context); if (item == null || item.Id == 0) return false; item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); bool ok = repository.Update(item, transaction, context); if (ok) RecordAudit("UPDATE", item.Id, transaction, context); return ok;
            }
            catch (Exception error)
            {
                PortalLogContext.AttachEntityToError(context, "BagisciBagis", item == null ? 0 : item.Id, error);
                throw;
            }
        }
        public bool Delete(BagisciBagis item, SqlTransactionContext transaction, PortalLogContext context) {
            try
            {
                RequireTransaction(transaction, context); if (item == null || item.Id == 0) return false; bool ok = repository.Delete(item.Id, transaction, context); if (ok) RecordAudit("DELETE", item.Id, transaction, context); return ok;
            }
            catch (Exception error)
            {
                PortalLogContext.AttachEntityToError(context, "BagisciBagis", item == null ? 0 : item.Id, error);
                throw;
            }
        }
        private static void RequireTransaction(SqlTransactionContext transaction, PortalLogContext context) { if (transaction == null) throw new ArgumentNullException("transaction", "Veritabanı işlem bağlamı gereklidir."); if (context == null) throw new ArgumentNullException("context", "TBYS log bağlamı gereklidir."); }
        private static void RecordAudit(string operation, int id, SqlTransactionContext transaction, PortalLogContext context) { new DAO.Repositories.Ortak.PortalLogRepository().RecordAudit(context, operation, "BagisciBagis", id.ToString(System.Globalization.CultureInfo.InvariantCulture), transaction); }
        private static BagisciBagis Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static List<BagisciBagis> ToList(DataTable table) { return new BagisciBagis().ToList<BagisciBagis>(table); }
    }
}
