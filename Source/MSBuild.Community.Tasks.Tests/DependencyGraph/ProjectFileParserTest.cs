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

using MSBuild.Community.Tasks.DependencyGraph;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MSBuild.Community.Tasks.Tests.DependencyGraph
{
    [TestFixture]
    public class ProjectFileParserTest
    {
        public Stream GetStreamForFile(string fileName)
        {
            FileStream fs = new FileStream(Path.Combine(TaskUtility.GetProjectRootDirectory(true), @"Source\MSBuild.Community.Tasks.Tests\DependencyGraph\" + fileName), FileMode.Open, FileAccess.Read);
            return fs;
        }

        [Test]
        public void GetAssemblyNameVS2013ProjectTest()
        {           
            using (Stream s = GetStreamForFile("MSBuild.Community.Tasks.csproj"))
            {
                ProjectFileParser p = new ProjectFileParser(s);
                Assert.AreEqual("MSBuild.Community.Tasks", p.GetAssemblyName());
            }
        }

        [Test]
        public void GetAssemblyNameVS2008ProjectTest()
        {
            using (Stream s = GetStreamForFile("DBInfo.CodeGen.csproj"))
            {
                ProjectFileParser p = new ProjectFileParser(s);
                Assert.AreEqual("DBInfo.CodeGen", p.GetAssemblyName());
            }
        }

        [Test]
        public void GetReferencesVS2013Test()
        {
            using (Stream s = GetStreamForFile("MSBuild.Community.Tasks.csproj"))
            {
                ProjectFileParser p = new ProjectFileParser(s);
                Assert.AreEqual("Ionic.Zip.Reduced, Version=1.9.1.8, Culture=neutral, PublicKeyToken=edbe51ad942a3f5c, processorArchitecture=MSIL", p.GetAssemblyReferences().ToList()[0].Include);
                Assert.AreEqual(@"..\packages\DotNetZip.Reduced.1.9.1.8\lib\net20\Ionic.Zip.Reduced.dll", p.GetAssemblyReferences().ToList()[0].HintPath);
                Assert.AreEqual("Microsoft.Build.Framework", p.GetAssemblyReferences().ToList()[1].Include);
            }
        }

        [Test]
        public void GetReferencesVS2008Test()
        {
            using (Stream s = GetStreamForFile("DBInfo.CodeGen.csproj"))
            {
                ProjectFileParser p = new ProjectFileParser(s);
                Assert.AreEqual("System", p.GetAssemblyReferences().ToList()[0].Include);
                Assert.AreEqual("System.Core", p.GetAssemblyReferences().ToList()[1].Include);                
            }
        }

        [Test]
        public void GetAssemblyNameFromFullNameTest()
        {
            using (Stream s = GetStreamForFile("MSBuild.Community.Tasks.csproj"))
            {
                Assert.AreEqual("Ionic.Zip.Reduced", ProjectFileParser.GetAssemblyNameFromFullName("Ionic.Zip.Reduced, Version=1.9.1.8, Culture=neutral, PublicKeyToken=edbe51ad942a3f5c, processorArchitecture=MSIL"));
            }
        }

    }
}
