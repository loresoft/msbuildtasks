using System;
using System.Text;
using NUnit.Framework;



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

        [Test(Description="Query serice 'w3svc'")]
        public void ServiceQueryW3SVC()
        {
            ServiceQuery task = new ServiceQuery();
            task.BuildEngine = new MockBuild();
            task.ServiceName = "w3svc";

            Assert.IsTrue(task.Execute(), "Execute Failed");
            Assert.IsNotNull(task.Status, "Status Null");
        }

        [Test(Description = "Query serice 'HTTPFilter'")]
        public void ServiceQueryHTTPFilter()
        {
            ServiceQuery task = new ServiceQuery();
            task.BuildEngine = new MockBuild();
            task.ServiceName = "HTTPFilter";

            Assert.IsTrue(task.Execute(), "Execute Failed");
            Assert.IsNotNull(task.Status, "Status Null");
        }
    }
}
