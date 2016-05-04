

using System;
using System.IO;
using System.Text;
using Microsoft.Build.Utilities;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests
{
    [TestFixture]
    public class FileUpdateTest
    {
        private string testDirectory;
        string[] files;

        [OneTimeSetUp]
		public void FixtureInit()
		{
            MockBuild buildEngine = new MockBuild();

            testDirectory = TaskUtility.makeTestDirectory(buildEngine);
            
            // Create test files
            files = new string[3];
            files[0] = Path.Combine(testDirectory, @"numberupdate.txt");
            files[1] = Path.Combine(testDirectory, @"versionupdate.txt");
            files[2] = Path.Combine(testDirectory, @"parsetestupdate.txt");
            
            File.WriteAllText(files[0], "1.0.0.0");
			File.WriteAllText(files[1], "1.0.0.0");
			File.WriteAllText(files[2], "Today's date is: @DATE@." + Environment.NewLine +
				"In case you didn't understand, today's date is: @DATE@!");
		}

        //[OneTimeTearDown]
        //public void FixtureDispose()
        //{
        //    // Clean up test files
        //    File.Delete(@".\number.txt");
        //    File.Delete(@".\version.txt");
        //    File.Delete(@".\parsetest.txt");
        //}

        [Test]
        public void TestFilesUpdate()
        {
            FileUpdate task = new FileUpdate();
            task.BuildEngine = new MockBuild();

            TaskItem[] items = TaskUtility.StringArrayToItemArray(files);

            task.Files = items;
            task.Regex = @"(\d+)\.(\d+)\.(\d+)\.(\d+)";
            task.ReplacementText = "$1.$2.$3.123";
            Assert.IsTrue(task.Execute(), "Execute Failed!");

			task = new FileUpdate();
			task.BuildEngine = new MockBuild();
			task.Files = items;
			task.Regex = @"@\w*?@";
			task.ReplacementText = DateTime.Now.ToString();
			Assert.IsTrue(task.Execute(), "Execute Failed!");
		}

        [Test]
        public void TestItemsNotUpdated()
        {
            FileUpdate task = new FileUpdate();
            task.BuildEngine = new MockBuild();

            TaskItem[] items = TaskUtility.StringArrayToItemArray(files);

            task.Files = items;
            task.Regex = @"(\d+)\.(\d+)\.(\d+)\.(\d+)";
            task.ReplacementText = "$1.$2.$3.123";
            Assert.IsTrue(task.Execute(), "Execute Failed!");

            task = new FileUpdate();
            task.BuildEngine = new MockBuild();
            task.Files = items;
            task.Regex = @"TestExitStatusAndNotUpdatedItems";
            task.ReplacementText = DateTime.Now.ToString();
            Assert.IsTrue(task.Execute(), "Execute Failed!");
            Assert.IsTrue(task.ItemsNotUpdated.Length == 3);
            Assert.IsFalse(task.AllItemsUpdated);
        }       
    }
}
