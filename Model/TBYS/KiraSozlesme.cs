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
            try
            {
                GenericEntity<KiraSozlesme> genericEntity = new GenericEntity<KiraSozlesme>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                SqlQuery query = genericEntity.GetQueryParametreli(this);
                int id = dao.Insert(query);

                this.Id = id;
                if (id > 0 && ProjeConstants.TBYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.TBYS, ProjeConstants.TBYS_KIRASOZLESME);
                }
                return id;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public override bool Update()
        {
            bool isSuccess = false;
            try
            {
                if (this != null)
                {
                    KiraSozlesme item = Select<KiraSozlesme>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<KiraSozlesme> genericEntity = new GenericEntity<KiraSozlesme>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        SqlQuery query = genericEntity.GetQueryParametreli(this);
                        isSuccess = dao.Update2Db(query);
                    }
                    if (isSuccess && ProjeConstants.TBYS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.TBYS, ProjeConstants.TBYS_KIRASOZLESME);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return isSuccess;
        }
        public override bool Delete()
        {
            try
            {
                bool isDeleted = false;
                if (Id != 0)
                {
                    GenericEntity<KiraSozlesme> genericEntity = new GenericEntity<KiraSozlesme>(ProjeConstants.SQL_DELETE);
                    SqlQuery query = genericEntity.GetQueryParametreli(this);
                    KiraSozlesme item = Select<KiraSozlesme>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(query, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.TBYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_KIRASOZLESME);
                    }
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw;
            }
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
            DateTime ayinIlkGunu = new DateTime(yil, ay, 1);
            DateTime ayinSonGunu = ayinIlkGunu.AddMonths(1).AddDays(-1);

            string bolgeStr = bolgeId == ProjeConstants.HEPSI_INT || bolgeId == ProjeConstants.BOLGE_GENELMUDURLUK_INT ? string.Empty : string.Format(" AND A.BolgeId={0} ", bolgeId);


            string odemePlaniStr = string.Format(@"
                (
                    C.Id in 
                    (
                        SELECT Id FROM OdemePlani_Table WHERE A.TaksitSayisi>1 AND VadeBitTar BETWEEN {0} AND {1}
                    )
					OR 
                    C.Id in
				    (
					   SELECT Id FROM OdemePlani_Table 
                        WHERE A.TaksitSayisi < 2 
                            AND 
                            (
                                (C.VadeBitTar BETWEEN {0} AND {1} )
						        AND 
                                (ABS(FaizliBakiye/A.KiraBedeli)*C.Sira >= 0.5 )
                            )
					)
				)
                ", ayinIlkGunu.ReturnTRDateFormat(), ayinSonGunu.ReturnDDMMYYYFormat());
            string sqlString = string.Format(@"
                SELECT --COUNT(A.Id) Adet
                    A.Id KiraSozlesmeId,H.KisaAdi Bolge,A.BolgeId,  A.DosyaNo, B.Adi+' '+B.Soyadi Kiraci, E.Adres + ' ' + ISNULL(F.BolumNo,'') Adres, 
                    A.IlkSozlesmeTar, A.SozBasTar, A.SozBitTar, A.ArtisAyi, A.OdemeSekli, 
                    A.KiraBedeli, C.AnaPara AnaPara,C.FaizTutari, C.FaizliBakiye, C.VadeBasTar, C.VadeBitTar, 
                    E.KullanimSekli,E.Ili,E.Ilcesi,
                    A.TeminatOdemeTarihi, A.TeminatTutari, A.OdenenTeminatTutari, A.IadeTeminatTutari, A.KalanTeminatTutari,
                    ABS(FaizliBakiye/(A.KiraBedeli/12))*C.Sira/(A.KiraBedeli/12) AySayisi, A.TaksitSayisi, A.OdemeSekli,B.KiralamaAmaci
                FROM KiraSozlesme_Table A
                    INNER JOIN Kiraci_Table B ON B.Id=A.KiraciId
                    INNER JOIN Bolge_Table H ON H.Id=A.BolgeId
                    LEFT JOIN OdemePlani_Table C ON C.SozlesmeId=A.Id AND {3} 
                    LEFT JOIN SozlesmeTasinmaz_Table D On D.SozlesmeId=A.Id AND D.Id = (Select top 1 Id from SozlesmeTasinmaz_Table where SozlesmeId = A.Id) 
                    LEFT JOIN Tasinmaz_Table E On E.Id=D.TasinmazId
                    LEFT JOIN BagimsizBolum_Table F On F.Id=D.BolumId
                WHERE 1>0
	                {0}    
                    AND ( 
                        A.SozBasTar<{2} AND A.SozBitTar >= {1} 
						AND 
							(
								A.SozlesmeDurumu = 'Devam Ediyor' --hala devam edenler
								OR
								(A.SozlesmeDurumu != 'Devam Ediyor' AND A.SozlesmeDurumu != 'Yenilendi' AND A.DurumDegismeTar BETWEEN {1} AND {2}) --bu ay durumu degismis olanlar
        						OR 
		        				(A.SozlesmeDurumu='Yenilendi' AND A.SozBitTar>{2}) --sonOdemeTar dan sonra (önündeki aylarda) yenilenenler.. aslinda A.SozlesmeDurumu != 'Devam Ediyor' daha dogru olabilir
					        )
                    )
                ORDER BY A.BolgeId,A.DosyaNo,B.Adi
                ", bolgeStr, ayinIlkGunu.ConvertToDatetimeEmptyIfNull().ReturnQuotedValue(), ayinSonGunu.ConvertToDatetimeEmptyIfNull().ReturnQuotedValue(),odemePlaniStr);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            return dataTable;

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
            bool isUpdated = false;
            SqlQuery query = new SqlQuery(@"
                Update KiraSozlesme_Table
                Set BolgeId= @BolgeId
                Where KiraciId=@KiraciId");
            query.AddParameter("@BolgeId", bolgeId);
            query.AddParameter("@KiraciId", kiraciId);
            isUpdated = dao.Update2Db(query);
            return isUpdated;
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
            string sqlString = string.Format(@"	
                SELECT DosyaNo, Adi + Soyadi, COUNT(Adi + Soyadi)
                FROM KiraSozlesme_Table A
	                INNER JOIN Kiraci_Table B ON B.Id=A.KiraciId
                WHERE A.KiraciId > 0 AND B.Adi ={0}
                GROUP BY DosyaNo, Adi + Soyadi
                HAVING COUNT(Adi + Soyadi)>1
                ", adi.ReturnQuotedValue());

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<KiraSozlesme> list = ToList<KiraSozlesme>(dataTable);
            return list;
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
            Degistiren = UtilityHelper.GetCurrentUserName();
            DegistirmeTarihi = DateTime.Now;
            string durumDegistirmeTar = string.IsNullOrEmpty(degistirmeTar) ? DateTime.Now.ConvertToTimeSpanReturnInHHmm() : degistirmeTar;
            SqlQuery query = new SqlQuery(@" UPDATE KiraSozlesme_Table
                                SET Aktif=@Aktif,
                                    SozlesmeDurumu=@Durum,
                                    DurumDegismeTar=@DurumDegismeTar,
                                    Degistiren=@Degistiren,
                                    DegistirmeTarihi=@DegistirmeTarihi
                                WHERE  Aktif=1 AND Id=@Id");
            query.AddParameter("@Aktif", aktif);
            query.AddParameter("@Durum", durum);
            query.AddParameter("@DurumDegismeTar", durumDegistirmeTar);
            query.AddParameter("@Degistiren", Degistiren);
            query.AddParameter("@DegistirmeTarihi", DegistirmeTarihi);
            query.AddParameter("@Id", Id);
            bool isSaved = dao.Update2Db(query);
            return isSaved;
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
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            string sqlString = string.Empty;
            if (DosyaNo > 0)
            {
                sqlString = string.Format(@"
                    SELECT * FROM KiraSozlesme_Table
                    WHERE Aktif=0 AND DosyaNo > {0}
                    ORDER BY DosyaNo,Id ", DosyaNo.ReturnQuotedValue());
            }
            else
            {
                sqlString = string.Format(@"
                    SELECT * FROM KiraSozlesme_Table
                    WHERE Aktif=0 AND DosyaNo IS NULL AND Id > {0}
                    ORDER BY DosyaNo,Id ", Id.ReturnQuotedValue());
            }

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            if (dataTable != null)
            {
                List<KiraSozlesme> list = ToList<KiraSozlesme>(dataTable);
                kiraSozlesme = list.FirstOrDefault();
            }
            else
            {
                kiraSozlesme = SelectMinBiten();

            }
            return kiraSozlesme;
        }
        public KiraSozlesme SelectPrevBiten()
        {
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            string sqlString = string.Empty;
            if (DosyaNo > 0)
            {
                sqlString = string.Format(@"
                    SELECT * FROM KiraSozlesme_Table
                    WHERE Aktif=0 AND DosyaNo < {0}
                    ORDER BY DosyaNo DESC ,Id DESC", DosyaNo.ReturnQuotedValue());
            }
            else
            {
                sqlString = string.Format(@"
                    SELECT * FROM KiraSozlesme_Table
                    WHERE Aktif=0 AND DosyaNo IS NULL AND Id < {0}
                    ORDER BY DosyaNo DESC,Id DESC ", Id.ReturnQuotedValue());
            }

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            if (dataTable != null)
            {
                List<KiraSozlesme> list = ToList<KiraSozlesme>(dataTable);
                kiraSozlesme = list.FirstOrDefault();
            }
            else
            {
                kiraSozlesme = SelectMaxBiten();

            }
            return kiraSozlesme;
        }
        public KiraSozlesme SelectMaxBiten()
        {
            KiraSozlesme kiraSozlesme = null;
            string sqlString = string.Format(@"
                SELECT * FROM KiraSozlesme_Table
                WHERE DosyaNo=(
                    SELECT MAX(DosyaNo) 
                    FROM KiraSozlesme_Table
                    WHERE Aktif=0  
                    ) ");
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            if (dataTable != null)
            {
                List<KiraSozlesme> list = ToList<KiraSozlesme>(dataTable);
                kiraSozlesme = list.FirstOrDefault();
            }
            return kiraSozlesme;
        }
        public KiraSozlesme SelectMinBiten()
        {
            KiraSozlesme kiraSozlesme = null;
            string sqlString = string.Format(@"
                SELECT * FROM KiraSozlesme_Table
                WHERE DosyaNo=(
                    SELECT MIN(DosyaNo) 
                    FROM KiraSozlesme_Table
                    WHERE Aktif=0 AND DosyaNo>0
                    ) ");
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            if (dataTable != null)
            {
                List<KiraSozlesme> list = ToList<KiraSozlesme>(dataTable);
                kiraSozlesme = list.FirstOrDefault();
            }
            return kiraSozlesme;
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
            string sqlString = @"
                SELECT
                    A.Id            AS SozlesmeId,
                    A.DosyaNo,
                    A.KiraciId,
                    K.Adi + ' ' + K.Soyadi AS KiraciAdi,
                    A.SozBasTar,
                    A.SozBitTar,
                    A.KiraBedeli,
                    A.DevirAnaPara,
                    A.DevirFaizTutari,
                    A.DevirFaizliBakiye,
                    OP.AnaPara      AS SonAnaPara,
                    OP.FaizliBakiye AS SonFaizliBakiye
                FROM KiraSozlesme_Table A
                INNER JOIN Kiraci_Table K ON K.Id = A.KiraciId
                -- Önceki sözlesme: ayni kiraciya ait, baslangiç tarihi daha eski
                CROSS APPLY (
                    SELECT TOP 1 Id
                    FROM KiraSozlesme_Table
                    WHERE KiraciId = A.KiraciId
                      AND SozBasTar < A.SozBasTar
                    ORDER BY SozBasTar DESC
                ) AS OncekiSoz
                -- Önceki sözlesmenin MAX(Sira) ödeme plani satiri
                CROSS APPLY (
                    SELECT TOP 1 AnaPara, FaizliBakiye
                    FROM OdemePlani_Table
                    WHERE SozlesmeId = OncekiSoz.Id
                    ORDER BY Sira DESC
                ) AS OP
                WHERE A.Aktif = 1
                  AND (OP.AnaPara != A.DevirAnaPara OR OP.FaizliBakiye != A.DevirFaizliBakiye)
                ORDER BY A.DosyaNo, A.Id";

            return dao.SelectFromDb(sqlString, "");
        }

    }
}
