using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using Microsoft.Build.Utilities;

namespace MSBuild.Community.Tasks.Tests
{
    [TestFixture]
    public class FileUpdateTest
    {
		[TestFixtureSetUp]
		public void FixtureInit()
		{
			// Create test files
			File.WriteAllText(@".\number.txt", "1.0.0.0");
			File.WriteAllText(@".\version.txt", "1.0.0.0");
			File.WriteAllText(@".\parsetest.txt", "Today's date is: @DATE@." + Environment.NewLine +
				"In case you didn't understand, today's date is: @DATE@!");
		}

		[TestFixtureTearDown]
		public void FixtureDispose()
		{
			// Clean up test files
			File.Delete(@".\number.txt");
			File.Delete(@".\version.txt");
			File.Delete(@".\parsetest.txt");
		}

        [Test]
        public void TestFilesUpdate()
        {
            FileUpdate task = new FileUpdate();
            task.BuildEngine = new MockBuild();

			string[] files = new string[] { "number.txt", "version.txt", "parsetest.txt" };
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
    }
}
