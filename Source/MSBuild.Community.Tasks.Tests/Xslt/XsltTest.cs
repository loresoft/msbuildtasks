//-----------------------------------------------------------------------
// <copyright file="XsltTest.cs" company="MSBuild Community Tasks Project">
//     Copyright © 2006 Ignaz Kohlbecker
// </copyright>
//-----------------------------------------------------------------------


namespace MSBuild.Community.Tasks.Tests
{
	using System.IO;
	using global::NUnit.Framework;
	using Microsoft.Build.Utilities;

	/// <summary>
	/// NUnit tests for the MSBuild <see cref="Microsoft.Build.Framework.Task"/> 
	/// <see cref="Xslt"/>.
	/// </summary>
	[TestFixture]
	public class XsltTest
	{
		/// <summary>
		/// Executes the Xslt task without input.
		/// </summary>
		[Test(Description = "Executes the Xslt task without input")]
		public void XsltNoInput()
		{
			Xslt task = new Xslt();
			task.BuildEngine = new MockBuild();

			// assert default values
			Assert.IsNull(task.Inputs, @"Wrong default inputs");
			Assert.IsNull(task.RootTag, @"Wrong default root tag");
			Assert.IsNull(task.RootAttributes, @"Wrong default root attributes");
			Assert.IsNull(task.Xsl, @"Wrong default xsl");
			Assert.IsNull(task.Output, @"Wrong default output");

			// executing without input files must fail
			Assert.IsFalse(task.Execute(), @"Task without input files did not fail");
		}

		/// <summary>
		/// Executes the Xslt task with one input file.
		/// </summary>
		[Test(Description = "Executes the Xslt task with one input file")]
		public void XsltOneInput()
		{
			Xslt task = new Xslt();
			task.BuildEngine = new MockBuild();

			string testDir;
			string assemblyDir = TaskUtility.AssemblyDirectory;
			TaskUtility.logEnvironmentInfo(task.Log);
			if (TaskUtility.CalledInBuildDirectory)
			{
				task.Log.LogMessage("Called in build directory");

				// get the files from where msbuild put them
				testDir = TaskUtility.TestDirectory;
			}
			else
			{
				task.Log.LogMessage("Not called in build directory");

				// get the files from where Visual Studio put them
				testDir = Path.Combine(TaskUtility.AssemblyDirectory, @"Xslt");
			}

			task.Inputs = TaskUtility.StringArrayToItemArray(Path.Combine(testDir, @"XsltTestInput.xml"));
			task.Inputs[0].SetMetadata(@"configuration", "test");
			task.Xsl = new TaskItem(Path.Combine(assemblyDir, @"NUnitReport.xsl"));
			task.Xsl.SetMetadata(@"project", "XsltTest");

			task.RootTag = "mergedroot";
			task.Output = Path.Combine(testDir, @"XsltTestOutput.html");

			Assert.IsNotNull(task.Inputs, @"No inputs");
			Assert.IsNull(task.RootAttributes, @"Wrong default root attributes");
			Assert.IsNotNull(task.Xsl, @"No xsl");
			Assert.IsNotNull(task.Output, @"No output");

			// executing with one input files
			Assert.IsTrue(task.Execute(), @"Task with one input files failed");
			Assert.IsTrue(File.Exists(task.Output), @"Missing output file");

			string taskOutput = File.ReadAllText(task.Output);
			Assert.IsTrue(taskOutput.Contains(
				@"<h1>Project ""XsltTest"" - Configuration ""test"" - NUnit Test Report</h1>"),
				@"Input or Xsl metadata not correctly added.");
		}
	}
}