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

        public override int Save()
        {
            try
            {
                GenericEntity<RandevuKatilim> genericEntity = new GenericEntity<RandevuKatilim>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);
                if (id > 0 && ProjeConstants.MTS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.MTS, ProjeConstants.MTS_RANDEVUKATILIM);
                }
                this.Id = id;
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
                    RandevuKatilim item = Select<RandevuKatilim>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<RandevuKatilim> genericEntity = new GenericEntity<RandevuKatilim>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        string sqlString = genericEntity.GetQuery(this);
                        isSuccess = dao.Update2Db(sqlString);
                    }
                    if (isSuccess && ProjeConstants.MTS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.MTS, ProjeConstants.MTS_RANDEVUKATILIM);
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
                    GenericEntity<RandevuKatilim> genericEntity = new GenericEntity<RandevuKatilim>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);
                    RandevuKatilim item = Select<RandevuKatilim>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.MTS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.MTS, ProjeConstants.MTS_RANDEVUKATILIM);
                    }
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public RandevuKatilim Select(int id)
        {
            GenericEntity<RandevuKatilim> genericEntity = new GenericEntity<RandevuKatilim>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
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

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
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

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
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

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<RandevuKatilim> list = ToList<RandevuKatilim>(dataTable);

            return list;
        }
        public override T Select<T>(int id)
        {
            GenericEntity<RandevuKatilim> genericEntity = new GenericEntity<RandevuKatilim>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
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

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<RandevuKatilim> list = ToList<RandevuKatilim>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public int DeleteByRandevuId(int randevuid)
        {
            
            bool deleteLog = ProjeConstants.MTS_DELETE_LOG;
            int deleted;
            string randevuIdStr = randevuid < 1? string.Empty : string.Format(" WHERE RandevuId={0} ", randevuid);
            string sqlString = string.Format(@"
                DELETE FROM RandevuKatilim_Table
            ", randevuIdStr);

            if (deleteLog)
            {
                string wherestr = string.Format(" WHERE RandevuId={0}", randevuid);
                GenericEntity<RandevuKatilim> genericEntitySelect = new GenericEntity<RandevuKatilim>(ProjeConstants.SQL_SELECT);
                string sqlStringSelect = genericEntitySelect.GetQuery(this, wherestr);
                DataTable dataTable = dao.SelectFromDb(sqlStringSelect, "");
                List<RandevuKatilim> list = ToList<RandevuKatilim>(dataTable);
                if (list.Count > 0)
                    deleted = dao.DeleteFromDb(sqlString, "", true);
                else deleted = 0;
                if (deleted > 0)
                {
                    foreach (RandevuKatilim item in list)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.MTS, ProjeConstants.MTS_RANDEVUKATILIM);
                    }
                }
            }
            else
            {
                deleted = dao.DeleteFromDb(sqlString, "", true);
            }
            return deleted;
        }
    }
}