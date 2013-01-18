#region Copyright © 2006 Doug Ramirez. All rights reserved.
/*
 Copyright © 2006 Doug Ramirez. All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions
are met:

1. Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright
   notice, this list of conditions and the following disclaimer in the
   documentation and/or other materials provided with the distribution.
3. The name of the author may not be used to endorse or promote products
   derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE AUTHOR "AS IS" AND ANY EXPRESS OR
IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 
*/
#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSBuild.Community.Tasks.Mks;

namespace MSBuild.Community.Tasks.Test.Mks
{
	/*---------------------------------------------------------------------------------------------------------------*/
	/// <summary>
	///		Unit test for custom MSBuild task that calls the MKS Create Sandbox command.
	/// </summary>
	/*---------------------------------------------------------------------------------------------------------------*/
	[TestClass]
	public class MksCreateSandboxTest
	{
		[TestMethod]
		[Priority(0)]
		public void MksCreateSandboxExecute()
		{
			MksCreateSandbox mksCreateSandbox = new MksCreateSandbox();
			mksCreateSandbox.BuildEngine = new MockBuild();

			Assert.IsNotNull(mksCreateSandbox);

			mksCreateSandbox.Directory = @"C:\data\OpenSource\MSBuild\Community\MSBuild.Community.Tasks\Source\MSBuild.Community.Tasks.Test\MksTemp";
			mksCreateSandbox.Populate = true;
			mksCreateSandbox.Project = @"f:\MKSRCS\ADG\.Net\OpenSource\MSBuild\Community\MSBuild.Community.Tasks\Source\MSBuild.Community.Tasks\Mks\project.pj";
			mksCreateSandbox.Recurse = true;

			Assert.IsTrue(mksCreateSandbox.CommandText == @"C:\Program Files\MKS\IntegrityClient\bin\si.exe createsandbox --populate --recurse --project=f:\MKSRCS\ADG\.Net\OpenSource\MSBuild\Community\MSBuild.Community.Tasks\Source\MSBuild.Community.Tasks\Mks\project.pj C:\data\OpenSource\MSBuild\Community\MSBuild.Community.Tasks\Source\MSBuild.Community.Tasks.Test\MksTemp");

			// If an MKS environment is available then uncomment the execution and assertion.
			// bool result = mksCreateSandbox.Execute();
			// Assert.IsTrue(result);
		}
	}
}
