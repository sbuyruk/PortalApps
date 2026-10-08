using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.TBYS
{
    public class OdemeAyristirmaRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public OdemeAyristirmaRepository()
            : this(new DbClass())
        {
        }

        public OdemeAyristirmaRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            SqlQuery q = new SqlQuery("SELECT * FROM OdemeAyristirma_Table WHERE Id=@Id");
            q.AddParameter("@Id", id);
            return db.SelectFromDb(q, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery("SELECT * FROM OdemeAyristirma_Table"), "");
        }

        public DataTable SelectByKiraEkstreAktarmaId(int kiraEkstreAktarmaId)
        {
            SqlQuery q = new SqlQuery(@"
                SELECT A.KiraEkstreAktarmaId,A.KiraciId,A.OdemeTarihi,C.OdemeSebebi,A.Tutar,B.Adi+' '+B.Soyadi AdiSoyadi
                FROM OdemeAyristirma_Table A
                INNER JOIN Kiraci_Table B ON B.Id=A.KiraciId
                INNER JOIN OdemeSebebiTanim_Table C ON C.Id=A.OdemeSebebiId
                WHERE A.KiraEkstreAktarmaId=@KiraEkstreAktarmaId");
            q.AddParameter("@KiraEkstreAktarmaId", kiraEkstreAktarmaId);
            return db.SelectFromDb(q, "");
        }

        public int Insert<T>(T entity)
        {
            return db.Insert(queryBuilder.BuildInsert(entity, "OdemeAyristirma_Table"));
        }

        public bool Update<T>(T entity)
        {
            return db.Update2Db(queryBuilder.BuildUpdate(entity, "OdemeAyristirma_Table"));
        }

        public bool Delete(int id)
        {
            return db.DeleteFromDb(queryBuilder.BuildDelete("OdemeAyristirma_Table", id), "");
        }
    }
}
