using Model.NBYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.TBYS
{
    [Serializable]
    public class KiraEkstreAktarma : EntityBase
    {
        public DateTime IslemTarihi { get; set; }
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public long TCKimlikNo { get; set; }
        public DateTime OdemeTarihi { get; set; }
        
        public decimal Tutar { get; set; }
        public string DovizCinsi { get; set; }
        public string BankaAdi { get; set; }
        public string Telefon1 { get; set; }
        public string Adres { get; set; }
        public string Ili { get; set; }
        public string Ilcesi { get; set; }
        public string Eposta { get; set; }
        public string PostaKodu { get; set; }
        public string Aciklama { get; set; }
        public bool AktarildiMi { get; set; }
        public bool ElleKayit { get; set; }
        public int OdemeId { get; set; }
        public int KiraciId { get; set; }
        public string IslemNo { get; set; }
        public bool Uyari { get; set; }
        public int OdemeSebebiId { get; set; }

        public int Save()
        {
            return new KiraEkstreAktarmaService().Save(this);
        }
        public bool Update()
        {
            return new KiraEkstreAktarmaService().Update(this);
        }
        public bool Delete()
        {
            return new KiraEkstreAktarmaService().Delete(this);
        }
        public T Select<T>(int id)
        {
            return (T)Convert.ChangeType(new KiraEkstreAktarmaService().GetById(id), typeof(T));
        }
        public List<T> SelectAll<T>()
        {
            return (List<T>)Convert.ChangeType(new KiraEkstreAktarmaService().GetAll(), typeof(List<T>));
        }
        public List<KiraEkstreAktarma> SelectKiraciIdByAdi(string adi)
        {
            return new KiraEkstreAktarmaService().GetByKiraciAdi(adi);
        }
        public List<KiraEkstreAktarma> SelectByIdList(string idListStr)
        {
            return new KiraEkstreAktarmaService().GetByIdList(idListStr);
        }
        public List<KiraEkstreAktarma> SelectByIslemNo(string islemNo)
        {
            return new KiraEkstreAktarmaService().GetByIslemNo(islemNo);
        }
        public List<KiraEkstreAktarma> SelectByColumns(string adi, string soyadi, decimal tutar, DateTime odemeTarihi)
        {
            return new KiraEkstreAktarmaService().GetByColumns(adi, soyadi, tutar, odemeTarihi);
        }
        public List<KiraEkstreAktarma> SelectByEkstreIdList(string idListStr, ref int rowCount)
        {
            return new KiraEkstreAktarmaService().GetByEkstreIdList(idListStr, ref rowCount);
        }


        public DataTable SelectYuklenenKayit(ref int rowCount, bool aktarilanlarHaric, bool kiraTeminatDiger)
        {
            return new KiraEkstreAktarmaService().GetUploadedRecords(ref rowCount, aktarilanlarHaric, kiraTeminatDiger);
        }
        public DataTable SelectById(int ekstreAktarmaId)
        {
            return new KiraEkstreAktarmaService().GetByIdDetailed(ekstreAktarmaId);
        }

		public DataTable SelectTarihOdemeSebebi(DateTime tarih, int odemeSebebiId)
		{
			return new KiraEkstreAktarmaService().GetByDateAndPaymentReason(tarih, odemeSebebiId);
		}
		public DataTable SelectSumTutarByTarihOdemeSebebi(DateTime tarih, int odemeSebebiId)
		{
			return new KiraEkstreAktarmaService().GetSumByDateAndPaymentReason(tarih, odemeSebebiId);
		}
    }
}
