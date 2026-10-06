using Model.Ortak;
using Model.Services.TBYS;
using System;
using System.Collections.Generic;
using System.Data;

namespace Model.TBYS
{
    [Serializable]
    public class TasinmazBagisci : ParentClass
    {
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public long TCKimlikNo { get; set; }
        public string DogumYeri { get; set; }
        public DateTime DogumTarihi { get; set; }
        public string Ili { get; set; }
        public string Ilcesi { get; set; }
        public int IlId { get; set; }
        public int IlceId { get; set; }
        public string Adres { get; set; }
        public string Telefon1 { get; set; }
        public string Telefon2 { get; set; }
        public string EPosta { get; set; }
        public string Meslegi { get; set; }
        public string SosyalGuvence { get; set; }
        public string Foto { get; set; }
        public string Sag_vefat { get; set; }
        public DateTime VefatTarihi { get; set; }
        public string DefinYeri { get; set; }
        public string DefinIli { get; set; }
        public string DefinIlcesi { get; set; }
        public string DefinAciklama { get; set; }
        public string Aciklama { get; set; }
        public bool Gizli { get; set; }
        public string Tahsil { get; set; }
        private static TasinmazBagisciService Service { get { return new TasinmazBagisciService(); } }
        private static TasinmazBagisciReportService Reports { get { return new TasinmazBagisciReportService(); } }
        public override T Select<T>(int id) { return (T)Convert.ChangeType(Service.GetById(id), typeof(T)); }
        public override int Save() { return Service.Save(this); }
        public override bool Update() { return Service.Update(this); }
        public override bool Delete() { return Service.Delete(this); }
        public override List<T> SelectAll<T>() { return (List<T>)Convert.ChangeType(Service.GetAll(), typeof(List<T>)); }
        public List<TasinmazBagisci> SelectAllSagBagiscilar(string sag) { return Service.GetAllBySagVefat(sag); }
        public List<TasinmazBagisci> SelectByBolge(int bolgeId) { return Service.GetByBolge(bolgeId); }
        public List<TasinmazBagisci> SelectByFilters(bool a, bool b, bool c, bool d) { return Service.GetByFilters(a,b,c,d); }
        public List<TasinmazBagisci> SelectByIlAdi(string ilAdi) { return Service.GetByIlAdi(ilAdi); }
        public DataTable SelectAllCountBagisAdediReturnDataTable(bool a, bool b) { return Reports.GetAllCountBagisAdedi(a,b); }
        public DataTable SelectAllCountBagisAdediReturnDataTable_Deprecated(string bolge) { return Reports.GetDeprecatedByBolge(bolge); }
        public DataTable SelectAllCountBagisAdediReturnDataTable(int bolgeId) { return Reports.GetAllCountBagisAdediByBolge(bolgeId); }
        public DataTable SelectTasinmazBagisciReturnDataTable(bool gizli) { return Reports.GetTasinmazBagisci(gizli); }
        public DataTable SelectSecilmemisKatilimcilarByFaaliyetIdReturnDT() { return Reports.GetUnselectedParticipants(); }
    }
}
