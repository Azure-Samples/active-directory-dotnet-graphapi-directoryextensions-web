param($installPath, $toolsPath, $package, $project)

Write-Host "Uninstall in progress"

$hgRoot = hg root

Write-Host "Deleting the following file:"

$hgignore = Join-Path $hgRoot ".hgignore"

Write-Host $hgignore

Remove-Item $hgignore