using Model.Ortak;
using Model.Services.MTS;
using System;
using System.Collections.Generic;

namespace Model.MTS
{
    public class FaaliyetKatilim : EntityBase
    {
        public int FaaliyetId { get; set; }
        public int KatilimciId { get; set; }
        public int KatilimciTipi { get; set; } = 2;
        public string KurumGorev { get; set; }
        public string TakvimDaveti { get; set; }
        public string Aciklama { get; set; }

        public int Save()
        {
            return new FaaliyetKatilimService().Save(this);
        }

        public bool Update()
        {
            return new FaaliyetKatilimService().Update(this);
        }

        public bool Delete()
        {
            return new FaaliyetKatilimService().Delete(this);
        }

        public FaaliyetKatilim Select(int id)
        {
            Id = id;
            return new FaaliyetKatilimService().GetById(id);
        }

        public List<FaaliyetKatilim> Select(int faaliyetId, int katilimciId)
        {
            return new FaaliyetKatilimService().GetByFaaliyetAndKatilimci(faaliyetId, katilimciId);
        }

        public List<FaaliyetKatilim> SelectByKatilimciId(int katilimciId)
        {
            return new FaaliyetKatilimService().GetByKatilimciId(katilimciId);
        }

        public List<FaaliyetKatilim> SelectByFaaliyetId(int faaliyetId)
        {
            return new FaaliyetKatilimService().GetByFaaliyetId(faaliyetId);
        }

        public T Select<T>(int id)
        {
            Id = id;
            return (T)Convert.ChangeType(new FaaliyetKatilimService().GetById(id), typeof(T));
        }

        public List<T> SelectAll<T>()
        {
            return (List<T>)Convert.ChangeType(new FaaliyetKatilimService().GetAll(), typeof(List<T>));
        }
    }
}
