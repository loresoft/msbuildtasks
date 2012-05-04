

using System;
using System.Globalization;
using System.Threading;
using Microsoft.Build.Framework;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests
{
	[TestFixture]
	public class TimeTest
	{
		[Test(Description = "Basic execution of the Time task")]
		public void TimeExecute()
		{
			Time task = new Time();
			task.BuildEngine = new MockBuild();
			Assert.IsTrue(task.Execute(), "Execute Failed!");
		}

		[Test(Description = "Create timestamps with and without format")]
		public void FormattedTime()
		{
			Time task = new Time();
			task.BuildEngine = new MockBuild();

			// test without the optional Format property
			Assert.IsNull(task.Format, @"Format not null");
			Assert.IsNull(task.FormattedTime, @"Formatted time not null");
			Assert.IsTrue(task.Execute(), @"Time task without Format failed");
			Assert.IsNull(task.Format, @"Format not null after executing task");
			DateTime time = task.DateTimeValue;
			Assert.IsNotNull(task.FormattedTime, @"Formatted time null after executing task");
			task.Log.LogMessage(MessageImportance.Low, "Time without format: \"{0}\" (local)", task.FormattedTime);
			Assert.AreEqual(time.ToString(DateTimeFormatInfo.InvariantInfo), task.FormattedTime,
				@"Wrong default local time");

			// the .Now property has a limited resolution
			// according to ms-help://MS.VSCC.v80/MS.MSDN.v80/MS.NETDEVFX.v20.en/cpref2/html/P_System_DateTime_Now.htm
			// therefore wait some time to ensure we get a different value
			// when executing a second time
			Thread.Sleep(1000);

			// second execute must yield another time stamp
			Assert.IsTrue(task.Execute(), @"Time task without Format failed in second execution");
			task.Log.LogMessage(MessageImportance.Low, "Time without format: \"{0}\" (local)", task.FormattedTime);
			Assert.AreNotEqual(time, task.DateTimeValue, @"Time doesn't change with second execution");

			// specify the format
			task.Format = @"yyyyMMddHHmmss";
			task.Kind = System.DateTimeKind.Utc.ToString();
			Assert.IsTrue(task.Execute(), @"Time task with Format failed");
			task.Log.LogMessage(MessageImportance.Low, "Time with format: \"{0}\" (UTC)", task.FormattedTime);
			Assert.AreEqual(task.Format.Length, task.FormattedTime.Length, @"Wrong time length");

		}
	}
}
