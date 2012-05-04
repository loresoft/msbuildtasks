using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using MSBuild.Community.Tasks.SqlServer;
using Microsoft.Build.Utilities;



namespace MSBuild.Community.Tasks.Tests.SqlServer
{
    [TestFixture]
    public class SqlPubWizTest
    {
        [Test]
        public void Connection()
        {
            MockBuild build = new MockBuild();

            SqlPubWiz wiz = new SqlPubWiz();
            wiz.BuildEngine = build;
            wiz.Command = "script";
            wiz.ConnectionString = "Data Source=(local);Initial Catalog=LoreSoft;Integrated Security=True";
            wiz.Server = "(local)";
            wiz.Username = "blah";
            wiz.Output = new TaskItem("test.sql");

            bool result = wiz.Execute();

            Assert.IsFalse(result);
        }
    }
}
