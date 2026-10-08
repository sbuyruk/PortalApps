using DAO.Repositories.Portal;
using Model.Ortak;
using Model.Portal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.Services.Portal
{
    public class DuyuruService
    {
        private readonly DuyuruRepository repository;
        public DuyuruService() : this(new DuyuruRepository()) { }
        public DuyuruService(DuyuruRepository repository) { if (repository == null) throw new ArgumentNullException("repository"); this.repository = repository; }
        public Duyuru GetById(int id) { return Map(repository.SelectById(id)); }
        public List<Duyuru> GetAll() { return ToList(repository.SelectAll()); }
        public List<Duyuru> GetByDate(DateTime now) { return ToList(repository.SelectByDate(now)); }
        public List<Duyuru> GetByRepeat(DateTime now, string tekrar) { return string.IsNullOrWhiteSpace(tekrar) ? new List<Duyuru>() : ToList(repository.SelectByRepeat(now, tekrar.Trim())); }
        public List<Duyuru> GetAnnouncementList() { return ToList(repository.SelectAnnouncementList()); }
        public int Save(Duyuru item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Id = repository.Insert(item); return item.Id; }
        public bool Update(Duyuru item) { if (item == null || item.Id == 0) return false; item.DegistirmeTarihi = DateTime.Now; return repository.Update(item); }
        public bool Delete(Duyuru item) { return item != null && item.Id != 0 && repository.Delete(item.Id); }
        private static List<Duyuru> ToList(DataTable table) { return new Duyuru().ToList<Duyuru>(table); }
        private static Duyuru Map(DataTable table) { return ToList(table).FirstOrDefault(); }
    }
}
