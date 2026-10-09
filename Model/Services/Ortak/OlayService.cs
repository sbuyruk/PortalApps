using DAO.Repositories.Ortak;
using DAO.Ortak;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Model.Services.Ortak
{
    public class OlayService
    {
        private readonly OlayRepository repository;

        public OlayService()
            : this(new OlayRepository())
        {
        }

        public OlayService(OlayRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            this.repository = repository;
        }

        public Olay GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public List<Olay> GetAll()
        {
            return ToList(repository.SelectAll());
        }

        public List<Olay> GetByDateAndProgram(DateTime islemTarihi, string program)
        {
            return ToList(repository.SelectByDateAndProgram(islemTarihi, program));
        }

        public List<Olay> GetAuditList()
        {
            return ToList(repository.SelectAuditList());
        }

        public int Save(Olay item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            item.OlusturmaTarihi = DateTime.Now;
            return item.Id = repository.Insert(item);
        }

        public int Save(Olay item, SqlTransactionContext transaction)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            if (transaction == null)
                throw new ArgumentNullException("transaction");

            item.OlusturmaTarihi = DateTime.Now;
            return item.Id = repository.Insert(item, transaction);
        }

        public bool Update(Olay item)
        {
            if (item == null || item.Id == 0)
                return false;

            item.DegistirmeTarihi = DateTime.Now;
            return repository.Update(item);
        }

        public bool Update(Olay item, SqlTransactionContext transaction)
        {
            if (item == null || item.Id == 0)
                return false;
            if (transaction == null)
                throw new ArgumentNullException("transaction");

            item.DegistirmeTarihi = DateTime.Now;
            return repository.Update(item, transaction);
        }

        public bool Delete(Olay item)
        {
            return item != null && item.Id != 0 && repository.Delete(item.Id);
        }

        private static List<Olay> ToList(DataTable table)
        {
            return new Olay().ToList<Olay>(table);
        }

        private static Olay Map(DataTable table)
        {
            return ToList(table).FirstOrDefault();
        }
    }
}
