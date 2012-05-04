

using System;
using System.IO;
using System.Net;
using System.Text;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests
{
    [TestFixture()]
    public class WebDownloadTest
    {
        private string testDirectory;
        private WebDownload task;


        [SetUp()]
        public void Setup()
        {
            MockBuild buildEngine = new MockBuild();
            testDirectory = TaskUtility.makeTestDirectory(buildEngine);
            task = new WebDownload();
            task.BuildEngine = new MockBuild();
        }

        [Test(Description = "Download www.microsoft.com/default.aspx")]
        public void WebDownloadExecute()
        {
            task.FileUri = "http://www.microsoft.com/default.aspx";
            string downloadFile = Path.Combine(testDirectory, "microsoft.html");
            task.FileName = downloadFile;

            Assert.IsTrue(task.Execute(), "Execute Failed");
            Assert.IsTrue(File.Exists(downloadFile), "No download file");
        }

        [Test]
        public void No_credentials_should_be_sent_by_default()
        {
            Assert.IsNull(task.GetConfiguredCredentials());
        }

        [Test]
        public void Default_credentials_should_be_sent_when_UseDefaultCredentials_is_true()
        {
            task.UseDefaultCredentials = true;
            ICredentials credentials = task.GetConfiguredCredentials();
            Assert.IsNotNull(credentials);
            Assert.AreEqual(CredentialCache.DefaultCredentials, credentials);
        }

        [Test]
        public void Default_credentials_should_be_overridden_when_username_supplied_regardless_of_UseDefaultCredentials()
        {
            task.UseDefaultCredentials = true;
            task.Username = "joeuser";
            ICredentials credentials = task.GetConfiguredCredentials();
            Assert.AreNotEqual(CredentialCache.DefaultCredentials, credentials);
        }

        [Test]
        public void Specific_credentials_should_be_sent_when_username_and_password_supplied()
        {
            task.UseDefaultCredentials = true;
            string username = "joeuser";
            task.Username = username;
            string password = "password";
            task.Password = password;
            
            ICredentials credentials = task.GetConfiguredCredentials();
            ICredentials expectedCredentials = new NetworkCredential(username, password, String.Empty);
            assertAreEffectivelyEqual(expectedCredentials, credentials);
        }

        [Test]
        public void Empty_password_should_be_sent_when_username_supplied_alone()
        {
            task.UseDefaultCredentials = true;
            string username = "joeuser";
            task.Username = username;
            ICredentials credentials = task.GetConfiguredCredentials();
            ICredentials expectedCredentials = new NetworkCredential(username, String.Empty, String.Empty);
            assertAreEffectivelyEqual(expectedCredentials, credentials);
        }

        [Test]
        public void Domain_should_be_sent_when_username_and_domain_supplied()
        {
            task.UseDefaultCredentials = true;
            string username = "joeuser";
            task.Username = username;
            string domain = "UNIVERSE";
            task.Domain = domain;
            ICredentials credentials = task.GetConfiguredCredentials();
            ICredentials expectedCredentials = new NetworkCredential(username, String.Empty, domain);
            assertAreEffectivelyEqual(expectedCredentials, credentials);
        }

        private static void assertAreEffectivelyEqual(ICredentials expectedCredentials, ICredentials actualCredentials)
        {
            Uri uri = new Uri("http://localhost/");
            NetworkCredential expectedCredential = expectedCredentials.GetCredential(uri, "BASIC");
            NetworkCredential actualCredential = actualCredentials.GetCredential(uri, "BASIC");

            Assert.AreEqual(expectedCredential.UserName, actualCredential.UserName);
            Assert.AreEqual(expectedCredential.Password, actualCredential.Password);
            Assert.AreEqual(expectedCredential.Domain, actualCredential.Domain);
        }
    }
}
