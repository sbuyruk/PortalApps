using DAO.Ortak;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.TBYS
{
    [Serializable]
    public class BagisciTalepleri : ParentClass
    {
        public string Talep { get; set; }
        public string Irtibat { get; set; }
        public string Tarih { get; set; }
        public string Aciklama { get; set; }
        public int BagisciId { get; set; }
        public override T Select<T>(int id)
        {
            SqlQuery query = new SqlQuery(@"SELECT *
                               FROM BagisciTalepleri_Table 
                               WHERE  Id=@Id");
            query.AddParameter("@Id", id);
            DataTable dataTable = dao.SelectFromDb(query, "");
            List<BagisciTalepleri> list = ToList<BagisciTalepleri>(dataTable);
            BagisciTalepleri bagisciTalepleri = new BagisciTalepleri();
            bagisciTalepleri = list.FirstOrDefault();
            return (T)Convert.ChangeType(bagisciTalepleri, typeof(T));

        }
        public override int Save()
        {
            try
            {
                GenericEntity<BagisciTalepleri> genericEntity = new GenericEntity<BagisciTalepleri>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                SqlQuery query = genericEntity.GetQueryParametreli(this);
                int id = dao.Insert(query);

                this.Id = id;
                if (id > 0 && ProjeConstants.TBYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGISCITALEPLERI);
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
                    BagisciTalepleri item = Select<BagisciTalepleri>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<BagisciTalepleri> genericEntity = new GenericEntity<BagisciTalepleri>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        SqlQuery query = genericEntity.GetQueryParametreli(this);
                        isSuccess = dao.Update2Db(query);
                    }
                    if (isSuccess && ProjeConstants.TBYS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGISCITALEPLERI);
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
                    GenericEntity<BagisciTalepleri> genericEntity = new GenericEntity<BagisciTalepleri>(ProjeConstants.SQL_DELETE);
                    SqlQuery query = genericEntity.GetQueryParametreli(this);
                    BagisciTalepleri item = Select<BagisciTalepleri>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(query, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.TBYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGISCITALEPLERI);
                    }
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public override List<T> SelectAll<T>()
        {
            SqlQuery query = new SqlQuery(@"SELECT *
                               FROM BagisciTalepleri_Table");

            DataTable dataTable = dao.SelectFromDb(query, "");
            List<BagisciTalepleri> list = ToList<BagisciTalepleri>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public List<BagisciTalepleri> SelectByBagisciId(int bagisciId)
        {

            SqlQuery query = new SqlQuery(@"SELECT * FROM BagisciTalepleri_Table
                              WHERE BagisciId=@BagisciId");
            query.AddParameter("@BagisciId", bagisciId);
            DataTable dataTable = dao.SelectFromDb(query, "");
            List<BagisciTalepleri> list = ToList<BagisciTalepleri>(dataTable);
            return list;
        }


    }
}
