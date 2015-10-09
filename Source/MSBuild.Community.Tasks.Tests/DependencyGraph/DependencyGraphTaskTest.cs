#region Copyright © 2007 Eric Lemes de Godoy Cintra. All rights reserved.
/*
Copyright © 2014 Eric Lemes de Godoy Cintra. All rights reserved.

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

using Microsoft.Build.Framework;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MSBuild.Community.Tasks.Tests.DependencyGraph
{

    [TestFixture]
    public class DependencyGraphTaskTest
    {
        [Test]
        public void SingleFileRunTest()
        {
            var taskItemStub = MockRepository.GenerateStub<ITaskItem>();
            taskItemStub.ItemSpec = Path.Combine(TaskUtility.GetProjectRootDirectory(true), @"Source\MSBuild.Community.Tasks.Tests\DependencyGraph\MSBuild.Community.Tasks.csproj");
            taskItemStub.Stub(ti => ti.GetMetadata("FullPath")).Return(taskItemStub.ItemSpec);

            var buildEngineStub = MockRepository.GenerateStub<IBuildEngine>();

            MSBuild.Community.Tasks.DependencyGraph.DependencyGraph task = new Tasks.DependencyGraph.DependencyGraph();
            task.BuildEngine = buildEngineStub;
            task.InputFiles = new ITaskItem[] { taskItemStub };                        
            task.OutputFile = "output.txt";
            task.Execute();

            string expectedOutput = GetFileContent(Path.Combine(TaskUtility.GetProjectRootDirectory(true), @"Source\MSBuild.Community.Tasks.Tests\DependencyGraph\outputgraph1.txt"));
            string taskOutput = GetFileContent(task.OutputFile);

            Assert.IsNotNullOrEmpty(taskOutput);
            Assert.AreEqual(expectedOutput, taskOutput);                       
        }

        [Test]
        public void SingleFileRunWithFilterTest()
        {
            var taskItemStub = MockRepository.GenerateStub<ITaskItem>();
            taskItemStub.ItemSpec = Path.Combine(TaskUtility.GetProjectRootDirectory(true), @"Source\MSBuild.Community.Tasks.Tests\DependencyGraph\MSBuild.Community.Tasks.csproj");
            taskItemStub.Stub(ti => ti.GetMetadata("FullPath")).Return(taskItemStub.ItemSpec);

            var filterItemStub1 = MockRepository.GenerateStub<ITaskItem>();
            filterItemStub1.ItemSpec = @"System";

            var filterItemStub2 = MockRepository.GenerateStub<ITaskItem>();
            filterItemStub2.ItemSpec = @"Microsoft\.";

            var buildEngineStub = MockRepository.GenerateStub<IBuildEngine>();

            MSBuild.Community.Tasks.DependencyGraph.DependencyGraph task = new Tasks.DependencyGraph.DependencyGraph();
            task.BuildEngine = buildEngineStub;
            task.InputFiles = new ITaskItem[] { taskItemStub };
            task.Filters = new ITaskItem[] { filterItemStub1, filterItemStub2 };
            task.OutputFile = "output.txt";
            task.Execute();

            string expectedOutput = GetFileContent(Path.Combine(TaskUtility.GetProjectRootDirectory(true), @"Source\MSBuild.Community.Tasks.Tests\DependencyGraph\outputgraph2.txt"));
            string taskOutput = GetFileContent(task.OutputFile);

            Assert.IsNotNullOrEmpty(taskOutput);
            Assert.AreEqual(expectedOutput, taskOutput);
        }

        [Test]
        public void TestWithoutInputFiles()
        {
            var buildEngineStub = MockRepository.GenerateStub<IBuildEngine>();

            MSBuild.Community.Tasks.DependencyGraph.DependencyGraph task = new Tasks.DependencyGraph.DependencyGraph();
            task.BuildEngine = buildEngineStub;
            task.Execute();
        }

        private string GetFileContent(string fileName)
        {
            StreamReader rd = new StreamReader(new FileStream(fileName, FileMode.Open, FileAccess.Read));
            using (rd)
            {
                return rd.ReadToEnd();
            }            
        }
    }
}
