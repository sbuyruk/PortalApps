using Model.Ortak;
using Model.Services.TBYS;
using System;
using System.Collections.Generic;

namespace Model.TBYS
{
    [Serializable]
    public class GecikmeZammi : EntityBase
    {
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public decimal ZamOrani { get; set; }
        public string Aciklama { get; set; }

        public T Select<T>(int id)
        {
            return (T)Convert.ChangeType(new GecikmeZammiService().GetById(id), typeof(T));
        }

        public int Save()
        {
            return new GecikmeZammiService().Save(this);
        }

        public bool Update()
        {
            return new GecikmeZammiService().Update(this);
        }

        public bool Delete()
        {
            return new GecikmeZammiService().Delete(this);
        }

        public List<T> SelectAll<T>()
        {
            return (List<T>)Convert.ChangeType(
                new GecikmeZammiService().GetAll(),
                typeof(List<T>));
        }

        public List<GecikmeZammi> SelectBuAyIcindeDegisen(DateTime ilkOdemeTarihi, DateTime sonOdemeTarihi)
        {
            return new GecikmeZammiService().GetChangedBetween(ilkOdemeTarihi, sonOdemeTarihi);
        }

        public List<GecikmeZammi> SelectByBaslangicTarihi(DateTime baslangicTarihi)
        {
            return new GecikmeZammiService().GetByStartDate(baslangicTarihi);
        }

        public List<GecikmeZammi> SelectByBaslangicTarihi(DateTime vadeBaslangicTarihi, DateTime vadeBitisTarihi)
        {
            return new GecikmeZammiService().GetByDateRange(vadeBaslangicTarihi, vadeBitisTarihi);
        }

        public GecikmeZammi SelectSonDegisenByTarih(DateTime sonOdemeTar)
        {
            return new GecikmeZammiService().GetLatestByDate(sonOdemeTar);
        }

        public GecikmeZammi SelectOncekiGecikmeZammi(DateTime tarih)
        {
            return new GecikmeZammiService().GetPrevious(tarih);
        }

        public GecikmeZammi SelectSonrakiGecikmeZammi(DateTime tarih)
        {
            return new GecikmeZammiService().GetNext(tarih);
        }

        public List<GecikmeZammi> SelectByTarih(DateTime ilkOdemeTarihi, DateTime sonOdemeTarihi)
        {
            return new GecikmeZammiService().GetByDate(ilkOdemeTarihi, sonOdemeTarihi);
        }

        public GecikmeZammi SelectSonrakiGecikmeZammi()
        {
            return new GecikmeZammiService().GetLatest();
        }
    }
}
