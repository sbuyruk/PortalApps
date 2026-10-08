using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;
using Model.Services.Ortak;

namespace Model.Ortak
{

    public class Ilce : EntityBase
    {
        public int IlId { get; set; }
        public string IlAdi { get; set; }
        public int IlceId { get; set; }
        public string IlceAdi { get; set; }
        public bool Delete()
        {
            throw new NotImplementedException();
        }
        public int Save()
        {
            throw new NotImplementedException();
        }
        public T Select<T>(int id)
        {
            return (T)Convert.ChangeType(new IlceService().GetById(id), typeof(T));
        }
        public List<T> SelectAll<T>()
        {
            List<Ilce> list = new IlceService().GetAll();
            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }

        public bool Update()
        {
            throw new NotImplementedException();
        }
        public Ilce()
        {

        }
        public Ilce(int ilceId)
        {
            Ilce ilce = new IlceService().GetById(ilceId);
            if (ilce != null)
            {
                this.IlAdi = ilce.IlAdi;
                this.IlceId = ilce.IlceId;
                this.IlceAdi = ilce.IlceAdi;
            }

        }
        public Ilce SelectByIlceId(int ilceId)
        {
            return new IlceService().GetById(ilceId);
        }

        public DataTable SelectIlceAdiFromILCELER(int ilceId)
        {
            return new IlceService().GetDistrictFromLegacyTable(ilceId);
        }

        public Ilce SelectByIlAndIlceAdi(string ilceAdi, string ilAdi)
        {
            return new IlceService().GetByProvinceAndDistrictContains(ilAdi, ilceAdi);
        }


        public List<Ilce> SelectByIlAdi(string pIlAdi)
        {
            return new IlceService().GetByProvinceName(pIlAdi);
        }

        
        public int SelectCountIlceByBolgeId(int bolgeId)
        {
            return new IlceService().CountByRegion(bolgeId);
        }
        public List<Ilce> SelectByIlId(int pIlId)
        {
            return new IlceService().GetByProvinceId(pIlId);
        }
        public Ilce SelectByIlNameAndIlceName(string ilName, string ilceName)
        {
            return new IlceService().GetByProvinceAndDistrictName(ilName, ilceName);
        }
        public List<Ilce> SelectFTKKuruluOlanIlceler(int ilId)
        {
            return new IlceService().GetWithFTK(ilId);
        }

    }
}

