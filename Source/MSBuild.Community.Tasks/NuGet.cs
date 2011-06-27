using System;
using System.IO;
using System.Resources;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace MSBuild.Community.Tasks
{
    /// <summary>
    /// Creates a NuGet package based on the specified manifest (Nuspec) file.
    /// Can set the version number in the manifest prior to building the package.
    /// </summary>
    public class NuGet : ToolTask
    {
        /// <summary>
        /// The location of the manifest (Nuspec) file to create a package.
        /// </summary>
        /// <value>The manifest.</value>
        [Required]
        public ITaskItem Manifest { get; set; }

        /// <summary>
        /// Specifies the directory for the created NuGet package.
        /// </summary>
        /// <value>The output directory.</value>
        public string OutputDirectory { get; set; }

        /// <summary>
        /// The Base Path of the files defined in the nuspec file.
        /// </summary>
        /// <value>The base path.</value>
        public string BasePath { get; set; }

        /// <summary>
        /// Shows verbose output for package building.
        /// </summary>
        /// <value><c>true</c> if verbose; otherwise, <c>false</c>.</value>
        public bool Verbose { get; set; }

        /// <summary>
        /// Sets the version of the package in the manifest (Nuspec) file.
        /// The version must specify at least two places "X.X".
        /// </summary>
        /// <value>The version to set in the manifest.</value>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the working directory.
        /// </summary>
        /// <value>The working directory.</value>
        /// <returns>
        /// The directory in which to run the executable file, or a null reference (Nothing in Visual Basic) if the executable file should be run in the current directory.
        /// </returns>
        public string WorkingDirectory { get; set; }

        /// <summary>
        /// Runs the exectuable file with the specified task parameters.
        /// </summary>
        /// <returns>
        /// true if the task runs successfully; otherwise, false.
        /// </returns>
        public override bool Execute()
        {
            if (!string.IsNullOrEmpty(Version))
                UpdateVersion();

            return base.Execute();
        }

        private void UpdateVersion()
        {

            var document = new XmlDocument();
            document.Load(Manifest.ItemSpec);
            
            var navigator = document.CreateNavigator();
            var manager = new XmlNamespaceManager(navigator.NameTable);
            manager.AddNamespace("d", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");
            
            var expression = XPathExpression.Compile("/d:package/d:metadata/d:version", manager);
            var iterator = navigator.Select(expression);

            while (iterator.MoveNext())
                if (iterator.Current != null)
                    iterator.Current.SetValue(Version);

            using (var writer = new XmlTextWriter(Manifest.ItemSpec, Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                document.Save(writer);
                writer.Close();
            }

        }
        
        /// <summary>
        /// Returns a string value containing the command line arguments to pass directly to the executable file.
        /// </summary>
        /// <returns>
        /// A string value containing the command line arguments to pass directly to the executable file.
        /// </returns>
        protected override string GenerateCommandLineCommands()
        {
            CommandLineBuilder builder = new CommandLineBuilder();
            builder.AppendSwitch("package");
            builder.AppendFileNameIfNotNull(Manifest);
            builder.AppendSwitchIfNotNull("-OutputDirectory", OutputDirectory);
            builder.AppendSwitchIfNotNull("-BasePath", BasePath);
            if (Verbose)
                builder.AppendSwitch("-Verbose");

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
            if (string.IsNullOrEmpty(ToolPath))
                return ToolName;

            return Path.Combine(ToolPath, ToolName);
        }

        /// <summary>
        /// Gets the name of the executable file to run.
        /// </summary>
        /// <value></value>
        /// <returns>The name of the executable file to run.</returns>
        protected override string ToolName
        {
            get { return "NuGet.exe"; }
        }

        /// <summary>
        /// Returns the directory in which to run the executable file.
        /// </summary>
        /// <returns>
        /// The directory in which to run the executable file, or a null reference (Nothing in Visual Basic) if the executable file should be run in the current directory.
        /// </returns>
        protected override string GetWorkingDirectory()
        {
            return string.IsNullOrEmpty(WorkingDirectory) 
                ? base.GetWorkingDirectory() 
                : WorkingDirectory;
        }
    }
}