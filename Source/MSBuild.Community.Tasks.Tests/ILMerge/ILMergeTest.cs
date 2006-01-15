// $Id$
// Copyright © 2006 Ignaz Kohlbecker

using System;
using System.Collections;
using System.IO;
using System.Reflection;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests
{
    /// <summary>
    /// NUnit tests for the MSBuild <see cref="Microsoft.Build.Framework.Task"/> 
    /// <see cref="ILMerge"/>.
    /// </summary>
    [TestFixture]
    public class ILMergeTest
	{
		#region Constants

		public const int PUBLIC_A_COUNT = 2;
		public const int INTERNAL_A_COUNT = 1;
		public const int PUBLIC_B_COUNT = 3;
		public const int INTERNAL_B_COUNT = 1;

		public const int PUBLIC_COUNT = PUBLIC_A_COUNT + PUBLIC_B_COUNT;
		public const int INTERNAL_COUNT = INTERNAL_A_COUNT + INTERNAL_B_COUNT;
		public const int TYPE_COUNT = PUBLIC_COUNT + INTERNAL_COUNT;

		public const string VERSION_A = @"1.2.3.4";

		#endregion Constants

		#region Fields
		private bool ilMergeAvailable;
		private string assemblyA;
		private string assemblyB;
		private ITaskItem[] inputAssemblies;
		private string testDirectory;
		private string excludeFile;
		private string keyFile;

		#endregion Fields

		#region TestFixtureSetUp
		[TestFixtureSetUp]
		public void FixtureInit()
		{
			ilMergeAvailable = File.Exists(
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
				@"Microsoft\ILMerge\ILMerge.exe"));

			MockBuild buildEngine = new MockBuild();

			testDirectory = TaskUtility.makeTestDirectory(buildEngine);

			ILMerge task = new ILMerge();
			task.BuildEngine = buildEngine;

			if (TaskUtility.CalledInBuildDirectory)
			{
				assemblyA = Path.Combine(testDirectory, @"A.dll");
				assemblyB = Path.Combine(testDirectory, @"B.dll");
				excludeFile = Path.Combine(testDirectory, @"ExcludeTypes.txt");
				keyFile = Path.Combine(testDirectory, @"keypair.snk");
			}
			else
			{
				string config = TaskUtility.BuildConfiguration;
				string assDir = Path.GetFullPath(TaskUtility.AssemblyDirectory);
				assemblyA = Path.GetFullPath(Path.Combine(assDir, @"..\..\ILMerge\A\bin\" + config + @"\A.dll"));
				assemblyB = Path.GetFullPath(Path.Combine(assDir, @"..\..\ILMerge\B\bin\" + config + @"\B.dll"));
				excludeFile = Path.Combine(assDir, @"ILMerge\ExcludeTypes.txt");
				keyFile = Path.Combine(assDir, @"ILMerge\keypair.snk");
			}

			inputAssemblies = TaskUtility.StringArrayToItemArray(assemblyA, assemblyB);
		}

		#endregion TestFixtureSetUp

		#region Test cases

		[Test(Description = "ILMerge with duplicate type collision")]
		public void DuplicateTypeCollision()
		{
			if (!ilMergeAvailable) Assert.Ignore(@"ILMerge.exe not available");

			ILMerge task = new ILMerge();
			task.BuildEngine = new MockBuild();

			task.InputAssemblies = inputAssemblies;
			task.OutputFile = new TaskItem(Path.Combine(testDirectory, @"merged.dll"));

			Assert.IsFalse(task.Execute(), @"Task succeeded dispite of duplicate type names");

		}

		[Test(Description = "Allow duplicate type")]
		public void DuplicateTypeAllowed()
		{
			if (!ilMergeAvailable) Assert.Ignore(@"ILMerge.exe not available");

			ILMerge task = new ILMerge();
			task.BuildEngine = new MockBuild();

			task.InputAssemblies = inputAssemblies;
			task.OutputFile = new TaskItem(Path.Combine(testDirectory, @"merged1.dll"));
			task.AllowDuplicateTypes = TaskUtility.StringArrayToItemArray(@"ClassAB");
			task.XmlDocumentation = true;

			Assert.IsTrue(task.Execute(), @"Task failed");
			AssertResults(task, PUBLIC_COUNT, VERSION_A);
		}

		[Test(Description = "Internalize all byt primary assembly types")]
		public void PrimaryTypesOnly()
		{
			if (!ilMergeAvailable) Assert.Ignore(@"ILMerge.exe not available");

			ILMerge task = new ILMerge();
			task.BuildEngine = new MockBuild();

			task.InputAssemblies = inputAssemblies;
			task.OutputFile = new TaskItem(Path.Combine(testDirectory, @"merged2.dll"));
			task.AllowDuplicateTypes = TaskUtility.StringArrayToItemArray(@"ClassAB");
			task.DebugInfo = false;
			task.XmlDocumentation = false;
			task.ExcludeFile = new TaskItem();

			Assert.IsTrue(task.Execute(), @"Task failed");
			AssertResults(task, PUBLIC_A_COUNT, VERSION_A);
		}

		[Test(Description = "Internalize with exclusions")]
		public void PrimaryTypesAndExclusions()
		{
			if (!ilMergeAvailable) Assert.Ignore(@"ILMerge.exe not available");

			ILMerge task = new ILMerge();
			task.BuildEngine = new MockBuild();

			task.InputAssemblies = inputAssemblies;
			task.OutputFile = new TaskItem(Path.Combine(testDirectory, @"merged3.dll"));
			task.AllowDuplicateTypes = TaskUtility.StringArrayToItemArray(@"ClassAB");
			task.DebugInfo = false;
			task.XmlDocumentation = false;
			task.ExcludeFile = new TaskItem(excludeFile);

			Assert.IsTrue(task.Execute(), @"Task failed");
			AssertResults(task, PUBLIC_A_COUNT + 1, VERSION_A);
		}

		[Test(Description = "Signed merged assembly")]
		public void SignedMergedAssembly()
		{
			if (!ilMergeAvailable) Assert.Ignore(@"ILMerge.exe not available");

			ILMerge task = new ILMerge();
			task.BuildEngine = new MockBuild();

			task.InputAssemblies = inputAssemblies;
			task.OutputFile = new TaskItem(Path.Combine(testDirectory, @"merged4.dll"));
			task.AllowDuplicateTypes = TaskUtility.StringArrayToItemArray(@"ClassAB");
			task.DebugInfo = false;
			task.XmlDocumentation = false;
			task.KeyFile = new TaskItem(keyFile);

			Assert.IsTrue(task.Execute(), @"Task failed");
			AssertResults(task, PUBLIC_COUNT, VERSION_A);
		}

		[Test(Description = "Explicitely versioned merged assembly")]
		public void VersionedMergedAssembly()
		{
			if (!ilMergeAvailable) Assert.Ignore(@"ILMerge.exe not available");

			ILMerge task = new ILMerge();
			task.BuildEngine = new MockBuild();

			task.InputAssemblies = inputAssemblies;
			task.OutputFile = new TaskItem(Path.Combine(testDirectory, @"merged5.dll"));
			task.AllowDuplicateTypes = TaskUtility.StringArrayToItemArray(@"ClassAB");
			task.DebugInfo = false;
			task.XmlDocumentation = false;
			task.KeyFile = new TaskItem(keyFile);
			string version = @"5.6.7.8";
			task.Version = version;

			Assert.IsTrue(task.Execute(), @"Task failed");
			AssertResults(task, PUBLIC_COUNT, version);
		}

		#endregion Test cases

		#region Private Methods

		private void AssertResults(ILMerge task, int publicTypeCount, string version)
		{
			MessageImportance imp = MessageImportance.Low;

			Assert.IsTrue(File.Exists(task.OutputFile.ItemSpec), @"No merged assembly");
			string pdbFile = Path.ChangeExtension(task.OutputFile.ItemSpec, "pdb"); 
			Assert.AreEqual(task.DebugInfo, File.Exists(pdbFile), @"No debug file");
			string xmlFile = Path.ChangeExtension(task.OutputFile.ItemSpec, "doc");
			Assert.AreEqual(task.XmlDocumentation, File.Exists(xmlFile), @"No xml documentation file");

			Assembly merge = Assembly.LoadFile(task.OutputFile.ItemSpec);
			task.Log.LogMessage(imp, "Merged assembly: {0}", merge.FullName);

			Type[] types = merge.GetExportedTypes();
			foreach (Type type in types)
			{
				task.Log.LogMessage(imp, "Type: {0}", type.Name);
				task.Log.LogMessage(imp, " Attributes: {0}", type.Attributes.ToString());
			}
			Assert.AreEqual(publicTypeCount, types.Length, @"Wrong number of public types");

			AssemblyName assName = new AssemblyName(merge.FullName);
			task.Log.LogMessage(imp, "Version: {0}", assName.Version.ToString());
			Assert.AreEqual(version, assName.Version.ToString(), @"Wrong version");

			byte[] publicKeyToken = assName.GetPublicKeyToken();
			if (task.KeyFile == null)
			{
				Assert.AreEqual(0, publicKeyToken.Length, @"Merged assembly expected to be unsigned");
			}
			else
			{
				Assert.AreNotEqual(0, publicKeyToken, @"Merged assembly expected to be signed");
			}

		}

		#endregion Private Methods
	}
}