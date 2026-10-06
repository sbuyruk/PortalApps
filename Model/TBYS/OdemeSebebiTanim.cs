using Model.Ortak;
using System;

namespace Model.TBYS
{
    [Serializable]
    public class OdemeSebebiTanim : EntityBase
    {
        public string OdemeSebebi { get; set; }
        public string Aciklama { get; set; }

    }
}
