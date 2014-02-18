using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using MSBuild.Community.Tasks.SourceServer;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests.SourceServer
{
    /// <summary>
    /// NOTE: This test requires that the pdb under test is compiled from sources that are in a TFS workspace.
    /// </summary>
    [TestFixture]
    public class TfsSourceIndexTest
    {
        [Test]
        public void IndexSource()
        {
            var task = new TfsSourceIndex();
            task.BuildEngine = new MockBuild();

            //var pdfFile = new TaskItem(Path.GetFullPath("MSBuild.Community.Tasks.pdb"));
            //var pdfFile = new TaskItem(@"C:\Dev\Build\DTO\Utility\bin\Release\CityIndex.Genesis.DTO.Utility.pdb");
            var pdfFile = new TaskItem(@"C:\Dev\Build\DTO\MarketInformation\bin\Debug\CityIndex.Genesis.DTO.MarketInformation.pdb");

            task.SymbolFiles = new ITaskItem[] { pdfFile };
            task.TeamProjectRootDirectory = @"C:\Dev\Build";
            task.TeamProjectName = "Genesis/DevBranches/Build";
            task.ChangesetVersion = "67550";
            task.TeamProjectCollectionUri = "https://tfs.cityindex.co.uk/tfs/defaultcollection";

            task.WorkspaceDirectory = @"C:\Dev\Build";
            task.SourceServerSdkPath = @"C:\program Files (x86)\Windows Kits\8.1\Debuggers\x64\srcsrv";

            var result = task.Execute();

            Assert.IsTrue(result);
        }        
    }
}