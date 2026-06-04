using System.Collections.Generic;

namespace Model.Ortak
{
    public interface ICRUDInterface
    {
        T Select<T>(int id);
        int Save();
        bool Update();
        bool Delete();
        List<T> SelectAll<T>() where T : class;
    }
}
