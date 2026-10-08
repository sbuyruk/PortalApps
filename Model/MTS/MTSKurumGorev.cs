using Model.Ortak;
using Model.Services.MTS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Utility.ProjeGlobal;

namespace Model.MTS
{
    public class MTSKurumGorev : EntityBase
    {
        [Required]
        public int MTSKurumTanimId { get; set; }
        public int MTSGorevTanimId { get; set; }
        public int KisiId { get; set; }

        [DisplayName("Görev Durumu")]
        [Required(ErrorMessage = "Görev Durumu boş olamaz.")]
        public string Durum { get; set; } = ProjeConstants.MTSGOREVDURUMU_GOREVDE;

        [DisplayName("Başlama Tarihi")]
        public DateTime BaslamaTarihi { get; set; }
        [DisplayName("Ayrılma Tarihi")]
        public DateTime? AyrilmaTarihi { get; set; }
        public string AyrilmaSebebi { get; set; } = ProjeConstants.MTSAYRILMASEBEBI_BOS;
        public string KisaAdi { get; set; }

        public int Save()
        {
            return new MTSKurumGorevService().Save(this);
        }

        public bool Update()
        {
            return new MTSKurumGorevService().Update(this);
        }

        public bool Delete()
        {
            return new MTSKurumGorevService().Delete(this);
        }

        public MTSKurumGorev Select(int id)
        {
            Id = id;
            return new MTSKurumGorevService().GetById(id);
        }

        public T Select<T>(int id)
        {
            Id = id;
            return (T)Convert.ChangeType(new MTSKurumGorevService().GetById(id), typeof(T));
        }

        public List<T> SelectAll<T>()
        {
            return (List<T>)Convert.ChangeType(new MTSKurumGorevService().GetAll(), typeof(List<T>));
        }

        public string SelectByKisiIdReturnKurumGorev(int kisiId, ref string kurum, ref string gorev)
        {
            return new MTSKurumGorevService().GetByKisiId(kisiId, ref kurum, ref gorev);
        }
    }
}
