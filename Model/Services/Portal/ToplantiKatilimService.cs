using Utility.Logging;
using DAO.Repositories.Portal;
using Model.Ortak;
using Model.Portal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Model.Services.Portal
{
    public class ToplantiKatilimService
    {
        public ToplantiKatilimService(PortalLogContext context) : this(new ToplantiKatilimRepository(context)) { }
        private readonly ToplantiKatilimRepository r;

        public ToplantiKatilimService()
            : this(new ToplantiKatilimRepository())
        {
        }

        public ToplantiKatilimService(ToplantiKatilimRepository r)
        {
            if (r == null)
                throw new ArgumentNullException("r");

            this.r = r;
        }

        public ToplantiKatilim GetById(int id)
        {
            return Map(r.SelectById(id));
        }

        public List<ToplantiKatilim> GetAll()
        {
            return List(r.SelectAll());
        }

        public ToplantiKatilim GetByParticipantMeeting(int k, int t)
        {
            return Map(r.SelectByParticipantMeeting(k, t));
        }

        public List<ToplantiKatilim> GetByMeeting(int t)
        {
            return List(r.SelectByMeeting(t));
        }

        private static List<ToplantiKatilim> List(DataTable t)
        {
            return new ToplantiKatilim().ToList<ToplantiKatilim>(t);
        }

        private static ToplantiKatilim Map(DataTable t)
        {
            return List(t).FirstOrDefault();
        }
    }
}
