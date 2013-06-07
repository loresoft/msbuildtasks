@echo off

del *.nupkg

SET isodate=%date:~10,4%-%date:~7,2%-%date:~4,2%T%time:~0,2%.%time:~3,2%.%time:~6,2%.%time:~9,2%

"Source\.nuget\NuGet.exe" pack MSBuildTasks.nuspec
"Source\.nuget\NuGet.exe" pack MSBuildTasks.Project.nuspec
copy *.nupkg C:\Projects\NuGet\