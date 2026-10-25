using DAO.Repositories.MTS;
using Model.MTS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.ProjeGlobal;

namespace Model.Services.MTS
{
    public class MTSKurumGorevService
    {
        private readonly MTSKurumGorevRepository repository;
        public MTSKurumGorevService() : this(new MTSKurumGorevRepository())
        {
        }

        public MTSKurumGorevService(Utility.Logging.PortalLogContext context) : this(new MTSKurumGorevRepository(context)) { }

        public MTSKurumGorevService(MTSKurumGorevRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException("repository");
        }

        public MTSKurumGorev GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public List<MTSKurumGorev> GetAll()
        {
            return ToList(repository.SelectAll());
        }
        public string GetByKisiId(int id, ref string kurum, ref string gorev)
        {
            DataTable table = repository.SelectByKisiId(id);
            if (table != null && table.Rows.Count > 0)
            {
                kurum = table.Rows[0]["Kurum"].ToString();
                gorev = table.Rows[0]["Gorev"].ToString();
            }
            return kurum + " " + gorev;
        }

        private static List<MTSKurumGorev> ToList(DataTable table)
        {
            return new MTSKurumGorev().ToList<MTSKurumGorev>(table);
        }

        private static MTSKurumGorev Map(DataTable table)
        {
            return ToList(table).FirstOrDefault();
        }
    }
}
