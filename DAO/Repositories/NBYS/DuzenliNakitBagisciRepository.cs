using DAO.Ortak;
using System.Data;

namespace DAO.Repositories.NBYS
{
    public class DuzenliNakitBagisciRepository
    {
        private const string TableName = "DuzenliNakitBagisci_Table";
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder = new CrudQueryBuilder();

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
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery("SELECT * FROM DuzenliNakitBagisci_Table"), "");
        }

        public DataTable SelectActiveByBagisciId(int bagisciId)
        {
            SqlQuery query = new SqlQuery(
                "SELECT * FROM DuzenliNakitBagisci_Table WHERE Aktif=1 AND BagisciId=@BagisciId");
            query.AddParameter("@BagisciId", bagisciId);
            return db.SelectFromDb(query, "");
        }

        public int Insert<T>(T entity)
        {
            return db.Insert(queryBuilder.BuildInsert(entity, TableName));
        }

        public bool Update<T>(T entity)
        {
            return db.Update2Db(queryBuilder.BuildUpdate(entity, TableName));
        }

        public bool Delete(int id)
        {
            return db.DeleteFromDb(queryBuilder.BuildDelete(TableName, id), "");
        }
    }
}
