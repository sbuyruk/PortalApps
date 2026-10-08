using DAO.Ortak;
using Model.Ortak;
using Model.Services.TBYS;
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
            return (T)Convert.ChangeType(new OdemeService().GetById(id), typeof(T));
        }
        public Odeme Select(int id)
        {
            return new OdemeService().GetById(id);
        }
        public override int Save()
        {
            return new OdemeService().Save(this);
        }
        public override bool Update()
        {
            return new OdemeService().Update(this);
        }
        public override bool Delete()
        {
            return new OdemeService().Delete(this);
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
            return new OdemeService().DeleteBySozlesmeId(sozlesmeId);
        }
        public override List<T> SelectAll<T>()
        {
            return (List<T>)Convert.ChangeType(new OdemeService().GetAll(), typeof(List<T>));
        }
        public List<Odeme> SelectByKiraciId(int kiraciId)
        {
            return new OdemeService().GetByKiraciId(kiraciId);
        }
        public List<Odeme> SelectBySozlesmeIdOdemePlaniId(int sozlesmeId, int odemePlaniId)
        {
            return new OdemeService().GetBySozlesmeIdOdemePlaniId(sozlesmeId, odemePlaniId);
        }
		public DataTable SelectByKiraciAyYil(int kiraciId, int ay, int yil)
		{
			return new OdemeService().GetByKiraciAyYil(kiraciId, ay, yil);
		}
		public DataTable SelectByKiraciAyYilReturnDataTable(int bolgeId, int kiraciId, DateTime bastar, DateTime bittar)
		{
			return new OdemeService().GetByKiraciAyYilReturnDataTable(bolgeId, kiraciId, bastar, bittar);
		}
		public DataTable SelectByAyYilReturnDataTable(int ay, int yil)
		{
			return new OdemeService().GetByAyYilReturnDataTable(ay, yil);
		}
		public List<Odeme> SelectByKiraciVadeBasTarVadeBitTar(int sozlesmeId, int kiraciId, DateTime ilkTarih, DateTime ikinciTarih)
		{
			return new OdemeService().GetByKiraciVadeBasTarVadeBitTar(sozlesmeId, kiraciId, ilkTarih, ikinciTarih);
		}
		public decimal SelectSumBySozlesmeIdOdemePlaniId(int sozlesmeId, int odemePlaniId)
		{
			return new OdemeService().GetSumBySozlesmeIdOdemePlaniId(sozlesmeId, odemePlaniId);
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
