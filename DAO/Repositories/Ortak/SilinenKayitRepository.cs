using DAO.Ortak;
using System;

namespace DAO.Repositories.Ortak
{
    public class SilinenKayitRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public SilinenKayitRepository()
            : this(new DbClass())
        {
        }

        public SilinenKayitRepository(DbClass db)
        {
            if (db == null)
                throw new ArgumentNullException("db");

            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

        public int Insert<T>(T entity)
        {
            return db.Insert(queryBuilder.BuildInsert(entity, "SilinenKayit_Table"));
        }

        public bool Update<T>(T entity)
        {
            return db.Update2Db(queryBuilder.BuildUpdate(entity, "SilinenKayit_Table"));
        }
    }
}
