// $Id$

using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MSBuild.Community.Tasks.Test
{
    /// <summary>
    /// Summary description for MailTest
    /// </summary>
    [TestClass]
    public class MailTest
    {
        public MailTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
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
