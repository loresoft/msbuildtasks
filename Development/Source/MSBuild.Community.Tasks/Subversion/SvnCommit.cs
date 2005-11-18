// $Id$

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
            base.CommandSwitchs &= ~SvnSwitches.LocalPath;
            base.CommandSwitchs |= SvnSwitches.Targets;
        }
        
        protected override bool ValidateParameters()
        {
            if (base.Targets == null || base.Targets.Length == 0)
            {
                Log.LogError(MSBuild.Community.Tasks.Properties.Resources.ParameterRequired, "SvnCommit", "Targets");
                return false;
            }
            return base.ValidateParameters();
        }
    }
}
