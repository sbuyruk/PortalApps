using DAO.Ortak;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.NBYS
{
    [Serializable]
    public class Armagan : ParentClass
    {
        public int BagisciId { get; set; }
        public int ArmaganTanimId { get; set; }
        public DateTime Tarih { get; set; }
        public string Durum { get; set; }
        public decimal BagisMiktari { get; set; }
        public string DovizCinsi { get; set; }
        public string Aciklama { get; set; }
        public string BelgedeYazanIsim { get; set; }
        public int BelgeGecersizMi { get; set; }
        public int GecersizNBHareketId { get; set; }
        public string GecersizYapan { get; set; }
        public DateTime GecersizYapmaTarihi { get; set; }
        public decimal ArmaganBagisMiktari { get; set; }
        public decimal IadeMiktari { get; set; }
        public bool BagisMiktariYazmasin { get; set; }
        public bool CokluBagis { get; set; }
        public bool DuzenliBagis { get; set; }= false;
        public int KacinciBelge { get; set; }= 0;
        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM Armagan_Table 
                               WHERE BelgeGecersizMi!=1 AND Id={0}", id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Armagan> list = ToList<Armagan>(dataTable);
            Armagan armagan = new Armagan();
            armagan = list.FirstOrDefault();
            return (T)Convert.ChangeType(armagan, typeof(T));
        }

        public Armagan SelectByBagisciIdAndBagisTarihi(DateTime basTar, DateTime bitTar, int nakitBagisciId)
        {

            string sqlString = string.Format(@"SELECT * from Armagan_Table
                              WHERE BelgeGecersizMi!=1 AND BagisciId={0} and Tarih BETWEEN {1} AND {2}",
                           nakitBagisciId.ReturnQuotedValue(), basTar.ReturnTRDateFormat(), bitTar.ReturnTRDateFormat());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Armagan> list = ToList<Armagan>(dataTable);
            Armagan armagan = new Armagan();
            armagan = list.FirstOrDefault();
            return armagan;
        }
        public override int Save()
        {
            try
            {
                GenericEntity<Armagan> genericEntity = new GenericEntity<Armagan>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                SqlQuery query = genericEntity.GetQueryParametreli(this);
                int id = dao.Insert(query);

                this.Id = id;
                if (id > 0 && ProjeConstants.NBYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.NBYS, ProjeConstants.NBYS_ARMAGAN);
                }
                return id;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public override bool Update()
        {
            bool isSuccess = false;
            try
            {
                if (this != null)
                {
                    Armagan item = Select<Armagan>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<Armagan> genericEntity = new GenericEntity<Armagan>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren=UtilityHelper.GetCurrentUserName();
                        SqlQuery query = genericEntity.GetQueryParametreli(this);
                        isSuccess = dao.Update2Db(query);
                    }
                    if (isSuccess && ProjeConstants.NBYS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.NBYS, ProjeConstants.NBYS_ARMAGAN);
                    }
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
            try
            {
                bool isDeleted = false;
                if (Id != 0)
                {
                    GenericEntity<Armagan> genericEntity = new GenericEntity<Armagan>(ProjeConstants.SQL_DELETE);
                    SqlQuery query = genericEntity.GetQueryParametreli(this);
                    Armagan item = Select<Armagan>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(query, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.NBYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.NBYS, ProjeConstants.NBYS_ARMAGAN);
                    }
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        
        public string GetUpdateSQL(string extId)
        {
            try
            {
                GenericEntity<Armagan> genericEntity = new GenericEntity<Armagan>(ProjeConstants.SQL_UPDATE);
                OlusturmaTarihi = DateTime.Now;
                string sqlString = string.IsNullOrEmpty(extId) ? genericEntity.GetQuery(this) : genericEntity.GetQuery(this, extId);

                return sqlString;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public string GetDeleteSQL(string extId)
        {
            try
            {
                GenericEntity<Armagan> genericEntity = new GenericEntity<Armagan>(ProjeConstants.SQL_DELETE);
                string sqlString = genericEntity.GetQuery(this, extId);

                return sqlString;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public Armagan SelectByBagisciIdBagisTarihi(int nakitBagisciId, int armaganId, DateTime basTar, DateTime bitTar)
        {
            //DateTime ayinIlkGunu = new DateTime(bagisTarihi.Year, bagisTarihi.Month, 1);

            string sqlString = string.Format(@"
                SELECT * FROM Armagan_Table
                WHERE BelgeGecersizMi!=1 AND BagisciId={0} 
                    AND ArmaganTanimId={1} 
                    AND Tarih  BETWEEN {2} AND {3} ",
                           nakitBagisciId.ReturnQuotedValue(), armaganId, basTar.ReturnTRDateFormat(), bitTar.ReturnTRDateFormat());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Armagan> list = ToList<Armagan>(dataTable);
            Armagan armagan = new Armagan();
            armagan = list.FirstOrDefault();
            return armagan;
        }
        public List<Armagan> SelectByBagisciId(int nakitBagisciId)
        {
            string sqlString = string.Format(@"SELECT * from Armagan_Table
                              WHERE BelgeGecersizMi!=1 AND BagisciId={0} ",
                           nakitBagisciId.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Armagan> list = ToList<Armagan>(dataTable);
            Armagan armagan = new Armagan();
            armagan = list.FirstOrDefault();
            return list;
        }
        private string DurumGetir(int nakitBagisciId)
        {
            string durum = string.Empty;
            NakitBagisci nakitBagisci = new NakitBagisci();
            nakitBagisci = nakitBagisci.Select<NakitBagisci>(nakitBagisciId);
            if (nakitBagisci != null)
            {
                if (nakitBagisci.Ulasilamiyor)
                {
                    durum = ProjeConstants.DURUM_ULASILAMADI;
                }
                if (nakitBagisci.BelgeIstemiyor)
                {
                    durum = ProjeConstants.DURUM_BELGE_ISTEMIYOR;
                }
            }
            return durum;
        }
        //public bool SaveOrUpdate(DateTime bagisTarihi, int nakitBagisciId, int nakikbagisHareketId)
        //{
        //    bool isSaved = false;

        //    Armagan armagan = SelectByBagisciIdAndBagisTarihi(bagisTarihi, nakitBagisciId);
        //    int armaganId = 0;
        //    try
        //    {
        //        string durum = DurumGetir(nakitBagisciId);
        //        if (armagan != null)
        //        {
        //            if (!string.IsNullOrEmpty(durum))
        //            {
        //                Durum = durum;
        //            }
        //            Update(); //TODO burasi armagan.Update() olmali veya üstsatirda Durum=durum; olmali
        //            armaganId = armagan.Id;
        //        }
        //        else
        //        {
        //            if (!string.IsNullOrEmpty(durum))
        //            {
        //                Durum = durum;
        //            }
        //            armaganId = Save();
        //        }
        //        isSaved = true;

        //        UpdateArmaganIdAtNakitBagisHareket(nakitBagisciId,bagisTarihi, nakikbagisHareketId, armaganId);

        //    }
        //    catch (Exception)
        //    {
        //        isSaved = false;
        //    }
        //    return isSaved;
        //}
        public int SaveOrUpdate(DateTime bastar, DateTime bittar, DateTime bagisTarihi, int nakitBagisciId, List<NakitBagisHareket> nakitBagisHareketListesi)
        {
            Armagan armagan = SelectByBagisciIdAndBagisTarihi(bastar, bittar, nakitBagisciId);
            int armaganId = 0;
            try
            {
                string durum = DurumGetir(nakitBagisciId);
                if (armagan != null)
                {
                    if (!string.IsNullOrEmpty(durum))
                    {
                        Durum = durum;
                    }
                    Update(); //TODO burasi armagan.Update() olmali veya üstsatirda Durum=durum; olmali
                    armaganId = armagan.Id;
                }
                else
                {
                    if (!string.IsNullOrEmpty(durum))
                    {
                        Durum = durum;
                    }
                    armaganId = Save();
                }

                UpdateArmaganIdAtNakitBagisHareket(nakitBagisciId, bastar, bittar, nakitBagisHareketListesi, armaganId);

            }
            catch (Exception)
            {
                armaganId = 0;
            }
            return armaganId;
        }
        private void UpdateArmaganIdAtNakitBagisHareket(int nakitBagisciId, DateTime bastar,DateTime bittar, List<NakitBagisHareket> nakitBagisHareketListesi, int armaganId)
        {
            //if (nakikbagisHareketId != 0)
            //{
            //    NakitBagisHareket nakitBagisHareket = new NakitBagisHareket();
            //    nakitBagisHareket = nakitBagisHareket.Select<NakitBagisHareket>(nakikbagisHareketId);
            //    if (nakitBagisHareket != null)
            //    {
            //        nakitBagisHareket.ArmaganId = armaganId;
            //        nakitBagisHareket.Update();
            //    }
            //}
            if (nakitBagisHareketListesi.Count> 0)
            {

                foreach (NakitBagisHareket item in nakitBagisHareketListesi)
                {
                    item.ArmaganId = armaganId;
                    try
                    {
                        item.Update();
                    }
                    catch (Exception e)
                    {
                        ExceptionHelper ex = new ExceptionHelper(e);
                        ex.PublishException();
                    }
                }
            }
        }
        public bool UpdateDurumByBolge(string fromdurum, string todurum, string bastar, string bittar, int armaganTanimId, int bolgeId)
        {
            bool isSuccess = false;
            string bolgeStr = bolgeId == ProjeConstants.BOLGE_HEPSI_INT || bolgeId == ProjeConstants.BOLGE_GENELMUDURLUK_INT ? string.Empty : string.Format("  AND Il_Table.BolgeId={0} ", bolgeId);

            string sqlString = string.Format(@"UPDATE A
                                                SET Durum={0}
                                                FROM
                                                    Armagan_Table A
                                                    INNER JOIN ArmaganTanim_Table
                                                    ON A.ArmaganTanimId = ArmaganTanim_Table.Id
                                                    INNER JOIN NakitBagisci_Table
                                                    ON A.BagisciId = NakitBagisci_Table.Id
                                                INNER JOIN Il_Table
                                                    ON Il_Table.Id = NakitBagisci_Table.Ili
                                                WHERE A.BelgeGecersizMi!=1 AND A.Durum = {1}
                                                    AND A.Tarih BETWEEN {2} AND {3} --AND MONTH(A.Tarih)={2} AND YEAR(A.Tarih)={3} 
                                                    AND A.ArmaganTanimId={4} " + bolgeStr
                                                    , todurum.ReturnQuotedValue(), fromdurum.ReturnQuotedValue(), bastar.ReturnQuotedValue(), bittar.ReturnQuotedValue(), armaganTanimId);

            isSuccess = dao.Update2Db(sqlString);

            return isSuccess;
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM Armagan_Table
                                WHERE BelgeGecersizMi!=1 ");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Armagan> list = ToList<Armagan>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public List<Armagan> SelectByBagisciIdAndDurum(int nakitBagisciId, string durum)
        {

            string sqlString = string.Format(@"SELECT * from Armagan_Table
                              Where BagisciId={0} AND Durum='{1}'", nakitBagisciId, durum);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Armagan> list = ToList<Armagan>(dataTable);

            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="durum"></param>
        /// <param name="bastar"></param>
        /// <param name="bittar"></param>
        /// <param name="armaganTanimId"></param>
        /// <param name="rowCount"></param>
        /// Parasi iade edilen armaganlari da göstersin diye BelgeGecersizMi kontrolu burada yok
        /// <returns></returns>
        public string SelectByDurumTarih(string durum, DateTime bastar, DateTime bittar, string armaganTanimId, ref int rowCount, int bolgeId, int ili)
        {
            DataTable dataTable = SelectByDurumTarihReturnDT(durum, bastar, bittar, armaganTanimId, bolgeId,ili);
            if (dataTable != null)
            {
                rowCount = dataTable.Rows.Count;
            }
            string json = ToJSON(dataTable);
            return json;
        }
        public DataTable SelectByDurumTarihReturnDT(string durum, DateTime bastar, DateTime bittar, string armaganTanimId, int bolgeId, int ili)
        {
            var durumQuery = string.Empty;
            if (!durum.Equals(ProjeConstants.HEPSI)) //eger bos ise query'e hiç eklenmesin
            {
                durumQuery = string.Format("Durum = '{0}' AND", durum);
            }

            var armaganTanimIdQuery = string.Empty;
            if (!armaganTanimId.Equals(ProjeConstants.HEPSI))
            {
                armaganTanimIdQuery = string.Format("AND ArmaganTanimId={0}", armaganTanimId);
            }
           
            string bolgeQuery = (bolgeId == ProjeConstants.BOLGE_HEPSI_INT || bolgeId == ProjeConstants.BOLGE_GENELMUDURLUK_INT)?string.Empty
                :string.Format(@" AND B.Ili IN (SELECT Id FROM Il_Table WHERE BolgeId = '{0}' ) ", bolgeId);

            string ilQuery = string.Empty;
            if (ili > 0 || ili != ProjeConstants.HEPSI_INT)
                ilQuery = string.Format(@" AND B.Ili = '{0}' ", ili);

            string sqlString = string.Format(@"
                SELECT distinct(A.Id) ArmaganId,ROW_NUMBER() OVER(ORDER BY A.Id) AS Sirano
                    ,B.Id as NakitBagisciId
                    ,B.Adi as NakitBagisciAdi
                    ,B.TCKimlikNo as NakitBagisciTC
					,B.Adres
                    ,IIF(ISNULL(Telefon1,'')!='',Telefon1, IIF(ISNULL(Telefon2,'')!='',Telefon2,'')) Telefon
                    ,E.IlAdi,E.Id IlId
					,F.IlceAdi                    
                    ,Convert(nvarchar,replace (A.BagisMiktari,'.',',')) as Tutar
                    --,C.DovizCinsi as DovizCinsi
                    ,D.Armagan as ArmaganBaslik
                    ,A.BagisciId
                    ,ArmaganTanimId
                    ,Tarih 
                    ,CONVERT(varchar,FORMAT(Tarih,'dd.MM.yyyy')) ArmaganTarihi
                    ,FORMAT(A.BagisMiktari, 'N2', 'tr-TR') ArmaganTutari
                    ,A.Durum
                    ,ISNULL(BelgedeYazanIsim, '') BelgedeYazanIsim
                    ,A.BelgeGecersizMi, A.IadeMiktari, A.DovizCinsi,A.BagisMiktariYazmasin, 
                    --IIF(A.CokluBagis=1,'Çoklu Bagis','Bagis') CokluBagis
                     IIF(A.DuzenliBagis=1, 'Düzenli Bağış', IIF(A.CokluBagis=1, 'Çoklu Bağış', 'Bağış')) AS CokluBagis
                FROM Armagan_Table A
                    INNER JOIN NakitBagisci_Table B ON B.Id=A.BagisciId
                    LEFT OUTER JOIN ArmaganTanim_Table D ON D.Id=A.ArmaganTanimId
					LEFT OUTER JOIN Il_Table E ON E.Id= B.Ili 
                    LEFT OUTER JOIN Ilce_Table F ON F.Id= B.Ilcesi AND F.IlId=E.Id
                WHERE {0} Tarih between {1} and {2} {3} {4} {5}
                ORDER BY A.Id
            ", durumQuery, bastar.ReturnTRDateFormat(), bittar.ReturnTRDateFormat(), armaganTanimIdQuery, bolgeQuery, ilQuery);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            return dataTable;
        }
        public DataTable SelectCountDurumByBolgeBasTarBitTar(DateTime basTar, DateTime bitTar, int armaganTanimId, int bolgeId)
        {
            string bolgeStr = bolgeId == ProjeConstants.HEPSI_INT || bolgeId == ProjeConstants.BOLGE_GENELMUDURLUK_INT ? string.Empty : string.Format(" AND N.Ili IN (SELECT Id FROM Il_Table WHERE BolgeId={0})", bolgeId);

            //if (!string.IsNullOrEmpty(bolge))
            //    bolgeStr = string.Format(@" AND N.Ili IN (SELECT Id FROM Il_Table WHERE Bolge = '{0}' ) ", bolge);

            string sqlString = string.Format(@"        
                                SELECT A.Durum , COUNT(Durum) Adet FROM Armagan_Table A
                                INNER JOIN NakitBagisci_Table N on N.Id=A.BagisciId
                                WHERE A.BelgeGecersizMi!=1 AND A.ArmaganTanimId={0} AND A.Tarih BETWEEN {1} AND {2} {3}
                                GROUP BY Durum ", armaganTanimId, basTar.ReturnTRDateFormat(), bitTar.ReturnTRDateFormat(), bolgeStr);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            return dataTable;
        }
        public DataTable SelectCountDurumByBolgeTarih(int armaganTanimId, int bolgeId, DateTime bastar, DateTime bittar)
        {
            string bolgeStr = (bolgeId == ProjeConstants.BOLGE_HEPSI_INT || bolgeId == ProjeConstants.BOLGE_GENELMUDURLUK_INT) ? string.Empty
                    :string.Format(@" AND N.Ili IN (SELECT Id FROM Il_Table WHERE BolgeId = '{0}' ) ", bolgeId);

            string sqlString = string.Format(@"        
                                SELECT A.Durum , COUNT(Durum) Adet FROM Armagan_Table A
                                INNER JOIN NakitBagisci_Table N on N.Id=A.BagisciId
                                WHERE A.BelgeGecersizMi!=1 AND A.Tarih BETWEEN {0} AND {1} AND A.ArmaganTanimId={2} " + bolgeStr +
                                @" GROUP BY Durum ", bastar.ReturnTRDateFormat(), bittar.ReturnTRDateFormat(), armaganTanimId);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            return dataTable;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="basTar"></param>
        /// <param name="bitTar"></param>
        /// <returns></returns>
        public DataTable SelectCountByBagisTarihiBolge(DateTime basTar, DateTime bitTar)
        {
            string sqlString = string.Format(@"        
                SELECT  COUNT(A.Id) Adet, D.Id As BolgeId, A.ArmaganTanimId ArmaganTanimId 
                FROM 
	                Armagan_Table A
	                INNER JOIN NakitBagisci_Table B on B.Id=A.BagisciId
	                INNER JOIN Il_Table C on C.Id=B.Ili
	                LEFT JOIN Bolge_Table D on D.Id=C.BolgeId
                WHERE Tarih BETWEEN {0} AND {1} 
                GROUP BY  D.Id, ArmaganTanimId ", basTar.ReturnTRDateFormat(), bitTar.ReturnTRDateFormat());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            return dataTable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="eksiId"></param>
        /// Parasi iade edilen armaganlari da göstermesin
        /// <returns></returns>
        public string SelectByFilter(string filter, int eksiId)
        {
            string ilStr = string.Empty;

            string sqlString = string.Format(@"
                SELECT A.Id ArmaganId, B.Armagan, A.Durum, A.Tarih, 
					CONVERT(varchar,FORMAT(A.Tarih,'dd.MM.yyyy')) BelgeTarihi, A.BagisMiktari, C.Id NakitBagisciId, C.Adi, C.Soyadi, C.TCKimlikNo, D.IlAdi Ili ,E.IlceAdi Ilcesi,Adres,
	                Telefon1,Telefon2, Telefon1 + IIF(ISNULL(Telefon1,'')!='' AND ISNULL(Telefon2,'')!='',' - ','') + Telefon2 Telefon, 
                    C.OlusturmaTarihi  ,C.DegistirmeTarihi,C.Degistiren,Sag,Eposta ,PostaKodu, Ulasilamiyor, BelgeIstemiyor                                            
                FROM Armagan_Table A 
					LEFT JOIN ArmaganTanim_Table B ON B.Id= A.ArmaganTanimId 
					INNER JOIN NakitBagisci_Table C ON C.Id= A.BagisciId 
                    LEFT JOIN Il_Table D ON D.Id= C.Ili 
                    LEFT JOIN Ilce_Table E ON E.Id= C.Ilcesi  AND E.IlId=D.Id
                WHERE A.BelgeGecersizMi!=1 AND C.Id!= {0} 
                    AND A.Id={1} --like '%{1}%'
                    --AND (C.Adi like '%{1}%'
	                --OR A.Id like '%{1}%')
	                --OR C.TCKimlikNo like '%{1}%'
	                --OR C.Telefon1 like '%{1}%'
	                --OR C.Adres like '%{1}%'
	                --OR C.BagisTarihi like '%{1}%'
	                --OR C.BagisMiktari like '%{1}%'
	                --OR A.IlAdi like '%{1}%'
	                --OR B.IlceAdi like '%{1}%'
                ORDER BY A.Tarih DESC ", eksiId, filter);
            DataTable dataTable = null;
            try
            {
                dataTable = dao.SelectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                throw;
            }
            string json = ToJSON(dataTable);
            return json;
        }

        public DataTable SelectVerilenArmaganlarGroupByBagisciReturnList()
        {
            //Armagan_Table'dan CokliNagis=true olan kayitlari seç
            string sqlString = string.Format(@"
                SELECT A.BagisciId,B.Adi, B.Soyadi, D.Armagan, COUNT(C.Id) BagisAdedi, SUM(C.BagisMiktari) ToplamBagis,A.Tarih
                FROM Armagan_Table A
                INNER JOIN NakitBagisci_Table B ON A.BagisciId = B.Id
                INNER JOIN NakitBagisHareket_Table C ON C.ArmaganId = A.Id
                INNER JOIN ArmaganTanim_Table D ON D.Id = A.ArmaganTanimId

                WHERE A.BelgeGecersizMi!=1 AND A.CokluBagis=1
                GROUP BY A.BagisciId, B.Adi, B.Soyadi, C.ArmaganId,A.Tarih,D.Armagan
                ORDER BY Tarih DESC");
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            return dataTable;

        }

        public Armagan SelectByBagisciIdAndArmaganTanimId(int nakitBagisciId, int armaganTanimId)
        {

            string sqlString = string.Format(@"SELECT * from Armagan_Table
                              WHERE BelgeGecersizMi!=1 AND BagisciId={0} AND ArmaganTanimId={1} ",
                           nakitBagisciId.ReturnQuotedValue(), armaganTanimId.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Armagan> list = ToList<Armagan>(dataTable);
            Armagan armagan = new Armagan();
            armagan = list.FirstOrDefault();
            return armagan;
        }
        public int SelectCountByBagisciIdAndArmaganTanimId(int nakitBagisciId, int armaganTanimId)
        {
            string sqlString = string.Format(@"SELECT COUNT(*)
                                      FROM Armagan_Table
                                      WHERE BelgeGecersizMi!=1
                                        AND BagisciId={0}
                                        AND ArmaganTanimId={1}",
                nakitBagisciId.ReturnQuotedValue(),
                armaganTanimId.ReturnQuotedValue());

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                return Convert.ToInt32(dataTable.Rows[0][0]);
            }

            return 0;
        }
    }
}
