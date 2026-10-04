using Model.Ortak;
using Model.Services.TBYS;
using System;
using System.Collections.Generic;

namespace Model.TBYS
{
    [Serializable]
    public class SerhBeyanIrtifak : ParentClass
    {
        public int TasinmazId { get; set; }
        public string SBI { get; set; }
        public string MalikLehtar { get; set; }
        public string TesisKurum { get; set; }
        public DateTime Tarih { get; set; }
        public string TerkinSebebi { get; set; }
        public string Yevmiye { get; set; }
        public string Aciklama { get; set; }

        public override T Select<T>(int id)
        {
            return (T)Convert.ChangeType(new SerhBeyanIrtifakService().GetById(id), typeof(T));
        }

        public override int Save()
        {
            return new SerhBeyanIrtifakService().Save(this);
        }

        public override bool Update()
        {
            return new SerhBeyanIrtifakService().Update(this);
        }

        public override bool Delete()
        {
            return new SerhBeyanIrtifakService().Delete(this);
        }

        public override List<T> SelectAll<T>()
        {
            return (List<T>)Convert.ChangeType(
                new SerhBeyanIrtifakService().GetAll(),
                typeof(List<T>));
        }

        public List<SerhBeyanIrtifak> SelectByTasinmazId(int tasinmazId)
        {
            return new SerhBeyanIrtifakService().GetByTasinmazId(tasinmazId);
        }
    }
}
