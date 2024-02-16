using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.MTS
{
    public class AramaGorusme : ParentClass
    {
        public int ArayanId { get; set; }
        public int KatilimciTipi { get; set; }
        public int RandevuId { get; set; }
        public DateTime Tarih { get; set; }
        public string GorusmeSekli { get; set; }
        public string Konu { get; set; }
        public string Aciklama { get; set; }
        public bool GorusmeSaglandi { get; set; }
        public bool RandevuIstendi { get; set; }
        public override int Save()
        {
            try
            {
                GenericEntity<AramaGorusme> genericEntity = new GenericEntity<AramaGorusme>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);
                if (id > 0 && ProjeConstants.MTS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.MTS, ProjeConstants.MTS_ARAMAGORUSME);
                }
                this.Id = id;
                return id;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public override bool Update()
        {
            bool isSuccess = false;
            try
            {
                if (this != null)
                {
                    AramaGorusme item = Select<AramaGorusme>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<AramaGorusme> genericEntity = new GenericEntity<AramaGorusme>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        string sqlString = genericEntity.GetQuery(this);
                        isSuccess = dao.Update2Db(sqlString);
                    }
                    if (isSuccess && ProjeConstants.MTS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.MTS, ProjeConstants.MTS_ARAMAGORUSME);
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
                    GenericEntity<AramaGorusme> genericEntity = new GenericEntity<AramaGorusme>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);
                    AramaGorusme item = Select<AramaGorusme>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.MTS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.MTS, ProjeConstants.MTS_ARAMAGORUSME);
                    }
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public AramaGorusme Select(int id)
        {
            GenericEntity<AramaGorusme> genericEntity = new GenericEntity<AramaGorusme>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<AramaGorusme> list = ToList<AramaGorusme>(dataTable);
            AramaGorusme item = new AramaGorusme();
            item = list.FirstOrDefault();
            return item;
        }
        public AramaGorusme SelectByRandevuId(int randevuId)
        {

            string sqlString = string.Format(@"
                SELECT * FROM AramaGorusme_Table
                WHERE RandevuId={0}
            ", randevuId);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<AramaGorusme> list = ToList<AramaGorusme>(dataTable);
            AramaGorusme item = new AramaGorusme();
            item = list.FirstOrDefault();
            return item;
        }
        public override T Select<T>(int id)
        {
            GenericEntity<AramaGorusme> genericEntity = new GenericEntity<AramaGorusme>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<AramaGorusme> list = ToList<AramaGorusme>(dataTable);
            AramaGorusme item = new AramaGorusme();
            item = list.FirstOrDefault();
            return ((T)Convert.ChangeType(item, typeof(T)));
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM AramaGorusme_Table ORDER BY Adi
                ");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<AramaGorusme> list = ToList<AramaGorusme>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public List<AramaGorusme> SelectAllByArayanIdReturnList(int arayanId, int katilimciTipi)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM AramaGorusme_Table
                WHERE ArayanId={0} AND
                      KatilimciTipi={1}
            ", arayanId, katilimciTipi);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<AramaGorusme> list = ToList<AramaGorusme>(dataTable);

            return list;
        }
        public DataTable SelectAllReturnDT(int arayanId, int katilimciTipi, string gorusmeSekli, DateTime basTar, DateTime bitTar)
        {
            if (bitTar < ProjeConstants.REFERANS_TARIHI)
            {
                bitTar = DateTime.Today;
            }
            gorusmeSekli = string.IsNullOrEmpty(gorusmeSekli) || gorusmeSekli.Equals(ProjeConstants.HEPSI) ? "" : gorusmeSekli;
            string arayanIdstr = string.Format(arayanId > 0 ? " WHERE ArayanId={0} AND KatilimciTipi= {1}" : "", arayanId, katilimciTipi);
            string gorusmeSekliStr = string.IsNullOrEmpty(gorusmeSekli) ? "" :
                (string.Format(string.IsNullOrEmpty(arayanIdstr) ? " WHERE GorusmeSekli={0}" : " AND GorusmeSekli={0} ", gorusmeSekli.ReturnQuotedValue()));
            string basTarStr = basTar < ProjeConstants.REFERANS_TARIHI ? "" :
                (string.Format(string.IsNullOrEmpty(gorusmeSekliStr) && string.IsNullOrEmpty(arayanIdstr) ?
                " WHERE Tarih BETWEEN {0} AND {1} " : " AND Tarih BETWEEN {0} AND {1}  ", basTar.ReturnTRDateFormat(), bitTar.ReturnTRDateFormat()));
            string sqlString = string.Format(@"
                SELECT A.Id AramaId, C.Id ArayanId, A.GorusmeSekli, A.Tarih, A.Konu, A.GorusmeSaglandi, A.RandevuIstendi, A.RandevuId,
                    CASE
	                    WHEN A.KatilimciTipi=1 THEN D.Adi
                        WHEN A.KatilimciTipi=2 THEN C.Adi
	                    WHEN A.KatilimciTipi=3 THEN E.Adi
                        WHEN A.KatilimciTipi=4 THEN F.Adi
                    ELSE C.Adi
                    END AS Adi,
                    CASE
	                    WHEN A.KatilimciTipi=1 THEN D.Soyadi
                        WHEN A.KatilimciTipi=2 THEN C.Soyadi
	                    WHEN A.KatilimciTipi=3 THEN E.Soyadi
                        WHEN A.KatilimciTipi=4 THEN F.Soyadi
                    ELSE C.Soyadi
                    END AS Soyadi,
	                CASE
		                WHEN A.KatilimciTipi=1 THEN 'TSKGV'
                        WHEN A.KatilimciTipi=2 THEN H.Adi
		                WHEN A.KatilimciTipi=3 THEN 'Nakit Bağışçı'
		                WHEN A.KatilimciTipi=4 THEN 'Taşınmaz Bağışçı'
                    ELSE C.Kurumu
	                END AS Kurumu,
                    CASE
		                WHEN A.KatilimciTipi=1 THEN '0' 
                        WHEN A.KatilimciTipi=2 THEN C.RandevuKisiti
		                WHEN A.KatilimciTipi=3 THEN '0' 
		                WHEN A.KatilimciTipi=4 THEN '0' 
                    ELSE C.RandevuKisiti
	                END AS RandevuKisiti
                FROM AramaGorusme_Table A
                    LEFT JOIN Kisi_Table C ON C.Id = A.ArayanId
	                LEFT JOIN Personel_Table D ON D.Id = A.ArayanId
                    LEFT JOIN NakitBagisci_Table E ON E.Id = A.ArayanId
                    LEFT JOIN TasinmazBagisci_Table F ON F.Id = A.ArayanId
					LEFT JOIN MTSKurumGorev_Table G ON G.KisiId = C.Id AND G.Durum={3}
					LEFT JOIN MTSKurumTanim_Table H ON H.Id = G.MTSKurumTanimId
                {0}
                {1}
                {2}
                ORDER BY Tarih DESC
                ", arayanIdstr, gorusmeSekliStr, basTarStr,ProjeConstants.MTSGOREVDURUMU_GOREVDE.ReturnQuotedValue());

            DataTable dataTable = dao.SelectFromDb(sqlString, "");

            return dataTable;
        }
    }
}
