using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Utilities;

namespace MSBuild.Community.Tasks.SourceSafe
{
    /// <summary>
    /// Task that removes source control binding information and status
    /// files from a solution tree.
    /// </summary>
    /// <remarks>At the moment, this task can only clean solutions with
    /// that contain C# projects.</remarks>
    public class VssClean : Task
    {
        /// <summary>
        /// Executes the task.
        /// </summary>
        /// <returns><see langword="true"/> if the task ran successfully; 
        /// otherwise <see langword="false"/>.</returns>
        public override bool Execute()
        {
            throw new NotImplementedException();
        }
    }
}
