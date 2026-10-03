using DAO.Repositories.IKYS;
using Model.IKYS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Model.Services.IKYS
{
    public class YoklamaService
    {
        private readonly YoklamaRepository repository;

        public YoklamaService() : this(new YoklamaRepository()) { }

        public YoklamaService(YoklamaRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public Yoklama GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public List<Yoklama> GetAll()
        {
            return new Yoklama().ToList<Yoklama>(repository.SelectAll());
        }

        public List<Yoklama> GetByPersonelIdAndDate(int personelId, DateTime start, DateTime end)
        {
            return new Yoklama().ToList<Yoklama>(
                repository.SelectByPersonelIdAndDate(personelId, start, end));
        }

        public DataTable GetAllByPersonelId(int personelId)
        {
            return repository.SelectAllByPersonelId(personelId);
        }

        public string GetAllByPersonelIdAsJson(int personelId)
        {
            return new Yoklama().ToJSON(GetAllByPersonelId(personelId));
        }

        private static Yoklama Map(DataTable table)
        {
            return new Yoklama().ToList<Yoklama>(table).FirstOrDefault();
        }
    }
}
