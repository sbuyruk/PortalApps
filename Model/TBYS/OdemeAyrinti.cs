using Model.Ortak;
using Model.Services.TBYS;
using System;
using System.Collections.Generic;

namespace Model.TBYS
{
    [Serializable]
    public class OdemeAyrinti : ParentClass
    {
        public int KiraciId { get; set; }
        public int SozlesmeId { get; set; }
        public int OdemePlaniId { get; set; }
        public int OdemeId { get; set; }
        public int GecikmeZammiId { get; set; }
        public int OdemePlaniSirasi { get; set; }
        public DateTime SonOdemeTarihi { get; set; }
        public DateTime GecikmeZammiDegisimTar { get; set; }
        public DateTime OdemeTarihi { get; set; }
        public DateTime IlkTarih { get; set; }
        public DateTime SonTarih { get; set; }
        public int AySayisi { get; set; }
        public int GunSayisi { get; set; }
        public decimal GecikmeZammiOrani { get; set; }
        public decimal AnaPara { get; set; }
        public decimal OdenenTutar { get; set; }
        public decimal KalanAnaPara { get; set; }
        public decimal GecikmeZammiTutari { get; set; }
        public string Aciklama { get; set; }
        public override T Select<T>(int id) { return (T)Convert.ChangeType(new OdemeAyrintiService().GetById(id), typeof(T)); }
        public OdemeAyrinti Select(int id) { return new OdemeAyrintiService().GetById(id); }
        public override int Save() { return new OdemeAyrintiService().Save(this); }
        public override bool Update() { return new OdemeAyrintiService().Update(this); }
        public override bool Delete() { return new OdemeAyrintiService().Delete(this); }
        public bool DeleteBySozlesmeId(int id) { return new OdemeAyrintiService().DeleteBySozlesmeId(id); }
        public bool DeleteByOdemeIdOdemePlaniId(int odemeId, int planId) { return new OdemeAyrintiService().DeleteByOdemeIdOdemePlaniId(odemeId, planId); }
        public override List<T> SelectAll<T>() { return (List<T>)Convert.ChangeType(new OdemeAyrintiService().GetAll(), typeof(List<T>)); }
        public List<OdemeAyrinti> SelectBySozlesmeId(int id) { return new OdemeAyrintiService().GetBySozlesmeId(id); }
        public List<OdemeAyrinti> SelectByOdemeIdOdemePlaniId(int odemeId, int planId) { return new OdemeAyrintiService().GetByOdemeIdOdemePlaniId(odemeId, planId); }
        public OdemeAyrinti Select(KiraSozlesme sozlesme, int planId, int delayId, int odemeId) { return new OdemeAyrintiService().GetByPlanAndDelay(sozlesme, planId, delayId, odemeId); }
        public List<OdemeAyrinti> Select(KiraSozlesme sozlesme) { return new OdemeAyrintiService().GetBySozlesme(sozlesme); }
        public List<OdemeAyrinti> Select(OdemePlani plan) { return new OdemeAyrintiService().GetByPlan(plan); }
        public decimal SelectLastAnaParaByOdemePlaniId(int id) { return new OdemeAyrintiService().GetLastAnaPara(id); }
        public decimal SelectSumGecikmeZammiTutariByOdemePlaniId(int id) { return new OdemeAyrintiService().GetSumDelayAmount(id); }
        public decimal SelectSonGecikmeZammiTutariByOdemePlaniId(int id) { return new OdemeAyrintiService().GetLastDelayRate(id); }
    }
}
