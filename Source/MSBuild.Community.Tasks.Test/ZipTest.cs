// $Id$

using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Microsoft.Build.Utilities;

namespace MSBuild.Community.Tasks.Tests
{
    /// <summary>
    /// Summary description for ZipTest
    /// </summary>
    [TestFixture]
    public class ZipTest
    {
        public ZipTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        [Test]
        public void ZipExecute()
        {
            Zip task = new Zip();
            task.BuildEngine = new MockBuild();

            string[] files = Directory.GetFiles(@"..\..\", "*.*", SearchOption.TopDirectoryOnly);

            TaskItem[] items = TaskUtility.StringArrayToItemArray(files);

            task.Files = items;
            task.WorkingDirectory = @"..\..\";
            task.ZipFileName = @"MSBuild.Community.Tasks.zip";

            bool result = task.Execute();

            Assert.IsTrue(result, "Execute Failed");
            Assert.IsTrue(File.Exists(@"MSBuild.Community.Tasks.zip"), "Zip file not found");
        }
    }
}
