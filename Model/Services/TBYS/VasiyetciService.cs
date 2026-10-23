using DAO.Repositories.TBYS;
using DAO.Ortak;
using DAO.Repositories.Ortak;
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
    public class VasiyetciService
    {
        private readonly VasiyetciRepository repository;
        private readonly PortalLogContext context;

        public VasiyetciService() : this(new VasiyetciRepository()) { }

        public VasiyetciService(VasiyetciRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public VasiyetciService(PortalLogContext context) : this(new VasiyetciRepository(context)) { this.context = context; }

        public Vasiyetci GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public List<Vasiyetci> GetAll()
        {
            return ToList(repository.SelectAll());
        }

        public DataTable GetByRegion(int bolgeId)
        {
            return repository.SelectByRegion(bolgeId, ProjeConstants.HEPSI_INT, ProjeConstants.BOLGE_GENELMUDURLUK_INT);
        }

        public Vasiyetci GetById(PortalLogContext context, int id) { return Map(new VasiyetciRepository(context).SelectById(id)); }
        public DataTable GetByRegion(int bolgeId, PortalLogContext context) { return new VasiyetciRepository(context).SelectByRegion(bolgeId, ProjeConstants.HEPSI_INT, ProjeConstants.BOLGE_GENELMUDURLUK_INT); }

        public string GetByIdAsJson(int id, ref int rowCount)
        {
            DataTable table = repository.SelectByIdForJson(id);
            rowCount = table == null ? 0 : table.Rows.Count;
            return new Vasiyetci().ToJSON(table);
        }

        public DataTable GetAllForDataTable(bool excludeDeceased, ref int rowCount)
        {
            DataTable table = repository.SelectAllForDataTable(excludeDeceased, int.Parse(ProjeConstants.BAGISCI_SAG_INT));
            rowCount = table == null ? 0 : table.Rows.Count;
            return table;
        }

        public DataTable GetAllForDataTable(bool excludeDeceased, ref int rowCount, PortalLogContext context) { DataTable table = new VasiyetciRepository(context).SelectAllForDataTable(excludeDeceased, int.Parse(ProjeConstants.BAGISCI_SAG_INT)); rowCount = table == null ? 0 : table.Rows.Count; return table; }

        public List<Vasiyetci> GetByFilters(bool onlyAlive, bool fullTcKimlikNo, bool fullBirthDate)
        {
            return ToList(repository.SelectByFilters(onlyAlive, fullTcKimlikNo, fullBirthDate, int.Parse(ProjeConstants.BAGISCI_VEFAT_INT)));
        }

        public List<Vasiyetci> GetByFilters(bool onlyAlive, bool fullTcKimlikNo, bool fullBirthDate, PortalLogContext context) { return ToList(new VasiyetciRepository(context).SelectByFilters(onlyAlive, fullTcKimlikNo, fullBirthDate, int.Parse(ProjeConstants.BAGISCI_VEFAT_INT))); }

        public int Save(Vasiyetci item)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext tx = new SqlTransactionContext()) { int id = Save(item, tx); tx.Complete(); return id; }
        }

        public int Save(Vasiyetci item, SqlTransactionContext tx) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item, tx, context); if (item.Id > 0 && PortalLogSettings.IsEnabled(context)) new PortalLogRepository().RecordAudit(context, "INSERT", "Vasiyetci", item.Id.ToString(), tx); return item.Id; }

        public bool Update(Vasiyetci item)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext tx = new SqlTransactionContext()) { bool ok = Update(item, tx); if (ok) tx.Complete(); return ok; }
        }

        public bool Update(Vasiyetci item, SqlTransactionContext tx) { if (item == null || item.Id == 0) return false; item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); bool ok = repository.Update(item, tx, context); if (ok && PortalLogSettings.IsEnabled(context)) new PortalLogRepository().RecordAudit(context, "UPDATE", "Vasiyetci", item.Id.ToString(), tx); return ok; }

        public bool Delete(Vasiyetci item)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext tx = new SqlTransactionContext()) { bool ok = Delete(item, tx); if (ok) tx.Complete(); return ok; }
        }

        public bool Delete(Vasiyetci item, SqlTransactionContext tx) { if (item == null || item.Id == 0) return false; bool ok = repository.Delete(item.Id, tx, context); if (ok && PortalLogSettings.IsEnabled(context)) new PortalLogRepository().RecordAudit(context, "DELETE", "Vasiyetci", item.Id.ToString(), tx); return ok; }

        private static List<Vasiyetci> ToList(DataTable table)
        {
            return new Vasiyetci().ToList<Vasiyetci>(table);
        }

        private static Vasiyetci Map(DataTable table)
        {
            return ToList(table).FirstOrDefault();
        }
    }
}
