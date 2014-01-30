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
            var pdfFile = new TaskItem(@"C:\TeamCity\BuildAgent1\work\81dfcc86a237ffdc\build-output\Market\MTSubsystem.Market.pdb");

            task.SymbolFiles = new ITaskItem[] { pdfFile };
            task.TeamProjectRootDirectory = @"C:\TeamCity\BuildAgent1\work\81dfcc86a237ffdc";
            task.TeamProjectName = "Genesis";
            task.ChangesetVersion = "66803";
            task.TeamProjectCollectionUri = "https://tfs.cityindex.co.uk/tfs/defaultcollection";

            task.WorkspaceDirectory = @"C:\TeamCity\BuildAgent1\work\81dfcc86a237ffdc\DevBranches\TeamCity";
            task.SourceServerSdkPath = @"C:\program Files (x86)\Windows Kits\8.1\Debuggers\x64\srcsrv";

            var result = task.Execute();

            Assert.IsTrue(result);
        }        
    }
}