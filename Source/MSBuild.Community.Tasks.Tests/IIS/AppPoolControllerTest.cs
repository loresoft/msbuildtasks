using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using MSBuild.Community.Tasks.IIS;

namespace MSBuild.Community.Tasks.Tests.IIS
{
	[TestFixture]
	public class AppPoolControllerTest
	{
		private string mAppPoolName = "TestAppPool";
		private string mServer = "fenway";

		[TestFixtureSetUp]
		public void TestFixtureInitilize()
		{
			Console.WriteLine("Setting up test objects...");

			AppPoolCreate task = new AppPoolCreate();
			task.BuildEngine = new MockBuild();
			task.ApplicationPoolName = mAppPoolName;
			task.ServerName = mServer;
			task.Execute();
			
			Console.WriteLine("TestFixture SetUp is complete.");
		}

		[TestFixtureTearDown]
		public void TestFixtureDispose()
		{
			Console.WriteLine("Cleaning up...");
			
			AppPoolDelete task = new AppPoolDelete();
			task.BuildEngine = new MockBuild();
			task.ApplicationPoolName = mAppPoolName;
			task.ServerName = mServer;
			task.Execute();
			
			Console.WriteLine("TestFixture TearDown is complete.");
		}

		[Test]
		public void RecycleTest()
		{
			// Recycle
			AppPoolController task = new AppPoolController();
			task.BuildEngine = new MockBuild();
			task.ServerName = mServer;
			task.ApplicationPoolName = mAppPoolName;
			task.Action = "Recycle";
			Assert.IsTrue(task.Execute(), "Execute Failed!");
		}

		[Test]
		public void RestartTest()
		{
			// Restart
			AppPoolController task = new AppPoolController();
			task.BuildEngine = new MockBuild();
			task.ServerName = mServer;
			task.ApplicationPoolName = mAppPoolName;
			task.Action = "Restart";
			Assert.IsTrue(task.Execute(), "Execute Failed!");
		}

		[Test]
		public void StartTest()
		{
			// Start
			AppPoolController task = new AppPoolController();
			task.BuildEngine = new MockBuild();
			task.ServerName = mServer;
			task.ApplicationPoolName = mAppPoolName;
			task.Action = "Start";
			Assert.IsTrue(task.Execute(), "Execute Failed!");
		}

		[Test]
		public void StopTest()
		{
			// Stop
			AppPoolController task = new AppPoolController();
			task.BuildEngine = new MockBuild();
			task.ServerName = mServer;
			task.ApplicationPoolName = mAppPoolName;
			task.Action = "Stop";
			Assert.IsTrue(task.Execute(), "Execute Failed!");
		}
	}
}
