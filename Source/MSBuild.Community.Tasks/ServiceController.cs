using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Utilities;

namespace MSBuild.Community.Tasks
{
    /// <summary>
    /// Task that can control a Windows service.
    /// </summary>
    public class ServiceController : Task
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
