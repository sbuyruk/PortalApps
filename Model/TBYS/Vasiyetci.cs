using Model.Ortak;
using System;

namespace Model.TBYS
{
    [Serializable]
    public class Vasiyetci : EntityBase
    {
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public long TCKimlikNo { get; set; }
        public string SagVefat { get; set; }
        public DateTime VefatTarihi { get; set; }
        public int IkametIli { get; set; }
        public int IkametIlcesi { get; set; }
        public string Telefon1 { get; set; }
        public string Telefon2 { get; set; }
        public DateTime DogumTarihi { get; set; }
        public string DogumYeri { get; set; }
        public string IkametAdresi { get; set; }
        public string VasiyetTipi { get; set; }
        public string VasiyetinDurumu { get; set; }
        public string Noter { get; set; }
        public DateTime VasiyetTarihi { get; set; }
        public string YevmiyeNumarasi { get; set; }
        public string VasiyetcininTalebi { get; set; }
        public string Aciklama { get; set; }
    }
}
