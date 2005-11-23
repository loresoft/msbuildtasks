// $Id$

using System;
using System.Collections.Generic;
using System.Text;

namespace MSBuild.Community.Tasks.Subversion
{
    public class SvnUpdate : SvnClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:SvnUpdate"/> class.
        /// </summary>
        public SvnUpdate()
        {
            base.Command = "update";
            base.CommandSwitchs &= ~SvnSwitches.RepositoryPath;
        }

        /// <summary>
        /// Indicates whether all task paratmeters are valid.
        /// </summary>
        /// <returns>
        /// true if all task parameters are valid; otherwise, false.
        /// </returns>
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
