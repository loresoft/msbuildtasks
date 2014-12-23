using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests
{
    [TestFixture]
    public class DeleteTreeTest
    {
        [Test]
        public void GetDirectories()
        {
            string root = Path.GetFullPath(@"..\..\..\");

            string[] directories = Directory.GetDirectories(root, "*", SearchOption.AllDirectories);
        }

        [Test]
        [Explicit]
        public void ExecuteInnerAndTrailingRecursive()
        {
            DeleteTree task = new DeleteTree();
            task.BuildEngine = new MockBuild();
            task.Directories = new ITaskItem[] { new TaskItem(@"..\..\..\**\obj\**")};
            bool result = task.Execute();

            Assert.IsTrue(result);
        }

        [Test]
        [Explicit]
        public void ExecuteInnerRecursive()
        {
            DeleteTree task = new DeleteTree();
            task.BuildEngine = new MockBuild();
            task.Directories = new ITaskItem[] { new TaskItem(@"..\..\..\**\obj") };
            bool result = task.Execute();

            Assert.IsTrue(result);
        }

        [Test]
        [Explicit]
        public void ExecuteItemWithTrailingSeparator()
        {
            DeleteTree task = new DeleteTree();
            task.BuildEngine = new MockBuild();
            task.Directories = new ITaskItem[] { new TaskItem(@"..\..\..\**\obj\") };
            bool result = task.Execute();

            Assert.IsTrue(result);
        }

        [Test]
        [Explicit]
        public void ExecuteMultipleItems()
        {
            DeleteTree task = new DeleteTree();
            task.BuildEngine = new MockBuild();
            task.Directories = new ITaskItem[]
                                   {
                                       new TaskItem(@"..\..\..\**\obj"), 
                                       new TaskItem(@"..\..\..\**\bin")
                                   };

            bool result = task.Execute();

            Assert.IsTrue(result);
        }

        [Test]
        [Explicit]
        public void ExecuteWildCard()
        {
            DeleteTree task = new DeleteTree();
            task.BuildEngine = new MockBuild();
            task.Directories = new ITaskItem[]
                                   {
                                       new TaskItem(@"..\..\..\MSBuild.Community.*\**\obj"), 
                                       new TaskItem(@"..\..\..\**\b?n")
                                   };

            bool result = task.Execute();

            Assert.IsTrue(result);
        }

        [Test]
        public void MatchDirectoriesNoWildCards()
        {
            var path = DeleteTree.MatchDirectories(@"..\..\..");
            Assert.AreEqual(1, path.Count);
        }

        [Test]
        public void MatchDirectoriesInnerRelative()
        {
            var path = DeleteTree.MatchDirectories(@"..\..\..\**\obj");
            Assert.Greater(path.Count, 0);
        }

        [Test]
        public void MatchDirectoriesInnerAndOuterRelative()
        {
            var path = DeleteTree.MatchDirectories(@"..\..\..\**\obj\**");
            Assert.Greater(path.Count, 0);
        }


        [Test]
        public void MatchDirectoriesRelative()
        {
            var path = DeleteTree.MatchDirectories(@"..\..\..\**");
            Assert.Greater(path.Count, 0);
        }

        [Test]
        public void MatchDirectoriesRelativeWildCard()
        {
            var path = DeleteTree.MatchDirectories(@"..\..\..\MSBuild.Community.*\**\obj");
            Assert.Greater(path.Count, 0);

            path = DeleteTree.MatchDirectories(@"..\..\..\MSBuild.*.Tests\**\obj\**");
            Assert.Greater(path.Count, 0);
        }

        
    }
}
