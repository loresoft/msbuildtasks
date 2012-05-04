

using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests
{
    /// <summary>
    /// Summary description for RegistryReadTest
    /// </summary>
    [TestFixture]
    public class RegistryReadTest
    {
        [Test(Description="Read a value from the registry")]
        public void RegistryReadExecute()
        {
            RegistryRead task = new RegistryRead();
            task.BuildEngine = new MockBuild();
            task.KeyName = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\.NETFramework";
            task.ValueName = "InstallRoot";
            Assert.IsTrue(task.Execute(), "Execute should have succeeded");

            string expected = Environment.GetEnvironmentVariable("SystemRoot") + @"\Microsoft.NET\Framework\";
            StringAssert.AreEqualIgnoringCase(expected, task.Value);
        }

        [Test(Description = "Read a value from the registry that doesn't exist")]
        public void RegistryReadValueNotFoundFailure()
        {
            RegistryRead task = new RegistryRead();
            task.BuildEngine = new MockBuild();
            task.KeyName = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\.NETFramework";
            task.ValueName = "SomethingElse";
            task.DefaultValue = "NotFound";
            Assert.IsTrue(task.Execute(), "Execute should have succeeded");

            Assert.AreEqual("NotFound", task.Value);
        }

        [Test(Description = "Read a value from the registry that doesn't exist")]
        public void RegistryReadValueNotFoundNoDefaultFailure()
        {
            RegistryRead task = new RegistryRead();
            task.BuildEngine = new MockBuild();
            task.KeyName = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\.NETFramework";
            task.ValueName = "SomethingElse";
            Assert.IsTrue(task.Execute(), "Execute should have succeeded");

            Assert.AreEqual(String.Empty, task.Value);
        }


        [Test(Description = "Read a value from a registry key that doesn't exist")]
        public void RegistryReadKeyNotFoundFailure()
        {
            RegistryRead task = new RegistryRead();
            task.BuildEngine = new MockBuild();
            task.KeyName = @"HKEY_LOCAL_MACHINE\SOFTWARE\BLAH\BLAH";
            task.ValueName = "InstallRoot";
            task.DefaultValue = "NotFound";
            Assert.IsTrue(task.Execute(), "Execute should have succeeded");

            Assert.AreEqual("NotFound", task.Value);
        }

        [Test(Description = "Read a value from a registry key that doesn't exist, no default value.")]
        public void RegistryReadKeyNotFoundNoDefaultFailure()
        {
            RegistryRead task = new RegistryRead();
            task.BuildEngine = new MockBuild();
            task.KeyName = @"HKEY_LOCAL_MACHINE\SOFTWARE\BLAH\BLAH";
            task.ValueName = "InstallRoot";
            Assert.IsFalse(task.Execute(), "Execute should have failed");

            Assert.AreEqual(null, task.Value);
        }

    }
}
