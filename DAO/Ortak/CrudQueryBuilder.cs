using System;
using System.Reflection;
using System.Text;

namespace DAO.Ortak
{
    public class CrudQueryBuilder
    {
        public SqlQuery BuildInsert<T>(T entity, string tableName)
        {
            if ((object)entity == null)
                throw new ArgumentNullException("entity");

            SqlQuery query = new SqlQuery();
            StringBuilder columns = new StringBuilder();
            StringBuilder values = new StringBuilder();

            foreach (PropertyInfo property in entity.GetType().GetProperties())
            {
                if (property.Name.Equals("Id"))
                    continue;

                AppendSeparator(columns);
                AppendSeparator(values);
                columns.Append(property.Name);
                values.Append("@" + property.Name);
                query.AddParameter("@" + property.Name, GetParameterValue(entity, property));
            }

            query.Sql = string.Format(
                "INSERT INTO {0} ({1}) VALUES({2})",
                tableName,
                columns,
                values);
            return query;
        }

        public SqlQuery BuildUpdate<T>(T entity, string tableName)
        {
            if ((object)entity == null)
                throw new ArgumentNullException("entity");

            SqlQuery query = new SqlQuery();
            StringBuilder setClause = new StringBuilder();

            foreach (PropertyInfo property in entity.GetType().GetProperties())
            {
                query.AddParameter("@" + property.Name, GetParameterValue(entity, property));
                if (property.Name.Equals("Id"))
                    continue;

                AppendSeparator(setClause);
                setClause.AppendFormat("{0}=@{0}", property.Name);
            }

            query.Sql = string.Format(
                "UPDATE {0} SET {1} WHERE Id=@Id",
                tableName,
                setClause);
            return query;
        }

        public SqlQuery BuildDelete(string tableName, int id)
        {
            SqlQuery query = new SqlQuery(
                string.Format("DELETE FROM {0} WHERE Id=@Id", tableName));
            query.AddParameter("@Id", id);
            return query;
        }

        private static void AppendSeparator(StringBuilder builder)
        {
            if (builder.Length > 0)
                builder.Append(", ");
        }

        private static object GetParameterValue<T>(T entity, PropertyInfo property)
        {
            object value = property.GetValue(entity, null);
            if (value == null)
                return DBNull.Value;

            if (property.PropertyType == typeof(DateTime)
                && (DateTime)value < new DateTime(1901, 1, 1))
            {
                return DBNull.Value;
            }

            return value;
        }
    }
}
