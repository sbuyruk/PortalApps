using DAO.Repositories.TBYS;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Model.Services.TBYS
{
    public class TasinmazService
    {
        private readonly TasinmazRepository repository;

        public TasinmazService() : this(new TasinmazRepository()) { }

        public TasinmazService(TasinmazRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public Tasinmaz GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public Tasinmaz GetInventoryById(int id)
        {
            return Map(repository.SelectInventoryById(id));
        }

        public List<Tasinmaz> GetInventory()
        {
            return new Tasinmaz().ToList<Tasinmaz>(repository.SelectInventory());
        }

        private static Tasinmaz Map(DataTable table)
        {
            return new Tasinmaz().ToList<Tasinmaz>(table).FirstOrDefault();
        }
    }
}
