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
    public class Odeme : ParentClass
    {
        public int SozlesmeId { get; set; }
        public int KiraciId { get; set; }
        public int OdemePlaniId { get; set; }
        public DateTime OdemeTarihi { get; set; }
        public decimal OdenenTutar { get; set; }
        public string Aciklama { get; set; }
        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM Odeme_Table 
                               WHERE  Id={0}", id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Odeme> list = ToList<Odeme>(dataTable);
            Odeme odeme = new Odeme();
            odeme = list.FirstOrDefault();
            return (T)Convert.ChangeType(odeme, typeof(T));

        }
        public Odeme Select(int id)
        {
            GenericEntity<Odeme> genericEntity = new GenericEntity<Odeme>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Odeme> list = ToList<Odeme>(dataTable);
            Odeme odeme = new Odeme();
            odeme = list.FirstOrDefault();
            return odeme;
        }
        public override int Save()
        {
            try
            {
                GenericEntity<Odeme> genericEntity = new GenericEntity<Odeme>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);

                this.Id = id;
                if (id > 0 && ProjeConstants.TBYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEME);
                }
                return id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public override bool Update()
        {
            bool isSuccess = false;
            try
            {
                if (this != null)
                {
                    Odeme item = Select<Odeme>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<Odeme> genericEntity = new GenericEntity<Odeme>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        string sqlString = genericEntity.GetQuery(this);
                        isSuccess = dao.Update2Db(sqlString);
                    }
                    if (isSuccess && ProjeConstants.TBYS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEME);
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
                    GenericEntity<Odeme> genericEntity = new GenericEntity<Odeme>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);
                    Odeme item = Select<Odeme>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.TBYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEME);
                    }
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string GetInsertSQL(string extId)
        {
            try
            {
                GenericEntity<Odeme> genericEntity = new GenericEntity<Odeme>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                string sqlString = genericEntity.GetQuery(this, extId) + " ;SELECT SCOPE_IDENTITY() ";

                return sqlString;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public string GetUpdateSQL(string extId)
        {
            try
            {
                GenericEntity<Odeme> genericEntity = new GenericEntity<Odeme>(ProjeConstants.SQL_UPDATE);
                OlusturmaTarihi = DateTime.Now;
                string sqlString = genericEntity.GetQuery(this, extId);

                return sqlString;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public string GetDeleteSQL(string extId)
        {
            try
            {
                GenericEntity<Odeme> genericEntity = new GenericEntity<Odeme>(ProjeConstants.SQL_DELETE);
                OlusturmaTarihi = DateTime.Now;
                string sqlString = genericEntity.GetQuery(this, extId);

                return sqlString;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public bool DeleteBySozlesmeId(int sozlesmeId)
        {
            string sqlString = string.Format(@"
                DELETE Odeme_Table 
                WHERE SozlesmeId={0}", sozlesmeId);
            bool isSuccess = dao.DeleteFromDb(sqlString, this);
            return isSuccess;
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM Odeme_Table");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Odeme> list = ToList<Odeme>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public List<Odeme> SelectByKiraciId(int kiraciId)
        {
            string sqlString = string.Format(@"
                SELECT * FROM Odeme_Table
                WHERE KiraciId={0}
                ORDER BY OdemeTarihi Desc", kiraciId.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Odeme> list = ToList<Odeme>(dataTable);
            return list;

        }
        public List<Odeme> SelectBySozlesmeIdOdemePlaniId(int sozlesmeId, int odemePlaniId)
        {
            string sqlString = string.Format(@"
                SELECT * FROM Odeme_Table
                WHERE SozlesmeId={0} AND OdemePlaniId={1}
                ORDER BY OdemeTarihi ", sozlesmeId.ReturnQuotedValue(), odemePlaniId);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Odeme> list = ToList<Odeme>(dataTable);
            return list;

        }
        public DataTable SelectByKiraciAyYil(int kiraciId, int ay, int yil)
        {
            string ayYilStr = string.Empty;
            if ((ay != ProjeConstants.HEPSI_INT) && (yil != ProjeConstants.HEPSI_INT))
            {
                ayYilStr = string.Format(@"
                    WHERE MONTH(A.OdemeTarihi)={0} AND YEAR(A.OdemeTarihi) ={1}", ay.ReturnQuotedValue(), yil.ReturnQuotedValue());
            }
            else if ((ay == ProjeConstants.HEPSI_INT) && (yil != ProjeConstants.HEPSI_INT))
            {
                ayYilStr = string.Format(@"
                    WHERE YEAR(A.OdemeTarihi) ={0}", yil.ReturnQuotedValue());
            }
            else if ((ay != ProjeConstants.HEPSI_INT) && (yil == ProjeConstants.HEPSI_INT))
            {
                ayYilStr = string.Format(@"
                    WHERE MONTH(A.OdemeTarihi)={0} ", ay.ReturnQuotedValue());
            }
            //WHERE MONTH(OdemeTarihi)={0} AND YEAR(OdemeTarihi)={1} {2}
            string kiraciIdStr = kiraciId > 0 ? string.Format(@" AND A.KiraciId={0}", kiraciId) : string.Empty;

            string sqlString = string.Format(@"
				SELECT A.Id, B.Adi KiraciAdiSoyadi, F.Bolge, 
                    A.Id OdemeId, A.OdemeTarihi, A.OdenenTutar, A.Aciklama, A.SozlesmeId, A.KiraciId, A.OdemePlaniId, B.KiralamaAmaci 
                FROM Odeme_Table A
                    INNER JOIN Kiraci_Table B ON B.Id=A.KiraciId 
				    INNER JOIN KiraSozlesme_Table C ON C.Id=A.SozlesmeId
					INNER JOIN SozlesmeTasinmaz_Table D ON D.Id=(Select TOP 1 Id From SozlesmeTasinmaz_Table WHERE SozlesmeId=A.SozlesmeId) 
					INNER JOIN Tasinmaz_Table E ON E.Id=D.TasinmazId 
                    INNER JOIN Il_Table F ON F.IlAdi= E.Ili
                {0} {1}
                ORDER BY A.OdemeTarihi DESC, B.Id, A.SozlesmeId
            ", ayYilStr, kiraciIdStr);
            // bölge de seçime eklendiği için sorgu üstteki ile değişti SB 23/09/2019
            //string sqlString = string.Format(@"
            //    SELECT A.Id, B.Adi,B.Soyadi,  B.Adi+' '+B.Soyadi KiraciAdiSoyadi,
            //        A.Id OdemeId, A.OdemeTarihi, A.OdenenTutar, A.Aciklama, A.SozlesmeId, A.KiraciId, A.OdemePlaniId 
            //    FROM Odeme_Table A
            //    INNER JOIN Kiraci_Table B ON B.Id=A.KiraciId 
            //    {0} {1}
            //    ORDER BY A.OdemeTarihi DESC, B.Id, A.SozlesmeId
            //", ayYilStr,kiraciIdStr);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");

            return dataTable;

        }
        public DataTable SelectByKiraciAyYilReturnDataTable(int bolgeId,int kiraciId,DateTime bastar, DateTime bittar)//int ay, int yil)
        {
            bittar = UtilityHelper.TariheSaatEkle(bittar, "23:59:59");
            string   bastarStr = string.Format(@"
                    AND A.OdemeTarihi BETWEEN {0} AND {1}", bastar.ReturnQuotedValue(), bittar.ReturnQuotedValue());

            //WHERE MONTH(OdemeTarihi)={0} AND YEAR(OdemeTarihi)={1} {2}
            string kiraciIdStr = kiraciId > 0 ? string.Format(@" AND A.KiraciId={0}", kiraciId) : string.Empty;
            string bolgeStr = ((bolgeId == ProjeConstants.HEPSI_INT) || (bolgeId == ProjeConstants.BOLGE_GENELMUDURLUK_INT)) ? string.Empty : string.Format(" AND C.BolgeId={0}", bolgeId);
            string sqlString = string.Format(@"
				SELECT A.Id, A.Id OdemeId,A.OdemePlaniId,A.SozlesmeId,A.KiraciId,
	                A.OdemeTarihi, A.OdenenTutar, A.Aciklama, 
	                B.Adi, B.Soyadi, B.KiralamaAmaci, 
	                C.SozBasTar, C.SozBitTar, C.IlkSozlesmeTar,C.DosyaNo, C.ArtisAyi,C.KiraBedeli,C.OdemeSekli, 
	                D.VadeBitTar, E.Id TeminatId,
					H.KisaAdi Bolge
                FROM Odeme_Table A
                
                    INNER JOIN Kiraci_Table B ON B.Id=A.KiraciId 
	                INNER JOIN KiraSozlesme_Table C ON C.Id=A.SozlesmeId
	                INNER JOIN OdemePlani_Table D ON D.Id=A.OdemePlaniId
                    LEFT JOIN TeminatIslem_Table E ON E.OdemeId=A.Id
					LEFT JOIN Bolge_Table H ON H.Id=C.BolgeId
                WHERE 1>0 
                {0} {1} {2}
                ORDER BY A.OdemeTarihi DESC, B.Id, A.SozlesmeId
            ", bastarStr, kiraciIdStr,bolgeStr);
            // bölge de seçime eklendiği için sorgu üstteki ile değişti SB 23/09/2019
            //string sqlString = string.Format(@"
            //    SELECT A.Id, B.Adi,B.Soyadi,  B.Adi+' '+B.Soyadi KiraciAdiSoyadi,
            //        A.Id OdemeId, A.OdemeTarihi, A.OdenenTutar, A.Aciklama, A.SozlesmeId, A.KiraciId, A.OdemePlaniId 
            //    FROM Odeme_Table A
            //    INNER JOIN Kiraci_Table B ON B.Id=A.KiraciId 
            //    {0} {1}
            //    ORDER BY A.OdemeTarihi DESC, B.Id, A.SozlesmeId
            //", ayYilStr,kiraciIdStr);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");

            return dataTable;

        }
        public DataTable SelectByAyYilReturnDataTable( int ay, int yil)
        {
            string ayYilStr = string.Empty;
            if ((ay != ProjeConstants.HEPSI_INT) && (yil != ProjeConstants.HEPSI_INT))
            {
                ayYilStr = string.Format(@"
                    WHERE MONTH(A.OdemeTarihi)={0} AND YEAR(A.OdemeTarihi) ={1}", ay.ReturnQuotedValue(), yil.ReturnQuotedValue());
            }
            else if ((ay == ProjeConstants.HEPSI_INT) && (yil != ProjeConstants.HEPSI_INT))
            {
                ayYilStr = string.Format(@"
                    WHERE YEAR(A.OdemeTarihi) ={0}", yil.ReturnQuotedValue());
            }
            else if ((ay != ProjeConstants.HEPSI_INT) && (yil == ProjeConstants.HEPSI_INT))
            {
                ayYilStr = string.Format(@"
                    WHERE MONTH(A.OdemeTarihi)={0} ", ay.ReturnQuotedValue());
            }


            string sqlString = string.Format(@"
                SELECT 
	                A.OdemeTarihi, A.OdenenTutar, A.Aciklama, 
	                B.Adi, B.Soyadi 
	                --D.Id TeminatId, D.IslemTipi
                FROM Odeme_Table A
                    INNER JOIN Kiraci_Table B ON B.Id=A.KiraciId 
	                INNER JOIN KiraSozlesme_Table C ON C.Id=A.SozlesmeId	                
                    LEFT JOIN TeminatIslem_Table E ON E.OdemeId=A.Id
                {0} 
                ORDER BY A.OdemeTarihi DESC, B.Id, A.SozlesmeId
            ", ayYilStr);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");

            return dataTable;

        }
        public List<Odeme> SelectByKiraciVadeBasTarVadeBitTar(int sozlesmeId,int kiraciId, DateTime ilkTarih, DateTime ikinciTarih)
        {
            DateTime tarih1= new DateTime(ilkTarih.Year,ilkTarih.Month,ilkTarih.Day);
            DateTime tarih2= new DateTime(ikinciTarih.Year, ikinciTarih.Month, ikinciTarih.Day);
            DateTime tarihbas = UtilityHelper.TariheSaatEkle(tarih1, "00:00"); 
            DateTime tarihbit = UtilityHelper.TariheSaatEkle(tarih2, "23:59"); 
            string sqlString = string.Format(@"
				SELECT * 
                FROM Odeme_Table A
                WHERE KiraciId= {0} AND SozlesmeId={1}
                    AND OdemeTarihi BETWEEN {2} AND {3}
                ORDER BY A.OdemeTarihi 
            ", kiraciId,sozlesmeId, tarihbas.ConvertToDDMMYYYHHmmFormat().ReturnQuotedValue(), tarihbit.ConvertToDDMMYYYHHmmFormat().ReturnQuotedValue());

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Odeme> list = ToList<Odeme>(dataTable);
            return list;
        }
        public decimal SelectSumBySozlesmeIdOdemePlaniId(int sozlesmeId, int odemePlaniId)
        {
            decimal toplam = 0;
            string sqlString = string.Format(@"
                SELECT SUM(OdenenTutar) Toplam FROM Odeme_Table
                WHERE SozlesmeId={0} 
                    AND OdemePlaniId={1} 
                ", sozlesmeId.ReturnQuotedValue(), odemePlaniId.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            if (dataTable != null)
            {
                if (dataTable.Rows.Count > 0)
                {
                    DataRow row = dataTable.Rows[0];
                    toplam = row["Toplam"].ToString().ConvertToDecimal();
                }
            }

            return toplam;

        }
        public Odeme OdemeyiKaydetOdemePlaniniGuncelle(KiraSozlesme kiraSozlesme, OdemePlani odemePlani, DateTime odemeTarihi, decimal odenenTutar, string aciklama, string kullanici)
        {
            bool kaydedildiMi = false;

            Odeme odeme = null;

            try
            {
                //dao.StartTransaction();
                //odemeyi yap
                odeme = new Odeme();
                odeme.SozlesmeId = kiraSozlesme.Id;
                odeme.KiraciId = kiraSozlesme.KiraciId;
                odeme.OdemePlaniId = odemePlani.Id;
                odeme.OdemeTarihi = odemeTarihi;
                odeme.OdenenTutar = odenenTutar;
                odeme.Aciklama = aciklama;
                odeme.Olusturan = kullanici;
                odeme.Id = odeme.Save();
                if (odeme.Id > 0)
                {
                    // toplamı bul
                    Odeme odemeDao = new Odeme();


                    //toplam Odenenin bulunması Aylık ve Gunluk olarak ayrılmalı
                    //Aylık ise 
                    decimal toplamOdenen = odemeDao.SelectSumBySozlesmeIdOdemePlaniId(kiraSozlesme.Id, odemePlani.Id);
                    //Gunluk ise
                    // OdemeAyrintiya kayıt atsın TODO SB
                    //if (kiraSozlesme.GecikmeZammiTipi.Equals("Günlük"))
                    //{
                    //    UtilityHelper.OdemeAyrintiliGunlukFaizhesapla(kiraSozlesme, odemePlani);
                    //}
                    odemePlani.OdenenTutar = toplamOdenen;
                    odemePlani.Aciklama += aciklama + System.Environment.NewLine;
                    odemePlani.Degistiren = kullanici;
                    kaydedildiMi = odemePlani.Update();
                }
                //dao.EndTransaction();
            }
            catch (Exception exception1)
            {
                ExceptionHelper exHelper = new ExceptionHelper();
                Exception exception2 = new Exception("Ödeme Kaydedilemedi");
                exHelper.Exceptions.Add(exception2);
                exHelper.Exceptions.Add(exception1);
                exHelper.PublishException();
            }
            return odeme;
        }
        public bool OdemeyiSilOdemePlaniniGuncelle(int odemeId, string aciklama, int odemePlaniId, string kullanici)
        {
            bool silindiMi = false;
            try
            {
                IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
                Odeme odeme = new Odeme();
                odeme = odeme.Select<Odeme>(odemeId);
                if (odeme != null)
                {
                    silindiMi = odeme.Delete();
                }
                if (silindiMi)
                {
                    OdemeAyrinti odemeAyrintiDao = new OdemeAyrinti();
                    List<OdemeAyrinti> odemeAyrintiListesi = odemeAyrintiDao.SelectByOdemeIdOdemePlaniId(odemeId, odemePlaniId);
                    if (odemeAyrintiListesi.Count > 0)
                    {
                        odemeAyrintiDao.DeleteByOdemeIdOdemePlaniId(odemeId, odemePlaniId);
                    }

                    OdemePlani odemePlani = new OdemePlani();
                    odemePlani = odemePlani.Select<OdemePlani>(odemePlaniId);
                    Odeme odemeDao = new Odeme();
                    decimal toplamOdenen = odemeDao.SelectSumBySozlesmeIdOdemePlaniId(odemePlani.SozlesmeId, odemePlani.Id);
                    odemePlani.OdenenTutar = toplamOdenen;
                    odemePlani.Aciklama += aciklama + System.Environment.NewLine +
                        " *" + odeme.OdemeTarihi.ConvertToDatetimeEmptyIfNull() + " tarihli " + odeme.OdenenTutar.ToString("N", culturInfo) + " ödeme silindi." + System.Environment.NewLine;
                    odemePlani.Degistiren = kullanici;
                    odemePlani.Update();
                }

            }
            catch (Exception exception1)
            {

                ExceptionHelper exHelper = new ExceptionHelper();
                Exception exception2 = new Exception("Ödeme Silinemedi");
                exHelper.Exceptions.Add(exception2);
                exHelper.Exceptions.Add(exception1);
                exHelper.PublishException();
            }
            return silindiMi;
        }
        public bool OdemeyiVeOdemePlaniniGuncelle(int sozlesmeId, int oncekiOdemePlaniId,int yeniOdemePlaniId, int odemeId, DateTime odemeTarihi, decimal odenenTutar, string aciklama, string kullanici)
        {
            bool odemeVeOdemePlaniGuncellendiMi = false;
            try
            {
                
                OdemePlani oncekiOdemePlani = new OdemePlani();
                oncekiOdemePlani = oncekiOdemePlani.Select<OdemePlani>(oncekiOdemePlaniId);
                
                Odeme odeme = new Odeme();
                bool guncellendiMi = false;
                OdemePlani yeniOdemePlani = new OdemePlani();

                yeniOdemePlani = yeniOdemePlani.Select<OdemePlani>(yeniOdemePlaniId);//SelectBySozlesmeIdOdemeTarihi(sozlesmeId, odemeTarihi);

                if (yeniOdemePlani == null)
                {
                    yeniOdemePlani = new OdemePlani();
                    if (odemeTarihi <= oncekiOdemePlani.OdemeBasTar)
                    {
                        yeniOdemePlani = yeniOdemePlani.SelectIlkOdemePlaniBySozlesmeId(sozlesmeId);
                    }
                    else if (odemeTarihi >= oncekiOdemePlani.OdemeBitTar)
                    {
                        yeniOdemePlani = yeniOdemePlani.SelectSonOdemePlaniBySozlesmeId(sozlesmeId);
                    }
                    else
                    {
                        ExceptionHelper ex = new ExceptionHelper(new Exception("Ödeme Tablosunda OdemePlaniId=0 olduğundan kayıt yapılamadı"));
                        ex.PublishException();
                    }
                }
                //Odeme tablosunu güncelle
                if (yeniOdemePlani != null)
                {
                    odeme = odeme.Select(odemeId.ConvertToInt());
                    if (odeme != null)
                    {
                        odeme.OdemeTarihi = odemeTarihi;
                        odeme.OdenenTutar = odenenTutar;
                        odeme.Aciklama = aciklama;
                        odeme.Degistiren = kullanici;
                        odeme.OdemePlaniId = yeniOdemePlani.Id;
                        odeme.SozlesmeId = yeniOdemePlani.SozlesmeId;
                        guncellendiMi = odeme.Update();
                    }
                }
                
                if (guncellendiMi)
                {
                    //onceki Odeme planını güncelle
                    if (oncekiOdemePlani != null)
                    {
                        Odeme odemeDao = new Odeme();
                        decimal toplamOdenen = odemeDao.SelectSumBySozlesmeIdOdemePlaniId(oncekiOdemePlani.SozlesmeId, oncekiOdemePlani.Id);
                        oncekiOdemePlani.OdenenTutar = toplamOdenen;
                        oncekiOdemePlani.Degistiren = kullanici;
                        oncekiOdemePlani.Update();
                    }
                    //yeni Odeme planını güncelle
                    if (yeniOdemePlani != null)
                    {
                        Odeme odemeDao = new Odeme();
                        decimal yeniToplamOdenen = odemeDao.SelectSumBySozlesmeIdOdemePlaniId(yeniOdemePlani.SozlesmeId, yeniOdemePlani.Id);
                        yeniOdemePlani.OdenenTutar = yeniToplamOdenen;
                        yeniOdemePlani.Aciklama = aciklama;
                        yeniOdemePlani.Degistiren = kullanici;
                        odemeVeOdemePlaniGuncellendiMi = yeniOdemePlani.Update();
                    }
                        
                }

            }
            catch (Exception exception1)
            {
                odemeVeOdemePlaniGuncellendiMi = false;
                ExceptionHelper exHelper = new ExceptionHelper();
                Exception exception2 = new Exception("Ödeme Kaydedilemedi");
                exHelper.Exceptions.Add(exception2);
                exHelper.Exceptions.Add(exception1);
                exHelper.PublishException();
            }
            return odemeVeOdemePlaniGuncellendiMi;
        }
    }
}
