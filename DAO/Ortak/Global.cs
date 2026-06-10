
using Microsoft.SharePoint;
using System;
using System.Linq;

namespace DAO.Ortak
{
    public static class Global
    {
        public static string GetCurrentUser()
        {
            string userName = string.Empty;
            using (SPSite site = new SPSite(SPContext.Current.Web.Url))
            {
                using (SPWeb currentWeb = site.OpenWeb())
                {
                    SPUser user = currentWeb.CurrentUser;
                    if (user != null)
                    {
                        userName = user.LoginName;
                    }
                }
            }
            return userName;
        }
        

        public static string FindTable(string sqlString)
        {
            string retval = "Bulunamadı!";
            var punctuation = sqlString.Where(Char.IsPunctuation).Distinct().ToArray();
            var words = sqlString.Split().Select(x => x.Trim(punctuation));
            foreach (string item in words)
            {
                bool contains = item.ToUpper().Contains("_TABLE");
                if (contains)
                {
                    retval = item;
                    break;
                }

            }

            return retval;
        }

        public static T GetPropertyValue<T>(object obj, string propName) 
        {
            return (T)obj.GetType().GetProperty(propName).GetValue(obj, null); 
        }

        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }
    }
}
