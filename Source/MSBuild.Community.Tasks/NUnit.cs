#region Copyright © 2005 Paul Welter. All rights reserved.
/*
Copyright © 2005 Paul Welter. All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions
are met:

1. Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright
   notice, this list of conditions and the following disclaimer in the
   documentation and/or other materials provided with the distribution.
3. The name of the author may not be used to endorse or promote products
   derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE AUTHOR "AS IS" AND ANY EXPRESS OR
IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 
*/
#endregion

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.Win32;

// $Id$

namespace MSBuild.Community.Tasks
{
	/// <summary>
	/// Run NUnit on a group of assemblies.
	/// </summary>
	/// <example>Run NUnit tests.
	/// <code><![CDATA[
	/// <ItemGroup>
	///     <TestAssembly Include="C:\Program Files\NUnit 2.2.3\bin\*.tests.dll" />
	/// </ItemGroup>
	/// <Target Name="NUnit">
	///     <NUnit Assemblies="@(TestAssembly)" />
	/// </Target>
	/// ]]></code>
	/// </example>
	public class NUnit : ToolTask
	{
		#region Constants

		/// <summary>
		/// The default relative path of the NUnit installation.
		/// The value is <c>@"NUnit-Net-2.0 2.2.5\bin"</c>.
		/// </summary>
		public const string DEFAULT_NUNIT_DIRECTORY = @"NUnit-Net-2.0 2.2.5\bin";

		#endregion Constants

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="T:NUnit"/> class.
		/// </summary>
		public NUnit()
		{

		}

		#endregion Constructor

		#region Properties
		private ITaskItem[] _assemblies;

		/// <summary>
		/// Gets or sets the assemblies.
		/// </summary>
		/// <value>The assemblies.</value>
		[Required]
		public ITaskItem[] Assemblies
		{
			get { return _assemblies; }
			set { _assemblies = value; }
		}

		private string _includeCategory;

		/// <summary>
		/// Gets or sets the include category.
		/// </summary>
		/// <value>The include category.</value>
		public string IncludeCategory
		{
			get { return _includeCategory; }
			set { _includeCategory = value; }
		}

		private string _excludeCategory;

		/// <summary>
		/// Gets or sets the exclude category.
		/// </summary>
		/// <value>The exclude category.</value>
		public string ExcludeCategory
		{
			get { return _excludeCategory; }
			set { _excludeCategory = value; }
		}

		private string _fixture;

		/// <summary>
		/// Gets or sets the fixture.
		/// </summary>
		/// <value>The fixture.</value>
		public string Fixture
		{
			get { return _fixture; }
			set { _fixture = value; }
		}

		private string _xsltTransformFile;

		/// <summary>
		/// Gets or sets the XSLT transform file.
		/// </summary>
		/// <value>The XSLT transform file.</value>
		public string XsltTransformFile
		{
			get { return _xsltTransformFile; }
			set { _xsltTransformFile = value; }
		}

		private string _outputXmlFile;

		/// <summary>
		/// Gets or sets the output XML file.
		/// </summary>
		/// <value>The output XML file.</value>
		public string OutputXmlFile
		{
			get { return _outputXmlFile; }
			set { _outputXmlFile = value; }
		}

		private string _workingDirectory;

		/// <summary>
		/// Gets or sets the working directory.
		/// </summary>
		/// <value>The working directory.</value>
		/// <returns>
		/// The directory in which to run the executable file, or a null reference (Nothing in Visual Basic) if the executable file should be run in the current directory.
		/// </returns>
		public string WorkingDirectory
		{
			get { return _workingDirectory; }
			set { _workingDirectory = value; }
		}

		#endregion

		#region Task Overrides
		/// <summary>
		/// Returns a string value containing the command line arguments to pass directly to the executable file.
		/// </summary>
		/// <returns>
		/// A string value containing the command line arguments to pass directly to the executable file.
		/// </returns>
		protected override string GenerateCommandLineCommands()
		{
			StringBuilder builder = new StringBuilder();
			foreach (ITaskItem item in _assemblies)
			{
				builder.AppendFormat(" \"{0}\"", item.ItemSpec);
			}

			if (!string.IsNullOrEmpty(_fixture))
				builder.AppendFormat(" /fixture={0}", _fixture);

			if (!string.IsNullOrEmpty(_includeCategory))
				builder.AppendFormat(" /include={0}", _includeCategory);

			if (!string.IsNullOrEmpty(_excludeCategory))
				builder.AppendFormat(" /exclude={0}", _excludeCategory);

			if (!string.IsNullOrEmpty(_xsltTransformFile))
				builder.AppendFormat(" /transform={0}", _xsltTransformFile);

			if (!string.IsNullOrEmpty(_outputXmlFile))
				builder.AppendFormat(" /xml={0}", _outputXmlFile);

			return builder.ToString();
		}

		private void CheckToolPath()
		{
			string nunitPath = ToolPath == null ? String.Empty : ToolPath.Trim();
			if (String.IsNullOrEmpty(nunitPath))
			{
				nunitPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
				nunitPath = Path.Combine(nunitPath, DEFAULT_NUNIT_DIRECTORY);

				try
				{
					using (RegistryKey buildKey = Registry.ClassesRoot.OpenSubKey(@"NUnitTestProject\shell\open\command"))
					{
						if (buildKey == null)
						{
							Log.LogError(Properties.Resources.NUnitNotFound);
						}
						else
						{
							nunitPath = buildKey.GetValue(null, nunitPath).ToString();
							Regex nunitRegex = new Regex("(.+)nunit-gui\\.exe", RegexOptions.IgnoreCase);
							Match pathMatch = nunitRegex.Match(nunitPath);
							nunitPath = pathMatch.Groups[1].Value.Replace("\"", "");
						}
					}
				}
				catch (Exception ex)
				{
					Log.LogErrorFromException(ex);
				}
				ToolPath = nunitPath;
			}
		}
		
		/// <summary>
		/// Returns the fully qualified path to the executable file.
		/// </summary>
		/// <returns>
		/// The fully qualified path to the executable file.
		/// </returns>
		protected override string GenerateFullPathToTool()
		{
			CheckToolPath();
			return Path.Combine(ToolPath, ToolName);
		}

        /// <summary>
        /// Logs the starting point of the run to all registered loggers.
        /// </summary>
        /// <param name="message">A descriptive message to provide loggers, usually the command line and switches.</param>
        protected override void LogToolCommand(string message)
        {
            Log.LogCommandLine(MessageImportance.Low, message);
        }

		/// <summary>
		/// Gets the name of the executable file to run.
		/// </summary>
		/// <value></value>
		/// <returns>The name of the executable file to run.</returns>
		protected override string ToolName
		{
			get { return "nunit-console.exe"; }
		}

		/// <summary>
		/// Gets the <see cref="T:Microsoft.Build.Framework.MessageImportance"></see> with which to log errors.
		/// </summary>
		/// <value></value>
		/// <returns>The <see cref="T:Microsoft.Build.Framework.MessageImportance"></see> with which to log errors.</returns>
		protected override MessageImportance StandardOutputLoggingImportance
		{
			get
			{
				return MessageImportance.Normal;
			}
		}

		/// <summary>
		/// Returns the directory in which to run the executable file.
		/// </summary>
		/// <returns>
		/// The directory in which to run the executable file, or a null reference (Nothing in Visual Basic) if the executable file should be run in the current directory.
		/// </returns>
		protected override string GetWorkingDirectory()
		{
			return string.IsNullOrEmpty(_workingDirectory) ? base.GetWorkingDirectory() : _workingDirectory;
		}

		#endregion Task Overrides

	}
}
