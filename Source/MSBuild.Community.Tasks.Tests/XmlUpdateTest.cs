

using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests
{
    /// <summary>
    /// Summary description for XmlUpdateTest
    /// </summary>
    [TestFixture]
    public class XmlUpdateTest
    {
        string prjRootPath;
        string testFile;

        public XmlUpdateTest()
        {
            prjRootPath = TaskUtility.GetProjectRootDirectory(true);
            testFile = Path.Combine(prjRootPath, @"Source\Test\Subversion.proj");
        }

        [SetUp]
        public void Setup()
        {
            string sourceFile = Path.Combine(prjRootPath, @"Source\Subversion.proj");
            string testFileFolder = Path.GetDirectoryName(testFile);
            if (!Directory.Exists(testFileFolder))
            {
                Directory.CreateDirectory(testFileFolder);
            }
            File.Copy(sourceFile, testFile, true);
        }

        [TearDown]
        public void Cleanup()
        {
            File.Delete(testFile);
        }

        [Test(Description="Update an XML file with XPath navigation")]
        public void XmlUpdateExecute()
        {
            XmlUpdate task = new XmlUpdate();
            task.BuildEngine = new MockBuild();
            
            task.Prefix = "n";
            task.Namespace = "http://schemas.microsoft.com/developer/msbuild/2003";
            task.XmlFileName = testFile;
            task.XPath = "/n:Project/n:PropertyGroup/n:LastUpdate";
            task.Value = DateTime.Now.ToLongDateString();
            
            Assert.IsTrue(task.Execute(), "Execute Failed");

        }


        [Test(Description = "Update an XML file with XPath navigation")]
        public void XmlUpdateDelete()
        {
            XmlUpdate task = new XmlUpdate();
            task.BuildEngine = new MockBuild();

            task.Prefix = "n";
            task.Namespace = "http://schemas.microsoft.com/developer/msbuild/2003";
            task.XmlFileName = testFile;
            task.XPath = "/n:Project/n:PropertyGroup/n:LastUpdate";
            task.Delete = true;

            Assert.IsTrue(task.Execute(), "Execute Failed");

        }
    }
}
