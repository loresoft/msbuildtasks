// $Id$

using System;
using System.Collections.Generic;
using System.Text;

namespace MSBuild.Community.Tasks.Subversion
{
    /// <summary>
    /// Export a folder from a Subversion repository
    /// </summary>
    /// <example> Export from repository
    /// <code><![CDATA[
    /// <Target Name="Export">
    ///   <MakeDir Directories="$(MSBuildProjectDirectory)\Test" />
    ///   <RemoveDir Directories="$(MSBuildProjectDirectory)\Test\Export" />
    ///   <SvnExport RepositoryPath="file:///d:/svn/repo/Test/trunk" 
    ///     LocalPath="$(MSBuildProjectDirectory)\Test\Export">
    ///     <Output TaskParameter="Revision" PropertyName="Revision" />
    ///   </SvnExport>
    ///   <Message Text="Revision: $(Revision)"/>
    /// </Target>
    /// ]]></code>
    /// </example>
    public class SvnExport : SvnClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:SvnExport"/> class.
        /// </summary>
        public SvnExport()
        {
            base.Command = "export";
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
                Log.LogError(MSBuild.Community.Tasks.Properties.Resources.ParameterRequired, "SvnCheckout", "RepositoryPath");
                return false;
            }
            return base.ValidateParameters();
        }

    }
}
