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
    public class Sigorta : EntityBase
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
        public int Save()
        {
            return new SigortaService().Save(this);
        }
        public bool Update()
        {
            return new SigortaService().Update(this);
        }
        public bool Delete()
        {
            return new SigortaService().Delete(this);
        }
    }
}
