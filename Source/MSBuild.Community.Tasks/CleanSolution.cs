using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Utilities;

namespace MSBuild.Community.Tasks
{
    /// <summary>
    /// Task that removes source control binding information amd status
    /// files from a solution tree.
    /// </summary>
    /// <remarks>At the moment, this task can only clean solutions with
    /// that contain C# projects.</remarks>
    public class CleanSolution : Task
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
