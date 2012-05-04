

using System;
using System.Text;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests
{
    /// <summary>
    /// Summary description for MailTest
    /// </summary>
    [TestFixture]
    public class MailTest
    {
        public MailTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        [Test, Explicit]
        public void MailExecute()
        {
            Mail task = new Mail();
            task.BuildEngine = new MockBuild();
            task.SmtpServer = "localhost";
            task.From = "from@email.com";
            task.To = new string[] { "user@email.com" };
            task.Subject = "This is a test of Mail Task";
            task.Body = "This is a test email from the Mail Task";
            Assert.IsTrue(task.Execute(), "Execute Failed");
        }
    }
}
