using Model.Ortak;
using Model.Services.TBYS;
using System;
using System.Collections.Generic;

namespace Model.TBYS
{
    public class VasiyeteKonuVarlik : EntityBase
    {
        public int VasiyetciId { get; set; }
        public string Konusu { get; set; }
        public string Cinsi { get; set; }
        public string AdetMiktar { get; set; }
        public decimal TahminiRayic { get; set; }
        public string Aciklama { get; set; }

        public int Save()
        {
            return new VasiyeteKonuVarlikService().Save(this);
        }

        public bool Update()
        {
            return new VasiyeteKonuVarlikService().Update(this);
        }

        public bool Delete()
        {
            return new VasiyeteKonuVarlikService().Delete(this);
        }

        public VasiyeteKonuVarlik Select(int id)
        {
            return new VasiyeteKonuVarlikService().GetById(id);
        }

        public T Select<T>(int id)
        {
            return (T)Convert.ChangeType(new VasiyeteKonuVarlikService().GetById(id), typeof(T));
        }

        public List<T> SelectAll<T>()
        {
            return (List<T>)Convert.ChangeType(
                new VasiyeteKonuVarlikService().GetAll(),
                typeof(List<T>));
        }

        public List<VasiyeteKonuVarlik> SelectByVasiyetciId(int vasiyetciId)
        {
            return new VasiyeteKonuVarlikService().GetByVasiyetciId(vasiyetciId);
        }
    }
}
