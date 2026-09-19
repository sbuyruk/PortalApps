using DAO.Ortak;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.IKYS
{
    public class DereceKademeDegisim : ParentClass
    {

        public int PersonelId { get; set; }
        public string Degisim { get; set; }
        public DateTime DegisimTarihi{ get; set; }
        public int Derece { get; set; }
        public int Kademe { get; set; }
        public string Aciklama { get; set; }

        public override T Select<T>(int id)
        {
            GenericEntity<DereceKademeDegisim> genericEntity = new GenericEntity<DereceKademeDegisim>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<DereceKademeDegisim> list = ToList<DereceKademeDegisim>(dataTable);
            DereceKademeDegisim dereceKademeDegisim = new DereceKademeDegisim();
            dereceKademeDegisim = list.FirstOrDefault();
            return (T)Convert.ChangeType(dereceKademeDegisim, typeof(T));
        }
       
        public DereceKademeDegisim Select(int id)
        {
            GenericEntity<DereceKademeDegisim> genericEntity = new GenericEntity<DereceKademeDegisim>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<DereceKademeDegisim> list = ToList<DereceKademeDegisim>(dataTable);
            DereceKademeDegisim dereceKademeDegisim = new DereceKademeDegisim();
            dereceKademeDegisim = list.FirstOrDefault();
            return dereceKademeDegisim;
        }
        public override int Save()
        {
            try
            {

                GenericEntity<DereceKademeDegisim> genericEntity = new GenericEntity<DereceKademeDegisim>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                SqlQuery query = genericEntity.GetQueryParametreli(this);
                int id = dao.Insert(query);
                if (id > 0 && ProjeConstants.IKYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.IKYS, ProjeConstants.IKYS_GOREVONAY);
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
                DereceKademeDegisim item = Select<DereceKademeDegisim>(Id);
                if (Id != 0)
                {
                    GenericEntity<DereceKademeDegisim> genericEntity = new GenericEntity<DereceKademeDegisim>(ProjeConstants.SQL_UPDATE);
                    DegistirmeTarihi = DateTime.Now;
                    Degistiren = UtilityHelper.GetCurrentUserName();
                    SqlQuery query = genericEntity.GetQueryParametreli(this);
                    isSuccess = dao.Update2Db(query);
                }
                if (isSuccess && ProjeConstants.IKYS_UPDATE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.IKYS, ProjeConstants.IKYS_GOREVONAY);
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
                    GenericEntity<DereceKademeDegisim> genericEntity = new GenericEntity<DereceKademeDegisim>(ProjeConstants.SQL_DELETE);
                    SqlQuery query = genericEntity.GetQueryParametreli(this);

                    DereceKademeDegisim item = Select<DereceKademeDegisim>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(query, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.IKYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_GOREVONAY);
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
            string sqlString = string.Format(@"
                SELECT *
                FROM DereceKademeDegisim_Table 
                ");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<DereceKademeDegisim> list = ToList<DereceKademeDegisim>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }

        public DataTable SelectAllByPersonelIdReturnDT(int personelId)
        {
            string sqlString = string.Format(@"
            SELECT *
            FROM DereceKademeDegisim_Table
            WHERE PersonelId ={0}
            ORDER BY DegisimTarihi DESC
            ", personelId);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            return dataTable;

        }
        public DereceKademeDegisim SelectByPersonelId(int personelId)
        {
            string sqlString = string.Format(@"
            SELECT *
            FROM DereceKademeDegisim_Table
            WHERE PersonelId ={0}
            ORDER BY DegisimTarihi DESC
            ", personelId);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<DereceKademeDegisim> list = ToList<DereceKademeDegisim>(dataTable);
            DereceKademeDegisim data = list.FirstOrDefault();
            return data;

        }
    }
}
