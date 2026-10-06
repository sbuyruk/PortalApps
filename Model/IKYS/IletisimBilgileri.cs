using Model.Ortak;
using System;

namespace Model.IKYS
{
    public class IletisimBilgileri : EntityBase
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

    }
}
