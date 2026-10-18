using DAO.Repositories.IKYS;
using DAO.Repositories.Ortak;
using DAO.Ortak;
using Model.IKYS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Globalization;
using Model.Ortak;
using Utility.ProjeGlobal;
using Utility.Logging;

namespace Model.Services.IKYS
{
    public class PersonelService
    {
        private readonly PersonelRepository repository;

        public PersonelService() : this(new PersonelRepository())
        {
        }

        public PersonelService(PersonelRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public Personel GetById(int id)
        {
            return MapSingle(repository.SelectById(id));
        }
        public Personel GetById(int id, PortalLogContext context)
        {
            return MapSingle(repository.SelectById(id, context));
        }

        public List<Personel> GetAll()
        {
            return MapList(repository.SelectAll());
        }

        public Personel GetByUserName(string userName)
        {
            return MapSingle(repository.SelectByUserName(userName));
        }
        public Personel GetByUserName(string userName, PortalLogContext context)
        {
            return MapSingle(repository.SelectByUserName(userName, context));
        }
        public bool TryRecordWebPartOpened(PortalLogContext context) { return new PortalLogRepository().TryRecordWebPartOpened(context); }

        public List<Personel> GetActiveEmployees(Personel.PersonelTipi personelTipi)
        {
            return MapList(repository.SelectActiveEmployees((int)personelTipi));
        }
        public List<Personel> GetActiveEmployees(Personel.PersonelTipi personelTipi, PortalLogContext context)
        {
            return MapList(repository.SelectActiveEmployees((int)personelTipi, context));
        }

        public int Save(Personel item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.IKYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_PERSONEL); return item.Id; }
        public int Save(Personel item, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (!PortalLogSettings.IsEnabled(context)) return Save(item);
            if (item == null) throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName();
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                item.Id = Save(item, transaction, context);
                transaction.Complete();
            }
            return item.Id;
        }
        public int Save(Personel item, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (context == null) throw new ArgumentNullException("context");
            if (item == null) throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item, transaction, context);
            if (item.Id <= 0) throw new InvalidOperationException("Personel kaydı oluşturulamadı.");
            int auditId = new PortalLogRepository().RecordAudit(context, "INSERT", "Personel", item.Id.ToString(CultureInfo.InvariantCulture), transaction);
            RecordChanges(null, item, auditId, transaction);
            return item.Id;
        }
        public bool Update(Personel item) { if (item == null) throw new ArgumentNullException("item"); Personel old = GetById(item.Id); bool ok = false; if (item.Id != 0) { item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); ok = repository.Update(item); } if (ok && ProjeConstants.IKYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.IKYS, ProjeConstants.IKYS_PERSONEL); return ok; }
        public bool Update(Personel item, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context"); if (!PortalLogSettings.IsEnabled(context)) return Update(item);
            if (item == null || item.Id <= 0) return false;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                bool updated = Update(item, transaction, context);
                if (updated) transaction.Complete();
                return updated;
            }
        }
        public bool Update(Personel item, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (context == null) throw new ArgumentNullException("context");
            if (item == null || item.Id <= 0) return false;
            Personel old = MapSingle(repository.SelectById(item.Id, transaction, context));
            item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName();
            if (old == null || !repository.Update(item, transaction, context)) return false;
            int auditId = new PortalLogRepository().RecordAudit(context, "UPDATE", "Personel", item.Id.ToString(CultureInfo.InvariantCulture), transaction);
            RecordChanges(old, item, auditId, transaction);
            return true;
        }
        public bool Delete(Personel item) { if (item == null || item.Id == 0) return false; Personel old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.IKYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.IKYS, ProjeConstants.IKYS_PERSONEL); return ok; }
        public Personel GetActiveEmployee(int personelId) { return MapSingle(repository.SelectActiveEmployee(personelId)); }
        public DataTable GetActiveEmployeesByUnitReturnDataTable(int birimId, Personel.PersonelTipi type) { return repository.SelectActiveEmployeesByUnit(birimId, (int)type, type == Personel.PersonelTipi.Tumu); }
        public List<Personel> GetActiveEmployeesByUnit(int birimId, Personel.PersonelTipi type) { return MapList(repository.SelectActiveEmployeesByUnit(birimId, (int)type, type == Personel.PersonelTipi.Tumu)); }
        public List<Personel> GetActiveEmployeesByRegion(int bolgeId) { return MapList(repository.SelectActiveEmployeesByRegion(bolgeId)); }
        public DataTable GetActiveEmployeesReturnDataTable(Personel.PersonelTipi type) { return repository.SelectActiveEmployeesReturnDataTable((int)type, type == Personel.PersonelTipi.Tumu); }
        public DataTable GetPersonelReturnDataTable(int personelId) { return repository.SelectPersonelReturnDataTable(personelId); }
        public DataTable GetEmployeeListReturnDataTable(Personel.PersonelTipi type) { return repository.SelectEmployeeListReturnDataTable((int)type, type == Personel.PersonelTipi.Tumu); }
        public DataTable GetEmployeeListReturnDataTable(Personel.PersonelTipi type, PortalLogContext context) { return repository.SelectEmployeeListReturnDataTable((int)type, type == Personel.PersonelTipi.Tumu, context); }
        public DataTable GetFormerEmployeeListReturnDataTable() { return repository.SelectFormerEmployeeListReturnDataTable(); }
        public DataTable GetFormerEmployeeListReturnDataTable(PortalLogContext context) { return repository.SelectFormerEmployeeListReturnDataTable(context); }
        public DataTable GetManagers() { return repository.SelectManagers(); }
        public List<Personel> GetSchemaManagers(PortalLogContext context) { return MapList(repository.SelectSchemaManagers(context)); }
        public List<Personel> GetSchemaActiveManagers(PortalLogContext context) { return MapList(repository.SelectSchemaActiveManagers(context)); }
        public DataTable GetSchemaEmployees(PortalLogContext context) { return repository.SelectSchemaEmployees(context); }
        public DataTable GetActiveEmployeesReturnDT() { return repository.SelectActiveEmployeesReturnDT(); }
        public DataTable GetActiveEmployeesByUnitReturnDT(string ids) { return repository.SelectActiveEmployeesByUnitReturnDataTable(ParseIds(ids), IsAllUnits(ids)); }
        public DataTable GetActiveEmployeesByUnitReturnDT(string ids, PortalLogContext context) { return repository.SelectActiveEmployeesByUnitReturnDataTable(ParseIds(ids), IsAllUnits(ids), context); }
        public List<Personel> GetActiveEmployeesByUnitReturnList(string ids) { return MapList(repository.SelectActiveEmployeesByUnitReturnList(ParseIds(ids), IsAllUnits(ids))); }
        public List<Personel> GetByBirthday(int gun, int ay) { return MapList(repository.SelectByDogumGunu(gun, ay)); }
        public List<Personel> GetByMarriageDate(int gun, int ay) { return MapList(repository.SelectByEvlilikTarihi(gun, ay)); }
        public DataTable GetUnselectedMeetingParticipants(int toplantiId) { return repository.SelectUnselectedMeetingParticipants(toplantiId); }
        public List<Personel> GetMeetingParticipants(int toplantiId, bool bilgiVerildi) { return MapList(repository.SelectMeetingParticipants(toplantiId, bilgiVerildi)); }
        private static void RecordChanges(Personel old, Personel current, int auditId, SqlTransactionContext transaction)
        {
            PortalLogRepository log = new PortalLogRepository();
            AddChange(log, auditId, "PerId", old == null ? null : old.PerId.ToString(CultureInfo.InvariantCulture), current == null ? null : current.PerId.ToString(CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "SicilNo", old == null ? null : old.SicilNo.ToString(CultureInfo.InvariantCulture), current == null ? null : current.SicilNo.ToString(CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "Tahsili", old == null ? null : old.Tahsili.ToString(CultureInfo.InvariantCulture), current == null ? null : current.Tahsili.ToString(CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "Tipi", old == null ? null : old.Tipi.ToString(CultureInfo.InvariantCulture), current == null ? null : current.Tipi.ToString(CultureInfo.InvariantCulture), transaction);
        }
        private static void AddChange(PortalLogRepository log, int auditId, string field, string oldValue, string newValue, SqlTransactionContext transaction) { if (!string.Equals(oldValue, newValue, StringComparison.Ordinal)) log.RecordAuditChange(auditId, field, oldValue, newValue, transaction); }

        private static Personel MapSingle(DataTable dataTable)
        {
            return MapList(dataTable).FirstOrDefault();
        }

        private static List<Personel> MapList(DataTable dataTable)
        {
            return new Personel().ToList<Personel>(dataTable);
        }

        private static List<int> ParseIds(string value)
        {
            List<int> ids = new List<int>(); if (string.IsNullOrWhiteSpace(value) || value.Trim() == "0") return ids;
            foreach (string part in value.Split(',')) { int id; if (!int.TryParse(part.Trim(), out id)) throw new ArgumentException("Birim ID listesi geçersiz bir değer içeriyor.", "value"); ids.Add(id); }
            return ids;
        }
        private static bool IsAllUnits(string value) { return !string.IsNullOrWhiteSpace(value) && value.Trim() == "0"; }
    }
}
