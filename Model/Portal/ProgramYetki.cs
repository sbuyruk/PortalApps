using Model.Ortak;

namespace Model.Portal
{
    public class ProgramYetki : EntityBase
    {
        public int BirimId { get; set; }
        public string Program { get; set; }
        public string Modul { get; set; }
        public string Kosul { get; set; }
        public bool Deger { get; set; }

    }
}


































