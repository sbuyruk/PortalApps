using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.ProjeGlobal;

namespace Model.MTS
{
    public class ToplantiKatilim : ParentClass
    {
        public int ToplantiId { get; set; }
        public int KatilimciId { get; set; }
        public bool Bilgi { get; set; }
        public string Aciklama { get; set; }


        public override bool Delete()
        {
            try
            {
                if (Id != 0)
                {
                    GenericEntity<ToplantiKatilim> genericEntity = new GenericEntity<ToplantiKatilim>(ProjeConstants.SQL_DELETE);
                    OlusturmaTarihi = DateTime.Now;
                    string sqlString = genericEntity.GetQuery(this);
                    bool isDeleted = dao.DeleteFromDb(sqlString, "");
                    return isDeleted;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public override int Save()
        {
            try
            {
                GenericEntity<ToplantiKatilim> genericEntity = new GenericEntity<ToplantiKatilim>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);

                this.Id = id;
                return id;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public ToplantiKatilim Select(int id)
        {
            GenericEntity<ToplantiKatilim> genericEntity = new GenericEntity<ToplantiKatilim>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
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

            DataTable dataTable = dao.selectFromDb(sqlString, "");
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

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<ToplantiKatilim> list = ToList<ToplantiKatilim>(dataTable);

            return list;
        }
        public override T Select<T>(int id)
        {
            GenericEntity<ToplantiKatilim> genericEntity = new GenericEntity<ToplantiKatilim>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
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

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<ToplantiKatilim> list = ToList<ToplantiKatilim>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public override bool Update()
        {
            bool isSuccess = false;
            try
            {
                if (Id != 0)
                {
                    GenericEntity<ToplantiKatilim> genericEntity = new GenericEntity<ToplantiKatilim>(ProjeConstants.SQL_UPDATE);
                    DegistirmeTarihi = DateTime.Now;
                    string sqlString = genericEntity.GetQuery(this);
                    isSuccess = dao.Update2Db(sqlString);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return isSuccess;
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
