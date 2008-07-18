// $Id: FtpUploadTest.cs 281 2006-12-12 05:24:25Z pettys $

using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;

namespace MSBuild.Community.Tasks.Tests
{
    /// <summary>
    /// Summary description for FtpTest
    /// </summary>
    [TestFixture]
    public class FtpUploadTest
    {

		private MockBuild _mockBuild;

        [SetUp]
		public void SetUp() {
			_mockBuild = new MockBuild();
			TaskUtility.makeTestDirectory(_mockBuild);
		}

		[TearDown]
		public void TearDown() {
			TaskUtility.deleteTestDirectory(_mockBuild);
		}

		[Test]
		public void UploadSingleFile() {
			// set up test environment
			string testFile = Path.Combine(TaskUtility.TestDirectory, "FtpUploadFile.txt");

			string fileContent = "some stuff in this file";
            File.WriteAllText(testFile, fileContent);
			MemoryStream ftpRequestStream = new MemoryStream();

			// record expected operations
			var mockery = new MockRepository();
			var ftpService = mockery.CreateMock<IFtpWebRequest>();
			var ftpCreator = mockery.CreateMock<IFtpWebRequestCreator>();

			FtpUpload task = new FtpUpload(ftpCreator);

			using (mockery.Record()) {
				ftpCreator.Create(new Uri("ftp://server.com/folder/FtpUploadFile.txt"), "STOR");
				LastCall.Return(ftpService);

				ftpService.SetContentLength(23);

				ftpService.GetRequestStream();
				LastCall.Return(ftpRequestStream);

				ftpService.GetStatusDescriptionAndCloseResponse();
				LastCall.Return("okay");
			}

			task.BuildEngine = _mockBuild;
			task.RemoteUri = "ftp://server.com/folder/";
			task.LocalFile = testFile;
			bool result = task.Execute();

			Assert.AreEqual(fileContent, GetString(ftpRequestStream));

			mockery.VerifyAll();
		}

		[Test]
		public void UploadMultipleFilesInFolderStructure() {
			// set up test environment
			string testFile1 = Path.Combine(TaskUtility.TestDirectory, "file1.txt");
			string testDir2 = Path.Combine(TaskUtility.TestDirectory, "testfolder");
			string testFile2 = Path.Combine(testDir2, "file2.txt");

			Directory.CreateDirectory(testDir2);
			File.WriteAllText(testFile1, "file number one");
			File.WriteAllText(testFile2, "file number two");
			MemoryStream file1Stream = new MemoryStream();
			MemoryStream file2Stream = new MemoryStream();

			// record expected operations
			var mockery = new MockRepository();
			var ftpService = mockery.CreateMock<IFtpWebRequest>();
			var ftpCreator = mockery.CreateMock<IFtpWebRequestCreator>();

			FtpUpload task = new FtpUpload(ftpCreator);

			using (mockery.Record()) {
				// this call responds to the FtpUpload's request to list folders; return no folders.
				ftpCreator.Create(new Uri("ftp://server.com/folder/"), "NLST");
				LastCall.Return(ftpService);
				ftpService.GetResponseStream();
				LastCall.Return(new MemoryStream(new byte[0]));

				// this call should make the testfolder
				ftpCreator.Create(new Uri("ftp://server.com/folder/testfolder"), "MKD");
				LastCall.Return(ftpService);
				ftpService.GetAndCloseResponse();

				// now upload file1
				ftpCreator.Create(new Uri("ftp://server.com/folder/file1.txt"), "STOR");
				LastCall.Return(ftpService);
				ftpService.SetContentLength(15);
				ftpService.GetRequestStream();
				LastCall.Return(file1Stream);
				ftpService.GetStatusDescriptionAndCloseResponse();
				LastCall.Return("okay");

				// now upload file2
				ftpCreator.Create(new Uri("ftp://server.com/folder/testfolder/file2.txt"), "STOR");
				LastCall.Return(ftpService);
				ftpService.SetContentLength(15);
				ftpService.GetRequestStream();
				LastCall.Return(file2Stream);
				ftpService.GetStatusDescriptionAndCloseResponse();
				LastCall.Return("okay");
			}

			task.BuildEngine = _mockBuild;
			task.RemoteUri = "ftp://server.com/folder/";
			task.LocalFiles = TaskUtility.StringArrayToItemArray(testFile1, testFile2);
			task.RemoteFiles = TaskUtility.StringArrayToItemArray("file1.txt", "testfolder\\file2.txt");
			bool result = task.Execute();

			Assert.AreEqual("file number one", GetString(file1Stream));
			Assert.AreEqual("file number two", GetString(file2Stream));

			mockery.VerifyAll();
		}

		private string GetString(MemoryStream stream) {
			var newStream = new MemoryStream(stream.ToArray());
			return new StreamReader(newStream).ReadToEnd();
		}

    }
}
