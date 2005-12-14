using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests
{
    [TestFixture()]
    public class ServiceControllerTest
    {
        [SetUp()]
        public void Setup()
        {
            //TODO: NUnit setup
        }

        [TearDown()]
        public void TearDown()
        {
            //TODO: NUnit TearDown
        }

        [Test()]
        public void ServiceControllerExecute()
        {
            ServiceController task = new ServiceController();
            task.BuildEngine = new MockBuild();
            task.ServiceName = "w3svc";
            task.Action = "Restart";

            Assert.IsTrue(task.Execute(), "Execute Failed");
            Assert.IsNotNull(task.Status, "Status Null");
        }
    }
}
