using DAO.Ortak;
using System;
using System.Collections.Generic;

namespace Model.Ortak
{
    [Serializable]
    public abstract class ParentClass : EntityBase, ICRUDInterface
    {
        protected readonly DbClass dao = new DbClass();

        public abstract T Select<T>(int id);
        public abstract int Save();
        public abstract bool Update();
        public abstract bool Delete();
        public abstract List<T> SelectAll<T>() where T : class;
    }
}
