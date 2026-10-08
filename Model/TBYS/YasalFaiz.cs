using Model.Ortak;
using Model.Services.TBYS;
using System;
using System.Collections.Generic;

namespace Model.TBYS
{
    [Serializable]
    public class YasalFaiz : ParentClass
    {
        public int Yil { get; set; }
        public int Ay { get; set; }
        public string AyAdi { get; set; }
        public decimal FaizOrani { get; set; }
        public decimal Tufe { get; set; }
        public decimal Ufe { get; set; }
        public string Aciklama { get; set; }
        public override T Select<T>(int id)
        {
            return (T)Convert.ChangeType(new YasalFaizService().GetById(id), typeof(T));
        }
        public override int Save()
        {
            return new YasalFaizService().Save(this);
        }
        public override bool Update()
        {
            return new YasalFaizService().Update(this);
        }
        public override bool Delete()
        {
            return new YasalFaizService().Delete(this);
        }
        public override List<T> SelectAll<T>()
        {
            return (List<T>)Convert.ChangeType(new YasalFaizService().GetAll(), typeof(List<T>));
        }
        public List<YasalFaiz> SelectByYil(int yil)
        {
            return new YasalFaizService().GetByYear(yil);
        }
        public YasalFaiz SelectByYilAy(int yil, int ay)
        {
            return new YasalFaizService().GetByYearMonth(yil, ay);
        }
        public decimal SelectSonFaizOrani()
        {
            return new YasalFaizService().GetLatestRate();
        }
        public decimal SelectSonTufe()
        {
            return new YasalFaizService().GetLatestTufe();
        }
        public decimal SelectSonUfe()
        {
            return new YasalFaizService().GetLatestUfe();
        }
    }
}
