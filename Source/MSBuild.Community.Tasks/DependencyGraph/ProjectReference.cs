using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSBuild.Community.Tasks.DependencyGraph
{
    /// <summary>
    /// Represents a project reference inside a project file
    /// </summary>
    public class ProjectReference
    {
        /// <summary>
        /// Name of the assembly of a project reference
        /// </summary>
        public string Include
        {
            get;
            private set;
        }

        /// <summary>
        /// HintPath, or relative path to file in a project reference
        /// </summary>
        public string HintPath
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates new instance
        /// </summary>
        /// <param name="include">The name of the assembly reference</param>
        /// <param name="HintPath">The hint path, if aplicable</param>
        public ProjectReference(string include, string HintPath)
        {
            this.Include = include;
            this.HintPath = HintPath;
        }

        /// <summary>
        /// Creates new instance specifying only the referenced assembly
        /// </summary>
        /// <param name="include"></param>
        public ProjectReference(string include)
        {
            this.Include = include;
            this.HintPath = String.Empty;
        }

    }
}
