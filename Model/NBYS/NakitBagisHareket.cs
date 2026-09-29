using DAO.Ortak;
using DocumentFormat.OpenXml.Bibliography;
using Model.Ortak;
using Model.Services.NBYS;
using System;
using System.Collections.Generic;
using System.Data;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.NBYS
{
    public class NakitBagisHareket : ParentClass
    {
        public DateTime BagisTarihi { get; set; }
        public int BagisciId { get; set; }
        public decimal BagisMiktari { get; set; }
        public string DovizCinsi { get; set; }
        public int BankaId { get; set; }
        public int Ili { get; set; }
        public int Ilcesi { get; set; }
        public string Adresi { get; set; }
        public string Telefon { get; set; }
        public string Aciklama { get; set; }
        public int ArmaganId { get; set; }
        public bool IadeEdildiMi { get; set; }
        public decimal IadeMiktari { get; set; }
        public DateTime IadeTarihi { get; set; }
        public string IadeSebebi { get; set; }
        public string IadeEden { get; set; }
        public decimal DovizTutari { get; set; }
        public decimal DovizKuru { get; set; }
        public DateTime KurTarihi { get; set; }
        public int EkstreAktarmaId { get; set; }
        public string BagisTipi { get; set; }
        //Methods
        public override int Save()
        {
            try
            {
                DateTime minTarih = new DateTime(1987, 9, 1);
                if (BagisTarihi == default(DateTime) || BagisTarihi < minTarih)
                {
                    throw new ArgumentException("BagisTarihi boş olamaz ve 01.09.1987 tarihinden önce olamaz.");
                }
                GenericEntity<NakitBagisHareket> genericEntity = new GenericEntity<NakitBagisHareket>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                SqlQuery query = genericEntity.GetQueryParametreli(this);
                int id = dao.Insert(query);

                this.Id = id;
                if (id > 0 && ProjeConstants.NBYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.NBYS, ProjeConstants.NBYS_NAKITBAGISHAREKET);
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
                    NakitBagisHareket item = new NakitBagisHareketService().GetById(Id);
                    if (Id != 0)
                    {
                        GenericEntity<NakitBagisHareket> genericEntity = new GenericEntity<NakitBagisHareket>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        SqlQuery query = genericEntity.GetQueryParametreli(this);
                        isSuccess = dao.Update2Db(query);
                    }
                    if (isSuccess && ProjeConstants.NBYS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.NBYS, ProjeConstants.NBYS_NAKITBAGISHAREKET);
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
                    GenericEntity<NakitBagisHareket> genericEntity = new GenericEntity<NakitBagisHareket>(ProjeConstants.SQL_DELETE);
                    SqlQuery query = genericEntity.GetQueryParametreli(this);
                    NakitBagisHareket item = new NakitBagisHareketService().GetById(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(query, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.NBYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.NBYS, ProjeConstants.NBYS_NAKITBAGISHAREKET);
                    }
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public override T Select<T>(int id)
        {
            return (T)Convert.ChangeType(new NakitBagisHareketService().GetById(id), typeof(T));
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM NakitBagisHareket_Table");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<NakitBagisHareket> list = ToList<NakitBagisHareket>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public string GetInsertSQL(string extId)
        {
            try
            {
                GenericEntity<NakitBagisHareket> genericEntity = new GenericEntity<NakitBagisHareket>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this, extId) + " ;SELECT SCOPE_IDENTITY() ";

                return sqlString;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public string GetUpdateSQL(string extId)
        {
            try
            {
                GenericEntity<NakitBagisHareket> genericEntity = new GenericEntity<NakitBagisHareket>(ProjeConstants.SQL_UPDATE);
                DegistirmeTarihi = DateTime.Now;
                Degistiren = UtilityHelper.GetCurrentUserName();
                string sqlString = string.IsNullOrEmpty(extId)?genericEntity.GetQuery(this): genericEntity.GetQuery(this, extId);

                return sqlString;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public string GetDeleteSQL(string extId)
        {
            try
            {
                GenericEntity<NakitBagisHareket> genericEntity = new GenericEntity<NakitBagisHareket>(ProjeConstants.SQL_DELETE);
                string sqlString = genericEntity.GetQuery(this, extId);

                return sqlString;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }

}
