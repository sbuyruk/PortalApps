using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

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

        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM KiraSozlesme_Table 
                               WHERE  Id={0}", id);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<KiraSozlesme> list = ToList<KiraSozlesme>(dataTable);
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            kiraSozlesme = list.FirstOrDefault();
            return (T)Convert.ChangeType(kiraSozlesme, typeof(T));

        }
        public KiraSozlesme Select(int kiraSozlesmeId)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM KiraSozlesme_Table 
                               WHERE  Id={0}", kiraSozlesmeId);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<KiraSozlesme> list = ToList<KiraSozlesme>(dataTable);
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            kiraSozlesme = list.FirstOrDefault();
            return kiraSozlesme;

        }
        public string ArtisAyi { get; set; }
        public string Bolge { get; set; }
        public string GecikmeZammiTipi { get; set; }
        public override int Save()
        {
            try
            {
                GenericEntity<KiraSozlesme> genericEntity = new GenericEntity<KiraSozlesme>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                SozlesmeDurumu = string.IsNullOrEmpty(SozlesmeDurumu) ? ProjeConstants.KIRASOZLESME_DURUMU_DEVAM : SozlesmeDurumu;
                Aktif = true;
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);

                this.Id = id;
                return id;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public int SaveSozlesme()
        {
            try
            {
                GenericEntity<KiraSozlesme> genericEntity = new GenericEntity<KiraSozlesme>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);
                this.Id = id;
                return id;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public override bool Update()
        {
            bool isSuccess = false;
            try
            {
                if (Id != 0)
                {
                    GenericEntity<KiraSozlesme> genericEntity = new GenericEntity<KiraSozlesme>(ProjeConstants.SQL_UPDATE);
                    DegistirmeTarihi = DateTime.Now;
                    SozlesmeDurumu = string.IsNullOrEmpty(SozlesmeDurumu) ? ProjeConstants.KIRASOZLESME_DURUMU_DEVAM : SozlesmeDurumu;
                    string sqlString = genericEntity.GetQuery(this);
                    isSuccess = dao.Update2Db(sqlString);
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
            string sqlString = string.Format(@"DELETE 
                               FROM KiraSozlesme_Table
                               WHERE Id={0}", Id);

            bool isSuccess = dao.DeleteFromDb(sqlString, this);

            return isSuccess;
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM KiraSozlesme_Table
                ORDER BY CASE WHEN DosyaNo=0 THEN 2 ELSE 1 END,ISNULL(DosyaNo,999999)
                ");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<KiraSozlesme> list = ToList<KiraSozlesme>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public List<KiraSozlesme> SelectAllAktifSozlesme()
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM KiraSozlesme_Table
                WHERE Aktif=1
                ORDER BY CASE WHEN DosyaNo=0 THEN 2 ELSE 1 END,ISNULL(DosyaNo,999999)
                ");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<KiraSozlesme> list = ToList<KiraSozlesme>(dataTable);

            return (list);
        }
        public int SelectAktifSozlesmeSayisiByBolge(string bolge)
        {
            int adet = 0;
            string sqlString = string.Format(@"
                SELECT COUNT(distinct(A.Id)) Adet 
                FROM KiraSozlesme_Table A
                    INNER JOIN SozlesmeTasinmaz_Table C On C.SozlesmeId=A.Id
                    INNER JOIN Kiraci_Table B On B.Id=A.KiraciId
                    INNER JOIN Tasinmaz_Table D On D.Id=C.TasinmazId
                    INNER JOIN Il_Table E ON E.IlAdi=D.Ili
                WHERE A.Aktif=1 AND E.Bolge={0}
                GROUP BY E.Bolge", bolge.ReturnQuotedValue());
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            if (dataTable != null)
            {
                DataRow row = dataTable.Rows[0];
                adet = row["Adet"].ConvertToInt();
            }
            return adet;

        }

        public DataTable SelectKiraciSayisiByBolgeTarih(string bolge, int ay, int yil)
        {
            DateTime ayinIlkGunu = new DateTime(yil, ay, 1);
            DateTime ayinSonGunu = ayinIlkGunu.AddMonths(1).AddDays(-1);

            string bolgeStr = string.Format(bolge.Equals(ProjeConstants.HEPSI) ? " " : " A.Bolge ={0} AND ", bolge.ReturnQuotedValue());

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
                    A.Id KiraSozlesmeId, A.Bolge, A.DosyaNo, B.Adi+' '+B.Soyadi Kiraci, E.Adres + ' ' + ISNULL(F.BolumNo,'') Adres, 
                    A.IlkSozlesmeTar, A.SozBasTar, A.SozBitTar, A.ArtisAyi, A.OdemeSekli, 
                    A.KiraBedeli, C.AnaPara AnaPara,C.FaizTutari, C.FaizliBakiye, C.VadeBasTar, C.VadeBitTar, 
                    E.KullanimSekli,E.Ili,E.Ilcesi,
                    A.TeminatOdemeTarihi, A.TeminatTutari, A.OdenenTeminatTutari, A.IadeTeminatTutari, A.KalanTeminatTutari,
                    ABS(FaizliBakiye/(A.KiraBedeli/12))*C.Sira/(A.KiraBedeli/12) AySayisi, A.TaksitSayisi
                FROM KiraSozlesme_Table A
                    INNER JOIN Kiraci_Table B ON B.Id=A.KiraciId
                    LEFT JOIN OdemePlani_Table C ON C.SozlesmeId=A.Id AND {3} 
                    LEFT JOIN SozlesmeTasinmaz_Table D On D.SozlesmeId=A.Id AND D.Id = (Select top 1 Id from SozlesmeTasinmaz_Table where SozlesmeId = A.Id) 
                    LEFT JOIN Tasinmaz_Table E On D.Id=D.TasinmazId
                    LEFT JOIN BagimsizBolum_Table F On F.Id=D.BolumId
                WHERE 
	                {0}    
                    ( 
                        A.SozBasTar<{2} AND A.SozBitTar >= {1} 
						AND 
							(
								A.SozlesmeDurumu = 'Devam Ediyor' --hala devam edenler
								OR
								(A.SozlesmeDurumu != 'Devam Ediyor' AND A.SozlesmeDurumu != 'Yenilendi' AND A.DurumDegismeTar BETWEEN {1} AND {2}) --bu ay durumu değişmiş olanlar
        						OR 
		        				(A.SozlesmeDurumu='Yenilendi' AND A.SozBitTar>{2}) --sonOdemeTar dan sonra (önündeki aylarda) yenilenenler.. aslında A.SozlesmeDurumu != 'Devam Ediyor' daha doğru olabilir
					        )
                    )
                ORDER BY A.Bolge,A.DosyaNo,B.Adi
                ", bolgeStr, ayinIlkGunu.ConvertToDatetimeEmptyIfNull().ReturnQuotedValue(), ayinSonGunu.ConvertToDatetimeEmptyIfNull().ReturnQuotedValue(),odemePlaniStr);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            return dataTable;

        }
        public DataTable SelectKiraSozlesmeListReturnDT(int kiraciId, int aktif, string bolge)
        {
            string bolgeStr = string.IsNullOrEmpty(bolge) || 
                bolge.Equals(ProjeConstants.BOLGE_HEPSI) || 
                bolge.Equals(ProjeConstants.TBYS_YETKILI_BIRIM ) ? "" : " AND S.Bolge = " + bolge.ReturnQuotedValue();
            string aktifStr = aktif == ProjeConstants.KIRASOZLESME_AKTIF_HEPSI_INT ? "" : " AND S.Aktif = " + aktif;
            string kiracistr = kiraciId < 1 ? "" : " AND S.KiraciId=" + kiraciId ;

            string sqlString = string.Format(@"				
                SELECT S.Id KiraSozlesmeId, S.DosyaNo, E.Bolge, S.KiraciId, S.Aktif, F.BolumNo, D.Adres, D.Ili,D.Ilcesi,ArtisAyi,
                    IlkSozlesmeTar,SozBasTar, SozBitTar,DATEDIFF(dd,S.SozBasTar,GETDATE()) SozlesmeBasladi,
	                S.KiraBedeli KiraBedeli,S.OdemeSekli,S.TaksitSayisi, S.KefilAdiSoyadi, 
	                S.KefilTCKimlikNo, S.KefilAdresi, S.KefilTel,S.TeminatCinsi, 
                    S.TeminatTutari, S.OdenenTeminatTutari, S.IadeTeminatTutari, S.KalanTeminatTutari,
	                S.TeminatAciklama, S.TeminatOdemeTarihi,
	                K.Adi KiraciAdi, K.Soyadi KiraciSoyadi, S.SozlesmeDurumu, S.DurumDegismeTar, S.Aktif, S.SozlesmePDFDosyasi,K.KiralamaAmaci
                FROM KiraSozlesme_Table S
					LEFT JOIN Kiraci_Table K on K.Id= S.KiraciId
					LEFT OUTER JOIN SozlesmeTasinmaz_Table C On C.SozlesmeId=S.Id 
					LEFT OUTER JOIN Tasinmaz_Table D ON D.Id=C.TasinmazId
	                LEFT OUTER JOIN BagimsizBolum_Table F ON F.TasinmazId=D.Id AND F.Id=C.BolumId
					LEFT JOIN Il_Table E ON E.IlAdi=D.Ili
                WHERE 1 > 0 
                {0} {1} {2} -- AND D.EnvanterdeMi=1 envanterde olmayan ama kirada olanlar var
				ORDER BY DosyaNo", aktifStr, kiracistr,bolgeStr);
            DataTable dataTable = null;
            try
            {
                dataTable = dao.selectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                throw e;
            }
            return dataTable;
        }
        public DataTable SelectKiraArtisiGelenSozlesmelerReturnDT(string bolge)
        {
            string bolgeStr = string.Format(bolge.Equals(ProjeConstants.BOLGE_HEPSI) ? " " : " AND S.Bolge ={0} ", bolge.ReturnQuotedValue());
            string sqlString = string.Format(@"
                SELECT S.Id KiraSozlesmeId, K.Adres, K.Ili,K.Ilcesi,K.Semt,S.Bolge,
                    IlkSozlesmeTar,SozBasTar,SozBitTar,S.ArtisAyi,
	                S.KiraBedeli KiraBedeli, S.Aktif, S.OdemeSekli,               
	                S.KiraciId, K.Adi KiraciAdi, K.Soyadi KiraciSoyadi,K.KiralamaAmaci
                FROM KiraSozlesme_Table S
	                LEFT JOIN Kiraci_Table K on K.Id= S.KiraciId
	                WHERE 1>0 AND S.Aktif = 1 
                        {0}
                        --AND ArtisAyi = MONTH(GETDATE())+1 AND YEAR(SozBitTar)=YEAR(GETDATE())  --12nci ayda yanlış çalıştı
                        --AND (ArtisAyi = DATEPART(MM,DATEADD(mm,1, GETDATE())) AND YEAR(SozBitTar)=YEAR(DATEADD(mm,1, GETDATE())) )
                        --AND CONVERT(int,ArtisAyi) = DATEPART(MM,DATEADD(mm,1, GETDATE())) --sözlesmesi yenilenenlerde esi v yeni kirabedeli yanlış çıkıyor                        
                        AND (CONVERT(int,ArtisAyi) = DATEPART(MM,DATEADD(mm,1, GETDATE())) AND (YEAR(SozBitTar)=YEAR(GETDATE())) )
	                ORDER BY S.Bolge, SozBitTar
            ", bolgeStr);
            DataTable dataTable;
            try
            {
                dataTable = dao.selectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                throw e;
            }
            return dataTable;
        }

        public bool UpdateByKiraciId(string bolge, int kiraciId)
        {
            bool isUpdated = false;
            string sqlString = string.Format(@"
                Update KiraSozlesme_Table
                Set Bolge= {0} 
                Where KiraciId={1}", bolge.ReturnQuotedValue(), kiraciId);
            isUpdated = dao.Update2Db(sqlString);
            return isUpdated;
        }

        public DataTable SelectSozlesmeListByYilReturnDT(int yil)
        {
            //string aktifStr = string.Format(" AND Aktif={0}", aktif);
            string sqlString = string.Format(@"				
                SELECT S.Id KiraSozlesmeId, S.DosyaNo, E.Bolge, S.KiraciId, S.Aktif, F.BolumNo, D.Adres, D.Ili,D.Ilcesi,
	                FORMAT(S.IlkSozlesmeTar,'dd/MM/yyyy') IlkSozlesmeTar,
	                FORMAT(S.SozBasTar, 'dd/MM/yyyy') SozBasTar, 
	                FORMAT(S.SozBitTar, 'dd/MM/yyyy') SozBitTar,
	                S.KiraBedeli KiraBedeli,S.OdemeSekli,S.TaksitSayisi, S.KefilAdiSoyadi, 
	                S.KefilTCKimlikNo, S.KefilAdresi, S.KefilTel,S.TeminatCinsi, 
                    S.TeminatTutari, S.OdenenTeminatTutari, S.IadeTeminatTutari, S.KalanTeminatTutari,
	                S.TeminatAciklama, S.TeminatOdemeTarihi,
	                K.Adi KiraciAdi, K.Soyadi KiraciSoyadi, S.SozlesmeDurumu, S.DurumDegismeTar
                FROM KiraSozlesme_Table S
					LEFT JOIN Kiraci_Table K on K.Id= S.KiraciId
					LEFT OUTER JOIN SozlesmeTasinmaz_Table C On C.SozlesmeId=S.Id 
					LEFT OUTER JOIN Tasinmaz_Table D ON D.Id=C.TasinmazId
	                LEFT OUTER JOIN BagimsizBolum_Table F ON F.TasinmazId=D.Id AND F.Id=C.BolumId
					LEFT JOIN Il_Table E ON E.IlAdi=D.Ili
                WHERE YEAR(S.IlkSozlesmeTar)={0} 
				ORDER BY DosyaNo", yil);

            DataTable dataTable = null;
            try
            {
                dataTable = dao.selectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                throw e;
            }
            return dataTable;
        }
        public DataTable SelectBitenSozlesmeListByYilReturnDT(int yil)
        {
            //string aktifStr = string.Format(" AND Aktif={0}", aktif);
            string sqlString = string.Format(@"				
                SELECT S.Id KiraSozlesmeId, S.DosyaNo, E.Bolge, S.KiraciId, S.Aktif, F.BolumNo, D.Adres, D.Ili,D.Ilcesi,
	                FORMAT(S.IlkSozlesmeTar,'dd/MM/yyyy') IlkSozlesmeTar,
	                FORMAT(S.SozBasTar, 'dd/MM/yyyy') SozBasTar, 
	                FORMAT(S.SozBitTar, 'dd/MM/yyyy') SozBitTar,
	                S.KiraBedeli KiraBedeli,S.OdemeSekli,S.TaksitSayisi, S.KefilAdiSoyadi, 
	                S.KefilTCKimlikNo, S.KefilAdresi, S.KefilTel,S.TeminatCinsi, 
                    S.TeminatTutari  TeminatTutari, S.OdenenTeminatTutari, S.IadeTeminatTutari, S.KalanTeminatTutari,
	                S.TeminatAciklama, S.TeminatOdemeTarihi,
	                K.Adi KiraciAdi, K.Soyadi KiraciSoyadi, S.SozlesmeDurumu, S.DurumDegismeTar
                FROM KiraSozlesme_Table S
					LEFT JOIN Kiraci_Table K on K.Id= S.KiraciId
					LEFT OUTER JOIN SozlesmeTasinmaz_Table C On C.SozlesmeId=S.Id 
					LEFT OUTER JOIN Tasinmaz_Table D ON D.Id=C.TasinmazId
	                LEFT OUTER JOIN BagimsizBolum_Table F ON F.TasinmazId=D.Id AND F.Id=C.BolumId
					LEFT JOIN Il_Table E ON E.IlAdi=D.Ili
                WHERE YEAR(S.DurumDegismeTar)={0} AND S.SozlesmeDurumu in ('" + ProjeConstants.KIRASOZLESME_DURUMU_BITTI + "," + ProjeConstants.KIRASOZLESME_DURUMU_FESIH + @"')
				ORDER BY DosyaNo", yil);

            DataTable dataTable = null;
            try
            {
                dataTable = dao.selectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                throw e;
            }
            return dataTable;
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

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<KiraSozlesme> list = ToList<KiraSozlesme>(dataTable);
            return list;
        }

        public List<KiraSozlesme> SelectBitenSozlesmeListByKiraciIdOrderBySozBitTarDesc(int kiraciId)
        {
            //string aktifStr = string.Format(" AND Aktif={0}", aktif);
            string sqlString = string.Format(@"				
                SELECT *
                FROM KiraSozlesme_Table 
                WHERE Aktif=0 AND KiraciId={0}
				ORDER BY SozBitTar DESC", kiraciId);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            if (dataTable != null)
            {
                List<KiraSozlesme> list = ToList<KiraSozlesme>(dataTable);
                return list;
            }
            else
            {
                return null;
            }
        }
        public KiraSozlesme SelectAktifSozlesmeByKiraciId(int kiraciId)
        {
            string sqlString = string.Format(@"
                SELECT  *
                FROM KiraSozlesme_Table
                WHERE Aktif=1 AND KiraciId={0}
                ORDER BY SozBastar DESC", kiraciId.ReturnQuotedValue());
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            if (dataTable != null)
            {
                List<KiraSozlesme> list = ToList<KiraSozlesme>(dataTable);
                KiraSozlesme kiraSozlesme = list.FirstOrDefault();
                return kiraSozlesme;
            }
            else
            {
                return null;
            }

        }
        public KiraSozlesme SelectSozlesmeByKiraciId(int kiraciId)
        {
            string sqlString = string.Format(@"
                                SELECT  *
                                FROM KiraSozlesme_Table
                                WHERE KiraciId={0}", kiraciId.ReturnQuotedValue());
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            if (dataTable != null)
            {
                List<KiraSozlesme> list = ToList<KiraSozlesme>(dataTable);
                KiraSozlesme kiraSozlesme = list.FirstOrDefault();
                return kiraSozlesme;
            }
            else
            {
                return null;
            }

        }
        public List<KiraSozlesme> SelectByKiraciIdReturnList(int kiraciId)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM KiraSozlesme_Table
                WHERE KiraciId={0}
                ORDER BY SozBasTar DESC, DosyaNo
                ", kiraciId);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<KiraSozlesme> list = ToList<KiraSozlesme>(dataTable);

            return (list);
        }
        public KiraSozlesme SelectByKiraciIdTarih(int kiraciId, DateTime tarih)
        {
            string sqlString = string.Format(@"
                SELECT  *
                FROM KiraSozlesme_Table
                WHERE KiraciId={0} AND ({1} >= SozBasTar AND {1} < SozBitTar)", kiraciId.ReturnQuotedValue(), tarih.ReturnTRDateFormat());
            //WHERE KiraciId={0} AND ({1} BETWEEN SozBasTar AND SozBitTar)", kiraciId.ReturnQuotedValue(),tarih.ReturnTRDateFormat());
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            if (dataTable != null)
            {
                List<KiraSozlesme> list = ToList<KiraSozlesme>(dataTable);
                KiraSozlesme kiraSozlesme = list.FirstOrDefault();
                return kiraSozlesme;
            }
            else
            {
                return null;
            }

        }
        public KiraSozlesme SelectEnYakinTarihliSozlesmeByKiraciIdTarih(int kiraciId, DateTime tarih)
        {
            //ödeme tarihinden sonra yapılmış bir sözleşme var mı
            KiraSozlesme kiraSozlesme = null;
            string sqlString = string.Format(@"
                SELECT  *
                FROM KiraSozlesme_Table
                WHERE KiraciId={0} AND SozBasTar >= {1} 
                ORDER BY SozBasTar ", kiraciId.ReturnQuotedValue(), tarih.ReturnTRDateFormat());
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            if (dataTable != null)
            {
                List<KiraSozlesme> list = ToList<KiraSozlesme>(dataTable);
                KiraSozlesme sonrakiIlkKiraSozlesmesi = list.FirstOrDefault();
                kiraSozlesme= sonrakiIlkKiraSozlesmesi;
            }
            else //ödeme tarihinden önce yapılmış bir sözleşme var mı
            {
                sqlString = string.Format(@"
                SELECT  *
                FROM KiraSozlesme_Table
                WHERE KiraciId={0} AND SozBasTar < {1} 
                ORDER BY SozBasTar DESC ", kiraciId.ReturnQuotedValue(), tarih.ReturnTRDateFormat());
                dataTable = dao.selectFromDb(sqlString, "");
                if (dataTable != null)
                {
                    List<KiraSozlesme> list = ToList<KiraSozlesme>(dataTable);
                    KiraSozlesme oncekiIlkKiraSozlesmesi = list.FirstOrDefault();
                    kiraSozlesme= oncekiIlkKiraSozlesmesi;
                }
                    
            }
            return kiraSozlesme;
        }
        public KiraSozlesme SelectBitenSozlesmeByKiraciId(int kiraciId)
        {
            string sqlString = string.Format(@"
                SELECT  *
                FROM KiraSozlesme_Table
                WHERE Aktif=0 AND KiraciId={0}
                ORDER BY SozBastar DESC", kiraciId.ReturnQuotedValue());
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            if (dataTable != null)
            {
                List<KiraSozlesme> list = ToList<KiraSozlesme>(dataTable);
                KiraSozlesme kiraSozlesme = list.FirstOrDefault();
                return kiraSozlesme;
            }
            else
            {
                return null;
            }

        }
        public string SelectBitenSozlesmelerByKiraciIdReturnJson(int kiraciId)
        {
            string sqlString = string.Format(@"
                SELECT Id SozlesmeId, FORMAT(SozBasTar,'dd/MM/yyyy') SozBasTar,
	                FORMAT(SozBitTar,'dd/MM/yyyy') SozBitTar,
                    DosyaNo,KiraBedeli
                FROM KiraSozlesme_Table
                WHERE Aktif=0 AND KiraciId={0}
                ORDER BY SozBitTar DESC", kiraciId.ReturnQuotedValue());
            DataTable dataTable = null;
            try
            {
                dataTable = dao.selectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                throw e;
            }
            string json = ToJSON(dataTable);
            return json;
        }
        public bool UpdateAktifDurum(string durum, string degistirmeTar, bool aktif)
        {
            string durumDegistirmeTar = string.IsNullOrEmpty(degistirmeTar) ? DateTime.Now.ConvertToTimeSpanReturnInHHmm() : degistirmeTar;
            string sqlString = string.Format(@" UPDATE KiraSozlesme_Table 
                                SET Aktif={0},
                                    SozlesmeDurumu={1},
                                    DurumDegismeTar={2} 
                                WHERE  Aktif=1 AND Id={3}", aktif.ReturnQuotedValue(), durum.ReturnQuotedValue(), durumDegistirmeTar.ReturnQuotedValue(), Id);
            bool isSaved = dao.Update2Db(sqlString);
            return isSaved;
        }
        public List<KiraSozlesme> SelectByTasinmazId(int tasinmazId)
        {
            string sqlString = string.Format(@"
                SELECT A.*
                    FROM KiraSozlesme_Table A
                    LEFT OUTER JOIN SozlesmeTasinmaz_Table B ON B.SozlesmeId=A.Id
                    LEFT OUTER JOIN Bagis_Table C ON C.TasinmazId=B.TasinmazId
                WHERE A.Aktif=1 AND B.TasinmazId={0}
                ORDER BY B.TasinmazId", tasinmazId.ReturnQuotedValue());
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            if (dataTable != null)
            {
                List<KiraSozlesme> list = ToList<KiraSozlesme>(dataTable);
                return list;
            }
            else
            {
                return null;
            }

        }
        public DataTable SelectBySozlesmeId(int sozlesmeId)
        {
            string sqlString = string.Format(@"
                SELECT A.Id,C.Adres
                FROM KiraSozlesme_Table A
	                LEFT JOIN SozlesmeTasinmaz_Table B On B.SozlesmeId=A.Id
	                LEFT JOIN Tasinmaz_Table C On C.Id=B.TasinmazId
                WHERE A.Id= {0}
                ", sozlesmeId.ReturnQuotedValue());
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            return dataTable;

        }
        public DataTable SelectSUMAdetByBolge(string bolge)
        {
            string bolgeStr = string.IsNullOrEmpty(bolge) || bolge.Equals(ProjeConstants.BOLGE_HEPSI) ? "" : " AND Bolge=" + bolge;
            string sqlString = string.Format(@"				
                Select Bolge,  Count(A.Id) Adet, SUM(TeminatTutari) TeminatTutari
                From KiraSozlesme_Table A
                Where A.Aktif=1
                Group By Bolge
                Order By Bolge", bolgeStr);

            DataTable dataTable = null;
            try
            {
                dataTable = dao.selectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                throw e;
            }
            return dataTable;
        }
        public DataTable SelectSUMTeminatByBolgeKiralamaAmaciReturnDT(string bolge, string kiralamaAmaci)
        {
            string bolgeStr = string.IsNullOrEmpty(bolge) || bolge.Equals(ProjeConstants.BOLGE_HEPSI) ? "" : " AND Bolge=" + bolge.ReturnQuotedValue();
            string kiralamaAmaciStr = string.IsNullOrEmpty(kiralamaAmaci) || kiralamaAmaci.Equals(ProjeConstants.HEPSI) ? "" : " AND KiralamaAmaci=" + kiralamaAmaci.ReturnQuotedValue();
            string sqlString = string.Format(@"				
				SELECT Bolge,  B.KiralamaAmaci, COUNT(A.Id) Adet, SUM(TeminatTutari) TeminatTutari, SUM(KalanTeminatTutari) KalanTeminatTutari
                FROM KiraSozlesme_Table A
					INNER JOIN Kiraci_Table B ON B.Id=A.KiraciId
                WHERE A.Aktif=1
                    {0}
                    {1}
                GROUP BY A.Bolge, B.KiralamaAmaci
                ORDER BY Bolge", bolgeStr, kiralamaAmaciStr);

            DataTable dataTable = null;
            try
            {
                dataTable = dao.selectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                throw e;
            }
            return dataTable;
        }
        public KiraSozlesme SelectNext()
        {
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            string sqlString = string.Empty;
            if (DosyaNo > 0)
            {
                sqlString = string.Format(@"
                    SELECT * FROM KiraSozlesme_Table
                    WHERE Aktif=1 AND DosyaNo > {0}
                    ORDER BY DosyaNo,Id ", DosyaNo.ReturnQuotedValue());
            }
            else
            {
                sqlString = string.Format(@"
                    SELECT * FROM KiraSozlesme_Table
                    WHERE Aktif=1 AND DosyaNo IS NULL AND Id > {0}
                    ORDER BY DosyaNo,Id ", Id.ReturnQuotedValue());
            }

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            if (dataTable != null)
            {
                List<KiraSozlesme> list = ToList<KiraSozlesme>(dataTable);
                kiraSozlesme = list.FirstOrDefault();
            }
            else
            {
                kiraSozlesme = SelectMin();

            }
            return kiraSozlesme;
        }

        public KiraSozlesme SelectOncekiKiraSozlesme()
        {
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            string sqlString = string.Empty;
            sqlString = string.Format(@"
                    SELECT * FROM KiraSozlesme_Table
                    WHERE KiraciId={0} AND SozBastar < {1}
                    ORDER BY SozBasTar DESC", KiraciId.ReturnQuotedValue(), SozBasTar.ReturnTRDateFormat());
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            if (dataTable != null)
            {
                List<KiraSozlesme> list = ToList<KiraSozlesme>(dataTable);
                kiraSozlesme = list.FirstOrDefault();
            }
            else
            {
                kiraSozlesme = null;
            }
            return kiraSozlesme;
        }

        public KiraSozlesme SelectPrev()
        {
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            string sqlString = string.Empty;
            if (DosyaNo > 0)
            {
                sqlString = string.Format(@"
                    SELECT * FROM KiraSozlesme_Table
                    WHERE Aktif=1 AND DosyaNo < {0}
                    ORDER BY DosyaNo DESC ,Id DESC", DosyaNo.ReturnQuotedValue());
            }
            else
            {
                sqlString = string.Format(@"
                    SELECT * FROM KiraSozlesme_Table
                    WHERE Aktif=1 AND DosyaNo IS NULL AND Id < {0}
                    ORDER BY DosyaNo DESC,Id DESC ", Id.ReturnQuotedValue());
            }

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            if (dataTable != null)
            {
                List<KiraSozlesme> list = ToList<KiraSozlesme>(dataTable);
                kiraSozlesme = list.FirstOrDefault();
            }
            else
            {
                kiraSozlesme = SelectMax();

            }
            return kiraSozlesme;
        }
        public KiraSozlesme SelectMax()
        {
            KiraSozlesme kiraSozlesme = null;
            string sqlString = string.Format(@"
                SELECT * FROM KiraSozlesme_Table
                WHERE DosyaNo=(
                    SELECT MAX(DosyaNo) 
                    FROM KiraSozlesme_Table
                    WHERE Aktif=1  
                    ) ");
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            if (dataTable != null)
            {
                List<KiraSozlesme> list = ToList<KiraSozlesme>(dataTable);
                kiraSozlesme = list.FirstOrDefault();
            }
            return kiraSozlesme;
        }
        public KiraSozlesme SelectMin()
        {
            KiraSozlesme kiraSozlesme = null;
            string sqlString = string.Format(@"
                SELECT * FROM KiraSozlesme_Table
                WHERE DosyaNo=(
                    SELECT MIN(DosyaNo) 
                    FROM KiraSozlesme_Table
                    WHERE Aktif=1 AND DosyaNo>0
                    ) ");
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            if (dataTable != null)
            {
                List<KiraSozlesme> list = ToList<KiraSozlesme>(dataTable);
                kiraSozlesme = list.FirstOrDefault();
            }
            return kiraSozlesme;
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

            DataTable dataTable = dao.selectFromDb(sqlString, "");
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

            DataTable dataTable = dao.selectFromDb(sqlString, "");
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
            DataTable dataTable = dao.selectFromDb(sqlString, "");
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
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            if (dataTable != null)
            {
                List<KiraSozlesme> list = ToList<KiraSozlesme>(dataTable);
                kiraSozlesme = list.FirstOrDefault();
            }
            return kiraSozlesme;
        }

        internal KiraSozlesme SelectOncekiSozlesme(KiraSozlesme kiraSozlesme)
        {
            DateTime bastar = kiraSozlesme.SozBasTar;
            KiraSozlesme oncekiKiraSozlesme = null;
            string sqlString = string.Format(@"
                SELECT * FROM KiraSozlesme_Table
                FROM KiraSozlesme_Table
                WHERE KiraciId={0} AND SozBasTar<{1}
                order by SozBasTar Desc, Id DESC
                    ) ", kiraSozlesme.KiraciId, SozBasTar.ReturnTRDateFormat());
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            if (dataTable != null)
            {
                List<KiraSozlesme> list = ToList<KiraSozlesme>(dataTable);
                oncekiKiraSozlesme = list.FirstOrDefault();
            }
            return oncekiKiraSozlesme;
        }
    }
}
