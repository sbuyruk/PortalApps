using Model.Ortak;
using Model.Services.IKYS;
using System;
using System.Collections.Generic;

namespace Model.IKYS
{
    [Serializable]
    public class PersonelItem : EntityBase
    {
        public PersonelItem()
        {
        }

        public int PersonelId { get; set; }
        private Personel _PersonelBilgileri
        {
            get
            {
                return new PersonelService().GetById(PersonelId);
            }
            set
            {
                _PersonelBilgileri = value; 
            }
        }
        private Kimlik _KimlikBilgileri
        {
            get
            {
                return new KimlikService().GetById(PersonelId);
            }
            set
            {
                _KimlikBilgileri = value;
            }
        }
        private IsBilgileriItem _IsBilgileriItem
        {
            get
            {
                IsBilgileriItem isBilgileriItem = new IsBilgileriItem();
                isBilgileriItem.PersonelId = PersonelId;
                return isBilgileriItem;
            }
            set
            {
                _IsBilgileriItem = value;
            }
        }
        private IletisimBilgileri _IletisimBilgileri
        {
            get
            {
                return new IletisimBilgileriService().GetByPersonelId(PersonelId);
            }
            set
            {
                _IletisimBilgileri = value;
            }
        }
        private List<Aile> _AileBilgileri
        {
            get
            {
                return new AileService().GetByPersonelId(PersonelId);
            }
            set
            {
                _AileBilgileri = value;
            }
        }
        public Personel PersonelBilgileri
        {
            get { return _PersonelBilgileri; }
            set { _PersonelBilgileri = value; }
        }
        public Kimlik KimlikBilgileri
        {
            get { return _KimlikBilgileri; }
            set { _KimlikBilgileri = value; }
        }
        public IsBilgileriItem IsBilgileriItem
        {
            get { return _IsBilgileriItem; }
            set { _IsBilgileriItem = value; }
        }
        public IletisimBilgileri IletisimBilgileri
        {
            get { return _IletisimBilgileri; }
            set { _IletisimBilgileri = value; }
        }
        public List<Aile> AileBilgileri
        {
            get { return _AileBilgileri; }
            set { _AileBilgileri = value; }
        }
        //buttonlar
        //public bool Secildi { get; set; }
        //public string Duzenle { get; set; }
        //public string Baglanti1 { get; set; }
        //public string Baglanti2 { get; set; }

    }
}
