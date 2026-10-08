using Model.Ortak;
using Model.Services.Portal;
using System;
using System.Collections.Generic;

namespace Model.Portal
{
    public class ToplantiKatilim : EntityBase
    {
        public int ToplantiId { get; set; }
        public int KatilimciId { get; set; }
        public bool Bilgi { get; set; }
        public string Aciklama { get; set; }

        public T Select<T>(int id)
        {
            return (T)Convert.ChangeType(new ToplantiKatilimService().GetById(id), typeof(T));
        }

        public ToplantiKatilim Select(int id)
        {
            return new ToplantiKatilimService().GetById(id);
        }

        public ToplantiKatilim Select(int k, int t)
        {
            return new ToplantiKatilimService().GetByParticipantMeeting(k, t);
        }

        public int Save()
        {
            return new ToplantiKatilimService().Save(this);
        }

        public bool Update()
        {
            return new ToplantiKatilimService().Update(this);
        }

        public bool Delete()
        {
            return new ToplantiKatilimService().Delete(this);
        }

        public List<T> SelectAll<T>()
        {
            return (List<T>)Convert.ChangeType(new ToplantiKatilimService().GetAll(), typeof(List<T>));
        }

        public List<ToplantiKatilim> SelectBytoplantiId(int id)
        {
            return new ToplantiKatilimService().GetByMeeting(id);
        }

        public bool DeleteByToplantiId(int id)
        {
            return new ToplantiKatilimService().DeleteByMeeting(id);
        }
    }
}





















































