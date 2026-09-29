using Model.Ortak;
using Model.Services.NBYS;
using System;
using System.Collections.Generic;
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
            return new NakitBagisHareketService().Save(this);
        }
        public override bool Update()
        {
            return new NakitBagisHareketService().Update(this);
        }
        public override bool Delete()
        {
            return new NakitBagisHareketService().Delete(this);
        }
        public override T Select<T>(int id)
        {
            return (T)Convert.ChangeType(new NakitBagisHareketService().GetById(id), typeof(T));
        }
        public override List<T> SelectAll<T>()
        {
            return (List<T>)Convert.ChangeType(
                new NakitBagisHareketService().GetAll(), typeof(List<T>));
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
