using DAO.Repositories.Ortak;
using Model.Ortak;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;

namespace Model.Services.Ortak
{
    public class BolgeService
    {
        private readonly BolgeRepository repository;

        public BolgeService() : this(new BolgeRepository())
        {
        }

        public BolgeService(BolgeRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public Bolge GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public Bolge GetByDonorId(int donorId)
        {
            return Map(repository.SelectByDonorId(donorId));
        }

        public List<Bolge> GetActive(int regionId)
        {
            return new Bolge().ToList<Bolge>(repository.SelectActive(regionId));
        }

        // Bolge.Select(int) previously returned the first row for non-positive IDs.
        public Bolge GetSelected(int regionId)
        {
            return Map(regionId > 0 ? repository.SelectById(regionId) : repository.SelectFirst());
        }

        private static Bolge Map(DataTable table)
        {
            return new Bolge().ToList<Bolge>(table).FirstOrDefault();
        }
    }
}
