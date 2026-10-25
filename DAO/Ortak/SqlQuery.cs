using System.Collections.Generic;
using System.Data.SqlClient;

namespace DAO.Ortak
{
    /// <summary>
    /// Parametreli SQL sorgusunu ve parametrelerini birlikte taşır.
    /// SQL injection'a karşı string birleştirme yerine bu sınıf kullanılmalıdır.
    /// </summary>
    public class SqlQuery
    {
        public string Sql { get; set; }
        public List<SqlParameter> Parameters { get; } = new List<SqlParameter>();

        public SqlQuery()
        {
        }

        public SqlQuery(string sql)
        {
            Sql = sql;
        }

        public void AddParameter(string name, object value)
        {
            Parameters.Add(new SqlParameter(name, value ?? System.DBNull.Value));
        }


    }
}
