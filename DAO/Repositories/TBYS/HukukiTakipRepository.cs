using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.TBYS
{
    public class HukukiTakipRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public HukukiTakipRepository() : this(new DbClass()) { }

        public HukukiTakipRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

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

        public int Insert<T>(T entity)
        {
            return db.Insert(queryBuilder.BuildInsert(entity, "HukukiTakip_Table"));
        }

        public bool Update<T>(T entity)
        {
            return db.Update2Db(queryBuilder.BuildUpdate(entity, "HukukiTakip_Table"));
        }

        public bool Delete(int id)
        {
            return db.DeleteFromDb(queryBuilder.BuildDelete("HukukiTakip_Table", id), "");
        }

        public bool DeleteBySozlesmeId(int sozlesmeId)
        {
            SqlQuery query = new SqlQuery("DELETE HukukiTakip_Table WHERE SozlesmeId=@SozlesmeId");
            query.AddParameter("@SozlesmeId", sozlesmeId);
            return db.DeleteFromDb(query, "");
        }
    }
}
