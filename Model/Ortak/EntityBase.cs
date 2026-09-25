using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web.Script.Serialization;
using Utility.HelperClasses;

namespace Model.Ortak
{
    [Serializable]
    public abstract class EntityBase
    {
        public int Id { get; set; }
        public DateTime OlusturmaTarihi { get; set; }
        public DateTime DegistirmeTarihi { get; set; }
        public string Olusturan { get; set; }
        public string Degistiren { get; set; }

        public List<T> ToList<T>(DataTable dataTable) where T : new()
        {
            try
            {
                var dataList = new List<T>();
                if (dataTable != null)
                {
                    const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

                    var objFieldNames = typeof(T).GetProperties(flags).Cast<PropertyInfo>()
                        .Select(item => new
                        {
                            Name = item.Name,
                            Type = Nullable.GetUnderlyingType(item.PropertyType) ?? item.PropertyType
                        }).ToList();

                    var dtlFieldNames = dataTable.Columns.Cast<DataColumn>()
                        .Select(item => new
                        {
                            Name = item.ColumnName,
                            Type = item.DataType
                        }).ToList();

                    foreach (DataRow dataRow in dataTable.AsEnumerable().ToList())
                    {
                        var classObj = new T();

                        foreach (var dtField in dtlFieldNames)
                        {
                            PropertyInfo propertyInfos = classObj.GetType().GetProperty(dtField.Name);
                            var field = objFieldNames.Find(x => x.Name == dtField.Name);

                            if (field != null)
                            {
                                if (propertyInfos.PropertyType == typeof(DateTime))
                                {
                                    propertyInfos.SetValue(classObj, ConvertToDateTime(dataRow[dtField.Name]), null);
                                }
                                else if (propertyInfos.PropertyType == typeof(int))
                                {
                                    propertyInfos.SetValue(classObj, ConvertToInt(dataRow[dtField.Name]), null);
                                }
                                else if (propertyInfos.PropertyType == typeof(long))
                                {
                                    propertyInfos.SetValue(classObj, ConvertToLong(dataRow[dtField.Name]), null);
                                }
                                else if (propertyInfos.PropertyType == typeof(bool))
                                {
                                    propertyInfos.SetValue(classObj, ConvertToBool(dataRow[dtField.Name]), null);
                                }
                                else if (propertyInfos.PropertyType == typeof(decimal))
                                {
                                    propertyInfos.SetValue(
                                        classObj,
                                        dataRow[dtField.Name].ReturnZeroIfNull().ConvertToDecimal(),
                                        null);
                                }
                                else if (propertyInfos.PropertyType == typeof(string))
                                {
                                    if (dataRow[dtField.Name].GetType() == typeof(DateTime))
                                    {
                                        propertyInfos.SetValue(classObj, ConvertToDateString(dataRow[dtField.Name]), null);
                                    }
                                    else
                                    {
                                        propertyInfos.SetValue(classObj, ConvertToString(dataRow[dtField.Name]), null);
                                    }
                                }
                                else if (propertyInfos.PropertyType == typeof(Guid))
                                {
                                    string data = dataRow[dtField.Name].ToString();
                                    Guid guid = string.IsNullOrEmpty(data)
                                        ? Guid.NewGuid()
                                        : Guid.Parse(dataRow[dtField.Name].ToString());
                                    propertyInfos.SetValue(classObj, guid, null);
                                }
                            }
                        }

                        dataList.Add(classObj);
                    }
                }

                return dataList;
            }
            catch (Exception ex)
            {
                throw new Exception("ParentClass.cs Method: ToList", ex);
            }
        }

        public string ToJSON(DataTable table)
        {
            string json = "[]";
            if (table != null)
            {
                JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
                List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
                foreach (DataRow row in table.Rows)
                {
                    Dictionary<string, object> childRow = new Dictionary<string, object>();
                    foreach (DataColumn col in table.Columns)
                    {
                        childRow.Add(col.ColumnName, row[col]);
                    }
                    parentRow.Add(childRow);
                }
                jsSerializer.MaxJsonLength = Int32.MaxValue;
                json = jsSerializer.Serialize(parentRow);
            }
            return json;
        }

        private static string ConvertToDateString(object date)
        {
            if (date == null)
                return string.Empty;

            return HelperFunctions.ConvertDate(Convert.ToDateTime(date));
        }

        private static string ConvertToString(object value)
        {
            return Convert.ToString(HelperFunctions.ReturnEmptyIfNull(value));
        }

        private static int ConvertToInt(object value)
        {
            return Convert.ToInt32(HelperFunctions.ReturnZeroIfNull(value));
        }

        private static long ConvertToLong(object value)
        {
            return Convert.ToInt64(HelperFunctions.ReturnZeroIfNull(value));
        }

        private static bool ConvertToBool(object value)
        {
            return Convert.ToBoolean(HelperFunctions.ReturnFalseIfNull(value));
        }

        private static DateTime ConvertToDateTime(object date)
        {
            if (date == null || date == DBNull.Value)
                return DateTime.MinValue;

            if (date is DateTime)
                return (DateTime)date;

            try
            {
                var dateTime = Convert.ToDateTime(
                    date,
                    System.Globalization.CultureInfo.InvariantCulture);
                return new DateTime(
                    dateTime.Year,
                    dateTime.Month,
                    dateTime.Day,
                    dateTime.Hour,
                    dateTime.Minute,
                    dateTime.Second);
            }
            catch
            {
                try
                {
                    var dateTime = DateTime.Parse(
                        date.ToString(),
                        System.Globalization.CultureInfo.GetCultureInfo("tr-TR"));
                    return new DateTime(
                        dateTime.Year,
                        dateTime.Month,
                        dateTime.Day,
                        dateTime.Hour,
                        dateTime.Minute,
                        dateTime.Second);
                }
                catch
                {
                    throw new Exception(
                        string.Format("Tarih parsing hatasi: '{0}' - Geçersiz tarih formati", date));
                }
            }
        }
    }
}
