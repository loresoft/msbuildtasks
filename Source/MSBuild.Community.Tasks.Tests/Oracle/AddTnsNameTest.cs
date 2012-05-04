

using System;
using Microsoft.Win32;
using MSBuild.Community.Tasks.Oracle;
using MSBuild.Community.Tasks.Services;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using Rhino.Mocks.Impl;

namespace MSBuild.Community.Tasks.Tests.Oracle
{
    [TestFixture]
    public class AddTnsNameTest
    {
        private AddTnsName task;
        private MockRepository mocks;
        private IRegistry registry;
        private IFilesSystem fileSystem;

        [SetUp]
        public void Setup()
        {
            //RhinoMocks.Logger = new TextWriterExpectationLogger(Console.Out);
            mocks = new MockRepository();
            registry = mocks.DynamicMock<IRegistry>();
            fileSystem = mocks.DynamicMock<IFilesSystem>();

            task = new AddTnsName(registry, fileSystem);
            task.BuildEngine = new MockBuild();
        }

        [Test]
        public void A_specific_tnsnames_file_should_be_used_when_the_TnsNamesFile_property_is_set()
        {
            string specifiedFile = @"c:\data\tnsnames.ora";
            task.TnsNamesFile = specifiedFile;
            string fileBeingUpdated = task.GetEffectivePathToTnsNamesFile();

            Assert.AreEqual(specifiedFile, fileBeingUpdated);
        }

        [Test]
        public void The_tnsnames_file_in_the_oracle_home_should_be_used_when_the_TnsNamesFile_property_is_not_set()
        {
            string lastUsedOracleHome = @"c:\oracle\product\10";
            string tnsnamesFileInOracleHome = @"c:\oracle\product\10\NETWORK\ADMIN\tnsnames.ora";

            string[] oracleSubKeys = new string[] {"OraHome"};
            using (mocks.Record())
            {
                SetupResult.For(registry.GetSubKeys(RegistryHive.LocalMachine, @"SOFTWARE\ORACLE")).Return(oracleSubKeys);
                SetupResult.For(registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\ORACLE\OraHome", "ORACLE_HOME")).Return(lastUsedOracleHome);
                SetupResult.For(fileSystem.FileExists(tnsnamesFileInOracleHome)).Return(true);
            }
            string fileBeingUpdated = task.GetEffectivePathToTnsNamesFile();
            Assert.AreEqual(tnsnamesFileInOracleHome, fileBeingUpdated);
        }

        [Test]
        public void The_task_should_fail_when_no_TnsNamesFile_can_be_found()
        {
            using (mocks.Record())
            {
                SetupResult.For(registry.GetSubKeys(RegistryHive.LocalMachine, @"SOFTWARE\ORACLE")).Return(new string[0]);
            }
            bool success = task.Execute();
            Assert.IsFalse(success, "Task should have failed when no file could be found.");
        }

        [Test]
        public void ModifiedFile_is_populated()
        {
            string specifiedFile = @"c:\data\tnsnames.ora";
            task.TnsNamesFile = specifiedFile;
            task.EntryName = "foo.world";
            using (mocks.Record())
            {
                SetupResult.For(fileSystem.ReadTextFromFile(specifiedFile)).Return(SAMPLE_FILE);
            }

            Assert.IsTrue(task.Execute());
            Assert.AreEqual(specifiedFile, task.ModifiedFile);
        }

        [Test]
        public void OriginalFileText_is_populated()
        {
            string specifiedFile = @"c:\data\tnsnames.ora";
            task.TnsNamesFile = specifiedFile;
            task.EntryName = "foo.world";

            using (mocks.Record())
            {
                Expect.Call(fileSystem.ReadTextFromFile(specifiedFile)).Return(SAMPLE_FILE);
            }

            Assert.IsTrue(task.Execute());
            mocks.VerifyAll();
            Assert.AreEqual(SAMPLE_FILE, task.OriginalFileText);
        }

        [Test]
        public void A_new_entry_is_added_when_EntryName_is_not_already_in_the_file()
        {
            string specifiedFile = @"c:\data\tnsnames.ora";
            task.TnsNamesFile = specifiedFile;
            task.EntryName = "eastwind.world";
            task.EntryText = "(DESCRIPTION = (ADDRESS_LIST = (ADDRESS = (PROTOCOL = TCP)(HOST = eastwinddb)(PORT = 1521))) (CONNECT_DATA = (SERVICE_NAME = southwind.world)))";

            using (mocks.Record())
            {
                SetupResult.For(fileSystem.ReadTextFromFile(specifiedFile)).Return(SAMPLE_FILE);
            }
            Assert.IsTrue(task.Execute());
            Assert.AreEqual(ADDED_FILE, task.ModifiedFileText);
        }

        [Test]
        public void An_existing_entry_is_updated_when_EntryName_is_already_in_the_file_and_updates_are_allowed()
        {
            string specifiedFile = @"c:\data\tnsnames.ora";
            task.TnsNamesFile = specifiedFile;
            task.AllowUpdates = true;
            task.EntryName = "SOUTHWIND.WORLD";
            task.EntryText = "(DESCRIPTION =    (ADDRESS_LIST =      (ADDRESS = (PROTOCOL = TCP)(HOST = newsouthwinddb01)(PORT = 1521))    )    (CONNECT_DATA =      (SERVICE_NAME = southwind.world)    )  )";

            using (mocks.Record())
            {
                SetupResult.For(fileSystem.ReadTextFromFile(specifiedFile)).Return(SAMPLE_FILE);
            }
            Assert.IsTrue(task.Execute());
            Assert.AreEqual(MODIFIED_FILE, task.ModifiedFileText);
        }

        [Test]
        public void The_task_fails_when_EntryName_is_already_in_the_file_and_allowupdates_is_false_or_not_specified()
        {
            string specifiedFile = @"c:\data\tnsnames.ora";
            task.TnsNamesFile = specifiedFile;
            task.EntryName = "SOUTHWIND.WORLD";
            task.EntryText = "(DESCRIPTION =    (ADDRESS_LIST =      (ADDRESS = (PROTOCOL = TCP)(HOST = newsouthwinddb01)(PORT = 1521))    )    (CONNECT_DATA =      (SERVICE_NAME = southwind.world)    )  )";

            using (mocks.Record())
            {
                SetupResult.For(fileSystem.ReadTextFromFile(specifiedFile)).Return(SAMPLE_FILE);
            }
            Assert.IsFalse(task.Execute());
        }

        [Test]
        public void The_original_file_is_replaced_when_an_entry_is_added_or_updated()
        {
            string specifiedFile = @"c:\data\tnsnames.ora";
            task.TnsNamesFile = specifiedFile;
            task.EntryName = "foo.world";
            using (mocks.Record())
            {
                SetupResult.For(fileSystem.ReadTextFromFile(specifiedFile)).Return(SAMPLE_FILE);
                fileSystem.WriteTextToFile(specifiedFile, SAMPLE_FILE);
                LastCall.Constraints(Rhino.Mocks.Constraints.Is.Equal(specifiedFile), Rhino.Mocks.Constraints.Is.Anything());
            }

            Assert.IsTrue(task.Execute());
            mocks.VerifyAll();
        }


        public const string SAMPLE_FILE = @"
NORTHWIND.WORLD =
  (DESCRIPTION =
    (ADDRESS_LIST =
      (ADDRESS = (PROTOCOL = TCP)(HOST = northwinddb)(PORT = 1521))
    )
    (CONNECT_DATA =
      (SERVICE_NAME = northwind.world)
    )
  )

SOUTHWIND.WORLD =  (DESCRIPTION =    (ADDRESS_LIST =      (ADDRESS = (PROTOCOL = TCP)(HOST = southwinddb)(PORT = 1521))    )    (CONNECT_DATA =      (SERVICE_NAME = southwind.world)    )  )

westwind.world =
(DESCRIPTION =
(ADDRESS_LIST =
  (ADDRESS = (PROTOCOL = TCP)(HOST = westwinddb)(PORT = 1521))
)
(CONNECT_DATA =
  (SERVICE_NAME = westwind.world)
)
)
";
        private const string MODIFIED_FILE = @"
NORTHWIND.WORLD =
  (DESCRIPTION =
    (ADDRESS_LIST =
      (ADDRESS = (PROTOCOL = TCP)(HOST = northwinddb)(PORT = 1521))
    )
    (CONNECT_DATA =
      (SERVICE_NAME = northwind.world)
    )
  )

SOUTHWIND.WORLD = (DESCRIPTION =    (ADDRESS_LIST =      (ADDRESS = (PROTOCOL = TCP)(HOST = newsouthwinddb01)(PORT = 1521))    )    (CONNECT_DATA =      (SERVICE_NAME = southwind.world)    )  )

westwind.world =
(DESCRIPTION =
(ADDRESS_LIST =
  (ADDRESS = (PROTOCOL = TCP)(HOST = westwinddb)(PORT = 1521))
)
(CONNECT_DATA =
  (SERVICE_NAME = westwind.world)
)
)
";

        private const string ADDED_FILE = @"eastwind.world = (DESCRIPTION = (ADDRESS_LIST = (ADDRESS = (PROTOCOL = TCP)(HOST = eastwinddb)(PORT = 1521))) (CONNECT_DATA = (SERVICE_NAME = southwind.world)))

NORTHWIND.WORLD =
  (DESCRIPTION =
    (ADDRESS_LIST =
      (ADDRESS = (PROTOCOL = TCP)(HOST = northwinddb)(PORT = 1521))
    )
    (CONNECT_DATA =
      (SERVICE_NAME = northwind.world)
    )
  )

SOUTHWIND.WORLD =  (DESCRIPTION =    (ADDRESS_LIST =      (ADDRESS = (PROTOCOL = TCP)(HOST = southwinddb)(PORT = 1521))    )    (CONNECT_DATA =      (SERVICE_NAME = southwind.world)    )  )

westwind.world =
(DESCRIPTION =
(ADDRESS_LIST =
  (ADDRESS = (PROTOCOL = TCP)(HOST = westwinddb)(PORT = 1521))
)
(CONNECT_DATA =
  (SERVICE_NAME = westwind.world)
)
)
";
    }
}