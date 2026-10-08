using Model.Ortak;
using Model.Services.TBYS;
using System;
using System.Collections.Generic;
using System.Data;

namespace Model.TBYS
{
    [Serializable]
    public class TasinmazTaahhut : EntityBase
    {
        public int TasinmazId { get; set; }
        public int BagisciId { get; set; }
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public long TCKimlikNo { get; set; }
        public DateTime DogumTarihi { get; set; }
        public string Telefon { get; set; }
        public string Adres { get; set; }
        public int Ili { get; set; }
        public int Ilcesi { get; set; }
        public string TaahhutAciklama { get; set; }
        public DateTime EvrakTarihi { get; set; }
        public string EvrakSayisi { get; set; }
        public string Sag_vefat { get; set; }
        public DateTime VefatTarihi { get; set; }


        public int Save()
        {
            return new TasinmazTaahhutService().Save(this);
        }

        public bool Update()
        {
            return new TasinmazTaahhutService().Update(this);
        }

        public bool Delete()
        {
            return new TasinmazTaahhutService().Delete(this);
        }

    }
}
