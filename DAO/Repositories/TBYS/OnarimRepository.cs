using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.TBYS
{
    public class OnarimRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public OnarimRepository() : this(new DbClass()) { }

        public OnarimRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM Onarim_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery("SELECT * FROM Onarim_Table"), "");
        }

        public DataTable SelectByTasinmazId(int tasinmazId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT * FROM Onarim_Table
                WHERE TasinmazId=@TasinmazId
                ORDER BY TasinmazId");
            query.AddParameter("@TasinmazId", tasinmazId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAllForDataTable()
        {
            return db.SelectFromDb(new SqlQuery(@"
                SELECT A.Id OnarimId, A.TasinmazId,A.YapilanIs,A.HarcamaUsulu,A.OnayTarihi,
                    Convert(nvarchar,replace (A.Tutar,'.',',')) as Tutar, A.Aciklama,
                    B.Adres, B.Ili,B.Ilcesi, B.Ili+' '+B.Ilcesi IliIlcesi
                FROM Onarim_Table A
                INNER JOIN Tasinmaz_Table B ON B.Id=A.TasinmazId
                ORDER BY OnayTarihi DESC"), "");
        }

        public DataTable SelectByOnarimId(int onarimId)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM Onarim_Table WHERE Id=@Id");
            query.AddParameter("@Id", onarimId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectNext(int onarimId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT * FROM Onarim_Table
                WHERE Id > @Id
                ORDER BY Id");
            query.AddParameter("@Id", onarimId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectPrevious(int onarimId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT * FROM Onarim_Table
                WHERE Id < @Id
                ORDER BY Id");
            query.AddParameter("@Id", onarimId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectMaxId()
        {
            return db.SelectFromDb(new SqlQuery("SELECT MAX(Id) Id FROM Onarim_Table"), "");
        }

        public DataTable SelectMinId()
        {
            return db.SelectFromDb(new SqlQuery("SELECT MIN(Id) Id FROM Onarim_Table"), "");
        }

        public DataTable SelectByTasinmazIdWithAddress(int tasinmazId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT A.Id, A.TasinmazId, A.YapilanIs, convert(decimal(10, 2), A.Tutar) Tutar,
                    A.OnayTarihi, A.Aciklama,A.HarcamaUsulu, B.Ili, B.Ilcesi,B.Adres
                FROM Onarim_Table A
                INNER JOIN Tasinmaz_Table B ON B.Id = A.TasinmazId
                WHERE A.TasinmazId=@TasinmazId");
            query.AddParameter("@TasinmazId", tasinmazId);
            return db.SelectFromDb(query, "");
        }

        public int Insert<T>(T entity)
        {
            return db.Insert(queryBuilder.BuildInsert(entity, "Onarim_Table"));
        }

        public bool Update<T>(T entity)
        {
            return db.Update2Db(queryBuilder.BuildUpdate(entity, "Onarim_Table"));
        }

        public bool Delete(int id)
        {
            return db.DeleteFromDb(queryBuilder.BuildDelete("Onarim_Table", id), "");
        }
    }
}
