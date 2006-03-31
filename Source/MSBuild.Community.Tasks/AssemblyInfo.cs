#region Copyright © 2005 Paul Welter. All rights reserved.
/*
Copyright © 2005 Paul Welter. All rights reserved.

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
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

// $Id$

namespace MSBuild.Community.Tasks
{
    /// <summary>
    /// Generates an AssemblyInfo files
    /// </summary>
    /// <example>
    /// <para>Generates a common version file.</para>
    /// <code><![CDATA[
    /// <AssemblyInfo CodeLanguage="CS"  
    ///     OutputFile="VersionInfo.cs" 
    ///     AssemblyVersion="1.0.0.0" 
    ///     AssemblyFileVersion="1.0.0.0" />
    /// ]]></code>
    /// <para>Generates a complete version file.</para>
    /// <code><![CDATA[
    /// <AssemblyInfo CodeLanguage="CS"  
    ///     OutputFile="$(MSBuildProjectDirectory)\Test\GlobalInfo.cs" 
    ///     AssemblyTitle="AssemblyInfoTask" 
    ///     AssemblyDescription="AssemblyInfo Description"
    ///     AssemblyConfiguration=""
    ///     AssemblyCompany="Company Name, LLC"
    ///     AssemblyProduct="AssemblyInfoTask"
    ///     AssemblyCopyright="Copyright (c) Company Name, LLC 2006"
    ///     AssemblyTrademark=""
    ///     ComVisible="false"
    ///     CLSCompliant="true"
    ///     Guid="d038566a-1937-478a-b5c5-b79c4afb253d"
    ///     AssemblyVersion="1.0.0.0" 
    ///     AssemblyFileVersion="1.0.0.0" />
    /// ]]></code>
    /// </example>
    public class AssemblyInfo : Task
    {
        #region Constants

        /// <summary>
        /// The default value of <see cref="OutputFile"/>.
        /// The value is <c>"AssemblyInfo.cs"</c>.
        /// </summary>
        public const string DEFAULT_OUTPUT_FILE = @"AssemblyInfo.cs";

        #endregion Constants

        #region Fields
        private Dictionary<string, string> _attributes;
        private string[] _Imports;

        #endregion Fields

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AssemblyInfo"/> class.
        /// </summary>
        public AssemblyInfo()
        {
            _attributes = new Dictionary<string, string>();
            _Imports = new string[] { "System", "System.Reflection", "System.Runtime.CompilerServices", "System.Runtime.InteropServices" };
            _OutputFile = DEFAULT_OUTPUT_FILE;
        }

        #endregion Constructor

        #region Input Parameters
        private string _CodeLanguage;

        /// <summary>
        /// Gets or sets the code language.
        /// </summary>
        /// <value>The code language.</value>
        [Required]
        public string CodeLanguage
        {
            get { return _CodeLanguage; }
            set { _CodeLanguage = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [COMVisible].
        /// </summary>
        /// <value><c>true</c> if [COMVisible]; otherwise, <c>false</c>.</value>
        public bool ComVisible
        {
            get { return ReadBooleanAttribute("ComVisible"); }
            set { _attributes["ComVisible"] = value.ToString(); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [CLSCompliant].
        /// </summary>
        /// <value><c>true</c> if [CLSCompliant]; otherwise, <c>false</c>.</value>
        public bool CLSCompliant
        {
            get { return ReadBooleanAttribute("CLSCompliant"); }
            set { _attributes["CLSCompliant"] = value.ToString(); }
        }

        /// <summary>
        /// Gets or sets the GUID.
        /// </summary>
        /// <value>The GUID.</value>
        public string Guid
        {
            get { return ReadAttribute("Guid"); }
            set { _attributes["Guid"] = value; }
        }

        /// <summary>
        /// Gets or sets the assembly title.
        /// </summary>
        /// <value>The assembly title.</value>
        public string AssemblyTitle
        {
            get { return ReadAttribute("AssemblyTitle"); }
            set { _attributes["AssemblyTitle"] = value; }
        }

        /// <summary>
        /// Gets or sets the assembly description.
        /// </summary>
        /// <value>The assembly description.</value>
        public string AssemblyDescription
        {
            get { return ReadAttribute("AssemblyDescription"); }
            set { _attributes["AssemblyDescription"] = value; }
        }

        /// <summary>
        /// Gets or sets the assembly configuration.
        /// </summary>
        /// <value>The assembly configuration.</value>
        public string AssemblyConfiguration
        {
            get { return ReadAttribute("AssemblyConfiguration"); }
            set { _attributes["AssemblyConfiguration"] = value; }
        }

        /// <summary>
        /// Gets or sets the assembly company.
        /// </summary>
        /// <value>The assembly company.</value>
        public string AssemblyCompany
        {
            get { return ReadAttribute("AssemblyCompany"); }
            set { _attributes["AssemblyCompany"] = value; }
        }

        /// <summary>
        /// Gets or sets the assembly product.
        /// </summary>
        /// <value>The assembly product.</value>
        public string AssemblyProduct
        {
            get { return ReadAttribute("AssemblyProduct"); }
            set { _attributes["AssemblyProduct"] = value; }
        }

        /// <summary>
        /// Gets or sets the assembly copyright.
        /// </summary>
        /// <value>The assembly copyright.</value>
        public string AssemblyCopyright
        {
            get { return ReadAttribute("AssemblyCopyright"); }
            set { _attributes["AssemblyCopyright"] = value; }
        }

        /// <summary>
        /// Gets or sets the assembly trademark.
        /// </summary>
        /// <value>The assembly trademark.</value>
        public string AssemblyTrademark
        {
            get { return ReadAttribute("AssemblyTrademark"); }
            set { _attributes["AssemblyTrademark"] = value; }
        }

        /// <summary>
        /// Gets or sets the assembly culture.
        /// </summary>
        /// <value>The assembly culture.</value>
        public string AssemblyCulture
        {
            get { return ReadAttribute("AssemblyCulture"); }
            set { _attributes["AssemblyCulture"] = value; }
        }

        /// <summary>
        /// Gets or sets the assembly version.
        /// </summary>
        /// <value>The assembly version.</value>
        public string AssemblyVersion
        {
            get { return ReadAttribute("AssemblyVersion"); }
            set { _attributes["AssemblyVersion"] = value; }
        }

        /// <summary>
        /// Gets or sets the assembly file version.
        /// </summary>
        /// <value>The assembly file version.</value>
        public string AssemblyFileVersion
        {
            get { return ReadAttribute("AssemblyFileVersion"); }
            set { _attributes["AssemblyFileVersion"] = value; }
        }

        /// <summary>
        /// Gets or sets the assembly key file.
        /// </summary>
        public string AssemblyKeyFile
        {
            get { return ReadAttribute("AssemblyKeyFile"); }
            set { _attributes["AssemblyKeyFile"] = value; }
        }

        /// <summary>
        /// Gets or sets the assembly key name.
        /// </summary>
        public string AssemblyKeyName
        {
            get { return ReadAttribute("AssemblyKeyName"); }
            set { _attributes["AssemblyKeyName"] = value; }
        }

        /// <summary>
        /// Gets or sets the assembly delay sign value.
        /// </summary>
        public string AssemblyDelaySign
        {
            get { return ReadAttribute("AssemblyDelaySign"); }
            set { _attributes["AssemblyDelaySign"] = value; }
        }

        #endregion Input Parameters

        #region Input/Output Parameters
        private string _OutputFile;

        /// <summary>
        /// Gets or sets the output file.
        /// </summary>
        /// <value>The output file.</value>
        [Output]
        public string OutputFile
        {
            get { return _OutputFile; }
            set { _OutputFile = value; }
        }

        #endregion Input/Output Parameters

        #region Task Overrides
        /// <summary>
        /// When overridden in a derived class, executes the task.
        /// </summary>
        /// <returns>
        /// true if the task successfully executed; otherwise, false.
        /// </returns>
        public override bool Execute()
        {
            if (_attributes.Count == 0)
            {
                Log.LogError("No assembly parameter were set for file \"{0}\".", _OutputFile);
                return false;
            }

            using (StreamWriter writer = File.CreateText(_OutputFile))
            {
                GenerateFile(writer);
                writer.Flush();
                writer.Close();
                Log.LogMessage("Created AssemblyInfo file \"{0}\".", _OutputFile);
            }
            return true;
        }

        #endregion Task Overrides

        #region Private Methods
        private void GenerateFile(TextWriter writer)
        {
            CodeDomProvider provider = null;

            if (string.Compare(_CodeLanguage, "VB", true) == 0)
            {
                provider = new Microsoft.VisualBasic.VBCodeProvider();
                _OutputFile = Path.ChangeExtension(_OutputFile, ".vb");
            }
            else
            {
                provider = new Microsoft.CSharp.CSharpCodeProvider();
                _OutputFile = Path.ChangeExtension(_OutputFile, ".cs");
            }

            if (provider == null)
                throw new NotSupportedException("The specified code language is not supported.");

            CodeCompileUnit codeCompileUnit = new CodeCompileUnit();
            CodeNamespace codeNamespace = new CodeNamespace();

            foreach (string import in _Imports)
            {
                codeNamespace.Imports.Add(new CodeNamespaceImport(import));
            }

            codeCompileUnit.Namespaces.Add(codeNamespace);

            foreach (KeyValuePair<string, string> assemblyAttribute in _attributes)
            {
                // create new assembly-level attribute
                CodeAttributeDeclaration codeAttributeDeclaration = new CodeAttributeDeclaration(assemblyAttribute.Key);

                bool typedValue;

                if ((assemblyAttribute.Key == "CLSCompliant" || assemblyAttribute.Key == "ComVisible")
                    && bool.TryParse(assemblyAttribute.Value, out typedValue))
                {
                    codeAttributeDeclaration.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(typedValue)));
                }
                else
                {
                    codeAttributeDeclaration.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(assemblyAttribute.Value)));
                }

                // add assembly-level argument to code compile unit
                codeCompileUnit.AssemblyCustomAttributes.Add(codeAttributeDeclaration);
            }
            provider.GenerateCodeFromCompileUnit(codeCompileUnit, writer, new CodeGeneratorOptions());
        }

        private string ReadAttribute(string key)
        {
            string value;
            _attributes.TryGetValue(key, out value);
            return value;
        }

        private bool ReadBooleanAttribute(string key)
        {
            string value;
            bool result;

            if (!_attributes.TryGetValue(key, out value))
                return false;
            if (!bool.TryParse(value, out result))
                return false;

            return result;
        }

        #endregion Private Methods

    }
}
