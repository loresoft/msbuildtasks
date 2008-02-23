// $Id$
// Copyright © 2008 Ignaz Kohlbecker

using System;
using System.IO;
using Microsoft.Build.Utilities;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests
{
	/// <summary>
	/// NUnit tests for the MSBuild <see cref="Microsoft.Build.Framework.Task"/> 
	/// <see cref="Sound"/>.
	/// </summary>
	[TestFixture]
	public class SoundTest
	{
		[Test(Description = @"Execute the Sound task with a .wav file from the windows media directory")]
		public void SystemSound()
		{
			Sound task = new Sound();
			task.BuildEngine = new MockBuild();

			// assert default values
			Assert.AreEqual(string.Empty, task.SoundLocation, @"unexpected default sound location");

			string soundFile = @"..\Media\notify.wav";
			task.SystemSoundFile = soundFile;
			Assert.AreEqual(Path.Combine(Environment.SystemDirectory, soundFile).ToString(), task.SoundLocation,
				@"Wrong sound file");

			Assert.IsTrue(task.Execute(), @"Sound task failed");

		}

		[Test(Description = @"Execute the Sound task with a .wav file from the ""MyMusic"" directory")]
		public void MyMusicSound()
		{
			string myMusicFile = @"25881_acclivity_3beeps1000.wav";
			if (!File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
				myMusicFile).ToString()))
			{
				Assert.Ignore(@"Test case needs file """ + myMusicFile + @""" in the ""MyMusic"" folder.");
			}

			Sound task = new Sound();
			task.BuildEngine = new MockBuild();
			task.MyMusicFile = myMusicFile;

			Assert.IsTrue(task.Execute(), @"Sound task failed");

		}

	}
}