using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

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

        /// <summary>
        /// Loglama amaçlı; sorgu ve parametre değerlerini metin olarak döndürür.
        /// </summary>
        public string ToLogString()
        {
            StringBuilder sb = new StringBuilder(Sql);
            foreach (SqlParameter p in Parameters)
            {
                sb.AppendFormat(" [{0}={1}]", p.ParameterName, p.Value);
            }
            return sb.ToString();
        }
    }
}
