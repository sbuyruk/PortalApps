using DAO.Ortak;
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
    public class FaaliyetKatilim : ParentClass
    {
        public int FaaliyetId { get; set; }
        public int KatilimciId { get; set; }
        public int KatilimciTipi { get; set; } = 2;
        public string KurumGorev { get; set; }
        public string TakvimDaveti { get; set; }
        public string Aciklama { get; set; }

        public override int Save()
        {
            try
            {
                GenericEntity<FaaliyetKatilim> genericEntity = new GenericEntity<FaaliyetKatilim>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                SqlQuery query = genericEntity.GetQueryParametreli(this);
                int id = dao.Insert(query);
                if (id > 0 && ProjeConstants.MTS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.MTS, ProjeConstants.MTS_FAALIYETKATILIM);
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
                    FaaliyetKatilim item = Select<FaaliyetKatilim>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<FaaliyetKatilim> genericEntity = new GenericEntity<FaaliyetKatilim>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        string sqlString = genericEntity.GetQuery(this);
                        isSuccess = dao.Update2Db(sqlString);
                    }
                    if (isSuccess && ProjeConstants.MTS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.MTS, ProjeConstants.MTS_FAALIYETKATILIM);
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
                    GenericEntity<FaaliyetKatilim> genericEntity = new GenericEntity<FaaliyetKatilim>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);
                    FaaliyetKatilim item = Select<FaaliyetKatilim>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.MTS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.MTS, ProjeConstants.MTS_FAALIYETKATILIM);
                    }
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public FaaliyetKatilim Select(int id)
        {
            GenericEntity<FaaliyetKatilim> genericEntity = new GenericEntity<FaaliyetKatilim>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<FaaliyetKatilim> list = ToList<FaaliyetKatilim>(dataTable);
            FaaliyetKatilim item = new FaaliyetKatilim();
            item = list.FirstOrDefault();
            return item;
        }
        public List<FaaliyetKatilim> Select(int faaliyetId, int katilimciId)
        {

            string sqlString = string.Format(
                @"
                SELECT * FROM FaaliyetKatilim_Table
                WHERE FaaliyetId={0} AND KatilimciId={1}     
                ", faaliyetId, katilimciId);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<FaaliyetKatilim> list = ToList<FaaliyetKatilim>(dataTable);

            return list;
        }
        public List<FaaliyetKatilim> SelectByKatilimciId(int katilimciId)
        {

            string sqlString = string.Format(
                @"
                SELECT * FROM FaaliyetKatilim_Table
                WHERE KatilimciId={0}     
                ", katilimciId);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<FaaliyetKatilim> list = ToList<FaaliyetKatilim>(dataTable);

            return list;
        }
        public List<FaaliyetKatilim> SelectByFaaliyetId(int faaliyetId)
        {

            string sqlString = string.Format(
                @"
                SELECT * FROM FaaliyetKatilim_Table
                WHERE FaaliyetId={0}    
                ", faaliyetId);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<FaaliyetKatilim> list = ToList<FaaliyetKatilim>(dataTable);

            return list;
        }
        public override T Select<T>(int id)
        {
            GenericEntity<FaaliyetKatilim> genericEntity = new GenericEntity<FaaliyetKatilim>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<FaaliyetKatilim> list = ToList<FaaliyetKatilim>(dataTable);
            FaaliyetKatilim item = new FaaliyetKatilim();
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
            List<FaaliyetKatilim> list = ToList<FaaliyetKatilim>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public int DeleteByFaaliyetId(int faaliyetid)
        {
            
            bool deleteLog = ProjeConstants.MTS_DELETE_LOG;
            int deleted;
            string faaliyetIdStr = faaliyetid < 1? string.Empty : string.Format(" WHERE FaaliyetId={0} ", faaliyetid);
            string sqlString = string.Format(@"
                DELETE FROM FaaliyetKatilim_Table
            ", faaliyetIdStr);

            if (deleteLog)
            {
                string wherestr = string.Format(" WHERE FaaliyetId={0}", faaliyetid);
                GenericEntity<FaaliyetKatilim> genericEntitySelect = new GenericEntity<FaaliyetKatilim>(ProjeConstants.SQL_SELECT);
                string sqlStringSelect = genericEntitySelect.GetQuery(this, wherestr);
                DataTable dataTable = dao.SelectFromDb(sqlStringSelect, "");
                List<FaaliyetKatilim> list = ToList<FaaliyetKatilim>(dataTable);
                if (list.Count > 0)
                    deleted = dao.DeleteFromDb(sqlString, "", true);
                else deleted = 0;
                if (deleted > 0)
                {
                    foreach (FaaliyetKatilim item in list)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.MTS, ProjeConstants.MTS_FAALIYETKATILIM);
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