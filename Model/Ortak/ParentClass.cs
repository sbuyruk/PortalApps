using DAO.Ortak;
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
    public abstract class ParentClass : ICRUDInterface
    {
        public DbClass dao = new DbClass();
        public int Id { get; set; }
        public DateTime OlusturmaTarihi { get; set; }
        public DateTime DegistirmeTarihi { get; set; }
        public string Olusturan { get; set; }
        public string Degistiren { get; set; }
        public abstract T Select<T>(int id);
        public abstract int Save();
        public abstract bool Update();
        public abstract bool Delete();
        public abstract List<T> SelectAll<T>() where T : class;

        public List<T> ToList<T>(DataTable dataTable) where T : new()
        {
            try
            {
                var dataList = new List<T>();
                if (dataTable != null)
                {
                    //Define what attributes to be read from the class
                    const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

                    //Read Attribute Names and Types
                    var objFieldNames = typeof(T).GetProperties(flags).Cast<PropertyInfo>().
                        Select(item => new
                        {
                            Name = item.Name,
                            Type = Nullable.GetUnderlyingType(item.PropertyType) ?? item.PropertyType
                        }).ToList();

                    //Read Datatable column names and types
                    var dtlFieldNames = dataTable.Columns.Cast<DataColumn>().
                        Select(item => new
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
                                    propertyInfos.SetValue
                                    (classObj, convertToDateTime(dataRow[dtField.Name]), null);
                                }
                                else if (propertyInfos.PropertyType == typeof(int))
                                {
                                    propertyInfos.SetValue
                                    (classObj, ConvertToInt(dataRow[dtField.Name]), null);
                                }
                                else if (propertyInfos.PropertyType == typeof(long))
                                {
                                    propertyInfos.SetValue
                                    (classObj, ConvertToLong(dataRow[dtField.Name]), null);
                                }
                                else if (propertyInfos.PropertyType == typeof(bool))
                                {
                                    propertyInfos.SetValue
                                    (classObj, ConvertToBool(dataRow[dtField.Name]), null);
                                }
                                else if (propertyInfos.PropertyType == typeof(decimal))
                                {
                                    propertyInfos.SetValue
                                    (classObj, ConvertToDecimal(dataRow[dtField.Name]), null);
                                }
                                else if (propertyInfos.PropertyType == typeof(String))
                                {
                                    if (dataRow[dtField.Name].GetType() == typeof(DateTime))
                                    {
                                        propertyInfos.SetValue
                                        (classObj, ConvertToDateString(dataRow[dtField.Name]), null);
                                    }
                                    else
                                    {
                                        propertyInfos.SetValue
                                        (classObj, ConvertToString(dataRow[dtField.Name]), null);
                                    }
                                }
                                else if (propertyInfos.PropertyType == typeof(Guid))
                                {
                                    Guid guid = Guid.Parse(dataRow[dtField.Name].ToString());
                                    propertyInfos.SetValue
                                    (classObj, guid, null);
                                }
                            }
                        }
                        dataList.Add(classObj);
                    }
                }
                else
                {
                    //todo:query boş
                }
                return dataList;
            }
            catch (Exception ex)
            {
                throw new Exception("ParentClass.cs Method: ToList", ex);
                //throw;
            }
        }
        private string ConvertToDateString(object date)
        {
            if (date == null)
                return string.Empty;

            return HelperFunctions.ConvertDate(Convert.ToDateTime(date));
        }

        private string ConvertToString(object value)
        {
            return Convert.ToString(HelperFunctions.ReturnEmptyIfNull(value));
        }

        private int ConvertToInt(object value)
        {
            return Convert.ToInt32(HelperFunctions.ReturnZeroIfNull(value));
        }

        private long ConvertToLong(object value)
        {
            return Convert.ToInt64(HelperFunctions.ReturnZeroIfNull(value));
        }
        private bool ConvertToBool(object value)
        {
            return Convert.ToBoolean(HelperFunctions.ReturnFalseIfNull(value));
        }

        private decimal ConvertToDecimal(object value)
        {
            return Convert.ToDecimal(HelperFunctions.ReturnZeroIfNull(value));
        }

        private DateTime convertToDateTime(object date)
        {


            var dateTime = Convert.ToDateTime(HelperFunctions.ReturnDateTimeMinIfNull(date));

            DateTime trDateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);

            return trDateTime;
        }

        public string ToJSON(DataTable table)
        {
            string json = "[]";
            if (table != null)
            {

                JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
                List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
                Dictionary<string, object> childRow;
                foreach (DataRow row in table.Rows)
                {
                    childRow = new Dictionary<string, object>();
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


    }
}
