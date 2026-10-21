using DAO.Ortak;
using Utility.Logging;
using System;
using System.Data;

namespace DAO.Repositories.TBYS
{
    public class OdemeAyristirmaRepository
    {
        private readonly TbysSqlExecutor db;
        private readonly CrudQueryBuilder queryBuilder;

        public OdemeAyristirmaRepository()
            : this(new DbClass())
        {
        }

        public OdemeAyristirmaRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = new TbysSqlExecutor(db, "OdemeAyristirmaRepository");
            queryBuilder = new CrudQueryBuilder();
        }

        public OdemeAyristirmaRepository(PortalLogContext context) { db = new TbysSqlExecutor(context, "OdemeAyristirmaRepository"); queryBuilder = new CrudQueryBuilder(); }

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

        public DataTable SelectById(int id, SqlTransactionContext transaction, PortalLogContext context) { SqlQuery q = new SqlQuery("SELECT * FROM OdemeAyristirma_Table WITH (UPDLOCK, HOLDLOCK) WHERE Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, transaction, context); }
        public int Insert<T>(T entity, SqlTransactionContext transaction, PortalLogContext context) { return db.Insert(queryBuilder.BuildInsert(entity, "OdemeAyristirma_Table"), transaction, context); }
        public bool Update<T>(T entity, SqlTransactionContext transaction, PortalLogContext context) { return db.Update2Db(queryBuilder.BuildUpdate(entity, "OdemeAyristirma_Table"), transaction, context); }
        public bool Delete(int id, SqlTransactionContext transaction, PortalLogContext context) { return db.DeleteFromDb(queryBuilder.BuildDelete("OdemeAyristirma_Table", id), transaction, context); }
    }
}
