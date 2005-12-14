using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

// $Id$

namespace MSBuild.Community.Tasks.Tests
{
    [TestFixture()]
    public class ServiceQueryTest
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
        public void ServiceQueryExecute()
        {
            ServiceQuery task = new ServiceQuery();
            task.BuildEngine = new MockBuild();
            task.ServiceName = "w3svc";

            Assert.IsTrue(task.Execute(), "Execute Failed");
            Assert.IsNotNull(task.Status, "Status Null");
        }
    }
}
