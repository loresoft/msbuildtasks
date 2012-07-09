param($installPath, $toolsPath, $package, $project)

Import-Module (Join-Path $toolsPath "MSBuild.psm1")

function Uninstall-Targets ( $project )
{
    Write-Host ("Uninstalling MSBuild Community Tasks Targets file import from project " + $project.Name)

    $buildProject = Get-MSBuildProject

    $buildProject.Xml.Imports | Where-Object { $_.Project -match "MSBuild.Community.Tasks" } | foreach-object {
        Write-Host ("Removing old import:      " + $_.Project)
        $buildProject.Xml.RemoveChild($_) 
    }

    $project.Save() 

    Write-Host ("Import removed!")
}

function Remove-MSBuildTasks($project) {
    $solutionDir = Get-SolutionDir
    $tasksToolsPath = (Join-Path $solutionDir "Build")

    if(!(Test-Path $tasksToolsPath)) {
        return
    }

    Write-Host "Removing MSBuild Community Tasks files from $tasksToolsPath"
    Remove-Item "$tasksToolsPath\MSBuild.Community.Tasks.*" | Out-Null

    return '$(SolutionDir)\Build'
}

function Main 
{
    Uninstall-Targets $project
    Remove-MSBuildTasks $project
}

Main
