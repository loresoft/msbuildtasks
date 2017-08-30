

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ionic.Zip;
using Microsoft.Build.Utilities;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests
{
    /// <summary>
    /// Summary description for ZipTest
    /// </summary>
    [TestFixture]
    public class ZipTest
    {
        public const string ZIP_FILE_NAME = @"MSBuild.Community.Tasks.zip";
        public const string ZIP_WITH_FOLDERS_FILE_NAME = @"MSBuild.Community.Tasks.WithFolders.zip";
        public const string ZIP_WITH_RELATIVE_FILE_NAME = @"MSBuild.Community.Tasks.WithReltive.zip";

        [Test(Description = "Zip files into a zip archive")]
        public void ZipExecute()
        {
            var task = new Zip();
            task.BuildEngine = new MockBuild();

            string testDir = TaskUtility.TestDirectory;
            string prjRootPath = TaskUtility.GetProjectRootDirectory(true);

            string workingDir = Path.Combine(prjRootPath, @"Source\MSBuild.Community.Tasks.Tests");
            string[] files = Directory.GetFiles(workingDir, "*.*", SearchOption.TopDirectoryOnly);

            TaskItem[] items = TaskUtility.StringArrayToItemArray(files);

            task.Files = items;
            task.WorkingDirectory = workingDir;
            task.ZipFileName = Path.Combine(testDir, ZIP_FILE_NAME);

            if (File.Exists(task.ZipFileName))
                File.Delete(task.ZipFileName);

            Assert.IsTrue(task.Execute(), "Execute Failed");
            Assert.IsTrue(File.Exists(task.ZipFileName), "Zip file not found");
        }

        [Test(Description = "Zip files and empty folders into a zip archive")]
        public void ZipExecuteWithEmptyFolders()
        {
            var task = new Zip();
            task.BuildEngine = new MockBuild();

            string testDir = TaskUtility.TestDirectory;
            string prjRootPath = TaskUtility.GetProjectRootDirectory(true);

            string workingDir = Path.Combine(prjRootPath, @"Source\MSBuild.Community.Tasks.Tests");
            string[] files = Directory.GetFiles(workingDir, "*.*", SearchOption.TopDirectoryOnly);
            string[] directories = Directory.GetDirectories(workingDir, "*.*", SearchOption.TopDirectoryOnly);
            string[] filesFromXmlDirectory = new string[0]; //Directory.GetFiles(workingDir + "\\" + "Xml", "*.*", SearchOption.TopDirectoryOnly);

            var filesAndDirectories = new string[files.Length + directories.Length + filesFromXmlDirectory.Length];


            files.CopyTo(filesAndDirectories, 0);
            directories.CopyTo(filesAndDirectories, files.Length + filesFromXmlDirectory.Length);
            filesFromXmlDirectory.CopyTo(filesAndDirectories, files.Length);

            TaskItem[] items = TaskUtility.StringArrayToItemArray(filesAndDirectories);

            task.Files = items;
            task.WorkingDirectory = workingDir;
            task.ZipFileName = Path.Combine(testDir, ZIP_WITH_FOLDERS_FILE_NAME);

            if (File.Exists(task.ZipFileName))
                File.Delete(task.ZipFileName);

            Assert.IsTrue(task.Execute(), "Execute Failed");
            Assert.IsTrue(File.Exists(task.ZipFileName), "Zip file not found");
        }

        [Test]
        public void ZipNoRoot()
        {
            var task = new Zip();
            task.BuildEngine = new MockBuild();

            string testDir = TaskUtility.TestDirectory;
            string prjRootPath = TaskUtility.GetProjectRootDirectory(true);

            string workingDir = Path.Combine(prjRootPath, @"Source\MSBuild.Community.Tasks.Tests");
            string[] files = Directory.GetFiles(workingDir, "*.*", SearchOption.AllDirectories);

            TaskItem[] items = TaskUtility.StringArrayToItemArray(files);

            task.Files = items;
            task.ZipFileName = Path.Combine(testDir, ZIP_FILE_NAME);

            if (File.Exists(task.ZipFileName))
                File.Delete(task.ZipFileName);

            Assert.IsTrue(task.Execute(), "Execute Failed");
            Assert.IsTrue(File.Exists(task.ZipFileName), "Zip file not found");
        }

        [Test(Description = "Zip files with relative path")]
        public void ZipRelativeExecute()
        {
            var task = new Zip();
            task.BuildEngine = new MockBuild();

            string testDir = TaskUtility.TestDirectory;
            string prjRootPath = TaskUtility.GetProjectRootDirectory(true);

            string workingDir = Path.Combine(prjRootPath, @"Source\MSBuild.Community.Tasks.Tests");
            List<string> files = Directory.GetFiles(workingDir, "*.*", SearchOption.TopDirectoryOnly).ToList();
            files.Add(Path.Combine(workingDir, "..\\..\\readme.md"));

            TaskItem[] items = TaskUtility.StringArrayToItemArray(files.ToArray());

            task.Files = items;
            task.WorkingDirectory = workingDir;
            task.ZipFileName = Path.Combine(testDir, ZIP_WITH_RELATIVE_FILE_NAME);

            if (File.Exists(task.ZipFileName))
                File.Delete(task.ZipFileName);
            
            Assert.IsTrue(task.Execute(), "Execute Failed");                        
            Assert.IsTrue(ZipFile.Read(task.ZipFileName).ContainsEntry("readme.md"), "The zip doesnt contains the readme.md file");            
            Assert.IsTrue(File.Exists(task.ZipFileName), "Zip file not found");            
        }
    }
}