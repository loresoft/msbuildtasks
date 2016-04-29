@echo off
Nuget.exe restore "Source\MSBuild.Community.Tasks.sln"

NuGet.exe install MSBuildTasks -OutputDirectory .\Tools\ -ExcludeVersion -NonInteractive
NuGet.exe install ilmerge -OutputDirectory .\Tools\ -ExcludeVersion -NonInteractive
