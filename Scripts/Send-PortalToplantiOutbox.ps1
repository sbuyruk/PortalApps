param(
    [Parameter(Mandatory = $true)][string]$WebConfigPath,
    [Parameter(Mandatory = $true)][string]$AssemblyDirectory,
    [ValidateRange(1, 100)][int]$BatchSize = 20
)

# Run with 64-bit Windows PowerShell 5.1 on the SharePoint application server.
$ErrorActionPreference = 'Stop'
try {
    Add-Type -AssemblyName System.Web
    $configFile = Get-Item -LiteralPath $WebConfigPath
    if ($configFile.Name -ne 'web.config') { throw 'İlgili web uygulamasının web.config dosyasını seçin.' }
    $map = New-Object System.Web.Configuration.WebConfigurationFileMap
    $mapping = New-Object System.Web.Configuration.VirtualDirectoryMapping($configFile.DirectoryName, $true)
    $map.VirtualDirectories.Add('/', $mapping)
    $configuration = [System.Web.Configuration.WebConfigurationManager]::OpenMappedWebConfiguration($map, '/')
    $connection = $configuration.ConnectionStrings.ConnectionStrings['TSKGV_Connection']
    if ($null -eq $connection -or [string]::IsNullOrWhiteSpace($connection.ConnectionString)) {
        throw 'Web uygulamasında TSKGV_Connection bağlantısı bulunamadı.'
    }
    foreach ($assembly in @('Utility.dll', 'DAO.dll', 'Model.dll')) {
        $assemblyPath = Join-Path $AssemblyDirectory $assembly
        if (-not (Test-Path -LiteralPath $assemblyPath -PathType Leaf)) { throw 'Gönderim için gerekli uygulama dosyası bulunamadı.' }
        Add-Type -Path $assemblyPath
    }
    $result = [Model.Services.Portal.PortalToplantiOutboxDispatcher]::ProcessPending($connection.ConnectionString, $BatchSize)
    Write-Output ('İşlenen: {0}; gönderimi tamamlanan: {1}; tamamlanamayan: {2}' -f $result.Processed, $result.Sent, $result.Incomplete)
    if ($result.Incomplete -gt 0) { exit 2 }
    exit 0
}
catch {
    # Do not expose web.config contents, connection strings or raw exceptions in task history.
    Write-Error 'Toplantı bildirim görevi tamamlanamadı. Kurulumu, görev hesabını ve veritabanı kuyruğunu kontrol edin.' -ErrorAction Continue
    exit 1
}
