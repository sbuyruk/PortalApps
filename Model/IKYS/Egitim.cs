
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
    public class Egitim : ParentClass
    {
        public int PersonelId { get; set; }
        public string Seviye { get; set; }
        public string Okul { get; set; }
        public DateTime MezuniyetTar { get; set; }
        public string Aciklama { get; set; }


        public override T Select<T>(int id)
        {
            string sqlString = SelectSQL(id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Egitim> list = ToList<Egitim>(dataTable);
            Egitim egitim = new Egitim();
            egitim = list.FirstOrDefault();
            return (T)Convert.ChangeType(egitim, typeof(T));
        }

        public override int Save()
        {
            try
            {

                GenericEntity<Egitim> genericEntity = new GenericEntity<Egitim>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                SqlQuery query = genericEntity.GetQueryParametreli(this);
                int id = dao.Insert(query);
                if (id > 0 && ProjeConstants.IKYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.IKYS, ProjeConstants.IKYS_EGITIM);
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
                Egitim item = Select<Egitim>(Id);
                if (Id != 0)
                {
                    GenericEntity<Egitim> genericEntity = new GenericEntity<Egitim>(ProjeConstants.SQL_UPDATE);
                    DegistirmeTarihi = DateTime.Now;
                    Degistiren = UtilityHelper.GetCurrentUserName();
                    SqlQuery query = genericEntity.GetQueryParametreli(this);
                    isSuccess = dao.Update2Db(query);
                }
                if (isSuccess && ProjeConstants.IKYS_UPDATE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.IKYS, ProjeConstants.IKYS_EGITIM);
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
                    GenericEntity<Egitim> genericEntity = new GenericEntity<Egitim>(ProjeConstants.SQL_DELETE);
                    SqlQuery query = genericEntity.GetQueryParametreli(this);

                    Egitim item = Select<Egitim>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(query, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.IKYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_EGITIM);
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
                               FROM Egitim_Table");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Egitim> list = ToList<Egitim>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        private string saveSQL()
        {
            //Insert  SQL
            string InsertSQL = string.Format(@" 
                                    INSERT INTO Egitim_Table 
                                        (PersonelId, Seviye, Okul, Aciklama,MezuniyetTar,Olusturan,OlusturmaTarihi)
                                    VALUES ({0},{1},{2},{3},{4},{5},{6}) ",
                                    PersonelId.ReturnQuotedValue(), Seviye.ReturnQuotedValue(), Okul.ReturnQuotedValue(), Aciklama.ReturnQuotedValue(),
                                    MezuniyetTar.ReturnTRDateFormat(), Olusturan.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat());
            return InsertSQL;
        }
        private string UpdateSQL()
        {
            //Insert  SQL
            string sqlSQL = string.Format(@"
                                    UPDATE Egitim_Table 
                                    SET PersonelId = {0},Seviye={1}, Okul={2}, Aciklama={3}, MezuniyetTar={4},
                                        Degistiren={5},DegistirmeTarihi={6}
                                        WHERE Id= {7}", PersonelId.ReturnQuotedValue(), Seviye.ReturnQuotedValue(), Okul.ReturnQuotedValue(),
                                        Aciklama.ReturnQuotedValue(), MezuniyetTar.ReturnTRDateFormat(), Degistiren.ReturnQuotedValue(),
                                        DateTime.Now.ReturnTRDateFormat(), Id);
            return sqlSQL;
        }

        private string SelectSQL(int id)
        {
            string sqlstr = string.Format(@"SELECT *
                               FROM Egitim_Table 
                               WHERE  Id={0}", id);
            return sqlstr;
        }
        private string DeleteSQL()
        {
            string sqlString = string.Format(@"
                            DELETE 
                            FROM Egitim_Table
                            WHERE Id={0}", Id);
            return sqlString;
        }
        public List<Egitim> SelectByPersonelId(int personelId)
        {
            string sqlString = SelectByPersonelIdSQL(personelId);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Egitim> list = ToList<Egitim>(dataTable);

            return (list);
        }
        private string SelectByPersonelIdSQL(int pId)
        {
            string sqlstr = string.Format(@" 
                    SELECT * FROM Egitim_Table  
                    WHERE PersonelId={0}
                    ORDER BY MezuniyetTar DESC", pId);
            return sqlstr;
        }

    }
}
