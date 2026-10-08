using DAO.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using Utility.ProjeGlobal;

namespace DAO.Repositories.MTS
{
    public class FaaliyetRepository
    {
        private const string TableName = "Faaliyet_Table";
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public FaaliyetRepository() : this(new DbClass()) { }
        public FaaliyetRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }
        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM " + TableName + " WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM " + TableName + " ORDER BY Id"), ""); }
        public int Insert<T>(T entity) { return db.Insert(queryBuilder.BuildInsert(entity, TableName)); }
        public bool Update<T>(T entity) { return db.Update2Db(queryBuilder.BuildUpdate(entity, TableName)); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete(TableName, id), ""); }

        public DataTable SelectByAcikTarih(string acikTarih)
        {
            string filter = acikTarih != ProjeConstants.HEPSI ? " WHERE AcikTarih = @AcikTarih" : "";
            SqlQuery query = new SqlQuery("SELECT * FROM Faaliyet_Table" + filter + " ORDER BY FaaliyetKonusu");
            if (!string.IsNullOrEmpty(filter)) query.AddParameter("@AcikTarih", acikTarih == ProjeConstants.FAALIYET_ACIKTARIHLI);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByDate(DateTime start, DateTime end)
        {
            SqlQuery query = new SqlQuery(@"SELECT * FROM Faaliyet_Table
                WHERE BaslangicTarihi <= @BitisTarihi AND BitisTarihi >= @BaslangicTarihi ORDER BY BaslangicTarihi");
            query.AddParameter("@BaslangicTarihi", start);
            query.AddParameter("@BitisTarihi", end);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectParticipants(int faaliyetId, int monthBefore, string acikTarihli, DateTime basTar, DateTime bitTar, string faaliyetAmaci, string gorevde, DateTime referansTarihi)
        {
            List<string> filters = new List<string> { "B.Id IS NOT NULL" };
            SqlQuery query = new SqlQuery(@"SELECT C.KatilimciTipi, A.KatilimciId, A.Id KatilimId, A.TakvimDaveti,
                B.Aciklama, B.OlusturmaTarihi, C.Adi, C.Soyadi, C.EPosta, B.Id FaaliyetId,
                B.BaslangicTarihi, B.BaslangicSaati, B.BitisTarihi, B.BitisSaati, B.FaaliyetAmaciId,
                B.FaaliyetDurumu, B.FaaliyetKonusu, B.FaaliyetTipi, B.FaaliyetYeriStr FaaliyetYeri,
                B.TumGun, B.AcikTarih, H.Adi Kurumu FROM Faaliyet_Table B
                LEFT JOIN FaaliyetKatilim_Table A ON A.FaaliyetId = B.Id
                LEFT JOIN Kisi_Table C ON C.Id = A.KatilimciId
                LEFT JOIN MTSKurumGorev_Table D ON D.KisiId = C.Id AND D.Durum = @Gorevde
                LEFT JOIN MTSKurumTanim_Table H ON H.Id = D.MTSKurumTanimId
                LEFT JOIN IletisimBilgileri_Table G ON G.PersonelId = A.KatilimciId");
            query.AddParameter("@Gorevde", gorevde);
            if (acikTarihli != ProjeConstants.HEPSI) { filters.Add("B.AcikTarih = @AcikTarih"); query.AddParameter("@AcikTarih", acikTarihli == ProjeConstants.FAALIYET_ACIKTARIHLI); }
            if (faaliyetId != ProjeConstants.HEPSI_INT) { filters.Add("B.Id = @FaaliyetId"); query.AddParameter("@FaaliyetId", faaliyetId); }
            if (monthBefore != 0) { filters.Add("B.BaslangicTarihi > DATEADD(month, @MonthBefore, CONVERT(date, GETDATE()))"); query.AddParameter("@MonthBefore", monthBefore); }
            if (basTar >= referansTarihi) { filters.Add("B.BaslangicTarihi >= @BasTar"); query.AddParameter("@BasTar", basTar); }
            if (bitTar >= referansTarihi) { filters.Add("B.BitisTarihi <= @BitTar"); query.AddParameter("@BitTar", bitTar); }
            query.Sql += " WHERE " + string.Join(" AND ", filters);
            if (!string.IsNullOrEmpty(faaliyetAmaci) && faaliyetAmaci != ProjeConstants.HEPSI) query.Sql += " AND B.FaaliyetAmaciId IN " + faaliyetAmaci;
            query.Sql += " ORDER BY B.BaslangicTarihi DESC, C.KatilimciTipi, C.Adi, C.Soyadi";
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByParticipant(int katilimciId, int faaliyetId, string gorevde)
        {
            List<string> filters = new List<string> { "A.FaaliyetId IS NOT NULL" };
            SqlQuery query = new SqlQuery(@"SELECT C.KatilimciTipi, A.KatilimciId, A.Id KatilimId, C.Adi, C.Soyadi,
                E.Adi Kurumu, B.Id FaaliyetId, B.BaslangicTarihi, B.BaslangicSaati, B.BitisTarihi, B.BitisSaati,
                B.FaaliyetAmaciId, B.FaaliyetDurumu, B.FaaliyetKonusu, B.FaaliyetTipi, B.FaaliyetYeriStr FaaliyetYeri,
                B.TumGun, B.AcikTarih FROM Faaliyet_Table B
                LEFT JOIN FaaliyetKatilim_Table A ON A.FaaliyetId = B.Id
                LEFT JOIN Kisi_Table C ON C.Id = A.KatilimciId
                LEFT JOIN MTSKurumGorev_Table D ON D.KisiId = C.Id AND D.Durum = @Gorevde
                LEFT JOIN MTSKurumTanim_Table E ON E.Id = D.MTSKurumTanimId");
            query.AddParameter("@Gorevde", gorevde);
            if (katilimciId > 0) { filters.Add("A.KatilimciId = @KatilimciId"); query.AddParameter("@KatilimciId", katilimciId); }
            if (faaliyetId > 0) { filters.Add("A.FaaliyetId = @FaaliyetId"); query.AddParameter("@FaaliyetId", faaliyetId); }
            query.Sql += " WHERE " + string.Join(" AND ", filters) + " ORDER BY B.BaslangicTarihi DESC, C.KatilimciTipi, C.Adi, C.Soyadi";
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectDistinctPlaces()
        {
            return db.SelectFromDb(new SqlQuery("SELECT DISTINCT(FaaliyetYeriStr) FaaliyetYeri FROM Faaliyet_Table"), "");
        }
    }
}
