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


        public MaasHareket GetByTarih(DateTime tarih) { return Map(repository.SelectByTarih(tarih)); }
        public MaasHareket GetByTarih(DateTime tarih, PortalLogContext context)
        { return Map(repository.SelectByTarih(tarih, context)); }
        public List<MaasHareket> GetMaasListesiByTarih(DateTime tarih) { return ToList(repository.SelectByTarih(tarih)); }
        public List<MaasHareket> GetMaasListesiByTarih(DateTime tarih, PortalLogContext context)
        { return ToList(repository.SelectByTarih(tarih, context)); }

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
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                bool result = DeleteByGrupId(item, grupId, transaction, context);
                if (result) transaction.Complete();
                return result;
            }
        }

        public bool DeleteByGrupId(MaasHareket item, int grupId, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (context == null) throw new ArgumentNullException("context");
            List<MaasHareket> rows = ToList(repository.SelectByGroupForDelete(grupId, transaction, context));
            if (!repository.DeleteByGrupId(grupId, transaction, context)) return false;
            foreach (MaasHareket old in rows)
            {
                int auditId = new PortalLogRepository().RecordAudit(context, "DELETE", "MaasHareket", old.Id.ToString(CultureInfo.InvariantCulture), transaction);
                RecordChanges(old, null, auditId, transaction);
            }
            return true;
        }
        private static void RecordChanges(MaasHareket old, MaasHareket current, int auditId, SqlTransactionContext transaction)
        { if (auditId == 0) return; PortalLogRepository log = new PortalLogRepository(); AddChange(log, auditId, "PersonelId", old == null ? null : old.PersonelId.ToString(CultureInfo.InvariantCulture), current == null ? null : current.PersonelId.ToString(CultureInfo.InvariantCulture), transaction); AddChange(log, auditId, "Tarih", old == null ? null : AuditDate(old.Tarih), current == null ? null : AuditDate(current.Tarih), transaction); AddChange(log, auditId, "Derece", old == null ? null : old.Derece.ToString(CultureInfo.InvariantCulture), current == null ? null : current.Derece.ToString(CultureInfo.InvariantCulture), transaction); AddChange(log, auditId, "Kademe", old == null ? null : old.Kademe.ToString(CultureInfo.InvariantCulture), current == null ? null : current.Kademe.ToString(CultureInfo.InvariantCulture), transaction); AddChange(log, auditId, "GrupId", old == null ? null : old.GrupId.ToString(CultureInfo.InvariantCulture), current == null ? null : current.GrupId.ToString(CultureInfo.InvariantCulture), transaction); AddChange(log, auditId, "ProtokolSiraNo", old == null ? null : old.ProtokolSiraNo.ToString(CultureInfo.InvariantCulture), current == null ? null : current.ProtokolSiraNo.ToString(CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "Adi", old == null ? null : Convert.ToString(old.Adi, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.Adi, CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "Soyadi", old == null ? null : Convert.ToString(old.Soyadi, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.Soyadi, CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "Unvan", old == null ? null : Convert.ToString(old.Unvan, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.Unvan, CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "DereceKademeIlerlemeTarihi", old == null ? null : old.DereceKademeIlerlemeTarihi.ToString("o", CultureInfo.InvariantCulture), current == null ? null : current.DereceKademeIlerlemeTarihi.ToString("o", CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "Ucret", old == null ? null : Convert.ToString(old.Ucret, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.Ucret, CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "Ikramiye", old == null ? null : Convert.ToString(old.Ikramiye, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.Ikramiye, CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "Agi", old == null ? null : Convert.ToString(old.Agi, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.Agi, CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "ToplamUcret", old == null ? null : Convert.ToString(old.ToplamUcret, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.ToplamUcret, CultureInfo.InvariantCulture), transaction);
        }
        private static string AuditDate(DateTime value) { return value < new DateTime(1901, 1, 1) ? null : value.ToString("o", CultureInfo.InvariantCulture); }
        private static void AddChange(PortalLogRepository log, int auditId, string field, string oldValue, string newValue, SqlTransactionContext transaction) { if (!string.Equals(oldValue, newValue, StringComparison.Ordinal)) log.RecordAuditChange(auditId, field, oldValue, newValue, transaction); }
        private static MaasHareket Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static List<MaasHareket> ToList(DataTable table) { return new MaasHareket().ToList<MaasHareket>(table); }

        public MaasHareket GetById(int id, PortalLogContext context)
        { return Map(repository.SelectById(id, context)); }

        public List<MaasHareket> GetAll(PortalLogContext context)
        { return ToList(repository.SelectAll(context)); }

        public MaasHareket GetByTarih(DateTime tarih, SqlTransactionContext transaction, PortalLogContext context)
        { return Map(repository.SelectByTarih(tarih, transaction, context)); }

        public List<MaasHareket> GetMaasListesiByTarih(DateTime tarih, SqlTransactionContext transaction, PortalLogContext context)
        { return ToList(repository.SelectByTarih(tarih, transaction, context)); }
    }
}
