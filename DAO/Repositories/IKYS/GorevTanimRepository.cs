using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.IKYS
{
    public class GorevTanimRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public GorevTanimRepository() : this(new DbClass()) { }
        public GorevTanimRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM GorevTanim_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM GorevTanim_Table"), ""); }
        public int Insert<T>(T item) { return db.Insert(queryBuilder.BuildInsert(item, "GorevTanim_Table")); }
        public bool Update<T>(T item) { return db.Update2Db(queryBuilder.BuildUpdate(item, "GorevTanim_Table")); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("GorevTanim_Table", id), ""); }

        public DataTable SelectByPersonelId(int personelId)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM GorevTanim_Table WHERE PersonelId=@PersonelId ORDER BY Id");
            query.AddParameter("@PersonelId", personelId);
            return db.SelectFromDb(query, "");
        }
        public DataTable SelectByBirimId(int birimId)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM GorevTanim_Table WHERE BirimId=@BirimId ORDER BY Id");
            query.AddParameter("@BirimId", birimId);
            return db.SelectFromDb(query, "");
        }
        public DataTable SelectAllReturnDataTable(int? personelTipi)
        {
            string personelFilter = personelTipi.HasValue ? " AND B.Tipi=@Tipi" : "";
            string sql = @"
                SELECT A.Id GorevTanimId, A.Adi GorevAdi, A.KisaAdi GorevKisaAdi, C.Sira,
                       ISNULL(B.Adi,'') + ' ' + ISNULL(B.Soyadi,'') Personel,
                       C.Adi BirimAdi
                FROM GorevTanim_Table A
                LEFT JOIN Personel_Table B ON B.Id=A.PersonelId" + personelFilter + @"
                LEFT JOIN BirimTanim_Table C ON C.Id=A.BirimId
                WHERE A.Id>0";
            SqlQuery query = new SqlQuery();
            if (personelTipi.HasValue)
            {
                query.AddParameter("@Tipi", personelTipi.Value);
            }
            query.Sql = sql + " ORDER BY C.Sira,A.Id";
            return db.SelectFromDb(query, "");
        }
    }
}
