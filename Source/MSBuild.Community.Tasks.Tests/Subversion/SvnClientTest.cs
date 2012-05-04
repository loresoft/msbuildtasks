

using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using MSBuild.Community.Tasks.Subversion;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests.Subversion
{
    [TestFixture]
    public class SvnClientTest
    {
        [Test]
        public void SvnClientExecute()
        {
            string targetFile = "myfile.txt";
            SvnClient client = new SvnClient();
            client.Command = "revert";
            client.Targets = new ITaskItem[] {new TaskItem(targetFile)};

            string expectedCommand = String.Format("revert \"{0}\"", targetFile);
            string actualCommand = TaskUtility.GetToolTaskCommand(client);
            Assert.AreEqual(expectedCommand, actualCommand);

        }

        [Test]
        public void SvnClientExecuteWithArguments()
        {
            SvnClient client = new SvnClient();
            client.Command = "diff";
            client.Arguments = "--non-interactive --no-auth-cache";

            string expectedCommand = "diff --non-interactive --no-auth-cache";
            string actualCommand = TaskUtility.GetToolTaskCommand(client);
            Assert.AreEqual(expectedCommand, actualCommand);

        }

        [Test]
        public void ParsesStandardUpdateRevisionOutput()
        {
            SvnClientStub task = new SvnClientStub();
            task.BuildEngine = new MockBuild();

            int actualRevision = task.GetRevisionFromLogEventsFromTextOutput("At revision 222", MessageImportance.Low);

            const int expectedRevision = 222;

            Assert.AreEqual(expectedRevision, actualRevision);
        }

        [Test]
        public void IgnoresSkippedPathsUpdateOutput()
        {
            SvnClientStub task = new SvnClientStub();
            task.BuildEngine = new MockBuild();

            //  "Skipped paths..." is a potential output of svn.exe update and follows the actual revision output
            const string updateMsg = "Skipped paths: 1";

            int actualRevision = task.GetRevisionFromLogEventsFromTextOutput(updateMsg, MessageImportance.Low);

            const int expectedRevision = -1;

            Assert.AreEqual(expectedRevision, actualRevision);
        }

        private class SvnClientStub : SvnClient
        {
            private int _revisionResult;

            public int GetRevisionFromLogEventsFromTextOutput(string singleLine, MessageImportance messageImportance)
            {
                LogEventsFromTextOutput(singleLine, messageImportance);

                return _revisionResult;
            }

            protected override void LogEventsFromTextOutput(string singleLine, MessageImportance messageImportance)
            {
                base.LogEventsFromTextOutput(singleLine, messageImportance);

                _revisionResult = Revision;
            }
        }
    }

}