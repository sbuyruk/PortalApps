using Model.Ortak;
using Model.Services.NBYS;
using System;
using System.Collections.Generic;

namespace Model.NBYS
{
    [Serializable]
    public class DuzenliNakitBagisci : ParentClass
    {

        public int BagisciId { get; set; }
        public long TCKimlikNo { get; set; }
        public string BagisciAdi { get; set; }
        public int BagisAdedi { get; set; }
        public decimal BagisToplami { get; set; }
        public DateTime BaslamaTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public decimal Tutar { get; set; }
        public bool Aktif { get; set; }
        public int ArmaganId { get; set; }
        public int NakitBagisHareketId { get; set; }
        public string Telefon { get; set; }
        public string EPosta { get; set; }
        public string EslesmeBilgisi { get; set; }
        public string Aciklama { get; set; }
        public override T Select<T>(int id)
        {
            return (T)Convert.ChangeType(new DuzenliNakitBagisciService().GetById(id), typeof(T));
        }

        public override int Save()
        {
            return new DuzenliNakitBagisciService().Save(this);
        }

        public override bool Update()
        {
            return new DuzenliNakitBagisciService().Update(this);
        }

        public override bool Delete()
        {
            return new DuzenliNakitBagisciService().Delete(this);
        }

        public override List<T> SelectAll<T>()
        {
            return (List<T>)Convert.ChangeType(new DuzenliNakitBagisciService().GetAll(), typeof(List<T>));
        }

        public DuzenliNakitBagisci SelectByBagisciId(int bagisciId)
        {
            return new DuzenliNakitBagisciService().GetActiveByBagisciId(bagisciId);
        }
    }
}
