// $Id$

using System;
using System.IO;
using System.Text;
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

        [Test(Description="Zip files into a zip archive")]
        public void ZipExecute()
        {
            Zip task = new Zip();
            task.BuildEngine = new MockBuild();

            string testDir = TaskUtility.TestDirectory;
            string prjRootPath = TaskUtility.getProjectRootDirectory(true);

            string workingDir = Path.Combine(prjRootPath, "Source/MSBuild.Community.Tasks.Tests");
            string[] files = Directory.GetFiles(workingDir, "*.*", SearchOption.TopDirectoryOnly);

            TaskItem[] items = TaskUtility.StringArrayToItemArray(files);

            task.Files = items;
            task.WorkingDirectory = workingDir;
            task.ZipFileName = Path.Combine(testDir, ZIP_FILE_NAME);

            if (File.Exists(task.ZipFileName)) File.Delete(task.ZipFileName);

            Assert.IsTrue(task.Execute(), "Execute Failed");
            Assert.IsTrue(File.Exists(task.ZipFileName), "Zip file not found");
        }

        [Test(Description = "Zip files and empty folders into a zip archive")]
        public void ZipExecuteWithEmptyFolders()
        {
            Zip task = new Zip();
            task.BuildEngine = new MockBuild();

            string testDir = TaskUtility.TestDirectory;
            string prjRootPath = TaskUtility.getProjectRootDirectory(true);

            string workingDir = Path.Combine(prjRootPath, "Source/MSBuild.Community.Tasks.Tests");
            string[] files = Directory.GetFiles(workingDir, "*.*", SearchOption.TopDirectoryOnly);
            string[] directories = Directory.GetDirectories(workingDir, "*.*", SearchOption.TopDirectoryOnly);
            string[] filesFromXmlDirectory = Directory.GetFiles(workingDir + "\\" + "Xml", "*.*",
                                                                      SearchOption.TopDirectoryOnly);

            string[] filesAndDirectories = new string[files.Length + directories.Length + filesFromXmlDirectory.Length];

            

            files.CopyTo(filesAndDirectories, 0);            
            directories.CopyTo(filesAndDirectories, files.Length + filesFromXmlDirectory.Length);
            filesFromXmlDirectory.CopyTo(filesAndDirectories, files.Length);

            TaskItem[] items = TaskUtility.StringArrayToItemArray(filesAndDirectories);

            task.Files = items;
            task.WorkingDirectory = workingDir;
            task.ZipFileName = Path.Combine(testDir, ZIP_WITH_FOLDERS_FILE_NAME);

            if (File.Exists(task.ZipFileName)) File.Delete(task.ZipFileName);

            Assert.IsTrue(task.Execute(), "Execute Failed");
            Assert.IsTrue(File.Exists(task.ZipFileName), "Zip file not found");
        }
    }
}
