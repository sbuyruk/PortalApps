using Model.Ortak;
using Model.Services.TBYS;
using System;
using System.Collections.Generic;
using System.Data;

namespace Model.TBYS
{
    [Serializable]
    public class TasinmazTaahhut : ParentClass
    {
        public int TasinmazId { get; set; }
        public int BagisciId { get; set; }
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public long TCKimlikNo { get; set; }
        public DateTime DogumTarihi { get; set; }
        public string Telefon { get; set; }
        public string Adres { get; set; }
        public int Ili { get; set; }
        public int Ilcesi { get; set; }
        public string TaahhutAciklama { get; set; }
        public DateTime EvrakTarihi { get; set; }
        public string EvrakSayisi { get; set; }
        public string Sag_vefat { get; set; }
        public DateTime VefatTarihi { get; set; }

        public override T Select<T>(int id)
        {
            return (T)Convert.ChangeType(new TasinmazTaahhutService().GetById(id), typeof(T));
        }

        public override int Save()
        {
            return new TasinmazTaahhutService().Save(this);
        }

        public override bool Update()
        {
            return new TasinmazTaahhutService().Update(this);
        }

        public override bool Delete()
        {
            return new TasinmazTaahhutService().Delete(this);
        }

        public override List<T> SelectAll<T>()
        {
            return (List<T>)Convert.ChangeType(
                new TasinmazTaahhutService().GetAll(),
                typeof(List<T>));
        }

        public List<TasinmazTaahhut> SelectByBagisciId(int bagisciId)
        {
            return new TasinmazTaahhutService().GetByBagisciId(bagisciId);
        }

        public TasinmazTaahhut SelectByTCKimlikNo(long tcKimlikNo)
        {
            return new TasinmazTaahhutService().GetByTcKimlikNo(tcKimlikNo);
        }

        public List<TasinmazTaahhut> SelectByFilters(bool isSagVefat, bool isTCKimlikNoFull, bool isDogumTarihiFull, int bolgeId)
        {
            return new TasinmazTaahhutService().GetByFilters(isSagVefat, isTCKimlikNoFull, isDogumTarihiFull, bolgeId);
        }

        public List<TasinmazTaahhut> SelectByIlAdi(string ilAdi)
        {
            return new TasinmazTaahhutService().GetByIlAdi(ilAdi);
        }

        public string SelectAllCountBagisAdediReturnJson()
        {
            return new TasinmazTaahhutService().GetAllCountDonationAsJson();
        }

        public DataTable SelectAllCountBagisAdediReturnDataTable(bool vefatEdenBagiscilarHaric, bool gizliBagiscilarHaric)
        {
            return new TasinmazTaahhutService().GetAllCountDonation(vefatEdenBagiscilarHaric, gizliBagiscilarHaric);
        }
    }
}
