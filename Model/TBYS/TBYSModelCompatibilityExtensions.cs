using Model.Services.TBYS;
using System;
using System.Collections.Generic;
using System.Data;

namespace Model.TBYS
{
    /// <summary>
    /// Eski entity çağrılarını geçici olarak korur; bütün işlemleri Service katmanına yönlendirir.
    /// Yeni kod doğrudan ilgili Service sınıfını kullanmalıdır.
    /// </summary>
    public static class TBYSModelCompatibilityExtensions
    {
        public static T Select<T>(this Tasinmaz item, int id) { return (T)Convert.ChangeType(new TasinmazService().GetInventoryById(id), typeof(T)); }
        public static int Save(this Tasinmaz item) { return new TasinmazService().Save(item); }
        public static bool Update(this Tasinmaz item) { return new TasinmazService().Update(item); }
        public static bool Delete(this Tasinmaz item) { return new TasinmazService().Delete(item); }
        public static List<T> SelectAll<T>(this Tasinmaz item) { return (List<T>)Convert.ChangeType(new TasinmazService().GetInventory(), typeof(List<T>)); }

        public static T Select<T>(this KiraSozlesme item, int id) { return (T)Convert.ChangeType(new KiraSozlesmeService().GetById(id), typeof(T)); }
        public static KiraSozlesme Select(this KiraSozlesme item, int id) { return new KiraSozlesmeService().GetById(id); }
        public static int Save(this KiraSozlesme item) { return new KiraSozlesmeService().Save(item); }
        public static bool Update(this KiraSozlesme item) { return new KiraSozlesmeService().Update(item); }
        public static bool Delete(this KiraSozlesme item) { return new KiraSozlesmeService().Delete(item); }
        public static List<T> SelectAll<T>(this KiraSozlesme item) { return (List<T>)Convert.ChangeType(new KiraSozlesmeService().GetAll(), typeof(List<T>)); }
        public static List<KiraSozlesme> SelectByTasinmazId(this KiraSozlesme item, int tasinmazId) { return new KiraSozlesmeService().GetByTasinmazId(tasinmazId); }

        public static T Select<T>(this SozlesmeTasinmaz item, int id) { return (T)Convert.ChangeType(new SozlesmeTasinmazService().GetById(id), typeof(T)); }
        public static int Save(this SozlesmeTasinmaz item) { return new SozlesmeTasinmazService().Save(item); }
        public static bool Update(this SozlesmeTasinmaz item) { return new SozlesmeTasinmazService().Update(item); }
        public static bool Delete(this SozlesmeTasinmaz item) { return new SozlesmeTasinmazService().Delete(item); }
        public static bool DeleteBySozlesmeId(this SozlesmeTasinmaz item, int id) { return new SozlesmeTasinmazService().DeleteBySozlesmeId(item, id); }
        public static List<T> SelectAll<T>(this SozlesmeTasinmaz item) { return (List<T>)Convert.ChangeType(new SozlesmeTasinmazService().GetAll(), typeof(List<T>)); }
        public static string SelectBySozlesmeIdReturnJson(this SozlesmeTasinmaz item, int id) { return new SozlesmeTasinmazService().GetBySozlesmeIdReturnJson(id); }
        public static T Select<T>(this KiraEkstreAktarma item, int id) { return (T)Convert.ChangeType(new KiraEkstreAktarmaService().GetById(id), typeof(T)); }
        public static int Save(this KiraEkstreAktarma item) { return new KiraEkstreAktarmaService().Save(item); }
        public static bool Update(this KiraEkstreAktarma item) { return new KiraEkstreAktarmaService().Update(item); }
        public static bool Delete(this KiraEkstreAktarma item) { return new KiraEkstreAktarmaService().Delete(item); }
        public static List<T> SelectAll<T>(this KiraEkstreAktarma item) { return (List<T>)Convert.ChangeType(new KiraEkstreAktarmaService().GetAll(), typeof(List<T>)); }

        public static T Select<T>(this Onarim item, int id) { return (T)Convert.ChangeType(new OnarimService().GetById(id), typeof(T)); }
        public static int Save(this Onarim item) { return new OnarimService().Save(item); }
        public static bool Update(this Onarim item) { return new OnarimService().Update(item); }
        public static bool Delete(this Onarim item) { return new OnarimService().Delete(item); }
        public static List<T> SelectAll<T>(this Onarim item) { return (List<T>)Convert.ChangeType(new OnarimService().GetAll(), typeof(List<T>)); }

        public static T Select<T>(this Bagis item, int id) { return (T)Convert.ChangeType(new BagisService().GetById(id), typeof(T)); }
        public static Bagis Select(this Bagis item, int id) { return new BagisService().GetById(id); }
        public static int Save(this Bagis item) { return new BagisService().Save(item); }
        public static bool Update(this Bagis item) { return new BagisService().Update(item); }
        public static bool Delete(this Bagis item) { return new BagisService().Delete(item); }
        public static List<T> SelectAll<T>(this Bagis item) { return (List<T>)Convert.ChangeType(new BagisService().GetAll(), typeof(List<T>)); }
    }
}
