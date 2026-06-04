using DocumentFormat.OpenXml.Wordprocessing;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.TBYS
{
    [Serializable]
    public class OdemePlani : ParentClass
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
        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM OdemePlani_Table 
                               WHERE  Id={0}", id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<OdemePlani> list = ToList<OdemePlani>(dataTable);
            OdemePlani odemePlani = new OdemePlani();
            odemePlani = list.FirstOrDefault();
            return (T)Convert.ChangeType(odemePlani, typeof(T));

        }
        public override int Save()
        {
            try
            {
                GenericEntity<OdemePlani> genericEntity = new GenericEntity<OdemePlani>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);

                this.Id = id;
                if (id > 0 && ProjeConstants.TBYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEMEPLANI);
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
                    OdemePlani item = Select<OdemePlani>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<OdemePlani> genericEntity = new GenericEntity<OdemePlani>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        string sqlString = genericEntity.GetQuery(this);
                        isSuccess = dao.Update2Db(sqlString);
                    }
                    if (isSuccess && ProjeConstants.TBYS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEMEPLANI);
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
                    GenericEntity<OdemePlani> genericEntity = new GenericEntity<OdemePlani>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);
                    OdemePlani item = Select<OdemePlani>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.TBYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEMEPLANI);
                    }
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public bool DeleteBySozlesmeId(int sozlesmeId)
        {
            string sqlString = string.Format(@" 
                DELETE OdemePlani_Table 
                WHERE SozlesmeId={0}", sozlesmeId);
            bool isDeleted = dao.DeleteFromDb(sqlString, this);
            if (isDeleted && ProjeConstants.TBYS_DELETE_LOG)
            {
                OlayKayit olayKayit = new OlayKayit();
                olayKayit.SilmeOlayKaydet(this, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEMEPLANI);
            }
            return isDeleted;
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM OdemePlani_Table");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<OdemePlani> list = ToList<OdemePlani>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }

        public DataTable SelectBorcluOdemePlanlariByBolgeTarih(int bolgeId, DateTime ilkTarih, DateTime sonTarih, int aySayisiBas, int aySayisiBit)
        {
            string sqlString = GetBorcluOdemePlanlariByBolgeTarihSqlScript(bolgeId, ilkTarih, sonTarih, aySayisiBas, aySayisiBit);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            return dataTable;
        }
        public string SelectBorcluOdemePlanlariByBolgeTarihJson(int bolgeId, DateTime ilkTarih, DateTime sonTarih, int aySayisi, int aySayisiBit, ref int kayitSayisi)
        {
            string sqlString = GetBorcluOdemePlanlariByBolgeTarihSqlScript(bolgeId, ilkTarih, sonTarih, aySayisi, aySayisiBit);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            kayitSayisi = dataTable == null ? 0 : dataTable.Rows.Count;
            string json = ToJSON(dataTable);
            return json;

        }
        private string GetBorcluOdemePlanlariByBolgeTarihSqlScript(int bolgeId, DateTime ilkTarih, DateTime sonTarih, int aySayisiBas, int aySayisiBit)
        {
            //string aySayisiStr = string.Format(" ((ABS(C.FaizliBakiye) - ABS(A.KiraBedeli))  / A.KiraBedeli) BETWEEN {0} AND {1}  AND ", (aySayisiBas - 0.5).ToString().Replace(",", "."), (aySayisiBit + 0.5).ToString().Replace(",", "."));
            string aySayisiStr = string.Format(@"
                (
                    (
                        A.TaksitSayisi>1 AND ((ABS(C.FaizliBakiye) - ABS(A.KiraBedeli))  / A.KiraBedeli) BETWEEN {2} AND {3}  
                    )
                    OR
                    (
                        A.TaksitSayisi<2 AND 
                        (
                            (C.VadeBitTar BETWEEN {0} AND {1} )
						    AND 
                            (ABS(FaizliBakiye/A.KiraBedeli)*C.Sira BETWEEN {2} AND {3}) 
                        )
                     ) 
                ) ", ilkTarih.ReturnTRDateFormat(), sonTarih.ReturnDDMMYYYFormat(), (aySayisiBas - 0.5).ToString().Replace(",", "."), (aySayisiBit + 0.5).ToString().Replace(",", "."));

            string bolgeStr = bolgeId == ProjeConstants.HEPSI_INT || bolgeId == ProjeConstants.BOLGE_GENELMUDURLUK_INT ? string.Empty : string.Format(" AND A.BolgeId={0} ", bolgeId);


            string sqlString = string.Format(@"
                SELECT 
                    A.Id KiraSozlesmeId, H.KisaAdi Bolge, A.DosyaNo, B.Adi+' '+B.Soyadi Kiraci, A.KiraciId,
                    A.IlkSozlesmeTar, A.SozBasTar, A.SozBitTar, A.ArtisAyi, A.OdemeSekli, 
                    A.KiraBedeli, C.AnaPara AnaPara,C.FaizTutari, C.FaizliBakiye, C.VadeBasTar, C.VadeBitTar, C.Id OdemePlaniId,
                    FORMAT(C.FaizliBakiye,'###.00') FaizliBakiyeFormat,
                    B.Adres, B.Ili,B.Ilcesi,B.Semt,
	                A.TeminatOdemeTarihi,A.TeminatTutari,
                    ABS(FaizliBakiye/A.KiraBedeli)*C.Sira AySayisi, A.TaksitSayisi
                FROM KiraSozlesme_Table A
                    INNER JOIN Kiraci_Table B ON B.Id=A.KiraciId
                    INNER JOIN Bolge_Table H ON H.Id=A.BolgeId
                    LEFT JOIN OdemePlani_Table C ON C.SozlesmeId=A.Id 
                WHERE 1>0
                    {0}
                    AND C.FaizliBakiye < 0 AND (C.VadeBitTar BETWEEN {1} AND {2}) AND
	                {3} AND 
                    (
                        A.SozBasTar<{2} AND A.SozBitTar >= {1} 
						AND
                        (--SB 12.08.2022 parantez icine aldim cift cikan kayitlar oluyordu
							(
								A.SozlesmeDurumu = 'Devam Ediyor' 
								OR
								(A.SozlesmeDurumu != 'Devam Ediyor' AND A.SozlesmeDurumu != 'Yenilendi' AND A.DurumDegismeTar BETWEEN {1} AND {2})
							)
						    OR 
						    (A.SozlesmeDurumu='Yenilendi' AND A.SozBitTar>{2}) 
                        )--SB 12.08.2022
                    )
                ORDER BY A.BolgeId, A.DosyaNo,B.Adi

                ", bolgeStr, ilkTarih.ReturnTRDateFormat(), sonTarih.ReturnDDMMYYYFormat(), aySayisiStr);
            return sqlString;
        }
        public DataTable SelectMevcutOdemePlanlariByTarih(DateTime ilkTarih, DateTime sonTarih, string bolge)
        {
            string bolgeStr = string.Format(bolge.Equals(ProjeConstants.HEPSI) || string.IsNullOrEmpty(bolge) ? " " : " AND E.Bolge ={0} ", bolge.ReturnQuotedValue());
            string sqlString = string.Format(@"
                SELECT A.Id KiraSozlesmeId, E.Bolge, A.DosyaNo, G.Adi+' '+G.Soyadi Kiraci, D.Adres + ' ' + ISNULL(F.BolumNo,'') Adres, 
                    A.IlkSozlesmeTar, A.SozBasTar, A.SozBitTar, A.ArtisAyi, A.OdemeSekli, 
                    A.KiraBedeli, B.AnaPara AnaPara,B.FaizTutari, B.FaizliBakiye, B.VadeBasTar, 
					D.KullanimSekli,D.Ili,D.Ilcesi,
					A.TeminatOdemeTarihi,A.TeminatTutari
                FROM KiraSozlesme_Table A
                    LEFT JOIN OdemePlani_Table B ON B.SozlesmeId=A.Id AND B.Id in (SELECT Id FROM OdemePlani_Table WHERE VadeBasTar BETWEEN {0} AND {1} )
                    LEFT JOIN SozlesmeTasinmaz_Table C On C.SozlesmeId=A.Id AND C.Id = (Select top 1 Id from SozlesmeTasinmaz_Table where SozlesmeId = A.Id) 
                    LEFT JOIN Tasinmaz_Table D On D.Id=C.TasinmazId
                    LEFT JOIN Il_Table E ON E.IlAdi=D.Ili
                    LEFT JOIN BagimsizBolum_Table F On F.Id=C.BolumId
	                LEFT JOIN Kiraci_Table G On G.Id=A.KiraciId
                WHERE A.Aktif= 1
                    {2} 
                ORDER BY CASE WHEN A.DosyaNo = 0 THEN 2 ELSE 1 END,ISNULL(A.DosyaNo, 999999),  A.Id
	                
                ", ilkTarih.ReturnTRDateFormat(), sonTarih.ReturnTRDateFormat(), bolgeStr);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            return dataTable;
        }
        public List<OdemePlani> SelectBySozlesmeId(int sozlesmeId)
        {
            string sqlString = string.Format(@"
                SELECT * FROM OdemePlani_Table
                WHERE SozlesmeId={0}
                ORDER BY Sira ", sozlesmeId.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<OdemePlani> list = ToList<OdemePlani>(dataTable);
            return list;

        }
        public OdemePlani SelectBySozlesmeIdSira(int sozlesmeId, int sira)
        {
            string sqlString = string.Format(@"
                SELECT * FROM OdemePlani_Table
                WHERE SozlesmeId={0} AND Sira={1}
            ", sozlesmeId.ReturnQuotedValue(), sira.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<OdemePlani> list = ToList<OdemePlani>(dataTable);
            OdemePlani odemePlani = list.FirstOrDefault<OdemePlani>();
            return odemePlani;

        }
        public DataTable SelectKiraGeliriByBolgeAyYil(int bolgeId, int ay, int yil)
        {
            string bolgeStr = bolgeId == ProjeConstants.HEPSI_INT || bolgeId == ProjeConstants.BOLGE_GENELMUDURLUK_INT ? string.Empty : string.Format(" AND C.BolgeId={0} ", bolgeId);

            string sqlString = string.Format(@"
				SELECT H.KisaAdi Bolge,B.kiralamaAmaci, SUM(A.OdenenTutar) ToplamOdemeTutari, COUNT(DISTINCT(C.Id)) ToplamKiraciSayisi
				FROM Odeme_Table A
					INNER JOIN Kiraci_Table B ON B.Id=A.KiraciId 
					INNER JOIN KiraSozlesme_Table C ON C.Id=A.SozlesmeId
					INNER JOIN OdemePlani_Table D ON D.Id=A.OdemePlaniId
					LEFT JOIN TeminatIslem_Table E ON E.OdemeId=A.Id
					LEFT JOIN Bolge_Table H ON H.Id=C.BolgeId
                WHERE  
                    YEAR(A.OdemeTarihi) ={0}
                    AND MONTH(A.OdemeTarihi) ={1}
                    {2}
                GROUP BY H.KisaAdi, B.KiralamaAmaci
                ORDER BY H.KisaAdi, B.KiralamaAmaci
                
            ", yil, ay, bolgeStr);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            return dataTable;

        }
        public OdemePlani SelectBySozlesmeIdOdemeTarihi(int sozlesmeId, DateTime odemeTarihi)
        {
            string vadeBasTarStr = string.Format(" AND {0} BETWEEN VadeBasTar AND DateADD(day,-1,DATEADD(month,1,VadeBasTar)) ", odemeTarihi.ReturnTRDateFormat());
            string sqlString = string.Format(@"
                SELECT * FROM OdemePlani_Table
                WHERE Sira!=0 AND SozlesmeId={0} 
                {1} 
                ORDER BY VadeBasTar ", sozlesmeId.ReturnQuotedValue(), vadeBasTarStr);
            //Sira!=0 olmali çünkü ilk ay devir ayi
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<OdemePlani> list = ToList<OdemePlani>(dataTable);
            OdemePlani odemePlani = list.FirstOrDefault<OdemePlani>();
            return odemePlani;

        }
        public bool OdemePlaniVarMi(int sozlesmeId)
        {
            string sqlString = string.Format(@"SELECT * FROM OdemePlani_Table
                               WHERE SozlesmeId={0}", sozlesmeId.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<OdemePlani> list = ToList<OdemePlani>(dataTable);
            return list.Count > 0;

        }
        public OdemePlani SelectSonOdemePlaniBySozlesmeId(int sozlesmeId)
        {
            DateTime tarih = DateTime.MinValue;
            string sqlString = string.Format(@"
                SELECT *
                FROM OdemePlani_Table 
                WHERE SozlesmeId = {0}
                ORDER BY VadeBasTar ", sozlesmeId);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<OdemePlani> list = ToList<OdemePlani>(dataTable);
            OdemePlani odemePlani = list.LastOrDefault<OdemePlani>();
            return odemePlani;
        }
        public OdemePlani SelectIlkOdemePlaniBySozlesmeId(int sozlesmeId)
        {
            DateTime tarih = DateTime.MinValue;
            string sqlString = string.Format(@"
                SELECT *
                FROM OdemePlani_Table 
                WHERE SozlesmeId = {0} AND Sira=1
                ", sozlesmeId);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<OdemePlani> list = ToList<OdemePlani>(dataTable);
            OdemePlani odemePlani = list.FirstOrDefault<OdemePlani>();
            return odemePlani;
        }
        public DataTable SelectOdemePlaniListByTarihReturnDT(DateTime tarih)
        {
            string sqlString = string.Format(@"
                SELECT A.Id SozlesmeId,A.DosyaNo,B.Adi, B.Soyadi, B.Adi+' '+ B.Soyadi KiraciAdi,
	                D.Adres, G.BolumNo, F.OdemeBasTar,F.OdemeBitTar,F.KiraBedeli,F.OdenenTutar,F.FaizliBakiye
                FROM KiraSozlesme_Table A
                INNER JOIN Kiraci_Table B ON B.Id=A.KiraciId
                    LEFT JOIN SozlesmeTasinmaz_Table C On C.Id=(Select top 1 Id from SozlesmeTasinmaz_Table where SozlesmeId=A.Id) 
                    LEFT JOIN Tasinmaz_Table D On D.Id=C.TasinmazId
                    LEFT JOIN BagimsizBolum_Table G ON G.Id=C.BolumId
                    LEFT JOIN Il_Table E ON E.IlAdi=D.Ili
                    LEFT JOIN OdemePlani_Table F ON F.Id=(Select MAX(Id) from OdemePlani_Table where SozlesmeId=A.Id AND VadeBasTar < {0}) 
                WHERE Aktif=1
                ORDER BY CASE WHEN A.DosyaNo=0 THEN 2 ELSE 1 END,ISNULL(A.DosyaNo,999999),  ISNULL(A.BolgeId,0),  A.Id
                ", tarih.ReturnTRDateFormat());
            DataTable dataTable = null;
            try
            {
                dataTable = dao.SelectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                throw e;
            }
            return dataTable;
        }
        public bool OdemePlaniOlustur(KiraSozlesme kiraSozlesme)
        {
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
            odemePlani.Save();
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
            odemePlani.Save();
            return odemePlani;
        }

    }
}
