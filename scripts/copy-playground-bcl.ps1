param(
    [Parameter(Mandatory = $true)][string]$FrameworkDir,
    [Parameter(Mandatory = $true)][string]$OutputDir
)

$assemblies = @(
    "System.Private.CoreLib",
    "System.Runtime",
    "System.Console",
    "System.Collections",
    "System.Linq",
    "System.Runtime.Extensions",
    "System.Text.Json",
    "Microsoft.CSharp",
    "System.IO",
    "System.Threading"
)

if (-not (Test-Path $FrameworkDir)) {
    Write-Warning "Framework directory not found: $FrameworkDir"
    exit 0
}

New-Item -ItemType Directory -Force -Path $OutputDir | Out-Null

foreach ($name in $assemblies) {
    $pattern = "^$([regex]::Escape($name))\.[a-z0-9]+\.dll$"
    $file = Get-ChildItem -Path $FrameworkDir -Filter "*.dll" |
        Where-Object { $_.Name -match $pattern } |
        Select-Object -First 1

    if ($null -eq $file) {
        Write-Warning "Missing playground BCL assembly: $name"
        continue
    }

    Copy-Item -Path $file.FullName -Destination (Join-Path $OutputDir "$name.dll") -Force
    Write-Host "Copied $($file.Name) -> $name.dll"
}
