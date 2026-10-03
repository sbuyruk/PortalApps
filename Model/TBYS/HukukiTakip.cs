using Model.Ortak;
using Model.Services.TBYS;
using System;
using System.Collections.Generic;
using System.Data;

namespace Model.TBYS
{
    [Serializable]
    public class HukukiTakip : ParentClass
    {
        public int SozlesmeId { get; set; }
        public int KiraciId { get; set; }
        public decimal BorcAnaPara { get; set; }
        public decimal BorcFaiz { get; set; }
        public DateTime IslemTarihi { get; set; }
        public string Aciklama { get; set; }
        public bool Aktif { get; set; }

        public override T Select<T>(int id)
        {
            return (T)Convert.ChangeType(new HukukiTakipService().GetById(id), typeof(T));
        }

        public override int Save()
        {
            return new HukukiTakipService().Save(this);
        }

        public override bool Update()
        {
            return new HukukiTakipService().Update(this);
        }

        public override bool Delete()
        {
            return new HukukiTakipService().Delete(this);
        }

        public bool DeleteBySozlesmeId(int sozlesmeId)
        {
            return new HukukiTakipService().DeleteBySozlesmeId(sozlesmeId);
        }

        public string SelectAllReturnJson()
        {
            return new HukukiTakipService().GetActiveListAsJson();
        }

        public DataTable SelectAllReturnDataTable()
        {
            return new HukukiTakipService().GetActiveList();
        }

        public override List<T> SelectAll<T>()
        {
            return (List<T>)Convert.ChangeType(
                new HukukiTakipService().GetAll(),
                typeof(List<T>));
        }

        public HukukiTakip SelectBySozlesmeId(int sozlesmeId)
        {
            return new HukukiTakipService().GetBySozlesmeId(sozlesmeId);
        }
    }
}
