
using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Utilities;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests
{
	[TestFixture]
	public class TemplateFileTest
	{
		private static string _template =
			@"
			Template File 1
			${TemplateItem} = TemplateItem
			{Non-TemplateItem} = NonTemplateItem
			${item2}
			${CASEInsenSiTiveTest}
            ${Template.Item.With.Dot}
		";
		private static string _templateReplaced =
			_template.Replace("${TemplateItem}", "**Item1**").Replace("${item2}", "**Item2**")
                .Replace("${CASEInsenSiTiveTest}", "**Item3**")
                .Replace("${Template.Item.With.Dot}", "**Item4**");
		private static string _templateFilename;
		private string _replacedFilename;

		[OneTimeSetUp]
		public void FixtureInit()
		{
			MockBuild buildEngine = new MockBuild();
			TaskUtility.makeTestDirectory(buildEngine);
			_templateFilename = Path.Combine(TaskUtility.TestDirectory, typeof (TemplateFileTest).Name + ".txt");
		}

		[SetUp]
		public void Setup()
		{
			File.WriteAllText(_templateFilename, _template);
			_replacedFilename = null;
		}

		[TearDown]
		public void Teardown()
		{
			if (_replacedFilename != null && File.Exists(_replacedFilename))
			{
				File.Delete(_replacedFilename);
			}
		}

		private void SetMetaData(TaskItem item, string data, bool set)
		{
			if (set)
			{
				item.SetMetadata(TemplateFile.MetadataValueTag, data);
			}
		}

		private TaskItem[] GetTaskItems()
		{
			return GetTaskItems(true);
		}

		private TaskItem[] GetTaskItems(bool includeMetaData)
		{
			List<TaskItem> result = new List<TaskItem>();
			TaskItem item = new TaskItem("TemplateItem");
			SetMetaData(item, "**Item1**", includeMetaData);
			result.Add(item);
			item = new TaskItem("item2");
			SetMetaData(item, "**Item2**", includeMetaData);
			result.Add(item);
			item = new TaskItem("caseInsensitiveTest");
			SetMetaData(item, "**Item3**", includeMetaData);
			result.Add(item);
            item = new TaskItem("Template.Item.With.Dot");
            SetMetaData(item, "**Item4**", includeMetaData);
            result.Add(item);
			return result.ToArray();
		}

		private TaskItem[] GetTaskItemsMissing()
		{
			List<TaskItem> result = new List<TaskItem>();
			TaskItem item = new TaskItem("TemplateItem");
			SetMetaData(item, "**Item1**", true);
			result.Add(item);
			item = new TaskItem("caseInsensitiveTest");
			SetMetaData(item, "**Item3**", true);
			result.Add(item);
			return result.ToArray();
		}

		[Test]
		public void TemplateFileDefault()
		{
			MockBuild build = new MockBuild();
			TemplateFile tf = new TemplateFile();
			tf.BuildEngine = build;
			tf.Template = new TaskItem(_templateFilename);
			tf.Tokens = GetTaskItems();
			Assert.IsTrue(tf.Execute());
			Assert.IsNotNull(tf.OutputFile);
			Assert.IsTrue(File.Exists(tf.OutputFile.ItemSpec));
			_replacedFilename = tf.OutputFile.ItemSpec;
			Assert.AreEqual(Path.ChangeExtension(_templateFilename, ".out"), _replacedFilename);
			string replaced = File.ReadAllText(tf.OutputFile.ItemSpec);
			Assert.AreEqual(_templateReplaced, replaced);
		}

		[Test]
		public void TemplateFileInvalidTemplate()
		{
			MockBuild build = new MockBuild();
			TemplateFile tf = new TemplateFile();
			tf.BuildEngine = build;
			tf.Template = new TaskItem("non_existant_file");
			tf.Tokens = GetTaskItems();
			Assert.IsFalse(tf.Execute());
			Assert.AreEqual(1, build.ErrorCount);
		}

		[Test]
		public void TemplateFileNewFilename()
		{
			MockBuild build = new MockBuild();
			TemplateFile tf = new TemplateFile();
			tf.BuildEngine = build;
			tf.Template = new TaskItem(_templateFilename);
			string outputfile = Path.Combine(Path.GetDirectoryName(_templateFilename), "Replacement.file");
			tf.OutputFilename = outputfile;
			tf.Tokens = GetTaskItems();
			Assert.IsTrue(tf.Execute());
			Assert.IsNotNull(tf.OutputFile);
			Assert.IsTrue(File.Exists(tf.OutputFile.ItemSpec));
			_replacedFilename = tf.OutputFile.ItemSpec;
			Assert.AreEqual(outputfile, _replacedFilename);
			string replaced = File.ReadAllText(tf.OutputFile.ItemSpec);
			Assert.AreEqual(_templateReplaced, replaced);
		}

		[Test]
		public void TemplateFileNoMetaData()
		{
			MockBuild build = new MockBuild();
			TemplateFile tf = new TemplateFile();
			tf.BuildEngine = build;
			tf.Template = new TaskItem(_templateFilename);
			tf.Tokens = GetTaskItems(false);
			Assert.IsTrue(tf.Execute());
			Assert.IsNotNull(tf.OutputFile);
			Assert.IsTrue(File.Exists(tf.OutputFile.ItemSpec));
			_replacedFilename = tf.OutputFile.ItemSpec;
			Assert.AreEqual(Path.ChangeExtension(_templateFilename, ".out"), _replacedFilename);
			string replaced = File.ReadAllText(tf.OutputFile.ItemSpec);
			string shouldBeReplaced =
				_template.Replace("${TemplateItem}", "").Replace("${item2}", "").Replace("${CASEInsenSiTiveTest}", "")
                    .Replace("${Template.Item.With.Dot}", "");
			Assert.AreEqual(shouldBeReplaced, replaced);
		}

		[Test]
		public void TemplateFileNoTokens()
		{
			MockBuild build = new MockBuild();
			TemplateFile tf = new TemplateFile();
			tf.BuildEngine = build;
			tf.Template = new TaskItem(_templateFilename);
			Assert.IsTrue(tf.Execute());
			Assert.IsNotNull(tf.OutputFile);
			Assert.IsTrue(File.Exists(tf.OutputFile.ItemSpec));
			_replacedFilename = tf.OutputFile.ItemSpec;
			Assert.AreEqual(Path.ChangeExtension(_templateFilename, ".out"), _replacedFilename);
			string replaced = File.ReadAllText(tf.OutputFile.ItemSpec);
			Assert.AreEqual(_template, replaced);
		}

		[Test]
		public void TemplateFileMissingToken()
		{
			MockBuild build = new MockBuild();
			TemplateFile tf = new TemplateFile();
			tf.BuildEngine = build;
			tf.Template = new TaskItem(_templateFilename);
			tf.Tokens = GetTaskItemsMissing();
			Assert.IsTrue(tf.Execute());
			Assert.IsNotNull(tf.OutputFile);
			Assert.IsTrue(File.Exists(tf.OutputFile.ItemSpec));
			_replacedFilename = tf.OutputFile.ItemSpec;
			Assert.AreEqual(Path.ChangeExtension(_templateFilename, ".out"), _replacedFilename);
			string replaced = File.ReadAllText(tf.OutputFile.ItemSpec);
			string shouldBeReplaced = _template.Replace("${TemplateItem}", "**Item1**").Replace("${CASEInsenSiTiveTest}", "**Item3**");
			Assert.AreEqual(shouldBeReplaced, replaced);
		}
	}
}