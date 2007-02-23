#region Copyright © 2007 Geoff Lane. All rights reserved.
/*
Copyright © 2007 Geoff Lane <geoff@zorched.net>. All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions
are met:

1. Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright
   notice, this list of conditions and the following disclaimer in the
   documentation and/or other materials provided with the distribution.
3. The name of the author may not be used to endorse or promote products
   derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE AUTHOR "AS IS" AND ANY EXPRESS OR
IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 
*/
#endregion

using System;

// $Id$

namespace MSBuild.Community.Tasks.Subversion
{
    /// <summary>
    /// Create a copy a remote path.
    /// This can be used for branching and tagging.
    /// </summary>
    /// <example>Create a tag of the trunk with the current revision number
    /// <code><![CDATA[
    /// <Target Name="Copy">
    ///   <SvnInfo RepositoryPath="file:///d:/svn/repo/Test/trunk">
	///		<Output TaskParameter="LastChangedRevision" PropertyName="RemoteSvnRevisionNumber"  />
	///	  </SvnInfo>
    ///   <SvnCopy RepositoryPath="file:///d:/svn/repo/Test/trunk"
    ///            DestinationPath="file:///d:/svn/repo/Test/tags/REV-$(RemoteSvnRevisionNumber)">      
    ///   </SvnCopy>
    ///   <Message Text="Revision: $(RemoteSvnRevisionNumber)"/>
    /// </Target>
    /// ]]></code>
    /// </example>
    public class SvnCopy : SvnClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:MSBuild.Community.Tasks.Subversion.SvnCopy"/> class.
        /// </summary>
        public SvnCopy()
        {
            base.Command = "copy";
            base.CommandSwitches = SvnSwitches.NonInteractive | SvnSwitches.NoAuthCache;
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
                Log.LogError(Properties.Resources.ParameterRequired, "SvnCopy", "RepositoryPath");
                return false;
            }

            if (String.IsNullOrEmpty(base.DestinationPath))
            {
                Log.LogError(Properties.Resources.ParameterRequired, "SvnCopy", "DestinationPath");
                return false;
            }
            return base.ValidateParameters();
        }
    }
}
