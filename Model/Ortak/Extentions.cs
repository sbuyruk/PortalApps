using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using Utility.HelperClasses;

namespace Model.Ortak
{
    public static class Extentions
    {
        public static List<Variance> DetailedCompare<T>(this T val1, T val2)
        {
            List<Variance> variances = new List<Variance>();
            List<Variance> varianceIds = new List<Variance>();
            Type type1 = val1.GetType();//typeof(T);
            Type type2 = val2.GetType();//typeof(T);
            if (type1.Equals(type2))
            {
                
                PropertyInfo[] propInfo = type1.GetProperties();
                bool isChanged = false;
                
                foreach (PropertyInfo pi in propInfo)
                {
                    if (pi.Name.Contains("Id") ||
                       pi.Name.Contains("Tip") )
                    {
                        string propval1 = pi.GetValue(val1).ReturnEmptyIfNull().ToString();
                        Variance vIds = new Variance();
                        Variance v = new Variance();
                        v.Prop = pi.Name;
                        v.valA = propval1;
                        varianceIds.Add(v);

                    }else if (pi.Name.Equals("OlusturmaTarihi") ||
                        pi.Name.Equals("DegistirmeTarihi") )
                    {
                        continue;
                    }else
                    {
                        string propval1 = pi.GetValue(val1).ReturnEmptyIfNull().ToString();
                        string propval2 = pi.GetValue(val2).ReturnEmptyIfNull().ToString();
                        if (!propval1.Equals(propval2)){
                            Variance v = new Variance();
                            v.Prop = pi.Name;
                            v.valA = propval1;
                            v.valB = propval2;
                            variances.Add(v);
                            isChanged = true;
                        }
                    }

                }
                if (isChanged)
                {
                    variances.AddRange(varianceIds);
                }
            }
            return variances;
        }

        public static List<Variance> GetFields<T>(this T val1)
        {
            List<Variance> fields = new List<Variance>();
            Type type1 = val1.GetType();//typeof(T);
            PropertyInfo[] propInfo = type1.GetProperties();
            foreach (PropertyInfo pi in propInfo)
                {
                    string propval1 = pi.GetValue(val1).ReturnEmptyIfNull().ToString();
                    Variance v = new Variance();
                    v.Prop = pi.Name;
                    v.valA = propval1;
                    fields.Add(v);
                }
            return fields;
        }

    }
}