using DAO.Repositories.Portal;
using Model.Ortak;
using Model.Portal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Model.Services.Portal
{
    public class DuyuruGosterimService
    {
        private readonly DuyuruGosterimRepository r;

        public DuyuruGosterimService()
            : this(new DuyuruGosterimRepository())
        {
        }

        public DuyuruGosterimService(DuyuruGosterimRepository r)
        {
            if (r == null)
                throw new ArgumentNullException("r");

            this.r = r;
        }

        public DuyuruGosterim GetById(int id)
        {
            return Map(r.SelectById(id));
        }

        public List<DuyuruGosterim> GetAll()
        {
            return List(r.SelectAll());
        }

        public DuyuruGosterim GetByDuyuruId(int id)
        {
            return Map(r.SelectByDuyuruId(id));
        }

        public int Save(DuyuruGosterim x)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            x.OlusturmaTarihi = DateTime.Now;
            x.Id = r.Insert(x);
            return x.Id;
        }

        public bool Update(DuyuruGosterim x)
        {
            if (x == null || x.Id == 0)
                return false;

            x.DegistirmeTarihi = DateTime.Now;
            return r.Update(x);
        }

        public bool Delete(DuyuruGosterim x)
        {
            return x != null && x.Id != 0 && r.Delete(x.Id);
        }

        public void SaveDuyuru(DuyuruGosterim x, Duyuru d)
        {
            x.DuyuruId = d.Id;
            x.GosterildigiTarih = DateTime.Now;
            x.Baslik = d.Baslik;
            x.Metin = d.Metin;
            x.YayinBasTar = d.YayinBasTar;
            x.YayinBitTar = d.YayinBitTar;
            x.DuyuruAlicilari = d.DuyuruAlicilari;
            x.Resim = d.Resim;
            x.Tekrar = d.Tekrar;
            x.Popup = d.Popup;
            x.Aktif = d.Aktif;
            x.Aciklama = d.Aciklama;
            x.Olusturan = string.IsNullOrEmpty(d.Degistiren) ? d.Olusturan : d.Degistiren;
            Save(x);
        }

        private static List<DuyuruGosterim> List(DataTable t)
        {
            return new DuyuruGosterim().ToList<DuyuruGosterim>(t);
        }

        private static DuyuruGosterim Map(DataTable t)
        {
            return List(t).FirstOrDefault();
        }
    }
}




















































n
