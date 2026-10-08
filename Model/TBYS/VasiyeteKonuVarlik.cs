using Model.Ortak;
using System;

namespace Model.TBYS
{
    public class VasiyeteKonuVarlik : EntityBase
    {
        public int VasiyetciId { get; set; }
        public string Konusu { get; set; }
        public string Cinsi { get; set; }
        public string AdetMiktar { get; set; }
        public decimal TahminiRayic { get; set; }
        public string Aciklama { get; set; }

    }
}
