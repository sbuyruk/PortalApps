using System;
using System.Text;
using DAO.Ortak;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.Ortak
{
    public class GenericEntity<T>
    {
        int _sqlOption;

        public GenericEntity()
        {
            _sqlOption = ProjeConstants.SQL_SELECT;
        }
        public GenericEntity(int sqlOption)
        {
            _sqlOption = sqlOption;
        }

        public SqlQuery GetQueryParametreli(T t)
        {
            if (_sqlOption == ProjeConstants.SQL_SELECT)
                return GetSelectParametreli(t);
            else if (_sqlOption == ProjeConstants.SQL_UPDATE || _sqlOption == ProjeConstants.SQL_SELECTWITHFILTER)
                return GetUpdateParametreli(t);
            else if (_sqlOption == ProjeConstants.SQL_DELETE)
                return GetDeleteParametreli(t);
            else
                return GetInsertParametreli(t);
        }

        private SqlQuery GetInsertParametreli(T modelType)
        {
            Type type = modelType.GetType();
            SqlQuery query = new SqlQuery();
            StringBuilder sbCols = new StringBuilder();
            StringBuilder sbVals = new StringBuilder();

            foreach (System.Reflection.PropertyInfo pi in type.GetProperties())
            {
                if (pi.Name.Equals("Id"))
                    continue;

                if (sbCols.Length > 0)
                {
                    sbCols.Append(", ");
                    sbVals.Append(", ");
                }
                sbCols.Append(pi.Name);
                sbVals.Append("@" + pi.Name);
                query.AddParameter("@" + pi.Name, GetParameterValue(modelType, pi));
            }

            query.Sql = string.Format("INSERT INTO {0} ({1}) VALUES({2})", type.Name + "_Table", sbCols, sbVals);
            return query;
        }

        private SqlQuery GetUpdateParametreli(T modelType)
        {
            Type type = typeof(T);
            SqlQuery query = new SqlQuery();
            StringBuilder sbSet = new StringBuilder();

            foreach (System.Reflection.PropertyInfo pi in type.GetProperties())
            {
                query.AddParameter("@" + pi.Name, GetParameterValue(modelType, pi));
                if (pi.Name.Equals("Id"))
                    continue;

                if (sbSet.Length > 0)
                    sbSet.Append(", ");
                sbSet.AppendFormat("{0}=@{0}", pi.Name);
            }

            query.Sql = string.Format("Update {0} Set {1} Where Id=@Id", type.Name + "_Table", sbSet);
            return query;
        }

        private SqlQuery GetDeleteParametreli(T modelType)
        {
            Type type = typeof(T);
            SqlQuery query = new SqlQuery();
            System.Reflection.PropertyInfo pi = type.GetProperty("Id");
            query.AddParameter("@Id", GetParameterValue(modelType, pi));
            query.Sql = string.Format("Delete From {0} Where Id=@Id", type.Name + "_Table");
            return query;
        }

        private SqlQuery GetSelectParametreli(T modelType)
        {
            Type type = typeof(T);
            SqlQuery query = new SqlQuery();
            StringBuilder sbCols = new StringBuilder();

            foreach (System.Reflection.PropertyInfo pi in type.GetProperties())
            {
                if (sbCols.Length > 0)
                    sbCols.Append(", ");
                sbCols.Append(pi.Name);
            }

            System.Reflection.PropertyInfo idPi = type.GetProperty("Id");
            query.AddParameter("@Id", GetParameterValue(modelType, idPi));
            query.Sql = string.Format("Select {0} From {1} Where Id=@Id", sbCols, type.Name.Replace("Entity", string.Empty) + "_Table");
            return query;
        }

        private static object GetParameterValue(T modelType, System.Reflection.PropertyInfo pi)
        {
            object raw = pi.GetValue(modelType);
            if (raw == null)
                return DBNull.Value;

            // Mevcut GetInsertValues kuralı: 1901 öncesi tarihler null yazılır
            if (pi.PropertyType == typeof(DateTime) && (DateTime)raw < new DateTime(1901, 1, 1))
                return DBNull.Value;

            return raw;
        }

        public string GetQuery(T t)
        {
            if (_sqlOption == ProjeConstants.SQL_SELECT)
                return GetSelect(t);
            else if (_sqlOption == ProjeConstants.SQL_SELECTWITHFILTER)
                return GetUpdate(t);
            else if (_sqlOption == ProjeConstants.SQL_UPDATE)
                return GetUpdate(t);
            else if (_sqlOption == ProjeConstants.SQL_DELETE)
                return GetDelete(t);
            else
                return GetInsert(t);
        }
        
        public string GetQuery(T t, string extId)
        {
            if (_sqlOption == ProjeConstants.SQL_SELECT)
                return GetSelect(t,extId);
            else if (_sqlOption == ProjeConstants.SQL_UPDATE)
                return GetUpdate(t, extId);
            else if (_sqlOption == ProjeConstants.SQL_DELETE)
                return GetDelete(t);
            else
                return GetInsert(t, extId);
        }

        private string GetDelete(T modelType)
        {
            Type type = typeof(T);
            StringBuilder sbQry = new StringBuilder();
            StringBuilder idstr = new StringBuilder();
            System.Reflection.PropertyInfo[] propInfo = type.GetProperties();

            foreach (System.Reflection.PropertyInfo pi in propInfo)
            {
                string value = pi.GetValue(modelType).ReturnEmptyIfNull().ToString();
                Type dataType = pi.PropertyType;
                object typestr = GetInsertValues(value, dataType);
                if (pi.Name.Equals("Id"))
                {
                    idstr.AppendFormat("{0}={1} ", pi.Name, typestr.ToString());
                    break;
                }
            }
            sbQry.AppendFormat("Delete From {0} Where {1}",
               type.Name + "_Table", idstr.ToString());
            return sbQry.ToString();
        }

        private string GetUpdate(T modelType)
        {
            int ctr = 0;
            Type type = typeof(T);
            StringBuilder sbQry = new StringBuilder();
            StringBuilder idstr = new StringBuilder();
            System.Reflection.PropertyInfo[] propInfo = type.GetProperties();
            foreach (System.Reflection.PropertyInfo pi in propInfo)
            {
                string value = pi.GetValue(modelType).ReturnEmptyIfNull().ToString();//(t.GetType().GetProperties())[ctr].GetValue(t).ReturnEmptyIfNull().ToString();
                Type dataType = pi.PropertyType;
                object typestr = GetInsertValues(value, dataType);
                if (pi.Name.Equals("Id"))
                {
                    idstr.AppendFormat("{0}={1} ", pi.Name, typestr.ToString());
                    continue;
                }
                else
                {


                    if (sbQry.ToString() == string.Empty)
                        sbQry.AppendFormat("Update {0} Set {1}={2}",
                                 type.Name + "_Table", pi.Name, typestr.ToString());
                    else
                    {
                        sbQry.AppendFormat(", {0}={1}", pi.Name, typestr.ToString());

                    }


                    ctr++;
                }
            }

            if (sbQry.ToString() != string.Empty)
            {
                sbQry.AppendFormat(" Where {0}", idstr.ToString());
            }


            sbQry.Replace("[", "{").Replace("]", "}");

            return sbQry.ToString();
        }

        private string GetInsert(T modelType)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder entityValues = new StringBuilder();
            int ctr = 0;


            Type type = modelType.GetType();//typeof(T);
            StringBuilder sbQry = new StringBuilder();
            System.Reflection.PropertyInfo[] propInfo = type.GetProperties();
            foreach (System.Reflection.PropertyInfo pi in propInfo)
            {
                if (pi.Name.Equals("Id"))
                {
                    continue;
                }
                else
                {
                    if (sbQry.ToString() == string.Empty)
                        sbQry.AppendFormat("INSERT INTO {0} ({1}",
                           type.Name + "_Table", pi.Name);//type.Name.Replace("Entity", string.Empty), pi.Name);
                    else
                    {
                        sbQry.AppendFormat(", {0}", pi.Name);
                        sb.Append(",");
                        entityValues.Append(",");
                    }
                    string value = pi.GetValue(modelType).ReturnEmptyIfNull().ToString();//(t.GetType().GetProperties())[ctr].GetValue(t).ReturnEmptyIfNull().ToString();
                    Type dataType = pi.PropertyType;
                    object typestr = GetInsertValues(value, dataType);
                    entityValues.Append(typestr);
                    sb.Append("{" + ctr++ + "}");
                }

            }

            if (sbQry.ToString() != string.Empty)
                sbQry.AppendFormat(") VALUES({0})", entityValues.ToString());

            return sbQry.ToString();
        }

        private object GetInsertValues(string value, Type dataType)
        {

            object retVal = string.Empty;

            if (dataType == typeof(DateTime))
            {
                retVal = value.ConvertToDatetime().ReturnTRDateFormat();
                DateTime tarih = new DateTime(1901, 1, 1);
                if (value.ConvertToDatetime() < tarih)
                    retVal = "null";
            }
            else if (dataType == typeof(int))
            {
                retVal = value.ReturnQuotedValue();
            }
            else if (dataType == typeof(long))
            {
                retVal = value.ReturnQuotedValue();
            }
            else if (dataType == typeof(bool))
            {
                retVal = value.ReturnQuotedValue();
            }
            else if (dataType == typeof(decimal))
            {
                retVal = value.ConvertDecimalToString();
            }
            else if (dataType == typeof(String))
            {
                retVal = value.ReturnQuotedValue();
            }
            else if (dataType == typeof(TimeSpan))
            {
                retVal = value.ReturnQuotedValue();
            }
            else if (dataType == typeof(Guid))
            {
                retVal = value.ToString().ReturnQuotedValue();
            }
            return retVal;
        }
        private string GetSelect(T modelType)
        {
            Type type = typeof(T);
            StringBuilder sbQry = new StringBuilder();
            System.Reflection.PropertyInfo[] propInfo = type.GetProperties();
            foreach (System.Reflection.PropertyInfo pi in propInfo)
            {
                if (sbQry.ToString() == string.Empty)
                    sbQry.AppendFormat("Select {0}", pi.Name);
                else
                    sbQry.AppendFormat(", {0}", pi.Name);
            }

            if (sbQry.ToString() != string.Empty)
            {
                sbQry.AppendFormat(" From {0} ", type.Name.Replace("Entity", string.Empty) + "_Table");
                System.Reflection.PropertyInfo pi = type.GetProperty("Id");
                string value = pi.GetValue(modelType).ReturnZeroIfNull().ToString();//(t.GetType().GetProperties())[ctr].GetValue(t).ReturnEmptyIfNull().ToString();

                StringBuilder idstr = new StringBuilder();
                idstr.AppendFormat("{0}={1} ", "Id", value.ToString());
                sbQry.AppendFormat(" Where {0}", idstr.ToString());
            }
            return sbQry.ToString();
        }
        private string GetSelect(T modelType, string wherestr)
        {
            Type type = typeof(T);
            StringBuilder sbQry = new StringBuilder();
            System.Reflection.PropertyInfo[] propInfo = type.GetProperties();
            foreach (System.Reflection.PropertyInfo pi in propInfo)
            {
                if (sbQry.ToString() == string.Empty)
                    sbQry.AppendFormat("Select {0}", pi.Name);
                else
                    sbQry.AppendFormat(", {0}", pi.Name);
            }

            if (sbQry.ToString() != string.Empty)
            {
                sbQry.AppendFormat(" From {0} ", type.Name.Replace("Entity", string.Empty) + "_Table");
                sbQry.AppendFormat(!string.IsNullOrEmpty(wherestr)?" {0}":string.Empty, wherestr.ToString());
            }

            return sbQry.ToString();
        }
        // TRANSACTION
        private string GetUpdate(T modelType, string extId)
        {
            int ctr = 0;
            Type type = typeof(T);
            StringBuilder sbQry = new StringBuilder();
            StringBuilder idstr = new StringBuilder();
            System.Reflection.PropertyInfo[] propInfo = type.GetProperties();
            foreach (System.Reflection.PropertyInfo pi in propInfo)
            {
                string value = pi.GetValue(modelType).ReturnEmptyIfNull().ToString();//(t.GetType().GetProperties())[ctr].GetValue(t).ReturnEmptyIfNull().ToString();
                Type dataType = pi.PropertyType;
                object typestr = GetInsertValues(value, dataType, extId, pi.Name);
                if (pi.Name.Equals("Id"))
                {
                    if (extId.Contains("{") && extId.Contains("}"))//ID degeri transaction icinde sonradan beli olacak ise burada Id={0} seklinde olsun
                    {
                        idstr.AppendFormat("{0}={1} ", pi.Name, extId.ToString());
                    }
                    else
                    {
                        idstr.AppendFormat("{0}={1} ", pi.Name, extId.ToString());// typestr.ToString());
                    }
                    continue;
                }
                else
                {
                    if (sbQry.ToString() == string.Empty)
                        sbQry.AppendFormat("Update {0} Set {1}={2}",
                                 type.Name + "_Table", pi.Name, typestr.ToString());
                    else
                    {
                        sbQry.AppendFormat(", {0}={1}", pi.Name, typestr.ToString());

                    }


                    ctr++;
                }
            }

            if (sbQry.ToString() != string.Empty)
            {
                sbQry.AppendFormat(" Where {0}", idstr.ToString());
            }


            sbQry.Replace("[", "{").Replace("]", "}");

            return sbQry.ToString();
        }

        private string GetInsert(T modelType, string extId)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder entityValues = new StringBuilder();
            int ctr = 0;


            Type type = modelType.GetType();//typeof(T);
            StringBuilder sbQry = new StringBuilder();
            System.Reflection.PropertyInfo[] propInfo = type.GetProperties();
            foreach (System.Reflection.PropertyInfo pi in propInfo)
            {
                if (pi.Name.Equals("Id"))
                {
                    continue;
                }
                else
                {
                    if (sbQry.ToString() == string.Empty)
                        sbQry.AppendFormat("INSERT INTO {0} ({1}",
                           type.Name + "_Table", pi.Name);//type.Name.Replace("Entity", string.Empty), pi.Name);
                    else
                    {
                        sbQry.AppendFormat(", {0}", pi.Name);
                        sb.Append(",");
                        entityValues.Append(",");
                    }
                    string value = pi.GetValue(modelType).ReturnEmptyIfNull().ToString();//(t.GetType().GetProperties())[ctr].GetValue(t).ReturnEmptyIfNull().ToString();
                    Type dataType = pi.PropertyType;
                    object typestr = GetInsertValues(value, dataType, extId, pi.Name);
                    entityValues.Append(typestr);
                    sb.Append("{" + ctr++ + "}");
                }

            }

            if (sbQry.ToString() != string.Empty)
                sbQry.AppendFormat(") VALUES({0})", entityValues.ToString());

            return sbQry.ToString();
        }

        private object GetInsertValues(string value, Type dataType, string extId, string piName)
        {
            object retVal = string.Empty;
            if (piName.Equals(extId))
            {
                retVal = "{0}";
            }

            else
            {
                if (dataType == typeof(DateTime))
                {
                    retVal = value.ConvertToDatetime().ReturnTRDateFormat();
                    DateTime tarih = new DateTime(1901, 1, 1);
                    if (value.ConvertToDatetime() < tarih)
                        retVal = "null";
                }
                else if (dataType == typeof(int))
                {
                    retVal = value.ReturnQuotedValue();
                }
                else if (dataType == typeof(long))
                {
                    retVal = value.ReturnQuotedValue();
                }
                else if (dataType == typeof(bool))
                {
                    retVal = value.ReturnQuotedValue();
                }
                else if (dataType == typeof(decimal))
                {
                    retVal = value.ConvertDecimalToString();
                }
                else if (dataType == typeof(String))
                {
                    retVal = value.ReturnQuotedValue();
                }
                else if (dataType == typeof(Guid))
                {
                    retVal = value.ToString().ReturnQuotedValue();
                }
            }
            return retVal;
        }

    }
}
