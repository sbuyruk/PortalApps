param([string]$CscPath)

$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path $PSScriptRoot -Parent
if (!$CscPath) {
    $CscPath = Get-ChildItem 'C:/Program Files/Microsoft Visual Studio' -Filter csc.exe -Recurse |
        Where-Object { $_.FullName -match 'Roslyn' } | Select-Object -First 1 -ExpandProperty FullName
}
if (!$CscPath) { throw 'Roslyn csc.exe bulunamadı; -CscPath belirtin.' }

# Gerçek kaynak metotlar derlenir; SharePoint ve veritabanı yerine bellek fixture'ları kullanılır.
# Bu test SQL Server üzerinde kilitleme veya SharePoint belge basımı testi değildir.
function Read-MethodSection($path, $start, $end) {
    $source = [IO.File]::ReadAllText((Join-Path $repoRoot $path))
    $first = $source.IndexOf($start)
    $last = $source.IndexOf($end, $first + $start.Length)
    if ($first -lt 0 -or $last -lt 0) { throw "Kaynak metot bulunamadı: $start" }
    return $source.Substring($first, $last - $first)
}

$webpartPath = 'NBYS_WebParts/DuzenliNakitBagisciListesiWP/DuzenliNakitBagisciListesiWP.ascx.cs'
$linkMethod = Read-MethodSection $webpartPath 'private string DuzenliBagisciBelgesiLinkiGetir(' 'private string CreateDataTable('
$createMethod = Read-MethodSection $webpartPath 'public ExceptionHelper ArmaganOlustur(' 'protected void AyDDL_SelectedIndexChanged('
$transferMethod = Read-MethodSection 'Model/Services/NBYS/EkstreAktarmaTransferService.cs' 'public static int ArmaganiKaydet(' 'private static int SaveNakitBagisciFromEkstre('
$saveMethod = Read-MethodSection 'Model/Services/NBYS/ArmaganService.cs' 'public int SaveDuzenliBagisIfMissing(' 'public bool Update('

$fixtureSource = @'
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Model.NBYS;
using Model.Ortak;
using Model.Services.NBYS;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.Ortak {
    public abstract class EntityBase {
        public int Id { get; set; }
        public DateTime OlusturmaTarihi { get; set; }
        public DateTime DegistirmeTarihi { get; set; }
        public string Olusturan { get; set; }
        public string Degistiren { get; set; }
    }
    public class OlayKayit {
        public void GirisOlayKaydet(object entity, string app, string type) { Fixture.Logs++; }
    }
}
namespace Utility.HelperClasses {
    public class ExceptionHelper { public List<Exception> Exceptions = new List<Exception>(); }
    public static class UtilityHelper { public static string GetCurrentUserName() { return "test"; } }
}
namespace DAO.Ortak {
    public class DbClass {
        public static Func<SqlQuery, DataTable> Select;
        public DataTable SelectFromDb(SqlQuery query, string description) { return Select(query); }
        public int Insert(SqlQuery query) { throw new Exception("Unexpected ordinary insert"); }
        public bool Update2Db(SqlQuery query) { throw new Exception("Unexpected SQL update"); }
        public bool DeleteFromDb(SqlQuery query, string description) { throw new Exception("Unexpected delete"); }
    }
}
public static class Fixture {
    public static List<Armagan> Gifts = new List<Armagan>();
    public static NakitBagisHareket Movement;
    public static DateTime RequestedYearStart;
    public static int Logs, MovementUpdates, DonorUpdates, SaveCalls, OrdinarySaves;
    public static void Reset() {
        Gifts.Clear(); Logs = MovementUpdates = DonorUpdates = SaveCalls = OrdinarySaves = 0;
        Movement = new NakitBagisHareket { Id=81, BagisciId=7, BagisTarihi=new DateTime(2025, 12, 20) };
        DAO.Ortak.DbClass.Select = query => {
            // Scripted response for the repository's insert-or-get result contract.
            int number = Convert.ToInt32(query.Parameters.Single(p => p.ParameterName=="@KacinciBelge").Value);
            Armagan gift = Gifts.FirstOrDefault(g => g.KacinciBelge==number);
            bool created = gift==null;
            if (created) {
                gift = new Armagan { Id=100+number, BagisciId=7, KacinciBelge=number, DuzenliBagis=true,
                    ArmaganTanimId=ProjeConstants.ARMAGAN_DUZENLIBAGISCIBELGESIID,
                    Tarih=Convert.ToDateTime(query.Parameters.Single(p => p.ParameterName=="@Tarih").Value),
                    BagisMiktari=Convert.ToDecimal(query.Parameters.Single(p => p.ParameterName=="@BagisMiktari").Value) };
                Gifts.Add(gift);
            }
            SaveCalls++;
            DataTable result = new DataTable();
            result.Columns.Add("Id", typeof(int)); result.Columns.Add("Olusturuldu", typeof(bool));
            result.Rows.Add(gift.Id, created);
            return result;
        };
    }
}
namespace Model.Services.NBYS {
    public class NakitBagisHareketService {
        public NakitBagisHareket GetLastInYearByBagisciId(int id, DateTime start) {
            Fixture.RequestedYearStart=start; return Fixture.Movement;
        }
        public bool Update(NakitBagisHareket movement) { Fixture.MovementUpdates++; return true; }
    }
    public class DuzenliNakitBagisciService {
        public bool Update(DuzenliNakitBagisci donor) { Fixture.DonorUpdates++; return true; }
    }
    public class ArmaganService {
        private DAO.Repositories.NBYS.ArmaganRepository repository = new DAO.Repositories.NBYS.ArmaganRepository();
        public Armagan GetDuzenliBagisByBelgeSirasi(int donor, int number) {
            return Fixture.Gifts.FirstOrDefault(g => g.BagisciId==donor && g.BelgeGecersizMi!=1
                && (g.KacinciBelge==0 ? 1 : g.KacinciBelge)==number);
        }
        public int CountByBagisciIdAndTanimId(int donor, int definition) { return Fixture.Gifts.Count; }
        public List<Armagan> GetByBagisciIdAndDurum(int donor, string status) { return new List<Armagan>(); }
        public int Save(Armagan gift) { Fixture.OrdinarySaves++; Fixture.Gifts.Add(gift); return 700; }
        __SAVE_METHOD__
    }
    public static class EkstreAktarmaTransferService {
        __TRANSFER_METHOD__
    }
}
public class WebpartHarness {
    private string CurrentUserName = "test";
    private void TabloOlustur() { }
    public string Link(int donations, int giftId=0, int number=0, string status="", bool noDocument=false, int donorId=7) {
        return DuzenliBagisciBelgesiLinkiGetir(donorId, giftId, new DateTime(2023, 1, 1), noDocument, status, 17, donations, number);
    }
    __LINK_METHOD__
    __CREATE_METHOD__
}
public static class Regression {
    private static int assertions;
    private static void Check(bool condition, string message) {
        assertions++; if (!condition) throw new Exception(message);
    }
    public static int Main() {
        try { Run(); return 0; }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
    private static void Run() {
        WebpartHarness webpart = new WebpartHarness();
        int[] eligible = {12, 24, 36, 48, 60, 120};
        foreach (int count in eligible) {
            int number=count/12;
            Check(DuzenliNakitBagisci.GetHakedilenBelgeSirasi(count)==number, "Wrong milestone number");
            Check(webpart.Link(count).Contains("<button"), "Missing create button: " + count);
            if (number>1)
                Check(webpart.Link(count, 100, number-1, ProjeConstants.DURUM_GONDERILDI).Contains("<button"), "Previous sent document blocks new year");
            Check(!webpart.Link(count, 100, number).Contains("<button"), "Current document permits duplicate");
            Check(webpart.Link(count, 100, number).Contains("Düzenle"), "Current document cannot be edited");
            Check(!webpart.Link(count, 100, number, ProjeConstants.DURUM_GONDERILDI).Contains("<button"), "Sent current document permits duplicate");
            Fixture.Reset();
            DuzenliNakitBagisci donor = new DuzenliNakitBagisci {
                Id=17, BagisciId=7, Aktif=true, BagisAdedi=count, BaslamaTarihi=new DateTime(2023, 1, 1), BagisToplami=2400
            };
            NakitBagisci matched = new NakitBagisci { Id=7 };
            Check(webpart.ArmaganOlustur(donor, matched).Exceptions.Count==0, "Creation failed: " + count);
            Check(Fixture.RequestedYearStart==donor.BaslamaTarihi.AddMonths(count-12), "Wrong donation year");
            Check(Fixture.Gifts.Single().KacinciBelge==number, "Skipped years assigned wrong number");
            Check(Fixture.Gifts.Single().BagisMiktari==2400 && Fixture.Gifts.Single().Tarih==Fixture.Movement.BagisTarihi, "Wrong amount/date");
            int id=donor.ArmaganId;
            Check(webpart.ArmaganOlustur(donor, matched).Exceptions.Count==0, "Retry failed");
            Check(Fixture.Gifts.Count==1 && Fixture.SaveCalls==1 && donor.ArmaganId==id && Fixture.MovementUpdates==1, "Retry created another gift");
            Check(Fixture.Logs==1, "Duplicate save log");
        }
        foreach (int count in new[] {-12, 0, 1, 11, 13, 23, 25, 35, 37, 49}) {
            Fixture.Reset();
            Check(!webpart.Link(count).Contains("<button"), "Non-milestone permits creation");
            var donor = new DuzenliNakitBagisci { BagisciId=7, Aktif=true, BagisAdedi=count };
            Check(webpart.ArmaganOlustur(donor, new NakitBagisci { Id=7 }).Exceptions.Count==1, "Server accepted non-milestone");
            Check(Fixture.SaveCalls==0 && Fixture.DonorUpdates==0, "Rejected request wrote data");
        }
        Check(!webpart.Link(36, noDocument:true).Contains("<button"), "Document preference ignored");
        Check(!webpart.Link(36, donorId:-1).Contains("<button"), "Unmatched donor permits creation");
        Check(!webpart.Link(12, 100, 0).Contains("<button"), "Legacy first document ignored");
        Fixture.Reset();
        var progressing = new DuzenliNakitBagisci { BagisciId=7, Aktif=true, BaslamaTarihi=new DateTime(2023, 1, 1) };
        foreach (int count in new[] {12, 24, 36}) {
            progressing.BagisAdedi=count;
            int previousId=progressing.ArmaganId;
            Check(webpart.ArmaganOlustur(progressing, new NakitBagisci { Id=7 }).Exceptions.Count==0, "Next year failed");
            Check(progressing.ArmaganId!=previousId && Fixture.Gifts.Count==count/12, "Next year reused previous document");
            Fixture.Gifts.Last().Durum=ProjeConstants.DURUM_GONDERILDI;
        }
        progressing.ArmaganId=0; // Re-uploaded donor row without a document pointer.
        Check(webpart.ArmaganOlustur(progressing, new NakitBagisci { Id=7 }).Exceptions.Count==0, "Re-upload failed");
        Check(Fixture.Gifts.Count==3 && progressing.ArmaganId==Fixture.Gifts.Last().Id, "Re-upload duplicated or lost current document");
        Check(Fixture.Gifts.All(g => g.Durum==ProjeConstants.DURUM_GONDERILDI), "Earlier sent statuses were changed");
        Fixture.Reset();
        var active = new DuzenliNakitBagisci { BagisciId=7, Aktif=true, BagisAdedi=24, BaslamaTarihi=new DateTime(2023, 1, 1) };
        Check(webpart.ArmaganOlustur(active, new NakitBagisci { Id=8 }).Exceptions.Count==1, "Mismatched IDs accepted");
        Check(webpart.ArmaganOlustur(active, new NakitBagisci { Id=7, BelgeIstemiyor=true }).Exceptions.Count==1, "Server ignored document preference");
        active.Aktif=false;
        Check(webpart.ArmaganOlustur(active, new NakitBagisci { Id=7 }).Exceptions.Count==1, "Inactive donor accepted");
        active.Aktif=true; Fixture.Movement=null;
        Check(webpart.ArmaganOlustur(active, new NakitBagisci { Id=7 }).Exceptions.Count==1, "Missing movement silently ignored");
        Check(Fixture.SaveCalls==0, "Invalid requests inserted gifts");
        Fixture.Reset();
        var gift = new Armagan { DuzenliBagis=true, KacinciBelge=3, BagisciId=7, Tarih=DateTime.Today, ArmaganTanimId=ProjeConstants.ARMAGAN_DUZENLIBAGISCIBELGESIID };
        ArmaganService service = new ArmaganService();
        int existingId=service.SaveDuzenliBagisIfMissing(gift);
        Check(service.SaveDuzenliBagisIfMissing(gift)==existingId && Fixture.Logs==1, "Existing insert-or-get result logged as new");
        gift.KacinciBelge=0;
        bool rejected=false; try { service.SaveDuzenliBagisIfMissing(gift); } catch (ArgumentException) { rejected=true; }
        Check(rejected, "Invalid period passed service validation");
        Fixture.Reset();
        EkstreAktarmaTransferService.ArmaganiKaydet(Fixture.Movement, new NakitBagisci { Id=7 }, 500, ProjeConstants.ARMAGAN_TESEKKURID, "test");
        Check(Fixture.OrdinarySaves==1 && Fixture.SaveCalls==0, "Ordinary gift behavior changed");
        Console.WriteLine("PASS: " + assertions + " assertions (database and SharePoint isolated).");
    }
}
'@
$fixtureSource = $fixtureSource.Replace('__LINK_METHOD__', $linkMethod).Replace('__CREATE_METHOD__', $createMethod).
    Replace('__TRANSFER_METHOD__', $transferMethod).Replace('__SAVE_METHOD__', $saveMethod)
$testStem = Join-Path ([IO.Path]::GetTempPath()) ('PortalApps-Armagan-' + [Guid]::NewGuid().ToString('N'))
$testSource = $testStem + '.cs'
$testExe = $testStem + '.exe'
try {
    [IO.File]::WriteAllText($testSource, $fixtureSource, [Text.UTF8Encoding]::new($false))
    $sources = @('Model/NBYS/DuzenliNakitBagisci.cs', 'Model/NBYS/Armagan.cs', 'Model/NBYS/NakitBagisci.cs',
        'Model/NBYS/NakitBagisHareket.cs', 'Utility/ProjeGlobal/ProjeConstants.cs', 'DAO/Ortak/SqlQuery.cs',
        'DAO/Ortak/CrudQueryBuilder.cs', 'DAO/Repositories/NBYS/ArmaganRepository.cs',
        'DAO/Repositories/NBYS/NakitBagisciReportRepository.cs') | ForEach-Object { Join-Path $repoRoot $_ }
    & $CscPath /nologo /target:exe "/out:$testExe" /reference:System.Data.dll $testSource @sources
    if ($LASTEXITCODE -ne 0) { throw 'Regresyon testi derlenemedi.' }
    & $testExe
    if ($LASTEXITCODE -ne 0) { throw 'Regresyon testi başarısız.' }
} finally {
    foreach ($testFile in @($testSource, $testExe)) {
        if (Test-Path -LiteralPath $testFile) { Remove-Item -LiteralPath $testFile }
    }
}
