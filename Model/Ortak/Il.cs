using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Model.Services.Ortak;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.Ortak
{
    public class Il : ParentClass
    {
        public string IlAdi { get; set; }
        public int PlakaKodu { get; set; }
        public string IngIlAdi { get; set; }
        public string Bolge { get; set; }
        public int BolgeId { get; set; }

        public override int Save()
        {
            throw new NotImplementedException();
        }
        public override bool Update()
        {
            throw new NotImplementedException();
        }
        public override bool Delete()
        {
            throw new NotImplementedException();
        }
        public override T Select<T>(int id)
        {
            return (T)Convert.ChangeType(new IlService().GetById(id), typeof(T));
        }
        public override List<T> SelectAll<T>()
        {
            List<Il> list = new IlService().GetAll();
            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public List<Il> SelectAllOrderByBolge()
        {
            return new IlService().GetAllOrderByRegion();
        }
        public Il SelectByIngAdi(string ingIlAdi)
        {
            return new IlService().GetByEnglishName(ingIlAdi);
        }

        public int SelectCountIlByBolgeId(int bolgeId= ProjeConstants.BOLGE_HEPSI_INT)
        {
            return new IlService().CountByRegion(bolgeId);
        }
        public List<Il> SelectByBolge(int bolgeId = ProjeConstants.BOLGE_HEPSI_INT)
        {
            return new IlService().GetByRegion(bolgeId, false);
        }
        public List<Il> SelectByBolgeId(int bolgeId)
        {
            return new IlService().GetByRegion(bolgeId, true);
        }

        public Il SelectByIlAdi(string ilAdi)
        {
            return new IlService().GetByName(ilAdi);
        }
        /// <summary>
        /// Valilikte veya en az bir ilcede ftk kurulu olan iller
        /// </summary>
        /// <param name="ilId"></param>
        /// <returns></returns>
        public List<Il> SelectFTKKuruluOlanIller()
        {
            return new IlService().GetWithFTK();
        }
        public DataTable SelectFTKKuruluOlanBolgeler()
        {
            return new IlService().GetFTKRegions();
        }
    }
}
