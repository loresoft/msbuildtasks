// $Id$

using System;
using System.Collections.Generic;
using System.Text;

namespace MSBuild.Community.Tasks.Subversion
{
    public class SvnUpdate : SvnClient
    {
        public SvnUpdate()
        {
            base.Command = "update";
            base.CommandSwitchs &= ~SvnSwitches.RepositoryPath;
        }
    }
}
