using System;
using System.Collections.Generic;
using Model.Services.Ortak;

namespace Model.Ortak
{
    public class SilinenKayit : EntityBase
    {
        public string TabloAdi { get; set; }
        public string SilinenKayitBilgisi { get; set; }
        public string Silen { get; set; }
        public DateTime SilinmeTarihi { get; set; }
        public string SilinmeSebebi { get; set; }
        public int Save()
        {
            return new SilinenKayitService().Save(this);
        }
        public bool Update()
        {
            return new SilinenKayitService().Update(this);
        }
        public bool Delete()
        {
            throw new NotImplementedException();
        }
        public T Select<T>(int id)
        {
            throw new NotImplementedException();
        }
        public List<T> SelectAll<T>()
        {
            throw new NotImplementedException();
        }


    }
}
