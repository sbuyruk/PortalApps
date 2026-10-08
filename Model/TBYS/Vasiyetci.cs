using Model.Ortak;
using Model.Services.TBYS;
using System;
using System.Collections.Generic;
using System.Data;

namespace Model.TBYS
{
    [Serializable]
    public class Vasiyetci : EntityBase
    {
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public long TCKimlikNo { get; set; }
        public string SagVefat { get; set; }
        public DateTime VefatTarihi { get; set; }
        public int IkametIli { get; set; }
        public int IkametIlcesi { get; set; }
        public string Telefon1 { get; set; }
        public string Telefon2 { get; set; }
        public DateTime DogumTarihi { get; set; }
        public string DogumYeri { get; set; }
        public string IkametAdresi { get; set; }
        public string VasiyetTipi { get; set; }
        public string VasiyetinDurumu { get; set; }
        public string Noter { get; set; }
        public DateTime VasiyetTarihi { get; set; }
        public string YevmiyeNumarasi { get; set; }
        public string VasiyetcininTalebi { get; set; }
        public string Aciklama { get; set; }

        public int Save()
        {
            return new VasiyetciService().Save(this);
        }

        public bool Update()
        {
            return new VasiyetciService().Update(this);
        }

        public bool Delete()
        {
            return new VasiyetciService().Delete(this);
        }

        public Vasiyetci Select(int id)
        {
            return new VasiyetciService().GetById(id);
        }

        public T Select<T>(int id)
        {
            return (T)Convert.ChangeType(new VasiyetciService().GetById(id), typeof(T));
        }

        public List<T> SelectAll<T>()
        {
            return (List<T>)Convert.ChangeType(
                new VasiyetciService().GetAll(),
                typeof(List<T>));
        }

        public DataTable SelectByBolgeReturnDataTable(int bolgeId)
        {
            return new VasiyetciService().GetByRegion(bolgeId);
        }

        public string SelectVasiyetciByIdReturnJson(int vasiyetciId, ref int rowCount)
        {
            return new VasiyetciService().GetByIdAsJson(vasiyetciId, ref rowCount);
        }

        public DataTable SelectAllVasiyetciReturnDataTable(bool VefatEdenVasiyetcilerHaric, ref int rowCount)
        {
            return new VasiyetciService().GetAllForDataTable(VefatEdenVasiyetcilerHaric, ref rowCount);
        }

        public List<Vasiyetci> SelectByFilters(bool SadeceSagOlanlar, bool isTCKimlikNoFull, bool isDogumTarihiFull)
        {
            return new VasiyetciService().GetByFilters(SadeceSagOlanlar, isTCKimlikNoFull, isDogumTarihiFull);
        }
    }
}
