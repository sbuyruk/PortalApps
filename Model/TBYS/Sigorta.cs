using DAO.Ortak;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;
using Model.Services.TBYS;

namespace Model.TBYS
{
    [Serializable]
    public class Sigorta : ParentClass
    {
        public int TasinmazId { get; set; }
        public string SigortaCinsi { get; set; }
        public string AdresKodu { get; set; }
        public string PoliceNo { get; set; }
        public DateTime SigortaBasTar { get; set; }
        public DateTime SigortaBitTar { get; set; }
        public string YapiTarzi { get; set; }
        public string InsaYili { get; set; }
        public string BulunduguKat { get; set; }
        public string ToplamKatSayisi { get; set; }
        public string BBNetAlan { get; set; }
        public string BBBrutAlan { get; set; }
        public decimal SigortaBedeli { get; set; }
        public decimal Prim { get; set; }
        public string DaskPoliceNo { get; set; }
        public int BolumId { get; set; }
        public string TeminatListesi { get; set; }
        public string TeminatAciklama { get; set; }
        public string BagimsizBolumNo { get; set; }
        public string PDFDosyasi { get; set; }
        public string Aciklama { get; set; }
        public string KullanimSekli { get; set; }
        public override T Select<T>(int id)
        {
            return (T)Convert.ChangeType(new SigortaService().GetById(id), typeof(T));

        }
        public override int Save()
        {
            return new SigortaService().Save(this);
        }
        public override bool Update()
        {
            return new SigortaService().Update(this);
        }
        public override bool Delete()
        {
            return new SigortaService().Delete(this);
        }
        public override List<T> SelectAll<T>()
        {
            return (List<T>)Convert.ChangeType(
                new SigortaService().GetAll(),
                typeof(List<T>));
        }
        public List<Sigorta> SelectAllByTasinmazId(int tasinmazId)
        {
            return new SigortaService().GetByTasinmazId(tasinmazId);
        }
        public Sigorta SelectByTasinmazId(int tasinmazId)
        {
            return new SigortaService().GetLatestByTasinmazId(tasinmazId);
        }
        public string SelectAllReturnJson()
        {
            return new SigortaService().GetInventoryListAsJson();
        }

        public DataTable SelectAllReturnDataTable()
        {
            return new SigortaService().GetInventoryList();

        }
        public DataTable SelectByTeminatSigortaCinsiReturnDataTable(string sigortaCinsi, bool vadesiGelenler, bool isDeprem, bool isYangin, bool isMakine100000,
            bool isMakine5000, bool isJenerator, bool isAsansor, bool isKazan, int bolgeId, string auth, DateTime basTarih, DateTime bitTarih)
        {
            return new SigortaService().GetByTeminatSigortaCinsi(sigortaCinsi, vadesiGelenler, isDeprem, isYangin, isMakine100000,
                isMakine5000, isJenerator, isAsansor, isKazan, bolgeId, basTarih, bitTarih);
        }
        public List<Sigorta> SelectBySigortaId(int sigortaId)
        {
            return new SigortaService().GetByIdList(sigortaId);
        }
        public Sigorta SelectNext(int sigortaId)
        {
            return new SigortaService().GetNext(sigortaId);
        }
        public Sigorta SelectPrev(int sigortaId)
        {
            return new SigortaService().GetPrev(sigortaId);
        }
        public Sigorta SelectMax()
        {
            return new SigortaService().GetMax();
        }

        public List<Sigorta> selectByTasinmazId(int tasinmazId)
        {
            return new SigortaService().GetByTasinmazId(tasinmazId);
        }
        public decimal SelectSigortaBedeliToplamiBySigorta(string sigorta)
        {
            return new SigortaService().GetInsuranceValueTotal(sigorta);
        }
        public decimal SelectPirimToplamiBySigorta(string sigorta)
        {
            return new SigortaService().GetPremiumTotal(sigorta);
        }
        public Sigorta SelectMin()
        {
            return new SigortaService().GetMin();
        }
    }
}
