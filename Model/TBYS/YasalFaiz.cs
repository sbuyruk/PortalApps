using Model.Ortak;
using Model.Services.TBYS;
using System;
using System.Collections.Generic;

namespace Model.TBYS
{
    [Serializable]
    public class YasalFaiz : EntityBase
    {
        public int Yil { get; set; }
        public int Ay { get; set; }
        public string AyAdi { get; set; }
        public decimal FaizOrani { get; set; }
        public decimal Tufe { get; set; }
        public decimal Ufe { get; set; }
        public string Aciklama { get; set; }
        public T Select<T>(int id)
        {
            return (T)Convert.ChangeType(new YasalFaizService().GetById(id), typeof(T));
        }
        public int Save()
        {
            return new YasalFaizService().Save(this);
        }
        public bool Update()
        {
            return new YasalFaizService().Update(this);
        }
        public bool Delete()
        {
            return new YasalFaizService().Delete(this);
        }
        public List<T> SelectAll<T>()
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
