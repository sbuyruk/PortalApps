using System;
using System.Collections.Generic;
using Model.Services.Ortak;

namespace Model.Ortak
{
    [Serializable]
    public class Olay : EntityBase
    {
        public string Program { get; set; }
        public string IslemTipi { get; set; } //Giris-düzeltme-silme
        public string IslemKonusu { get; set; } //faaliyet-tasinmaz-kiraci, sözlesme, nakitbagis vs
        public DateTime IslemTarihi { get; set; }
        public string IslemYapan { get; set; }
        public string Aciklama { get; set; }
        
        public T Select<T>(int id)
        {
            Id = id;
            return (T)Convert.ChangeType(new OlayService().GetById(id), typeof(T));

        }
        public Olay Select(int id)
        {
            Id = id;
            return new OlayService().GetById(id);
        }
        public int Save()
        {
            return new OlayService().Save(this);
        }
        
        public List<Olay> SelectByTarihReturnList(DateTime islemTarihi, string program)
        {
            return new OlayService().GetByDateAndProgram(islemTarihi, program);
        }
        public bool Update()
        {
            return new OlayService().Update(this);
        }
        public bool Delete()
        {
            return new OlayService().Delete(this);
        }
        public List<T> SelectAll<T>()
        {
            return (List<T>)Convert.ChangeType(new OlayService().GetAll(), typeof(List<T>));
        }
        public List<Olay> SelectOlayListesi()
        {
            return new OlayService().GetAuditList();
        }
    }
}
