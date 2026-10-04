using Model.Services.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Model.IKYS
{
    [Serializable]
    public class Personel : ParentClass
    {
        public enum PersonelTipi { [Display(Name = "Tüm Personel")] Tumu = 0, [Display(Name = "Kadrolu Personel")] Kadrolu = 1, [Display(Name = "Kadro Harici Personel")] Kadrosuz = 2 }
        public int PerId { get; set; }
        public int SicilNo { get; set; }
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public int Tahsili { get; set; }
        public string KullaniciAdi { get; set; }
        public int Asker_sivil { get; set; }
        public int Tipi { get; set; } = (int)PersonelTipi.Kadrolu;
        public override T Select<T>(int id) { Id = id; return (T)Convert.ChangeType(new PersonelService().GetById(id), typeof(T)); }
        public Personel Select(int id) { Id = id; return new PersonelService().GetById(id); }
        public override int Save() { return new PersonelService().Save(this); }
        public override bool Update() { return new PersonelService().Update(this); }
        public override bool Delete() { return new PersonelService().Delete(this); }
        public override List<T> SelectAll<T>() { return (List<T>)Convert.ChangeType(new PersonelService().GetAll(), typeof(List<T>)); }
        public Personel SelectByUserName(string userName) { return new PersonelService().GetByUserName(userName); }
        public Personel SelectCalisanPersonel(int personelId) { return new PersonelService().GetActiveEmployee(personelId); }
        public DataTable SelectCalisanPersonelByBirimIdReturnDataTable(int birimId, PersonelTipi personelTipi = PersonelTipi.Kadrolu) { return new PersonelService().GetActiveEmployeesByUnitReturnDataTable(birimId, personelTipi); }
        public List<Personel> SelectCalisanPersonelByBirimId(int birimId, PersonelTipi personelTipi = PersonelTipi.Kadrolu) { return new PersonelService().GetActiveEmployeesByUnit(birimId, personelTipi); }
        public List<Personel> SelectByBolgeId(int bolgeId) { return new PersonelService().GetActiveEmployeesByRegion(bolgeId); }
        public DataTable SelectCalisanPersonelReturnDataTable(PersonelTipi personelTipi = PersonelTipi.Kadrolu) { return new PersonelService().GetActiveEmployeesReturnDataTable(personelTipi); }
        public DataTable SelectPersonelReturnDataTable(int personelId) { return new PersonelService().GetPersonelReturnDataTable(personelId); }
        public DataTable SelectCalisanPersonelListesiReturnDataTable(PersonelTipi personelTipi = PersonelTipi.Kadrolu) { return new PersonelService().GetEmployeeListReturnDataTable(personelTipi); }
        public DataTable SelectAyrilanPersonelListesiReturnDataTable() { return new PersonelService().GetFormerEmployeeListReturnDataTable(); }
        public DataTable SelectAmirReturnDataTable() { return new PersonelService().GetManagers(); }
        public List<Personel> SelectCalisanPersonel() { return new PersonelService().GetActiveEmployees(PersonelTipi.Kadrolu); }
        public DataTable SelectCalisanPersonelReturnDT() { return new PersonelService().GetActiveEmployeesReturnDT(); }
        public DataTable SelectCalisanPersonelByBirimReturnDT(string birimListesiStr) { return new PersonelService().GetActiveEmployeesByUnitReturnDT(birimListesiStr); }
        public List<Personel> SelectCalisanPersonelByBirimReturnList(string birimListesiStr) { return new PersonelService().GetActiveEmployeesByUnitReturnList(birimListesiStr); }
        public List<Personel> SelectByDogumGunu(int gun, int ay) { return new PersonelService().GetByBirthday(gun, ay); }
        public List<Personel> SelectByEvlilikTar(int gun, int ay) { return new PersonelService().GetByMarriageDate(gun, ay); }
        public DataTable SelectSecilmemisIcKatilimcilarByToplantiIdReturnDT(int toplantiId) { return new PersonelService().GetUnselectedMeetingParticipants(toplantiId); }
        public List<Personel> SelectKatilimcilarByToplantiIdList(int toplantiId) { return new PersonelService().GetMeetingParticipants(toplantiId, false); }
        public List<Personel> SelectBilgiVerilenlerByToplantiIdList(int toplantiId) { return new PersonelService().GetMeetingParticipants(toplantiId, true); }
    }
}
