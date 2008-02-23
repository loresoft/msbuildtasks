// $Id$
// Copyright © 2008 Ignaz Kohlbecker

using System;
using Microsoft.Build.Utilities;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests
{
	/// <summary>
	/// NUnit tests for the MSBuild <see cref="Microsoft.Build.Framework.Task"/> 
	/// <see cref="Beep"/>.
	/// </summary>
	[TestFixture]
	public class BeepTest
	{
		[Test(Description = "Execute the Beep task with default values")]
		public void BeepDefault()
		{
			Beep task = new Beep();
			task.BuildEngine = new MockBuild();

			// assert default values
			Assert.AreEqual(800, task.Frequency, @"Wrong default frequency");
			Assert.AreEqual(200, task.Duration, @"Wrong default duration");

			Assert.IsTrue(task.Execute(), @"Beep task failed");

		}

		[Test(Description = "Execute the Beep task with custom values")]
		public void BeepCustom()
		{
			Beep task = new Beep();
			task.BuildEngine = new MockBuild();
			task.Frequency = 440;
			task.Duration = 400;

			Assert.IsTrue(task.Execute(), @"Beep task failed");

		}

		[Test(Description = "Execute the Beep task with frequency out of range")]
		public void BeepBadFrequency()
		{
			Beep task = new Beep();
			task.BuildEngine = new MockBuild();
			task.Frequency = 0;

			Assert.IsTrue(task.Execute(), @"Beep task failed");

		}

		[Test(Description = "Execute the Beep task with duration out of range")]
		public void BeepBadDuration()
		{
			Beep task = new Beep();
			task.BuildEngine = new MockBuild();
			task.Duration = 0;

			Assert.IsTrue(task.Execute(), @"Beep task failed");

		}

	}
}