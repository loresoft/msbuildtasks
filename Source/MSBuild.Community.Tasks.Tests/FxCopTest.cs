

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Build.Utilities;
using Microsoft.Win32;
using NUnit.Framework;
using System.Reflection;

namespace MSBuild.Community.Tasks.Tests
{
    /// <summary>
    /// Summary description for NUnitTest
    /// </summary>
    [TestFixture]
    public class FxCopTest
    {
        [Test(Description = "Test explicit tool path.")]
        public void SetExplicitToolPath()
        {
            FxCop task = new FxCop();
            task.ToolPath = @"c:\fxcop";
            Assert.AreEqual(@"c:\fxcop\fxcopcmd.exe", (string)typeof(FxCop).InvokeMember("GenerateFullPathToTool", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod, null, task, null));
        }

        [Test(Description = "Test invalid parameters (both project file and target assemblies are missing).")]
        public void MissingTargetsAndProject()
        {
            FxCop task = new FxCop();
            Assert.IsFalse((bool)typeof(FxCop).InvokeMember("ValidateParameters", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod, null, task, null));
        }

        [Test(Description = "Test invalid parameters (target assemblies set but rules are missing).")]
        public void MissingRulesWithTargets()
        {
            FxCop task = new FxCop();
            task.TargetAssemblies = new TaskItem[] { new TaskItem("target.dll") };
            Assert.IsFalse((bool)typeof(FxCop).InvokeMember("ValidateParameters", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod, null, task, null));
        }

        [Test(Description = "Test all parameters")]
        public void GenerateAllParameters()
        {

            // These do not need to make sense really since we are not invoking
            // the executable.
            FxCop task = new FxCop();
            task.ToolPath = @"c:\fxcop";
            task.AnalysisReportFileName = @"report.xml";
            task.DirectOutputToConsole = true;
            task.ConsoleXslFileName = @"console-transform.xsl";
            task.DependencyDirectories = new TaskItem[] { new TaskItem("depDir1"), new TaskItem("depDir2") };
            task.ApplyOutXsl = true;
            task.OutputXslFileName = @"transform.xsl";
            task.ProjectFile = @"c:\project.fxcop";
            task.PlatformDirectory = @"c:\platform";
            task.TargetAssemblies = new TaskItem[] { new TaskItem("ABC.dll"), new TaskItem("DEF.dll") };
            task.ImportFiles = new TaskItem[] { new TaskItem("import.xml") };
            task.RuleLibraries = new TaskItem[] { new TaskItem("a-rules.dll"), new TaskItem("b-rules.dll") };
            task.Rules = new TaskItem[] { new TaskItem("Microsoft.Design#CA1012"), new TaskItem("-Microsoft.Design#CA2210") };
            task.IncludeSummaryReport = true;
            task.TypeList = "TypeA,TypeB";
            task.Verbose = true;
            task.SaveResults = true;
            task.IgnoreGeneratedCode = true;

            Assert.AreEqual("/aXsl /c /cXsl:\"console-transform.xsl\" /d:\"depDir1\" /d:\"depDir2\" " +
                                             "/f:\"ABC.dll\" /f:\"DEF.dll\" /i:\"import.xml\" /o:\"report.xml\" " +
                                             "/oXsl:\"transform.xsl\" /plat:\"c:\\platform\" /p:\"c:\\project.fxcop\" " +
                                             "/r:\"c:\\fxcop\\Rules\\a-rules.dll\" /r:\"c:\\fxcop\\Rules\\b-rules.dll\" " +
                                             "/rid:Microsoft.Design#CA1012 /rid:-Microsoft.Design#CA2210 " +
                                             "/s /t:TypeA,TypeB " +
                                             "/u /v /igc ",
                                             (string)typeof(FxCop).InvokeMember("GenerateCommandLineCommands", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod, null, task, null));
        }
    }
}
