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
    public class RandevuKatilim : ParentClass
    {
        public int RandevuId { get; set; }
        public int KatilimciId { get; set; }
        public int KatilimciTipi { get; set; }
        public string Aciklama { get; set; }


        public override bool Delete()
        {
            try
            {
                if (Id != 0)
                {
                    GenericEntity<RandevuKatilim> genericEntity = new GenericEntity<RandevuKatilim>(ProjeConstants.SQL_DELETE);
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
                GenericEntity<RandevuKatilim> genericEntity = new GenericEntity<RandevuKatilim>(ProjeConstants.SQL_INSERT);
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
        public RandevuKatilim Select(int id)
        {
            GenericEntity<RandevuKatilim> genericEntity = new GenericEntity<RandevuKatilim>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<RandevuKatilim> list = ToList<RandevuKatilim>(dataTable);
            RandevuKatilim item = new RandevuKatilim();
            item = list.FirstOrDefault();
            return item;
        }
        public List<RandevuKatilim> Select(int randevuId, int katilimciId, int katilimciTipi)
        {

            string sqlString = string.Format(
                @"
                SELECT * FROM RandevuKatilim_Table
                WHERE RandevuId={0} AND KatilimciId={1} AND KatilimciTipi={2}    
                ", randevuId, katilimciId, katilimciTipi);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<RandevuKatilim> list = ToList<RandevuKatilim>(dataTable);

            return list;
        }
        public List<RandevuKatilim> SelectByKatilimciIdKatilimciTipi(int katilimciId, int katilimciTipi)
        {

            string sqlString = string.Format(
                @"
                SELECT * FROM RandevuKatilim_Table
                WHERE KatilimciId={0} AND KatilimciTipi={1}    
                ", katilimciId,katilimciTipi);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<RandevuKatilim> list = ToList<RandevuKatilim>(dataTable);

            return list;
        }
        public List<RandevuKatilim> SelectByrandevuId(int randevuId)
        {

            string sqlString = string.Format(
                @"
                SELECT * FROM RandevuKatilim_Table
                WHERE RandevuId={0}    
                ", randevuId);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<RandevuKatilim> list = ToList<RandevuKatilim>(dataTable);

            return list;
        }
        public override T Select<T>(int id)
        {
            GenericEntity<RandevuKatilim> genericEntity = new GenericEntity<RandevuKatilim>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<RandevuKatilim> list = ToList<RandevuKatilim>(dataTable);
            RandevuKatilim item = new RandevuKatilim();
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
            List<RandevuKatilim> list = ToList<RandevuKatilim>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public override bool Update()
        {
            bool isSuccess = false;
            try
            {
                if (Id != 0)
                {
                    GenericEntity<RandevuKatilim> genericEntity = new GenericEntity<RandevuKatilim>(ProjeConstants.SQL_UPDATE);
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
        public bool DeleteByRandevuId(int randevuid)
        {
            string sqlString = string.Format(@"
                DELETE RandevuKatilim_Table 
                WHERE RandevuId={0}", randevuid);
            bool isSuccess = dao.DeleteFromDb(sqlString, this);
            return isSuccess;

        }

    }
}