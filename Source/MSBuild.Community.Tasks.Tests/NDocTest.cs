

using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests
{
    /// <summary>
    /// Summary description for NDocTest
    /// </summary>
    [TestFixture]
    public class NDocTest
    {
        [Test(Description="Generate NDoc project")]
        public void NDocExecute()
        {
            string prjRootPath = TaskUtility.GetProjectRootDirectory(true);
            string workingDir = Path.Combine(prjRootPath, @"Documentation");

            NDoc task = new NDoc();
            task.BuildEngine = new MockBuild();
            task.ToolPath = Path.Combine(workingDir, "ndoc");
            task.ProjectFilePath = Path.Combine(workingDir, @"MSBuild.Community.Tasks.ndoc");
            task.WorkingDirectory = workingDir;
            task.Documenter = "XML";
            Assert.IsTrue(task.Execute(), "Execute Failed");
        }
    }
}
