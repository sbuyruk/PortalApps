
using DAO.Ortak;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.IKYS
{
    public class IsBilgileri : ParentClass
    {
        public int PersonelId { get; set; }        
        public int UnvanId { get; set; }
        public int GorevId { get; set; }
        public int BirimId { get; set; }
        public DateTime BaslamaTar { get; set; }
        public DateTime IzinDonemiBasTar { get; set; }
        public int CalismaDurumu { get; set; }
        public DateTime AyrilmaTar { get; set; }
        public string AyrilmaSebebi { get; set; }
        public int ProtokolSiraNo { get; set; }
        public string SGKSicilNo { get; set; }
        public DateTime SGKBasTar { get; set; }
        public int VakifOncesiPrimGunSayisi { get; set; }
        public DateTime EmeklilikTarihi { get; set; }
        public string Aciklama { get; set; }

        public override T Select<T>(int id)
        {
            GenericEntity<IsBilgileri> genericEntity = new GenericEntity<IsBilgileri>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<IsBilgileri> list = ToList<IsBilgileri>(dataTable);
            IsBilgileri isBilgileri = new IsBilgileri();
            isBilgileri = list.FirstOrDefault();
            return (T)Convert.ChangeType(isBilgileri, typeof(T));
        }
        public override int Save()
        {
            try
            {

                GenericEntity<IsBilgileri> genericEntity = new GenericEntity<IsBilgileri>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                SqlQuery query = genericEntity.GetQueryParametreli(this);
                int id = dao.Insert(query);
                if (id > 0 && ProjeConstants.IKYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.IKYS, ProjeConstants.IKYS_ISBILGILERI);
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
                IsBilgileri item = Select<IsBilgileri>(Id);
                if (Id != 0)
                {
                    GenericEntity<IsBilgileri> genericEntity = new GenericEntity<IsBilgileri>(ProjeConstants.SQL_UPDATE);
                    DegistirmeTarihi = DateTime.Now;
                    Degistiren = UtilityHelper.GetCurrentUserName();
                    string sqlString = genericEntity.GetQuery(this);
                    isSuccess = dao.Update2Db(sqlString);
                }
                if (isSuccess && ProjeConstants.IKYS_UPDATE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.IKYS, ProjeConstants.IKYS_ISBILGILERI);
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
                bool isDeleted;
                if (Id != 0)
                {
                    GenericEntity<IsBilgileri> genericEntity = new GenericEntity<IsBilgileri>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);

                    IsBilgileri item = Select<IsBilgileri>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.IKYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_ISBILGILERI);
                    }
                    return isDeleted;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM IsBilgileri_Table");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<IsBilgileri> list = ToList<IsBilgileri>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public IsBilgileri SelectByPersonelId(int personelId)
        {
            string sqlString = SelectByPersonelIdSQL(personelId);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<IsBilgileri> list = ToList<IsBilgileri>(dataTable);
            IsBilgileri ib = list.FirstOrDefault();
            return (ib);
        }
        public IsBilgileri SelectByGorevId(int gorevId)
        {
            string sqlString = string.Format(@" 
                    SELECT * FROM IsBilgileri_Table  
                    WHERE GorevId={0}
                    ORDER BY UnvanId", gorevId);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<IsBilgileri> list = ToList<IsBilgileri>(dataTable);
            IsBilgileri ib = list.FirstOrDefault();
            return (ib);
        }
        private string SelectByPersonelIdSQL(int pId)
        {
            string sqlstr = string.Format(@" 
                    SELECT * FROM IsBilgileri_Table  
                    WHERE PersonelId={0}
                    ORDER BY UnvanId", pId);
            return sqlstr;
        }
        private string SelectSQL(int id)
        {
            string sqlstr = string.Format(@"SELECT *
                               FROM IsBilgileri_Table 
                               WHERE  Id={0}", id);
            return sqlstr;
        }
        public DataTable SelectAllFromIS_YERI_BILGILERI()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM IS_YERI_BILGILERI");
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            return (dataTable);
        }
    }
}
