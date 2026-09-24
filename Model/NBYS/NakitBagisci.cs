using DAO.Ortak;
using DAO.Repositories.NBYS;
using Model.Ortak;
using Model.Services.NBYS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.NBYS
{
    [Serializable]
    public class NakitBagisci : ParentClass
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
        public override T Select<T>(int id)
        {
            NakitBagisciService service = new NakitBagisciService();
            NakitBagisci nakitBagisci = service.GetById(id);
            return (T)Convert.ChangeType(nakitBagisci, typeof(T));

        }
        public override int Save()
        {
            try
            {
                NakitBagisciService service = new NakitBagisciService();
                return service.Save(this);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public override bool Update()
        {
            try
            {
                NakitBagisciService service = new NakitBagisciService();
                return service.Update(this);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public override bool Delete()
        {
            try
            {
                NakitBagisciService service = new NakitBagisciService();
                return service.Delete(this);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public override List<T> SelectAll<T>()
        {
            NakitBagisciRepository repository = new NakitBagisciRepository();
            DataTable dataTable = repository.SelectAll();
            List<NakitBagisci> list = ToList<NakitBagisci>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }

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
        public string GetDeleteSQL(string extId)
        {
            try
            {
                GenericEntity<NakitBagisci> genericEntity = new GenericEntity<NakitBagisci>(ProjeConstants.SQL_DELETE);
                string sqlString = genericEntity.GetQuery(this, extId);

                return sqlString;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        /// <summary>
        /// NakitBagisHareket_Table'da Bagisi olmayan Bagisçiyi bulur
        /// </summary>
        /// <param name="bagisciId"></param>
        /// <returns></returns>
        public NakitBagisci SelectBagisiOlmayanBagisciById(int bagisciId)
        {
            NakitBagisciRepository repository = new NakitBagisciRepository();
            DataTable dataTable = repository.SelectBagisiOlmayanBagisciById(bagisciId);
            List<NakitBagisci> list = ToList<NakitBagisci>(dataTable);
            NakitBagisci nakitBagisci = new NakitBagisci();
            nakitBagisci = list.FirstOrDefault();
            return nakitBagisci;

        }
        public DataTable SelectSecilmemisKatilimcilarByFaaliyetIdReturnDT()
        {
            NakitBagisciReportRepository repository = new NakitBagisciReportRepository();
            return repository.SelectSecilmemisKatilimcilar(
                DateTime.Today.AddYears(-2),
                ProjeConstants.NAKITBAGISCI_SORGUBAGISTUTARI);
        }
        public DataTable SelectDuzenliBagisci(DateTime bastar,DateTime bittar, string durum)
        {
            bool sadeceBelgeOlusturulmadi = durum.Equals(ProjeConstants.DURUM_BELGEOLUSTURULMADI);
            bool durumFiltrele = !sadeceBelgeOlusturulmadi && !durum.Equals(ProjeConstants.HEPSI);
            NakitBagisciReportRepository repository = new NakitBagisciReportRepository();
            return repository.SelectDuzenliBagisci(
                bastar,
                bittar,
                sadeceBelgeOlusturulmadi,
                durumFiltrele,
                durum);
        }
    }
}
