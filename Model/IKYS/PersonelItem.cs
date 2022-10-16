using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.IKYS
{
    [Serializable]
    public class PersonelItem : ParentClass
    {
        public PersonelItem()
        {
        }

        public int PersonelId { get; set; }
        private Personel _PersonelBilgileri
        {
            get
            {
                Personel personel = new Personel();
                personel = personel.Select(PersonelId);
                return personel;
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
                Kimlik kimlik = new Kimlik();
                kimlik = kimlik.Select(PersonelId);
                return kimlik;
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
                IletisimBilgileri iletisimBilgileri = new IletisimBilgileri();
                iletisimBilgileri = iletisimBilgileri.SelectByPersonelId(PersonelId);
                return iletisimBilgileri;
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
                Aile aile = new Aile();
                List<Aile> list = aile.SelectByPersonelId(PersonelId);
                return list;
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

        public override bool Delete()
        {
            throw new NotImplementedException();
        }
        public override int Save()
        {
            throw new NotImplementedException();
        }
        public override T Select<T>(int id)
        {
            throw new NotImplementedException();
        }
        public override List<T> SelectAll<T>()
        {
            throw new NotImplementedException();
        }
        public override bool Update()
        {
            throw new NotImplementedException();
        }
    }
}
