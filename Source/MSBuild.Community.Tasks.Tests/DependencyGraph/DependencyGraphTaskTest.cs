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

        private class Test
        {
            private const string RootDir = @"Source\MSBuild.Community.Tasks.Tests\DependencyGraph\";

            private readonly ITaskItem[] _taskItems;
            private readonly ITaskItem[] _excludeRefereces;
            private readonly  string _outputFile;
            private IBuildEngine _buildEngineStub;

            public Test(string include, string[] excludeReferences)
            {
                var taskItem = MockRepository.GenerateStub<ITaskItem>();
                taskItem.ItemSpec = Path.Combine(TaskUtility.GetProjectRootDirectory(true), RootDir + include);
                taskItem.Stub(ti => ti.GetMetadata("FullPath")).Return(taskItem.ItemSpec);
                _taskItems = new[] { taskItem };

                _excludeRefereces = (excludeReferences ?? new string[0])
                    .Select(filter =>
                    {
                        var item = MockRepository.GenerateStub<ITaskItem>();
                        item.ItemSpec = filter;
                        return item;
                    })
                    .ToArray();

                _outputFile = Path.GetTempFileName();

                _buildEngineStub = MockRepository.GenerateStub<IBuildEngine>();
            }

            public bool Execute()
            {
                Tasks.DependencyGraph.DependencyGraph task = new Tasks.DependencyGraph.DependencyGraph();
                task.BuildEngine = _buildEngineStub;
                task.InputFiles = _taskItems;
                task.ExcludeReferences = _excludeRefereces;
                task.OutputFile = _outputFile;

                return task.Execute();
            }

            public void CheckOutput(string ethalon, bool deleteOutput)
            {
                string expectedOutput = GetFileContent(Path.Combine(TaskUtility.GetProjectRootDirectory(true), RootDir + ethalon));
                string taskOutput = GetFileContent(_outputFile);

                if (deleteOutput)
                    DeleteOutput();
                
                Assert.That(taskOutput, Is.Not.Null.And.Not.Empty);
                Assert.AreEqual(expectedOutput, taskOutput);
            }

            public bool DeleteOutput()
            {
                if (!File.Exists(_outputFile)) return false;

                File.Delete(_outputFile);
                return true;
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

        [Test]
        public void SingleFileRunTest()
        {
            var test = new Test("MSBuild.Community.Tasks.csproj", null);
            test.Execute();
            test.CheckOutput("outputgraph1.txt", true);
        }

        [Test]
        public void SingleFileRunWithFilterTest()
        {
            var test = new Test("MSBuild.Community.Tasks.csproj", new[] { @"System", @"Microsoft\." });
            test.Execute();
            test.CheckOutput("outputgraph2.txt", true);
        }

        [Test]
        public void Test01_ProjectReferenceDoesNotExist()
        {
            var test = new Test(@"Test01_ProjectReferenceDoesNotExist\Project01\Project01.csproj", null);

            Exception exception = null;
            bool result = true;
            try
            {
                test.DeleteOutput();
                result = test.Execute();
            }
            catch(Exception ex)
            {
                exception = ex;
            }

            Assert.That(!result, "Any error should lead to negative result");
            Assert.That(exception == null, "Exception {0} was not expected", exception);

            var fileExist = test.DeleteOutput();
            Assert.That(!fileExist, "In case of error there should not be file generated");
        }

        [Test]
        public void Test02_DifferentPathSameProject()
        {
            var test = new Test(@"Test02_DifferentRefPathSameProject\Project01\Project01.csproj", null);
            test.Execute();
            test.CheckOutput(@"Test02_DifferentRefPathSameProject\outputfile.txt", true);
        }

        [Test]
        public void Test03_EmptyDependicies()
        {
            var test = new Test(@"Test03_EmptyDependencies\Project01\Project01.csproj", new[] { @"System", @"Microsoft\." });
            test.Execute();
            test.CheckOutput(@"Test03_EmptyDependencies\outputfile.txt", true);
        }

        [Test]
        public void TestWithoutInputFiles()
        {
            var buildEngineStub = MockRepository.GenerateStub<IBuildEngine>();

            MSBuild.Community.Tasks.DependencyGraph.DependencyGraph task = new Tasks.DependencyGraph.DependencyGraph();
            task.BuildEngine = buildEngineStub;
            task.Execute();
        }
    }
}
