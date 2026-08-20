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
    public class BagisciBagis : ParentClass
    {
        public int BagisciId { get; set; }
        public string ArmaganId { get; set; }
        public string ArmaganDurumu { get; set; }
        public DateTime ArmaganTarihi { get; set; }
        public string ArmaganAciklama { get; set; }

        public override T Select<T>(int id)
        {
            SqlQuery query = new SqlQuery(@"SELECT *
                               FROM BagisciBagis_Table 
                               WHERE  Id=@Id");
            query.AddParameter("@Id", id);
            DataTable dataTable = dao.SelectFromDb(query, "");
            List<BagisciBagis> list = ToList<BagisciBagis>(dataTable);
            BagisciBagis bagis = new BagisciBagis();
            bagis = list.FirstOrDefault();
            return (T)Convert.ChangeType(bagis, typeof(T));

        }
        public BagisciBagis Select(int id)
        {
            GenericEntity<BagisciBagis> genericEntity = new GenericEntity<BagisciBagis>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            SqlQuery query = genericEntity.GetQueryParametreli(this);

            DataTable dataTable = dao.SelectFromDb(query, "");
            List<BagisciBagis> list = ToList<BagisciBagis>(dataTable);
            BagisciBagis bagis = new BagisciBagis();
            bagis = list.FirstOrDefault();
            return bagis;
        }
        public override int Save()
        {
            try
            {
                GenericEntity<BagisciBagis> genericEntity = new GenericEntity<BagisciBagis>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                SqlQuery query = genericEntity.GetQueryParametreli(this);
                int id = dao.Insert(query);

                this.Id = id;
                if (id > 0 && ProjeConstants.TBYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGISCIBAGIS);
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
                    BagisciBagis item = Select<BagisciBagis>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<BagisciBagis> genericEntity = new GenericEntity<BagisciBagis>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        SqlQuery query = genericEntity.GetQueryParametreli(this);
                        isSuccess = dao.Update2Db(query);
                    }
                    if (isSuccess && ProjeConstants.TBYS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGISCIBAGIS);
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
                    GenericEntity<BagisciBagis> genericEntity = new GenericEntity<BagisciBagis>(ProjeConstants.SQL_DELETE);
                    SqlQuery query = genericEntity.GetQueryParametreli(this);
                    BagisciBagis item = Select<BagisciBagis>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(query, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.TBYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGISCIBAGIS);
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
                               FROM BagisciBagis_Table");

            DataTable dataTable = dao.SelectFromDb(query, "");
            List<BagisciBagis> list = ToList<BagisciBagis>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public List<BagisciBagis> SelectByBagisId(int bagisId)
        {

            SqlQuery query = new SqlQuery(@"SELECT * FROM BagisciBagis_Table
                              WHERE BagisId=@BagisId");
            query.AddParameter("@BagisId", bagisId);
            DataTable dataTable = dao.SelectFromDb(query, "");
            List<BagisciBagis> list = ToList<BagisciBagis>(dataTable);
            return list;
        }

    }
}
