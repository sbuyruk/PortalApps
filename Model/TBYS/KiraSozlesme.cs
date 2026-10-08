using DAO.Ortak;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;
using Model.Services.TBYS;

namespace Model.TBYS
{
    [Serializable]
    public class KiraSozlesme : ParentClass
    {
        public int KiraciId { get; set; }
        public DateTime IlkSozlesmeTar { get; set; }
        public DateTime SozBasTar { get; set; }
        public DateTime SozBitTar { get; set; }
        public decimal KiraBedeli { get; set; }
        public string OdemeSekli { get; set; }
        public int TaksitSayisi { get; set; }
        public string KefilAdiSoyadi { get; set; }
        public string KefilTCKimlikNo { get; set; }
        public string KefilAdresi { get; set; }
        public string KefilTel { get; set; }
        public string TeminatCinsi { get; set; }
        public decimal TeminatTutari { get; set; }
        public decimal OdenenTeminatTutari { get; set; }
        public decimal IadeTeminatTutari { get; set; }
        public decimal KalanTeminatTutari { get; set; }
        public DateTime TeminatOdemeTarihi { get; set; }
        public string TeminatAciklama { get; set; }
        public int DosyaNo { get; set; }
        public decimal DevirAnaPara { get; set; }
        public decimal DevirFaizTutari { get; set; }
        public decimal DevirFaizliBakiye { get; set; }
        public bool Aktif { get; set; }
        public string SozlesmeDurumu { get; set; }
        public DateTime DurumDegismeTar { get; set; }
        public string Aciklama { get; set; }
        public string SozlesmePDFDosyasi { get; set; }
        public string ArtisAyi { get; set; }
        public string Bolge { get; set; }
        public int BolgeId { get; set; }
        public string GecikmeZammiTipi { get; set; }

        public override T Select<T>(int id)
        {
            return (T)Convert.ChangeType(new KiraSozlesmeService().GetById(id), typeof(T));

        }
        public KiraSozlesme Select(int kiraSozlesmeId)
        {
            return new KiraSozlesmeService().GetById(kiraSozlesmeId);
        }

      
        public override int Save()
        {
            return new KiraSozlesmeService().Save(this);
        }
        public override bool Update()
        {
            return new KiraSozlesmeService().Update(this);
        }
        public override bool Delete()
        {
            return new KiraSozlesmeService().Delete(this);
        }
        public override List<T> SelectAll<T>()
        {
            return (List<T>)Convert.ChangeType(
                new KiraSozlesmeService().GetAll(),
                typeof(List<T>));
        }
        public List<KiraSozlesme> SelectAllAktifSozlesme()
        {
            return new KiraSozlesmeService().GetAllActive();
        }
        public DataTable SelectKiraciSayisiByBolgeTarih(int bolgeId, int ay, int yil)
        {
            return new KiraSozlesmeService().GetTenantCountByRegionAndDate(bolgeId, ay, yil);

        }
        public DataTable SelectKiraSozlesmeListReturnDT(int kiraciId, int aktif, int bolgeId)
        {
            return new KiraSozlesmeService().GetListReturnDataTable(kiraciId, aktif, bolgeId);
        }
        public List<KiraSozlesme> SelectKiraSozlesmeList(int kiraciId, int aktif, int bolgeId)
        {
            return new KiraSozlesmeService().GetList(kiraciId, aktif, bolgeId);
        }
        public DataTable SelectKiraArtisiGelenSozlesmelerReturnDT(int bolgeId, DateTime tarih)
        {
            return new KiraSozlesmeService().GetRentIncreaseDue(bolgeId, tarih);
        }
        public DataTable SelectGerceklesenKiraArtislariReturnDT(int bolgeId)
        {
            return new KiraSozlesmeService().GetRealizedRentIncreases(bolgeId);
        }
        public bool UpdateByKiraciId(int bolgeId, int kiraciId)
        {
            return new KiraSozlesmeService().UpdateByKiraciId(bolgeId, kiraciId);
        }

        public DataTable SelectSozlesmeListByYilReturnDT(int yil)
        {
            return new KiraSozlesmeService().GetListByYear(yil);
        }
        public DataTable SelectBitenSozlesmeListByYilReturnDT(int yil)
        {
            return new KiraSozlesmeService().GetCompletedListByYear(yil);
        }

        public List<KiraSozlesme> SelectByKiraciAdi(string adi)
        {
            return new KiraSozlesmeService().GetByKiraciAdi(adi);
        }

        public KiraSozlesme SelectAktifSozlesmeByKiraciId(int kiraciId)
        {
            return new KiraSozlesmeService().GetActiveByKiraciId(kiraciId);
        }
        public KiraSozlesme SelectSozlesmeByKiraciId(int kiraciId)
        {
            return new KiraSozlesmeService().GetByKiraciId(kiraciId);
        }
        public List<KiraSozlesme> SelectByKiraciIdReturnList(int kiraciId)
        {
            return new KiraSozlesmeService().GetAllByKiraciId(kiraciId);
        }
        public KiraSozlesme SelectByKiraciIdTarih(int kiraciId, DateTime tarih)
        {
            return new KiraSozlesmeService().GetByKiraciIdAndDate(kiraciId, tarih);
        }
        public KiraSozlesme SelectEnYakinTarihliSozlesmeByKiraciIdTarih(int kiraciId, DateTime tarih)
        {
            return new KiraSozlesmeService().GetNearestByKiraciIdAndDate(kiraciId, tarih);
        }
        public KiraSozlesme SelectBitenSozlesmeByKiraciId(int kiraciId)
        {
            return new KiraSozlesmeService().GetCompletedByKiraciId(kiraciId);
        }
        public bool UpdateAktifDurum(string durum, string degistirmeTar, bool aktif)
        {
            return new KiraSozlesmeService().UpdateActiveStatus(this, durum, degistirmeTar, aktif);
        }
        public List<KiraSozlesme> SelectByTasinmazId(int tasinmazId)
        {
            return new KiraSozlesmeService().GetByTasinmazId(tasinmazId);
        }
        public DataTable SelectBySozlesmeId(int sozlesmeId)
        {
            return new KiraSozlesmeService().GetAddressById(sozlesmeId);
        }
        public DataTable SelectSUMTeminatByBolgeKiralamaAmaciReturnDT(int bolgeId, string kiralamaAmaci)
        {
            return new KiraSozlesmeService().GetSecurityDepositSummaryByRegionAndPurpose(bolgeId, kiralamaAmaci);
        }
        public DataTable SelectKiraciSayisiVeToplamKiraBedeli(int bolgeId, int ay, int yil)
        {
            return new KiraSozlesmeService().GetTenantCountAndRentTotal(bolgeId, ay, yil);

        }
        public KiraSozlesme SelectNext()
        {
            return new KiraSozlesmeService().GetNext(Id, DosyaNo);
        }

        public KiraSozlesme SelectOncekiKiraSozlesme()
        {
            return new KiraSozlesmeService().GetPreviousByTenant(KiraciId, SozBasTar);
        }

        public KiraSozlesme SelectPrev()
        {
            return new KiraSozlesmeService().GetPrevious(Id, DosyaNo);
        }
        public KiraSozlesme SelectMax()
        {
            return new KiraSozlesmeService().GetMax();
        }
        public KiraSozlesme SelectMin()
        {
            return new KiraSozlesmeService().GetMin();
        }

        public KiraSozlesme SelectNextBiten()
        {
            return new KiraSozlesmeService().GetNextCompleted(Id, DosyaNo);
        }
        public KiraSozlesme SelectPrevBiten()
        {
            return new KiraSozlesmeService().GetPreviousCompleted(Id, DosyaNo);
        }
        public KiraSozlesme SelectMaxBiten()
        {
            return new KiraSozlesmeService().GetMaxCompleted();
        }
        public KiraSozlesme SelectMinBiten()
        {
            return new KiraSozlesmeService().GetMinCompleted();
        }

        /// <summary>
        /// Devir tutari farkli olan aktif sözlesmeleri tek sorguda döndürür.
        /// Önceki sözlesmenin MAX(Sira) ödeme plani satirindaki AnaPara/FaizliBakiye degerleri
        /// aktif sözlesmenin DevirAnaPara/DevirFaizliBakiye degerlerinden farkli olanlari getirir.
        /// Sütunlar: DosyaNo, KiraciAdi, KiraciId, SozlesmeId, SozBasTar, SozBitTar, KiraBedeli,
        ///           SonAnaPara, SonFaizliBakiye, DevirAnaPara, DevirFaizTutari, DevirFaizliBakiye
        /// </summary>
        public DataTable SelectDevirGerekenAktifSozlesmelerReturnDT()
        {
            return new KiraSozlesmeService().GetTransferRequiredActiveContracts();
        }

    }
}
