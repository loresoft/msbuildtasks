using System;
using MSBuild.Community.Tasks.Subversion;
using NUnit.Framework;

/// $Id$

namespace MSBuild.Community.Tasks.Tests.Subversion
{
    [TestFixture]
    public class SvnStatusTest
    {
        [Test]
        public void SvnStatusCommand()
        {
            SvnStatus task = new SvnStatus();
            const string localPath = @"c:\code";

            task.LocalPath = localPath;

            string expectedCommand = String.Format("status \"{0}\" --xml --non-interactive --no-auth-cache", localPath);
            string actualCommand = TaskUtility.GetToolTaskCommand(task);
            Assert.AreEqual(expectedCommand, actualCommand);
        }
    }
}
