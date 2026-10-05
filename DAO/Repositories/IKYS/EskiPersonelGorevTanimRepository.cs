using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.IKYS
{
    public class EskiPersonelGorevTanimRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public EskiPersonelGorevTanimRepository() : this(new DbClass()) { }
        public EskiPersonelGorevTanimRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM EskiPersonelGorevTanim_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM EskiPersonelGorevTanim_Table"), ""); }
        public int Insert<T>(T item) { return db.Insert(queryBuilder.BuildInsert(item, "EskiPersonelGorevTanim_Table")); }
        public bool Update<T>(T item) { return db.Update2Db(queryBuilder.BuildUpdate(item, "EskiPersonelGorevTanim_Table")); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("EskiPersonelGorevTanim_Table", id), ""); }

        public DataTable SelectByPersonelId(int personelId)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM EskiPersonelGorevTanim_Table WHERE PersonelId=@PersonelId ORDER BY Id");
            query.AddParameter("@PersonelId", personelId);
            return db.SelectFromDb(query, "");
        }
        public DataTable SelectByBirimId(int birimId)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM EskiPersonelGorevTanim_Table WHERE BirimId=@BirimId ORDER BY Id");
            query.AddParameter("@BirimId", birimId);
            return db.SelectFromDb(query, "");
        }
        public DataTable SelectAllReturnDataTable(int? personelTipi)
        {
            string personelFilter = personelTipi.HasValue ? " AND B.Tipi=@Tipi" : "";
            string sql = @"
                SELECT A.Id EskiPersonelGorevTanimId, A.Adi GorevAdi, A.KisaAdi GorevKisaAdi, C.Sira,
                       ISNULL(B.Adi,'') + ' ' + ISNULL(B.Soyadi,'') Personel,
                       C.Adi BirimAdi
                FROM EskiPersonelGorevTanim_Table A
                LEFT JOIN Personel_Table B ON B.Id=A.PersonelId" + personelFilter + @"
                LEFT JOIN BirimTanim_Table C ON C.Id=A.BirimId
                WHERE A.Id>0
                ORDER BY C.Sira,A.Id";
            SqlQuery query = new SqlQuery(sql);
            if (personelTipi.HasValue) query.AddParameter("@Tipi", personelTipi.Value);
            return db.SelectFromDb(query, "");
        }
    }
}
