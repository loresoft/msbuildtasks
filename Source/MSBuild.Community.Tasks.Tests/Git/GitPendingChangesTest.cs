using System;
using System.IO;
using MSBuild.Community.Tasks.Git;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests.Git
{
    [TestFixture]
    public class GitPendingChangesTest
    {
        [Test]
        public void Execute_PendingChanges_False()
        {
            // NOTE: this test will fail is there are pending changes in the Git repository
            var task = new GitPendingChanges();
            task.BuildEngine = new MockBuild();
            task.ToolPath = @"C:\Program Files (x86)\Git\bin";

            string prjRootPath = TaskUtility.GetProjectRootDirectory(true);
            task.LocalPath = Path.Combine(prjRootPath, @"Source");

            bool result = task.Execute();

            Assert.IsTrue(result, "Execute failed.");
            Assert.IsFalse(task.HasPendingChanges, "There should be no pending changes.");
        }

        [Test]
        public void Execute_PendingChanges_True()
        {
            var task = new GitPendingChanges();
            task.BuildEngine = new MockBuild();
            task.ToolPath = @"C:\Program Files (x86)\Git\bin";

            string prjRootPath = TaskUtility.GetProjectRootDirectory(true);
            task.LocalPath = Path.Combine(prjRootPath, @"Source");

            // create a temporary file in the source repository, to be picked up by git status
            string temporaryPath = prjRootPath;
            temporaryPath = Path.Combine(temporaryPath, @"Source");
            temporaryPath = Path.Combine(temporaryPath, @"MSBuild.Community.Tasks.Tests");
            temporaryPath = Path.Combine(temporaryPath, @"Git");
            temporaryPath = Path.Combine(temporaryPath, Guid.NewGuid() + ".txt");
            bool result;
            try
            {
                File.WriteAllText(temporaryPath, string.Format("This is a test file used for a test in {0}, and will be deleted after the .", GetType().FullName));
                result = task.Execute();
            }
            finally
            {
                // make sure the file gets deleted
                if (File.Exists(temporaryPath))
                {
                    File.Delete(temporaryPath);
                }
            }

            Assert.IsTrue(result, "Execute failed.");
            Assert.IsTrue(task.HasPendingChanges, "There should be pending changes.");
        }

        [Test]
        public void Execute_PendingChanges_Argument_False()
        {
            // NOTE: this test will fail if "packages.config" has pending changes in the Git repository
            var task = new GitPendingChanges();
            task.BuildEngine = new MockBuild();
            task.ToolPath = @"C:\Program Files (x86)\Git\bin";
            task.Arguments = "packages.config";

            string prjRootPath = TaskUtility.GetProjectRootDirectory(true);
            task.LocalPath = Path.Combine(prjRootPath, @"Source");

            bool result = task.Execute();

            Assert.IsTrue(result, "Execute failed.");
            Assert.IsFalse(task.HasPendingChanges, "There should be no pending changes.");
        }

        [Test]
        public void Execute_PendingChanges_Argument_True()
        {
            var task = new GitPendingChanges();
            task.BuildEngine = new MockBuild();
            task.ToolPath = @"C:\Program Files (x86)\Git\bin";

            string prjRootPath = TaskUtility.GetProjectRootDirectory(true);
            task.LocalPath = Path.Combine(prjRootPath, @"Source");

            // create a temporary file in the source repository, to be picked up by git status
            string temporaryPath = prjRootPath;
            temporaryPath = Path.Combine(temporaryPath, @"Source");
            temporaryPath = Path.Combine(temporaryPath, @"MSBuild.Community.Tasks.Tests");
            temporaryPath = Path.Combine(temporaryPath, @"Git");
            temporaryPath = Path.Combine(temporaryPath, Guid.NewGuid() + ".txt");
            task.Arguments = temporaryPath;
            bool result;
            try
            {
                File.WriteAllText(temporaryPath, string.Format("This is a test file used for a test in {0}, and will be deleted after the .", GetType().FullName));
                result = task.Execute();
            }
            finally
            {
                // make sure the file gets deleted
                if (File.Exists(temporaryPath))
                {
                    File.Delete(temporaryPath);
                }
            }

            Assert.IsTrue(result, "Execute failed.");
            Assert.IsTrue(task.HasPendingChanges, "There should be pending changes.");
        }
    }
}