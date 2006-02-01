// $Id$
// Copyright © 2006 Ignaz Kohlbecker

using System;
using System.Collections;
using System.IO;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace MSBuild.Community.Tasks
{
	/// <summary>
	/// A wrapper for the ILMerge tool.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The ILMerge tool itself must be installed separately.
	/// It is available <a href="http://research.microsoft.com/~mbarnett/ILMerge.aspx">here</a>.
	/// </para>
	/// <para>
	/// The command line options "/wildcards" and "/lib" of ILMerge is not supported,
	/// because MSBuild is in charge of expanding wildcards for item groups.
	/// </para>
	/// </remarks>
	/// <example>
	/// This example merges two assemblies A.dll and B.dll into one:
	/// <code><![CDATA[
	/// <PropertyGroup>
	///     <outputFile>$(testDir)\ilmergetest.dll</outputFile>
	///     <keyFile>$(testDir)\keypair.snk</keyFile>
	///     <excludeFile>$(testDir)\ExcludeTypes.txt</excludeFile>
	///     <logFile>$(testDir)\ilmergetest.log</logFile>
	/// </PropertyGroup>
	///
	/// <ItemGroup>
	///     <inputAssemblies Include="$(testDir)\A.dll" />
	///     <inputAssemblies Include="$(testDir)\B.dll" />
	///
	///     <allowDuplicates Include="ClassAB" />
	/// </ItemGroup>
	///
	/// <Target Name="merge" >
	///    <ILMerge InputAssemblies="@(inputAssemblies)" 
	///        AllowDuplicateTypes="@(allowDuplicates)"
	///        ExcludeFile="$(excludeFile)"
	///        OutputFile="$(outputFile)" LogFile="$(logFile)"
	///        DebugInfo="true" XmlDocumentation="true" 
	///        KeyFile="$(keyFile)" DelaySign="true" />
	/// </Target>]]></code>
	/// </example>
	public class ILMerge : ToolTask
	{
		#region Fields

		private ITaskItem[] allowDuplicateTypes;
		private bool allowZeroPeKind;
		private ITaskItem attributeFile;
		private bool closed;
		private bool copyAttributes;
		private bool debugInfo = true;
		private bool delaySign;
		private ITaskItem excludeFile;
		private ITaskItem keyFile;
		private ITaskItem logFile;
		private ITaskItem outputFile;
		private bool publicKeyTokens;
		private ITaskItem[] inputAssemblies;
		private string targetPlatformVersion;
		private ITaskItem targetPlatformDirectory;
		private string targetKind;
		private string version;
		private bool xmlDocumentation;
		
		#endregion Fields
		
		#region Input Parameters
		/// <summary>
		/// Gets or sets the names of public types
		/// to be renamed when they are duplicates.
		/// </summary>
		/// <remarks>
		/// <para>Set to an empty item group to allow all public types to be renamed.</para>
		/// <para>Don't provide this parameter if no duplicates of public types are allowed.</para>
		/// <para>Corresponds to command line option "/allowDup".</para>
		/// <para>The default value is <c>null</c>.</para>
		/// </remarks>
		public ITaskItem[] AllowDuplicateTypes
		{
			get { return allowDuplicateTypes; }
			set { allowDuplicateTypes = value; }
		}

		/// <summary>
		/// Gets or sets the flag to treat an assembly 
		/// with a zero PeKind flag 
		/// (this is the value of the field listed as .corflags in the Manifest)
		/// as if it was ILonly.
		/// </summary>
		/// <remarks>
		/// <para>Corresponds to command line option "/zeroPeKind".</para>
		/// <para>The default value is <c>false</c>.</para>
		/// </remarks>
		public bool AllowZeroPeKind
		{
			get { return allowZeroPeKind; }
			set { allowZeroPeKind = value; }
		}

		/// <summary>
		/// Gets or sets the attribute assembly
		/// from whre to get all of the assembly-level attributes
		/// such as Culture, Version, etc.
		/// It will also be used to get the Win32 Resources from.
		/// </summary>
		/// <remarks>
		/// <para>This property is mutually exclusive with <see cref="CopyAttributes"/>.</para>
		/// <para>
		/// When not specified, then the Win32 Resources from the primary assembly 
		/// are copied over into the target assembly.
		/// </para>
		/// <para>Corresponds to command line option "/attr".</para>
		/// <para>The default value is <c>null</c>.</para>
		/// </remarks>
		public ITaskItem AttributeFile
		{
			get { return attributeFile; }
			set { attributeFile = value; }
		}

		/// <summary>
		/// Gets or sets the flag to indicate
		/// whether to augment the list of input assemblies
		/// to its "transitive closure".
		/// </summary>
		/// <remarks>
		/// <para>
		/// An assembly is considered part of the transitive closure if it is referenced,
		/// either directly or indirectly, 
		/// from one of the originally specified input assemblies 
		/// and it has an external reference to one of the input assemblies, 
		/// or one of the assemblies that has such a reference.
		/// </para>
		/// <para>Corresponds to command line option "/closed".</para>
		/// <para>The default value is <c>false</c>.</para>
		/// </remarks>
		public bool Closed
		{
			get { return closed; }
			set { closed = value; }
		}

		/// <summary>
		/// Gets or sets the flag to indicate
		/// whether to copy the assembly level attributes
		/// of each input assembly over into the target assembly.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Any duplicate attribute overwrites a previously copied attribute.
		/// The input assemblies are processed in the order they are specified.
		/// </para>
		/// <para>This parameter is mutually exclusive with <see cref="AttributeFile"/>.</para>
		/// <para>Corresponds to command line option "/copyattrs".</para>
		/// <para>The default value is <c>false</c>.</para>
		/// </remarks>
		public bool CopyAttributes
		{
			get { return copyAttributes; }
			set { copyAttributes = value; }
		}

		/// <summary>
		/// Gets or sets the flag to indicate
		/// whether to preserve any .pdb files
		/// that are found for the input assemblies
		/// into a .pdb file for the target assembly.
		/// </summary>
		/// <remarks>
		/// <para>Corresponds to command line option "/ndebug".</para>
		/// <para>The default value is <c>true</c>.</para>
		/// </remarks>
		public bool DebugInfo
		{
			get { return debugInfo; }
			set { debugInfo = value; }
		}

		/// <summary>
		/// Gets or sets the flag to indicate
		/// whether the target assembly will be delay signed.
		/// </summary>
		/// <remarks>
		/// <para>This property can be set only in conjunction with <see cref="KeyFile"/>.</para>
		/// <para>Corresponds to command line option "/delaysign".</para>
		/// <para>The default value is <c>false</c>.</para>
		/// </remarks>
		public bool DelaySign
		{
			get { return delaySign; }
			set { delaySign = value; }
		}

		/// <summary>
		/// Gets or sets the file
		/// that will be used to identify types
		/// that are not to have their visibility modified.
		/// </summary>
		/// <remarks>
		/// <para>
		/// If an empty item group is provided, 
		/// then all types in any assembly other than the primary assembly are made non-public.
		/// </para>
		/// <para>Omit this parameter to prevent ILMerge from modifying the visibility of any types.</para>
		/// <para>
		/// The contents of the file should be one <see cref="System.Text.RegularExpressions.Regex"/> per line. 
		/// The regular expressions are matched against each type's full name, 
		/// e.g., <c>System.Collections.IList</c>. 
		/// If the match fails, it is tried again with the assembly name (surrounded by square brackets) 
		/// prepended to the type name. 
		/// Thus, the pattern <c>\[A\].*</c> excludes all types in assembly <c>A</c> from being made non-public. 
		/// The pattern <c>N.T</c> will match all types named <c>T</c> in the namespace named <c>N</c>
		/// no matter what assembly they are defined in.
		/// </para>
		/// <para>Corresponds to command line option "/internalize".</para>
		/// <para>The default value is <c>null</c>.</para>
		/// </remarks>
		public ITaskItem ExcludeFile
		{
			get { return excludeFile; }
			set { excludeFile = value; }
		}

		/// <summary>
		/// Gets or sets the input assemblies to merge.
		/// </summary>
		[Required]
		public ITaskItem[] InputAssemblies
		{
			get { return inputAssemblies; }
			set { inputAssemblies = value; }
		}

		/// <summary>
		/// Gets or sets the .snk file
		/// to sign the target assembly.
		/// </summary>
		/// <remarks>
		/// <para>Can be used with <see cref="DelaySign"/>.</para>
		/// <para>Corresponds to command line option "/keyfile".</para>
		/// <para>The default value is <c>null</c>.</para>
		/// </remarks>
		public ITaskItem KeyFile
		{
			get { return keyFile; }
			set { keyFile = value; }
		}

		/// <summary>
		/// Gets or sets a log file
		/// to write log messages to.
		/// </summary>
		/// <remarks>
		/// <para>
		/// If an empty item group is provided,
		/// then log messages are writte to <see cref="Console.Out"/>.
		/// </para>
		/// <para>Corresponds to command line option "/log".</para>
		/// <para>The default value is <c>null</c>.</para>
		/// </remarks>
		public ITaskItem LogFile
		{
			get { return logFile; }
			set { logFile = value; }
		}

		/// <summary>
		/// Gets or sets the target assembly.
		/// </summary>
		/// <remarks>
		/// <para>Corresponds to command line option "/out".</para>
		/// </remarks>
		[Required]
		public ITaskItem OutputFile
		{
			get { return outputFile; }
			set { outputFile = value; }
		}

		/// <summary>
		/// Gets or sets the flag to indicate
		/// whether external assembly references in the manifest
		/// of the target assembly will use public keys (<c>false</c>)
		/// or public key tokens (<c>true</c>).
		/// </summary>
		/// <remarks>
		/// <para>Corresponds to command line option "/publickeytokens".</para>
		/// <para>The default value is <c>false</c>.</para>
		/// </remarks>
		public bool PublicKeyTokens
		{
			get { return publicKeyTokens; }
			set { publicKeyTokens = value; }
		}

		/// <summary>
		/// Gets or sets the .NET framework version for the target assembly.
		/// </summary>
		/// <remarks>
		/// <para>Valid values are "v1", "v1.1", "v2".</para>
		/// <para>Corresponds to the first part of command line option "/targetplatform".</para>
		/// <para>The default value is <c>null</c>.</para>
		/// </remarks>
		public string TargetPlatformVersion
		{
			get { return targetPlatformVersion; }
			set { targetPlatformVersion = value; }
		}

		/// <summary>
		/// Gets or sets the directory in which <c>mscorlib.dll</c> is to be found.
		/// </summary>
		/// <remarks>
		/// <para>Can only be used in conjunction with <see cref="TargetPlatformVersion"/>.</para>
		/// <para>Corresponds to the second part of command line option "/targetplatform".</para>
		/// <para>The default value is <c>null</c>.</para>
		/// </remarks>
		public ITaskItem TargetPlatformDirectory
		{
			get { return targetPlatformDirectory; }
			set { targetPlatformDirectory = value; }
		}

		/// <summary>
		/// Gets or sets the indicator
		/// whether the target assembly is created as a library (<c>Dll</c>),
		/// a console application (<c>Exe</c>) or as a Windows application (<c>WinExe</c>).
		/// </summary>
		/// <remarks>
		/// <para>Corresponds to command line option "/target".</para>
		/// <para>The default value is the same kind as that of the primary assembly.</para>
		/// </remarks>
		public string TargetKind
		{
			get { return targetKind; }
			set { targetKind = value; }
		}

		/// <summary>
		/// Gets or sets the version number of the target assembly.
		/// </summary>
		/// <remarks>
		/// <para>The parameter should look like <c>6.2.1.3</c>.</para>
		/// <para>Corresponds to command line option "/ver".</para>
		/// <para>The default value is null.</para>
		/// </remarks>
		public string Version
		{
			get { return version; }
			set { version = value; }
		}

		/// <summary>
		/// Gets or sets the flag to indicate
		/// whether to merge XML documentation files
		/// into one for the target assembly.
		/// </summary>
		/// <remarks>
		/// <para>Corresponds to command line option "/xmldocs".</para>
		/// <para>The default value is <c>false</c>.</para>
		/// </remarks>
		public bool XmlDocumentation
		{
			get { return xmlDocumentation; }
			set { xmlDocumentation = value; }
		}

		#endregion Input Parameters

		#region TookTask Overrides

		/// <summary>
		/// Gets the name of the executable file to run.
		/// </summary>
		protected override string ToolName
		{
			get { return "ILMerge.exe"; }
		}

		/// <summary>
		/// Gets the standard installation path of ILMerge.exe.
		/// </summary>
		/// <remarks>
		/// If ILMerge is not installed at its standard installation path,
		/// provide its location to <see cref="ToolTask.ToolPath"/>.
		/// </remarks>
		/// <returns>Returns [ProgramFiles]\Microsoft\ILMerge.exe.</returns>
		protected override string GenerateFullPathToTool()
		{
			return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), 
				@"Microsoft\ILMerge\" + ToolName);
		}

		/// <summary>
		/// Returns a string value containing the command line arguments
		/// to pass directly to the executable file.
		/// </summary>
		/// <returns>
		/// Returns a string value containing the command line arguments 
		/// to pass directly to the executable file.
		/// </returns>
		protected override string GenerateCommandLineCommands()
		{
			CommandLineBuilder builder = new CommandLineBuilder();

			if (allowDuplicateTypes != null)
			{
				foreach (ITaskItem allowDuplicateType in allowDuplicateTypes)
				{
					builder.AppendSwitch(@"/allowDup:" + allowDuplicateType.ItemSpec);
				}
			}

			if (allowZeroPeKind) builder.AppendSwitch(@"/zeroPeKind");

			if (attributeFile != null) builder.AppendSwitch(@"/attr:" + attributeFile.ItemSpec);

			if (closed) builder.AppendSwitch(@"/closed");

			if (copyAttributes) builder.AppendSwitch(@"/copyattrs");

			if (!debugInfo) builder.AppendSwitch(@"/ndebug");

			if (delaySign) builder.AppendSwitch(@"/delaysign");

			if (excludeFile != null)
			{
				builder.AppendSwitch(@"/internalize" +
					((excludeFile.ItemSpec.Length == 0) ? string.Empty : @":" + excludeFile.ItemSpec));
			}

			if (keyFile != null) builder.AppendSwitch(@"/keyfile:" + keyFile.ItemSpec);

			if (logFile != null)
			{
				builder.AppendSwitch(@"/log" +
					((logFile.ItemSpec.Length == 0) ? string.Empty : @":" + logFile.ItemSpec));
			}

			if (outputFile != null) builder.AppendSwitch(@"/out:" + outputFile.ItemSpec);

			if (publicKeyTokens) builder.AppendSwitch(@"/publickeytokens");

			if (targetPlatformVersion != null)
			{
				builder.AppendSwitch(@"/targetplatform:" +
					targetPlatformVersion +
					((targetPlatformDirectory == null) ? string.Empty : @"," + targetPlatformDirectory.ItemSpec));
			}

			if (targetKind != null) builder.AppendSwitch(@"/target:" + targetKind);

			if (version != null) builder.AppendSwitch(@"/ver:" + version);

			if (xmlDocumentation) builder.AppendSwitch(@"/xmldocs");

			builder.AppendFileNamesIfNotNull(inputAssemblies, @" ");
			
			return builder.ToString();
		}

		#endregion TookTask Overrides
	}
}