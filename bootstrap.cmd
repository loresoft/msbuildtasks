@echo off
Nuget.exe restore "Source\MSBuild.Community.Tasks.sln"

NuGet.exe install MSBuildTasks -OutputDirectory .\Tools\ -ExcludeVersion -NonInteractive
Nuget.exe install ILRepack.MSBuild.Task -Version 1.1.2 -Source https://www.myget.org/F/sympa-public/api/v3/index.json -OutputDirectory .\Tools\ -ExcludeVersion -NonInteractive
Nuget.exe install Microsoft.Build.Framework -Version 17.5.0 -Framework net2.0 -OutputDirectory .\Tools\ -ExcludeVersion -NonInteractive
Nuget.exe install Microsoft.Build.Utilities.Core -Version 17.5.0 -Framework net2.0 -OutputDirectory .\Tools\ -ExcludeVersion -NonInteractive
