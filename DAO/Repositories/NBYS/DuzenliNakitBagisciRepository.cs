using DAO.Ortak;
using System.Data;
using System;
using Utility.Logging;

namespace DAO.Repositories.NBYS
{
    public class DuzenliNakitBagisciRepository
    {
        private const string TableName = "DuzenliNakitBagisci_Table";
        private readonly DbClass db;
        private readonly NbysSqlExecutor sql;

        public DuzenliNakitBagisciRepository(PortalLogContext context)
        {
            sql = new NbysSqlExecutor(context, "DuzenliNakitBagisciRepository");
        }

        public DuzenliNakitBagisciRepository() : this(new DbClass())
        {
        }

        public DuzenliNakitBagisciRepository(DbClass db)
        {
            this.db = db;
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM DuzenliNakitBagisci_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return sql == null ? db.SelectFromDb(query, "") : sql.Select(query);
        }

        public DataTable SelectAll()
        {
            SqlQuery query = new SqlQuery("SELECT * FROM DuzenliNakitBagisci_Table");
            return sql == null ? db.SelectFromDb(query, "") : sql.Select(query);
        }

        public DataTable SelectActiveByBagisciId(int bagisciId)
        {
            SqlQuery query = new SqlQuery(
                "SELECT * FROM DuzenliNakitBagisci_Table WHERE Aktif=1 AND BagisciId=@BagisciId");
            query.AddParameter("@BagisciId", bagisciId);
            return sql == null ? db.SelectFromDb(query, "") : sql.Select(query);
        }

    }
}
