using DAO.Repositories.TBYS;
using Model.TBYS;
using System;
using System.Data;
using Utility.ProjeGlobal;

namespace Model.Services.TBYS
{
    public class OdemePlaniRaporService
    {
        private readonly OdemePlaniRaporRepository repository;
        public OdemePlaniRaporService() : this(new OdemePlaniRaporRepository()) { }
        public OdemePlaniRaporService(OdemePlaniRaporRepository repository) { if (repository == null) throw new ArgumentNullException("repository"); this.repository = repository; }
        public DataTable GetBorcluByBolgeTarih(int bolgeId, DateTime ilkTarih, DateTime sonTarih, int ayBas, int ayBit) { return repository.SelectBorcluByBolgeTarih(bolgeId, ilkTarih, sonTarih, ayBas, ayBit, ProjeConstants.HEPSI_INT, ProjeConstants.BOLGE_GENELMUDURLUK_INT); }
        public string GetBorcluByBolgeTarihJson(int bolgeId, DateTime ilkTarih, DateTime sonTarih, int ayBas, int ayBit, ref int rowCount) { DataTable t = GetBorcluByBolgeTarih(bolgeId, ilkTarih, sonTarih, ayBas, ayBit); rowCount = t == null ? 0 : t.Rows.Count; return new OdemePlani().ToJSON(t); }
        public DataTable GetCurrentByDate(DateTime ilkTarih, DateTime sonTarih, string bolge) { return repository.SelectCurrentByDate(ilkTarih, sonTarih, bolge, ProjeConstants.HEPSI); }
        public DataTable GetIncomeByRegionMonth(int bolgeId, int ay, int yil) { return repository.SelectIncomeByRegionMonth(bolgeId, ay, yil, ProjeConstants.HEPSI_INT, ProjeConstants.BOLGE_GENELMUDURLUK_INT); }
        public DataTable GetListByDate(DateTime tarih) { return repository.SelectListByDate(tarih); }
    }
}
