// $Id$

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
            Assert.IsTrue(task.Execute(), "Execute Failed");

            // omit the drive letter because that is likely
            // to differ on different testing machines
            string expected = ":\\WINDOWS\\Microsoft.NET\\Framework\\";
            Assert.AreEqual(expected, task.Value.Substring(1));

        }
    }
}
