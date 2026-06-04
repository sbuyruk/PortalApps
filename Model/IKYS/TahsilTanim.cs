
using Model.Ortak;
using Model.Portal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.IKYS
{
    public class TahsilTanim : ParentClass
    {
        public string TahsilDurumu { get; set; }

        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM TahsilTanim_Table 
                               WHERE  Id={0}", id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<TahsilTanim> list = ToList<TahsilTanim>(dataTable);
            TahsilTanim tahsilTanim = new TahsilTanim();
            tahsilTanim = list.FirstOrDefault();
            return (T)Convert.ChangeType(tahsilTanim, typeof(T));
        }

        public TahsilTanim Select(int id)
        {
            GenericEntity<TahsilTanim> genericEntity = new GenericEntity<TahsilTanim>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<TahsilTanim> list = ToList<TahsilTanim>(dataTable);
            TahsilTanim tahsilTanim = new TahsilTanim();
            tahsilTanim = list.FirstOrDefault();
            return tahsilTanim;
        }
        public override int Save()
        {
            try
            {

                GenericEntity<TahsilTanim> genericEntity = new GenericEntity<TahsilTanim>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);
                if (id > 0 && ProjeConstants.IKYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.IKYS, ProjeConstants.IKYS_TAHSILTANIM);
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
                TahsilTanim item = Select<TahsilTanim>(Id);
                if (Id != 0)
                {
                    GenericEntity<TahsilTanim> genericEntity = new GenericEntity<TahsilTanim>(ProjeConstants.SQL_UPDATE);
                    DegistirmeTarihi = DateTime.Now;
                    Degistiren = UtilityHelper.GetCurrentUserName();
                    string sqlString = genericEntity.GetQuery(this);
                    isSuccess = dao.Update2Db(sqlString);
                }
                if (isSuccess && ProjeConstants.IKYS_UPDATE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.IKYS, ProjeConstants.IKYS_TAHSILTANIM);
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
                    GenericEntity<TahsilTanim> genericEntity = new GenericEntity<TahsilTanim>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);

                    TahsilTanim item = Select<TahsilTanim>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.IKYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_TAHSILTANIM);
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
            string sqlString = string.Format(@"SELECT *
                               FROM TahsilTanim_Table");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<TahsilTanim> list = ToList<TahsilTanim>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }

       
    }
}
