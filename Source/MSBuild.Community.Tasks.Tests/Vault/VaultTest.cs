// $Id$
// Copyright © 2006 Douglas Rohm

using System;
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
		private string _url;
		private string _repository;
		private string _path;
		private string _diskFile;
		private string _checkoutComment;
		private string _checkinComment;
		private string _workingFolder;
		private VaultCheckout _checkout;
		private VaultCheckin _checkin;

		[TestFixtureSetUp]
		public void FixtureInit()
		{
			_username = "msbuild";
			_password = "m$build";
			_url = "http://fenway";
			_repository = "DougRohm";
			_path = "$/Temp/VaultTestFile.txt";
			_diskFile = @"C:\Development\Temp\VaultTestFile.txt";
			_checkoutComment = "Vault unit test for Checkout task.";
			_checkinComment = "Vault unit test for Checkin task.";

			MockBuild buildEngine = new MockBuild();
			_workingFolder = TaskUtility.makeTestDirectory(buildEngine);
			
			// Initialize Checkout object
			_checkout = new VaultCheckout();
			_checkout.BuildEngine = buildEngine;
			_checkout.Username = _username;
			_checkout.Password = _password;
			_checkout.Url = _url;
			_checkout.Repository = _repository;
			_checkout.Path = _path;
			_checkout.Comment = _checkoutComment;
			_checkout.WorkingFolder = _workingFolder;
			try
			{
				_checkout.Login();
			}
			catch (Exception)
			{
				Assert.Ignore(@"The Vault server was not found and is required to run this test fixture.");
			}
			
			// Initialize Checkin object
			_checkin = new VaultCheckin();
			_checkin.BuildEngine = buildEngine;
			_checkin.Username = _username;
			_checkin.Password = _password;
			_checkin.Url = _url;
			_checkin.Repository = _repository;
			_checkin.Path = _path;
			_checkin.DiskFile = _diskFile;
			_checkin.Comment = _checkinComment;
			_checkin.WorkingFolder = _workingFolder;
		}
		
		[Test(Description = "Checkout from repository")]
		public void VaultCheckoutTest()
		{
			Assert.IsNotNull(_checkout);
			bool result = _checkout.Execute();
			Assert.IsTrue(result, "Checkout failed!");
			Assert.IsTrue(_checkout.Version > 0, "Version number is not greater than zero.");
		}
		
		[Test(Description = "Checkin to repository")]
		public void VaultCheckinTest()
		{
			Assert.IsNotNull(_checkin);
			bool result = _checkin.Execute();
			Assert.IsTrue(result, "Checkin failed!");
			Assert.IsTrue(_checkin.Version > 0, "Version number is not greater than zero.");
		}
	}
}
