using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests
{
    [TestFixture]
    public class FileUpdateTest
    {
        [Test]
        public void FileUpdateExecute()
        {
            VersionTest version = new VersionTest();
            version.VersionExecute();

            FileUpdate task = new FileUpdate();
            task.BuildEngine = new MockBuild();
            task.FileName = "version.txt";
            task.Regex = @"(\d+)\.(\d+)\.(\d+)\.(\d+)";
            task.ReplacementText = "$1.$2.$3.123";
            
            Assert.IsTrue(task.Execute(), "Execute Failed");

            
        }
    }
}
