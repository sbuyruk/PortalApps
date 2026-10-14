$ErrorActionPreference = 'Stop'
$source = Get-Content (Join-Path $PSScriptRoot '..\Model\Services\NBYS\EkstreAktarmaImportService.cs') -Raw
$start = $source.IndexOf('private static ExceptionHelper ExecuteFileImport(')
$end = $source.IndexOf('private static bool IsUniqueViolation(', $start)
if ($start -lt 0 -or $end -lt 0) { throw 'Dosya işlem yordamı bulunamadı.' }
$method = $source.Substring($start, $end - $start).Replace('private static ExceptionHelper', 'public static ExceptionHelper')
# SQL unique ihlali ayrı bir entegrasyon durumudur; bu test commit/rollback kararını çalıştırır.
$code = @'
using System;
using System.Collections.Generic;
using System.Linq;
public sealed class SqlTransactionContext : IDisposable {
    public static List<SqlTransactionContext> All = new List<SqlTransactionContext>();
    public bool Committed, Disposed, Locked;
    public SqlTransactionContext() { All.Add(this); }
    public void AcquireEkstreImportLock() { Locked = true; }
    public void Complete() { Committed = true; }
    public void Dispose() { Disposed = true; }
}
public class ExceptionHelper {
    public List<Exception> Exceptions = new List<Exception>();
    public int SuccessCount, AlreadyProcessedCount, SkippedCount, ErrorCount;
    public ExceptionHelper() { }
    public ExceptionHelper(Exception error) { Exceptions.Add(error); }
}
public class ImportAlreadyProcessedException : Exception {
    public ImportAlreadyProcessedException(string message) : base(message) { }
    public ImportAlreadyProcessedException(string message, Exception inner) : base(message, inner) { }
}
public static class UnderTest {
    private static bool IsUniqueViolation(Exception error) { return false; }
    private static bool IsAlreadyProcessedException(Exception error) { return error is ImportAlreadyProcessedException; }
'@ + "`n" + $method + "`n}" 
Add-Type -TypeDefinition $code

$cases = @(
    @{ Name='boş dosya'; Added=0; Duplicate=0; Skipped=0; Error=$false; Commit=$false },
    @{ Name='beklenmeyen şablon'; Added=0; Duplicate=0; Skipped=0; Error=$true; Commit=$false },
    @{ Name='uygun satır yok'; Added=0; Duplicate=0; Skipped=3; Error=$false; Commit=$false },
    @{ Name='yalnız mükerrer'; Added=0; Duplicate=2; Skipped=0; Error=$false; Commit=$false },
    @{ Name='yeni ve mükerrer'; Added=1; Duplicate=1; Skipped=0; Error=$false; Commit=$true },
    @{ Name='ortada hatalı satır'; Added=1; Duplicate=0; Skipped=0; Error=$true; Commit=$false },
    @{ Name='tümü başarılı'; Added=2; Duplicate=0; Skipped=0; Error=$false; Commit=$true }
)
foreach ($case in $cases) {
    $result = [ExceptionHelper]::new()
    $result.SuccessCount = $case.Added
    $result.SkippedCount = $case.Skipped
    for ($i=0; $i -lt $case.Duplicate; $i++) {
        $result.Exceptions.Add([ImportAlreadyProcessedException]::new('önceden işlendi'))
    }
    if ($case.Error) { $result.Exceptions.Add([Exception]::new('satır hatası')) }
    $actual = [UnderTest]::ExecuteFileImport([Func[SqlTransactionContext,ExceptionHelper]] { param($tx) $result }, 'Banka', 'dosya.xlsx')
    $tx = [SqlTransactionContext]::All[[SqlTransactionContext]::All.Count-1]
    if ($tx.Committed -ne $case.Commit -or !$tx.Disposed -or !$tx.Locked) { throw "$($case.Name): transaction kararı hatalı" }
    if ($actual.SuccessCount -ne $(if ($case.Error) {0} else {$case.Added})) { throw "$($case.Name): eklenen sayısı hatalı" }
    if ($actual.AlreadyProcessedCount -ne $case.Duplicate) { throw "$($case.Name): mükerrer sayısı hatalı" }
    Write-Output "PASS: $($case.Name)"
}
$good = [ExceptionHelper]::new(); $good.SuccessCount = 1
$bad = [ExceptionHelper]::new(); $bad.SuccessCount = 1
$bad.Exceptions.Add([Exception]::new('hatalı satır'))
$first = [UnderTest]::ExecuteFileImport([Func[SqlTransactionContext,ExceptionHelper]] { param($tx) $good }, 'Banka A', 'a.xlsx')
$firstTx = [SqlTransactionContext]::All[[SqlTransactionContext]::All.Count-1]
$second = [UnderTest]::ExecuteFileImport([Func[SqlTransactionContext,ExceptionHelper]] { param($tx) $bad }, 'Banka B', 'b.xlsx')
$secondTx = [SqlTransactionContext]::All[[SqlTransactionContext]::All.Count-1]
if (!$firstTx.Committed -or $secondTx.Committed -or $first.SuccessCount -ne 1 -or $second.SuccessCount -ne 0) {
    throw 'Aynı istekte iki dosyanın transaction sonuçları birbirinden bağımsız değil.'
}
Write-Output 'PASS: aynı istekte başarılı ve başarısız iki dosya'
