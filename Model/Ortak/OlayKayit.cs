using Model.MTS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utility.ProjeGlobal;

namespace Model.Ortak
{
    public class OlayKayit
    {
        public OlayKayit() { }
        public bool GuncellemeOlayKaydet<T> (T eski, T yeni, string program,string modul) 
        {
            bool kaydedildi = false;
            if (eski != null && yeni != null)
            {
                //This is the comparison class

                
                List<Variance> rt = yeni.DetailedCompare(eski);
                if (rt.Count > 0)
                {
                    Olay olay = new Olay();
                    olay.Aciklama = "Modul="+modul+ProjeConstants.DELIMITER;
                    foreach (var item in rt)
                    {
                        olay.Aciklama += (string.IsNullOrEmpty(item.Prop) ? "PropYok" : item.Prop + "=") +
                            (item.valA == null ? "" : item.valA) +
                            (item.valB == null ? "" : "#" + item.valB) +ProjeConstants.DELIMITER;
                    }
                    olay.IslemKonusu =modul;
                    olay.IslemTarihi = DateTime.Now;
                    olay.IslemTipi = "Güncelleme";
                    olay.IslemYapan = UtilityHelper.GetCurrentUserName();
                    olay.Olusturan = UtilityHelper.GetCurrentUserName();
                    olay.OlusturmaTarihi = DateTime.Now;
                    olay.Program = program;
                    olay.Save();
                }
                
            }
            return kaydedildi;
        }
        public bool SilmeOlayKaydet<T>(T silinen, string program, string modul)
        {
            bool silindi = false;
            if (silinen != null)
            {
                List<Variance> rt = silinen.GetFields();
                if (rt.Count > 0)
                {
                    
                    Olay olay = new Olay();
                    foreach (var item in rt)
                    {
                        olay.Aciklama += (string.IsNullOrEmpty(item.Prop) ? "PropYok" : item.Prop + "=") +
                            (item.valA == null ? "" : item.valA) + ProjeConstants.DELIMITER;
                    }
                    olay.IslemKonusu = modul;
                    olay.IslemTarihi = DateTime.Now;
                    olay.IslemTipi = "Silme";
                    olay.IslemYapan = UtilityHelper.GetCurrentUserName();
                    olay.Olusturan = UtilityHelper.GetCurrentUserName();
                    olay.OlusturmaTarihi = DateTime.Now;
                    olay.Program = program;
                    olay.Save();
                }

            }
            return silindi;
        }
        
        public bool GirisOlayKaydet<T>(T yeni, string program, string modul)
        {
            bool kaydedildi = false;
            if (yeni != null)
            {
                List<Variance> rt = yeni.GetFields();
                if (rt.Count > 0)
                {
                    
                    Olay olay = new Olay();
                    foreach (var item in rt)
                    {
                        if (item.Prop.Equals("Id"))
                        {
                            string type = yeni.GetType().Name;
                            olay.Aciklama += type + "Id="+ item.valA.ToString() + ProjeConstants.DELIMITER;
                        }
                        olay.Aciklama += item.Prop + "=" + item.valA + ProjeConstants.DELIMITER;
                        olay.Aciklama += (string.IsNullOrEmpty(item.Prop) ? "PropYok" : item.Prop + "=") +
                            (item.valA == null ? "" : item.valA) + ProjeConstants.DELIMITER;

                    }
                    olay.IslemKonusu = modul;
                    olay.IslemTarihi = DateTime.Now;
                    olay.IslemTipi = "Giriş";
                    olay.IslemYapan = UtilityHelper.GetCurrentUserName();
                    olay.Olusturan = UtilityHelper.GetCurrentUserName();
                    olay.OlusturmaTarihi = DateTime.Now;
                    olay.Program = program;
                    olay.Save();
                }
            }
            return kaydedildi;
        }
    }
}
