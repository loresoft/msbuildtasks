#region Copyright © 2006 Andy Johns. All rights reserved.
/*
Copyright © 2006 Andy Johns. All rights reserved.

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

//$Id$

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NUnit.Framework;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace MSBuild.Community.Tasks.Tests
{
    [TestFixture]
    public class GetSolutionProjectsTest
    {
        [Test(Description = "Tests invalid solution filename")]
        public void SolutionNotFound()
        {
            GetSolutionProjects task = new GetSolutionProjects();
            task.BuildEngine = new MockBuild();

            task.Solution = "filenamethatcannotexist.sln";
            bool retVal = task.Execute();

            Assert.IsFalse(retVal);
        }

        [Test(Description="Tests getting projects from solution")]
        public void GetSolutionProjects()
        {
            GetSolutionProjects task = new GetSolutionProjects();
            task.BuildEngine = new MockBuild();
            task.Solution = Path.Combine(TaskUtility.getProjectRootDirectory(true), @"Source\MSBuild.Community.Tasks.Tests\Solution\TestSolution.sln");

            Assert.IsTrue(task.Execute());

            ITaskItem[] items = task.Output;
            string expectedFileName;
            string actualFileName;
            string expectedProjectName;
            string actualProjectName;
            
            Assert.AreEqual(4, items.Length);
            for (int i = 0; i < 3; i++)
            {
                expectedFileName = string.Format("TestProject{0}\\TestProject{0}.csproj", i + 1);
                actualFileName = items[i].ItemSpec;
                expectedProjectName = string.Format("TestProject{0}", i + 1);
                actualProjectName = items[i].GetMetadata("ProjectName");

                Assert.AreEqual(expectedFileName, actualFileName);
                Assert.AreEqual(expectedProjectName, actualProjectName);
            }

            //Added test to check handling of projects with spaces in the name
            expectedFileName = "Test Project4\\Test Project4.csproj";
            actualFileName =    items[3].ItemSpec;
            expectedProjectName = "Test Project4";
            actualProjectName = items[3].GetMetadata("ProjectName");

            Assert.AreEqual(expectedFileName, actualFileName);
            Assert.AreEqual(expectedProjectName, actualProjectName);

            //Added test for reading the GUID attribute
            string expectedGUID = "{D6CFCEDB-15CF-4EB6-87FB-8A5113727718}";
            string actualGUID = items[3].GetMetadata("ProjectGUID");
            Assert.AreEqual(expectedGUID, actualGUID);
        }


    }
}
