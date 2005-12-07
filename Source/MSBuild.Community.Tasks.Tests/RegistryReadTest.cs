// $Id$

using System;
using System.Text;
using System.Collections.Generic;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests
{
    /// <summary>
    /// Summary description for RegistryReadTest
    /// </summary>
    [TestFixture]
    public class RegistryReadTest
    {
        public RegistryReadTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        [Test]
        public void RegistryReadExecute()
        {
            RegistryRead task = new RegistryRead();
            task.BuildEngine = new MockBuild();
            task.KeyName = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\.NETFramework";
            task.ValueName = "InstallRoot";
            Assert.IsTrue(task.Execute(), "Execute Failed");

            string expected = "C:\\WINDOWS\\Microsoft.NET\\Framework\\";
            Assert.AreEqual(expected, task.Value);

        }
    }
}
