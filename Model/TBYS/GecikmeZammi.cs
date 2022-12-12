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
            string sqlString = string.Format(@"SELECT *
                               FROM GecikmeZammi_Table 
                               WHERE  Id={0}", id);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
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
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);

                this.Id = id;
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
                if (Id != 0)
                {
                    GenericEntity<GecikmeZammi> genericEntity = new GenericEntity<GecikmeZammi>(ProjeConstants.SQL_UPDATE);
                    DegistirmeTarihi = DateTime.Now;
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
                               FROM GecikmeZammi_Table
                               WHERE Id={0}", Id);

            bool isSuccess = dao.DeleteFromDb(sqlString, this);

            return isSuccess;
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM GecikmeZammi_Table
                ORDER BY BaslangicTarihi DESC");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<GecikmeZammi> list = ToList<GecikmeZammi>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public List<GecikmeZammi> SelectBuAyIcindeDegisen(DateTime ilkOdemeTarihi, DateTime sonOdemeTarihi)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM GecikmeZammi_Table
				WHERE BaslangicTarihi <= {0} AND ISNULL(BitisTarihi,{0}) >= {1}
                ORDER BY BaslangicTarihi ", sonOdemeTarihi.ReturnTRDateFormat(), ilkOdemeTarihi.ReturnTRDateFormat());

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<GecikmeZammi> list = ToList<GecikmeZammi>(dataTable);

            return list;
        }
        public List<GecikmeZammi> SelectByBaslangicTarihi(DateTime baslangicTarihi)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM GecikmeZammi_Table
				WHERE BaslangicTarihi <= {0}
                ORDER BY BaslangicTarihi DESC ", baslangicTarihi.ReturnTRDateFormat());

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<GecikmeZammi> list = ToList<GecikmeZammi>(dataTable);

            return list;
        }
        public List<GecikmeZammi> SelectByBaslangicTarihi(DateTime vadeBaslangicTarihi, DateTime vadeBitisTarihi)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM GecikmeZammi_Table
				WHERE BaslangicTarihi <= {1} AND (BitisTarihi is null OR  BitisTarihi>={0}) AND (BitisTarihi is null OR BitisTarihi>={0})
                ORDER BY BaslangicTarihi DESC ", vadeBaslangicTarihi.ReturnTRDateFormat(), vadeBitisTarihi.ReturnTRDateFormat());

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<GecikmeZammi> list = ToList<GecikmeZammi>(dataTable);

            return list;
        }
        public GecikmeZammi SelectSonDegisenByTarih(DateTime sonOdemeTar)
        {

            string sqlString = string.Format(@"
                SELECT *
                FROM GecikmeZammi_Table
				WHERE BaslangicTarihi <= {0}
                ORDER BY BaslangicTarihi DESC
                ", sonOdemeTar.ReturnTRDateFormat());

            DataTable dataTable = dao.selectFromDb(sqlString, "");
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
            string sqlString = string.Format(@"
                SELECT *
                FROM GecikmeZammi_Table
                WHERE  BaslangicTarihi < {0} 
                ORDER BY BaslangicTarihi DESC ", tarih.ReturnTRDateFormat());
            DataTable dataTable = dao.selectFromDb(sqlString, "");

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
            string sqlString = string.Format(@"
                SELECT *
                FROM GecikmeZammi_Table
                WHERE  BaslangicTarihi > {0} 
                ORDER BY BaslangicTarihi ", tarih.ReturnTRDateFormat());
            DataTable dataTable = dao.selectFromDb(sqlString, "");

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
            //bu ay içinde oran değişti mi?
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
            string sqlString = string.Format(@"
                SELECT *
                FROM GecikmeZammi_Table
                ORDER BY BaslangicTarihi DESC
                ");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
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