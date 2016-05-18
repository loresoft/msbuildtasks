# MSBuild Community Tasks

[![Join the chat at https://gitter.im/loresoft/msbuildtasks](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/loresoft/msbuildtasks?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

The MSBuild Community Tasks Project is an open source project for MSBuild tasks.

[![Build status](https://ci.appveyor.com/api/projects/status/16y9eh9swsqn5g8l?svg=true)](https://ci.appveyor.com/project/LoreSoft/msbuildtasks)

[![Build Status](https://travis-ci.org/loresoft/msbuildtasks.svg?branch=master)](https://travis-ci.org/loresoft/msbuildtasks)

[![NuGet Version](https://img.shields.io/nuget/v/MSBuildTasks.svg)](https://nuget.org/packages/MSBuildTasks)


## Download

The latest build can be downloaded from the releases section.
https://github.com/loresoft/msbuildtasks/releases

The MSBuild Community Tasks library is also available on nuget.org via package name `MSBuildTasks`.

To install MSBuildTasks, run the following command in the Package Manager Console

    PM> Install-Package MSBuildTasks
    
More information about NuGet package avaliable at
https://nuget.org/packages/MSBuildTasks

## Development Builds

Development builds are available on the myget.org feed.  A development build is promoted to the main NuGet feed when it's determined to be stable. 

In your Package Manager settings add the following package source for development builds:
http://www.myget.org/F/loresoft/

## Join Project

Please join the MSBuild Community Tasks Project and help contribute in building the tasks. 

Google Group for MSBuild Community Tasks
https://groups.google.com/d/forum/msbuildtasks

## Current Community Tasks

<!-- This is generated from Documentation\TaskList.xslt, and Documentation\TaskDocs.md is generated from TaskDocs.xslt -->
<table border="0" cellpadding="3" cellspacing="0" width="90%" id="tasksTable" xmlns:xs="http://www.w3.org/2001/XMLSchema">
  <tr>
    <th align="left" width="190">
          Task
        </th>
    <th align="left">
          Description
        </th>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#Add">Add</a></td>
    <td>Add numbers</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#AppPoolController">AppPoolController</a></td>
    <td>Allows control for an application pool on a local or remote machine with IIS installed.  The default is 
            to control the application pool on the local machine.  If connecting to a remote machine, you can
            specify the  and  for the task
            to run under.</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#AppPoolCreate">AppPoolCreate</a></td>
    <td>Creates a new application pool on a local or remote machine with IIS installed.  The default is 
            to create the new application pool on the local machine.  If connecting to a remote machine, you can
            specify the  and  for the task
            to run under.</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#AppPoolDelete">AppPoolDelete</a></td>
    <td>Deletes an existing application pool on a local or remote machine with IIS installed.  The default is 
            to delete an existing application pool on the local machine.  If connecting to a remote machine, you can
            specify the  and  for the task
            to run under.</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#AssemblyInfo">AssemblyInfo</a></td>
    <td>Generates an AssemblyInfo files</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#Attrib">Attrib</a></td>
    <td>Changes the attributes of files and/or directories</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#Divide">Divide</a></td>
    <td>Divide numbers</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#EmbedNativeResource">EmbedNativeResource</a></td>
    <td>Embed native (rather than .NET) resource into a DLL or EXE.</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#FileUpdate">FileUpdate</a></td>
    <td>Replace text in file(s) using a Regular Expression.</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#FtpUpload">FtpUpload</a></td>
    <td>Uploads a file using File Transfer Protocol (FTP).</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#FxCop">FxCop</a></td>
    <td>Uses FxCop to analyse managed code assemblies and reports on
            their design best-practice compliance.</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#GetSolutionProjects">GetSolutionProjects</a></td>
    <td>Task to get paths to projects and project names from VS2005 solution file</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#ILMerge">ILMerge</a></td>
    <td>A wrapper for the ILMerge tool.</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#Mail">Mail</a></td>
    <td>Sends an email message</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#Multiple">Multiple</a></td>
    <td>Multiple numbers</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#MV">MV</a></td>
    <td>Moves files on the filesystem to a new location.</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#NDoc">NDoc</a></td>
    <td>Runs the NDoc application.</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#NUnit">NUnit</a></td>
    <td>Run NUnit on a group of assemblies.</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#NUnit3">NUnit3</a></td>
    <td>Run NUnit3.x on a group of assemblies.</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#RegexMatch">RegexMatch</a></td>
    <td>Task to filter an Input list with a Regex expression.
            Output list contains items from Input list that matched given expression</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#RegexReplace">RegexReplace</a></td>
    <td>Task to replace portions of strings within the Input list
            Output list contains all the elements of the Input list after
            performing the Regex Replace.</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#RegistryRead">RegistryRead</a></td>
    <td>Reads a value from the Registry</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#RegistryWrite">RegistryWrite</a></td>
    <td>Writes a value to the Registry</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#Script">Script</a></td>
    <td>Executes code contained within the task.</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#ServiceController">ServiceController</a></td>
    <td>Task that can control a Windows service.</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#ServiceQuery">ServiceQuery</a></td>
    <td>Task that can determine the status of a specified service
            on a target server.</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#Sleep">Sleep</a></td>
    <td>A task for sleeping for a specified period of time.</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#SqlExecute">SqlExecute</a></td>
    <td>Executes a SQL command.</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#Subtract">Subtract</a></td>
    <td>Subtract numbers</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#SvnCheckout">SvnCheckout</a></td>
    <td>Checkout a local working copy of a Subversion repository.</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#SvnClient">SvnClient</a></td>
    <td>Subversion client base class</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#SvnCommit">SvnCommit</a></td>
    <td>Subversion Commit command</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#SvnExport">SvnExport</a></td>
    <td>Export a folder from a Subversion repository</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#SvnInfo">SvnInfo</a></td>
    <td>Run the "svn info" command and parse the output</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#SvnUpdate">SvnUpdate</a></td>
    <td>Subversion Update command</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#SvnVersion">SvnVersion</a></td>
    <td>Summarize the local revision(s) of a working copy.</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#TaskSchema">TaskSchema</a></td>
    <td>A Task that generates a XSD schema of the tasks in an assembly.</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#Time">Time</a></td>
    <td>Gets the current date and time.</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#Unzip">Unzip</a></td>
    <td>Unzip a file to a target directory.</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#Version">Version</a></td>
    <td>Get Version information from file.</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#VssAdd">VssAdd</a></td>
    <td>Task that adds files to a Visual SourceSafe database.</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#VssCheckin">VssCheckin</a></td>
    <td>Task that executes a checkin against a VSS Database.</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#VssCheckout">VssCheckout</a></td>
    <td>Task that executes a checkout of files or projects
            against a Visual SourceSafe database.</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#VssClean">VssClean</a></td>
    <td>Task that can strip the source control information from a
            Visual Studio Solution and subprojects.</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#VssDiff">VssDiff</a></td>
    <td>Task that records differences between the latest version
            of all the items in a Visual SourceSafe project and another version or label
            to a file.</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#VssGet">VssGet</a></td>
    <td>Task that retireves an item or project from a Visual SourceSafe database.</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#VssHistory">VssHistory</a></td>
    <td>Generates an XML file containing details of all changes made
            to a Visual SourceSafe project or file between specified labels or dates.</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#VssLabel">VssLabel</a></td>
    <td>Task that applies a label to a Visual SourceSafe item.</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#VssUndoCheckout">VssUndoCheckout</a></td>
    <td>Task that undoes a checkout of files or projects
            against a Visual SourceSafe database.</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#WebDirectoryCreate">WebDirectoryCreate</a></td>
    <td>Creates a new web directory on a local or remote machine with IIS installed.  The default is 
            to create the new web directory on the local machine.  The physical path is required to already exist
            on the target machine.  If connecting to a remote machine, you can specify the 
            and  for the task to run under.</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#WebDirectoryDelete">WebDirectoryDelete</a></td>
    <td>Deletes a web directory on a local or remote machine with IIS installed.  The default is 
            to delete the web directory on the local machine.  If connecting to a remote machine, you
            can specify the  and  for the
            task to run under.</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#WebDownload">WebDownload</a></td>
    <td>Downloads a resource with the specified URI to a local file.</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#XmlRead">XmlRead</a></td>
    <td>Reads a value from a XML document using a XPath.</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#XmlUpdate">XmlUpdate</a></td>
    <td>Updates a XML document using a XPath.</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#Xslt">Xslt</a></td>
    <td>A task to merge and transform
             a set of xml files.</td>
  </tr>
  <tr>
    <td><a href="Documentation/TaskDocs.md#Zip">Zip</a></td>
    <td>Create a zip file with the files specified.</td>
  </tr>
</table>

    
## Getting Started

In order to use the tasks in this project, you need to import the MSBuild.Community.Tasks.Targets files. 

If you installed the project with the msi installer, you can use the following.

    <Import Project="$(MSBuildExtensionsPath)\MSBuildCommunityTasks\MSBuild.Community.Tasks.Targets"/>

Alternatively if you want to get started with the nuget packages please add the following.
  
    <PropertyGroup>
        <MSBuildCommunityTasksPath>$(SolutionDir)\.build</MSBuildCommunityTasksPath>
    </PropertyGroup>  
 
    <Import Project="$(MSBuildCommunityTasksPath)\MSBuild.Community.Tasks.Targets" />

## License

Copyright (c) 2016, LoreSoft
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

- Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
- Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
- Neither the name of LoreSoft nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
