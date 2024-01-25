using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.ProjeGlobal;
using Model.IKYS;
using Model.NBYS;
using Model.TBYS;

namespace Model.MTS
{
    public class Katilimci : ParentClass
    {
        [Required]
        [DisplayName("Katılımcı Tipi")]
        public int KatilimciTipi { get; set; }

        [Required]
        [DisplayName("Adı")]
        public string Adi { get; set; }
        [Required]
        [DisplayName("Soyadı")]
        public string Soyadi { get; set; }
        [DisplayName("TC Kimlik No")]
        public long TCKimlikNo { get; set; }
        [DisplayName("Kurumu")]
        public string Kurumu { get; set; }
        [DisplayName("Ünvanı")]
        public string Unvani { get; set; }
        [DisplayName("Görevi")]
        public string Gorevi { get; set; }
        [DisplayName("Telefon 1")]
        public string Telefon1 { get; set; }
        [DisplayName("Telefon 2")]
        public string Telefon2 { get; set; }
        
        [DisplayName("Telefon 3")]
        public string Telefon3 { get; set; }
        
        [DisplayName("Açıklama 1")]
        public string TelAciklama1 { get; set; }
        
        [DisplayName("Açıklama 2")]
        public string TelAciklama2 { get; set; }
        
        [DisplayName("Açıklama 3")]
        public string TelAciklama3 { get; set; }
        
        [DisplayName("Adres")]
        public string Adres { get; set; }
            
        [DisplayName("EPosta")]
        public string EPosta { get; set; }
        
        [DisplayName("İl")]
        public int Ili { get; set; }
        
        [DisplayName("İlçe")]
        public int Ilcesi { get; set; }
        
        [DisplayName("Dahili Telefon 1")]
        public string Dahili1 { get; set; }
        
        [DisplayName("Dahili Telefon 2")]
        public string Dahili2 { get; set; }
        
        [DisplayName("Dahili Telefon 3")]
        public string Dahili3 { get; set; }
        
        [DisplayName("Doğum Tarihi")]
        public DateTime DogumTarihi { get; set; }
        
        [DisplayName("Kutlama")]
        public bool Kutlama { get; set; } = false;
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
        public Katilimci GetKatilimci(int katilimciId, int katilimciTipi)
        {
            Katilimci retVal = new Katilimci();
            if (katilimciTipi == ProjeConstants.FAALIYET_KATILIMCI_DIS_INT)
            {

                Kisi kisi = new Kisi();
                kisi = kisi.Select(katilimciId);
                if (kisi != null)
                {
                    MTSKurumGorev kurumGorev = new MTSKurumGorev();
                    string kurum = string.Empty;
                    string gorev = string.Empty;
                    string kurumGorevStr = kurumGorev.SelectByKisiIdReturnKurumGorev(katilimciId, ref kurum, ref gorev);
                   
                    Katilimci katilimci = new Katilimci()
                    {
                        Id = kisi.Id,
                        Adi = kisi.Adi,
                        Adres = kisi.Adres,
                        Dahili1 = kisi.Dahili1,
                        Dahili2 = kisi.Dahili2,
                        Dahili3 = kisi.Dahili3,
                        DogumTarihi = kisi.DogumTarihi,
                        Gorevi = gorev,
                        Ilcesi = kisi.Ilcesi,
                        Ili = kisi.Ili,
                        Kurumu = kurum,
                        Kutlama = kisi.Kutlama,
                        KatilimciTipi = ProjeConstants.FAALIYET_KATILIMCI_DIS_INT,
                        Soyadi = kisi.Soyadi,
                        Unvani = kisi.Unvani,
                        EPosta = kisi.EPosta,
                    };

                    
                    retVal = katilimci;
                }
            }
            else if (katilimciTipi == ProjeConstants.FAALIYET_KATILIMCI_IC_INT)
            {
                Personel personel = new Personel();
                personel = personel.Select(katilimciId);
                if (personel != null)
                {
                    IletisimBilgileri iletisimBilgileriDao = new IletisimBilgileri();
                    IletisimBilgileri iletsimBilgileri = iletisimBilgileriDao.SelectByPersonelId(personel.Id);
                    string ePosta=iletsimBilgileri!=null?iletsimBilgileri.InternetEPosta:string.Empty;

                    Katilimci katilimci = new Katilimci()
                    {
                        Id = katilimciId,
                        Adi = personel.Adi,
                        //Adres = personel.Adres,
                        //Dahili1 = personel.Dahili1,
                        //Dahili2 = personel.Dahili2,
                        //Dahili3 = personel.Dahili3,
                        //DogumTarihi = personel.DogumTarihi,
                        //Gorevi = personel.Gorevi,
                        //Ilcesi = personel.Ilcesi,
                        //Ili = personel.Ili,
                        Kurumu = ProjeConstants.FAALIYET_KATILIMCI_IC,
                        //Kutlama = personel.Kutlama,
                        KatilimciTipi = ProjeConstants.FAALIYET_KATILIMCI_IC_INT,
                        Soyadi = personel.Soyadi,
                        //Unvani = personel.Unvani,
                        EPosta = ePosta,
                    };
                    retVal = katilimci;
                }

            }
            else if (katilimciTipi == ProjeConstants.FAALIYET_KATILIMCI_NAKITBAGISCI_INT)
            {
                NakitBagisci nakitBagisci = new NakitBagisci();
                nakitBagisci = nakitBagisci.Select<NakitBagisci>(katilimciId);
                if (nakitBagisci != null)
                {
                    Katilimci katilimci = new Katilimci()
                    {
                        Id = katilimciId,
                        Adi = nakitBagisci.Adi,
                        Adres = nakitBagisci.Adres,
                        //Dahili1 = personel.Dahili1,
                        //Dahili2 = personel.Dahili2,
                        //Dahili3 = personel.Dahili3,
                        //DogumTarihi = personel.DogumTarihi,
                        //Gorevi = personel.Gorevi,
                        Ilcesi = nakitBagisci.Ilcesi,
                        Ili = nakitBagisci.Ili,
                        Kurumu = ProjeConstants.FAALIYET_KATILIMCI_NAKITBAGISCI,
                        //Kutlama = personel.Kutlama,
                        KatilimciTipi = ProjeConstants.FAALIYET_KATILIMCI_NAKITBAGISCI_INT,
                        Soyadi = nakitBagisci.Soyadi,
                        //Unvani = personel.Unvani,
                        EPosta=nakitBagisci.Eposta,
                    };
                    retVal = katilimci;

                }
            }
            else if (katilimciTipi == ProjeConstants.FAALIYET_KATILIMCI_TASINMAZBAGISCI_INT)
            {
                TasinmazBagisci tasinmazBagisci = new TasinmazBagisci();
                tasinmazBagisci= new TasinmazBagisci();
                tasinmazBagisci = tasinmazBagisci.Select<TasinmazBagisci>(katilimciId);
                if (tasinmazBagisci != null)
                {

                    Katilimci katilimci = new Katilimci()
                    {
                        Id = katilimciId,
                        Adi = tasinmazBagisci.Adi,
                        //Adres = tasinmazBagisci.Adres,
                        //Dahili1 = personel.Dahili1,
                        //Dahili2 = personel.Dahili2,
                        //Dahili3 = personel.Dahili3,
                        //DogumTarihi = personel.DogumTarihi,
                        //Gorevi = personel.Gorevi,
                        //Ilcesi = personel.Ilcesi,
                        //Ili = tasinmazBagisci.Ili,
                        Kurumu = ProjeConstants.FAALIYET_KATILIMCI_TASINMAZBAGISCI,
                        //Kutlama = personel.Kutlama,
                        KatilimciTipi = ProjeConstants.FAALIYET_KATILIMCI_TASINMAZBAGISCI_INT,
                        Soyadi = tasinmazBagisci.Soyadi,
                        //Unvani = personel.Unvani,
                        EPosta = tasinmazBagisci.EPosta,
                    };
                    retVal = katilimci;
                }

            }
            return retVal;
        }
    }
}
