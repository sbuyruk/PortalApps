using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.IKYS
{
    [Serializable]
    public class IsBilgileriItem : ParentClass
    {
        public int PersonelId { get; set; }
        private IsBilgileri _IsBilgileri
        {
            get
            {
                IsBilgileri isBilgileri = new IsBilgileri();
                isBilgileri = isBilgileri.SelectByPersonelId(PersonelId);
                return isBilgileri;
            }
            set
            {
                _IsBilgileri = value;
            }
        }
        private UnvanTanim _Unvan
        {
            get
            {
                UnvanTanim unvan = new UnvanTanim();
                unvan = unvan.Select<UnvanTanim>(_IsBilgileri.UnvanId);
                return unvan;
            }
            set
            {
                _Unvan = value;
            }
        }
        private BirimTanim _Birim
        {
            get
            {
                BirimTanim birim = new BirimTanim();
                birim = birim.Select<BirimTanim>(_IsBilgileri.BirimId);
                return birim;
            }
            set
            {
                _Birim = value;
            }
        }
        private Bolge _Bolge
        {
            get
            {
                Bolge bolge = new Bolge();
                bolge = bolge.Select<Bolge>(_Birim.BolgeId);
                return bolge;
            }
            set
            {
                _Bolge = value;
            }
        }
        public IsBilgileri IsBilgileri
        {
            get { return _IsBilgileri; }
            set { _IsBilgileri = value; }
        }
        public UnvanTanim Unvan
        {
            get { return _Unvan; }
            set { _Unvan = value; }
        }
        public BirimTanim Birim
        {
            get { return _Birim; }
            set { _Birim = value; }
        }
        public Bolge Bolge
        {
            get { return _Bolge; }
            set { _Bolge = value; }
        }
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
