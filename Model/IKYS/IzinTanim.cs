
using DAO.Ortak;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.IKYS
{
    public class IzinTanim : ParentClass
    {
        public string Adi { get; set; }
        public override T Select<T>(int id)
        {
            string sqlString = SelectSQL(id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<IzinTanim> list = ToList<IzinTanim>(dataTable);
            IzinTanim izinTanim = new IzinTanim();
            izinTanim = list.FirstOrDefault();
            return (T)Convert.ChangeType(izinTanim, typeof(T));
        }
        public override int Save()
        {
            try
            {

                GenericEntity<IzinTanim> genericEntity = new GenericEntity<IzinTanim>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                SqlQuery query = genericEntity.GetQueryParametreli(this);
                int id = dao.Insert(query);
                if (id > 0 && ProjeConstants.IKYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.IKYS, ProjeConstants.IKYS_IZINTANIM);
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
                IzinTanim item = Select<IzinTanim>(Id);
                if (Id != 0)
                {
                    GenericEntity<IzinTanim> genericEntity = new GenericEntity<IzinTanim>(ProjeConstants.SQL_UPDATE);
                    DegistirmeTarihi = DateTime.Now;
                    Degistiren = UtilityHelper.GetCurrentUserName();
                    string sqlString = genericEntity.GetQuery(this);
                    isSuccess = dao.Update2Db(sqlString);
                }
                if (isSuccess && ProjeConstants.IKYS_UPDATE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.IKYS, ProjeConstants.IKYS_IZINTANIM);
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
                    GenericEntity<IzinTanim> genericEntity = new GenericEntity<IzinTanim>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);

                    IzinTanim item = Select<IzinTanim>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.IKYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_IZINTANIM);
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
                               FROM IzinTanim_Table");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<IzinTanim> list = ToList<IzinTanim>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        private string saveSQL()
        {
            //Insert  SQL
            string InsertSQL = string.Format(@" 
                                    INSERT INTO IzinTanim_Table 
                                        (Adi, Olusturan,OlusturmaTarihi)
                                    VALUES ({0},{1},{3}) ",
                                    Adi.ReturnQuotedValue(),
                                    Olusturan.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat());
            return InsertSQL;
        }
        private string UpdateSQL()
        {
            //Update  SQL 
            string sqlSQL = string.Format(@"
                                    UPDATE IzinTanim_Table 
                                    SET Adi={0}, Degistiren={1},DegistirmeTarihi={2}
                                    WHERE Id= {3}", Adi.ReturnQuotedValue(),
                                    Degistiren.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat(), Id);
            return sqlSQL;
        }
        private string SelectSQL(int id)
        {
            string sqlstr = string.Format(@"SELECT *
                               FROM IzinTanim_Table 
                               WHERE  Id={0}", id);
            return sqlstr;
        }
        private string DeleteSQL()
        {
            string sqlString = string.Format(@"
                            DELETE 
                            FROM IzinTanim_Table
                            WHERE Id={0}", Id);
            return sqlString;
        }

    }
}
