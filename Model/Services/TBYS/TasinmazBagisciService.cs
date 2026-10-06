using DAO.Repositories.TBYS;
using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.Services.TBYS
{
    public class TasinmazBagisciService
    {
        private readonly TasinmazBagisciRepository repository;
        public TasinmazBagisciService() : this(new TasinmazBagisciRepository()) { }
        public TasinmazBagisciService(TasinmazBagisciRepository repository) { if (repository == null) throw new ArgumentNullException("repository"); this.repository = repository; }
        public TasinmazBagisci GetById(int id) { return Map(repository.SelectById(id)); }
        public List<TasinmazBagisci> GetAll() { return ToList(repository.SelectAll()); }
        public List<TasinmazBagisci> GetAllBySagVefat(string sag) { return ToList(repository.SelectAllBySagVefat(sag == ProjeConstants.HEPSI ? null : sag)); }
        public List<TasinmazBagisci> GetByBolge(int bolgeId) { return ToList(repository.SelectByBolge(bolgeId, bolgeId == ProjeConstants.BOLGE_HEPSI_INT || bolgeId == ProjeConstants.BOLGE_GENELMUDURLUK_INT)); }
        public List<TasinmazBagisci> GetByFilters(bool isSagVefat, bool isCiplakMulkiyet, bool isTCKimlikNoFull, bool isDogumTarihiFull) { return ToList(repository.SelectByFilters(isSagVefat, isCiplakMulkiyet, isTCKimlikNoFull, isDogumTarihiFull, ProjeConstants.BAGISCI_SAG, ProjeConstants.MULKIYETSEKLI_CM)); }
        public List<TasinmazBagisci> GetByIlAdi(string ilAdi) { return ToList(repository.SelectByIlAdi(ilAdi)); }
        public int Save(TasinmazBagisci item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi=DateTime.Now; item.Olusturan=UtilityHelper.GetCurrentUserName(); item.Id=repository.Insert(item); if(item.Id>0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item,ProjeConstants.TBYS,ProjeConstants.TBYS_TASINMAZBAGISCI); return item.Id; }
        public bool Update(TasinmazBagisci item) { if(item==null) throw new ArgumentNullException("item"); TasinmazBagisci old=GetById(item.Id); if(item.Id==0) return false; item.DegistirmeTarihi=DateTime.Now; item.Degistiren=UtilityHelper.GetCurrentUserName(); bool ok=repository.Update(item); if(ok && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item,old,ProjeConstants.TBYS,ProjeConstants.TBYS_TASINMAZBAGISCI); return ok; }
        public bool Delete(TasinmazBagisci item) { if(item==null || item.Id==0) return false; TasinmazBagisci old=GetById(item.Id); if(old==null) return false; bool ok=repository.Delete(item.Id); if(ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old,ProjeConstants.TBYS,ProjeConstants.TBYS_TASINMAZBAGISCI); return ok; }
        private static List<TasinmazBagisci> ToList(DataTable table) { return new TasinmazBagisci().ToList<TasinmazBagisci>(table); }
        private static TasinmazBagisci Map(DataTable table) { return ToList(table).FirstOrDefault(); }
    }
}
