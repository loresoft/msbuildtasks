using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MSBuild.Community.Tasks.SymbolServer;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests.SymbolServer
{
    [TestFixture]
    public class SymStoreTest
    {
        [SetUp]
        public void Setup()
        {
            //TODO: NUnit setup
        }

        [TearDown]
        public void TearDown()
        {
            //TODO: NUnit TearDown
        }

        [Test]
        public void Store()
        {
            //symstore add /r /f \\largeapp\appserver\bins\*.* /s \\testdir\symsrv /t "Large Application" /v "Build 432" /c "Sample add"

            SymStore task = new SymStore();
            task.BuildEngine = new MockBuild();
            task.ToolPath = @"C:\Program Files\Debugging Tools for Windows (x64)";
            task.Recursive = true;
            task.Files = @"..\..\..\..\Build\*.*";
            task.Store = @"\\server\Backup\Symbol";
            task.Product = ThisAssembly.AssemblyProduct;
            task.Version = "v" + ThisAssembly.AssemblyFileVersion;
            task.Comment = "Build v" + ThisAssembly.AssemblyFileVersion;

            bool result = task.Execute();
            Assert.IsTrue(result);

        }
    }
}
