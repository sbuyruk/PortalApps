using DAO.Repositories.Portal;
using Model.Ortak;
using System;
using System.Collections.Generic;
using Utility.Logging;

namespace Model.Services.Portal
{
    public sealed class PortalEventArchiveEntry : EntityBase
    {
        public string Program { get; set; }
        public string IslemTipi { get; set; }
        public string IslemKonusu { get; set; }
        public DateTime IslemTarihi { get; set; }
        public string IslemYapan { get; set; }
        public string Aciklama { get; set; }
    }

    public sealed class PortalEventArchiveService
    {
        private readonly PortalEventArchiveRepository repository;

        public PortalEventArchiveService(PortalLogContext context)
        {
            repository = new PortalEventArchiveRepository(context);
        }

        public List<PortalEventArchiveEntry> GetDay(DateTime day, string program)
        {
            return new PortalEventArchiveEntry().ToList<PortalEventArchiveEntry>(repository.SelectDay(day, program));
        }
    }
}
