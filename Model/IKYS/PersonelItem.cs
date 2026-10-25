using Model.Ortak;
using Model.Services.IKYS;
using System;
using System.Collections.Generic;

namespace Model.IKYS
{
    [Serializable]
    public class PersonelItem : EntityBase
    {
        public PersonelItem(Utility.Logging.PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context", "Log bağlamı gereklidir.");
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
        private Personel _PersonelBilgileri
        {
            get
            {
                return new PersonelService().GetById(PersonelId, LoggingContext);
            }
        }
        private Kimlik _KimlikBilgileri
        {
            get
            {
                return new KimlikService().GetById(PersonelId, LoggingContext);
            }
        }
        private IsBilgileriItem _IsBilgileriItem
        {
            get
            {
                IsBilgileriItem isBilgileriItem = new IsBilgileriItem(LoggingContext);
                isBilgileriItem.PersonelId = PersonelId;
                return isBilgileriItem;
            }
        }
        private IletisimBilgileri _IletisimBilgileri
        {
            get
            {
                return new IletisimBilgileriService().GetByPersonelId(PersonelId, LoggingContext);
            }
        }
        private List<Aile> _AileBilgileri
        {
            get
            {
                return new AileService().GetByPersonelId(PersonelId, LoggingContext);
            }
        }
        public Personel PersonelBilgileri
        {
            get { return _PersonelBilgileri; }
        }
        public Kimlik KimlikBilgileri
        {
            get { return _KimlikBilgileri; }
        }
        public IsBilgileriItem IsBilgileriItem
        {
            get { return _IsBilgileriItem; }
        }
        public IletisimBilgileri IletisimBilgileri
        {
            get { return _IletisimBilgileri; }
        }
        public List<Aile> AileBilgileri
        {
            get { return _AileBilgileri; }
        }

    }
}
