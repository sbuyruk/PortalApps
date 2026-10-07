using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.TBYS
{
    public class TasinmazRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public TasinmazRepository() : this(new DbClass()) { }

        public TasinmazRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM Tasinmaz_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }
        public DataTable SelectAddressByBolumId(int tasinmazId, int bolumId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT A.Adres, A.Ili, A.Ilcesi, B.BolumNo
                FROM Tasinmaz_Table A
                LEFT JOIN BagimsizBolum_Table B ON B.TasinmazId=A.Id AND B.Id=@BolumId
                WHERE A.Id=@TasinmazId");
            query.AddParameter("@BolumId", bolumId);
            query.AddParameter("@TasinmazId", tasinmazId);
            return db.SelectFromDb(query, "");
        }
        public DataTable SelectByBolge(int bolgeId, int allRegion, int headquarters)
        {
            string regionFilter = bolgeId == allRegion || bolgeId == headquarters ? string.Empty : " AND E.Id=@BolgeId ";
            SqlQuery query = new SqlQuery(@"
                SELECT ROW_NUMBER() OVER(ORDER BY T.Id) AS Sirano, T.Id, T.Id TasinmazId, D.IlceAdi +'/'+C.IlAdi IliIlcesi,E.KisaAdi Bolge,
                    T.*,
                    B.Adi+' '+B.Soyadi Bagisci, B.Id BagisciId, B.Sag_vefat
                FROM Tasinmaz_Table T
                    LEFT JOIN Bagis_Table A ON A.TasinmazId=T.Id
                    LEFT JOIN TasinmazBagisci_Table B ON B.Id=A.BagisciId
                    LEFT JOIN IL_Table C ON C.Id=T.IlId
                    LEFT JOIN ILCE_Table D ON D.Id=T.IlceId
                    LEFT JOIN Bolge_Table E ON E.Id=C.BolgeId
                WHERE T.EnvanterdeMi=1" + regionFilter);
            if (!string.IsNullOrEmpty(regionFilter)) query.AddParameter("@BolgeId", bolgeId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectInventoryById(int id)
        {
            SqlQuery query = new SqlQuery(
                "SELECT * FROM Tasinmaz_Table WHERE EnvanterdeMi=1 AND Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectInventory()
        {
            return db.SelectFromDb(new SqlQuery(
                "SELECT * FROM Tasinmaz_Table WHERE EnvanterdeMi=1"), "");
        }
        public DataTable SelectInventoryByIlAdi(string ilAdi) { SqlQuery q = new SqlQuery("SELECT * FROM Tasinmaz_Table WHERE EnvanterdeMi=1 AND Ili=@IlAdi"); q.AddParameter("@IlAdi", ilAdi); return db.SelectFromDb(q, ""); }
        public DataTable SelectOutOfInventoryById(int id) { SqlQuery q = new SqlQuery("SELECT *,Convert(nvarchar,replace (EnvanterdenCikmaBedeli,'.',',')) as EnvanterdenCikmaBedeli FROM Tasinmaz_Table WHERE EnvanterdeMi=0 AND Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectInventoryValueTotal(string column, int bolgeId, int allRegion, int headquarters)
        {
            string filter = bolgeId == allRegion || bolgeId == headquarters ? string.Empty : " AND E.Id=@BolgeId";
            SqlQuery q = new SqlQuery("SELECT SUM(T." + column + ") Toplam FROM Tasinmaz_Table T LEFT JOIN Il_Table C ON C.Id=T.IlId LEFT JOIN Ilce_Table D ON D.Id=T.IlceId LEFT JOIN Bolge_Table E ON E.Id=C.BolgeId WHERE T.EnvanterdeMi=1" + filter);
            if (!string.IsNullOrEmpty(filter)) q.AddParameter("@BolgeId", bolgeId);
            return db.SelectFromDb(q, "");
        }
        public DataTable SelectFilteredValueTotal(string column, string filterColumn, string filterValue)
        {
            SqlQuery q = new SqlQuery("SELECT SUM(" + column + ") Toplam FROM Tasinmaz_Table WHERE EnvanterdeMi=1 AND " + filterColumn + "=@FilterValue");
            q.AddParameter("@FilterValue", filterValue);
            return db.SelectFromDb(q, "");
        }
        public DataTable SelectCountByIl(string countColumn, string ilAdi, string mulkiyetSekli, string value, string valueColumn)
        {
            SqlQuery q = new SqlQuery("SELECT COUNT(" + countColumn + ") Adet FROM Tasinmaz_Table WHERE EnvanterdeMi=1 AND Ili=@IlAdi AND MulkiyetSekli=@MulkiyetSekli AND " + valueColumn + "=@Value");
            q.AddParameter("@IlAdi", ilAdi);
            q.AddParameter("@MulkiyetSekli", mulkiyetSekli);
            q.AddParameter("@Value", value);
            return db.SelectFromDb(q, "");
        }
        public DataTable SelectCountByBolge(string countColumn, int bolgeId, string value, string valueColumn, int allRegion, int headquarters)
        {
            string regionFilter = bolgeId == allRegion || bolgeId == headquarters ? string.Empty : " AND D.Id=@BolgeId";
            SqlQuery q = new SqlQuery("SELECT COUNT(" + countColumn + ") Adet FROM Tasinmaz_Table A LEFT JOIN Il_Table B ON B.Id=A.IlId LEFT JOIN Bolge_Table D ON D.Id=B.BolgeId WHERE A.EnvanterdeMi=1" + regionFilter + " AND " + valueColumn + "=@Value");
            if (!string.IsNullOrEmpty(regionFilter)) q.AddParameter("@BolgeId", bolgeId);
            q.AddParameter("@Value", value);
            return db.SelectFromDb(q, "");
        }
        public DataTable SelectCountByBolgeFilters(string countColumn, string primaryColumn, string primaryValue, string kiraDurumu, string mulkiyetSekli, string kirayaUygunluk, int bolgeId, int allRegion, int headquarters)
        {
            string regionFilter = bolgeId == allRegion || bolgeId == headquarters ? string.Empty : " AND D.Id=@BolgeId";
            string optionalFilters = string.Empty;
            SqlQuery q = new SqlQuery();
            if (!string.IsNullOrEmpty(kiraDurumu)) { optionalFilters += " AND KiraDurumu=@KiraDurumu"; q.AddParameter("@KiraDurumu", kiraDurumu); }
            if (!string.IsNullOrEmpty(mulkiyetSekli)) { optionalFilters += " AND MulkiyetSekli=@MulkiyetSekli"; q.AddParameter("@MulkiyetSekli", mulkiyetSekli); }
            if (!string.IsNullOrEmpty(kirayaUygunluk)) { optionalFilters += " AND KirayaUygunluk=@KirayaUygunluk"; q.AddParameter("@KirayaUygunluk", kirayaUygunluk); }
            if (!string.IsNullOrEmpty(regionFilter)) q.AddParameter("@BolgeId", bolgeId);
            string primaryFilter = string.IsNullOrEmpty(primaryColumn) ? string.Empty : " AND " + primaryColumn + "=@PrimaryValue";
            if (!string.IsNullOrEmpty(primaryFilter)) q.AddParameter("@PrimaryValue", primaryValue);
            q.Sql = "SELECT COUNT(" + countColumn + ") Adet FROM Tasinmaz_Table A LEFT JOIN Il_Table B ON B.Id=A.IlId LEFT JOIN Bolge_Table D ON D.Id=B.BolgeId WHERE A.EnvanterdeMi=1" + regionFilter + primaryFilter + optionalFilters;
            return db.SelectFromDb(q, "");
        }
        public DataTable SelectNext(int id) { SqlQuery q = new SqlQuery("SELECT * FROM Tasinmaz_Table WHERE EnvanterdeMi=1 AND Id>@Id ORDER BY Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectPrev(int id) { SqlQuery q = new SqlQuery("SELECT * FROM Tasinmaz_Table WHERE EnvanterdeMi=1 AND Id<@Id ORDER BY Id DESC"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectExtreme(bool max) { return db.SelectFromDb(new SqlQuery("SELECT " + (max ? "MAX" : "MIN") + "(Id) Id FROM Tasinmaz_Table WHERE EnvanterdeMi=1"), ""); }

        public int Insert<T>(T item) { return db.Insert(queryBuilder.BuildInsert(item, "Tasinmaz_Table")); }
        public bool Update<T>(T item) { return db.Update2Db(queryBuilder.BuildUpdate(item, "Tasinmaz_Table")); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("Tasinmaz_Table", id), ""); }
    }
}
