using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using MSBuild.Community.Tasks.Subversion;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests.Subversion
{
    [TestFixture]
    public class SvnInfoTests
    {
        /// <summary>
        /// This test is coupled with the repository. It may require network connectivity
        /// in order to pass (haven't yet tested). Also, if this site is ever rehosted
        /// such that the URL changes then this test will need to be updated. The upside here
        /// is that we actually run "svn info" and test some if it's results.
        /// </summary>
        [Test]
        public void TestInfoReturnValues()
        {
            SvnInfo info = new SvnInfo();
            info.LocalPath = Path.Combine(TaskUtility.getProjectRootDirectory(true), "Source");
            info.BuildEngine = new MockBuild();
            Assert.IsTrue(info.Execute());

            string val = info.RepositoryPath;

            // "http://msbuildtasks.tigris.org/svn/msbuildtasks/trunk"
            // could also be svn://
            Assert.AreEqual(0, val.IndexOf("http://"));
            Assert.AreEqual(NodeKind.dir.ToString(), info.NodeKind);
            Assert.AreEqual("http://msbuildtasks.tigris.org/svn/msbuildtasks", info.RepositoryRoot);
            Assert.AreNotEqual(Guid.Empty, info.RepositoryUuid);
        }

        [Test]
        public void TestInfoCommand()
        {
            SvnInfo info = new SvnInfo();
            string localPath = Path.Combine(TaskUtility.getProjectRootDirectory(true), "Source");
            info.LocalPath = localPath;

            string expectedCommand = String.Format("info \"{0}\" --xml --non-interactive --no-auth-cache", localPath);
            string actualCommand = TaskUtility.GetToolTaskCommand(info);
            Assert.AreEqual(expectedCommand, actualCommand);
        }

        [Test]
        public void XmlSerializerTest()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Info));
            
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(@"<?xml version=""1.0""?>");
            builder.AppendLine(@"<info>");
            builder.AppendLine(@"  <entry kind=""file"" path=""D:\Projects\Tigris\MSBuildTasks\Source\MSBuild.Community.Tasks\Subversion\SvnVersion.cs"" revision=""478"">");
            builder.AppendLine(@"    <url>http://msbuildtasks.tigris.org/svn/msbuildtasks/trunk/Source/MSBuild.Community.Tasks/Subversion/SvnVersion.cs</url>");
            builder.AppendLine(@"    <repository>");
            builder.AppendLine(@"      <root>http://msbuildtasks.tigris.org/svn/msbuildtasks</root>");
            builder.AppendLine(@"      <uuid>299a232d-b705-0410-b782-f21f2f1e606a</uuid>");
            builder.AppendLine(@"    </repository>");
            builder.AppendLine(@"    <wc-info>");
            builder.AppendLine(@"      <schedule>normal</schedule>");
            builder.AppendLine(@"      <depth>infinity</depth>");
            builder.AppendLine(@"      <text-updated>2008-05-21T23:12:25.728550Z</text-updated>");
            builder.AppendLine(@"      <checksum>9cd43bab7d313779ef84920f46f593ec</checksum>");
            builder.AppendLine(@"    </wc-info>");
            builder.AppendLine(@"    <commit revision=""386"">");
            builder.AppendLine(@"      <author>pwelter</author>");
            builder.AppendLine(@"      <date>2008-05-21T23:12:25.728550Z</date>");
            builder.AppendLine(@"    </commit>");
            builder.AppendLine(@"  </entry>");
            builder.AppendLine(@"</info>");

            StringReader sr = new StringReader(builder.ToString());
            XmlReader reader = XmlReader.Create(sr);
            
            var info = serializer.Deserialize(reader) as Info;

            Assert.IsNotNull(info);
        }
    }
}