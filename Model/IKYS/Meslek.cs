
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
    public class Meslek : ParentClass
    {
        public string Adi { get; set; }
        public string KisaAdi { get; set; }


        public override T Select<T>(int id)
        {
            string sqlString = SelectSQL(id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Meslek> list = ToList<Meslek>(dataTable);
            Meslek meslek = new Meslek();
            meslek = list.FirstOrDefault();
            return (T)Convert.ChangeType(meslek, typeof(T));
        }

        public override int Save()
        {
            try
            {

                GenericEntity<Meslek> genericEntity = new GenericEntity<Meslek>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                SqlQuery query = genericEntity.GetQueryParametreli(this);
                int id = dao.Insert(query);
                if (id > 0 && ProjeConstants.IKYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.IKYS, ProjeConstants.IKYS_MESLEK);
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
                Meslek item = Select<Meslek>(Id);
                if (Id != 0)
                {
                    GenericEntity<Meslek> genericEntity = new GenericEntity<Meslek>(ProjeConstants.SQL_UPDATE);
                    DegistirmeTarihi = DateTime.Now;
                    Degistiren = UtilityHelper.GetCurrentUserName();
                    string sqlString = genericEntity.GetQuery(this);
                    isSuccess = dao.Update2Db(sqlString);
                }
                if (isSuccess && ProjeConstants.IKYS_UPDATE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.IKYS, ProjeConstants.IKYS_MESLEK);
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
                    GenericEntity<Meslek> genericEntity = new GenericEntity<Meslek>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);

                    Meslek item = Select<Meslek>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.IKYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_MESLEK);
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
                               FROM Meslek_Table");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Meslek> list = ToList<Meslek>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        private string saveSQL()
        {
            //Insert  SQL
            string InsertSQL = string.Format(@" 
                                    INSERT INTO Meslek_Table 
                                        (Adi, KisaAdi,Olusturan,OlusturmaTarihi)
                                    VALUES ({0},{1}) ",
                                    Adi.ReturnQuotedValue(), KisaAdi.ReturnQuotedValue(),
                                    Olusturan.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat());
            return InsertSQL;
        }
        private string UpdateSQL()
        {
            //Insert  SQL
            string sqlSQL = string.Format(@"
                                    UPDATE Meslek_Table 
                                    SET PersonelId = {0},Adi={1}, KisaAdi={2}, Degistiren={3},DegistirmeTarihi={4}
                                    WHERE Id= {5}", Adi.ReturnQuotedValue(), KisaAdi.ReturnQuotedValue(),
                                    Degistiren.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat(), Id);
            return sqlSQL;
        }

        private string SelectSQL(int id)
        {
            string sqlstr = string.Format(@"SELECT *
                               FROM Meslek_Table 
                               WHERE  Id={0}", id);
            return sqlstr;
        }
        private string DeleteSQL()
        {
            string sqlString = string.Format(@"
                            DELETE 
                            FROM Meslek_Table
                            WHERE Id={0}", Id);
            return sqlString;
        }
        public List<Meslek> SelectByPersonelId(int personelId)
        {
            string sqlString = SelectByPersonelIdSQL(personelId);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Meslek> list = ToList<Meslek>(dataTable);

            return (list);
        }
        private string SelectByPersonelIdSQL(int pId)
        {
            string sqlstr = string.Format(@" 
                    SELECT * FROM Meslek_Table  
                    WHERE PersonelId={0}
                    ORDER BY Id", pId);
            return sqlstr;
        }
    }
}
