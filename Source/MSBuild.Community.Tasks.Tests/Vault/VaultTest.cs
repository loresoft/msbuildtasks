// $Id$
// Copyright © 2006 Douglas Rohm

using System;
using System.IO;
using Microsoft.Build.Utilities;
using MSBuild.Community.Tasks.Vault;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests.Vault
{
	/// <summary>
	/// Summary description for VaultTest.
	/// </summary>
	[TestFixture]
	public class VaultTest
	{
		private string _username;
		private string _password;
		private string _adminUsername;
		private string _adminPassword;
		private string _url;
		private string _repository;
		private string _repositoryPath;
		private string _repositoryFile;
		private string _diskFile;
		private string _checkoutComment;
		private string _checkinComment;
		private string _addFileComment;
		private string _workingFolder;
		private string _newTestFile1;
		private string _newTestFile2;
		private MockBuild _buildEngine;

		[TestFixtureSetUp]
		public void FixtureInit()
		{
			_username = "msbuild";
			_password = "m$bui1d";
			_adminUsername = "testadmin";
			_adminPassword = "password";
			_url = "http://fenway";
			_repository = "DougRohm";
			_repositoryPath = "$/Temp/";
			_repositoryFile = "VaultTestFile.txt";
			_diskFile = @"C:\Development\MSBuildTasks\trunk\Build\Debug\test\MSBuild.Community.Tasks.Tests\VaultTestFile.txt";
			_checkoutComment = "Vault unit test for Checkout task.";
			_checkinComment = "Vault unit test for Checkin task.";
			_addFileComment = "Vault unit test for AddFile task.";
			_buildEngine = new MockBuild();
			_workingFolder = TaskUtility.makeTestDirectory(_buildEngine);
			_newTestFile1 = "VaultAddFile1.txt";
			_newTestFile2 = "VaultAddFile2.txt";
			
			// Determine if Vault server is available
			DetermineVaultServerStatus();
		}

		private void DetermineVaultServerStatus()
		{
			VaultCheckout _vaultTest = new VaultCheckout();
			_vaultTest.BuildEngine = _buildEngine;
			_vaultTest.Username = _username;
			_vaultTest.Password = _password;
			_vaultTest.Url = _url;
			try
			{
				_vaultTest.Login();
			}
			catch (Exception)
			{
				Assert.Ignore(@"The Vault server was not found and is required to run this test fixture.");
			}
		}

		[Test(Description = "Checkout from repository test.")]
		public void VaultCheckoutTest()
		{
			VaultCheckout checkout = new VaultCheckout();
			checkout.BuildEngine = _buildEngine;
			checkout.Username = _username;
			checkout.Password = _password;
			checkout.Url = _url;
			checkout.Repository = _repository;
			checkout.Path = _repositoryPath + _repositoryFile;
			checkout.Comment = _checkoutComment;
			checkout.WorkingFolder = _workingFolder;
			
			Assert.IsNotNull(checkout);
			bool result = checkout.Execute();
			Assert.IsTrue(result, "Checkout failed!");
			Assert.IsTrue(checkout.Version > 0, "Version number is not greater than zero.");

			UndoCheckout();
		}

		[Test(Description = "Checkin to repository test.")]
		public void VaultCheckinTest()
		{
			CheckoutFile();

			VaultCheckin checkin = new VaultCheckin();
			checkin.BuildEngine = _buildEngine;
			checkin.Username = _username;
			checkin.Password = _password;
			checkin.Url = _url;
			checkin.Repository = _repository;
			checkin.Path = _repositoryPath + _repositoryFile;
			checkin.DiskFile = _diskFile;
			checkin.Comment = _checkinComment;
			checkin.WorkingFolder = _workingFolder;
			
			Assert.IsNotNull(checkin);
			bool result = checkin.Execute();
			Assert.IsTrue(result, "Checkin failed!");
			Assert.IsTrue(checkin.Version > 0, "Version number is not greater than zero.");
		}

		[Test(Description = "Undo checkout from repository test.")]
		public void VaultUndoCheckoutTest()
		{
			CheckoutFile();
			
			VaultUndoCheckout undoCheckout = new VaultUndoCheckout();
			undoCheckout.BuildEngine = _buildEngine;
			undoCheckout.Username = _username;
			undoCheckout.Password = _password;
			undoCheckout.Url = _url;
			undoCheckout.Repository = _repository;
			undoCheckout.Path = _repositoryPath + _repositoryFile;
			undoCheckout.WorkingFolder = _workingFolder;
			
			Assert.IsNotNull(undoCheckout);
			bool result = undoCheckout.Execute();
			Assert.IsTrue(result, "UndoCheckout failed!");
		}

		[Test(Description = "Get file from repository test.")]
		public void VaultGetFileTest()
		{
			VaultGetFile getFile = new VaultGetFile();
			getFile.BuildEngine = _buildEngine;
			getFile.Username = _username;
			getFile.Password = _password;
			getFile.Url = _url;
			getFile.Repository = _repository;
			getFile.Path = _repositoryPath + _repositoryFile;
			getFile.WorkingFolder = _workingFolder;
			// Can use either WorkingFolder or Destination
			// getFile.Destination = _workingFolder;
			
			Assert.IsNotNull(getFile);
			bool result = getFile.Execute();
			Assert.IsTrue(result, "Get file failed!");

			bool fileExists = File.Exists(_workingFolder + "\\" + _repositoryFile);
			Assert.IsTrue(fileExists, "Get file failed, does not exist in working folder.");
		}

		[Test(Description = "Add file to repository test.")]
		public void VaultAddFileTest()
		{
			// Create test files to add
			File.Create(_workingFolder + "\\" + _newTestFile1);
			File.Create(_workingFolder + "\\" + _newTestFile2);
			
			// Add new test file to repository
			VaultAddFile addFile = new VaultAddFile();
			addFile.BuildEngine = _buildEngine;
			addFile.Username = _adminUsername;
			addFile.Password = _adminPassword;
			addFile.Url = _url;
			addFile.Repository = _repository;
			addFile.Path = _repositoryPath;
			addFile.Comment = _addFileComment;

			TaskItem[] items = new TaskItem[2];
			items[0] = new TaskItem(_workingFolder + "\\" + _newTestFile1);
			items[1] = new TaskItem(_workingFolder + "\\" + _newTestFile2);
			addFile.AddFileSet = items;
			
			Assert.IsNotNull(addFile);
			bool result = addFile.Execute();
			Assert.IsTrue(result, "Add file failed!");
		}

		#region Private Helper Methods

		private void CheckoutFile()
		{
			VaultCheckout checkout = new VaultCheckout();
			checkout.BuildEngine = _buildEngine;
			checkout.Username = _username;
			checkout.Password = _password;
			checkout.Url = _url;
			checkout.Repository = _repository;
			checkout.Path = _repositoryPath;
			checkout.Comment = _checkoutComment;
			checkout.WorkingFolder = _workingFolder;
			checkout.Execute();
		}

		private void UndoCheckout()
		{
			VaultUndoCheckout undoCheckout = new VaultUndoCheckout();
			undoCheckout.BuildEngine = new MockBuild();
			undoCheckout.Username = _username;
			undoCheckout.Password = _password;
			undoCheckout.Url = _url;
			undoCheckout.Repository = _repository;
			undoCheckout.Path = _repositoryPath;
			undoCheckout.Execute();
		}

		#endregion
	}
}
