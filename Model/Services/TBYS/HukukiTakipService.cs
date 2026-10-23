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
    public class HukukiTakipService
    {
        private readonly HukukiTakipRepository repository;
        private readonly PortalLogContext context;

        public HukukiTakipService() : this(new HukukiTakipRepository()) { }

        public HukukiTakipService(HukukiTakipRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public HukukiTakipService(PortalLogContext context) : this(new HukukiTakipRepository(context)) { this.context = context; }

        public HukukiTakip GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public List<HukukiTakip> GetAll()
        {
            return new HukukiTakip().ToList<HukukiTakip>(repository.SelectAll());
        }

        public HukukiTakip GetBySozlesmeId(int sozlesmeId)
        {
            return new HukukiTakip().ToList<HukukiTakip>(repository.SelectBySozlesmeId(sozlesmeId)).FirstOrDefault();
        }

        public string GetActiveListAsJson()
        {
            return new HukukiTakip().ToJSON(repository.SelectActiveList());
        }

        public DataTable GetActiveList()
        {
            return repository.SelectActiveListForDataTable();
        }

        public int Save(HukukiTakip item)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext tx = new SqlTransactionContext()) { int id = Save(item, tx); tx.Complete(); return id; }
        }
        public int Save(HukukiTakip item, SqlTransactionContext tx) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item, tx, context); if (item.Id > 0 && PortalLogSettings.IsEnabled(context)) new PortalLogRepository().RecordAudit(context, "INSERT", "HukukiTakip", item.Id.ToString(), tx); return item.Id; }

        public bool Update(HukukiTakip item)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext tx = new SqlTransactionContext()) { bool ok = Update(item, tx); if (ok) tx.Complete(); return ok; }
        }
        public bool Update(HukukiTakip item, SqlTransactionContext tx) { if (item == null || item.Id == 0) return false; item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); bool ok = repository.Update(item, tx, context); if (ok && PortalLogSettings.IsEnabled(context)) new PortalLogRepository().RecordAudit(context, "UPDATE", "HukukiTakip", item.Id.ToString(), tx); return ok; }

        public bool Delete(HukukiTakip item)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext tx = new SqlTransactionContext()) { bool ok = Delete(item, tx); if (ok) tx.Complete(); return ok; }
        }
        public bool Delete(HukukiTakip item, SqlTransactionContext tx) { if (item == null || item.Id == 0) return false; bool ok = repository.Delete(item.Id, tx, context); if (ok && PortalLogSettings.IsEnabled(context)) new PortalLogRepository().RecordAudit(context, "DELETE", "HukukiTakip", item.Id.ToString(), tx); return ok; }

        public bool DeleteBySozlesmeId(int id)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                bool deleted = DeleteBySozlesmeId(id, transaction);
                if (deleted) transaction.Complete();
                return deleted;
            }
        }

        public bool DeleteBySozlesmeId(int id, SqlTransactionContext transaction)
        {
            DataTable rows = repository.SelectBySozlesmeId(id, transaction, context);
            if (rows == null || rows.Rows.Count == 0) return true;
            if (!repository.DeleteBySozlesmeId(id, transaction, context)) return false;
            foreach (DataRow row in rows.Rows)
                new PortalLogRepository().RecordAudit(context, "DELETE", "HukukiTakip",
                    Convert.ToString(row["Id"], System.Globalization.CultureInfo.InvariantCulture), transaction);
            return true;
        }

        private static HukukiTakip Map(DataTable table)
        {
            return new HukukiTakip().ToList<HukukiTakip>(table).FirstOrDefault();
        }
    }
}
