using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using MSBuild.Community.Tasks.Fusion;
using System.Reflection;

namespace MSBuild.Community.Tasks.Tests.Fusion
{
    [TestFixture()]
    public class FusionWrapperTest
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
        public void InstallAssembly()
        {
            string path = @"..\..\..\MSBuild.Community.Tasks\bin\Debug\MSBuild.Community.Tasks.dll";
            FusionWrapper.InstallAssembly(path, true);
        }

        [Test()]
        public void UninstallAssembly()
        {
            string path = @"..\..\..\MSBuild.Community.Tasks\bin\Debug\MSBuild.Community.Tasks.dll";
            FusionWrapper.InstallAssembly(path, true);
            
            UninstallStatus result;
            bool successful = FusionWrapper.UninstallAssembly("MSBuild.Community.Tasks, Version=1.4.0.0, Culture=neutral, PublicKeyToken=e8bf2261941c3948", true, out result);

            Assert.IsTrue(successful);            
        }

        [Test()]
        public void UninstallAssemblyShort()
        {
            string path = @"..\..\..\MSBuild.Community.Tasks\bin\Debug\MSBuild.Community.Tasks.dll";
            FusionWrapper.InstallAssembly(path, true);

            UninstallStatus result;
            bool successful = FusionWrapper.UninstallAssembly("MSBuild.Community.Tasks", true, out result);

            Assert.IsTrue(successful);
        }


        [Test()]
        public void QueryAssemblyPath()
        {
            string path = FusionWrapper.GetAssemblyPath("System.Core, Version=3.5.0.0, Culture=neutral, PublicKeyToken=B77A5C561934E089");

            Assert.IsNotNull(path);
        }

        [Test()]
        public void GetDisplayName()
        {
            AssemblyName name = FusionWrapper.GetAssemblyName("System.Core");

            Assert.IsNotNull(name);
        }

    }
}
