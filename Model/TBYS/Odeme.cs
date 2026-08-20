using DAO.Ortak;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
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
            SqlQuery query = new SqlQuery(@"SELECT *
                               FROM Odeme_Table 
                               WHERE  Id=@Id");
            query.AddParameter("@Id", id);
            DataTable dataTable = dao.SelectFromDb(query, "");
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
            SqlQuery query = genericEntity.GetQueryParametreli(this);

            DataTable dataTable = dao.SelectFromDb(query, "");
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
                SqlQuery query = genericEntity.GetQueryParametreli(this);
                int id = dao.Insert(query);

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
                    Odeme item = Select<Odeme>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<Odeme> genericEntity = new GenericEntity<Odeme>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        SqlQuery query = genericEntity.GetQueryParametreli(this);
                        isSuccess = dao.Update2Db(query);
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
                    SqlQuery query = genericEntity.GetQueryParametreli(this);
                    Odeme item = Select<Odeme>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(query, "");
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
                throw;
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

                throw;
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

                throw;
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

                throw;
            }
        }
        public bool DeleteBySozlesmeId(int sozlesmeId)
        {
            SqlQuery query = new SqlQuery(@"
                DELETE Odeme_Table 
                WHERE SozlesmeId=@SozlesmeId");
            query.AddParameter("@SozlesmeId", sozlesmeId);
            bool isSuccess = dao.DeleteFromDb(query, "");
            return isSuccess;
        }
        public override List<T> SelectAll<T>()
        {
            SqlQuery query = new SqlQuery(@"SELECT *
                               FROM Odeme_Table");

            DataTable dataTable = dao.SelectFromDb(query, "");
            List<Odeme> list = ToList<Odeme>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public List<Odeme> SelectByKiraciId(int kiraciId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT * FROM Odeme_Table
                WHERE KiraciId=@KiraciId
                ORDER BY OdemeTarihi Desc");
            query.AddParameter("@KiraciId", kiraciId);
            DataTable dataTable = dao.SelectFromDb(query, "");
            List<Odeme> list = ToList<Odeme>(dataTable);
            return list;

        }
        public List<Odeme> SelectBySozlesmeIdOdemePlaniId(int sozlesmeId, int odemePlaniId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT * FROM Odeme_Table
                WHERE SozlesmeId=@SozlesmeId AND OdemePlaniId=@OdemePlaniId
                ORDER BY OdemeTarihi ");
            query.AddParameter("@SozlesmeId", sozlesmeId);
            query.AddParameter("@OdemePlaniId", odemePlaniId);
            DataTable dataTable = dao.SelectFromDb(query, "");
            List<Odeme> list = ToList<Odeme>(dataTable);
            return list;

        }
		public DataTable SelectByKiraciAyYil(int kiraciId, int ay, int yil)
		{
			StringBuilder sb = new StringBuilder(@"
				SELECT A.Id, B.Adi KiraciAdiSoyadi, F.Bolge, 
					A.Id OdemeId, A.OdemeTarihi, A.OdenenTutar, A.Aciklama, A.SozlesmeId, A.KiraciId, A.OdemePlaniId, B.KiralamaAmaci 
				FROM Odeme_Table A
					INNER JOIN Kiraci_Table B ON B.Id=A.KiraciId 
					INNER JOIN KiraSozlesme_Table C ON C.Id=A.SozlesmeId
					INNER JOIN SozlesmeTasinmaz_Table D ON D.Id=(Select TOP 1 Id From SozlesmeTasinmaz_Table WHERE SozlesmeId=A.SozlesmeId) 
					INNER JOIN Tasinmaz_Table E ON E.Id=D.TasinmazId 
					INNER JOIN Il_Table F ON F.IlAdi= E.Ili
				WHERE 1=1 ");
			SqlQuery query = new SqlQuery();
			if ((ay != ProjeConstants.HEPSI_INT) && (yil != ProjeConstants.HEPSI_INT))
			{
				sb.Append(" AND MONTH(A.OdemeTarihi)=@Ay AND YEAR(A.OdemeTarihi)=@Yil ");
				query.AddParameter("@Ay", ay);
				query.AddParameter("@Yil", yil);
			}
			else if ((ay == ProjeConstants.HEPSI_INT) && (yil != ProjeConstants.HEPSI_INT))
			{
				sb.Append(" AND YEAR(A.OdemeTarihi)=@Yil ");
				query.AddParameter("@Yil", yil);
			}
			else if ((ay != ProjeConstants.HEPSI_INT) && (yil == ProjeConstants.HEPSI_INT))
			{
				sb.Append(" AND MONTH(A.OdemeTarihi)=@Ay ");
				query.AddParameter("@Ay", ay);
			}
			if (kiraciId > 0)
			{
				sb.Append(" AND A.KiraciId=@KiraciId ");
				query.AddParameter("@KiraciId", kiraciId);
			}
			sb.Append(" ORDER BY A.OdemeTarihi DESC, B.Id, A.SozlesmeId ");
			query.Sql = sb.ToString();
            // bölge de seçime eklendigi için sorgu üstteki ile degisti SB 23/09/2019
            //string sqlString = string.Format(@"
            //    SELECT A.Id, B.Adi,B.Soyadi,  B.Adi+' '+B.Soyadi KiraciAdiSoyadi,
            //        A.Id OdemeId, A.OdemeTarihi, A.OdenenTutar, A.Aciklama, A.SozlesmeId, A.KiraciId, A.OdemePlaniId 
            //    FROM Odeme_Table A
            //    INNER JOIN Kiraci_Table B ON B.Id=A.KiraciId 
            //    {0} {1}
            //    ORDER BY A.OdemeTarihi DESC, B.Id, A.SozlesmeId
            //", ayYilStr,kiraciIdStr);
			DataTable dataTable = dao.SelectFromDb(query, "");

			return dataTable;

		}
		public DataTable SelectByKiraciAyYilReturnDataTable(int bolgeId,int kiraciId,DateTime bastar, DateTime bittar)//int ay, int yil)
		{
			bittar = UtilityHelper.TariheSaatEkle(bittar, "23:59:59");
			StringBuilder sb = new StringBuilder(@"
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
				WHERE 1=1 ");
			SqlQuery query = new SqlQuery();
			sb.Append(" AND A.OdemeTarihi BETWEEN @BasTarih AND @BitTarih ");
			query.AddParameter("@BasTarih", bastar);
			query.AddParameter("@BitTarih", bittar);
			if (kiraciId > 0)
			{
				sb.Append(" AND A.KiraciId=@KiraciId ");
				query.AddParameter("@KiraciId", kiraciId);
			}
			if (bolgeId != ProjeConstants.HEPSI_INT && bolgeId != ProjeConstants.BOLGE_GENELMUDURLUK_INT)
			{
				sb.Append(" AND C.BolgeId=@BolgeId ");
				query.AddParameter("@BolgeId", bolgeId);
			}
			sb.Append(" ORDER BY A.OdemeTarihi DESC, B.Id, A.SozlesmeId ");
			query.Sql = sb.ToString();
            // bölge de seçime eklendigi için sorgu üstteki ile degisti SB 23/09/2019
            DataTable dataTable = dao.SelectFromDb(query, "");

            return dataTable;

        }
		public DataTable SelectByAyYilReturnDataTable( int ay, int yil)
		{
			StringBuilder sb = new StringBuilder(@"
				SELECT 
					A.OdemeTarihi, A.OdenenTutar, A.Aciklama, 
					B.Adi, B.Soyadi 
					--D.Id TeminatId, D.IslemTipi
				FROM Odeme_Table A
					INNER JOIN Kiraci_Table B ON B.Id=A.KiraciId 
					INNER JOIN KiraSozlesme_Table C ON C.Id=A.SozlesmeId	                
					LEFT JOIN TeminatIslem_Table E ON E.OdemeId=A.Id
				WHERE 1=1 ");
			SqlQuery query = new SqlQuery();
			if ((ay != ProjeConstants.HEPSI_INT) && (yil != ProjeConstants.HEPSI_INT))
			{
				sb.Append(" AND MONTH(A.OdemeTarihi)=@Ay AND YEAR(A.OdemeTarihi)=@Yil ");
				query.AddParameter("@Ay", ay);
				query.AddParameter("@Yil", yil);
			}
			else if ((ay == ProjeConstants.HEPSI_INT) && (yil != ProjeConstants.HEPSI_INT))
			{
				sb.Append(" AND YEAR(A.OdemeTarihi)=@Yil ");
				query.AddParameter("@Yil", yil);
			}
			else if ((ay != ProjeConstants.HEPSI_INT) && (yil == ProjeConstants.HEPSI_INT))
			{
				sb.Append(" AND MONTH(A.OdemeTarihi)=@Ay ");
				query.AddParameter("@Ay", ay);
			}
			sb.Append(" ORDER BY A.OdemeTarihi DESC, B.Id, A.SozlesmeId ");
			query.Sql = sb.ToString();

			DataTable dataTable = dao.SelectFromDb(query, "");

            return dataTable;

        }
		public List<Odeme> SelectByKiraciVadeBasTarVadeBitTar(int sozlesmeId,int kiraciId, DateTime ilkTarih, DateTime ikinciTarih)
		{
			DateTime tarih1= new DateTime(ilkTarih.Year,ilkTarih.Month,ilkTarih.Day);
			DateTime tarih2= new DateTime(ikinciTarih.Year, ikinciTarih.Month, ikinciTarih.Day);
			DateTime tarihbas = UtilityHelper.TariheSaatEkle(tarih1, "00:00"); 
			DateTime tarihbit = UtilityHelper.TariheSaatEkle(tarih2, "23:59"); 
			SqlQuery query = new SqlQuery(@"
				SELECT * 
				FROM Odeme_Table A
				WHERE KiraciId=@KiraciId AND SozlesmeId=@SozlesmeId
					AND OdemeTarihi BETWEEN @BasTarih AND @BitTarih
				ORDER BY A.OdemeTarihi ");
			query.AddParameter("@KiraciId", kiraciId);
			query.AddParameter("@SozlesmeId", sozlesmeId);
			query.AddParameter("@BasTarih", tarihbas);
			query.AddParameter("@BitTarih", tarihbit);

			DataTable dataTable = dao.SelectFromDb(query, "");
            List<Odeme> list = ToList<Odeme>(dataTable);
            return list;
        }
        public decimal SelectSumBySozlesmeIdOdemePlaniId(int sozlesmeId, int odemePlaniId)
        {
            decimal toplam = 0;
            SqlQuery query = new SqlQuery(@"
                SELECT SUM(OdenenTutar) Toplam FROM Odeme_Table
                WHERE SozlesmeId=@SozlesmeId 
                    AND OdemePlaniId=@OdemePlaniId");
            query.AddParameter("@SozlesmeId", sozlesmeId);
            query.AddParameter("@OdemePlaniId", odemePlaniId);
            DataTable dataTable = dao.SelectFromDb(query, "");
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
                    // toplami bul
                    Odeme odemeDao = new Odeme();


                    //toplam Odenenin bulunmasi Aylik ve Gunluk olarak ayrilmali
                    //Aylik ise 
                    decimal toplamOdenen = odemeDao.SelectSumBySozlesmeIdOdemePlaniId(kiraSozlesme.Id, odemePlani.Id);
                    //Gunluk ise
                    // OdemeAyrintiya kayit atsin TODO SB
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
                        ExceptionHelper ex = new ExceptionHelper(new Exception("Ödeme Tablosunda OdemePlaniId=0 oldugundan kayit yapilamadi"));
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
                    //onceki Odeme planini güncelle
                    if (oncekiOdemePlani != null)
                    {
                        Odeme odemeDao = new Odeme();
                        decimal toplamOdenen = odemeDao.SelectSumBySozlesmeIdOdemePlaniId(oncekiOdemePlani.SozlesmeId, oncekiOdemePlani.Id);
                        oncekiOdemePlani.OdenenTutar = toplamOdenen;
                        oncekiOdemePlani.Degistiren = kullanici;
                        oncekiOdemePlani.Update();
                    }
                    //yeni Odeme planini güncelle
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
