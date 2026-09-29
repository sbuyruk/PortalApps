using Model.Ortak;
using Model.Services.NBYS;
using System;
using System.Collections.Generic;

namespace Model.NBYS
{
    public class NakitBagisHareket : ParentClass
    {
        public DateTime BagisTarihi { get; set; }
        public int BagisciId { get; set; }
        public decimal BagisMiktari { get; set; }
        public string DovizCinsi { get; set; }
        public int BankaId { get; set; }
        public int Ili { get; set; }
        public int Ilcesi { get; set; }
        public string Adresi { get; set; }
        public string Telefon { get; set; }
        public string Aciklama { get; set; }
        public int ArmaganId { get; set; }
        public bool IadeEdildiMi { get; set; }
        public decimal IadeMiktari { get; set; }
        public DateTime IadeTarihi { get; set; }
        public string IadeSebebi { get; set; }
        public string IadeEden { get; set; }
        public decimal DovizTutari { get; set; }
        public decimal DovizKuru { get; set; }
        public DateTime KurTarihi { get; set; }
        public int EkstreAktarmaId { get; set; }
        public string BagisTipi { get; set; }
        //Methods
        public override int Save()
        {
            return new NakitBagisHareketService().Save(this);
        }
        public override bool Update()
        {
            return new NakitBagisHareketService().Update(this);
        }
        public override bool Delete()
        {
            return new NakitBagisHareketService().Delete(this);
        }
        public override T Select<T>(int id)
        {
            return (T)Convert.ChangeType(new NakitBagisHareketService().GetById(id), typeof(T));
        }
        public override List<T> SelectAll<T>()
        {
            return (List<T>)Convert.ChangeType(
                new NakitBagisHareketService().GetAll(), typeof(List<T>));
        }
    }

}
