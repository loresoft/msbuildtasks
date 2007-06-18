// $Id$
using System;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.Win32;
using MSBuild.Community.Tasks.Services;

namespace MSBuild.Community.Tasks.Oracle
{
    /// <summary>
    /// Defines a database host within the Oracle TNSNAMES.ORA file.
    /// </summary>
    /// <example>Adding an entry to a specific file.
    /// <code><![CDATA[ <AddTnsName TnsNamesFile="c:\oracle\network\admin\tnsnames.ora" EntryName="northwind.world" EntryText="(DESCRIPTION = (ADDRESS_LIST = (ADDRESS = (PROTOCOL = TCP)(HOST = northwinddb01)(PORT = 1521))) (CONNECT_DATA = (SERVICE_NAME = northwind.world)))"  /> ]]>
    /// </code>
    /// </example>
    public class AddTnsName : Task
    {
        private const string ORACLE_REGISTRY = @"SOFTWARE\ORACLE";
        private const string TNSNAMES_PATH = @"NETWORK\ADMIN\tnsnames.ora";
        private IRegistry registry;
        private IFilesSystem fileSystem;

        /// <summary>
        /// Creates a new instance of the AddTnsName task using dependency injection.
        /// </summary>
        /// <param name="registry">A service that provides access to the Windows registry.</param>
        /// <param name="fileSystem">A service that provides access to the file system.</param>
        public AddTnsName(IRegistry registry, IFilesSystem fileSystem)
        {
            this.registry = registry;
            this.fileSystem = fileSystem;
        }

        /// <summary>
        /// Creates a new instance of the AddTnsName task using the default system services.
        /// </summary>
        public AddTnsName() : this(new Win32Registry(), new FileSystem()) {}

        ///<summary>
        ///When overridden in a derived class, executes the task.
        ///</summary>
        ///
        ///<returns>
        ///true if the task successfully executed; otherwise, false.
        ///</returns>
        public override bool Execute()
        {
            modifiedFile = GetEffectivePathToTnsNamesFile();
            originalFileText = fileSystem.ReadTextFromFile(modifiedFile);
            
            TnsParser parser = new TnsParser(originalFileText);
            TnsEntry targetEntry = parser.FindEntry(entryName);
            if (targetEntry == null)
            {
                // append entry definition to the beginning of the file
                modifiedFileText = String.Format("{0} = {1}{2}{3}", entryName, entryText, Environment.NewLine, originalFileText);
                Log.LogMessage(Properties.Resources.TnsnameAdded, entryName, modifiedFile);
            }
            else
            {
                if (!allowUpdates)
                {
                    Log.LogError(Properties.Resources.TnsnameUpdateAborted, entryName, modifiedFile);
                    return false;
                }
                // replace the entry definition within the file
                string beforeEntry = originalFileText.Substring(0, targetEntry.StartPosition);
                string afterEntry = originalFileText.Substring(targetEntry.StartPosition + targetEntry.Length);
                modifiedFileText = String.Format("{0}{1} = {2}{3}", beforeEntry, entryName, entryText, afterEntry);
                Log.LogMessage(Properties.Resources.TnsnameUpdated, entryName, modifiedFile);
            }
            fileSystem.WriteTextToFile(modifiedFile, modifiedFileText);
            return true;
        }

        /// <summary>
        /// The path to a specific TNSNAMES.ORA file to update.
        /// </summary>
        public string TnsNamesFile
        {
            set { tnsNamesFile = value; }
        }

        /// <summary>
        /// The contents of the TNSNAMES.ORA file before any changes are made.
        /// </summary>
        [Output]
        public string OriginalFileText
        {
            get { return originalFileText; }
        }

        /// <summary>
        /// The path to the TNSNAMES.ORA that was used by task.
        /// </summary>
        [Output]
        public string ModifiedFile
        {
            get { return modifiedFile; }
        }

        /// <summary>
        /// The name of the host entry to add.
        /// </summary>
        [Required]
        public string EntryName
        {
            set { entryName = value; }
        }

        /// <summary>
        /// The contents of the TNSNAMES.ORA file after the task executes.
        /// </summary>
        [Output]
        public string ModifiedFileText
        {
            get { return modifiedFileText; }
        }

        /// <summary>
        /// The definition of the host entry to add.
        /// </summary>
        public string EntryText
        {
            set { entryText = value; }
        }

        /// <summary>
        /// When true, the task will update an existing entry with <see cref="EntryName"/>. 
        /// If false, the task will fail if <see cref="EntryName"/> already exists.
        /// </summary>
        public bool AllowUpdates
        {
            set { allowUpdates = value; }
        }

        /// <summary>
        /// Determines which TNSNAMES.ORA file to update based on task input and the current system environment.
        /// </summary>
        /// <returns>The path of the TNSNAMES.ORA file that will be used by the task.</returns>
        /// <exclude />
        public string GetEffectivePathToTnsNamesFile()
        {
            if (!String.IsNullOrEmpty(tnsNamesFile))
            {
                return tnsNamesFile;
            }
            //TODO: Implement Win32Registry to support locating the TNSNAMES.ORA file in the current Oracle home
            string[] oracleHomes = registry.GetSubKeys(RegistryHive.LocalMachine, ORACLE_REGISTRY);
            foreach(string home in oracleHomes)
            {
                string homePathKey = String.Format(@"HKEY_LOCAL_MACHINE\{1}\{0}\ORACLE_HOME", home, ORACLE_REGISTRY);
                string homePath = registry.GetValue(homePathKey) as string;
                string tnsNamesPath = Path.Combine(homePath, TNSNAMES_PATH);
                if (fileSystem.FileExists(tnsNamesPath))
                {
                    return tnsNamesPath;
                }

            }
            return tnsNamesFile;
        }

        private string tnsNamesFile;
        private string originalFileText;
        private string modifiedFile;
        private string entryName;
        private string modifiedFileText;
        private string entryText;
        private bool allowUpdates;
    }
}
