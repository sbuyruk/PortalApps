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
    public class BagisciYakinlari : ParentClass
    {
        public string AdSoyad { get; set; }
        public string Telefon { get; set; }
        public string YakinlikDerecesi { get; set; }
        public int BagisciId { get; set; }
        public override T Select<T>(int id)
        {
            SqlQuery query = new SqlQuery(@"SELECT *
                               FROM BagisciYakinlari_Table 
                               WHERE  Id=@Id");
            query.AddParameter("@Id", id);
            DataTable dataTable = dao.SelectFromDb(query, "");
            List<BagisciYakinlari> list = ToList<BagisciYakinlari>(dataTable);
            BagisciYakinlari bagisciYakinlari = new BagisciYakinlari();
            bagisciYakinlari = list.FirstOrDefault();
            return (T)Convert.ChangeType(bagisciYakinlari, typeof(T));

        }
        public override int Save()
        {
            try
            {
                GenericEntity<BagisciYakinlari> genericEntity = new GenericEntity<BagisciYakinlari>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                SqlQuery query = genericEntity.GetQueryParametreli(this);
                int id = dao.Insert(query);

                this.Id = id;
                if (id > 0 && ProjeConstants.TBYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGISCIYAKINLARI);
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
                    BagisciYakinlari item = Select<BagisciYakinlari>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<BagisciYakinlari> genericEntity = new GenericEntity<BagisciYakinlari>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        SqlQuery query = genericEntity.GetQueryParametreli(this);
                        isSuccess = dao.Update2Db(query);
                    }
                    if (isSuccess && ProjeConstants.TBYS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGISCIYAKINLARI);
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
                    GenericEntity<BagisciYakinlari> genericEntity = new GenericEntity<BagisciYakinlari>(ProjeConstants.SQL_DELETE);
                    SqlQuery query = genericEntity.GetQueryParametreli(this);
                    BagisciYakinlari item = Select<BagisciYakinlari>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(query, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.TBYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGISCIYAKINLARI);
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
                               FROM BagisciYakinlari_Table");

            DataTable dataTable = dao.SelectFromDb(query, "");
            List<BagisciYakinlari> list = ToList<BagisciYakinlari>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public List<BagisciYakinlari> SelectByBagisciId(int bagisciId)
        {

            SqlQuery query = new SqlQuery(@"SELECT * FROM BagisciYakinlari_Table
                              WHERE BagisciId=@BagisciId");
            query.AddParameter("@BagisciId", bagisciId);
            DataTable dataTable = dao.SelectFromDb(query, "");
            List<BagisciYakinlari> list = ToList<BagisciYakinlari>(dataTable);
            return list;
        }


    }
}
