// $Id$

using System;
using System.Collections.Generic;
using System.Text;

namespace MSBuild.Community.Tasks.Subversion
{
    public class SvnCommit : SvnClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:SvnCommit"/> class.
        /// </summary>
        public SvnCommit()
        {
            base.Command = "commit";
            base.CommandSwitchs &= ~SvnSwitches.RepositoryPath;
            base.CommandSwitchs &= ~SvnSwitches.LocalPath;
            base.CommandSwitchs |= SvnSwitches.Targets;
        }

        /// <summary>
        /// Indicates whether all task paratmeters are valid.
        /// </summary>
        /// <returns>
        /// true if all task parameters are valid; otherwise, false.
        /// </returns>
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
