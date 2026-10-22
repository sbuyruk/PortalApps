using DAO.Ortak;
using System.Data;
using Utility.Logging;

namespace DAO.Repositories.NBYS
{
    // Read-only dependencies of the remaining NBYS support screens. No Portal/IKYS write path is migrated here.
    public sealed class NbysSupportReadRepository
    {
        private readonly NbysSqlExecutor sql;

        public NbysSupportReadRepository(PortalLogContext context)
        {
            sql = new NbysSqlExecutor(context, "NbysSupportReadRepository");
        }

        public DataTable FtkProvinces()
        {
            return sql.Select(new SqlQuery(@"SELECT A.* FROM Il_Table A
                WHERE A.Id BETWEEN 0 AND 81 AND A.IlAdi!=''
                AND A.Id IN (SELECT Ili FROM FTK_Table) ORDER BY A.IlAdi"));
        }

        public DataTable FtkDistricts(int provinceId, string centralDistrict)
        {
            SqlQuery query = new SqlQuery(@"SELECT A.* FROM Ilce_Table A INNER JOIN Il_Table B ON B.Id=A.IlId
                WHERE A.IlceAdi!=@Central AND B.Id BETWEEN 0 AND 81 AND B.IlAdi!=''
                AND A.Id IN (SELECT Ilcesi FROM FTK_Table WHERE Ilcesi>0) AND B.Id=@ProvinceId ORDER BY A.IlceAdi");
            query.AddParameter("@Central", centralDistrict);
            query.AddParameter("@ProvinceId", provinceId);
            return sql.Select(query);
        }

        public DataTable PermissionSettings(string project, string module)
        {
            SqlQuery query = new SqlQuery(@"SELECT * FROM ProgramYetki_Table
                WHERE Program=@Project AND Modul=@Module AND BirimId IN (7,8,9) ORDER BY BirimId,Id");
            query.AddParameter("@Project", project);
            query.AddParameter("@Module", module);
            return sql.Select(query);
        }

        public DataTable FtkMailRecipients(int regionId)
        {
            // Preserve active regional staff and the first contact row ordered by Adres.
            SqlQuery query = new SqlQuery(@"SELECT C.IntranetEPosta FROM Personel_Table P
                INNER JOIN IsBilgileri_Table I ON I.PersonelId=P.Id
                INNER JOIN BirimTanim_Table B ON B.Id=I.BirimId
                CROSS APPLY (SELECT TOP (1) IntranetEPosta FROM IletisimBilgileri_Table
                    WHERE PersonelId=P.Id ORDER BY Adres) C
                WHERE I.CalismaDurumu=1 AND B.BolgeId=@RegionId ORDER BY I.ProtokolSiraNo");
            query.AddParameter("@RegionId", regionId);
            return sql.Select(query);
        }

        public DataTable MailServer(string key)
        {
            SqlQuery query = new SqlQuery(@"SELECT TOP (1) Deger FROM OrtakParametre_Table
                WHERE Anahtar=@Key ORDER BY Sira,Deger");
            query.AddParameter("@Key", key);
            return sql.Select(query);
        }
    }
}
