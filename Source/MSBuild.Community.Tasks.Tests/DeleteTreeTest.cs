using System;
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
            var root = Path.GetFullPath(@"..\..\..\");

            var directories = Directory.GetDirectories(root, "*", SearchOption.AllDirectories);
        }

        [Test]
        [Explicit]
        public void ExecuteInnerAndTrailingRecursive()
        {
            var task = new DeleteTree
            {
                BuildEngine = new MockBuild(),
                Directories = new ITaskItem[]
                {
                    new TaskItem(@"..\..\..\**\obj\**")
                }
            };

            var result = task.Execute();

            Assert.IsTrue(result);
        }

        [Test]
        [Explicit]
        public void ExecuteInnerRecursive()
        {
            var task = new DeleteTree
            {
                BuildEngine = new MockBuild(),
                Directories = new ITaskItem[]
                {
                    new TaskItem(@"..\..\..\**\obj")
                }
            };

            var result = task.Execute();

            Assert.IsTrue(result);
        }

        [Test]
        [Explicit]
        public void ExecuteItemWithTrailingSeparator()
        {
            var task = new DeleteTree
            {
                BuildEngine = new MockBuild(),
                Directories = new ITaskItem[]
                {
                    new TaskItem(@"..\..\..\**\obj\")
                }
            };

            var result = task.Execute();

            Assert.IsTrue(result);
        }

        [Test]
        [Explicit]
        public void ExecuteMultipleItems()
        {
            var task = new DeleteTree
            {
                BuildEngine = new MockBuild(),
                Directories = new ITaskItem[]
                {
                    new TaskItem(@"..\..\..\**\obj"),
                    new TaskItem(@"..\..\..\**\bin")
                }
            };

            var result = task.Execute();

            Assert.IsTrue(result);
        }

        [Test]
        [Explicit]
        public void ExecuteWildCard()
        {
            var task = new DeleteTree
            {
                BuildEngine = new MockBuild(),
                Directories = new ITaskItem[]
                {
                    new TaskItem(@"..\..\..\MSBuild.Community.*\**\obj"),
                    new TaskItem(@"..\..\..\**\b?n")
                }
            };

            var result = task.Execute();

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