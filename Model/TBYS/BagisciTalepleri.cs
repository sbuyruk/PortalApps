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
            string sqlString = string.Format(@"SELECT *
                               FROM BagisciTalepleri_Table 
                               WHERE  Id={0}", id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
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
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);

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
                        string sqlString = genericEntity.GetQuery(this);
                        isSuccess = dao.Update2Db(sqlString);
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
                    string sqlString = genericEntity.GetQuery(this);
                    BagisciTalepleri item = Select<BagisciTalepleri>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
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
            string sqlString = string.Format(@"SELECT *
                               FROM BagisciTalepleri_Table");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<BagisciTalepleri> list = ToList<BagisciTalepleri>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public List<BagisciTalepleri> SelectByBagisciId(int bagisciId)
        {

            string sqlString = string.Format(@"SELECT * FROM BagisciTalepleri_Table
                              WHERE BagisciId={0}", bagisciId.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<BagisciTalepleri> list = ToList<BagisciTalepleri>(dataTable);
            return list;
        }


    }
}
