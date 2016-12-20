
    
        MSBuild.Community.Tasks
    
    
        
## <a id="InstallAspNet">InstallAspNet</a> (<a id="AspNet.InstallAspNet">AspNet.InstallAspNet</a>)
### Description
Installs and register script mappings for ASP.NET
### Example
Install the latest version of ASP.NET on the server:
      
       <InstallAspNet /> 
      
    
* * *

        
        
        
        
        
        
        
        
        
        
        
## <a id="AssemblyInfo">AssemblyInfo</a>
### Description
Generates an AssemblyInfo files
### Example
Generates a common version file.
            
      <AssemblyInfo CodeLanguage="CS"  
                OutputFile="VersionInfo.cs" 
                AssemblyVersion="1.0.0.0" 
                AssemblyFileVersion="1.0.0.0" />
Generates a complete version file.
            
      <AssemblyInfo CodeLanguage="CS"  
                OutputFile="$(MSBuildProjectDirectory)\Test\GlobalInfo.cs" 
                AssemblyTitle="AssemblyInfoTask" 
                AssemblyDescription="AssemblyInfo Description"
                AssemblyConfiguration=""
                AssemblyCompany="Company Name, LLC"
                AssemblyProduct="AssemblyInfoTask"
                AssemblyCopyright="Copyright (c) Company Name, LLC 2006"
                AssemblyTrademark=""
                ComVisible="false"
                CLSCompliant="true"
                Guid="d038566a-1937-478a-b5c5-b79c4afb253d"
                AssemblyVersion="1.0.0.0" 
                AssemblyFileVersion="1.0.0.0" />
Generates a complete version file for C++/CLI.
            
      <AssemblyInfo CodeLanguage="CPP"  
                OutputFile="$(MSBuildProjectDirectory)\Properties\AssemblyInfo.cpp"
                AssemblyTitle="MyAssembly" 
                AssemblyDescription="MyAssembly Description"
                AssemblyConfiguration="$(Configuration)"
                AssemblyCompany="Company Name, LLC"
                AssemblyProduct="MyAssembly"
                AssemblyCopyright="Copyright (c) Company Name, LLC 2008"
                AssemblyTrademark=""
                ComVisible="false"
                CLSCompliant="true"
                Guid="d038566a-1937-478a-b5c5-b79c4afb253d"
                AssemblyVersion="1.0.0.0" 
                AssemblyFileVersion="1.0.0.0"
                UnmanagedCode="true" />
            
            
* * *

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
## <a id="Attrib">Attrib</a>
### Description
Changes the attributes of files and/or directories
### Example
Make file Readonly, Hidden and System.
            
      <Attrib Files="Test\version.txt" 
                ReadOnly="true" Hidden="true" System="true"/>
Clear Hidden and System attributes.
            
      <Attrib Files="Test\version.txt" 
                Hidden="false" System="false"/>
Make file Normal.
            
      <Attrib Files="Test\version.txt" 
                Normal="true"/>
            
            
* * *

        
        
        
        
        
        
        
        
        
        
        
## <a id="ByteDescriptions">ByteDescriptions</a>
### Description
Describes certain byte measurements as nice strings.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
## <a id="Computer">Computer</a>
### Description
Provides information about the build computer.
### Example
Get build computer information.
            
      <Computer>
      <Output TaskParameter="Name" PropertyName="BuildMachineName" />
      <Output TaskParameter="IPAddress" PropertyName="BuildMachineIPAddress" />
      <Output TaskParameter="IPAddressV4" PropertyName="BuildMachineIPAddressV4" />
      <Output TaskParameter="OSPlatform" PropertyName="BuildMachineOSPlatform" />
      <Output TaskParameter="OSVersion" PropertyName="BuildMachineOSVersion" />
      </Computer>
            
            
* * *

        
        
        
        
        
        
        
        
## <a id="Beep">Beep</a>
### Description
A task to play the sound of a beep through the console speaker.
### Example
To play the sound of a beep at a frequency of 800 hertz and for a duration of 200 milliseconds, use
      
      <Beep />
      
    
* * *

        
        
        
        
## <a id="DeleteTree">DeleteTree</a>
### Description
Delete a directory tree.  This task supports wild card directory selection.
### Example
Delete all bin and obj directories.
            
      <DeleteTree Directories="**\bin;**\obj" />
Delete all bin and obj directories that start with MSBuild.Community.
            
      <DeleteTree Directories="MSBuild.Community.*\**\bin;MSBuild.Community.*\**\obj" />
            
            
* * *

        
        
        
        
        
        
## <a id="DependencyGraph">DependencyGraph</a> (<a id="DependencyGraph.DependencyGraph">DependencyGraph.DependencyGraph</a>)
### Description
Reads a set of project files (.csproj, .vbproj) in InputFiles and generate a GraphViz style syntax.
             You can paste the result of the graphs in places like http://graphviz-dev.appspot.com/ to see your chart or
             run the file using the GraphViz tool http://www.graphviz.org/
### Example

             
       <ItemGroup>
       <Dependency Include="Project01.csproj" />
       </ItemGroup>
            
       <Target Name="Default">
       <DependencyGraph InputFiles="@(Dependency)" IsIncludeProjectDependecies="true" ExcludeReferences="^System" />
       </Target>
Result:
                 digraph {
                     subgraph ProjectReferences {
                         node [shape=box];
                         "{4993C164-5F2A-4831-A5B1-E5E579C76B28}" [label="Project01"];
                         "{1B5D5300-8070-48DB-8A81-B39764231954}" [label="Project03"];
                         "{E7D8035C-3CEA-4D9C-87FD-0F5C0DB5F592}" [label="Project02"];
                         "{7DBCDEE7-D048-432E-BEEB-928E362E3063}" [label="Project03"];
                     }
                     "{4993C164-5F2A-4831-A5B1-E5E579C76B28}" -> "Microsoft.CSharp";
                     "{1B5D5300-8070-48DB-8A81-B39764231954}" -> "Microsoft.CSharp";
                     "{E7D8035C-3CEA-4D9C-87FD-0F5C0DB5F592}" -> "Microsoft.CSharp";
                     "{7DBCDEE7-D048-432E-BEEB-928E362E3063}" -> "Microsoft.CSharp";
                     "{4993C164-5F2A-4831-A5B1-E5E579C76B28}" -> "{1B5D5300-8070-48DB-8A81-B39764231954}";
                     "{4993C164-5F2A-4831-A5B1-E5E579C76B28}" -> "{E7D8035C-3CEA-4D9C-87FD-0F5C0DB5F592}";
                     "{E7D8035C-3CEA-4D9C-87FD-0F5C0DB5F592}" -> "{7DBCDEE7-D048-432E-BEEB-928E362E3063}";
            	}    
             
* * *

        
        
        
        
        
        
        
## <a id="ProjectFileParser">ProjectFileParser</a> (<a id="DependencyGraph.ProjectFileParser">DependencyGraph.ProjectFileParser</a>)
### Description
Very simple parser that gets reference and assembly name information from project files
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
        
## <a id="BaseReference">BaseReference</a> (<a id="DependencyGraph.BaseReference">DependencyGraph.BaseReference</a>)
### Description
Base class for all references
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
## <a id="AssemblyReference">AssemblyReference</a> (<a id="DependencyGraph.AssemblyReference">DependencyGraph.AssemblyReference</a>)
### Description
Represents an assembly reference inside a project file
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
## <a id="ProjectReference">ProjectReference</a> (<a id="DependencyGraph.ProjectReference">DependencyGraph.ProjectReference</a>)
### Description
Represents a project reference inside a project file
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
        
        
        
## <a id="EmbedNativeResource">EmbedNativeResource</a>
### Description
A task for embedded native resource.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
## <a id="FtpUpload">FtpUpload</a>
### Description
Uploads a group of files using File Transfer Protocol (FTP).
### Example
Upload a file.
            
      <FtpUpload 
                LocalFile="MSBuild.Community.Tasks.zip" 
                RemoteUri="ftp://localhost/" />
Upload all the files in an ItemGroup:
            
      <FtpUpload
                Username="username"
                Password="password"
                UsePassive="true"
                RemoteUri="ftp://webserver.com/httpdocs/"
                LocalFiles="@(FilesToUpload)"
                RemoteFiles="@(FilesToUpload->'%(RecursiveDir)%(Filename)%(Extension)')" />
            
            
* * *

        
## <a id="IFtpWebRequestCreator">IFtpWebRequestCreator</a>
### Description
Describes a factory for IFtpWebRequest.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
        
        
        
        
        
        
        
        
## <a id="IFtpWebRequest">IFtpWebRequest</a>
### Description
This class references an interface that looks like FtpWebRequest
            in order to support unit testing without an actual FTP Server.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
## <a id="RealFtpWebRequest">RealFtpWebRequest</a>
### Description
An adapter to make the real FtpWebRequest look like
            an IFtpWebRequest.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
## <a id="FtpClientTaskBase">FtpClientTaskBase</a> (<a id="Ftp.FtpClientTaskBase">Ftp.FtpClientTaskBase</a>)
### Description
Ftp client base class.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
## <a id="FtpCreateRemoteDirectory">FtpCreateRemoteDirectory</a> (<a id="Ftp.FtpCreateRemoteDirectory">Ftp.FtpCreateRemoteDirectory</a>)
### Description
Creates a full remote directory on the remote server if not exists using the File Transfer Protocol (FTP).
            This can be one directory or a full path to create.
### Example
Create remote directory:
            
      <FtpCreateRemoteDirectoty 
                ServerHost="ftp.myserver.com"
                Port="42"
                RemoteDirectory="Directory\Subdirectory\MyOtherSubdirectory"
                Username="user"
                Password="p@ssw0rd"
            />
            
            
* * *

        
        
        
        
## <a id="FtpDirectoryExists">FtpDirectoryExists</a> (<a id="Ftp.FtpDirectoryExists">Ftp.FtpDirectoryExists</a>)
### Description
Determ if a remote directory exists on a FTP server or not.
### Example
Determ of Directory\1 exists:
            
      <Target Name="CheckIfDirectoryExists">
      <FtpDirectoryExists 
                    ServerHost="ftp.myserver.com"
                    Port="42"
                    RemoteDirectory="1\2\3"
                    Username="user"
                    Password="p@ssw0rd"
                >
      <Output TaskParameter="Exists" PropertyName="Exists" /> 
      </FtpDirectoryExists>
      <Message Text="Directory '1\2\3' exists: $(Exists)"/>
If the directory exists on the server you should see the following output in the console:
            Directory '1\2\3' exists: true
            
* * *

        
        
        
        
        
        
## <a id="FtpException">FtpException</a> (<a id="Ftp.FtpException">Ftp.FtpException</a>)
### Description
Exception returned by FTP server.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
## <a id="FtpEntry">FtpEntry</a> (<a id="Ftp.FtpEntry">Ftp.FtpEntry</a>)
### Description
Represents an remote file or directory on a FTP server.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
        
        
## <a id="FtpReply">FtpReply</a> (<a id="Ftp.FtpReply">Ftp.FtpReply</a>)
### Description
Represenatation of a FTP reply message.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
## <a id="FtpUploadDirectoryContent">FtpUploadDirectoryContent</a> (<a id="Ftp.FtpUploadDirectoryContent">Ftp.FtpUploadDirectoryContent</a>)
### Description
Uploads a full directory content to a remote directory.
### Example
Uploads directory content, including all subdirectories and subdirectory content:
            
      <Target Name="DeployWebsite">
      <FtpUploadDirectoryContent 
                    ServerHost="ftp.myserver.com"
                    Port="42"
                    Username="user"
                    Password="p@ssw0rd"
                    LocalDirectory="c:\build\mywebsite"
                    RemoteDirectory="root\www\mywebsite"
                    Recursive="true"
                />
To go a little step further. If the local directory looked like this:
            
            [mywebsite]
                [images]
                    1.gif
                    2.gif
                    3.gif
                [js]
                    clientscript.js
                    nofocus.js
                [css]
                    print.css
                    main.css
                index.htm
                contact.htm
                downloads.htm
            
            All directories and there content will be uploaded and a excact copy of the content of mywebsite directory will be created remotely.
            
            If  is set the false; only index.htm, contact.htm and downloads.htm will be uploaded and no subdirectories will be created remotely.
            
            
* * *

        
        
        
        
        
        
        
        
## <a id="UninstallStatus">UninstallStatus</a> (<a id="Fusion.UninstallStatus">Fusion.UninstallStatus</a>)
### Description
The status of an uninstall.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
        
        
## <a id="FusionWrapper">FusionWrapper</a> (<a id="Fusion.FusionWrapper">Fusion.FusionWrapper</a>)
### Description
A class wrapping fusion api calls
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
## <a id="GitCommitDate">GitCommitDate</a> (<a id="Git.GitCommitDate">Git.GitCommitDate</a>)
### Description
A task for git to get the current commit datetime.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
## <a id="GitClient">GitClient</a> (<a id="Git.GitClient">Git.GitClient</a>)
### Description
A task for Git commands.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
## <a id="GitDescribe">GitDescribe</a> (<a id="Git.GitDescribe">Git.GitDescribe</a>)
### Description
A task for git to get the most current tag, commit count since tag, and commit hash.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
        
        
        
        
        
## <a id="GitBranch">GitBranch</a> (<a id="Git.GitBranch">Git.GitBranch</a>)
### Description
A task to get the name of the branch or tag of git repository
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
        
## <a id="GitCommits">GitCommits</a> (<a id="Git.GitCommits">Git.GitCommits</a>)
### Description
A task for git to retrieve the number of commits on a revision.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
## <a id="GitPendingChanges">GitPendingChanges</a> (<a id="Git.GitPendingChanges">Git.GitPendingChanges</a>)
### Description
A task for git to detect if there are pending changes
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
## <a id="GitVersion">GitVersion</a> (<a id="Git.GitVersion">Git.GitVersion</a>)
### Description
A task for git to get the current commit hash.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
        
        
## <a id="ChmCompiler">ChmCompiler</a> (<a id="HtmlHelp.ChmCompiler">HtmlHelp.ChmCompiler</a>)
### Description
Html Help 1x compiler task.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
        
        
## <a id="HxCompiler">HxCompiler</a> (<a id="HtmlHelp.HxCompiler">HtmlHelp.HxCompiler</a>)
### Description
A Html Help 2.0 compiler task.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
## <a id="InnoSetup">InnoSetup</a>
### Description
MSBuild task to create installer with InnoSetup
### Example
Create installer
            
      <InnoSetup 
                    ScriptFile="setup.iss"
                    OutputFileName="MySetup.exe"
                    OutputPath="C:\SetupDir"
                    Quiet="True" />
            
            
* * *

        
        
        
        
        
        
        
        
        
        
        
## <a id="CssCompress">CssCompress</a> (<a id="JavaScript.CssCompress">JavaScript.CssCompress</a>)
### Description
MSBuild task to minimize the size of a css file.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
## <a id="CssCompressor">CssCompressor</a> (<a id="JavaScript.CssCompressor">JavaScript.CssCompressor</a>)
### Description
Work in progress ...
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
## <a id="MergeModes">MergeModes</a>
### Description
Defines the modes for merging files.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
## <a id="Merge">Merge</a>
### Description
Merge files into the destination file.
### Example
Merge CSS files together for better browser performance.
            
      <Merge Mode="TextLine" 
                SourceFiles="Main.css;Login.css" 
                DestinationFile="All.css" />
            
            
* * *

        
        
        
        
        
## <a id="HttpRequest">HttpRequest</a> (<a id="Net.HttpRequest">Net.HttpRequest</a>)
### Description
Makes an HTTP request, optionally validating the result and writing it to a file.
### Example
Example of a update request ensuring "Database upgrade check completed successfully." was returned.
            
      <HttpRequest Url="http://mydomain.com/index.php?checkdb=1" 
                    EnsureResponseContains="Database upgrade check completed successfully." 
                    FailOnNon2xxResponse="true" />
            
            
* * *

        
        
        
        
        
        
        
        
        
        
        
## <a id="NuGetBase">NuGetBase</a> (<a id="NuGet.NuGetBase">NuGet.NuGetBase</a>)
### Description
A base class for NuGet tasks.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
        
## <a id="NuGetDelete">NuGetDelete</a> (<a id="NuGet.NuGetDelete">NuGet.NuGetDelete</a>)
### Description
Deletes a package with a specific version. It can be useful if the server has disallow 
            to overwrite existing packages.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
        
        
## <a id="NuGetInstall">NuGetInstall</a> (<a id="NuGet.NuGetInstall">NuGet.NuGetInstall</a>)
### Description
Installs a package using the specified sources.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
        
        
        
        
        
        
## <a id="NuGetPack">NuGetPack</a> (<a id="NuGet.NuGetPack">NuGet.NuGetPack</a>)
### Description
Creates a NuGet package based on the specified nuspec or project file.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
## <a id="NuGetPush">NuGetPush</a> (<a id="NuGet.NuGetPush">NuGet.NuGetPush</a>)
### Description
Pushes a package to the server and optionally publishes it.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
## <a id="NuGetRestore">NuGetRestore</a> (<a id="NuGet.NuGetRestore">NuGet.NuGetRestore</a>)
### Description
Downloads and unzips (restores) any packages missing from the packages folder.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
        
        
        
        
## <a id="NuGetUpdate">NuGetUpdate</a> (<a id="NuGet.NuGetUpdate">NuGet.NuGetUpdate</a>)
### Description
Updates packages
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
        
        
        
        
        
        
        
        
## <a id="RegexCompiler">RegexCompiler</a>
### Description
Compiles regular expressions and saves them to disk in an assembly.
### Example
Creates an assembly with the compiled regular expressions.
      
        
  <ItemGroup>
    <RegexPatterns Include="TextRegex">
      <Pattern>\G[^&lt;]+</Pattern>
      <Options>RegexOptions.Singleline | RegexOptions.Multiline</Options>
    </RegexPatterns>
    <RegexPatterns Include="CommentRegex">
      <Pattern>\G&lt;%--(([^-]*)-)*?-%&gt;</Pattern>
      <Options>RegexOptions.Singleline | RegexOptions.Multiline</Options>
    </RegexPatterns>
    <RegexPatterns Include="CodeRegex">
      <Pattern>\G&lt;%(?![@%])(?&lt;code&gt;.*?)%&gt;</Pattern>
      <Options>RegexOptions.Singleline | RegexOptions.Multiline</Options>
      <Namespace>MSBuild.Community.RegularExpressions</Namespace>
    </RegexPatterns>
  </ItemGroup>

  <Target Name="RegexCompiler">
    <RegexCompiler
      OutputDirectory="Test"
      RegularExpressions="@(RegexPatterns)"
      Namespace="MSBuild.RegularExpressions"
      AssemblyName="MSBuild.RegularExpressions.dll"
      AssemblyTitle="MSBuild.RegularExpressions"
      AssemblyDescription="MSBuild Community Tasks Regular Expressions"
      AssemblyCompany="Company Name, LLC"
      AssemblyProduct="MSBuildTasks"
      AssemblyCopyright="Copyright (c) MSBuildTasks 2008"
      AssemblyVersion="1.0.0.0"
      AssemblyFileVersion="1.0.0.0" />
  </Target>

      
    
* * *

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
## <a id="BuildAssembler">BuildAssembler</a> (<a id="Sandcastle.BuildAssembler">Sandcastle.BuildAssembler</a>)
### Description
BuildAssembler task for Sandcastle.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
## <a id="SandcastleToolBase">SandcastleToolBase</a> (<a id="Sandcastle.SandcastleToolBase">Sandcastle.SandcastleToolBase</a>)
### Description
A base class for Sandcastle Tools,
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
## <a id="ChmBuilder">ChmBuilder</a> (<a id="Sandcastle.ChmBuilder">Sandcastle.ChmBuilder</a>)
### Description
ChmBuilder task for Sandcastle.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
        
        
        
## <a id="DBCSFix">DBCSFix</a> (<a id="Sandcastle.DBCSFix">Sandcastle.DBCSFix</a>)
### Description
DBCSFix task for Sandcastle.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
## <a id="MRefBuilder">MRefBuilder</a> (<a id="Sandcastle.MRefBuilder">Sandcastle.MRefBuilder</a>)
### Description
MRefBuilder task for Sandcastle.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
        
        
## <a id="Sandcastle">Sandcastle</a> (<a id="Sandcastle.Sandcastle">Sandcastle.Sandcastle</a>)
### Description
The Sandcastle task.
### Example
Create the Html Help for MSBuild Community Task project.
            
      <Sandcastle TopicStyle="vs2005"
                WorkingDirectory="$(MSBuildProjectDirectory)\Help"
                Assemblies="@(Assemblies)"
                Comments="@(Comments)"
                References="@(References)"
                ChmName="MSBuildTasks"
                HxName="MSBuildTasks" />
            
            
* * *

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
## <a id="SandcastleEnviroment">SandcastleEnviroment</a> (<a id="Sandcastle.SandcastleEnviroment">Sandcastle.SandcastleEnviroment</a>)
### Description
A class representing the sandcastle enviroment.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
        
## <a id="XslTransform">XslTransform</a> (<a id="Sandcastle.XslTransform">Sandcastle.XslTransform</a>)
### Description
XslTransform task for Sandcastle.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
        
## <a id="IFilesSystem">IFilesSystem</a> (<a id="Services.IFilesSystem">Services.IFilesSystem</a>)
### Description
The contract for a service that will provide access to the file system.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
## <a id="FileSystem">FileSystem</a> (<a id="Services.FileSystem">Services.FileSystem</a>)
### Description
Provides access to the file system.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
## <a id="GacUtilCommands">GacUtilCommands</a>
### Description
The list of the commands available to the GacUtil Task
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
## <a id="GacUtil">GacUtil</a>
### Description
MSBuild task to install and uninstall assemblies into the GAC
### Example
Install a dll into the GAC.
            
      <GacUtil 
                    Command="Install" 
                    Assemblies="MSBuild.Community.Tasks.dll" 
                    Force="true" />
            
            
* * *

        
        
        
        
        
        
        
        
        
        
        
        
        
## <a id="JSCompress">JSCompress</a> (<a id="JavaScript.JSCompress">JavaScript.JSCompress</a>)
### Description
Compresses JavaScript source by removing comments and unnecessary 
            whitespace. It typically reduces the size of the script by half, 
            resulting in faster downloads and code that is harder to read.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
## <a id="AddTnsName">AddTnsName</a> (<a id="Oracle.AddTnsName">Oracle.AddTnsName</a>)
### Description
Defines a database host within the Oracle TNSNAMES.ORA file.
### Example
Add an entry to the system default TNSNAMES.ORA file and update any entry that already exists:
       <AddTnsName AllowUpdates="True" EntryName="northwind.world" EntryText="(DESCRIPTION = (ADDRESS_LIST = (ADDRESS = (PROTOCOL = TCP)(HOST = northwinddb01)(PORT = 1521))) (CONNECT_DATA = (SERVICE_NAME = northwind.world)))"  /> 
            
            
* * *

        
        
        
        
        
        
        
        
        
        
        
        
## <a id="TnsEntry">TnsEntry</a> (<a id="Oracle.TnsEntry">Oracle.TnsEntry</a>)
### Description
Contains information about a TNS definition
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
## <a id="TnsParser">TnsParser</a> (<a id="Oracle.TnsParser">Oracle.TnsParser</a>)
### Description
Locates host entries within a TNSNAMES.ORA file
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
## <a id="RoboCopy">RoboCopy</a>
### Description
Task wrapping the Window Resource Kit Robocopy.exe command.
### Example
Deploy website to web server.
            
      <RoboCopy 
                SourceFolder="$(MSBuildProjectDirectory)" 
                DestinationFolder="\\server\webroot\" 
                Mirror="true"
                ExcludeFolders=".svn;obj;Test"
                ExcludeFiles="*.cs;*.resx;*.csproj;*.webinfo;*.log"
                NoJobHeader="true"
            />  
            
            
* * *

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
## <a id="Sound">Sound</a>
### Description
A task to play a sound from a .wav file path or URL.
### Example
To play the windows XP startup sound, use
      
      <Sound SystemSoundFile="..\Media\Windows XP Startup.wav" />
      
    
* * *

        
        
        
        
        
        
        
## <a id="FileBase">FileBase</a> (<a id="SourceServer.FileBase">SourceServer.FileBase</a>)
### Description
A base class that has a file.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
## <a id="PdbStrCommands">PdbStrCommands</a> (<a id="SourceServer.PdbStrCommands">SourceServer.PdbStrCommands</a>)
### Description
Commands for the  tasks.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
## <a id="PdbStr">PdbStr</a> (<a id="SourceServer.PdbStr">SourceServer.PdbStr</a>)
### Description
A task for the pdbstr from source server.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
        
        
## <a id="SourceFile">SourceFile</a> (<a id="SourceServer.SourceFile">SourceServer.SourceFile</a>)
### Description
A class representing a source file.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
## <a id="SourceIndexBase">SourceIndexBase</a> (<a id="SourceServer.SourceIndexBase">SourceServer.SourceIndexBase</a>)
### Description
A base class for source indexing a pdb symbol file.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
## <a id="SrcTool">SrcTool</a> (<a id="SourceServer.SrcTool">SourceServer.SrcTool</a>)
### Description
A task for the srctool from source server.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
## <a id="SvnSourceIndex">SvnSourceIndex</a> (<a id="SourceServer.SvnSourceIndex">SourceServer.SvnSourceIndex</a>)
### Description
A subversion source index task.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
## <a id="SymbolFile">SymbolFile</a> (<a id="SourceServer.SymbolFile">SourceServer.SymbolFile</a>)
### Description
A class representing a symbol file.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
## <a id="TfsSourceIndex">TfsSourceIndex</a> (<a id="SourceServer.TfsSourceIndex">SourceServer.TfsSourceIndex</a>)
### Description
Task to index pdb files and entries to retrieve source files from Team Foundation Server source control.
### Example
Index a PDB.
            
      <TfsSourceIndex SymbolFiles="@(Symbols)" TeamProjectCollectionUri="http://my-tfsserver/tfs/DefaultCollection" />
            
            
* * *

        
        
        
        
        
        
        
        
## <a id="SqlPubCommands">SqlPubCommands</a> (<a id="SqlServer.SqlPubCommands">SqlServer.SqlPubCommands</a>)
### Description
The SqlPubWiz commands
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
## <a id="SqlPubWiz">SqlPubWiz</a> (<a id="SqlServer.SqlPubWiz">SqlServer.SqlPubWiz</a>)
### Description
The Database Publishing Wizard enables the deployment of
            SQL Server databases (both schema and data) into a shared
            hosting environment.
### Example
Generate the database script for Northwind on localhost.
            
      <SqlPubWiz 
                Database="Northwind" 
                Output="Northwind.sql" 
                SchemaOnly="true" />
            
            
* * *

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
## <a id="SvnCopy">SvnCopy</a> (<a id="Subversion.SvnCopy">Subversion.SvnCopy</a>)
### Description
Copy a file or folder in Subversion
### Example
Create a tag of the trunk with the current Cruise Control build number:
            
      <Target Name="TagTheBuild">
      <SvnCopy SourcePath="file:///d:/svn/repo/Test/trunk"
                       DestinationPath="file:///d:/svn/repo/Test/tags/BUILD-$(CCNetLabel)" 
                       Message="Automatic build of $(CCNetProject)" />      
      </Target>
            
            
* * *

        
## <a id="SvnClient">SvnClient</a> (<a id="Subversion.SvnClient">Subversion.SvnClient</a>)
### Description
Subversion client base class
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
## <a id="Info">Info</a> (<a id="Subversion.Info">Subversion.Info</a>)
### Description

### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
## <a id="Entry">Entry</a> (<a id="Subversion.Entry">Subversion.Entry</a>)
### Description

### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
        
        
        
## <a id="EntryCollection">EntryCollection</a> (<a id="Subversion.EntryCollection">Subversion.EntryCollection</a>)
### Description

### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
## <a id="Repository">Repository</a> (<a id="Subversion.Repository">Subversion.Repository</a>)
### Description

### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
## <a id="WorkingCopy">WorkingCopy</a> (<a id="Subversion.WorkingCopy">Subversion.WorkingCopy</a>)
### Description

### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
## <a id="LastCommit">LastCommit</a> (<a id="Subversion.LastCommit">Subversion.LastCommit</a>)
### Description

### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
## <a id="SvnStatus">SvnStatus</a> (<a id="Subversion.SvnStatus">Subversion.SvnStatus</a>)
### Description
Subversion status command.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
## <a id="SymStoreCommands">SymStoreCommands</a> (<a id="SymbolServer.SymStoreCommands">SymbolServer.SymStoreCommands</a>)
### Description
Commands for the SymStore tasks.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
## <a id="SymStore">SymStore</a> (<a id="SymbolServer.SymStore">SymbolServer.SymStore</a>)
### Description
Task that wraps the Symbol Server SymStore.exe application.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
## <a id="InfoCommandResponse">InfoCommandResponse</a> (<a id="Tfs.InfoCommandResponse">Tfs.InfoCommandResponse</a>)
### Description
Represents the response from a tf.exe info command
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
## <a id="ServerInformation">ServerInformation</a> (<a id="Tfs.ServerInformation">Tfs.ServerInformation</a>)
### Description
Represents the server information section created by a tf.exe info command
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
        
        
        
        
        
        
## <a id="LocalInformation">LocalInformation</a> (<a id="Tfs.LocalInformation">Tfs.LocalInformation</a>)
### Description
Represents the local information section from a tf.exe info command
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
## <a id="TfsClient">TfsClient</a> (<a id="Tfs.TfsClient">Tfs.TfsClient</a>)
### Description
A task for Team Foundation Server version control.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
## <a id="IRegistry">IRegistry</a> (<a id="Services.IRegistry">Services.IRegistry</a>)
### Description
The contract for a service that will provide access to the registry.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
## <a id="Win32Registry">Win32Registry</a> (<a id="Services.Win32Registry">Services.Win32Registry</a>)
### Description
Provides access to the Windows registry.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
## <a id="User">User</a>
### Description
Provides information about the build user.
### Example
Get build user information.
            
      <User>
      <Output TaskParameter="UserNameWithDomain" PropertyName="BuildUserID" />
      <Output TaskParameter="FullName" PropertyName="BuildUserName" />
      <Output TaskParameter="Email" PropertyName="BuildUserEmail" />
      <Output TaskParameter="Phone" PropertyName="BuildUserPhone" />
      </User>    
            
            
* * *

        
        
        
        
        
        
        
        
        
        
        
        
## <a id="WebUpload">WebUpload</a>
### Description
Upload a local file to a remote URI.
### Example
Upload the xml file.
            
      <WebUpload RemoteUri="http://intranet/upload" FileName="page.xml" />
            
            
* * *

        
        
        
        
        
        
        
        
        
## <a id="XmlMassUpdate">XmlMassUpdate</a> (<a id="Xml.XmlMassUpdate">Xml.XmlMassUpdate</a>)
### Description
Performs multiple updates on an XML file
### Example
These examples will demonstrate how to make multiple updates to a XML file named web.config. It looks like:
        
      <?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <appSettings>
    <add key="ItemsPerPage" value="10" />
    <add key="EnableCaching" value="true" />
    <add key="DefaultServer" value="TIGRIS" />
  </appSettings>
    <system.web>
      <compilation defaultLanguage="c#" debug="true" />
      <customErrors mode="Off" />
      <trace enabled="true" requestLimit="10" pageOutput="true" />
      <globalization requestEncoding="utf-8" responseEncoding="utf-8" />
    </system.web>
</configuration> 
        
      
    
* * *

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
## <a id="WebDirectoryScriptMap">WebDirectoryScriptMap</a> (<a id="IIS.WebDirectoryScriptMap">IIS.WebDirectoryScriptMap</a>)
### Description
Sets an application mapping for a filename extension on an existing web directory.
### Example
Map the .axd extension to the lastest version of ASP.NET:
            
      <WebDirectoryScriptMap VirtualDirectoryName="MyWeb" Extension=".axd" MapToAspNet="True" VerifyFileExists="False" />
            
            
* * *

        
## <a id="WebBase">WebBase</a> (<a id="IIS.WebBase">IIS.WebBase</a>)
### Description
Base task for any IIS-related task.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
## <a id="IISVersion">IISVersion</a> (<a id="IIS.WebBase.IISVersion">IIS.WebBase.IISVersion</a>)
### Description
Defines the possible IIS versions supported by the task.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
        
        
## <a id="ApplicationPoolAction">ApplicationPoolAction</a> (<a id="IIS.WebBase.ApplicationPoolAction">IIS.WebBase.ApplicationPoolAction</a>)
### Description
Defines the possible application pool actions to be performed.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
## <a id="IIS7ApplicationPoolState">IIS7ApplicationPoolState</a> (<a id="IIS.WebBase.IIS7ApplicationPoolState">IIS.WebBase.IIS7ApplicationPoolState</a>)
### Description
Defines the current application pool state.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
        
        
        
        
        
        
        
        
## <a id="WebDirectorySetting">WebDirectorySetting</a> (<a id="IIS.WebDirectorySetting">IIS.WebDirectorySetting</a>)
### Description
Reads and modifies a web directory configuration setting.
### Example
Display the file system path of the MyWeb web directory:
            
      <WebDirectorySetting VirtualDirectoryName="MyWeb" SettingName="Path">
      <Output TaskParameter="SettingValue" PropertyName="LocalPath" />
      </WebDirectorySetting>
      <Message Text="MyWeb is located at $(LocalPath)" />
            
            
* * *

        
        
        
        
        
## <a id="ILMerge">ILMerge</a>
### Description
A wrapper for the ILMerge tool.
### Example
This example merges two assemblies A.dll and B.dll into one:
            
      <PropertyGroup>
      <outputFile>$(testDir)\ilmergetest.dll</outputFile>
      <keyFile>$(testDir)\keypair.snk</keyFile>
      <excludeFile>$(testDir)\ExcludeTypes.txt</excludeFile>
      <logFile>$(testDir)\ilmergetest.log</logFile>
      </PropertyGroup>
      <ItemGroup>
      <inputAssemblies Include="$(testDir)\A.dll" />
      <inputAssemblies Include="$(testDir)\B.dll" />
      <allowDuplicates Include="ClassAB" />
      </ItemGroup>
      <Target Name="merge" >
       <ILMerge InputAssemblies="@(inputAssemblies)" 
                   AllowDuplicateTypes="@(allowDuplicates)"
                   ExcludeFile="$(excludeFile)"
                   OutputFile="$(outputFile)" LogFile="$(logFile)"
                   DebugInfo="true" XmlDocumentation="true" 
                   KeyFile="$(keyFile)" DelaySign="true" />
      </Target>
            
* * *

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
## <a id="InstallAssembly">InstallAssembly</a> (<a id="Install.InstallAssembly">Install.InstallAssembly</a>)
### Description
Installs assemblies.
### Example
Install multiple assemblies by specifying the file names:
      
        
<InstallAssembly AssemblyFiles="Engine.dll;Presenter.dll" />

      
    
* * *

        
        
        
        
        
        
        
        
        
## <a id="UninstallAssembly">UninstallAssembly</a> (<a id="Install.UninstallAssembly">Install.UninstallAssembly</a>)
### Description
Uninstalls assemblies.
### Example
Uninstall multiple assemblies by specifying the file names:
            
      <UninstallAssembly AssemblyFiles="Engine.dll;Presenter.dll" />
            
            
* * *

        
        
## <a id="Modulo">Modulo</a> (<a id="Math.Modulo">Math.Modulo</a>)
### Description
Performs the modulo operation on numbers.
### Example
Numbers evenly divide:
      
        
<Math.Modulo Numbers="12;4">
    <Output TaskParameter="Result" PropertyName="Result" />
</Math.Modulo>
<Message Text="12 modulo 4 = $(Result)"/>
Above example will display:
      12 modulo 4 = 0
    
* * *

        
## <a id="MathBase">MathBase</a> (<a id="Math.MathBase">Math.MathBase</a>)
### Description
Math task base class
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
        
## <a id="Prompt">Prompt</a>
### Description
Displays a message on the console and waits for user input.
### Example
Pause the build if the interactive property is set:
            
      <!-- Pause when invoked with the interactive property: msbuild myproject.proj /property:interactive=true -->
            
      <Prompt Text="You can now attach the debugger to the msbuild.exe process..." Condition="'$(Interactive)' == 'True'" />
            
            
* * *

        
        
        
        
## <a id="RegexBase">RegexBase</a>
### Description
Base class for Regex tasks
            Handles public properties for Input, Expression, Options and Output
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
## <a id="RegexMatch">RegexMatch</a>
### Description
Task to filter an Input list with a Regex expression.
            Output list contains items from Input list that matched given expression
### Example
Matches from TestGroup those names ending in a, b or c
            
      <ItemGroup>
       <TestGroup Include="foo.my.foo.foo.test.o" />
       <TestGroup Include="foo.my.faa.foo.test.a" />
       <TestGroup Include="foo.my.fbb.foo.test.b" />
       <TestGroup Include="foo.my.fcc.foo.test.c" />
       <TestGroup Include="foo.my.fdd.foo.test.d" />
       <TestGroup Include="foo.my.fee.foo.test.e" />
       <TestGroup Include="foo.my.fff.foo.test.f" />
      </ItemGroup>
      <Target Name="Test">
       <!-- Outputs only items that end with a, b or c -->
       <RegexMatch Input="@(TestGroup)" Expression="[a-c]$">
      <Output ItemName ="MatchReturn" TaskParameter="Output" />
       </RegexMatch>
       <Message Text="&#xA;Output Match:&#xA;@(MatchReturn, '&#xA;')" />
      </Target>
            
            
* * *

        
        
## <a id="RegexReplace">RegexReplace</a>
### Description
Task to replace portions of strings within the Input list
            Output list contains all the elements of the Input list after
            performing the Regex Replace.
### Example
1st example replaces first occurance of "foo." with empty string
            2nd example replaces occurance of "foo." after character 6 with "oop." string
            
      <ItemGroup>
       <TestGroup Include="foo.my.foo.foo.test.o" />
       <TestGroup Include="foo.my.faa.foo.test.a" />
       <TestGroup Include="foo.my.fbb.foo.test.b" />
       <TestGroup Include="foo.my.fcc.foo.test.c" />
       <TestGroup Include="foo.my.fdd.foo.test.d" />
       <TestGroup Include="foo.my.fee.foo.test.e" />
       <TestGroup Include="foo.my.fff.foo.test.f" />
      </ItemGroup>
      <Target Name="Test">
       <Message Text="Input:&#xA;@(TestGroup, '&#xA;')"/>
       <!-- Replaces first occurance of "foo." with empty string-->
       <RegexReplace Input="@(TestGroup)" Expression="foo\." Replacement="" Count="1">
      <Output ItemName ="ReplaceReturn1" TaskParameter="Output" />
       </RegexReplace>
       <Message Text="&#xA;Output Replace 1:&#xA;@(ReplaceReturn1, '&#xA;')" />
       <!-- Replaces occurance of "foo." after character 6 with "oop." string-->
       <RegexReplace Input="@(TestGroup)" Expression="foo\." Replacement="oop" Startat="6">
      <Output ItemName ="ReplaceReturn2" TaskParameter="Output" />
       </RegexReplace>
       <Message Text="&#xA;Output Replace 2:&#xA;@(ReplaceReturn2, '&#xA;')" />
      </Target>
            
            
* * *

        
        
        
        
        
## <a id="TaskListAssemblyFormatType">TaskListAssemblyFormatType</a> (<a id="Schema.TaskListAssemblyFormatType">Schema.TaskListAssemblyFormatType</a>)
### Description
Different ways to specify the assembly in a UsingTask element.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
## <a id="TaskSchema">TaskSchema</a> (<a id="Schema.TaskSchema">Schema.TaskSchema</a>)
### Description
A Task that generates a XSD schema of the tasks in an assembly.
### Example
Creates schema for MSBuild Community Task project
            
      <TaskSchema Assemblies="Build\MSBuild.Community.Tasks.dll" 
                OutputPath="Build" 
                CreateTaskList="true" 
                IgnoreMsBuildSchema="true"
                Includes="Microsoft.Build.Commontypes.xsd"/>
            
            
* * *

        
        
        
        
        
        
        
        
        
        
        
## <a id="GetSolutionProjects">GetSolutionProjects</a>
### Description
Retrieves the list of Projects contained within a Visual Studio Solution (.sln) file
### Example
Returns project name, GUID, and path information from test solution
            
      <Target Name="Test">
      <GetSolutionProjects Solution="TestSolution.sln">
      <Output ItemName="ProjectFiles" TaskParameter="Output"/>
      </GetSolutionProjects>
            
      <Message Text="Project names:" />
      <Message Text="%(ProjectFiles.ProjectName)" />
      <Message Text="Relative project paths:" />
      <Message Text="%(ProjectFiles.ProjectPath)" />
      <Message Text="Project GUIDs:" />
      <Message Text="%(ProjectFiles.ProjectGUID)" />
      <Message Text="Full paths to project files:" />
      <Message Text="%(ProjectFiles.FullPath)" />
      </Target>
            
            
* * *

        
        
        
        
## <a id="ExecuteDDL">ExecuteDDL</a> (<a id="SqlServer.ExecuteDDL">SqlServer.ExecuteDDL</a>)
### Description
MSBuild task to execute DDL and SQL statements.
### Example

             
       <PropertyGroup>
            		<ConnectionString>Server=localhost;Integrated Security=True</ConnectionString>
            	</PropertyGroup>
            
       <Target Name="ExecuteDDL">
            		<ExecuteDDL ConnectionString="$(ConnectionString)" Files="SqlBatchScript.sql" ContinueOnError="false" />
       </Target>
             
             
* * *

        
        
        
        
        
        
        
## <a id="NodeKind">NodeKind</a> (<a id="Subversion.NodeKind">Subversion.NodeKind</a>)
### Description
The kind of Subversion node. The names match the text output
            by "svn info".
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
## <a id="Schedule">Schedule</a> (<a id="Subversion.Schedule">Subversion.Schedule</a>)
### Description
The Subversion schedule type.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
## <a id="SvnInfo">SvnInfo</a> (<a id="Subversion.SvnInfo">Subversion.SvnInfo</a>)
### Description
Run the "svn info" command and parse the output
### Example
This example will determine the Subversion repository root for
            a working directory and print it out.
            
      <Target Name="printinfo">
      <SvnInfo LocalPath="c:\code\myapp">
      <Output TaskParameter="RepositoryRoot" PropertyName="root" />
      </SvnInfo>
      <Message Text="root: $(root)" />
      </Target>
            
            
* * *

        
        
        
        
        
        
        
        
        
        
        
## <a id="TemplateFile">TemplateFile</a>
### Description
MSBuild task that replaces tokens in a template file and writes out a new file.
### Example

            
      <ItemGroup>
            	<Tokens Include="Name">
            		<ReplacementValue>MSBuild Community Tasks</ReplacementValue>
            	</Tokens>
      </ItemGroup>
            
      <TemplateFile Template="ATemplateFile.template" OutputFilename="ReplacedFile.txt" Tokens="@(Tokens)" />
            
            
* * *

        
        
        
        
        
        
        
        
## <a id="Time">Time</a>
### Description
Gets the current date and time.
### Example
Using the Time task to get the Month, Day,
            Year, Hour, Minute, and Second:
            
      <Time>
      <Output TaskParameter="Month" PropertyName="Month" />
      <Output TaskParameter="Day" PropertyName="Day" />
      <Output TaskParameter="Year" PropertyName="Year" />
      <Output TaskParameter="Hour" PropertyName="Hour" />
      <Output TaskParameter="Minute" PropertyName="Minute" />
      <Output TaskParameter="Second" PropertyName="Second" />
      </Time>
      <Message Text="Current Date and Time: $(Month)/$(Day)/$(Year) $(Hour):$(Minute):$(Second)" />
Set property "BuildDate" to the current date and time:
            
      <Time Format="yyyyMMddHHmmss">
      <Output TaskParameter="FormattedTime" PropertyName="buildDate" />
      </Time>
            
* * *

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
## <a id="XmlNodeTaskItem">XmlNodeTaskItem</a> (<a id="Xml.XmlNodeTaskItem">Xml.XmlNodeTaskItem</a>)
### Description
Represents a single XmlNode selected using an XML task.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
        
        
        
        
        
        
## <a id="XmlQuery">XmlQuery</a> (<a id="Xml.XmlQuery">Xml.XmlQuery</a>)
### Description
Reads a value or values from lines of XML
### Example
Read an attribute value by selecting it with an XPath expression:
      
        
<ReadLinesFromFile File="web.config">
    <Output TaskParameter="Lines" ItemName="FileContents" />
</ReadLinesFromFile>

<XmlQuery Lines="@(FileContents)"
    XPath = "/configuration/system.web/compilation/@defaultLanguage">
  <Output TaskParameter="Values" PropertyName="CompilationLanguage" />
</XmlQuery>

<Message Text="The default language is $(CompilationLanguage)." />

      
    
* * *

        
        
        
        
        
        
        
        
        
## <a id="XmlTaskHelper">XmlTaskHelper</a> (<a id="Xml.XmlTaskHelper">Xml.XmlTaskHelper</a>)
### Description
Provides methods used by all of the XML tasks
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
## <a id="Xslt">Xslt</a>
### Description
A task to merge and transform a set of xml files.
### Example
This example for generating a report
      from a set of NUnit xml results:
      
        
<ItemGroup>
    <nunitReportXslFile Include="$(MSBuildCommunityTasksPath)\$(nunitReportXsl)">
      <project>$(project)</project>
      <configuration>$(configuration)</configuration>
      <msbuildFilename>$(MSBuildProjectFullPath)</msbuildFilename>
      <msbuildBinpath>$(MSBuildBinPath)</msbuildBinpath>
      <xslFile>$(MSBuildCommunityTasksPath)\$(nunitReportXsl)</xslFile>
    </nunitReportXslFile>
</ItemGroup>

<Target Name="test-report" >
    <Xslt Inputs="@(nunitFiles)"
        RootTag="mergedroot"
        Xsl="@(nunitReportXslFile)" 
        Output="$(testDir)\TestReport.html" />
</Target>
This examples shows all available task attributes:
      
        
<Time Format="yyyyMMddHHmmss">
    <Output TaskParameter="LocalTimestamp" PropertyName="buildDate" />
</Time>

<Xslt
     Inputs="@(xmlfiles)"
     RootTag="mergedroot"
     RootAttributes="foo=bar;date=$(buildDate)"
     Xsl="transformation.xsl"
     Output="report.html"
/>
      
    
* * *

        
        
        
        
        
        
        
        
        
        
        
## <a id="FileUpdate">FileUpdate</a>
### Description
Replace text in file(s) using a Regular Expression.
### Example
Search for a version number and update the revision.
            
      <FileUpdate Files="version.txt"
                Regex="(\d+)\.(\d+)\.(\d+)\.(\d+)"
                ReplacementText="$1.$2.$3.123" />
            
            
* * *

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
## <a id="FxCop">FxCop</a>
### Description
Uses FxCop to analyse managed code assemblies and reports on
            their design best-practice compliance.
### Example
Shows how to analyse an assembly and use an XSLT stylesheet 
            to present the report as an HTML file. If the static anlysis fails,
            the build does not stop - this is controlled with the FailOnError
            parameter.
            
      <FxCop 
              TargetAssemblies="$(MSBuildCommunityTasksPath)\MSBuild.Community.Tasks.dll"
              RuleLibraries="@(FxCopRuleAssemblies)" 
              Rules="Microsoft.Design#CA1012;-Microsoft.Performance#CA1805"
              AnalysisReportFileName="Test.html"
              DependencyDirectories="$(MSBuildCommunityTasksPath)"
              FailOnError="False"
              ApplyOutXsl="True"
              OutputXslFileName="C:\Program Files\Microsoft FxCop 1.32\Xml\FxCopReport.xsl"
            />
            
            
* * *

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
## <a id="ServiceActions">ServiceActions</a>
### Description
Defines the actions that can be performed on a service.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
## <a id="ServiceController">ServiceController</a>
### Description
Task that can control a Windows service.
### Example
Restart Web Server
            
      <ServiceController ServiceName="w3svc" Action="Restart" />
            
            
* * *

        
## <a id="ServiceQuery">ServiceQuery</a>
### Description
Task that can determine the status of a specified service
            on a target server.
### Example
Check status of SQL Server
            
      <ServiceQuery ServiceName="MSSQLServer">
      <Output TaskParameter="Status" PropertyName="ResultStatus" />
      </ServiceQuery>
      <Message Text="MSSQLServer Service Status: $(ResultStatus)"/>
            
            
* * *

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
## <a id="AppPoolCreate">AppPoolCreate</a> (<a id="IIS.AppPoolCreate">IIS.AppPoolCreate</a>)
### Description
Creates a new application pool on a local or remote machine with IIS installed.  The default is 
            to create the new application pool on the local machine.  If connecting to a remote machine, you can
            specify the  and  for the task
            to run under.
### Example
Create a new application pool on the local machine.
            
      <AppPoolCreate AppPoolName="MyAppPool" />
            
            
* * *

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
## <a id="WebDirectoryCreate">WebDirectoryCreate</a> (<a id="IIS.WebDirectoryCreate">IIS.WebDirectoryCreate</a>)
### Description
Creates a new web directory on a local or remote machine with IIS installed.  The default is 
            to create the new web directory on the local machine.  The physical path is required to already exist
            on the target machine.  If connecting to a remote machine, you can specify the 
            and  for the task to run under.
### Example
Create a new web directory on the local machine.
            
      <WebDirectoryCreate VirtualDirectoryName="MyVirDir"
                VirtualDirectoryPhysicalPath="C:\Inetpub\MyWebDir" />
            
            
* * *

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
## <a id="AppPoolDelete">AppPoolDelete</a> (<a id="IIS.AppPoolDelete">IIS.AppPoolDelete</a>)
### Description
Deletes an existing application pool on a local or remote machine with IIS installed.  The default is 
            to delete an existing application pool on the local machine.  If connecting to a remote machine, you can
            specify the  and  for the task
            to run under.
### Example
Delete an existing application pool on the local machine.
            
      <AppPoolDelete AppPoolName="MyAppPool" />
            
            
* * *

        
        
        
## <a id="WebDirectoryDelete">WebDirectoryDelete</a> (<a id="IIS.WebDirectoryDelete">IIS.WebDirectoryDelete</a>)
### Description
Deletes a web directory on a local or remote machine with IIS installed.  The default is 
            to delete the web directory on the local machine.  If connecting to a remote machine, you
            can specify the  and  for the
            task to run under.
### Example
Deletes a web directory on the local machine.
            
      <WebDirectoryDelete VirtualDirectoryName="MyVirDir" />
            
            
* * *

        
        
        
## <a id="AppPoolControllerActions">AppPoolControllerActions</a> (<a id="IIS.AppPoolControllerActions">IIS.AppPoolControllerActions</a>)
### Description
Actions the  can do.
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
## <a id="AppPoolController">AppPoolController</a> (<a id="IIS.AppPoolController">IIS.AppPoolController</a>)
### Description
Allows control for an application pool on a local or remote machine with IIS installed.  The default is 
            to control the application pool on the local machine.  If connecting to a remote machine, you can
            specify the  and  for the task
            to run under.
### Example
Restart an application pool on the local machine.
            
      <AppPoolController AppPoolName="MyAppPool" Action="Restart" />
            
            
* * *

        
        
        
        
## <a id="Mail">Mail</a>
### Description
Sends an email message
### Example
Example of sending an email.
            
      <Target Name="Mail">
      <Mail SmtpServer="localhost"
                    To="user@email.com"
                    From="from@email.com"
                    Subject="Test Mail Task"
                    Body="This is a test of the mail task." />
      </Target>
            
            
* * *

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
## <a id="Add">Add</a> (<a id="Math.Add">Math.Add</a>)
### Description
Add numbers
### Example
Adding numbers:
            
      <Math.Add Numbers="4;3">
      <Output TaskParameter="Result" PropertyName="Result" />
      </Math.Add>
      <Message Text="Add 4+3= $(Result)"/>
            
            
* * *

        
        
## <a id="Divide">Divide</a> (<a id="Math.Divide">Math.Divide</a>)
### Description
Divide numbers
### Example

      
        
<Math.Divide Numbers="1;2">
    <Output TaskParameter="Result" PropertyName="Result" />
</Math.Divide>
<Message Text="Divide 1/2= $(Result)"/>
Above example will display:
      Divide 1/2= 0.5
    
* * *

        
        
        
## <a id="Multiple">Multiple</a> (<a id="Math.Multiple">Math.Multiple</a>)
### Description
Multiple numbers
### Example

            
      <Math.Multiple Numbers="10;3">
      <Output TaskParameter="Result" PropertyName="Result" />
      </Math.Multiple>
      <Message Text="Multiple 10*3= $(Result)"/>
            
            
* * *

        
        
## <a id="Subtract">Subtract</a> (<a id="Math.Subtract">Math.Subtract</a>)
### Description
Subtract numbers
### Example

            
      <Math.Subtract Numbers="10;3">
      <Output TaskParameter="Result" PropertyName="Result" />
      </Math.Subtract>
      <Message Text="Subtract 10-3= $(Result)"/>
            
            
* * *

        
        
## <a id="MV">MV</a>
### Description
Moves files on the filesystem to a new location.
### Example
Move a file to another folder
            
      <MV SourceFiles="Test\MoveMe.txt"
                DestinationFolder="Test\Move" />
Rename a file
            
      <MV SourceFiles="Test\Move\MoveMe.txt"
                DestinationFiles="Test\Move\Renamed.txt" />
            
            
* * *

        
        
        
        
        
        
## <a id="NDoc">NDoc</a>
### Description
Runs the NDoc application.
### Example
Generated html help file.
            
      <NDoc Documenter="MSDN" 
                ProjectFilePath="MSBuild.Community.Tasks.ndoc" />
            
            
* * *

        
        
        
        
        
        
        
        
        
        
        
        
## <a id="NUnit">NUnit</a>
### Description
Run NUnit 2.4 on a group of assemblies.
### Example
Run NUnit tests.
            
      <ItemGroup>
      <TestAssembly Include="C:\Program Files\NUnit 2.4\bin\*.tests.dll" />
      </ItemGroup>
      <Target Name="NUnit">
      <NUnit Assemblies="@(TestAssembly)" />
      </Target>
            
            
* * *

        
        
        
        
        
        
        
        
        
        
        
        
## <a id="NUnit3">NUnit3</a>
### Description
Run NUnit 3.x on a group of assemblies.
### Example
Run NUnit3 tests.
            
      <CreateItem Include="*\bin\Debug\*.*.UnitTests.dll">
         <Output TaskParameter="Include" ItemName="TestAssemblies"/>
      </CreateItem>
      <Target Name="NUnit3">
      <!-- Run NUnit passing in the list of assemblies built above -->		
      <NUnit3 Assemblies="@(TestAssemblies)" 
	          Process="Multiple" 
			  TestTimeout="2000" 
	          Framework="v4.0" 
			  Force32Bit="true" 
			  Workers="10" 
			  EnableShadowCopy="true" 
			  OutputXmlFile="myTestOutput.xml"
			  WorkingDirectory="./"
			  ShowLabels="All"
			  NoHeader="true"
			  NoColor="true"
			  Verbose="true"/>
      </Target>
            
            
* * *

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
## <a id="Resources">Resources</a> (<a id="Properties.Resources">Properties.Resources</a>)
### Description
一个强类型的资源类，用于查找本地化的字符串等。
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
## <a id="RegistryRead">RegistryRead</a>
### Description
Reads a value from the Registry
### Example
Read .NET Framework install root from Registry.
            
      <RegistryRead 
                KeyName="HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\.NETFramework" 
                ValueName="InstallRoot">
      <Output TaskParameter="Value" PropertyName="InstallRoot" />
      </RegistryRead>
      <Message Text="InstallRoot: $(InstallRoot)"/>
            
            
* * *

        
        
        
        
        
        
        
## <a id="RegistryWrite">RegistryWrite</a>
### Description
Writes a value to the Registry
### Example
Write a value to Registry
            
      <RegistryWrite 
                KeyName="HKEY_CURRENT_USER\SOFTWARE\MSBuildTasks"
                ValueName="RegistryWrite"
                Value="Test Write" />
            
            
* * *

        
        
        
        
        
        
## <a id="Script">Script</a>
### Description
Executes code contained within the task.
### Example
Simple script that writes to the console
      
        
<PropertyGroup>
    <HelloCode>
      <![CDATA[
        public static void ScriptMain() {
            Console.WriteLine("Hello MSBuild Community Scripting World.");
        }
        ]] >
    </HelloCode>
</PropertyGroup>
<Target Name="Hello">
    <Script Language="C#" Code="$(HelloCode)" Imports="System" />
</Target>

      
    
* * *

        
        
        
        
        
        
        
        
## <a id="Sleep">Sleep</a>
### Description
A task for sleeping for a specified period of time.
### Example
Causes the build to sleep for 300 milliseconds.
            
      <Sleep Milliseconds="300" />
            
            
* * *

        
        
        
        
        
        
## <a id="SqlExecute">SqlExecute</a>
### Description
Executes a SQL command.
### Example
Example of returning a count of items in a table.  Uses the default SelectMode of NonQuery.
            
      <SqlExecute ConnectionString="server=MyServer;Database=MyDatabase;Trusted_Connection=yes;"
                    Command="create database MyDatabase" />
Example of returning the items of a table in an xml format.
            
      <SqlExecute ConnectionString="server=MyServer;Database=MyDatabase;Trusted_Connection=yes;"
            		Command="select * from SomeTable for xml auto"
            		SelectMode="ScalarXml"
            		OutputFile="SomeTable.xml" />
            
            
* * *

        
        
        
        
        
        
        
        
        
## <a id="SvnCheckout">SvnCheckout</a> (<a id="Subversion.SvnCheckout">Subversion.SvnCheckout</a>)
### Description
Checkout a local working copy of a Subversion repository.
### Example
Checkout a working copy
            
      <Target Name="Checkout">
      <RemoveDir Directories="$(MSBuildProjectDirectory)\Test\Checkout" />
      <SvnCheckout RepositoryPath="file:///d:/svn/repo/Test/trunk" 
                           LocalPath="$(MSBuildProjectDirectory)\Test\Checkout">      
      <Output TaskParameter="Revision" PropertyName="Revision" />
      </SvnCheckout>
      <Message Text="Revision: $(Revision)"/>
      </Target>
            
            
* * *

        
        
        
## <a id="SvnCommit">SvnCommit</a> (<a id="Subversion.SvnCommit">Subversion.SvnCommit</a>)
### Description
Subversion Commit command
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
## <a id="SvnExport">SvnExport</a> (<a id="Subversion.SvnExport">Subversion.SvnExport</a>)
### Description
Export a folder from a Subversion repository
### Example
Export from repository
            
      <Target Name="Export">
      <MakeDir Directories="$(MSBuildProjectDirectory)\Test" />
      <RemoveDir Directories="$(MSBuildProjectDirectory)\Test\Export" />
      <SvnExport RepositoryPath="file:///d:/svn/repo/Test/trunk" 
                LocalPath="$(MSBuildProjectDirectory)\Test\Export">
      <Output TaskParameter="Revision" PropertyName="Revision" />
      </SvnExport>
      <Message Text="Revision: $(Revision)"/>
      </Target>
            
            
* * *

        
        
        
## <a id="SvnVersion">SvnVersion</a> (<a id="Subversion.SvnVersion">Subversion.SvnVersion</a>)
### Description
Summarize the local revision(s) of a working copy.
### Example
The following example gets the revision of the current folder.
            
      <Target Name="Version">
      <SvnVersion LocalPath=".">
      <Output TaskParameter="Revision" PropertyName="Revision" />
      </SvnVersion>
      <Message Text="Revision: $(Revision)"/>
      </Target>
            
            
* * *

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
## <a id="SvnUpdate">SvnUpdate</a> (<a id="Subversion.SvnUpdate">Subversion.SvnUpdate</a>)
### Description
Subversion Update command
### No example given
The developer of this task did not add an example in the summary documentation.

* * *

        
        
        
## <a id="Unzip">Unzip</a>
### Description
Unzip a file to a target directory.
### Example
Unzip file tasks
            
      <Unzip ZipFileName="MSBuild.Community.Tasks.zip" 
                TargetDirectory="Backup"/>
            
            
* * *

        
        
        
        
        
        
        
        
## <a id="Version">Version</a>
### Description
Generates version information based on various algorithms
### Example
Get version information from file and increment revision.
            
      <Version VersionFile="number.txt" BuildType="Automatic" RevisionType="Increment">
      <Output TaskParameter="Major" PropertyName="Major" />
      <Output TaskParameter="Minor" PropertyName="Minor" />
      <Output TaskParameter="Build" PropertyName="Build" />
      <Output TaskParameter="Revision" PropertyName="Revision" />
      </Version>
      <Message Text="Version: $(Major).$(Minor).$(Build).$(Revision)"/>
            
            
* * *

        
        
        
        
        
        
        
        
        
        
        
        
        
## <a id="WebDownload">WebDownload</a>
### Description
Downloads a resource with the specified URI to a local file.
### Example
Download the Microsoft.com home page.
            
      <WebDownload FileUri="http://www.microsoft.com/default.aspx" 
                FileName="microsoft.html" />
            
            
* * *

        
        
        
        
        
        
        
        
        
## <a id="XmlRead">XmlRead</a>
### Description
Reads a value from a XML document using a XPath.
### Example
Read all targest from a build project.
            
      <XmlRead Prefix="n"
                Namespace="http://schemas.microsoft.com/developer/msbuild/2003" 
                XPath="/n:Project/n:Target/@Name"
                XmlFileName="Subversion.proj">
      <Output TaskParameter="Value" PropertyName="BuildTargets" />
      </XmlRead>
      <Message Text="Build Targets: $(BuildTargets)"/>
            
            
* * *

        
        
        
        
        
        
        
        
## <a id="XmlUpdate">XmlUpdate</a>
### Description
Updates a XML document using a XPath.
### Example
Update a XML element.
            
      <XmlUpdate Prefix="n"
                Namespace="http://schemas.microsoft.com/developer/msbuild/2003" 
                XPath="/n:Project/n:PropertyGroup/n:TestUpdate"
                XmlFileName="Subversion.proj"
                Value="Test from $(MSBuildProjectFile)"/>
            
            
* * *

        
        
        
        
        
        
        
        
        
## <a id="Zip">Zip</a>
### Description
Create a zip file with the files specified.
### Example
Create a zip file
            
      <ItemGroup>
      <ZipFiles Include="**\*.*" Exclude="*.zip" />
      </ItemGroup>
      <Target Name="Zip">
      <Zip Files="@(ZipFiles)" 
                    ZipFileName="MSBuild.Community.Tasks.zip" />
      </Target>
Create a zip file using a working directory.
            
      <ItemGroup>
      <RepoFiles Include="D:\svn\repo\**\*.*" />
      </ItemGroup>
      <Target Name="Zip">
      <Zip Files="@(RepoFiles)" 
                    WorkingDirectory="D:\svn\repo" 
                    ZipFileName="D:\svn\repo.zip" />
      </Target>
            
            
* * *

        
        
        
        
        
        
        
        
        
        
        
        
        
        
    
