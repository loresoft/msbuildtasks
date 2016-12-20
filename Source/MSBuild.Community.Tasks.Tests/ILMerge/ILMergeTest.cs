//-----------------------------------------------------------------------
// <copyright file="ILMergeTest.cs" company="MSBuild Community Tasks Project">
//     Copyright © 2006 Ignaz Kohlbecker
// </copyright>
//-----------------------------------------------------------------------


namespace MSBuild.Community.Tasks.Tests
{
	using System;
	using System.IO;
	using System.Reflection;
	using global::NUnit.Framework;
	using Microsoft.Build.Framework;
	using Microsoft.Build.Utilities;

	/// <summary>
	/// NUnit tests for the MSBuild <see cref="Microsoft.Build.Framework.ITask"/> 
	/// <see cref="ILMerge"/>.
	/// </summary>
	[TestFixture]
	public class ILMergeTest
	{
		#region Constants

		public const int PublicACount = 2;
		public const int InternalACount = 1;
		public const int PublicBCount = 3;
		public const int InternalBCount = 1;

		public const int PublicCount = PublicACount + PublicBCount;
		public const int InternalCount = InternalACount + InternalBCount;
		public const int TypeCount = PublicCount + InternalCount;

		public const string VersionA = @"1.2.3.4";

		#endregion Constants

		#region Fields

	    private string assemblyA;
		private string assemblyB;
		private ITaskItem[] inputAssemblies;
		private string testDirectory;
		private string excludeFile;
		private string keyFile;

		#endregion Fields

		#region OneTimeSetUp

		/// <summary>
		/// Initializes the test fixture.
		/// </summary>
		[OneTimeSetUp]
		public void FixtureInit()
		{
			MockBuild buildEngine = new MockBuild();

			this.testDirectory = TaskUtility.makeTestDirectory(buildEngine);

			if (TaskUtility.CalledInBuildDirectory)
			{
				this.assemblyA = Path.Combine(this.testDirectory, @"A.dll");
				this.assemblyB = Path.Combine(this.testDirectory, @"B.dll");
				this.excludeFile = Path.Combine(this.testDirectory, @"ExcludeTypes.txt");
				this.keyFile = Path.Combine(this.testDirectory, @"keypair.snk");
			}
			else
			{
				string config = TaskUtility.BuildConfiguration;
				string assDir = Path.GetFullPath(TaskUtility.AssemblyDirectory);
				this.assemblyA = Path.GetFullPath(Path.Combine(assDir, @"..\..\ILMerge\A\bin\" + config + @"\A.dll"));
				this.assemblyB = Path.GetFullPath(Path.Combine(assDir, @"..\..\ILMerge\B\bin\" + config + @"\B.dll"));
				this.excludeFile = Path.Combine(assDir, @"ILMerge\ExcludeTypes.txt");
				this.keyFile = Path.Combine(assDir, @"ILMerge\keypair.snk");
			}

			this.inputAssemblies = TaskUtility.StringArrayToItemArray(this.assemblyA, this.assemblyB);
		}

		#endregion OneTimeSetUp

        ILMerge task;

	    [SetUp]
	    public void Setup()
        {
	        task = new ILMerge();
	        task.ToolPath = Path.Combine(TaskUtility.GetProjectRootDirectory(true),
                                         @"Source\packages\ilmerge.2.12.0803\");
            task.BuildEngine = new MockBuild();
            task.InputAssemblies = this.inputAssemblies;
	    }

	    #region Test cases

		/// <summary>
		/// ILMerge with duplicate type collision.
		/// </summary>
		[Test(Description = "ILMerge with duplicate type collision")]
		public void DuplicateTypeCollision()
		{
			task.OutputFile = new TaskItem(Path.Combine(this.testDirectory, @"merged.dll"));

			Assert.IsFalse(task.Execute(), @"Task succeeded dispite of duplicate type names");
		}

		/// <summary>
		/// Allow duplicate type.
		/// </summary>
		[Test(Description = "Allow duplicate type")]
		public void DuplicateTypeAllowed()
		{
			task.OutputFile = new TaskItem(Path.Combine(this.testDirectory, @"merged1.dll"));
			task.AllowDuplicateTypes = TaskUtility.StringArrayToItemArray(@"ClassAB", "ClassBA");
			task.XmlDocumentation = true;

			Assert.IsTrue(task.Execute(), @"Task failed");
			this.AssertResults(task, PublicCount, VersionA);
		}

		/// <summary>
		/// Internalize all but primary assembly types.
		/// </summary>
		[Test(Description = "Internalize all but primary assembly types")]
		public void PrimaryTypesOnly()
		{
			task.OutputFile = new TaskItem(Path.Combine(this.testDirectory, @"merged2.dll"));
			task.AllowDuplicateTypes = TaskUtility.StringArrayToItemArray(@"ClassAB");
			task.DebugInfo = false;
			task.XmlDocumentation = false;
			task.ExcludeFile = new TaskItem();

			Assert.IsTrue(task.Execute(), @"Task failed");
			this.AssertResults(task, PublicACount, VersionA);
		}

		/// <summary>
		/// Internalize with exclusions.
		/// </summary>
		[Test(Description = "Internalize with exclusions")]
		public void PrimaryTypesAndExclusions()
		{
			task.OutputFile = new TaskItem(Path.Combine(this.testDirectory, @"merged3.dll"));
			task.AllowDuplicateTypes = TaskUtility.StringArrayToItemArray(@"ClassAB");
			task.DebugInfo = false;
			task.XmlDocumentation = false;
			task.ExcludeFile = new TaskItem(this.excludeFile);

			Assert.IsTrue(task.Execute(), @"Task failed");
			this.AssertResults(task, PublicACount + 1, VersionA);
		}

		/// <summary>
		/// Signed merged assembly.
		/// </summary>
		[Test(Description = "Signed merged assembly")]
		public void SignedMergedAssembly()
		{
			task.OutputFile = new TaskItem(Path.Combine(this.testDirectory, @"merged4.dll"));
			task.AllowDuplicateTypes = TaskUtility.StringArrayToItemArray(@"ClassAB");
			task.DebugInfo = false;
			task.XmlDocumentation = false;
			task.KeyFile = new TaskItem(this.keyFile);

			Assert.IsTrue(task.Execute(), @"Task failed");
			this.AssertResults(task, PublicCount, VersionA);
		}

		/// <summary>
		/// Explicitely versioned merged assembly.
		/// </summary>
		[Test(Description = "Explicitely versioned merged assembly")]
		public void VersionedMergedAssembly()
		{
			task.OutputFile = new TaskItem(Path.Combine(this.testDirectory, @"merged5.dll"));
			task.AllowDuplicateTypes = TaskUtility.StringArrayToItemArray(@"ClassAB");
			task.DebugInfo = false;
			task.XmlDocumentation = false;
			task.KeyFile = new TaskItem(this.keyFile);
			string version = @"5.6.7.8";
			task.Version = version;

			Assert.IsTrue(task.Execute(), @"Task failed");
			this.AssertResults(task, PublicCount, version);
		}

		#endregion Test cases

		#region Private Methods

		private void AssertResults(ILMerge task, int publicTypeCount, string version)
		{
			MessageImportance imp = MessageImportance.Low;

			Assert.IsTrue(File.Exists(task.OutputFile.ItemSpec), @"No merged assembly");
			string pdbFile = Path.ChangeExtension(task.OutputFile.ItemSpec, "pdb");
			Assert.AreEqual(task.DebugInfo, File.Exists(pdbFile), @"No debug file");
			string xmlFile = Path.ChangeExtension(task.OutputFile.ItemSpec, @"xml");
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