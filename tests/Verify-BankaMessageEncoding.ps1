$ErrorActionPreference = 'Stop'
[void][System.Reflection.Assembly]::LoadWithPartialName('System.Web')
$source = Get-Content (Join-Path $PSScriptRoot '..\Utility\HelperClasses\MessageHelper.cs') -Raw
if (!$source.Contains('System.Web.HttpUtility.JavaScriptStringEncode(message, true)')) {
    throw 'MessageHelper JavaScript dizgesini serialize etmiyor.'
}
foreach ($sample in @(
    'dosya"''<>&\',
    "banka`r`nsatır",
    '</script><script>alert(1)</script>'
)) {
    $html = [System.Web.HttpUtility]::HtmlEncode($sample)
    $encoded = [System.Web.HttpUtility]::JavaScriptStringEncode($html, $true)
    $decoded = $encoded | ConvertFrom-Json
    if ($decoded -ne $html -or $encoded.Contains('<') -or $encoded.Contains('>')) {
        throw "Mesaj güvenli ve kayıpsız taşınamadı: $sample"
    }
    Write-Output 'PASS: özel karakterler HTML ve JavaScript bağlamında güvenli'
}
