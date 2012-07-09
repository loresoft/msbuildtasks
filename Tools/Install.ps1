param($installPath, $toolsPath, $package, $project)

Import-Module (Join-Path $toolsPath "MSBuild.psm1")

function Delete-Temporary-File 
{
    Write-Host "Delete temporary file"

    $project.ProjectItems | Where-Object { $_.Name -eq 'MSBuild.Community.Tasks.ReadMe.md' } | Foreach-Object {
        Remove-Item ( $_.FileNames(0) )
        $_.Remove() 
    }
}

function Install-Targets ( $project, $importFile )
{
    Write-Host ("Installing MSBuild Community Tasks Targets file import into project " + $project.Name)

    $buildProject = Get-MSBuildProject

    $buildProject.Xml.Imports | Where-Object { $_.Project -match "MSBuild.Community.Tasks" } | foreach-object {
        Write-Host ("Removing old import:      " + $_.Project)
        $buildProject.Xml.RemoveChild($_) 
    }

    Write-Host ("Import will be added for: " + $importFile)
    $target = $buildProject.Xml.AddImport( $importFile )

    $project.Save() 

    Write-Host ("Import added!")
}

function Copy-MSBuildTasks($project) {
    $solutionDir = Get-SolutionDir
    $tasksToolsPath = (Join-Path $solutionDir "Build")

    if(!(Test-Path $tasksToolsPath)) {
        mkdir $tasksToolsPath | Out-Null
    }

    Write-Host "Copying MSBuild Community Tasks files to $tasksToolsPath"
    Copy-Item "$toolsPath\MSBuild.Community.Tasks.dll" $tasksToolsPath -Force | Out-Null
    Copy-Item "$toolsPath\MSBuild.Community.Tasks.targets" $tasksToolsPath -Force | Out-Null

    Write-Host "Don't forget to commit the Build folder"
    return '$(SolutionDir)\Build'
}

function Update-MSBuildTasksProject($project, $tasksPath) {
    Set-MSBuildProperty "MSBuildCommunityTasksPath" '$(SolutionDir)\Build' $project.Name
    Install-Targets $project '$(SolutionDir)\Build\MSBuild.Community.Tasks.targets'
}


function Main 
{
    Delete-Temporary-File
    $taskPath = Copy-MSBuildTasks $project
    
    Update-MSBuildTasksProject $project $taskPath
}

Main
