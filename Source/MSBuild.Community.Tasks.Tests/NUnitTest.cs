

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Build.Utilities;
using Microsoft.Win32;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests
{
    /// <summary>
    /// Summary description for NUnitTest
    /// </summary>
    [TestFixture]
    public class NUnitTest
    {
        [Test(Description = "Excute NUnit tests of the NUnit framework")]
        public void NUnitExecute()
        {
            #region Find NUnit installation
            string nunitPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            nunitPath = Path.Combine(nunitPath, NUnit.DEFAULT_NUNIT_DIRECTORY);
            
            RegistryKey buildKey = Registry.ClassesRoot.OpenSubKey(@"NUnitTestProject\shell\open\command");
            if (buildKey == null) Assert.Ignore(@"Can't find NUnit installation");

            nunitPath = buildKey.GetValue(null, nunitPath).ToString();
            Regex nunitRegex = new Regex("(.+)nunit-gui\\.exe", RegexOptions.IgnoreCase);
            Match pathMatch = nunitRegex.Match(nunitPath);
            nunitPath = pathMatch.Groups[1].Value.Replace("\"", "");

            #endregion Find NUnit installation

            MockBuild buildEngine = new MockBuild();

            string testDirectory = TaskUtility.makeTestDirectory(buildEngine);

            NUnit task = new NUnit();
            task.BuildEngine = buildEngine;
            task.Assemblies = TaskUtility.StringArrayToItemArray(
                Path.Combine(nunitPath, "nunit.framework.tests.dll"));
            task.WorkingDirectory = testDirectory;
            task.OutputXmlFile = Path.Combine(testDirectory, @"nunit.framework.tests-results.xml");
            Assert.IsTrue(task.Execute(), "Execute Failed");
        }
    }
}
