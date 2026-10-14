$ErrorActionPreference = 'Stop'
[void][System.Reflection.Assembly]::LoadWithPartialName('System.Web')

function Assert-SafeMessage([string]$message) {
    $html = '<div class=''alert alert-danger''>' + [System.Web.HttpUtility]::HtmlEncode($message) + '</div>'
    $scriptLiteral = [System.Web.HttpUtility]::JavaScriptStringEncode($html, $true)
    $decoded = $scriptLiteral | ConvertFrom-Json
    if ($decoded -ne $html) { throw 'Mesaj kodlaması kayıplı.' }
    if ($scriptLiteral.Contains('<') -or $scriptLiteral.Contains('>') -or $scriptLiteral.Contains('</script>')) {
        throw 'Mesaj JavaScript bağlamından kaçamadı.'
    }
}

Assert-SafeMessage 'rapor "özel" <müşteri> & dosya''sı.xlsx'
Assert-SafeMessage "banka`r`nsatır"
Assert-SafeMessage '</script><script>alert(1)</script>'
Write-Output 'PASS: HTML içeriği JavaScript string olarak kayıpsız ve güvenli taşınıyor'
