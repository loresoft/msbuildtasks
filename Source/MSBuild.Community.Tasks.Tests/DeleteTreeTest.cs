using System;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using NUnit.Framework;
using Task = System.Threading.Tasks.Task;

namespace MSBuild.Community.Tasks.Tests
{
    [TestFixture]
    public class DeleteTreeTest
    {
        [Test]
        public void GetDirectories()
        {
            var root = Path.GetFullPath(@"..\..\..\");

            var _ = Directory.GetDirectories(root, "*", SearchOption.AllDirectories);
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

        [Test]
        [Explicit]
        public void DeleteWithRetriesDirectoryWhichAppearsWithDelay_ShouldSucceed()
        {
            //Arrange
            const string fullTargetDirectoryPath = @"C:\Temp\xn_TestDirectoryWhichDoesntInitiallyExist";

            var task = new DeleteTree
            {
                BuildEngine = new MockBuild(),
                
                Retries = 10,
                RetryDelayMilliseconds = 300,

                Directories = new ITaskItem[]
                {
                    new TaskItem(fullTargetDirectoryPath)
                }
            };
            
            //Act
            new Task(() =>
            {
                Thread.Sleep(2000);

                Directory.CreateDirectory(fullTargetDirectoryPath);
            }).Start();
            var result = task.Execute();

            //Assert
            Assert.IsTrue(result);
            Assert.AreEqual(1, task.DeletedDirectories.Length);
            Assert.AreEqual(fullTargetDirectoryPath, task.DeletedDirectories.FirstOrDefault().ItemSpec);
        }

        [Test]
        [Explicit]
        public void DeleteWithRetriesDirectoryWhichDoesntAppearsAtAll_ShouldFailWithException()
        {
            //Arrange
            const string fullTargetDirectoryPath = @"C:\Temp\xn_TestDirectoryWhichDoesntInitiallyExist";

            var task = new DeleteTree
            {
                BuildEngine = new MockBuild(),
                
                Retries = 10,
                RetryDelayMilliseconds = 300,

                Directories = new ITaskItem[]
                {
                    new TaskItem(fullTargetDirectoryPath)
                }
            };

            var exception = (Exception) null;
            
            //Act
            try
            {
                task.Execute();
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            
            //Assert
            Assert.IsInstanceOf<DirectoryNotFoundException>(exception);
        }
    }
}

