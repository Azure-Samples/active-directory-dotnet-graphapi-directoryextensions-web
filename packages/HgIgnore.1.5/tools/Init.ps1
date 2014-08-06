param($installPath, $toolsPath, $package, $project)

Write-Host "Preparing to copy the default .hgignore"

$hgRoot = hg root

Copy-Item (Join-Path $toolsPath '.hgignore') $hgRoot