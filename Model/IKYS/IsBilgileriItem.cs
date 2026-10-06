using Model.Ortak;
using Model.Services.Ortak;
using Model.Services.IKYS;
using System;

namespace Model.IKYS
{
    [Serializable]
    public class IsBilgileriItem : EntityBase
    {
        public int PersonelId { get; set; }
        private IsBilgileri _IsBilgileri
        {
            get
            {
                return new IsBilgileriService().GetByPersonelId(PersonelId);
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
                return new UnvanTanimService().GetById(_IsBilgileri.UnvanId);
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
                return new BirimTanimService().GetById(_IsBilgileri.BirimId);
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
                return new BolgeService().GetById(_Birim.BolgeId);
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
    }
}
