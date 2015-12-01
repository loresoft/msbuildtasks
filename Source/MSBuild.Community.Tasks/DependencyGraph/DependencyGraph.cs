using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MSBuild.Community.Tasks.DependencyGraph
{
    /// <summary>
    /// Reads a set of project files (.csproj, .vbproj) in InputFiles and generate a GraphViz style syntax.
    /// You can paste the result of the graphs in places like http://graphviz-dev.appspot.com/ to see your chart or
    /// run the file using the GraphViz tool http://www.graphviz.org/
    /// </summary>
    /// <example>
    /// <code><![CDATA[
    /// 	<ItemGroup>
	///         <Dependency Include="Trader.csproj" />
	///     </ItemGroup>
    ///
	///     <Target Name="Default">
    ///		    <DependencyGraph InputFiles="@(Dependency)" IsIncludeProjectDependecies="true" ExcludeProjectReferences="API;UI" />	<!-- API and UI project references will be excluded -->
    ///     </Target>
    ///
    /// ]]></code></example>
    /// <para>Result:</para>
    /// <para><![CDATA[  digraph {
    ///    "Trader" -> "System";
    ///    "Trader" -> "System.configuration";
    ///    "Trader" -> "System.Data";
    ///    "Trader" -> "System.Deployment";
    ///    "Trader" -> "System.Design";
    ///    "Trader" -> "System.Drawing";
    ///    "Trader" -> "System.Drawing.Design";
    ///    "Trader" -> "System.EnterpriseServices";
    ///    "Trader" -> "System.Web";
    ///    "Trader" -> "System.Web.Services";
    ///    "Trader" -> "System.Windows.Forms";
    ///    "Trader" -> "System.Xml";
    ///    "Trader" -> "Project References";
    ///            "Project References" -> "CommLib";
    ///            "Project References" -> "DdeApi";</para>
    ///  ]]></para>
    /// <para>
    ///     Other attributes:
    /// <list type="table">
    /// <item>
    ///     <term>Exclude</term>
    ///     <description>filter input files</description>
    /// </item>
    ///  <item>
    ///     <term>ExcludeReferences</term>
    ///     <description>filter referenced assemblies</description>
    /// </item>
    /// </list>
    /// </para>
    public class DependencyGraph : Task
    {
        private static readonly Regex[] EmptyRegexBuffer = new Regex[0];

        private readonly Dictionary<string, List<BaseReference>> _dependencies = new Dictionary<string, List<BaseReference>>();
        private Regex[] _excludeRegex;
        private Regex[] _excludeReferencesRegex;
        private Regex[] _excludeProjectReferencesRegex;

        /// <summary> Project files to parse </summary>
        [Required]
        public ITaskItem[] InputFiles { get; set; }

        /// <summary>A set of regular expression to filter the input files</summary>
        public ITaskItem[] Exclude { get; set; }

        private Regex[] ExcludeRegex
        {
            get { return MakeFilterRegex(ref _excludeRegex, Exclude); }
        }

        /// <summary>A set of regular expression to filter the referenced assemblies</summary>
        public ITaskItem[] ExcludeReferences { get; set; }

        private Regex[] ExcludeReferencesRegex
        {
            get { return MakeFilterRegex(ref _excludeReferencesRegex, ExcludeReferences); }
        }

        /// <summary>A set of regular expression to filter the referenced projects</summary>
        public ITaskItem[] ExcludeProjectReferences { get; set; }

        private Regex[] ExcludeProjectReferencesRegex
        {
            get { return MakeFilterRegex(ref _excludeProjectReferencesRegex, ExcludeProjectReferences); }
        }

        /// <summary>includes project dependencies to output</summary>
        public bool IsIncludeProjectDependecies { get; set; }

        /// <summary>FileName to output results</summary>        
        public string OutputFile { get; set; }

        public override bool Execute()
        {
            if (InputFiles == null)
            {
                Log.LogError("No input files!");
                return false;
            }

            foreach (ITaskItem file in InputFiles)
                ProcessProject(file.GetMetadata("FullPath"));

            Stream s = GenerateGraphVizOutput();
            LogToConsole(s);
            LogToFile(s);

            return true;
        }

        private void ProcessProject(string fullFileName)
        {
            Log.LogMessage("Parsing " + fullFileName);
            FileStream fs = new FileStream(fullFileName, FileMode.Open, FileAccess.Read);
            using (fs)
            {
                ProjectFileParser parser = new ProjectFileParser(fs);
                var assemblyName = parser.GetAssemblyName();

                if (!IsApplicable(ExcludeRegex, assemblyName))
                    return;

                AddApplicableReference(assemblyName, ExcludeReferencesRegex, parser.GetAssemblyReferences());
                if (IsIncludeProjectDependecies)
                    AddApplicableReference(assemblyName, ExcludeProjectReferencesRegex, parser.GetProjectReferences());
            }
        }

        private void AddApplicableReference(string projectName, Regex[] filters, IEnumerable<BaseReference> references)
        {
            foreach (BaseReference reference in references.Where(reference => IsApplicable(filters, reference.DisplayName)))
                AddDependency(projectName, reference);
        }

        public static void Test()
        {
            var rex = new Regex("System");
            Console.WriteLine(rex.IsMatch("Ionic.Zip.Reduced.dll"));
        }

        private void AddDependency(string projectName, BaseReference baseReference)
        {
            List<BaseReference> list;
            if (!_dependencies.TryGetValue(projectName, out list))
            {
                list = new List<BaseReference>();
                _dependencies.Add(projectName, list);
            }
            list.Add(baseReference);
        }

        private Stream GenerateGraphVizOutput()
        {
            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms);

            sw.WriteLine("digraph {");

            foreach (KeyValuePair<string, List<BaseReference>> p in _dependencies)
            {
                foreach (var dep in p.Value.OfType<AssemblyReference>())
                    sw.WriteLine("\t\"{0}\" -> \"{1}\";", p.Key, dep);

                if (IsIncludeProjectDependecies && p.Value.Any())
                {
                    const string references = "Project References";
                    sw.WriteLine("\t\"{0}\" -> \"{1}\";", p.Key, references);
                    foreach (var dep in p.Value.OfType<ProjectReference>())
                        sw.WriteLine("\t\t\"{0}\" -> \"{1}\";", references, dep);
                }
            }

            sw.WriteLine("}");
            sw.Flush();

            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        private void LogToFile(Stream s)
        {
            if (String.IsNullOrEmpty(OutputFile))
                return;

            s.Seek(0, SeekOrigin.Begin);
            FileStream fs = new FileStream(OutputFile, FileMode.Create, FileAccess.Write);
            using (fs)
                CopyStream(s, fs);
        }

        private void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[32768];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
        }

        private void LogToConsole(Stream s)
        {
            s.Seek(0, SeekOrigin.Begin);
            StreamReader rd = new StreamReader(s);
            Log.LogMessage(rd.ReadToEnd());
        }

        private static Regex[] MakeFilterRegex(ref Regex[] buffer, IEnumerable<ITaskItem> items)
        {
            return 
                buffer 
                ?? (buffer = items == null
                    ? EmptyRegexBuffer
                    : items.Select(filter => new Regex(filter.ItemSpec)).ToArray());
        }

        private static bool IsApplicable(Regex[] filters, string name)
        {
            return filters.Length == 0 || filters.All(filter => !filter.IsMatch(name));
        }
    }
}
