using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;
using Model.Services.Ortak;

namespace Model.Ortak
{
    public class Bolge : ParentClass
    {
        public string Adi { get; set; }
        public string KisaAdi { get; set; }

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
            return (T)Convert.ChangeType(new BolgeService().GetById(id), typeof(T));
        }
        public override List<T> SelectAll<T>()
        {
            List<Bolge> list = new BolgeService().GetAll();
            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public List<Bolge> SelectAktifBolgeler(int bolgeId)
        {
            return new BolgeService().GetActive(bolgeId);
        }
        public Bolge SelectByBagisciId( int nakitBagisciId)
        {
            return new BolgeService().GetByDonorId(nakitBagisciId);
        }
        public Bolge Select(int bolgeId)
        {
            return new BolgeService().GetSelected(bolgeId);
        }
    }
}
