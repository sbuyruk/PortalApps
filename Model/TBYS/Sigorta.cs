using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.TBYS
{
    [Serializable]
    public class Sigorta : ParentClass
    {
        public int TasinmazId { get; set; }
        public string SigortaCinsi { get; set; }
        public string AdresKodu { get; set; }
        public string PoliceNo { get; set; }
        public DateTime SigortaBasTar { get; set; }
        public DateTime SigortaBitTar { get; set; }
        public string YapiTarzi { get; set; }
        public string InsaYili { get; set; }
        public string BulunduguKat { get; set; }
        public string ToplamKatSayisi { get; set; }
        public string Metrekare { get; set; }
        public string BrutYuzolcumu { get; set; }
        public decimal SigortaBedeli { get; set; }
        public decimal Prim { get; set; }
        public string DaskPoliceNo { get; set; }
        public int BolumId { get; set; }
        public string TeminatListesi { get; set; }
        public string TeminatAciklama { get; set; }
        public string BagimsizBolumNo { get; set; }
        public string PDFDosyasi { get; set; }
        public string Aciklama { get; set; }
        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM Sigorta_Table 
                               WHERE  Id={0}", id);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<Sigorta> list = ToList<Sigorta>(dataTable);
            Sigorta sigorta = new Sigorta();
            sigorta = list.FirstOrDefault();
            return (T)Convert.ChangeType(sigorta, typeof(T));

        }
        public override int Save()
        {
            try
            {
                GenericEntity<Sigorta> genericEntity = new GenericEntity<Sigorta>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);

                this.Id = id;
                return id;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public override bool Update()
        {
            bool isSuccess = false;
            try
            {
                if (Id != 0)
                {
                    GenericEntity<Sigorta> genericEntity = new GenericEntity<Sigorta>(ProjeConstants.SQL_UPDATE);
                    DegistirmeTarihi = DateTime.Now;
                    string sqlString = genericEntity.GetQuery(this);
                    isSuccess = dao.Update2Db(sqlString);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return isSuccess;
        }
        public override bool Delete()
        {
            string sqlString = string.Format(@"DELETE 
                               FROM Sigorta_Table
                               WHERE Id={0}", Id);

            bool isSuccess = dao.DeleteFromDb(sqlString, this);

            return isSuccess;
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM Sigorta_Table");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<Sigorta> list = ToList<Sigorta>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public List<Sigorta> SelectAllByTasinmazId(int tasinmazId)
        {
            string sqlString = string.Format(@"
                SELECT *
                    FROM Sigorta_Table A
                WHERE TasinmazId={0}
                ORDER BY TasinmazId", tasinmazId.ReturnQuotedValue());
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            if (dataTable != null)
            {
                List<Sigorta> list = ToList<Sigorta>(dataTable);
                return list;
            }
            else
            {
                return null;
            }
        }
        public Sigorta SelectByTasinmazId(int tasinmazId)
        {
            Sigorta sigorta = null;
            string sqlString = string.Format(@"
                SELECT *
                    FROM Sigorta_Table A
                WHERE TasinmazId={0}
                ORDER BY SigortaBitTar DESC ", tasinmazId.ReturnQuotedValue());
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            if (dataTable != null)
            {
                List<Sigorta> list = ToList<Sigorta>(dataTable);
                sigorta = list.FirstOrDefault<Sigorta>();
            }
            return sigorta;

        }
        public string SelectAllReturnJson()
        {
            string sqlString = SelectAllString();
            DataTable dataTable = null;
            try
            {
                dataTable = dao.selectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                throw e;
            }
            string json = ToJSON(dataTable);
            return json;
        }

        public DataTable SelectAllReturnDataTable()
        {
            string sqlString = SelectAllString();
            DataTable dataTable = null;
            try
            {
                dataTable = dao.selectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                throw e;
            }
            return dataTable;

        }
        private string SelectAllString()
        {
            string sqlString = string.Format(@"
                SELECT A.Id SigortaId, B.SorumluBolge, A.TasinmazId,B.SorumluBolge,A.SigortaCinsi,A.AdresKodu,A.PoliceNo,A.SigortaBasTar,A.SigortaBitTar,A.YapiTarzi,A.InsaYili,
                    A.BulunduguKat,A.ToplamKatSayisi, A.Metrekare, A.BrutYuzolcumu, A.SigortaBedeli, A.Prim,A.DaskPoliceNo,
                    B.Adres+ISNULL(C.BolumNo,'') +' '+ B.Ilcesi +'-'+ B.Ili Adres, B.Ili,B.Ilcesi, B.Ilcesi +' '+ B.Ili IliIlcesi, 
                    B.KullanimSekli, B.Cinsi, B.PaftaNo,B.AdaNo,B.ParselNo,B.SahifeNo,C.BolumNo,
                    A.TeminatListesi,A.TeminatAciklama,A.Aciklama
                FROM Sigorta_Table A
                INNER JOIN Tasinmaz_Table B ON B.Id=A.TasinmazId AND B.EnvanterdeMi=1
                LEFT JOIN BagimsizBolum_Table C ON C.Id=A.BolumId
                ORDER BY B.SorumluBolge, B.Ili,B.Ilcesi, A.Id, SigortaBasTar DESC
                            ");
            return sqlString;
        }
        public string SelectByTeminatSigortaCinsiReturnJson(string sigortaCinsi, bool vadesiGelenler,bool isDeprem, bool isYangin, bool isMakine100000, bool isMakine5000, bool isJenerator, bool isAsansor, bool isKazan)
        {
            string sqlString = SelectByTeminatSigortaCinsiSQL(sigortaCinsi, vadesiGelenler,isDeprem, isYangin, isMakine100000, isMakine5000, isJenerator, isAsansor, isKazan);
            DataTable dataTable = null;
            try
            {
                dataTable = dao.selectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                throw e;
            }
            string json = ToJSON(dataTable);
            return json;
        }
        public DataTable SelectByTeminatSigortaCinsiReturnDataTable(string sigortaCinsi, bool vadesiGelenler, bool isDeprem, bool isYangin, bool isMakine100000, bool isMakine5000, bool isJenerator, bool isAsansor, bool isKazan)
        {
            string sqlString = SelectByTeminatSigortaCinsiSQL(sigortaCinsi, vadesiGelenler, isDeprem, isYangin, isMakine100000, isMakine5000, isJenerator, isAsansor, isKazan);
            DataTable dataTable = null;
            try
            {
                dataTable = dao.selectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                throw e;
            }
            return dataTable;

        }
        private string SelectByTeminatSigortaCinsiSQL(string sigortaCinsi, bool vadesiGelenler, bool isDeprem, bool isYangin, bool isMakine100000, bool isMakine5000, bool isJenerator, bool isAsansor, bool isKazan)
        {
            string sigortaCinsiStr = string.IsNullOrEmpty(sigortaCinsi) || sigortaCinsi.Equals(ProjeConstants.HEPSI) ? " SigortaCinsi is not null " : " SigortaCinsi = " + sigortaCinsi.ReturnQuotedValue();
            string depremStr = isDeprem ? string.Format(" TeminatListesi Like '%{0}%'", "1") : "";
            string yanginStr = isYangin ? string.Format(" TeminatListesi Like '%{0}%'", "2") : "";
            string makine100000Str = isMakine100000 ? string.Format(" TeminatListesi Like '%{0}%'", "3") : "";
            string makine5000Str = isMakine5000 ? string.Format(" TeminatListesi Like '%{0}%'", "4") : "";
            string jeneratorStr = isJenerator ? string.Format(" TeminatListesi Like '%{0}%'", "5") : "";
            string asansorStr = isAsansor ? string.Format(" TeminatListesi Like '%{0}%'", "6") : "";
            string kazanStr = isKazan ? string.Format(" TeminatListesi Like '%{0}%'", "7") : "";

            string andStr = isDeprem || isYangin || isMakine100000 || isMakine5000 || isJenerator || isAsansor || isKazan ? " AND ( " : "";
            string teminatStr = depremStr;
            teminatStr += (!string.IsNullOrEmpty(teminatStr) && !string.IsNullOrEmpty(yanginStr) ? " OR " + yanginStr : yanginStr);
            teminatStr += (!string.IsNullOrEmpty(teminatStr) && !string.IsNullOrEmpty(makine100000Str) ? " OR " + makine100000Str : makine100000Str);
            teminatStr += (!string.IsNullOrEmpty(teminatStr) && !string.IsNullOrEmpty(makine5000Str) ? " OR " + makine5000Str : makine5000Str);
            teminatStr += (!string.IsNullOrEmpty(teminatStr) && !string.IsNullOrEmpty(jeneratorStr) ? " OR " + jeneratorStr : jeneratorStr);
            teminatStr += (!string.IsNullOrEmpty(teminatStr) && !string.IsNullOrEmpty(asansorStr) ? " OR " + asansorStr : asansorStr);
            teminatStr += (!string.IsNullOrEmpty(teminatStr) && !string.IsNullOrEmpty(kazanStr) ? " OR " + kazanStr : kazanStr);

            teminatStr = andStr + teminatStr + (isDeprem || isYangin || isMakine100000 || isMakine5000 || isJenerator || isAsansor || isKazan ? " ) " : "");

          
                string vadeStr = vadesiGelenler?string.Format(" AND SigortaBitTar <{0}", DateTime.Today.AddMonths(1).ReturnTRDateFormat()):string.Empty;
            
            string sqlString = string.Format(@"
                SELECT A.Id SigortaId, B.SorumluBolge, A.TasinmazId,B.SorumluBolge,A.SigortaCinsi,A.AdresKodu,A.PoliceNo,A.SigortaBasTar,A.SigortaBitTar,A.YapiTarzi,A.InsaYili,
                    A.BulunduguKat,A.ToplamKatSayisi, A.Metrekare ,A.BrutYuzolcumu, A.SigortaBedeli, A.Prim,A.DaskPoliceNo,A.BagimsizBolumNo,
                    B.Adres+ISNULL(C.BolumNo,'') Adres, B.Ili,B.Ilcesi, B.Ilcesi +' '+ B.Ili IliIlcesi, B.KullanimSekli, B.Cinsi, B.PaftaNo,B.AdaNo,B.ParselNo,B.SahifeNo,C.BolumNo,
                    A.TeminatListesi,A.TeminatAciklama,A.Aciklama,B.EnvanterdeMi,
                    B.Adres+ISNULL(C.BolumNo,'') +' '+ B.Ilcesi+'-'+ B.Ili TamAdres 
                FROM Sigorta_Table A
                    INNER JOIN Tasinmaz_Table B ON B.Id=A.TasinmazId AND (B.EnvanterdeMi=1 OR B.EnvanterdeMi=2) 
                    LEFT JOIN BagimsizBolum_Table C ON C.Id=A.BolumId AND C.TasinmazId=B.Id
                WHERE 
                    {0}
                    {1} 
                    {2}
                ORDER BY B.SorumluBolge, B.Ili,B.Ilcesi, A.Id, SigortaBasTar DESC
                            ", sigortaCinsiStr, teminatStr,vadeStr);
            return sqlString;
        }
        public List<Sigorta> SelectBySigortaId(int sigortaId)
        {
            string sqlString = string.Format(@"SELECT * FROM Sigorta_Table
                              WHERE Id={0}", sigortaId.ReturnQuotedValue());
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<Sigorta> list = ToList<Sigorta>(dataTable);
            return list;
        }
        public Sigorta SelectNext(int sigortaId)
        {
            Sigorta sigorta = new Sigorta();
            string sqlString = string.Format(@"
                SELECT * FROM Sigorta_Table A
                INNER JOIN Tasinmaz_Table B ON B.Id=A.TasinmazId AND B.EnvanterdeMi=1
                --WHERE A.Id > {0}
                ORDER BY B.SorumluBolge, B.Ili,B.Ilcesi, A.Id, SigortaBasTar DESC ", sigortaId.ReturnQuotedValue());
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            if (dataTable != null)
            {
                List<Sigorta> list = ToList<Sigorta>(dataTable);
                int index = list.FindIndex(s => s.Id == sigortaId);
                if (index < list.Count - 1)
                {
                    sigorta = list[index + 1];
                }
                else
                {
                    sigorta = list[list.Count - 1];
                }

            }

            return sigorta;
        }
        public Sigorta SelectPrev(int sigortaId)
        {
            Sigorta sigorta = new Sigorta();
            string sqlString = string.Format(@"
                SELECT * FROM Sigorta_Table A
                INNER JOIN Tasinmaz_Table B ON B.Id=A.TasinmazId AND B.EnvanterdeMi=1
                --WHERE A.Id < {0}
                ORDER BY B.SorumluBolge, B.Ili,B.Ilcesi, A.Id, SigortaBasTar DESC ", sigortaId.ReturnQuotedValue());
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            if (dataTable != null)
            {
                List<Sigorta> list = ToList<Sigorta>(dataTable);
                int index = list.FindIndex(s => s.Id == sigortaId);
                if (index > 0)
                {
                    sigorta = list[index - 1];
                }
                else
                {
                    sigorta = list[0];
                }

            }
            return sigorta;
        }
        public Sigorta SelectMax()
        {
            string sqlString = string.Format(@"SELECT MAX(Id) Id  FROM Sigorta_Table ");
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            if (dataTable != null)
            {
                DataRow row = dataTable.Rows[0];
                int sigortaId = row["Id"].ConvertToInt();
                Sigorta sigorta = new Sigorta();
                sigorta = sigorta.Select<Sigorta>(sigortaId);
                return sigorta;
            }
            else
            {
                return null;
            }
        }

        public List<Sigorta> selectByTasinmazId(int tasinmazId)
        {
            string sqlString = string.Format(@"
                SELECT A.Id SigortaId, A.Id Id, A.TasinmazId,A.SigortaCinsi,A.AdresKodu,A.PoliceNo,A.SigortaBasTar,A.SigortaBitTar,A.YapiTarzi,A.InsaYili,
                    A.BulunduguKat,A.ToplamKatSayisi, A.Metrekare ,A.BrutYuzolcumu,
                    A.SigortaBedeli SigortaBedeli, A.Prim Prim,A.DaskPoliceNo
                FROM Sigorta_Table A
                INNER JOIN Tasinmaz_Table B ON B.Id=A.TasinmazId
                WHERE A.TasinmazId={0}", tasinmazId.ReturnQuotedValue());
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<Sigorta> list = ToList<Sigorta>(dataTable);
            return list;
        }
        public decimal SelectSigortaBedeliToplamiBySigorta(string sigorta)
        {
            decimal toplam = 0;
            string sqlString = string.Format(@"
                SELECT SUM(S.SigortaBedeli) Toplam
                FROM Sigorta_Table S
					INNER JOIN Tasinmaz_Table T on T.Id=S.TasinmazId 
                WHERE T.EnvanterdeMi=1 AND SigortaCinsi= {0}", sigorta.ReturnQuotedValue());
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            if (dataTable != null)
            {
                DataRow row = dataTable.Rows[0];
                toplam = row["Toplam"].ConvertToDecimal();

            }
            return toplam;
        }
        public decimal SelectPirimToplamiBySigorta(string sigorta)
        {
            decimal toplam = 0;
            string sqlString = string.Format(@"
                SELECT SUM(S.Prim) Toplam
                FROM Sigorta_Table S
					INNER JOIN Tasinmaz_Table T on T.Id=S.TasinmazId 
                WHERE T.EnvanterdeMi=1 AND SigortaCinsi= {0}", sigorta.ReturnQuotedValue());
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            if (dataTable != null)
            {
                DataRow row = dataTable.Rows[0];
                toplam = row["Toplam"].ConvertToDecimal();

            }
            return toplam;
        }
        public Sigorta SelectMin()
        {
            string sqlString = string.Format(@"SELECT MIN(Id) Id  FROM Sigorta_Table ");
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            if (dataTable != null)
            {
                DataRow row = dataTable.Rows[0];
                int sigortaId = row["Id"].ConvertToInt();
                Sigorta sigorta = new Sigorta();
                sigorta = sigorta.Select<Sigorta>(sigortaId);
                return sigorta;
            }
            else
            {
                return null;
            }
        }
    }
}
