$files = Get-ChildItem -Path C:\Users\smiwa\source\repos\ArtemisBankingPro -Filter *.cs -Recurse
foreach ($file in $files) {
    $text = [System.IO.File]::ReadAllText($file.FullName)
    $changed = $false
    
    if ($text.Contains("namespace ArtemisBankingPro.Core.Application.Features.SavingsAccount;") -or $text.Contains("namespace ArtemisBankingPro.Core.Application.Features.SavingsAccount.")) {
        $text = $text.Replace("namespace ArtemisBankingPro.Core.Application.Features.SavingsAccount;", "namespace ArtemisBankingPro.Core.Application.Features.SavingsAccounts;")
        $text = $text.Replace("namespace ArtemisBankingPro.Core.Application.Features.SavingsAccount.", "namespace ArtemisBankingPro.Core.Application.Features.SavingsAccounts.")
        $changed = $true
    }
    
    if ($text.Contains("using ArtemisBankingPro.Core.Application.Features.SavingsAccount;") -or $text.Contains("using ArtemisBankingPro.Core.Application.Features.SavingsAccount.")) {
        $text = $text.Replace("using ArtemisBankingPro.Core.Application.Features.SavingsAccount;", "using ArtemisBankingPro.Core.Application.Features.SavingsAccounts;")
        $text = $text.Replace("using ArtemisBankingPro.Core.Application.Features.SavingsAccount.", "using ArtemisBankingPro.Core.Application.Features.SavingsAccounts.")
        $changed = $true
    }
    
    if ($changed) {
        [System.IO.File]::WriteAllText($file.FullName, $text, [System.Text.Encoding]::UTF8)
    }
}
