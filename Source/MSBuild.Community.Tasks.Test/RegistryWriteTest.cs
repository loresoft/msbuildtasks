// $Id$

using System;
using System.Text;
using System.Collections.Generic;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests
{
    /// <summary>
    /// Summary description for RegistryWriteTest
    /// </summary>
    [TestFixture]
    public class RegistryWriteTest
    {
        public RegistryWriteTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        [Test]
        public void RegistryWriteExecute()
        {
            RegistryWrite task = new RegistryWrite();
            task.BuildEngine = new MockBuild();
            task.KeyName = @"HKEY_CURRENT_USER\SOFTWARE\MSBuildTasks";
            task.ValueName = "RegistryWrite";
            task.Value = "Test Write";
            Assert.IsTrue(task.Execute(), "Execute Failed");
            
        }
    }
}
