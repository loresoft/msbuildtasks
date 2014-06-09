using MSBuild.Community.Tasks.Tfs;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests.Tfs
{
    [TestFixture]
    public class InfoCommandResponseTest
    {
        [Test]
        public void Can_parse_response()
        {
        
            string response = @"Local information:
                  Local path : c:\dev\file.cs
                  Server path: $/Main/file.cs
                  Changeset  : 5
                  Change     : none
                  Type       : file
                Server information:
                  Server path  : $/Main/file.cs
                  Changeset    : 5
                  Deletion ID  : 0
                  Lock         : none
                  Lock owner   :
                  Last modified: 20 January 2014 11:22:27
                  Type         : file
                  File type    : utf-8
                  Size         : 578
                Local information:
                  Local path : c:\dev\file2.cs
                  Server path: $/Main/file2.cs
                  Changeset  : 5
                  Change     : none
                  Type       : file
                Server information:
                  Server path  : $/Main/file2.cs
                  Changeset    : 5
                  Deletion ID  : 0
                  Lock         : none
                  Lock owner   :
                  Last modified: 20 January 2014 11:22:27
                  Type         : file
                  File type    : utf-8
                  Size         : 988";

            var infoResponse = new InfoCommandResponse(response);

            Assert.That(infoResponse.LocalInformation.Count, Is.EqualTo(2));
        }

    }
}