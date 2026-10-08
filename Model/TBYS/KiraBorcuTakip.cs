using DAO.Ortak;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.TBYS
{
    [Serializable]
    public class KiraBorcuTakip : ParentClass
    {
        public int KiraciId { get; set; }
        public int KiraSozlesmeId { get; set; }
        public int OdemePlaniId { get; set; }
        public decimal KiraBedeli { get; set; }
        public decimal ToplamBorcu { get; set; }
        public int KiraBorcuAySayisi { get; set; }
        public string TakipIslemi { get; set; }
        public int IslemAyi { get; set; }
        public int IslemYili { get; set; }
        public string IslemYapan { get; set; }
        public DateTime IslemTarihi { get; set; }
        public string TebligEdilenKisi { get; set; }
        public DateTime TebligTarihi { get; set; }
        public string Bolge { get; set; }
        public string Aciklama { get; set; }

        public override T Select<T>(int id)
        {
            return (T)Convert.ChangeType(new KiraBorcuTakipService().GetById(id), typeof(T));

        }
        public KiraBorcuTakip Select(int kiraBorcuTakipId)
        {
            return new KiraBorcuTakipService().GetById(kiraBorcuTakipId);

        }

        public override int Save()
        {
            return new KiraBorcuTakipService().Save(this);
        }
        public override bool Update()
        {
            return new KiraBorcuTakipService().Update(this);
        }
        public override bool Delete()
        {
            return new KiraBorcuTakipService().Delete(this);
        }
        public override List<T> SelectAll<T>()
        {
            return (List<T>)Convert.ChangeType(new KiraBorcuTakipService().GetAll(), typeof(List<T>));
        }

        public KiraBorcuTakip SelectByKiraciIdAyYil(int kiraciId)
        {
            return new KiraBorcuTakipService().GetByKiraciIdAyYil(kiraciId);
        }
        public int SelectCountAdetByTakipIslemiBolge(string takipIslemi, string bolge, int ay, int yil)
        {
            return new KiraBorcuTakipService().GetCountByFilters(takipIslemi, bolge, ay, yil);

        }
        public int SelectCountBySozlesmeId(int kiraSozlesmeId, string takipIslemi)
        {
            return new KiraBorcuTakipService().GetCountBySozlesmeId(kiraSozlesmeId, takipIslemi);

        }
    }
}
