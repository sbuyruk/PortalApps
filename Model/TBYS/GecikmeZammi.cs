using DAO.Ortak;
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
    public class GecikmeZammi : ParentClass
    {
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public decimal ZamOrani { get; set; }
        public string Aciklama { get; set; }
        public override T Select<T>(int id)
        {
            SqlQuery query = new SqlQuery(@"SELECT *
                               FROM GecikmeZammi_Table 
                               WHERE  Id=@Id");
            query.AddParameter("@Id", id);
            DataTable dataTable = dao.SelectFromDb(query, "");
            List<GecikmeZammi> list = ToList<GecikmeZammi>(dataTable);
            GecikmeZammi gecikmeZammi = new GecikmeZammi();
            gecikmeZammi = list.FirstOrDefault();
            return (T)Convert.ChangeType(gecikmeZammi, typeof(T));

        }
        public override int Save()
        {
            try
            {
                GenericEntity<GecikmeZammi> genericEntity = new GenericEntity<GecikmeZammi>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                SqlQuery query = genericEntity.GetQueryParametreli(this);
                int id = dao.Insert(query);

                this.Id = id;
                if (id > 0 && ProjeConstants.TBYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.TBYS, ProjeConstants.TBYS_GECIKMEZAMMI);
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
                    GecikmeZammi item = Select<GecikmeZammi>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<GecikmeZammi> genericEntity = new GenericEntity<GecikmeZammi>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        SqlQuery query = genericEntity.GetQueryParametreli(this);
                        isSuccess = dao.Update2Db(query);
                    }
                    if (isSuccess && ProjeConstants.TBYS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.TBYS, ProjeConstants.TBYS_GECIKMEZAMMI);
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
                    GenericEntity<GecikmeZammi> genericEntity = new GenericEntity<GecikmeZammi>(ProjeConstants.SQL_DELETE);
                    SqlQuery query = genericEntity.GetQueryParametreli(this);
                    GecikmeZammi item = Select<GecikmeZammi>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(query, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.TBYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_GECIKMEZAMMI);
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
            SqlQuery query = new SqlQuery(@"
                SELECT *
                FROM GecikmeZammi_Table
                ORDER BY BaslangicTarihi DESC");

            DataTable dataTable = dao.SelectFromDb(query, "");
            List<GecikmeZammi> list = ToList<GecikmeZammi>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
		public List<GecikmeZammi> SelectBuAyIcindeDegisen(DateTime ilkOdemeTarihi, DateTime sonOdemeTarihi)
		{
			SqlQuery query = new SqlQuery(@"
				SELECT *
				FROM GecikmeZammi_Table
				WHERE BaslangicTarihi <= @SonOdemeTarihi AND ISNULL(BitisTarihi,@SonOdemeTarihi) >= @IlkOdemeTarihi
				ORDER BY BaslangicTarihi ");
			query.AddParameter("@IlkOdemeTarihi", ilkOdemeTarihi);
			query.AddParameter("@SonOdemeTarihi", sonOdemeTarihi);

			DataTable dataTable = dao.SelectFromDb(query, "");
			List<GecikmeZammi> list = ToList<GecikmeZammi>(dataTable);

			return list;
		}
		public List<GecikmeZammi> SelectByBaslangicTarihi(DateTime baslangicTarihi)
		{
			SqlQuery query = new SqlQuery(@"
				SELECT *
				FROM GecikmeZammi_Table
				WHERE BaslangicTarihi <= @BaslangicTarihi
				ORDER BY BaslangicTarihi DESC ");
			query.AddParameter("@BaslangicTarihi", baslangicTarihi);

			DataTable dataTable = dao.SelectFromDb(query, "");
			List<GecikmeZammi> list = ToList<GecikmeZammi>(dataTable);

			return list;
		}
		public List<GecikmeZammi> SelectByBaslangicTarihi(DateTime vadeBaslangicTarihi, DateTime vadeBitisTarihi)
		{
			SqlQuery query = new SqlQuery(@"
				SELECT *
				FROM GecikmeZammi_Table
				WHERE BaslangicTarihi <= @VadeBitisTarihi AND (BitisTarihi is null OR  BitisTarihi>=@VadeBaslangicTarihi)
				ORDER BY BaslangicTarihi DESC ");
			query.AddParameter("@VadeBaslangicTarihi", vadeBaslangicTarihi);
			query.AddParameter("@VadeBitisTarihi", vadeBitisTarihi);

			DataTable dataTable = dao.SelectFromDb(query, "");
			List<GecikmeZammi> list = ToList<GecikmeZammi>(dataTable);

			return list;
		}
		public GecikmeZammi SelectSonDegisenByTarih(DateTime sonOdemeTar)
		{

			SqlQuery query = new SqlQuery(@"
				SELECT *
				FROM GecikmeZammi_Table
				WHERE BaslangicTarihi <= @SonOdemeTarihi
				ORDER BY BaslangicTarihi DESC");
			query.AddParameter("@SonOdemeTarihi", sonOdemeTar);

			DataTable dataTable = dao.SelectFromDb(query, "");
            if (dataTable != null)
            {
                List<GecikmeZammi> list = ToList<GecikmeZammi>(dataTable);
                GecikmeZammi gecikmeZammi = new GecikmeZammi();
                gecikmeZammi = list.FirstOrDefault();
                return gecikmeZammi;
            }
            else
            {
                return null;
            }
        }
        public GecikmeZammi SelectOncekiGecikmeZammi(DateTime tarih)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT *
                FROM GecikmeZammi_Table
                WHERE  BaslangicTarihi < @Tarih 
                ORDER BY BaslangicTarihi DESC ");
            query.AddParameter("@Tarih", tarih);
            DataTable dataTable = dao.SelectFromDb(query, "");

            if (dataTable != null)
            {
                List<GecikmeZammi> list = ToList<GecikmeZammi>(dataTable);
                GecikmeZammi gecikmeZammi = new GecikmeZammi();
                gecikmeZammi = list.FirstOrDefault();
                return gecikmeZammi;
            }
            else
            {
                return null;
            }
        }
        public GecikmeZammi SelectSonrakiGecikmeZammi(DateTime tarih)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT *
                FROM GecikmeZammi_Table
                WHERE  BaslangicTarihi > @Tarih 
                ORDER BY BaslangicTarihi ");
            query.AddParameter("@Tarih", tarih);
            DataTable dataTable = dao.SelectFromDb(query, "");

            if (dataTable != null)
            {
                List<GecikmeZammi> list = ToList<GecikmeZammi>(dataTable);
                GecikmeZammi gecikmeZammi = new GecikmeZammi();
                gecikmeZammi = list.FirstOrDefault();
                return gecikmeZammi;
            }
            else
            {
                return null;
            }
        }
        public List<GecikmeZammi> SelectByTarih(DateTime ilkOdemeTarihi, DateTime sonOdemeTarihi)
        {
            GecikmeZammi gecikmeZammiDao = new GecikmeZammi();
            //bu ay içinde oran degisti mi?
            List<GecikmeZammi> buayDegisenGecikmeZammiList = gecikmeZammiDao.SelectBuAyIcindeDegisen(ilkOdemeTarihi, sonOdemeTarihi);

            bool oncekiGecikmeZammiDaEklensinMi = false;
            if (buayDegisenGecikmeZammiList.Count < 1)
            {
                oncekiGecikmeZammiDaEklensinMi = true;
            }
            else
            {
                oncekiGecikmeZammiDaEklensinMi = false;// gecikmeZammiList[0].BaslangicTarihi > ilkOdemeTarihi ? true : false;
            }
            List<GecikmeZammi> gecikmeZammiList = new List<GecikmeZammi>();
            if (oncekiGecikmeZammiDaEklensinMi)
            {
                GecikmeZammi gecikmeZammi = new GecikmeZammi();
                gecikmeZammi = gecikmeZammi.SelectSonDegisenByTarih(sonOdemeTarihi);
                if (gecikmeZammi != null)
                    gecikmeZammiList.Add(gecikmeZammi);
            }

            gecikmeZammiList.AddRange(buayDegisenGecikmeZammiList);
            return gecikmeZammiList;
        }

        public GecikmeZammi SelectSonrakiGecikmeZammi()
        {
            SqlQuery query = new SqlQuery(@"
                SELECT *
                FROM GecikmeZammi_Table
                ORDER BY BaslangicTarihi DESC");

            DataTable dataTable = dao.SelectFromDb(query, "");
            if (dataTable != null)
            {
                List<GecikmeZammi> list = ToList<GecikmeZammi>(dataTable);
                GecikmeZammi gecikmeZammi = new GecikmeZammi();
                gecikmeZammi = list.FirstOrDefault();
                return gecikmeZammi;
            }
            else
            {
                return null;
            }
        }
    }
}