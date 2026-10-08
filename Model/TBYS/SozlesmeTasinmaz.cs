using Model.Ortak;
using Model.Services.TBYS;
using System;
using System.Collections.Generic;
using System.Data;

namespace Model.TBYS
{
    [Serializable]
    public class SozlesmeTasinmaz : EntityBase
    {
        public int SozlesmeId { get; set; }
        public int TasinmazId { get; set; }
        public int BolumId { get; set; }

        public T Select<T>(int id)
        {
            return (T)Convert.ChangeType(new SozlesmeTasinmazService().GetById(id), typeof(T));
        }

        public int Save() { return new SozlesmeTasinmazService().Save(this); }
        public bool Update() { return new SozlesmeTasinmazService().Update(this); }
        public bool Delete() { return new SozlesmeTasinmazService().Delete(this); }

        public bool DeleteBySozlesmeId(int sozlesmeId)
        {
            return new SozlesmeTasinmazService().DeleteBySozlesmeId(this, sozlesmeId);
        }

        public List<T> SelectAll<T>()
        {
            return (List<T>)Convert.ChangeType(new SozlesmeTasinmazService().GetAll(), typeof(List<T>));
        }

        public string SelectBySozlesmeIdReturnJson(int sozlesmeId)
        {
            return new SozlesmeTasinmazService().GetBySozlesmeIdReturnJson(sozlesmeId);
        }

        public DataTable SelectBySozlesmeIdReturnDataTable(int sozlesmeId)
        {
            return new SozlesmeTasinmazService().GetBySozlesmeIdReturnList(sozlesmeId);
        }

        public List<SozlesmeTasinmaz> SelectByTasinmazId(int tasinmazId)
        {
            return new SozlesmeTasinmazService().GetByTasinmazId(tasinmazId);
        }

        public List<SozlesmeTasinmaz> SelectBySozlesmeId(int sozlesmeId)
        {
            return new SozlesmeTasinmazService().GetBySozlesmeId(sozlesmeId);
        }

        public decimal SelectSumMetrekareBySozlesmeId(int sozlesmeId)
        {
            return new SozlesmeTasinmazService().GetSumMetrekareBySozlesmeId(sozlesmeId);
        }

        public DataTable SelectBySozlesmeIdReturnDT(int sozlesmeId)
        {
            return new SozlesmeTasinmazService().GetBySozlesmeIdReturnDT(sozlesmeId);
        }

        public List<SozlesmeTasinmaz> SelectBySozlesmeIdTasinmazId(int sozlesmeId, int tasinmazId, int bolumId)
        {
            return new SozlesmeTasinmazService().GetBySozlesmeIdTasinmazId(sozlesmeId, tasinmazId, bolumId);
        }

        public List<SozlesmeTasinmaz> SelectByBolumId(int bolumId)
        {
            return new SozlesmeTasinmazService().GetByBolumId(bolumId);
        }
    }
}
