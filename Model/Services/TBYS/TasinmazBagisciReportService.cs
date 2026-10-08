using DAO.Repositories.TBYS;
using System.Data;
using Utility.ProjeGlobal;

namespace Model.Services.TBYS
{
    public class TasinmazBagisciReportService
    {
        private readonly TasinmazBagisciReportRepository repository;

        public TasinmazBagisciReportService()
            : this(new TasinmazBagisciReportRepository())
        {
        }

        public TasinmazBagisciReportService(TasinmazBagisciReportRepository repository)
        {
            this.repository = repository;
        }

        public DataTable GetAllCountBagisAdedi(bool excludeDeceased, bool excludeHidden)
        {
            return repository.SelectAllCountBagisAdedi(excludeDeceased, excludeHidden, ProjeConstants.BAGISCI_SAG);
        }

        public DataTable GetAllCountBagisAdediByBolge(int bolgeId)
        {
            return repository.SelectAllCountBagisAdediByBolge(
                bolgeId,
                ProjeConstants.BOLGE_HEPSI_INT,
                ProjeConstants.BOLGE_GENELMUDURLUK_INT);
        }

        public DataTable GetTasinmazBagisci(bool excludeHidden)
        {
            return repository.SelectTasinmazBagisci(excludeHidden, ProjeConstants.EDINMESEKLI_BAGIS);
        }

        public DataTable GetUnselectedParticipants()
        {
            return repository.SelectUnselectedParticipants();
        }

        public DataTable GetDeprecatedByBolge(string bolge)
        {
            return repository.SelectDeprecatedByBolge(
                bolge,
                ProjeConstants.BOLGE_HEPSI,
                ProjeConstants.TBYS_YETKILI_BIRIM);
        }
    }
}
