using Model.Ortak;
using System;
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

    }
}
