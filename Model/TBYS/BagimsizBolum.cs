using DAO.Ortak;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.TBYS
{
    [Serializable]
    public class BagimsizBolum : EntityBase
    {
        public int TasinmazId { get; set; }
        public string BolumNo { get; set; }
        public string Nitelik{ get; set; }
        public decimal BBBrutAlan { get; set; }
        public decimal BBNetAlan { get; set; }
        public string KullanimAmaci { get; set; }
        public decimal MuhasebeyeKayitliDeger { get; set; }
        public decimal TahminiRayicDegeri { get; set; }
        public decimal EmlakBeyanDegeri { get; set; }
        public decimal YaklasikPiyasaDegeri { get; set; }
        public string Aciklama { get; set; }

        public T Select<T>(int id)
        {
            return (T)Convert.ChangeType(new BagimsizBolumService().GetById(id), typeof(T));

        }
        public int Save()
        {
            return new BagimsizBolumService().Save(this);
        }
        public bool Update()
        {
            return new BagimsizBolumService().Update(this);
        }
        public bool Delete()
        {
            return new BagimsizBolumService().Delete(this);
        }
        public List<T> SelectAll<T>()
        {
            return (List<T>)Convert.ChangeType(new BagimsizBolumService().GetAll(), typeof(List<T>));
        }
        public List<BagimsizBolum> SelectByTasinmazId(int tasinmazId)
        {
            return new BagimsizBolumService().GetByTasinmazId(tasinmazId);
        }
        public List<BagimsizBolum> SelectByBolumNO(string bolumNo)
        {
            return new BagimsizBolumService().GetByBolumNo(bolumNo);
        }
        public BagimsizBolum SelectByBolumId(int bolumId)
        {
            return new BagimsizBolumService().GetByBolumId(bolumId);
        }
    }
}
