using Model.Ortak;
using Model.Services.Ortak;
using Model.Services.IKYS;
using System;

namespace Model.IKYS
{
    [Serializable]
    public class IsBilgileriItem : EntityBase
    {
        public IsBilgileriItem(Utility.Logging.PortalLogContext context)
        {
            LoggingContext = context;
        }

        [NonSerialized]
        private Utility.Logging.PortalLogContext loggingContext;
        internal Utility.Logging.PortalLogContext LoggingContext
        {
            get
            {
                if (loggingContext == null)
                    throw new InvalidOperationException("Personel bilgilerini okumak için log bağlamı gereklidir.");
                return loggingContext;
            }
            set
            {
                if (value == null) throw new ArgumentNullException("value", "Log bağlamı gereklidir.");
                loggingContext = value;
            }
        }
        public int PersonelId { get; set; }
        private IsBilgileri _IsBilgileri
        {
            get
            {
                return new IsBilgileriService().GetByPersonelId(PersonelId, LoggingContext);
            }
        }
        private UnvanTanim _Unvan
        {
            get
            {
                return new UnvanTanimService().GetById(_IsBilgileri.UnvanId, LoggingContext);
            }
        }
        private BirimTanim _Birim
        {
            get
            {
                return new BirimTanimService().GetById(_IsBilgileri.BirimId, LoggingContext);
            }
        }
        private Bolge _Bolge
        {
            get
            {
                return new BolgeService().GetById(_Birim.BolgeId, LoggingContext);
            }
        }
        public IsBilgileri IsBilgileri
        {
            get { return _IsBilgileri; }
        }
        public UnvanTanim Unvan
        {
            get { return _Unvan; }
        }
        public BirimTanim Birim
        {
            get { return _Birim; }
        }
        public Bolge Bolge
        {
            get { return _Bolge; }
        }
    }
}
