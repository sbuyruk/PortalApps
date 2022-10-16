using Model.Ortak;
using Utility.ProjeGlobal;

namespace Model.NBYS
{
    public class NBYSOrtak
    {
        public NBYSOrtak()
        {

        }
        public static string YonergeURLGetir(string grup, string anahtar)
        {

            string url = UtilityHelper.URLGetir();
            string yonergeUrl = url + "/../" + ProjeConstants.NBYSBELGELERI_LIB + "/yonerge/default.pdf";
            NBYSParametre param = new NBYSParametre();
            param = param.SelectByGrupAnahtar(grup, anahtar);
            if (param != null)
            {
                yonergeUrl = url + "/../" + ProjeConstants.NBYSBELGELERI_LIB + "/yonerge/" + param.Deger;
            }

            return yonergeUrl;
        }
        public static string ParametreGetir(string grup,string anahtar)
        {

            NBYSParametre param = new NBYSParametre();
            param = param.SelectByGrupAnahtar(grup, anahtar);
            if (param != null)
            {
                return param.Deger;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
