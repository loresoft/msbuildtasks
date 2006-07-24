using System;
using NUnit.Framework;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace MSBuild.Community.Tasks.Tests
{
    [TestFixture]
    public class ScriptTest
    {
        [Test]
        public void Execute()
        {
            Script task = new Script();
            task.BuildEngine = new MockBuild();
            task.Code = @"public static void ScriptMain(){
int x = 1;
System.Diagnostics.Debug.WriteLine(x);
}";
            Assert.IsTrue(task.Execute(), "Task should have succeeded.");
        }

        [Test]
        public void ExecuteWithImports()
        {
            Script task = new Script();
            task.BuildEngine = new MockBuild();
            task.Code = @"public static void ScriptMain(){
int x = 1;
Debug.WriteLine(x);
}";
            task.Imports = new ITaskItem[] { new TaskItem("System.Diagnostics") };
            Assert.IsTrue(task.Execute(), "Task should have succeeded.");
        }

        [Test, Ignore(@"Cannot get strong name assemblies to load within test runner, but they work from an actual MSBuild script (see Source\Script.proj).")]
        public void ExecuteWithStrongNameReference()
        {
            Script task = new Script();
            task.BuildEngine = new MockBuild();
            task.Code = @"public static void ScriptMain(){
string x = System.Web.HttpUtility.HtmlEncode(""<tag>"");
System.Diagnostics.Debug.WriteLine(x);
}";
            task.References = new ITaskItem[] { new TaskItem("System.Web") };
            Assert.IsTrue(task.Execute(), "Task should have succeeded.");
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowActualExceptionInsteadOfTargetInvocationException()
        {
            Script task = new Script();
            task.BuildEngine = new MockBuild();
            task.Code = @"public static void ScriptMain(){
throw new InvalidOperationException(""This is the actual exception."");
}";
            Assert.IsTrue(task.Execute(), "Task should have succeeded.");
        }
    }
}
