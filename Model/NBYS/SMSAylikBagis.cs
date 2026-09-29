using Model.Ortak;
using System;

namespace Model.NBYS
{
    [Serializable]
    public class SMSAylikBagis : EntityBase
    {
        public int Yil { get; set; }
        public int Ay { get; set; }
        public int TurkcellSMSAdedi { get; set; }
        public int VodafoneSMSAdedi { get; set; }
        public int TurkTelekomSMSAdedi { get; set; }
        public decimal SMSTutari { get; set; }
        public string DovizCinsi { get; set; }
        public DateTime IslemTarihi { get; set; }
        public string Aciklama { get; set; }
    }
}
