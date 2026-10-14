$ErrorActionPreference = 'Stop'
$sourcePath = Join-Path $PSScriptRoot '..\Model\Services\NBYS\EkstreAktarmaImportService.cs'
$source = Get-Content -LiteralPath $sourcePath -Raw
$start = $source.IndexOf('private static bool BuKayitDahaOnceGirilmisMiByFisNo(')
$end = $source.IndexOf('public static ExceptionHelper SaveZiraatEkstreFile(', $start)
if ($start -lt 0 -or $end -lt 0) { throw 'Mükerrerlik yordamı bulunamadı.' }
$method = $source.Substring($start, $end - $start).Trim().Replace('private static bool', 'public static bool')

$code = @'
using System;
using System.Collections.Generic;
namespace DAO.Repositories.NBYS {
    public class EkstreAktarmaRepository {
        public EkstreAktarmaRepository(Probe.SqlTransactionContext transaction) { }
    }
}
namespace Probe {
    public class SqlTransactionContext { }
    public class EkstreAktarma { public DateTime BagisTarihi; public decimal Tutar; }
    public class EkstreAktarmaService {
        public static List<EkstreAktarma> Rows;
        public EkstreAktarmaService(DAO.Repositories.NBYS.EkstreAktarmaRepository repository) { }
        public List<EkstreAktarma> GetByBankAndReceipt(string bank, string receipt,
            SqlTransactionContext transaction) { return Rows; }
    }
    public static class UnderTest {
'@ + "`n" + $method + "`n}" + "`n}"
Add-Type -TypeDefinition $code

$date = [DateTime]::new(2026, 1, 2)
$match = [Probe.EkstreAktarma]::new()
$match.BagisTarihi = $date
$match.Tutar = 10.00
$other = [Probe.EkstreAktarma]::new()
$other.BagisTarihi = $date
$other.Tutar = 11.00
$transaction = [Probe.SqlTransactionContext]::new()

foreach ($case in @(
    @{ Name = 'ilk sırada eşleşme'; Rows = @($match, $other); Expected = $true },
    @{ Name = 'son sırada eşleşme'; Rows = @($other, $match); Expected = $true },
    @{ Name = 'eşleşme yok'; Rows = @($other); Expected = $false }
)) {
    [Probe.EkstreAktarmaService]::Rows = [System.Collections.Generic.List[Probe.EkstreAktarma]]::new()
    foreach ($row in $case.Rows) { [Probe.EkstreAktarmaService]::Rows.Add($row) }
    $actual = [Probe.UnderTest]::BuKayitDahaOnceGirilmisMiByFisNo('Akbank', '123', '', $date, 10.00, $transaction)
    if ($actual -ne $case.Expected) { throw "$($case.Name): beklenen $($case.Expected), gerçekleşen $actual" }
    Write-Output "PASS: $($case.Name)"
}
