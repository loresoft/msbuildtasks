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
	Write-Host ("Installing Targets file import into project " + $project.Name)

	$buildProject = Get-MSBuildProject

	$buildProject.Xml.Imports | Where-Object { $_.Project -match "MSBuild.Community.Tasks" } | foreach-object {     
		Write-Host ("Removing old import:      " + $_.Project)
		$buildProject.Xml.RemoveChild($_) 
	}

	$projectItem = Get-ChildItem $project.FullName
	Write-Host ("The current project is:   " + $project.FullName)
	Write-Host ("Project parent directory: " + $projectItem.Directory)
	Write-Host ("Import will be added for: " + $importFile)

	$target = $buildProject.Xml.AddImport( $importFile )

	$project.Save() 

	Write-Host ("Import added!")
}

function Copy-MSBuildTasks($project) {
	$solutionDir = Get-SolutionDir
	$tasksToolsPath = (Join-Path $solutionDir ".build")

	if(!(Test-Path $tasksToolsPath)) {
		mkdir $tasksToolsPath | Out-Null
	}

	Write-Host "Copying MSBuild Community Tasks files to $tasksToolsPath"
	Copy-Item "$toolsPath\MSBuild.Community.Tasks.dll" $tasksToolsPath -Force | Out-Null
	Copy-Item "$toolsPath\MSBuild.Community.Tasks.targets" $tasksToolsPath -Force | Out-Null

	$buildFile = Join-Path $solutionDir "Build.proj"
	
	if(!(Test-Path $buildFile)) {
		Write-Host "Copying Sample Build.proj to $solutionDir"
		Copy-Item "$toolsPath\Build.proj" $solutionDir | Out-Null
	}

	Write-Host "Don't forget to commit the .build folder"
	return "$tasksToolsPath"
}

function Add-Solution-Folder($buildPath) {
	# Get the open solution.
	$solution = Get-Interface $dte.Solution ([EnvDTE80.Solution2])

	# Create the solution folder.
	$buildFolder = $solution.Projects | Where {$_.ProjectName -eq ".build"}
	if (!$buildFolder) {
		$buildFolder = $solution.AddSolutionFolder(".build")
	}

	
	# Add files to solution folder
	$projectItems = Get-Interface $buildFolder.ProjectItems ([EnvDTE.ProjectItems])

	$targetsPath = [IO.Path]::GetFullPath( (Join-Path $buildPath "MSBuild.Community.Tasks.targets") )
	$projectItems.AddFromFile($targetsPath)

	$dllPath = [IO.Path]::GetFullPath( (Join-Path $buildPath "MSBuild.Community.Tasks.dll") )
	$projectItems.AddFromFile($dllPath)

	$projPath = [IO.Path]::GetFullPath( (Join-Path $buildPath "..\Build.proj") )
	$projectItems.AddFromFile($projPath)
}

function Main 
{
	Delete-Temporary-File
	$taskPath = Copy-MSBuildTasks $project
	Add-Solution-Folder $taskPath

	$targetFile = '$(SolutionDir)\.build\MSBuild.Community.Tasks.targets'
	Install-Targets $project $targetFile
}

Main
