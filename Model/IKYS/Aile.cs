using DAO.Ortak;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.IKYS
{
    public class Aile : ParentClass
    {
        public int PersonelId { get; set; }
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public string TcKimlikNo { get; set; }
        public int YakinlikDerecesi { get; set; }
        public DateTime DogumTar { get; set; }
        public string Tahsil { get; set; }
        public string Okul { get; set; }
        public string Telefon { get; set; }
        public int Meslek { get; set; }

        public override T Select<T>(int id)
        {
            string sqlString = SelectSQL(id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Aile> list = ToList<Aile>(dataTable);
            Aile aile = new Aile();
            aile = list.FirstOrDefault();
            return (T)Convert.ChangeType(aile, typeof(T));
        }

        public override int Save()
        {
            try
            {

                GenericEntity<Aile> genericEntity = new GenericEntity<Aile>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                SqlQuery query = genericEntity.GetQueryParametreli(this);
                int id = dao.Insert(query);
                if (id > 0 && ProjeConstants.IKYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.IKYS, ProjeConstants.IKYS_AILE);
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
                if (this != null)
                {
                    Aile item = Select<Aile>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<Aile> genericEntity = new GenericEntity<Aile>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        SqlQuery query = genericEntity.GetQueryParametreli(this);
                        isSuccess = dao.Update2Db(query);
                    }
                    if (isSuccess && ProjeConstants.IKYS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.IKYS, ProjeConstants.IKYS_AILE);
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
                bool isDeleted;
                if (Id != 0)
                {
                    GenericEntity<Aile> genericEntity = new GenericEntity<Aile>(ProjeConstants.SQL_DELETE);
                    SqlQuery query = genericEntity.GetQueryParametreli(this);

                        Aile item = Select<Aile>(Id);
                        if (item != null)
                        {
                            isDeleted = dao.DeleteFromDb(query, "");
                        }
                        else isDeleted = false;
                        if (isDeleted && ProjeConstants.IKYS_DELETE_LOG)
                        {
                            OlayKayit olayKayit = new OlayKayit();
                            olayKayit.SilmeOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_AILE);
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
        public Aile Select(int id)
        {
            GenericEntity<Aile> genericEntity = new GenericEntity<Aile>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Aile> list = ToList<Aile>(dataTable);
            Aile kimlik = new Aile();
            kimlik = list.FirstOrDefault();
            return kimlik;
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM Aile_Table");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Aile> list = ToList<Aile>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }

        public List<Aile> SelectByPersonelId(int personelId)
        {
            string sqlString = SelectByPersonelIdSQL(personelId);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Aile> list = ToList<Aile>(dataTable);

            return (list);
        }
        public Aile SelectEnGencCocukByPersonelId(int personelId)
        {
            string sqlString = string.Format(@" 
                    SELECT * FROM Aile_Table  
                    WHERE PersonelId={0} AND YakinlikDerecesi={1}
                    ORDER BY DogumTar Desc", personelId, ProjeConstants.PER_YAKINLIKDERECESI_COCUK_INT);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Aile> list = ToList<Aile>(dataTable);
            Aile aile = list.FirstOrDefault();
            return aile;
        }
        private string SelectByPersonelIdSQL(int pId)
        {
            string sqlstr = string.Format(@" 
                    SELECT * FROM Aile_Table  
                    WHERE PersonelId={0}
                    ORDER BY YakinlikDerecesi", pId);
            return sqlstr;
        }
        private string SelectSQL(int id)
        {
            string sqlstr = string.Format(@"SELECT *
                               FROM Aile_Table 
                               WHERE  Id={0}", id);
            return sqlstr;
        }

    }
}
