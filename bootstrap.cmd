@echo off
NuGet.exe install MSBuildTasks -OutputDirectory .\Tools\ -ExcludeVersion -NonInteractive
NuGet.exe install NUnit.Runners -OutputDirectory .\Tools\ -ExcludeVersion -NonInteractive
NuGet.exe install ilmerge -OutputDirectory .\Tools\ -ExcludeVersion -NonInteractive
