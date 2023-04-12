param()

$ProjDir = $PSScriptRoot;

if ($null -eq $(Get-Command -Name msbuild.exe -ErrorAction SilentlyContinue)) {
  Write-Error "msbuild.exe not found on path please run through VSBuildTools...";
  exit 1;
}

& "$($ProjDir)\clean.ps1";

& "$($(Get-Command -Name cmd.exe).Path)" /C "$($ProjDir)\bootstrap.cmd";

& "$($(Get-Command -Name msbuild.exe).Path)" "$($ProjDir)\master.proj" "/verbosity:normal";