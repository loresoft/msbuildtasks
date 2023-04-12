param()

git status --ignored | Select-String "\t" | ForEach-Object { $NewPath = $($_ -replace "\t", ""); Remove-Item -Recurse -Force $PWD\$NewPath }