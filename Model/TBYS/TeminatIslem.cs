using Model.Ortak;
using Model.Services.TBYS;
using System;
using System.Collections.Generic;
using System.Data;
using Utility.ProjeGlobal;

namespace Model.TBYS
{
    [Serializable]
    public class TeminatIslem : EntityBase
    {
        public int KiraciId { get; set; }
        public int DosyaNo { get; set; }
        public string IslemTipi { get; set; }
        public DateTime IslemTarihi { get; set; }
        public decimal IslemTutari { get; set; }
        public string DovizCinsi { get; set; }
        public string Aciklama { get; set; }
        public int OdemeId { get; set; }

        public int Save() { return new TeminatIslemService().Save(this); }
        public bool Update() { return new TeminatIslemService().Update(this); }
        public bool Delete() { return new TeminatIslemService().Delete(this); }
        public bool DeleteBySozlesmeId(int id) { return new TeminatIslemService().DeleteBySozlesmeId(id); }

    }
}
