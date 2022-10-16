
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.IKYS
{
    public class IzinTalep : ParentClass
    {
        public IzinTalep()
        {
            Aktif = true;
        }
        public int PersonelId { get; set; }
        public int IzinTipi { get; set; }
        public int IzinDonemId { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public string Sure { get; set; }
        public string Birim { get; set; }
        public int VekilImza { get; set; }
        public int AmirImza { get; set; }
        public string Adres { get; set; }
        public string Aciklama { get; set; }
        public int OnayImza { get; set; }
        public int OnayDurumu { get; set; }
        public bool Aktif { get; set; }
        public bool EPostaGonder { get; set; }
        public override T Select<T>(int id)
        {
            string sqlString = SelectSQL(id);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<IzinTalep> list = ToList<IzinTalep>(dataTable);
            IzinTalep izinTalep = new IzinTalep();
            izinTalep = list.FirstOrDefault();
            return (T)Convert.ChangeType(izinTalep, typeof(T));
        }
        public override int Save()
        {
            try
            {
                GenericEntity<IzinTalep> genericEntity = new GenericEntity<IzinTalep>(ProjeConstants.SQL_INSERT);
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
            //string sqlString = saveSQL();
            //int id = dao.Insert(sqlString);
            //this.Id = id;
            //return id;
        }
        public override bool Update()
        {
            bool isSuccess = false;
            try
            {
                if (Id != 0)
                {
                    GenericEntity<IzinTalep> genericEntity = new GenericEntity<IzinTalep>(ProjeConstants.SQL_UPDATE);
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
            //bool isSuccess = false;
            //if (Id != 0)
            //{
            //    string sqlString = UpdateSQL();
            //    isSuccess = dao.Update2Db(sqlString);
            //}
            //return isSuccess;
        }
        public override bool Delete()
        {
            string sqlString = DeleteSQL();

            bool isSuccess = dao.DeleteFromDb(sqlString, this);

            return isSuccess;
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM IzinTalep_Table");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<IzinTalep> list = ToList<IzinTalep>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        private string saveSQL()
        {
            //Insert  SQL
            string InsertSQL = string.Format(@" 
                                    INSERT INTO IzinTalep_Table 
                                        (PersonelId,IzinTipi,IzinDonemId,BaslangicTarihi,BitisTarihi,Sure, Birim,VekilImza,AmirImza,OnayImza,Adres,Aciklama,OnayDurumu,Aktif, Olusturan,OlusturmaTarihi)
                                    VALUES ({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15}) ",
                                    PersonelId.ReturnQuotedValue(), IzinTipi.ReturnQuotedValue(), IzinDonemId.ReturnQuotedValue(),
                                    BaslangicTarihi.ReturnTRDateFormat(), BitisTarihi.ReturnTRDateFormat(), Sure.ReturnQuotedValue(), Birim.ReturnQuotedValue(),
                                    VekilImza.ReturnQuotedValue(), AmirImza.ReturnQuotedValue(), OnayImza.ReturnQuotedValue(),
                                    Adres.ReturnQuotedValue(), Aciklama.ReturnQuotedValue(), OnayDurumu.ReturnQuotedValue(), Aktif.ReturnQuotedValue(),
                                    Olusturan.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat());
            return InsertSQL;
        }
        private string UpdateSQL()
        {
            //Update  SQL 
            string sqlSQL = string.Format(@"
                                    UPDATE IzinTalep_Table 
                                    SET PersonelId={0}, IzinTipi={1}, IzinDonemId={2},  BaslangicTarihi={3},BitisTarihi={4},Sure={5},Birim={6},  
                                        VekilImza={7}, AmirImza={8}, OnayImza={9}, Adres={10},Aciklama={11}, OnayDurumu={12},Aktif={13},
                                        Degistiren={14},DegistirmeTarihi={15}
                                    WHERE Id= {16}",
                                    PersonelId.ReturnQuotedValue(), IzinTipi.ReturnQuotedValue(), IzinDonemId.ReturnQuotedValue(),
                                    BaslangicTarihi.ReturnTRDateFormat(), BitisTarihi.ReturnTRDateFormat(), Sure.ReturnQuotedValue(), Birim.ReturnQuotedValue(),
                                    VekilImza.ReturnQuotedValue(), AmirImza.ReturnQuotedValue(), OnayImza.ReturnQuotedValue(), Adres.ReturnQuotedValue(),
                                    Aciklama.ReturnQuotedValue(), OnayDurumu.ReturnQuotedValue(), Aktif.ReturnQuotedValue(),
                                    Degistiren.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat(), Id);
            return sqlSQL;
        }
        private string SelectSQL(int id)
        {
            string sqlstr = string.Format(@"SELECT *
                               FROM IzinTalep_Table 
                               WHERE  Id={0}", id);
            return sqlstr;
        }
        private string DeleteSQL()
        {
            string sqlString = string.Format(@"
                            DELETE 
                            FROM IzinTalep_Table
                            WHERE Id={0}", Id);
            return sqlString;
        }
        public string SelectIzinTalepleriReturnJson(int personelId, int izinTipi, bool mazeretHaric, bool sadeceGecerliDonemTalepleri)
        {

            string sqlString = SelectIzinTalepleriSQL(personelId, izinTipi, mazeretHaric, sadeceGecerliDonemTalepleri);
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
        public DataTable SelectIzinTalepleriReturnDT(int personelId, int izinTipi, bool mazeretHaric, bool sadeceGecerliDonemTalepleri)
        {

            string sqlString = SelectIzinTalepleriSQL(personelId, izinTipi, mazeretHaric, sadeceGecerliDonemTalepleri);
            DataTable dataTable = null;
            try
            {
                dataTable = dao.selectFromDb(sqlString, "");

            }
            catch (Exception e)
            {
                throw e;
            }

            return (dataTable);
        }
        private string SelectIzinTalepleriSQL(int personelId, int izinTipi, bool mazeretHaric, bool sadeceGecerliDonemVeDahaYeniTarihlitalepler)
        {
            string perStr = string.Empty;
            string izinTipiStr = string.Empty;
            string izinDonemiStr = string.Empty;

            perStr = personelId > 0 ? string.Format(" AND PersonelId={0} ", personelId) : "";
            if (mazeretHaric)
            {
                izinTipiStr = string.Format(" AND IzinTipi!={0} ", ProjeConstants.IZINTIPI_MAZERET_INT);
            }
            else
            {
                izinTipiStr = izinTipi > 0 ? string.Format(" AND IzinTipi={0} ", izinTipi) : "";
            }
            if (sadeceGecerliDonemVeDahaYeniTarihlitalepler)
            {
                DateTime today = DateTime.Today;
                IzinDonem ucretliIzinDonemi = new IzinDonem();
                ucretliIzinDonemi = ucretliIzinDonemi.SelectByIzinTarihi(personelId, ProjeConstants.IZINTIPI_UCRETLI_INT, today);
                IzinDonem mazeretIzinDonemi = new IzinDonem();
                mazeretIzinDonemi = mazeretIzinDonemi.SelectByIzinTarihi(personelId, ProjeConstants.IZINTIPI_MAZERET_INT, today);


                if ((ucretliIzinDonemi != null) && (mazeretIzinDonemi != null))
                {
                    DateTime gelecekUcretliIzinDonemBasi = ucretliIzinDonemi.BitisTarihi.AddDays(1);
                    DateTime gelecekMazeretIzinDonemBasi = mazeretIzinDonemi.BitisTarihi.AddDays(1);
                    IzinDonem gelecekUcretliIzinDonemi = new IzinDonem();
                    gelecekUcretliIzinDonemi = gelecekUcretliIzinDonemi.SelectByIzinTarihi(personelId, ProjeConstants.IZINTIPI_UCRETLI_INT, gelecekUcretliIzinDonemBasi);
                    IzinDonem gelecekMazeretIzinDonemi = new IzinDonem();
                    gelecekMazeretIzinDonemi = gelecekMazeretIzinDonemi.SelectByIzinTarihi(personelId, ProjeConstants.IZINTIPI_MAZERET_INT, gelecekMazeretIzinDonemBasi);

                    string gelecekUcretliIzinDonemiStr = gelecekUcretliIzinDonemi == null ? "" : string.Format(" OR  IzinDonemId={0}", gelecekUcretliIzinDonemi.Id);
                    string gelecekMazeretIzinDonemiStr = gelecekMazeretIzinDonemi == null ? "" : string.Format(" OR  IzinDonemId={0}", gelecekMazeretIzinDonemi.Id);
                    izinDonemiStr = string.Format(" AND (IzinDonemId={0} OR IzinDonemId={1} OR IzinDonemId=0 {2} {3}) ", ucretliIzinDonemi.Id, mazeretIzinDonemi.Id, gelecekUcretliIzinDonemiStr, gelecekMazeretIzinDonemiStr);// OR IzinDonemId=0 çünkü ücretli ve mazeret izni dışındakilerin izindonemId si yok SB_UPDATE 25.03.2019
                }
            }

            string sqlstr = string.Format(@" 
                    SELECT A.Id IzinTalepId, P.Adi+' '+P.Soyadi AdiSoyadi, A.PersonelId,A.BaslangicTarihi,A.BitisTarihi,A.Sure,A.Birim,
                           B.Adi IzinTipi,A.IzinTipi IzinTipiId, IzinDonemId, A.OnayDurumu OnayDurumuId,
                           C.Adi OnayDurumu, A.VekilImza, A.AmirImza,A.OnayImza, A.Adres,A.Aciklama,A.IzinDonemId 
                    FROM IzinTalep_Table A
                        INNER JOIN Personel_Table P On A.PersonelId=P.Id  
						INNER JOIN IzinTanim_Table B On A.IzinTipi=B.Id  
						INNER JOIN OnayTanim_Table C On A.OnayDurumu=C.Id     
                    WHERE Aktif=1 {0} {1} {2}
                    ORDER BY A.Id DESC", perStr, izinTipiStr, izinDonemiStr);
            return sqlstr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="personelId"></param>
        /// <param name="basTar"></param>
        /// <param name="bitTar"></param>
        /// <returns>Girilen tarihler arasında :
        /// OnayDurumu Reddedildi veya İptal Edildi olanlar hariç olmak koşulu ile,
        /// IzinTalebi varsa bu döner, yoksa null döner </returns>
        public IzinTalep SelectByPersonelIdBasBitTar(int personelId, DateTime basTar, DateTime bitTar)
        {
            string sqlString = string.Format(@"
                            SELECT *
                            FROM IzinTalep_Table
                            WHERE OnayDurumu not in(3,4) AND PersonelId={0} 
                                AND (BaslangicTarihi<={2} AND BitisTarihi>={1})
                            ", personelId, basTar.ReturnTRDateFormat(), bitTar.ReturnTRDateFormat());
            //string sqlString = string.Format(@"
            //                SELECT *
            //                FROM IzinTalep_Table
            //                WHERE OnayDurumu not in(3,4) AND PersonelId={0} 
            //                    AND ((BaslangicTarihi>={1} AND BaslangicTarihi<={2}) 
            //                        OR (BitisTarihi>={1} AND BitisTarihi<={2})
            //                        OR (BaslangicTarihi>={1} AND BitisTarihi<={2}))
            //                ", personelId, basTar.ReturnTRDateFormat(), bitTar.ReturnTRDateFormat());

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<IzinTalep> list = ToList<IzinTalep>(dataTable);
            IzinTalep izinTalebi = new IzinTalep();
            izinTalebi = list.FirstOrDefault<IzinTalep>();
            return izinTalebi;
        }
        public IzinTalep SelectIslemiDevamEdenIzinTalebiVarMı(int personelId, int izinTipi)
        {
            string sqlString = string.Format(@"
                            SELECT *
                            FROM IzinTalep_Table
                            WHERE OnayDurumu not in(2,3,4) AND PersonelId={0} 
                                AND IzinTipi={1}
                            ", personelId, izinTipi);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<IzinTalep> list = ToList<IzinTalep>(dataTable);
            IzinTalep izinTalebi = new IzinTalep();
            izinTalebi = list.FirstOrDefault<IzinTalep>();
            return izinTalebi;
        }
        public IzinTalep SelectSonIzinTalebiByPersonel(int izinTipi, int personelId)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM IzinTalep_Table
                WHERE IzinTipi={0} AND PersonelId={1} 
                ORDER BY BaslangicTarihi DESC
                ", izinTipi,personelId);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<IzinTalep> list = ToList<IzinTalep>(dataTable);
            IzinTalep izinTalebi = new IzinTalep();
            izinTalebi = list.FirstOrDefault<IzinTalep>();
            return izinTalebi;
        }
    }
}
