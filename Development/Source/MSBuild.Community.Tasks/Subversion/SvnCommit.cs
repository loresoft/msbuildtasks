using System;
using System.Collections.Generic;
using System.Text;

namespace MSBuild.Community.Tasks.Subversion
{
    public class SvnCommit : SvnClient
    {
        public SvnCommit()
        {
            base.Command = "commit";
            base.CommandSwitchs &= ~SvnSwitches.RepositoryPath;
        }
    }
}
