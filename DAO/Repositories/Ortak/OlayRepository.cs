using DAO.Ortak;
using System;
using System.Data;
using Utility.ProjeGlobal;

namespace DAO.Repositories.Ortak
{
    public class OlayRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public OlayRepository()
            : this(new DbClass())
        {
        }

        public OlayRepository(DbClass db)
        {
            if (db == null)
                throw new ArgumentNullException("db");

            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM Olay_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery("SELECT * FROM Olay_Table"), "");
        }

        public DataTable SelectByDateAndProgram(DateTime islemTarihi, string program)
        {
            string sql = "SELECT * FROM Olay_Table WHERE IslemTarihi>=@IslemTarihi";
            SqlQuery query;

            if (program == ProjeConstants.HEPSI || string.IsNullOrEmpty(program))
            {
                sql += " ORDER BY IslemTarihi DESC";
                query = new SqlQuery(sql);
            }
            else
            {
                sql += " AND Program=@Program ORDER BY IslemTarihi DESC";
                query = new SqlQuery(sql);
                query.AddParameter("@Program", program);
            }

            query.AddParameter("@IslemTarihi", islemTarihi);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAuditList()
        {
            const string sql = @"
                SELECT ROW_NUMBER() OVER (ORDER BY Id) AS Sirano,
                    Id, Baslik, Metin, YayinBasTar, YayinBitTar, Tekrar, Popup, Aktif,
                    FORMAT(YayinBasTar,'dd.MM.yyyy hh:mm') BaslamaTarihi,
                    FORMAT(YayinBitTar,'dd.MM.yyyy hh:mm') BitisTarihi,
                    OlayAlicilari, Aciklama, Resim,
                    Olusturan, OlusturmaTarihi, Degistiren, DegistirmeTarihi
                FROM Olay_Table
                ORDER BY Aktif DESC, YayinBasTar DESC, Popup";

            return db.SelectFromDb(new SqlQuery(sql), "");
        }

        public int Insert<T>(T entity)
        {
            return db.Insert(queryBuilder.BuildInsert(entity, "Olay_Table"));
        }

        public int Insert<T>(T entity, SqlTransactionContext transaction)
        {
            return transaction.Insert(queryBuilder.BuildInsert(entity, "Olay_Table"));
        }

        public bool Update<T>(T entity)
        {
            return db.Update2Db(queryBuilder.BuildUpdate(entity, "Olay_Table"));
        }

        public bool Update<T>(T entity, SqlTransactionContext transaction)
        {
            return transaction.Update(queryBuilder.BuildUpdate(entity, "Olay_Table"));
        }

        public bool Delete(int id)
        {
            return db.DeleteFromDb(queryBuilder.BuildDelete("Olay_Table", id), "");
        }
    }
}
