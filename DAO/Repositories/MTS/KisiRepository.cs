using DAO.Ortak;
using System;
using System.Data;
using Utility.ProjeGlobal;

namespace DAO.Repositories.MTS
{
    public class KisiRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public KisiRepository()
            : this(new DbClass())
        {
        }

        public KisiRepository(DbClass db)
        {
            if (db == null)
                throw new ArgumentNullException("db");

            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM Kisi_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery("SELECT * FROM Kisi_Table ORDER BY Adi"), "");
        }

        public DataTable SelectAllData()
        {
            const string sql = @"
                SELECT E.Adi MTSKurumTanim, F.Adi MTSGorevTanim, F.Adi MTSUnvanTanim, A.*, B.IlAdi, C.IlceAdi
                FROM Kisi_Table A
                    LEFT JOIN Il_Table B ON A.Ili = B.Id
                    LEFT JOIN Ilce_Table C ON A.Ilcesi = C.Id
                    LEFT JOIN MTSKurumGorev_Table D ON D.KisiId = A.Id
                    LEFT JOIN MTSKurumTanim_Table E ON E.Id = D.MTSKurumTanimId
                    LEFT JOIN MTSGorevTanim_Table F ON F.Id = D.MTSGorevTanimId
                    LEFT JOIN MTSUnvanTanim_Table G ON F.Id = A.MTSUnvanTanimId
                ORDER BY A.Adi";

            return db.SelectFromDb(new SqlQuery(sql), "");
        }

        public DataTable SelectBirthdayCelebrations()
        {
            SqlQuery query = new SqlQuery("SELECT * FROM Kisi_Table WHERE Kutlama=1 AND DogumTarihi IS NOT NULL AND DogumTarihi>@MinDate");
            query.AddParameter("@MinDate", new DateTime(1900, 1, 1));
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAllJsonData()
        {
            return db.SelectFromDb(new SqlQuery("SELECT A.Id KisiId, A.Adi, A.Soyadi FROM Kisi_Table A"), "");
        }

        public DataTable SelectUnselectedParticipants(int faaliyetId)
        {
            string sql = @"
                SELECT A.Id KatilimciId, A.Adi, A.Soyadi, A.KatilimciTipi, A.RandevuKisiti, C.Adi Kurumu
                FROM Kisi_Table A
                    LEFT JOIN MTSKurumGorev_Table B ON B.KisiId = A.Id AND B.Durum=@Durum
                    LEFT JOIN MTSKurumTanim_Table C ON C.Id = B.MTSKurumTanimId";

            SqlQuery query;
            if (faaliyetId > 0)
            {
                sql += " WHERE A.Id NOT IN (SELECT KatilimciId FROM FaaliyetKatilim_Table WHERE FaaliyetId=@FaaliyetId)";
                query = new SqlQuery(sql + " ORDER BY A.Adi");
                query.AddParameter("@FaaliyetId", faaliyetId);
            }
            else
            {
                query = new SqlQuery(sql + " ORDER BY A.Adi");
            }

            query.AddParameter("@Durum", ProjeConstants.MTSGOREVDURUMU_GOREVDE);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByName(string adi, string soyadi)
        {
            string ad = string.IsNullOrEmpty(adi) ? "#${}?" : adi.Trim();
            string soyad = string.IsNullOrEmpty(soyadi) ? "#${}?" : soyadi.Trim();
            SqlQuery query = new SqlQuery("SELECT * FROM Kisi_Table WHERE Adi=@Adi AND Soyadi=@Soyadi");
            query.AddParameter("@Adi", ad);
            query.AddParameter("@Soyadi", soyad);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByTcKimlikNo(string tcKimlikNo)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM Kisi_Table WHERE TCKimlikNo=@TCKimlikNo");
            query.AddParameter("@TCKimlikNo", tcKimlikNo);
            return db.SelectFromDb(query, "");
        }

        public int Insert<T>(T entity)
        {
            return db.Insert(queryBuilder.BuildInsert(entity, "Kisi_Table"));
        }

        public bool Update<T>(T entity)
        {
            return db.Update2Db(queryBuilder.BuildUpdate(entity, "Kisi_Table"));
        }

        public bool Delete(int id)
        {
            return db.DeleteFromDb(queryBuilder.BuildDelete("Kisi_Table", id), "");
        }
    }
}
