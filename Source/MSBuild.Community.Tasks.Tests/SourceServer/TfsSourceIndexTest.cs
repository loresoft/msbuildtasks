using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using MSBuild.Community.Tasks.SourceServer;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests.SourceServer
{
    [TestFixture]
    public class TfsSourceIndexTest
    {


        [Test]
        public void IndexSource()
        {
            var task = new TfsSourceIndex();
            task.TeamProjectCollectionUri = "http://localhost:8080/tfs/DefaultCollection";
            task.BuildEngine = new MockBuild();

            var pdfFile = new TaskItem(Path.GetFullPath("MSBuild.Community.Tasks.pdb"));

            task.SymbolFiles = new ITaskItem[] { pdfFile };
            task.SourceServerSdkPath = @"C:\program Files (x86)\Windows Kits\8.1\Debuggers\x64\srcsrv";

            var result = task.Execute();

            Assert.IsTrue(result);
        }        
    }
}