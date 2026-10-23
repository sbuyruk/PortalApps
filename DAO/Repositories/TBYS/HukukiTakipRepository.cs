using DAO.Ortak;
using Utility.Logging;
using System;
using System.Data;

namespace DAO.Repositories.TBYS
{
    public class HukukiTakipRepository
    {
        private readonly TbysSqlExecutor db;
        private readonly CrudQueryBuilder queryBuilder;

        public HukukiTakipRepository()
        {
            db = new TbysSqlExecutor("HukukiTakipRepository");
            queryBuilder = new CrudQueryBuilder();
        }

        public HukukiTakipRepository(PortalLogContext context) { db = new TbysSqlExecutor(context, "HukukiTakipRepository"); queryBuilder = new CrudQueryBuilder(); }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM HukukiTakip_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery(@"
                SELECT * FROM HukukiTakip_Table
                WHERE Aktif=1"), "");
        }

        public DataTable SelectBySozlesmeId(int sozlesmeId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT * FROM HukukiTakip_Table
                WHERE SozlesmeId=@SozlesmeId
                ORDER BY IslemTarihi");
            query.AddParameter("@SozlesmeId", sozlesmeId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectBySozlesmeId(int sozlesmeId, SqlTransactionContext transaction, PortalLogContext context)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT * FROM HukukiTakip_Table WITH (UPDLOCK, HOLDLOCK)
                WHERE SozlesmeId=@SozlesmeId
                ORDER BY IslemTarihi");
            query.AddParameter("@SozlesmeId", sozlesmeId);
            return db.SelectFromDb(query, transaction, context);
        }

        public DataTable SelectActiveList()
        {
            return db.SelectFromDb(new SqlQuery(@"
                SELECT A.Id, A.SozlesmeId,B.DosyaNo,A.KiraciId,A.Aciklama,
                    FORMAT(A.BorcAnaPara,'###.00') BorcAnaPara,
                    FORMAT(A.BorcFaiz,'###.00') BorcFaiz,
                    FORMAT(A.IslemTarihi,'dd/MM/yyyy') IslemTarihi,
                    FORMAT(B.IlkSozlesmeTar,'dd/MM/yyyy') IlkSozlesmeTar,
                    FORMAT(B.SozBasTar,'dd/MM/yyyy') SozBasTar,
                    FORMAT(B.SozBitTar,'dd/MM/yyyy') SozBitTar,
                    C.Adi,C.Soyadi, C.Adi+' '+C.Soyadi KiraciAdiSoyadi
                FROM HukukiTakip_Table A
                INNER JOIN KiraSozlesme_Table B ON B.Id=A.SozlesmeId
                INNER JOIN Kiraci_Table C ON C.Id=A.KiraciId
                WHERE A.Aktif=1"), "");
        }

        public DataTable SelectActiveListForDataTable()
        {
            return db.SelectFromDb(new SqlQuery(@"
                SELECT A.Id, A.SozlesmeId,A.KiraciId,A.BorcAnaPara,A.BorcFaiz,A.Aciklama,
                    FORMAT(A.IslemTarihi,'dd/MM/yyyy') IslemTarihi,
                    FORMAT(B.IlkSozlesmeTar,'dd/MM/yyyy') IlkSozlesmeTar,
                    FORMAT(B.SozBasTar,'dd/MM/yyyy') SozBasTar,
                    FORMAT(B.SozBitTar,'dd/MM/yyyy') SozBitTar,
                    C.Adi,C.Soyadi, C.Adi+' '+C.Soyadi KiraciAdiSoyadi
                FROM HukukiTakip_Table A
                INNER JOIN KiraSozlesme_Table B ON B.Id=A.SozlesmeId
                INNER JOIN Kiraci_Table C ON C.Id=A.KiraciId
                WHERE A.Aktif=1"), "");
        }

        public bool DeleteBySozlesmeId(int sozlesmeId, SqlTransactionContext transaction, PortalLogContext context)
        {
            SqlQuery query = new SqlQuery("DELETE HukukiTakip_Table WHERE SozlesmeId=@SozlesmeId");
            query.AddParameter("@SozlesmeId", sozlesmeId);
            return db.DeleteFromDb(query, transaction, context);
        }

        public DataTable SelectById(int id, SqlTransactionContext transaction, PortalLogContext context) { SqlQuery q = new SqlQuery("SELECT * FROM HukukiTakip_Table WITH (UPDLOCK, HOLDLOCK) WHERE Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, transaction, context); }
        public int Insert<T>(T entity, SqlTransactionContext transaction, PortalLogContext context) { return db.Insert(queryBuilder.BuildInsert(entity, "HukukiTakip_Table"), transaction, context); }
        public bool Update<T>(T entity, SqlTransactionContext transaction, PortalLogContext context) { return db.Update2Db(queryBuilder.BuildUpdate(entity, "HukukiTakip_Table"), transaction, context); }
        public bool Delete(int id, SqlTransactionContext transaction, PortalLogContext context) { return db.DeleteFromDb(queryBuilder.BuildDelete("HukukiTakip_Table", id), transaction, context); }
    }
}
