//$Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace MSBuild.Community.Tasks.Tests
{
	/// <summary>
	/// Tests for RegexMatch and RegexReplace functions
	/// </summary>
	[TestFixture]
	public class RegexTest
	{
		/// <summary>
		/// Tests RegExMatch finds correct number of matches in a known sample set
		/// </summary>
		[Test(Description = "RegexMatch test")]
		public void RegexMatchExecute()
		{
			RegexMatch task = new RegexMatch();
			task.BuildEngine = new MockBuild();

			task.Input = TaskUtility.StringArrayToItemArray("foo.my.foo.foo.test.o", "foo.my.faa.foo.test.a", "foo.my.fbb.foo.test.b", "foo.my.fcc.foo.test.c", "foo.my.fdd.foo.test.d");
			task.Expression = new TaskItem("[a-c]$");

			Assert.IsTrue(task.Execute(), "Execute Failed");

			Assert.AreEqual(3, task.Output.Length, "Match should have matched three items");

		}

		/// <summary>
		/// Tests RegexReplace by removing the first "foo." string from a list of items
		/// </summary>
		[Test(Description = "RegexReplace test, removes first 'foo.' from a list of test items")]
		public void RegexReplaceRemoveFirstFoo()
		{
			RegexReplace task = new RegexReplace();
			task.BuildEngine = new MockBuild();

			task.Input = TaskUtility.StringArrayToItemArray("foo.my.foo.foo.test.o", "foo.my.faa.foo.test.a", "foo.my.fbb.foo.test.b", "foo.my.fcc.foo.test.c", "foo.my.fdd.foo.test.d");
			task.Expression = new TaskItem("foo\\.");
			task.Count = new TaskItem("1");

			Assert.IsTrue(task.Execute(), "Execute Failed");

			foreach (ITaskItem item in task.Output)
			{
				Assert.IsTrue(!item.ItemSpec.StartsWith("foo."), string.Format("Item still starts with foo: {0}", item.ItemSpec));
			}
		}

		/// <summary>
		/// Tests RegexReplace by replacing the string "foo." appearing after the 6th character with the string "oop." from a list of items
		/// </summary>
		[Test(Description = "Tests RegexReplace by replacing the string 'foo.' appearing after the 6th character with the string 'oop.' from a list of items")]
		public void RegexReplaceFooForOop()
		{
			RegexReplace task = new RegexReplace();
			task.BuildEngine = new MockBuild();
			string[] expectedResult = new string[] { "foo.my.oop.oop.test.o", "foo.my.faa.oop.test.a", "foo.my.fbb.oop.test.b", "foo.my.fcc.oop.test.c", "foo.my.fdd.oop.test.d" };

			task.Input = TaskUtility.StringArrayToItemArray("foo.my.foo.foo.test.o", "foo.my.faa.foo.test.a", "foo.my.fbb.foo.test.b", "foo.my.fcc.foo.test.c", "foo.my.fdd.foo.test.d");
			task.Expression = new TaskItem("foo\\.");
			task.StartAt = new TaskItem("6");
			task.Replacement = new TaskItem("oop.");

			Assert.IsTrue(task.Execute(), "Execute Failed");

			for (int ndx = 0; ndx < task.Output.Length; ndx++)
			{
				Assert.AreEqual(expectedResult[ndx], task.Output[ndx].ItemSpec, "Results did not match expectations");
			}
		}


		/// <summary>
		/// Tests RegexReplace by replacing the first "." with a "!" starting from the right from a list of items
		/// </summary>
		[Test(Description = "Tests RegexReplace by replacing the first '.' with a '!' starting from the right from a list of items")]
		public void RegexReplaceLastDotForBang()
		{
			RegexReplace task = new RegexReplace();
			task.BuildEngine = new MockBuild();

			task.Input = TaskUtility.StringArrayToItemArray("foo.my.foo.foo.test.o", "foo.my.faa.foo.test.a", "foo.my.fbb.foo.test.b", "foo.my.fcc.foo.test.c", "foo.my.fdd.foo.test.d");
			task.Expression = new TaskItem("\\.");
			task.Replacement = new TaskItem("!");
			task.Count = new TaskItem("1");
			task.Options = TaskUtility.StringArrayToItemArray("RightToLeft");

			Assert.IsTrue(task.Execute(), "Execute Failed");

			foreach (ITaskItem item in task.Output)
			{
				Assert.AreEqual("!", item.ItemSpec.Substring(19, 1), string.Format("Item not replaced properly: {0}", item.ItemSpec));
			}
		}

		/// <summary>
		/// Tests RegexMatchGroups by extracting first and/or second numeric parts from numeric expressions concantenated by a dot
		/// </summary>
		[Test(Description = "Tests RegexMatchGroups by extracting first and/or second numeric parts from numeric expressions concantenated by a dot")]
		public void RegexExtractValuesForDefinedGroup()
		{
			var task = new RegexMatchGroups
			{
				BuildEngine = new MockBuild(),
				Expression = new TaskItem(@"(?<FirstGroup>\d+)\.(?<SecondGroup>\d+)"),
				Input = TaskUtility.StringArrayToItemArray("1.2", "erster.zweiter", "1.2.3")
			};

			Assert.IsTrue(task.Execute(), "Execute Failed");

			Assert.IsNotNull(
				task.Output.FirstOrDefault(q =>
					q.GetMetadata("OriginalItem").Equals("1.2") && q.GetMetadata("GroupName").Equals("FirstGroup") &&
					q.ItemSpec.Equals("1"))
				);

			task = new RegexMatchGroups
			{
				BuildEngine = new MockBuild(),
				Expression = new TaskItem(@"(?<FirstGroup>\d+)\.(?<SecondGroup>\d+)"),
				Input = TaskUtility.StringArrayToItemArray("1","1.2.3")
			};

			Assert.IsTrue(task.Execute(), "Execute Failed");

			Assert.IsNull(
				task.Output.FirstOrDefault(q =>
					q.GetMetadata("OriginalItem").Equals("1") && q.GetMetadata("GroupName").Equals("FirstGroup") &&
					q.ItemSpec.Equals("1"))
			);

			Assert.IsNotNull(
				task.Output.FirstOrDefault(q =>
					q.GetMetadata("OriginalItem").Equals("1.2.3") && q.GetMetadata("GroupName").Equals("FirstGroup") &&
					q.ItemSpec.Equals("1"))
			);
		}
	}
}