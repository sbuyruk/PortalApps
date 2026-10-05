using Model.Ortak;
using Model.Services.IKYS;
using System;
using System.Collections.Generic;
using System.Data;

namespace Model.IKYS
{
    public class IsBilgileri : ParentClass
    {
        public int PersonelId { get; set; }
        public int UnvanId { get; set; }
        public int GorevId { get; set; }
        public int BirimId { get; set; }
        public DateTime BaslamaTar { get; set; }
        public DateTime IzinDonemiBasTar { get; set; }
        public int CalismaDurumu { get; set; }
        public DateTime AyrilmaTar { get; set; }
        public string AyrilmaSebebi { get; set; }
        public int ProtokolSiraNo { get; set; }
        public string SGKSicilNo { get; set; }
        public DateTime SGKBasTar { get; set; }
        public int VakifOncesiPrimGunSayisi { get; set; }
        public DateTime EmeklilikTarihi { get; set; }
        public string Aciklama { get; set; }

        public override T Select<T>(int id) { return (T)Convert.ChangeType(new IsBilgileriService().GetById(id), typeof(T)); }
        public override int Save() { return new IsBilgileriService().Save(this); }
        public override bool Update() { return new IsBilgileriService().Update(this); }
        public override bool Delete() { return new IsBilgileriService().Delete(this); }
        public override List<T> SelectAll<T>() { return (List<T>)Convert.ChangeType(new IsBilgileriService().GetAll(), typeof(List<T>)); }
        public IsBilgileri SelectByPersonelId(int personelId) { return new IsBilgileriService().GetByPersonelId(personelId); }
        public IsBilgileri SelectByGorevId(int gorevId) { return new IsBilgileriService().GetByGorevId(gorevId); }
        public DataTable SelectAllFromIS_YERI_BILGILERI() { return new IsBilgileriService().GetAllFromIsYeriBilgileri(); }
    }
}
