@echo off
Nuget.exe restore "Source\MSBuild.Community.Tasks.sln"

NuGet.exe install MSBuildTasks -OutputDirectory .\Tools\ -ExcludeVersion -NonInteractive
NuGet.exe install ILRepack.MSBuild.Task -OutputDirectory .\Tools\ -ExcludeVersion -NonInteractive
