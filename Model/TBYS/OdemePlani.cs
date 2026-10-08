using Model.Ortak;
using System;

namespace Model.TBYS
{
    [Serializable]
    public class OdemePlani : EntityBase
    {
        public int SozlesmeId { get; set; }
        public int Yil { get; set; }
        public string Ay { get; set; }
        public DateTime VadeBasTar { get; set; }
        public DateTime VadeBitTar { get; set; }
        public decimal KiraBedeli { get; set; }
        public decimal OdenenTutar { get; set; }
        public decimal AnaPara { get; set; }
        public decimal FaizliBakiye { get; set; }
        public decimal FaizOrani { get; set; }
        public decimal FaizTutari { get; set; }
        public DateTime OdemeBasTar { get; set; }
        public DateTime OdemeBitTar { get; set; }
        public int Sira { get; set; }
        public string Aciklama { get; set; }
        /*
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            int taksitSayisi = kiraSozlesme.TaksitSayisi;
            if (taksitSayisi < 1)
            {
                taksitSayisi = 1;
            }
            else if (taksitSayisi > 12)
            {
                taksitSayisi = 12;
            }
            bool isSaved = false;

            DateTime sozBastarDate = kiraSozlesme.SozBasTar;//.AddMonths(1);//vade tarihi bir ay sonra olsun Zekayi Çalis 22/04/2019
            DateTime bittarDate = sozBastarDate.AddMonths(taksitSayisi);

            int odemeBitYil = bittarDate.Year;

            string OdemeBitAy = bittarDate.ToString("MMMM", culturInfo);

            DateTime vadeBasTarX = sozBastarDate;
            DateTime vadeBitTarX = vadeBasTarX.AddMonths(1).AddDays(-1);
            //taksitsayisi verilebilsin diye asagisi kapatildi
            //if (kiraSozlesme.OdemeSekli == ProjeConstants.KIRA_ODMSEKLI_YILLIK)
            //{
            //    vadeBasTarX = kiraSozlesme.SozBitTar;
            //    bittarDate = kiraSozlesme.SozBitTar;
            //}
            try
            {
                decimal aylikKira = kiraSozlesme.KiraBedeli;
                if (kiraSozlesme.OdemeSekli.Equals(ProjeConstants.KIRA_ODMSEKLI_YILLIK))
                {
                    decimal odenecekTutar = kiraSozlesme.KiraBedeli / taksitSayisi;
                    aylikKira = odenecekTutar;
                }

                OdemePlani odemePlaniDevir = new OdemePlani();
                odemePlaniDevir = SaveDevir(kiraSozlesme, kiraSozlesme.DevirAnaPara, kiraSozlesme.DevirFaizTutari, "Önceki Sözlesmeden devir", 0);

                for (int i = 1; i <= taksitSayisi; i++)//taksitlere böl
                {

                    int yil = vadeBasTarX.Year;
                    string ay = vadeBasTarX.ToString("MMMM", culturInfo);

                    if ((ay + "/" + yil).Equals(OdemeBitAy + "/" + odemeBitYil))
                    {
                        break;
                    }
                    //odeme sozlesme bas ayda yapildi o yüzden bit ayda ödeme olmasin
                    OdemePlani odemePlani = new OdemePlani();
                    odemePlani = SaveNew(kiraSozlesme, yil, ay, vadeBasTarX, vadeBitTarX, sozBastarDate, bittarDate, aylikKira, i);
                    //odemeBasTar ilk ay baslasin zekayi bey 16/11/2017
                    vadeBasTarX = vadeBasTarX.AddMonths(1);
                    vadeBitTarX = vadeBasTarX.AddMonths(1).AddDays(-1); //vadeBitTarX.AddMonths(1) -- bu subat ayi için yanlis çalisiyor, 29 subat ayin son günü, 1 ay ekleyince 29 mart oluyor, ama ayin sonu olmuyordu. SB
                }
                //sözlesme bitimine kadar olan aylar için satir ekle
                for (int i = taksitSayisi+1; i <= 12; i++)
                {
                    int yil = vadeBasTarX.Year;
                    string ay = vadeBasTarX.ToString("MMMM", culturInfo);

                    OdemePlani odemePlani = new OdemePlani();
                    odemePlani = SaveNew(kiraSozlesme, yil, ay, vadeBasTarX, vadeBitTarX, sozBastarDate, bittarDate, 0, i);
                    vadeBasTarX = vadeBasTarX.AddMonths(1);
                    vadeBitTarX = vadeBasTarX.AddMonths(1).AddDays(-1);
                }

                isSaved = true;
            }
            catch (Exception)
            {

                isSaved = false;
            }

            return isSaved;
        }
        private OdemePlani SaveNew(KiraSozlesme kiraSozlesme, int pYil, string pAy, DateTime pVadeBasTar, DateTime pVadeBitTar,
            DateTime pOdemeBasTar, DateTime pOdemeBitTar, decimal aylikKira, int sira)
        {

            OdemePlani odemePlani = new OdemePlani();
            odemePlani.SozlesmeId = kiraSozlesme.Id;
            odemePlani.Yil = pYil;
            odemePlani.Ay = pAy;
            odemePlani.VadeBasTar = pVadeBasTar;
            odemePlani.VadeBitTar = pVadeBitTar;
            odemePlani.KiraBedeli = aylikKira;
            odemePlani.AnaPara = 0;// aylikKira * (-1);
            odemePlani.FaizTutari = 0;
            odemePlani.OdenenTutar = 0;
            odemePlani.OdemeBasTar = pOdemeBasTar;
            odemePlani.OdemeBitTar = pOdemeBitTar;
            odemePlani.Sira = sira;
            odemePlani.Aciklama = "";
            new Model.Services.TBYS.OdemePlaniService().Save(odemePlani);
            return odemePlani;
        }
        private OdemePlani SaveDevir(KiraSozlesme kiraSozlesme, decimal devirAnaPara, decimal devirFaizTutari, string aciklama, int sira)
        {
            DateTime pVadeBitTar = kiraSozlesme.SozBasTar.AddDays(-1);
            OdemePlani odemePlani = new OdemePlani();
            odemePlani.SozlesmeId = kiraSozlesme.Id;
            //VadeBasTar null veya bos olmali 
            //odemePlani.VadeBasTar = pVadeBasTar; 
            
            //VadeBitTar eklendi SB 20.01.2021 sebebi BölgelereGöeBorcluKiracilar raporunda içinde bulunulan ayda yeni sözlesmesi olanlarin devir borcu varsa dikkate almiyordu
            odemePlani.VadeBitTar = pVadeBitTar;

            odemePlani.AnaPara = devirAnaPara;
            odemePlani.FaizTutari = devirFaizTutari;
            odemePlani.FaizliBakiye = devirAnaPara + devirFaizTutari;
            odemePlani.OdenenTutar = 0;
            odemePlani.Aciklama = aciklama;
            odemePlani.Sira = sira;
            new Model.Services.TBYS.OdemePlaniService().Save(odemePlani);
            return odemePlani;
        }
        */
    }
}
