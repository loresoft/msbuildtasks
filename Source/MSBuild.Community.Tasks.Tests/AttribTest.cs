using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.IO;

namespace MSBuild.Community.Tasks.Tests
{
    [TestFixture]
    public class AttribTest
    {
        [TestFixtureSetUp]
        public void FixtureInit()
        {
            MockBuild buildEngine = new MockBuild();
            TaskUtility.makeTestDirectory(buildEngine);
        }

        [Test]
        public void ExecuteAttrib()
        {
            string attribFile = Path.Combine(TaskUtility.TestDirectory, @"attrib.txt");
            File.WriteAllText(attribFile, "This is a test file");

            Attrib task = new Attrib();
            task.BuildEngine = new MockBuild();
            task.Files = TaskUtility.StringArrayToItemArray(attribFile);
            task.ReadOnly = true;
            task.Hidden = true;
            task.System = true;
            task.Execute();

            bool isReadOnly = ((File.GetAttributes(attribFile) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly);
            bool isHidden = ((File.GetAttributes(attribFile) & FileAttributes.Hidden) == FileAttributes.Hidden);
            bool isSystem = ((File.GetAttributes(attribFile) & FileAttributes.System) == FileAttributes.System);

            Assert.IsTrue(isReadOnly, "Attribute should be readonly");
            Assert.IsTrue(isHidden, "Attribute should be hidden");
            Assert.IsTrue(isSystem, "Attribute should be system");

            task = new Attrib();
            task.BuildEngine = new MockBuild();
            task.Files = TaskUtility.StringArrayToItemArray(attribFile);
            task.Hidden = false;
            task.System = false;
            task.Execute();

            isReadOnly = ((File.GetAttributes(attribFile) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly);
            isHidden = ((File.GetAttributes(attribFile) & FileAttributes.Hidden) == FileAttributes.Hidden);
            isSystem = ((File.GetAttributes(attribFile) & FileAttributes.System) == FileAttributes.System);

            Assert.IsTrue(isReadOnly, "Attribute should be readonly");
            Assert.IsFalse(isHidden, "Attribute should not be hidden");
            Assert.IsFalse(isSystem, "Attribute should not be system");

            task = new Attrib();
            task.BuildEngine = new MockBuild();
            task.Files = TaskUtility.StringArrayToItemArray(attribFile);
            task.Normal = true;
            task.Execute();

            isReadOnly = ((File.GetAttributes(attribFile) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly);
            Assert.IsFalse(isReadOnly, "Attribute should not be readonly");

        }
    }
}
