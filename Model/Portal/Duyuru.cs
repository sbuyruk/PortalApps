using Model.Ortak;
using Model.Services.Portal;
using System;
using System.Collections.Generic;

namespace Model.Portal
{
    [Serializable]
    public class Duyuru : EntityBase
    {
        public string Baslik { get; set; }
        public string Metin { get; set; }
        public DateTime YayinBasTar { get; set; }
        public DateTime YayinBitTar { get; set; }
        public string Tekrar { get; set; }
        public string DuyuruAlicilari { get; set; }
        public string Resim { get; set; }
        public string Aciklama { get; set; }
        public bool Aktif { get; set; }
        public bool Popup { get; set; }
        public T Select<T>(int id) { return (T)Convert.ChangeType(new DuyuruService().GetById(id), typeof(T)); }
        public Duyuru Select(int id) { Id = id; return new DuyuruService().GetById(id); }
        public int Save() { return new DuyuruService().Save(this); }
        public bool Update() { return new DuyuruService().Update(this); }
        public bool Delete() { return new DuyuruService().Delete(this); }
        public List<Duyuru> SelectByTarihReturnList(DateTime now) { return new DuyuruService().GetByDate(now); }
        public List<Duyuru> SelectByTarihReturnList(DateTime now, string tekrar) { return new DuyuruService().GetByRepeat(now, tekrar); }
        public List<T> SelectAll<T>() { return (List<T>)Convert.ChangeType(new DuyuruService().GetAll(), typeof(List<T>)); }
        public List<Duyuru> SelectDuyuruListesi() { return new DuyuruService().GetAnnouncementList(); }
    }
}
