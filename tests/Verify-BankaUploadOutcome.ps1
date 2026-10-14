$ErrorActionPreference = 'Stop'

$sourcePath = Join-Path $PSScriptRoot '..\Model\Services\NBYS\EkstreAktarmaImportService.cs'
$source = Get-Content -LiteralPath $sourcePath -Raw
$start = $source.IndexOf('private static ExceptionHelper ExecuteFileImport(')
$end = $source.IndexOf('private static bool IsUniqueViolation(', $start)
if ($start -lt 0 -or $end -lt 0) { throw 'Dosya işlem yordamı alınamadı.' }
$method = $source.Substring($start, $end - $start).Replace('private static ExceptionHelper', 'public static ExceptionHelper')

$code = @'
using System;
using System.Collections.Generic;
using System.Linq;

public sealed class SqlTransactionContext : IDisposable {
    public static readonly List<SqlTransactionContext> All = new List<SqlTransactionContext>();
    public bool Committed, Disposed, LockAcquired;
    public SqlTransactionContext() { All.Add(this); }
    public void AcquireEkstreImportLock() { LockAcquired = true; }
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

function Invoke-ImportCase($name, $success, $duplicates, $skipped, $hasError, $shouldCommit) {
    $result = [ExceptionHelper]::new()
    $result.SuccessCount = $success
    $result.SkippedCount = $skipped
    if ($duplicates -gt 0) {
        1..$duplicates | ForEach-Object { $result.Exceptions.Add([ImportAlreadyProcessedException]::new('önceden işlendi')) }
    }
    if ($hasError) { $result.Exceptions.Add([Exception]::new('HATA SATIRI: tutar geçersiz')) }

    $actual = [UnderTest]::ExecuteFileImport(
        [Func[SqlTransactionContext, ExceptionHelper]] { param($tx) $result },
        'Banka adı <özel>', 'rapor ''2026'' & müşteriler.xlsx')
    $tx = [SqlTransactionContext]::All[[SqlTransactionContext]::All.Count - 1]
    $expectedSuccess = if ($hasError) { 0 } else { $success }
    if ($actual.SuccessCount -ne $expectedSuccess -or $actual.AlreadyProcessedCount -ne $duplicates) {
        throw "${name}: sonuç sayıları beklenenden farklı."
    }
    if ($tx.Committed -ne $shouldCommit -or !$tx.Disposed -or !$tx.LockAcquired) {
        throw "${name}: transaction/kilit kararı beklenenden farklı."
    }
    Write-Output "PASS: $name"
}

Invoke-ImportCase 'boş dosya' 0 0 0 $false $false
Invoke-ImportCase 'yalnız mükerrer' 0 2 0 $false $false
Invoke-ImportCase 'yeni ve mükerrer' 1 1 0 $false $true
Invoke-ImportCase 'satır hatasıyla rollback' 1 0 0 $true $false
Invoke-ImportCase 'dosya içi satır atlama' 0 0 3 $false $false

$successful = [ExceptionHelper]::new(); $successful.SuccessCount = 1
$failed = [ExceptionHelper]::new(); $failed.SuccessCount = 1
$failed.Exceptions.Add([Exception]::new('HATA SATIRI: bozuk kayıt'))
[void][UnderTest]::ExecuteFileImport([Func[SqlTransactionContext, ExceptionHelper]] { param($tx) $successful }, 'A', 'a.xlsx')
[void][UnderTest]::ExecuteFileImport([Func[SqlTransactionContext, ExceptionHelper]] { param($tx) $failed }, 'B', 'b.xlsx')
$transactions = [SqlTransactionContext]::All
if (!$transactions[$transactions.Count - 2].Committed -or $transactions[$transactions.Count - 1].Committed) {
    throw 'Aynı istekteki dosyaların transaction sonuçları birbirinden bağımsız değil.'
}
Write-Output 'PASS: aynı istekte başarılı ve başarısız dosya ayrışması'
