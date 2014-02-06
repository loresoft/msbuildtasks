using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using MSBuild.Community.Tasks.Git;

namespace MSBuild.Community.Tasks.Tests.Git
{
	[TestFixture]
	public class GitBranchTest
	{
		[Test]
		public void TestParseStatusLineOnTag()
		{
			string statusLine = "# On branch 0.1.0";
			GitBranch gitBranch = new GitBranch();
			Assert.AreEqual("0.1.0", gitBranch.ParseStatusLineOutput(statusLine));

			statusLine = "On branch 0.1.0";
			Assert.AreEqual("0.1.0", gitBranch.ParseStatusLineOutput(statusLine));
		}

		[Test]
		public void TestParseStatusLineOnBranch()
		{
			string statusLine = "# On branch 0.1  ";
			GitBranch gitBranch = new GitBranch();
			Assert.AreEqual("0.1", gitBranch.ParseStatusLineOutput(statusLine));

			statusLine = "On branch 0.1  ";
			Assert.AreEqual("0.1", gitBranch.ParseStatusLineOutput(statusLine));
		}

		[Test]
		public void TestParseStatusLineOnMaster()
		{
			string statusLine = "# On branch master    ";
			GitBranch gitBranch = new GitBranch();
			Assert.AreEqual("master", gitBranch.ParseStatusLineOutput(statusLine));

			statusLine = "On branch master    ";
			Assert.AreEqual("master", gitBranch.ParseStatusLineOutput(statusLine));
		}

		[Test]
		public void TestIsBranchStatusLine()
		{
			GitBranch gitBranch = new GitBranch();
			Assert.IsTrue(gitBranch.IsBranchStatusLine("# On branch 0.1"));
			Assert.IsFalse(gitBranch.IsBranchStatusLine("# Changes not staged for commit:"));
			Assert.IsFalse(gitBranch.IsBranchStatusLine("#   (use \"git add <file>...\" to update what will be committed)"));
			Assert.IsFalse(gitBranch.IsBranchStatusLine("#   (use \"git checkout -- <file>...\" to discard changes in working directory)"));
			Assert.IsFalse(gitBranch.IsBranchStatusLine("#"));
			Assert.IsFalse(gitBranch.IsBranchStatusLine("#       modified:   build/MSBuildCodeMetrics.build"));
			Assert.IsFalse(gitBranch.IsBranchStatusLine("#"));
			Assert.IsFalse(gitBranch.IsBranchStatusLine("# Untracked files:"));
			Assert.IsFalse(gitBranch.IsBranchStatusLine("#   (use \"git add <file>...\" to include in what will be committed)"));
			Assert.IsFalse(gitBranch.IsBranchStatusLine("#"));
			Assert.IsFalse(gitBranch.IsBranchStatusLine("#       build/git.bat"));
			Assert.IsFalse(gitBranch.IsBranchStatusLine("no changes added to commit (use \"git add\" and/or \"git commit -a\")"));

			Assert.IsTrue(gitBranch.IsBranchStatusLine("On branch 0.1"));
			Assert.IsFalse(gitBranch.IsBranchStatusLine("Changes not staged for commit:"));
			Assert.IsFalse(gitBranch.IsBranchStatusLine("  (use \"git add <file>...\" to update what will be committed)"));
			Assert.IsFalse(gitBranch.IsBranchStatusLine("  (use \"git checkout -- <file>...\" to discard changes in working directory)"));
			Assert.IsFalse(gitBranch.IsBranchStatusLine(""));
			Assert.IsFalse(gitBranch.IsBranchStatusLine("      modified:   build/MSBuildCodeMetrics.build"));
			Assert.IsFalse(gitBranch.IsBranchStatusLine(""));
			Assert.IsFalse(gitBranch.IsBranchStatusLine("Untracked files:"));
			Assert.IsFalse(gitBranch.IsBranchStatusLine("  (use \"git add <file>...\" to include in what will be committed)"));
			Assert.IsFalse(gitBranch.IsBranchStatusLine(""));
			Assert.IsFalse(gitBranch.IsBranchStatusLine("      build/git.bat"));
			Assert.IsFalse(gitBranch.IsBranchStatusLine("no changes added to commit (use \"git add\" and/or \"git commit -a\")"));
		}
	}
}
