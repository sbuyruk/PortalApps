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
    public class OdemeAyristirmaService
    {
        private readonly OdemeAyristirmaRepository repository;
        private readonly PortalLogContext context;

        public OdemeAyristirmaService() : this(new OdemeAyristirmaRepository()) { }
        public OdemeAyristirmaService(OdemeAyristirmaRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }
        public OdemeAyristirmaService(PortalLogContext context) : this(new OdemeAyristirmaRepository(context)) { this.context = context; }

        public OdemeAyristirma GetById(int id) { return Map(repository.SelectById(id)); }
        public List<OdemeAyristirma> GetAll() { return ToList(repository.SelectAll()); }
        public DataTable GetByKiraEkstreAktarmaId(int kiraEkstreAktarmaId) { return repository.SelectByKiraEkstreAktarmaId(kiraEkstreAktarmaId); }
        public int Save(OdemeAyristirma item)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext tx = new SqlTransactionContext()) { int id = Save(item, tx); tx.Complete(); return id; }
        }
        public int Save(OdemeAyristirma item, SqlTransactionContext tx) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item, tx, context); if (item.Id > 0) Audit("INSERT", item.Id, tx); return item.Id; }
        public bool Update(OdemeAyristirma item)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext tx = new SqlTransactionContext()) { bool transactionSucceeded = Update(item, tx); if (transactionSucceeded) tx.Complete(); return transactionSucceeded; }
        }
        public bool Update(OdemeAyristirma item, SqlTransactionContext tx) { if (item == null || item.Id == 0) return false; item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); bool ok = repository.Update(item, tx, context); if (ok) Audit("UPDATE", item.Id, tx); return ok; }
        public bool Delete(OdemeAyristirma item)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext tx = new SqlTransactionContext()) { bool transactionSucceeded = Delete(item, tx); if (transactionSucceeded) tx.Complete(); return transactionSucceeded; }
        }
        public bool Delete(OdemeAyristirma item, SqlTransactionContext tx) { if (item == null || item.Id == 0) return false; bool ok = repository.Delete(item.Id, tx, context); if (ok) Audit("DELETE", item.Id, tx); return ok; }
        private void Audit(string operation, int id, SqlTransactionContext tx) { if (PortalLogSettings.IsEnabled(context)) new PortalLogRepository().RecordAudit(context, operation, "OdemeAyristirma", id.ToString(), tx); }
        private static List<OdemeAyristirma> ToList(DataTable table) { return new OdemeAyristirma().ToList<OdemeAyristirma>(table); }
        private static OdemeAyristirma Map(DataTable table) { return ToList(table).FirstOrDefault(); }
    }
}
