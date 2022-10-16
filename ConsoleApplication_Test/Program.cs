using Model.NBYS;
using System;
using System.Linq;

namespace ConsoleApplication_Test
{
    class Program
    {
        static void Main(string[] args)
        {

            EkstreAktarma eaDao = new EkstreAktarma();
            eaDao = eaDao.Select<EkstreAktarma>(128780);
            if (eaDao!=null)
            {
                bool isDeleted = eaDao.Delete();
                Console.WriteLine("Silindi="+isDeleted.ToString());
            }
                       
                       
            Console.WriteLine("Bitti");
            Console.ReadKey();
        }
        private static string FindTable(string sqlString)
        {
            string retval = "Bulunamadı!";
            var punctuation = sqlString.Where(Char.IsPunctuation).Distinct().ToArray();
            var words = sqlString.Split().Select(x => x.Trim(punctuation));
            var containsHi = words.Contains("Table", StringComparer.OrdinalIgnoreCase);
            foreach (string item in words)
            {
                bool contains = item.ToUpper().Contains("TABLE");
                if (contains)
                {
                    retval = item;
                }
                    
            }

            return retval;
        }
    }
}
