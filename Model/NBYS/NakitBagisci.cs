using Model.Ortak;
using System;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.NBYS
{
    [Serializable]
    public class NakitBagisci : EntityBase
    {

        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public long TCKimlikNo { get; set; }
        public int Ili { get; set; }
        public int Ilcesi { get; set; }
        public string Adres { get; set; }
        public string Telefon1 { get; set; }
        public string Telefon2 { get; set; }
        public bool TuzelKisi { get; set; }
        public bool Sag { get; set; }
        public string Eposta { get; set; }
        public string PostaKodu { get; set; }
        public string Meslek { get; set; }
        public string Aciklama { get; set; }
        public bool Ulasilamiyor { get; set; }
        public bool BelgeIstemiyor { get; set; }
        public bool DergiGonderilmesin { get; set; }

        public string GetInsertSQL(string extId)
        {
            try
            {
                GenericEntity<NakitBagisci> genericEntity = new GenericEntity<NakitBagisci>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi=DateTime.Now;
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
                GenericEntity<NakitBagisci> genericEntity = new GenericEntity<NakitBagisci>(ProjeConstants.SQL_UPDATE);
                DegistirmeTarihi = DateTime.Now;
                Degistiren = UtilityHelper.GetCurrentUserName();
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
