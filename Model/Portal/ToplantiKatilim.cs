using DAO.Ortak;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.ProjeGlobal;

namespace Model.Portal
{
    public class ToplantiKatilim : ParentClass
    {
        public int ToplantiId { get; set; }
        public int KatilimciId { get; set; }
        public bool Bilgi { get; set; }
        public string Aciklama { get; set; }

        public override int Save()
        {
            try
            {
                GenericEntity<ToplantiKatilim> genericEntity = new GenericEntity<ToplantiKatilim>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                SqlQuery query = genericEntity.GetQueryParametreli(this);
                int id = dao.Insert(query);

                this.Id = id;
                if (id > 0 && ProjeConstants.PORTAL_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.PORTAL, ProjeConstants.PORTAL_TOPLANTIKATILIM);
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
                    ToplantiKatilim item = Select<ToplantiKatilim>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<ToplantiKatilim> genericEntity = new GenericEntity<ToplantiKatilim>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        string sqlString = genericEntity.GetQuery(this);
                        isSuccess = dao.Update2Db(sqlString);
                    }
                    if (isSuccess && ProjeConstants.PORTAL_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.PORTAL, ProjeConstants.PORTAL_TOPLANTIKATILIM);
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
                    GenericEntity<ToplantiKatilim> genericEntity = new GenericEntity<ToplantiKatilim>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);
                    ToplantiKatilim item = Select<ToplantiKatilim>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.PORTAL_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.PORTAL, ProjeConstants.PORTAL_TOPLANTIKATILIM);
                    }
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public ToplantiKatilim Select(int id)
        {
            GenericEntity<ToplantiKatilim> genericEntity = new GenericEntity<ToplantiKatilim>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<ToplantiKatilim> list = ToList<ToplantiKatilim>(dataTable);
            ToplantiKatilim item = new ToplantiKatilim();
            item = list.FirstOrDefault();
            return item;
        }

        public ToplantiKatilim Select(int katilimciId, int toplantiId)
        {

            string sqlString = string.Format(
                @"
                SELECT * FROM ToplantiKatilim_Table
                WHERE ToplantiId={0} AND KatilimciId={1}   
                ", toplantiId, katilimciId);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<ToplantiKatilim> list = ToList<ToplantiKatilim>(dataTable);
            ToplantiKatilim item = new ToplantiKatilim();
            item = list.FirstOrDefault();
            return item;
        }
      
        public List<ToplantiKatilim> SelectBytoplantiId(int toplantiId)
        {

            string sqlString = string.Format(
                @"
                SELECT * FROM ToplantiKatilim_Table
                WHERE ToplantiId={0}    
                ", toplantiId);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<ToplantiKatilim> list = ToList<ToplantiKatilim>(dataTable);

            return list;
        }
        public override T Select<T>(int id)
        {
            GenericEntity<ToplantiKatilim> genericEntity = new GenericEntity<ToplantiKatilim>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<ToplantiKatilim> list = ToList<ToplantiKatilim>(dataTable);
            ToplantiKatilim item = new ToplantiKatilim();
            item = list.FirstOrDefault();
            return ((T)Convert.ChangeType(item, typeof(T)));
        }
       public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM Kisi_Table ORDER BY Adi
                ");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<ToplantiKatilim> list = ToList<ToplantiKatilim>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public bool DeleteByToplantiId(int toplantiid)
        {
            string sqlString = string.Format(@"
                DELETE ToplantiKatilim_Table 
                WHERE ToplantiId={0}", toplantiid);
            bool isSuccess = dao.DeleteFromDb(sqlString, this);
            return isSuccess;

        }

    }
}
