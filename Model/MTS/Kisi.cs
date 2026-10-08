using Model.Services.MTS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;

namespace Model.MTS
{
    public class Kisi : EntityBase
    {
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public long TCKimlikNo { get; set; }

        public int MTSUnvanTanimId { get; set; }
        public string Kurumu { get; set; }
        public string Unvani { get; set; }
        public string Gorevi { get; set; }
        public string Telefon1 { get; set; }
        public string Telefon2 { get; set; }
        public string Telefon3 { get; set; }
        public string TelAciklama1 { get; set; }
        public string TelAciklama2 { get; set; }
        public string TelAciklama3 { get; set; }
        public string EPosta { get; set; }
        public string Adres { get; set; }
        public int Ili { get; set; }
        public int Ilcesi { get; set; }
        public string Aciklama { get; set; }
        public string Dahili1 { get; set; }
        public string Dahili2 { get; set; }
        public string Dahili3 { get; set; }
        public DateTime DogumTarihi { get; set; }
        public bool Kutlama { get; set; }
        public bool RandevuKisiti { get; set; }

        public int Save()
        {
            return new KisiService().Save(this);
        }
        public bool Update()
        {
            return new KisiService().Update(this);
        }
        public bool Delete()
        {
            return new KisiService().Delete(this);
        }
        public Kisi Select(int id)
        {
            Id = id;
            return new KisiService().GetById(id);
        }
        public T Select<T>(int id)
        {
            Id = id;
            return (T)Convert.ChangeType(new KisiService().GetById(id), typeof(T));
        }
        public List<T> SelectAll<T>()
        {
            return (List<T>)Convert.ChangeType(new KisiService().GetAll(), typeof(List<T>));
        }
        public DataTable SelectAllReturnDT()
        {
            return new KisiService().GetAllData();
        }
        public List<Kisi> SelectByDogumGunuKutlamaReturnDT()
        {
            return new KisiService().GetBirthdayCelebrations();
        }
        public string SelectAllReturnJson()
        {
            return new KisiService().GetAllJson();
        }
        public DataTable SelectSecilmemisDisKatilimcilarByFaaliyetIdReturnDT(int faaliyetId)
        {
            return new KisiService().GetUnselectedParticipants(faaliyetId);
        }

        public Kisi SelectByAdiSoyadi(string adi, string soyadi)
        {
            return new KisiService().GetByName(adi, soyadi);
        }
        public Kisi SelectByTCKimlikNo(string tckimlik)
        {
            return new KisiService().GetByTcKimlikNo(tckimlik);
        }
    }
}
