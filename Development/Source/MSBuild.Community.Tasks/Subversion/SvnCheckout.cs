// $Id$

using System;
using System.Collections.Generic;
using System.Text;

namespace MSBuild.Community.Tasks.Subversion
{
    public class SvnCheckout : SvnClient
    {
        public SvnCheckout()
        {
            base.Command = "checkout";
        }

        protected override bool ValidateParameters()
        {
            if (string.IsNullOrEmpty(base.RepositoryPath))
            {
                Log.LogError(MSBuild.Community.Tasks.Properties.Resources.ParameterRequired, "SvnCheckout", "RepositoryPath");
                return false;
            }
            return base.ValidateParameters();
        }
    }
}
