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
        public PersonelItem(Utility.Logging.PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context", "Log bağlamı gereklidir.");
            LoggingContext = context;
        }

        [field: NonSerialized]
        internal Utility.Logging.PortalLogContext LoggingContext { get; set; }
        public int PersonelId { get; set; }
        private Personel _PersonelBilgileri
        {
            get
            {
                return (LoggingContext == null ? new PersonelService().GetById(PersonelId) : new PersonelService().GetById(PersonelId, LoggingContext));
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
                return (LoggingContext == null ? new KimlikService().GetById(PersonelId) : new KimlikService().GetById(PersonelId, LoggingContext));
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
                IsBilgileriItem isBilgileriItem = new IsBilgileriItem { LoggingContext = LoggingContext };
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
                return (LoggingContext == null ? new IletisimBilgileriService().GetByPersonelId(PersonelId) : new IletisimBilgileriService().GetByPersonelId(PersonelId, LoggingContext));
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
                return (LoggingContext == null ? new AileService().GetByPersonelId(PersonelId) : new AileService().GetByPersonelId(PersonelId, LoggingContext));
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
