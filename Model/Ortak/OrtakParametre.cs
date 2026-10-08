using System;
using System.Collections.Generic;
using System.Data;
using Model.Services.Ortak;

namespace Model.Ortak
{
    [Serializable]
    public class OrtakParametre : EntityBase
    {
        public string Grup { get; set; }
        public string Anahtar { get; set; }
        public string Deger { get; set; }
        public int Sira { get; set; }
        public bool Delete()
        {
            return new OrtakParametreService().Delete(this);
        }
        public int Save()
        {
            return new OrtakParametreService().Save(this);
        }
        public OrtakParametre Select(int id)
        {
            Id = id;
            return new OrtakParametreService().GetById(id);
        }
        public T Select<T>(int id)
        {
            Id = id;
            return (T)Convert.ChangeType(new OrtakParametreService().GetById(id), typeof(T));
        }
        public List<T> SelectAll<T>()
        {
            return (List<T>)Convert.ChangeType(new OrtakParametreService().GetAll(), typeof(List<T>));
        }
        public bool Update()
        {
            return new OrtakParametreService().Update(this);
        }
        public OrtakParametre SelectByAnahtar(string anahtar)
        {
            return new OrtakParametreService().GetByKey(anahtar);
        }
        public List<OrtakParametre> SelectByGrupReturnList(string parametreGrubu)
        {
            return new OrtakParametreService().GetByGroup(parametreGrubu);
        }
        public DataTable SelectAllReturnDT()
        {
            return new OrtakParametreService().GetAllData();
        }
        public string SelectAllReturnJson()
        {
            return new OrtakParametreService().GetAllJson();
        }

        public List<OrtakParametre> SelectByGrupDeger(string grup, string deger)
        {
            return new OrtakParametreService().GetByGroupValue(grup, deger);
        }
    }
}
