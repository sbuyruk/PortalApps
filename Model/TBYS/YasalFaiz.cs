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
    public class YasalFaiz : ParentClass
    {
        public int Yil { get; set; }
        public int Ay { get; set; }
        public string AyAdi { get; set; }
        public decimal FaizOrani { get; set; }
        public decimal Tufe { get; set; }
        public decimal Ufe { get; set; }
        public string Aciklama { get; set; }
        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM YasalFaiz_Table 
                               WHERE  Id={0}", id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<YasalFaiz> list = ToList<YasalFaiz>(dataTable);
            YasalFaiz yasalFaiz = new YasalFaiz();
            yasalFaiz = list.FirstOrDefault();
            return (T)Convert.ChangeType(yasalFaiz, typeof(T));

        }
        public override int Save()
        {
            try
            {
                GenericEntity<YasalFaiz> genericEntity = new GenericEntity<YasalFaiz>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);

                this.Id = id;
                if (id > 0 && ProjeConstants.TBYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.TBYS, ProjeConstants.TBYS_YASALFAIZ);
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
                    YasalFaiz item = Select<YasalFaiz>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<YasalFaiz> genericEntity = new GenericEntity<YasalFaiz>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        string sqlString = genericEntity.GetQuery(this);
                        isSuccess = dao.Update2Db(sqlString);
                    }
                    if (isSuccess && ProjeConstants.TBYS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.TBYS, ProjeConstants.TBYS_YASALFAIZ);
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
                    GenericEntity<YasalFaiz> genericEntity = new GenericEntity<YasalFaiz>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);
                    YasalFaiz item = Select<YasalFaiz>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.TBYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_YASALFAIZ);
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
            string sqlString = string.Format(@"SELECT *
                               FROM YasalFaiz_Table");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<YasalFaiz> list = ToList<YasalFaiz>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public List<YasalFaiz> SelectByYil(int yil)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM YasalFaiz_Table 
                WHERE Yil={0}", yil.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<YasalFaiz> list = ToList<YasalFaiz>(dataTable);
            return list;
        }
        public YasalFaiz SelectByYilAy(int yil, int ay)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM YasalFaiz_Table 
                WHERE Yil={0} AND Ay={1}", yil.ReturnQuotedValue(), ay.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<YasalFaiz> list = ToList<YasalFaiz>(dataTable);
            YasalFaiz yasalFaiz = new YasalFaiz();
            yasalFaiz = list.FirstOrDefault();
            return yasalFaiz;
        }
        public decimal SelectSonFaizOrani()
        {
            decimal value = 0;
            string sqlString = string.Format(@"
                SELECT *
                FROM YasalFaiz_Table
                WHERE  FaizOrani IS NOT Null AND FaizOrani > 0 
                ORDER BY Yil DESC, Ay DESC");
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            if (dataTable != null)
            {
                List<YasalFaiz> list = ToList<YasalFaiz>(dataTable);
                YasalFaiz yasalFaiz = new YasalFaiz();
                yasalFaiz = list.FirstOrDefault();
                value = yasalFaiz.FaizOrani;
            }

            return value;
        }
        public decimal SelectSonTufe()
        {
            decimal value = 0;
            string sqlString = string.Format(@"
                SELECT *
                FROM YasalFaiz_Table
                WHERE  Tufe IS NOT Null AND Tufe > 0 
                ORDER BY Yil DESC, Ay DESC");
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            if (dataTable != null)
            {
                List<YasalFaiz> list = ToList<YasalFaiz>(dataTable);
                YasalFaiz yasalFaiz = new YasalFaiz();
                yasalFaiz = list.FirstOrDefault();
                value = yasalFaiz.Tufe;
            }

            return value;
        }
        public decimal SelectSonUfe()
        {
            decimal value = 0;
            string sqlString = string.Format(@"
                SELECT *
                FROM YasalFaiz_Table
                WHERE  Ufe IS NOT Null AND Ufe > 0 
                ORDER BY Yil DESC, Ay DESC");
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            if (dataTable != null)
            {
                List<YasalFaiz> list = ToList<YasalFaiz>(dataTable);
                YasalFaiz yasalFaiz = new YasalFaiz();
                yasalFaiz = list.FirstOrDefault();
                value = yasalFaiz.Ufe;
            }

            return value;
        }
    }
}