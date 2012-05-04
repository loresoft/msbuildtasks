using System.Collections.Generic;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using NUnit.Framework;



namespace MSBuild.Community.Tasks.Tests
{
    [TestFixture]
    public class RegexCompilerTest
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
        public void Build()
        {
            RegexCompiler task = new RegexCompiler();
            task.BuildEngine = new MockBuild();
            task.OutputDirectory = TaskUtility.TestDirectory;
            task.AssemblyName = "MSBuild.Community.RegularExpressions.dll";
            task.AssemblyTitle = "MSBuild.Community.RegularExpressions";
            task.AssemblyDescription = "MSBuild Community Tasks Regular Expressions";
            task.AssemblyCompany = "MSBuildTasks";
            task.AssemblyProduct = "MSBuildTasks";
            task.AssemblyCopyright = "Copyright (c) MSBuildTasks 2008";
            task.AssemblyVersion = "1.0.0.0";
            task.AssemblyFileVersion = "1.0.0.0";
            task.AssemblyInformationalVersion = "1.0.0.0";
            task.AssemblyKeyFile = @"..\..\..\MSBuild.Community.Tasks\MSBuild.Community.Tasks.snk";

            List<ITaskItem> expressions = new List<ITaskItem>();

            TaskItem item1 = new TaskItem("TextRegex");
            item1.SetMetadata("Pattern", @"\G[^<]+");
            item1.SetMetadata("Options", "RegexOptions.Singleline | RegexOptions.Multiline");
            item1.SetMetadata("IsPublic", "true");

            TaskItem item2 = new TaskItem("CommentRegex");
            item2.SetMetadata("Pattern", @"\G<%--(([^-]*)-)*?-%>");
            item2.SetMetadata("Options", "RegexOptions.Singleline | RegexOptions.Multiline");
            item2.SetMetadata("IsPublic", "true");

            task.RegularExpressions = new ITaskItem[] {item1, item2};

            bool result = task.Execute();
            Assert.IsTrue(result);
        }

        [Test]
        public void BuildFile()
        {
            RegexCompiler task = new RegexCompiler();
            task.BuildEngine = new MockBuild();
            task.OutputDirectory = TaskUtility.TestDirectory;
            task.AssemblyName = "MSBuild.Community.RegularExpressions.dll";
            task.AssemblyTitle = "MSBuild.Community.RegularExpressions";
            task.AssemblyDescription = "MSBuild Community Tasks Regular Expressions";
            task.AssemblyCompany = "MSBuildTasks";
            task.AssemblyProduct = "MSBuildTasks";
            task.AssemblyCopyright = "Copyright (c) MSBuildTasks 2008";
            task.AssemblyVersion = "1.0.0.0";
            task.AssemblyFileVersion = "1.0.0.0";
            task.AssemblyInformationalVersion = "1.0.0.0";
            task.AssemblyKeyFile = @"..\..\..\MSBuild.Community.Tasks\MSBuild.Community.Tasks.snk";

            task.RegularExpressionsFile = new TaskItem(@"..\..\..\RegularExpressions.xml");

            bool result = task.Execute();
            Assert.IsTrue(result);
        }
    }
}
