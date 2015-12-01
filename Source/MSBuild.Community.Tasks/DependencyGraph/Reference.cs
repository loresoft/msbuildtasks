using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSBuild.Community.Tasks.DependencyGraph
{
    /// <summary>
    /// Base class for all references 
    /// </summary>
    public class BaseReference
    {
        /// <summary>
        /// A name for display in a graph
        /// </summary>
        public string DisplayName { get; private set; }

        /// <summary>
        /// Creates new instance
        /// </summary>
        /// <param name="displayName">A name for display in a graph</param>
        protected BaseReference(string displayName)
        {
            DisplayName = displayName;
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }

    /// <summary>
    /// Represents an assembly reference inside a project file
    /// </summary>
    public class AssemblyReference : BaseReference
    {
        /// <summary>
        /// Name of the assembly of an assembly reference
        /// </summary>
        public string Include { get; private set; }

        /// <summary>
        /// HintPath, or relative path to file in an assembly reference
        /// </summary>
        public string HintPath { get; private set; }

        /// <summary>
        /// Creates new instance
        /// </summary>
        /// <param name="include">The name of the assembly reference</param>
        /// <param name="hintPath">The hint path, if aplicable</param>
        public AssemblyReference(string include, string hintPath)
            : base(MakeDisplayName(include))
        {
            Include = include;
            HintPath = hintPath;
        }

        private static string MakeDisplayName(string include)
        {
            var result = ProjectFileParser.GetAssemblyNameFromFullName(include);
            return string.IsNullOrEmpty(result) ? include : result;
        }
    }

    /// <summary>
    /// Represents a project reference inside a project file
    /// </summary>
    public class ProjectReference : BaseReference
    {
        /// <summary> 
        /// Path to a project file of reference 
        /// </summary>
        public string Include { get; private set; }

        /// <summary>
        /// GUID of referenced project
        /// </summary>
        public string Project { get; private set; }

        /// <summary>
        /// Name of referenced project
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Creates new instance
        /// </summary>
        /// <param name="include">The name of the assembly reference</param>
        /// <param name="project">GUID of referenced project</param>
        /// <param name="name">Name of referenced project</param>
        public ProjectReference(string include, string project, string name)
            : base (name)
        {
            Include = include;
            Project = project;
            Name = name;
        }
    }
}
