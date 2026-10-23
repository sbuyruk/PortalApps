using DAO.Ortak;
using System;
using System.Data;
using System.Diagnostics;
using Utility.Logging;
namespace DAO.Repositories.IKYS
{
    public class EgitimSeviyesiRepository
    {
        private readonly DbClass db; private readonly CrudQueryBuilder queryBuilder;
        public EgitimSeviyesiRepository() : this(new DbClass()) { }
        public EgitimSeviyesiRepository(DbClass db) { if (db == null) throw new ArgumentNullException("db"); this.db = db; queryBuilder = new CrudQueryBuilder(); }
        public DataTable SelectById(int id) { SqlQuery q = new SqlQuery("SELECT * FROM EgitimSeviyesi_Table WHERE Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM EgitimSeviyesi_Table"), ""); }
        public DataTable SelectAll(PortalLogContext context)
        { return PortalSqlReadExecutor.Select(new SqlQuery("SELECT * FROM EgitimSeviyesi_Table"), context, "EgitimSeviyesi.SelectAll"); }


        public DataTable SelectByPersonelId(int personelId) { SqlQuery q = new SqlQuery("SELECT * FROM EgitimSeviyesi_Table WHERE PersonelId=@PersonelId ORDER BY Id"); q.AddParameter("@PersonelId", personelId); return db.SelectFromDb(q, ""); }

        public DataTable SelectById(int id, PortalLogContext context)
        { SqlQuery q = new SqlQuery("SELECT * FROM EgitimSeviyesi_Table WHERE Id=@Id"); q.AddParameter("@Id", id); return PortalSqlReadExecutor.Select(q, context, "EgitimSeviyesi.SelectById"); }

        public DataTable SelectByPersonelId(int personelId, PortalLogContext context)
        { SqlQuery q = new SqlQuery("SELECT * FROM EgitimSeviyesi_Table WHERE PersonelId=@PersonelId ORDER BY Id"); q.AddParameter("@PersonelId", personelId); return PortalSqlReadExecutor.Select(q, context, "EgitimSeviyesi.SelectByPersonelId"); }
    }
}
