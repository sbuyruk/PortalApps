using Utility.Logging;
using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.Portal
{
    public class ToplantiParametreRepository
    {
        private readonly PortalSqlExecutor sql = new PortalSqlExecutor("ToplantiParametreRepository");
        public ToplantiParametreRepository(PortalLogContext context)
        {
            sql = new PortalSqlExecutor(context, "ToplantiParametreRepository");
        }
        private DataTable Select(SqlQuery query, [System.Runtime.CompilerServices.CallerMemberName] string method = "")
        {
            return sql.Select(query, method: method);
        }

        public ToplantiParametreRepository()
        {
        }

        public DataTable SelectById(int id)
        {
            var q = new SqlQuery("SELECT * FROM ToplantiParametre_Table WHERE Id=@Id");
            q.AddParameter("@Id", id);
            return Select(q);
        }

        public DataTable SelectById(int id, PortalLogContext context)
        {
            var q = new SqlQuery("SELECT * FROM ToplantiParametre_Table WHERE Id=@Id");
            q.AddParameter("@Id", id);
            return PortalSqlReadExecutor.Select(q, context, "ToplantiParametreRepository.SelectById");
        }

        public DataTable SelectAll()
        {
            return Select(new SqlQuery("SELECT * FROM ToplantiParametre_Table"));
        }

        public DataTable SelectByGroup(string g)
        {
            var q = new SqlQuery("SELECT * FROM ToplantiParametre_Table WHERE Grup=@Grup ORDER BY Sira");
            q.AddParameter("@Grup", g);
            return Select(q);
        }

        public DataTable SelectByGroupUnselected(string g)
        {
            var q = new SqlQuery("SELECT * FROM ToplantiParametre_Table WHERE Grup=@Grup AND Id NOT IN (SELECT ToplantiYeri FROM Toplanti_Table) ORDER BY Sira");
            q.AddParameter("@Grup", g);
            return Select(q);
        }

        public DataTable SelectByGroupSelected(string g)
        {
            var q = new SqlQuery("SELECT * FROM ToplantiParametre_Table WHERE Grup=@Grup AND Id IN (SELECT ToplantiYeri FROM Toplanti_Table) ORDER BY Sira");
            q.AddParameter("@Grup", g);
            return Select(q);
        }

        public DataTable SelectAllData()
        {
            return Select(new SqlQuery("SELECT * FROM ToplantiParametre_Table ORDER BY Sira"));
        }

        public DataTable SelectByGroupValue(string g, string d)
        {
            var q = new SqlQuery("SELECT * FROM ToplantiParametre_Table WHERE Grup=@Grup AND Deger=@Deger");
            q.AddParameter("@Grup", g);
            q.AddParameter("@Deger", d);
            return Select(q);
        }
    }
}

