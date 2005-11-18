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
        
        protected override bool ValidateParameters()
        {
            if (string.IsNullOrEmpty(base.LocalPath))
            {
                Log.LogError(MSBuild.Community.Tasks.Properties.Resources.ParameterRequired, "SvnUpdate", "LocalPath");
                return false;
            }
            return base.ValidateParameters();
        }
    }
}
