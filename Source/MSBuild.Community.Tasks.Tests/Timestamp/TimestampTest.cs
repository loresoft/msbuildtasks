// $Id$
// Copyright © 2006 Ignaz Kohlbecker

using System;
using System.Globalization;
using System.Threading;
using Microsoft.Build.Framework;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests
{
    /// <summary>
    /// NUnit tests for the MSBuild <see cref="Microsoft.Build.Framework.Task"/> 
    /// <see cref="Timestamp"/>.
    /// </summary>
    [TestFixture]
    public class TimestampTest
    {

        [Test(Description = "Create timestamps with and without format")]
        public void TimestampExecute()
        {
            Timestamp task = new Timestamp();
            task.BuildEngine = new MockBuild();

            // test without the optional Format property
            Assert.IsNull(task.Format, @"Format not null");
            Assert.IsNull(task.LocalTimestamp, @"Formatted local time stamp not null");
            Assert.IsNull(task.UniversalTimestamp, @"Formatted universal time stamp not null");
            Assert.IsTrue(task.Execute(), @"Timestamp task without Format failed");
            Assert.IsNull(task.Format, @"Format not null after executing task");
            DateTime timestamp = task.LocalTimestampValue;
            Assert.IsNotNull(task.LocalTimestamp, @"Formatted local time stamp null after executing task");
            Assert.IsNotNull(task.UniversalTimestamp, @"Formatted universal time stamp null after executing task");
            task.Log.LogMessage(MessageImportance.Low, "Timestamp without format: \"{0}\" (local)", task.LocalTimestamp);
            task.Log.LogMessage(MessageImportance.Low, "Timestamp without format: \"{0}\" (UTC)", task.UniversalTimestamp);
            Assert.AreEqual(timestamp.ToString(DateTimeFormatInfo.InvariantInfo), task.LocalTimestamp,
                @"Wrong default local time stamp");
            Assert.AreEqual(timestamp.ToUniversalTime().ToString(DateTimeFormatInfo.InvariantInfo), task.UniversalTimestamp,
                @"Wrong default universal time stamp");

            // the .Now property has a limited resolution
            // according to ms-help://MS.VSCC.v80/MS.MSDN.v80/MS.NETDEVFX.v20.en/cpref2/html/P_System_DateTime_Now.htm
            // therefore wait some time to ensure we get a different value
            // when executing a second time
            Thread.Sleep(1000);

            // second execute must yield another time stamp
            Assert.IsTrue(task.Execute(), @"Timestamp task without Format failed in second execution");
            task.Log.LogMessage(MessageImportance.Low, "Timestamp without format: \"{0}\" (local)", task.LocalTimestamp);
            task.Log.LogMessage(MessageImportance.Low, "Timestamp without format: \"{0}\" (UTC)", task.UniversalTimestamp);
            Assert.AreNotEqual(timestamp, task.LocalTimestampValue, @"Time stamp doesn't change with second execution");

            // specify the format
            task.Format = @"yyyyMMddHHmmss";
            Assert.IsTrue(task.Execute(), @"Timestamp task with Format failed");
            task.Log.LogMessage(MessageImportance.Low, "Timestamp with format: \"{0}\" (local)", task.LocalTimestamp);
            task.Log.LogMessage(MessageImportance.Low, "Timestamp with format: \"{0}\" (UTC)", task.UniversalTimestamp);
            Assert.AreEqual(task.Format.Length, task.LocalTimestamp.Length, @"Wrong local timestamp length");
            Assert.AreEqual(task.Format.Length, task.UniversalTimestamp.Length, @"Wrong universal timestamp length");

        }
    }
}
