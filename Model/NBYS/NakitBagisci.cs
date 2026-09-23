using DAO.Ortak;
using DAO.Repositories.NBYS;
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
            NakitBagisciRepository repository = new NakitBagisciRepository();
            DataTable dataTable = repository.SelectById(id);
            List<NakitBagisci> list = ToList<NakitBagisci>(dataTable);
            NakitBagisci nakitBagisci = new NakitBagisci();
            nakitBagisci = list.FirstOrDefault();
            return (T)Convert.ChangeType(nakitBagisci, typeof(T));

        }
        public override int Save()
        {
            try
            {
                GenericEntity<NakitBagisci> genericEntity = new GenericEntity<NakitBagisci>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                SqlQuery query = genericEntity.GetQueryParametreli(this);
                int id = dao.Insert(query);

                this.Id = id;
                if (id > 0 && ProjeConstants.NBYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.NBYS, ProjeConstants.NBYS_NAKITBAGISCI);
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
                    NakitBagisci item = Select<NakitBagisci>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<NakitBagisci> genericEntity = new GenericEntity<NakitBagisci>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        SqlQuery query = genericEntity.GetQueryParametreli(this);
                        isSuccess = dao.Update2Db(query);
                    }
                    if (isSuccess && ProjeConstants.NBYS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.NBYS, ProjeConstants.NBYS_NAKITBAGISCI);
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
                    GenericEntity<NakitBagisci> genericEntity = new GenericEntity<NakitBagisci>(ProjeConstants.SQL_DELETE);
                    SqlQuery query = genericEntity.GetQueryParametreli(this);
                    NakitBagisci item = Select<NakitBagisci>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(query, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.NBYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.NBYS, ProjeConstants.NBYS_NAKITBAGISCI);
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
            NakitBagisciRepository repository = new NakitBagisciRepository();
            DataTable dataTable = repository.SelectAll();
            List<NakitBagisci> list = ToList<NakitBagisci>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }

        public string GetSelectSQL(string extId)
        {
            try
            {
                GenericEntity<NakitBagisci> genericEntity = new GenericEntity<NakitBagisci>(ProjeConstants.SQL_INSERT);
                string sqlString = genericEntity.GetQuery(this, extId);

                return sqlString;
            }
            catch (Exception ex)
            {

                throw;
            }
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
        public DataTable SelectBagisciGroupByBagisAdediReturnDataTable(decimal bronzMadalyaMiktari)
        {
            NakitBagisciReportRepository repository = new NakitBagisciReportRepository();
            return repository.SelectBagisciGroupByBagisAdedi(
                bronzMadalyaMiktari,
                ProjeConstants.NAKITBAGISCI_BILINMEYEN,
                ProjeConstants.COKBAGISYAPAN_BASLAMATARIHI.ReturnQuotedValue().ToString(),
                ProjeConstants.COKBAGISYAPAN_SONBAGISI_KAC_AY_ONCE_YAPTI);
        }
        public NakitBagisci SelectByTcKimlikno(long tcKimlikno)
        {
            NakitBagisciRepository repository = new NakitBagisciRepository();
            DataTable dataTable = repository.SelectByTcKimlikno(tcKimlikno);
            List<NakitBagisci> list = ToList<NakitBagisci>(dataTable);
            NakitBagisci nakitBagisci = new NakitBagisci();
            nakitBagisci = list.FirstOrDefault();
            return nakitBagisci;

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
        public NakitBagisci SelectByAdAndTelefon(string adi, string telefon)
        {
            NakitBagisciRepository repository = new NakitBagisciRepository();
            DataTable dataTable = repository.SelectByAdAndTelefon(adi, telefon);
            List<NakitBagisci> list = ToList<NakitBagisci>(dataTable);
            NakitBagisci nakitBagisci = new NakitBagisci();
            nakitBagisci = list.FirstOrDefault();
            return nakitBagisci;

        }
        public NakitBagisci SelectByTelefon(string telefon)
        {
            NakitBagisciRepository repository = new NakitBagisciRepository();
            DataTable dataTable = repository.SelectByTelefon(telefon);
            List<NakitBagisci> list = ToList<NakitBagisci>(dataTable);
            NakitBagisci nakitBagisci = new NakitBagisci();
            nakitBagisci = list.FirstOrDefault();
            return nakitBagisci;

        }
        public NakitBagisci SelectByEposta(string eposta)
        {
            NakitBagisciRepository repository = new NakitBagisciRepository();
            DataTable dataTable = repository.SelectByEposta(eposta);
            List<NakitBagisci> list = ToList<NakitBagisci>(dataTable);
            NakitBagisci nakitBagisci = new NakitBagisci();
            nakitBagisci = list.FirstOrDefault();
            return nakitBagisci;

        }
        public DataTable SelectByAd(string adi)
        {
            NakitBagisciRepository repository = new NakitBagisciRepository();
            return repository.SelectByAd(adi);

        }
        public string SelectByIl(int pIlId, ref List<NakitBagisci> list, ref int rowCount)
        {
            int? ilId = pIlId > ProjeConstants.IL_HEPSI ? (int?)pIlId : null;
            NakitBagisciRepository repository = new NakitBagisciRepository();
            DataTable dataTable = repository.SelectByIl(ilId);
            if (dataTable != null)
            {
                rowCount = dataTable.Rows.Count;
            }
            string json = ToJSON(dataTable);
            return json;
        }
        public string SelectByIlBagisTarihi(int pIlId, string bTar, string sTar, ref List<NakitBagisci> list, ref int rowCount)
        {
            int? ilId = pIlId > ProjeConstants.IL_HEPSI ? (int?)pIlId : null;
            NakitBagisciRepository repository = new NakitBagisciRepository();
            DataTable dataTable = repository.SelectByIlBagisTarihi(ilId, bTar, sTar);

            if (dataTable != null)
            {
                rowCount = dataTable.Rows.Count;
            }
            string json = ToJSON(dataTable);
            return json;
        }
        public string SelectByIlBagisTarihiYeni(int pIlId, string basTar, string sonTar, ref List<NakitBagisci> list, ref int rowCount)
        {
            int? ilId = pIlId > ProjeConstants.IL_HEPSI ? (int?)pIlId : null;
            NakitBagisciRepository repository = new NakitBagisciRepository();
            DataTable dataTable = repository.SelectByIlBagisTarihiYeni(ilId, basTar, sonTar);
            if (dataTable != null)
            {
                rowCount = dataTable.Rows.Count;
            }
            string json = ToJSON(dataTable);
            return json;
        }
        public DataTable SelectByBagisTarihiBagisSayisi(DateTime basTar, DateTime bitTar, int bagisciSayisi, ref int rowCount, bool belgeIsitemeyenlerHaric,
            bool adresiBosOlanlarHaric, bool postadanIadelerHaric, bool dergiGonderilmesinlerHaric, bool ulasilamayanlarHaric, bool sadeceYeniBagiscilar)
        {
            NakitBagisciAdresRaporKriteri kriter = new NakitBagisciAdresRaporKriteri
            {
                BaslangicTarihi = basTar,
                BitisTarihi = bitTar,
                TLGecisTarihi = ProjeConstants.TL_GECIS_TARIHI,
                BagisciSayisi = bagisciSayisi,
                BelgeIstemeyenlerHaric = belgeIsitemeyenlerHaric,
                AdresiBosOlanlarHaric = adresiBosOlanlarHaric,
                PostadanIadelerHaric = postadanIadelerHaric,
                DergiGonderilmesinlerHaric = dergiGonderilmesinlerHaric,
                UlasilamayanlarHaric = ulasilamayanlarHaric,
                SadeceYeniBagiscilar = sadeceYeniBagiscilar,
                ParaIadeDurumu = ProjeConstants.DURUM_PARAIADE,
                DahaOnceIadeDurumu = ProjeConstants.DURUM_DAHAONCEIADE
            };
            NakitBagisciReportRepository repository = new NakitBagisciReportRepository();
            DataTable dataTable = repository.SelectByBagisTarihiBagisSayisi(kriter);
            if (dataTable != null)
            {
                rowCount = dataTable.Rows.Count;
            }
            return dataTable;
        }
        public DataTable SelectByFilterReturnDataTable(string filter, int eksiId)
        {
            NakitBagisciRepository repository = new NakitBagisciRepository();
            return repository.SelectByFilter(filter, eksiId);
        }
        public string SelectByFilter(string filter, int eksiId)
        {
            DataTable dataTable = SelectByFilterReturnDataTable(filter, eksiId);
            string json = ToJSON(dataTable);
            return json;
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
