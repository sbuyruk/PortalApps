using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.ProjeGlobal;

namespace Model.IKYS
{
    public sealed class PersonelItemListFactory
    {
        private PersonelItemListFactory()
        {
            GeneratePersonelList();
        }
        #region Lazy initialize
        private static readonly Lazy<PersonelItemListFactory> lazy =
                new Lazy<PersonelItemListFactory>(() => new PersonelItemListFactory());

        public static PersonelItemListFactory Instance { get { return lazy.Value; } }
        #endregion 

        #region Eager Initialize
        //private static readonly PersonelItemListFactory instance = new PersonelItemListFactory();

        //// Explicit static constructor to tell C# compiler
        //// not to mark type as beforefieldinit
        //static PersonelItemListFactory()
        //{
        //}
        //public static PersonelItemListFactory Instance
        //{
        //    get
        //    {
        //        return instance;
        //    }
        //}
        #endregion

        private List<PersonelItem> _PersonelItemList { get; set; }

        private List<PersonelItem> _CalisanPersonelItemList { get; set; }
        private List<PersonelItem> _AyrilanPersonelItemList { get; set; }

        public List<PersonelItem> PersonelItemList
        {
            get
            {
                
                return _PersonelItemList;
            }
            set
            {
                _PersonelItemList = value;
            }
        }
        public List<PersonelItem> CalisanPersonelItemList
        {
            get
            {
                return _CalisanPersonelItemList;
            }
            set
            {
                _CalisanPersonelItemList = value;
            }
        }
        public List<PersonelItem> AyrilanPersonelItemList
        {
            get
            {
                return _AyrilanPersonelItemList;
            }
            set
            {
                _AyrilanPersonelItemList = value;
            }
        }

        private void GeneratePersonelList() 
        {
            _PersonelItemList = new List<PersonelItem>();
            _CalisanPersonelItemList = new List<PersonelItem>();
            _AyrilanPersonelItemList = new List<PersonelItem>();

            Personel personelDao = new Personel();
            List<Personel> PersonelList = personelDao.SelectAll<Personel>();
            foreach (var item in PersonelList)
            {
                PersonelItem personelItem = new PersonelItem();
                personelItem.PersonelId = item.Id;
                _PersonelItemList.Add(personelItem);
                if (personelItem.IsBilgileriItem.IsBilgileri.CalismaDurumu == ProjeConstants.PER_CALISIYOR_INT)
                {
                    _CalisanPersonelItemList.Add(personelItem);
                }
                else if (personelItem.IsBilgileriItem.IsBilgileri.CalismaDurumu == ProjeConstants.PER_AYRILDI_INT)
                {
                    _AyrilanPersonelItemList.Add(personelItem);
                }
            }
        }
    }
}
