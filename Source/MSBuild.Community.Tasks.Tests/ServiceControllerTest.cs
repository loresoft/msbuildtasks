

using System;
using System.Text;
using System.ServiceProcess;
using NUnit.Framework;
using System.Security.Principal;

namespace MSBuild.Community.Tasks.Tests
{
    [TestFixture()]
    public class ServiceControllerTest
    {
        [SetUp()]
        public void Setup()
        {
            //TODO: NUnit setup
            IPrincipal currentUser = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            if (!currentUser.IsInRole(WindowsBuiltInRole.Administrator.ToString()))
            {
                Assert.Ignore("{0} is not an administrator and so cannot run the ServiceController tests.", currentUser.Identity.Name);
            }
        }

        [TearDown()]
        public void TearDown()
        {
            //TODO: NUnit TearDown
        }

        [Test(Description = "Start or restart service 'w3svc'")]
        public void ServiceControllerW3SVC()
        {
            ServiceControllerExecute(@"w3svc");
        }

        [Test(Description = "Start or restart service 'HTTPFilter'")]
        public void ServiceControllerHTTPFilter()
        {
            ServiceControllerExecute(@"HTTPFilter");
        }

        private void ServiceControllerExecute(string serviceName)
        {
            ServiceQuery queryTask = new ServiceQuery();
            queryTask.BuildEngine = new MockBuild();
            queryTask.ServiceName = serviceName;
            Assert.IsTrue(queryTask.Execute(), "Execute query failed");

            ServiceController task = new ServiceController();
            task.BuildEngine = new MockBuild();
            task.ServiceName = serviceName;

            if (ServiceQuery.UNKNOWN_STATUS.Equals(queryTask.Status)) 
            {
                Assert.Ignore("Couldn't find the '{0}' service on '{1}'",
                        queryTask.ServiceName, queryTask.MachineName);
            }
            else if (ServiceControllerStatus.Stopped.ToString().Equals(queryTask.Status))
            {
                task.Action = "Start";
            }
            else if (ServiceControllerStatus.Running.ToString().Equals(queryTask.Status))
            {
                task.Action = "Restart";
            }
            else
            {
                Assert.Ignore("'{0}' service on '{1}' is '{2}'",
                    queryTask.ServiceName, queryTask.MachineName, queryTask.Status);
            }

            Assert.IsTrue(task.Execute(), "Execute Failed");
            Assert.IsNotNull(task.Status, "Status Null");
        }
    }
}
