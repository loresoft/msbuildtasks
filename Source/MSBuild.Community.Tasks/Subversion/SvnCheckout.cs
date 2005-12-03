// $Id$

using System;
using System.Collections.Generic;
using System.Text;

namespace MSBuild.Community.Tasks.Subversion
{
    /// <summary>
    /// Checkout a local working copy of a Subversion repository.
    /// </summary>
    /// <example>Checkout a working copy
    /// <code><![CDATA[
    /// <Target Name="Checkout">
    ///   <RemoveDir Directories="$(MSBuildProjectDirectory)\Test\Checkout" />
    ///   <SvnCheckout RepositoryPath="file:///d:/svn/repo/Test/trunk" 
    ///                LocalPath="$(MSBuildProjectDirectory)\Test\Checkout">      
    ///     <Output TaskParameter="Revision" PropertyName="Revision" />
    ///   </SvnCheckout>
    ///   <Message Text="Revision: $(Revision)"/>
    /// </Target>
    /// ]]></code>
    /// </example>
    public class SvnCheckout : SvnClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:SvnCheckout"/> class.
        /// </summary>
        public SvnCheckout()
        {
            base.Command = "checkout";
        }

        /// <summary>
        /// Indicates whether all task paratmeters are valid.
        /// </summary>
        /// <returns>
        /// true if all task parameters are valid; otherwise, false.
        /// </returns>
        protected override bool ValidateParameters()
        {
            if (string.IsNullOrEmpty(base.RepositoryPath))
            {
                Log.LogError(Properties.Resources.ParameterRequired, "SvnCheckout", "RepositoryPath");
                return false;
            }
            return base.ValidateParameters();
        }
    }
}
