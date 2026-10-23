using Model.Ortak;
using Model.Services.Ortak;
using Model.Services.IKYS;
using System;

namespace Model.IKYS
{
    [Serializable]
    public class IsBilgileriItem : EntityBase
    {
        [field: NonSerialized]
        internal Utility.Logging.PortalLogContext LoggingContext { get; set; }
        public int PersonelId { get; set; }
        private IsBilgileri _IsBilgileri
        {
            get
            {
                return (LoggingContext == null ? new IsBilgileriService().GetByPersonelId(PersonelId) : new IsBilgileriService().GetByPersonelId(PersonelId, LoggingContext));
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
                return (LoggingContext == null ? new UnvanTanimService().GetById(_IsBilgileri.UnvanId) : new UnvanTanimService().GetById(_IsBilgileri.UnvanId, LoggingContext));
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
                return (LoggingContext == null ? new BirimTanimService().GetById(_IsBilgileri.BirimId) : new BirimTanimService().GetById(_IsBilgileri.BirimId, LoggingContext));
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
                return (LoggingContext == null ? new BolgeService().GetById(_Birim.BolgeId) : new BolgeService().GetById(_Birim.BolgeId, LoggingContext));
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
