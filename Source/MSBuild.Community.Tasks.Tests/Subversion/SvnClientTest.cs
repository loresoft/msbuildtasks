// $Id$

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
    }
}