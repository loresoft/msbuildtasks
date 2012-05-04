

using MSBuild.Community.Tasks.Oracle;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests.Oracle
{
    [TestFixture]
    public class TnsParserTest
    {
        [Test]
        public void FindEntry_should_return_null_when_the_requested_entry_does_not_exist()
        {
            TnsParser parser = new TnsParser(AddTnsNameTest.SAMPLE_FILE);
            TnsEntry foundEntry = parser.FindEntry("eastwind.world");
            Assert.IsNull(foundEntry);
        }

        [Test]
        public void FindEntry_should_only_ecognize_entry_names_that_start_in_the_first_column_of_a_new_line()
        {
            string textThatIsInTheFileButNotAnEntryName = "CONNECT_DATA";
            StringAssert.Contains(textThatIsInTheFileButNotAnEntryName, AddTnsNameTest.SAMPLE_FILE);
            
            TnsParser parser = new TnsParser(AddTnsNameTest.SAMPLE_FILE);
            TnsEntry foundEntry = parser.FindEntry(textThatIsInTheFileButNotAnEntryName);
            Assert.IsNull(foundEntry);
        }

        [Test]
        public void FindEntry_should_return_position_of_the_requested_entry_when_it_does_exist()
        {
            string entryName = "SOUTHWIND.WORLD";
            int startPosition = AddTnsNameTest.SAMPLE_FILE.IndexOf(entryName);
            Assert.IsTrue(startPosition >=0, "Entry should exist in sample file.");

            TnsParser parser = new TnsParser(AddTnsNameTest.SAMPLE_FILE);
            TnsEntry foundEntry = parser.FindEntry(entryName);
            Assert.AreEqual(startPosition, foundEntry.StartPosition);
        }

        [Test]
        public void FindEntry_should_return_length_of_the_found_entry_definition()
        {
            string entryName = "SOUTHWIND.WORLD";
            const int LENGTH_OF_SOUTHWIND_DEFINITION = 189; // this corresponds to the text of SAMPLE_FILE

            int startPosition = AddTnsNameTest.SAMPLE_FILE.IndexOf(entryName);
            Assert.IsTrue(startPosition >= 0, "Entry should exist in sample file.");

            TnsParser parser = new TnsParser(AddTnsNameTest.SAMPLE_FILE);
            TnsEntry foundEntry = parser.FindEntry(entryName);
            Assert.AreEqual(LENGTH_OF_SOUTHWIND_DEFINITION, foundEntry.Length);
        }
    }
}