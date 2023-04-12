param()

$ProjDir = $PSScriptRoot;

git status --ignored | Select-String -Pattern "\t" -AllMatches | Where-Object { $_ -notmatch "modified:" } | ForEach-Object {
  $NewPath = $($_ -replace "\t", "");
  Write-Host "Removing $($ProjDir)\$($NewPath)"
  Remove-Item -Recurse -Force "$($ProjDir)\$($NewPath)"
}