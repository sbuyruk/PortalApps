using DAO.Repositories.NBYS;
using Utility.Logging;
using Model.NBYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Model.Services.NBYS
{
    public class DuzenliNakitBagisciService
    {
        private readonly DuzenliNakitBagisciRepository repository;

        public DuzenliNakitBagisciService(PortalLogContext context) : this(new DuzenliNakitBagisciRepository(context))
        {
        }

        public DuzenliNakitBagisciService() : this(new DuzenliNakitBagisciRepository())
        {
        }

        public DuzenliNakitBagisciService(DuzenliNakitBagisciRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public DuzenliNakitBagisci GetById(int id)
        {
            return Map(repository.SelectById(id)).FirstOrDefault();
        }

        public List<DuzenliNakitBagisci> GetAll()
        {
            return Map(repository.SelectAll());
        }

        public DuzenliNakitBagisci GetActiveByBagisciId(int bagisciId)
        {
            if (bagisciId < 1)
                return null;
            return Map(repository.SelectActiveByBagisciId(bagisciId)).FirstOrDefault();
        }

        private static List<DuzenliNakitBagisci> Map(DataTable table)
        {
            return new DuzenliNakitBagisci().ToList<DuzenliNakitBagisci>(table);
        }
    }
}
