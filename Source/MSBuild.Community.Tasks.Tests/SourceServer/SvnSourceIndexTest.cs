using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using MSBuild.Community.Tasks.SourceServer;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests.SourceServer
{
    [TestFixture]
    public class SvnSourceIndexTest
    {
        [SetUp]
        public void Setup()
        {
            //TODO: NUnit setup
        }

        [TearDown]
        public void TearDown()
        {
            //TODO: NUnit TearDown
        }

        [Test]
        public void IndexSource()
        {
            var task = new SvnSourceIndex();
            task.BuildEngine = new MockBuild();

            var pdfFile = new TaskItem(Path.GetFullPath("MSBuild.Community.Tasks.pdb"));

            task.SymbolFiles = new ITaskItem[] { pdfFile };
            task.SourceServerSdkPath = @"C:\Program Files\Debugging Tools for Windows (x64)\srcsrv";

            var result = task.Execute();

            Assert.IsTrue(result);
        }
    }
}
