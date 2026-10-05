using DAO.Repositories.IKYS;
using Model.IKYS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Model.Ortak;
using Utility.ProjeGlobal;

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

        public List<Personel> GetAll()
        {
            return MapList(repository.SelectAll());
        }

        public Personel GetByUserName(string userName)
        {
            return MapSingle(repository.SelectByUserName(userName));
        }

        public List<Personel> GetActiveEmployees(Personel.PersonelTipi personelTipi)
        {
            return MapList(repository.SelectActiveEmployees((int)personelTipi));
        }

        public int Save(Personel item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.IKYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_PERSONEL); return item.Id; }
        public bool Update(Personel item) { if (item == null) throw new ArgumentNullException("item"); Personel old = GetById(item.Id); bool ok = false; if (item.Id != 0) { item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); ok = repository.Update(item); } if (ok && ProjeConstants.IKYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.IKYS, ProjeConstants.IKYS_PERSONEL); return ok; }
        public bool Delete(Personel item) { if (item == null || item.Id == 0) return false; Personel old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.IKYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.IKYS, ProjeConstants.IKYS_PERSONEL); return ok; }
        public Personel GetActiveEmployee(int personelId) { return MapSingle(repository.SelectActiveEmployee(personelId)); }
        public DataTable GetActiveEmployeesByUnitReturnDataTable(int birimId, Personel.PersonelTipi type) { return repository.SelectActiveEmployeesByUnit(birimId, (int)type, type == Personel.PersonelTipi.Tumu); }
        public List<Personel> GetActiveEmployeesByUnit(int birimId, Personel.PersonelTipi type) { return MapList(repository.SelectActiveEmployeesByUnit(birimId, (int)type, type == Personel.PersonelTipi.Tumu)); }
        public List<Personel> GetActiveEmployeesByRegion(int bolgeId) { return MapList(repository.SelectActiveEmployeesByRegion(bolgeId)); }
        public DataTable GetActiveEmployeesReturnDataTable(Personel.PersonelTipi type) { return repository.SelectActiveEmployeesReturnDataTable((int)type, type == Personel.PersonelTipi.Tumu); }
        public DataTable GetPersonelReturnDataTable(int personelId) { return repository.SelectPersonelReturnDataTable(personelId); }
        public DataTable GetEmployeeListReturnDataTable(Personel.PersonelTipi type) { return repository.SelectEmployeeListReturnDataTable((int)type, type == Personel.PersonelTipi.Tumu); }
        public DataTable GetFormerEmployeeListReturnDataTable() { return repository.SelectFormerEmployeeListReturnDataTable(); }
        public DataTable GetManagers() { return repository.SelectManagers(); }
        public DataTable GetActiveEmployeesReturnDT() { return repository.SelectActiveEmployeesReturnDT(); }
        public DataTable GetActiveEmployeesByUnitReturnDT(string ids) { return repository.SelectActiveEmployeesByUnitReturnDataTable(ParseIds(ids), IsAllUnits(ids)); }
        public List<Personel> GetActiveEmployeesByUnitReturnList(string ids) { return MapList(repository.SelectActiveEmployeesByUnitReturnList(ParseIds(ids), IsAllUnits(ids))); }
        public List<Personel> GetByBirthday(int gun, int ay) { return MapList(repository.SelectByDogumGunu(gun, ay)); }
        public List<Personel> GetByMarriageDate(int gun, int ay) { return MapList(repository.SelectByEvlilikTarihi(gun, ay)); }
        public DataTable GetUnselectedMeetingParticipants(int toplantiId) { return repository.SelectUnselectedMeetingParticipants(toplantiId); }
        public List<Personel> GetMeetingParticipants(int toplantiId, bool bilgiVerildi) { return MapList(repository.SelectMeetingParticipants(toplantiId, bilgiVerildi)); }

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
