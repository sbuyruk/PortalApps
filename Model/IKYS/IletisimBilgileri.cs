using Model.Ortak;
using Model.Services.IKYS;
using System;
using System.Collections.Generic;

namespace Model.IKYS
{
    public class IletisimBilgileri : ParentClass
    {
        public int PersonelId { get; set; }
        public string Adres { get; set; }
        public string Semt { get; set; }
        public string Ili { get; set; }
        public int Ilcesi { get; set; }
        public string PostaKodu { get; set; }
        public string DahiliTelefonu { get; set; }
        public string EvTelefonu { get; set; }
        public string CepTelefonu { get; set; }
        public string CepTelefonu2 { get; set; }
        public string IntranetEPosta { get; set; }
        public string InternetEPosta { get; set; }
        public string OzelEPosta { get; set; }
        public string Plaka { get; set; }

        public override T Select<T>(int id) { return (T)Convert.ChangeType(new IletisimBilgileriService().GetById(id), typeof(T)); }
        public override int Save() { return new IletisimBilgileriService().Save(this); }
        public override bool Update() { return new IletisimBilgileriService().Update(this); }
        public override bool Delete() { return new IletisimBilgileriService().Delete(this); }
        public override List<T> SelectAll<T>() { return (List<T>)Convert.ChangeType(new IletisimBilgileriService().GetAll(), typeof(List<T>)); }
        public IletisimBilgileri SelectByPersonelId(int personelId) { return new IletisimBilgileriService().GetByPersonelId(personelId); }
    }
}
