// $Id$

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using System.IO;
using Microsoft.Win32;
using System.Text.RegularExpressions;

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
        /// <summary>
        /// Initializes a new instance of the <see cref="T:NUnit"/> class.
        /// </summary>
        public NUnit()
        {

        }

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

        #endregion

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

            if(!string.IsNullOrEmpty(_fixture))
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

        /// <summary>
        /// Returns the fully qualified path to the executable file.
        /// </summary>
        /// <returns>
        /// The fully qualified path to the executable file.
        /// </returns>
        protected override string GenerateFullPathToTool()
        {
            string nunitPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            nunitPath = Path.Combine(nunitPath, @"NUnit 2.2.3\bin");

            try
            {
                using (RegistryKey buildKey = Registry.ClassesRoot.OpenSubKey(@"NUnitTestProject\shell\open\command"))
                {
                    if (buildKey == null)
                    {
                        Log.LogError("Could not find the NUnit Project File open command. Please make sure NUnit is installed.");
                    }
                    else
                    {
                        nunitPath = buildKey.GetValue(null, nunitPath).ToString();
                        Regex ndocRegex = new Regex("(.+)nunit-gui\\.exe", RegexOptions.IgnoreCase);
                        Match pathMatch = ndocRegex.Match(nunitPath);
                        nunitPath = pathMatch.Groups[1].Value.Replace("\"", "");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
            }

            base.ToolPath = nunitPath;
            return Path.Combine(ToolPath, ToolName);
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

    }
}
