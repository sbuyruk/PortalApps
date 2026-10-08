using DAO.Repositories.Ortak;
using Model.Ortak;
using System;

namespace Model.Services.Ortak
{
    public class SilinenKayitService
    {
        private readonly SilinenKayitRepository repository;

        public SilinenKayitService()
            : this(new SilinenKayitRepository())
        {
        }

        public SilinenKayitService(SilinenKayitRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            this.repository = repository;
        }

        public int Save(SilinenKayit item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            item.OlusturmaTarihi = DateTime.Now;
            item.Id = repository.Insert(item);
            return item.Id;
        }

        public bool Update(SilinenKayit item)
        {
            if (item == null || item.Id == 0)
                return false;

            item.DegistirmeTarihi = DateTime.Now;
            return repository.Update(item);
        }
    }
}
