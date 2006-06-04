// $Id$

using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Tasks;
using Microsoft.Build.Utilities;
using Microsoft.Win32;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests
{
    public class TaskUtility
    {

        public static TaskItem[] StringArrayToItemArray(params string[] items)
        {
            TaskItem[] taskItems = new TaskItem[items.Length];

            for (int i = 0; i < items.Length; i++)
            {
                taskItems[i] = new TaskItem(items[i]);
            }
            return taskItems;
        }

        #region NUnit Environment Info
        /// <summary>
        /// Log NUnit environment information.
        /// </summary>
        /// <param name="log">The logger.</param>
        public static void logEnvironmentInfo(TaskLoggingHelper log)
        {
            log.LogMessage(MessageImportance.Low, "AssemblyName: {0}", AssemblyName);
            log.LogMessage(MessageImportance.Low, "AssemblyCodeBase: {0}", AssemblyCodeBase);
            log.LogMessage(MessageImportance.Low, "AssemblyDirectory: {0}", AssemblyDirectory);
            log.LogMessage(MessageImportance.Low, "IsInteractive: {0}", IsInteractive);
            log.LogMessage(MessageImportance.Low, "CalledInBuildDirectory: {0}", CalledInBuildDirectory);
            log.LogMessage(MessageImportance.Low, "TestDirectory: {0}", TestDirectory);

        }

        public static string AssemblyName
        {
            get { return Assembly.GetExecutingAssembly().FullName.Split(',')[0]; }
        }

        public static string AssemblyCodeBase
        {
            get { return Assembly.GetExecutingAssembly().CodeBase; }
        }

        public static string AssemblyDirectory
        {
            get { return new Uri(Path.GetDirectoryName(AssemblyCodeBase)).AbsolutePath; }
        }

        public static bool IsInteractive
        {
            get { return String.Equals(@"nunit-gui.exe", System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName); }
        }

        public static bool CalledInBuildDirectory
        {
            get { return (AssemblyCodeBase.IndexOf(@"/build/") != -1); }
        }

		public static string BuildConfiguration
		{
			get
			{
				if (AssemblyCodeBase.IndexOf(@"/debug/", StringComparison.InvariantCultureIgnoreCase) != -1) return @"Debug";
				if (AssemblyCodeBase.IndexOf(@"/release/", StringComparison.InvariantCultureIgnoreCase) != -1) return @"Release";
				return null;
			}
		}

		public static string TestDirectory
        {
            get { return Path.GetFullPath(Path.Combine(AssemblyDirectory, "../test/" + AssemblyName)); }
        }

        /// <summary>
        /// Create the directory <see cref="TestDirectory"/>.
        /// </summary>
        /// <param name="buildEngine">The build engine to use for the <see cref="MakeDir"/> task.</param>
        /// <returns>Returns the path name of the created directory.</returns>
        public static string makeTestDirectory(IBuildEngine buildEngine)
        {
            MakeDir makeDirTask = new MakeDir();
            makeDirTask.BuildEngine = buildEngine;
            string testDirectory = TaskUtility.TestDirectory;
            makeDirTask.Directories = StringArrayToItemArray(testDirectory);
            makeDirTask.Execute();
            return testDirectory;
        }

        /// <summary>
        /// Get the project root directory
        /// </summary>
        /// <param name="assertIgnoreOnFailure">Flag for ignoring the calling test case
        /// in case the project root directory cannot be found.</param>
        /// <returns>Return the project root directory or <c>null</c>
        /// if it cannot be determined.</returns>
        public static string getProjectRootDirectory(bool assertIgnoreOnFailure)
        {
            string testDir = TaskUtility.TestDirectory;
            string searchPath = (TaskUtility.CalledInBuildDirectory) ? @"build\" : @"source\";
            int foundIndex = testDir.IndexOf(searchPath, StringComparison.OrdinalIgnoreCase);
            if (foundIndex < 0)
            {
                if (assertIgnoreOnFailure)
                {
                    Assert.Ignore("Can't find \"{0}\" in \"{1}\"", searchPath, testDir);
                }
                else
                {
                    return null;
                }
            }

            return testDir.Substring(0, foundIndex);

        }
    	
    	/// <summary>
		/// Determines if IIS is installed on the machine.
    	/// </summary>
    	/// <param name="machineName">Machine to test if IIS is installed.</param>
    	/// <returns>True if installed, false if not.</returns>
    	public static bool isIISInstalled(string machineName)
    	{
    		bool iisExists = false;
			string iisRegKeyName = @"SYSTEM\CurrentControlSet\Services\W3SVC\Parameters";
			RegistryKey iisRegKey;
    		
			try
			{
				if (machineName == "localhost")
				{
					iisRegKey = Registry.LocalMachine.OpenSubKey(iisRegKeyName);
				}
				else
				{
					iisRegKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, machineName).OpenSubKey(iisRegKeyName);
				}

				if (iisRegKey != null)
				{
					object oValue1 = iisRegKey.GetValue("MajorVersion");
					object oValue2 = iisRegKey.GetValue("MinorVersion");
					if (oValue1 != null && oValue2 != null)
					{
						iisExists = true;
					}
					
					iisRegKey.Close();
				}
			}
			catch (Exception e)
			{
				iisExists = false;
			}

			return iisExists;
		}
    	
        #endregion NUnit Environment Info
    }
}
