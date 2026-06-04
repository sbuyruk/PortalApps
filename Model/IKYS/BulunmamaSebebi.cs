using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.IKYS
{
    public class BulunmamaSebebi : ParentClass
    {
        public string Adi { get; set; }
        public override T Select<T>(int id)
        {
            GenericEntity<BulunmamaSebebi> genericEntity = new GenericEntity<BulunmamaSebebi>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<BulunmamaSebebi> list = ToList<BulunmamaSebebi>(dataTable);
            BulunmamaSebebi item = new BulunmamaSebebi();
            item = list.FirstOrDefault();
            return (T)Convert.ChangeType(item, typeof(T));
        }
        public BulunmamaSebebi Select(int id)
        {
            GenericEntity<BulunmamaSebebi> genericEntity = new GenericEntity<BulunmamaSebebi>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<BulunmamaSebebi> list = ToList<BulunmamaSebebi>(dataTable);
            BulunmamaSebebi item = new BulunmamaSebebi();
            item = list.FirstOrDefault();
            return item;
        }
        public override int Save()
        {
            try
            {

                GenericEntity<BulunmamaSebebi> genericEntity = new GenericEntity<BulunmamaSebebi>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);
                if (id > 0 && ProjeConstants.IKYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.IKYS, ProjeConstants.IKYS_BULUNMAMASEBEBI);
                }
                this.Id = id;
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
                BulunmamaSebebi item = Select<BulunmamaSebebi>(Id);
                if (Id != 0)
                {
                    GenericEntity<BulunmamaSebebi> genericEntity = new GenericEntity<BulunmamaSebebi>(ProjeConstants.SQL_UPDATE);
                    DegistirmeTarihi = DateTime.Now;
                    Degistiren = UtilityHelper.GetCurrentUserName();
                    string sqlString = genericEntity.GetQuery(this);
                    isSuccess = dao.Update2Db(sqlString);
                }
                if (isSuccess && ProjeConstants.IKYS_UPDATE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.IKYS, ProjeConstants.IKYS_BULUNMAMASEBEBI);
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
                bool isDeleted;
                if (Id != 0)
                {
                    GenericEntity<BulunmamaSebebi> genericEntity = new GenericEntity<BulunmamaSebebi>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);

                    BulunmamaSebebi item = Select<BulunmamaSebebi>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.IKYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_BULUNMAMASEBEBI);
                    }
                    return isDeleted;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(
                @"SELECT *
                FROM BulunmamaSebebi_Table ");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<BulunmamaSebebi> list = ToList<BulunmamaSebebi>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
    }
}
