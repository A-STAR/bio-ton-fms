param(
    [Parameter(Position=0,mandatory=$false)]
    [switch]$linux)

<#
    There is still a problem in ef core 6+ version: https://github.com/dotnet/efcore/issues/25555
    They're going to fix it in 7.0 with https://github.com/dotnet/efcore/issues/26798
    
    A tip: using '--configuration Bundle' may help sometimes
#>


Set-Location ..\BioTonFMSApp
if ($linux)
{
    Write-Host "Creating bundle for linux..." -ForegroundColor Blue
    dotnet ef migrations bundle --force --configuration Bundle --self-contained -r linux-x64
}
else
{
    Write-Host "Creating bundle for windows..." -ForegroundColor Blue
    dotnet ef migrations bundle --force --configuration Bundle # --verbose
}

Write-Host "End" -ForegroundColor Blue