using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.IKYS
{
    public class MahsupRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public MahsupRepository() : this(new DbClass()) { }
        public MahsupRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM Mahsup_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM Mahsup_Table"), ""); }
        public int Insert<T>(T item) { return db.Insert(queryBuilder.BuildInsert(item, "Mahsup_Table")); }
        public bool Update<T>(T item) { return db.Update2Db(queryBuilder.BuildUpdate(item, "Mahsup_Table")); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("Mahsup_Table", id), ""); }

        public DataTable SelectByDonemId(int donemId)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM Mahsup_Table WHERE KullanildigiDonemId=@DonemId OR MahsupDonemId=@DonemId");
            query.AddParameter("@DonemId", donemId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByPersonelId(int personelId, int izinTipi, int mazeretIzinTipi)
        {
            string typeFilter = "";
            SqlQuery query = new SqlQuery();
            if (izinTipi != 0)
            {
                if (izinTipi == mazeretIzinTipi)
                {
                    typeFilter = " AND IzinTipi=@IzinTipi";
                    query.AddParameter("@IzinTipi", izinTipi);
                }
                else
                {
                    typeFilter = " AND IzinTipi<>@MazeretIzinTipi";
                    query.AddParameter("@MazeretIzinTipi", mazeretIzinTipi);
                }
            }
            query.Sql = "SELECT * FROM Mahsup_Table WHERE PersonelId=@PersonelId" + typeFilter;
            query.AddParameter("@PersonelId", personelId);
            return db.SelectFromDb(query, "");
        }
    }
}
