

namespace MSBuild.Community.Tasks.Tests.JavaScript
{
    #region Imports

    using System.IO;
    using MSBuild.Community.Tasks.JavaScript;

    #endregion

    /// <summary>
    /// Summary description for JSCompressTest
    /// </summary>
    [ global::NUnit.Framework.TestFixture ]
    public class JSCompressTest
    {
        private const string testFileName = "test.js";

        private string testDirectory;
        private MockBuild buildEngine;

        [ global::NUnit.Framework.SetUp ]
        public void Init()
        {
            buildEngine = new MockBuild();
            testDirectory = TaskUtility.makeTestDirectory(buildEngine);
        }

        [ global::NUnit.Framework.TearDown ]
        public void Dispose()
        {
            string path = Path.Combine(testDirectory, testFileName);
            if (File.Exists(path))
                File.Delete(path);
        }

        [ global::NUnit.Framework.Test(Description = "Compress JavaScript files") ]
        public void CompressOne()
        {
            JSCompress task = new JSCompress();
            task.BuildEngine = buildEngine;
            string testFilePath = Path.Combine(testDirectory, testFileName);

            File.WriteAllText(testFilePath,
                @"
                // is.js

                // (c) 2001 Douglas Crockford
                // 2001 June 3


                // is

                // The -is- object is used to identify the browser.  Every browser edition
                // identifies itself, but there is no standard way of doing it, and some of
                // the identification is deceptive. This is because the authors of web
                // browsers are liars. For example, Microsoft's IE browsers claim to be
                // Mozilla 4. Netscape 6 claims to be version 5.

                var is = {
                    ie:      navigator.appName == 'Microsoft Internet Explorer',
                    java:    navigator.javaEnabled(),
                    ns:      navigator.appName == 'Netscape',
                    ua:      navigator.userAgent.toLowerCase(),
                    version: parseFloat(navigator.appVersion.substr(21)) ||
                             parseFloat(navigator.appVersion),
                    win:     navigator.platform == 'Win32'
                }
                is.mac = is.ua.indexOf('mac') >= 0;
                if (is.ua.indexOf('opera') >= 0) {
                    is.ie = is.ns = false;
                    is.opera = true;
                }
                if (is.ua.indexOf('gecko') >= 0) {
                    is.ie = is.ns = false;
                    is.gecko = true;
                }
                ");

            task.Files = TaskUtility.StringArrayToItemArray(new string[] {testFilePath});

            global::NUnit.Framework.Assert.IsTrue(task.Execute(), "Execute failed.");
            global::NUnit.Framework.Assert.IsTrue(File.Exists(testFilePath), "File not found.");

            string result = File.ReadAllText(testFilePath);

            string expected =
                "\nvar is={ie:navigator.appName=='Microsoft Internet Explorer',java:navigator.javaEnabled(),ns:navigator.appName=='Netscape',ua:navigator.userAgent.toLowerCase(),version:parseFloat(navigator.appVersion.substr(21))||parseFloat(navigator.appVersion),win:navigator.platform=='Win32'}" +
                "\nis.mac=is.ua.indexOf('mac')>=0;if(is.ua.indexOf('opera')>=0){is.ie=is.ns=false;is.opera=true;}" +
                "\nif(is.ua.indexOf('gecko')>=0){is.ie=is.ns=false;is.gecko=true;}";
            
            global::NUnit.Framework.Assert.AreEqual(expected, result, "Compression problem.");
        }
    }
}
