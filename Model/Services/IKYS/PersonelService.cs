using DAO.Repositories.IKYS;
using Model.IKYS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Model.Services.IKYS
{
    public class PersonelService
    {
        private readonly PersonelRepository repository;

        public PersonelService() : this(new PersonelRepository())
        {
        }

        public PersonelService(PersonelRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public Personel GetById(int id)
        {
            return MapSingle(repository.SelectById(id));
        }

        public List<Personel> GetAll()
        {
            return MapList(repository.SelectAll());
        }

        public Personel GetByUserName(string userName)
        {
            return MapSingle(repository.SelectByUserName(userName));
        }

        public List<Personel> GetActiveEmployees(Personel.PersonelTipi personelTipi)
        {
            return MapList(repository.SelectActiveEmployees((int)personelTipi));
        }

        private static Personel MapSingle(DataTable dataTable)
        {
            return MapList(dataTable).FirstOrDefault();
        }

        private static List<Personel> MapList(DataTable dataTable)
        {
            return new Personel().ToList<Personel>(dataTable);
        }
    }
}
