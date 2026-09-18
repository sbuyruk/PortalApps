using DAO.Ortak;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.NBYS
{
    [Serializable]
    public class DuzenliNakitBagisci : ParentClass
    {

        public int BagisciId { get; set; }
        public long TCKimlikNo { get; set; }
        public string BagisciAdi { get; set; }
        public int BagisAdedi { get; set; }
        public decimal BagisToplami { get; set; }
        public DateTime BaslamaTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public decimal Tutar { get; set; }
        public bool Aktif { get; set; }
        public int ArmaganId { get; set; }
        public int NakitBagisHareketId { get; set; }
        public string Telefon { get; set; }
        public string EPosta { get; set; }
        public string EslesmeBilgisi { get; set; }
        public string Aciklama { get; set; }
        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM DuzenliNakitBagisci_Table 
                               WHERE  Id={0}", id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<DuzenliNakitBagisci> list = ToList<DuzenliNakitBagisci>(dataTable);
            DuzenliNakitBagisci DuzenliNakitBagisci = new DuzenliNakitBagisci();
            DuzenliNakitBagisci = list.FirstOrDefault();
            return (T)Convert.ChangeType(DuzenliNakitBagisci, typeof(T));

        }
        public override int Save()
        {
            try
            {
                GenericEntity<DuzenliNakitBagisci> genericEntity = new GenericEntity<DuzenliNakitBagisci>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                SqlQuery query = genericEntity.GetQueryParametreli(this);
                int id = dao.Insert(query);

                this.Id = id;
                if (id > 0 && ProjeConstants.NBYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.NBYS, ProjeConstants.NBYS_DUZENLINAKITBAGISCI);
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
                    DuzenliNakitBagisci item = Select<DuzenliNakitBagisci>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<DuzenliNakitBagisci> genericEntity = new GenericEntity<DuzenliNakitBagisci>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        string sqlString = genericEntity.GetQuery(this);
                        isSuccess = dao.Update2Db(sqlString);
                    }
                    if (isSuccess && ProjeConstants.NBYS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.NBYS, ProjeConstants.NBYS_DUZENLINAKITBAGISCI);
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
                    GenericEntity<DuzenliNakitBagisci> genericEntity = new GenericEntity<DuzenliNakitBagisci>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);
                    DuzenliNakitBagisci item = Select<DuzenliNakitBagisci>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.NBYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.NBYS, ProjeConstants.NBYS_DUZENLINAKITBAGISCI);
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
                               FROM DuzenliNakitBagisci_Table");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<DuzenliNakitBagisci> list = ToList<DuzenliNakitBagisci>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }

        public DuzenliNakitBagisci SelectByBagisciId(int bagisciId)
        {
            if (bagisciId < 1) 
            { 
                return null; 
            }
            string sqlString = string.Format(@"SELECT *
                               FROM DuzenliNakitBagisci_Table 
                               WHERE Aktif=1 AND BagisciId={0}", bagisciId);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<DuzenliNakitBagisci> list = ToList<DuzenliNakitBagisci>(dataTable);
            DuzenliNakitBagisci DuzenliNakitBagisci = new DuzenliNakitBagisci();
            DuzenliNakitBagisci = list.FirstOrDefault();
            return DuzenliNakitBagisci;

        }
       
    }
}
