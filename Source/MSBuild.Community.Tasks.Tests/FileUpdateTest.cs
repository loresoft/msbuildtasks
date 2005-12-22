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
			File.WriteAllText(@".\pathtest.txt", "Today's date is: @DATE@." + Environment.NewLine +
				"In case you didn't understand, today's date is: @DATE@!");
		}

		[TestFixtureTearDown]
		public void FixtureDispose()
		{
			// Clean up test files
			File.Delete(@".\number.txt");
			File.Delete(@".\version.txt");
			File.Delete(@".\pathtest.txt");
		}

        [Test]
        public void TestFilesPropertyUpdate()
        {
            FileUpdate task = new FileUpdate();
            task.BuildEngine = new MockBuild();

            string[] files = new string[] {"number.txt", "version.txt"};
            TaskItem[] items = TaskUtility.StringArrayToItemArray(files);

            task.Files = items;
            task.Regex = @"(\d+)\.(\d+)\.(\d+)\.(\d+)";
            task.ReplacementText = "$1.$2.$3.123";
            Assert.IsTrue(task.Execute(), "Execute Failed!");
		}

		[Test]
		public void TestPathPropertyUpdate()
		{
			// Test inputting a path
			FileUpdate task = new FileUpdate();
			task.BuildEngine = new MockBuild();
			task.Path = @".\";
			task.Regex = @"@\w*?@";
			task.ExcludeFileTypes = ".exe, .dll, .compiled, .jpg, .gif, .pdb";
			task.ReplacementText = DateTime.Now.ToString();
			Assert.IsTrue(task.Execute(), "Execute Failed!");
		}
    }
}
