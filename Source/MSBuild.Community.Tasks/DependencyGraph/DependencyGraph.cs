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
    public class DependencyGraph : Task
    {

        /// <summary>
        /// Project files to parse
        /// </summary>
        [Required]
        public ITaskItem[] InputFiles
        {
            get;
            set;
        }

        /// <summary>
        /// A set of regular expression to filter the input files.
        /// </summary>
        public ITaskItem[] Filters
        {
            get;
            set;
        }

        /// <summary>
        /// FileName to output results
        /// </summary>        
        public string OutputFile { get; set; }

        private Dictionary<string, List<string>> dependencies = new Dictionary<string, List<string>>();

        /// <summary>
        /// Runs the task.
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            if (InputFiles == null)
            {
                Log.LogError("No input files!");
                return false;
            }

            foreach (ITaskItem file in InputFiles)
            {
                var fullPath = file.GetMetadata("FullPath");
                Log.LogMessage("Parsing " + fullPath);
                FileStream fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
                using (fs)
                {
                    ProjectFileParser p = new ProjectFileParser(fs);
                    if (!Filtered(p.GetAssemblyName()))
                    {
                        if (String.IsNullOrEmpty(p.GetAssemblyName()))
                            Log.LogWarning("Não foi possível identificar AssemblyName para: " + file.GetMetadata("FullPath"));

                        foreach (ProjectReference r in p.GetReferences())
                        {
                            if (String.IsNullOrEmpty(p.GetAssemblyNameFromFullName(r.Include)))
                                Log.LogWarning("Não foi possível identificar referência encontrada em : " + file.GetMetadata("FullPath"));

                            if (!Filtered(p.GetAssemblyNameFromFullName(r.Include)))
                                AddDependency(p.GetAssemblyName(), p.GetAssemblyNameFromFullName(r.Include));
                        }
                    }
                }
            }

            Stream s = GenerateGraphVizOutput();
            LogToConsole(s);
            LogToFile(s);

            return true;
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

        private Stream GenerateGraphVizOutput()
        {
            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms);

            sw.WriteLine("digraph {");
            foreach (KeyValuePair<string, List<string>> p in this.dependencies)
                foreach (string dep in p.Value)
                    sw.WriteLine("\t\"{0}\" -> \"{1}\";", p.Key, dep);
            sw.WriteLine("}");
            sw.Flush();
            
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        private void AddDependency(string projectName, string referenceName)
        {
            if (!dependencies.ContainsKey(projectName))
                dependencies.Add(projectName, new List<string>());

            dependencies[projectName].Add(referenceName);
        }

        private bool Filtered(string identifier)
        {
            if (Filters == null || Filters.Length <= 0)
                return false;

            bool filtered = false;
            foreach (ITaskItem filter in Filters)
            {
                Regex r = new Regex(filter.ItemSpec);
                filtered = r.IsMatch(identifier);
                if (filtered)
                    break;
            }
            return filtered;
        }

    }
}
