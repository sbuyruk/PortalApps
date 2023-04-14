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
    public class OdemeSebebiTanim : ParentClass
    {
        public string OdemeSebebi { get; set; }
        public string Aciklama { get; set; }
        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM OdemeSebebiTanim_Table 
                               WHERE  Id={0}", id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<OdemeSebebiTanim> list = ToList<OdemeSebebiTanim>(dataTable);
            OdemeSebebiTanim odeme = new OdemeSebebiTanim();
            odeme = list.FirstOrDefault();
            return (T)Convert.ChangeType(odeme, typeof(T));

        }
        public OdemeSebebiTanim Select(int id)
        {
            GenericEntity<OdemeSebebiTanim> genericEntity = new GenericEntity<OdemeSebebiTanim>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<OdemeSebebiTanim> list = ToList<OdemeSebebiTanim>(dataTable);
            OdemeSebebiTanim odeme = new OdemeSebebiTanim();
            odeme = list.FirstOrDefault();
            return odeme;
        }
        public override int Save()
        {
            try
            {
                GenericEntity<OdemeSebebiTanim> genericEntity = new GenericEntity<OdemeSebebiTanim>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);

                this.Id = id;
                if (id > 0 && ProjeConstants.TBYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEMESEBEBITANIM);
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
                    OdemeSebebiTanim item = Select<OdemeSebebiTanim>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<OdemeSebebiTanim> genericEntity = new GenericEntity<OdemeSebebiTanim>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        string sqlString = genericEntity.GetQuery(this);
                        isSuccess = dao.Update2Db(sqlString);
                    }
                    if (isSuccess && ProjeConstants.TBYS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEMESEBEBITANIM);
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
                    GenericEntity<OdemeSebebiTanim> genericEntity = new GenericEntity<OdemeSebebiTanim>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);
                    OdemeSebebiTanim item = Select<OdemeSebebiTanim>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.TBYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEMESEBEBITANIM);
                    }
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM OdemeSebebiTanim_Table");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<OdemeSebebiTanim> list = ToList<OdemeSebebiTanim>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
    }
}
