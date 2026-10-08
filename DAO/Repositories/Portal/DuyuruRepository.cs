using DAO.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using Utility.ProjeGlobal;

namespace DAO.Repositories.Portal
{
    public class DuyuruRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public DuyuruRepository()
            : this(new DbClass())
        {
        }

        public DuyuruRepository(DbClass db)
        {
            if (db == null)
                throw new ArgumentNullException("db");

            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            SqlQuery q = new SqlQuery("SELECT * FROM Duyuru_Table WHERE Id=@Id");
            q.AddParameter("@Id", id);
            return db.SelectFromDb(q, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery("SELECT * FROM Duyuru_Table"), "");
        }

        public DataTable SelectByDate(DateTime now)
        {
            SqlQuery q = new SqlQuery("SELECT * FROM Duyuru_Table WHERE YayinBasTar<=@Now AND YayinBitTar>=@Now ORDER BY YayinBasTar,YayinBitTar DESC");
            q.AddParameter("@Now", now);
            return db.SelectFromDb(q, "");
        }

        public DataTable SelectByRepeat(DateTime now, string tekrar)
        {
            string sql = "SELECT * FROM Duyuru_Table WHERE Aktif=1 AND Tekrar=@Tekrar AND CONVERT(nvarchar,YayinBasTar,108)<=@Saat AND CONVERT(nvarchar,YayinBitTar,108)>=@Saat";

            if (tekrar == ProjeConstants.DUYURU_TEKRAR_YOK)
                sql = "SELECT * FROM Duyuru_Table WHERE Aktif=1 AND Tekrar=@Tekrar AND YayinBasTar<=@Now AND YayinBitTar>=@Now";
            else if (tekrar == ProjeConstants.DUYURU_TEKRARLA_YIL)
                sql += " AND DAY(YayinBasTar)<=@Gun AND DAY(YayinBitTar)>=@Gun AND MONTH(YayinBasTar)<=@Ay AND MONTH(YayinBitTar)>=@Ay";
            else if (tekrar == ProjeConstants.DUYURU_TEKRARLA_AY)
                sql += " AND DAY(YayinBasTar)<=@Gun AND DAY(YayinBitTar)>=@Gun";
            else if (tekrar == ProjeConstants.DUYURU_TEKRARLA_HAFTA)
                sql += " AND DATEPART(dw,YayinBasTar)=DATEPART(dw,GETDATE())";
            else
                return new DataTable();

            sql += " ORDER BY YayinBasTar,YayinBitTar DESC";

            SqlQuery q = new SqlQuery(sql);
            q.AddParameter("@Tekrar", tekrar);
            q.AddParameter("@Now", now);
            q.AddParameter("@Saat", now.ToString("HH:mm:ss"));
            q.AddParameter("@Gun", now.Day);
            q.AddParameter("@Ay", now.Month);
            return db.SelectFromDb(q, "");
        }

        public DataTable SelectAnnouncementList()
        {
            return db.SelectFromDb(new SqlQuery("SELECT ROW_NUMBER() OVER (ORDER BY Id) AS Sirano, Id,Baslik,Metin,YayinBasTar,YayinBitTar,Tekrar,Popup,Aktif,FORMAT(YayinBasTar,'dd.MM.yyyy hh:mm') BaslamaTarihi,FORMAT(YayinBitTar,'dd.MM.yyyy hh:mm') BitisTarihi,DuyuruAlicilari,Aciklama,Resim,Olusturan,OlusturmaTarihi,Degistiren,DegistirmeTarihi FROM Duyuru_Table ORDER BY Aktif DESC,YayinBasTar DESC,Popup"), "");
        }

        public int Insert<T>(T entity)
        {
            return db.Insert(queryBuilder.BuildInsert(entity, "Duyuru_Table"));
        }

        public bool Update<T>(T entity)
        {
            return db.Update2Db(queryBuilder.BuildUpdate(entity, "Duyuru_Table"));
        }

        public bool Delete(int id)
        {
            return db.DeleteFromDb(queryBuilder.BuildDelete("Duyuru_Table", id), "");
        }
    }
}

