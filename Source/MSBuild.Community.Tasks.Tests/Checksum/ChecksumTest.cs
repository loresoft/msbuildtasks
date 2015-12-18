using NUnit.Framework;
using System.IO;

namespace MSBuild.Community.Tasks.Tests
{
  [TestFixture]
  class ChecksumTest
  {

    private string testDirectory;
    private string[] inputFiles;

    public ChecksumTest()
    {
    }

    [SetUp]
    public void Setup()
    {
      MockBuild buildEngine = new MockBuild();
      testDirectory = Path.Combine(TaskUtility.makeTestDirectory( buildEngine ), "ChecksumTest");
      Directory.CreateDirectory( testDirectory );
      // Create test file
      inputFiles = new string[2];
      inputFiles[0] = Path.Combine( testDirectory, "testFile1.txt" );
      inputFiles[1] = Path.Combine( testDirectory, "testFile2.txt" );
      File.WriteAllText( inputFiles[0], "foo\nbar" );
      File.WriteAllText( inputFiles[1], "test2\ntest2" );
    }

    [TearDown]
    public void Cleanup()
    {
      Directory.Delete( testDirectory, true );
    }


    [Test]
    public void ChecksumTestMD5()
    {
      Checksum task = new Checksum();
      task.BuildEngine = new MockBuild();

      task.Files = TaskUtility.StringArrayToItemArray( inputFiles );
      task.Algorithm = "MD5";
      Assert.IsTrue( task.Execute(), "Execute Failed" );
    }

    [Test]
    public void ChecksumTestSHA1()
    {
      Checksum task = new Checksum();
      task.BuildEngine = new MockBuild();

      task.Files = TaskUtility.StringArrayToItemArray( inputFiles );
      task.Algorithm = "SHA1";
      Assert.IsTrue( task.Execute(), "Execute Failed" );
    }

    [Test]
    public void ChecksumTestUnsupportedAlg()
    {
      Checksum task = new Checksum();
      task.BuildEngine = new MockBuild();

      task.Files = TaskUtility.StringArrayToItemArray( inputFiles );
      task.Algorithm = "foo";
      Assert.IsFalse( task.Execute(), "Execute Failed" );
    }

    [Test]
    public void ChecksumTestNoFiles()
    {
      Checksum task = new Checksum();
      task.BuildEngine = new MockBuild();
      Assert.IsFalse( task.Execute(), "Execute Failed" );
    }
  }
}
