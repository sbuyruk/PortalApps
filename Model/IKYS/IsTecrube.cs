using Model.Ortak;
using Model.Services.IKYS;
using System;
using System.Collections.Generic;

namespace Model.IKYS
{
    public class IsTecrube : ParentClass
    {
        public int PersonelId { get; set; }
        public string Isyeri { get; set; }
        public string Gorevi { get; set; }
        public DateTime BasTar { get; set; }
        public DateTime BitTar { get; set; }
        public string Adres { get; set; }

        public override T Select<T>(int id) { return (T)Convert.ChangeType(new IsTecrubeService().GetById(id), typeof(T)); }
        public override int Save() { return new IsTecrubeService().Save(this); }
        public override bool Update() { return new IsTecrubeService().Update(this); }
        public override bool Delete() { return new IsTecrubeService().Delete(this); }
        public override List<T> SelectAll<T>() { return (List<T>)Convert.ChangeType(new IsTecrubeService().GetAll(), typeof(List<T>)); }
        public List<IsTecrube> SelectByPersonelId(int personelId) { return new IsTecrubeService().GetByPersonelId(personelId); }
    }
}
