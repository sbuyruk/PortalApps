using DAO.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAO.Repositories.IKYS
{
    public class GorevOnayRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public GorevOnayRepository() : this(new DbClass()) { }

        public GorevOnayRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM GorevOnay_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery("SELECT * FROM GorevOnay_Table"), "");
        }

        public int Insert<T>(T entity)
        {
            return db.Insert(queryBuilder.BuildInsert(entity, "GorevOnay_Table"));
        }

        public bool Update<T>(T entity)
        {
            return db.Update2Db(queryBuilder.BuildUpdate(entity, "GorevOnay_Table"));
        }

        public bool Delete(int id)
        {
            return db.DeleteFromDb(queryBuilder.BuildDelete("GorevOnay_Table", id), "");
        }

        public bool UpdateAllSecildiToFalse()
        {
            return db.Update2Db("UPDATE GorevOnay_Table SET Secildi=0");
        }

        public bool UpdateAllSecildiToTrue(IList<int> ids)
        {
            return UpdateFlag(ids, "Secildi");
        }

        public bool UpdateAllOdendiToTrue(IList<int> ids)
        {
            return UpdateFlag(ids, "Odendi");
        }

        public DataTable SelectAllReturnDataTable(int personelId, DateTime since)
        {
            string personelFilter = personelId > 0 ? " AND A.PersonelId=@PersonelId" : string.Empty;
            SqlQuery query = new SqlQuery(@"
                SELECT A.Id GorevOnayId, P.Adi+' '+P.Soyadi AdiSoyadi, A.Secildi,A.UlasimAraci,
                    A.PersonelId,A.GorevinSebebi,A.GorevinYeri,A.BaslangicTarihi,A.BitisTarihi,A.Sure,A.Avans,A.Yevmiye,A.ParaBirimi,
                    A.AracTahsisi,A.AracPlakasi,A.PerSubeImza,A.PerSubeVekil,A.OnayImza,A.OnayMakam,A.OnayMakamVekil,
                    A.UlasimAraci,A.Transfer,A.Konaklama,
                    A.AmirOnayi,A.GMImza,A.GMVekil, A.Aciklama, A.OnayRedAciklama,A.Odendi,A.OncekiId
                FROM GorevOnay_Table A
                    INNER JOIN Personel_Table P On A.PersonelId=P.Id
                WHERE A.BitisTarihi>=@Since" + personelFilter + @"
                ORDER BY A.BaslangicTarihi DESC, A.BitisTarihi DESC");
            query.AddParameter("@Since", since);
            if (personelId > 0) query.AddParameter("@PersonelId", personelId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAllByPersonelReturnDataTable(int personelId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT A.Id GorevOnayId, A.PersonelId, P.Adi+' '+P.Soyadi AdiSoyadi,
                    A.GorevinSebebi, A.BaslangicTarihi, A.BitisTarihi, A.GorevinYeri,
                    A.AmirOnayi, A.Odendi
                FROM GorevOnay_Table A
                    INNER JOIN Personel_Table P ON P.Id=A.PersonelId
                WHERE A.PersonelId=@PersonelId
                ORDER BY A.BitisTarihi DESC");
            query.AddParameter("@PersonelId", personelId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByTarihReturnDataTable(DateTime baslangic, DateTime bitis)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT B.Adi+' ' + B.Soyadi AdiSoyadi,
                    A.PersonelId, A.BaslangicTarihi, A.BitisTarihi,A.Sure,A.GorevinYeri GidilecekYer,A.GorevinSebebi,
                    C.BirimId,D.KisaAdi GorevYeri, D.Vekil
                FROM GorevOnay_Table A
                    INNER JOIN Personel_Table B ON B.Id= A.PersonelId
                    LEFT JOIN IsBilgileri_Table C ON C.PersonelId= A.PersonelId
                    INNER JOIN GorevTanim_Table D ON D.Id= C.GorevId
                WHERE BaslangicTarihi<=@BaslangicTarihi AND BitisTarihi>=@BitisTarihi
                ORDER BY ProtokolSiraNo,BitisTarihi, BaslangicTarihi");
            query.AddParameter("@BaslangicTarihi", baslangic);
            query.AddParameter("@BitisTarihi", bitis);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByPersonelTarih(int personelId, DateTime baslangic, DateTime bitis)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT *
                FROM GorevOnay_Table A
                WHERE PersonelId=@PersonelId
                    AND BaslangicTarihi<=@BaslangicTarihi AND BitisTarihi>=@BitisTarihi
                ORDER BY BitisTarihi DESC");
            query.AddParameter("@PersonelId", personelId);
            query.AddParameter("@BaslangicTarihi", baslangic);
            query.AddParameter("@BitisTarihi", bitis);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAllBySecildi(bool secildi)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM GorevOnay_Table WHERE Secildi=@Secildi");
            query.AddParameter("@Secildi", secildi);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectBekleyenAmirOnayi()
        {
            SqlQuery query = new SqlQuery(@"
                SELECT A.Id GorevOnayId, A.PersonelId, P.Adi+' '+P.Soyadi AdiSoyadi,
                    A.GorevinSebebi, A.GorevinYeri, A.BaslangicTarihi, A.BitisTarihi, A.Sure, A.Aciklama,
                    A.UlasimAraci, A.Transfer, A.Konaklama, A.AmirOnayi, A.OnayRedAciklama
                FROM GorevOnay_Table A
                    INNER JOIN Personel_Table P ON A.PersonelId=P.Id
                WHERE (A.AmirOnayi=@OnayBekliyor OR A.AmirOnayi=@Reddedildi OR A.AmirOnayi=@Onaylandi)
                ORDER BY A.BaslangicTarihi DESC, A.BitisTarihi DESC");
            query.AddParameter("@OnayBekliyor", 0);
            query.AddParameter("@Reddedildi", 2);
            query.AddParameter("@Onaylandi", 1);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectBekleyenAmirOnayiByBirimIds(IList<int> birimIds)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT A.Id GorevOnayId, A.PersonelId, P.Adi+' '+P.Soyadi AdiSoyadi,
                    A.GorevinSebebi, A.GorevinYeri, A.BaslangicTarihi, A.BitisTarihi, A.Sure, A.Aciklama,
                    A.UlasimAraci, A.Transfer, A.Konaklama, A.AmirOnayi, A.OnayRedAciklama
                FROM GorevOnay_Table A
                    INNER JOIN Personel_Table P ON A.PersonelId=P.Id
                    INNER JOIN IsBilgileri_Table I ON I.PersonelId=P.Id
                WHERE (A.AmirOnayi=@OnayBekliyor OR A.AmirOnayi=@Reddedildi OR A.AmirOnayi=@Onaylandi)
                    AND I.BirimId IN (");
            List<string> parameters = new List<string>();
            for (int i = 0; i < birimIds.Count; i++)
            {
                string parameterName = "@BirimId" + i;
                parameters.Add(parameterName);
                query.Parameters.Add(new SqlParameter(parameterName, SqlDbType.Int) { Value = birimIds[i] });
            }
            query.Sql += string.Join(",", parameters) + ") ORDER BY A.BaslangicTarihi DESC, A.BitisTarihi DESC";
            query.AddParameter("@OnayBekliyor", 0);
            query.AddParameter("@Reddedildi", 2);
            query.AddParameter("@Onaylandi", 1);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectForDateConflict(int personelId, DateTime baslangic, DateTime bitis, int gorevOnayId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT TOP 1 *
                FROM GorevOnay_Table
                WHERE PersonelId=@PersonelId AND AmirOnayi<>@Reddedildi
                    AND BitisTarihi>=@BitisTarihi AND BaslangicTarihi<=@BaslangicTarihi AND Id<>@GorevOnayId");
            query.AddParameter("@PersonelId", personelId);
            query.AddParameter("@BitisTarihi", bitis);
            query.AddParameter("@BaslangicTarihi", baslangic);
            query.AddParameter("@GorevOnayId", gorevOnayId);
            query.AddParameter("@Reddedildi", 2);
            return db.SelectFromDb(query, "");
        }

        private bool UpdateFlag(IList<int> ids, string columnName)
        {
            if (ids == null || ids.Count == 0) return false;
            SqlQuery query = new SqlQuery("UPDATE GorevOnay_Table SET " + columnName + "=1 WHERE Id IN (");
            List<string> parameters = new List<string>();
            for (int i = 0; i < ids.Count; i++)
            {
                string parameterName = "@Id" + i;
                parameters.Add(parameterName);
                query.Parameters.Add(new SqlParameter(parameterName, SqlDbType.Int) { Value = ids[i] });
            }
            query.Sql += string.Join(",", parameters) + ")";
            return db.Update2Db(query);
        }
    }
}
