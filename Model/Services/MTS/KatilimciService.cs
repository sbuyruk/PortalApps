using Model.IKYS;
using Model.NBYS;
using Model.MTS;
using Model.Ortak;
using Model.Services.IKYS;
using Model.Services.NBYS;
using Model.TBYS;
using Model.Services.TBYS;
using System;
using Utility.ProjeGlobal;

namespace Model.Services.MTS
{
    public class KatilimciService
    {
        public Katilimci GetKatilimci(int katilimciId, int katilimciTipi)
        {
            Katilimci retVal = new Katilimci();
            if (katilimciTipi == ProjeConstants.FAALIYET_KATILIMCI_DIS_INT)
            {
                Kisi kisi = new KisiService().GetById(katilimciId);
                if (kisi != null)
                {
                    string kurum = string.Empty;
                    string gorev = string.Empty;
                    new MTSKurumGorevService().GetByKisiId(katilimciId, ref kurum, ref gorev);
                    retVal = new Katilimci
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
                        RandevuKisiti = kisi.RandevuKisiti
                    };
                }
            }
            else if (katilimciTipi == ProjeConstants.FAALIYET_KATILIMCI_IC_INT)
            {
                Personel personel = new PersonelService().GetById(katilimciId);
                if (personel != null)
                {
                    IletisimBilgileri iletisim = new IletisimBilgileriService().GetByPersonelId(personel.Id);
                    retVal = new Katilimci
                    {
                        Id = katilimciId,
                        Adi = personel.Adi,
                        Kurumu = ProjeConstants.FAALIYET_KATILIMCI_IC,
                        KatilimciTipi = ProjeConstants.FAALIYET_KATILIMCI_IC_INT,
                        Soyadi = personel.Soyadi,
                        EPosta = iletisim == null ? string.Empty : iletisim.InternetEPosta
                    };
                }
            }
            else if (katilimciTipi == ProjeConstants.FAALIYET_KATILIMCI_NAKITBAGISCI_INT)
            {
                NakitBagisci nakitBagisci = new NakitBagisciService().GetById(katilimciId);
                if (nakitBagisci != null)
                {
                    retVal = new Katilimci
                    {
                        Id = katilimciId,
                        Adi = nakitBagisci.Adi,
                        Adres = nakitBagisci.Adres,
                        Ilcesi = nakitBagisci.Ilcesi,
                        Ili = nakitBagisci.Ili,
                        Kurumu = ProjeConstants.FAALIYET_KATILIMCI_NAKITBAGISCI,
                        KatilimciTipi = ProjeConstants.FAALIYET_KATILIMCI_NAKITBAGISCI_INT,
                        Soyadi = nakitBagisci.Soyadi,
                        EPosta = nakitBagisci.Eposta
                    };
                }
            }
            else if (katilimciTipi == ProjeConstants.FAALIYET_KATILIMCI_TASINMAZBAGISCI_INT)
            {
                TasinmazBagisci tasinmazBagisci = new TasinmazBagisciService().GetById(katilimciId);
                if (tasinmazBagisci != null)
                {
                    retVal = new Katilimci
                    {
                        Id = katilimciId,
                        Adi = tasinmazBagisci.Adi,
                        Kurumu = ProjeConstants.FAALIYET_KATILIMCI_TASINMAZBAGISCI,
                        KatilimciTipi = ProjeConstants.FAALIYET_KATILIMCI_TASINMAZBAGISCI_INT,
                        Soyadi = tasinmazBagisci.Soyadi,
                        EPosta = tasinmazBagisci.EPosta
                    };
                }
            }

            return retVal;
        }
    }
}
